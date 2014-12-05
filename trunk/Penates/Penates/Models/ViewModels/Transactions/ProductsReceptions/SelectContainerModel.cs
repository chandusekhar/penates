using Penates.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.Transactions.ProductsReceptions
{
    public class SelectContainerModel
    {
        public SelectContainerModel()
        {
            this.Error = false;
            this.Message = "";
            this.ProductBoxes = new List<SupplierOrderItem>();
        }

      
        public bool? Error { get; set; }

        public string Message { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        public long? SupplierID { get; set; }
        
        public long? SelectedSupplier { get; set; }

        public string OrderID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        public long? SelectedOrder { get; set; }

        public long DistributionCenterID { get; set; }

        public List<SupplierOrderItem> ProductBoxes { get; set; }

        public string ContainerType { get; set; }

        public List<long> Containers { get; set; }

        public List<string> ContainersNames { get; set; }

        public string COT { get; set; }

        public bool IsPurchase { get; set; }

        public List<long> PackID { get; set; }

        public List<bool> Transitory { get; set; }

        public List<string> PackSerialNumber { get; set; }

        public bool LeaveInTemporaryDeposit { get; set; }
    }
}