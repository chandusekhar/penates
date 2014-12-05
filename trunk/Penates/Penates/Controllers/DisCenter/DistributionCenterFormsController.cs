using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Penates.Database;
using System.Drawing;
using Penates.Utils;
using Penates.Utils.Keepers;
using Penates.Services.ABMs;
using System.Data.Entity.Core;
using Penates.Exceptions.Database;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.DC;
using Penates.Interfaces.Services;
using Penates.Services.Geography;
using Penates.Services.DC;
using Penates.Services;
using Penates.Utils.Enums;


namespace Penates.Controllers.DisCenter
{
    /// <summary>Controller for Products</summary>
    [RoleValidation(RoleType.SU, RoleType.InventoryChief)]
    public class DistributionCenterFormsController : Controller
    {
        public ActionResult Index() {
            DistributionCenterABMController controller = new DistributionCenterABMController();
            ABMViewModel model = new ABMViewModel();
            return controller.Index(model);
        }

        [HttpGet]
        public ActionResult AddInternalForm() {
            var model = new InternalDistributionCenterViewModel();
            IGeoService geoService = new GeoService();
            model.CountryList = geoService.getCountryListWMessage();
            return View("~/Views/DistributionCenter/Forms/InternalDCForm.cshtml", model);
        }

        public ActionResult AddExternalForm() {
            var model = new ExternalDistributionCenterViewModel();
            IGeoService geoService = new GeoService();
            model.CountryList = geoService.getCountryListWMessage();
            return View("~/Views/DistributionCenter/Forms/ExternalDCForm.cshtml", model);
        }

        public ActionResult FormEdit(long id) {
            IDistributionCenterService service = new DistributionCenterService();
            if (service.isInternal(id)) {
                return this.EditInternalForm(id);
            }
            return this.EditExternalForm(id);
        }

        public ActionResult EditInternalForm(long DistributionCenterID) {
            IDistributionCenterService service = new DistributionCenterService();
            try {
                InternalDistributionCenterViewModel model = service.getInternalData(DistributionCenterID);
                IGeoService geoService = new GeoService();
                if (model == null) {//Si no encontre agrego el error para que lo muestre
                    model = new InternalDistributionCenterViewModel();
                    model.CountryList = geoService.getCountryListWMessage();
                    ModelState.AddModelError("error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.DistributionCenterWArt, DistributionCenterID));
                } else {
                    model.CountryList = geoService.getCountryList(model.CountryID);
                    model.StateList = geoService.getStateList(model.CountryID, model.StateID);
                    model.CityList = geoService.getCityList(model.StateID, model.CityID);
                }
                return View("~/Views/DistributionCenter/Forms/InternalDCForm.cshtml", model);
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, ex.Message, "FormsController", "ProductFormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "ProductFormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        public ActionResult EditExternalForm(long DistributionCenterID) {
            IDistributionCenterService service = new DistributionCenterService();
            try {
                ExternalDistributionCenterViewModel model = service.getExternalData(DistributionCenterID);
                IGeoService geoService = new GeoService();
                if (model == null) {//Si no encontre agrego el error para que lo muestre
                    model = new ExternalDistributionCenterViewModel();
                    model.CountryList = geoService.getCountryListWMessage();
                    ModelState.AddModelError("error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.DistributionCenterWArt, DistributionCenterID));
                } else {
                    model.CountryList = geoService.getCountryList(model.CountryID);
                    model.StateList = geoService.getStateList(model.CountryID, model.StateID);
                    model.CityList = geoService.getCityList(model.StateID, model.CityID);
                }
                return View("~/Views/DistributionCenter/Forms/ExternalDCForm.cshtml", model);
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, ex.Message, "FormsController", "ProductFormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "ProductFormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        /// <summary> Trato el POST del formulario de Producto </summary>
        /// <param name="prod">El modelo con los datos del producto</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveInternal(InternalDistributionCenterViewModel dc) {
            if (ModelState.IsValid) {
                IDistributionCenterService service = new DistributionCenterService();
                try {
                    var result = service.SaveInternal(dc);

                    if (result >= 0) { //Si es alguno de los errores conocidos salta x Exception
                        dc.DistributionCenterID = result;
                        return this.InternalFormSummary(dc);
                    } else {
                        ModelState.AddModelError("error", Resources.Errors.SPError);
                    }
                } catch (DataRestrictionProcedureException infe) {
                    ModelState.AddModelError(infe.atributeName, infe.Message);
                } catch (ForeignKeyConstraintException infe) {
                    ModelState.AddModelError(infe.atributeName, infe.Message);
                } catch (DatabaseException ex) {
                    ModelState.AddModelError("error", Resources.Errors.DatabaseError + ": " + ex.Message);
                } catch (Exception e) {
                    ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "SubmitProduct");
                    return View("~/Views/Error/ErrorDisplay.cshtml", error);
                }
            }
            IGeoService geoService = new GeoService();
            dc.CountryList = geoService.getCountryListWMessage();
            return View("~/Views/DistributionCenter/Forms/InternalDCForm.cshtml", dc);
        }

        /// <summary> Trato el POST del formulario de Producto </summary>
        /// <param name="prod">El modelo con los datos del producto</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveExternal(ExternalDistributionCenterViewModel dc) {
            if (ModelState.IsValid) {
                IDistributionCenterService service = new DistributionCenterService();
                try {
                    var result = service.SaveExternal(dc);

                    if (result >= 0) { //Si es alguno de los errores conocidos salta x Exception
                        dc.DistributionCenterID = result;
                        return this.ExternalFormSummary(dc);
                    } else {
                        ModelState.AddModelError("error", Resources.Errors.SPError);
                    }
                } catch (DataRestrictionProcedureException infe) {
                    ModelState.AddModelError(infe.atributeName, infe.Message);
                } catch (ForeignKeyConstraintException infe) {
                    ModelState.AddModelError(infe.atributeName, infe.Message);
                } catch (DatabaseException ex) {
                    ModelState.AddModelError("error", Resources.Errors.DatabaseError + ": " + ex.Message);
                } catch (Exception e) {
                    ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "DistributionCenterFormsController", "SaveExternal");
                    return View("~/Views/Error/ErrorDisplay.cshtml", error);
                }
            }
            IGeoService geoService = new GeoService();
            dc.CountryList = geoService.getCountryListWMessage();
            return View("~/Views/DistributionCenter/Forms/ExternalDCForm.cshtml", dc);
        }

        /// <summary> Muestra un Summary con los datos que se agregaron recientemente o editaron /// </summary>
        /// <param name="prod">Modelo con los datos a mostrar</param>
        /// <param name="ProdImage">La imagen se manda aparte para evitar errores</param>
        /// <returns>La vista del summary</returns>
        public ActionResult InternalFormSummary(InternalDistributionCenterViewModel model) {
            IDistributionCenterService service = new DistributionCenterService();
            model = service.getInternalData(model.DistributionCenterID);
            return View("~/Views/DistributionCenter/Forms/InternalDCFormSummary.cshtml", model);
        }

        /// <summary> Muestra un Summary con los datos que se agregaron recientemente o editaron /// </summary>
        /// <param name="prod">Modelo con los datos a mostrar</param>
        /// <param name="ProdImage">La imagen se manda aparte para evitar errores</param>
        /// <returns>La vista del summary</returns>
        public ActionResult ExternalFormSummary(ExternalDistributionCenterViewModel model) {
            IDistributionCenterService service = new DistributionCenterService();
            model = service.getExternalData(model.DistributionCenterID);
            return View("~/Views/DistributionCenter/Forms/ExternalDCFormSummary.cshtml", model);
        }

        /// <summary>Elimina el producto con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult Delete(long distributionCenterID) {
            IDistributionCenterService service = new DistributionCenterService();
            try {
                bool result = service.Delete(distributionCenterID);
                ABMViewModel model = new ABMViewModel();
                if (result) {
                    model.Message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.DistributionCentersWArt, distributionCenterID);
                    model.Error = false;
                } else {
                    model.Message = String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.DistributionCentersWArt, distributionCenterID);
                    model.Error = true;
                }
                return RedirectToAction("Index", "DistributionCenterABM", new { Message = model.Message, Error = model.Error});
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(ex.title, ex.Message, ex, "FormsController", "DeleteProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "DeleteProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }
    }
}
