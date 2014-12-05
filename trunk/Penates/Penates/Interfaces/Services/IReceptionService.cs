using Penates.Models.ViewModels.Transactions.ProductsReceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Services
{
    public interface IReceptionService
    {
       decimal save(ReceptionModel reception);

       bool existReceptionForOrder(string SupplierOrderID, long SupplierID);
    }
}
