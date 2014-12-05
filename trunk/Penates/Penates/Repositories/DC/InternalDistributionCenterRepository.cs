using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.DC;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace Penates.Repositories.DC {
    public class InternalDistributionCenterRepository : IInternalDistributionCenterRepository {
        PenatesEntities db = new PenatesEntities();
        public int itemsPerPage { get; set; }

        public InternalDistributionCenterRepository() {
            this.itemsPerPage = 50;
        }

        public InternalDistributionCenterRepository(int itemsPerPage) {
            this.itemsPerPage = itemsPerPage;
        }

        /// <summary>Obtiene los datos de un Centro de Distribucion Interno. Omite los ya eliminados</summary>
        /// <param name="id">Id del Centro de Distribucion</param>
        /// <returns></returns>
        public InternalDistributionCenter getData(long id) {
            try {
                DistributionCenter aux = db.DistributionCenters.Find(id);
                if (aux == null || aux.Deleted) {
                    return null;
                }
                return (InternalDistributionCenter) aux;
            } catch (InvalidCastException) {
                return null;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        /// <summary>Obtiene los datos de un Centro de Distribucion Interno</summary>
        /// <param name="id">Id del CD</param>
        /// <param name="includeDeleted">True para incluir a los ya eliminados, de lo contrario false</param>
        /// <returns></returns>
        public InternalDistributionCenter getData(long id, bool includeDeleted) {
            if (includeDeleted) {
                try {
                    return (InternalDistributionCenter) db.DistributionCenters.Find(id);
                } catch (InvalidCastException) {
                    return null;
                } catch (NullReferenceException) {
                    return null;
                } catch (Exception e) {
                    throw new DatabaseException(e.Message);
                }
            } else {
                return getData(id);
            }
        }

        /// <summary>Obtiene todos los centros de Distribuciones Internos</summary>
        /// <returns></returns>
        public IQueryable<InternalDistributionCenter> getData() {
            try {
                return this.db.DistributionCenters.OfType<InternalDistributionCenter>().Select(p => p).Where(p => p.Deleted == false);
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido
        ///         *Error del SP</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Save(InternalDistributionCenterViewModel dc) {
            try {
                DistributionCenter aux = db.DistributionCenters.Find(dc.DistributionCenterID);
                Nullable<long> val = null;
                if (aux == null) {
                    val = db.SP_InternalDistributionCenter_Add(dc.Floors, dc.CityID, dc.Address, dc.Width, dc.Height,
                        dc.Depth, dc.Telephone).SingleOrDefault();
                } else {
                    val = db.SP_InternalDistributionCenter_Edit(dc.DistributionCenterID, dc.Floors, dc.CityID,
                        dc.Address, dc.Width, dc.Height, dc.Depth, dc.Telephone).SingleOrDefault();
                }
                if (!val.HasValue) {
                    return -1;
                }
                return val.Value;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.DistributionCenter + ": " + dc.DistributionCenterID));
            }
        }

        /// <summary>Obtiene los depositos de un Centro de Distribuciones</summary>
        /// <param name="id">Id del Centro de Distribucion a Buscar</param>
        /// <returns>Collection de Depositos</returns>
        public ICollection<Deposit> getDeposits(long id) {
            try {
                InternalDistributionCenter aux = this.db.DistributionCenters.OfType<InternalDistributionCenter>().FirstOrDefault(x => x.DistributionCenterID == id);
                if (aux == null) {
                    return new List<Deposit>();
                }
                return aux.Deposits;
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.DistributionCenterWArt, id), e.Message);
            }
        }

        /// <summary>Obtiene los depositos temporales de un Centro de Distribuciones</summary>
        /// <param name="id">Id del Centro de Distribucion a Buscar</param>
        /// <returns>Collection de Depositos Temporales</returns>
        public ICollection<TemporaryDeposit> getTemporalDeposits(long id) {
            try {
                InternalDistributionCenter aux = this.db.DistributionCenters.OfType<InternalDistributionCenter>().FirstOrDefault(x => x.DistributionCenterID == id);
                if (aux == null) {
                    return new List<TemporaryDeposit>();
                }
                return aux.TemporaryDeposits;
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.DistributionCenterWArt, id), e.Message);
            }
        }

        public long getDepositsConstraintsNumber(long centerID) {
            try {
                InternalDistributionCenter dc = (InternalDistributionCenter) this.db.DistributionCenters.Find(centerID);
                if (dc.Deposits == null) {
                    return 0;
                }
                return dc.Deposits.Count;
            } catch (InvalidCastException) {
                return 0;
            } catch (NullReferenceException) {
                return 0;
            }
        }

        public List<Constraint> getDepositsConstraints(long centerID) {
            try {
                InternalDistributionCenter dc = (InternalDistributionCenter) this.db.DistributionCenters.Find(centerID);
                var result = dc.Deposits.Take(this.itemsPerPage).ToList();
                var constraints = result.Select(x => new Constraint {
                    id = x.DepositID.ToString(),
                    name = Penates.App_GlobalResources.Forms.ModelFormsResources.Description
                });
                return constraints.ToList();
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.DistributionCenterWArt, centerID), e.Message);
            }
        }

        public long getTemporaryDepositsConstraintsNumber(long centerID) {
            try {
                InternalDistributionCenter dc = (InternalDistributionCenter) this.db.DistributionCenters.Find(centerID);
                if (dc.TemporaryDeposits == null) {
                    return 0;
                }
                return dc.TemporaryDeposits.Count;
            } catch (InvalidCastException) {
                return 0;
            } catch (NullReferenceException) {
                return 0;
            }
        }

        public List<Constraint> getTemporaryDepositsConstraints(long centerID) {
            try {
                InternalDistributionCenter dc = (InternalDistributionCenter) this.db.DistributionCenters.Find(centerID);
                var result = dc.TemporaryDeposits.Take(this.itemsPerPage).ToList();
                var constraints = result.Select(x => new Constraint {
                    id = x.TemporaryDepositID.ToString(),
                    name = Penates.App_GlobalResources.Forms.ModelFormsResources.Description
                });
                return constraints.ToList();
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.DistributionCenterWArt, centerID), e.Message);
            }
        }

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        internal bool Delete(InternalDistributionCenter idc) {
            DistributionCenterRepository dcRepo = new DistributionCenterRepository();
            if (this.checkDeleteConstrains(idc)) {
                var tran = this.db.Database.BeginTransaction();
                try {
                    if (this.checkPhysicalDelete(idc)) {
                        dcRepo.deleteNotifications(idc);
                        var dcInt = db.DistributionCenters.Find(idc.DistributionCenterID);
                        db.DistributionCenters.Remove(dcInt);
                    } else {
                        idc.Deleted = true;
                        db.DistributionCenters.Attach(idc);
                        var entry = db.Entry(idc);
                        entry.Property(e => e.Deleted).IsModified = true;
                        dcRepo.deleteNotifications(idc);
                    }
                    db.SaveChanges();
                    tran.Commit();
                    return true;
                } catch (Exception e) {
                    tran.Rollback();
                    throw new DeleteException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.CategoryWArt, idc.DistributionCenterID), e.Message);
                }
            } else {
                throw new DeleteConstrainException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.CategoryWArt, idc.DistributionCenterID),
                    String.Format(Resources.ExceptionMessages.DeleteConstraintMessage,Resources.Resources.DistributionCenterWArt,
                    Resources.Resources.Boxes, Resources.Resources.BoxesWArt));
            }
        }

        /// <summary>Checkea si puede eliminar el Centro de Distribuciones con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id">Id del Centro a eliminar</param>
        /// <returns>True si puede eliminar el Centro de Distribuciones, false si no lo puede eliminar</returns>
        /// <exception cref="InvalidCastException">Si el ID no pertenece a un InternalDC</exception>
        private bool checkDeleteConstrains(long id) {
            try {
                InternalDistributionCenter dc = (InternalDistributionCenter) db.DistributionCenters.Find(id);
                return this.checkDeleteConstrains(dc);
            } catch (NullReferenceException) {
                return true;
            }
        }

        /// <summary> Checkea si puede eliminar el Producto con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> El Producto a Eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        private bool checkDeleteConstrains(InternalDistributionCenter dc) {
            DistributionCenterRepository dcRepo = new DistributionCenterRepository();
            if (dcRepo.checkDeleteConstrains(dc)) {
                if (this.checkDepositsConstraint(dc) && this.checkTemporalDepositsConstraint(dc)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary> Checkea si puede eliminar el Producto con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> El Producto a Eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        private bool checkPhysicalDelete(InternalDistributionCenter dc) {
            DistributionCenterRepository dcRepo = new DistributionCenterRepository();
            if (dcRepo.checkFisicalDelete(dc)) {
                if (dc.Deposits != null && dc.Deposits.Count != 0) {
                    foreach (Deposit dep in dc.Deposits) {
                        foreach (Sector sec in dep.Sectors) {
                            foreach (Hall hall in sec.Halls) {
                                if (hall.Racks != null && hall.Racks.Count != 0) {
                                    return false; //Si tengo almenos un rack no puedo borrarlo
                                }
                            }
                        }
                    }
                }
                return this.checkTemporalDepositsConstraint(dc);
            }
            return false;
        }

        private bool checkTemporalDepositsConstraint(InternalDistributionCenter dc) {
            foreach (TemporaryDeposit tDep in dc.TemporaryDeposits) {
                if (tDep.Containers != null && tDep.Containers.Count != 0) {
                    return false;
                }
            }
            return true;
        }

        private bool checkDepositsConstraint(InternalDistributionCenter dc) {
            foreach (Deposit dep in dc.Deposits) {
                if (dep.UsedUsableSpace > 0) { //Quiere decir que no tengo nada guardado adentro
                    return false;
                }
            }
            return true;
        }

        /// <summary>Remueve los Depositos Temporales de un CD</summary>
        /// <param name="dc"></param>
        /// <returns>true si los remueve, false si hay alguna constraint</returns>
        private bool deleteTemporalDeposits(InternalDistributionCenter dc) {
            try {
                foreach (TemporaryDeposit tDep in dc.TemporaryDeposits) {
                    this.db.TemporaryDeposits.Remove(tDep);
                }
                this.db.SaveChanges();
                return true;
            }catch(UpdateException){
                return false;
            }
        }

        private bool deleteDeposits(InternalDistributionCenter dc) {
            try {
                foreach (Deposit dep in dc.Deposits) {
                    foreach (Sector sec in dep.Sectors) {
                        foreach (Hall hall in sec.Halls) {
                            this.db.Halls.Remove(hall);
                        }
                        this.db.Sectors.Remove(sec);
                    }
                    this.db.Deposits.Remove(dep);
                }
                this.db.SaveChanges();
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public IQueryable<InternalDistributionCenter> getAutocomplete(string search) {
            try {
                var data = this.searchAndRank(search);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<InternalDistributionCenter> searchAndRank(string search) {
            var data = this.getData();
            return this.searchAndRank(data, search);
        }

        public IQueryable<InternalDistributionCenter> searchAndRank(IQueryable<InternalDistributionCenter> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.DistributionCenterID);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<InternalDistributionCenter> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<InternalDistributionCenter> {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double) x.table.DistributionCenterID).Trim() == item ? 1000 : 0) +
                                ((x.table.Address.Contains(item)) ? item.Length : 0) + ((x.table.Address.StartsWith(item)) ? (item.Length * 2) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.DistributionCenterID)
                    .Select(x => x.table);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public long getProductTypesQuantity(long DistributionCenterID)
        {
            ICollection<Deposit> DepositsList = this.getDeposits(DistributionCenterID);
            long ProductsTypesQuantity =0;

            if (DepositsList != null)
            {
                foreach (Deposit deposit in DepositsList)
                {
                    ProductsTypesQuantity += deposit.ProductCategories.Count;
                }
            }
            return ProductsTypesQuantity; 
        }

        public InternalDistributionCenterDetails getInternalDistributionCenterDetails(long DistributionCenterID)
        {
            InternalDistributionCenterDetails idc = new InternalDistributionCenterDetails();
            ICollection<Deposit> DepositsList = this.getDeposits(DistributionCenterID);
            foreach (Deposit Deposit in DepositsList)
            {               
                idc.Deposits++;
                ICollection<Sector> Sectors = Deposit.Sectors == null ? new List<Sector>() : Deposit.Sectors;
                foreach (Sector Sector in Sectors)
                {
                    idc.Sectors++;
                    ICollection<Hall> Halls = Sector.Halls == null ? new List<Hall>() : Sector.Halls;
                    foreach (Hall Hall in Halls)
                    {
                        idc.Halls++;
                        ICollection<Rack> Racks = Hall.Racks == null ? new List<Rack>() : Hall.Racks;
                        foreach(Rack Rack in Racks){
                            idc.Racks++;
                            ICollection<Shelf> Shelfs = Rack.Shelfs == null ? new List<Shelf>() : Rack.Shelfs;
                            foreach (Shelf Shelf in Shelfs)
                            {
                                idc.Shelfs++;
                                ICollection<ShelfsSubdivision> ShelfSubdivisions = Shelf.ShelfsSubdivisions == null ? new List<ShelfsSubdivision>() : Shelf.ShelfsSubdivisions;
                                foreach (ShelfsSubdivision Subdivision in ShelfSubdivisions)
                                {                                
                                    ICollection<Container> Containers = Subdivision.Containers == null ? new List<Container>() : Subdivision.Containers;
                                    foreach (Container Container in Containers)
                                    {
                                        ICollection<InternalBox> Boxes = Container.InternalBoxes == null ? new List<InternalBox>() : Container.InternalBoxes;
                                        foreach (Box Box in Boxes)
                                        {
                                            idc.Boxes++;
                                            idc.ProducsQuantity += Box.Quantity;                                          
                                        }
                                    }       
                                }
                            }
                        }    
                    }
                }
            }
            return idc;
        }


        public void updateTempDepositUsedSpace(long DistributionCenterID, long TempDepositID, decimal UsedSpaceInCm3)
        {
            InternalDistributionCenter dc = (InternalDistributionCenter) this.db.DistributionCenters.Find(DistributionCenterID);
            dc.UsedSpace = dc.UsedSpace + SizeUtils.fromCm3ToM3(UsedSpaceInCm3);
            this.db.DistributionCenters.Attach(dc);
            var dcEntry = db.Entry(dc);
            dcEntry.Property(e => e.UsedSpace).IsModified = true; 

            TemporaryDeposit deposit = this.db.TemporaryDeposits.Find(TempDepositID);
            deposit.UsedSpace = deposit.UsedSpace +  SizeUtils.fromCm3ToM3(UsedSpaceInCm3);
            this.db.TemporaryDeposits.Attach(deposit);
            var entry = db.Entry(deposit);
            entry.Property(e => e.UsedSpace).IsModified = true;

            db.SaveChanges();  
        }   

    }
}