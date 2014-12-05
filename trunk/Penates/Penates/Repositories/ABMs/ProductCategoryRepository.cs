using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Exceptions.System;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace Penates.Repositories.ABMs {
    public class ProductCategoryRepository : IProductCategoryRepository{

        PenatesEntities db = new PenatesEntities();
        public int itemsPerPage { get; set; }

        public ProductCategoryRepository() {
            this.itemsPerPage = 50;
        }

        public ProductCategoryRepository(int itemsPerPage) {
            this.itemsPerPage = itemsPerPage;
        }

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>true si funciona, sino una excepcion</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Save(ProductCategory cat) {
            try {
                ProductCategory aux = db.ProductCategories.Find(cat.ProductCategoriesID);
                Nullable<long> val = null;
                if (aux == null) {
                    val = db.SP_ProductCategories_Add(cat.Description).SingleOrDefault();
                } else {
                    val = db.SP_ProductCategories_Save(cat.Description, cat.ProductCategoriesID).SingleOrDefault();
                }
                if (!val.HasValue) {
                    return -1;
                }
                return val.Value;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException,cat.Description));
            }
        }

        public bool Delete(long id) {
            //Se supone que restrinjo para que siempre lo encuentre
            ProductCategory pc = db.ProductCategories.Find(id);
            if (pc == null) {
                return false;
            }
            if (checkDeleteConstrains(pc)) {
                var tran = this.db.Database.BeginTransaction();
                try {
                    ProductCategory aux = pc;
                    aux.ChildrenCategories.Clear();
                    aux.ParentCategories.Clear();
                    if (this.physicalDelete(id)) {
                        db.Entry(pc).CurrentValues.SetValues(aux);
                        db.SaveChanges();
                        db.ProductCategories.Remove(aux);
                    } else {
                        aux.Deleted = true;
                        db.Entry(pc).CurrentValues.SetValues(aux);
                    }
                    db.SaveChanges();
                    tran.Commit();
                    return true;
                } catch (Exception e) {
                    tran.Rollback();
                    throw new DatabaseException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.CategoryWArt, id), e.Message);
                }
            } else {
                throw new DeleteConstrainException();
            }
        }

        public ProductCategory getData(long id) {
            try {
                return db.ProductCategories.Find(id);
            } catch (Exception e){
                throw new DatabaseException(e.Message);
            }
        }

        public IEnumerable<ProductCategory> getCategories() {
            try {

                var categories =
                        from p in this.db.ProductCategories
                        where p.Deleted == false
                        orderby p.Description
                        select p;

                return categories;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<ProductCategory> getData() {
            try {

                return this.db.ProductCategories.Where(x => x.Deleted == false);
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<ProductCategory> getAutocomplete(string search) {
            try {
                var data = this.searchAndRank(search);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<ProductCategory> getAutocomplete(string search, long? categoryID) {
            try { //Obtengo todos y dps filtro los que 
                var data = this.searchAndRank(search);
                if (categoryID.HasValue) {
                    ProductCategory cat = this.db.ProductCategories.Find(categoryID.Value);
                    if (cat != null) {
                        //Elimino los que ya son Padres e Hijos, dps en el Add chequeo herencia circular
                        IEnumerable<long> aux = cat.ChildrenCategories.Select(x => x.ProductCategoriesID);
                        data = data.Where(x => !aux.Contains(x.ProductCategoriesID));
                        aux = cat.ParentCategories.Select(x => x.ProductCategoriesID);
                        data = data.Where(x => !aux.Contains(x.ProductCategoriesID));
                    }
                }
                return data;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<ProductCategory> searchAndRank(string search) {
            var data = this.getData();
            return this.searchAndRank(data, search);
        }

        public IQueryable<ProductCategory> searchAndRank(IQueryable<ProductCategory> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.Description).Skip(0).Take(Properties.Settings.Default.autocompleteItems);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<ProductCategory> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<ProductCategory> {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double) x.table.ProductCategoriesID).Trim() == item ? 1000 : 0) +
                                ((x.table.Description.Contains(item)) ? item.Length : 0) + ((x.table.Description.StartsWith(item)) ? (item.Length * 2) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.Description.Length)
                    .ThenBy(x => x.table.Description)
                    .Select(x => x.table);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IEnumerable<ProductCategory> searchAndRank(IEnumerable<ProductCategory> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.Description).Skip(0).Take(Properties.Settings.Default.autocompleteItems);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<ProductCategory> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<ProductCategory> {
                        table = x.table,
                        rankPoints = x.rankPoints + (x.table.ProductCategoriesID.ToString().Trim() == item ? 1000 : 0) +
                                ((x.table.Description.Contains(item)) ? item.Length : 0) + ((x.table.Description.StartsWith(item)) ? (item.Length * 2) : 0) +
                                (String.Compare(x.table.Description, item, StringComparison.OrdinalIgnoreCase))
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.Description.Length)
                    .ThenBy(x => x.table.Description)
                    .Select(x => x.table);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<ProductCategory> search(IQueryable<ProductCategory> query, string search) {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<ProductCategory> search(IQueryable<ProductCategory> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Description.Contains(item) || (SqlFunctions.StringConvert((double) p.ProductCategoriesID).Trim() == item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<ProductCategory> sort(IQueryable<ProductCategory> query, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.DESCRIPTION:
                    query = query.OrderBy(p => p.Description);
                    break;
                case Sorts.DESCRIPTION_DESC:
                    query = query.OrderByDescending(p => p.Description);
                    break;
                case Sorts.ID:
                    query = query.OrderBy(p => p.ProductCategoriesID);
                    break;
                case Sorts.ID_DESC:
                    query = query.OrderByDescending(p => p.ProductCategoriesID);
                    break;
                default:
                    query = query.OrderBy(p => p.ProductCategoriesID);
                    break;
            }
            return query;
        }

        public long getProductConstraintsNumber(long categoryID) {
            ProductCategory pc = this.db.ProductCategories.Find(categoryID);
            if (pc.Products == null) {
                return 0;
            }
            return pc.Products.Where(x => x.Deleted == false).Count();
        }

        public List<Constraint> getProductConstraints(long categoryID) {
            try {
                ProductCategory sp = this.db.ProductCategories.Find(categoryID);
                var result = sp.Products.Where(p => p.Deleted == false).Take(this.itemsPerPage).ToList();
                var items = result.Select(x => new Constraint {
                    id = x.ProductID.ToString(),
                    name = Resources.Resources.Product + ": " + x.Name
                });
                return items.ToList();
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.CategoryWArt, categoryID), e.Message);
            }
        }

        /// <summary>Agrega una categoria padre a una categoria</summary>
        /// <param name="categoryID">Categoria a la que se le va a agregar</param>
        /// <param name="parentID">Categoria Padre</param>
        /// <returns>True si la logra agregar, false si ya esta agregada</returns>
        /// <exception cref="IDNotFoundException"
        /// <exception cref="DatabaseException"
        /// <exception cref="HierarchyException"
        public bool addChild(long categoryID, long parentID) {
            try {
                ProductCategory pc = this.db.ProductCategories.Find(categoryID);
                ProductCategory parent = this.db.ProductCategories.Find(parentID);
                if (pc == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.CategoryParent, 
                        Resources.Resources.Category);
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.CategoryWArt, categoryID);
                    throw new IDNotFoundException(title, message);
                }
                if (parent == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.CategoryParent,
                        Resources.Resources.Category);
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.CategoryParentWArt, parentID);
                    throw new IDNotFoundException(title, message);
                }
                if (!this.checkParentAddDependency(pc, parent)) {
                    throw new HierarchyException(String.Format(Resources.ExceptionMessages.HierarchyExceptionTitle, Resources.Errors.ChildAdd),
                        String.Format(Resources.ExceptionMessages.HierarchyParentExceptionMessage, parent.Description, pc.Description));
                }
                if (!this.checkChildAddDependency(pc, parent)) {
                    throw new HierarchyException(String.Format(Resources.ExceptionMessages.HierarchyExceptionTitle, Resources.Errors.ChildAdd),
                        String.Format(Resources.ExceptionMessages.HierarchyParentExceptionCircularMessage, parent.Description, pc.Description));
                }
                pc.ChildrenCategories.Add(parent);
                this.db.SaveChanges();
                return true;
            } catch (HierarchyException he) {
                throw he;
            }catch (IDNotFoundException infe){
                throw infe;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary>Agrega una categoria hija a una categoria</summary>
        /// <param name="categoryID">Categoria a la que se le va a agregar</param>
        /// <param name="parentID">Categoria Padre</param>
        /// <returns>True si la logra agregar, false si ya esta agregada</returns>
        /// <exception cref="IDNotFoundException"
        /// <exception cref="DatabaseException"
        /// <exception cref="HierarchyException"
        public bool addParent(long categoryID, long childID) {
            try {
                ProductCategory pc = this.db.ProductCategories.Find(categoryID);
                ProductCategory child = this.db.ProductCategories.Find(childID);
                if (pc == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.CategoryChild,
                        Resources.Resources.Category);
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.CategoryWArt, categoryID);
                    throw new IDNotFoundException(title, message);
                }
                if (child == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.CategoryChild,
                        Resources.Resources.Category);
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.CategoryChildWArt, childID);
                    throw new IDNotFoundException(title, message);
                }
                if (!this.checkChildAddDependency(pc, child)) {
                    throw new HierarchyException(String.Format(Resources.ExceptionMessages.HierarchyExceptionTitle, Resources.Errors.ChildAdd),
                        String.Format(Resources.ExceptionMessages.HierarchyChildExceptionMessage, child.Description, pc.Description));
                }
                if (!this.checkParentAddDependency(pc, child)) {
                    throw new HierarchyException(String.Format(Resources.ExceptionMessages.HierarchyExceptionTitle, Resources.Errors.ChildAdd),
                        String.Format(Resources.ExceptionMessages.HierarchyChildExceptionCircularMessage, child.Description, pc.Description));
                }
                pc.ParentCategories.Add(child);
                this.db.SaveChanges();
                return true;
            } catch (HierarchyException he){
                throw he;
            } catch (IDNotFoundException infe) {
                throw infe;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public bool unasignChild(long catgoryID, long parentID) {
            try {
                ProductCategory pc = this.db.ProductCategories.Find(catgoryID);
                ProductCategory parent = this.db.ProductCategories.Find(parentID);
                if (pc == null) {
                    string title = String.Format(Resources.ExceptionMessages.UnassignException, Resources.Resources.CategoryParent,
                        Resources.Resources.Category);
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.Category, catgoryID);
                    throw new IDNotFoundException(title, message);
                }
                if (parent == null) {
                    string title = String.Format(Resources.ExceptionMessages.UnassignException, Resources.Resources.CategoryParent,
                        Resources.Resources.Category);
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.CategoryParent, parentID);
                    throw new IDNotFoundException(title, message);
                }
                if (pc.ChildrenCategories.Contains(parent)) {
                    bool resultado = pc.ChildrenCategories.Remove(parent);
                    this.db.SaveChanges();
                    return resultado;
                } else {
                    return false;
                }
            } catch (IDNotFoundException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Supplier,
                        Resources.Resources.Product));
            }
        }

        public bool unasignParent(long catgoryID, long childID) {
            try {
                ProductCategory pc = this.db.ProductCategories.Find(catgoryID);
                ProductCategory child = this.db.ProductCategories.Find(childID);
                if (pc == null) {
                    string title = String.Format(Resources.ExceptionMessages.UnassignException, Resources.Resources.CategoryChild,
                        Resources.Resources.Category);
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.Category, catgoryID);
                    throw new IDNotFoundException(title, message);
                }
                if (child == null) {
                    string title = String.Format(Resources.ExceptionMessages.UnassignException, Resources.Resources.CategoryChild,
                        Resources.Resources.Category);
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.CategoryChild, childID);
                    throw new IDNotFoundException(title, message);
                }
                if (pc.ParentCategories.Contains(child)) {
                    bool resultado = pc.ParentCategories.Remove(child);
                    this.db.SaveChanges();
                    return resultado;
                } else {
                    return false;
                }
            } catch (IDNotFoundException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Supplier,
                        Resources.Resources.Product));
            }
        }

        public ICollection<ProductCategory> getAssignableCategories(ICollection<ProductCategory> list) {
            try {
                List<ProductCategory> result = new List<ProductCategory>();
                foreach(ProductCategory category in list){
                    result.Add(category);
                    if (category.ChildrenCategories != null && category.ChildrenCategories.Count != 0) {
                        ICollection<ProductCategory> aux = this.getAssignableCategories(category.ChildrenCategories);
                        foreach (ProductCategory cat2 in aux) {
                            if (!result.Contains(cat2)) {
                                result.Add(cat2);
                            }
                        }
                    }
                }
                return result;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public ICollection<ProductCategory> getChildren(long categoryID) {
            ProductCategory pc = this.getData(categoryID);
            try {
                return pc.ChildrenCategories;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public ICollection<ProductCategory> getParents(long categoryID) {
            ProductCategory pc = this.getData(categoryID);
            try {
                return pc.ParentCategories;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary>Obtiene el Arbol con la descripciones de los Padres. Toma como nodo inicial la Categoria buscada</summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public Nodo<TreeItemViewModel> getParentHierarchy(long categoryID) {
            try {
                ProductCategory pc = this.db.ProductCategories.Find(categoryID);
                if (pc == null) {
                    string title = String.Format(Resources.Errors.SearchIDError, Resources.Resources.CategoryWArt);
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.Category, categoryID);
                    throw new IDNotFoundException(title, message);
                }
                return this.getParentHierarchy(pc);
            } catch (IDNotFoundException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Supplier,
                        Resources.Resources.Product));
            }
        }

        public Nodo<TreeItemViewModel> getChildrenHierarchy(long categoryID) {
            try {
                ProductCategory pc = this.db.ProductCategories.Find(categoryID);
                if (pc == null) {
                    string title = String.Format(Resources.Errors.SearchIDError, Resources.Resources.CategoryWArt);
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.Category, categoryID);
                    throw new IDNotFoundException(title, message);
                }
                return this.getChildrenHierarchy(pc);
            } catch (IDNotFoundException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Supplier,
                        Resources.Resources.Product));
            }
        }

        private bool physicalDelete(long categoryID) {
            ProductCategory pc = this.db.ProductCategories.Find(categoryID);
            if (pc.Products == null) {
                return true;
            }
            if (pc.Products.Count == 0) {
                return true;
            }
            return false;
        }

        private Nodo<TreeItemViewModel> getChildrenHierarchy(ProductCategory item) {
            Nodo<TreeItemViewModel> arbol = new Nodo<TreeItemViewModel>();
            if (item.ChildrenCategories != null && item.ChildrenCategories.Count > 0) {
                Nodo<TreeItemViewModel> aux;
                foreach (ProductCategory pc in item.ChildrenCategories) {
                    aux = getChildrenHierarchy(pc);
                    arbol.AddChild(aux);
                }
            }
            TreeItemViewModel model = new TreeItemViewModel();
            model.Label = item.Description;
            model.ID = item.ProductCategoriesID;
            arbol.Add(model);
            return arbol;
        }

        private Nodo<TreeItemViewModel> getParentHierarchy(ProductCategory item) {
            Nodo<TreeItemViewModel> arbol = new Nodo<TreeItemViewModel>();
            if (item.ParentCategories != null && item.ParentCategories.Count > 0) {
                Nodo<TreeItemViewModel> aux;
                foreach (ProductCategory pc in item.ParentCategories) {
                    aux = getParentHierarchy(pc);
                    arbol.AddChild(aux);
                }
            }
            TreeItemViewModel model = new TreeItemViewModel();
            model.Label = item.Description;
            model.ID = item.ProductCategoriesID;
            arbol.Add(model);
            return arbol;
        }

        /// <summary> Checkea si puede eliminar el proveedor con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> Id del producto a eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        private bool checkDeleteConstrains(long id) {
            ProductCategory sp = db.ProductCategories.Find(id);
            return this.checkDeleteConstrains(sp);
        }

        /// <summary> Checkea si puede eliminar el proveedor con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> El Supplier a Eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        private bool checkDeleteConstrains(ProductCategory sp) {
            if (sp.Products == null) {
                return true;
            } else {
                if (sp.Products.Where(x => x.Deleted == false).Count() == 0) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Chequea si ya existe como un padre en el Arbol de Herencias</summary>
        /// <param name="pc">ProductCategory para el cual chequear</param>
        /// <param name="parent">Padre que se va a agregar</param>
        /// <returns>true si se puede agregar, false si no se puede</returns>
        private bool checkParentAddDependency(ProductCategory pc, ProductCategory parent) {
            if (pc.ParentCategories == null || pc.ParentCategories.Count == 0) {
                return true;
            }
            if (pc.ParentCategories.Contains(parent)) {
                return false;
            }
            bool correct = true;
            IEnumerator<ProductCategory> i = pc.ParentCategories.GetEnumerator();
            while(i.MoveNext() && correct){
                correct = checkParentAddDependency(i.Current, parent);
            }
            return correct;
        }

        /// <summary>Chequea si ya existe como un hijo en el Arbol de Herencias</summary>
        /// <param name="pc">ProductCategory para el cual chequear</param>
        /// <param name="child">Hijo que se va a agregar</param>
        /// <returns>true si se puede agregar, false si no se puede</returns>
        private bool checkChildAddDependency(ProductCategory pc, ProductCategory child) {
            if (pc.ChildrenCategories == null || pc.ChildrenCategories.Count == 0) {
                return true;
            }
            if (pc.ChildrenCategories.Contains(child)) {
                return false;
            }
            bool correct = true;
            IEnumerator<ProductCategory> i = pc.ChildrenCategories.GetEnumerator();
            while (i.MoveNext() && correct) {
                correct = checkChildAddDependency(i.Current, child);
            }
            return correct;
        }
    }
}