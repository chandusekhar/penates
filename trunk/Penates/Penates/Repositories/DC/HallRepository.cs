using Penates.Database;
using Penates.Exceptions;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.DC;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace Penates.Repositories.DC {
    public class HallRepository : IHallRepository {

        PenatesEntities db = new PenatesEntities();

        public int itemsPerPage {get;set;}

        public HallRepository() {
            this.itemsPerPage = 50;
        }

        public HallRepository(int itemsPerPage) {
            this.itemsPerPage = itemsPerPage;
        }

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Save(HallViewModel hall) {
            try {
                Hall aux = null;
                decimal oldSize = 0;
                if (hall.HallID != 0) {
                    aux = db.Halls.Find(hall.HallID);
                    if (aux != null) {
                        hall.SectorID = aux.IDSector;
                        oldSize = aux.Size;
                    }
                }
                if (!this.fitsIntoDistributionCenter(hall.Width, hall.Depth, oldSize, hall.SectorID.Value)) {
                    return -1;
                }
                Nullable<long> val = null;
                if (aux == null) {
                    val = db.SP_Hall_Add(hall.Description, hall.Width, hall.Depth, hall.SectorID).SingleOrDefault();
                } else {
                    val = db.SP_Hall_Edit(hall.HallID, hall.Description, hall.Width, hall.Depth).SingleOrDefault();
                }
                if (!val.HasValue) {
                    return -1;
                }
                return val.Value;
            } catch (ModelErrorException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.HallWArt));
            }
        }

        public Hall getHallInfo(long id) {
            try {
                Hall aux = db.Halls.Find(id);
                return aux;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<Hall> getData() {
            try {
                return this.db.Halls;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Hall> search(IQueryable<Hall> query, string search) {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<Hall> search(IQueryable<Hall> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Description.Contains(item) || SqlFunctions.StringConvert((double) p.HallID).Contains(item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Hall> sort(IQueryable<Hall> query, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ID:
                    query = query.OrderBy(x => x.HallID);
                    break;
                case Sorts.ID_DESC:
                    query = query.OrderByDescending(x => x.HallID);
                    break;
                case Sorts.DESCRIPTION:
                    query = query.OrderBy(x => x.Description);
                    break;
                case Sorts.DESCRIPTION_DESC:
                    query = query.OrderByDescending(x => x.Description);
                    break;
                case Sorts.DISTRIBUTION_CENTER:
                    query = query.OrderBy(x => x.Sector.Deposit.IDDistributionCenter);
                    break;
                case Sorts.DISTRIBUTION_CENTER_DESC:
                    query = query.OrderByDescending(x => x.Sector.Deposit.IDDistributionCenter);
                    break;
                case Sorts.USEDPERCENT:
                    query = query.OrderBy(x => x.UsedSpace);
                    break;
                case Sorts.USEDPERCENT_DESC:
                    query = query.OrderByDescending(x => x.UsedSpace);
                    break;
                case Sorts.DEFAULT:
                    query = query.OrderBy(x => x.HallID);
                    break;
                default:
                    query = query.OrderBy(x => x.HallID);
                    break;
            }
            return query;
        }

        public IQueryable<Hall> filterByDistributionCenter(IQueryable<Hall> query, long dcID) {
            return query.Where(x => x.Sector.Deposit.IDDistributionCenter == dcID);
        }

        public IQueryable<Hall> filterByDeposit(IQueryable<Hall> query, long depositID) {
            return query.Where(x => x.Sector.IDDeposit == depositID);
        }

        public IQueryable<Hall> filterBySector(IQueryable<Hall> query, long sectorID) {
            return query.Where(x => x.IDSector == sectorID);
        }

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        public bool Delete(long hallID) {
            Hall hall = this.db.Halls.Find(hallID);
            if (hall == null) {
                return true;
            }
            if (this.checkDeleteConstraints(hall)) {
                var tran = this.db.Database.BeginTransaction();
                try {
                    db.Halls.Remove(hall);
                    Sector sector = this.db.Sectors.Find(hall.IDSector);
                    sector.UsedSpace = sector.UsedSpace - hall.Size;
                    this.db.Sectors.Attach(sector);
                    var entry = db.Entry(sector);
                    entry.Property(e => e.UsedSpace).IsModified = true;
                    db.SaveChanges();
                    tran.Commit();
                    return true;
                } catch (Exception e) {
                    tran.Rollback();
                    throw new DeleteException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.HallWArt, hallID), e.Message);
                }
            } else {
                if(this.deleteRacks(hall)){
                    var tran = this.db.Database.BeginTransaction();
                    try {
                        db.Halls.Remove(hall);
                        Sector sector = this.db.Sectors.Find(hall.IDSector);
                        sector.UsedSpace = sector.UsedSpace - hall.Size;
                        this.db.Sectors.Attach(sector);
                        var entry = db.Entry(sector);
                        entry.Property(e => e.UsedSpace).IsModified = true;
                        db.SaveChanges();
                        tran.Commit();
                        return true;
                    } catch (Exception e) {
                        tran.Rollback();
                        throw new DeleteException(String.Format(Resources.ExceptionMessages.DeleteException,
                            Resources.Resources.HallWArt, hallID), e.Message);
                    }
                }else{
                    throw new DeleteConstrainException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.HallWArt, hallID), String.Format(Resources.ExceptionMessages.DeleteConstraintMessage, Resources.Resources.HallWArt,
                        Resources.Resources.Racks, Resources.Resources.RacksWArt));
                }
            }
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool addCategory(long hallID, long categoryID) {
            try {
                ProductCategory category = this.db.ProductCategories.Find(categoryID);
                if (category == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Hall);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Category, categoryID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                Hall hall = this.db.Halls.Find(hallID);
                if (hall == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Hall);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Hall, hallID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                if (!hall.ProductCategories.Contains(category)) {
                    hall.ProductCategories.Add(category);
                    this.db.SaveChanges();
                }
                return true;
            } catch (ForeignKeyConstraintException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Hall));
            }
        }

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool saveCategories(long hallID, List<long> categoryiesIDs) {
            try {
                Hall hall = this.db.Halls.Find(hallID);
                if (hall == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Hall);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Hall, hallID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                IEnumerable<ProductCategory> aux = hall.ProductCategories.ToList();
                foreach (ProductCategory category in aux) {
                    if (!categoryiesIDs.Contains(category.ProductCategoriesID)) {
                        hall.ProductCategories.Remove(category);
                    }
                }
                this.db.SaveChanges();
                string error = null;
                foreach (long id in categoryiesIDs) {
                    if (!hall.ProductCategories.Any(x => x.ProductCategoriesID == id)) {
                        ProductCategory category = this.db.ProductCategories.Find(id);
                        if (category == null) {
                            error = error + String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                                Resources.Resources.Category, id) + "\n";
                        }
                        hall.ProductCategories.Add(category);
                    }
                }
                this.db.SaveChanges();
                if (!String.IsNullOrWhiteSpace(error)) {
                    ModelErrorException ex = new ModelErrorException(error);
                    ex.AttributeName = "Categories";
                }
                return true;
            } catch (ForeignKeyConstraintException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Hall));
            }
        }

        public IQueryable<Hall> getAutocomplete(string search, long? dcID, long? depositID, long? sectorID) {
            try {
                var data = this.searchAndRank(search, dcID, depositID, sectorID);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        private IQueryable<Hall> searchAndRank(string search, long? dcID, long? depositID, long? sectorID) {
            IQueryable<Hall> data = this.getData();
            if (dcID.HasValue) {
                data = data.Where(x => x.Sector.Deposit.IDDistributionCenter == dcID.Value);
            }
            if (depositID.HasValue) {
                data = data.Where(x => x.Sector.IDDeposit == depositID.Value);
            }
            if (sectorID.HasValue) {
                data = data.Where(x => x.IDSector == sectorID.Value);
            }
            return this.searchAndRank(data, search);
        }

        private IQueryable<Hall> searchAndRank(IQueryable<Hall> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.Description);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<Hall> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<Hall> {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double) x.table.HallID).Trim() == item ? 1000 : 0) +
                                ((x.table.Description.Contains(item)) ? item.Length : 0) + ((x.table.Description.StartsWith(item)) ? (item.Length * 2) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.Description)
                    .Select(x => x.table);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool unnasignCategory(long hallID, long categoryID) {
            try {
                ProductCategory category = this.db.ProductCategories.Find(categoryID);
                if (category == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Hall);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Category, categoryID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                Hall hall = this.db.Halls.Find(hallID);
                if (hall == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Hall);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Hall, hallID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                if (hall.ProductCategories.Contains(category)) {
                    hall.ProductCategories.Remove(category);
                    this.db.SaveChanges();
                }
                return true;
            } catch (ForeignKeyConstraintException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Hall));
            }
        }

        public bool deleteCategories(long hallID) {
            try {
                Hall hall = this.db.Halls.Find(hallID);
                if (hall == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Hall);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Hall, hallID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                hall.ProductCategories.Clear();
                this.db.SaveChanges();
                return true;
            } catch (ForeignKeyConstraintException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Hall));
            }
        }

        private bool checkDeleteConstraints(Hall hall) {
            if (hall.Racks != null && hall.Racks.Count != 0) {
                return false;
            }
            return true;
        }

        private bool deleteRacks(Hall hall) {
            var tran = this.db.Database.BeginTransaction();
            if (hall.Racks.Any(x => x.Deleted == false)) {
                return false;
            }
            try {
                ICollection<Rack> list = new List<Rack>(hall.Racks);
                foreach (Rack rack in list) {
                    this.db.Racks.Remove(rack);
                }
                this.db.SaveChanges();
                tran.Commit();
                return true;
            }catch(Exception){
                tran.Rollback();
                return false;
            }
        }

        private bool fitsIntoDistributionCenter(decimal width, decimal depth, decimal oldSize, long sectorID) {
            Sector sector = this.db.Sectors.Find(sectorID);
            if (sector == null) {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.SectorWArt, sectorID));
                e.AttributeName = "SectorName";
                throw e;
            }
            if (sector.Width < width) {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.WidthWArt, sector.Width));
                e.AttributeName = "Width";
                throw e;
            }
            if (sector.Depth < depth) {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.DepthWArt, sector.Depth));
                e.AttributeName = "Depth";
                throw e;
            }
            decimal size = width * sector.Deposit.Height * depth; // xq el sector toma la altura del deposito en el que esta
            size = size - oldSize;
            if (sector.Size < sector.UsedSpace + size) {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.SizeWArt, (sector.Size - sector.UsedSpace)));
                e.AttributeName = "Size";
                throw e;
            }
            return true;
        }

        public IQueryable<Rack> getRacks(long HallID)
        {
            try
            {
                return this.db.Racks.Where(x => x.IDHall == HallID);
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