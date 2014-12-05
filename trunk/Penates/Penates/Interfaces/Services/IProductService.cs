using Penates.Database;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Services {
    interface IProductService {
        long Save(ProductViewModel prod);

        bool Delete(long id);

        ProductViewModel getProductData(long id);

        IQueryable<Product> search(IQueryable<Product> query, string search);

        IQueryable<Product> categoryFilter(IQueryable<Product> query, long? categoryID);

        IQueryable<Product> sort(IQueryable<Product> productos, int sortColumnIndex, string sortDirection);

        IQueryable<Product> sort(IQueryable<Product> productos, Sorts sort);

        /// <summary>No solo hace skip y take sino que tambien ordena</summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sortColumnIndex">Numero de columna por el cual ordenar</param>
        /// <param name="sortDirection">Ascendente o Descendente</param>
        /// <returns></returns>
        IQueryable<Product> getProductsDisplayData(int sortColumnIndex, string sortDirection, long? categoryID, ref long total);

        /// <summary>No solo hace skip y take sino que tambien ordena</summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sort">Sorts por el cual ordenar</param>
        /// <returns></returns>
        IQueryable<Product> getProductsDisplayData(Sorts sort, long? categoryID, ref long total);

        /// <summary> Busca, pagina y ordena</summary>
        /// <param name="search"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sortColumnIndex">Numero de columna por el cual ordenar</param>
        /// <param name="sortDirection">Ascendente o descendente</param>
        /// <returns></returns>
        IQueryable<Product> getProductsDisplayData(string search, int sortColumnIndex, string sortDirection, long? categoryID, ref long total);

        List<ProductTableJson> getProductsDisplayData(string search, int start, int lenght, int sortColumnIndex, string sortDirection, long? categoryID, ref long total);

        List<ProductTableJson> getProductsDisplayData(int start, int lenght, int sortColumnIndex, string sortDirection, long? categoryID, ref long total);

        /// <summary>Busca pagina y ordenas</summary>
        /// <param name="search"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        IQueryable<Product> getProductsDisplayData(string search, Sorts sort, long? categoryID, ref long total);

        List<ConstraintViewModel> getConstrains(long prodID);

        ICollection<Supplier> getSuppliers(long? id);

        /// <summary>Gets the Sort Enum from the column index and sort direction</summary>
        /// <param name="sortColumnIndex">Column index</param>
        /// <param name="sortDirection">Ascendiente o Descendiente</param>
        /// <returns>Enum por el cual se hace el Sort</returns>
        /// <exception cref="SortException"
        Sorts toProductSort(int sortColumnIndex, string sortDirection);

        List<ProductTableJson> toJsonArray(IQueryable<Product> query);

        List<ProductTableJson> toJsonArray(ICollection<Product> list);

        /// <summary> Agrega un Proveedor a un Producto Determinado si es que existen </summary>
        /// <returns>true si logra agregarlo, false si no lo logra</returns>
        /// <exception cref="IDNotFoundException" Cuando no encuentra o el ProductID o el SupplierID
        /// <exception cref="DatabaseException" Cuando ocurre un error de otra Indole
        bool addSupplier(ProvidedByViewModel model);

        bool unasignSupplier(long ProductID, long SupplierID);

        bool existAsSupplier(long? ProductID, long? SupplierID);

        ProvidedByViewModel getProvidedByData(long ProductID, long SupplierID);

        IQueryable<Product> getAutocomplete(string search);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<Product> query);
        
    }
}
