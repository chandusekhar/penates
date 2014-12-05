using Penates.Utils;
using System;
using System.Collections.Generic;
namespace Penates.Interfaces.Services {
    public interface IUserService {
        bool ActivateDeactivate(string userID);

        /// <summary>Activa varios Usuarios</summary>
        /// <param name="id">ID a Activar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        Status Activate(List<string> userIDs);

        /// <summary>Activa varios Usuarios</summary>
        /// <param name="id">ID a Activar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        Status Deactivate(List<string> userIDs);

        Status AddUserToRole(string userID, long roleID);
        Status AttachDistributionCenter(string user, long DistributionCenterID);
        bool changePassword(string userName, string newPassword);
        bool changePassword(string userName, string password, string newPassword);
        bool Delete(string userID);
        Status DetachDistributionCenter(string UserID, long DistributionCenterID);
        System.Linq.IQueryable<Penates.Database.User> filterActive(System.Linq.IQueryable<Penates.Database.User> query, long? active);
        System.Linq.IQueryable<Penates.Database.User> filterByDistributionCenter(System.Linq.IQueryable<Penates.Database.User> query, long? dcID);
        System.Linq.IQueryable<Penates.Database.User> filterByRole(System.Linq.IQueryable<Penates.Database.User> query, long? roleID);
        System.Linq.IQueryable<Penates.Database.User> getAutocomplete(string search);
        System.Linq.IQueryable<Penates.Database.User> getData();
        Penates.Models.ViewModels.Users.UserViewModel getData(string UserID);
        System.Collections.Generic.ICollection<Penates.Database.DistributionCenter> getDistributionCenters(string FileNumber);
        System.Collections.Generic.ICollection<Penates.Database.Role> getRoles(string UserID);
        System.Collections.Generic.List<Penates.Database.Role> GetRolesForUser(string userID);
        Penates.Database.User GetUserByUserName(string userName);
        void InactiveUser(string fileNumber);
        bool isActive(string userID);
        bool Login(string username, string password);
        Status Register(Penates.Models.ViewModels.Users.RegisterViewModel user);
        Status RemoveUserFromRole(string userID, long roleID);
        Status Save(Penates.Models.ViewModels.Users.UserViewModel user);
        System.Linq.IQueryable<Penates.Database.User> search(System.Linq.IQueryable<Penates.Database.User> query, string search);
        System.Linq.IQueryable<Penates.Database.User> sort(System.Linq.IQueryable<Penates.Database.User> query, Penates.Utils.Sorts sort);
        System.Linq.IQueryable<Penates.Database.User> sort(System.Linq.IQueryable<Penates.Database.User> query, int index, string direction);
        System.Collections.Generic.List<Penates.Utils.JSON.TableObjects.UserTableJson> toJsonArray(System.Collections.Generic.ICollection<Penates.Database.User> list);
        System.Collections.Generic.List<Penates.Utils.JSON.TableObjects.UserTableJson> toJsonArray(System.Linq.IQueryable<Penates.Database.User> query);
        System.Collections.Generic.List<Penates.Models.AutocompleteItemStringID> toJsonAutocomplete(System.Linq.IQueryable<Penates.Database.User> query);
        Penates.Utils.Sorts toSort(int sortColumnIndex, string sortDirection);
        bool updateLastLoginDate(string userName);
        bool validateUser(string userName);
    }
}
