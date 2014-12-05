using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Penates.Database;
using Penates.Services;
using Penates.Exceptions.Database;
using Penates.Models;
using Penates.Services.DC;
using Penates.Interfaces.Services;
using Penates.Models.ViewModels.DC;
using Penates.Services.Geography;
using Penates.Utils.Enums;
using Penates.Models.ViewModels.ABMs;
using Penates.Exceptions;


namespace Penates.Controllers.DisCenter.Deposits
{
    /// <summary>Controller for Temporary Deposits</summary>
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
    public class TemporaryDepositsFormsController : Controller
    {

        [HttpGet]
        public ActionResult Index(long? DistributionCenterID) {
            DepositViewModel model = new DepositViewModel();
            if (DistributionCenterID.HasValue) {
                model.DistributionCenterID = DistributionCenterID.Value;
            }
            return View("~/Views/DistributionCenter/Deposits/TemporaryDepositForm.cshtml", model);
        }

        public ActionResult FormEdit(long TemporaryDepositID) {
            ITemporaryDepositService service = new TemporaryDepositService();
            try {
                DepositViewModel model = service.getDepositData(TemporaryDepositID);
                if (model == null) {//Si no encontre agrego el error para que lo muestre
                    model = new DepositViewModel();
                    ModelState.AddModelError("error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.TemporaryDepositWArt, TemporaryDepositID));
                }
                return View("~/Views/DistributionCenter/Deposits/TemporaryDepositForm.cshtml", model);
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
        public ActionResult Save(DepositViewModel depo) {
            if (ModelState.IsValid) {
                ITemporaryDepositService service = new TemporaryDepositService();
                try {
                    var result = service.Save(depo);

                    if (result >= 0) { //Si es alguno de los errores conocidos salta x Exception
                        depo.DepositID = result;
                        return this.FormSummary(depo);
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
            return View("~/Views/DistributionCenter/Deposits/TemporaryDepositForm.cshtml", depo);
        }

        /// <summary> Muestra un Summary con los datos que se agregaron recientemente o editaron /// </summary>
        /// <param name="prod">Modelo con los datos a mostrar</param>
        /// <param name="ProdImage">La imagen se manda aparte para evitar errores</param>
        /// <returns>La vista del summary</returns>
        public ActionResult FormSummary(DepositViewModel model) {
            ITemporaryDepositService service = new TemporaryDepositService();
            model = service.getDepositData(model.DepositID);
            return View("~/Views/DistributionCenter/Deposits/TemporaryDepositFormSummary.cshtml", model);
        }

        /// <summary>Elimina el producto con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult Delete(long TemporaryDepositID, long? DistributionCenterID) {
            ITemporaryDepositService service = new TemporaryDepositService();
            try {
                bool result = service.Delete(TemporaryDepositID);
                ABMViewModel model = new ABMViewModel();
                if (result) {
                    model.Message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.TemporaryDepositWArt, TemporaryDepositID);
                    model.Error = false;
                } else {
                    model.Message = String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.TemporaryDepositWArt, TemporaryDepositID);
                    model.Error = true;
                }
                if (DistributionCenterID.HasValue) {
                    model.SelectedValue = DistributionCenterID.Value;
                }
                return RedirectToAction("Index", "TemporaryDepositsABM", new { Message = model.Message, Error = model.Error, SelectedValue = model.SelectedValue });
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(ex.title, ex.Message, ex, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString());
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }
    }
}
