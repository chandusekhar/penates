using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace Penates.Models.ViewModels.Reports
{
    public class DistributionCenterReportViewModel {
                 
        public DistributionCenterReportViewModel(){
            this.tableID = "table";
            this.tableRefresh = "refreshTable";
            this.tableDelete = "DeleteEvent";
            this.Error = false;
        }

        public string Message { get; set; }

        public bool? Error { get; set; }

        public long? SelectedValue { get; set; }

        public IEnumerable<SelectListItem> List { get; set; }
         
        public string AjaxRequest { get; set; }

        public long filterID { get; set; }

        public string tableID { get; set; }

        public string tableRefresh { get; set; }

        public string tableDelete { get; set; }
    }



}

