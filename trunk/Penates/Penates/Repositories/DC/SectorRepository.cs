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
    public class SectorRepository : ISectorRepository {

        PenatesEntities db = new PenatesEntities();

        public int itemsPerPage {get;set;}

        public SectorRepository() {
            this.itemsPerPage = 50;
        }

        public SectorRepository(int itemsPerPage) {
            this.itemsPerPage = itemsPerPage;
        }

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Save(SectorViewModel sector) {
            try {
                Sector aux = null;
                decimal oldSize = 0;
                if (sector.SectorID != 0) {
                    aux = db.Sectors.Find(sector.SectorID);
                    if (aux != null) {
                        sector.DepositID = aux.IDDeposit;
                        oldSize = aux.Size;
                    }
                }
                if (!this.fitsIntoDistributionCenter(sector.Width, sector.Depth, oldSize, sector.DepositID.Value)) {
                    return -1;
                }
                Nullable<long> val = null;
                if (aux == null) {
                    val = db.SP_Sector_Add(sector.Description, sector.Width, sector.Depth, sector.DepositID).SingleOrDefault();
                } else {
                    val = db.SP_Sector_Edit(sector.SectorID, sector.Description, sector.Width, sector.Depth).SingleOrDefault();
                }
                if (!val.HasValue) {
                    return -1;
                }
                return val.Value;
            }catch(ModelErrorException e){
                throw e;
            }catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.DepositWArt));
            }
        }

        public Sector getSectorInfo(long id) {
            try {
                Sector aux = db.Sectors.Find(id);
                return aux;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<Sector> getData() {
            try {
                return this.db.Sectors;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Sector> search(IQueryable<Sector> query, string search) {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<Sector> search(IQueryable<Sector> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Description.Contains(item) || SqlFunctions.StringConvert((double) p.SectorID).Contains(item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Sector> sort(IQueryable<Sector> query, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ID:
                    query = query.OrderBy(x => x.SectorID);
                    break;
                case Sorts.ID_DESC:
                    query = query.OrderByDescending(x => x.SectorID);
                    break;
                case Sorts.DESCRIPTION:
                    query = query.OrderBy(x => x.Description);
                    break;
                case Sorts.DESCRIPTION_DESC:
                    query = query.OrderByDescending(x => x.Description);
                    break;
                case Sorts.DISTRIBUTION_CENTER:
                    query = query.OrderBy(x => x.Deposit.IDDistributionCenter);
                    break;
                case Sorts.DISTRIBUTION_CENTER_DESC:
                    query = query.OrderByDescending(x => x.Deposit.IDDistributionCenter);
                    break;
                case Sorts.USEDPERCENT:
                    query = query.OrderBy(x => x.UsedSpace);
                    break;
                case Sorts.USEDPERCENT_DESC:
                    query = query.OrderByDescending(x => x.UsedSpace);
                    break;
                case Sorts.DEFAULT:
                    query = query.OrderBy(x => x.SectorID);
                    break;
                default:
                    query = query.OrderBy(x => x.SectorID);
                    break;
            }
            return query;
        }

        public IQueryable<Sector> filterByDistributionCenter(IQueryable<Sector> query, long dcID) {
            return query.Where(x => x.Deposit.IDDistributionCenter == dcID);
        }

        public IQueryable<Sector> filterByDeposit(IQueryable<Sector> query, long depositID) {
            return query.Where(x => x.IDDeposit == depositID);
        }

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        public bool Delete(long sectorID) {
            Sector sector = this.db.Sectors.Find(sectorID);
            if (sector == null) {
                return true;
            }
            if (this.checkDeleteConstraints(sector)) {
                var tran = this.db.Database.BeginTransaction();
                try {
                    db.Sectors.Remove(sector);
                    Deposit deposit = this.db.Deposits.Find(sector.IDDeposit);
                    deposit.UsedSpace = deposit.UsedSpace - sector.Size;
                    this.db.Deposits.Attach(deposit);
                    var entry = db.Entry(deposit);
                    entry.Property(e => e.UsedSpace).IsModified = true;
                    db.SaveChanges();
                    tran.Commit();
                    return true;
                } catch (Exception e) {
                    tran.Rollback();
                    throw new DeleteException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.SectorWArt, sectorID), e.Message);
                }
            } else {
                throw new DeleteConstrainException(String.Format(Resources.ExceptionMessages.DeleteException,
                    Resources.Resources.SectorWArt, sectorID),
                    String.Format(Resources.ExceptionMessages.DeleteConstraintMessage, Resources.Resources.SectorWArt,
                    Resources.Resources.Racks, Resources.Resources.RacksWArt));
            }
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool addCategory(long sectorID, long categoryID) {
            try {
                ProductCategory category = this.db.ProductCategories.Find(categoryID);
                if (category == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Sector);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Category, categoryID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                Sector sector = this.db.Sectors.Find(sectorID);
                if (sector == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Sector);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Sector, sectorID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                if (!sector.ProductCategories.Contains(category)) {
                    sector.ProductCategories.Add(category);
                    this.db.SaveChanges();
                }
                return true;
            } catch (ForeignKeyConstraintException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Sector));
            }
        }

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool saveCategories(long sectorID, List<long> categoryiesIDs) {
            try {
                Sector sector = this.db.Sectors.Find(sectorID);
                if (sector == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Sector);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Sector, sectorID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                IEnumerable<ProductCategory> aux = sector.ProductCategories.ToList();
                foreach(ProductCategory category in aux){
                    if (!categoryiesIDs.Contains(category.ProductCategoriesID)) {
                        sector.ProductCategories.Remove(category);
                    }
                }
                this.db.SaveChanges();
                string error = null;
                foreach (long id in categoryiesIDs) {
                    if (!sector.ProductCategories.Any(x => x.ProductCategoriesID == id)) {
                        ProductCategory category = this.db.ProductCategories.Find(id);
                        if (category == null) {
                            error = error + String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                                Resources.Resources.Category, id) + "\n";
                        }
                        sector.ProductCategories.Add(category);
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
                        Resources.Resources.Sector));
            }
        }

        public IQueryable<Sector> getAutocomplete(string search, long? dcID, long? depositID) {
            try {
                var data = this.searchAndRank(search, dcID, depositID);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        private IQueryable<Sector> searchAndRank(string search, long? dcID, long? depositID) {
            IQueryable<Sector> data = this.getData();
            if (dcID.HasValue) {
                data = data.Where(x => x.Deposit.IDDistributionCenter == dcID.Value);
            }
            if (depositID.HasValue) {
                data = data.Where(x => x.IDDeposit == depositID.Value);
            }
            return this.searchAndRank(data, search);
        }

        private IQueryable<Sector> searchAndRank(IQueryable<Sector> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.Description);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<Sector> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<Sector> {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double) x.table.SectorID).Trim() == item ? 1000 : 0) +
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
        public bool unnasignCategory(long sectorID, long categoryID) {
            try {
                ProductCategory category = this.db.ProductCategories.Find(categoryID);
                if (category == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Sector);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Category, categoryID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                Sector sector = this.db.Sectors.Find(sectorID);
                if (sector == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Sector);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Sector, sectorID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                if (sector.ProductCategories.Contains(category)) {
                    sector.ProductCategories.Remove(category);
                    this.db.SaveChanges();
                }
                return true;
            } catch (ForeignKeyConstraintException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Sector));
            }
        }

        public bool deleteCategories(long sectorID) {
            try {
                Sector sector = this.db.Sectors.Find(sectorID);
                if (sector == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Sector);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Sector, sectorID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                sector.ProductCategories.Clear();
                this.db.SaveChanges();
                return true;
            } catch (ForeignKeyConstraintException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Sector));
            }
        }
        
        private bool checkDeleteConstraints(Sector sector) {
            foreach (Hall hall in sector.Halls) {
                if (hall.Racks != null && hall.Racks.Count != 0) {
                    return false;
                }
            }
            return true;
        }

        private bool fitsIntoDistributionCenter(decimal width, decimal depth, decimal oldSize, long depositID) {
            Deposit deposit = this.db.Deposits.Find(depositID);
            if (deposit == null) {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.DepositWArt, depositID));
                e.AttributeName = "DepositName";
                throw e;
            }
            if (deposit.Width < width) {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.WidthWArt, deposit.Width));
                e.AttributeName = "Width";
                throw e;
            }
            if (deposit.Depth < depth) {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.DepthWArt, deposit.Depth));
                e.AttributeName = "Depth";
                throw e;
            }
            decimal size = width * deposit.Height * depth; // xq el sector toma la altura del deposito en el que esta
            size = size - oldSize;
            if (deposit.Size < deposit.UsedSpace + size) {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.SizeWArt, (deposit.Size-deposit.UsedSpace)));
                e.AttributeName = "Size";
                throw e;
            }
            return true;
        }

        public IQueryable<Hall> getHalls(long SectorID)
        {
            try
            {
                return this.db.Halls.Where(x => x.IDSector == SectorID);
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