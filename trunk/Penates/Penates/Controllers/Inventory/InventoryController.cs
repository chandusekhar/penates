using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Models;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.Inventory;
using Penates.Models.ViewModels.Reports;
using Penates.Services;
using Penates.Services.DC;
using Penates.Services.Geography;
using Penates.Services.Users;
using Penates.Utils;
using Penates.Utils.Enums;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Resources;

namespace Penates.Controllers.Inventory
{
    public class InventoryController : Controller
    {
        [RoleValidation(RoleType.SU, RoleType.InventoryChief)]
        public ActionResult Index(CreateInventoryViewModel model)
        {
            if (model == null)
            {
                model = new CreateInventoryViewModel();
            }

            var typeList = new List<SelectListItem>();
            var i = new SelectListItem
                {
                    Text = Penates.App_GlobalResources.Forms.ModelFormsResources.Retail,
                    Value = PredifinedMethodsTypes.Retail.getTypeNumber().ToString()
                };
            typeList.Add(i);

            i = new SelectListItem
                {
                    Text = Penates.App_GlobalResources.Forms.ModelFormsResources.WeightedAverage,
                    Value = PredifinedMethodsTypes.WeightedAverage.getTypeNumber().ToString()
                };
            typeList.Add(i);

            i = new SelectListItem
                {
                    Text = Penates.App_GlobalResources.Forms.ModelFormsResources.Fifo,
                    Value = PredifinedMethodsTypes.Fifo.getTypeNumber().ToString()
                };
            typeList.Add(i);

            ViewBag.TypeList = typeList;
            ViewBag.Action = Url.Action("ShowInventory");

            return View("~/Views/Inventory/CreateInventory.cshtml", model);
        }

        [RoleValidation(RoleType.SU, RoleType.InventoryChief)]
        public ActionResult ViewLast(CreateInventoryViewModel model)
        {
            IInventoryService service = new InventoryService();

            var result = service.ViewLastInventory(model.SelectedValue.HasValue ? model.SelectedValue.Value : 0);

            return View("~/Views/Inventory/ViewLastInventory.cshtml", result);
        }

        [RoleValidation(RoleType.SU, RoleType.InventoryChief)]
        public ActionResult EditLast(CreateInventoryViewModel model)
        {
            IInventoryService service = new InventoryService();

            var result = service.ViewLastInventory(model.SelectedValue.HasValue ? model.SelectedValue.Value : 0);

            return View("~/Views/Inventory/EditLastInventory.cshtml", result);
        }

        [RoleValidation(RoleType.SU, RoleType.InventoryChief)]
        public ActionResult ABMAjax(jQueryDataTableParamModel param, long? ProvinceType, long? DCType)
        {
            try
            {
                IDistributionCenterService service = new DistributionCenterService();
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortDirection = Request["sSortDir_0"];
                IQueryable<Database.DistributionCenter> query = service.getData();
                if (!string.IsNullOrEmpty(param.sSearch))
                {
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
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = count,
                    iTotalDisplayRecords = total,
                    aaData = result
                },
                            JsonRequestBehavior.AllowGet);
            }
            catch (DatabaseException ex)
            {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
            catch (Exception e)
            {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }

        public ActionResult ShowInventory(CreateInventoryViewModel model, PredifinedMethodsTypes typeSelectList)
        {
            if (ModelState.IsValid)
            {
                IInventoryService service = new InventoryService();
                IDistributionCenterService serviceDC = new DistributionCenterService();

                if (!serviceDC.validDeposit(model.SelectedValue.Value))
                {
                    model.Error = true;
                    model.Message = FormsErrors.DistributionCenterValid;

                    return RedirectToAction("Index", model);
                }

                var result = service.CreateInventory(typeSelectList, model.SelectedValue.Value, model.Code, model.InventoryName);

                switch (typeSelectList)
                {
                    case PredifinedMethodsTypes.Retail:

                        return View("~/Views/Inventory/Tables/_Retail.cshtml", result);
                        break;

                    case PredifinedMethodsTypes.WeightedAverage:
                        return View("~/Views/Inventory/Tables/_WeightedAverage.cshtml", result);
                        break;

                    case PredifinedMethodsTypes.Fifo:
                        return View("~/Views/Inventory/Tables/_Fifo.cshtml", result);
                        break;

                    default:
                        return View("~/Views/Inventory/Tables/Fifo.cshtml", result);
                }
            }

            model.Error = true;
            model.Message = FormsErrors.DistributionCenterRequired;

            return RedirectToAction("Index", model);
        }

        [RoleValidation(RoleType.All)]
        public JsonResult Autocomplete(string term)
        {
            try
            {
                IDistributionCenterService service = new DistributionCenterService();
                var items = service.getAutocomplete(term);
                var json = service.toJsonAutocomplete(items);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (DatabaseException ex)
            {
                Response.StatusCode = 504;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            }
            catch (Exception e)
            {
                Response.StatusCode = 504;
                Response.StatusDescription = Resources.Errors.DatabaseError;
                Response.Write(e.Message);
                return null;
            }
        }
    }
}