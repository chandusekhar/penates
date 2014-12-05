using Penates.Models.ViewModels.Transactions.ProductsReceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories
{
    public interface IReceptionRepository
    {

        long Save(ReceptionModel reception);

        bool existsReceptionForOrder(string SuppplierOrderID, long IDSuplier);
        
    }
}
