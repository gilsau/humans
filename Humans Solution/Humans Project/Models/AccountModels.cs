using System;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace Humans.Web.Models
{
    public class MyProfileModel
    {
        [Required]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public int? StateId { get; set; }
        public IEnumerable<State> States { get; set; }
        public string Zipcode { get; set; }
        public string ProfilePicLoad { get; set; }
        public HttpPostedFileBase ProfilePic { get; set; }
        public string Gender { get; set; }
        public DateTime Birthdate { get; set; }
        public bool FB { get; set; }

        [Required]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Must be a valid Email Address")]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }
        
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Title { get; set; }
        public string Company { get; set; }
        public string CompanyAddress1 { get; set; }
        public string CompanyAddress2 { get; set; }
        public string CompanyCity { get; set; }
        public int? CompanyStateId { get; set; }
        public IEnumerable<State> CompanyStates { get; set; }
        public string CompanyZipcode { get; set; }
        public string CompanyPhone { get; set; }

        [DataType(DataType.Currency)]
        public decimal? PricePerHrMin { get; set; }

        [DataType(DataType.Currency)]
        public decimal? PricePerHrMax { get; set; }

        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Must be a valid Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Paypal { get; set; }

        public string NameOnCard { get; set; }
        public string CreditCardNo { get; set; }
        public int? ExpDateMo { get; set; }
        public int? ExpDateYr { get; set; }
        public string CardType { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class FacebookLoginModel
    {
        public string fb_Id { get; set; }
        public string fb_FirstName { get; set; }
        public string fb_LastName { get; set; }
        public string fb_Email { get; set; }
        public string fb_Gender { get; set; }
        public string fb_Birthdate { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Email")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Must be a valid Email Address")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required]
        [RegularExpression(@"^((0?[13578]|10|12)(-|\/)(([1-9])|(0[1-9])|([12])([0-9]?)|(3[01]?))(-|\/)((19)([2-9])(\d{1})|(20)([01])(\d{1})|([8901])(\d{1}))|(0?[2469]|11)(-|\/)(([1-9])|(0[1-9])|([12])([0-9]?)|(3[0]?))(-|\/)((19)([2-9])(\d{1})|(20)([01])(\d{1})|([8901])(\d{1})))$", ErrorMessage = "Must be a valid Date")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Birthdate")]
        public string Birthdate { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }

    public class Peep
    {
        public int UserId { get; set; }
        public int UserSportId { get; set; }
        public string ProfilePic { get; set; }
        public string FirstName { get; set; }
        public int Age { get; set; }
        public int Distance { get; set; }
        public string Note { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public bool Remembered { get; set; }
    }

    public enum StatusType
    {
        New = 1,
        Pending = 2,
        Completed = 3,
        Archived = 4,
        Accepted = 5,
        Denied = 6,
        Cancelled = 7
    }

    public enum Notification
    {
        VenueNear = 1,
        CommentForVenue = 2,
        ReplyToVenueComment = 3,
        ReviewForVenue = 4,
        PhotoForVenue = 5,
        VideoForVenue = 6,
        HumanNear = 7,
        CommentForHuman = 8,
        ReplyToHumanComment = 9,
        PhotoForHuman = 10,
        VideoForHuman = 11
    }
}