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
using Moq;

namespace Penates.Tests.Controllers.Transactions.ProductReceptions {
    [TestClass]
    public class ProductsReceptionsControllerTest {

        public Mock<IDistributionCenterService> dcServiceMock;
        public Mock<IReceptionService> receptionServiceMock;
        public Mock<ITemporaryDepositService> tempDepositServiceMock;

        [TestInitialize]
        public void Initialize()
        {
            var dcServiceMock = new Mock<IDistributionCenterService>();
            var receptionServiceMock = new Mock<IReceptionService>();
            var tempDepositServiceMock = new Mock<ITemporaryDepositService>();
        }

        [TestMethod]
        public void LeaveInTemporaryDepositShouldGoToExternalLocationAssignation()
        {
            var dcServiceMock = new Mock<IDistributionCenterService>();
            var receptionServiceMock = new Mock<IReceptionService>();
            var tempDepositServiceMock = new Mock<ITemporaryDepositService>();

            var controller = new ProductsReceptionsController(dcServiceMock.Object, receptionServiceMock.Object, tempDepositServiceMock.Object);

            var pack = new List<long>();
            var productBoxes = new List<SupplierOrderItem>();

            dcServiceMock.Setup(x => x.getExternalData(It.IsAny<long>())).Returns(new Models.ViewModels.DC.ExternalDistributionCenterViewModel { UsableSpace = 100, Address = "ads" });
            receptionServiceMock.Setup(x => x.save(new ReceptionModel())).Returns(It.IsAny<int>());

            var result = controller.sendBoxes(productBoxes, pack, 2, "2", "1", 3, true, false) as ViewResult;

            Assert.AreEqual("ads", ((Penates.Models.ViewModels.Transactions.ProductsReceptions.ExternalLocationAssignationModel)(result.Model)).DistributionCenterDestiny);
            Assert.AreEqual(false, ((Penates.Models.ViewModels.Transactions.ProductsReceptions.ExternalLocationAssignationModel)(result.Model)).Error);
            Assert.AreEqual("~/Views/Transactions/ProductsReceptions/ExternalLocationAssignation.cshtml", result.ViewName);
        }

        [TestMethod]
        public void LeaveInTemporaryDepositShouldGoToFinishReception()
        {
            var dcServiceMock = new Mock<IDistributionCenterService>();
            var receptionServiceMock = new Mock<IReceptionService>();
            var tempDepositServiceMock = new Mock<ITemporaryDepositService>();

            var controller = new ProductsReceptionsController(dcServiceMock.Object, receptionServiceMock.Object, tempDepositServiceMock.Object);

            var pack = new List<long>();
            var productBoxes = new List<SupplierOrderItem>();

            dcServiceMock.Setup(x => x.getExternalData(It.IsAny<long>())).Returns(new Models.ViewModels.DC.ExternalDistributionCenterViewModel { UsableSpace = -1, Address = "ads" });
            receptionServiceMock.Setup(x => x.save(new ReceptionModel())).Returns(It.IsAny<int>());

            var result = controller.sendBoxes(productBoxes, pack, 2, "2", "1", 3, true, false) as ViewResult;

            Assert.AreEqual(Resources.Errors.NotEnoughSpace, ((Penates.Models.ViewModels.Transactions.ProductsReceptions.FinishReceptionModel)(result.Model)).Message);
            Assert.AreEqual(true, ((Penates.Models.ViewModels.Transactions.ProductsReceptions.FinishReceptionModel)(result.Model)).Error);
            Assert.AreEqual("~/Views/Transactions/ProductsReceptions/FinishReception.cshtml", result.ViewName);
        }

        [TestMethod]
        public void NotLeaveInTemporaryDepositShouldGoToProductsReceptionsOK()
        {
            var dcServiceMock = new Mock<IDistributionCenterService>();
            var receptionServiceMock = new Mock<IReceptionService>();
            var tempDepositServiceMock = new Mock<ITemporaryDepositService>();

            var controller = new ProductsReceptionsController(dcServiceMock.Object, receptionServiceMock.Object, tempDepositServiceMock.Object);

            var pack = new List<long>();
            var productBoxes = new List<SupplierOrderItem>();

            dcServiceMock.Setup(x => x.getInternalData(It.IsAny<long>())).Returns(new Models.ViewModels.DC.InternalDistributionCenterViewModel { Address = "ads" });
            receptionServiceMock.Setup(x => x.save(new ReceptionModel())).Returns(It.IsAny<int>());
            tempDepositServiceMock.Setup(x => x.findDepositWithSpace(It.IsAny<long>(), It.IsAny<decimal>())).Returns(new TemporaryDeposit { TemporaryDepositID = 1, Description = "test" });

            var result = controller.sendBoxes(productBoxes, pack, 2, "2", "1", 3, true, true) as RedirectToRouteResult;

            Assert.AreEqual("tomparalDepositAssignation", result.RouteValues["action"]);
        }

        [TestMethod]
        public void NotLeaveInTemporaryDepositShouldGoToProductsReceptionsError()
        {
            var dcServiceMock = new Mock<IDistributionCenterService>();
            var receptionServiceMock = new Mock<IReceptionService>();
            var tempDepositServiceMock = new Mock<ITemporaryDepositService>();

            var controller = new ProductsReceptionsController(dcServiceMock.Object, receptionServiceMock.Object, tempDepositServiceMock.Object);

            var pack = new List<long>();
            var productBoxes = new List<SupplierOrderItem>();

            dcServiceMock.Setup(x => x.getInternalData(It.IsAny<long>())).Returns(new Models.ViewModels.DC.InternalDistributionCenterViewModel { Address = "ads" });
            receptionServiceMock.Setup(x => x.save(new ReceptionModel())).Returns(It.IsAny<int>());
            tempDepositServiceMock.Setup(x => x.findDepositWithSpace(It.IsAny<long>(), It.IsAny<decimal>())).Returns(new TemporaryDeposit());

            var result = controller.sendBoxes(productBoxes, pack, 2, "2", "1", 3, true, true) as RedirectToRouteResult;

            Assert.AreEqual("tomparalDepositAssignation", result.RouteValues["action"]);
        }
    }
}
