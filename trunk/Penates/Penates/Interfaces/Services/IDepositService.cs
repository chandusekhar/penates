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
    public interface IDepositService {

        /// <summary> Guarda los datos de un Centro de Distribucion en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        long Save(DepositViewModel depo);

        /// <summary>Elimina un centro de Distribuciones</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        bool Delete(long depositID);

        /// <summary>Obtiene los datos de un Deposito Temporal</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        DepositViewModel getDepositData(long depositID);

        /// <summary>Obtiene los datos de un Deposito Temporal, pero con los nombres de las categorias en lugar de los IDs</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        DepositViewModel getDepositSummery(long depositID);

        IQueryable<Deposit> getData(long? depositID);

        List<DepositTableJson> toJsonArray(IQueryable<Deposit> query);

        List<DepositTableJson> toJsonArray(ICollection<Deposit> list);

        IQueryable<Deposit> search(IQueryable<Deposit> query, string search);

        IQueryable<Deposit> sort(IQueryable<Deposit> query, int index, string direction);

        IQueryable<Deposit> sort(IQueryable<Deposit> query, Sorts sort);

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool addCategory(long depositID, long categoryID);

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool saveCategories(long depositID, string categoryiesIDs);

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool unnasignCategory(long depositID, long categoryID);

        IQueryable<Deposit> getAutocomplete(string search, long? dcID);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<Deposit> query);

        string getDepositDescription(long depositID);

        InternalDistributionCenter getDistributionCenter(long depositID);

        IQueryable<Sector> getSectors(long depositId);
    }
}
