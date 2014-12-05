using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.Home;
using Penates.Services;
using Penates.Services.Geography;
using Penates.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.Geography
{
    [RoleValidation(RoleType.All)]
    public class GeographyController : Controller
    {
        public ActionResult Index()
        {
            HomeController controller = new HomeController();
            DashBoardViewModel model = new DashBoardViewModel();
            return controller.HomeDashboards(model);
        }

        public ActionResult GetCitiesAll(long? provinceID) {
            try {
                SelectList cities;
                if (provinceID.HasValue && provinceID.Value != -1) {
                    IGeoService service = new GeoService();
                    cities = service.getCityList(provinceID.Value, true);
                } else {
                    List<SelectListItem> stateListInitialize = new List<SelectListItem>();
                    SelectListItem i = new SelectListItem();
                    i.Text = Resources.Resources.All;
                    i.Value = "-1";
                    stateListInitialize.Add(i);
                    cities = new SelectList(stateListInitialize, "Value", "Text");
                }
                return Json(cities,
                            JsonRequestBehavior.AllowGet);
            } catch (DatabaseException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "GeographyController", "GetCitiesAll");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            } catch (Exception e) {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, "GeographyController", "GetCitiesAll");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }

        public ActionResult GetStatesAll(long? countryID) {
            try {
                SelectList states;
                if (countryID.HasValue && countryID.Value != -1) {
                    IGeoService service = new GeoService();
                    states = service.getStateList(countryID.Value, true);
                } else {
                    List<SelectListItem> stateListInitialize = new List<SelectListItem>();
                    SelectListItem i = new SelectListItem();
                    i.Text = Resources.Resources.All;
                    i.Value = "-1";
                    stateListInitialize.Add(i);
                    states = new SelectList(stateListInitialize, "Value", "Text");
                }
                return Json(states,
                            JsonRequestBehavior.AllowGet);
            } catch (DatabaseException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            } catch (Exception e) {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }

        public ActionResult GetStates(long? countryID) {
            try {
                SelectList states;
                if (countryID.HasValue && countryID.Value != -1) {
                    IGeoService service = new GeoService();
                    states = service.getStateList(countryID.Value);
                } else {
                    List<SelectListItem> stateListInitialize = new List<SelectListItem>();
                    SelectListItem i = new SelectListItem();
                    i.Text = "";
                    i.Value = "0";
                    stateListInitialize.Add(i);
                    states = new SelectList(stateListInitialize, "Value", "Text");
                }
                return Json(states,
                            JsonRequestBehavior.AllowGet);
            } catch (DatabaseException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            } catch (Exception e) {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }

        public ActionResult GetCities(long? stateID) {
            try {
                SelectList cities;
                if (stateID.HasValue && stateID.Value != -1) {
                    IGeoService service = new GeoService();
                    cities = service.getCityList(stateID.Value);
                } else {
                    List<SelectListItem> cityListInitialize = new List<SelectListItem>();
                    SelectListItem i = new SelectListItem();
                    i.Text = "";
                    i.Value = "0";
                    cityListInitialize.Add(i);
                    cities = new SelectList(cityListInitialize, "Value", "Text");
                }
                return Json(cities,
                            JsonRequestBehavior.AllowGet);
            } catch (DatabaseException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            } catch (Exception e) {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }
    }
}