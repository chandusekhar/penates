using Penates.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories {
    public interface IStateRepository {
        /// <summary>Obtiene los datos de una provincia/estado</summary>
        /// <param name="id">Id de la provincia/estado</param>
        /// <returns>Province</returns>
        Province getProvinceData(long id);

        /// <summary>Obtiene todas las provincaias/estados</summary>
        /// <returns>IQueryable<Province></returns>
        IQueryable<Province> getProvinces();

        /// <summary>Obtiene las provincias/estados para un determinado pais</summary>
        /// <param name="provinceID">Id del pais</param>
        /// <returns>IQueryable<Province></returns>
        IQueryable<Province> getProvinces(long countryID);
    }
}
