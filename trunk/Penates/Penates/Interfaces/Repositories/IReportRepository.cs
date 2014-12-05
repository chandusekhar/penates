namespace Penates.Interfaces.Repositories
{
    using Penates.Models.ViewModels;
    using Penates.Models.ViewModels.Inventory;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IReportRepository
    {
        IEnumerable<InventoryViewModel> GetAllInventories();
    }
}
