using Penates.Exceptions;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.DC;
using Penates.Services;
using Penates.Services.ABMs;
using Penates.Services.DC;
using Penates.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.DisCenter.Halls
{
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
    public class HallsFormsController : Controller
    {
        public ActionResult Index(long? DistributionCenterID, long? DepositID, long? SectorID) {
            HallViewModel model = new HallViewModel();
            if (SectorID.HasValue) {
                ISectorService sectorService = new SectorService();
                SectorViewModel sector = sectorService.getSectorData(SectorID.Value);
                if (sector != null) {
                    model.SectorID = SectorID.Value;
                    model.SectorName = sector.Description;
                    model.Height = sector.Height;
                    model.MaxWidth = sector.Width;
                    model.MaxSize = sector.Size - sector.UsedSpace;
                    model.MaxDepth = sector.Depth;
                    model.Width = sector.Width;
                    model.Depth = sector.Depth;
                    model.calculateSize();
                    model.DepositID = sector.DepositID;
                    model.DepositName = sector.DepositName;
                    model.DistributionCenter = sector.DistributionCenter;
                    model.initialCategories = sector.initialCategories;
                    model.Categories = sector.Categories;
                }
            }else{
                if (DepositID.HasValue) {
                    IDepositService service = new DepositService();
                    DepositViewModel depo = service.getDepositData(DepositID.Value);
                    if (depo != null) {
                        model.DepositID = DepositID.Value;
                        model.DepositName = depo.Description;
                        model.DistributionCenter = depo.DistributionCenterID;
                        model.Height = depo.Height;
                    }
                } else {
                    if (DistributionCenterID.HasValue) {
                        model.DistributionCenter = DistributionCenterID.Value;
                    }
                }
            }
            return View("~/Views/DistributionCenter/Halls/HallForm.cshtml", model);
        }

        public ActionResult FormEdit(long HallID) {
            IHallService service = new HallService();
            try {
                HallViewModel model = service.getHallData(HallID);
                if (model == null) {//Si no encontre agrego el error para que lo muestre
                    model = new HallViewModel();
                    ModelState.AddModelError("error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.HallWArt, HallID));
                }
                return View("~/Views/DistributionCenter/Halls/HallForm.cshtml", model);
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, ex.Message, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        /// <summary> Trato el POST del formulario</summary>
        /// <param name="prod">El modelo con los datos del producto</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Save(HallViewModel hall) {
            if (ModelState.IsValid) {
                IHallService service = new HallService();
                try {
                    var result = service.Save(hall);

                    if (result >= 0) { //Si es alguno de los errores conocidos salta x Exception
                        service.saveCategories(result, hall.Categories);
                        hall.HallID = result;
                        return RedirectToAction("FormSummary", "HallsForms", new { HallID = result });
                    } else {
                        ModelState.AddModelError("error", Resources.Errors.SPError);
                    }
                } catch (ModelErrorException me) {
                    if (String.IsNullOrWhiteSpace(me.AttributeName)) {
                        me.AttributeName = "error";
                    }
                    if (String.IsNullOrWhiteSpace(me.title)) {
                        ModelState.AddModelError(me.AttributeName, me.Message);
                    } else {
                        ModelState.AddModelError(me.AttributeName, me.title + ": " + me.Message);
                    }
                } catch (DatabaseException ex) {
                    ModelState.AddModelError("error", Resources.Errors.DatabaseError + ": " + ex.Message);
                } catch (Exception e) {
                    ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                    return View("~/Views/Error/ErrorDisplay.cshtml", error);
                }
            }
            ICategoryService categoryService = new CategoryService();
            hall.initialCategories = categoryService.getCategoriesSelect(hall.Categories);
            return View("~/Views/DistributionCenter/Halls/HallForm.cshtml", hall);
        }

        /// <summary> Muestra un Summary con los datos que se agregaron recientemente o editaron /// </summary>
        /// <param name="prod">Modelo con los datos a mostrar</param>
        /// <param name="ProdImage">La imagen se manda aparte para evitar errores</param>
        /// <returns>La vista del summary</returns>
        public ActionResult FormSummary(long HallID) {
            IHallService service = new HallService();
            HallViewModel model = service.getHallSummary(HallID);
            return View("~/Views/DistributionCenter/Halls/HallFormSummary.cshtml", model);
        }

        /// <summary>Elimina el producto con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult Delete(long HallID, long? DistributionCenterID, long? DepositID, long? SectorID) {
            IHallService service = new HallService();
            try {
                bool result = service.Delete(HallID);
                ABMViewModel model = new ABMViewModel();
                if (result) {
                    model.Message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.HallWArt, HallID);
                    model.Error = false;
                } else {
                    model.Message = String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.HallWArt, HallID);
                    model.Error = true;
                }
                if (DistributionCenterID.HasValue) {
                    model.SelectedValue = DistributionCenterID.Value;
                }
                return RedirectToAction("Index", "HallsABM", new { Message = model.Message, Error = model.Error, SelectedValue = model.SelectedValue, DepositID = DepositID, SectorID = SectorID });
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(ex.title, ex.Message, ex, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        [RoleValidation(RoleType.All)]
        public JsonResult CategoryAutocomplete(string term, long? SectorID) {
            try {
                IHallService service = new HallService();
                ICategoryService categoryService = new CategoryService();
                var items = service.getCategoryAutocomplete(term, SectorID);
                var json = categoryService.toJsonAutocomplete(items);
                return Json(json, JsonRequestBehavior.AllowGet);
            } catch (DatabaseException ex) {
                Response.StatusCode = 511;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            } catch (Exception e) {
                Response.StatusCode = 511;
                Response.StatusDescription = Resources.Errors.DatabaseError;
                Response.Write(e.Message);
                return null;
            }
        }
    }
}