using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.JSON.TableObjects {
    public class RackTableJson {

        public long RackID { get; set; }

        public string Description { get; set; }

        public string RackCode { get; set; }

        public string Deposit { get; set; }

        public decimal UsedPercentage { get; set; }
    }
}