using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penates.Models.ViewModels.Forms;
using Penates.Tests.Utils;
using Penates.Controllers;
using Penates.Services.ABMs;
using Penates.Models.ViewModels.Transactions;
using Penates.Repositories.ABMs;
using Penates.Repositories.Transactions;
using NUnit.Framework;

namespace Penates.Tests.Controllers.Forms {
    [TestClass]
    public class OrdersABMControllerTest {
        
        [TestMethod]
        [TestCase()]
        public void Save() {
            OrderViewModel model = new OrderViewModel();
            model.OrderID = "1";
            model.SupplierID = 2;
            OrderRepository repo = new OrderRepository();
            repo.Save(model);
        }

        [TestMethod]
        public void Cancel() {
           OrderRepository repo = new OrderRepository();
            repo.Cancel("1", 1);
        }
    }
}
