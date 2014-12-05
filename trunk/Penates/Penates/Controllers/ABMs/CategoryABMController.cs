using Penates.Database;
using Penates.Exceptions;
using Penates.Exceptions.Database;
using Penates.Exceptions.System;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels;
using Penates.Models.ViewModels.ABMs;
using Penates.Services;
using Penates.Services.ABMs;
using Penates.Utils;
using Penates.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.ABMs
{
    public class CategoryABMController : Controller
    {
        ICategoryService service = new CategoryService();

        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult Index(ABMViewModel model) {
            return View("~/Views/ABMs/CategoryABM.cshtml",model);
        }

        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult ABMAjax(jQueryDataTableParamModel param) {
            try {
                ICategoryService service = new CategoryService();
                IQueryable<ProductCategory> query;
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortDirection = Request["sSortDir_0"];
                query = service.getData();
                if (!string.IsNullOrEmpty(param.sSearch)) {
                    query = service.search(query, param.sSearch);
                }
                long total = query.Count();
                query = service.sort(query, sortColumnIndex, sortDirection);
                Paginator<ProductCategory> pag = new Paginator<ProductCategory>(query);
                query = pag.page(param.iDisplayStart, param.iDisplayLength);
                List<List<string>> result = service.toJsonArray(query);
                long count = result.Count();
                return Json(new {
                    sEcho = param.sEcho,
                    iTotalRecords = count,
                    iTotalDisplayRecords = total,
                    aaData = result
                },
                            JsonRequestBehavior.AllowGet);
            } catch (DatabaseException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "CategoryABMController", "ABMAjax");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            } catch (Exception e) {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, "CategoryABMController", "ABMAjax");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }

        /// <summary>Elimina el proveedor con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult Delete(long? DeleteCategoryID) {
            ICategoryService service = new CategoryService();
            try {
                bool result = service.Delete(DeleteCategoryID);
                    ABMViewModel model = new ABMViewModel();
                    if (result) {
                        return Json(new {
                            title = Resources.Messages.OperationSuccessfull,
                            message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.CategoryWArt, DeleteCategoryID.Value)
                        }, JsonRequestBehavior.AllowGet);
                    } else {
                        Response.StatusCode = 511;
                        Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                        Response.Write(String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.CategoryWArt, DeleteCategoryID.Value));
                        return null;
                    }

            } catch (DeleteConstrainException) {
                Response.StatusCode = 601;
                List<ConstraintViewModel> models = service.getConstrains(DeleteCategoryID.Value);
                return View("~/Views/Error/ConstraintsDisplay.cshtml", models);
            } catch (NullIDException ne){
                Response.StatusCode = 511;
                Response.StatusDescription = ne.title;
                Response.Write(ne.Message);
                return null;
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

        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult ParentsLoad(jQueryDataTableParamModel param, long? CategoryID) {
            try {
                ICollection<ProductCategory> parents = this.service.getParents(CategoryID.Value);
                IEnumerable<List<string>> result = this.service.toJsonArray(parents);
                return Json(new {
                    sEcho = param.sEcho,
                    //iTotalRecords = count,
                    //iTotalDisplayRecords = total,
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

        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult ChildrenLoad(jQueryDataTableParamModel param, long? CategoryID) {
            try {
                ICollection<ProductCategory> parents = this.service.getChildren(CategoryID.Value);
                IEnumerable<List<string>> result = this.service.toJsonArray(parents);
                return Json(new {
                    sEcho = param.sEcho,
                    //iTotalRecords = count,
                    //iTotalDisplayRecords = total,
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

        [RoleValidation(RoleType.All)]
        public JsonResult Autocomplete(string term, long? CategoryID) {
            try {
                var items = this.service.getAutocomplete(term, CategoryID);
                var json = service.toJsonAutocomplete(items);
                return Json(json, JsonRequestBehavior.AllowGet);
            } catch (DatabaseException ex) {
                Response.StatusCode = 511;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            } catch (Exception e) {
                Response.StatusCode = 511;
                Response.StatusDescription = Resources.Errors.DatabaseError;
                Response.Write(e.Message);
                return null;
            }
        }

        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult AssignParent(SearchAndAddViewModel model, long? CategoryID) {
            if (ModelState.IsValid) {
                if (model.AddID.HasValue) {
                    if (CategoryID.HasValue) {
                        try {
                            //El filterID es la Categoria a la que se le agrega y el Add es el que se va a agregar
                            this.service.addParent(CategoryID.Value, model.AddID.Value);
                            Response.StatusCode = 200;
                            return Json(new {
                                title = Resources.Messages.OperationSuccessfull,
                                message = String.Format(Resources.Messages.SuccessfullAdd, Resources.Resources.CategoryParentWArt)
                            }, JsonRequestBehavior.AllowGet);
                        } catch (HierarchyException he){
                            Response.StatusCode = 511;
                            Response.StatusDescription = he.title;
                            Response.Write(he.Message);
                            return null;
                        } catch (IDNotFoundException infe) {
                            Response.StatusCode = 511;
                            Response.StatusDescription = infe.title;
                            Response.Write(infe.Message);
                            return null;
                        } catch (DatabaseException ex) {
                            Response.StatusCode = 511;
                            Response.StatusDescription = ex.title;
                            Response.Write(ex.Message);
                            return null;
                        } catch (Exception e) {
                            Response.StatusCode = 511;
                            Response.StatusDescription = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.CategoryParent, Resources.Resources.Category);
                            Response.Write(e.Message);
                            return null;
                        }
                    } else {
                        Response.StatusCode = 511;
                        Response.StatusDescription = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.CategoryParent, Resources.Resources.Category);
                        Response.Write(String.Format(Resources.Errors.FilterIDNull, Resources.Resources.CategoryWArt, Resources.Resources.Category));
                        return null;
                    }
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.CategoryParent, Resources.Resources.Category);
                    Response.Write(String.Format(Resources.Errors.AddIDNull, Resources.Resources.CategoryParent));
                    return null;
                }
            } else {
                Response.StatusCode = 505;
                StringBuilder sb = new StringBuilder();
                int i = 1;
                foreach (KeyValuePair<string, ModelState> item in ModelState.ToList()) {
                    if (item.Value.Errors != null && item.Value.Errors.Count > 0) {
                        sb.Append("<strong>" + item.Key + ": ");
                        sb.AppendLine("</strong>");
                        foreach (ModelError mec in item.Value.Errors) {
                            sb.Append("<p>\t" + i + ") " + mec.ErrorMessage);
                            sb.AppendLine("</p>");
                            i++;
                        }
                    }
                }
                Response.StatusDescription = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.CategoryParent, Resources.Resources.Category);
                Response.Write(sb.ToString());
                return null;
            }
        }

        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult AssignChild(SearchAndAddViewModel model, long? CategoryID) {
            if (ModelState.IsValid) {
                if (model.AddID.HasValue) {
                    if (CategoryID.HasValue) {
                        try {
                            //El filterID es la Categoria a la que se le agrega y el Add es el que se va a agregar
                            this.service.addChild(CategoryID.Value, model.AddID.Value);
                            Response.StatusCode = 200;
                            return Json(new {
                                title = Resources.Messages.OperationSuccessfull,
                                message = String.Format(Resources.Messages.SuccessfullAdd, Resources.Resources.CategoryChildWArt)
                            }, JsonRequestBehavior.AllowGet);
                        } catch (HierarchyException he) {
                            Response.StatusCode = 511;
                            Response.StatusDescription = he.title;
                            Response.Write(he.Message);
                            return null;
                        } catch (IDNotFoundException infe) {
                            Response.StatusCode = 511;
                            Response.StatusDescription = infe.title;
                            Response.Write(infe.Message);
                            return null;
                        } catch (DatabaseException ex) {
                            Response.StatusCode = 511;
                            Response.StatusDescription = ex.title;
                            Response.Write(ex.Message);
                            return null;
                        } catch (Exception e) {
                            Response.StatusCode = 511;
                            Response.StatusDescription = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.CategoryChild, Resources.Resources.Category);
                            Response.Write(e.Message);
                            return null;
                        }
                    } else {
                        Response.StatusCode = 511;
                        Response.StatusDescription = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.CategoryChild, Resources.Resources.Category);
                        Response.Write(String.Format(Resources.Errors.FilterIDNull, Resources.Resources.CategoryWArt, Resources.Resources.Category));
                        return null;
                    }
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.CategoryChild, Resources.Resources.Category);
                    Response.Write(String.Format(Resources.Errors.AddIDNull, Resources.Resources.CategoryChild));
                    return null;
                }
            } else {
                Response.StatusCode = 505;
                StringBuilder sb = new StringBuilder();
                int i = 1;
                foreach (KeyValuePair<string, ModelState> item in ModelState.ToList()) {
                    if (item.Value.Errors != null && item.Value.Errors.Count > 0) {
                        sb.Append("<strong>" + item.Key + ": ");
                        sb.AppendLine("</strong>");
                        foreach (ModelError mec in item.Value.Errors) {
                            sb.Append("<p>\t" + i + ") " + mec.ErrorMessage);
                            sb.AppendLine("</p>");
                            i++;
                        }
                    }
                }
                Response.StatusDescription = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.CategoryChild, Resources.Resources.Category);
                Response.Write(sb.ToString());
                return null;
            }
        }

        /// <summary>Elimina el producto con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult UnassignParent(long DeleteCategoryID, long CategoryID) { //Los parametros se tienen que llamar asi para cada Unassign
            try {
                //El filterID es al Producto que se le agrega y el Add es el que se va a agregar
                bool resultado = this.service.unasignParent(CategoryID, DeleteCategoryID);
                if (resultado) {
                    Response.StatusCode = 200;
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = String.Format(Resources.Messages.SuccessfullUnassign, Resources.Resources.CategoryParentWArt)
                    }, JsonRequestBehavior.AllowGet);
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                    Response.Write(String.Format(Resources.Errors.UnassignUnsaccessfull, Resources.Resources.CategoryParentWArt));
                    return null;
                }
            } catch (IDNotFoundException infe) {
                Response.StatusCode = 511;
                Response.StatusDescription = infe.title;
                Response.Write(infe.Message);
                return null;
            } catch (DatabaseException ex) {
                Response.StatusCode = 511;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            } catch (Exception e) {
                Response.StatusCode = 511;
                Response.StatusDescription = String.Format(Resources.ExceptionMessages.UnassignException, Resources.Resources.CategoryParent, Resources.Resources.Category);
                Response.Write(e.Message);
                return null;
            }
        }

        [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
        public ActionResult UnassignChild(long DeleteCategoryID, long CategoryID) { //Los parametros se tienen que llamar asi para cada Unassign
            try {
                bool resultado = this.service.unasignChild(CategoryID, DeleteCategoryID);
                if (resultado) {
                    Response.StatusCode = 200;
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = String.Format(Resources.Messages.SuccessfullUnassign, Resources.Resources.CategoryChildWArt)
                    }, JsonRequestBehavior.AllowGet);
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                    Response.Write(String.Format(Resources.Errors.UnassignUnsaccessfull, Resources.Resources.CategoryChildWArt));
                    return null;
                }
            } catch (IDNotFoundException infe) {
                Response.StatusCode = 511;
                Response.StatusDescription = infe.title;
                Response.Write(infe.Message);
                return null;
            } catch (DatabaseException ex) {
                Response.StatusCode = 511;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            } catch (Exception e) {
                Response.StatusCode = 511;
                Response.StatusDescription = String.Format(Resources.ExceptionMessages.UnassignException, Resources.Resources.CategoryChild, Resources.Resources.Category);
                Response.Write(e.Message);
                return null;
            }
        }
	}
}