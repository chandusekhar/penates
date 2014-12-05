using Penates.Models.ViewModels.Users;

namespace Penates.Interfaces.Repositories
{
    using Penates.Database;
    using Penates.Models;
    using Penates.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IRoleRepository
    {
        IQueryable<Role> getData();

        Status DeleteRole(int RoleId);

        Roles GetRole(long RoleId);

        Status EditRole(Roles Role);

    }
}
