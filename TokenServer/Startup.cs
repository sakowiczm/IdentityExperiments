using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;
using IdentityModel;
using System.Text;
using IdentityServer4;

namespace TokenServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddInMemoryClients(Clients.Get())
                .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                .AddInMemoryApiResources(Resources.GetApiResources())
                .AddTestUsers(Users.Get())
                .AddTemporarySigningCredential();

            services.AddMvc();
        }
/*
        private string GetHeaderValue()
        {
            // taken from Resource.GetApiResources().Name & ApiSecret
            var credentials = string.Format("{0}:{1}", "customAPI", "scopeSecret");
            var headerValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            return headerValue;
        }
*/
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }

    internal class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "mvc-client",
                    ClientName = "MVC Client",
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256()),
                    },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowAccessTokensViaBrowser = false,
                    RedirectUris = { "http://localhost:5001/signin-oidc" },
                    LogoutUri = "http://localhost:5001/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "role",
                        //"name",
                        "service1-api.full-access",
                        "service1-api.read-only",
                        "service1-api.get-datetime"
                    },

                }
            };
        }
    }

    internal class Resources
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "role",
                    DisplayName = "Role",
                    UserClaims = new List<string> { "role" }
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "service1-api", // username in Authorization Basic
                    DisplayName = "Service 1 API",
                    Description = "Service 1 API - test service",
                    UserClaims = new List<string>
                    {
                        "role",
                        IdentityServerConstants.StandardScopes.Profile, // not working with name?
                        IdentityServerConstants.StandardScopes.Email,
                        "name"
                    },

                    ApiSecrets = new List<Secret> { new Secret("secret".Sha256())}, // password in Authrizaiton Basic
                    Scopes = new List<Scope>
                    {
                        new Scope("service1-api.full-access", "Service 01 - Full Access"),
                        new Scope("service1-api.read-only", "Service 01 - ReadOnly Access"),
                        new Scope("service1-api.get-datetime", "Service 01 - DateTime Access")
                    }
                }
            };
        }
    }

    internal class Users
    {
        public static List<TestUser> Get()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                    Username = "user",
                    Password = "password",
                    Claims = new List<Claim> {
                        new Claim(JwtClaimTypes.Email, "user@securityexperiments.com"),
                        new Claim(JwtClaimTypes.Role, "admin"),
                        new Claim(JwtClaimTypes.Role, "user-contact"),
                        new Claim(JwtClaimTypes.Name, "John Doe"),
                    }
                }
            };
        }
    }

    
}

