using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Services;
using Penates.Services.ABMs;
using Penates.Utils;
using Penates.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.ABMs
{
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common, RoleType.Executive)]
    public class SupplierABMController : Controller
    {
        public ActionResult Index(ABMViewModel model) {
            return View("~/Views/ABMs/SupplierABM.cshtml",model);
        }

        public ActionResult ABMAjax(jQueryDataTableParamModel param) {
            try {
                ISupplierService service = new SupplierService();
                List<List<string>> result;
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortDirection = Request["sSortDir_0"];
                long total = 0; //El total sin paginacion
                if (!string.IsNullOrEmpty(param.sSearch)) {
                    result = service.getDisplayData(param.sSearch, param.iDisplayStart, param.iDisplayLength, sortColumnIndex, sortDirection, ref total);
                } else {
                    result = service.getDisplayData(param.iDisplayStart, param.iDisplayLength, sortColumnIndex, sortDirection, ref total);
                }
                long count = result.Count();
                return Json(new {
                    sEcho = param.sEcho,
                    iTotalRecords = count,
                    iTotalDisplayRecords = total,
                    aaData = result
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

        /// <summary>Elimina el proveedor con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult Delete(long? SupplierID) {
            ISupplierService supplierService = new SupplierService();
            try {
                if (SupplierID.HasValue) {
                        try {
                            bool result = supplierService.Delete(SupplierID.Value);
                            ABMViewModel model = new ABMViewModel();
                            if (result) {
                                return Json(new {
                                    title = Resources.Messages.OperationSuccessfull,
                                    message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.SupplierWArt, SupplierID)
                                }, JsonRequestBehavior.AllowGet);
                            } else {
                                Response.StatusCode = 511;
                                Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                                Response.Write(String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.SupplierWArt, SupplierID));
                                return null;
                            }

                        } catch (DeleteConstrainException) {
                            Response.StatusCode = 601;
                            List<ConstraintViewModel> models = supplierService.getConstrains(SupplierID.Value);
                            return View("~/Views/Error/ConstraintsDisplay.cshtml", models);
                        }
                }else{
                        Response.StatusCode = 511;
                        Response.StatusDescription = String.Format(Resources.ExceptionMessages.DeleteExceptionNoID, Resources.Resources.SupplierWArt);
                        Response.Write(String.Format(Resources.Errors.DeleteIDNull, Resources.Resources.SupplierWArt));
                        return null;
                }
            } catch (DatabaseException ex) {
                Response.StatusCode = 511;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            } catch (Exception e) {
                Response.StatusCode = 511;
                Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                Response.Write(e.Message);
                return null;
            }
        }
	}
}