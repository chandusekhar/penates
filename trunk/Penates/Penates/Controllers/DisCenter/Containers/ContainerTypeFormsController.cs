using Penates.Exceptions;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.DC;
using Penates.Services;
using Penates.Services.DC;
using Penates.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.DisCenter.Containers
{
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
    public class ContainerTypeFormsController : Controller
    {
        public ActionResult Index() {
            ContainerTypeViewModel model = new ContainerTypeViewModel();
            return View("~/Views/DistributionCenter/Containers/ContainerTypeForm.cshtml", model);
        }

        public ActionResult FormEdit(long ContainerTypeID) {
            IContainerTypeService service = new ContainerTypeService();
            try {
                ContainerTypeViewModel model = service.getData(ContainerTypeID);
                if (model == null) {//Si no encontre agrego el error para que lo muestre
                    model = new ContainerTypeViewModel();
                    ModelState.AddModelError("error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.ContainerTypeWArt, ContainerTypeID));
                }
                return View("~/Views/DistributionCenter/Containers/ContainerTypeForm.cshtml", model);
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
        public ActionResult Save(ContainerTypeViewModel type) {
            if (type.ContainerTypeID.HasValue && type.ContainerTypeID.Value == 0) {
                return RedirectToAction("Index", "ContainerTypesABM");
            }
            if (ModelState.IsValid) {
                IContainerTypeService service = new ContainerTypeService();
                try {
                    var result = service.Save(type);

                    if (result >= 0) { //Si es alguno de los errores conocidos salta x Exception
                        ABMViewModel model = new ABMViewModel();
                        model.Message = Resources.Messages.OperationSuccessfull;
                        return RedirectToAction("Index", "ContainerTypesABM", new { Message = model.Message });
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
            return View("~/Views/DistributionCenter/Containers/ContainerTypeForm.cshtml", type);
        }

        /// <summary>Elimina el producto con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult Delete(long ContainerTypeID) {
            IContainerTypeService service = new ContainerTypeService();
            try {
                bool result = service.Delete(ContainerTypeID);
                ABMViewModel model = new ABMViewModel();
                if (result) {
                    model.Message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.ContainerTypeWArt, ContainerTypeID);
                    model.Error = false;
                } else {
                    model.Message = String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.ContainerTypeWArt, ContainerTypeID);
                    model.Error = true;
                }
                return RedirectToAction("Index", "ContainerTypesABM", new { Message = model.Message, Error = model.Error });
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