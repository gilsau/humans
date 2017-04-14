using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using Humans.Web.Models;

namespace Humans.Web.Controllers
{
    public class HumansController : Controller
    {
        [HttpGet]
        [Authorize]
        [SessionCheckAttribute]
        public ActionResult Manage(int sportid)
        {
            UserProfile acctCurrent = (UserProfile)Session["User"];

            HumansEntities db = new HumansEntities();

            UserSport usersport = acctCurrent.UserSports.FirstOrDefault(us => us.SportId == sportid);

            return View(usersport);
        }
        
        [HttpPost]
        public JsonResult UnRemember(int userid)
        {
            int currUserId = ((UserProfile)Session["User"]).UserId;

            HumansEntities db = new HumansEntities();

            UserUser rem = db.UserUsers.FirstOrDefault(r => r.UserId == currUserId && r.RememberedId == userid);
            if (rem != null)
            {
                db.UserUsers.Remove(rem);
                db.SaveChanges();
            }

            Session["User"] = db.UserProfiles.SingleOrDefault(a => a.UserId == currUserId);

            return Json("");
        }

        [HttpPost]
        public JsonResult Remember(int userid)
        {
            int currUserId = ((UserProfile)Session["User"]).UserId;

            HumansEntities db = new HumansEntities();

            if (!db.UserUsers.Any(r => r.UserId == currUserId && r.RememberedId == userid))
            {
                db.UserUsers.Add(new UserUser() { UserId = currUserId, RememberedId = userid });
                db.SaveChanges();
            }

            Session["User"] = db.UserProfiles.SingleOrDefault(a => a.UserId == currUserId);

            return Json("");
        }

        [HttpPost]
        public JsonResult You(int id, bool findable, string note, int skilllevel)
        {
            string retMsg = string.Empty;

            HumansEntities db = new HumansEntities();
            UserProfile acctFromDb = db.UserProfiles.SingleOrDefault(u => u.UserName.Equals(User.Identity.Name));

            try
            {
                UserSport usersport = acctFromDb.UserSports.SingleOrDefault(us => us.Id == id);
                usersport.Findable = findable;
                usersport.Note = note;
                usersport.SkillLevel = skilllevel;

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
        public JsonResult GetRemembered(int sportid)
        {
            return Search(2, sportid);
        }

        [HttpPost]
        public JsonResult Find(int sportid, int distance, int agemin, int agemax, string gender, int skill)
        {
            return Search(1, sportid, distance, agemin, agemax, gender, skill);
        }

        private JsonResult Search(int searchType, int sportid, int distance = 0, int agemin = 0, int agemax = 0, string gender = "", int skill = 0)
        {
            DateTime today = DateTime.Today;
            UserProfile acctCurrent = ((UserProfile)Session["User"]);
            double acctLat = double.Parse((acctCurrent.Lat == null ? "0" : acctCurrent.Lat.ToString()));
            double acctLng = double.Parse((acctCurrent.Lng == null ? "0" : acctCurrent.Lng.ToString()));

            //Use ADO to get results, the Haversine formula could not be implemented easily for Linq to Entities
            SqlConnection oConn = new SqlConnection();
            oConn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            oConn.Open();

            string strComm = string.Empty;

            //Find
            if (searchType == 1)
            {
                strComm = string.Format("select " +
                                            "us.UserId, " +
                                            "UserSportId=us.Id, " +
                                            "up.ProfilePic, " +
                                            "up.FirstName, " +
                                            "up.Lat, " +
                                            "up.Lng, " +
                                            "us.Note, " +
                                            "Age = DateDiff(d, up.Birthdate, getdate())/365, Distance=dbo.DistanceBetween({0}, {1}, " +
                                            "up.Lat, " +
                                            "up.Lng), " +
                                            "Remembered=isnull(uu.id, 0) " +
                                        "from usersports us " +
                                        "inner join userprofile up on up.UserId = us.UserId " +
                                        "left join UserUsers uu on uu.RememberedId = up.UserId " +
                                        "where us.UserId <> {2} " +
                                            "and us.SportId = {3} " +
                                            "and us.Findable = 1 " +
                                            "and us.SkillLevel = {4} " +
                                            "and up.Gender = '{5}' " +
                                            "and (DateDiff(d, up.Birthdate, getdate())/365) between {6} and {7} " +
                                            "and dbo.DistanceBetween({0}, {1}, up.Lat, up.Lng) < {8} ",
                                        acctLat, acctLng, acctCurrent.UserId, sportid, skill, gender, agemin, agemax, distance);
            }

            //Get Remembered
            else if (searchType == 2)
            {
                strComm = string.Format("select us.UserId, UserSportId=us.Id, up.ProfilePic, up.FirstName, up.Lat, up.Lng, us.Note, Age = DateDiff(d, up.Birthdate, getdate())/365, Distance=dbo.DistanceBetween({0}, {1}, up.Lat, up.Lng) " +
                                            "from UserUsers ru " +
                                            "left join UserProfile up on up.UserId = ru.RememberedId " +
                                            "left join UserSports us on us.UserId = ru.RememberedId " +
                                            "where  " +
                                            "ru.UserId = {2} and " +
                                            "us.SportId = {3}",
                                            acctLat, acctLng, acctCurrent.UserId, sportid);
            }

            SqlCommand oComm = new SqlCommand();
            oComm.Connection = oConn;
            oComm.CommandText = strComm;

            List<Peep> peeps = new List<Peep>();

            SqlDataReader oReader = oComm.ExecuteReader();
            while (oReader.Read())
            {
                Peep peep = new Peep();
                peep.UserId = (int)oReader["UserId"];
                peep.UserSportId = (int)oReader["UserSportId"];
                peep.FirstName = oReader["FirstName"].ToString();
                peep.ProfilePic = oReader["ProfilePic"].ToString();
                peep.Age = (int)oReader["Age"];
                peep.Note = oReader["Note"].ToString();
                peep.Distance = (int)Math.Round((float)oReader["Distance"], 0);
                peep.Lat = (double)oReader["Lat"];
                peep.Lng = (double)oReader["Lng"];

                if (searchType == 1)
                {
                    peep.Remembered = int.Parse(oReader["Remembered"].ToString()) > 0 ? true : false;
                }

                peeps.Add(peep);
            }
            oConn.Close();
            oConn.Dispose();
            oComm.Dispose();
            oReader.Close();
            oReader.Dispose();

            Peep currPeep = new Peep() { FirstName = acctCurrent.FirstName, ProfilePic = acctCurrent.ProfilePic, Distance = 0, Note = "", Age = DateTime.Today.Subtract(acctCurrent.Birthdate).Days / 365, Lat = acctLat, Lng = acctLng };

            return Json(new { CurrPeep = currPeep, Peeps = peeps });
        }
    }
}
