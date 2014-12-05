using Penates.Database;
using Penates.Interfaces.Models;
using Penates.Models;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Interfaces.Repositories {
    public interface IProductRepository {

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido
        ///         *Error del SP</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Save(ProductViewModel prod);

        bool Delete(long id);

        IQueryable<Product> search(IQueryable<Product> query, string search);

        IQueryable<Product> search(IQueryable<Product> query, List<string> search);

        IQueryable<Product> categoryFilter(IQueryable<Product> query, long categoryID);

        IQueryable<Product> sort(IQueryable<Product> productos, Sorts sort);

        Product getData(long id);

        Product getData(long id, bool includeDeleted);

        /// <summary>Retorna los Productos que no fueron eliminados</summary>
        /// <returns>IQueryable<Product></returns>
        IQueryable<Product> getData();

        IQueryable<Product> getData(Sorts sort, ref long total);

        IQueryable<Product> getData(Sorts sort, long categoryFilter, ref long total);

        IQueryable<Product> getData(string search, Sorts sort, ref long total);

        IQueryable<Product> getData(string search, Sorts sort, long categoryFilter, ref long total);

        long getItemsConstraintsNumber(long productID);

        List<Constraint> getItemsConstraints(long productID);

        ICollection<Supplier> getSuppliers(long id);

        /// <summary> Agrega un Proveedor a un Producto Determinado si es que existen </summary>
        /// <returns>true si logra agregarlo, false si no lo logra</returns>
        /// <exception cref="IDNotFoundException" Cuando no encuentra o el ProductID o el SupplierID
        /// <exception cref="DatabaseException" Cuando ocurre un error de otra Indole
        bool addSupplier(ProvidedBy providedBy);

        bool unasignSupplier(long ProductID, long SupplierID);

        bool existAsSupplier(long ProductID, long SupplierID);

        ProvidedBy getProvidedByData(long ProductID, long SupplierID);

        /// <summary>Retorna todos los productos rankeados por el Autocomplete y no solo los primeros n(Donde n esta 
        /// definido por Properties -> Settings -> autocompleteItems)</summary>
        /// <param name="search"></param>
        /// <returns></returns>
        IQueryable<Product> getAllAutocomplete(string search);

        IQueryable<Product> getAutocomplete(string search);

        IQueryable<Product> searchAndRank(IQueryable<Product> data, string search);
    }
}