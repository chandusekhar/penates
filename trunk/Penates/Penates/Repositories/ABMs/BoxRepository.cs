using Penates.Database;
using Penates.Exceptions;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.Forms;
using Penates.Repositories.DC;
using Penates.Repositories.Transactions;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace Penates.Repositories.ABMs {
    public class BoxRepository : IBoxRepository {
        PenatesEntities db = new PenatesEntities();

        public int itemsPerPage {get;set;}

        public BoxRepository() {
            this.itemsPerPage = 50;
        }

        public BoxRepository(int itemsPerPage) {
            this.itemsPerPage = itemsPerPage;
        }

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Add(BoxViewModel box) {
            try {
                Nullable<long> val = val = db.SP_InternalBox_Add(box.Reserved, box.Transit, box.ProductID, box.AdquisitionDate, box.Quantity, box.BuyerCost,
                    box.ContainerID, box.StatusID, box.PackID, box.Depth, box.Width, box.Height).SingleOrDefault();
                if (!val.HasValue) {
                    return -1;
                }
                return val.Value;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.BoxWArt));
            }
        }

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Add(ExternalBoxViewModel box) {
            try {
                Nullable<long> val = val = db.SP_ExternalBox_Add(box.Reserved, box.Transit, box.ProductID, box.AdquisitionDate, box.Quantity, box.BuyerCost,
                    box.DistributionCenterID, box.StatusID, box.PackID, box.Depth, box.Width, box.Height).SingleOrDefault();
                if (!val.HasValue) {
                    return -1;
                }
                return val.Value;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.BoxWArt));
            }
        }

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Edit(BoxViewModel box) {
            try {
                Box aux = null;
                Nullable<long> val = null;
                aux = this.db.Boxes.Find(box.BoxID);
                if (aux == null) {
                    val = -1;
                } else {
                    val = db.SP_Box_Edit(box.BoxID, box.IsWaste, box.Reevaluate, box.Reserved, box.Transit, box.StatusID,
                        box.PackID).SingleOrDefault();
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

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Edit(ExternalBoxViewModel box) {
            try {
                Box aux = null;
                Nullable<long> val = null;
                aux = this.db.Boxes.Find(box.BoxID);
                if (aux == null) {
                    val = -1;
                } else {
                    val = db.SP_Box_Edit(box.BoxID, box.IsWaste, box.Reevaluate, box.Reserved, box.Transit, box.StatusID,
                        box.PackID).SingleOrDefault();
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

        public Box getBoxInfo(long id) {
            try {
                Box aux = db.Boxes.Find(id);
                return aux;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<Box> getData() {
            try {
                return this.db.Boxes.Where(x => x.Deleted == false);
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Box> search(IQueryable<Box> query, string search) {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<Box> search(IQueryable<Box> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Product.Name.Contains(item) || SqlFunctions.StringConvert((double) p.BoxID).Contains(item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Box> sort(IQueryable<Box> query, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ID:
                    query = query.OrderBy(x => x.BoxID);
                    break;
                case Sorts.ID_DESC:
                    query = query.OrderByDescending(x => x.BoxID);
                    break;
                case Sorts.PRODUCT:
                    query = query.OrderBy(x => x.Product.Name);
                    break;
                case Sorts.PRODUCT_DESC:
                    query = query.OrderByDescending(x => x.Product.Name);
                    break;
                case Sorts.STATUS:
                    query = query.OrderBy(x => x.ItemsState.Description);
                    break;
                case Sorts.STATUS_DESC:
                    query = query.OrderByDescending(x => x.ItemsState.Description);
                    break;
                case Sorts.QUANTITY:
                    query = query.OrderBy(x => x.Quantity);
                    break;
                case Sorts.QUANTITY_DESC:
                    query = query.OrderByDescending(x => x.Quantity);
                    break;
                case Sorts.DATE:
                    query = query.OrderBy(x => x.AdquisitionDate);
                    break;
                case Sorts.DATE_DESC:
                    query = query.OrderByDescending(x => x.AdquisitionDate);
                    break;
                case Sorts.DEFAULT:
                    query = query.OrderBy(x => x.BoxID);
                    break;
                default:
                    query = query.OrderBy(x => x.BoxID);
                    break;
            }
            return query;
        }

        public IQueryable<Box> filterByDistributionCenter(IQueryable<Box> query, long dcID) {
            IQueryable<ExternalBox> aux = query.OfType<ExternalBox>();
            aux = aux.Where(x => x.ExternalDistributionCenterID == dcID);
            IQueryable<Box> boxAux = query.OfType<InternalBox>().Where(x => (x.Container.IDShelfSubdivition.HasValue && x.Container.ShelfsSubdivision.Shelf.Rack.Hall.Sector.Deposit.IDDistributionCenter == dcID) ||
                (x.Container.IDTemporalDeposit.HasValue && x.Container.TemporaryDeposit.IDDistributionCenter == dcID));
            boxAux = boxAux.Concat(aux);
            return boxAux;
        }

        public IQueryable<Box> filterByDeposit(IQueryable<Box> query, long depositID) {
            return query.OfType<InternalBox>().Where(x => (x.Container.IDShelfSubdivition.HasValue && x.Container.ShelfsSubdivision.Shelf.Rack.Hall.Sector.IDDeposit == depositID));
        }

        public IQueryable<Box> filterByTemporaryDeposit(IQueryable<Box> query, long depositID) {
            return query.OfType<InternalBox>().Where(x => x.Container.IDTemporalDeposit.HasValue && x.Container.IDTemporalDeposit.Value == depositID);
        }

        public IQueryable<Box> filterBySector(IQueryable<Box> query, long sectorID) {
            return query.OfType<InternalBox>()
                .Where(x => (x.Container.IDShelfSubdivition.HasValue && x.Container.ShelfsSubdivision.Shelf.Rack.Hall.IDSector == sectorID));
        }

        public IQueryable<Box> filterByRack(IQueryable<Box> query, long rackID) {
            return query.OfType<InternalBox>()
                .Where(x => (x.Container.IDShelfSubdivition.HasValue && x.Container.ShelfsSubdivision.Shelf.IDRack == rackID));
        }

        public IQueryable<Box> filterByPack(IQueryable<Box> query, long packID) {
            return query.Where(x => (x.IDPack == packID));
        }

        public IQueryable<Box> filterByStatus(IQueryable<Box> query, long statusID) {
            return query.Where(x => (x.IDStatus == statusID));
        }

        public IQueryable<Box> filterByProduct(IQueryable<Box> query, long productID) {
            return query.Where(x => (x.IDProduct == productID));
        }

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        public Status sell(long boxID, long saleID) {
            Box box = this.db.Boxes.Find(boxID);
            if (box == null) {
                return new Status {
                    Success = true
                };
            }
            if (box.IDSale.HasValue) {
                throw new DeleteConstrainException(String.Format(Resources.ExceptionMessages.DeleteException,
                    Resources.Resources.BoxesWArt, boxID),
                String.Format(Resources.ExceptionMessages.BoxAllreadySoldException, boxID));
            }
            var tran = this.db.Database.BeginTransaction();
            try {
                box.IDSale = saleID;
                box.Deleted = true;
                this.db.Boxes.Attach(box);
                var entry = db.Entry(box);
                entry.Property(e => e.IDSale).IsModified = true;
                entry.Property(e => e.Deleted).IsModified = true;
                if(box is ExternalBox){
                    this.db.SaveChanges();
                    ExternalBox eBox = (ExternalBox) box;
                    if (eBox.Size.HasValue) {
                        IExternalDistributionCenterRepository externalRepo = new ExternalDistributionCenterRepository();
                        externalRepo.setUsedSpace(eBox.ExternalDistributionCenterID, eBox.Size.Value * -1);
                    }
                }else{
                    InternalBox iBox = (InternalBox) box;
                    Container container = this.db.Containers.Find(iBox.IDContainer);
                    if (iBox.Size.HasValue) {
                        container.UsedSpace = container.UsedSpace - iBox.Size.Value;
                        if (container.UsedSpace < 0) {
                            container.UsedSpace = 0;
                        }
                    }
                    this.db.Containers.Attach(container);
                    var entry2 = db.Entry(container);
                    entry2.Property(e => e.UsedSpace).IsModified = true;
                    db.SaveChanges();
                    if (container.UsedSpace <= 0) {
                        MovementRepository movementRepo = new MovementRepository();
                        TemporaryDeposit td = movementRepo.moveEmptyContainer(container.ContainerID);
                        tran.Commit();
                        return new Status {
                            Success = true,
                            Message = String.Format(Resources.Messages.EmptyContainerMoved, container.ContainerID,
                            td.Description, td.TemporaryDepositID)
                        };
                    }
                }
                tran.Commit();
                return new Status {
                    Success = true
                };
            } catch (Exception e) {
                tran.Rollback();
                throw new DeleteException(String.Format(Resources.ExceptionMessages.DeleteException,
                    Resources.Resources.BoxesWArt, boxID), e.Message);
            }
        }

        public IQueryable<Box> getAutocomplete(string search, long? dcID, long? depositID, long? temporaryDepositID, long? sectorID, long? rackID) {
            try {
                var data = this.searchAndRank(search, dcID, depositID, temporaryDepositID, sectorID, rackID);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        private IQueryable<Box> searchAndRank(string search, long? dcID, long? depositID, long? temporaryDepositID, long? sectorID, long? rackID) {
            IQueryable<Box> data = this.getData();

            if (dcID.HasValue) {
                IQueryable<ExternalBox> aux = data.OfType<ExternalBox>();
                aux = aux.Where(x => x.ExternalDistributionCenterID == dcID);
                data = data.OfType<InternalBox>()
                    .Where(x => x.Container.IDShelfSubdivition.HasValue && x.Container.ShelfsSubdivision.Shelf.Rack.Hall.Sector.Deposit.IDDistributionCenter == dcID.Value);
                data = data.Concat(aux);
            }
            if (depositID.HasValue) {
                data = data.OfType<InternalBox>()
                    .Where(x => x.Container.IDShelfSubdivition.HasValue && x.Container.ShelfsSubdivision.Shelf.Rack.Hall.Sector.IDDeposit == depositID.Value);
            }
            if (temporaryDepositID.HasValue) {
                data = data.OfType<InternalBox>()
                    .Where(x => x.Container.IDTemporalDeposit.HasValue && x.Container.IDTemporalDeposit == temporaryDepositID.Value);
            }
            if (sectorID.HasValue) {
                data = data.OfType<InternalBox>()
                    .Where(x => x.Container.IDShelfSubdivition.HasValue && x.Container.ShelfsSubdivision.Shelf.Rack.Hall.IDSector == sectorID.Value);
            }
            if (rackID.HasValue) {
                data = data.OfType<InternalBox>()
                    .Where(x => x.Container.IDShelfSubdivition.HasValue && x.Container.ShelfsSubdivision.Shelf.IDRack == rackID.Value);
            }
            return this.searchAndRank(data, search);
        }

        private IQueryable<Box> searchAndRank(IQueryable<Box> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.BoxID);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<Box> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<Box> {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double) x.table.BoxID).Trim() == item ? 1000 : 0) +
                                ((x.table.Product.Name.Contains(item)) ? item.Length *2 : 0) + ((x.table.Product.Name.StartsWith(item)) ? (item.Length * 4) : 0) +
                                ((x.table.Product.Description.Contains(item)) ? item.Length : 0) + ((x.table.Product.Description.StartsWith(item)) ? (item.Length * 2) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenBy(x => x.table.BoxID)
                    .Select(x => x.table);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }


        public bool Delete(long BoxId)
        {
           Box box = db.Boxes.Find(BoxId);
           if (box == null)
           {
                return false;
           }
           var tran = this.db.Database.BeginTransaction();
           if (!box.IDSale.HasValue)
           {
               try
               {
                   db.Boxes.Remove(box);
                   this.db.SaveChanges();
                   tran.Commit();
                   return true;
               }
               catch (Exception e)
               {
                   tran.Rollback();
                   throw new DatabaseException(String.Format(Resources.ExceptionMessages.DeleteException,
                       Resources.Resources.BoxesWArt, BoxId), e.Message);
               }
           }
           return false;
        }



        public bool setContainerID(long BoxID, long ContainerID)
        {
           Box box = db.Boxes.Find(BoxID);
           if (box == null)
           {
                return false;
           }
           if (box is InternalBox) {
               InternalBox iBox = (InternalBox)box;
               var tran = this.db.Database.BeginTransaction();
               try {
                   iBox.IDContainer = ContainerID;
                   this.db.Boxes.Attach(iBox);
                   var entry = db.Entry(iBox);
                   entry.Property(e => e.IDContainer).IsModified = true;
                   this.db.SaveChanges();
                   tran.Commit();
               } catch (Exception e) {
                   tran.Rollback();
                   throw new DatabaseException(e.Message);
               }
               return true;
           } else {
               return false;
           }
        }

        //private bool fitsIntoDivition(decimal width, decimal depth, decimal height, long divitionID) {
        //    ShelfsSubdivision divition = this.db.ShelfsSubdivisions.Find(divitionID);
        //    if (divition == null) {
        //        ModelErrorException e = new ModelErrorException(String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
        //            Resources.Resources.RacksWArt, divition.Shelf.IDRack));
        //        e.AttributeName = "";
        //        throw e;
        //    }
        //    if (s.Width < width) {
        //        ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
        //            Resources.Attributes.WidthWArt, sector.Width));
        //        e.AttributeName = "Width";
        //        throw e;
        //    }
        //    if (sector.Depth < depth) {
        //        ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
        //            Resources.Attributes.DepthWArt, sector.Depth));
        //        e.AttributeName = "Depth";
        //        throw e;
        //    }
        //    decimal size = width * sector.Deposit.Height * depth; // xq el sector toma la altura del deposito en el que esta
        //    size = size - oldSize;
        //    if (sector.Size < sector.UsedSpace + size) {
        //        ModelErrorException e = new ModelErrorException(String.Format(Resources.FormsErrors.DimentionTooBig,
        //            Resources.Attributes.SizeWArt, sector.Size));
        //        e.AttributeName = "Size";
        //        throw e;
        //    }
        //    return true;
        //}
    }
}