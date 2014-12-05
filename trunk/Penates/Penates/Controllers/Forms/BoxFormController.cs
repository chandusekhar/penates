using Penates.Controllers.ABMs;
using Penates.Exceptions;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.Forms;
using Penates.Services;
using Penates.Services.ABMs;
using Penates.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.Forms
{
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
    public class BoxFormController : Controller
    {
        public ActionResult Index(){
            BoxesABMController controller = new BoxesABMController();
            ABMViewModel model = new ABMViewModel();
            return controller.Index(model, null, null);
        }

        public ActionResult FormEdit(long BoxID) {
            IBoxService service = new BoxService();
            try {
                if (service.isExternal(BoxID)) {
                    return this.FormEditExternal(BoxID);
                } else {
                    return this.FormEditInternal(BoxID);
                }
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, ex.Message, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        private ActionResult FormEditExternal(long BoxID) {
            IBoxService service = new BoxService();
            ExternalBoxViewModel model = service.getExternalBoxInfo(BoxID);
            if (model == null) {//Si no encontre agrego el error para que lo muestre
                model = new ExternalBoxViewModel();
                ModelState.AddModelError("error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.BoxesWArt, BoxID));
            }
            IStatusService statusService = new StatusService();
            if (model.StatusID.HasValue) {
                model.StatusList = statusService.getStatusList(model.StatusID.Value);
            } else {
                model.StatusList = statusService.getStatusList();
            }
            return View("~/Views/ABMs/Forms/ExternalBoxForm.cshtml", model);
        }

        private ActionResult FormEditInternal(long BoxID) {
            IBoxService service = new BoxService();
            BoxViewModel model = service.getInternalBoxInfo(BoxID);
            if (model == null) {//Si no encontre agrego el error para que lo muestre
                model = new BoxViewModel();
                ModelState.AddModelError("error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.BoxesWArt, BoxID));
            }
            IStatusService statusService = new StatusService();
            if (model.StatusID.HasValue) {
                model.StatusList = statusService.getStatusList(model.StatusID.Value);
            } else {
                model.StatusList = statusService.getStatusList();
            }
            return View("~/Views/ABMs/Forms/BoxForm.cshtml", model);
        }

        /// <summary> Trato el POST del formulario</summary>
        /// <param name="prod">El modelo con los datos del producto</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Save(BoxViewModel box) {
            if (ModelState.IsValid) {
                IBoxService service = new BoxService();
                try {
                    var result = service.Edit(box);

                    if (result >= 0) { //Si es alguno de los errores conocidos salta x Exception
                        return RedirectToAction("BoxSummary", "BoxForm", new { BoxID = result});
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
            IStatusService statusService = new StatusService();
            if (box.StatusID.HasValue) {
                box.StatusList = statusService.getStatusList(box.StatusID.Value);
            } else {
                box.StatusList = statusService.getStatusList();
            }
            return View("~/Views/ABMs/Forms/BoxForm.cshtml", box);
        }

        /// <summary> Trato el POST del formulario</summary>
        /// <param name="prod">El modelo con los datos del producto</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ExternalSave(ExternalBoxViewModel box) {
            if (ModelState.IsValid) {
                IBoxService service = new BoxService();
                try {
                    var result = service.Edit(box);

                    if (result >= 0) { //Si es alguno de los errores conocidos salta x Exception
                        return RedirectToAction("ExternalBoxSummary", "BoxForm", new { BoxID = result });
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
            IStatusService statusService = new StatusService();
            if (box.StatusID.HasValue) {
                box.StatusList = statusService.getStatusList(box.StatusID.Value);
            } else {
                box.StatusList = statusService.getStatusList();
            }
            return View("~/Views/ABMs/Forms/ExternalBoxForm.cshtml", box);
        }

        /// <summary> Muestra un Summary con los datos que se agregaron recientemente o editaron /// </summary>
        /// <param name="prod">Modelo con los datos a mostrar</param>
        /// <param name="ProdImage">La imagen se manda aparte para evitar errores</param>
        /// <returns>La vista del summary</returns>
        public ActionResult BoxSummary(long BoxID) {
            IBoxService service = new BoxService();
            BoxViewModel model = service.getInternalBoxInfo(BoxID);
            if (model == null) {
                RedirectToAction("Index","BoxesABM");
            }
            return View("~/Views/ABMs/Forms/BoxFormSummary.cshtml", model);
        }

        /// <summary> Muestra un Summary con los datos que se agregaron recientemente o editaron /// </summary>
        /// <param name="prod">Modelo con los datos a mostrar</param>
        /// <param name="ProdImage">La imagen se manda aparte para evitar errores</param>
        /// <returns>La vista del summary</returns>
        public ActionResult ExternalBoxSummary(long BoxID) {
            IBoxService service = new BoxService();
            ExternalBoxViewModel model = service.getExternalBoxInfo(BoxID);
            if (model == null) {
                RedirectToAction("Index", "BoxesABM");
            }
            return View("~/Views/ABMs/Forms/ExternalBoxFormSummary.cshtml", model);
        }
    }
}