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
    
    public partial class ItemsState
    {
        public ItemsState()
        {
            this.Boxes = new HashSet<Box>();
        }
    
        public long ItemStateID { get; set; }
        public string Description { get; set; }
        public bool Deleted { get; set; }
    
        public virtual ICollection<Box> Boxes { get; set; }
    }
}
