using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.Transactions.ProductsReceptions
{
    public class ExternalLocationAssignationModel
    {
        public ExternalLocationAssignationModel()
        {
            this.Error = false;
            this.Message = ""; 
        }

        public bool Error { set; get; }

        public string Message { set; get; }                       

        public string DistributionCenterDestiny { get; set; }

        public SortedDictionary<string, List<long>> boxesIdsPerProductName { get; set; }


    }
}