using Penates.App_GlobalResources.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.Transactions.ProductsReceptions
{
    public class VerifyOrderModel
    {

        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "OrderID", ResourceType = typeof(ModelFormsResources))]
        public string OrderID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, long.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "SupplierID", ResourceType = typeof(ModelFormsResources))]
        public long? SupplierID { get; set; }

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

        public bool Error { get; set; }

        public string Message { get; set; }

        public string TableID { get; set; }

        public string concatWithId(string s)
        {
            return s + this.TableID;
        }

    }
}