using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Services;
using Penates.Services.DC;
using Penates.Services.Geography;
using Penates.Utils;
using Penates.Utils.Enums;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.DisCenter
{
    public class DistributionCenterABMController : Controller
    {
        [RoleValidation(RoleType.SU, RoleType.InventoryChief)]
        public ActionResult Index(ABMViewModel model) {
            if (model == null) {
                model = new ABMViewModel();
            }
            IGeoService service = new GeoService();
            model.List = service.getCountryList(true);

            List<SelectListItem> typeList = new List<SelectListItem>();
            SelectListItem i = new SelectListItem();
            i.Text = Resources.Resources.All;
            i.Value = DistributionCenterTypes.ALL.getTypeNumber().ToString();
            typeList.Add(i);
            i = new SelectListItem();
            i.Text = Penates.App_GlobalResources.Forms.ModelFormsResources.Internal;
            i.Value = DistributionCenterTypes.INTERNAL.getTypeNumber().ToString(); ;
            typeList.Add(i);
            i = new SelectListItem();
            i.Text = Penates.App_GlobalResources.Forms.ModelFormsResources.External;
            i.Value = DistributionCenterTypes.EXTERNAL.getTypeNumber().ToString(); ;
            typeList.Add(i);
            ViewBag.TypeList = typeList;

            return View("~/Views/DistributionCenter/DistributionCenterABM.cshtml",model);
        }

        [RoleValidation(RoleType.SU, RoleType.InventoryChief)]
        public ActionResult ABMAjax(jQueryDataTableParamModel param, long? ProvinceType, long? DCType) {
            try {
                IDistributionCenterService service = new DistributionCenterService();
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortDirection = Request["sSortDir_0"];
                IQueryable<Database.DistributionCenter> query = service.getData();
                if (!string.IsNullOrEmpty(param.sSearch)) {
                    query = service.search(query, param.sSearch);
                }
                query = service.filterByCountry(query, param.SelectedValue);
                query = service.filterByState(query, ProvinceType);
                query = service.filterByType(query, DCType);
                query = service.sort(query, sortColumnIndex, sortDirection);
                long total = query.Count(); //El total sin paginacion
                Paginator<Database.DistributionCenter> pag = new Paginator<Database.DistributionCenter>(query);
                query = pag.page(param.iDisplayStart, param.iDisplayLength);
                List<DistributionCenterTableJson> result = service.toJsonArray(query);
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

        /// <summary>Elimina el centro de distribucion con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        [RoleValidation(RoleType.SU, RoleType.InventoryChief)]
        public ActionResult Delete(long? DistributionCenterID) {
            IDistributionCenterService dcService = new DistributionCenterService();
            try {
                if (DistributionCenterID.HasValue) {
                    bool result = dcService.Delete(DistributionCenterID.Value);
                    if (result) {
                        return Json(new {
                            title = Resources.Messages.OperationSuccessfull,
                            message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.DistributionCenterWArt, DistributionCenterID)
                            }, JsonRequestBehavior.AllowGet);
                    } else {
                        Response.StatusCode = 511;
                        Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                        Response.Write(String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.DistributionCenterWArt, DistributionCenterID));
                        return null;
                    }
                } else {
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

        [RoleValidation(RoleType.All)]
        public JsonResult Autocomplete(string term) {
            try {
                IDistributionCenterService service = new DistributionCenterService();
                var items = service.getAutocomplete(term);
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