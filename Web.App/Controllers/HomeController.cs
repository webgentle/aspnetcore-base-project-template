using System;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.App.Managers;
using Web.App.Models.Common;
using Web.App.Models.Lead;

namespace Web.App.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() => View());
        }

        [Route("about-us")]
        public async Task<IActionResult> About()
        {
            return await Task.Run(() => View());
        }
    }
}