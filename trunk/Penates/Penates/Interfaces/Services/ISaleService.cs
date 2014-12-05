using Penates.Database;
using Penates.Models;
using Penates.Models.ViewModels.Transactions.Sales;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Services {
    public interface ISaleService {

        /// <summary>Elimina un centro de Distribuciones</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        Status Anulate(long saleID);

        /// <summary>Obtiene los datos de un Hall</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>RackViewModel</returns>
        /// <exception cref="IDNotFoundException"
        SaleViewModel getRackData(long saleID);

        IQueryable<Sale> getData();

        List<SaleTableJson> toJsonArray(IQueryable<Sale> query);

        List<SaleTableJson> toJsonArray(ICollection<Sale> list);

        IQueryable<Sale> search(IQueryable<Sale> query, string search);

        IQueryable<Sale> sort(IQueryable<Sale> query, int index, string direction);

        IQueryable<Sale> sort(IQueryable<Sale> query, Sorts sort);

        IQueryable<Sale> filterByDistributionCenter(IQueryable<Sale> query, long? dcID);

        IQueryable<Sale> filterByClient(IQueryable<Sale> query, long? clientID);

        IQueryable<Sale> filterByAnulated(IQueryable<Sale> query, int? anulated);

        IQueryable<Sale> getAutocomplete(string search, long? dcID, long? clientID, bool? anulated);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<Sale> query);
    }
}
