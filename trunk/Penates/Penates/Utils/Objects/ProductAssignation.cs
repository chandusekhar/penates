using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.Objects
{
    public class ProductAssignation
    {
        public ProductAssignation()
        {
            this.BoxesID = new List<long>();
            this.IsTransitory = false; 
        }
        public long ContainerID { get; set; }

        public string ContainerTypeDescription { get; set; }

        public long ProductID { get; set; }

        public string ProductName { get; set; }

        public List<long> BoxesID { get; set; }

        public decimal BoxesQuantity { get; set; }

        public Location ContainerLocation { get; set; }

        public bool IsTransitory { get; set; }

    }
}