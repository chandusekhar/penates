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
    
    public partial class Transfer
    {
        public Transfer()
        {
            this.Boxes = new HashSet<Box>();
        }
    
        public long TransferID { get; set; }
        public long IDSender { get; set; }
        public long IDReciever { get; set; }
        public System.DateTime TransferDepartureDate { get; set; }
        public Nullable<System.DateTime> TransferArrivalDate { get; set; }
        public string COT { get; set; }
    
        public virtual DistributionCenter DistributionCenter { get; set; }
        public virtual DistributionCenter DistributionCenterSend { get; set; }
        public virtual ICollection<Box> Boxes { get; set; }
    }
}