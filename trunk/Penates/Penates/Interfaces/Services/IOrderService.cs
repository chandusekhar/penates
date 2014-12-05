using Penates.Database;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.Transactions;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Services {
    public interface IOrderService {

        /// <summary> Guarda los datos de un proveedor en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como SupplierViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        bool Save(OrderViewModel order);

        /// <summary>Cancela una Orden con un Determinado ID</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        bool Cancel(string OrderID, long SupplierID);

        /// <summary>Descancela una order</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <returns>True si la logra descancelar, false si no estaba cancelada</returns>
        bool Restore(string OrderID, long SupplierID);

        /// <summary>Chequea si una orden esta cancelada o no</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <returns>true si esta cancelada, false si no lo esta o no existe</returns>
        bool isCanceled(string OrderID, long SupplierID);


        /// <summary>Obtiene Una Orden incluyendo si esta Cancelada</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <returns></returns>
        OrderViewModel getData(string orderID, long supplierID);

        /// <summary>Obtiene Una Orden</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <param name="includeDeleted">Indica si incluir o no ordenes canceladas</param>
        /// <returns></returns>
        OrderViewModel getData(string orderID, long supplierID, bool includeDeleted);

        IQueryable<SupplierOrder> getData();

        IQueryable<SupplierOrder> getData(bool includeCanceled);

        List<OrderTableJson> toJsonArray(IQueryable<SupplierOrder> query);

        List<OrderTableJson> toJsonArray(ICollection<SupplierOrder> list);

        IQueryable<SupplierOrder> filterBySupplier(IQueryable<SupplierOrder> query, long? FilterID);

        ConstraintViewModel getConstrains();

        ICollection<Reception> getReceptions(string OrderID, long? SupplierID);

        ICollection<SupplierOrderItem> getItems(string OrderID, long? SupplierID);

        IEnumerable<Product> getProducts(string OrderID, long? SupplierID);

        Supplier getSupplier(string OrderID, long? SupplierID);

        IQueryable<SupplierOrder> getAutocomplete(string search);

        IQueryable<SupplierOrder> getAutocomplete(string search, bool includeCanceled);

        IQueryable<SupplierOrder> getAutocomplete(string search, long SupplierID);

        IQueryable<SupplierOrder> getAutocomplete(string search, long SupplierID, bool includeCanceled);

        IQueryable<SupplierOrder> searchAndRank(string search);

        IQueryable<SupplierOrder> searchAndRank(IQueryable<SupplierOrder> data, string search);

        IQueryable<SupplierOrder> searchAndRank(string search, bool includeCanceled);

        IQueryable<SupplierOrder> searchAndRank(string search, long SupplierID);

        IQueryable<SupplierOrder> searchAndRank(string search, long SupplierID, bool includeCanceled);

        List<AutocompleteItemStringID> toJsonAutocomplete(IQueryable<SupplierOrder> query);

        Sorts toSort(int sortColumnIndex, string sortDirection);

        IQueryable<SupplierOrder> search(IQueryable<SupplierOrder> query, string search);

        IQueryable<SupplierOrder> sort(IQueryable<SupplierOrder> query, int index, string direction);

        IQueryable<SupplierOrder> sort(IQueryable<SupplierOrder> query, Sorts sort);

        bool existInOrder(string OrderID, long? SupplierID, long? ProductID);

        /// <summary>Obtiene los datos de un Producto de una Orden especifica</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <param name="ProductID"></param>
        /// <returns>OrderItemsViewModel</returns>
        OrderItemsViewModel getOrderItemData(string OrderID, long SupplierID, long ProductID);

        /// <summary>Obtiene los Productos a autocompletar para una Orden Omitiendo los que ya existen</summary>
        /// <param name="search">Cadena del Autocomplete</param>
        /// <param name="OrderID">ID de la Orden a la que se va a agregar</param>
        /// <param name="SupplierID">ID del proveedor de la Orden</param>
        /// <returns>IQueryable<Product></returns>
        IQueryable<Product> getAutocompleteItems(string search, string OrderID, long? SupplierID);

        bool updateProductsReceivedQuantityInOrder(string orderID, long supplierID, long ProductID, int quanityty);

        IQueryable<SupplierOrderItem> getOrderItemsList(string OrderID, long SupplierID);

        decimal getReceivedQuantity(string OrderID, long SupplierID, string ProductID);
    }
}
