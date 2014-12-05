using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penates;
using Penates.Controllers;
using Moq;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.Users;
using Penates.Services;
using Penates.Services.Users;
using System.Web;
using System.Security.Principal;
using Penates.Interfaces.Services;
using Penates.Interfaces.Models;

namespace Penates.Tests.Controllers {
    [TestClass]
    public class HomeControllerTest {

        [TestMethod]
        public void Index()
        {
            var UserServiceMock = new Mock<IUserService>();
            var fakeHttpContext = new Mock<HttpContextBase>();

            var fakeIdentity = new GenericIdentity("User");
            var principal = new GenericPrincipal(fakeIdentity, null);

            fakeHttpContext.Setup(t => t.User).Returns(principal);

            var controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(t => t.HttpContext).Returns(fakeHttpContext.Object);

            UserServiceMock.Setup(x => x.validateUser(It.IsAny<string>())).Returns(true);

            var target = new HomeController(UserServiceMock.Object);

            //Set your controller ControllerContext with fake context
            target.ControllerContext = controllerContext.Object;
                        
            ViewResult result = target.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About() {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Página de descripción de la aplicación.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact() {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }        

        [TestMethod]
        public void LoginOK()
        {
            const string expectedViewName = "HomeDashboards";

            var model = new Login() { 
                Username = "Admin",
                Password = "test"
            };
            var userServiceMock = new Mock<IUserService>();
            var fakesFormsAuth = new Mock<IFormsAuthenticationService>();
            var cookieMock = new Mock<ICookieUser>();
            
            userServiceMock.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            fakesFormsAuth.Setup(x => x.SetAuthCookie(It.IsAny<string>(), It.IsAny<bool>()));
            
            var controller = new HomeController(userServiceMock.Object, fakesFormsAuth.Object, cookieMock.Object);

            var result = controller.Index(model) as RedirectToRouteResult;

            fakesFormsAuth.Verify(x => x.SetAuthCookie(It.IsAny<string>(), true), Times.AtLeastOnce());
            cookieMock.Verify(x => x.ActualizarValor(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce());

            Assert.AreEqual(expectedViewName, result.RouteValues["action"]);
        }

        [TestMethod]
        public void LoginFailed()
        {
            const string expectedViewName = "Index";

            var model = new Login();
            var userServiceMock = new Mock<IUserService>();
            var fakesFormsAuth = new Mock<IFormsAuthenticationService>();
            var cookieMock = new Mock<ICookieUser>();

            userServiceMock.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var controller = new HomeController(userServiceMock.Object, fakesFormsAuth.Object, cookieMock.Object);

            ViewResult result = controller.Index(model) as ViewResult;

            fakesFormsAuth.Verify(x => x.SetAuthCookie(It.IsAny<string>(), true), Times.Never());
            cookieMock.Verify(x => x.ActualizarValor(It.IsAny<string>(), It.IsAny<string>()), Times.Never());

            Assert.IsNotNull(result);
        }
    }
}
