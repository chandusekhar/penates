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
    public class TemporaryDepositRepository : ITemporaryDepositRepository {

        PenatesEntities db = new PenatesEntities();

        public int itemsPerPage {get;set;}

        public TemporaryDepositRepository() {
            this.itemsPerPage = 50;
        }

        public TemporaryDepositRepository(int itemsPerPage) {
            this.itemsPerPage = itemsPerPage;
        }

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido
        ///         *Error del SP</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Save(DepositViewModel deposit) {
            try {
                TemporaryDeposit aux = null;
                decimal oldSize = 0;
                if (deposit.DepositID != 0) {
                    aux = db.TemporaryDeposits.Find(deposit.DepositID);
                    deposit.DistributionCenterID = aux.IDDistributionCenter;
                    oldSize = aux.Width * aux.Depth * aux.Height;
                }
                Nullable<long> val = null;
                if (!this.fitsIntoDistributionCenter(deposit.Floor, deposit.Width, deposit.Height, deposit.Depth, oldSize, deposit.DistributionCenterID)) {
                    return -1;
                }
                if (aux == null) {
                    val = db.SP_TemporaryDeposit_Add(deposit.Description, deposit.Height, deposit.Width, deposit.Depth,
                        deposit.DistributionCenterID, deposit.Floor).SingleOrDefault();
                } else {
                    val = db.SP_TemporaryDeposit_Edit(deposit.DepositID, deposit.Description, deposit.Height, deposit.Width,
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
                    Resources.Resources.TemporaryDepositWArt));
            }
        }

        public TemporaryDeposit getDepositInfo(long id) {
            try {
                TemporaryDeposit aux = db.TemporaryDeposits.Find(id);
                return aux;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }


        public void increaseUsedSpace(long DepositID, decimal SpaceInM3)
        {
            TemporaryDeposit deposit = this.getDepositInfo(DepositID);            
            deposit.UsedSpace += SpaceInM3;
            this.db.TemporaryDeposits.Attach(deposit);
            var entry = db.Entry(deposit);
            entry.Property(e => e.UsedSpace).IsModified = true;
            this.db.SaveChanges();        
        }

        public IQueryable<TemporaryDeposit> getData(long dcID) {
            try {
                return this.db.TemporaryDeposits.Select(p => p).Where(p => p.IDDistributionCenter == dcID);
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<TemporaryDeposit> search(IQueryable<TemporaryDeposit> query, string search) {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<TemporaryDeposit> search(IQueryable<TemporaryDeposit> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Description.Contains(item) || SqlFunctions.StringConvert((double) p.TemporaryDepositID).Contains(item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<TemporaryDeposit> sort(IQueryable<TemporaryDeposit> query, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ID:
                    query = query.OrderBy(x => x.TemporaryDepositID);
                    break;
                case Sorts.ID_DESC:
                    query = query.OrderByDescending(x => x.TemporaryDepositID);
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
                    query = query.OrderBy(x => x.TemporaryDepositID);
                    break;
                default:
                    query = query.OrderBy(x => x.TemporaryDepositID);
                    break;
            }
            return query;
        }

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        public bool Delete(long depositID) {
            TemporaryDeposit td = this.db.TemporaryDeposits.Find(depositID);
            if (td == null) {
                return true;
            }
            if (this.checkDeleteConstraints(td)) {
                var tran = this.db.Database.BeginTransaction();
                try {
                    td = db.TemporaryDeposits.Find(depositID);
                    db.TemporaryDeposits.Remove(td);
                    if(!td.Size.HasValue){
                        td.Size = td.Width * td.Height * td.Depth;
                    }
                    InternalDistributionCenter idc = (InternalDistributionCenter)this.db.DistributionCenters.Find(td.IDDistributionCenter);
                    idc.UsableSpace = idc.UsableSpace - td.Size.Value;
                    idc.UsedSpace = idc.UsedSpace - td.Size.Value;
                    this.db.DistributionCenters.Attach(idc);
                    var entry = db.Entry(idc);
                    entry.Property(e => e.UsedSpace).IsModified = true;
                    entry.Property(e => e.UsableSpace).IsModified = true;
                    db.SaveChanges();
                    tran.Commit();
                    return true;
                } catch (Exception e) {
                    tran.Rollback();
                    throw new DeleteException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.TemporaryDepositWArt, depositID), e.Message);
                }
            } else {
                throw new DeleteConstrainException(String.Format(Resources.ExceptionMessages.DeleteException,
                    Resources.Resources.TemporaryDepositWArt, depositID),
                    String.Format(Resources.ExceptionMessages.DeleteConstraintMessage, Resources.Resources.TemporaryDepositWArt,
                    Resources.Resources.Boxes, Resources.Resources.BoxesWArt));
            }
        }

        public IQueryable<TemporaryDeposit> getAutocomplete(string search, long? dcID) {
            try {
                var data = this.searchAndRank(search, dcID);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        private IQueryable<TemporaryDeposit> searchAndRank(string search, long? dcID) {
            IQueryable<TemporaryDeposit> data = this.db.TemporaryDeposits;
            if (dcID.HasValue) {
                data = data.Where(x => x.IDDistributionCenter == dcID.Value);
            }
            return this.searchAndRank(data, search);
        }

        private IQueryable<TemporaryDeposit> searchAndRank(IQueryable<TemporaryDeposit> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.Description);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<TemporaryDeposit> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<TemporaryDeposit> {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double) x.table.TemporaryDepositID).Trim() == item ? 1000 : 0) +
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

        private bool checkDeleteConstraints(TemporaryDeposit td) {
            if (td.Containers == null || td.Containers.Count == 0) {
                return true;
            }
            return false;
        }

        private bool fitsIntoDistributionCenter(int floor, decimal width, decimal height, decimal depth, decimal oldSize, long dcID) {
            IInternalDistributionCenterRepository repo = new InternalDistributionCenterRepository();
            InternalDistributionCenter idc = repo.getData(dcID);
            if (idc == null) {
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
    }
}