using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penates.Interfaces.Services;
using Penates.Services.Security;
using Penates.Models.ViewModels.Users;
using Penates.Utils;

namespace Penates.Tests.Services.Security {
    [TestClass]
    public class SecurityServiceTest {
        [TestMethod]
        public void getParameters() {
            ISecurityService service = new SecurityService();
            SecurityParametersViewModel model = service.getParameters();

            Assert.AreNotEqual(null, model);
        }
    }
}
