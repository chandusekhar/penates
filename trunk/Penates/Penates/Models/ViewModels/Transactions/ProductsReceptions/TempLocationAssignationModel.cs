using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.Transactions.ProductsReceptions
{
    public class TempLocationAssignationModel
    {

        public TempLocationAssignationModel()
        {
            this.Error = false;
            this.Message = "";
            this.boxesIdsPerProductName = new SortedDictionary<string, List<long>>();
        }

        public bool Error { set; get; }

        public string Message { set; get; }                       

        public string TempDepositDescription { get; set; }

        public long TempDepositID { get; set; }

        public long DistributionCenterID { get; set; }

        public string DistributionCenterAddress { get; set; }

        public SortedDictionary<string, List<long>> boxesIdsPerProductName { get; set; }
    }
}