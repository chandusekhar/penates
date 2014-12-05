using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penates.Controllers;
using System.Web.Mvc;
using Penates.Models.ViewModels.Users;
using Penates.Controllers.Users;
//using Moq;

namespace Penates.Tests.Controllers
{
    [TestClass]
    public class UsersControllerTest
    {
        //[TestInitialize]
        //public void Init()
        //{
        //    RepositoryMock = new Mock<IRepository>();

        //    target = new UsersController(RepositoryMock.Object);
        //}

        [TestMethod]
        public void RegisterOk()
        {
            UserController controller = new UserController();
            ViewResult result = controller.Register("") as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RegisterUserWithInvalidModelState()
        {
            UserController controller = new UserController();
            
            controller.ModelState.AddModelError("test", "test");

            ActionResult result = controller.Register(new RegisterViewModel());
        }

        [TestMethod]
        public void RegisterUserWithValidModelState()
        {
            UserController controller = new UserController();

            controller.ModelState.Clear();

            ActionResult result = controller.Register(new RegisterViewModel());
        }

        //[TestMethod]
        //public void RegisterUser()
        //{
        //    var repository = new Mock<IRepository>();
        //    UsersController controller = new UsersController(repository.Object);

        //    // Assert      
        //    Lead actual = (Lead)result.ViewData.Model;
        //    // All fields should be equal, not only name
        //    Assert.That(actual, Is.EqualTo(expected));
        //    Assert.AreEqual("Success", result.ViewBag.Message);
        //    // You need to be sure, that expected lead object passed to repository
        //    repository.Verify(r => r.InsertLead(expected));
        //    repository.Verify(r => r.Save());
        //}

        //[TestMethod]
        //public void RegisterWithRepositoryOk()
        //{
        //    //arrange
        //    // There is a setup of 'GetAllEffectiveProductDetails'
        //    // When 'GetAllEffectiveProductDetails' method is invoked 'expectedallProducts' collection is exposed.
        //    var expectedallProducts = new List<ProductDetailDto> { new ProductDetailDto() };
        //    productServiceMock
        //        .Setup(it => it.GetAllEffectiveProductDetails())
        //        .Returns(expectedallProducts);

        //    //act
        //    var result = target.Products();

        //    //assert
        //    var model = (result as ViewResult).Model as ProductModels.ProductCategoryListModel;
        //    Assert.AreEqual(model.ProductDetails, expectedallProducts);
        //    /* Any other assertions */
        //}
    }
}
