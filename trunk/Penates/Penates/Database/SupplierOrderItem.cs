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
    
    public partial class SupplierOrderItem
    {
        public string IDSupplierOrder { get; set; }
        public long IDSupplier { get; set; }
        public long IDProduct { get; set; }
        public string SupplierProductID { get; set; }
        public long ItemBoxes { get; set; }
        public long ReceivedQty { get; set; }
    
        public virtual ProvidedBy ProvidedBy { get; set; }
        public virtual SupplierOrder SupplierOrder { get; set; }
    }
}
