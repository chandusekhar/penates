using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.DC;
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

namespace Penates.Controllers.DisCenter.Containers
{
    public class ContainersABMController : Controller
    {
        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult Index(ABMViewModel model, long? RackID, long? TemporaryDepositID, long? ContainerTypeID) {
            if (model == null) {
                model = new ABMViewModel();
            }
            if (RackID.HasValue) {
                IRackService rackService = new RackService();
                RackViewModel rack = rackService.getRackData(RackID.Value);
                ViewBag.RackID = RackID;
                ViewBag.RackFilter = rack.RackCode;
                ViewBag.SectorFilter = rack.SectorName;
                ViewBag.SectorID = rack.SectorID;
                ViewBag.DepositFilter = rack.DepositName;
                ViewBag.DepositID = rack.DepositID;
                model.SelectedValue = rack.DistributionCenter;
            }
            if (TemporaryDepositID.HasValue) {
                ITemporaryDepositService depositService = new TemporaryDepositService();
                DepositViewModel tempDeposit = depositService.getDepositData(TemporaryDepositID.Value);
                ViewBag.TemporaryDepositFilter = tempDeposit.Description;
                ViewBag.TemporaryDepositID = TemporaryDepositID.Value;
                model.SelectedValue = tempDeposit.DistributionCenterID;
            }
            IContainerTypeService typeService = new ContainerTypeService();
            if (ContainerTypeID.HasValue) {
                model.List = typeService.getTypeList(true, ContainerTypeID.Value);
                model.filterID = ContainerTypeID.Value;
            } else {
                model.List = typeService.getTypeList(true);
                model.filterID = -1;
            }
            return View("~/Views/DistributionCenter/Containers/ContainersABM.cshtml", model);
        }

        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult GetTable(jQueryDataTableParamModel param, long? DistributionCenterID, long? TemporaryDepositID, long? DepositID, long? SectorID, long? RackID, long? ContainerTypeID) {
            try {
                IContainerService service = new ContainerService();
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortDirection = Request["sSortDir_0"];
                IQueryable<Container> query = service.getData();
                if (!string.IsNullOrEmpty(param.sSearch)) {
                    query = service.search(query, param.sSearch);
                }
                query = service.filterByDistributionCenter(query, DistributionCenterID);
                query = service.filterByTemporaryDeposit(query, TemporaryDepositID);
                query = service.filterByDeposit(query, DepositID);
                query = service.filterBySector(query, SectorID);
                query = service.filterByRack(query, RackID);
                query = service.filterByType(query, ContainerTypeID);
                query = service.sort(query, sortColumnIndex, sortDirection);
                long total = query.Count(); //El total sin paginacion
                Paginator<Container> pag = new Paginator<Container>(query);
                query = pag.page(param.iDisplayStart, param.iDisplayLength);
                List<ContainerTableJson> result = service.toJsonArray(query);
                long count = result.Count();
                return Json(new {
                    sEcho = param.sEcho,
                    iTotalRecords = count,
                    iTotalDisplayRecords = total,
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
        public ActionResult Delete(long ContainerID) {
            IContainerService service = new ContainerService();
            try {
                bool result = service.Delete(ContainerID);
                if (result) {
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.ContainerWArt, ContainerID)
                    }, JsonRequestBehavior.AllowGet);
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                    Response.Write(String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.ContainerWArt, ContainerID));
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
        public JsonResult Autocomplete(string term, long? DistributionCenterID, long? DepositID, long? SectorID, long? RackID, long? TemporaryDepositID, long? ContainerTypeID) {
            try {
                IContainerService service = new ContainerService();
                var items = service.getAutocomplete(term, DistributionCenterID, DepositID, TemporaryDepositID, SectorID, RackID, ContainerTypeID);
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