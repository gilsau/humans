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
    
    public partial class UserNotification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int NotificationTypeId { get; set; }
        public int ReferenceId { get; set; }
        public int StatusId { get; set; }
    
        public virtual NotificationType NotificationType { get; set; }
        public virtual Status Status { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
