using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penates.Interfaces.Repositories;
using Penates.Repositories.ABMs;
using System.Linq;
using Penates.Database;
using System.Collections.Generic;

namespace Penates.Tests.Repositories.ABMs {
    [TestClass]
    public class BoxRepositoryTest {
        [TestMethod]
        public void filterByTemporaryDeposit() {
            IBoxRepository repo = new BoxRepository();
            IQueryable<Box> query = repo.getData();
            query = repo.filterByTemporaryDeposit(query, 1);

            List<Box> list = query.ToList();

            foreach(Box box in list){
                System.Console.WriteLine(box.BoxID);
            }
        }
    }
}
