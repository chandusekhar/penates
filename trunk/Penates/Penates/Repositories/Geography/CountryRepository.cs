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
    public class CountryRepository : ICountryRepository {

        PenatesEntities db = new PenatesEntities();

        /// <summary>Obtiene los datos de un pais</summary>
        /// <param name="id">Id del pais</param>
        /// <returns>Country</returns>
        public Country getData(long id) {
            try {
                return db.Countries.Find(id);
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        /// <summary>Obtiene todos los paises</summary>
        /// <returns>IQueryable<Country></returns>
        public IQueryable<Country> getData() {
            try {
                return this.db.Countries.OrderBy(x => x.Name);
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary>Obtiene el autocompletado para los paises</summary>
        /// <param name="search">Nombre a buscar</param>
        public IQueryable<Country> getAutocomplete(string search) {
            try {
                var data = this.searchAndRank(search);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Country> searchAndRank(string search) {
            var data = this.getData();
            return this.searchAndRank(data, search);
        }

        public IQueryable<Country> searchAndRank(IQueryable<Country> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.Name).Skip(0).Take(Properties.Settings.Default.autocompleteItems);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<Country> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<Country> {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double) x.table.CountryID).Trim() == item ? 1000 : 0) +
                                ((x.table.Name.Contains(item)) ? item.Length : 0) + ((x.table.Name.StartsWith(item)) ? (item.Length * 2) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.Name.Length)
                    .ThenBy(x => x.table.Name)
                    .Select(x => x.table);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }
    }
}