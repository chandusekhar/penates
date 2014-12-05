//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Penates.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Notification
    {
        public Notification()
        {
            this.Users = new HashSet<User>();
        }
    
        public System.DateTime Date { get; set; }
        public long NotificationID { get; set; }
        public long IDNotificationType { get; set; }
        public Nullable<long> IDDistributionCenter { get; set; }
        public Nullable<long> IDPolicy { get; set; }
        public string Message { get; set; }
    
        public virtual DistributionCenter DistributionCenter { get; set; }
        public virtual NotificationType NotificationType { get; set; }
        public virtual Policy Policy { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}