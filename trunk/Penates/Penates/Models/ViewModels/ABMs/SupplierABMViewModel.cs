using Penates.Exceptions.Database;
using Penates.Models.ViewModels.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Models.ViewModels.ABMs {
    public class SupplierABMViewModel : ABMViewModel{

        public SupplierABMViewModel() {

        }

        public long? ProductID { get; set; }

        public string ProductName { get; set; }
    }
}