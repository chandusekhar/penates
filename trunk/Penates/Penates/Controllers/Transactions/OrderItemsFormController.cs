using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Services;
using Penates.Models.ViewModels.Forms;
using Penates.Models.ViewModels.Transactions;
using Penates.Services;
using Penates.Services.ABMs;
using Penates.Services.Transactions;
using Penates.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Penates.Controllers
{
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
    public class OrderItemsFormController : Controller
    {
        //
        // GET: /Forms/
        PenatesEntities database = new PenatesEntities();

        public ActionResult Index(string ViewID, string OrderID, long SupplierID, long ProductID) {
            return this.ItemsFormModal(ViewID, OrderID, SupplierID, ProductID);
        }

        [HttpPost]
        public ActionResult ItemsFormModal(string ViewID, string OrderID, long SupplierID, long? ProductID) {
            try {
                if (ProductID.HasValue) {
                    var model = new OrderItemsViewModel();
                    model.ViewId = ViewID;
                    IOrderService service = new OrderService();
                    IProductService prodService = new ProductService();
                    if (service.existInOrder(OrderID, SupplierID, ProductID)) {
                        return this.FormEdit(ViewID, OrderID, SupplierID, ProductID.Value);
                    }
                    ProductViewModel prod = prodService.getProductData(ProductID.Value);
                    if (prod == null) {//Si no encontre el producto agrego el error para que lo muestre
                        ErrorException e = new ErrorException("Error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.ProductWArt, ProductID));
                        throw e;
                    } else {
                        model.ProductName = prod.Name;
                        this.HttpContext.Cache["ProductName"] = model.ProductName;
                    }
                    model.OrderID = OrderID;
                    model.SupplierID = SupplierID;
                    model.ProductID = ProductID.Value;
                    return PartialView("~/Views/Transactions/Orders/_OrderItemsFormModal.cshtml", model);
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = "Error";
                    Response.Write(String.Format(Resources.Errors.IDNull, Resources.Resources.ProductWArt));
                    return null;
                }
            } catch (DatabaseException ex) {
                Response.StatusCode = 511;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            } catch (ErrorException ex) {
                Response.StatusCode = 511;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            } catch (Exception e) {
                Response.StatusCode = 511;
                Response.StatusDescription = "Error";
                Response.Write(e.Message);
                return null;
            }
        }

        [HttpGet]
        public ActionResult FormEdit(string ViewID, string OrderID, long SupplierID, long ProductID) {
            IOrderService service = new OrderService();
            try {
                OrderItemsViewModel model = service.getOrderItemData(OrderID, SupplierID, ProductID);
                this.HttpContext.Cache["ProductName"] = model.ProductName;
                model.edit = true;
                model.ViewId = ViewID;
                return PartialView("~/Views/Transactions/Orders/_OrderItemsFormModal.cshtml", model);
            } catch (DatabaseException ex) {
                Response.StatusCode = 511;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            } catch (ErrorException ex) {
                Response.StatusCode = 511;
                Response.StatusDescription = ex.title;
                Response.Write(ex.Message);
                return null;
            } catch (Exception e) {
                Response.StatusCode = 511;
                Response.StatusDescription = "Error";
                Response.Write(e.Message);
                return null;
            }
        }

        /// <summary> Trato el POST del formulario de Producto </summary>
        /// <param name="prod">El modelo con los datos del producto</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddProduct(OrderItemsViewModel model) {
            if (ModelState.IsValid) {
                IOrderItemsService orderService = new OrderItemsService();
                try {
                    if (orderService.saveItem(model)) {
                        Response.StatusCode = 200;
                        return Json(new {
                            title = Resources.Messages.OperationSuccessfull,
                            message = String.Format(Resources.Messages.SuccessfullAdd, Resources.Resources.ProductWArt)
                        }, JsonRequestBehavior.AllowGet);
                    } else {
                        throw new Exception("Error");
                    }
                } catch (ForeignKeyConstraintException infe) {
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
                    Response.StatusDescription = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Product, Resources.Resources.Order);
                    Response.Write(e.Message);
                    return null;
                }
            } else {
                Response.StatusCode = 550;
                if (this.HttpContext.Cache["ProductName"] == null) {
                    model.ProductName = this.getProductName(model.ProductID);
                } else {
                    try {
                        model.ProductName = (string)this.HttpContext.Cache["ProductName"];
                    } catch (InvalidCastException) {
                        model.ProductName = this.getProductName(model.ProductID);
                    }
                }
                return PartialView("~/Views/Transactions/Orders/_OrderItemsFormModal.cshtml", model);
            }
        }

        /// <summary>Elimina el item con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult UnassignProduct(string OrderID, long SupplierID, long ProductID) {
            try {
                IOrderItemsService orderService = new OrderItemsService();
                if (orderService.unasignItem(OrderID, SupplierID, ProductID)) {
                    Response.StatusCode = 200;
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = String.Format(Resources.Messages.SuccessfullUnassign, Resources.Resources.ProductWArt)
                    }, JsonRequestBehavior.AllowGet);
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                    Response.Write(String.Format(Resources.Errors.UnassignUnsaccessfull, Resources.Resources.ProductWArt));
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
                Response.StatusDescription = String.Format(Resources.ExceptionMessages.UnassignException, Resources.Resources.Product, Resources.Resources.Order);
                Response.Write(e.Message);
                return null;
            }
        }

        private string getProductName(long ProductID) {
            IProductService prodService = new ProductService();
            ProductViewModel prod = prodService.getProductData(ProductID);
            if (prod == null) {//Si no encontre el producto agrego el error para que lo muestre
                ErrorException e = new ErrorException("Error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.ProductWArt, ProductID));
                throw e;
            } else {
                this.HttpContext.Cache["ProductName"] = prod.Name;
                return prod.Name;
            }
        }
    }
}
