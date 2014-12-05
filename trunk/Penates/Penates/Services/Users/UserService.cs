using Penates.Exceptions.Database;

namespace Penates.Services.Users
{
    using Penates.Repositories.Users;
    using Penates.Models.ViewModels.Users;
    using System.Web.Mvc;
    using Penates.Database;
    using System.Collections.Generic;
    using Penates.Models;
    using Penates.Utils.JSON.TableObjects;
    using System.Linq;
    using Penates.Utils;
    using Penates.Exceptions.Views;
    using Penates.Utils.Enums;
    using Penates.Interfaces.Repositories;
    using Penates.Interfaces.Services;
    using System;

    public class UserService : IUserService
    {
        private IUserRepository repository = new UserRepository();

        public bool Login(string username, string password)
        {
            var repository = new UserRepository();
            return repository.Login(username, password);
        }

        public Status Register(RegisterViewModel user)
        {
            var repository = new UserRepository();
            return repository.Register(user);
        }

        public Status Save(UserViewModel user) {
            return this.repository.Save(user);
        }

        /// <summary>Activa o Desactiva un Usuario</summary>
        /// <param name="id">ID a Desactivar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        public bool ActivateDeactivate(string userID) {
            User user = this.repository.getData(userID);
            if (user == null) {
                return false;
            }
            if (user.Active) {
                return this.repository.Deactivate(userID);
            } else {
                return this.repository.Activate(userID);
            }
        }

        /// <summary>Activa varios Usuarios</summary>
        /// <param name="id">ID a Activar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        public Status Activate(List<string> userIDs) {
            Status status = new Status(){
                Success = true,
                Message = ""
            };
            if(userIDs != null){
                foreach (string userID in userIDs) {
                    User user = this.repository.getData(userID);
                    if (user == null) {
                        status.Success = false;
                        status.Message = status.Message + "- "+ 
                            String.Format(Resources.Errors.SearchIDError, Resources.Resources.UserWArt) + "\n";
                    }
                    if (!user.Active) {
                        if (!this.repository.Activate(userID)) {
                            status.Success = false;
                            status.Message = status.Message + "- " +
                                String.Format(Resources.ExceptionMessages.ActivateException, Resources.Resources.UserWArt, userID) +
                                "\n";
                        }
                    }
                }
            }
            return status;
        }

        /// <summary>Activa varios Usuarios</summary>
        /// <param name="id">ID a Activar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        public Status Deactivate(List<string> userIDs) {
            Status status = new Status() {
                Success = true,
                Message = ""
            };
            if (userIDs != null) {
                foreach (string userID in userIDs) {
                    User user = this.repository.getData(userID);
                    if (user == null) {
                        status.Success = false;
                        status.Message = status.Message + "- " +
                            String.Format(Resources.Errors.SearchIDError, Resources.Resources.UserWArt) + "\n";
                    }
                    if (user.Active) {
                        if (!this.repository.Deactivate(userID)) {
                            status.Success = false;
                            status.Message = status.Message + "- " +
                                String.Format(Resources.ExceptionMessages.DeactivateException, Resources.Resources.UserWArt, userID) +
                                "\n";
                        }
                    }
                }
            }
            return status;
        }

        /// <summary>Elimina un Usuario</summary>
        /// <param name="id">ID a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        public bool Delete(string userID) {
            return this.repository.Delete(userID);
        }

        public Status AddUserToRole(string userID, long roleID)
        {
            var repository = new UserRepository();
            return repository.AddUserToRole(userID, roleID);
        }

        public List<Role> GetRolesForUser(string userID)
        {
            var repository = new UserRepository();
            return repository.GetRolesForUser(userID);
        }

        public Status RemoveUserFromRole(string userID, long roleID)
        {
            var repository = new UserRepository();
            return repository.RemoveUserFromRole(userID, roleID);
        }

        public List<UserTableJson> toJsonArray(IQueryable<User> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<UserTableJson> toJsonArray(ICollection<User> list) {
            List<UserTableJson> result = new List<UserTableJson>();
            UserTableJson aux;
            foreach (User user in list) {
                aux = new UserTableJson() {
                    UserID = user.FileNumber,
                    Name = user.LastName + ", " + user.FirstName,
                    Username = user.Username,
                    Email = user.Email,
                    Active = user.Active
                };
                result.Add(aux);
            }
            return result;
        }

        public UserViewModel getData(string UserID) {
            User user = this.repository.getData(UserID);
            if (UserID == null) {
                return null;
            }
            UserViewModel model = new UserViewModel() {
                Active = user.Active,
                Address = user.Address,
                Email = user.Email,
                FileNumber = user.FileNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Telephone = user.Phone,
                UserName = user.Username,
                LastLoginDate = user.LastLoginDate
            };
            return model;
        }

        public IQueryable<User> getData() {
            return this.repository.getData();
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
                        return Sorts.NAME;
                    } else {
                        return Sorts.NAME_DESC;
                    }
                case 2:
                    if (sortDirection == "asc") {
                        return Sorts.USERNAME;
                    } else {
                        return Sorts.USERNAME_DESC;
                    }
                case 3:
                    if (sortDirection == "asc") {
                        return Sorts.EMAIL;
                    } else {
                        return Sorts.EMAIL_DESC;
                    }
                default:
                    throw new SortException(Resources.ExceptionMessages.InvalidSortException, "Col: " + sortColumnIndex);
            }
        }

        public IQueryable<User> search(IQueryable<User> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<User> sort(IQueryable<User> query, int index, string direction) {
            try {
                Sorts sort = this.toSort(index, direction);
                return this.repository.sort(query, sort);
            } catch (SortException) {
                return query;
            }
        }

        public IQueryable<User> sort(IQueryable<User> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        public IQueryable<User> filterActive(IQueryable<User> query, long? active) {
            if (active.HasValue && active >= 0) {
                if (active == UserTypes.INACTIVE.getTypeNumber()) {
                    return this.repository.filterActive(query, false);
                } else {
                    return this.repository.filterActive(query, true);
                }
            }
            return query;
        }

        public IQueryable<User> filterByRole(IQueryable<User> query, long? roleID) {
            if (roleID.HasValue && roleID.Value != -1) {
                return this.repository.filterRole(query, roleID.Value);
            }
            return query;
        }

        public IQueryable<User> filterByDistributionCenter(IQueryable<User> query, long? dcID) {
            if(dcID.HasValue){
                return this.repository.filterByDistributionCenter(query, dcID.Value);
            }
            return query;
        }

        public bool isActive(string userID) {
            User user = this.repository.getData(userID);
            if (user == null || user.Active == false) {
                return false;
            }
            return true;
        }

        public User GetUserByUserName(string userName) {
            return this.repository.GetUserByUserName(userName);
        }

        public void InactiveUser(string fileNumber)
        {
            this.repository.InactiveUser(fileNumber);
        }

        /// <summary>Valida que un usuario exista y este activo</summary>
        /// <param name="userName">Nomvre de usuario</param>
        /// <returns>True si es valido, sino false</returns>
        public bool validateUser(string userName) {
            return this.repository.validateUser(userName);
        }

        /// <summary>Change de password of a user</summary>
        /// <param name="userName">Username to change the password</param>
        /// <param name="newPassword">New Password</param>
        /// <returns>true si logra cambiarla, false sino</returns>
        public bool changePassword(string userName, string newPassword) {
            return this.repository.changePassword(userName, newPassword);
        }

        /// <summary>Change de password of a user</summary>
        /// <param name="userName">Username to change the password</param>
        /// <param name="password">Current Password</param>
        /// <param name="newPassword">New Password</param>
        /// <returns>true si logra cambiarla, false sino</returns>
        public bool changePassword(string userName, string password, string newPassword) {
            return this.repository.changeMyPassword(userName, password, newPassword);
        }

        public IQueryable<User> getAutocomplete(string search) {
            return this.repository.getAutocomplete(search);
        }

        public List<AutocompleteItemStringID> toJsonAutocomplete(IQueryable<User> query) {
            List<User> list = query.ToList();
            List<AutocompleteItemStringID> result = new List<AutocompleteItemStringID>();
            AutocompleteItemStringID aux;
            foreach (User user in list) {
                aux = new AutocompleteItemStringID() {
                    ID = user.FileNumber,
                    Label = user.Username,
                    Description = user.LastName + ", " + user.FirstName
                };
                result.Add(aux);
            }
            return result;
        }

        public ICollection<Role> getRoles(string UserID) {
            return this.repository.getRoles(UserID);
        }

        public ICollection<DistributionCenter> getDistributionCenters(string FileNumber) {
            return this.repository.getDistributionCenters(FileNumber);
        }

        public Status AttachDistributionCenter(string user, long DistributionCenterID) {
            return this.repository.AttachDistributionCenter(user, DistributionCenterID);
        }

        public Status DetachDistributionCenter(string UserID, long DistributionCenterID) {
            return this.repository.DetachDistributionCenter(UserID, DistributionCenterID);
        }

        public bool updateLastLoginDate(string userName) {
            return this.repository.updateLastLoginDate(userName);
        }
    }
}