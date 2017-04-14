using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Objects;
using WebMatrix.WebData;
using Humans.Web.Models;
using Humans.Web.Helpers;
using System.Data.SqlClient;
using System.Configuration;

namespace Humans.Web.Controllers
{
    public class SportsController : Controller
    {
        [HttpPost]
        public JsonResult Add(string sportIds)
        {
            string retMsg = string.Empty;
            
            HumansEntities db = new HumansEntities();
            UserProfile acctFromDb = db.UserProfiles.SingleOrDefault(u => u.UserName.Equals(User.Identity.Name));

            try
            {
                //Remove all sports from user
                List<int> userSportIds = acctFromDb.UserSports.Select(us => us.Id).ToList();
                foreach (int usid in userSportIds)
                {
                    db.UserSports.Remove(db.UserSports.SingleOrDefault(us => us.Id == usid));
                }

                //Add sports selected
                string[] ids = sportIds.Split(',');
                foreach (string id in ids)
                {
                    int sportid = int.Parse(id);
                    Sport sportToAdd = db.Sports.SingleOrDefault(s=>s.Id==sportid);
                    acctFromDb.UserSports.Add(new UserSport() { SportId = sportid, Sport=sportToAdd, UserId = acctFromDb.UserId, SkillLevel=1, Findable=true });

                    retMsg += string.Format("<div><a href=\"../sports/page?sportid={0}\">{1}</a></div>", sportToAdd.Id, sportToAdd.Longname);
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                retMsg = ex.Message;
            }

            //Get updated userprofile
            Session["User"] = db.UserProfiles.SingleOrDefault(u => u.UserName.Equals(User.Identity.Name));

            return Json(retMsg);
        }

        [HttpPost]
        public JsonResult You(int sportid, bool findable, string note, int skilllevel)
        {
            Result result = new Result();
            UserProfile acctCurrent = (UserProfile)Session["User"];
            HumansEntities db = new HumansEntities();

            try
            {
                UserSport usersport = db.UserSports.FirstOrDefault(us => us.UserId == acctCurrent.UserId && us.SportId == sportid);

                if (usersport != null)
                {
                    usersport.Findable = findable;
                    usersport.Note = note;
                    usersport.SkillLevel = skilllevel;
                    db.SaveChanges();

                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.MessageForUser = ex.Message;
                Logger.LogError(string.Format("ACTION:{0}\r\n---------------------------------------\r\nMESSAGE: {1}\r\n---------------------------------------\r\nINNER EXCEPTION {2}\r\n---------------------------------------\r\nSTACK TRACE: {3}\r\n---------------------------------------\r\nSOURCE: {4}", "Saving YOU section on humans page.", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : string.Empty), ex.StackTrace, ex.Source));
            }

            return Json(result);
        }

        [HttpGet]
        [Authorize]
        [SessionCheckAttribute]
        public ActionResult Manage()
        {
            HumansEntities db = new HumansEntities();
            ViewBag.SportCats = db.SportCategories.ToList();
            ViewBag.Sports = db.Sports.ToList();

            return View();
        }

        [HttpGet]
        [Authorize]
        [SessionCheckAttribute]
        public ActionResult Choose(string f)
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        [SessionCheckAttribute]
        public ActionResult Page()
        {
            return View();
        }
    }
}
