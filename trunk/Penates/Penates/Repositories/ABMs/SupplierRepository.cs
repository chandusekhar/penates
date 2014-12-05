using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Exceptions.Views;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.Forms;
using Penates.Repositories.Transactions;
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
    public class SupplierRepository : ISupplierRepository{

        PenatesEntities db = new PenatesEntities();
        public int itemsPerPage { get; set; }

        public SupplierRepository() {
            this.itemsPerPage = 50;
        }

        public SupplierRepository(int itemsPerPage) {
            this.itemsPerPage = itemsPerPage;
        }

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido
        ///         *Error del SP</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Save(SupplierViewModel supplier) {
            try {
                Supplier aux = db.Suppliers.Find(supplier.SupplierID);
                Nullable<long> val = null;
                if (aux == null) {
                    val = db.SP_Supplier_Add(supplier.Address, supplier.Email, supplier.Name, supplier.Phone).SingleOrDefault();
                } else {
                    val = db.SP_Supplier_Edit(supplier.SupplierID, supplier.Address, supplier.Email,
                        supplier.Name, supplier.Phone).SingleOrDefault();
                }
                if (!val.HasValue) {
                    return -1;
                }
                return val.Value;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.Suppliers + ": " + supplier.Name));
            }
        }

        /// <summary> Borra un Supplier con un ID determinado</summary>
        /// <param name="id">ID del supplier a eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        public bool Delete(long id) {
            //Se supone que restrinjo para que siempre lo encuentre
            Supplier sup = db.Suppliers.Find(id);
            if (sup == null || sup.Deleted == true) {
                return false;
            }
            if (checkDeleteConstrains(sup)) {
                try {
                    long? ret = db.SP_Supplier_Delete(id).SingleOrDefault();
                    if (!ret.HasValue || ret.Value < 0) {
                        throw new Exception(Resources.ExceptionMessages.StoredProcedureException);
                    }
                    return true;
                } catch (Exception e) {
                    throw new DatabaseException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.SupplierWArt, id), e.Message);
                }
            } else {
                throw new DeleteConstrainException();
            }
        }

        public Supplier getData(long id) {
            try {
                var db = new PenatesEntities();
                Supplier aux = db.Suppliers.Find(id);
                if (aux != null && aux.Deleted) {
                    return null;
                }
                return aux;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public Supplier getData(long id, bool includeDeleted) {
            if (includeDeleted) {
                try {
                    var db = new PenatesEntities();
                    return db.Suppliers.Find(id);
                } catch (Exception e) {
                    throw new DatabaseException(e.Message);
                }
            } else {
                return getData(id);
            }
        }

        public IQueryable<Supplier> getData() {
            try {

                return this.db.Suppliers.Select(p => p).Where(p => p.Deleted == false);
            } catch (Exception e) {
                throw new DatabaseException(e.Message,e);
            }
        }

        public IQueryable<Supplier> getData(int start, int length, Sorts sort, ref long total) {
            try {
                var prod = this.getData();
                prod = this.sort(prod, sort);
                total = prod.Count();
                if (start >= 0 && length >= 0) {
                    prod = prod.Skip(start).Take(length);
                }
                return prod;
            } catch (DatabaseException de) {
                throw de;
            }catch (SortException se) {
                throw se;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Supplier> getData(int start, int length, ref long total) {
            try {
                var prod = this.getData();
                prod = prod.OrderBy(p => p.SupplierID);
                total = prod.Count();
                return prod.Skip(start).Take(length);
            } catch (DatabaseException de) {
                throw de;
            } catch (SortException se) {
                throw se;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Supplier> getData(string search, int start, int length, Sorts sort, ref long total) {
            List<string> list = this.splitString(search);
            return this.getData(list, start, length, sort, ref total);
        }

        public IQueryable<Supplier> getData(string search, int start, int length, ref long total) {
            List<string> list = this.splitString(search);
            return this.getData(list, start, length, ref total);
        }

        public IQueryable<Product> getProducts(long id) {
            try {
                var supplier = this.getData(id, false);
                if (supplier == null) {
                    throw new ItemDeletedException(String.Format(Resources.ExceptionMessages.ItemDeletedException, Resources.Resources.SupplierWArt,
                        id, Resources.Resources.Suppliers));
                }
                return this.db.Products.Where(x => x.ProvidedBies.Any(pb => pb.IDSupplier == id));
            } catch (DatabaseException de) {
                throw de;
            } catch (SortException se) {
                throw se;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Supplier> getAutocomplete(string search, long? productID) {
            try{
                var data = this.searchAndRank(search);
                if (productID.HasValue) {
                    IProductRepository repo = new ProductRepository();
                    IEnumerable<long> aux = repo.getSuppliers(productID.Value).Select(x => x.SupplierID);
                    data = data.Where(x => !aux.Contains(x.SupplierID));
                }
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }


        public IQueryable<SupplierOrder> getOrdersAutocomplete(string search, long? supplierID)
        {
            try
            {
                IOrderRepository orders = new OrderRepository();
                var data = orders.getOrdersBySupplier(supplierID);
                data = orders.searchAndRank(data, search);
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

        public IQueryable<Supplier> searchAndRank(string search) {
            var data = this.getData();
            return this.searchAndRank(data, search);
        }

        public IQueryable<Supplier> searchAndRank(IQueryable<Supplier> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.Name).Skip(0).Take(Properties.Settings.Default.autocompleteItems);
                    return data;
                }
            List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<Supplier>{
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<Supplier>{
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double) x.table.SupplierID).Trim() == item ? 1000 : 0) +
                                ((x.table.Name.Contains(item)) ? item.Length : 0) + ((x.table.Name.StartsWith(item)) ? (item.Length*2) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.Name.Length)
                    .ThenBy(x => x.table.Name)
                    .Select(x => x.table);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Supplier> search(IQueryable<Supplier> query, string search) {
            return this.search(query, this.splitString(search));
        }

        public IQueryable<Supplier> search(IQueryable<Supplier> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Address.Contains(item) || p.Name.Contains(item) || p.Email.Contains(item) || p.Phone.Contains(item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Supplier> sort(IQueryable<Supplier> query, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ADDRESS:
                    query = query.OrderBy(p => p.Address);
                    break;
                case Sorts.ADDRESS_DESC:
                    query = query.OrderByDescending(p => p.Address);
                    break;
                case Sorts.EMAIL:
                    query = query.OrderBy(p => p.Email);
                    break;
                case Sorts.EMAIL_DESC:
                    query = query.OrderByDescending(p => p.Email);
                    break;
                case Sorts.ID:
                    query = query.OrderBy(p => p.SupplierID);
                    break;
                case Sorts.ID_DESC:
                    query = query.OrderByDescending(p => p.SupplierID);
                    break;
                case Sorts.NAME:
                    query = query.OrderBy(p => p.Name);
                    break;
                case Sorts.NAME_DESC:
                    query = query.OrderByDescending(p => p.Name);
                    break;
                default:
                    query = query.OrderBy(p => p.SupplierID);
                    break;
            }
            return query;
        }

        public long getCommercialAgreementsConstraintsNumber(long supplierID) {
            Supplier supp = this.db.Suppliers.Find(supplierID);
            if (supp.CommercialAgreements == null) {
                return 0;
            }
            return supp.CommercialAgreements.Count;
        }

        public List<Constraint> getCommercialAgreementsConstraints(long supplierID) {
            try {
                Supplier supp = this.db.Suppliers.Find(supplierID);
                var result = supp.CommercialAgreements.Take(this.itemsPerPage).ToList();
                var items = result.Select(x => new Constraint {
                    id = x.CommercialAgreementID.ToString(),
                    name = Resources.Resources.Product + ": " + x.Description
                });
                return items.ToList();
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.SupplierWArt, supplierID), e.Message);
            }
        }

        /// <summary>Obtiene las restricciones por la cual no puede ser eliminado</summary>
        /// <param name="productID">Id del Supplier para obtener las restricciones</param>
        /// <returns></returns>
        public long getOrderConstraintsNumber(long supplierID) {
            var query = this.db.SupplierOrders
                .Where(x => x.IDSupplier == supplierID && x.Canceled == false);
            query = query.Where(x => x.Receptions.Count == 0);
            if (query == null) {
                return 0;
            }
            return query.Count();
        }

        public List<Constraint> getOrderConstraints(long supplierID) {
            try {
                var query = this.db.SupplierOrders
                    .Where(x => x.IDSupplier == supplierID && x.Canceled == false).ToList();
                var items = query.Select(x => new Constraint {
                    id = x.SupplierOrderID.ToString(),
                    name = Penates.App_GlobalResources.Forms.ModelFormsResources.OrderDate + ": " + x.OrderDate
                });
                return items.ToList();
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.SupplierWArt, supplierID), e.Message);
            }
        }


        /// <summary> Checkea si puede eliminar el proveedor con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> Id del producto a eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        private bool checkDeleteConstrains(long id) {
            Supplier sup = db.Suppliers.Find(id);
            return this.checkDeleteConstrains(sup);
        }

        /// <summary> Checkea si puede eliminar el proveedor con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> El Supplier a Eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        private bool checkDeleteConstrains(Supplier sup) {
            if (this.getCommercialAgreementsConstraintsNumber(sup.SupplierID) <= 0 && this.getOrderConstraintsNumber(sup.SupplierID) <= 0){
                return true;
            }
            return false;
        }

        private IQueryable<Supplier> getData(List<string> search) {
            try {
                var data = this.getData();
                foreach (string item in search) {
                    data = data.Where(p => SqlFunctions.StringConvert((double) p.SupplierID).Trim() == item || p.Name.Contains(item) || p.Address.Contains(item) || p.Email.Contains(item));
                }
                return data;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        private IQueryable<Supplier> getData(List<string> search, int start, int length, Sorts sort, ref long total) {
            try {
                var prod = this.getData(search);
                prod = this.sort(prod, sort);
                total = prod.Count();
                return prod.Skip(start).Take(length);
            } catch (DatabaseException de) {
                throw de;
            } catch (SortException se) {
                throw se;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        private IQueryable<Supplier> getData(List<string> search, int start, int length, ref long total) {
            try {
                var prod = this.getData(search);
                prod = prod.OrderBy(p => p.SupplierID);
                total = prod.Count();
                return prod.Skip(start).Take(length);
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
    }
}