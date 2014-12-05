using Penates.Database;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Interfaces.Repositories {
    public interface IBoxRepository {

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Add(BoxViewModel box);

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Add(ExternalBoxViewModel box);

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Edit(BoxViewModel box);

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Edit(ExternalBoxViewModel box);

        Box getBoxInfo(long id);

        IQueryable<Box> getData();

        IQueryable<Box> search(IQueryable<Box> query, string search);

        IQueryable<Box> search(IQueryable<Box> query, List<string> search);

        IQueryable<Box> sort(IQueryable<Box> query, Sorts sort);

        IQueryable<Box> filterByDistributionCenter(IQueryable<Box> query, long dcID);

        IQueryable<Box> filterByDeposit(IQueryable<Box> query, long depositID);

        IQueryable<Box> filterByTemporaryDeposit(IQueryable<Box> query, long depositID);

        IQueryable<Box> filterBySector(IQueryable<Box> query, long sectorID);

        IQueryable<Box> filterByRack(IQueryable<Box> query, long rackID);

        IQueryable<Box> filterByPack(IQueryable<Box> query, long packID);

        IQueryable<Box> filterByStatus(IQueryable<Box> query, long statusID);

        IQueryable<Box> filterByProduct(IQueryable<Box> query, long productID);

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        Status sell(long boxID, long saleID);

        IQueryable<Box> getAutocomplete(string search, long? dcID, long? depositID, long? temporaryDepositID, long? sectorID, long? rackID);

        bool Delete(long BoxId);

        bool setContainerID(long BoxID, long ContainerID);
    }
}