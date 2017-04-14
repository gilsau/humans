using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Humans.Web.Controllers
{
    [Authorize]
    public class ContactsController : Controller
    {
        //
        // GET: /Contacts/

        public ActionResult Manage()
        {
            return View();
        }

        public ActionResult Requests()
        {
            return View();
        }

    }
}
