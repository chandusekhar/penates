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
    
    public partial class ExternalDistributionCenter : DistributionCenter
    {
        public ExternalDistributionCenter()
        {
            this.ExternalBoxes = new HashSet<ExternalBox>();
        }
    
        public string ContactName { get; set; }
        public string Telephone2 { get; set; }
        public bool HasMaxCapacity { get; set; }
    
        public virtual ICollection<ExternalBox> ExternalBoxes { get; set; }
    }
}
