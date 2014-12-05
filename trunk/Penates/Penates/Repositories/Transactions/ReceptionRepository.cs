using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.Transactions.ProductsReceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Repositories.Transactions
{
    public class ReceptionRepository : IReceptionRepository
    {
        PenatesEntities db = new PenatesEntities();

        public long Save(ReceptionModel reception)
        {
            try
            {
                if(this.db.Receptions.Any(x => x.IDSupplierOrder == reception.IDSupplierOrder && x.IDSupplier == reception.IDSupplier)){
                    Reception rec = this.db.Receptions.FirstOrDefault(x => x.IDSupplierOrder == reception.IDSupplierOrder && x.IDSupplier == reception.IDSupplier);
                    if (rec == null) {
                        return 0;
                    }
                    return rec.ReceiveID;
                }
                Nullable<long> val = null;
                val = db.SP_Reception_Add(reception.date,reception.IDSupplierOrder, reception.IDSupplier, reception.IDDistribucionCenter ,reception.COT,reception.IsPurchase).SingleOrDefault();
                if (!val.HasValue)
                {
                    return -1;
                }
                return val.Value;
            }
            catch (DatabaseException de)
            {
                throw de;
            }
            catch (Exception)
            {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException));
            }
        }


        public bool existsReceptionForOrder(string SuppplierOrderID, long IDSuplier)
        {
            try           
            {             
                IQueryable<Reception> query = this.db.Receptions.Where(x => x.IDSupplierOrder == SuppplierOrderID && x.IDSupplier == IDSuplier);
                if (query.Count() == 0)
                {
                    return false;
                }
                return true;              
            }
            catch (DatabaseException de)
            {
                throw de;
            }
        }
    }
}