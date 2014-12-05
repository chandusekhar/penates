using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace Penates.Repositories.ABMs {
    public class PackRepository : IPackRepository {

        PenatesEntities db = new PenatesEntities();

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>true si funciona, sino una excepcion</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Save(PackViewModel pack) {
            try {
                Pack aux = db.Packs.Find(pack.PackID);
                Nullable<long> val = null;
                if (aux == null) {
                    val = db.SP_Pack_Add(pack.Description, pack.ExpirationDate, pack.SerialNumber).SingleOrDefault();
                } else {
                    val = db.SP_Pack_Edit(pack.PackID, pack.Description, pack.ExpirationDate, pack.SerialNumber).SingleOrDefault();
                }
                if (!val.HasValue) {
                    return -1;
                }
                return val.Value;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.PackWArt));
            }
        }

        public bool Delete(long id) {
            //Se supone que restrinjo para que siempre lo encuentre
            Pack pack = db.Packs.Find(id);
            if (pack == null) {
                return true;
            }
            if (checkDeleteConstrains(pack)) {
                var tran = this.db.Database.BeginTransaction();
                try {
                    pack.Deleted = true;
                    this.db.Packs.Attach(pack);
                    var entry = db.Entry(pack);
                    entry.Property(e => e.Deleted).IsModified = true;
                    db.SaveChanges();
                    tran.Commit();
                    return true;
                } catch (Exception e) {
                    tran.Rollback();
                    throw new DatabaseException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.PackWArt, id), e.Message);
                }
            } else {
                throw new DeleteConstrainException(String.Format(Resources.ExceptionMessages.DeleteException,
                Resources.Resources.PackWArt, id),
                String.Format(Resources.ExceptionMessages.DeleteConstraintMessage, Resources.Resources.PackWArt,
                Resources.Resources.Boxes, Resources.Resources.BoxesWArt));
            }
        }

        public Pack getData(long id) {
            try {
                return db.Packs.Find(id);
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<Pack> getData() {
            try {

                var categories =
                        from p in this.db.Packs
                        where p.Deleted == false
                        orderby p.Description
                        select p;

                return categories;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<Pack> getData(bool includeDeleted) {
            try {
                if (includeDeleted) {
                    return this.db.Packs;
                } else {
                    return this.getData();
                }
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Pack> getAutocomplete(string search) {
            try {
                var data = this.searchAndRank(search);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Pack> searchAndRank(string search) {
            var data = this.getData();
            return this.searchAndRank(data, search);
        }

        public IQueryable<Pack> searchAndRank(IQueryable<Pack> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.Description).Skip(0).Take(Properties.Settings.Default.autocompleteItems);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<Pack> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<Pack> {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double) x.table.PackID).Trim() == item ? 1000 : 0) +
                                ((x.table.Description.Contains(item)) ? item.Length : 0) + ((x.table.Description.StartsWith(item)) ? (item.Length * 2) : 0) +
                                ((x.table.SerialNumber.Contains(item)) ? item.Length*8 : 0) + ((x.table.SerialNumber.StartsWith(item)) ? (item.Length * 16) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.SerialNumber)
                    .ThenBy(x => x.table.SerialNumber.Length)
                    .Select(x => x.table);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Pack> search(IQueryable<Pack> query, string search) {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<Pack> search(IQueryable<Pack> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Description.Contains(item) || (SqlFunctions.StringConvert((double) p.PackID).Trim() == item) ||
                        p.SerialNumber.Contains(item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Pack> sort(IQueryable<Pack> query, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ID:
                    query = query.OrderBy(p => p.PackID);
                    break;
                case Sorts.ID_DESC:
                    query = query.OrderByDescending(p => p.PackID);
                    break;
                case Sorts.CODE:
                    query = query.OrderBy(p => p.SerialNumber);
                    break;
                case Sorts.CODE_DESC:
                    query = query.OrderByDescending(p => p.SerialNumber);
                    break;
                case Sorts.DESCRIPTION:
                    query = query.OrderBy(p => p.Description);
                    break;
                case Sorts.DESCRIPTION_DESC:
                    query = query.OrderByDescending(p => p.Description);
                    break;
                case Sorts.DATE:
                    query = query.OrderBy(p => p.ExpirationDate);
                    break;
                case Sorts.DATE_DESC:
                    query = query.OrderByDescending(p => p.ExpirationDate);
                    break;
                default:
                    query = query.OrderBy(p => p.PackID);
                    break;
            }
            return query;
        }

        private bool checkDeleteConstrains(Pack pack) {
            if (pack.Boxes == null || pack.Boxes.Count == 0) {
                return true;
            } else {
                if (pack.Boxes.Any(x => x.Deleted == false)) {
                    return false;
                } else {
                    return true;
                }
            }
        }
    }
}