using Penates.Database;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories {
    public interface ISaleRepository {

        Sale getSaleInfo(long id);

        IQueryable<Sale> getData();

        IQueryable<Sale> search(IQueryable<Sale> query, string search);

        IQueryable<Sale> search(IQueryable<Sale> query, List<string> search);

        IQueryable<Sale> sort(IQueryable<Sale> query, Sorts sort);

        IQueryable<Sale> filterByDistributionCenter(IQueryable<Sale> query, long dcID);

        IQueryable<Sale> filterByClient(IQueryable<Sale> query, long id);

        IQueryable<Sale> filterByAnnulated(IQueryable<Sale> query, bool annulated);

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        Status Annulate(long saleID);

        IQueryable<Sale> getAutocomplete(string search, long? dcID, long? clientID, bool? anulated);
    }
}
