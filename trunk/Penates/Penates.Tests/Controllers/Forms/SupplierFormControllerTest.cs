using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penates.Models.ViewModels.Forms;
using Penates.Tests.Utils;
using Penates.Controllers;
using Penates.Services.ABMs;

namespace Penates.Tests.Controllers.Forms {
    [TestClass]
    public class SupplierFormControllerTest {
        //[TestMethod]
        //public void DatabaseLoad() {
        //    SupplierViewModel model = new SupplierViewModel();
        //    Random rand;
        //    SupplierService service;
        //    string aux;
        //    for (int i = 0; i <= 90006; i++) {
        //        rand = new Random((int) DateTime.Now.Ticks);
        //        aux = RandomStringGenerator.RandomString(rand.Next(4,50));
        //        model.Name = aux;
        //        model.Address = aux + "Street";
        //        if (rand.NextDouble() > 0.5) {
        //            model.Email = "info@" + aux + ".com";
        //        }else{
        //            model.Email = "empresas@" + aux + ".com";
        //        }
        //        model.Phone = RandomStringGenerator.RandomNumericString(4) + "-" + RandomStringGenerator.RandomNumericString(4);
        //        service = new SupplierService();
        //        Console.WriteLine(service.Save(model));

        //        System.Threading.Thread.Sleep(10);
        //    }
        //}
    }
}
