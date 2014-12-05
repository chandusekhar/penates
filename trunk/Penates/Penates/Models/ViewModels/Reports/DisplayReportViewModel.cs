using Penates.App_GlobalResources.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace Penates.Models.ViewModels.Reports
{
    public class DisplayReportViewModel {

        public DisplayReportViewModel()
        {
            this.tableID = "table";   
            this.Error = false;
        }

        public bool? Error { get; set; }

        [Display(Name = "DistributionCenterValuation", ResourceType = typeof(ModelFormsResources))]
        public long DistributionCenterValuation { get; set; }

        [Display(Name = "DistributionCenterAddress", ResourceType = typeof(ModelFormsResources))]
        public string DistributionCenterAddress { get; set; }

        [Display(Name = "AmountOfProducts", ResourceType = typeof(ModelFormsResources))]
        public long AmountOfProducts { get; set; }

        [Display(Name = "AmountOfProductsTypes", ResourceType = typeof(ModelFormsResources))]
        public long AmountOfProductsTypes { get; set; }

        [Display(Name = "Deposits", ResourceType = typeof(ModelFormsResources))]
        public long Deposits { get; set; }
        [Display(Name = "Sectors", ResourceType = typeof(ModelFormsResources))]
        public long Sectors { get; set; }
        [Display(Name = "Halls", ResourceType = typeof(ModelFormsResources))]
        public long Halls { get; set; }
        [Display(Name = "Racks", ResourceType = typeof(ModelFormsResources))]
        public long Racks { get; set; }
        [Display(Name = "Shelfs", ResourceType = typeof(ModelFormsResources))]
        public long Shelfs { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "AverageOfSpaceCovered", ResourceType = typeof(ModelFormsResources))]
        public decimal AverageOfSpaceCovered { get; set; }
  
        public string tableID { get; set; }

        [Display(Name = "City", ResourceType = typeof(ModelFormsResources))]
        public string DistributionCenterCity { get; set; }

        [Display(Name = "Country", ResourceType = typeof(ModelFormsResources))]
        public string DistributionCenterCountry { get; set; }

        [Display(Name = "Type", ResourceType = typeof(ModelFormsResources))]
        public string DistributionCenterCtype { get; set; }

    }



}

