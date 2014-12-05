using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Services;
using Penates.Services.ABMs;
using Penates.Utils;
using Penates.Utils.Enums;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.ABMs
{
    public class BoxesABMController : Controller
    {
        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult Index(ABMViewModel model, long? DistributionCenterID, long? DepositID) {
            if (model == null) {
                model = new ABMViewModel();
            }
            if(DistributionCenterID.HasValue){
                model.SelectedValue = DistributionCenterID.Value;
            }
            if(DepositID.HasValue){
                ViewBag.DepositFilter = DepositID.Value;
            }
            return View("~/Views/ABMS/BoxesABM.cshtml", model);
        }

        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult GetTable(jQueryDataTableParamModel param, long? DistributionCenterID, long? DepositID, long? SectorID, long? RackID, long? PackID, long? StatusID, long? ProductID) {
            try {
                IBoxService service = new BoxService();
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortDirection = Request["sSortDir_0"];
                IQueryable<Box> query = service.getData();
                if (!string.IsNullOrEmpty(param.sSearch)) {
                    query = service.search(query, param.sSearch);
                }
                query = service.filterByDistributionCenter(query, DistributionCenterID);
                if (DepositID.HasValue) {
                    IQueryable<Box> aux = query;
                    query = service.filterByDeposit(query, DepositID);
                    aux = service.filterByTemporaryDeposit(aux, DepositID);
                    query = query.Union(aux);
                }
                query = service.filterByProduct(query, ProductID);
                query = service.filterBySector(query, SectorID);
                query = service.filterByRack(query, RackID);
                query = service.filterByPack(query, PackID);
                query = service.filterByState(query, StatusID);
                query = service.sort(query, sortColumnIndex, sortDirection);
                long total = query.Count(); //El total sin paginacion
                Paginator<Box> pag = new Paginator<Box>(query);
                query = pag.page(param.iDisplayStart, param.iDisplayLength);
                List<BoxTableJson> result = service.toJsonArray(query);
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

        [RoleValidation(RoleType.All)]
        public JsonResult Autocomplete(string term, long? dcID, long? depositID, long? temporaryDepositID, long? sectorID, long? rackID) {
            try {
                IBoxService service = new BoxService();
                var items = service.getAutocomplete(term, dcID, depositID, temporaryDepositID, sectorID, rackID);
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