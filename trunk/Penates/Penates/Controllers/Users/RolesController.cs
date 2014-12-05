using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Penates.Models.ViewModels.Forms;
using Penates.Database;
using Penates.Models.ViewModels.Users;
using Penates.Services.Users;
using Expression = Microsoft.Ajax.Utilities.Expression;
using Penates.Services;
using Penates.Utils.Enums;
using Penates.Models;
using Penates.Interfaces.Services;
using Penates.Utils.JSON.TableObjects;
using Penates.Exceptions;
using Penates.Models.ViewModels.ABMs;

namespace Penates.Controllers
{
    [RoleValidation(RoleType.Admin, RoleType.SU)]
    public class RolesController : BaseController
    {
        PenatesEntities database = new PenatesEntities();

        public const int PageSize = 10;

        public ActionResult Index()
        {
            ABMViewModel model = new ABMViewModel();
            return View("~/Views/User/Roles/RoleEdit.cshtml", model);
        }

        public ActionResult ABMAjax(jQueryDataTableParamModel param) {
            try {
                IRoleService service = new RoleService();
                IQueryable<Role> query = service.getData();
                List<RoleTableJson> result = service.toJsonArray(query);
                return Json(new {
                    sEcho = param.sEcho,
                    aaData = result
                },
                            JsonRequestBehavior.AllowGet);
            } catch (MyException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            } catch (Exception e) {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }
    }   
}
