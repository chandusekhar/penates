using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.JSON.TableObjects {
    public class SectorTableJson {

        public long SectorID { get; set; }

        public string Description { get; set; }

        public string Deposit { get; set; }

        public long DistributionCenter { get; set; }

        public decimal UsedPercentage { get; set; }

        public decimal UsablePercentage { get; set; }
    }
}