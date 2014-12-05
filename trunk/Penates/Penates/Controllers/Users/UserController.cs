using Penates.Database;
using Penates.Exceptions;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.Home;
using Penates.Models.ViewModels.Users;
using Penates.Services;
using Penates.Services.DC;
using Penates.Services.Users;
using Penates.Utils;
using Penates.Utils.Enums;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.Users
{
    public class UserController : BaseController
    {

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult Index(ABMViewModel model) {
            if (model == null) {
                model = new ABMViewModel();
            }
            RoleService service = new RoleService();
            model.List = service.getRolesList(true);

            List<SelectListItem> activeList = new List<SelectListItem>();
            SelectListItem i = new SelectListItem();
            i.Text = Resources.Resources.All;
            i.Value = UserTypes.ALL.getTypeNumber().ToString();
            activeList.Add(i);
            i = new SelectListItem();
            i.Text = Penates.App_GlobalResources.Forms.ModelFormsResources.Active;
            i.Value = UserTypes.ACTIVE.getTypeNumber().ToString(); ;
            activeList.Add(i);
            i = new SelectListItem();
            i.Text = Penates.App_GlobalResources.Forms.ModelFormsResources.Inactive;
            i.Value = UserTypes.INACTIVE.getTypeNumber().ToString(); ;
            activeList.Add(i);
            ViewBag.ActiveList = activeList;

            return View(model);
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult Register(string error) {
            RegisterViewModel model = new RegisterViewModel();
            ModelState.AddModelError("", error);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult Register(RegisterViewModel model) {
            try {
                if (ModelState.IsValid) {
                    UserService service = new UserService();
                    Status stat = service.Register(model);
                    if (stat.Success) {
                        UserViewModel user = new UserViewModel() {
                            Address = model.Address,
                            Email = model.Email,
                            Active = true,
                            FileNumber = model.FileNumber,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Telephone = model.Telephone,
                            UserName = model.UserName,
                            LastLoginDate = System.DateTime.Now
                        };
                        return RedirectToAction("UserSummary", "User", user);
                    } else {
                        ModelState.AddModelError("", stat.Message);
                    }
                }
                return View(model);
            } catch (UniqueRestrictionException ex) {
                ModelState.AddModelError("UserName", ex.title + ": " + ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult Save(UserViewModel model) {
            try {
                if (ModelState.IsValid) {
                    UserService service = new UserService();
                    Status stat = service.Save(model);
                    if (stat.Success) {
                        return RedirectToAction("UserSummary", "User", model);
                    } else {
                        ModelState.AddModelError("", stat.Message);
                    }
                }
            } catch (IDNotFoundException) {
                RegisterViewModel registeModel = new RegisterViewModel() {
                    Address = model.Address,
                    Email = model.Email,
                    FileNumber = model.FileNumber,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Telephone = model.Telephone,
                    UserName = model.UserName
                };
                ModelState.AddModelError("FileNumber", String.Format(Resources.Messages.EditNotFound,Resources.Resources.UserWArt));
            }
            return View("~/Views/User/FormEdit.cshtml",model); 
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult FormEdit(string UserID) {
            UserService service = new UserService();
            try {
                UserViewModel model = service.getData(UserID);
                if (model == null) {//Si no encontre agrego el error para que lo muestre
                    return this.Register(String.Format(Resources.Messages.ItemNotFound, Resources.Resources.UserWArt, UserID));
                }
                return View(model);
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, ex.Message, "FormsController", "ProductFormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "ProductFormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult UserSummary(UserViewModel model){
            UserService service = new UserService();
            model.Active = service.isActive(model.FileNumber);
            return View("~/Views/User/UserSummary.cshtml", model);
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult ABMAjax(jQueryDataTableParamModel param, long? UserActive, long? DistributionCenterID) {
            try {
                UserService service = new UserService();
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortDirection = Request["sSortDir_0"];
                IQueryable<User> query = service.getData();
                if (!string.IsNullOrEmpty(param.sSearch)) {
                    query = service.search(query, param.sSearch);
                }
                query = service.filterActive(query, UserActive);
                query = service.filterByRole(query, param.SelectedValue);
                query = service.filterByDistributionCenter(query, DistributionCenterID);
                query = service.sort(query, sortColumnIndex, sortDirection);
                long total = query.Count(); //El total sin paginacion
                Paginator<User> pag = new Paginator<User>(query);
                query = pag.page(param.iDisplayStart, param.iDisplayLength);
                List<UserTableJson> result = service.toJsonArray(query);
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

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult Activate(string UserID) {
            if (Request.IsAjaxRequest()) {
                return this.AjaxActivate(UserID);
            } else {
                return this.NormalActivate(UserID);
            }
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult ChangePassword(string UserName) {
            ChangePasswordViewModel model = new ChangePasswordViewModel(){
                UserName = UserName
            };
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult ChangePassword(ChangePasswordViewModel model) {
            try {
                if (ModelState.IsValid) {
                    UserService service = new UserService();
                    if (service.changePassword(model.UserName, model.NewPassword)) {
                        ABMViewModel abmModel = new ABMViewModel();
                        abmModel.Error = false;
                        abmModel.Message = Resources.Messages.OperationSuccessfull + ": " + Resources.Messages.PasswordChangeSuccessfull;
                        return RedirectToAction("Index","User",abmModel);
                    } else {
                        ModelState.AddModelError("UserName", Resources.Errors.InvalidLogin);
                    }
                }
                return View(model);
            } catch (Exception e) {
                ModelState.AddModelError("", e.Message);
                return View(model);
            }
        }


        /// <summary>Elimina el usuario con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        private ActionResult AjaxDelete(string UserID) {
            UserService service = new UserService();
            try {
                if (UserID != null) {
                    bool result = service.Delete(UserID);
                    if (result) {
                        return Json(new {
                            title = Resources.Messages.OperationSuccessfull,
                            message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.UserWArt, UserID)
                        }, JsonRequestBehavior.AllowGet);
                    } else {
                        Response.StatusCode = 511;
                        Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                        Response.Write(String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.UserWArt, UserID));
                        return null;
                    }
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = String.Format(Resources.ExceptionMessages.DeleteExceptionNoID, Resources.Resources.UserWArt);
                    Response.Write(String.Format(Resources.Errors.DeleteIDNull, Resources.Resources.UserWArt));
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
            UserService service = new UserService();
            try {
                bool result = service.Delete(UserID);
                ABMViewModel model = new ABMViewModel();
                if (result) {
                    model.Message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.UserWArt, UserID);
                    model.Error = false;
                } else {
                    model.Message = String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.UserWArt, UserID);
                    model.Error = true;
                }
                return RedirectToAction("Index", "User", model);
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(ex.title, ex.Message, ex, "FormsController", "DeleteProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "DeleteProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        /// <summary>Activa o desactiva el usuario con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a activar o desactivar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        private ActionResult AjaxActivate(string UserID) {
            UserService service = new UserService();
            try {
                if (UserID != null) {
                    bool result = service.ActivateDeactivate(UserID);
                    if (result) {
                        if (service.isActive(UserID)) {
                            return Json(new {
                                title = Resources.Messages.OperationSuccessfull,
                                message = String.Format(Resources.Messages.ActivatedItem, Resources.Resources.UserWArt, UserID)
                            }, JsonRequestBehavior.AllowGet);
                        } else {
                            return Json(new {
                                title = Resources.Messages.OperationSuccessfull,
                                message = String.Format(Resources.Messages.DeactivatedItem, Resources.Resources.UserWArt, UserID)
                            }, JsonRequestBehavior.AllowGet);
                        }
                    } else {
                        Response.StatusCode = 511;
                        Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                        Response.Write("Error");
                        return null;
                    }
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = String.Format(Resources.ExceptionMessages.DeleteExceptionNoID, Resources.Resources.UserWArt);
                    Response.Write(String.Format(Resources.Errors.DeleteIDNull, Resources.Resources.UserWArt));
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

        /// <summary>Activa o desactiva el producto con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a activar o desactivar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        private ActionResult NormalActivate(string UserID) {
            UserService service = new UserService();
            try {
                bool result = service.ActivateDeactivate(UserID);
                //UserViewModel model = service.getData(UserID);
                if (!result) {
                    ModelState.AddModelError("", Resources.Errors.OperationUnsaccessfull);
                }
                //if (model == null) {//Si no encontre agrego el error para que lo muestre
                    //return RedirectToAction("Index","User");
                //}
                return RedirectToAction("FormEdit", "User", new { UserID = UserID });
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(ex.title, ex.Message, ex, "User", "NormalActivate");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "User", "NormalActivate");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        /// <summary>Activa o desactiva el usuario con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a activar o desactivar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult BulkActivate(List<string> UserIDs) {
            UserService service = new UserService();
            try {
                Status result = service.Activate(UserIDs);
                if (result.Success) {
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = String.Format(Resources.Messages.ActivatedItems, Resources.Resources.UsersWArt)
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

        public ActionResult BulkDeactivate(List<string> UserIDs) {
            UserService service = new UserService();
            try {
                Status result = service.Deactivate(UserIDs);
                if (result.Success) {
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = String.Format(Resources.Messages.DeactivatedItems, Resources.Resources.UsersWArt)
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

        /// <summary>Retorna el Json con el Autocomplete</summary>
        /// <param name="term"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public JsonResult Autocomplete(string term) {
            try {
                UserService service = new UserService();
                var items = service.getAutocomplete(term);
                var json = service.toJsonAutocomplete(items);
                return Json(json, JsonRequestBehavior.AllowGet);
            } catch (DatabaseException ex) {
                Response.StatusCode = 504;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            } catch (Exception e) {
                Response.StatusCode = 504;
                Response.StatusDescription = Resources.Errors.DatabaseError;
                Response.Write(e.Message);
                return null;
            }
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult ChangePasswordUserSelect() {
            ViewBag.Action = Url.Action("ChangePassword", "User");
            return View("~/Views/User/UserPicker.cshtml");
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult AdvancedSearch(ABMViewModel model) {
            if (model == null) {
                model = new ABMViewModel();
            }
            RoleService service = new RoleService();
            model.List = service.getRolesList(true);

            List<SelectListItem> activeList = new List<SelectListItem>();
            SelectListItem i = new SelectListItem();
            i.Text = Resources.Resources.All;
            i.Value = UserTypes.ALL.getTypeNumber().ToString();
            activeList.Add(i);
            i = new SelectListItem();
            i.Text = Penates.App_GlobalResources.Forms.ModelFormsResources.Active;
            i.Value = UserTypes.ACTIVE.getTypeNumber().ToString(); ;
            activeList.Add(i);
            i = new SelectListItem();
            i.Text = Penates.App_GlobalResources.Forms.ModelFormsResources.Inactive;
            i.Value = UserTypes.INACTIVE.getTypeNumber().ToString(); ;
            activeList.Add(i);
            ViewBag.ActiveList = activeList;

            return View(model);
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult AssignUserSelect() {
            ViewBag.Action = Url.Action("AssignToUser", "User");
            return View("~/Views/User/UserPicker.cshtml");
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult AssignToUser(string FileNumber) {
            try {
                if (String.IsNullOrWhiteSpace(FileNumber)) {
                    throw new MyException(String.Format(Resources.Errors.IDNull, Resources.Resources.UserWArt));
                }
                UserService service = new UserService();
                UserViewModel user = service.getData(FileNumber);
                if (user == null) {
                    throw new IDNotFoundException(String.Format(Resources.Errors.AddIDNull, Resources.Resources.Role));
                }
                if (String.Compare(user.UserName, HttpContext.User.Identity.Name, true) == 0) {
                    ViewBag.Action = Url.Action("AssignToUser", "User");
                    ViewBag.Error = Resources.Errors.CantChangeOwnRoles;
                    return View("~/Views/User/UserPicker.cshtml");
                }
                AssignViewModel model = new AssignViewModel() {
                    UserFileNumber = FileNumber,
                    UserName = user.UserName
                };
                IRoleService roleService = new RoleService();
                model.Selectlist = roleService.getRolesList();
                return View("~/Views/User/AssignToUser.cshtml", model);
            } catch (MyException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "User", "AssignToUser");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            } catch (Exception e) {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, "User", "AssignToUser");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult AssignRolesUserSelect() {
            ViewBag.Action = Url.Action("AssignRoles", "User");
            return View("~/Views/User/UserPicker.cshtml");
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult AssignRoles(string FileNumber) {
            try {
                if (String.IsNullOrWhiteSpace(FileNumber)) {
                    throw new MyException(String.Format(Resources.Errors.IDNull, Resources.Resources.UserWArt));
                }
                UserService service = new UserService();
                UserViewModel user = service.getData(FileNumber);
                if (user == null) {
                    throw new IDNotFoundException(String.Format(Resources.Errors.AddIDNull, Resources.Resources.Role));
                }
                if(String.Compare(user.UserName, HttpContext.User.Identity.Name, true) == 0){
                    ViewBag.Action = Url.Action("AssignRoles", "User");
                    ViewBag.Error = Resources.Errors.CantChangeOwnRoles;
                    return View("~/Views/User/UserPicker.cshtml");
                }
                AssignViewModel model = new AssignViewModel() {
                    UserFileNumber = FileNumber,
                    UserName = user.UserName
                };
                IRoleService roleService = new RoleService();
                model.Selectlist = roleService.getRolesList();
                return View("~/Views/User/Roles/AssignRoles.cshtml", model);
            } catch (MyException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            } catch (Exception e) {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult UserRoles(jQueryDataTableParamModel param, string UserID) {
            try {
                UserService service = new UserService();
                ICollection<Role> query = service.getRoles(UserID);
                IRoleService roleService = new RoleService();
                List<RoleTableJson> result = roleService.toJsonArray(query);
                long count = result.Count();
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

        /// <summary>Saca al Usuario de un determinado rol</summary>
        /// <param name="UserID">ID del usuario a sacarle el rol</param>
        /// <param name="RoleID">ID del Rol a eliminar</param>
        /// <returns></returns>
        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult AddRoleToUser(string UserID, long AddID) {
            UserService service = new UserService();
            try {
                Status resultado = service.AddUserToRole(UserID, AddID);
                if (resultado.Success) {
                    Response.StatusCode = 200;
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = String.Format(Resources.Messages.SuccessfullAssign, Resources.Resources.RoleWArt)
                    }, JsonRequestBehavior.AllowGet);
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                    Response.Write(String.Format(Resources.Errors.UnassignUnsaccessfull, Resources.Resources.RoleWArt));
                    return null;
                }
            } catch (Exception e) {
                Response.StatusCode = 511;
                Response.StatusDescription = String.Format(Resources.ExceptionMessages.UnassignException, Resources.Resources.Role, Resources.Resources.User);
                Response.Write(e.Message);
                return null;
            }
        }

        /// <summary>Saca al Usuario de un determinado rol</summary>
        /// <param name="UserID">ID del usuario a sacarle el rol</param>
        /// <param name="RoleID">ID del Rol a eliminar</param>
        /// <returns></returns>
        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult UnassignRole(string UserID, long RoleID) {
            UserService service = new UserService();
            try {
                Status resultado = service.RemoveUserFromRole(UserID, RoleID);
                if (resultado.Success) {
                    Response.StatusCode = 200;
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = String.Format(Resources.Messages.SuccessfullUnassign, Resources.Resources.RoleWArt)
                    }, JsonRequestBehavior.AllowGet);
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                    Response.Write(String.Format(Resources.Errors.UnassignUnsaccessfull, Resources.Resources.RoleWArt));
                    return null;
                }
            } catch (Exception e) {
                Response.StatusCode = 511;
                Response.StatusDescription = String.Format(Resources.ExceptionMessages.UnassignException, Resources.Resources.Role, Resources.Resources.User);
                Response.Write(e.Message);
                return null;
            }
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult AssignDCUserSelect() {
            ViewBag.Action = Url.Action("AssignDC", "User");
            return View("~/Views/User/UserPicker.cshtml");
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult AssignDC(string FileNumber) {
            try {
                if (String.IsNullOrWhiteSpace(FileNumber)) {
                    throw new MyException(String.Format(Resources.Errors.IDNull, Resources.Resources.UserWArt));
                }
                UserService service = new UserService();
                UserViewModel user = service.getData(FileNumber);
                if (user == null) {
                    throw new IDNotFoundException(String.Format(Resources.Errors.AddIDNull, Resources.Resources.User));
                }
                AssignViewModel model = new AssignViewModel() {
                    UserFileNumber = FileNumber,
                    UserName = user.UserName
                };
                return View("~/Views/User/DC/AssignDC.cshtml", model);
            } catch (MyException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "UserController", "AssignDC");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            } catch (Exception e) {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, "UserController", "AssignDC");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }

        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult UserDC(jQueryDataTableParamModel param, string UserID) {
            try {
                UserService service = new UserService();
                ICollection<DistributionCenter> query = service.getDistributionCenters(UserID);
                IDistributionCenterService dcService = new DistributionCenterService();
                List<DistributionCenterTableJson> result = dcService.toJsonArray(query);
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

        /// <summary>Saca al Usuario de un determinado rol</summary>
        /// <param name="UserID">ID del usuario a sacarle el rol</param>
        /// <param name="RoleID">ID del Rol a eliminar</param>
        /// <returns></returns>
        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult AddDCToUser(string UserID, long AddID) {
            UserService service = new UserService();
            try {
                Status resultado = service.AttachDistributionCenter(UserID, AddID);
                if (resultado.Success) {
                    Response.StatusCode = 200;
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = String.Format(Resources.Messages.SuccessfullAssign, Resources.Resources.DistributionCenterWArt)
                    }, JsonRequestBehavior.AllowGet);
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                    Response.Write(String.Format(Resources.Errors.UnassignUnsaccessfull, Resources.Resources.DistributionCenterWArt));
                    return null;
                }
            } catch (Exception e) {
                Response.StatusCode = 511;
                Response.StatusDescription = String.Format(Resources.ExceptionMessages.UnassignException, Resources.Resources.DistributionCenter, Resources.Resources.User);
                Response.Write(e.Message);
                return null;
            }
        }

        /// <summary>Saca al Usuario de un determinado rol</summary>
        /// <param name="UserID">ID del usuario a sacarle el rol</param>
        /// <param name="RoleID">ID del Rol a eliminar</param>
        /// <returns></returns>
        [RoleValidation(RoleType.Admin, RoleType.SU)]
        public ActionResult UnassignDC(string UserID, long DistributionCenterID) {
            UserService service = new UserService();
            try {
                Status resultado = service.DetachDistributionCenter(UserID, DistributionCenterID);
                if (resultado.Success) {
                    Response.StatusCode = 200;
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = String.Format(Resources.Messages.SuccessfullUnassign, Resources.Resources.DistributionCenterWArt)
                    }, JsonRequestBehavior.AllowGet);
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                    Response.Write(String.Format(Resources.Errors.UnassignUnsaccessfull, Resources.Resources.DistributionCenterWArt));
                    return null;
                }
            } catch (Exception e) {
                Response.StatusCode = 511;
                Response.StatusDescription = String.Format(Resources.ExceptionMessages.UnassignException, Resources.Resources.DistributionCenter, Resources.Resources.User);
                Response.Write(e.Message);
                return null;
            }
        }
    }
}