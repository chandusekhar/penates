using DataAnnotationsExtensions;
using Penates.App_GlobalResources.Forms;
using Penates.Utils.JSON;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.Users {
    public class ValSecurityViewModel {

        public ValSecurityViewModel() {
            this.HasMaxOrderTotal = false;
            this.HasMaxProductPrice = false;
            this.HasMinOrderTotal = false;
            this.HasMinProductPrice = false;
        }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [StringLength(256, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "FileNumber", ResourceType = typeof(ModelFormsResources))]
        public string FileNumber { get; set; }

        [Display(Name = "Username", ResourceType = typeof(ModelFormsResources))]
        public string Username { get; set; }

        [Display(Name = "HasMaxProductPrice", ResourceType = typeof(ModelFormsResources))]
        public bool HasMaxProductPrice { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, 1E20, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DataType(DataType.Currency, ErrorMessageResourceName = "SellerPriceTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "MaxProductPrice", ResourceType = typeof(ModelFormsResources))]
        public decimal? MaxProductPrice { get; set; }

        [Display(Name = "HasMinProductPrice", ResourceType = typeof(ModelFormsResources))]
        public bool HasMinProductPrice { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, 1E20, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DataType(DataType.Currency, ErrorMessageResourceName = "SellerPriceTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "MinProductPrice", ResourceType = typeof(ModelFormsResources))]
        public decimal? MinProductPrice { get; set; }

        [Display(Name = "HasMaxOrderTotal", ResourceType = typeof(ModelFormsResources))]
        public bool HasMaxOrderTotal { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, 1E20, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DataType(DataType.Currency, ErrorMessageResourceName = "SellerPriceTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "MaxOrderTotal", ResourceType = typeof(ModelFormsResources))]
        public decimal? MaxOrderTotal { get; set; }

        [Display(Name = "HasMinOrderTotal", ResourceType = typeof(ModelFormsResources))]
        public bool HasMinOrderTotal { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, 1E20, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DataType(DataType.Currency, ErrorMessageResourceName = "SellerPriceTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "MinOrderTotal", ResourceType = typeof(ModelFormsResources))]
        public decimal? MinOrderTotal { get; set; }

        [Display(Name = "Categories", ResourceType = typeof(ModelFormsResources))]
        public string Categories { get; set; }

        public IEnumerable<SelectItem> initialCategories { get; set; }

        [Display(Name = "Error", ResourceType = typeof(ModelFormsResources))]
        public string error { get; set; }
    }
}