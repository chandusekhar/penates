using Penates.Exceptions.Database;
using Penates.Models.ViewModels.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Models.ViewModels.ABMs {
    public class ABMViewModel {

        public ABMViewModel() {
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