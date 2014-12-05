using Penates.Database;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Penates.Models.ViewModels.Forms;
using Penates.Utils.JSON;
using Penates.Models.ViewModels.DC;
using Penates.Utils.JSON.TableObjects;

namespace Penates.Interfaces.Services {
    public interface IContainerService {

        /// <summary> Guarda los datos de un Contenerdor en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        long Save(ContainerViewModel container);

        /// <summary>Elimina un Contenedor</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        bool Delete(long containerID);

        /// <summary>Obtiene los datos de un container</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>ContainerViewModel</returns>
        /// <exception cref="IDNotFoundException"
        ContainerViewModel getContainerData(long containerID);

        IQueryable<Container> getData();

        List<ContainerTableJson> toJsonArray(IQueryable<Container> query);

        List<ContainerTableJson> toJsonArray(ICollection<Container> list);

        IQueryable<Container> search(IQueryable<Container> query, string search);

        IQueryable<Container> sort(IQueryable<Container> query, int index, string direction);

        IQueryable<Container> sort(IQueryable<Container> query, Sorts sort);

        string getContainerTypeDescription(long containerID);

        Container getEmptyContainer(long ContainerTypeID);

        void setContainerUsedSpace(long ContainerID, decimal UsedSpace);

        IQueryable<Container> filterByDistributionCenter(IQueryable<Container> query, long? dcID);

        IQueryable<Container> filterByDeposit(IQueryable<Container> query, long? depositID);

        IQueryable<Container> filterByTemporaryDeposit(IQueryable<Container> query, long? depositID);

        IQueryable<Container> filterBySector(IQueryable<Container> query, long? sectorID);

        IQueryable<Container> filterByRack(IQueryable<Container> query, long? rackID);

        IQueryable<Container> filterByType(IQueryable<Container> query, long? typeID);

        IQueryable<Container> getAutocomplete(string search);

        IQueryable<Container> getAutocomplete(string search, long? dcID, long? depositID, long? temporaryDepositID, long? sectorID, long? rackID, long? typeID);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<Container> query);
    }
}
