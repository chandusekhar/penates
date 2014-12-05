using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penates.Utils;

namespace Penates.Tests.Utils {
    [TestClass]
    public class EmailTest {
        
        [TestMethod]
        public void Send() {
            Mail mail = new Mail();
            mail.send("Test", "Hola");
        }
    }
}
