using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.Objects
{
    public class Location
    {
        public Location()
        {
            this.TempDeposit = false; 
        }

        public long DepositID { get; set; }

        public string DepositDescription { get; set; }

        public long SectorID { get; set; }

        public string SectorDescription { get; set; }

        public long HallID { get; set; }

        public string HallDescription { get; set; }

        public long RackID { set; get; }

        public string RackDescription { get; set; }

        public int ShelfNumber { get; set; }

        public string ShelfDivisionCode { get; set; }

        public long ShelfSubdivisionID { get; set; }

        public bool TempDeposit { get; set; }
    }
}