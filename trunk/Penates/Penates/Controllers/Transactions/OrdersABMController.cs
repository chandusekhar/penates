using Penates.Database;
using Penates.Exceptions;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.Transactions;
using Penates.Services;
using Penates.Services.ABMs;
using Penates.Services.Transactions;
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
    public class OrdersABMController : Controller
    {
        public ActionResult Index(ABMViewModel model) {
            return View("~/Views/Transactions/Orders/OrdersABM.cshtml",model);
        }

        public ActionResult ABMAjax(jQueryDataTableParamModel param, long? FilterID) {
            try {
                IOrderService service = new OrderService();
                int sortColumnIndex = param.iSortCol_0;
                string sortDirection = param.sSortDir_0;
                IQueryable<SupplierOrder> query = service.getData();
                query = service.filterBySupplier(query, FilterID);
                if (!string.IsNullOrEmpty(param.sSearch)) {
                    query = service.search(query, param.sSearch);
                }
                long total = query.Count();
                query = service.sort(query, sortColumnIndex, sortDirection);
                Paginator<SupplierOrder> pag = new Paginator<SupplierOrder>(query);
                query = pag.page(param.iDisplayStart, param.iDisplayLength);
                List<OrderTableJson> result = service.toJsonArray(query);
                long count = result.Count();
                return Json(new {
                    sEcho = param.sEcho,
                    iTotalRecords = count,
                    iTotalDisplayRecords = total,
                    aaData = result
                },
                            JsonRequestBehavior.AllowGet);
            } catch (DatabaseException ex) {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "OrdersABMController", "ABMAjax");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }catch(MyException ex){
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "OrdersABMController", "ABMAjax");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            } catch (Exception e) {
                ErrorModel errorModel = new ErrorModel(Resources.Errors.DatabaseError, e, "OrdersABMController", "ABMAjax");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }
        }

        /// <summary>Elimina el proveedor con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult Delete(string orderID, long? supplierID) {
            IOrderService service = new OrderService();
            try {
                if (supplierID.HasValue && !String.IsNullOrWhiteSpace(orderID)) {
                    if (service.isCanceled(orderID, supplierID.Value)) {
                        bool result = service.Restore(orderID, supplierID.Value);
                        if (result) {
                            return Json(new {
                                title = Resources.Messages.OperationSuccessfull,
                                message = String.Format(Resources.Messages.RestoredItem, Resources.Resources.OrderWArt)
                            }, JsonRequestBehavior.AllowGet);
                        } else {
                            Response.StatusCode = 511;
                            Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                            Response.Write(String.Format(Resources.Errors.AlreadyRestoredAlert, Resources.Resources.OrderWArt));
                            return null;
                        }
                    }else{
                        try {
                            bool result = service.Cancel(orderID, supplierID.Value);
                            if (result) {
                                return Json(new {
                                    title = Resources.Messages.OperationSuccessfull,
                                    message = String.Format(Resources.Messages.CancelledItem, Resources.Resources.OrderWArt)
                                }, JsonRequestBehavior.AllowGet);
                            } else {
                                Response.StatusCode = 511;
                                Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                                Response.Write(String.Format(Resources.Errors.AlreadyCancelledAlert, Resources.Resources.OrderWArt));
                                return null;
                            }

                        } catch (DeleteConstrainException) {
                            Response.StatusCode = 511;
                            ConstraintViewModel cons = service.getConstrains();
                            Response.StatusDescription = cons.Title;
                            Response.Write(cons.Message);
                            return null;
                        }
                    }
                }else{
                        Response.StatusCode = 511;
                        Response.StatusDescription = String.Format(Resources.ExceptionMessages.DeleteExceptionNoID, Resources.Resources.SupplierWArt);
                        Response.Write(String.Format(Resources.Errors.DeleteIDNull, Resources.Resources.SupplierWArt));
                        return null;
                }
            } catch (DatabaseException ex) {
                Response.StatusCode = 511;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            } catch (MyException ex) {
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

        public JsonResult SupplierAutocomplete(string term, long? productID) {
            try {
                ISupplierService service = new SupplierService();
                var items = service.getAutocomplete(term, productID);
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

        /// <summary> Trato el POST del formulario de Producto </summary>
        /// <param name="prod">El modelo con los datos del producto</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Save(OrderViewModel model) {
            if (ModelState.IsValid) {
                IOrderService orderService = new OrderService();
                try {
                    if (orderService.Save(model)) {
                        OrderFormController contr = new OrderFormController();
                        return Json(new{ model = model,
                            date = model.OrderDate.ToLongDateString()},
                            JsonRequestBehavior.AllowGet);
                    } else {
                        throw new Exception("Error");
                    }
                } catch (ForeignKeyConstraintException infe) {
                    ModelState.AddModelError("SupplierID", infe.Message);
                } catch (DuplicatedKeyException) {
                    ModelState.AddModelError("OrderID", String.Format(Resources.ExceptionMessages.ItemAlreadyExist,
                        Resources.Resources.Order));
                    ModelState.AddModelError("SupplierID", String.Format(Resources.ExceptionMessages.ItemAlreadyExist,
                        Resources.Resources.Order));
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
                    Response.StatusDescription = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Supplier, Resources.Resources.Product);
                    Response.Write(e.Message);
                    return null;
                }
            }
            Response.StatusCode = 550;
            return PartialView("~/Views/Transactions/Orders/_OrderFormModal.cshtml", model);
        }
	}
}