using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.Forms;
using Penates.Services;
using Penates.Services.ABMs;
using Penates.Utils.Attributes;
using Penates.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.Forms
{
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
    public class CategoryFormController : Controller
    {
        ICategoryService categoryService = new CategoryService();

        public ActionResult Index()
        {
            CategoryViewModel model = new CategoryViewModel();
            return View("~/Views/ABMs/Forms/ProductCategoryForm.cshtml", model);
        }

        [HttpPost]
        public ActionResult SubmitCategory(CategoryViewModel model) {
            if (ModelState.IsValid) {
                //Agrego la categoria a la bd
                try {
                    var result = categoryService.Save(model);
                    if (result >= 0) {
                        if (model.ProductCategoriesID == 0) {
                            model.ProductCategoriesID = result;
                            return RedirectToAction("FormEdit", new { id = model.ProductCategoriesID });
                        } else {
                            return RedirectToAction("Index","CategoryABM");
                        }
                    } else {
                        ModelState.AddModelError("Description", String.Format(Resources.Errors.AllreadyExists,model.Description));
                    }
                } catch (DatabaseException ex) {
                    //El unico error que puedo tener es con la base de datos, ya que si el ID existe hace un Update
                    ModelState.AddModelError("error", Resources.Errors.DatabaseError + ": " + ex.Message);
                } catch (Exception e) {
                    ErrorModel error = new ErrorModel("Error saving Product in database", e, "FormsController", "SubmitProduct");
                    return View("~/Views/Error/ErrorDisplay.cshtml", error);
                }
            }
            return View("~/Views/ABMs/Forms/ProductCategoryForm.cshtml", model);
        }

        public ActionResult CategoryForm() {
            CategoryViewModel model = new CategoryViewModel();
            if (Request.IsAjaxRequest()) {
                return PartialView("~/Views/ABMs/Forms/ProductCategoryForm.cshtml", model);
            }
            return View("~/Views/ABMs/Forms/ProductCategoryForm.cshtml", model);
        }

        public ActionResult FormEdit(long id) {
            CategoryService categoryService = new CategoryService();
            try {
                CategoryViewModel model = categoryService.getCategoryData(id); //se supone que existe siempre
                if (Request.IsAjaxRequest()) {
                    return PartialView("~/Views/ABMs/Forms/ProductCategoryForm.cshtml", model);
                }
                return View("~/Views/ABMs/Forms/ProductCategoryForm.cshtml", model);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(e, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        /// <summary>Elimina el producto con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult Delete(long categoryID) {
            try {
                try {
                    bool result = this.categoryService.Delete(categoryID);
                    ABMViewModel model = new ABMViewModel();
                    if (result) {
                        model.Message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.CategoryWArt, categoryID);
                        model.Error = false;
                    } else {
                        model.Message = String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.CategoryWArt, categoryID);
                        model.Error = true;
                    }
                    return RedirectToAction("Index", "CategoryABM", new { Message = model.Message, Error = model.Error });

                } catch (DeleteConstrainException) {
                    List<ConstraintViewModel> models = this.categoryService.getConstrains(categoryID);
                    return View("~/Views/Error/ConstraintsDisplay.cshtml", models);
                }
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(ex.title, ex.Message, ex, "CategoryFormController", "Delete");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "CategoryFormController", "Delete");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        public ActionResult ShowHierarchy(long categoryID) {
            CategoryHierarchyViewModel model = new CategoryHierarchyViewModel();
            try {
                CategoryViewModel category = new CategoryViewModel();
                category.ProductCategoriesID = categoryID;
                category.Description = this.categoryService.getCategoryData(categoryID).Description;
                model.actual = category;
                model.padres = this.categoryService.getParentHierarchy(categoryID);
                model.hijos = this.categoryService.getChildrenHierarchy(categoryID);
                return View("~/Views/ABMs/Forms/ProductCategoryHierarchy.cshtml", model);
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(ex.title, ex.Message, ex, "CategoryFormController", "Delete");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception) {
                model.Error = true;
                model.Title = "Error";
                return View("~/Views/ABMs/Forms/ProductCategoryHierarchy.cshtml", model);
            }
        }
    }
}
