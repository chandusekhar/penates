using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penates.Controllers.Transactions.ProductsReceptions;
using System.Web.Mvc;
using Penates.Database;
using System.Collections.Generic;
using Penates.Interfaces.Services;
using Penates.Services.Transactions;
using System.Linq;
using Penates.Models.ViewModels.Transactions.ProductsReceptions;

namespace Penates.Tests.Controllers.Transactions.ProductReceptions {
    [TestClass]
    public class ProductsReceptionsControllerTest {
        [TestMethod]
        public void SendBoxes() {
            var controller = new ProductsReceptionsController();
            IOrderService orderService = new OrderService();
            IQueryable<SupplierOrderItem> query = orderService.getOrderItemsList("2",2);
            List<SupplierOrderItem> ProductBoxes = query.ToList();
            List<long> pack = new List<long>();
            foreach(SupplierOrderItem item in ProductBoxes){
                pack.Add(1);
            }

            var result = controller.sendBoxes(ProductBoxes, pack, 2, "2", "1", 3, true, true) as ViewResult;
            var model = (TempLocationAssignationModel) result.ViewData.Model;
            Assert.AreEqual(false, model.Error);
        }
    }
}
