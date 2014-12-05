using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Models;
using Penates.Models.ViewModels.Home;
using Penates.Services.Notifications;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.Home
{
    public class NotificationsController : Controller
    {
        public ActionResult Index()
        {
            HomeController controller = new HomeController();
            DashBoardViewModel model = new DashBoardViewModel();
            return controller.HomeDashboards(model);
        }

        public ActionResult GetNotifications(jQueryDataTableParamModel param, string UserName)
        {
            try
            {
                NotificationsService service = new NotificationsService();
                IQueryable<Notification> query = service.getData(UserName);
                List<NotificationTableJson> result = service.toJsonArray(query);
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = result
                },
                            JsonRequestBehavior.AllowGet);
            }
            catch (DatabaseException ex)
            {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "FormsController", "GetNotifications");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }

        /// <summary>Elimina la Notificacion con determinado id/// </summary>
        /// <param name="NotificationId">Id de la Notificacion a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult DeleteNotification(long NotificationID)
        {
            NotificationsService service = new NotificationsService();
            try
            {
                    bool result = service.Delete(NotificationID);
                    if (result)
                    {
                        return Json(new
                        {
                            title = Resources.Messages.OperationSuccessfull,
                            message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.NotificationWArt, NotificationID)
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Response.StatusCode = 511;
                        Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                        Response.Write(String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.NotificationWArt, NotificationID));
                        return null;
                    }
            }
            catch (DatabaseException ex)
            {
                Response.StatusCode = 511;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            }
            catch (Exception e)
            {
                Response.StatusCode = 511;
                Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                Response.Write(e.Message);
                return null;
            }
        }


       

      

	}
}