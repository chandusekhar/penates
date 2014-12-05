using Penates.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Interfaces.Repositories
{
    public interface IActivityStreamRepository
    {

        IQueryable<Sale> getSales(string UserName);

        IQueryable<Transfer> getTransfers(string UserName);

        IQueryable<Reception> getReceptions(string UserName);
    }
}