using Penates.Database;
using Penates.Exceptions;
using Penates.Exceptions.Database;
using Penates.Exceptions.Views;
using Penates.Interfaces.Repositories;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.Forms;
using Penates.Repositories.ABMs;
using Penates.Utils;
using Penates.Utils.JSON;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Services.ABMs {
    public class CategoryService : ICategoryService {

        ProductCategoryRepository repository = new ProductCategoryRepository();

        /// <summary> Guarda los datos de una producto en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como ProductViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        public long Save(CategoryViewModel cat) {
            ProductCategory aux;
            if(cat.ProductCategoriesID>0){ //Si es que ya tengo el ID para asignar
                aux = new ProductCategory {ProductCategoriesID=cat.ProductCategoriesID, Description = cat.Description };
            }else{
                aux = new ProductCategory { Description = cat.Description };
            }
            return repository.Save(aux);
        }

        public bool Delete(long? id) {
            if (id.HasValue) {
                return this.repository.Delete(id.Value);
            } else {
                throw new NullIDException(String.Format(Resources.ExceptionMessages.DeleteExceptionNoID, Resources.Resources.CategoryWArt),
                    String.Format(Resources.Errors.DeleteIDNull, Resources.Resources.CategoryWArt));
            }
        }

        /// <summary> Retorna el Nombre de la Categoria o null si no se encuentra esa categoria </summary>
        /// <param name="id">Id de la categoria</param>
        /// <returns>Nombre de la Categoria</returns>
        public string getCategoryName(long id) {
            CategoryViewModel category = this.getCategoryData(id);
            if (category == null) {
                return null;
            }
            return category.Description;
        }

        /// <summary> Retorna el View Model de la Categoria Listo para mostrar en el Formulario</summary>
        /// <param name="id">Id de la categoria</param>
        /// <returns>View Model de la categoria</returns>
        public CategoryViewModel getCategoryData(long id) {
            ProductCategory cat = repository.getData(id);

            if (cat == null) {
                return null;
            }
            CategoryViewModel aux = new CategoryViewModel();
            aux.ProductCategoriesID = cat.ProductCategoriesID;
            aux.Description = cat.Description;

            return aux;
        }

        /// <summary> Obtiene la lista de Categorias de Producto para mostrar en el formulario </summary>
        /// <returns>SelectList - Lista de Categorias de Producto</returns>
        /// <exception cref="DatabaseException"
        public SelectList getCategoriesList() {
            return this.getCategoriesList(false);
        }

        /// <summary> Obtiene la lista de Categorias de Producto para mostrar en el formulario </summary>
        /// <returns>SelectList - Lista de Categorias de Producto</returns>
        /// <exception cref="DatabaseException"
        public SelectList getCategoriesList(bool includeAll) {
            var categories = this.getCategories();
            if(includeAll){
                categories.Insert(0, new ProductCategory { ProductCategoriesID = -1, Description = Resources.Resources.All });
                return new SelectList(categories, "ProductCategoriesID", "Description", -1);
            }
            return new SelectList(categories, "ProductCategoriesID", "Description");
        }

        /// <summary> Obtiene la lista de categorias y elije la categoria pasada por parametro </summary>
        /// <param name="categoryId"> ID de la categoria a elejir </param>
        /// <returns>Selectlist para mostrar en el DropDownList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getCategoriesList(long categoryId) {
            var list = this.getCategories();
            return new SelectList(list, "ProductCategoriesID", "Description", categoryId);
        }

        /// <summary> Obtiene las categorias existentes para mostrar /// </summary>
        /// <returns>SelectList: Lista de las categorias</returns>
        /// <exception cref="DatabaseException"
        private List<ProductCategory> getCategories() {
            try {
                return this.repository.getCategories().ToList();
            }catch(Exception e){
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<ProductCategory> getData() {
            return this.repository.getData();
        }

        public IQueryable<ProductCategory> getAutocomplete(string search) {
            return this.repository.getAutocomplete(search);
        }

        public IQueryable<ProductCategory> getAutocomplete(string search, long? categoryID){
            return this.repository.getAutocomplete(search, categoryID);
        }

        public IQueryable<ProductCategory> searchAndRank(string search) {
            return this.repository.searchAndRank(search);
        }

        public IQueryable<ProductCategory> searchAndRank(IQueryable<ProductCategory> data, string search) {
            return this.repository.searchAndRank(data, search);
        }

        public IQueryable<ProductCategory> search(IQueryable<ProductCategory> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<ProductCategory> search(IQueryable<ProductCategory> query, List<string> search) {
            return this.repository.search(query, search);
        }

        public IQueryable<ProductCategory> sort(IQueryable<ProductCategory> query, int index, string direction) {
            try {
                Sorts sort = this.toSort(index, direction);
                return this.repository.sort(query, sort);
            } catch (SortException) {
                return query; //Si no hay sort no hago nada
            }
        }

        public IQueryable<ProductCategory> sort(IQueryable<ProductCategory> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        public List<ConstraintViewModel> getConstrains(long categoryID) {
            List<ConstraintViewModel> constraintsList = new List<ConstraintViewModel>();

            IProductCategoryRepository repo = new ProductCategoryRepository(Properties.Settings.Default.nConstrainsToView);
            ConstraintViewModel constraint;
            long constraintsCount = 0;

            constraintsCount = repo.getProductConstraintsNumber(categoryID);
            if (constraintsCount > 0) {
                constraint = new ConstraintViewModel(
                    String.Format(Resources.Constraints.ConstraintTitle, Resources.Resources.Products),
                    String.Format(Resources.Constraints.ConstraintMessage, Resources.Resources.Products, Resources.Resources.Suppliers));
                constraint.Count = constraintsCount;
                constraint.TableWithConstrain = "Products";
                constraint.constraints = repo.getProductConstraints(categoryID);
                constraintsList.Add(constraint);
            }
            return constraintsList;
        }

        /// <summary>Agrega una categoria padre a una categoria</summary>
        /// <param name="categoryID">Categoria a la que se le va a agregar</param>
        /// <param name="parentID">Categoria Padre</param>
        /// <returns>True si la logra agregar, false si ya esta agregada</returns>
        /// <exception cref="IDNotFoundException"
        /// <exception cref="DatabaseException"
        public bool addParent(long categoryID, long parentID) {
            return this.repository.addParent(categoryID, parentID);
        }

        /// <summary>Agrega una categoria hija a una categoria</summary>
        /// <param name="categoryID">Categoria a la que se le va a agregar</param>
        /// <param name="parentID">Categoria Padre</param>
        /// <returns>True si la logra agregar, false si ya esta agregada</returns>
        /// <exception cref="IDNotFoundException"
        /// <exception cref="DatabaseException"
        public bool addChild(long categoryID, long childID) {
            return this.repository.addChild(categoryID, childID);
        }

        public bool unasignParent(long catgoryID, long parentID) {
            return this.repository.unasignParent(catgoryID, parentID);
        }

        public bool unasignChild(long catgoryID, long parentID) {
            return this.repository.unasignChild(catgoryID, parentID);
        }

        public ICollection<ProductCategory> getParents(long? categoryID) {
            if (categoryID.HasValue) {
                return this.repository.getParents(categoryID.Value);
            } else {
                return new List<ProductCategory>();
            }
        }

        public ICollection<ProductCategory> getChildren(long? categoryID) {
            if (categoryID.HasValue) {
                return this.repository.getChildren(categoryID.Value);
            } else {
                return new List<ProductCategory>();
            }
        }

        public Nodo<TreeItemViewModel> getParentHierarchy(long categoryID) {
            return this.repository.getParentHierarchy(categoryID);
        }

        public Nodo<TreeItemViewModel> getChildrenHierarchy(long categoryID) {
            return this.repository.getChildrenHierarchy(categoryID);
        }

        public Sorts toSort(int sortColumnIndex, string sortDirection) {

            switch (sortColumnIndex) {
                case 0:
                    if (sortDirection == "asc") {
                        return Sorts.ID;
                    } else {
                        return Sorts.ID_DESC;
                    }
                case 1:
                    if (sortDirection == "asc") {
                        return Sorts.DESCRIPTION;
                    } else {
                        return Sorts.DESCRIPTION_DESC;
                    }
                default:
                    throw new SortException(Resources.ExceptionMessages.InvalidSortException, "Col: " + sortColumnIndex);
            }
        }

        public List<List<string>> toJsonArray(IQueryable<ProductCategory> query) {
            List<ProductCategory> list = query.ToList();
            List<List<string>> result = new List<List<string>>();
            List<string> aux;
            foreach (ProductCategory pc in list) {
                aux = new List<string>() { pc.ProductCategoriesID.ToString(), pc.Description };
                result.Add(aux);
            }
            return result;
        }

        public List<List<string>> toJsonArray(ICollection<ProductCategory> list) {
            List<List<string>> result = new List<List<string>>();
            List<string> aux;
            foreach (ProductCategory pc in list) {
                aux = new List<string>() { pc.ProductCategoriesID.ToString(), pc.Description };
                result.Add(aux);
            }
            return result;
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<ProductCategory> query) {
            List<ProductCategory> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (ProductCategory pc in list) {
                aux = new AutocompleteItem() { ID = pc.ProductCategoriesID, Label = pc.Description };
                result.Add(aux);
            }
            return result;
        }

        public List<SelectItem> getCategoriesSelect(string categoriesID) {
            List<string> aux = StringUtils.split(categoriesID, ',');
            IProductCategoryRepository repo = new ProductCategoryRepository();
            List<SelectItem> categories = new List<SelectItem>();
            SelectItem item;
            foreach (string s in aux) {
                ProductCategory cat = repo.getData(long.Parse(s));
                item = new SelectItem {
                    id = cat.ProductCategoriesID,
                    label = cat.Description
                };
                categories.Add(item);
            }
            return categories;
        }


      
        public void getAllChildren(long RootCategoryTree, ICollection<ProductCategory> ChildrenList)
        {
            ICollection<ProductCategory> children = this.getChildren(RootCategoryTree);
            if (children.Count != 0)
            {
                ProductCategory root = this.getCategory(RootCategoryTree);
                ChildrenList.Add(root);
                foreach (ProductCategory category in children)
                {
                    ChildrenList.Add(category);
                    getAllChildren(category.ProductCategoriesID, ChildrenList);                    
                }            
            }else{
                ProductCategory root = this.getCategory(RootCategoryTree);
                ChildrenList.Add(root);             
            }
        }


        public ProductCategory getCategory(long CategoryID)
        {
            return this.repository.getData(CategoryID);
        }

        
    }
}