using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Interfaces.Services;
using Penates.Models.ViewModels.Transactions.ProductsReceptions;
using Penates.Repositories.Transactions;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Services.Transactions
{
    public class ReceptionService : IReceptionService
    {
        IReceptionRepository repository = new ReceptionRepository();

        public decimal save(ReceptionModel reception)
        {
            long value = this.repository.Save(reception); //Capturo el ID o Errores del Sp
            switch (value)
            {
                case -1:
                    throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.OrderWArt));
                case -2:
                    var ex3 = new DuplicatedKeyException();
                    ex3.Attributes.Add(ReflectionExtension.getVarName(() => reception.COT));
                    ex3.Attributes.Add(ReflectionExtension.getVarName(() => reception.IDSupplierOrder));
                    throw ex3;
                case -3:
                    var ex2 = new ForeignKeyConstraintException(String.Format(Resources.ExceptionMessages.SaveException));
                    ex2.atributeName = ReflectionExtension.getVarName(() => reception.COT);
                    throw ex2;
            }
            return value;
        }

        public bool existReceptionForOrder(string SupplierOrderID, long SupplierID)
        {
            return this.repository.existsReceptionForOrder(SupplierOrderID, SupplierID);
        }


    }
}