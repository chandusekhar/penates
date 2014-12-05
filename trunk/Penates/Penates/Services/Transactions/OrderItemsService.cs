using Penates.Database;
using Penates.Interfaces.Repositories;
using Penates.Interfaces.Services;
using Penates.Models.ViewModels.Transactions;
using Penates.Repositories.Transactions;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Services.Transactions {
    public class OrderItemsService: IOrderItemsService {

        IOrderRepository repository = new OrderRepository();

        public List<OrderItemsTableJson> toJsonArray(IQueryable<SupplierOrderItem> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<OrderItemsTableJson> toJsonArray(ICollection<SupplierOrderItem> list) {
            List<OrderItemsTableJson> result = new List<OrderItemsTableJson>();
            OrderItemsTableJson aux;
            foreach (SupplierOrderItem item in list) {
                aux = new OrderItemsTableJson() {
                    ProductID = item.IDProduct,
                    OrderID = item.IDSupplierOrder,
                    SupplierID = item.IDSupplier,
                    ProductName = item.ProvidedBy.Product.Name,
                    SupplierProductCode = item.SupplierProductID,
                    ItemQty = item.ItemBoxes
                };
                result.Add(aux);
            }
            return result;
        }


        public List<OrderItemsTableJson> toJsonArrayVerificationOrder(ICollection<SupplierOrderItem> list)
        {
            List<OrderItemsTableJson> result = new List<OrderItemsTableJson>();
            OrderItemsTableJson aux;
            foreach (SupplierOrderItem item in list)
            {
                aux = new OrderItemsTableJson()
                {
                    DT_RowId = item.IDSupplierOrder + "_" + item.IDSupplier + "_" + item.IDProduct,
                    ProductID = item.IDProduct,
                    OrderID = item.IDSupplierOrder,
                    SupplierID = item.IDSupplier,
                    ProductName = item.ProvidedBy.Product.Name,
                    SupplierProductCode = item.SupplierProductID,
                    ItemQty = item.ItemBoxes,
                    ItemsVerified = item.ReceivedQty
                };
                result.Add(aux);
            }
            return result;
        }


        /// <summary> Agrega un Item a una Orden Determinado si es que existen </summary>
        /// <returns>true si logra agregarlo, false si no lo logra</returns>
        /// <exception cref="ForeignKeyConstraintException" Cuando no encuentra o el ProductID o la orden>
        /// <exception cref="DatabaseException" Cuando ocurre un error de otra Indole>
        public bool saveItem(OrderItemsViewModel model) {
            SupplierOrderItem item = new SupplierOrderItem() {
                IDSupplierOrder = model.OrderID,
                IDSupplier = model.SupplierID,
                IDProduct = model.ProductID,
                SupplierProductID = model.SupplierProductID,
                ItemBoxes = model.Boxes,
                ReceivedQty = model.Boxes
            };
            return this.repository.saveItem(item);
        }

        /// <summary>Elimina un Item de una orden</summary>
        /// <param name="OrderID">Id de la orden</param>
        /// <param name="SupplierID">Id del proveedor</param>
        /// <param name="ProductID">Id del producto</param>
        /// <exception cref="DatabaseException"></exception>
        /// <returns></returns>
        public bool unasignItem(string OrderID, long SupplierID, long ProductID) {
            return this.repository.unasignItem(OrderID, SupplierID, ProductID);
        }
    }
}