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
    public interface IRackService {

        /// <summary> Guarda los datos de un Centro de Distribucion en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        long Save(RackViewModel rack);

        /// <summary>Elimina un centro de Distribuciones</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        bool Delete(long rackID);

        /// <summary>Obtiene los datos de un Hall</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>RackViewModel</returns>
        /// <exception cref="IDNotFoundException"
        RackViewModel getRackData(long rackID);

        /// <summary>Obtiene los datos de un Sector, pero con los nombres de las categorias en lugar de los IDs</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        RackViewModel getRackSummary(long rackID);

        IQueryable<Rack> getData();

        List<RackTableJson> toJsonArray(IQueryable<Rack> query);

        List<RackTableJson> toJsonArray(ICollection<Rack> list);

        IQueryable<Rack> search(IQueryable<Rack> query, string search);

        IQueryable<Rack> sort(IQueryable<Rack> query, int index, string direction);

        IQueryable<Rack> sort(IQueryable<Rack> query, Sorts sort);

        IQueryable<Rack> filterByDistributionCenter(IQueryable<Rack> query, long? dcID);

        IQueryable<Rack> filterByDeposit(IQueryable<Rack> query, long? depositID);

        IQueryable<Rack> filterBySector(IQueryable<Rack> query, long? sectorID);

        IQueryable<Rack> filterByHall(IQueryable<Rack> query, long? hallID);

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool addCategory(long rackID, long categoryID);

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool unnasignCategory(long rackID, long categoryID);

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool saveCategories(long rackID, string categoryiesIDs);

        IQueryable<Rack> getAutocomplete(string search, long? dcID, long? depositID, long? sectorID, long? hallID);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<Rack> query);

        IQueryable<ProductCategory> getCategoryAutocomplete(string search, long? HallID);

        void updateUsedSpace(long ShelfSubdivisionID, decimal UsedSpaceInCm);
    }
}
