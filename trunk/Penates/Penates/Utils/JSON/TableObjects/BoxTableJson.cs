using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.JSON.TableObjects {
    public class BoxTableJson {

        public long BoxID { get; set; }

        public string ProductName { get; set; }

        public string AdquisitionDate { get; set; }

        public string State { get; set; }

        public long Quantity { get; set; }
    }
}