using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.JSON.TableObjects {
    public class DistributionCenterTableJson {

        public long DistributionCenterID { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public decimal UsedPercentage { get; set; }

        public decimal UsableSpacePercentage { get; set; }
    }
}