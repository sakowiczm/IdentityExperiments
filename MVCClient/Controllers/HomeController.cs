using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading;
using System.Security.Claims;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace MVCClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [Authorize]
        public IActionResult Secured()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [Authorize]
        public async Task<IActionResult> GetIdentity()
        {
            var token = await HttpContext.Authentication.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.SetBearerToken(token);

            var response = await client.GetAsync("http://localhost:5002/api/values/identity");
            bool unauthorized = response.StatusCode == System.Net.HttpStatusCode.Forbidden || response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
            if(unauthorized)
            {
                ViewBag.Json = "Access forbidden";
            }
            else
            {
                var result = response.Content.ReadAsStringAsync().Result;
                ViewBag.Json = JArray.Parse(result).ToString();
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> GetDateTime()
        {
            var token = await HttpContext.Authentication.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.SetBearerToken(token);

            var response = await client.GetAsync("http://localhost:5002/api/values/getdatetime");
            bool unauthorized = response.StatusCode == System.Net.HttpStatusCode.Forbidden || response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
            ViewBag.Json = unauthorized ? "Access forbidden" : response.Content.ReadAsStringAsync().Result;

            return View("GetIdentity");
        }

        public async Task<IActionResult> RenewTokens()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError) throw new Exception(disco.Error);

            var tokenClient = new TokenClient(disco.TokenEndpoint, "mvc-client", "secret");
            var rt = await HttpContext.Authentication.GetTokenAsync("refresh_token");
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(rt);

            if (!tokenResult.IsError)
            {
                var old_id_token = await HttpContext.Authentication.GetTokenAsync("id_token");
                var new_access_token = tokenResult.AccessToken;
                var new_refresh_token = tokenResult.RefreshToken;

                var tokens = new List<AuthenticationToken>();
                tokens.Add(new AuthenticationToken { Name = OpenIdConnectParameterNames.IdToken, Value = old_id_token });
                tokens.Add(new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = new_access_token });
                tokens.Add(new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = new_refresh_token });

                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
                tokens.Add(new AuthenticationToken { Name = "expires_at", Value = expiresAt.ToString("o", CultureInfo.InvariantCulture) });

                var info = await HttpContext.Authentication.GetAuthenticateInfoAsync("Cookies");
                info.Properties.StoreTokens(tokens);
                await HttpContext.Authentication.SignInAsync("Cookies", info.Principal, info.Properties);

                return Redirect("~/Home/Secured");
            }

            ViewData["Error"] = tokenResult.Error;
            return View("Error");
        }

        public IActionResult Logout()
        {
            return new SignOutResult(new[] { "Cookies", "oidc" });
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
