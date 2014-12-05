using System;

namespace Penates.Repositories.Users
{
    using Penates.Interfaces.Repositories;
    using Penates.Models.ViewModels.Users;
    using System.Linq;
    using Penates.Database;
    using System.Collections.Generic;
    using System.Web.Helpers;
    using Penates.Models;
    using Penates.Exceptions.Database;
    using Penates.Utils;
    using Penates.Exceptions.Views;
    using System.Data.Entity.Core;
    using System.Configuration;
    using System.Web.Configuration;
    using Penates.Utils.Objects;
    using Penates.Utils.Enums;

    public class UserRepository : IUserRepository
    {
        PenatesEntities db = new PenatesEntities();

        public Status Register(RegisterViewModel user)
        {
            try
            {
                user.UserName = user.UserName.ToLower();
                var db = new PenatesEntities();
                if (db.Users.Any(x => x.Username.ToLower() == user.UserName.ToLower())) {
                    var ex2 = new UniqueRestrictionException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.User, user.FileNumber), String.Format(Resources.ExceptionMessages.UniqueRestrictionException,
                        Resources.Resources.UsernameWArt, user.UserName));
                    ex2.atributeName = ReflectionExtension.getVarName(() => user.UserName);
                    throw ex2;
                }
                var hashedUser = CryptoUtils.HashPassword(user.FileNumber, user.UserName, user.Password);
                var us = new User { FileNumber = user.FileNumber, Address = user.Address, Email = user.Email, FirstName = user.FirstName,
                LastName = user.LastName, Password = hashedUser.Password, Phone = user.Telephone, Username = user.UserName, Active = true, Salt = hashedUser.Salt,
                LastLoginDate = System.DateTime.Now};

                User aux = db.Users.Find(us.FileNumber);
                if (aux == null) {
                    db.Users.Add(us);
                } else {
                    return new Status { Success = false, Message = String.Format(Resources.Errors.AllreadyExists, Resources.Resources.UserWArt) };
                }
                db.SaveChanges();

                return new Status { Success = true, Message = string.Empty };
            } catch (UniqueRestrictionException e) {
                throw e;
            }
            catch (Exception ex)
            {
                return new Status { Success = false, Message = ex.Message };
            }
        }

        public Status Save(UserViewModel user) {
            try {
                User us = db.Users.Find(user.FileNumber);
                if (us == null) {
                    throw new IDNotFoundException();
                }
                User aux = us;
                aux.Address = user.Address;
                aux.Email = user.Email;
                aux.FirstName = user.FirstName;
                aux.LastName = user.LastName;
                aux.Phone = user.Telephone;
                this.db.Entry(us).CurrentValues.SetValues(aux);
                db.SaveChanges();

                return new Status { Success = true, Message = string.Empty };
            } catch (IDNotFoundException e) {
                throw e;
            } catch (Exception ex) {
                return new Status { Success = false, Message = ex.Message };
            }
        }

        /// <summary> Devuelve true si lo desactiva y false si no puede /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        public bool Deactivate(string userID) {
            User user = this.db.Users.Find(userID);
            if(user == null){
                return false;
            }
            var tran = this.db.Database.BeginTransaction();
            try {
                user.Active = false;
                db.Users.Attach(user);
                var entry = db.Entry(user);
                entry.Property(e => e.Active).IsModified = true;
                this.deleteNotifications(user);
            
                this.db.SaveChanges();
                tran.Commit();
                return true;
            } catch (Exception e) {
                tran.Rollback();
                throw new DeleteException(String.Format(Resources.ExceptionMessages.DeactivateException,
                    Resources.Resources.UserWArt, userID), e.Message);
            }
        }

        /// <summary> Devuelve true si lo activa y false si no puede /// </summary>
        /// <param name="id">ID a activar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        public bool Activate(string userID) {
            User user = this.db.Users.Find(userID);
            if (user == null) {
                return false;
            }
            var tran = this.db.Database.BeginTransaction();
            try {
                user.Active = true;
                user.LastLoginDate = System.DateTime.Now;
                db.Users.Attach(user);
                var entry = db.Entry(user);
                entry.Property(e => e.Active).IsModified = true;
                entry.Property(e => e.LastLoginDate).IsModified = true;

                this.db.SaveChanges();
                tran.Commit();
                return true;
            } catch (Exception e) {
                tran.Rollback();
                throw new DeleteException(String.Format(Resources.ExceptionMessages.ActivateException,
                    Resources.Resources.UserWArt, userID), e.Message);
            }
        }

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        public bool Delete(string userID) {
            User user = this.db.Users.Find(userID);
            if (user == null) {
                return false;
            }
            var tran = this.db.Database.BeginTransaction();
            try {
                this.deleteNotifications(user);
                this.deleteAdministred(user);
                this.deleteRoles(user);
                User u = db.Users.Find(userID);
                this.db.Users.Remove(u);
                db.SaveChanges();
                tran.Commit();
                return true;
            } catch (Exception e) {
                tran.Rollback();
                throw new DeleteException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.UserWArt, userID), e.Message);
            }
        }

        public User getData(string UserID) {
            try {
                return this.db.Users.Find(UserID);
            } catch (Exception) {
                return null;
            }
        }

        public bool Login(string username, string password)
        {
            var db = new PenatesEntities();

            try{
                if(String.Equals(username, ConfigurationManager.AppSettings["SuperUserName"].ToString(), StringComparison.OrdinalIgnoreCase) &&
                    Crypto.VerifyHashedPassword(ConfigurationManager.AppSettings["SuperPassword"].ToString(), password)){
                        return true;
                }
                User user = db.Users.FirstOrDefault(x => x.Username == username && x.Active);
                if (user == null) {
                    return false;
                }
                UserPassword hashedUser = new UserPassword() {
                    FileNumber = user.FileNumber,
                    UserName = user.Username,
                    Password = password,
                    Salt = user.Salt
                };
                return CryptoUtils.VerifyHash(hashedUser, user.Password);
            }
            catch
            {
                return false;
            }
        }
        
        public List<Role> GetRolesForUser(string userID)
        {
            var db = new PenatesEntities();

            return db.Roles.Where(x => x.Users.Any(u => u.FileNumber == userID)).ToList();
        }

        public User GetUserByUserName(string userName)
        {
            try {
                var db = new PenatesEntities();
                var users = new User();

                return (from u in db.Users
                        where u.Username == userName && u.Active == true
                        select u).FirstOrDefault();
            }catch(EntityException e){
                return null;
            }
        }

        public void InactiveUser(string fileNumber)
        {
            try
            {
                User us = db.Users.Find(fileNumber);
                if (us == null)
                {
                    throw new IDNotFoundException();
                }
                us.Active = false;
                db.SaveChanges();
            }
            catch (IDNotFoundException e)
            {
                throw e;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<User> getData() {
            try {
                return this.db.Users;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<User> search(IQueryable<User> query, string search) {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<User> search(IQueryable<User> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Address.Contains(item) || p.Email.Contains(item) || p.FileNumber.Contains(item)
                        || p.FirstName.Contains(item) || p.LastName.Contains(item) || p.Username.Contains(item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<User> filterActive(IQueryable<User> query, bool active) {
            return query.Where(x => x.Active == active);
        }

        public IQueryable<User> filterRole(IQueryable<User> query, long roleID) {
            return query.Where(x => x.Roles.Any(p => p.RoleID == roleID));
        }

        public IQueryable<User> filterByDistributionCenter(IQueryable<User> query, long dcID) {
            return query.Where(x => x.DistributionCenters.Any(p => p.DistributionCenterID == dcID));
        }

        public IQueryable<User> sort(IQueryable<User> dc, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ID:
                    dc = dc.OrderBy(x => x.FileNumber);
                    break;
                case Sorts.ID_DESC:
                    dc = dc.OrderByDescending(x => x.FileNumber);
                    break;
                case Sorts.NAME:
                    dc = dc.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
                    break;
                case Sorts.NAME_DESC:
                    dc = dc.OrderByDescending(x => x.LastName).ThenByDescending(x => x.FirstName);
                    break;
                case Sorts.USERNAME:
                    dc = dc.OrderBy(x => x.Username);
                    break;
                case Sorts.USERNAME_DESC:
                    dc = dc.OrderByDescending(x => x.Username);
                    break;
                case Sorts.EMAIL:
                    dc = dc.OrderBy(x => x.Email);
                    break;
                case Sorts.EMAIL_DESC:
                    dc = dc.OrderByDescending(x => x.Email);
                    break;
                case Sorts.DEFAULT:
                    dc = dc.OrderBy(x => x.FileNumber);
                    break;
                default:
                    throw new SortException(Resources.ExceptionMessages.InvalidSortException, sort.ToString());
            }
            return dc;
        }

        /// <summary>Valida que un usuario exista y este activo</summary>
        /// <param name="userName">Nomvre de usuario</param>
        /// <returns>True si es valido, sino false</returns>
        public bool validateUser(string userName) {
            try {
                if (String.Equals(userName, ConfigurationManager.AppSettings["SuperUserName"].ToString(), StringComparison.OrdinalIgnoreCase)) {
                    return true;
                }
                return this.db.Users.Any(x => x.Username.ToLower() == userName.ToLower() && x.Active);
            } catch (EntityException) {
                return false;
            }
        }

        /// <summary>Change de password of a user</summary>
        /// <param name="userName">Username to change the password</param>
        /// <param name="newPassword">New Password</param>
        /// <returns>true si logra cambiarla, false sino</returns>
        public bool changePassword(string userName, string newPassword) {
            var tran = this.db.Database.BeginTransaction();
            try {
                User user = this.GetUserByUserName(userName);
                if (user == null) {
                    return false;
                }
                var hashedUser = CryptoUtils.HashPassword(user.FileNumber, userName, newPassword);
                user = this.db.Users.Find(user.FileNumber);
                user.Password = hashedUser.Password;
                user.Salt = hashedUser.Salt;
                this.db.SaveChanges();
                tran.Commit();
                return true;
            } catch (Exception e) {
                tran.Rollback();
                throw e;
            }
        }

        /// <summary>Change de password of a user</summary>
        /// <param name="userName">Username to change the password</param>
        /// <param name="password">Current Password</param>
        /// <param name="newPassword">New Password</param>
        /// <returns>true si logra cambiarla, false sino</returns>
        public bool changeMyPassword(string userName, string password, string newPassword) {
            if (String.Equals(userName, ConfigurationManager.AppSettings["SuperUserName"].ToString(), StringComparison.OrdinalIgnoreCase)) {
                if(Crypto.VerifyHashedPassword(ConfigurationManager.AppSettings["SuperPassword"].ToString(), password)){
                    var hashedPass = Crypto.HashPassword(newPassword);
                    var configuration = WebConfigurationManager.OpenWebConfiguration("~");
                    configuration.AppSettings.Settings.Remove("SuperPassword");
                    configuration.AppSettings.Settings.Add("SuperPassword", hashedPass);
                    configuration.Save();
                    return true;
                }
                return false;
            } else {
            var tran = this.db.Database.BeginTransaction();
            try {
                User user = this.GetUserByUserName(userName);
                if (user == null) {
                    return false;
                }
                if (CryptoUtils.VerifyHash(user.FileNumber, user.Username, password, user.Salt, user.Password)) {
                    var hashedUser = CryptoUtils.HashPassword(user.FileNumber, user.Username, newPassword);
                    user = this.db.Users.Find(user.FileNumber);
                    user.Password = hashedUser.Password;
                    user.Salt = hashedUser.Salt;
                    this.db.SaveChanges();
                    tran.Commit();
                    return true;
                }
                return false;
            } catch (Exception e) {
                tran.Rollback();
                throw e;
            }
                }
        }

        public IQueryable<User> getAutocomplete(string search) {
            try {
                var data = this.searchAndRank(search);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<User> searchAndRank(string search) {
            var data = this.getData();
            return this.searchAndRank(data, search);
        }

        public IQueryable<User> searchAndRank(IQueryable<User> data, string search) {
            try {
                if (String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.FileNumber).Skip(0).Take(Properties.Settings.Default.autocompleteItems);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<User> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<User> {
                        table = x.table,
                        rankPoints = x.rankPoints + (x.table.FileNumber.Trim() == item ? 1000 : 0) + (x.table.Username.StartsWith(item) ? (item.Length * 20) : 0) +
                        (x.table.Username.Contains(item) ? (item.Length * 10) : 0) + ((x.table.LastName.Contains(item)) ? item.Length : 0) +
                        ((x.table.LastName.StartsWith(item)) ? (item.Length * 2) : 0) + ((x.table.FirstName.Contains(item)) ? item.Length : 0) +
                        ((x.table.FirstName.StartsWith(item)) ? (item.Length * 2) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.Username.Length)
                    .Select(x => x.table);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public ICollection<Role> getRoles(string FileNumber) {
            User user = this.db.Users.Find(FileNumber);
            return user.Roles;
        }

        public Status AddUserToRole(string user, long role) {
            try {
                var db = new PenatesEntities();
                User us = db.Users.Find(user);
                Role ro = db.Roles.Find(role);
                if (us == null) {
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.UserWArt, user);
                    throw new IDNotFoundException(message);
                }
                if (ro == null) {
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.RoleWArt, ro);
                    throw new IDNotFoundException(message);
                }
                us.Roles.Add(ro);
                db.SaveChanges();

                return new Status { Success = true, Message = string.Empty };
            } catch (Exception ex) {
                return new Status { Success = false, Message = ex.Message };
            }
        }

        public Status RemoveUserFromRole(string UserID, long RoleID) {
            try {
                var db = new PenatesEntities();
                User user = db.Users.Find(UserID);
                Role role = db.Roles.Find(RoleID);
                if (user == null) {
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.UserWArt, user);
                    throw new IDNotFoundException(message);
                }
                if (role == null) {
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.RoleWArt, RoleID);
                    throw new IDNotFoundException(message);
                }
                user.Roles.Remove(role);
                db.SaveChanges();

                return new Status { Success = true, Message = string.Empty };
            } catch (Exception ex) {
                return new Status { Success = false, Message = ex.Message };
            }
        }

        public ICollection<DistributionCenter> getDistributionCenters(string FileNumber) {
            User us = this.db.Users.Find(FileNumber);
            if (us == null) {
                throw new IDNotFoundException(String.Format(Resources.Errors.SearchIDError, Resources.Resources.UserWArt));
            }
            return us.DistributionCenters;
        }

        public Status AttachDistributionCenter(string user, long DistributionCenterID) {
            try {
                var db = new PenatesEntities();
                User us = db.Users.Find(user);
                DistributionCenter dc = db.DistributionCenters.Find(DistributionCenterID);
                if (us == null) {
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.UserWArt, user);
                    throw new IDNotFoundException(message);
                }
                if (dc == null) {
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.DistributionCenterWArt, DistributionCenterID);
                    throw new IDNotFoundException(message);
                }
                us.DistributionCenters.Add(dc);
                db.SaveChanges();

                return new Status { Success = true, Message = string.Empty };
            } catch (Exception ex) {
                return new Status { Success = false, Message = ex.Message };
            }
        }

        public Status DetachDistributionCenter(string UserID, long DistributionCenterID) {
            try {
                var db = new PenatesEntities();
                User user = db.Users.Find(UserID);
                DistributionCenter dc = db.DistributionCenters.Find(DistributionCenterID);
                if (user == null) {
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.UserWArt, user);
                    throw new IDNotFoundException(message);
                }
                if (dc == null) {
                    string message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.DistributionCenterWArt, DistributionCenterID);
                    throw new IDNotFoundException(message);
                }
                user.DistributionCenters.Remove(dc);
                db.SaveChanges();

                return new Status { Success = true, Message = string.Empty };
            } catch (Exception ex) {
                return new Status { Success = false, Message = ex.Message };
            }
        }

        public bool updateLastLoginDate(string userName) {
            User user = this.GetUserByUserName(userName);
            if (user == null) {
                return false;
            }
            user = this.db.Users.Find(user.FileNumber);
            user.LastLoginDate = System.DateTime.Now;
            this.db.SaveChanges();
            return true;
        }

        public IQueryable<User> getUsersByRole(RoleType role) {
            return this.db.Users.Where(x => x.Roles.Any(y => y.RoleID == (int)role) && x.Active == true);
        }

        private bool deleteNotifications(User user) {
            try {
                user.Notifications.Clear();
                this.db.SaveChanges();
                return true;
            } catch (UpdateException) {
                return false;
            }
        }

        private bool deleteAdministred(User user) {
            try {
                foreach (DistributionCenter dc in user.DistributionCenters) {
                    user.DistributionCenters.Remove(dc);
                }
                this.db.SaveChanges();
                return true;
            } catch (UpdateException) {
                return false;
            }
        }

        private bool deleteRoles(User user) {
            try {
                foreach (Role role in user.Roles) {
                    user.Roles.Remove(role);
                }
                this.db.SaveChanges();
                return true;
            } catch (UpdateException) {
                return false;
            }
        }
    }
}