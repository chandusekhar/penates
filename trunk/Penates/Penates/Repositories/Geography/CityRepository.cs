using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Exceptions.System;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace Penates.Repositories.Geography {
    public class CityRepository : ICityRepository {

        PenatesEntities db = new PenatesEntities();

        /// <summary>Obtiene los datos de una ciudad</summary>
        /// <param name="id">Id de la ciudad</param>
        /// <returns>City</returns>
        public City getCityData(long id) {
            try {
                return db.Cities.Find(id);
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        /// <summary>Obtiene todas las ciudades</summary>
        /// <returns>IQueryable<City></returns>
        public IQueryable<City> getCities() {
            try {
                return this.db.Cities.OrderBy(x => x.Name);
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary>Obtiene las ciudades para una determinada provincia/estado</summary>
        /// <param name="provinceID">Id de la provincia/Estado</param>
        /// <returns>IQueryable<City></returns>
        public IQueryable<City> getCities(long provinceID) {
            IQueryable<City> data = this.getCities();
            return data.Where(x => x.IDProvince == provinceID);
        }
    }
}