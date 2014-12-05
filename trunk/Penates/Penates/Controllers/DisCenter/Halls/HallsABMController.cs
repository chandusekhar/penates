using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.DC;
using Penates.Services;
using Penates.Services.DC;
using Penates.Utils.Enums;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.DisCenter.Halls
{
    public class HallsABMController : Controller
    {
        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult Index(ABMViewModel model, long? DepositID, long? SectorID) {
            if (model == null) {
                model = new ABMViewModel();
            }
            if (DepositID.HasValue) {
                ViewBag.DepositID = DepositID;
                IDepositService serviceRepo = new DepositService();
                InternalDistributionCenter dc = serviceRepo.getDistributionCenter(DepositID.Value);
                if (dc != null) {
                    model.SelectedValue = dc.DistributionCenterID;
                }
                ViewBag.DepositFilter = serviceRepo.getDepositDescription(DepositID.Value);
            }
            if (SectorID.HasValue) {
                ISectorService sectorService = new SectorService();
                SectorViewModel sector = sectorService.getSectorData(SectorID.Value);
                if (!DepositID.HasValue) {
                    if (sector.DistributionCenter.HasValue) {
                        model.SelectedValue = sector.DistributionCenter.Value;
                    }
                    ViewBag.DepositFilter = sector.DepositID;
                }
                ViewBag.SectorID = SectorID;
                ViewBag.SectorFilter = sector.Description;
            }
            return View("~/Views/DistributionCenter/Halls/HallsABM.cshtml", model);
        }

        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult GetTable(jQueryDataTableParamModel param, long? DistributionCenterID, long? DepositID, long? SectorID) {
            try {
                IHallService service = new HallService();
                IQueryable<Hall> query = new List<Hall>().AsQueryable();
                if (DistributionCenterID.HasValue || DepositID.HasValue || SectorID.HasValue) {
                    query = service.getData();
                    query = service.filterByDistributionCenter(query, DistributionCenterID);
                    query = service.filterByDeposit(query, DepositID);
                    query = service.filterBySector(query, SectorID);
                }
                List<HallTableJson> result = service.toJsonArray(query);
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
        public ActionResult Delete(long HallID) {
            IHallService service = new HallService();
            try {
                bool result = service.Delete(HallID);
                if (result) {
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.HallWArt, HallID)
                    }, JsonRequestBehavior.AllowGet);
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                    Response.Write(String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.HallWArt, HallID));
                    return null;
                }
            } catch (DatabaseException ex) {
                Response.StatusCode = 511;
                Response.StatusDescription = ex.title;
                if (String.IsNullOrEmpty(ex.Message)) {
                    Response.Write("Error");
                } else {
                    Response.Write(ex.Message);
                }
                return null;
            } catch (Exception e) {
                Response.StatusCode = 511;
                Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                Response.Write(e.Message);
                return null;
            }
        }

        [RoleValidation(RoleType.All)]
        public JsonResult Autocomplete(string term, long? DistributionCenterID, long? DepositID, long? SectorID) {
            try {
                IHallService service = new HallService();
                var items = service.getAutocomplete(term, DistributionCenterID, DepositID, SectorID);
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