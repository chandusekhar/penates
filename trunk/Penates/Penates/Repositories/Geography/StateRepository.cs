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
    public class StateRepository : IStateRepository {

        PenatesEntities db = new PenatesEntities();

        /// <summary>Obtiene los datos de una provincia/estado</summary>
        /// <param name="id">Id de la provincia/estado</param>
        /// <returns>Province</returns>
        public Province getProvinceData(long id) {
            try {
                return db.Provinces.Find(id);
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        /// <summary>Obtiene todas las provincaias/estados</summary>
        /// <returns>IQueryable<Province></returns>
        public IQueryable<Province> getProvinces() {
            try {
                return this.db.Provinces.OrderBy(x => x.Name);
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary>Obtiene las provincias/estados para un determinado pais</summary>
        /// <param name="provinceID">Id del pais</param>
        /// <returns>IQueryable<Province></returns>
        public IQueryable<Province> getProvinces(long countryID) {
            IQueryable<Province> data = this.getProvinces();
            return data.Where(x => x.IDCountry == countryID);
        }
    }
}