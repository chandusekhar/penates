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
using Penates.Controllers.ABMs;
using Penates.Interfaces.Services;
using Penates.Models;
using System.Text;
using Penates.Models.ViewModels;
using Penates.Services;
using Penates.Utils.Enums;


namespace Penates.Controllers
{
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
    public class ProvidedByFormController : Controller
    {
        PenatesEntities database = new PenatesEntities();

        public ActionResult Index(string ViewID, long? ID, long? SupplierID) {
            return this.ProvidedByFormModal(ViewID, ID, SupplierID);
        }

        [HttpPost]
        public ActionResult ProvidedByFormModal(string ViewID, long? ID, long? SupplierID) {
            try {
                var model = new ProvidedByViewModel();
                model.ViewId = ViewID;
                var service = new SupplierService();
                var prodService = new ProductService();
                if (prodService.existAsSupplier(ID, SupplierID)) {
                    return this.FormEdit(ViewID, ID, SupplierID);
                }
                if (SupplierID.HasValue) {
                    SupplierViewModel supp = service.getData(SupplierID.Value);
                    if (supp == null) {//Si no encontre el producto agrego el error para que lo muestre
                        ErrorException e = new ErrorException("Error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.SupplierWArt, SupplierID.Value));
                        throw e;
                    } else {
                        model.SupplierID = SupplierID.Value;
                        model.SupplierName = supp.Name;
                    }
                } else {
                    throw new Exception(String.Format(Resources.Errors.AddIDNull, Resources.Resources.SupplierWArt));
                }
                if (ID.HasValue) {
                    ProductViewModel prod = prodService.getProductData(ID.Value);
                    if (prod == null) {//Si no encontre el producto agrego el error para que lo muestre
                        ErrorException e = new ErrorException("Error", String.Format(Resources.Messages.ItemNotFound, Resources.Resources.ProductWArt, ID.Value));
                        throw e;
                    } else {
                        model.ProductID = ID.Value;
                        model.Barcode = prod.Barcode;
                        model.ProductName = prod.Name;
                    }
                } else {
                    throw new Exception(String.Format(Resources.Errors.FilterIDNull, Resources.Resources.ProductWArt, Resources.Resources.Product));
                }
                return PartialView("~/Views/ABMs/Modals/ProvidedByFormModal.cshtml", model);
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
        public ActionResult FormEdit(string ViewID, long? ID, long? SupplierID) {
            var service = new SupplierService();
            var prodService = new ProductService();
            try {
                if (!SupplierID.HasValue) {
                    throw new Exception(String.Format(Resources.Errors.AddIDNull, Resources.Resources.SupplierWArt));
                }
                if (!ID.HasValue) {
                    throw new Exception(String.Format(Resources.Errors.FilterIDNull, Resources.Resources.ProductWArt, Resources.Resources.Product));
                }
                ProvidedByViewModel model = prodService.getProvidedByData(ID.Value, SupplierID.Value);
                model.ViewId = ViewID;
                return PartialView("~/Views/ABMs/Modals/ProvidedByFormModal.cshtml", model);
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
        [HttpPost]
        public ActionResult Save(ProvidedByViewModel model) {
            if (ModelState.IsValid) {
                        var productService = new ProductService();
                        try {
                            //El filterID es al Producto que se le agrega y el Add es el que se va a agregar
                            if (productService.addSupplier(model)) {
                                Response.StatusCode = 200;
                                return Json(new {
                                    title = Resources.Messages.OperationSuccessfull,
                                    message = String.Format(Resources.Messages.SuccessfullAdd, Resources.Resources.SupplierWArt)
                                }, JsonRequestBehavior.AllowGet);
                            } else {
                                throw new Exception("Error");
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
                            Response.StatusDescription = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Supplier, Resources.Resources.Product);
                            Response.Write(e.Message);
                            return null;
                        }
            } else {
                Response.StatusCode = 550;
                return PartialView("~/Views/ABMs/Modals/ProvidedByFormModal.cshtml", model);
            }
        }

        /// <summary>Elimina el supplier con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult UnassignSupplier(long ID, long SupplierID) {
            var productService = new ProductService();
            try {
                //El filterID es al Producto que se le agrega y el Add es el que se va a agregar
                bool resultado = productService.unasignSupplier(ID, SupplierID);
                if (resultado) {
                    Response.StatusCode = 200;
                    return Json(new {
                        title = Resources.Messages.OperationSuccessfull,
                        message = String.Format(Resources.Messages.SuccessfullUnassign, Resources.Resources.SupplierWArt)
                    }, JsonRequestBehavior.AllowGet);
                } else {
                    Response.StatusCode = 511;
                    Response.StatusDescription = Resources.Errors.OperationUnsaccessfull;
                    Response.Write(String.Format(Resources.Errors.UnassignUnsaccessfull, Resources.Resources.SupplierWArt));
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
                Response.StatusDescription = String.Format(Resources.ExceptionMessages.UnassignException, Resources.Resources.Supplier, Resources.Resources.Product);
                Response.Write(e.Message);
                return null;
            }
        }
    }
}
