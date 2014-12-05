using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.Objects
{
    public class InternalDistributionCenterDetails
    {
        public InternalDistributionCenterDetails()
        {
            Deposits = 0;
            Sectors = 0;
            Halls = 0;
            Racks = 0;
            Shelfs = 0;
            Boxes = 0;
            ProducsQuantity = 0;
        }

        public long Deposits { get; set; }

        public long Sectors { get; set; }

        public long Halls { get; set; }

        public long Racks { get; set; }

        public long Shelfs { get; set; }

        public long Boxes { get; set; }

        public long ProducsQuantity { get; set; }
    }
}