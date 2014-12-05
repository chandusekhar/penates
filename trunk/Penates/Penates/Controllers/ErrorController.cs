using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Penates.Models;
using Penates.Models.ViewModels.Users;
using Penates.Utils;
using Penates.Models.ViewModels.Home;

namespace Penates.Controllers
{
    public class ErrorController : BaseController
    {
        public ActionResult Index() {
            HomeController controller = new HomeController();
            DashBoardViewModel model = new DashBoardViewModel();
            return controller.HomeDashboards(model);
        }

        public ActionResult NotFound(string url)
        {
            var originalUri = url ?? Request.QueryString["aspxerrorpath"] ?? Request.Url.OriginalString;

            var controllerName = (string)RouteData.Values["controller"];
            var actionName = (string)RouteData.Values["action"];
            var model = new NotFoundModel(new HttpException(404, "Failed to find page"), controllerName, actionName)
            {
                RequestedUrl = originalUri,
                ReferrerUrl = Request.UrlReferrer == null ? string.Empty : Request.UrlReferrer.OriginalString
            };

            Response.StatusCode = 404;
            return View("NotFound", model);
        }

        public ActionResult ErrorDisplay(string message) {
            var controllerName = (string) RouteData.Values["controller"];
            var actionName = (string) RouteData.Values["action"];
            var model = new ErrorModel(message, controllerName, actionName);
            Response.StatusCode = 404;
            return View("ErrorDisplay", model);
        }

        public ActionResult ErrorDisplay(string title, string message) {
            var controllerName = (string) RouteData.Values["controller"];
            var actionName = (string) RouteData.Values["action"];
            var model = new ErrorModel(title, message, controllerName, actionName);
            Response.StatusCode = 404;
            return View("ErrorDisplay", model);
        }

        public ActionResult PermissionDenied() {
            return View("~/Views/Error/PermissionDenied.cshtml");
        }

        public ActionResult UnsignedUser() {
            Login model = new Login();
            model.ErrorMessage = Resources.Messages.UnsignedUser;
            return View("~/Views/Home/Index.cshtml", model);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            var name = GetViewName(ControllerContext, "~/Views/Error/{0}.cshtml"/*(actionName)*/,
                                                        "~/Views/Error/Error.cshtml",
                                                        "~/Views/Error/General.cshtml",
                                                        "~/Views/Shared/Error.cshtml");

            var controllerName = (string)RouteData.Values["controller"];
            var model = new HandleErrorInfo(Server.GetLastError(), controllerName, actionName);
            var result = new ViewResult
            {
                ViewName = name,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
            };


            Response.StatusCode = 501;
            result.ExecuteResult(ControllerContext);
        }

        protected string GetViewName(ControllerContext context, params string[] names)
        {
            foreach (var name in names)
            {
                var result = ViewEngines.Engines.FindView(ControllerContext, name, null);
                if (result.View != null)
                    return name;
            }
            return null;
        }
    }
}
