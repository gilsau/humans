using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Humans.Web.Controllers
{
    [Authorize]
    public class MediaController : Controller
    {
        //
        // GET: /Media/

        public ActionResult Photos()
        {
            return View();
        }

        public ActionResult Videos()
        {
            return View();
        }

    }
}
