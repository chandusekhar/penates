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
    public class PackFormController : Controller
    {
        public ActionResult Index() {
            PackViewModel model = new PackViewModel();
            return View("~/Views/ABMs/Forms/PackForm.cshtml", model);
        }

        public ActionResult FormEdit(long PackID) {
            IPackService service = new PackService();
            try {
                PackViewModel model = service.getData(PackID);
                if (model == null) {//Si no encontre agrego el error para que lo muestre
                    model = new PackViewModel();
                    ModelState.AddModelError("error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.PackWArt, PackID));
                }
                return View("~/Views/ABMs/Forms/PackForm.cshtml", model);
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
        public ActionResult Save(PackViewModel pack) {
            if (ModelState.IsValid) {
                IPackService service = new PackService();
                try {
                    if (!pack.HasExpirationDate) {
                        pack.ExpirationDate = null;
                    }
                    var result = service.Save(pack);

                    if (result >= 0) { //Si es alguno de los errores conocidos salta x Exception
                        pack.PackID = result;
                        return RedirectToAction("PackSummary", "PackForm", pack);
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
            return View("~/Views/ABMs/Forms/PackForm.cshtml", pack);
        }

        /// <summary> Muestra un Summary con los datos que se agregaron recientemente o editaron /// </summary>
        /// <param name="prod">Modelo con los datos a mostrar</param>
        /// <param name="ProdImage">La imagen se manda aparte para evitar errores</param>
        /// <returns>La vista del summary</returns>
        public ActionResult PackSummary(PackViewModel pack) {
            return View("~/Views/ABMs/Forms/PackFormSummary.cshtml", pack);
        }

        /// <summary>Elimina el producto con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult Delete(long PackID) {
            IPackService service = new PackService();
            try {
                bool result = service.Delete(PackID);
                ABMViewModel model = new ABMViewModel();
                if (result) {
                    model.Message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.PackWArt, PackID);
                    model.Error = false;
                } else {
                    model.Message = String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.PackWArt, PackID);
                    model.Error = true;
                }
                return RedirectToAction("Index", "PacksABM", new { Message = model.Message, Error = model.Error });
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