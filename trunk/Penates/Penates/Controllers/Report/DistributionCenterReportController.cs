using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models.ViewModels.DC;
using Penates.Models.ViewModels.Reports;
using Penates.Services;
using Penates.Services.DC;
using Penates.Utils.Enums;
using Penates.Utils.JSON.TableObjects;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Resources;

namespace Penates.Controllers.Report
{
    public class DistributionCenterReportController : Controller
    {

        public ActionResult Index(DistributionCenterReportViewModel model)
        {
            if (model == null)
            {
                model = new DistributionCenterReportViewModel();
            }

            ViewBag.Action = Url.Action("ShowReport", "DistributionCenterReport");
            return View("~/Views/Report/DistributionCenterReport.cshtml", model);
        }


        public ActionResult ShowReport(DistributionCenterReportViewModel model)
        {
            IDistributionCenterService service = new DistributionCenterService();
            DisplayReportViewModel reportModel = new DisplayReportViewModel();
            
            if (model.SelectedValue != null)
            {
                if (service.isInternal(model.SelectedValue.Value))
                {
                    if (!service.validDeposit(model.SelectedValue.Value)) {
                        model.Error = true;
                        model.Message = FormsErrors.DistributionCenterValid;

                        return RedirectToAction("Index", model);
                    }

                    InternalDistributionCenterDetails idcDetails = service.getIternalDistributionCenterDetails((long)model.SelectedValue);
                    InternalDistributionCenterViewModel internalModel = service.getInternalData((long) model.SelectedValue);
               
                    reportModel.DistributionCenterAddress = internalModel.Address;
                    reportModel.DistributionCenterCity = internalModel.CityName;
                    reportModel.DistributionCenterCountry = internalModel.CountryName;
                    reportModel.Deposits = idcDetails.Deposits;
                    reportModel.Sectors = idcDetails.Sectors;
                    reportModel.Halls = idcDetails.Halls;
                    reportModel.Racks = idcDetails.Racks;
                    reportModel.Shelfs = idcDetails.Shelfs;
                    reportModel.AmountOfProducts = idcDetails.ProducsQuantity;
                    reportModel.AmountOfProductsTypes = service.getInternalDistributionCenterProductTypesQuantity((long) model.SelectedValue);
                    reportModel.AverageOfSpaceCovered = internalModel.UsedPercentage;
                    reportModel.DistributionCenterValuation = 0;
                    reportModel.DistributionCenterCtype = Resources.FormsResources.Internal;
                }
                else
                {
                    ExternalDistributionCenterViewModel externalModel = service.getExternalData((long) model.SelectedValue);
                    reportModel.DistributionCenterAddress = externalModel.Address;
                    reportModel.DistributionCenterCity = externalModel.CityName;
                    reportModel.DistributionCenterCountry = externalModel.CountryName;
                    reportModel.AmountOfProducts = service.getExternalDistributionCenterProductsQuantity((long) model.SelectedValue);
                    reportModel.AmountOfProductsTypes = service.getExternallDistributionCenterProductTypesQuantity((long) model.SelectedValue);
                    reportModel.AverageOfSpaceCovered = 100 - externalModel.UsableSpacePercentage;
                    reportModel.DistributionCenterValuation = 0;
                    reportModel.DistributionCenterCtype = Resources.FormsResources.External;
                }

                return View("~/Views/Report/DisplayReport.cshtml", reportModel);
            }

            model.Error = true;
            model.Message = FormsErrors.DistributionCenterRequired;

            return RedirectToAction("Index", model);
        }


        [RoleValidation(RoleType.All)]
        public JsonResult Autocomplete(string term)
        {
            try
            {
                IDistributionCenterService service = new DistributionCenterService();
                var items = service.getAutocomplete(term);
                var json = service.toJsonAutocomplete(items);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (DatabaseException ex)
            {
                Response.StatusCode = 504;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            }
            catch (Exception e)
            {
                Response.StatusCode = 504;
                Response.StatusDescription = Resources.Errors.DatabaseError;
                Response.Write(e.Message);
                return null;
            }
        }

    }
}