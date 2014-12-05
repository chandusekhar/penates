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

namespace Penates.Models.ViewModels.Forms {
    public partial class ProvidedByViewModel : IFormViewModel, IModalViewModel {

        public ProvidedByViewModel(): this("ProvidedBy") {
        }

        public ProvidedByViewModel(string id) {
            this.ViewId = id;
            this.Width = 10;
            this.Height = 10;
            this.Depth = 10;
            this.Size = 1000;
        }

        public ProvidedByViewModel(ProvidedBy prov)
            : this(prov, "ProvidedBy") {
        }

        public ProvidedByViewModel(ProvidedBy prov, string id){
            this.ProductID = prov.Product.ProductID;
            this.ProductName = prov.Product.Name;
            this.PurchasePrice = prov.PurchasePrice;
            this.SupplierID = prov.Supplier.SupplierID;
            this.SupplierName = prov.Supplier.Name;
            this.Barcode = prov.Product.Barcode;
            this.ItemsPerBox = prov.ItemsPerBox;
            this.Depth = prov.BoxDepth;
            this.Height = prov.BoxHeight;
            this.Width = prov.BoxWidth;
            this.Size = this.Depth * this.Width * this.Height;
            if (prov.Inactive) {
                this.edit = false;
            } else {
                this.edit = true;
            }
        }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Display(Name = "SupplierID", ResourceType = typeof(ModelFormsResources))]
            public long SupplierID { get; set; }

            [Display(Name = "SupplierName", ResourceType = typeof(ModelFormsResources))]
            public string SupplierName { get; set; }

            [Display(Name = "ProductName", ResourceType = typeof(ModelFormsResources))]
            public string ProductName { get; set; }

            [Display(Name = "Barcode", ResourceType = typeof(ModelFormsResources))]
            public string Barcode { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Display(Name = "ProductID", ResourceType = typeof(ModelFormsResources))]
            public long ProductID { get; set; }

            [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Range(0, 1E20, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [DataType(DataType.Currency, ErrorMessageResourceName = "CurrencyTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Display(Name = "PurchasePrice", ResourceType = typeof(ModelFormsResources))]
            public decimal PurchasePrice { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Integer(ErrorMessageResourceName = "IntegerError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Range(0, Int32.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Display(Name = "ItemsPerBox", ResourceType = typeof(ModelFormsResources))]
            public int ItemsPerBox { get; set; }

            [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Range(0, 1E26, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [GraterThan(0, ErrorMessageResourceName = "GraterThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
            [Display(Name = "BoxDepth", ResourceType = typeof(ModelFormsResources))]
            public decimal Depth { get; set; }

            [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Range(0, 1E26, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [GraterThan(0, ErrorMessageResourceName = "GraterThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
            [Display(Name = "BoxWidth", ResourceType = typeof(ModelFormsResources))]
            public decimal Width { get; set; }

            [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Range(0, 1E26, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [GraterThan(0, ErrorMessageResourceName = "GraterThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
            [Display(Name = "BoxHeight", ResourceType = typeof(ModelFormsResources))]
            public decimal Height { get; set; }

            [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [DisplayFormat(DataFormatString = "{0:F2}")]
            [Display(Name = "BoxSize", ResourceType = typeof(ModelFormsResources))]
            public decimal? Size { get; set; }

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