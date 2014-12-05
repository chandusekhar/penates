using Penates.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories {
    public interface ICityRepository {

        /// <summary>Obtiene los datos de una ciudad</summary>
        /// <param name="id">Id de la ciudad</param>
        /// <returns>City</returns>
        City getCityData(long id);

        /// <summary>Obtiene todas las ciudades</summary>
        /// <returns>IQueryable<City></returns>
        IQueryable<City> getCities();

        /// <summary>Obtiene las ciudades para una determinada provincia/estado</summary>
        /// <param name="provinceID">Id de la provincia/Estado</param>
        /// <returns>IQueryable<City></returns>
        IQueryable<City> getCities(long provinceID);
    }
}
