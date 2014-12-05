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

namespace Penates.Controllers.DisCenter.Sectors
{
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
    public class SectorsFormsController : Controller
    {
        public ActionResult Index(long? DistributionCenterID, long? DepositID) {
            SectorViewModel model = new SectorViewModel();
            if (DistributionCenterID.HasValue) {
                model.DistributionCenter = DistributionCenterID.Value;
            }
            if (DepositID.HasValue) {
                IDepositService service = new DepositService();
                DepositViewModel depo = service.getDepositData(DepositID.Value);
                if (depo != null) {
                    model.DepositID = DepositID.Value;
                    model.DepositName = depo.Description;
                    model.Height = depo.Height;
                    model.Depth = depo.Depth;
                    model.Width = depo.Width;
                    model.MaxWidth = depo.Width;
                    model.MaxSize = depo.Size - depo.UsedSpace;
                    model.MaxDepth = depo.Depth;
                    model.calculateSize();
                }
            }
            return View("~/Views/DistributionCenter/Sectors/SectorForm.cshtml", model);
        }

        public ActionResult FormEdit(long SectorID) {
            ISectorService service = new SectorService();
            try {
                SectorViewModel model = service.getSectorData(SectorID);
                if (model == null) {//Si no encontre agrego el error para que lo muestre
                    model = new SectorViewModel();
                    ModelState.AddModelError("error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.SectorWArt, SectorID));
                }
                return View("~/Views/DistributionCenter/Sectors/SectorForm.cshtml", model);
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
        public ActionResult Save(SectorViewModel sector) {
            if (ModelState.IsValid) {
                ISectorService service = new SectorService();
                try {
                    var result = service.Save(sector);

                    if (result >= 0) { //Si es alguno de los errores conocidos salta x Exception
                        service.saveCategories(result, sector.Categories);
                        sector.SectorID = result;
                        return RedirectToAction("FormSummary", "SectorsForms", new { SectorID = result });
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
            sector.initialCategories = categoryService.getCategoriesSelect(sector.Categories);
            return View("~/Views/DistributionCenter/Sectors/SectorForm.cshtml", sector);
        }

        /// <summary> Muestra un Summary con los datos que se agregaron recientemente o editaron /// </summary>
        /// <param name="prod">Modelo con los datos a mostrar</param>
        /// <param name="ProdImage">La imagen se manda aparte para evitar errores</param>
        /// <returns>La vista del summary</returns>
        public ActionResult FormSummary(long SectorID) {
            ISectorService service = new SectorService();
            SectorViewModel model = service.getSectorSummary(SectorID);
            return View("~/Views/DistributionCenter/Sectors/SectorFormSummary.cshtml", model);
        }

        /// <summary>Elimina el producto con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult Delete(long SectorID, long? DistributionCenterID, long? DepositID) {
            ISectorService service = new SectorService();
            try {
                bool result = service.Delete(SectorID);
                ABMViewModel model = new ABMViewModel();
                if (result) {
                    model.Message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.SectorWArt, SectorID);
                    model.Error = false;
                } else {
                    model.Message = String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.SectorWArt, SectorID);
                    model.Error = true;
                }
                if (DistributionCenterID.HasValue) {
                    model.SelectedValue = DistributionCenterID.Value;
                }
                return RedirectToAction("Index", "SectorsABM", new { Message = model.Message, Error = model.Error, SelectedValue = model.SelectedValue, DepositID = DepositID });
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(ex.title, ex.Message, ex, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        [RoleValidation(RoleType.All)]
        public JsonResult CategoryAutocomplete(string term, long? DepositID) {
            try {
                ISectorService service = new SectorService();
                ICategoryService categoryService = new CategoryService();
                var items = service.getCategoryAutocomplete(term, DepositID);
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