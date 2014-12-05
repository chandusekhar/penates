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
    public interface ISectorService {

                /// <summary> Guarda los datos de un Centro de Distribucion en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        long Save(SectorViewModel sector);

        /// <summary>Elimina un centro de Distribuciones</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        bool Delete(long sectorID);

        /// <summary>Obtiene los datos de un Deposito Temporal</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>SectorViewModel</returns>
        /// <exception cref="IDNotFoundException"
        SectorViewModel getSectorData(long sectorID);

        /// <summary>Obtiene los datos de un Sector, pero con los nombres de las categorias en lugar de los IDs</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        SectorViewModel getSectorSummary(long sectorID);

        IQueryable<Sector> getData();

        List<SectorTableJson> toJsonArray(IQueryable<Sector> query);

        List<SectorTableJson> toJsonArray(ICollection<Sector> list);

        IQueryable<Sector> search(IQueryable<Sector> query, string search);

        IQueryable<Sector> sort(IQueryable<Sector> query, int index, string direction);

        IQueryable<Sector> sort(IQueryable<Sector> query, Sorts sort);

        IQueryable<Sector> filterByDistributionCenter(IQueryable<Sector> query, long? dcID);

        IQueryable<Sector> filterByDeposit(IQueryable<Sector> query, long? depositID);

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool addCategory(long sectorID, long categoryID);

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool unnasignCategory(long sectorID, long categoryID);

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool saveCategories(long sectorID, string categoryiesIDs);

        IQueryable<Sector> getAutocomplete(string search, long? dcID, long? depositID);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<Sector> query);

        string getSectorDescription(long sectorID);

        IQueryable<ProductCategory> getCategoryAutocomplete(string search, long? DepositID);

        long? getDistributionCenter(long sectorID);

        IQueryable<Hall> getHalls(long SectorID);
    }
}
