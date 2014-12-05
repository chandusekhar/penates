using Penates.Exceptions;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.Forms;
using Penates.Services;
using Penates.Services.ABMs;
using Penates.Services.Geography;
using Penates.Utils;
using Penates.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.Forms
{
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common, RoleType.Executive)]
    public class ClientFormsController : Controller
    {
        public ActionResult Index() {
            ClientViewModel model = new ClientViewModel();
            IGeoService geoService = new GeoService();
            model.CountryList = geoService.getCountryListWMessage();
            return View("~/Views/ABMs/Forms/ClientForm.cshtml", model);
        }

        public ActionResult FormEdit(long ClientID) {
            IClientService service = new ClientService();
            try {
                ClientViewModel model = service.getData(ClientID);
                IGeoService geoService = new GeoService();
                if (model == null) {//Si no encontre agrego el error para que lo muestre
                    model = new ClientViewModel();
                    model.CountryList = geoService.getCountryListWMessage();
                    ModelState.AddModelError("error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.ClientWArt, ClientID));
                } else {
                    model.CountryList = geoService.getCountryList(model.CountryID);
                    model.StateList = geoService.getStateList(model.CountryID, model.StateID);
                    model.CityList = geoService.getCityList(model.StateID, model.CityID);
                }
                return View("~/Views/ABMs/Forms/ClientForm.cshtml", model);
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
        public ActionResult Save(ClientViewModel client) {
            if (ModelState.IsValid) {
                IClientService service = new ClientService();
                try {
                    var result = service.Save(client);

                    if (result >= 0) { //Si es alguno de los errores conocidos salta x Exception
                        client.ClientID = result;
                        return RedirectToAction("ClientSummary", "ClientForms", new { ClientID = client.ClientID });
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
            return View("~/Views/ABMs/Forms/ClientForm.cshtml", client);
        }

        /// <summary> Muestra un Summary con los datos que se agregaron recientemente o editaron /// </summary>
        /// <param name="prod">Modelo con los datos a mostrar</param>
        /// <param name="ProdImage">La imagen se manda aparte para evitar errores</param>
        /// <returns>La vista del summary</returns>
        public ActionResult ClientSummary(long ClientID, string error) {
            IClientService service = new ClientService();
            ClientViewModel client = service.getData(ClientID);
            client.error = error;
            return View("~/Views/ABMs/Forms/ClientFormSummary.cshtml", client);
        }

        /// <summary>Elimina el producto con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult Delete(long ClientID) {
            IClientService service = new ClientService();
            try {
                Status result = service.Deactivate(ClientID);
                ABMViewModel model = new ABMViewModel();
                if (result.Success) {
                    return RedirectToAction("ClientSummary", "ClientForms", new { ClientID = ClientID });
                } else {
                    return RedirectToAction("ClientSummary", "ClientForms", new { ClientID = ClientID, error = result.Message });
                }
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