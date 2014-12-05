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
    public class RackRepository : IRackRepository
    {
        PenatesEntities db = new PenatesEntities();

        public int itemsPerPage { get; set; }

        public RackRepository()
        {
            this.itemsPerPage = 50;
        }

        public RackRepository(int itemsPerPage)
        {
            this.itemsPerPage = itemsPerPage;
        }

        /// <summary> Guarda los datos de un Rack en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Save(RackViewModel rack)
        {
            try
            {
                Rack aux = null;
                decimal oldSize = 0;
                if (rack.RackID != 0)
                {
                    aux = db.Racks.Find(rack.RackID);
                    if (aux != null)
                    {
                        oldSize = aux.Size.HasValue ? aux.Size.Value : 0;
                    }
                }
                if (!this.fitsIntoHall(rack.Width, rack.Depth, rack.Width, oldSize, rack.HallID.Value))
                {
                    return -1;
                }
                Nullable<long> val = null;
                if (aux == null)
                {
                    val = db.SP_Rack_Add(rack.Description, rack.Height, rack.Width, rack.Depth, rack.HallID, rack.RackCode, rack.ShelfsNumber, rack.DivitionsNumber).SingleOrDefault();
                }
                else
                {
                    int shelves = aux.Shelfs.Count;
                    int divitions = aux.Shelfs.FirstOrDefault().ShelfsSubdivisions.Count;
                    if (aux.Width != rack.Width || aux.Height != rack.Height || aux.Depth != rack.Depth ||
                        (rack.ShelfsNumber.HasValue && rack.ShelfsNumber.Value != shelves) || (rack.DivitionsNumber.HasValue && rack.DivitionsNumber.Value != divitions) ||
                        rack.HallID != aux.IDHall)
                    {
                        if (rack.ShelfsNumber.HasValue)
                        {
                            shelves = rack.ShelfsNumber.Value;
                        }
                        if (rack.DivitionsNumber.HasValue)
                        {
                            divitions = rack.DivitionsNumber.Value;
                        }
                        if (!this.checkDeleteConstraints(aux))
                        { //Si todavia tiene productos adentro
                            ModelErrorException excep = new ModelErrorException(Resources.Errors.EditRackError);
                            excep.AttributeName = "";
                            throw excep;
                        }
                        val = db.SP_Rack_Edit(rack.RackID, rack.Description, rack.Height, rack.Width, rack.Depth, rack.HallID, rack.RackCode, rack.ShelfsNumber, rack.DivitionsNumber).SingleOrDefault();
                    }
                    else
                    {
                        val = db.SP_Rack_LightEdit(rack.RackID, rack.Description, rack.RackCode).SingleOrDefault();
                    }

                }
                if (!val.HasValue)
                {
                    return -1;
                }
                return val.Value;
            }
            catch (ModelErrorException e)
            {
                throw e;
            }
            catch (Exception)
            {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.RackWArt));
            }
        }

        public Rack getRackInfo(long id)
        {
            try
            {
                Rack aux = db.Racks.Find(id);
                return aux;
            }
            catch (Exception e)
            {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<Rack> getData()
        {
            try
            {
                return this.db.Racks.Where(x => x.Deleted == false);
            }
            catch (Exception e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Rack> search(IQueryable<Rack> query, string search)
        {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<Rack> search(IQueryable<Rack> query, List<string> search)
        {
            try
            {
                foreach (string item in search)
                {
                    query = query.Where(p => p.Description.Contains(item) || p.RackCode.Contains(item) ||
                        SqlFunctions.StringConvert((double)p.RackID).Contains(item));
                }
                return query;
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

        public IQueryable<Rack> sort(IQueryable<Rack> query, Sorts sort)
        {
            switch (sort)
            { //Ordeno x lo que sea
                case Sorts.ID:
                    query = query.OrderBy(x => x.RackID);
                    break;
                case Sorts.ID_DESC:
                    query = query.OrderByDescending(x => x.RackID);
                    break;
                case Sorts.DESCRIPTION:
                    query = query.OrderBy(x => x.Description);
                    break;
                case Sorts.DESCRIPTION_DESC:
                    query = query.OrderByDescending(x => x.Description);
                    break;
                case Sorts.CODE:
                    query = query.OrderBy(x => x.RackCode);
                    break;
                case Sorts.CODE_DESC:
                    query = query.OrderByDescending(x => x.RackCode);
                    break;
                case Sorts.DISTRIBUTION_CENTER:
                    query = query.OrderBy(x => x.Hall.Sector.IDDeposit);
                    break;
                case Sorts.DISTRIBUTION_CENTER_DESC:
                    query = query.OrderByDescending(x => x.Hall.Sector.IDDeposit);
                    break;
                case Sorts.USEDPERCENT:
                    query = query.OrderBy(x => x.UsedSpace);
                    break;
                case Sorts.USEDPERCENT_DESC:
                    query = query.OrderByDescending(x => x.UsedSpace);
                    break;
                case Sorts.DEFAULT:
                    query = query.OrderBy(x => x.RackID);
                    break;
                default:
                    query = query.OrderBy(x => x.RackID);
                    break;
            }
            return query;
        }

        public IQueryable<Rack> filterByDistributionCenter(IQueryable<Rack> query, long dcID)
        {
            return query.Where(x => x.Hall.Sector.Deposit.IDDistributionCenter == dcID);
        }

        public IQueryable<Rack> filterByDeposit(IQueryable<Rack> query, long depositID)
        {
            return query.Where(x => x.Hall.Sector.IDDeposit == depositID);
        }

        public IQueryable<Rack> filterBySector(IQueryable<Rack> query, long sectorID)
        {
            return query.Where(x => x.Hall.IDSector == sectorID);
        }

        public IQueryable<Rack> filterByHall(IQueryable<Rack> query, long hallID)
        {
            return query.Where(x => x.IDHall == hallID);
        }

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        public bool Delete(long rackID)
        {
            Rack rack = this.db.Racks.Find(rackID);
            if (rack == null || rack.Deleted == true)
            {
                return true;
            }
            if (this.checkDeleteConstraints(rack))
            {
                var tran = this.db.Database.BeginTransaction();
                try
                {
                    rack.Deleted = true;
                    this.db.Racks.Attach(rack);
                    var entry = db.Entry(rack);
                    entry.Property(e => e.Deleted).IsModified = true;
                    Hall hall = this.db.Halls.Find(rack.IDHall);
                    if (rack.Size.HasValue)
                    {
                        hall.UsedSpace = hall.UsedSpace - rack.Size.Value;
                    }
                    this.db.Halls.Attach(hall);
                    var entry2 = db.Entry(hall);
                    entry2.Property(e => e.UsedSpace).IsModified = true;
                    db.SaveChanges();
                    this.deleteUsableSpace(rack);
                    this.deleteShelves(rack);
                    db.SaveChanges();
                    tran.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    throw new DeleteException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.RackWArt, rackID), e.Message);
                }
            }
            else
            {
                throw new DeleteConstrainException(String.Format(Resources.ExceptionMessages.DeleteException,
                    Resources.Resources.RackWArt, rackID),
                    String.Format(Resources.ExceptionMessages.DeleteConstraintMessage, Resources.Resources.RackWArt,
                    Resources.Resources.Products, Resources.Resources.ProductsWArt));
            }
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool addCategory(long rackID, long categoryID)
        {
            try
            {
                ProductCategory category = this.db.ProductCategories.Find(categoryID);
                if (category == null)
                {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Hall);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Category, categoryID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                Rack rack = this.db.Racks.Find(rackID);
                if (rack == null)
                {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Rack);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Rack, rackID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                if (!rack.ProductCategories.Contains(category))
                {
                    rack.ProductCategories.Add(category);
                    this.db.SaveChanges();
                }
                return true;
            }
            catch (ForeignKeyConstraintException e)
            {
                throw e;
            }
            catch (Exception)
            {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Hall));
            }
        }

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool saveCategories(long rackID, List<long> categoryiesIDs)
        {
            try
            {
                Rack rack = this.db.Racks.Find(rackID);
                if (rack == null)
                {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Rack);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Rack, rackID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                IEnumerable<ProductCategory> aux = rack.ProductCategories.ToList();
                foreach (ProductCategory category in aux)
                {
                    if (!categoryiesIDs.Contains(category.ProductCategoriesID))
                    {
                        rack.ProductCategories.Remove(category);
                    }
                }
                this.db.SaveChanges();
                string error = null;
                foreach (long id in categoryiesIDs)
                {
                    if (!rack.ProductCategories.Any(x => x.ProductCategoriesID == id))
                    {
                        ProductCategory category = this.db.ProductCategories.Find(id);
                        if (category == null)
                        {
                            error = error + String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                                Resources.Resources.Category, id) + "\n";
                        }
                        rack.ProductCategories.Add(category);
                    }
                }
                this.db.SaveChanges();
                if (!String.IsNullOrWhiteSpace(error))
                {
                    ModelErrorException ex = new ModelErrorException(error);
                    ex.AttributeName = "Categories";
                }
                return true;
            }
            catch (ForeignKeyConstraintException e)
            {
                throw e;
            }
            catch (Exception)
            {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Rack));
            }
        }

        public IQueryable<Rack> getAutocomplete(string search, long? dcID, long? depositID, long? sectorID, long? hallID)
        {
            try
            {
                var data = this.searchAndRank(search, dcID, depositID, sectorID, hallID);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
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

        private IQueryable<Rack> searchAndRank(string search, long? dcID, long? depositID, long? sectorID, long? hallID)
        {
            IQueryable<Rack> data = this.getData();
            if (dcID.HasValue)
            {
                data = data.Where(x => x.Hall.Sector.Deposit.IDDistributionCenter == dcID.Value);
            }
            if (depositID.HasValue)
            {
                data = data.Where(x => x.Hall.Sector.IDDeposit == depositID.Value);
            }
            if (sectorID.HasValue)
            {
                data = data.Where(x => x.Hall.IDSector == sectorID.Value);
            }
            if (hallID.HasValue)
            {
                data = data.Where(x => x.IDHall == hallID.Value);
            }
            return this.searchAndRank(data, search);
        }

        private IQueryable<Rack> searchAndRank(IQueryable<Rack> data, string search)
        {
            try
            {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search))
                {
                    data = data.OrderBy(x => x.Description);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<Rack>
                {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches)
                {
                    aux = aux.Select(x => new PageRankItem<Rack>
                    {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double)x.table.RackID).Trim() == item ? 10000 : 0) +
                                ((x.table.RackCode.Contains(item)) ? item.Length * 4 : 0) + ((x.table.RackCode.StartsWith(item)) ? (item.Length * 8) : 0) +
                                ((x.table.Description.Contains(item)) ? item.Length : 0) + ((x.table.Description.StartsWith(item)) ? (item.Length * 2) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.RackCode)
                    .Select(x => x.table);
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

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool unnasignCategory(long rackID, long categoryID)
        {
            try
            {
                ProductCategory category = this.db.ProductCategories.Find(categoryID);
                if (category == null)
                {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Hall);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Category, categoryID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                Rack rack = this.db.Racks.Find(rackID);
                if (rack == null)
                {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Rack);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Rack, rackID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                if (rack.ProductCategories.Contains(category))
                {
                    rack.ProductCategories.Remove(category);
                    this.db.SaveChanges();
                }
                return true;
            }
            catch (ForeignKeyConstraintException e)
            {
                throw e;
            }
            catch (Exception)
            {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Rack));
            }
        }

        public bool deleteCategories(long rackID)
        {
            try
            {
                Rack rack = this.db.Racks.Find(rackID);
                if (rack == null)
                {
                    string title = String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Rack);
                    string message = String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException, Resources.Resources.Rack, rackID);
                    throw new ForeignKeyConstraintException(title, message);
                }
                rack.ProductCategories.Clear();
                this.db.SaveChanges();
                return true;
            }
            catch (ForeignKeyConstraintException e)
            {
                throw e;
            }
            catch (Exception)
            {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.AddException, Resources.Resources.Category,
                        Resources.Resources.Rack));
            }
        }

        private bool checkDeleteConstraints(Rack rack)
        {
            foreach (Shelf shelf in rack.Shelfs)
            {
                foreach (ShelfsSubdivision shelfSub in shelf.ShelfsSubdivisions)
                {
                    if (shelfSub.Containers != null && shelfSub.Containers.Count != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool fitsIntoHall(decimal width, decimal depth, decimal height, decimal oldSize, long hallID)
        {
            Hall hall = this.db.Halls.Find(hallID);
            if (hall == null)
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.HallWArt, hallID));
                e.AttributeName = "HallName";
                throw e;
            }
            if (hall.Width < width)
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.WidthWArt, hall.Width));
                e.AttributeName = "Width";
                throw e;
            }
            if (hall.Depth < depth)
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.DepthWArt, hall.Depth));
                e.AttributeName = "Depth";
                throw e;
            }
            decimal size = width * height * depth; // xq el sector toma la altura del deposito en el que esta
            size = size - oldSize;
            if (hall.Size < hall.UsedSpace + size)
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.SizeWArt, (hall.Size-hall.UsedSpace)));
                e.AttributeName = "Size";
                throw e;
            }
            return true;
        }

        private bool deleteUsableSpace(Rack rack)
        {
            Hall hall = this.db.Halls.Find(rack.IDHall);
            hall.UsableSpace = rack.Size.HasValue ? hall.UsableSpace - rack.Size.Value : hall.UsableSpace;
            this.db.Halls.Attach(hall);
            var entry = db.Entry(hall);
            entry.Property(e => e.UsableSpace).IsModified = true;

            Sector sector = this.db.Sectors.Find(hall.IDSector);
            sector.UsableSpace = rack.Size.HasValue ? sector.UsableSpace - rack.Size.Value : sector.UsableSpace;
            this.db.Sectors.Attach(sector);
            var entry2 = db.Entry(hall);
            entry2.Property(e => e.UsableSpace).IsModified = true;

            Deposit depo = this.db.Deposits.Find(sector.IDDeposit);
            depo.UsableSpace = rack.Size.HasValue ? depo.UsableSpace - rack.Size.Value : depo.UsableSpace;
            this.db.Deposits.Attach(depo);
            var entry3 = db.Entry(depo);
            entry3.Property(e => e.UsableSpace).IsModified = true;

            DistributionCenter dc = this.db.DistributionCenters.Find(depo.IDDistributionCenter);
            dc.UsableSpace = rack.Size.HasValue ? dc.UsableSpace - rack.Size.Value : dc.UsableSpace;
            this.db.DistributionCenters.Attach(dc);
            var entry4 = db.Entry(dc);
            entry4.Property(e => e.UsableSpace).IsModified = true;
            return true;

        }


        public void updateUsedSpace(long ShelfSubdivisionID, decimal UsedSpaceInCm)
        {

            decimal UsedSpaceInM = SizeUtils.fromCm3ToM3(UsedSpaceInCm);

            ShelfsSubdivision shelfSubdivision = this.db.ShelfsSubdivisions.Find(ShelfSubdivisionID);
            shelfSubdivision.UsedSpace = shelfSubdivision.UsedSpace + UsedSpaceInM;
            this.db.ShelfsSubdivisions.Attach(shelfSubdivision);
            var entry0 = db.Entry(shelfSubdivision);
            entry0.Property(e => e.UsedSpace).IsModified = true;

            Shelf shelf = this.db.Shelfs.Find(shelfSubdivision.IDShelf);
            shelf.UsedSpace = shelf.UsedSpace + UsedSpaceInM;
            this.db.Shelfs.Attach(shelf);
            var entry6 = db.Entry(shelf);
            entry6.Property(e => e.UsedSpace).IsModified = true; 
            
            Rack rack = this.db.Racks.Find(shelf.IDRack);
            rack.UsedSpace = rack.UsedSpace + UsedSpaceInM;
            this.db.Racks.Attach(rack);
            var entry = db.Entry(rack);
            entry.Property(e => e.UsedSpace).IsModified = true;
            
            Hall hall = this.db.Halls.Find(rack.IDHall);
            hall.UsedUsableSpace = hall.UsedUsableSpace + UsedSpaceInM;
            this.db.Halls.Attach(hall);
            var entry2 = db.Entry(hall);
            entry2.Property(e => e.UsedUsableSpace).IsModified = true;

            Sector sector = this.db.Sectors.Find(hall.IDSector);
            sector.UsedUsableSpace = sector.UsedUsableSpace + UsedSpaceInM;
            this.db.Sectors.Attach(sector);
            var entry3 = db.Entry(hall);
            entry3.Property(e => e.UsedUsableSpace).IsModified = true;

            Deposit depo = this.db.Deposits.Find(sector.IDDeposit);
            depo.UsedUsableSpace = depo.UsedUsableSpace + UsedSpaceInM;  
            this.db.Deposits.Attach(depo);
            var entry4 = db.Entry(depo);
            entry4.Property(e => e.UsedUsableSpace).IsModified = true;

            DistributionCenter dc = this.db.DistributionCenters.Find(depo.IDDistributionCenter);
            dc.UsableUsedSpace = dc.UsableUsedSpace + UsedSpaceInM;
            this.db.DistributionCenters.Attach(dc);
            var entry5 = db.Entry(dc);
            entry5.Property(e => e.UsableUsedSpace).IsModified = true;

            db.SaveChanges(); 
        }

        public decimal getShelfSubdivisionSize(long ShelfSubdivisionID)
        {
            try
            {
                return this.db.ShelfsSubdivisions.Find(ShelfSubdivisionID).Size;
            }
            catch (ForeignKeyConstraintException e)
            {
                throw e;
            }
            catch (Exception)
            {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.InvalidIdSearch, ShelfSubdivisionID));
            }
        }

        public decimal getShelfSubdivisionUsedSpace(long ShelfSubdivisionID)
        {
            try
            {
                return this.db.ShelfsSubdivisions.Find(ShelfSubdivisionID).UsedSpace;
            }
            catch (ForeignKeyConstraintException e)
            {
                throw e;
            }
            catch (Exception)
            {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.InvalidIdSearch, ShelfSubdivisionID));
            }
        }




        private void deleteShelves(Rack rack)
        {
            ICollection<Shelf> remove = new List<Shelf>(rack.Shelfs);
            rack.Shelfs.Clear();
            foreach (Shelf shelf in remove)
            {
                this.db.Shelfs.Remove(shelf);
            }
        }
    }
}