using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Exceptions.Views;
using Penates.Interfaces.Repositories;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.Transactions;
using Penates.Repositories.ABMs;
using Penates.Repositories.Transactions;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace Penates.Services.Transactions {
    public class OrderService : IOrderService {

        IOrderRepository repository = new OrderRepository();

        /// <summary> Guarda los datos de un proveedor en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como SupplierViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        public bool Save(OrderViewModel order) {
            int value = this.repository.Save(order); //Capturo el ID o Errores del Sp
            switch (value) {
                case -1:
                    throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.OrderWArt));
                case -2:
                    var ex3 = new DuplicatedKeyException();
                    ex3.Attributes.Add(ReflectionExtension.getVarName(() => order.OrderID));
                    ex3.Attributes.Add(ReflectionExtension.getVarName(() => order.SupplierID));
                    throw ex3;
                case -3:
                    var ex2 = new ForeignKeyConstraintException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.OrderWArt, order.OrderID), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                        Resources.Resources.Category, order.SupplierID));
                    ex2.atributeName = ReflectionExtension.getVarName(() => order.SupplierID);
                    throw ex2;
            }
            return true;
        }

        /// <summary>Cancela una Orden con un Determinado ID</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        public bool Cancel(string OrderID, long SupplierID) {
            return this.repository.Cancel(OrderID, SupplierID);
        }

        /// <summary>Descancela una order</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <returns>True si la logra descancelar, false si no estaba cancelada</returns>
        public bool Restore(string OrderID, long SupplierID) {
            return this.repository.Restore(OrderID, SupplierID);
        }

        /// <summary>Chequea si una orden esta cancelada o no</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <returns>true si esta cancelada, false si no lo esta o no existe</returns>
        public bool isCanceled(string OrderID, long SupplierID) {
            SupplierOrder order = this.repository.getData(OrderID, SupplierID, false);
            if (order == null) {
                return true;
            }
            return false;
        }

        /// <summary>Obtiene Una Orden incluyendo si esta Cancelada</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <returns></returns>
        public OrderViewModel getData(string orderID, long supplierID) {
            return this.getData(orderID, supplierID, true);
        }

        /// <summary>Obtiene Una Orden</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <param name="includeDeleted">Indica si incluir o no ordenes canceladas</param>
        /// <returns></returns>
        public OrderViewModel getData(string orderID, long supplierID, bool includeCanceled) {
            try {
                SupplierOrder aux = this.repository.getData(orderID, supplierID, includeCanceled);
                if (aux == null) {
                    return null;
                }
                OrderViewModel model = new OrderViewModel() {
                    Canceled = aux.Canceled,
                    OldOrderID = aux.SupplierOrderID,
                    OldSupplierID = aux.IDSupplier,
                    OrderDate = aux.OrderDate,
                    OrderID = aux.SupplierOrderID,
                    SupplierID = aux.IDSupplier,
                    SupplierName = aux.Supplier.Name,
                    Received = aux.Received
                };
                //TODO: Probar si esto funciona!!
                model.Total = aux.SupplierOrderItems.Sum(x => x.ProvidedBy.PurchasePrice * x.ItemBoxes);
                return model;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<SupplierOrder> getData() {
            return this.repository.getData();
        }

        public IQueryable<SupplierOrder> getData(bool includeCanceled) {
            return this.repository.getData(includeCanceled);
        }

        public List<OrderTableJson> toJsonArray(IQueryable<SupplierOrder> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<OrderTableJson> toJsonArray(ICollection<SupplierOrder> list) {
            List<OrderTableJson> result = new List<OrderTableJson>();
            OrderTableJson aux;
            foreach (SupplierOrder order in list) {
                aux = new OrderTableJson() {
                    ID = order.SupplierOrderID,
                    SupplierName = order.Supplier.Name,
                    SupplierID = order.IDSupplier,
                    OrderDate = order.OrderDate.ToLongDateString(),
                    Canceled = order.Canceled,
                    Received = order.Received,
                    //TODO: probar que funciona
                    Total = order.SupplierOrderItems.Sum(x => x.ProvidedBy.PurchasePrice * x.ItemBoxes)
                };
                result.Add(aux);
            }
            return result;
        }

        public IQueryable<SupplierOrder> filterBySupplier(IQueryable<SupplierOrder> query, long? FilterID) {
            if (FilterID.HasValue) {
                return this.repository.filterBySupplier(query, FilterID.Value);
            }
            return query;
        }

        public ConstraintViewModel getConstrains() {
            return new ConstraintViewModel(
                String.Format(Resources.Constraints.CancelConstrainTitle, Resources.Resources.OrderWArt),
                String.Format(Resources.Constraints.CantCancelMessage, Resources.Resources.OrderWArt, Resources.Resources.Receptions)){
                TableWithConstrain = "Receptions"
            };
        }

        public ICollection<Reception> getReceptions(string OrderID, long? SupplierID) {
            if(String.IsNullOrEmpty(OrderID) || !SupplierID.HasValue){
                throw new IDNotFoundException(String.Format(Resources.ExceptionMessages.IDNotFoundException, 
                    Resources.Resources.Order, "null"));
            }else{
                return this.repository.getReceptions(OrderID, SupplierID.Value);
            }
        }

        public ICollection<SupplierOrderItem> getItems(string OrderID, long? SupplierID) {
            if (String.IsNullOrEmpty(OrderID) || !SupplierID.HasValue) {
                throw new IDNotFoundException(String.Format(Resources.ExceptionMessages.IDNotFoundException,
                    Resources.Resources.Order, "null"));
            } else {
                return this.repository.getItems(OrderID, SupplierID.Value);
            }
        }

        public IEnumerable<Product> getProducts(string OrderID, long? SupplierID) {
            if (String.IsNullOrEmpty(OrderID) || !SupplierID.HasValue) {
                throw new IDNotFoundException(String.Format(Resources.ExceptionMessages.IDNotFoundException,
                    Resources.Resources.Order, "null"));
            } else {
                return this.repository.getProducts(OrderID, SupplierID.Value);
            }
        }

        public Supplier getSupplier(string OrderID, long? SupplierID) {
            if (String.IsNullOrEmpty(OrderID) || !SupplierID.HasValue) {
                throw new IDNotFoundException(String.Format(Resources.ExceptionMessages.IDNotFoundException,
                    Resources.Resources.Order, "null"));
            } else {
                return this.repository.getSupplier(OrderID, SupplierID.Value);
            }
        }

        public IQueryable<SupplierOrder> getAutocomplete(string search) {
            return this.repository.getAutocomplete(search);
        }

        public IQueryable<SupplierOrder> getAutocomplete(string search, bool includeCanceled) {
            return this.repository.getAutocomplete(search, includeCanceled);
        }

        public IQueryable<SupplierOrder> getAutocomplete(string search, long SupplierID) {
            return this.repository.getAutocomplete(search, SupplierID);
        }

        public IQueryable<SupplierOrder> getAutocomplete(string search, long SupplierID, bool includeCanceled) {
            return this.repository.getAutocomplete(search, SupplierID, includeCanceled);
        }

        public IQueryable<SupplierOrder> searchAndRank(string search) {
            return this.repository.searchAndRank(search);
        }

        public IQueryable<SupplierOrder> searchAndRank(IQueryable<SupplierOrder> data, string search) {
            return this.repository.searchAndRank(data, search);
        }

        public IQueryable<SupplierOrder> searchAndRank(string search, bool includeCanceled) {
            return this.repository.searchAndRank(search, includeCanceled);
        }

        public IQueryable<SupplierOrder> searchAndRank(string search, long SupplierID) {
            return this.repository.searchAndRank(search, SupplierID);
        }

        public IQueryable<SupplierOrder> searchAndRank(string search, long SupplierID, bool includeCanceled) {
            return this.repository.searchAndRank(search, SupplierID, includeCanceled);
        }

        public List<AutocompleteItemStringID> toJsonAutocomplete(IQueryable<SupplierOrder> query) {
            List<SupplierOrder> list = query.ToList();
            List<AutocompleteItemStringID> result = new List<AutocompleteItemStringID>();
            AutocompleteItemStringID aux;
            foreach (SupplierOrder order in list) {
                aux = new AutocompleteItemStringID() { ID = order.SupplierOrderID, Label = order.SupplierOrderID,
                    Description = "(" + order.IDSupplier + ")" + order.Supplier.Name};
                result.Add(aux);
            }
            return result;
        }

        public Sorts toSort(int sortColumnIndex, string sortDirection) {

            switch (sortColumnIndex) {
                case 0:
                    if (sortDirection == "asc") {
                        return Sorts.ID;
                    } else {
                        return Sorts.ID_DESC;
                    }
                case 1:
                    if (sortDirection == "asc") {
                        return Sorts.NAME;
                    } else {
                        return Sorts.NAME_DESC;
                    }
                case 2:
                    if (sortDirection == "asc") {
                        return Sorts.DATE;
                    } else {
                        return Sorts.DATE_DESC;
                    }
                case 3:
                    if (sortDirection == "asc") {
                        return Sorts.DELETED;
                    } else {
                        return Sorts.DELETED_DESC;
                    }
                default:
                    throw new SortException(Resources.ExceptionMessages.InvalidSortException, "Col: " + sortColumnIndex);
            }
        }

        public IQueryable<SupplierOrder> search(IQueryable<SupplierOrder> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<SupplierOrder> sort(IQueryable<SupplierOrder> query, int index, string direction) {
            try {
                Sorts sort = this.toSort(index, direction);
                return this.repository.sort(query, sort);
            } catch (SortException) {
                return query;
            }
        }

        public IQueryable<SupplierOrder> sort(IQueryable<SupplierOrder> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        /// <summary>Chequea si existe ese Producto en esa Orden Ya</summary>
        /// <param name="ProductID"></param>
        /// <param name="SupplierID"></param>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public bool existInOrder(string OrderID, long? SupplierID, long? ProductID) {
            if (!String.IsNullOrWhiteSpace(OrderID) && ProductID.HasValue && SupplierID.HasValue) {
                return this.repository.existInOrder(OrderID, SupplierID.Value, ProductID.Value);
            }
            return false;
        }

        /// <summary>Obtiene los datos de un Producto de una Orden especifica</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <param name="ProductID"></param>
        /// <returns>OrderItemsViewModel</returns>
        public OrderItemsViewModel getOrderItemData(string OrderID, long SupplierID, long ProductID) {
            SupplierOrderItem item = this.repository.getOrderItemData(OrderID, SupplierID, ProductID);
            return new OrderItemsViewModel(item);
        }

        /// <summary>Obtiene la lista de Productos de una Orden especifica</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <returns>List<SupplierOrderItem></returns>
        public IQueryable<SupplierOrderItem> getOrderItemsList(string OrderID, long SupplierID)
        {
            return this.repository.getOrderItemsList(OrderID, SupplierID);
        }

        /// <summary>Obtiene los Productos a autocompletar para una Orden Omitiendo los que ya existen</summary>
        /// <param name="search">Cadena del Autocomplete</param>
        /// <param name="OrderID">ID de la Orden a la que se va a agregar</param>
        /// <param name="SupplierID">ID del proveedor de la Orden</param>
        /// <returns>IQueryable<Product></returns>
        public IQueryable<Product> getAutocompleteItems(string search, string OrderID, long? SupplierID) {
            IProductRepository prodRepo = new ProductRepository();
            IQueryable<Product> autocomplete = prodRepo.getData();
            if (SupplierID.HasValue) {
                autocomplete = autocomplete.Where(x => x.ProvidedBies.Any(pb => pb.IDSupplier == SupplierID.Value));
            }
            if (SupplierID.HasValue && !String.IsNullOrWhiteSpace(OrderID)) {
                SupplierOrder order = this.repository.getData(OrderID, SupplierID.Value, true);
                if (order != null) {
                    IEnumerable<long> aux = order.SupplierOrderItems.Select(x => x.IDProduct);
                    autocomplete = autocomplete.Where(x => !aux.Contains(x.ProductID));
                }
            }
            autocomplete = prodRepo.searchAndRank(autocomplete, search);
            return autocomplete.Skip(0).Take(Properties.Settings.Default.autocompleteItems).AsQueryable();
        }


        public bool updateProductsReceivedQuantityInOrder(string orderID, long supplierID, long productID, int quanityty)
        {
            return repository.updateProductsReceivedQuantityInOrder(orderID, supplierID, productID, quanityty);
        }

        public decimal getReceivedQuantity(string OrderID, long SupplierID, string ProductID)
        {
            return repository.getReceivedQuantity(OrderID, SupplierID, ProductID);
        }
    }
}