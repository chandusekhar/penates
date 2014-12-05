using Penates.Exceptions;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.Users;
using Penates.Services;
using Penates.Services.ABMs;
using Penates.Services.Users;
using Penates.Utils;
using Penates.Utils.Enums;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.Security
{
    [RoleValidation(RoleType.Admin, RoleType.SU)]
    public class ValueSecurityController : Controller {
        public ActionResult Index() {
            ABMViewModel model = new ABMViewModel();
            return View("~/Views/User/ValueSecurity/ValueSecurityABM.cshtml", model);
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult ABMAjax(jQueryDataTableParamModel param, long? UserActive, long? DistributionCenterID) {
            try {
                IUserValSecurityService service = new UserValSecurityService();
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortDirection = Request["sSortDir_0"];
                IQueryable<ValSecurityTableJson> query = service.getData();
                if (!string.IsNullOrEmpty(param.sSearch)) {
                    query = service.search(query, param.sSearch);
                }
                query = service.sort(query, sortColumnIndex, sortDirection);
                long total = query.Count(); //El total sin paginacion
                Paginator<ValSecurityTableJson> pag = new Paginator<ValSecurityTableJson>(query);
                query = pag.page(param.iDisplayStart, param.iDisplayLength);
                List<ValSecurityTableJson> result = query.ToList();
                long count = result.Count();
                return Json(new {
                    sEcho = param.sEcho,
                    iTotalRecords = count,
                    iTotalDisplayRecords = total,
                    aaData = result
                },
                            JsonRequestBehavior.AllowGet);
            } catch (DatabaseException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            } catch (Exception e) {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult Delete(string UserID) {
            if (Request.IsAjaxRequest()) {
                return this.AjaxDelete(UserID);
            } else {
                return this.NormalDelete(UserID);
            }
        }

        /// <summary>Elimina el usuario con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        private ActionResult AjaxDelete(string UserID) {
            IUserValSecurityService service = new UserValSecurityService();
            try {
                Status result = service.Delete(UserID);
                if (result.Success) {
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = String.Format(" ")
                    }, JsonRequestBehavior.AllowGet);
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                    Response.Write(result.Message);
                    return null;
                }
            } catch (DatabaseException ex) {
                Response.StatusCode = 511;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            } catch (Exception e) {
                Response.StatusCode = 511;
                Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                Response.Write(e.Message);
                return null;
            }
        }

        /// <summary>Elimina el producto con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        private ActionResult NormalDelete(string UserID) {
            IUserValSecurityService service = new UserValSecurityService();
            try {
                Status result = service.Delete(UserID);
                ABMViewModel model = new ABMViewModel();
                if (result.Success) {
                    model.Message = String.Format(Resources.Messages.OperationSuccessfull);
                    model.Error = false;
                } else {
                    model.Message = result.Message;
                    model.Error = true;
                }
                return RedirectToAction("Index", "ValueSecurity", model);
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(ex.title, ex.Message, ex, "ValueSecurity", "NormalDelete");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "ValueSecurity", "NormalDelete");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        public ActionResult BulkDeactivate(List<string> UserIDs) {
            IUserValSecurityService service = new UserValSecurityService();
            try {
                Status result = service.Delete(UserIDs);
                if (result.Success) {
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = ""
                    }, JsonRequestBehavior.AllowGet);
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                    Response.Write(result.Message);
                    return null;
                }
            } catch (DatabaseException ex) {
                Response.StatusCode = 511;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            } catch (Exception e) {
                Response.StatusCode = 511;
                Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                Response.Write(e.Message);
                return null;
            }
        }

        public ActionResult AddRule(string UserID) {
            if (!String.IsNullOrEmpty(UserID)) {
                return this.FormEdit(UserID);
            }
            ValSecurityViewModel model = new ValSecurityViewModel();
            return View("~/Views/User/ValueSecurity/ValueSecurityForm.cshtml", model);
        }

        public ActionResult FormEdit(string UserID) {
            IUserValSecurityService service = new UserValSecurityService();
            try {
                ValSecurityViewModel model = service.getSecurityData(UserID);
                if (model == null) {//Si no encontre agrego el error para que lo muestre
                    model = new ValSecurityViewModel();
                    ModelState.AddModelError("error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.RuleWArt, UserID));
                }
                return View("~/Views/User/ValueSecurity/ValueSecurityForm.cshtml", model);
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
        public ActionResult Save(ValSecurityViewModel sec) {
            if (ModelState.IsValid) {
                IUserValSecurityService service = new UserValSecurityService();
                try {
                    Status result = service.Save(sec);

                    if (result.Success) { //Si es alguno de los errores conocidos salta x Exception
                        return RedirectToAction("FormSummary", "ValueSecurity", new { UserID = sec.FileNumber });
                    } else {
                        ModelState.AddModelError("error", result.Message);
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
            ICategoryService categoryService = new CategoryService();
            sec.initialCategories = categoryService.getCategoriesSelect(sec.Categories);
            return View("~/Views/User/ValueSecurity/ValueSecurityForm.cshtml", sec);
        }

        /// <summary> Muestra un Summary con los datos que se agregaron recientemente o editaron /// </summary>
        /// <param name="prod">Modelo con los datos a mostrar</param>
        /// <param name="ProdImage">La imagen se manda aparte para evitar errores</param>
        /// <returns>La vista del summary</returns>
        public ActionResult FormSummary(string UserID) {
            IUserValSecurityService service = new UserValSecurityService();
            ValSecurityViewModel model = service.getSecuritySummary(UserID);
            return View("~/Views/User/ValueSecurity/ValueSecurityFormSummary.cshtml", model);
        }
    }
}