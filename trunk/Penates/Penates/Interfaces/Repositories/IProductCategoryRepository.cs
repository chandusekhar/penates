using Penates.Database;
using Penates.Interfaces.Models;
using Penates.Models.ViewModels;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Interfaces.Repositories {
    public interface IProductCategoryRepository {

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>true si funciona, sino una excepcion</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Save(ProductCategory prod);

        bool Delete(long id);

        ProductCategory getData(long id);

        IEnumerable<ProductCategory> getCategories();

        IQueryable<ProductCategory> getData();

        IQueryable<ProductCategory> getAutocomplete(string search);

        IQueryable<ProductCategory> getAutocomplete(string search, long? categoryID);

        IQueryable<ProductCategory> searchAndRank(string search);

        IQueryable<ProductCategory> searchAndRank(IQueryable<ProductCategory> data, string search);

        IEnumerable<ProductCategory> searchAndRank(IEnumerable<ProductCategory> data, string search);

        ICollection<ProductCategory> getAssignableCategories(ICollection<ProductCategory> list);

        IQueryable<ProductCategory> search(IQueryable<ProductCategory> query, string search);

        IQueryable<ProductCategory> search(IQueryable<ProductCategory> query, List<string> search);

        IQueryable<ProductCategory> sort(IQueryable<ProductCategory> query, Sorts sort);

        long getProductConstraintsNumber(long supplierID);

        List<Constraint> getProductConstraints(long supplierID);

        /// <summary>Agrega una categoria padre a una categoria</summary>
        /// <param name="categoryID">Categoria a la que se le va a agregar</param>
        /// <param name="parentID">Categoria Padre</param>
        /// <returns>True si la logra agregar, false si ya esta agregada</returns>
        /// <exception cref="IDNotFoundException"
        /// <exception cref="DatabaseException"
        bool addParent(long categoryID, long parentID);

        /// <summary>Agrega una categoria hija a una categoria</summary>
        /// <param name="categoryID">Categoria a la que se le va a agregar</param>
        /// <param name="parentID">Categoria Padre</param>
        /// <returns>True si la logra agregar, false si ya esta agregada</returns>
        /// <exception cref="IDNotFoundException"
        /// <exception cref="DatabaseException"
        bool addChild(long categoryID, long childID);

        bool unasignParent(long catgoryID, long parentID);

        bool unasignChild(long catgoryID, long parentID);

        ICollection<ProductCategory> getParents(long categoryID);

        ICollection<ProductCategory> getChildren(long categoryID);

        Nodo<TreeItemViewModel> getParentHierarchy(long categoryID);

        Nodo<TreeItemViewModel> getChildrenHierarchy(long categoryID);
    }
}