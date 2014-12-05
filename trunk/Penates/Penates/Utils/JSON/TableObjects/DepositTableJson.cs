using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.JSON.TableObjects {
    public class DepositTableJson {

        public long DepositID { get; set; }

        public string Description { get; set; }

        public int Floor { get; set; }

        public decimal UsedPercentage { get; set; }

        public decimal UsablePercentage { get; set; }
    }
}