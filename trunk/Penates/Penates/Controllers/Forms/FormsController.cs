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
using Penates.Services;
using Penates.Utils.Enums;


namespace Penates.Controllers
{
    [RoleValidation(RoleType.SU, RoleType.InventoryChief, RoleType.Common)]
    /// <summary>Controller for Products</summary>
    public class FormsController : Controller
    {
        PenatesEntities database = new PenatesEntities();

        public ActionResult Index(){
            return this.ProductForm();
        }

        [HttpGet]
        public ActionResult ProductForm(){
            try {
                var model = new ProductViewModel();
                CategoryService cs = new CategoryService();
                model.CategoriesList = cs.getCategoriesList();
                ProductKeeper.getInstance().productImage = model.ProdImage;
                return View("~/Views/ABMs/Forms/ProductForm.cshtml", model);
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, ex.Message, "FormsController", "ProductFormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "ProductFormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        [HttpGet]
        public ActionResult ProductFormUpdate(long ProductID) {
            var prodService = new ProductService();
            ProductViewModel model;
            try {
                model = prodService.getProductData(ProductID);
                if (model == null) {//Si no encontre el producto agrego el error para que lo muestre
                    model = new ProductViewModel();
                    model.error = String.Format(Resources.Messages.ItemNotFound, Resources.Resources.ProductWArt, ProductID);
                } else {
                    CategoryService cs = new CategoryService();
                    model.CategoriesList = cs.getCategoriesList(model.Category);
                    ProductKeeper.getInstance().productID = ProductID;
                    ProductKeeper.getInstance().productImage = model.ProdImage;
                }
                return View("~/Views/ABMs/Forms/ProductForm.cshtml", model);
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, ex.Message, "FormsController", "ProductFormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "ProductFormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        /// <summary> Muestra un Summary con los datos que se agregaron recientemente o editaron /// </summary>
        /// <param name="prod">Modelo con los datos a mostrar</param>
        /// <param name="ProdImage">La imagen se manda aparte para evitar errores</param>
        /// <returns>La vista del summary</returns>
        public ActionResult ProductFormSummary([Bind(Exclude = "ProdImage")]ProductViewModel prod, HttpPostedFileBase ProdImage) {
            try {
                CategoryService categoryService = new CategoryService();
                prod.Size = prod.Width * prod.Height * prod.Depth;
                prod.CategoryName = categoryService.getCategoryName(prod.Category);
                if (prod.MinStock != null) {
                    prod.HasMinStock = true;
                }
                if (prod.CategoryName == null) {
                    prod.CategoryName = String.Format(Resources.Errors.GetCategoryNameError, prod.Category);
                }
                return View("~/Views/ABMs/Forms/ProductFormSummary.cshtml", prod);
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, ex.Message, "FormsController", "ProductFormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "ProductFormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        /// <summary> Trato el POST del formulario de Producto </summary>
        /// <param name="prod">El modelo con los datos del producto</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SubmitProduct(ProductViewModel prod) {
            if (prod.Image != null && prod.validImage) {
                if (ModelState.IsValidField("Image")) {
                    try {
                        if (Validators.IsImage(prod.Image)) {
                            ProductKeeper.getInstance().productImage = Penates.Utils.Converters.ToByteArray(prod.Image);
                            prod.ProdImage = ProductKeeper.getInstance().productImage;
                        } else {
                            ModelState.AddModelError("Image", Resources.FormsErrors.ExtensionError);
                        }
                    } catch (Exception e) {
                        ModelState.AddModelError("Image", e.Message);
                    }
                }
            } else {
                if (ProductKeeper.getInstance().productImage != null) {
                    prod.ProdImage = ProductKeeper.getInstance().productImage;
                }
            }
            if(ModelState.IsValid) {
                //Agrego el producto
                //Comparo que la Imagen no sea la de default ya que no se permite
                var aux = Converters.gifToByteArray(System.Drawing.Image.FromFile(Server.MapPath("~/Images/ABMs/NoImage.gif"), true));
                if (prod.ProdImage.SequenceEqual(aux)) {
                    prod.ProdImage = null;
                }
                var productService = new ProductService();
                try {
                    var result = productService.Save(prod);

                    if (result >= 0) { //Si es alguno de los errores conocidos salta x Exception
                        prod.ProductID = result;
                        return RedirectToAction("ProductFormSummary", prod);
                    } else {
                        ModelState.AddModelError("Error", Resources.Errors.SPError);
                    }
                } catch (IDNotFoundException infe) {
                    ModelState.AddModelError(infe.atributeName, infe.Message);
                } catch (UniqueRestrictionException infe) {
                    ModelState.AddModelError(infe.atributeName, infe.Message);
                } catch (ForeignKeyConstraintException infe) {
                    ModelState.AddModelError(infe.atributeName, infe.Message);
                } catch (DatabaseException ex) {
                    //El unico error que puedo tener es con la base de datos, ya que si el ID existe hace un Update
                    ModelState.AddModelError("Error", Resources.Errors.DatabaseError + ": " + ex.Message);
                } catch (Exception e) {
                    ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "SubmitProduct");
                    return View("~/Views/Error/ErrorDisplay.cshtml", error);
                }
            }
            try {
                CategoryService cs = new CategoryService();
                prod.CategoriesList = cs.getCategoriesList(); //Primero reagrego las categorias al modelo para mostrar
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, ex.Message, "FormsController", "ProductFormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "ProductFormSummary");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
            return View("~/Views/ABMs/Forms/ProductForm.cshtml",prod);
        }


        /// <summary>Elimina el producto con determinado id del sistema /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns>El menu de ABM o en caso de tener una constrain la lista de constrains</returns>
        public ActionResult Delete(long productID) {
            ProductService productService = new ProductService();
            try{
                try {
                    bool result = productService.Delete(productID);
                    ABMViewModel model = new ABMViewModel();
                    if (result) {
                        model.Message = String.Format(Resources.Messages.DeletedItem, Resources.Resources.ProductWArt, productID);
                        model.Error = false;
                    } else {
                        model.Message = String.Format(Resources.Errors.AlreadyDeletedAlert, Resources.Resources.ProductWArt, productID);
                        model.Error = true;
                    }
                    return RedirectToAction("ProductABM", "ABMs", new { Message = model.Message, Error = model.Error });

                } catch (DeleteConstrainException) {
                    List<ConstraintViewModel> models = productService.getConstrains(productID);
                    return View("~/Views/Error/ConstraintsDisplay.cshtml", models);
                }
            } catch (DatabaseException ex) {
                ErrorModel error = new ErrorModel(ex.title, ex.Message, ex, "FormsController", "DeleteProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            } catch (Exception e) {
                ErrorModel error = new ErrorModel(Resources.Errors.DatabaseError, e, "FormsController", "DeleteProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", error);
            }
        }

        public ActionResult getProductImg(byte[] prodImg) {
            byte[] byteArray = ProductKeeper.getInstance().productImage;
            if (byteArray != null) {
                //return new FileContentResult(byteArray, "image/jpeg");
                return File(byteArray, "image/jpeg");
            } else {
                return null;
            }
        }
    }
}
