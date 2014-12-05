using Penates.Exceptions.Database;
using Penates.Models.ViewModels.Inventory;
using Penates.Models.ViewModels.Users;

namespace Penates.Interfaces.Repositories
{
    using Penates.Database;
    using Penates.Models;
    using Penates.Utils;
    using Penates.Utils.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IInventoryRepository
    {

        InventoryViewModel CreateInventory(PredifinedMethodsTypes method, long distributionCenterId, string code, string inventoryName);

        InventoryViewModel ViewLastInventory(long distributionCenterId);
    }
}
