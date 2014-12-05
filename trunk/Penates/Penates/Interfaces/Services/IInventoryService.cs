using System;
using Penates.Database;
using Penates.Models.ViewModels.Inventory;
using Penates.Utils.Enums;

namespace Penates.Interfaces.Services {
    public interface IInventoryService
    {
        InventoryViewModel CreateInventory(PredifinedMethodsTypes method, long distributionCenterId, string Code, string InventoryName);

        InventoryViewModel ViewLastInventory(long distributionCenterId);
    }
}
