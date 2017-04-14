using System;
using System.Data.Objects;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Humans.Web.Models;
using Humans.Web.Helpers;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Configuration;

namespace Humans.Web.Controllers
{
    public class MessagesController : Controller
    {
        [HttpGet]
        public JsonResult GetNotifications()
        {
            Result result = new Result();
            Result resultV = new Result();
            Result resultH = new Result();
            Result resultC = new Result();

            UserProfile acctCurrent = (UserProfile)Session["User"];
            HumansEntities db = new HumansEntities();

            //Get new venues near you
            try
            {
                int distance = 100;
                DateTime today = DateTime.Today;
                double acctLat = double.Parse((acctCurrent.Lat == null ? "0" : acctCurrent.Lat.ToString()));
                double acctLng = double.Parse((acctCurrent.Lng == null ? "0" : acctCurrent.Lng.ToString()));

                //Use ADO to get results, the Haversine formula could not be implemented easily for Linq to Entities
                SqlConnection oConn = new SqlConnection();
                oConn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                oConn.Open();

                string strComm = string.Empty;
                strComm = string.Format("select	v.Id, v.Name " +
                    "from venue v " +
                    "inner join VenueSports vs on vs.VenueId = v.Id  " +
                    "inner join Sport s on s.Id = vs.SportId " +
                    "where vs.SportId in (select sportid from usersports where userid = {3})  " +
	                    "and v.Id not in (select ReferenceId from UserNotifications where UserId = {3} and ReferenceId = v.Id and NotificationTypeId = {4}) " +
	                    "and dbo.DistanceBetween({0}, {1}, v.Lat, v.Lng) < {2}  " +
                    "group by v.Id " +
                    "order by v.Id ",
                    acctLat, acctLng, distance, acctCurrent.UserId, (int)Notification.VenueNear);

                SqlCommand oComm = new SqlCommand();
                oComm.Connection = oConn;
                oComm.CommandText = strComm;

                int venueCount = 0;
                SqlDataReader oReader = oComm.ExecuteReader();
                while (oReader.Read())
                {
                    venueCount++;

                    UserNotification un = new UserNotification();
                    un.ReferenceId = int.Parse(oReader["Id"].ToString());
                    un.StatusId = (int)StatusType.New;
                    un.NotificationTypeId = (int)Notification.VenueNear;
                    un.UserId = acctCurrent.UserId;

                    db.UserNotifications.Add(un);
                }
                db.SaveChanges();
                
                oConn.Close();
                oConn.Dispose();
                oComm.Dispose();
                oReader.Close();
                oReader.Dispose();

                var venues = db.Venues
                    .Where(v => db.UserNotifications
                                    .Where(un => un.UserId == acctCurrent.UserId && un.ReferenceId == v.Id && un.NotificationTypeId == (int)Notification.VenueNear)
                                    .
                    

                resultV.DynObject = Json(venueCount);
                resultV.Success = true;
            }
            catch (Exception ex)
            {
                resultV.MessageForUser = ex.Message;
                Logger.LogError(string.Format("ACTION:{0}\r\n---------------------------------------\r\nMESSAGE: {1}\r\n---------------------------------------\r\nINNER EXCEPTION {2}\r\n---------------------------------------\r\nSTACK TRACE: {3}\r\n---------------------------------------\r\nSOURCE: {4}", "Getting All Conversations", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : string.Empty), ex.StackTrace, ex.Source));
            }

            //Get new humans near you
            try
            {
                int distance = 100;
                DateTime today = DateTime.Today;
                double acctLat = double.Parse((acctCurrent.Lat == null ? "0" : acctCurrent.Lat.ToString()));
                double acctLng = double.Parse((acctCurrent.Lng == null ? "0" : acctCurrent.Lng.ToString()));

                //Use ADO to get results, the Haversine formula could not be implemented easily for Linq to Entities
                SqlConnection oConn = new SqlConnection();
                oConn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                oConn.Open();

                string strComm = string.Empty;
                strComm = string.Format("select	up.UserId " +
                    "from UserProfile up " +
                    "inner join UserSports us on us.UserId = up.UserId " +
                    "inner join Sport s on s.Id = us.SportId " +
                    "where us.SportId in (select sportid from usersports where userid = {3}) " +
                        "and up.UserId not in (select ReferenceId from UserNotifications where UserId = {3} and ReferenceId = up.UserId and NotificationTypeId = {4}) " +
                        "and dbo.DistanceBetween({0}, {1}, up.Lat, up.Lng) < {2}  " +
                        "and up.UserId <> {3} " +
                    "group by up.UserId " +
                    "order by up.UserId",
                    acctLat, acctLng, distance, acctCurrent.UserId, (int)Notification.HumanNear);

                SqlCommand oComm = new SqlCommand();
                oComm.Connection = oConn;
                oComm.CommandText = strComm;

                int humanCount = 0;
                SqlDataReader oReader = oComm.ExecuteReader();
                while (oReader.Read())
                {
                    humanCount++;

                    UserNotification un = new UserNotification();
                    un.ReferenceId = int.Parse(oReader["UserId"].ToString());
                    un.StatusId = (int)StatusType.New;
                    un.NotificationTypeId = (int)Notification.HumanNear;
                    un.UserId = acctCurrent.UserId;

                    db.UserNotifications.Add(un);
                }
                db.SaveChanges();

                oConn.Close();
                oConn.Dispose();
                oComm.Dispose();
                oReader.Close();
                oReader.Dispose();

                resultH.DynObject = Json(humanCount);
                resultH.Success = true;
            }
            catch (Exception ex)
            {
                resultH.MessageForUser = ex.Message;
                Logger.LogError(string.Format("ACTION:{0}\r\n---------------------------------------\r\nMESSAGE: {1}\r\n---------------------------------------\r\nINNER EXCEPTION {2}\r\n---------------------------------------\r\nSTACK TRACE: {3}\r\n---------------------------------------\r\nSOURCE: {4}", "Getting All Conversations", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : string.Empty), ex.StackTrace, ex.Source));
            }

            //Get new conversations
            try
            {
                var convos = db.UserMessages
                    .Where(um => (um.ToId == acctCurrent.UserId) && um.StatusId == ((int)StatusType.New))
                    .GroupBy(g => g.ConvoId)
                    .Select(g => g.OrderByDescending(t => t.Created).FirstOrDefault())
                    .Select(c => new
                    {
                        From = new
                        {
                            Id = c.FromId,
                            FirstName = c.UserProfile.FirstName,
                            LastName = c.UserProfile.LastName == null ? string.Empty : c.UserProfile.LastName,
                            Pic = c.UserProfile.ProfilePic,
                            Me = c.FromId == acctCurrent.UserId
                        },
                        To = new
                        {
                            Id = c.ToId,
                            FirstName = c.UserProfile1.FirstName,
                            LastName = c.UserProfile1.LastName == null ? string.Empty : c.UserProfile1.LastName,
                            Pic = c.UserProfile1.ProfilePic,
                            Me = c.ToId == acctCurrent.UserId
                        },
                        Msg = c.Msg.Substring(0, 100),
                        ConvoId = c.ConvoId,
                        Status = c.Status.Name,
                        Days = EntityFunctions.DiffDays(c.Created, DateTime.Now),
                        Hours = EntityFunctions.DiffHours(c.Created, DateTime.Now),
                        Minutes = EntityFunctions.DiffMinutes(c.Created, DateTime.Now),
                        Seconds = EntityFunctions.DiffSeconds(c.Created, DateTime.Now),
                        c.Created
                    })
                    .OrderByDescending(c2 => c2.Created);

                resultC.DynObject = Json(convos.Count());
                resultC.Success = true;
            }
            catch (Exception ex)
            {
                resultC.MessageForUser = ex.Message;
                Logger.LogError(string.Format("ACTION:{0}\r\n---------------------------------------\r\nMESSAGE: {1}\r\n---------------------------------------\r\nINNER EXCEPTION {2}\r\n---------------------------------------\r\nSTACK TRACE: {3}\r\n---------------------------------------\r\nSOURCE: {4}", "Getting All Conversations", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : string.Empty), ex.StackTrace, ex.Source));
            }

            result.Success = resultV.Success && resultH.Success && resultC.Success;
            result.DynObject = new { resultV, resultH, resultC };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Get(string convoid)
        {
            Result result = new Result();
            UserProfile acctCurrent = (UserProfile)Session["User"];
            HumansEntities db = new HumansEntities();

            //Get all conversations
            if (convoid == "all")
            {
                try
                {
                    var convos = db.UserMessages
                        .Where(um => um.FromId == acctCurrent.UserId || um.ToId == acctCurrent.UserId)
                        .GroupBy(g => g.ConvoId)
                        .Select(g => g.OrderByDescending(t => t.Created).FirstOrDefault())
                        .Select(c => new
                        {
                            From = new
                            {
                                Id = c.FromId,
                                FirstName = c.UserProfile.FirstName,
                                LastName = c.UserProfile.LastName == null ? string.Empty : c.UserProfile.LastName,
                                Pic = c.UserProfile.ProfilePic,
                                Me = c.FromId == acctCurrent.UserId
                            },
                            To = new
                            {
                                Id = c.ToId,
                                FirstName = c.UserProfile1.FirstName,
                                LastName = c.UserProfile1.LastName == null ? string.Empty : c.UserProfile1.LastName,
                                Pic = c.UserProfile1.ProfilePic,
                                Me = c.ToId == acctCurrent.UserId
                            },
                            Msg = c.Msg.Substring(0, 100),
                            ConvoId = c.ConvoId,
                            Status = c.Status.Name,
                            Days = EntityFunctions.DiffDays(c.Created, DateTime.Now),
                            Hours = EntityFunctions.DiffHours(c.Created, DateTime.Now),
                            Minutes = EntityFunctions.DiffMinutes(c.Created, DateTime.Now),
                            Seconds = EntityFunctions.DiffSeconds(c.Created, DateTime.Now),
                            c.Created
                        })
                        .OrderByDescending(c2 => c2.Created);

                    result.Success = true;
                    result.DynObject = convos;
                }
                catch (Exception ex)
                {
                    result.MessageForUser = ex.Message;
                    Logger.LogError(string.Format("ACTION:{0}\r\n---------------------------------------\r\nMESSAGE: {1}\r\n---------------------------------------\r\nINNER EXCEPTION {2}\r\n---------------------------------------\r\nSTACK TRACE: {3}\r\n---------------------------------------\r\nSOURCE: {4}", "Getting All Conversations", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : string.Empty), ex.StackTrace, ex.Source));
                }
            }

            //Get specific conversation
            else if (!string.IsNullOrEmpty(convoid))
            {
                Guid convoid2 = Guid.Parse(convoid);

                try
                {
                    //Update status
                    var msgsUpdate = db.UserMessages.Where(m => m.ConvoId == convoid2 && acctCurrent.UserId == m.ToId);
                    foreach (UserMessage mu in msgsUpdate)
                    {
                        mu.StatusId = (int)StatusType.Completed;
                    }
                    db.SaveChanges();

                    //Get messages to return
                    var msgs = db.UserMessages
                        .Where(um => um.ConvoId == convoid2)
                        .Select(c => new
                        {
                            From = new
                            {
                                Id = c.FromId,
                                FirstName = c.UserProfile.FirstName,
                                LastName = c.UserProfile.LastName == null ? string.Empty : c.UserProfile.LastName,
                                Pic = c.UserProfile.ProfilePic,
                                Me = c.FromId == acctCurrent.UserId
                            },
                            To = new
                            {
                                Id = c.ToId,
                                FirstName = c.UserProfile1.FirstName,
                                LastName = c.UserProfile1.LastName == null ? string.Empty : c.UserProfile1.LastName,
                                Pic = c.UserProfile1.ProfilePic,
                                Me = c.ToId == acctCurrent.UserId
                            },
                            Msg = c.Msg.Substring(0, 100),
                            ConvoId = c.ConvoId,
                            Status = c.Status.Name,
                            Days = EntityFunctions.DiffDays(c.Created, DateTime.Now),
                            Hours = EntityFunctions.DiffHours(c.Created, DateTime.Now),
                            Minutes = EntityFunctions.DiffMinutes(c.Created, DateTime.Now),
                            Seconds = EntityFunctions.DiffSeconds(c.Created, DateTime.Now),
                            c.Created
                        })
                        .OrderBy(c2 => c2.Created);

                    result.Success = true;
                    result.DynObject = msgs;
                }
                catch (Exception ex)
                {
                    result.MessageForUser = ex.Message;
                    Logger.LogError(string.Format("ACTION:{0}\r\n---------------------------------------\r\nMESSAGE: {1}\r\n---------------------------------------\r\nINNER EXCEPTION {2}\r\n---------------------------------------\r\nSTACK TRACE: {3}\r\n---------------------------------------\r\nSOURCE: {4}", "Getting All Conversations", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : string.Empty), ex.StackTrace, ex.Source));
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Add(string msg, string convoid="", int toId=0)
        {
            Result result = new Result();

            try
            {
                UserProfile acctCurrent = (UserProfile)Session["User"];
                HumansEntities db = new HumansEntities();
                UserMessage umNew = new UserMessage();

                //Use existing conversation
                if (!string.IsNullOrEmpty(convoid))
                {
                    Guid convoid2 = Guid.Parse(convoid);
                    UserMessage umConvo = db.UserMessages.FirstOrDefault(um => um.ConvoId == convoid2);
                    umNew.ToId = umConvo.FromId == acctCurrent.UserId ? umConvo.ToId : umConvo.FromId;
                    umNew.ConvoId = convoid2;
                }

                //New conversation
                else 
                {
                    //Check if a convo already exists with this person
                    UserMessage umExists = db.UserMessages.FirstOrDefault(u => (u.ToId == toId && u.FromId == acctCurrent.UserId) || (u.FromId == toId && u.ToId == acctCurrent.UserId));

                    umNew.ConvoId = umExists == null ? Guid.NewGuid() : umExists.ConvoId;
                    umNew.ToId = toId;

                    result.DynObject = umNew.ConvoId;
                }
                
                umNew.Created = DateTime.Now;
                umNew.FromId = acctCurrent.UserId;
                umNew.Msg = msg;
                umNew.StatusId = (int)StatusType.New;

                //Only add new msg, IF the conversations is NEW
                if (!string.IsNullOrEmpty(msg.Trim()) || string.IsNullOrEmpty(convoid))
                {
                    db.UserMessages.Add(umNew);
                    db.SaveChanges();
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.MessageForUser = ex.Message;
                Logger.LogError(string.Format("ACTION:{0}\r\n---------------------------------------\r\nMESSAGE: {1}\r\n---------------------------------------\r\nINNER EXCEPTION {2}\r\n---------------------------------------\r\nSTACK TRACE: {3}\r\n---------------------------------------\r\nSOURCE: {4}", "Getting All Conversations", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : string.Empty), ex.StackTrace, ex.Source));
            }

            return Json(result);
        }

        [HttpGet]
        [Authorize]
        [SessionCheckAttribute]
        public ActionResult Manage()
        {
            return View();
        }
    }
}
