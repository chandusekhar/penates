using Penates.Database;
using Penates.Models;
using Penates.Models.ViewModels.DC;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Services {
    public interface ITemporaryDepositService {

        /// <summary> Guarda los datos de un Centro de Distribucion en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        long Save(DepositViewModel dc);

        /// <summary>Elimina un centro de Distribuciones</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        bool Delete(long dcID);

        /// <summary>Obtiene los datos de un Deposito Temporal</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        DepositViewModel getDepositData(long depositID);

        IQueryable<TemporaryDeposit> getData(long? dcID);

        List<DepositTableJson> toJsonArray(IQueryable<TemporaryDeposit> query);

        List<DepositTableJson> toJsonArray(ICollection<TemporaryDeposit> list);

        IQueryable<TemporaryDeposit> search(IQueryable<TemporaryDeposit> query, string search);

        IQueryable<TemporaryDeposit> sort(IQueryable<TemporaryDeposit> query, int index, string direction);

        IQueryable<TemporaryDeposit> sort(IQueryable<TemporaryDeposit> query, Sorts sort);

        IQueryable<TemporaryDeposit> getAutocomplete(string search, long? dcID);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<TemporaryDeposit> query);

        TemporaryDeposit findDepositWithSpace(long DistributionCenterID, decimal SpaceNeededInM3);

        void increaseUsedSpace(long DepositID, decimal SpaceInM3);
    }
}
