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
    
    public partial class Stock_Invented
    {
        public long IDInventory { get; set; }
        public long IDProduct { get; set; }
        public long Quantity { get; set; }
        public decimal ValueOfStored { get; set; }
    
        public virtual Inventory Inventory { get; set; }
    }
}