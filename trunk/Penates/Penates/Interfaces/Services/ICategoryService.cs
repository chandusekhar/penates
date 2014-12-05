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

namespace Penates.Interfaces.Services {
    interface ICategoryService {

        /// <summary> Guarda los datos de una producto en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como ProductViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        long Save(CategoryViewModel cat);

        bool Delete(long? id);

        /// <summary> Retorna el Nombre de la Categoria o null si no se encuentra esa categoria </summary>
        /// <param name="id">Id de la categoria</param>
        /// <returns>Nombre de la Categoria</returns>
        string getCategoryName(long id);

        /// <summary> Retorna el View Model de la Categoria Listo para mostrar en el Formulario</summary>
        /// <param name="id">Id de la categoria</param>
        /// <returns>View Model de la categoria</returns>
        CategoryViewModel getCategoryData(long id);

        /// <summary> Obtiene la lista de Categorias de Producto para mostrar en el formulario </summary>
        /// <returns>SelectList - Lista de Categorias de Producto</returns>
        /// <exception cref="DatabaseException"
        SelectList getCategoriesList();

        /// <summary> Obtiene la lista de Categorias de Producto para mostrar en el formulario </summary>
        /// <returns>SelectList - Lista de Categorias de Producto</returns>
        /// <exception cref="DatabaseException"
        SelectList getCategoriesList(bool includeAll);

        /// <summary> Obtiene la lista de categorias y elije la categoria pasada por parametro </summary>
        /// <param name="categoryId"> ID de la categoria a elejir </param>
        /// <returns>Selectlist para mostrar en el DropDownList</returns>
        /// <exception cref="DatabaseException"
        SelectList getCategoriesList(long categoryId);

        IQueryable<ProductCategory> getData();

        IQueryable<ProductCategory> getAutocomplete(string search);

        IQueryable<ProductCategory> getAutocomplete(string search, long? categoryID);

        IQueryable<ProductCategory> searchAndRank(string search);

        IQueryable<ProductCategory> searchAndRank(IQueryable<ProductCategory> data, string search);

        IQueryable<ProductCategory> search(IQueryable<ProductCategory> query, string search);

        IQueryable<ProductCategory> search(IQueryable<ProductCategory> query, List<string> search);

        IQueryable<ProductCategory> sort(IQueryable<ProductCategory> query, int index, string direction);

        IQueryable<ProductCategory> sort(IQueryable<ProductCategory> query, Sorts sort);

        List<ConstraintViewModel> getConstrains(long categoryID);

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

        ICollection<ProductCategory> getParents(long? categoryID);

        ICollection<ProductCategory> getChildren(long? categoryID);

        Nodo<TreeItemViewModel> getParentHierarchy(long categoryID);

        Nodo<TreeItemViewModel> getChildrenHierarchy(long categoryID);

        Sorts toSort(int sortColumnIndex, string sortDirection);

        List<List<string>> toJsonArray(IQueryable<ProductCategory> query);

        List<List<string>> toJsonArray(ICollection<ProductCategory> list);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<ProductCategory> query);

        List<SelectItem> getCategoriesSelect(string categoriesID);
    
        void getAllChildren(long RootCategoryTree, ICollection<ProductCategory> ChildrenList);
    }
}
