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
using Penates.Services;
using Penates.Utils.Enums;


namespace Penates.Controllers
{
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common, RoleType.Executive)]
    public class SupplierFormsController : Controller
    {
        //
        // GET: /Forms/
        PenatesEntities database = new PenatesEntities();

        public ActionResult Index(){
            return this.SupplierForm();
        }

        [HttpGet]
        public ActionResult SupplierForm(){
            try {
                var model = new SupplierViewModel();
                return View("~/Views/ABMs/Forms/SupplierForm.cshtml", model);
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, ex.Message, "SupplierFormsController", "SupplierForm");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "SupplierFormsController", "SupplierForm");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        [HttpGet]
        public ActionResult FormEdit(long id) {
            var service = new SupplierService();
            SupplierViewModel model;
            try {
                model = service.getData(id);
                if (model == null) {//Si no encontre el producto agrego el error para que lo muestre
                    model = new SupplierViewModel();
                    model.error = String.Format(Resources.Messages.ItemNotFound, Resources.Resources.SupplierWArt, id);
                }
                return View("~/Views/ABMs/Forms/SupplierForm.cshtml", model);
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, ex.Message, "SupplierFormsController", "FormEdit");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "SupplierFormsController", "FormEdit");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        /// <summary> Muestra un Summary con los datos que se agregaron recientemente o editaron /// </summary>
        /// <param name="prod">Modelo con los datos a mostrar</param>
        /// <param name="ProdImage">La imagen se manda aparte para evitar errores</param>
        /// <returns>La vista del summary</returns>
        public ActionResult FormSummary(SupplierViewModel supplier) {
            try {
                return View("~/Views/ABMs/Forms/SupplierFormSummary.cshtml", supplier);
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, ex.Message, "SupplierFormsController", "FormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "SupplierFormsController", "FormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        /// <summary> Trato el POST del formulario de Producto </summary>
        /// <param name="prod">El modelo con los datos del producto</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(SupplierViewModel supplier) {
            if(ModelState.IsValid) {
                //Agrego el Supplier
                var service = new SupplierService();
                try {
                    var result = service.Save(supplier);

                    if (result >= 0) { //Si es alguno de los errores conocidos salta x Exception
                        supplier.SupplierID = result;
                        return RedirectToAction("FormSummary", supplier);
                    } else {
                        ModelState.AddModelError("Error", Resources.Errors.SPError);
                    }
                } catch (IDNotFoundException infe) {
                    ModelState.AddModelError(infe.atributeName, infe.Message);
                } catch (StoredProcedureException infe) {
                    ModelState.AddModelError(infe.atributeName, infe.Message);
                } catch (DatabaseException ex) {
                    //El unico error que puedo tener es con la base de datos, ya que si el ID existe hace un Update
                    ModelState.AddModelError("Error", Resources.Errors.DatabaseError + ": " + ex.Message);
                } catch (Exception e) {
                    ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "SupplierFormsController", "Save");
                    return View("~/Views/Error/ErrorDisplay.cshtml", error);
                }
            }
            return View("~/Views/ABMs/Forms/SupplierForm.cshtml", supplier);
        }


        /// <summary>Elimina el producto con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult Delete(long id) {
            ISupplierService supplierService = new SupplierService();
            try{
                try {
                    bool result = supplierService.Delete(id);
                    ABMViewModel model = new ABMViewModel();
                    if (result) {
                        model.Message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.ProductWArt, id);
                        model.Error = false;
                    } else {
                        model.Message = String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.ProductWArt, id);
                        model.Error = true;
                    }
                    return RedirectToAction("Index", "SupplierABM", new { Message = model.Message, Error = model.Error });

                } catch (DeleteConstrainException) {
                    List<ConstraintViewModel> models = supplierService.getConstrains(id);
                    return View("~/Views/Error/ConstraintsDisplay.cshtml", models);
                }
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(ex.title, ex.Message, ex, "SupplierFormsController", "DeleteProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "SupplierFormsController", "DeleteProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        public ActionResult ViewProducts(long? id) {
            if (id.HasValue) {
                ABMsController abm = new ABMsController();
                ABMViewModel model = new ABMViewModel();
                return RedirectToAction("ProductABM", "ABMs", new { AjaxRequest = "/ABMs/ViewProductsOfSupplier", filterID = id.Value});
            } else {
                ErrorModel error = new ErrorModel(String.Format(Resources.Errors.SearchIDError, Resources.Resources.SupplierWArt),  
                    "SupplierFormsController", "ViewProducts");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }
    }
}
