using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Services;
using Penates.Services.ABMs;
using Penates.Services.Geography;
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
    public class ClientsABMController : Controller
    {
        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common, RoleType.Executive)]
        public ActionResult Index(ABMViewModel model) {
            if (model == null) {
                model = new ABMViewModel();
            }
            IGeoService service = new GeoService();
            model.List = service.getCountryList(true);

            List<SelectListItem> typeList = new List<SelectListItem>();
            SelectListItem i = new SelectListItem();
            i.Text = Resources.Resources.All;
            i.Value = DeactivatedTypes.ALL.getTypeNumber().ToString();
            typeList.Add(i);
            i = new SelectListItem();
            i.Text = Penates.App_GlobalResources.Forms.ModelFormsResources.Active;
            i.Value = DeactivatedTypes.ACTIVE.getTypeNumber().ToString(); ;
            typeList.Add(i);
            i = new SelectListItem();
            i.Text = Penates.App_GlobalResources.Forms.ModelFormsResources.Deactivated;
            i.Value = DeactivatedTypes.DEACTIVATED.getTypeNumber().ToString(); ;
            typeList.Add(i);
            ViewBag.AnnulatedList = typeList;
            return View("~/Views/ABMS/ClientsABM.cshtml", model);
        }

        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common, RoleType.Executive)]
        public ActionResult GetTable(jQueryDataTableParamModel param, long? SelectedValue, long? ProvinceID, long? CityID, int? Annulated) {
            try {
                IClientService service = new ClientService();
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortDirection = Request["sSortDir_0"];
                IQueryable<Client> query = service.getData(true);
                query = service.filterByCountry(query, SelectedValue);
                query = service.filterByState(query, ProvinceID);
                query = service.filterByCity(query, CityID);
                query = service.filterByDeactivated(query, Annulated);
                query = service.sort(query, sortColumnIndex, sortDirection);
                long total = query.Count(); //El total sin paginacion
                Paginator<Client> pag = new Paginator<Client>(query);
                query = pag.page(param.iDisplayStart, param.iDisplayLength);
                List<ClientTableJson> result = service.toJsonArray(query);
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
        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common, RoleType.Executive)]
        public ActionResult Annulate(long ClientID) {
            IClientService service = new ClientService();
            try {
                Status result = service.Deactivate(ClientID);
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
                IClientService service = new ClientService();
                var items = service.getAutocomplete(term, false);
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