using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Penates.App_GlobalResources.Forms;
using Penates.Database;
using Penates.Utils;
using System.Drawing;
using System.Web.Helpers;
using Penates.Services.ABMs;
using Penates.Models.Validators;
using System.Web.Mvc;
using Penates.Interfaces.Models;

namespace Penates.Models.ViewModels.Transactions {
    public partial class OrderViewModel : IFormViewModel {

        public OrderViewModel():this("") {

        }

        public OrderViewModel(string id) {
            this.viewID = id;
        }

        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "OrderID", ResourceType = typeof(ModelFormsResources))]
        public string OrderID { get; set; }

        [DataType(DataType.Text)]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "OrderID", ResourceType = typeof(ModelFormsResources))]
        public string OldOrderID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, long.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "SupplierID", ResourceType = typeof(ModelFormsResources))]
        public long? SupplierID { get; set; }

        [Range(0, long.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "SupplierID", ResourceType = typeof(ModelFormsResources))]
        public long? OldSupplierID { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "SupplierName", ResourceType = typeof(ModelFormsResources))]
        public string SupplierName { get; set; }

        [DataType(DataType.DateTime, ErrorMessageResourceName = "DateTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "OrderDate", ResourceType = typeof(ModelFormsResources))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "CanceledOrder", ResourceType = typeof(ModelFormsResources))]
        public bool Canceled { get; set; }

        [Display(Name = "Received", ResourceType = typeof(ModelFormsResources))]
        public bool Received { get; set; }

        [Display(Name = "TotalPrice", ResourceType = typeof(ModelFormsResources))]
        public decimal Total { get; set; }

        public string Error { get; set; }

        private string viewID { get; set; }

        public string concatWithID(string s) {
            return s + this.viewID;
        }

        public string getJqueryID(string s) {
            return "#" + this.concatWithID(s);
        }
    }
}