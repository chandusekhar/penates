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
    
    public partial class Pack
    {
        public Pack()
        {
            this.Boxes = new HashSet<Box>();
            this.CommercialAgreements = new HashSet<CommercialAgreement>();
        }
    
        public long PackID { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public string SerialNumber { get; set; }
        public bool Deleted { get; set; }
    
        public virtual ICollection<Box> Boxes { get; set; }
        public virtual ICollection<CommercialAgreement> CommercialAgreements { get; set; }
    }
}