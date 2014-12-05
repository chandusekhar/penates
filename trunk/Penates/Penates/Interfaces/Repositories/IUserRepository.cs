using Penates.Models.ViewModels.Users;

namespace Penates.Interfaces.Repositories
{
    using Penates.Database;
    using Penates.Models;
    using Penates.Utils;
    using Penates.Utils.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IUserRepository
    {
        Status Register(RegisterViewModel user);

        Status Save(UserViewModel user);

        /// <summary> Devuelve true si lo desactiva y false si no puede /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        bool Deactivate(string userID);

        /// <summary> Devuelve true si lo activa y false si no puede /// </summary>
        /// <param name="id">ID a activar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        bool Activate(string userID);

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        bool Delete(string userID);

        User getData(string UserID);

        bool Login(string username, string password);

        Status AddUserToRole(string user, long role);

        List<Role> GetRolesForUser(string userID);

        User GetUserByUserName(string userName);

        void InactiveUser(string fileNumber);

        Status RemoveUserFromRole(string UserID, long RoleID);

        IQueryable<User> getData();

        IQueryable<User> search(IQueryable<User> query, string search);

        IQueryable<User> search(IQueryable<User> query, List<string> search);

        IQueryable<User> filterActive(IQueryable<User> query, bool active);

        IQueryable<User> filterRole(IQueryable<User> query, long roleID);

        IQueryable<User> filterByDistributionCenter(IQueryable<User> query, long dcID);

        IQueryable<User> sort(IQueryable<User> dc, Sorts sort);

        /// <summary>Valida que un usuario exista y este activo</summary>
        /// <param name="userName">Nomvre de usuario</param>
        /// <returns>True si es valido, sino false</returns>
        bool validateUser(string userName);

        /// <summary>Change de password of a user</summary>
        /// <param name="userName">Username to change the password</param>
        /// <param name="newPassword">New Password</param>
        /// <returns>true si logra cambiarla, false sino</returns>
        bool changePassword(string userName, string newPassword);

        /// <summary>Change de password of a user</summary>
        /// <param name="userName">Username to change the password</param>
        /// <param name="password">Current Password</param>
        /// <param name="newPassword">New Password</param>
        /// <returns>true si logra cambiarla, false sino</returns>
        bool changeMyPassword(string userName, string password, string newPassword);

        IQueryable<User> getAutocomplete(string search);

        ICollection<Role> getRoles(string FileNumber);

        ICollection<DistributionCenter> getDistributionCenters(string FileNumber);

        Status AttachDistributionCenter(string user, long DistributionCenterID);

        Status DetachDistributionCenter(string UserID, long DistributionCenterID);

        bool updateLastLoginDate(string userName);

        IQueryable<User> getUsersByRole(RoleType role);
    }
}
