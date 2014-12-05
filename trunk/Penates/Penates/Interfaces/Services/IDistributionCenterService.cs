using Penates.Database;
using Penates.Models;
using Penates.Models.ViewModels.DC;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Services {
    public interface IDistributionCenterService {

        /// <summary> Guarda los datos de un Centro de Distribucion en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException"
        /// <exception cref="ForeignKeyConstraintException"
        /// <exception cref="DataRestrictionProcedureException"
        long SaveInternal(InternalDistributionCenterViewModel dc);

        /// <summary> Guarda los datos de un Centro de Distribucion en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como ExternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException"
        /// <exception cref="ForeignKeyConstraintException"
        /// <exception cref="DataRestrictionProcedureException"
        long SaveExternal(ExternalDistributionCenterViewModel dc);

        /// <summary>Elimina un centro de Distribuciones</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        bool Delete(long dcID);

        /// <summary>Obtiene los datos de un centro de distribucion Interno</summary>
        /// <param name="dcID"></param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        InternalDistributionCenterViewModel getInternalData(long dcID);

        /// <summary>Obtiene los datos de un centro de distribucion Externo</summary>
        /// <param name="dcID"></param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        ExternalDistributionCenterViewModel getExternalData(long dcID);

        IQueryable<DistributionCenter> getData();

        List<DistributionCenterTableJson> toJsonArray(IQueryable<DistributionCenter> query);

        List<DistributionCenterTableJson> toJsonArray(ICollection<DistributionCenter> list);

        IQueryable<DistributionCenter> filterByCountry(IQueryable<DistributionCenter> query, long? countryID);

        IQueryable<DistributionCenter> filterByState(IQueryable<DistributionCenter> query, long? stateID);

        IQueryable<DistributionCenter> filterByType(IQueryable<DistributionCenter> query, long? typeID);

        IQueryable<DistributionCenter> getAutocomplete(string search);

        bool validDeposit(long DistributionCenterID);

        IQueryable<InternalDistributionCenter> getInternalAutocomplete(string search);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<DistributionCenter> query);

        Sorts toSort(int sortColumnIndex, string sortDirection);

        IQueryable<DistributionCenter> search(IQueryable<DistributionCenter> query, string search);

        IQueryable<DistributionCenter> sort(IQueryable<DistributionCenter> query, int index, string direction);

        IQueryable<DistributionCenter> sort(IQueryable<DistributionCenter> query, Sorts sort);

        /// <summary>Retorna ture si es un Centro de Distribucion Interno, sino false</summary>
        /// <param name="DistributionCenterID">Id a buscar</param>
        /// <returns>True si es interno, sino false</returns>
        bool isInternal(long DistributionCenterID);

        long getInternalDistributionCenterProductTypesQuantity(long DistributionCenterID);

        long getExternallDistributionCenterProductTypesQuantity(long DistributionCenterID);

        InternalDistributionCenterDetails getIternalDistributionCenterDetails(long DistributionCenterID);

        long getExternalDistributionCenterDetails(long DistributionCenterID);

        ICollection<Deposit> getDeposits(long DistributionCenterID);

        List<Rack> getAllRacks(long DistributionCenterID);
       
        Location findLocationForContainer(long InternalDistributionCenerID, long ProductCategory, decimal ContainerNeededSpace, decimal VolumeNeeded, decimal HeightNeeded);

        decimal getExternalDCUsableUsedSpace(long ExternalDistributionCenterID);

        void setExternalDCUsableUsedSpace(long DistributionCenterID, decimal Space);

        long getExternalDistributionCenterProductsQuantity(long ExternalDistributionCenterID);

        TempDepositLocation findTemporaryDepositWithSpace(long DistributionCenterID, decimal spaceNeeded);

        void updateTempDepositUsedSpace(long DistributionCenterID, long DepositID, decimal SpaceInCm3);
    }
}
