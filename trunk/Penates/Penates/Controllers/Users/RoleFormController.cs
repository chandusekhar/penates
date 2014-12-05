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
using Penates.Models.ViewModels.Users;
using Penates.Services.Users;


namespace Penates.Controllers
{
    /// <summary>Controller for Products</summary>
    public class RoleFormController : Controller
    {
        //
        // GET: /Forms/
        PenatesEntities database = new PenatesEntities();

        public ActionResult Index() {
            RolesController controller = new RolesController();
            return controller.Index();
        }

        public ActionResult FormEdit(long RoleID) {

            IRoleService service = new RoleService();

            var role = service.GetRole(RoleID);

            return View("~/Views/User/Forms/RoleForm.cshtml", role);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Save(Roles role) {
            if (ModelState.IsValid) {
                IRoleService service = new RoleService();
                try {
                    var stat = service.EditRole(role);
                    return RedirectToAction("Index","Roles");
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
            return View("~/Views/User/Forms/RoleForm.cshtml", role);
        }
    }
}
