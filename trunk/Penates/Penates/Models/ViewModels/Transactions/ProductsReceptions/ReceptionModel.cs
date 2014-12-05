using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.Transactions.ProductsReceptions
{
    public class ReceptionModel
    {
        public DateTime date {set; get;}

        public string COT { set; get; }

        public string IDSupplierOrder { set; get; }

        public long IDSupplier { set; get; }

        public long IDDistribucionCenter { set; get; }
    
        public bool IsPurchase { set; get; }
    }
}