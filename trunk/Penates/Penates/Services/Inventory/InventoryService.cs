using Penates.Exceptions.Database;
using Penates.Models.ViewModels.Inventory;

namespace Penates.Services.Users
{
    using Penates.Repositories.Users;
    using Penates.Models.ViewModels.Users;
    using System.Web.Mvc;
    using Penates.Database;
    using System.Collections.Generic;
    using Penates.Models;
    using Penates.Utils.JSON.TableObjects;
    using System.Linq;
    using Penates.Utils;
    using Penates.Exceptions.Views;
    using Penates.Utils.Enums;
    using Penates.Interfaces.Repositories;
    using Penates.Interfaces.Services;

    public class InventoryService : IInventoryService
    {
        private IInventoryRepository repository = new InventoryRepository();

        public InventoryViewModel CreateInventory(PredifinedMethodsTypes method, long distributionCenterId, string code, string inventoryName)
        {
            return repository.CreateInventory(method, distributionCenterId, code, inventoryName);
        }

        public InventoryViewModel ViewLastInventory(long distributionCenterId)
        {
            return repository.ViewLastInventory(distributionCenterId);
        }
    }
}