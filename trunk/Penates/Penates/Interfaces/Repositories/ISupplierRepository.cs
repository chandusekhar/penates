using Penates.Database;
using Penates.Interfaces.Models;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Interfaces.Repositories {
    public interface ISupplierRepository {
        long Save(SupplierViewModel supplier);

        ///// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        ///// <param name="id">ID a eliminar</param>
        ///// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        ///// <exception cref="DatabaseException"
        //[Obsolete("This function should not be used!!")]
        // bool Delete(IEnumerable<long> ids);

        bool Delete(long id);

        Supplier getData(long id);

        Supplier getData(long id, bool includeDeleted);

        IQueryable<Supplier> getData();

        IQueryable<Supplier> getData(int start, int length, Sorts sort, ref long total);

        IQueryable<Supplier> getData(int start, int length, ref long total);

        IQueryable<Supplier> getData(string search, int start, int length, Sorts sort, ref long total);

        IQueryable<Supplier> getData(string search, int start, int length, ref long total);

        IQueryable<Product> getProducts(long id);

        IQueryable<Supplier> getAutocomplete(string search, long? productID);

        IQueryable<SupplierOrder> getOrdersAutocomplete(string search, long? supplierID);

        IQueryable<Supplier> searchAndRank(string search);

        IQueryable<Supplier> searchAndRank(IQueryable<Supplier> data, string search);

        IQueryable<Supplier> search(IQueryable<Supplier> query, string search);

        IQueryable<Supplier> sort(IQueryable<Supplier> query, Sorts sort);

        long getCommercialAgreementsConstraintsNumber(long supplierID);

        List<Constraint> getCommercialAgreementsConstraints(long supplierID);

        long getOrderConstraintsNumber(long supplierID);

        List<Constraint> getOrderConstraints(long supplierID);
    }
}