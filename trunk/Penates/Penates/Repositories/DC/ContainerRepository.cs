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

namespace Penates.Repositories.DC
{
    public class ContainerRepository : IContainerRepository
    {
        PenatesEntities db = new PenatesEntities();

        public int itemsPerPage {get;set;}

        public ContainerRepository() {
            this.itemsPerPage = 50;
        }

        public ContainerRepository(int itemsPerPage)
        {
            this.itemsPerPage = itemsPerPage;
        }

        /// <summary> Guarda los datos de un Container en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del container</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Save(ContainerViewModel container) {
            try {
                if (container.ContainerID == 0) {
                    return 0;
                }
                Container aux = db.Containers.Find(container.ContainerID);
                Nullable<long> val = null;
                if (aux == null) {
                    if (!this.fitsIntoAvaliableSpace(container.ContainerTypeID, null, container.TemporaryDepositID, container.IDShelfSubdivision))
                    {
                        return -5;
                    }
                    val = db.SP_Container_Add(container.Code, container.ContainerTypeID, container.TemporaryDepositID, null).SingleOrDefault();
                } else {
                    if (!this.fitsIntoAvaliableSpace(container.ContainerTypeID, aux.ContainerType, aux.IDTemporalDeposit, aux.IDShelfSubdivition))
                    {
                        return -5;
                    }
                    val = db.SP_Container_Edit(container.Code, container.ContainerTypeID, aux.IDTemporalDeposit, aux.IDShelfSubdivition, container.ContainerID).SingleOrDefault();
                }
                if (!val.HasValue) {
                    return -1;
                }
                return val.Value;
            } catch (ModelErrorException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.ContainerWArt));
            }
        }

        public Container getContainerInfo(long id)
        {
            try {
                Container aux = db.Containers.Find(id);
                return aux;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<Container> getData()
        {
            try {
                return this.db.Containers.Where(x => x.ContainerID != 0 && x.IDContainerTypes != 0);
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Container> search(IQueryable<Container> query, string search)
        {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<Container> search(IQueryable<Container> query, List<string> search)
        {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Code.Contains(item) || p.ContainerType.Description.Contains(item) ||
                        SqlFunctions.StringConvert((double)p.ContainerID).Contains(item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Container> sort(IQueryable<Container> query, Sorts sort)
        {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ID:
                    query = query.OrderBy(x => x.ContainerID);
                    break;
                case Sorts.ID_DESC:
                    query = query.OrderByDescending(x => x.ContainerID);
                    break;
                case Sorts.CONTAINER_TYPE:
                    query = query.OrderBy(x => x.IDContainerTypes);
                    break;
                case Sorts.CONTAINER_TYPE_DESC:
                    query = query.OrderByDescending(x => x.IDContainerTypes);
                    break;
                case Sorts.TEMPORARY_DEPOSIT:
                    query = query.OrderBy(x => x.IDTemporalDeposit);
                    break;
                case Sorts.TEMPORARY_DEPOSIT_DESC:
                    query = query.OrderByDescending(x => x.IDTemporalDeposit);
                    break;
                case Sorts.SHELF_SUBDIVISION:
                    query = query.OrderBy(x => x.IDShelfSubdivition);
                    break;
                case Sorts.SHELF_SUBDIVISION_DESC:
                    query = query.OrderByDescending(x => x.IDShelfSubdivition);
                    break;
                case Sorts.USEDSPACE:
                    query = query.OrderBy(x => x.UsedSpace);
                    break;
                case Sorts.USEDSPACE_DESC:
                    query = query.OrderByDescending(x => x.UsedSpace);
                    break;
                case Sorts.DEFAULT:
                    query = query.OrderBy(x => x.ContainerID);
                    break;
                default:
                    query = query.OrderBy(x => x.ContainerID);
                    break;
            }
            return query;
        }


        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        public bool Delete(long ContainerID)
        {
            if (ContainerID == 0) {
                return false;
            }
            Container container = this.db.Containers.Find(ContainerID);
            if (container == null)
            {
                return true;
            }
            if (this.checkDeleteConstraints(container))
            {
                var tran = this.db.Database.BeginTransaction();
                try {
                    db.Containers.Remove(container);
                    if (container.IDShelfSubdivition.HasValue)
                    {
                        ShelfsSubdivision shelf = this.db.ShelfsSubdivisions.Find(container.IDShelfSubdivition.Value);
                        if (shelf != null)
                        {
                            shelf.UsedSpace = shelf.UsedSpace - container.ContainerType.Size.Value;
                            this.db.ShelfsSubdivisions.Attach(shelf);
                            var entry = db.Entry(shelf);
                            entry.Property(e => e.UsedSpace).IsModified = true;
                        }
                    }
                    else
                    {
                        if(container.IDTemporalDeposit.HasValue){
                            TemporaryDeposit td = this.db.TemporaryDeposits.Find(container.IDTemporalDeposit.Value);
                            if (td != null)
                            {
                                td.UsedSpace = td.UsedSpace - container.ContainerType.Size.Value;
                                this.db.TemporaryDeposits.Attach(td);
                                var entry = db.Entry(td);
                                entry.Property(e => e.UsedSpace).IsModified = true;
                            }
                        }
                    }
                    db.SaveChanges();
                    tran.Commit();
                    return true;
                } catch (Exception e) {
                    tran.Rollback();
                    throw new DeleteException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.ContainerWArt, ContainerID), e.Message);
                }
            } else {
                throw new DeleteConstrainException(String.Format(Resources.ExceptionMessages.DeleteException,
                    Resources.Resources.ContainerWArt, ContainerID),
                    String.Format(Resources.ExceptionMessages.DeleteConstraintMessage, Resources.Resources.SectorWArt,
                    Resources.Resources.Boxes, Resources.Resources.BoxesWArt));
            }
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public IQueryable<Container> getAutocomplete(string search, long? dcID, long? depositID, long? temporaryDepositID, long? sectorID, long? rackID, long? typeID)
        {
            try {
                var data = this.searchAndRank(search, dcID, depositID, temporaryDepositID, sectorID, rackID, typeID);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        private IQueryable<Container> searchAndRank(string search, long? dcID, long? depositID, long? temporaryDepositID, long? sectorID, long? rackID, long? typeID)
        {
            IQueryable<Container> data = this.getData();
            if (dcID.HasValue) {
                data = this.filterByDistributionCenter(data, dcID.Value);
            }
            if (depositID.HasValue) {
                data = this.filterByDeposit(data, depositID.Value);
            }
            if (temporaryDepositID.HasValue) {
                data = this.filterByTemporaryDeposit(data, temporaryDepositID.Value);
            }
            if (sectorID.HasValue) {
                data = this.filterBySector(data, sectorID.Value);
            }
            if (rackID.HasValue) {
                data = this.filterByRack(data, rackID.Value);
            }
            if (typeID.HasValue) {
                data = this.filterByType(data, typeID.Value);
            }
            return this.searchAndRank(data, search);
        }

        private IQueryable<Container> searchAndRank(IQueryable<Container> data, string search)
        {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.Code);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<Container>
                {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<Container>
                    {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double)x.table.ContainerID).Trim() == item ? 1000 : 0) +
                                ((x.table.Code.Contains(item)) ? item.Length : 0) + ((x.table.Code.StartsWith(item)) ? (item.Length * 2) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.Code)
                    .Select(x => x.table);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }


        private bool checkDeleteConstraints(Container container)
        {
            if (container.InternalBoxes != null && container.InternalBoxes.Count != 0)
            {
                return false;
            }
            return true;
        }

        private bool fitsIntoAvaliableSpace(long typeID, ContainerType oldType, long? depositID, long? shelfID)
        {
            decimal oldSize = 0;  // incializa en 0 tamaño del container anterior por si no es cambio.
            if (oldType != null)
            {
                if (oldType.Size.HasValue) {
                    oldSize = oldType.Size.Value;
                }else{
                    oldSize = oldType.Width * oldType.Height * oldType.Depth; // el tamaño del Container anterior
                }
            }

            if (depositID.HasValue)     // Verifica si puede colocarlo en un Deposito Temporal
            {
                return this.fitsIntoTempDepo(typeID, oldSize, depositID.Value);
            }
            else
            {
                if (shelfID.HasValue)   // Verifica si puede colocarlo en una Disvision de Estante
                {
                    return this.fitsIntoShelfSubdiv(typeID, oldSize, shelfID.Value);
                }else{
                    return false;
                }
            }
        }

        private bool fitsIntoTempDepo(long typeID, decimal oldSize, long depositID)
        {
            TemporaryDeposit depo = this.db.TemporaryDeposits.Find(depositID);   // Ubica el Deposito Temporal
            if (depo == null)
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.DepositWArt, depositID));
                e.AttributeName = "error";
                throw e;
            }

            ContainerType type = this.db.ContainerTypes.Find(typeID);
            if (type == null)
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.ContainerTypeWArt, typeID));
                e.AttributeName = "error";
                throw e;
            }


            if (SizeUtils.fromMToCm(depo.Width) < type.Width)
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.WidthWArt, SizeUtils.fromMToCm(depo.Width)));
                e.AttributeName = "Width";
                throw e;
            }
            if (SizeUtils.fromMToCm(depo.Depth) < type.Depth)
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.DepthWArt, SizeUtils.fromMToCm(depo.Depth)));
                e.AttributeName = "Depth";
                throw e;
            }
            if (SizeUtils.fromMToCm(depo.Height) < type.Height)
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.DepthWArt, SizeUtils.fromMToCm(depo.Depth)));
                e.AttributeName = "Depth";
                throw e;
            }

            decimal size = type.Width * type.Height * type.Depth; // el tamaño del Container
            if (SizeUtils.fromM3ToCm3(depo.Size) < (depo.UsedSpace - oldSize + size))    // Verifica si el Container puede entrar en el Deposito Temporal
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.SizeWArt, SizeUtils.fromM3ToCm3(depo.Size-depo.UsedSpace)));
                e.AttributeName = "Size";
                throw e;
            }

            return true;
        }

        private bool fitsIntoShelfSubdiv(long typeID, decimal oldSize, long shelfID)
        {
            ShelfsSubdivision shelfDiv = this.db.ShelfsSubdivisions.Find(shelfID);   // Ubica la División del Estante
            if (shelfDiv == null)
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.DivitionWArt, shelfID));
                e.AttributeName = "";
                throw e;
            }

            ContainerType type = this.db.ContainerTypes.Find(typeID);
            if (type == null)
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.ContainerTypeWArt, typeID));
                e.AttributeName = "";
                throw e;
            }


            if (SizeUtils.fromMToCm(shelfDiv.Width) < type.Width)
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.WidthWArt, SizeUtils.fromMToCm(shelfDiv.Width)));
                throw e;
            }
            if (SizeUtils.fromMToCm(shelfDiv.Shelf.Height) < type.Height)
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.DepthWArt, SizeUtils.fromMToCm(shelfDiv.Shelf.Height)));
                e.AttributeName = "Height";
                throw e;
            }
            if (SizeUtils.fromMToCm(shelfDiv.Shelf.Rack.Depth) < type.Depth)
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.DepthWArt, SizeUtils.fromMToCm(shelfDiv.Shelf.Rack.Depth)));
                e.AttributeName = "Depth";
                throw e;
            }

            decimal size = type.Width * type.Height * type.Depth; // el tamaño del Container
            if (SizeUtils.fromM3ToCm3(shelfDiv.Size) < (shelfDiv.UsedSpace - oldSize + size))    // Verifica si el Container puede entrar en el Deposito Temporal
            {
                ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
                    Resources.Attributes.SizeWArt, SizeUtils.fromM3ToCm3(shelfDiv.Size)));
                e.AttributeName = "Size";
                throw e;
            }

            return true;
        }


        public Container getEmptyContainer(long ContainerTypeID)
        {
            try {
               return this.db.Containers.Where(x => x.ContainerType.ContainerTypesID == ContainerTypeID && x.UsedSpace == 0).FirstOrDefault();
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

        public void setContainerUsedSpace(long ContainerID, decimal UsedSapce)
        {
            try
            {
                Container container = this.db.Containers.Find(ContainerID);
                container.UsedSpace = UsedSapce;
                this.db.Containers.Attach(container);
                var entry = this.db.Entry(container);
                entry.Property(e => e.UsedSpace).IsModified = true;
                db.SaveChanges();
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

        public IQueryable<Container> filterByDistributionCenter(IQueryable<Container> query, long dcID) {
            return query.Where(x => (x.IDShelfSubdivition.HasValue && x.ShelfsSubdivision.Shelf.Rack.Hall.Sector.Deposit.IDDistributionCenter == dcID) ||
                (x.IDTemporalDeposit.HasValue && x.TemporaryDeposit.IDDistributionCenter == dcID));
        }

        public IQueryable<Container> filterByDeposit(IQueryable<Container> query, long depositID) {
            return query.Where(x => (x.IDShelfSubdivition.HasValue && x.ShelfsSubdivision.Shelf.Rack.Hall.Sector.IDDeposit == depositID));
        }

        public IQueryable<Container> filterByTemporaryDeposit(IQueryable<Container> query, long depositID) {
            return query.Where(x => x.IDTemporalDeposit.HasValue && x.IDTemporalDeposit.Value == depositID);
        }

        public IQueryable<Container> filterBySector(IQueryable<Container> query, long sectorID) {
            return query.Where(x => (x.IDShelfSubdivition.HasValue && x.ShelfsSubdivision.Shelf.Rack.Hall.IDSector == sectorID));
        }

        public IQueryable<Container> filterByRack(IQueryable<Container> query, long rackID) {
            return query.Where(x => (x.IDShelfSubdivition.HasValue && x.ShelfsSubdivision.Shelf.IDRack == rackID));
        }

        public IQueryable<Container> filterByType(IQueryable<Container> query, long typeID) {
            return query.Where(x => x.IDContainerTypes == typeID);
        }
    }
}