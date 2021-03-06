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
    
    public partial class User
    {
        public User()
        {
            this.DistributionCenters = new HashSet<DistributionCenter>();
            this.Notifications = new HashSet<Notification>();
            this.Roles = new HashSet<Role>();
        }
    
        public string FileNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public System.DateTime LastLoginDate { get; set; }
        public bool Active { get; set; }
    
        public virtual ICollection<DistributionCenter> DistributionCenters { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual User Users1 { get; set; }
        public virtual User User1 { get; set; }
        public virtual OrdersValueSecurity OrdersValueSecurity { get; set; }
        public virtual ProductsValueSecurity ProductsValueSecurity { get; set; }
    }
}
