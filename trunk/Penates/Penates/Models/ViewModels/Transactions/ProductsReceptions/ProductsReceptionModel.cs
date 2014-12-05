using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.Transactions.ProductsReceptions
{
    public class ProductsReceptionModel
    {
        public ProductsReceptionModel()
        {

        }

        public string tableRefresh { get; set; }

        public bool? Error { get; set; }

        public string Message { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        public long? SupplierID { get; set; }
        
        public string SelectedSupplier { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        public string OrderID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        public long? SelectedOrder { get; set; }

    }
}