using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.JSON.TableObjects {
    public class OrderItemsTableJson {

        public string DT_RowId { get; set; }

        public long ProductID { get; set; }

        public string OrderID { get; set; }

        public long SupplierID { get; set; }

        public string ProductName { get; set; }

        public string SupplierProductCode { get; set; }

        public long ItemQty { get; set; }

        public long ItemsVerified { get; set; }

    }
}