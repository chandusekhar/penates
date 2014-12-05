using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Exceptions.Views;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.Transactions;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Penates.Repositories.Transactions {
    public class OrderRepository : IOrderRepository{

        PenatesEntities db = new PenatesEntities();
        public int itemsPerPage { get; set; }

        public OrderRepository() {
            this.itemsPerPage = 50;
        }

        public OrderRepository(int itemsPerPage) {
            this.itemsPerPage = itemsPerPage;
        }

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido
        ///         *Error del SP</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public int Save(OrderViewModel order) {
            try {
                Nullable<int> val = null;
                if (this.db.SupplierOrders.Any(o => o.SupplierOrderID == order.OldOrderID &&
                        o.IDSupplier == order.OldSupplierID)) {
                    val = db.SP_SupplierOrder_Save(order.OldOrderID, order.OldSupplierID.Value, order.OrderID, order.SupplierID, order.OrderDate).SingleOrDefault();
                } else {
                    if (this.db.SupplierOrders.Any(o => o.SupplierOrderID == order.OrderID &&
                        o.IDSupplier == order.SupplierID)) {
                        val = db.SP_SupplierOrder_Save(order.OrderID, order.SupplierID, order.OrderID, order.SupplierID, order.OrderDate).SingleOrDefault();
                    } else {
                        val = db.SP_SupplierOrder_Add(order.OrderID, order.SupplierID, order.OrderDate).SingleOrDefault();
                    }
                }
                if (!val.HasValue) {
                    return -1;
                }
                return val.Value;
            }catch(DatabaseException de){
                throw de;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.Order + ": " + order.OrderID));
            }
        }

        /// <summary> Cancela una Orden con un Determinado ID</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        public bool Cancel(string OrderID, long SupplierID) {
            SupplierOrder order = db.SupplierOrders.Find(OrderID, SupplierID);
            if (order == null || order.Canceled == true) {
                return false;
            }
            if (checkCancelConstrains(order)) {
                try {
                    order.Canceled = true;
                    db.SupplierOrders.Attach(order);
                    var entry = db.Entry(order);
                    entry.Property(e => e.Canceled).IsModified = true;
                    db.SaveChanges();
                    return true;
                } catch (Exception e) {
                    throw new DatabaseException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.OrderWArt, OrderID), e.Message);
                }
            } else {
                throw new DeleteConstrainException();
            }
        }

        /// <summary>Descancela una order</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <returns>True si la logra descancelar, false si no estaba cancelada</returns>
        public bool Restore(string OrderID, long SupplierID) {
            SupplierOrder order = db.SupplierOrders.Find(OrderID, SupplierID);
            if (order == null || order.Canceled == false) {
                return false;
            }
            try {
                order.Canceled = false;
                db.SupplierOrders.Attach(order);
                var entry = db.Entry(order);
                entry.Property(e => e.Canceled).IsModified = true;
                db.SaveChanges();
                return true;
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.DeleteException,
                    Resources.Resources.OrderWArt, OrderID), e.Message);
            }
        }

        /// <summary>Obtiene una orden</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <param name="includeCanceled">Indica si incluye o no las canceladas en la busqueda</param>
        /// <returns></returns>
        public SupplierOrder getData(string OrderID, long SupplierID, bool includeCanceled) {
            try {
                SupplierOrder order = db.SupplierOrders.Find(OrderID, SupplierID);
                if (order == null) {
                    return null;
                }
                if (!includeCanceled && order.Canceled) {
                    return null;
                }
                return order;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<SupplierOrder> getData() {
            try {

                return this.db.SupplierOrders;
            } catch (Exception e) {
                throw new DatabaseException(e.Message,e);
            }
        }

      


        public IQueryable<SupplierOrder> getData(bool includeCanceled) {
            try {
                if (!includeCanceled) {
                    return this.db.SupplierOrders.Where(x => x.Canceled == false);
                }
                return this.db.SupplierOrders;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public ICollection<Reception> getReceptions(string OrderID, long SupplierID) {
            try {
                var order = this.getData(OrderID, SupplierID, true);
                if (order == null) {
                    return null;
                }
                return order.Receptions;
            } catch (DatabaseException de) {
                throw de;
            }catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public ICollection<SupplierOrderItem> getItems(string OrderID, long SupplierID) {
            try {
                var order = this.getData(OrderID, SupplierID, true);
                if (order == null) {
                    return null;
                }
                return order.SupplierOrderItems;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IEnumerable<Product> getProducts(string OrderID, long SupplierID) {
            try {
                var order = this.getData(OrderID, SupplierID, true);
                if (order == null) {
                    return null;
                }
                return order.SupplierOrderItems.Select(p => p.ProvidedBy.Product);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public Supplier getSupplier(string OrderID, long SupplierID) {
            try {
                var order = this.getData(OrderID, SupplierID, true);
                if (order == null) {
                    return null;
                }
                return order.Supplier;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<SupplierOrder> getAutocomplete(string search) {
            try{
                var data = this.searchAndRank(search);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<SupplierOrder> getAutocomplete(string search, bool includeCanceled) {
            try {
                IQueryable<SupplierOrder> data;
                if (includeCanceled) {
                    data = this.searchAndRank(search);
                } else {
                    data = this.searchAndRank(search, includeCanceled);
                }
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<SupplierOrder> getAutocomplete(string search, long SupplierID) {
            try {
                var data = this.searchAndRank(search, SupplierID);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<SupplierOrder> getAutocomplete(string search, long SupplierID, bool includeCanceled) {
            try {
                var data = this.searchAndRank(search, SupplierID, includeCanceled);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<SupplierOrder> searchAndRank(string search) {
            var data = this.getData();
            return this.searchAndRank(data, search);
        }

        public IQueryable<SupplierOrder> getOrdersBySupplier(long? supplierID)
        {
            try
            {
                var query = this.getData(false);
                if (supplierID.HasValue)
                {
                    query = query.Where(x => x.IDSupplier == supplierID.Value);
                }
                return query;
            }
            catch (Exception e)
            {
                throw new DatabaseException(e.Message, e);
            }
   
        }

        public IQueryable<SupplierOrder> searchAndRank(IQueryable<SupplierOrder> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.SupplierOrderID).Skip(0).Take(Properties.Settings.Default.autocompleteItems);
                    return data;
                }
            List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<SupplierOrder>{
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<SupplierOrder>{
                        table = x.table,
                        rankPoints = x.rankPoints + (x.table.SupplierOrderID.Trim() == item ? 50000 : 0) + (x.table.SupplierOrderID.StartsWith(item) ? (item.Length*10) : 0) +
                        (x.table.SupplierOrderID.Contains(item) ? item.Length : 0) + (SqlFunctions.StringConvert((double) x.table.IDSupplier).Trim() == item ? 10000 : 0) +
                        ((x.table.Supplier.Name.Contains(item)) ? item.Length : 0) + ((x.table.Supplier.Name.StartsWith(item)) ? (item.Length*2) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.SupplierOrderID.Length)
                    .ThenBy(x => x.table.Supplier.Name)
                    .Select(x => x.table);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<SupplierOrder> searchAndRank(string search, bool includeCanceled) {
            var data = this.getData(includeCanceled);
            return this.searchAndRank(data, search);
        }

        public IQueryable<SupplierOrder> searchAndRank(string search, long SupplierID) {
            var data = this.getData();
            data = data.Where(x => x.IDSupplier == SupplierID);
            return this.searchAndRank(data, search);
        }

        public IQueryable<SupplierOrder> searchAndRank(string search, long SupplierID, bool includeCanceled) {
            var data = this.getData(includeCanceled);
            data = data.Where(x => x.IDSupplier == SupplierID);
            return this.searchAndRank(data, search);
        }

        public IQueryable<SupplierOrder> search(IQueryable<SupplierOrder> query, string search) {
            return this.search(query, this.splitString(search));
        }

        public IQueryable<SupplierOrder> search(IQueryable<SupplierOrder> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(x => x.SupplierOrderID.Contains(item) || x.Supplier.Name.Contains(item) || SqlFunctions.StringConvert((double) x.IDSupplier).Contains(item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<SupplierOrder> sort(IQueryable<SupplierOrder> query, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ID:
                    query = query.OrderBy(p => p.SupplierOrderID).ThenBy(p => p.Supplier.Name).ThenByDescending(p => p.OrderDate).ThenBy(p => p.Canceled);
                    break;
                case Sorts.ID_DESC:
                    query = query.OrderByDescending(p => p.SupplierOrderID).ThenBy(p => p.Supplier.Name);
                    break;
                case Sorts.NAME:
                    query = query.OrderBy(p => p.Supplier.Name).ThenByDescending(p => p.SupplierOrderID);
                    break;
                case Sorts.NAME_DESC:
                    query = query.OrderByDescending(p => p.Supplier.Name).ThenByDescending(p => p.SupplierOrderID);
                    break;
                case Sorts.DATE:
                    query = query.OrderBy(p => p.OrderDate).ThenBy(p => p.Canceled).ThenByDescending(p => p.SupplierOrderID).ThenBy(p => p.Supplier.Name);
                    break;
                case Sorts.DATE_DESC:
                    query = query.OrderByDescending(p => p.OrderDate).ThenBy(p => p.Canceled).ThenByDescending(p => p.SupplierOrderID).ThenBy(p => p.Supplier.Name);
                    break;
                case Sorts.DELETED:
                    query = query.OrderBy(p => p.Canceled).ThenByDescending(p => p.SupplierOrderID).ThenBy(p => p.Supplier.Name);
                    break;
                case Sorts.DELETED_DESC:
                    query = query.OrderByDescending(p => p.Canceled).ThenByDescending(p => p.SupplierOrderID).ThenBy(p => p.Supplier.Name);
                    break;
                default:
                    query = query.OrderByDescending(p => p.OrderDate).ThenBy(p => p.Canceled).ThenByDescending(p => p.SupplierOrderID).ThenBy(p => p.Supplier.Name);
                    break;
            }
            return query;
        }

        public List<Constraint> getReceptionsConstrains(string OrderID, long SupplierID) {
            try {
                SupplierOrder order = this.db.SupplierOrders.Find(OrderID, SupplierID);
                var result = order.Receptions.Take(this.itemsPerPage).ToList();
                var items = result.Select(x => new Constraint {
                    id = Resources.Resources.Order + ": " + OrderID + " & " + Resources.Resources.Supplier + ": " + SupplierID,
                    name = Penates.App_GlobalResources.Forms.ModelFormsResources.OrderDate + ": " + x.ReceivingDate

                });
                return items.ToList();
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.OrderWArt, OrderID + " & " + SupplierID), e.Message);
            }
        }

        public IQueryable<SupplierOrder> filterBySupplier(IQueryable<SupplierOrder> query, long FilterID) {
            return query.Where(o => o.IDSupplier == FilterID);
        }

        /// <summary>Chequea si existe ese Producto en esa Orden Ya</summary>
        /// <param name="ProductID"></param>
        /// <param name="SupplierID"></param>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public bool existInOrder(string OrderID, long SupplierID, long ProductID) {
            Product prod = this.db.Products.Find(ProductID);
            if (prod == null) {
                return false;
            }
            return this.db.SupplierOrderItems.Any(x => x.IDSupplierOrder == OrderID && x.IDProduct == ProductID && x.IDSupplier == SupplierID);
        }

        /// <summary>Devuelve un determinado Producto que se encuentra en una Orden</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <param name="ProductID"></param>
        /// <returns>SupplierOrderItem: Item de la orden a devolver</returns>
        public SupplierOrderItem getOrderItemData(string OrderID, long SupplierID, long ProductID) {
            SupplierOrderItem item = this.db.SupplierOrderItems.Find(OrderID, SupplierID, ProductID);
            if (item == null) {
                return new SupplierOrderItem();
            }
            return item;
        }

        public IQueryable<SupplierOrderItem> getOrderItemsList(string OrderID, long SupplierID)
        {
            return this.db.SupplierOrderItems.Where(x => x.IDSupplierOrder == OrderID && x.IDSupplier == SupplierID);            
        }

        /// <summary> Agrega un Item a una Orden Determinado si es que existen </summary>
        /// <returns>true si logra agregarlo, false si no lo logra</returns>
        /// <exception cref="ForeignKeyConstraintException" Cuando no encuentra o el ProductID o la orden>
        /// <exception cref="DatabaseException" Cuando ocurre un error de otra Indole>
        public bool saveItem(SupplierOrderItem item) {
            try {
                if (!this.db.ProvidedBies.Any(x => x.IDSupplier == item.IDSupplier && x.IDProduct == item.IDProduct)) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Product,
                        Resources.Resources.Order);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.ProductWArt, item.IDProduct);
                    throw new ForeignKeyConstraintException(title, message);
                }
                if(!this.db.SupplierOrders.Any(x => x.SupplierOrderID == item.IDSupplierOrder && x.IDSupplier == item.IDSupplier)){
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Product,
                        Resources.Resources.Order);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.OrderWArt, item.IDSupplierOrder);
                    throw new ForeignKeyConstraintException(title, message);
                }
                SupplierOrderItem aux = this.db.SupplierOrderItems.Find(item.IDSupplierOrder, item.IDSupplier, item.IDProduct);
                if (aux == null) {
                    this.db.SupplierOrderItems.Add(item);
                } else {
                    this.db.Entry(aux).CurrentValues.SetValues(item);
                }
                this.db.SaveChanges();
                return true;
            } catch (ForeignKeyConstraintException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Supplier,
                        Resources.Resources.Product));
            }
        }

        /// <summary>Elimina un Item de una orden</summary>
        /// <param name="OrderID">Id de la orden</param>
        /// <param name="SupplierID">Id del proveedor</param>
        /// <param name="ProductID">Id del producto</param>
        /// <exception cref="DatabaseException"></exception>
        /// <returns></returns>
        public bool unasignItem(string OrderID, long SupplierID, long ProductID) {
            try {
                SupplierOrderItem item = this.db.SupplierOrderItems.Find(OrderID, SupplierID, ProductID);
                if(item != null) {
                    this.db.SupplierOrderItems.Remove(item);
                    this.db.SaveChanges();
                }
                return true;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.UnassignException, Resources.Resources.Product,
                        Resources.Resources.Order));
            }
        }


        /// <summary> Checkea si puede eliminar el proveedor con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> Id del producto a eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        private bool checkCancelConstrains(long OrderID, long SupplierID) {
            SupplierOrder order = db.SupplierOrders.Find(OrderID, SupplierID);
            return this.checkCancelConstrains(order);
        }

        /// <summary> Checkea si puede eliminar el proveedor con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> El Supplier a Eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        private bool checkCancelConstrains(SupplierOrder order) {
            if (order.Receptions == null || order.Receptions.Count == 0){ //Si ya tuve una recepcion no puedo cancelarlo
                return true;
            }
            return false;
        }

        private List<string> splitString(string search) {
            search = search.Trim();
            var searches = search.Split(' ');

            for (int i = 0; i < searches.Count(); i++) {
                searches[i] = searches[i].Trim();
            }
            return new List<string>(searches);
        }


        public bool updateProductsReceivedQuantityInOrder(string orderID, long supplierID, long productID, int quanityty)
        {
            SupplierOrderItem itemToUpdate = getOrderItemData(orderID, supplierID, productID);
            if (itemToUpdate != null)
            {
                itemToUpdate.ReceivedQty = quanityty;
                return saveItem(itemToUpdate);                
            }
            else
            {
                return false;
            }
        }

       public decimal getReceivedQuantity(string OrderID, long SupplierID, string ProductID){
           try{
                ICollection<SupplierOrderItem> orderItems = this.getItems(OrderID,SupplierID);
                return orderItems.FirstOrDefault(x=> x.IDProduct == Int64.Parse(ProductID)).ReceivedQty; 
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
       }
      
    }
}