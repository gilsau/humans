//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Humans.Web
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserProfile
    {
        public UserProfile()
        {
            this.UserMessages = new HashSet<UserMessage>();
            this.UserMessages1 = new HashSet<UserMessage>();
            this.UserNotifications = new HashSet<UserNotification>();
            this.UserSports = new HashSet<UserSport>();
            this.UserUsers = new HashSet<UserUser>();
            this.UserUsers1 = new HashSet<UserUser>();
            this.UserVenues = new HashSet<UserVenue>();
        }
    
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public Nullable<int> StateId { get; set; }
        public string Zipcode { get; set; }
        public string ProfilePic { get; set; }
        public string Gender { get; set; }
        public System.DateTime Birthdate { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string CompanyAddress1 { get; set; }
        public string CompanyAddress2 { get; set; }
        public string CompanyCity { get; set; }
        public Nullable<int> CompanyStateId { get; set; }
        public string CompanyZipcode { get; set; }
        public string CompanyPhone { get; set; }
        public Nullable<decimal> PricePerHrMin { get; set; }
        public Nullable<decimal> PricePerHrMax { get; set; }
        public string Paypal { get; set; }
        public string NameOnCard { get; set; }
        public string CreditCardNo { get; set; }
        public Nullable<int> ExpDateMo { get; set; }
        public Nullable<int> ExpDateYr { get; set; }
        public string CardType { get; set; }
        public Nullable<double> Lat { get; set; }
        public Nullable<double> Lng { get; set; }
        public Nullable<bool> FB { get; set; }
    
        public virtual State State { get; set; }
        public virtual State State1 { get; set; }
        public virtual ICollection<UserMessage> UserMessages { get; set; }
        public virtual ICollection<UserMessage> UserMessages1 { get; set; }
        public virtual ICollection<UserNotification> UserNotifications { get; set; }
        public virtual ICollection<UserSport> UserSports { get; set; }
        public virtual ICollection<UserUser> UserUsers { get; set; }
        public virtual ICollection<UserUser> UserUsers1 { get; set; }
        public virtual ICollection<UserVenue> UserVenues { get; set; }
    }
}
