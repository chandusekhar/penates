using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penates.Jobs;

namespace Penates.Tests.Jobs {
    [TestClass]
    public class UserJobTest {
        [TestMethod]
        public void ExecuteJob() {
            UserJob job = new UserJob();
            job.Execute(null);
        }
    }
}
