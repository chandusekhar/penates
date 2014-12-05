using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Exceptions.Views;
using Penates.Interfaces.Services;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.Forms;
using Penates.Repositories.ABMs;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using Penates.Models;
using Penates.Utils.JSON.TableObjects;

namespace Penates.Services.ABMs {
    public class ProductService : IProductService {

        IProductRepository productRepository = new ProductRepository();

        /// <summary> Guarda los datos de una producto en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como ProductViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        public long Save(ProductViewModel prod) {
            var productRepository = new ProductRepository();
            long value = productRepository.Save(prod); //Capturo el ID o Errores del Sp
            switch (value) {
                case -1:
                    var ex = new IDNotFoundException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.ProductWArt, prod.ProductID), String.Format(Resources.ExceptionMessages.IDNotFoundException,
                        Resources.Resources.Product ,prod.ProductID));
                    ex.atributeName = ReflectionExtension.getVarName(() => prod.ProductID);
                    throw ex;
                case -2:
                    var ex2 = new UniqueRestrictionException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.ProductWArt, prod.ProductID), String.Format(Resources.ExceptionMessages.UniqueRestrictionException,
                        Resources.Attributes.BarcodeWArt, prod.Barcode));
                    ex2.atributeName = ReflectionExtension.getVarName(() => prod.Barcode);
                    throw ex2;
                case -3:
                    var ex3 = new ForeignKeyConstraintException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.ProductWArt, prod.ProductID), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                        Resources.Resources.Category, prod.Category));
                    ex3.atributeName = ReflectionExtension.getVarName(() => prod.Category);
                    throw ex3;
            }
            return value;
        }

        public bool Delete(long id) {
            var productRepository = new ProductRepository();
            return productRepository.Delete(id);
        }

        public ProductViewModel getProductData(long id) {
            var productRepository = new ProductRepository();
            Product prod = productRepository.getData(id, false);

            ProductViewModel aux = new ProductViewModel();
            aux.Barcode = prod.Barcode;
            aux.Category = prod.IDProductCategory;
            aux.Depth = prod.Depth;
            aux.Description = prod.Description;
            aux.Height = prod.Height;
            aux.Name = prod.Name;
            aux.ProductID = prod.ProductID;
            aux.SellPrice = prod.SellPrice;
            aux.Size = prod.Size;
            aux.Width = prod.Width;
            aux.MinStock = prod.MinStock;
            if (prod.MinStock != null) {
                aux.HasMinStock = true; //x default es false
            }
            aux.IsBasic = prod.IsBasic;
            if (prod.Photo != null) {
                aux.ProdImage = prod.Photo;
            }

            return aux;
        }

        public IQueryable<Product> search(IQueryable<Product> query, string search) {
            if (String.IsNullOrEmpty(search)) {
                return query;
            }
            return this.productRepository.search(query, search);
        }

        public IQueryable<Product> categoryFilter(IQueryable<Product> query, long? categoryID) {
            if (categoryID.HasValue) {
                if (categoryID.Value > 0) {
                    return this.productRepository.categoryFilter(query, categoryID.Value);
                } else {
                    return query;
                }
            } else {
                return query;
            }
        }

        public IQueryable<Product> sort(IQueryable<Product> productos, int sortColumnIndex, string sortDirection) {
            try {
                Sorts sort = this.toProductSort(sortColumnIndex, sortDirection);
                return this.productRepository.sort(productos, sort);
            } catch (SortException) {
                return productos; //Si no hay sort no hago nada
            }
        }

        public IQueryable<Product> sort(IQueryable<Product> productos, Sorts sort) {
            return this.productRepository.sort(productos, sort);
        }

        /// <summary>No solo hace skip y take sino que tambien ordena</summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sortColumnIndex">Numero de columna por el cual ordenar</param>
        /// <param name="sortDirection">Ascendente o Descendente</param>
        /// <returns></returns>
        public IQueryable<Product> getProductsDisplayData(int sortColumnIndex, string sortDirection, long? categoryID, ref long total) {
            Sorts sort = this.toProductSort(sortColumnIndex, sortDirection);
            return this.getProductsDisplayData(sort, categoryID, ref total);
        }

        /// <summary>No solo hace skip y take sino que tambien ordena</summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sort">Sorts por el cual ordenar</param>
        /// <returns></returns>
        public IQueryable<Product> getProductsDisplayData(Sorts sort, long? categoryID, ref long total) {
            return this.getProductsDisplayData(null, sort, categoryID, ref total);
        }

        /// <summary> Busca, pagina y ordena</summary>
        /// <param name="search"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sortColumnIndex">Numero de columna por el cual ordenar</param>
        /// <param name="sortDirection">Ascendente o descendente</param>
        /// <returns></returns>
        public IQueryable<Product> getProductsDisplayData(string search, int sortColumnIndex, string sortDirection, long? categoryID, ref long total) {
            Sorts sort = this.toProductSort(sortColumnIndex, sortDirection);
            return this.getProductsDisplayData(search, sort, categoryID, ref total);
        }

        public List<ProductTableJson> getProductsDisplayData(string search, int start, int lenght, int sortColumnIndex, string sortDirection, long? categoryID, ref long total) {
            IQueryable<Product> query = this.getProductsDisplayData(search, sortColumnIndex, sortDirection, categoryID, ref total);
            if (start >= 0 && lenght >= 0) {
                Paginator<Product> pag = new Paginator<Product>(query);
                query = pag.page(start, lenght);
            }
            return this.toJsonArray(query);
        }

        public List<ProductTableJson> getProductsDisplayData(int start, int lenght, int sortColumnIndex, string sortDirection, long? categoryID, ref long total) {
            IQueryable<Product> query = this.getProductsDisplayData(null, sortColumnIndex, sortDirection, categoryID, ref total);
            if (start >= 0 && lenght >= 0) {
                Paginator<Product> pag = new Paginator<Product>(query);
                query = pag.page(start, lenght);
            }
            return this.toJsonArray(query);
        }

        /// <summary>Busca pagina y ordenas</summary>
        /// <param name="search"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public IQueryable<Product> getProductsDisplayData(string search, Sorts sort, long? categoryID, ref long total) {
            long categoryFilter = categoryID ?? -1;
            IQueryable<Product> productos;
            if (categoryFilter > 0) {
                productos = this.getProducts(search, sort, categoryFilter, ref total);
            } else {
                productos = this.getProducts(search, sort, ref total);
            }

            return productos;
        }

        public List<ConstraintViewModel> getConstrains(long prodID) {
            List<ConstraintViewModel> constraintsList = new List<ConstraintViewModel>();

            ProductRepository repo = new ProductRepository(Properties.Settings.Default.nConstrainsToView);
            ConstraintViewModel constraint;
            long constraintsCount = 0;

            constraintsCount = repo.getItemsConstraintsNumber(prodID);
            if (constraintsCount > 0) {
                constraint = new ConstraintViewModel(
                    String.Format(Resources.Constraints.ConstraintTitle, Resources.Resources.Items),
                    String.Format(Resources.Constraints.ConstraintMessage, Resources.Resources.Items, Resources.Resources.Products));
                constraint.Count = constraintsCount;
                constraint.TableWithConstrain = "Items";
                constraint.constraints = repo.getItemsConstraints(prodID);
                constraintsList.Add(constraint);
            }

            return constraintsList;
        }

        public ICollection<Supplier> getSuppliers(long? id) {
            if (!id.HasValue || id.Value <= 0) {
                return new List<Supplier>();
            }
            return this.productRepository.getSuppliers(id.Value);
        }

        /// <summary>Gets the Sort Enum from the column index and sort direction</summary>
        /// <param name="sortColumnIndex">Column index</param>
        /// <param name="sortDirection">Ascendiente o Descendiente</param>
        /// <returns>Enum por el cual se hace el Sort</returns>
        /// <exception cref="SortException"
        public Sorts toProductSort(int sortColumnIndex, string sortDirection) {

            switch (sortColumnIndex) {
                case 0:
                    if (sortDirection == "asc") {
                        return Sorts.ID;
                    } else {
                        return Sorts.ID_DESC;
                    }
                case 1:
                    if (sortDirection == "asc") {
                        return Sorts.BARCODE;
                    } else {
                        return Sorts.BARCODE_DESC;
                    }
                case 2:
                    if (sortDirection == "asc") {
                        return Sorts.NAME;
                    } else {
                        return Sorts.NAME_DESC;
                    }
                case 3:
                    if (sortDirection == "asc") {
                        return Sorts.CATEGORY;
                    } else {
                        return Sorts.CATEGORY_DESC;
                    }
                default:
                    throw new SortException(Resources.ExceptionMessages.InvalidSortException, "Col: " + sortColumnIndex);
            }
        }

        public List<ProductTableJson> toJsonArray(IQueryable<Product> query) {
            List<Product> list = query.ToList();
            List<ProductTableJson> result = new List<ProductTableJson>();
            ProductTableJson aux;
            foreach (Product product in list) {
                aux = new ProductTableJson() {
                    ProductID = product.ProductID,
                    Barcode = product.Barcode,
                    Name = product.Name,
                    Category = product.ProductCategory.Description
                };
                result.Add(aux);
            }
            return result;
        }

        public List<ProductTableJson> toJsonArray(ICollection<Product> list) {
            List<ProductTableJson> result = new List<ProductTableJson>();
            ProductTableJson aux;
            foreach (Product product in list) {
                aux = new ProductTableJson(){ProductID = product.ProductID, Barcode = product.Barcode, 
                Name = product.Name, Category = product.ProductCategory.Description};
                result.Add(aux);
            }
            return result;
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<Product> query)
        {
            List<Product> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (Product prod in list)
            {
                aux = new AutocompleteItem() { ID = new { ProductID = prod.ProductID }, Label = prod.Name, Description = prod.ProductCategory.Description };
                result.Add(aux);
            }
            return result;
        }

        /// <summary> Agrega un Proveedor a un Producto Determinado si es que existen </summary>
        /// <returns>true si logra agregarlo, false si no lo logra</returns>
        /// <exception cref="IDNotFoundException" Cuando no encuentra o el ProductID o el SupplierID
        /// <exception cref="DatabaseException" Cuando ocurre un error de otra Indole
        public bool addSupplier(ProvidedByViewModel model) {
            model.Size = model.Depth * model.Height * model.Width;
            ProvidedBy providedBy = new ProvidedBy() {IDProduct = model.ProductID, IDSupplier = model.SupplierID, ItemsPerBox = model.ItemsPerBox,
            PurchasePrice = model.PurchasePrice, BoxDepth = model.Depth, BoxHeight = model.Height, BoxWidth = model.Width,
            BoxSize = model.Size};
            return this.productRepository.addSupplier(providedBy);
        }


        public bool unasignSupplier(long ProductID, long SupplierID) {
            return this.productRepository.unasignSupplier(ProductID, SupplierID);
        }

        public bool existAsSupplier(long? ProductID, long? SupplierID) {
            if (ProductID.HasValue && SupplierID.HasValue) {
                return this.productRepository.existAsSupplier(ProductID.Value, SupplierID.Value);
            }
            return false;
        }

        public ProvidedByViewModel getProvidedByData(long ProductID, long SupplierID) {
            ProvidedBy prov = this.productRepository.getProvidedByData(ProductID, SupplierID);
            return new ProvidedByViewModel(prov);
        }

        private IQueryable<Product> getProducts(string search, Sorts sort, ref long total) {
            IQueryable<Product> productos;
            if (search == null) {
                productos = productRepository.getData(sort, ref total);
            } else {
                search = search.Trim();
                if (search == null || search.Equals("")) {
                    productos = productRepository.getData(sort, ref total);
                } else {
                    productos = productRepository.getData(search, sort, ref total);
                }
                if (productos == null) {
                    productos = new List<Product>().AsQueryable();
                }
            }
            return productos;
        }

        private IQueryable<Product> getProducts(string search, Sorts sort, long categoryID, ref long total) {
            IQueryable<Product> productos;
            if (search == null) {
                productos = productRepository.getData(sort, categoryID, ref total);
            } else {
                search = search.Trim();
                if (search == null || search.Equals("")) {
                    productos = productRepository.getData(sort, categoryID, ref total);
                } else {
                    productos = productRepository.getData(search, sort, categoryID, ref total);
                }
                if (productos == null) {
                    productos = new List<Product>().AsQueryable();
                }
            }
            return productos;
        }

        public IQueryable<Product> getAutocomplete(string search)
        {
            return this.productRepository.getAutocomplete(search);
        }



    }
}