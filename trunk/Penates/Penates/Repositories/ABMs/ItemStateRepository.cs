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
    public class ItemStateRepository : IItemStateRepository {

        PenatesEntities db = new PenatesEntities();

        /// <summary> Guarda los datos de un Estado</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>true si funciona, sino una excepcion</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Save(StatusViewModel status) {
            try {
                ItemsState aux = db.ItemsStates.Find(status.StatusID);
                Nullable<long> val = null;
                if (aux == null) {
                    val = db.SP_ItemState_Add(status.Description).SingleOrDefault();
                } else {
                    val = db.SP_ItemState_Edit(status.StatusID, status.Description).SingleOrDefault();
                }
                if (!val.HasValue) {
                    return -1;
                }
                return val.Value;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.ItemStateWArt));
            }
        }

        public bool Delete(long id) {
            if (id == 0) {
                return false;
            }
            //Se supone que restrinjo para que siempre lo encuentre
            ItemsState state = db.ItemsStates.Find(id);
            if (state == null) {
                return true;
            }
            if (checkDeleteConstrains(state)) {
                var tran = this.db.Database.BeginTransaction();
                try {
                    state.Deleted = true;
                    this.db.ItemsStates.Attach(state);
                    var entry = db.Entry(state);
                    entry.Property(e => e.Deleted).IsModified = true;
                    db.SaveChanges();
                    tran.Commit();
                    return true;
                } catch (Exception e) {
                    tran.Rollback();
                    throw new DatabaseException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.ItemStateWArt, id), e.Message);
                }
            } else {
                throw new DeleteConstrainException(String.Format(Resources.ExceptionMessages.DeleteException,
                Resources.Resources.ItemStateWArt, id),
                String.Format(Resources.ExceptionMessages.DeleteConstraintMessage, Resources.Resources.ItemStateWArt,
                Resources.Resources.Boxes, Resources.Resources.BoxesWArt));
            }
        }

        public ItemsState getData(long id) {
            try {
                return db.ItemsStates.Find(id);
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<ItemsState> getData() {
            try {

                var categories =
                        from p in this.db.ItemsStates
                        where p.Deleted == false
                        orderby p.Description
                        select p;

                return categories;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<ItemsState> getData(bool includeDeleted) {
            try {
                if (includeDeleted) {
                    return this.db.ItemsStates;
                } else {
                    return this.getData();
                }
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<ItemsState> getAutocomplete(string search) {
            try {
                var data = this.searchAndRank(search);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<ItemsState> searchAndRank(string search) {
            var data = this.getData();
            return this.searchAndRank(data, search);
        }

        public IQueryable<ItemsState> searchAndRank(IQueryable<ItemsState> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.Description).Skip(0).Take(Properties.Settings.Default.autocompleteItems);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<ItemsState> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<ItemsState> {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double) x.table.ItemStateID).Trim() == item ? 1000 : 0) +
                                ((x.table.Description.Contains(item)) ? item.Length : 0) + ((x.table.Description.StartsWith(item)) ? (item.Length * 2) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.Description)
                    .ThenBy(x => x.table.Description.Length)
                    .Select(x => x.table);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<ItemsState> search(IQueryable<ItemsState> query, string search) {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<ItemsState> search(IQueryable<ItemsState> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Description.Contains(item) || (SqlFunctions.StringConvert((double) p.ItemStateID).Trim() == item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<ItemsState> sort(IQueryable<ItemsState> query, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ID:
                    query = query.OrderBy(p => p.ItemStateID);
                    break;
                case Sorts.ID_DESC:
                    query = query.OrderByDescending(p => p.ItemStateID);
                    break;
                case Sorts.DESCRIPTION:
                    query = query.OrderBy(p => p.Description);
                    break;
                case Sorts.DESCRIPTION_DESC:
                    query = query.OrderByDescending(p => p.Description);
                    break;
                default:
                    query = query.OrderBy(p => p.ItemStateID);
                    break;
            }
            return query;
        }

        private bool checkDeleteConstrains(ItemsState state) {
            if (state.Boxes == null || state.Boxes.Count == 0) {
                return true;
            } else {
                if (state.Boxes.Any(x => x.Deleted == false)) {
                    return false;
                } else {
                    return true;
                }
            }
        }

        public ItemsState getDefaultState()
        {
            try
            {
                return this.db.ItemsStates.Where(p => p.ItemStateID == 0).FirstOrDefault();       
            }
            catch (DatabaseException de)
            {
                throw de;
            }
            catch (Exception e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }
    }
}