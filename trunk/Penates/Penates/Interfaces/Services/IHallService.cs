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
    public interface IHallService {

        /// <summary> Guarda los datos de un Centro de Distribucion en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        long Save(HallViewModel hall);

        /// <summary>Elimina un centro de Distribuciones</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        bool Delete(long hallID);

        /// <summary>Obtiene los datos de un Hall</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>HallViewModel</returns>
        /// <exception cref="IDNotFoundException"
        HallViewModel getHallData(long hallID);

        /// <summary>Obtiene los datos de un Sector, pero con los nombres de las categorias en lugar de los IDs</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        HallViewModel getHallSummary(long hallID);

        IQueryable<Hall> getData();

        List<HallTableJson> toJsonArray(IQueryable<Hall> query);

        List<HallTableJson> toJsonArray(ICollection<Hall> list);

        IQueryable<Hall> search(IQueryable<Hall> query, string search);

        IQueryable<Hall> sort(IQueryable<Hall> query, int index, string direction);

        IQueryable<Hall> sort(IQueryable<Hall> query, Sorts sort);

        IQueryable<Hall> filterByDistributionCenter(IQueryable<Hall> query, long? dcID);

        IQueryable<Hall> filterByDeposit(IQueryable<Hall> query, long? depositID);

        IQueryable<Hall> filterBySector(IQueryable<Hall> query, long? hallID);

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool addCategory(long hallID, long categoryID);

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool unnasignCategory(long hallID, long categoryID);

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool saveCategories(long hallID, string categoryiesIDs);

        IQueryable<Hall> getAutocomplete(string search, long? dcID, long? depositID, long? sectorID);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<Hall> query);

        string getHallDescription(long hallID);

        IQueryable<ProductCategory> getCategoryAutocomplete(string search, long? SectorID);

        IQueryable<Rack> getRacks(long HallID);
    }
}
