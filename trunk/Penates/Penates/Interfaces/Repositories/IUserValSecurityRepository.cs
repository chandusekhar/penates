using Penates.Database;
using Penates.Models.ViewModels.Users;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Interfaces.Repositories {
    public interface IUserValSecurityRepository {

        Status Save(ValSecurityViewModel model);

        Status Delete(string fileNumber);

        OrdersValueSecurity getOrderValueData(string UserID);

        ProductsValueSecurity getProductValueData(string UserID);

        IQueryable<ValSecurityTableJson> getData();

        IQueryable<ValSecurityTableJson> search(IQueryable<ValSecurityTableJson> query, string search);

        IQueryable<ValSecurityTableJson> search(IQueryable<ValSecurityTableJson> query, List<string> search);

        IQueryable<ValSecurityTableJson> sort(IQueryable<ValSecurityTableJson> dc, Sorts sort);
    }
}