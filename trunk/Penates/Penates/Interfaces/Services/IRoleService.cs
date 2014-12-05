using Penates.Database;
using Penates.Models.ViewModels.Users;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Penates.Interfaces.Services {
    public interface IRoleService {

        IQueryable<Role> getData();

        Status DeleteRole(int roleID);

        Roles GetRole(long roleID);

        Status EditRole(Roles role);

        /// <summary>Obtiene la lista de roles</summary>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        SelectList getRolesList();

        /// <summary>Obtiene la lista de roles</summary>
        /// <param name="includeAll">Indica si se agrega o no la opcion de All</param>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        SelectList getRolesList(bool includeAll);

        List<RoleTableJson> toJsonArray(IQueryable<Role> query);

        List<RoleTableJson> toJsonArray(ICollection<Role> list);
    }
}
