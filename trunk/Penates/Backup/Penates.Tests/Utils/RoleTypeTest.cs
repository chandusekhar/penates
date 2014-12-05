using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penates.Utils.Enums;

namespace Penates.Tests.Utils {
    [TestClass]
    public class RoleTypeTest {
        [TestMethod]
        public void RoleTypeTesting() {

            RoleType role = RoleType.Admin;

            Console.WriteLine(role.GetStringValue());
        }
    }
}
