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
    public partial class OrderItemsViewModel : IFormViewModel, IModalViewModel {

        public OrderItemsViewModel(): this("OrderItems") {
        }

        public OrderItemsViewModel(string id) {
            this.ViewId = id;
        }

        public OrderItemsViewModel(SupplierOrderItem item)
            : this(item, "OrderItems") {
        }

        public OrderItemsViewModel(SupplierOrderItem item, string id) {
            this.OrderID = item.IDSupplierOrder;
            this.SupplierID = item.IDSupplier;
            this.ProductID = item.IDProduct;
            this.ProductName = item.ProvidedBy.Product.Name;
            this.SupplierProductID = item.SupplierProductID;
            this.Boxes = item.ItemBoxes;
        }


        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "OrderID", ResourceType = typeof(ModelFormsResources))]
        public string OrderID { get; set; }


        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "SupplierID", ResourceType = typeof(ModelFormsResources))]
        public long SupplierID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "ProductID", ResourceType = typeof(ModelFormsResources))]
        public long ProductID { get; set; }

        [Display(Name = "ProductName", ResourceType = typeof(ModelFormsResources))]
        public string ProductName { get; set; }

        [StringLength(128, ErrorMessageResourceName = "StringLenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DataType(DataType.Text, ErrorMessageResourceName = "TextTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "SuppliersProductCode", ResourceType = typeof(ModelFormsResources))]
        public string SupplierProductID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "BoxesQty", ResourceType = typeof(ModelFormsResources))]
        public long Boxes { get; set; }

        public bool edit { get; set; }

        public bool isAjax { get; set; }

        public string ViewId { get; set; }

        public string error { get; set; }

        public string concatWithID(string s) {
            return s + this.ViewId;
        }

        public string getJqueryID(string s) {
            return "#" + s + this.ViewId;
        }

        }
}