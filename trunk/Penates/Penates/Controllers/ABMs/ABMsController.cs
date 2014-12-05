using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Services;
using Penates.Services.ABMs;
using Penates.Services.Users;
using Penates.Utils;
using Penates.Utils.Enums;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Controllers.ABMs
{
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
    public class ABMsController : Controller
    {
        public ActionResult Index() {
            ABMViewModel model = new ABMViewModel();
            return this.ProductABM(model);
        }

        public ActionResult ProductABM(ABMViewModel model) {
            CategoryService cs = new CategoryService();
            model.List = cs.getCategoriesList(true);
            if (String.IsNullOrEmpty(model.AjaxRequest) || String.IsNullOrWhiteSpace(model.AjaxRequest)) {
                model.AjaxRequest = "/ABMs/ABMAjax";
            }
            return View(model);
        }

        public ActionResult ABMAjax(jQueryDataTableParamModel param) {
            try {
                ProductService productServices = new ProductService();
                List<ProductTableJson> products;
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortDirection = Request["sSortDir_0"];
                long total = 0; //El total sin paginacion
                if (!string.IsNullOrEmpty(param.sSearch)) {
                    products = productServices.getProductsDisplayData(param.sSearch, param.iDisplayStart, param.iDisplayLength, sortColumnIndex, sortDirection, param.SelectedValue, ref total);
                } else {
                    products = productServices.getProductsDisplayData(param.iDisplayStart, param.iDisplayLength, sortColumnIndex, sortDirection, param.SelectedValue, ref total);
                }
                long count = products.Count();
                return Json(new {
                    sEcho = param.sEcho,
                    iTotalRecords = count,
                    iTotalDisplayRecords = total,
                    aaData = products
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

        /// <summary>Elimina el producto con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult Delete(long? ProductID) {
            ProductService productService = new ProductService();
            try {
                if (ProductID.HasValue) {
                    try {
                        bool result = productService.Delete(ProductID.Value);
                        ABMViewModel model = new ABMViewModel();
                        if (result) {
                            return Json(new {
                                title = Resources.Messages.OperationSuccessfull,
                                message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.ProductWArt, ProductID.Value)
                            }, JsonRequestBehavior.AllowGet);
                        } else {
                            Response.StatusCode = 511;
                            Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                            Response.Write(String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.ProductWArt, ProductID.Value));
                            return null;
                        }

                    } catch (DeleteConstrainException) {
                        Response.StatusCode = 601;
                        List<ConstraintViewModel> models = productService.getConstrains(ProductID.Value);
                        return View("~/Views/Error/ConstraintsDisplay.cshtml", models);
                    }
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = String.Format(Resources.ExceptionMessages.DeleteExceptionNoID, Resources.Resources.ProductWArt);
                    Response.Write(String.Format(Resources.Errors.DeleteIDNull, Resources.Resources.ProductWArt));
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

        public ActionResult ViewProductsOfSupplier(jQueryDataTableParamModel param, long? ID) {
            try {
                ISupplierService supplierServices = new SupplierService();
                IProductService productServices = new ProductService();
                if (!ID.HasValue) {
                    return this.ABMAjax(param);
                }
                IQueryable<Product> products = supplierServices.getProducts(ID).AsQueryable();
                int sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
                string sortDirection = Request["sSortDir_0"];
                products = productServices.search(products, param.sSearch);
                products = productServices.categoryFilter(products, param.SelectedValue);
                products = productServices.sort(products, sortColumnIndex, sortDirection);
                long total = products.Count();
                Paginator<Product> paginator = new Paginator<Product>(products);
                products = paginator.page(param.iDisplayStart, param.iDisplayLength);
                List<ProductTableJson> data = productServices.toJsonArray(products);
                long count = products.Count();
                return Json(new {
                    sEcho = param.sEcho,
                    iTotalRecords = count,
                    iTotalDisplayRecords = total,
                    aaData = data
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
        public JsonResult Autocomplete(string term) {
            try {
                IProductService service = new ProductService();
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
	}
}