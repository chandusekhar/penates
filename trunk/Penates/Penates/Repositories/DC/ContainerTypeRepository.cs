using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.DC;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Repositories.DC {
    public class ContainerTypeRepository : IContainerTypeRepository {

        PenatesEntities db = new PenatesEntities();

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>true si funciona, sino una excepcion</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Save(ContainerTypeViewModel type) {
            try {
                ContainerType aux = null;
                if (type.ContainerTypeID.HasValue) {
                    if (type.ContainerTypeID.Value == 0) {
                        return 0;
                    }
                    aux = db.ContainerTypes.Find(type.ContainerTypeID);
                }
                Nullable<long> val = null;
                if (aux == null) {
                    val = db.SP_ContainerType_Add(type.Description, type.Height, type.Width, type.Depth).SingleOrDefault();
                } else {
                    val = db.SP_ContainerType_Edit(type.ContainerTypeID, type.Description, type.Height, type.Width, type.Depth).SingleOrDefault();
                }
                if (!val.HasValue) {
                    return -1;
                }
                return val.Value;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.ContainerTypeWArt));
            }
        }

        public bool Delete(long id) {
            //Se supone que restrinjo para que siempre lo encuentre
            ContainerType type = db.ContainerTypes.Find(id);
            if (type == null) {
                return true;
            }
            if (checkDeleteConstrains(type)) {
                var tran = this.db.Database.BeginTransaction();
                try {
                    this.db.ContainerTypes.Remove(type);
                    db.SaveChanges();
                    tran.Commit();
                    return true;
                } catch (Exception e) {
                    tran.Rollback();
                    throw new DatabaseException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.ContainerTypeWArt, id), e.Message);
                }
            } else {
                throw new DeleteConstrainException(String.Format(Resources.ExceptionMessages.DeleteException,
                Resources.Resources.ContainerTypeWArt, id),
                String.Format(Resources.ExceptionMessages.DeleteConstraintMessage, Resources.Resources.ContainerTypeWArt,
                Resources.Resources.Containers, Resources.Resources.ContainerWArt));
            }
        }

        public ContainerType getData(long id) {
            try {
                return db.ContainerTypes.Find(id);
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<ContainerType> getData() {
            try {
                return this.db.ContainerTypes;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public decimal getContainerArea(long ContainerTypeID) {
            try {
                ContainerType container = this.db.ContainerTypes.Find(ContainerTypeID);
                return (container.Depth * container.Width);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw e;
            }
        }

        public decimal getAvaibleContainersQuantity(long ContainerTypeID)
        {
            try
            {
                return db.Containers.Where(x => x.ContainerType.ContainerTypesID == ContainerTypeID  && x.UsedSpace == 0).Count();               
            }
            catch (DatabaseException de)
            {
                throw de;
            } catch (Exception e) {
                throw e;
            }
        }


        public string getContainerTypeName(long ContainerTypeID)
        {
            try
            {
                return db.ContainerTypes.Find(ContainerTypeID).Description;
            }
            catch (DatabaseException de)
            {
                throw de;
            } catch (Exception e) {
                throw e;
            }
        }


        public decimal getContainerSize(long ContainerTypeID)
        {
            try
            {
                return (decimal) db.ContainerTypes.Find(ContainerTypeID).Size;
            }
            catch (DatabaseException de)
            {
                throw de;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public decimal getContainerHeight(long ContainerTypeID)
        {
            try
            {
                return db.ContainerTypes.Find(ContainerTypeID).Height;
            }
            catch (DatabaseException de)
            {
                throw de;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IQueryable<ContainerType> getAutocomplete(string search) {
            try {
                var data = this.searchAndRank(search);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }


        public IQueryable<ContainerType> searchAndRank(string search) {
            var data = this.getData();
            return this.searchAndRank(data, search);
        }

        public IQueryable<ContainerType> searchAndRank(IQueryable<ContainerType> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.Description).Skip(0).Take(Properties.Settings.Default.autocompleteItems);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<ContainerType> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<ContainerType> {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double) x.table.ContainerTypesID).Trim() == item ? 1000 : 0) +
                                ((x.table.Description.Contains(item)) ? item.Length : 0) + ((x.table.Description.StartsWith(item)) ? (item.Length * 2) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.Description.Length)
                    .ThenBy(x => x.table.Description)
                    .Select(x => x.table);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<ContainerType> search(IQueryable<ContainerType> query, string search) {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<ContainerType> search(IQueryable<ContainerType> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Description.Contains(item) || (SqlFunctions.StringConvert((double) p.ContainerTypesID).Trim() == item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<ContainerType> sort(IQueryable<ContainerType> query, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ID:
                    query = query.OrderBy(p => p.ContainerTypesID);
                    break;
                case Sorts.ID_DESC:
                    query = query.OrderByDescending(p => p.ContainerTypesID);
                    break;
                case Sorts.SIZE:
                    query = query.OrderBy(p => p.Size);
                    break;
                case Sorts.SIZE_DESC:
                    query = query.OrderByDescending(p => p.Size);
                    break;
                case Sorts.DESCRIPTION:
                    query = query.OrderBy(p => p.Description);
                    break;
                case Sorts.DESCRIPTION_DESC:
                    query = query.OrderByDescending(p => p.Description);
                    break;
                default:
                    query = query.OrderBy(p => p.ContainerTypesID);
                    break;
            }
            return query;
        }

        private bool checkDeleteConstrains(ContainerType type) {
            if (type.Containers == null || type.Containers.Count == 0) {
                return true;
            }
            return false;
        }
    }
}
