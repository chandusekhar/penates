using Penates.Database;
using Penates.Models.ViewModels.Users;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Services {
    public interface IUserValSecurityService {

        Status Save(ValSecurityViewModel model);

        Status Delete(string fileNumber);

        /// <summary>Obtiene los datos de un Hall</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>HallViewModel</returns>
        /// <exception cref="IDNotFoundException"
        ValSecurityViewModel getSecurityData(string userID);

        /// <summary>Obtiene los datos de un Sector, pero con los nombres de las categorias en lugar de los IDs</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        ValSecurityViewModel getSecuritySummary(string userID);

        IQueryable<ValSecurityTableJson> getData();

        IQueryable<ValSecurityTableJson> search(IQueryable<ValSecurityTableJson> query, string search);

        IQueryable<ValSecurityTableJson> sort(IQueryable<ValSecurityTableJson> query, int index, string direction);

        IQueryable<ValSecurityTableJson> sort(IQueryable<ValSecurityTableJson> query, Sorts sort);

        /// <summary>Activa varios Usuarios</summary>
        /// <param name="id">ID a Activar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        Status Delete(List<string> userIDs);

        IQueryable<Product> checkProduct(IQueryable<Product> query, string FileNumber);

        IQueryable<SupplierOrder> filterOrders(IQueryable<SupplierOrder> query, string FileNumber);
    }
}
