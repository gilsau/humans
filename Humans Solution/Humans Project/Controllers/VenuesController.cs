using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using Humans.Web.Models;
using Humans.Web.Helpers;

namespace Humans.Web.Controllers
{
    
    public class VenuesController : Controller
    {
        [HttpGet]
        [Authorize]
        [SessionCheckAttribute]
        public ActionResult Manage(int sportid=0)
        {
            HumansEntities db = new HumansEntities();
            ViewBag.SportCats = db.SportCategories.ToList();
            ViewBag.Sports = db.Sports.ToList();
            
            UserProfile acctCurrent = (UserProfile)Session["User"];
            UserSport usport = acctCurrent.UserSports.SingleOrDefault(s => s.UserId == acctCurrent.UserId && s.SportId == sportid);
            
            return View(usport);
        }

        [HttpGet]
        [Authorize]
        [SessionCheckAttribute]
        public ActionResult ViewVenue(int id=0)
        {
            HumansEntities db = new HumansEntities();
            Venue venue = db.Venues.SingleOrDefault(v => v.Id == id);

            return View(venue);
        }

        [HttpPost]
        public JsonResult UnRemember(int venueid)
        {
            int currUserId = ((UserProfile)Session["User"]).UserId;

            HumansEntities db = new HumansEntities();

            UserVenue rem = db.UserVenues.FirstOrDefault(r => r.UserId == currUserId && r.VenueId == venueid);
            if (rem != null)
            {
                db.UserVenues.Remove(rem);
                db.SaveChanges();
            }

            Session["User"] = db.UserProfiles.SingleOrDefault(a => a.UserId == currUserId);

            return Json("");
        }

        [HttpPost]
        public JsonResult Remember(int venueid)
        {
            int currUserId = ((UserProfile)Session["User"]).UserId;

            HumansEntities db = new HumansEntities();

            if (!db.UserVenues.Any(r => r.UserId == currUserId && r.VenueId == venueid))
            {
                db.UserVenues.Add(new UserVenue() { UserId = currUserId, VenueId = venueid });
                db.SaveChanges();
            }

            Session["User"] = db.UserProfiles.SingleOrDefault(a => a.UserId == currUserId);

            return Json("");
        }

        [HttpPost]
        public JsonResult GetRemembered(int sportid)
        {
            return Search(2, sportid);
        }

        [HttpPost]
        public JsonResult Find(int sportid, int distance = 0)
        {
            return Search(1, sportid, distance);
        }

        public JsonResult Search(int searchType, int sportid, int distance=0)
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
                strComm = string.Format("select	VenueId=v.Id, " +
		                                        "VenueSportId=vs.Id, " +
		                                        "v.Logo, " +
		                                        "v.Name, " +
                                                "v.Address, " +
                                                "v.Address2, " +
                                                "v.City, " +
                                                "State=s.Abbrev, " +
                                                "v.Zip, " +
		                                        "v.Lat, " +
		                                        "v.Lng, " +
		                                        "v.[Description], " +
		                                        "Distance=dbo.DistanceBetween({0}, {1}, v.Lat, v.Lng), " +
                                                "Remembered=isnull(uv.id, 0) " +
                                        "from venue v " +
                                        "left join VenueSports vs on vs.VenueId = v.Id " +
                                        "left join [State] s on s.Id = v.StateId " +
                                        "left join UserVenues uv on uv.VenueId = v.Id " +
                                        "where vs.SportId = {2} " +
                                        "and dbo.DistanceBetween({0}, {1}, v.Lat, v.Lng) < {3} ",
                                            acctLat, acctLng, sportid, distance);
            }

            //Get Remembered
            else if (searchType == 2)
            {
                strComm = string.Format("select " +
                                            "VenueId=v.Id, " +
                                            "v.Logo, " +
                                            "v.Name, " +
                                            "v.Address, " +
                                            "v.Address2, " +
                                            "v.City, " +
                                            "State=s.Abbrev, " +
                                            "v.Zip, " +
                                            "v.Lat, " +
                                            "v.Lng, " +
                                            "v.[Description], " +
                                            "Distance=dbo.DistanceBetween({0}, {1}, v.Lat, v.Lng) " +
                                        "from venue v " +
                                        "inner join uservenues uv on uv.VenueId = v.Id " +
                                        "left join [State] s on s.Id = v.StateId " +
                                        "left join VenueSports vs on vs.VenueId = v.Id " +
                                        "where uv.UserId = {2} and  vs.SportId = {3}", acctLat, acctLng, acctCurrent.UserId, sportid);
            }

            SqlCommand oComm = new SqlCommand();
            oComm.Connection = oConn;
            oComm.CommandText = strComm;

            List<Spot> spots = new List<Spot>();

            SqlDataReader oReader = oComm.ExecuteReader();
            while (oReader.Read())
            {
                Spot spot = new Spot();
                spot.VenueId = (int)oReader["VenueId"];
                spot.Name = oReader["Name"].ToString();
                spot.Address = oReader["Address"].ToString();
                spot.Address2 = oReader["Address2"].ToString();
                spot.City = oReader["City"].ToString();
                spot.State = oReader["State"].ToString();
                spot.Zip = oReader["Zip"].ToString();
                spot.Logo = oReader["Logo"].ToString();
                spot.Description = oReader["Description"].ToString();
                spot.Distance = (int)Math.Round((float)oReader["Distance"], 0);
                spot.Lat = (double)oReader["Lat"];
                spot.Lng = (double)oReader["Lng"];

                if (searchType == 1)
                {
                    spot.Remembered = int.Parse(oReader["Remembered"].ToString()) > 0 ? true : false;
                }

                spots.Add(spot);
            }
            oConn.Close();
            oConn.Dispose();
            oComm.Dispose();
            oReader.Close();
            oReader.Dispose();

            Peep currPeep = new Peep() { FirstName = acctCurrent.FirstName, ProfilePic = acctCurrent.ProfilePic, Distance = 0, Note = "", Age = DateTime.Today.Subtract(acctCurrent.Birthdate).Days / 365, Lat = acctLat, Lng = acctLng };

            return Json(new { CurrPeep = currPeep, Spots = spots.OrderBy(s => s.Distance) });
        }

        [HttpPost]
        public JsonResult UpdateLogo()
        {
            int venueid = int.Parse(Request.Form["VenueId"]);

            HumansEntities db = new HumansEntities();
            Venue venue = db.Venues.Single(v => v.Id == venueid);

            //Upload file
            Result result = new Result();
            Uploader.UploadFile(Request.Files[0], "logos", out result);

            //Save in database
            if (result.Success && result.DynObject != null)
            {
                venue.Logo = result.DynObject;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.MessageForUser = ex.Message;
                }
            }

            return Json(result);

        }

        [HttpPost]
        public JsonResult AddOne(AddVenueModel venue, string sportids)
        {
            Result result = new Result();

            HumansEntities db = new HumansEntities();
            Venue venueFound = db.Venues.FirstOrDefault(v => v.PlaceId == venue.placeid);

            int vStateId = 0;
            if (!string.IsNullOrEmpty(venue.state))
            {
                State stateFound = db.States.FirstOrDefault(s => s.Abbrev.ToLower() == venue.state.Trim().ToLower());
                vStateId = stateFound != null ? stateFound.Id : 0;
            }

            try
            {
                //Add venue
                if (venueFound == null)
                {
                    Venue newVenue = new Venue();
                    newVenue.Name = venue.name;
                    newVenue.Address = !string.IsNullOrEmpty(venue.address) ? venue.address : string.Empty;
                    newVenue.Address2 = venue.address2;
                    newVenue.City = venue.city;
                    if (vStateId > 0) { newVenue.StateId = vStateId; }
                    newVenue.Zip = !string.IsNullOrEmpty(venue.zip) ? venue.zip : string.Empty;
                    newVenue.Description = venue.desc;
                    newVenue.Lat = venue.lat;
                    newVenue.Lng = venue.lng;
                    newVenue.PlaceId = venue.placeid;
                    newVenue.Logo = "icon_venue.png";

                    venueFound = db.Venues.Add(newVenue);
                    db.SaveChanges();
                }
                result.DynObject = venueFound.Id;

                //Add venue to sport
                foreach (string sportid in sportids.Split(','))
                {
                    int sportid_int = int.Parse(sportid);

                    if (!db.VenueSports.Any(vs => vs.VenueId == venueFound.Id && vs.SportId == sportid_int))
                    {
                        db.VenueSports.Add(new VenueSport() { VenueId = venueFound.Id, SportId = int.Parse(sportid) });
                        db.SaveChanges();
                    }
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.MessageForUser = ex.Message;
                Logger.LogError(string.Format("ACTION:{0}\r\n---------------------------------------\r\nMESSAGE: {1}\r\n---------------------------------------\r\nINNER EXCEPTION {2}\r\n---------------------------------------\r\nSTACK TRACE: {3}\r\n---------------------------------------\r\nSOURCE: {4}", "Adding New Venue", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : string.Empty), ex.StackTrace, ex.Source));
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Add(List<AddVenueModel> venues, int sportid)
        {
            HumansEntities db = new HumansEntities();

            foreach (AddVenueModel venue in venues)
            {
                Venue venueFound = db.Venues.FirstOrDefault(v => v.PlaceId == venue.placeid);

                int vStateId = 0;
                if(!string.IsNullOrEmpty(venue.state)){
                    State stateFound = db.States.FirstOrDefault(s => s.Abbrev.ToLower() == venue.state.Trim().ToLower());
                    vStateId = stateFound != null ? stateFound.Id : 0;
                }   

                //Add venue
                if(venueFound == null){
                    Venue newVenue = new Venue();
                    newVenue.Name = venue.name;
                    newVenue.Address = !string.IsNullOrEmpty(venue.address) ? venue.address : string.Empty;
                    newVenue.Address2 = venue.address2;
                    newVenue.City = venue.city;
                    if (vStateId > 0) { newVenue.StateId = vStateId; }
                    newVenue.Zip = !string.IsNullOrEmpty(venue.zip) ? venue.zip : string.Empty;
                    newVenue.Description = venue.desc;
                    newVenue.Lat = venue.lat;
                    newVenue.Lng = venue.lng;
                    newVenue.PlaceId = venue.placeid;
                    newVenue.Logo = "icon_venue.png";

                    venueFound = db.Venues.Add(newVenue);
                    db.SaveChanges();
                }

                //Add venue to sport
                if (!db.VenueSports.Any(vs => vs.VenueId == venueFound.Id && vs.SportId == sportid))
                {
                    db.VenueSports.Add(new VenueSport() { VenueId = venueFound.Id, SportId = sportid });
                    db.SaveChanges();
                }
            }

            return Json("");
        }
    }

    public class Spot
    {
        public int VenueId { get; set; }
        public int VenueSportId { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Description { get; set; }
        public int Distance { get; set; }
        public bool Remembered { get; set; }
    }

    public class AddVenueModel
    {
        public string name { get; set; }
        public string address { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string desc  { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string placeid { get; set; }
        public HttpPostedFileBase logo { get; set; }
    }
}
