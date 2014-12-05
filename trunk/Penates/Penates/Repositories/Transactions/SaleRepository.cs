using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace Penates.Repositories.Transactions {
    public class SaleRepository : ISaleRepository {

        PenatesEntities db = new PenatesEntities();

        ///// <summary> Guarda los datos de un Rack en la Base de Datos</summary>
        ///// <param name="prod">El viewmodel con los datos del producto</param>
        ///// <returns>* long > 0 si el ID es valido</returns>
        ///// <exception cref=></exception>
        ///// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        //public long Save(RackViewModel rack) {
        //    try {
        //        Rack aux = null;
        //        decimal oldSize = 0;
        //        if (rack.RackID != 0) {
        //            aux = db.Racks.Find(rack.RackID);
        //            if (aux != null) {
        //                oldSize = aux.Size.HasValue ? aux.Size.Value : 0;
        //            }
        //        }
        //        if (!this.fitsIntoHall(rack.Width, rack.Depth, rack.Width, oldSize, rack.HallID.Value)) {
        //            return -1;
        //        }
        //        Nullable<long> val = null;
        //        if (aux == null) {
        //            val = db.SP_Rack_Add(rack.Description, rack.Height, rack.Width, rack.Depth, rack.HallID, rack.RackCode, rack.ShelfsNumber, rack.DivitionsNumber).SingleOrDefault();
        //        } else {
        //            int shelves = aux.Shelfs.Count;
        //            int divitions = aux.Shelfs.FirstOrDefault().ShelfsSubdivisions.Count;
        //            if (aux.Width != rack.Width || aux.Height != rack.Height || aux.Depth != rack.Depth ||
        //                (rack.ShelfsNumber.HasValue && rack.ShelfsNumber.Value != shelves) || (rack.DivitionsNumber.HasValue && rack.DivitionsNumber.Value != divitions) ||
        //                rack.HallID != aux.IDHall) {
        //                if (rack.ShelfsNumber.HasValue) {
        //                    shelves = rack.ShelfsNumber.Value;
        //                }
        //                if (rack.DivitionsNumber.HasValue) {
        //                    divitions = rack.DivitionsNumber.Value;
        //                }
        //                if (!this.checkDeleteConstraints(aux)) { //Si todavia tiene productos adentro
        //                    ModelErrorException excep = new ModelErrorException(Resources.Errors.EditRackError);
        //                    excep.AttributeName = "";
        //                    throw excep;
        //                }
        //                val = db.SP_Rack_Edit(rack.RackID, rack.Description, rack.Height, rack.Width, rack.Depth, rack.HallID, rack.RackCode, rack.ShelfsNumber, rack.DivitionsNumber).SingleOrDefault();
        //            } else {
        //                val = db.SP_Rack_LightEdit(rack.RackID, rack.Description, rack.RackCode).SingleOrDefault();
        //            }

        //        }
        //        if (!val.HasValue) {
        //            return -1;
        //        }
        //        return val.Value;
        //    } catch (ModelErrorException e) {
        //        throw e;
        //    } catch (Exception) {
        //        throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException,
        //            Resources.Resources.RackWArt));
        //    }
        //}

        public Sale getSaleInfo(long id) {
            try {
                Sale aux = db.Sales.Find(id);
                return aux;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public IQueryable<Sale> getData() {
            try {
                return this.db.Sales;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Sale> search(IQueryable<Sale> query, string search) {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<Sale> search(IQueryable<Sale> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Client.Name.Contains(item) || p.BillNumber.Contains(item) ||
                        SqlFunctions.StringConvert((double) p.IDDistributionCenter).Contains(item) ||
                        SqlFunctions.StringConvert((double) p.SaleID).Contains(item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<Sale> sort(IQueryable<Sale> query, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ID:
                    query = query.OrderBy(x => x.SaleID);
                    break;
                case Sorts.ID_DESC:
                    query = query.OrderByDescending(x => x.SaleID);
                    break;
                case Sorts.BILL:
                    query = query.OrderBy(x => x.BillNumber);
                    break;
                case Sorts.BILL_DESC:
                    query = query.OrderByDescending(x => x.BillNumber);
                    break;
                case Sorts.DISTRIBUTION_CENTER:
                    query = query.OrderBy(x => x.IDDistributionCenter);
                    break;
                case Sorts.DISTRIBUTION_CENTER_DESC:
                    query = query.OrderByDescending(x => x.IDDistributionCenter);
                    break;
                case Sorts.CLIENT:
                    query = query.OrderBy(x => x.Client);
                    break;
                case Sorts.CLIENT_DESC:
                    query = query.OrderByDescending(x => x.Client);
                    break;
                case Sorts.DATE:
                    query = query.OrderBy(x => x.SaleDate);
                    break;
                case Sorts.DATE_DESC:
                    query = query.OrderByDescending(x => x.SaleDate);
                    break;
                case Sorts.DEFAULT:
                    query = query.OrderByDescending(x => x.SaleDate);
                    break;
                default:
                    query = query.OrderByDescending(x => x.SaleDate);
                    break;
            }
            return query;
        }

        public IQueryable<Sale> filterByDistributionCenter(IQueryable<Sale> query, long dcID) {
            return query.Where(x => x.IDDistributionCenter == dcID);
        }

        public IQueryable<Sale> filterByClient(IQueryable<Sale> query, long id) {
            return query.Where(x => x.IDClient == id);
        }

        public IQueryable<Sale> filterByAnnulated(IQueryable<Sale> query, bool annulated) {
            return query.Where(x => x.Annulated == annulated);
        }

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        public Status Annulate(long saleID) {
            Sale sale = this.db.Sales.Find(saleID);
            if (sale == null) {
                return new Status(){
                    Success = false,
                    Message = String.Format(Resources.ExceptionMessages.IDNotFoundException, Resources.Resources.SaleWArt, saleID)
                };
            }
            var tran = this.db.Database.BeginTransaction();
            try {
                string message;
                if (sale.Annulated) {
                    sale.Annulated = false;
                    message = Resources.Messages.SaleApproved;
                } else {
                    sale.Annulated = true;
                    message = Resources.Messages.SaleAnnulated;
                }
                this.db.Sales.Attach(sale);
                var entry = db.Entry(sale);
                entry.Property(e => e.Annulated).IsModified = true;
                db.SaveChanges();
                tran.Commit();
                return new Status() {
                    Success = true,
                    Message = message
                };
            } catch (Exception e) {
                tran.Rollback();
                throw new DeleteException(String.Format(Resources.ExceptionMessages.DeleteException,
                   Resources.Resources.SaleWArt, saleID), e.Message);
            }
        }

        public IQueryable<Sale> getAutocomplete(string search, long? dcID, long? clientID, bool? annulated) {
            try {
                var data = this.searchAndRank(search, dcID, clientID, annulated);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        private IQueryable<Sale> searchAndRank(string search, long? dcID, long? clientID, bool? annulated) {
            IQueryable<Sale> data = this.getData();
            if (dcID.HasValue) {
                data = data.Where(x => x.IDDistributionCenter == dcID.Value);
            }
            if (clientID.HasValue) {
                data = data.Where(x => x.IDClient == clientID.Value);
            }
            if (annulated.HasValue) {
                data = data.Where(x => x.Annulated == annulated.Value);
            }
            return this.searchAndRank(data, search);
        }

        private IQueryable<Sale> searchAndRank(IQueryable<Sale> data, string search) {
            try {
                if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderByDescending(x => x.SaleDate);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<Sale> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<Sale> {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double) x.table.SaleID).Trim() == item ? 10000 : 0) +
                                ((x.table.BillNumber.Contains(item)) ? item.Length * 4 : 0) + ((x.table.BillNumber.StartsWith(item)) ? (item.Length * 8) : 0) +
                                ((x.table.Client.Name.Contains(item)) ? item.Length : 0) + ((x.table.Client.Name.StartsWith(item)) ? (item.Length * 2) : 0) +
                                ((SqlFunctions.StringConvert((double) x.table.IDDistributionCenter).Contains(item)) ? item.Length : 0) + ((SqlFunctions.StringConvert((double) x.table.IDDistributionCenter).StartsWith(item)) ? (item.Length * 2) : 0)
                    });
                }
                aux = aux.Where(x => x.rankPoints > 0);
                return aux.OrderByDescending(x => x.rankPoints)
                    .ThenByDescending(x => x.table.SaleDate)
                    .ThenBy(x => x.table.BillNumber)
                    .Select(x => x.table);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        //public void updateUsedSpace(long ShelfSubdivisionID, decimal UsedSpaceInCm) {

        //    decimal UsedSpaceInM = SizeUtils.fromCm3ToM3(UsedSpaceInCm);

        //    ShelfsSubdivision shelfSubdivision = this.db.ShelfsSubdivisions.Find(ShelfSubdivisionID);
        //    shelfSubdivision.UsedSpace = shelfSubdivision.UsedSpace + UsedSpaceInM;
        //    this.db.ShelfsSubdivisions.Attach(shelfSubdivision);
        //    var entry0 = db.Entry(shelfSubdivision);
        //    entry0.Property(e => e.UsedSpace).IsModified = true;

        //    Shelf shelf = this.db.Shelfs.Find(shelfSubdivision.IDShelf);
        //    shelf.UsedSpace = shelf.UsedSpace + UsedSpaceInM;
        //    this.db.Shelfs.Attach(shelf);
        //    var entry6 = db.Entry(shelf);
        //    entry6.Property(e => e.UsedSpace).IsModified = true;

        //    Rack rack = this.db.Racks.Find(shelf.IDRack);
        //    rack.UsedSpace = rack.UsedSpace + UsedSpaceInM;
        //    this.db.Racks.Attach(rack);
        //    var entry = db.Entry(rack);
        //    entry.Property(e => e.UsedSpace).IsModified = true;

        //    Hall hall = this.db.Halls.Find(rack.IDHall);
        //    hall.UsedUsableSpace = hall.UsedUsableSpace + UsedSpaceInM;
        //    this.db.Halls.Attach(hall);
        //    var entry2 = db.Entry(hall);
        //    entry2.Property(e => e.UsedUsableSpace).IsModified = true;

        //    Sector sector = this.db.Sectors.Find(hall.IDSector);
        //    sector.UsedUsableSpace = sector.UsedUsableSpace + UsedSpaceInM;
        //    this.db.Sectors.Attach(sector);
        //    var entry3 = db.Entry(hall);
        //    entry3.Property(e => e.UsedUsableSpace).IsModified = true;

        //    Deposit depo = this.db.Deposits.Find(sector.IDDeposit);
        //    depo.UsedUsableSpace = depo.UsedUsableSpace + UsedSpaceInM;
        //    this.db.Deposits.Attach(depo);
        //    var entry4 = db.Entry(depo);
        //    entry4.Property(e => e.UsedUsableSpace).IsModified = true;

        //    DistributionCenter dc = this.db.DistributionCenters.Find(depo.IDDistributionCenter);
        //    dc.UsableUsedSpace = dc.UsableUsedSpace + UsedSpaceInM;
        //    this.db.DistributionCenters.Attach(dc);
        //    var entry5 = db.Entry(dc);
        //    entry5.Property(e => e.UsableUsedSpace).IsModified = true;

        //    db.SaveChanges();
        //}
    }
}