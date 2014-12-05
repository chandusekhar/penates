using Penates.Database;
using Penates.Models.ViewModels.Transactions;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories {
    public interface IOrderRepository {

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido
        ///         *Error del SP</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        int Save(OrderViewModel order);

        /// <summary> Cancela una Orden con un Determinado ID</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        bool Cancel(string OrderID, long SupplierID);

        bool Restore(string OrderID, long SupplierID);

        /// <summary>Obtiene una orden</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <param name="includeCanceled">Indica si incluye o no las canceladas en la busqueda</param>
        /// <returns></returns>
        SupplierOrder getData(string OrderID, long SupplierID, bool includeCanceled);

        IQueryable<SupplierOrder> getData();

        IQueryable<SupplierOrder> getData(bool includeCanceled);

        ICollection<Reception> getReceptions(string OrderID, long SupplierID);

        ICollection<SupplierOrderItem> getItems(string OrderID, long SupplierID);

        IEnumerable<Product> getProducts(string OrderID, long SupplierID);

        Supplier getSupplier(string OrderID, long SupplierID);

        IQueryable<SupplierOrder> getAutocomplete(string search);

        IQueryable<SupplierOrder> getAutocomplete(string search, bool includeCanceled);

        IQueryable<SupplierOrder> getAutocomplete(string search, long SupplierID);

        IQueryable<SupplierOrder> getAutocomplete(string search, long SupplierID, bool includeCanceled);

        IQueryable<SupplierOrder> searchAndRank(string search);

        IQueryable<SupplierOrder> searchAndRank(IQueryable<SupplierOrder> data, string search);

        IQueryable<SupplierOrder> searchAndRank(string search, bool includeCanceled);

        IQueryable<SupplierOrder> searchAndRank(string search, long SupplierID);

        IQueryable<SupplierOrder> searchAndRank(string search, long SupplierID, bool includeCanceled);

        IQueryable<SupplierOrder> search(IQueryable<SupplierOrder> query, string search);

        IQueryable<SupplierOrder> search(IQueryable<SupplierOrder> query, List<string> search);

        IQueryable<SupplierOrder> sort(IQueryable<SupplierOrder> query, Sorts sort);

        List<Constraint> getReceptionsConstrains(string OrderID, long SupplierID);

        IQueryable<SupplierOrder> filterBySupplier(IQueryable<SupplierOrder> query, long FilterID);

        /// <summary>Chequea si existe ese Producto en esa Orden Ya</summary>
        /// <param name="ProductID"></param>
        /// <param name="SupplierID"></param>
        /// <param name="ProductID"></param>
        /// <returns>True si ya existe, false si no existe</returns>
        bool existInOrder(string OrderID, long SupplierID, long ProductID);

        /// <summary>Devuelve un determinado Producto que se encuentra en una Orden</summary>
        /// <param name="OrderID"></param>
        /// <param name="SupplierID"></param>
        /// <param name="ProductID"></param>
        /// <returns>SupplierOrderItem: Item de la orden a devolver</returns>
        SupplierOrderItem getOrderItemData(string OrderID, long SupplierID, long ProductID);

        /// <summary> Agrega un Item a una Orden Determinado si es que existen </summary>
        /// <returns>true si logra agregarlo, false si no lo logra</returns>
        /// <exception cref="ForeignKeyConstraintException" Cuando no encuentra o el ProductID o la orden>
        /// <exception cref="DatabaseException" Cuando ocurre un error de otra Indole>
        bool saveItem(SupplierOrderItem item);

        /// <summary>Elimina un Item de una orden</summary>
        /// <param name="OrderID">Id de la orden</param>
        /// <param name="SupplierID">Id del proveedor</param>
        /// <param name="ProductID">Id del producto</param>
        /// <exception cref="DatabaseException"></exception>
        /// <returns></returns>
        bool unasignItem(string OrderID, long SupplierID, long ProductID);

        IQueryable<SupplierOrder> getOrdersBySupplier(long? supplierID);

        bool updateProductsReceivedQuantityInOrder(string orderID, long supplierID, long productID, int quanityty);

        IQueryable<SupplierOrderItem> getOrderItemsList(string OrderID, long SupplierID);

        decimal getReceivedQuantity(string OrderID, long SupplierID, string ProductID);    
      
    }
}
