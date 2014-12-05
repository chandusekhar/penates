using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.JSON.TableObjects {
    public class SaleTableJson {

        public long SaleID { get; set; }

        public string BillNumber { get; set; }

        public long DistributionCenter { get; set; }

        public string Client { get; set; }

        public string SaleDate { get; set; }
    }
}