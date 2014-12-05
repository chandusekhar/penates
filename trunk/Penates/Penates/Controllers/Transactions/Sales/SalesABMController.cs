using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Services;
using Penates.Services.Transactions;
using Penates.Utils;
using Penates.Utils.Enums;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.Transactions.Sales
{
    public class SalesABMController : Controller
    {
        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult Index(ABMViewModel model, long? DistributionCenterID) {
            if (model == null) {
                model = new ABMViewModel();
            }
            if (DistributionCenterID.HasValue) {
                model.SelectedValue = DistributionCenterID.Value;
            }
            List<SelectListItem> anulatedList = new List<SelectListItem>();
            SelectListItem i = new SelectListItem();
            i.Text = Resources.Resources.All;
            i.Value = AnulatedTypes.ALL.getTypeNumber().ToString();
            anulatedList.Add(i);
            i = new SelectListItem();
            i.Text = Penates.App_GlobalResources.Forms.ModelFormsResources.Internal;
            i.Value = AnulatedTypes.VALID.getTypeNumber().ToString(); ;
            anulatedList.Add(i);
            i = new SelectListItem();
            i.Text = Penates.App_GlobalResources.Forms.ModelFormsResources.External;
            i.Value = AnulatedTypes.ANULATED.getTypeNumber().ToString(); ;
            anulatedList.Add(i);
            ViewBag.AnulatedList = anulatedList;
            return View("~/Views/Transactions/Sales/SalesABM.cshtml", model);
        }

        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult GetTable(jQueryDataTableParamModel param, long? DistributionCenterID, long? ClientID, int? anulated) {
            try {
                ISaleService service = new SaleService();
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortDirection = Request["sSortDir_0"];
                IQueryable<Sale> query = service.getData();
                query = service.filterByDistributionCenter(query, DistributionCenterID);
                query = service.filterByClient(query, ClientID);
                query = service.filterByAnulated(query, anulated);
                query = service.sort(query, sortColumnIndex, sortDirection);
                long total = query.Count(); //El total sin paginacion
                Paginator<Sale> pag = new Paginator<Sale>(query);
                query = pag.page(param.iDisplayStart, param.iDisplayLength);
                List<SaleTableJson> result = service.toJsonArray(query);
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
        public ActionResult Anulate(long SaleID) {
            ISaleService service = new SaleService();
            try {
                Status result = service.Anulate(SaleID);
                if (result.Success) {
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = result.Message
                    }, JsonRequestBehavior.AllowGet);
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                    Response.Write(result.Message);
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
        public JsonResult Autocomplete(string term, long? DistributionCenterID, long? ClientID, bool? annulated) {
            try {
                ISaleService service = new SaleService();
                var items = service.getAutocomplete(term, DistributionCenterID, ClientID, annulated);
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