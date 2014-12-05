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
    
    public partial class InternalDistributionCenter : DistributionCenter
    {
        public InternalDistributionCenter()
        {
            this.Deposits = new HashSet<Deposit>();
            this.TemporaryDeposits = new HashSet<TemporaryDeposit>();
        }
    
        public int Floors { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Depth { get; set; }
        public Nullable<decimal> Capacity { get; set; }
        public decimal UsedSpace { get; set; }
    
        public virtual ICollection<Deposit> Deposits { get; set; }
        public virtual ICollection<TemporaryDeposit> TemporaryDeposits { get; set; }
    }
}