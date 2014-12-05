using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.Transactions.ProductsReceptions
{
    public class ContainersAndLocationAssignationModel
    {
        public ContainersAndLocationAssignationModel()
        {
            this.Error = false;
            this.Message = "";
            this.productsAssignations = new List<ProductAssignation>();
        }

        public bool? Error { get; set; }

        public string Message { get; set; }

        public List<ProductAssignation> productsAssignations { get; set; }

        
    }
}