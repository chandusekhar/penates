using Penates.Database;
using Penates.Models;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Penates.Interfaces.Services {
    public interface IBoxService {

        /// <summary> Guarda los datos de una caja en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        long Add(BoxViewModel box);

        /// <summary> Guarda los datos de una caja en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        long Add(ExternalBoxViewModel box);

        /// <summary> Guarda los datos de una caja en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        long Edit(BoxViewModel box);

        /// <summary> Guarda los datos de una caja en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        long Edit(ExternalBoxViewModel box);

        /// <summary>Obtiene los datos de una Caja</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>HallViewModel</returns>
        /// <exception cref="IDNotFoundException"
        ABoxViewModel getBoxInfo(long boxID);

        /// <summary>Obtiene los datos de una Caja</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>HallViewModel</returns>
        /// <exception cref="IDNotFoundException"
        ExternalBoxViewModel getExternalBoxInfo(long boxID);

        /// <summary>Obtiene los datos de una Caja</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>HallViewModel</returns>
        /// <exception cref="IDNotFoundException"
        BoxViewModel getInternalBoxInfo(long boxID);

        bool isExternal(long boxID);

        IQueryable<Box> getData();

        List<BoxTableJson> toJsonArray(IQueryable<Box> query);

        List<BoxTableJson> toJsonArray(ICollection<Box> list);

        IQueryable<Box> search(IQueryable<Box> query, string search);

        IQueryable<Box> sort(IQueryable<Box> query, int index, string direction);

        IQueryable<Box> sort(IQueryable<Box> query, Sorts sort);

        IQueryable<Box> filterByDistributionCenter(IQueryable<Box> query, long? dcID);

        IQueryable<Box> filterByDeposit(IQueryable<Box> query, long? depositID);

        IQueryable<Box> filterByTemporaryDeposit(IQueryable<Box> query, long? depositID);

        IQueryable<Box> filterBySector(IQueryable<Box> query, long? sectorID);

        IQueryable<Box> filterByRack(IQueryable<Box> query, long? rackID);

        IQueryable<Box> filterByPack(IQueryable<Box> query, long? packID);

        IQueryable<Box> filterByState(IQueryable<Box> query, long? stateID);

        IQueryable<Box> filterByProduct(IQueryable<Box> query, long? productID);

        Status sell(long boxID, long sellID);

        IQueryable<Box> getAutocomplete(string search, long? dcID, long? depositID, long? temporaryDepositID, long? sectorID, long? rackID);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<Box> query);

        bool setContainerID(long BoxID, long ContainerID);
    }
}
