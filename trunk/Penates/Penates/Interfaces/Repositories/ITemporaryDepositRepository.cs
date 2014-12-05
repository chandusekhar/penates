using Penates.Database;
using Penates.Models.ViewModels.DC;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories {
    public interface ITemporaryDepositRepository {

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref="ModelErrorException">Para agregar errores al modelo</exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Save(DepositViewModel deposit);

        TemporaryDeposit getDepositInfo(long id);

        IQueryable<TemporaryDeposit> getData(long dcID);

        IQueryable<TemporaryDeposit> search(IQueryable<TemporaryDeposit> query, string search);

        IQueryable<TemporaryDeposit> search(IQueryable<TemporaryDeposit> query, List<string> search);

        IQueryable<TemporaryDeposit> sort(IQueryable<TemporaryDeposit> query, Sorts sort);

        bool Delete(long depositID);

        IQueryable<TemporaryDeposit> getAutocomplete(string search, long? dcID);

        void increaseUsedSpace(long DepositID, decimal SpaceInM3);
    }
}
