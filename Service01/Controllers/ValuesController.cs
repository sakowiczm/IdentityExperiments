using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Service01.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [HttpGet]
        [Route("identity")]
        [Authorize(Roles = "admin, user-contact")] // not advised method - taken from User claim 'role'
        public ActionResult Identity()
        {
            return new JsonResult(User.Claims.Select(c => new { c.Type, c.Value }));
        }

        [HttpGet]
        [Route("getdatetime")]
        [Authorize(Policy = "get-datetime")] // remove this scope on consent screen to prevent access
        public ActionResult GetDateTime()
        {
            return new JsonResult(DateTime.Now.ToString());
        }

        [HttpPost]
        [Route("save")]
        [Authorize(Roles = "admin")]
        public ActionResult Save(string value)
        {
            // todo: save to sql server db (inside docker container?)

            //return new JsonResult(User.Claims.Select(c => new { c.Type, c.Value }));

            return null;
        }
    }
}
