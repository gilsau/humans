using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Humans.Web.Filters;
using Humans.Web.Models;
using Humans.Web.Helpers;

namespace Humans.Web.Controllers
{
    
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        [HttpGet]
        public JsonResult GetSuggestions(string partialname)
        {
            Result result = new Result();
            HumansEntities db = new HumansEntities();

            try
            {
                var users = db.UserProfiles
                    .Where(up => up.FirstName.StartsWith(partialname))
                    .Select(up => new { up.UserId, up.FirstName, up.LastName });

                result.Success = true;
                result.DynObject = users;
            }
            catch (Exception ex)
            {
                result.MessageForUser = ex.Message;
                Logger.LogError(string.Format("ACTION:{0}\r\n---------------------------------------\r\nMESSAGE: {1}\r\n---------------------------------------\r\nINNER EXCEPTION {2}\r\n---------------------------------------\r\nSTACK TRACE: {3}\r\n---------------------------------------\r\nSOURCE: {4}", "Getting All Conversations", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : string.Empty), ex.StackTrace, ex.Source));
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveLocation(double lat, double lng)
        {
            HumansEntities db = new HumansEntities();
            UserProfile acct = db.UserProfiles.Single(a => a.UserName.Equals(WebSecurity.CurrentUserName));
            acct.Lat = lat;
            acct.Lng = lng;
            db.SaveChanges();

            Session["User"] = acct;

            return Json("");
        }
        
        [HttpPost]
        public JsonResult UpdatePhoto()
        {
            HumansEntities db = new HumansEntities();
            UserProfile acct = db.UserProfiles.Single(a => a.UserName.Equals(WebSecurity.CurrentUserName));

            //Upload file
            Result result = new Result();
            Uploader.UploadFile(Request.Files[0], "profilepics", out result);

            //Save in database
            if (result.Success && result.DynObject != null)
            {
                acct.ProfilePic = result.DynObject;

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

            //Update session with latest user
            Session["User"] = acct;

            return Json(result);
        }

        [HttpGet]
        [Authorize]
        [SessionCheckAttribute]
        public ActionResult ViewProfile(int id)
        {
            HumansEntities db = new HumansEntities();
            UserSport userSport = db.UserSports.SingleOrDefault(us => us.Id == id);

            return View(userSport);
        }

        [HttpGet]
        [Authorize]
        [SessionCheckAttribute]
        public ActionResult MyProfile()
        {
            HumansEntities db = new HumansEntities();
            UserProfile acct = db.UserProfiles.Single(a => a.UserName.Equals(WebSecurity.CurrentUserName));
        
            MyProfileModel model = new MyProfileModel();
            model.UserName = acct.UserName;
            model.FirstName = acct.FirstName;
            model.MiddleName = acct.MiddleName;
            model.LastName = acct.LastName;
            model.City = acct.City;
            model.StateId = acct.StateId;
            model.Zipcode = acct.Zipcode;
            model.Title = acct.Title;
            model.Gender = acct.Gender;
            model.Birthdate = acct.Birthdate;
            model.Company = acct.Company;
            model.CompanyAddress1 = acct.CompanyAddress1;
            model.CompanyAddress2 = acct.CompanyAddress2;
            model.CompanyCity = acct.CompanyCity;
            model.CompanyStateId = acct.CompanyStateId;
            model.CompanyZipcode = acct.CompanyZipcode;
            model.CompanyPhone = acct.CompanyPhone;
            model.PricePerHrMin = acct.PricePerHrMin;
            model.PricePerHrMax = acct.PricePerHrMax;
            model.Paypal = acct.Paypal;
            model.NameOnCard = acct.NameOnCard;
            model.CreditCardNo = acct.CreditCardNo;
            model.ExpDateMo = acct.ExpDateMo;
            model.ExpDateYr = acct.ExpDateYr;
            model.CardType = acct.CardType;
            model.ProfilePicLoad = acct.ProfilePic;
            model.FB = acct.FB != true ? false : true;

            model.States = db.States;

            return View(model);
        }

        [HttpPost]
        public JsonResult MyProfile(MyProfileModel model)
        {
            HumansEntities db = new HumansEntities();

            UserProfile acct = db.UserProfiles.Single(a => a.UserName.Equals(WebSecurity.CurrentUserName));
            acct.FirstName = model.FirstName;
            acct.MiddleName = model.MiddleName;
            acct.LastName = model.LastName;
            acct.City = model.City;
            acct.StateId = model.StateId > 0 ? model.StateId : acct.StateId;
            acct.Zipcode = model.Zipcode;
            acct.Gender = model.Gender;
            acct.Birthdate = model.Birthdate;
            acct.Title = model.Title;
            acct.Company = model.Company;
            acct.CompanyAddress1 = model.CompanyAddress1;
            acct.CompanyAddress2 = model.CompanyAddress2;
            acct.CompanyCity = model.CompanyCity;
            acct.CompanyStateId = model.CompanyStateId > 0 ? model.CompanyStateId : acct.CompanyStateId;
            acct.CompanyZipcode = model.CompanyZipcode;
            acct.CompanyPhone = model.CompanyPhone;
            acct.PricePerHrMin = model.PricePerHrMin;
            acct.PricePerHrMax = model.PricePerHrMax;
            acct.Paypal = model.Paypal;
            acct.NameOnCard = model.NameOnCard;
            acct.CreditCardNo = model.CreditCardNo;
            acct.ExpDateMo = model.ExpDateMo;
            acct.ExpDateYr = model.ExpDateYr;
            acct.CardType = model.CardType;

            //Update password
            if (!string.IsNullOrEmpty(model.Password))
            {
                string token = WebSecurity.GeneratePasswordResetToken(model.UserName);
                WebSecurity.ResetPassword(token, model.Password);
            }

            //Update database
            try
            {
                db.SaveChanges();
                TempData["Msg"] = "Your account has been updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = string.Format("There was a problem updating your profile. ({0})", ex.Message);
            }
            
            //Return list of states
            model.States = db.States;

            //Save updated acct to session
            Session["User"] = acct;

            return Json("");
        }

        [HttpGet]
        [Authorize]
        [SessionCheckAttribute]
        public ActionResult Settings()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model, string returnUrl="")
        {
            //Log in
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: true))
            {
                HumansEntities db = new HumansEntities();
                Session["User"] = db.UserProfiles.SingleOrDefault(u => u.UserName.Equals(model.UserName));

                return RedirectToLocal("/sports/manage");
            }
            else
            {
                TempData["Msg"] = "The username or password provided is incorrect.";
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult FacebookLogin(FacebookLoginModel model)
        {
            string retView = "/account/login";

            if(ModelState.IsValid)
            {
                HumansEntities db = new HumansEntities();
                string picUrl = string.Format("http://graph.facebook.com/{0}/picture?type=normal", model.fb_Id);
                string pwd = string.Format("{0}-{1}", model.fb_Id, model.fb_Email);

                //Add new user
                if (!WebSecurity.UserExists(model.fb_Email))
                {
                    //Add user
                    WebSecurity.CreateUserAndAccount(model.fb_Email, pwd, new { ProfilePic = picUrl, FirstName = model.fb_FirstName, LastName = model.fb_LastName, Gender = (model.fb_Gender == "male" ? "M" : "F"), Birthdate = model.fb_Birthdate, FB = true }, false);
                }

                //Update user's info from facebook
                else
                {
                    UserProfile userToUpdate = db.UserProfiles.SingleOrDefault(u => u.UserName.Equals(model.fb_Email));
                    userToUpdate.FirstName = model.fb_FirstName;
                    userToUpdate.LastName = model.fb_LastName;
                    userToUpdate.Gender = (model.fb_Gender == "male" ? "M" : "F");
                    userToUpdate.Birthdate = DateTime.Parse(model.fb_Birthdate);
                    db.SaveChanges();
                }

                if(WebSecurity.Login(model.fb_Email, pwd, persistCookie: true))
                {
                    Session["User"] = db.UserProfiles.SingleOrDefault(u => u.UserName.Equals(model.fb_Email));
                    retView = "/sports/manage";
                }
            }

            return RedirectToLocal(retView);
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            
            FormsAuthentication.SignOut();
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            //FormsAuthentication.RedirectToLoginPage();

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Confirm(string token=null)
        {
            if (!string.IsNullOrEmpty(token))
            {
                if (WebSecurity.ConfirmAccount(token))
                {
                    TempData["Msg"] = "Your account has been confirmed. <a href='/account/login'>Log into</a> your account.";
                }
                else
                {
                    TempData["Msg"] = "Sorry, that is an invalid token. <a href='/account/register'>Register</a> for a new account.";
                }
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPass()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPass(string password, string token)
        {
            if (WebSecurity.ResetPassword(token, password))
            {
                TempData["Msg"] = "Your password has been reset. <a href='/account/login'>Log into</a> your account.";
            }
            else
            {
                TempData["Msg"] = "Sorry, your token is invalid or expired. <a href='/account/forgotpassword'>Try to reset</a> your password again.";
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(string email=null)
        {
            if (!string.IsNullOrEmpty(email))
            {
                if (WebSecurity.UserExists(email))
                {
                    string token = WebSecurity.GeneratePasswordResetToken(email);

                    //Send confirmation email
                    string server = Request.ServerVariables["server_name"];
                    string port = ":" + Request.ServerVariables["server_port"];
                    string baseAddress = server + port;

                    Result result = new Result();
                    Helpers.Emailer.SendMail(
                        "The Dog Sitter: Reset Your Password",
                            "Please follow the link below, to reset your password:<br/><br/><a href=\"http://"
                            + baseAddress + "/account/resetpass?token="
                            + token + "\">http://"
                            + baseAddress
                            + "/account/resetpass?token="
                            + token
                            + "</a>", email, out result);

                    TempData["Msg"] = "Please check your email for instructions to reset your password.";
                }
                else
                {
                    TempData["Msg"] = "We could not find a user with this email address. Try again, or <a href='/account/register'>Register</a> for a new account.";
                }
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    string pwd = Membership.GeneratePassword(8, 2);

                    //Create account
                    string token = WebSecurity.CreateUserAndAccount(model.UserName, pwd, new {ProfilePic="icon_profile.jpg", FirstName=model.FirstName, Gender=model.Gender, Birthdate=model.Birthdate}, true);

                    //Send confirmation email
                    string server = Request.ServerVariables["server_name"];
                    string port = ":" + Request.ServerVariables["server_port"];
                    string baseAddress = server + port;

                    Result result = new Result();
                    Helpers.Emailer.SendMail2(model.UserName, "HUMANS Registration Confirmation",
                        "Thanks for signing up with HUMANS!<br/><br/>"
                        + "Here is your temporary password: " + pwd + "<br/><br/>"
                        + "Please verify your email by clicking on the link below, then sign in.<br/><br/>"
                        + "<a href=\"http://"
                            + baseAddress + "/account/confirm?token="
                            + token + "\">http://"
                            + baseAddress
                            + "/account/confirm?token="
                            + token
                            + "</a><br/><br/>"
                            + "Get Ready To Ride!<br/><br/><br/>"
                            + "- HUMANS Admin Staff", out result);

                    if (result.Success)
                    {
                        TempData["Msg"] = "Registration was successful. Please check your email to confirm your registration and get your temporary password.";
                    }
                    else
                    {
                        TempData["Msg"] = result.MessageForUser;
                    }
                }
                catch (MembershipCreateUserException e)
                {
                    TempData["Msg"] = ErrorCodeToString(e.StatusCode);
                }
            }
            
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (HumansEntities db = new HumansEntities())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
        
    }
}
