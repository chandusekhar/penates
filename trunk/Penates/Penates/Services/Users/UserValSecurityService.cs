using Penates.Database;
using Penates.Interfaces.Repositories;
using Penates.Interfaces.Services;
using Penates.Models.ViewModels.Users;
using Penates.Repositories.Users;
using Penates.Utils;
using Penates.Utils.JSON;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Services.Users {
    public class UserValSecurityService : IUserValSecurityService {

        IUserValSecurityRepository repository = new UserValSecurityRepository();

        public Status Save(ValSecurityViewModel model) {
            if (!model.HasMaxOrderTotal) {
                model.MaxOrderTotal = null;
            }
            if (!model.HasMinOrderTotal) {
                model.MinOrderTotal = null;
            }
            if (!model.HasMaxProductPrice) {
                model.MaxProductPrice = null;
            }
            if (!model.HasMinProductPrice) {
                model.MinProductPrice = null;
            }
            return this.repository.Save(model);
        }

        public Status Delete(string fileNumber) {
            return this.repository.Delete(fileNumber);
        }

        /// <summary>Obtiene los datos de un Hall</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>HallViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public ValSecurityViewModel getSecurityData(string userID) {
            ProductsValueSecurity prod = this.repository.getProductValueData(userID);
            OrdersValueSecurity order = this.repository.getOrderValueData(userID);
            if (prod == null && order == null) {
                return null;
            }
            ValSecurityViewModel model = this.getHallViewModel(prod, order);
            List<SelectItem> categories = new List<SelectItem>();
            SelectItem item;
            model.Categories = null;
            foreach (ProductCategory category in prod.ProductCategories) {
                if (!String.IsNullOrEmpty(model.Categories)) {
                    model.Categories = model.Categories + ",";
                }
                model.Categories = model.Categories + category.ProductCategoriesID;
                item = new SelectItem {
                    id = category.ProductCategoriesID,
                    label = category.Description
                };
                categories.Add(item);
            }
            model.initialCategories = categories;
            return model;
        }

        /// <summary>Obtiene los datos de un Sector, pero con los nombres de las categorias en lugar de los IDs</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public ValSecurityViewModel getSecuritySummary(string userID) {
            ProductsValueSecurity prod = this.repository.getProductValueData(userID);
            OrdersValueSecurity order = this.repository.getOrderValueData(userID);
            if (prod == null && order == null) {
                return null;
            }
            ValSecurityViewModel model = this.getHallViewModel(prod, order);
            foreach (ProductCategory category in prod.ProductCategories) {
                if (model.Categories != null) { //si no es la primera vez
                    model.Categories = model.Categories + ", ";
                }
                model.Categories = model.Categories + category.Description;
            }
            return model;
        }

        public IQueryable<ValSecurityTableJson> getData() {
            return this.repository.getData();
        }

        public IQueryable<ValSecurityTableJson> search(IQueryable<ValSecurityTableJson> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<ValSecurityTableJson> sort(IQueryable<ValSecurityTableJson> query, int index, string direction) {
            Sorts sort = this.toSort(index, direction);
            return this.sort(query, sort);
        }

        public IQueryable<ValSecurityTableJson> sort(IQueryable<ValSecurityTableJson> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        private Sorts toSort(int sortColumnIndex, string sortDirection) {
            switch (sortColumnIndex) {
                case 0:
                    if (sortDirection == "asc") {
                        return Sorts.ID;
                    } else {
                        return Sorts.ID_DESC;
                    }
                case 1:
                    if (sortDirection == "asc") {
                        return Sorts.USERNAME;
                    } else {
                        return Sorts.USERNAME_DESC;
                    }
                case 2:
                    if (sortDirection == "asc") {
                        return Sorts.MAX_PRICE;
                    } else {
                        return Sorts.MAX_PRICE_DESC;
                    }
                case 3:
                    if (sortDirection == "asc") {
                        return Sorts.MIN_PRICE;
                    } else {
                        return Sorts.MIN_PRICE_DESC;
                    }
                case 4:
                    if (sortDirection == "asc") {
                        return Sorts.MAX_TOTAL;
                    } else {
                        return Sorts.MAX_TOTAL_DESC;
                    }
                case 5:
                    if (sortDirection == "asc") {
                        return Sorts.MIN_TOTAL;
                    } else {
                        return Sorts.MIN_TOTAL_DESC;
                    }
                default:
                    return Sorts.ID;
            }
        }

        /// <summary>Obtiene los datos de un Deposito Temporal</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        private ValSecurityViewModel getHallViewModel(ProductsValueSecurity prod, OrdersValueSecurity order) {
            ValSecurityViewModel model = new ValSecurityViewModel();
            if(order != null){
                model.FileNumber = order.IDUser;
                model.Username = order.User.Username;
                if (order.MaxTotal.HasValue) {
                    model.HasMaxOrderTotal = true;
                }
                model.MaxOrderTotal = order.MaxTotal;
                if (order.MinTotal.HasValue) {
                    model.HasMinOrderTotal = true;
                }
                model.MinOrderTotal = order.MinTotal;
            }
            if (prod != null) {
                model.FileNumber = prod.IDUser;
                model.Username = prod.User.Username;
                if (prod.MaxPrice.HasValue) {
                    model.HasMaxProductPrice = true;
                }
                model.MaxProductPrice = prod.MaxPrice;
                if (prod.MinPrice.HasValue) {
                    model.HasMinProductPrice = true;
                }
                model.MinProductPrice = prod.MinPrice;
            }
            model.Categories = null;
            return model;
        }

        /// <summary>Activa varios Usuarios</summary>
        /// <param name="id">ID a Activar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        public Status Delete(List<string> userIDs) {
            Status status = new Status() {
                Success = true,
                Message = ""
            };
            if (userIDs != null) {
                foreach (string userID in userIDs) {
                    Status stat = this.repository.Delete(userID);
                    if (!stat.Success) {
                        status.Success = false;
                        status.Message = status.Message + "- " + stat.Message + "\n";
                    }
                }
            }
            return status;
        }

        public IQueryable<Product> checkProduct(IQueryable<Product> query, string FileNumber){
            // Obtengo el Security del Producto
            IUserValSecurityRepository repo = new UserValSecurityRepository();
            ProductsValueSecurity sec = repo.getProductValueData(FileNumber);
            if(sec.MinPrice.HasValue){
                query = query.Where(x => x.SellPrice >= sec.MinPrice.Value);
            }
            if(sec.MaxPrice.HasValue){
                query = query.Where(x => x.SellPrice <= sec.MaxPrice.Value);
            }
            //TODO: chequear categorias

            return query;
        }

        public IQueryable<SupplierOrder> filterOrders(IQueryable<SupplierOrder> query, string FileNumber){
            // Obtengo el Security de la Orden
            IUserValSecurityRepository repo = new UserValSecurityRepository();
            OrdersValueSecurity sec = repo.getOrderValueData(FileNumber);
            if(sec.MinTotal.HasValue){
                query = query.Where(x => x.SupplierOrderItems.Sum(y => y.ProvidedBy.PurchasePrice * y.ItemBoxes) >= sec.MinTotal.Value);
            }
            if(sec.MaxTotal.HasValue){
                query = query.Where(x => x.SupplierOrderItems.Sum(y => y.ProvidedBy.PurchasePrice * y.ItemBoxes) <= sec.MaxTotal.Value);
            }
            return query;
        }
    }
}