using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Services;
using Penates.Services.DC;
using Penates.Utils;
using Penates.Utils.Enums;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.DisCenter.Deposits
{
    public class DepositsABMController : Controller
    {
        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult Index(ABMViewModel model) {
            if (model == null) {
                model = new ABMViewModel();
            }
            return View("~/Views/DistributionCenter/Deposits/DepositsABM.cshtml", model);
        }

        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult GetTable(jQueryDataTableParamModel param, long? DistributionCenterID) {
            try {
                IDepositService service = new DepositService();
                IQueryable<Deposit> query = service.getData(DistributionCenterID);
                List<DepositTableJson> result = service.toJsonArray(query);
                return Json(new {
                    sEcho = param.sEcho,
                    aaData = result
                },
                            JsonRequestBehavior.AllowGet);
            } catch (DatabaseException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            } catch (Exception e) {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }

        /// <summary>Elimina el centro de distribucion con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult Delete(long DepositID) {
            IDepositService dcService = new DepositService();
            try {
                bool result = dcService.Delete(DepositID);
                if (result) {
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.DepositWArt, DepositID)
                    }, JsonRequestBehavior.AllowGet);
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                    Response.Write(String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.DepositWArt, DepositID));
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

        [RoleValidation(RoleType.All)]
        public JsonResult DistributionCenterAutocomplete(string term) {
            try {
                IDistributionCenterService service = new DistributionCenterService();
                var items = service.getInternalAutocomplete(term);
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

        [RoleValidation(RoleType.All)]
        public JsonResult Autocomplete(string term, long? DistributionCenterID) {
            try {
                IDepositService service = new DepositService();
                var items = service.getAutocomplete(term, DistributionCenterID);
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

        [RoleValidation(RoleType.All)]
        public JsonResult AutocompleteAll(string term, long? DistributionCenterID) {
            try {
                IDepositService service = new DepositService();
                var items = service.getAutocomplete(term, DistributionCenterID);
                var json = service.toJsonAutocomplete(items);
                ITemporaryDepositService tempService = new TemporaryDepositService();
                var tempItems = tempService.getAutocomplete(term, DistributionCenterID);
                var tempJson = tempService.toJsonAutocomplete(tempItems);
                json = ListUtils<AutocompleteItem>.interlay(json, tempJson);
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