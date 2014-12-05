using Penates.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories {
    public interface ICountryRepository {

        /// <summary>Obtiene los datos de un pais</summary>
        /// <param name="id">Id del pais</param>
        /// <returns>Country</returns>
        Country getData(long id);

        /// <summary>Obtiene todos los paises</summary>
        /// <returns>IQueryable<Country></returns>
        IQueryable<Country> getData();

        /// <summary>Obtiene el autocompletado para los paises</summary>
        /// <param name="search">Nombre a buscar</param>
        IQueryable<Country> getAutocomplete(string search);

        IQueryable<Country> searchAndRank(string search);

        IQueryable<Country> searchAndRank(IQueryable<Country> data, string search);
    }
}
