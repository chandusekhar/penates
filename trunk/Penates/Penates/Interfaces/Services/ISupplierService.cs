using Penates.Database;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Services {
    interface ISupplierService {
        /// <summary> Guarda los datos de un proveedor en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como SupplierViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        long Save(SupplierViewModel supplier);

        bool Delete(long id);

        SupplierViewModel getData(long id);

        List<List<string>> getDisplayData(int start, int length, int sortColumnIndex, string sortDirection, ref long total);

        List<List<string>> getDisplayData(int start, int length, Sorts sort, ref long total);

        List<List<string>> toJsonArray(IQueryable<Supplier> query);

        List<List<string>> getDisplayData(string search, int start, int length, int sortColumnIndex, string sortDirection, ref long total);

        List<List<string>> getDisplayData(string search, int start, int length, Sorts sort, ref long total);

        List<ConstraintViewModel> getConstrains(long prodID);

        IQueryable<Product> getProducts(long? id);

        Sorts toSort(int sortColumnIndex, string sortDirection);

        IQueryable<Supplier> getAutocomplete(string search, long? productID);

        IQueryable<SupplierOrder> getOrdersAutocomplete(string search, long? supplierID);

        IQueryable<Supplier> searchAndRank(string search);

        IQueryable<Supplier> searchAndRank(IQueryable<Supplier> query, string search);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<Supplier> query);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<SupplierOrder> query);

        IQueryable<Supplier> search(IQueryable<Supplier> query, string search);

        IQueryable<Supplier> sort(IQueryable<Supplier> query, int index, string direction);

        IQueryable<Supplier> sort(IQueryable<Supplier> query, Sorts sort);
    }
}
