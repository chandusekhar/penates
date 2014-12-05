using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Exceptions.Views;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Penates.Repositories.ABMs {
    public class ProductRepository : IProductRepository{

        PenatesEntities db = new PenatesEntities();
        private int itemsPerPage = 50;

        public ProductRepository() {

        }

        public ProductRepository(int itemsPerPage) {
            this.itemsPerPage = itemsPerPage;
        }

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido
        ///         *Error del SP</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Save(ProductViewModel prod){
            try {
                Product aux = db.Products.Find(prod.ProductID);
                Nullable<long> val = null;
                if (!prod.HasMinStock) {
                    prod.MinStock = null;
                }
                if (aux == null) {
                    val = db.SP_Product_Add(prod.Barcode, prod.Name, prod.SellPrice, prod.Depth, prod.Width, prod.Height,
                        prod.Category, prod.Description, prod.ProdImage, prod.MinStock, prod.IsBasic).SingleOrDefault();
                } else {
                    val = db.SP_Product_Save(prod.Barcode, prod.Name, prod.SellPrice, prod.Depth, prod.Width, prod.Height,
                       prod.Category, prod.Description, prod.ProdImage, prod.MinStock, prod.IsBasic, prod.ProductID).SingleOrDefault();
                }
                if (!val.HasValue) {
                    return -1;
                }
                return val.Value;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException, "Prod: " + prod.Barcode));
            }
        }

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        [Obsolete("This function should not be used!!")]
        public bool Delete(IEnumerable<long> ids) {
            //Se supone que restrinjo para que siempre lo encuentre
            var deleteProducts =
                from products in db.Products
                where ids.Contains(products.ProductID) && products.Deleted == false //si ya se borro muestro que ya se borro
                select products;
            if (deleteProducts == null || deleteProducts.ToList().Count == 0) {
                return false;
            }
            try {
                Product aux;
                foreach (var products in deleteProducts) {
                    aux = products;
                    aux.Deleted = true;
                    db.Entry(products).CurrentValues.SetValues(aux);
                }
                db.SaveChanges();
                return true;
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.DeleteException, Resources.Resources.ProductWArt,ids.FirstOrDefault() + " - " + 
                    ids.Last()),e.Message);
            }
        }

        public bool Delete(long id) {
            //Se supone que restrinjo para que siempre lo encuentre
            Product prod = db.Products.Find(id);
            if (prod == null || prod.Deleted == true) {
                return false;
            }
            if (checkDeleteConstrains(prod)) {
                try {
                    long? ret = db.SP_Product_Delete(id).SingleOrDefault();
                    if (!ret.HasValue || ret.Value < 0) {
                        throw new Exception(Resources.ExceptionMessages.StoredProcedureException);
                    }
                    return true;
                } catch (Exception e) {
                    throw new DatabaseException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.ProductWArt, id), e.Message);
                }
            } else {
                throw new DeleteConstrainException();
            }
        }

        public Product getData(long id){
            try {
                var db = new PenatesEntities();
                Product aux = db.Products.Find(id);
                if (aux != null && aux.Deleted) {
                    return null;
                }
                return aux;
            } catch (Exception e){
                throw new DatabaseException(e.Message);
            }
        }

        public Product getData(long id, bool includeDeleted) {
            if (includeDeleted) {
                try {
                    var db = new PenatesEntities();
                    return db.Products.Find(id);
                } catch (Exception e) {
                    throw new DatabaseException(e.Message);
                }
            } else {
                return getData(id);
            }
        }

        public IQueryable<Product> getData(){
            try {

                return this.db.Products.Select(p => p).Where(p => p.Deleted == false);
            } catch (Exception e) {
                throw new DatabaseException(e.Message,e);
            }
        }

        public IQueryable<Product> getData(Sorts sort, ref long total) {
            try {
                var prod = this.getData();
                prod = this.sort(prod, sort);
                total = prod.Count();
                return prod;
            } catch (DatabaseException de) {
                throw de;
            }catch (SortException se) {
                throw se;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Product> getData(Sorts sort, long categoryFilter, ref long total) {
            try {
                var prod = this.getData();
                if (categoryFilter >= 0) {
                    prod = prod.Where(p => p.IDProductCategory == categoryFilter).Select(p => p);
                }
                prod = this.sort(prod, sort);
                total = prod.Count();
                return prod;
            } catch (DatabaseException de) {
                throw de;
            } catch (SortException se) {
                throw se;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Product> getData(string search, Sorts sort, ref long total) {
            List<string> list = this.splitString(search);
            return this.getData(list, sort, ref total);
        }

        public IQueryable<Product> getData(string search, Sorts sort, long categoryFilter, ref long total) {
            List<string> list = this.splitString(search);
            if(categoryFilter < 0){
                return this.getData(list,sort, ref total);
            }else{
                return this.getData(list, sort, categoryFilter, ref total);
            }
        }

        public long getItemsConstraintsNumber(long productID) {
            Product prod = this.db.Products.Find(productID);
            if (prod.Boxes == null) {
                return 0;
            }
            return prod.Boxes.Where(x => x.Deleted == false).Count();
        }

        public List<Constraint> getItemsConstraints(long productID) {
            try {
                Product prod = this.db.Products.Find(productID);
                var result = prod.Boxes.Where(x => x.Deleted == false).Take(this.itemsPerPage).ToList();
                var items = result.Select(x => new Constraint {
                    id = x.BoxID.ToString(),
                    name = Resources.Resources.Product + ": " + x.Product
                });
                return items.ToList();
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.ProductWArt, productID), e.Message);
            }
        }

        public IQueryable<Product> search(IQueryable<Product> query, string search) {
            return this.search(query, this.splitString(search));
        }

        public IQueryable<Product> search(IQueryable<Product> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Barcode.Contains(item) || p.Name.Contains(item) || p.Description.Contains(item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Product> categoryFilter(IQueryable<Product> query, long categoryID) {
            return query.Where(p => p.IDProductCategory == categoryID).Select(p => p);
        }

        public IQueryable<Product> sort(IQueryable<Product> productos, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.BARCODE:
                    productos = productos.OrderBy(p => p.Barcode);
                    break;
                case Sorts.BARCODE_DESC:
                    productos = productos.OrderByDescending(p => p.Barcode);
                    break;
                case Sorts.CATEGORY:
                    productos = productos.OrderBy(p => p.ProductCategory.Description);
                    break;
                case Sorts.CATEGORY_DESC:
                    productos = productos.OrderByDescending(p => p.ProductCategory.Description);
                    break;
                case Sorts.ID:
                    productos = productos.OrderBy(p => p.ProductID);
                    break;
                case Sorts.ID_DESC:
                    productos = productos.OrderByDescending(p => p.ProductID);
                    break;
                case Sorts.NAME:
                    productos = productos.OrderBy(p => p.Name);
                    break;
                case Sorts.NAME_DESC:
                    productos = productos.OrderByDescending(p => p.Name);
                    break;
                default:
                    throw new SortException(Resources.ExceptionMessages.InvalidSortException, sort.ToString());
            }
            return productos;
        }

        public ICollection<Supplier> getSuppliers(long id) {
            try {
                Product aux = this.db.Products.Find(id);
                if (aux == null) {
                    return new List<Supplier>();
                }
                return aux.ProvidedBies.Where(x => x.Inactive == false).Select(x => x.Supplier).ToList();
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.ProductWArt, id), e.Message);
            }
        }

        /// <summary> Agrega un Proveedor a un Producto Determinado si es que existen </summary>
        /// <returns>true si logra agregarlo, false si no lo logra</returns>
        /// <exception cref="IDNotFoundException" Cuando no encuentra o el ProductID o el SupplierID
        /// <exception cref="DatabaseException" Cuando ocurre un error de otra Indole
        public bool addSupplier(ProvidedBy providedBy) {
            try {
                Product prod = this.db.Products.Find(providedBy.IDProduct);
                Supplier supp = this.db.Suppliers.Find(providedBy.IDSupplier);
                if (prod == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Supplier, 
                        Resources.Resources.Product);
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.ProductWArt, providedBy.IDProduct);
                    throw new IDNotFoundException(title, message);
                }
                if (supp == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Supplier,
                        Resources.Resources.Product);
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.SupplierWArt, providedBy.IDSupplier);
                    throw new IDNotFoundException(title, message);
                }
                ProvidedBy prov = this.db.ProvidedBies.Find(providedBy.IDProduct, providedBy.IDSupplier);
                if (prov == null) {
                    this.db.ProvidedBies.Add(providedBy);
                    this.db.SaveChanges();
                } else {
                    this.db.Entry(prov).CurrentValues.SetValues(providedBy);
                    this.db.SaveChanges();
                }
                return true;
            } catch (SaveException e) {
                throw e;
            } catch (IDNotFoundException e) {
                throw e;
            }catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Supplier, 
                        Resources.Resources.Product));
            }
        }

        public bool unasignSupplier(long ProductID, long SupplierID) {
            try {
                Product prod = this.db.Products.Find(ProductID);
                Supplier supp = this.db.Suppliers.Find(SupplierID);
                if (prod == null) {
                    string title = String.Format(Resources.ExceptionMessages.UnassignException, Resources.Resources.Supplier,
                        Resources.Resources.Product);
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.ProductWArt, ProductID);
                    throw new IDNotFoundException(title, message);
                }
                if (supp == null) {
                    string title = String.Format(Resources.ExceptionMessages.UnassignException, Resources.Resources.Supplier,
                        Resources.Resources.Product);
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.SupplierWArt, SupplierID);
                    throw new IDNotFoundException(title, message);
                }
                ProvidedBy providedBy = this.db.ProvidedBies.Find(ProductID, SupplierID);
                if (providedBy != null && providedBy.Inactive == false) {
                    ProvidedBy aux = providedBy;
                    aux.Inactive = true;
                    db.Entry(providedBy).CurrentValues.SetValues(aux);
                    this.db.SaveChanges();
                    return true;
                } else {
                    return false;
                }
            } catch (IDNotFoundException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Supplier, 
                        Resources.Resources.Product));
            }
        }

        public bool existAsSupplier(long ProductID, long SupplierID) {
            Product prod = this.db.Products.Find(ProductID);
            if (prod == null) {
                return false;
            }
            return this.db.ProvidedBies.Any(x => x.IDProduct == ProductID && x.IDSupplier == SupplierID);
        }

        public ProvidedBy getProvidedByData(long ProductID, long SupplierID) {
            ProvidedBy prov = this.db.ProvidedBies.Find(ProductID, SupplierID);
            if (prov == null) {
                return new ProvidedBy();
            }
            return prov;
        }


        /// <summary> Checkea si puede eliminar el Producto con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> Id del producto a eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        private bool checkDeleteConstrains(long id) {
            Product prod = db.Products.Find(id);
            return this.checkDeleteConstrains(prod);
        }

        /// <summary> Checkea si puede eliminar el Producto con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> El Producto a Eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        private bool checkDeleteConstrains(Product prod) {
            if (this.getItemsConstraintsNumber(prod.ProductID) == 0) {
                return true;
            }
            return false;
        }

        private IQueryable<Product> getData(List<string> search) {
            try {
                var data = this.getData();
                foreach (string item in search) {
                    data = data.Where(p => p.Barcode.Contains(item) || p.Name.Contains(item) || p.Description.Contains(item));
                }
                return data;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        private IQueryable<Product> getData(List<string> search, Sorts sort, ref long total) {
            try {
                var prod = this.getData(search);
                prod = this.sort(prod, sort);
                total = prod.Count();
                return prod;
            } catch (DatabaseException de) {
                throw de;
            } catch (SortException se) {
                throw se;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        private IQueryable<Product> getData(List<string> search, Sorts sort, long categoryFilter, ref long total) {
            try {
                var prod = this.getData(search);
                prod = prod.Where(p => p.IDProductCategory == categoryFilter).Select(p => p);
                total = prod.Count();
                prod = this.sort(prod, sort);
                return prod;
            } catch (DatabaseException de) {
                throw de;
            } catch (SortException se) {
                throw se;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        private List<string> splitString(string search) {
            search = search.Trim();
            var searches = search.Split(' ');

            for (int i = 0; i < searches.Count(); i++) {
                searches[i] = searches[i].Trim();
            }
            return new List<string>(searches);
        }

        /// <summary>Retorna todos los productos rankeados por el Autocomplete y no solo los primeros n(Donde n esta 
        /// definido por Properties -> Settings -> autocompleteItems)</summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public IQueryable<Product> getAllAutocomplete(string search) {
            try {
                return this.searchAndRank(search);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Product> getAutocomplete(string search)
        {
            try
            {
                var data = this.searchAndRank(search);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            }
            catch (DatabaseException de)
            {
                throw de;
            }
            catch (Exception e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }

        private IQueryable<Product> searchAndRank(string search)
        {
            var data = this.getData();
            return this.searchAndRank(data, search);
        }

        public IQueryable<Product> searchAndRank(IQueryable<Product> data, string search)
        {
            try
            {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search))
                {
                    data = data.OrderBy(x => x.Name);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<Product>
                {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches)
                {
                    aux = aux.Select(x => new PageRankItem<Product>
                    {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double)x.table.ProductID).Trim() == item ? 1000 : 0) +
                                ((x.table.Name.Contains(item)) ? item.Length : 0) + ((x.table.Name.StartsWith(item)) ? (item.Length * 2) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.Name.Length)
                    .ThenBy(x => x.table.Name)
                    .Select(x => x.table);
            }
            catch (DatabaseException de)
            {
                throw de;
            }
            catch (Exception e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }
    }
}