using System;
using Penates.Models.ViewModels.Inventory;

namespace Penates.Repositories.Users
{
    using Penates.Interfaces.Repositories;
    using Penates.Models.ViewModels.Users;
    using System.Linq;
    using Penates.Database;
    using System.Collections.Generic;
    using System.Web.Helpers;
    using Penates.Models;
    using Penates.Exceptions.Database;
    using Penates.Utils;
    using Penates.Exceptions.Views;
    using System.Data.Entity.Core;
    using System.Configuration;
    using System.Web.Configuration;
    using Penates.Utils.Objects;
    using Penates.Utils.Enums;

    public class InventoryRepository : IInventoryRepository
    {
        PenatesEntities db = new PenatesEntities();

        public InventoryViewModel CreateInventory(PredifinedMethodsTypes method, long distributionCenterId, string code, string inventoryName)
        {
            var inventory = new InventoryViewModel();
            var inventoryDB = new Inventory();
            var stockInventedDB = new Stock_Invented();

            // TODO: agarrar los productos a agregar en el inventario e hacer un add a StockInvented!!!
            ICollection<Stock_Invented> stockInvented = new[] { new Stock_Invented() };

            try
            {
                var distributionCenter = GetDistributionCenter(distributionCenterId);
                var products = GetStoredProductsByMethod(method, distributionCenter);

                var inventoryId = db.IDs.Where(x => x.TableName == "Inventory").FirstOrDefault();

                inventoryId.LastID = inventoryId.LastID + 1;

                //Inventory
                inventoryDB.InventoryDate = DateTime.Now;
                inventoryDB.DistributionCenter = distributionCenter;
                inventoryDB.IDDistributionCenter = distributionCenterId;
                inventoryDB.Code = code;
                inventoryDB.InventoryName = inventoryName;
                inventoryDB.InventoryID = inventoryId.LastID;
                //inventoryDB.Stock_Invented = stockInvented;
                //inventoryDB.StoredProductsCount = products.Count();
                //inventoryDB.ValuesOfStoredProducts = ;

                db.Inventories.Add(inventoryDB);
                db.SaveChanges();


                //stockInvented = db.Stock_Invented.ToList();

                //Stock Invented
                foreach( var product in products)
                {
                    stockInventedDB.IDInventory = inventoryId.LastID;
                    stockInventedDB.IDProduct = product.ProductID;
                    stockInventedDB.Inventory = inventoryDB;
                    stockInventedDB.Quantity = products.Count();
                    stockInventedDB.ValueOfStored = product.SellPrice;

                    db.Stock_Invented.Add(stockInventedDB);
                }
                
                db.SaveChanges();


                /*********************/
                inventory.InventoryDate = inventoryDB.InventoryDate;
                inventory.DistributionCenter = inventoryDB.DistributionCenter;
                inventory.IDDistributionCenter = inventoryDB.IDDistributionCenter;
                inventory.Code = inventoryDB.Code;
                inventory.Stock_Invented = inventoryDB.Stock_Invented;
                inventory.MethodType = method;
                inventory.InventoryName = inventoryDB.InventoryName;
                inventory.Stock_Invented = stockInvented;

                return inventory;
            }
            catch (Exception)
            {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.InventoryWArt));
            }
        }

        public InventoryViewModel ViewLastInventory(long distributionCenterId)
        {
            var inventory = new InventoryViewModel();

            try
            {
                var inventoryDB = (from t in db.Inventories
                              orderby t.InventoryDate descending
                              select t).FirstOrDefault();

                inventory.InventoryDate = inventoryDB.InventoryDate;
                inventory.InventoryName = inventoryDB.InventoryName;
                inventory.DistributionCenter = inventoryDB.DistributionCenter;
                inventory.IDDistributionCenter = inventoryDB.IDDistributionCenter;
                inventory.Code = inventoryDB.Code;
                inventory.Stock_Invented = inventoryDB.Stock_Invented;

                return inventory;
            }
            catch (Exception)
            {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.InventoryWArt));
            }
        }

        private DistributionCenter GetDistributionCenter(long distributionCenterId)
        {
            return db.DistributionCenters.FirstOrDefault(x => x.DistributionCenterID == distributionCenterId);
        }

        private ICollection<Product> GetStoredProductsByMethod(PredifinedMethodsTypes method, DistributionCenter distributionCenter)
        {
            var newProducts = new List<Product>();
            var saleProducts = new List<Product>();

            var temporaryDeposits =
                db.TemporaryDeposits.Where(x => x.IDDistributionCenter == distributionCenter.DistributionCenterID);

            foreach (var temporaryDeposit in temporaryDeposits)
            {
                var containers = db.Containers.Where(x => x.IDTemporalDeposit == temporaryDeposit.TemporaryDepositID);

                foreach (var container in containers)
                {
                    var boxes = db.Boxes.OfType<InternalBox>().Where(x => x.IDContainer == container.ContainerID);

                    foreach (var box in boxes.Where(x => x.IDSale == null))
                    {
                        newProducts = db.Products.Where(x => x.ProductID == box.IDProduct).ToList();
                    }

                    foreach (var box in boxes.Where(x => x.IDSale != null))
                    {
                        saleProducts = db.Products.Where(x => x.ProductID == box.IDProduct).ToList();
                    }
                }
            }

            switch (method)
            {
                case PredifinedMethodsTypes.Retail:

                    break;

                case PredifinedMethodsTypes.Fifo:

                    break;

                case PredifinedMethodsTypes.WeightedAverage:

                    break;
            }

            return newProducts;
        }
    }
}