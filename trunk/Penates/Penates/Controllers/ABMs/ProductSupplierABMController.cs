using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.Forms;
using Penates.Services;
using Penates.Services.ABMs;
using Penates.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.ABMs
{
    public class ProductSupplierABMController : Controller
    {
        ProductService ps = new ProductService();
        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult Index(SupplierABMViewModel m, long? productID) {
            if (m == null) {
                m = new SupplierABMViewModel();
            }
            if (productID.HasValue) {
                return RedirectToAction("ShowFromProduct", new { model = m, productID = productID });
            }
            return RedirectToAction("ShowFromProduct", new { model = m, productID = 1 });
        }

        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult ShowFromProduct(SupplierABMViewModel model, long? productID) {
            //http://www.codeproject.com/Articles/155422/jQuery-DataTables-and-ASP-NET-MVC-Integration-Part#BackgroundSSP
            try {
                if (model == null) {
                    model = new SupplierABMViewModel();
                }
                model.ProductID = productID;
                return View("~/Views/ABMs/ProductSupplierABM.cshtml",model);
            } catch (DatabaseException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            } catch (Exception e) {
                ErrorModel errorModel = new ErrorModel("Error", e, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }

        [RoleValidation(RoleType.All)]
        public ActionResult SupplierLoad(jQueryDataTableParamModel param, long? ID) {
            try {
                IEnumerable<List<string>> supplier;
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortDirection = Request["sSortDir_0"];
                if (!string.IsNullOrEmpty(param.sSearch)) {
                    supplier = this.ps.getSuppliers(ID).Select(s => new List<string>() { s.SupplierID.ToString(), s.Name, s.Address, s.Email });
                } else {
                    supplier = this.ps.getSuppliers(ID).Select(s => new List<string>() { s.SupplierID.ToString(), s.Name, s.Address, s.Email });
                }
                long count = supplier.Count();
                return Json(new {
                    sEcho = param.sEcho,
                    //iTotalRecords = count,
                    //iTotalDisplayRecords = total,
                    aaData = supplier
                },
                            JsonRequestBehavior.AllowGet);
            } catch (DatabaseException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            } catch (Exception e) {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }

        public JsonResult Autocomplete(string term, long? productID) {
            try{
                ISupplierService service = new SupplierService();
                var items = service.getAutocomplete(term, productID);
                var json = service.toJsonAutocomplete(items);
                return Json(json, JsonRequestBehavior.AllowGet);
            } catch (DatabaseException ex) {
                Response.StatusCode = 504;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            } catch (Exception e) {
                Response.StatusCode = 504;
                Response.StatusDescription = Resources.Errors.DatabaseError;
                Response.Write(e.Message);
                return null;
            }
        }
	}
}