using Penates.Database;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories {
    public interface IDistributionCenterRepository {

        DistributionCenter getData(long id);

        DistributionCenter getData(long id, bool includeDeleted);

        IQueryable<DistributionCenter> getData();

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        bool Delete(long id);

        bool checkFisicalDelete(long id);

        bool checkFisicalDelete(DistributionCenter dc);

        long getReceptionsConstraintsNumber(DistributionCenter centerID);

        List<Constraint> getReceptionsConstraints(long centerID);

        long getTransfersConstraintsNumber(DistributionCenter centerID);

        List<Constraint> getTransfersConstraints(long centerID);

        List<Constraint> getTransfers1Constraints(long centerID);

        long getInventoriesConstraintsNumber(DistributionCenter centerID);

        long getPoliciesConstraintsNumber(DistributionCenter centerID);

        List<Constraint> getPoliciesConstraints(long centerID);

        long getSalesConstraintsNumber(DistributionCenter centerID);

        List<Constraint> getSalesConstraints(long centerID);

        IQueryable<DistributionCenter> search(IQueryable<DistributionCenter> query, string search);

        IQueryable<DistributionCenter> search(IQueryable<DistributionCenter> query, List<string> search);

        IQueryable<DistributionCenter> countryFilter(IQueryable<DistributionCenter> query, long countryID);

        IQueryable<DistributionCenter> stateFilter(IQueryable<DistributionCenter> query, long stateID);

        IQueryable<DistributionCenter> typeFilter(IQueryable<DistributionCenter> query, long typeID);

        IQueryable<DistributionCenter> sort(IQueryable<DistributionCenter> dc, Sorts sort);


        /// <summary> Checkea si puede eliminar el Producto con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> Id del producto a eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        bool checkDeleteConstrains(long id);

        /// <summary> Checkea si puede eliminar el Producto con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> El Producto a Eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        bool checkDeleteConstrains(DistributionCenter dc);

        IQueryable<DistributionCenter> getAutocomplete(string search);

        IQueryable<DistributionCenter> searchAndRank(string search);

        IQueryable<DistributionCenter> searchAndRank(IQueryable<DistributionCenter> data, string search);
    }
}
