using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace Penates.Models.ViewModels.Inventory
{
    public class CreateInventoryViewModel {

        public CreateInventoryViewModel()
        {
            this.tableID = "table";
            this.tableRefresh = "refreshTable";
            this.tableDelete = "DeleteEvent";
            this.Error = false;
        }

        public string Message { get; set; }

        public bool? Error { get; set; }

        [Required]
        public long? SelectedValue { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string InventoryName { get; set; }

        public IEnumerable<SelectListItem> List { get; set; }
         
        public string AjaxRequest { get; set; }

        public long filterID { get; set; }

        public string tableID { get; set; }

        public string tableRefresh { get; set; }

        public string tableDelete { get; set; }
    }



}

