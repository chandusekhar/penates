using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Penates.Models.ViewModels.Forms;
using Penates.Database;
using System.Drawing;
using Penates.Utils;
using Penates.Utils.Keepers;
using Penates.Services.ABMs;
using System.Data.Entity.Core;
using Penates.Exceptions.Database;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Controllers.ABMs;
using Penates.Interfaces.Services;
using Penates.Models.ViewModels.Transactions;
using Penates.Services.Transactions;
using Penates.Utils.JSON.TableObjects;
using Penates.Services;
using Penates.Utils.Enums;
using Penates.Exceptions;


namespace Penates.Controllers
{
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
    public class OrderFormController : Controller
    {
        public ActionResult Index(OrderViewModel model){
            return this.OrderForm(model);
        }

        public ActionResult OrderForm(OrderViewModel model) {
            try {
                model.OldOrderID = model.OrderID;
                model.OldSupplierID = model.SupplierID;
                return View("~/Views/Transactions/Orders/OrderForm.cshtml", model);
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, ex.Message, "SupplierFormsController", "SupplierForm");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "SupplierFormsController", "SupplierForm");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        public JsonResult ProductAutocomplete(string term, string OrderID, long? SupplierID) {
            try {
                IOrderService service = new OrderService();
                IProductService productService = new ProductService();
                IQueryable<Product> items = service.getAutocompleteItems(term, OrderID, SupplierID);
                var json = productService.toJsonAutocomplete(items);
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

        public ActionResult FormEdit(string orderID, long supplierID) {
            IOrderService service = new OrderService();
            try {
                OrderViewModel model = service.getData(orderID, supplierID);
                if (model == null) {//Si no encontre el producto agrego el error para que lo muestre
                    model = new OrderViewModel();
                    model.Error = String.Format(Resources.Messages.ItemNotFound, Resources.Resources.OrderWArt, orderID);
                }
                return this.OrderForm(model);
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, ex.Message, "SupplierFormsController", "FormEdit");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "SupplierFormsController", "FormEdit");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        public ActionResult ProductLoad(jQueryDataTableParamModel param, string OrderID, long? SupplierID) {
            try {
                IOrderService os = new OrderService();
                ICollection<SupplierOrderItem> query = os.getItems(OrderID, SupplierID);
                IOrderItemsService ois = new OrderItemsService();
                List<OrderItemsTableJson> table = ois.toJsonArray(query);
                return Json(new {
                    sEcho = param.sEcho,
                    aaData = table
                },
                            JsonRequestBehavior.AllowGet);
            } catch (DatabaseException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "OrderFormsController", "ProductLoad");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            } catch (Exception e) {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, "OrderFormsController", "ProductLoad");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }

        /// <summary> Trato el POST del formulario de Producto </summary>
        /// <param name="prod">El modelo con los datos del producto</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(OrderViewModel model) {
            if (ModelState.IsValid) {
                IOrderService orderService = new OrderService();
                try {
                    if (orderService.Save(model)) {
                        ABMViewModel abm = new ABMViewModel();
                        abm.Message = String.Format(Resources.Messages.OperationSuccessfull, Resources.Resources.OrderWArt);
                        abm.Error = false;
                        return RedirectToAction("Index", "OrdersABM", new { Message = abm.Message, Error = abm.Error });
                    } else {
                        ModelState.AddModelError("Error", Resources.Errors.SPError);
                    }
                } catch (ForeignKeyConstraintException infe) {
                    ModelState.AddModelError("SupplierID", infe.Message);
                } catch (DuplicatedKeyException) {
                    ModelState.AddModelError("OrderID", String.Format(Resources.ExceptionMessages.ItemAlreadyExist,
                        Resources.Resources.Order));
                    ModelState.AddModelError("SupplierID", String.Format(Resources.ExceptionMessages.ItemAlreadyExist,
                        Resources.Resources.Order));
                } catch (IDNotFoundException infe) {
                    ErrorModel error = new ErrorModel(infe.title, infe.Message, infe, "SupplierFormsController", "DeleteProduct");
                    return View("~/Views/Error/ErrorDisplay.cshtml", error);
                } catch (DatabaseException ex) {
                    ErrorModel error = new ErrorModel(ex.title, ex.Message, ex, "SupplierFormsController", "DeleteProduct");
                    return View("~/Views/Error/ErrorDisplay.cshtml", error);
                } catch (Exception e) {
                    ErrorModel error = new ErrorModel(e.Message, e, "SupplierFormsController", "DeleteProduct");
                    return View("~/Views/Error/ErrorDisplay.cshtml", error);
                }
            }
            return View("~/Views/Transactions/Orders/OrderForm.cshtml", model);
        }

        /// <summary>Elimina el proveedor con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult Delete(string orderID, long supplierID) {
            IOrderService service = new OrderService();
            try {
                ABMViewModel model = new ABMViewModel();
                if (service.isCanceled(orderID, supplierID)) {
                    bool result = service.Restore(orderID, supplierID);
                    if (result) {
                        model.Message = String.Format(Resources.Messages.RestoredItem, Resources.Resources.OrderWArt);
                        model.Error = false;
                    } else {
                        model.Message = String.Format(Resources.Errors.OperationUnsaccessfull);
                        model.Error = true;
                    }
                    return RedirectToAction("Index", "OrdersABM", new { Message = model.Message, Error = model.Error });
                } else {
                    try {
                        bool result = service.Cancel(orderID, supplierID);
                        if (result) {
                            model.Message = String.Format(Resources.Messages.CancelledItem, Resources.Resources.OrderWArt);
                            model.Error = false;
                        } else {
                            model.Message = String.Format(Resources.Errors.OperationUnsaccessfull);
                            model.Error = true;
                        }
                        return RedirectToAction("Index", "OrdersABM", new { Message = model.Message, Error = model.Error });

                    } catch (DeleteConstrainException) {
                        ConstraintViewModel models = service.getConstrains();
                        OrderViewModel mod = service.getData(orderID, supplierID);
                        ModelState.AddModelError("Error", models.Title + ": " + models.Message);
                        return View("~/Views/Transactions/Orders/OrderForm.cshtml", mod);
                    }
                }
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(ex.title, ex.Message, ex, "SupplierFormsController", "DeleteProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (MyException ex) {
                ErrorModel error = new ErrorModel(ex.title, ex.Message, ex, "SupplierFormsController", "DeleteProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(e.Message, e, "SupplierFormsController", "DeleteProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        public ActionResult ViewProducts(long? id) {
            if (id.HasValue) {
                ABMsController abm = new ABMsController();
                ABMViewModel model = new ABMViewModel();
                model.AjaxRequest = "/ABMs/ViewProductsOfSupplier";
                model.filterID = id.Value;
                return RedirectToAction("ProductABM", "ABMs", model);
            } else {
                ErrorModel error = new ErrorModel(String.Format(Resources.Errors.SearchIDError, Resources.Resources.SupplierWArt),  
                    "SupplierFormsController", "ViewProducts");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }
    }
}
