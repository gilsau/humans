using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Humans.Web;
using WebMatrix.WebData;
using System.Web.Security;

namespace Humans.Web.Models
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class SessionCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            UserProfile acctCurrent = (UserProfile)session["User"];
            if (acctCurrent == null)
            {
                FormsAuthentication.SignOut();
                filterContext.HttpContext.Session.Abandon();

                // clear authentication cookie
                HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                cookie1.Expires = DateTime.Now.AddYears(-1);
                filterContext.HttpContext.Response.Cookies.Add(cookie1);

                // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
                HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
                cookie2.Expires = DateTime.Now.AddYears(-1);
                filterContext.HttpContext.Response.Cookies.Add(cookie2);
                
                //Redirect
                var url = new UrlHelper(filterContext.RequestContext);
                var loginUrl = url.Content("~/account/login");
                filterContext.HttpContext.Response.Redirect(loginUrl, true);
            }

            //Reload Session user, to get any new updates
            else
            {
                HumansEntities db = new HumansEntities();
                HttpContext.Current.Session["User"] = db.UserProfiles.SingleOrDefault(up => up.UserId == acctCurrent.UserId);
            }
        }
    }

}