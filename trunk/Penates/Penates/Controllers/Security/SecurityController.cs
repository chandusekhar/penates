using Penates.Interfaces.Services;
using Penates.Models.ViewModels.Users;
using Penates.Services;
using Penates.Services.Security;
using Penates.Utils;
using Penates.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.Security
{
    [RoleValidation(RoleType.SU, RoleType.Admin)]
    public class SecurityController : Controller
    {
        public ActionResult Index()
        {
            SecurityParametersViewModel model = new SecurityParametersViewModel();
            try {
                ISecurityService service = new SecurityService();
                model = service.getParameters();
            }catch(Exception e){
                ModelState.AddModelError("error", Resources.Errors.SecurityParametersError +" "+e.Message);
            }
            return View("~/Views/User/ChangeSecurityParameters.cshtml", model);
        }

        /// <summary> Trato el POST del formulario de Producto </summary>
        /// <param name="prod">El modelo con los datos del producto</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ChangeParameters(SecurityParametersViewModel param) {
            if (ModelState.IsValid) {
                try {
                    ISecurityService service = new SecurityService();

                    Status result = service.setParameters(param);

                    if (result.Success) { //Si es alguno de los errores conocidos salta x Exception
                        return RedirectToAction("SecurityParametersSummary");
                    } else {
                        ModelState.AddModelError("error", result.Message);
                    }
                } catch (Exception e) {
                    ModelState.AddModelError("error", e);
                }
            }
            return View("~/Views/User/ChangeSecurityParameters.cshtml", param);
        }

        public ActionResult SecurityParametersSummary() {
            SecurityParametersViewModel model = new SecurityParametersViewModel();
            try {
                ISecurityService service = new SecurityService();
                model = service.getParameters();
            } catch (Exception e) {
                ModelState.AddModelError("error", Resources.Errors.SecurityParametersError + " " + e.Message);
            }
            return View("~/Views/User/SecurityParametersSummary.cshtml", model);
        }
    }
}