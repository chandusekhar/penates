using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.Users;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace Penates.Repositories.Users {
    public class UserValSecurityRepository : IUserValSecurityRepository {

        PenatesEntities db = new PenatesEntities();

        public Status Save(ValSecurityViewModel model) {
            var tran = this.db.Database.BeginTransaction();
            try {
                User user = this.db.Users.Find(model.FileNumber); //Me fijo que el Usuario exista
                if (user == null) {
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.User, model.FileNumber);
                    throw new ForeignKeyConstraintException(message);
                }
                OrdersValueSecurity orderSecurity = this.db.OrdersValueSecurities.Find(model.FileNumber);
                if (orderSecurity == null) {
                    orderSecurity = new OrdersValueSecurity() {
                        IDUser = model.FileNumber,
                        MaxTotal = model.MaxOrderTotal,
                        MinTotal = model.MinOrderTotal
                    };
                    this.db.OrdersValueSecurities.Add(orderSecurity);
                } else {
                    orderSecurity.MaxTotal = model.MaxOrderTotal;
                    orderSecurity.MinTotal = model.MinOrderTotal;
                }
                ProductsValueSecurity sec = this.db.ProductsValueSecurities.Find(model.FileNumber);
                if (sec == null) {
                    sec = new ProductsValueSecurity() {
                        IDUser = model.FileNumber,
                        MaxPrice = model.MaxProductPrice,
                        MinPrice = model.MinProductPrice
                    };
                    this.db.ProductsValueSecurities.Add(sec);
                    bool result = this.addCategory(sec, model.Categories);
                    if (!result) {
                        tran.Rollback();
                        return new Status() {
                            Success = false,
                            Message = String.Format(Resources.ExceptionMessages.AddException,
                                Resources.Resources.CategoryWArt, Resources.Resources.UserWArt)
                        };
                    }
                } else {
                    sec.MaxPrice = model.MaxProductPrice;
                    sec.MinPrice = model.MinProductPrice;
                    Status result = this.saveCategories(sec, model.Categories);
                    if (!result.Success) {
                        tran.Rollback();
                        return result;
                    }
                }
                this.db.SaveChanges();
                tran.Commit();
                return new Status() { Success = true};
            } catch (Exception e) {
                tran.Rollback();
                return new Status() { Success = false, Message = e.Message};
            }
        }

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        private bool addCategory(ProductsValueSecurity userVal, string categoryiesIDs) {
            if (String.IsNullOrWhiteSpace(categoryiesIDs)) {
                return true;
            }
            categoryiesIDs = categoryiesIDs.Trim();
            List<string> categories = StringUtils.split(categoryiesIDs, ',');
            List<long> ids = new List<long>();
            var success = true;
            foreach (string s in categories) {
                if (success) {
                    success = this.addCategory(userVal, long.Parse(s));
                } else {
                    this.addCategory(userVal, long.Parse(s));
                }
            }
            return success;
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        private bool addCategory(ProductsValueSecurity userVal, long categoryID) {
            try {
                ProductCategory category = this.db.ProductCategories.Find(categoryID);
                if (category == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.User);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Category, categoryID);
                    throw new ForeignKeyConstraintException(title, message);
                }

                if (!userVal.ProductCategories.Contains(category)) {
                    userVal.ProductCategories.Add(category);
                }
                return true;
            } catch (ForeignKeyConstraintException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Hall));
            }
        }

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        private Status saveCategories(ProductsValueSecurity userVal, string categoryiesIDs) {
            if (String.IsNullOrWhiteSpace(categoryiesIDs)) {
                userVal.ProductCategories.Clear();
                return new Status() { Success = true};
            }
            categoryiesIDs = categoryiesIDs.Trim();
            List<string> categories = StringUtils.split(categoryiesIDs, ',');
            List<long> ids = new List<long>();
            foreach (string s in categories) {
                ids.Add(long.Parse(s));
            }
            return this.saveCategories(userVal, ids);
        }

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        private Status saveCategories(ProductsValueSecurity userVal, List<long> categoryiesIDs) {
            try {
                IEnumerable<ProductCategory> aux = userVal.ProductCategories.ToList();
                foreach (ProductCategory category in aux) {
                    if (!categoryiesIDs.Contains(category.ProductCategoriesID)) {
                        userVal.ProductCategories.Remove(category);
                    }
                }
                string error = null;
                foreach (long id in categoryiesIDs) {
                    if (!userVal.ProductCategories.Any(x => x.ProductCategoriesID == id)) {
                        ProductCategory category = this.db.ProductCategories.Find(id);
                        if (category == null) {
                            error = error + String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                                Resources.Resources.Category, id) + "\n";
                        }
                        userVal.ProductCategories.Add(category);
                    }
                }
                if (!String.IsNullOrWhiteSpace(error)) {
                    return new Status() {
                        Success = false,
                        Message = error
                    };
                }
                return new Status() {Success = true};
            } catch (ForeignKeyConstraintException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.User));
            }
        }


        public Status Delete(string fileNumber) {
            var tran = this.db.Database.BeginTransaction();
            try {
                OrdersValueSecurity orderSecurity = this.db.OrdersValueSecurities.Find(fileNumber);
                if (orderSecurity != null) {
                    this.db.OrdersValueSecurities.Remove(orderSecurity);
                }
                ProductsValueSecurity sec = this.db.ProductsValueSecurities.Find(fileNumber);
                if (sec != null) {
                    this.db.ProductsValueSecurities.Remove(sec);
                }
                this.db.SaveChanges();
                tran.Commit();
                return new Status() { Success = true};
            } catch (Exception e) {
                tran.Rollback();
                return new Status() {Success = true, Message = e.Message};
            }
        }

        public OrdersValueSecurity getOrderValueData(string UserID) {
            try {
                return this.db.OrdersValueSecurities.Find(UserID);
            } catch (Exception) {
                return null;
            }
        }

        public ProductsValueSecurity getProductValueData(string UserID) {
            try {
                return this.db.ProductsValueSecurities.Find(UserID);
            } catch (Exception) {
                return null;
            }
        }

        public IQueryable<ValSecurityTableJson> getData() {
            try {
                return (from prod in this.db.ProductsValueSecurities
                        join order in this.db.OrdersValueSecurities on prod.IDUser equals order.IDUser
                        select new ValSecurityTableJson {
                            FileNumber = prod==null ? order.IDUser : prod.IDUser,
                            Username = prod==null ? order.User.Username : prod.User.Username,
                            MaxOrderTotal = order.MaxTotal,
                            MinOrderTotal = order.MinTotal,
                            MaxProductPrice = prod.MaxPrice,
                            MinProductPrice = prod.MinPrice
                        });
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<ValSecurityTableJson> search(IQueryable<ValSecurityTableJson> query, string search) {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<ValSecurityTableJson> search(IQueryable<ValSecurityTableJson> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.FileNumber.Contains(item) || p.Username.Contains(item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<ValSecurityTableJson> sort(IQueryable<ValSecurityTableJson> dc, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ID:
                    dc = dc.OrderBy(x => x.FileNumber);
                    break;
                case Sorts.ID_DESC:
                    dc = dc.OrderByDescending(x => x.FileNumber);
                    break;
                case Sorts.USERNAME:
                    dc = dc.OrderBy(x => x.Username);
                    break;
                case Sorts.USERNAME_DESC:
                    dc = dc.OrderByDescending(x => x.Username);
                    break;
                case Sorts.MAX_PRICE:
                    dc = dc.OrderBy(x => x.MaxProductPrice).ThenBy(x => x.FileNumber);
                    break;
                case Sorts.MAX_PRICE_DESC:
                    dc = dc.OrderByDescending(x => x.MaxProductPrice).ThenByDescending(x => x.FileNumber);
                    break;
                case Sorts.MIN_PRICE:
                    dc = dc.OrderBy(x => x.MinProductPrice).ThenBy(x => x.FileNumber);
                    break;
                case Sorts.MIN_PRICE_DESC:
                    dc = dc.OrderByDescending(x => x.MinProductPrice).ThenByDescending(x => x.FileNumber);
                    break;
                case Sorts.MAX_TOTAL:
                    dc = dc.OrderBy(x => x.MaxOrderTotal).ThenBy(x => x.FileNumber);
                    break;
                case Sorts.MAX_TOTAL_DESC:
                    dc = dc.OrderByDescending(x => x.MaxOrderTotal).ThenByDescending(x => x.FileNumber);
                    break;
                case Sorts.MIN_TOTAL:
                    dc = dc.OrderBy(x => x.MinOrderTotal).ThenBy(x => x.FileNumber);
                    break;
                case Sorts.MIN_TOTAL_DESC:
                    dc = dc.OrderByDescending(x => x.MinOrderTotal).ThenByDescending(x => x.FileNumber);
                    break;
                case Sorts.DEFAULT:
                    dc = dc.OrderBy(x => x.FileNumber);
                    break;
                default:
                    dc = dc.OrderBy(x => x.FileNumber);
                    break;
            }
            return dc;
        }
    }
}