using Penates.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Models {
    public class TableAjaxViewModel: ITableViewModel {

        public TableAjaxViewModel() : this("table"){
        }

        public TableAjaxViewModel(string id) {
            this.ServerProcessing = true;
            this.RefreshFunction = "refreshTable"+id;
            this.tableID = id;
            this.ToggleDeleteMessage = false;
        }
        
        public string AjaxRequest { get; set; }

        public string DeleteController { get; set; }

        public bool ToggleDeleteMessage { get; set; }

        public string DeleteAction { get; set; }

        public string DeleteText { get; set; }

        public string DeleteConfirmMessage { get; set; }

        public string DeleteText2 { get; set; }

        public string DeleteConfirmMessage2 { get; set; }

        public bool ServerProcessing { get; set; }

        public string RefreshFunction { get; set; }

        public string tableID { get; set; }

        public Object Params { get; set; }

        public string EditController { get; set; }

        public string EditAction { get; set; }

        public bool useJQueryEditFunction { get; set; }

        public string JQueryEditFunction { get; set; }

        public bool useDefault { get; set; }

        public string concatWithID(string s) {
            return s + this.tableID;
        }
    }
}