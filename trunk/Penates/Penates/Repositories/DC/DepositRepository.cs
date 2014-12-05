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
    public class DepositRepository : IDepositRepository {

        PenatesEntities db = new PenatesEntities();

        public int itemsPerPage {get;set;}

        public DepositRepository() {
            this.itemsPerPage = 50;
        }

        public DepositRepository(int itemsPerPage) {
            this.itemsPerPage = itemsPerPage;
        }

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Save(DepositViewModel deposit) {
            try {
                Deposit aux = null;
                decimal oldSize = 0;
                if (deposit.DepositID != 0) {
                    aux = db.Deposits.Find(deposit.DepositID);
                    deposit.DistributionCenterID = aux.IDDistributionCenter;
                    oldSize = aux.Width * aux.Height * aux.Depth;
                }
                Nullable<long> val = null;
                if (!this.fitsIntoDistributionCenter(deposit.Floor, deposit.Width, deposit.Height, deposit.Depth, oldSize, deposit.DistributionCenterID)) {
                    return -1;
                }
                if (aux == null) {
                    val = db.SP_Deposit_Add(deposit.Description, deposit.Height, deposit.Width, deposit.Depth,
                        deposit.DistributionCenterID, deposit.Floor).SingleOrDefault();
                } else {
                    val = db.SP_Deposit_Edit(deposit.DepositID, deposit.Description, deposit.Height, deposit.Width,
                        deposit.Depth, deposit.Floor).SingleOrDefault();
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

        public Deposit getDepositInfo(long id) {
            try {
                Deposit aux = db.Deposits.Find(id);
                return aux;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<Deposit> getData(long dcID) {
            try {
                return this.db.Deposits.Where(p => p.IDDistributionCenter == dcID);
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Deposit> search(IQueryable<Deposit> query, string search) {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<Deposit> search(IQueryable<Deposit> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Description.Contains(item) || SqlFunctions.StringConvert((double) p.DepositID).Contains(item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Deposit> sort(IQueryable<Deposit> query, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ID:
                    query = query.OrderBy(x => x.DepositID);
                    break;
                case Sorts.ID_DESC:
                    query = query.OrderByDescending(x => x.DepositID);
                    break;
                case Sorts.DESCRIPTION:
                    query = query.OrderBy(x => x.Description);
                    break;
                case Sorts.DESCRIPTION_DESC:
                    query = query.OrderByDescending(x => x.Description);
                    break;
                case Sorts.FLOOR:
                    query = query.OrderBy(x => x.Floor);
                    break;
                case Sorts.FLOOR_DESC:
                    query = query.OrderByDescending(x => x.Floor);
                    break;
                case Sorts.USEDPERCENT:
                    query = query.OrderBy(x => x.UsedSpace);
                    break;
                case Sorts.USEDPERCENT_DESC:
                    query = query.OrderByDescending(x => x.UsedSpace);
                    break;
                case Sorts.DEFAULT:
                    query = query.OrderBy(x => x.DepositID);
                    break;
                default:
                    query = query.OrderBy(x => x.DepositID);
                    break;
            }
            return query;
        }

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        public bool Delete(long depositID) {
            Deposit depo = this.db.Deposits.Find(depositID);
            if (depo == null) {
                return true;
            }
            if (this.checkDeleteConstraints(depo)) {
                var tran = this.db.Database.BeginTransaction();
                try {
                    db.Deposits.Remove(depo);
                    if(!depo.Size.HasValue){
                        depo.Size = depo.Width * depo.Height * depo.Depth;
                    }
                    InternalDistributionCenter idc = (InternalDistributionCenter)this.db.DistributionCenters.Find(depo.IDDistributionCenter);
                    idc.UsedSpace = idc.UsedSpace - depo.Size.Value;
                    this.db.DistributionCenters.Attach(idc);
                    var entry = db.Entry(idc);
                    entry.Property(e => e.UsedSpace).IsModified = true;
                    db.SaveChanges();
                    tran.Commit();
                    return true;
                } catch (Exception e) {
                    tran.Rollback();
                    throw new DeleteException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.DepositWArt, depositID), e.Message);
                }
            } else {
                throw new DeleteConstrainException(String.Format(Resources.ExceptionMessages.DeleteException,
                    Resources.Resources.DepositWArt, depositID),
                    String.Format(Resources.ExceptionMessages.DeleteConstraintMessage, Resources.Resources.DepositWArt,
                    Resources.Resources.Racks, Resources.Resources.RacksWArt));
            }
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool addCategory(long depositID, long categoryID) {
            try {
                ProductCategory category = this.db.ProductCategories.Find(categoryID);
                if (category == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Deposit);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Category, categoryID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                Deposit deposit = this.db.Deposits.Find(depositID);
                if (deposit == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Deposit);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Deposit, depositID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                if (!deposit.ProductCategories.Contains(category)) {
                    deposit.ProductCategories.Add(category);
                    this.db.SaveChanges();
                }
                return true;
            } catch (ForeignKeyConstraintException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Deposit));
            }
        }

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool saveCategories(long depositID, List<long> categoryiesIDs) {
            try {
                Deposit deposit = this.db.Deposits.Find(depositID);
                if (deposit == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Deposit);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Deposit, depositID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                IEnumerable<ProductCategory> aux = deposit.ProductCategories.ToList();
                foreach(ProductCategory category in aux){
                    if (!categoryiesIDs.Contains(category.ProductCategoriesID)) {
                        deposit.ProductCategories.Remove(category);
                    }
                }
                this.db.SaveChanges();
                string error = null;
                foreach (long id in categoryiesIDs) {
                    if (!deposit.ProductCategories.Any(x => x.ProductCategoriesID == id)) {
                        ProductCategory category = this.db.ProductCategories.Find(id);
                        if (category == null) {
                            error = error + String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                                Resources.Resources.Category, id) + "\n";
                        }
                        deposit.ProductCategories.Add(category);
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
                        Resources.Resources.Deposit));
            }
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool unnasignCategory(long depositID, long categoryID) {
            try {
                ProductCategory category = this.db.ProductCategories.Find(categoryID);
                if (category == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Deposit);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Category, categoryID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                Deposit deposit = this.db.Deposits.Find(depositID);
                if (deposit == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Deposit);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Deposit, depositID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                if (deposit.ProductCategories.Contains(category)) {
                    deposit.ProductCategories.Remove(category);
                    this.db.SaveChanges();
                }
                return true;
            } catch (ForeignKeyConstraintException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Deposit));
            }
        }

        public bool deleteCategories(long depositID) {
            try {
                Deposit depo = this.db.Deposits.Find(depositID);
                if (depo == null) {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Deposit);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Deposit, depositID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                depo.ProductCategories.Clear();
                this.db.SaveChanges();
                return true;
            } catch (ForeignKeyConstraintException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Sector));
            }
        }

        public IQueryable<Deposit> getAutocomplete(string search, long? dcID) {
            try {
                var data = this.searchAndRank(search, dcID);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        private IQueryable<Deposit> searchAndRank(string search, long? dcID) {
            IQueryable<Deposit> data = this.db.Deposits;
            if (dcID.HasValue) {
                data = data.Where(x => x.IDDistributionCenter == dcID.Value);
            }
            return this.searchAndRank(data, search);
        }

        private IQueryable<Deposit> searchAndRank(IQueryable<Deposit> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.Description);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<Deposit> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<Deposit> {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double) x.table.DepositID).Trim() == item ? 1000 : 0) +
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
        
        private bool checkDeleteConstraints(Deposit depo) {
            foreach (Sector sector in depo.Sectors) {
                foreach (Hall hall in sector.Halls) {
                    if (hall.Racks != null && hall.Racks.Count != 0) {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool fitsIntoDistributionCenter(int floor, decimal width, decimal height, decimal depth, decimal oldSize, long dcID) {
            IInternalDistributionCenterRepository repo = new InternalDistributionCenterRepository();
            InternalDistributionCenter idc = repo.getData(dcID);
            if(idc == null){
                ModelErrorException e = new ModelErrorException(Resources.FormsErrors.DistributionCenterValid);
                e.AttributeName = "DistributionCenterID";
                throw e;
            }
            if (idc.Floors < floor) {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.FloorTooBig,
                    Resources.Resources.DistributionCenterWArt, idc.Floors));
                e.AttributeName = "Floor";
                throw e;
            }
            if (idc.Width < width) {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.WidthWArt, idc.Width));
                e.AttributeName = "Width";
                throw e;
            }
            if(idc.Height < height){
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.HeightWArt, idc.Height));
                e.AttributeName = "Height";
                throw e;
            }
            if (idc.Depth < depth) {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.DepthWArt, idc.Depth));
                e.AttributeName = "Depth";
                throw e;
            }
            decimal size = width * height * depth;
            size = size - oldSize;
            if (idc.Capacity < idc.UsedSpace + size) {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.SizeWArt, (idc.Capacity-idc.UsedSpace)));
                e.AttributeName = "Size";
                throw e;
            }
            return true;
        }


        public IQueryable<Sector> getSectors(long DepositID)
        {
            try
            {
                return this.db.Sectors.Where(x => x.IDDeposit == DepositID);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }            
        }
    }
}