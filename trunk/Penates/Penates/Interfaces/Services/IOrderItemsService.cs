using Penates.Database;
using Penates.Models.ViewModels.Transactions;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Services {
    interface IOrderItemsService {

        List<OrderItemsTableJson> toJsonArray(IQueryable<SupplierOrderItem> query);

        List<OrderItemsTableJson> toJsonArray(ICollection<SupplierOrderItem> list);

        /// <summary> Agrega un Item a una Orden Determinado si es que existen </summary>
        /// <returns>true si logra agregarlo, false si no lo logra</returns>
        /// <exception cref="ForeignKeyConstraintException" Cuando no encuentra o el ProductID o la orden>
        /// <exception cref="DatabaseException" Cuando ocurre un error de otra Indole>
        bool saveItem(OrderItemsViewModel model);

        /// <summary>Elimina un Item de una orden</summary>
        /// <param name="OrderID">Id de la orden</param>
        /// <param name="SupplierID">Id del proveedor</param>
        /// <param name="ProductID">Id del producto</param>
        /// <exception cref="DatabaseException"></exception>
        /// <returns></returns>
        bool unasignItem(string OrderID, long SupplierID, long ProductID);

        List<OrderItemsTableJson> toJsonArrayVerificationOrder(ICollection<SupplierOrderItem> list);
    }
}
