using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Humans.Web.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult TermsOfUse()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }
    }
}
