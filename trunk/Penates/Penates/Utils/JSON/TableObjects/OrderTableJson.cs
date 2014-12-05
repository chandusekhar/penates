using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.JSON.TableObjects {
    public class OrderTableJson {

        public string ID { get; set; }

        public long SupplierID { get; set; }

        public string SupplierName { get; set; }

        public string OrderDate { get; set; }

        public bool Canceled { get; set; }

        public bool Received { get; set; }

        public decimal Total { get; set; }
    }
}