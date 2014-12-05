using Penates.Database;
using Penates.Models;
using Penates.Services.ActivityStream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Penates.Utils.JSON.TableObjects;
using Penates.Exceptions.Database;
using Penates.Models.ViewModels.Home;


namespace Penates.Controllers.Home
{
    public class ActivityStreamController : Controller {
        public ActionResult Index() {
            HomeController controller = new HomeController();
            DashBoardViewModel model = new DashBoardViewModel();
            return controller.HomeDashboards(model);
        }

        public ActionResult GetUpdates(jQueryDataTableParamModel param, string UserName) {
            try {
                ActivityStreamSerivce service = new ActivityStreamSerivce();
                IQueryable<Sale> sales = service.getSales(HttpContext.User.Identity.Name);
                IQueryable<Transfer> transfers = service.getTransfers(HttpContext.User.Identity.Name);
                IQueryable<Reception> receptions = service.getReceptions(HttpContext.User.Identity.Name);


                List<ActivityStreamTableJson> result = new List<ActivityStreamTableJson>();
                if (sales != null )
                {
                    result.AddRange(service.salesToActivityStream(sales));
                }
                if (transfers != null)
                {
                    result.AddRange(service.transfersToActivityStream(transfers));
                }
                if (receptions != null) { result.AddRange(service.receptionsToActivityStream(receptions)); }

                return Json(new {
                    sEcho = param.sEcho,
                    aaData = result
                },
                            JsonRequestBehavior.AllowGet);
            } catch (DatabaseException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "FormsController", "GetNotifications");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }
    }
}