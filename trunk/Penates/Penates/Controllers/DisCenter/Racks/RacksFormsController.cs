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

namespace Penates.Controllers.DisCenter.Racks
{
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
    public class RacksFormsController : Controller
    {
        public ActionResult Index(long? DistributionCenterID, long? DepositID, long? SectorID, long? HallID) {
            RackViewModel model = new RackViewModel();
            if (HallID.HasValue) {
                IHallService hallService = new HallService();
                HallViewModel hall = hallService.getHallData(HallID.Value);
                if (hall != null) {
                    model.HallID = HallID.Value;
                    model.HallName = hall.Description;
                    model.SectorID = hall.SectorID;
                    model.SectorName = hall.SectorName;
                    model.Height = hall.Height;
                    model.Depth = hall.Depth;
                    model.Width = hall.Width;
                    model.MaxWidth = hall.Width;
                    model.MaxHeight = hall.Height;
                    model.MaxSize = hall.Size - hall.UsedSpace;
                    model.calculateSize();
                    model.MaxDepth = hall.Depth;
                    model.DepositID = hall.DepositID;
                    model.DepositName = hall.DepositName;
                    model.DistributionCenter = hall.DistributionCenter;
                    model.initialCategories = hall.initialCategories;
                    model.Categories = hall.Categories;
                }
            } else {
                if (SectorID.HasValue) {
                    ISectorService sectorService = new SectorService();
                    SectorViewModel sector = sectorService.getSectorData(SectorID.Value);
                    if (sector != null) {
                        model.SectorID = SectorID.Value;
                        model.SectorName = sector.Description;
                        model.DepositID = sector.DepositID;
                        model.DepositName = sector.DepositName;
                        model.DistributionCenter = sector.DistributionCenter;
                    }
                } else {
                    if (DepositID.HasValue) {
                        IDepositService service = new DepositService();
                        DepositViewModel depo = service.getDepositData(DepositID.Value);
                        if (depo != null) {
                            model.DepositID = DepositID.Value;
                            model.DepositName = depo.Description;
                            model.DistributionCenter = depo.DistributionCenterID;
                        }
                    } else {
                        if (DistributionCenterID.HasValue) {
                            model.DistributionCenter = DistributionCenterID.Value;
                        }
                    }
                }
            }
            return View("~/Views/DistributionCenter/Racks/RackForm.cshtml", model);
        }

        public ActionResult FormEdit(long RackID) {
            IRackService service = new RackService();
            try {
                RackViewModel model = service.getRackData(RackID);
                if (model == null) {//Si no encontre agrego el error para que lo muestre
                    model = new RackViewModel();
                    ModelState.AddModelError("error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.RackWArt, RackID));
                }
                return View("~/Views/DistributionCenter/Racks/RackForm.cshtml", model);
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
        public ActionResult Save(RackViewModel rack) {
            if (ModelState.IsValid) {
                IRackService service = new RackService();
                try {
                    var result = service.Save(rack);

                    if (result >= 0) { //Si es alguno de los errores conocidos salta x Exception
                        service.saveCategories(result, rack.Categories);
                        rack.RackID = result;
                        return RedirectToAction("FormSummary", "RacksForms", new { RackID = result });
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
            rack.initialCategories = categoryService.getCategoriesSelect(rack.Categories);
            return View("~/Views/DistributionCenter/Racks/RackForm.cshtml", rack);
        }

        /// <summary> Muestra un Summary con los datos que se agregaron recientemente o editaron /// </summary>
        /// <param name="prod">Modelo con los datos a mostrar</param>
        /// <param name="ProdImage">La imagen se manda aparte para evitar errores</param>
        /// <returns>La vista del summary</returns>
        public ActionResult FormSummary(long RackID) {
            IRackService service = new RackService();
            RackViewModel model = service.getRackSummary(RackID);
            return View("~/Views/DistributionCenter/Racks/RackFormSummary.cshtml", model);
        }

        /// <summary>Elimina el producto con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult Delete(long RackID, long? DistributionCenterID, long? DepositID, long? SectorID, long? HallID) {
            IRackService service = new RackService();
            try {
                bool result = service.Delete(RackID);
                ABMViewModel model = new ABMViewModel();
                if (result) {
                    model.Message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.RackWArt, RackID);
                    model.Error = false;
                } else {
                    model.Message = String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.RackWArt, RackID);
                    model.Error = true;
                }
                if (DistributionCenterID.HasValue) {
                    model.SelectedValue = DistributionCenterID.Value;
                }
                return RedirectToAction("Index", "RacksABM", new { Message = model.Message, Error = model.Error, SelectedValue = model.SelectedValue, DepositID = DepositID, SectorID = SectorID, HallID = HallID });
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(ex.title, ex.Message, ex, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        [RoleValidation(RoleType.All)]
        public JsonResult CategoryAutocomplete(string term, long? HallID) {
            try {
                IRackService service = new RackService();
                ICategoryService categoryService = new CategoryService();
                var items = service.getCategoryAutocomplete(term, HallID);
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