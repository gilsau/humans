using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Humans.Web.Controllers
{
    public class PaymentsController : Controller
    {
        [HttpGet]
        public ActionResult Received()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RequestPayment()
        {
            return View();
        }

        [HttpGet]
        public ActionResult MakePayment()
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
