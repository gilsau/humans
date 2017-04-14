using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Humans.Web.Controllers
{
    public class RequestsController : Controller
    {
        [HttpGet]
        public ActionResult New()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Pending()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Completed()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Archived()
        {
            return View();
        }

    }
}
