using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Exceptions.Views;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using Penates.Utils.Enums;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Penates.Repositories.DC {
    public class DistributionCenterRepository : IDistributionCenterRepository {

        PenatesEntities db = new PenatesEntities();
        public int itemsPerPage {get;set;}

        public DistributionCenterRepository() {
            this.itemsPerPage = 50;
        }

        public DistributionCenterRepository(int itemsPerPage) {
            this.itemsPerPage = itemsPerPage;
        }

        public DistributionCenter getData(long id) {
            try {
                DistributionCenter aux = db.DistributionCenters.Find(id);
                if (aux == null || aux.Deleted) {
                    return null;
                }
                return aux;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        public DistributionCenter getData(long id, bool includeDeleted) {
            if (includeDeleted) {
                try {
                    return db.DistributionCenters.Find(id);
                } catch (Exception e) {
                    throw new DatabaseException(e.Message);
                }
            } else {
                return getData(id);
            }
        }

        public IQueryable<DistributionCenter> getData() {
            try {
                return this.db.DistributionCenters.Select(p => p).Where(p => p.Deleted == false);
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        public bool Delete(long id) 
        {
            DistributionCenter dc = db.DistributionCenters.Find(id);
            if (dc == null || dc.Deleted == true) {
                return false;
            }

            if (dc is InternalDistributionCenter) {
                InternalDistributionCenterRepository repo = new InternalDistributionCenterRepository();
                return repo.Delete((InternalDistributionCenter) dc);
            }

            if (dc is ExternalDistributionCenter) {
                ExternalDistributionCenterRepository repo = new ExternalDistributionCenterRepository();
                return repo.Delete((ExternalDistributionCenter) dc);
            }

            return false;
        }

        public bool checkFisicalDelete(long id) {
            DistributionCenter dc = this.db.DistributionCenters.Find(id);
            return this.checkFisicalDelete(dc);
        }

        public bool checkFisicalDelete(DistributionCenter dc) {
            if(this.getInventoriesConstraintsNumber(dc) == 0 && this.getPoliciesConstraintsNumber(dc) == 0 && this.getReceptionsConstraintsNumber(dc) == 0 &&
                this.getSalesConstraintsNumber(dc) == 0 && this.getTransfersConstraintsNumber(dc) == 0){
                    return true;
                }
            return false;
        }

        public long getReceptionsConstraintsNumber(DistributionCenter dc) {
            try {
                if (dc.Receptions == null) {
                    return 0;
                }
                return dc.Receptions.Count;
            } catch (NullReferenceException) {
                return 0;
            }
        }

        public List<Constraint> getReceptionsConstraints(long centerID) {
            try {
                DistributionCenter dc = this.db.DistributionCenters.Find(centerID);
                var result = dc.Receptions.Take(this.itemsPerPage).ToList();
                var constraints = result.Select(x => new Constraint {
                    id = x.ReceiveID.ToString(),
                    name = Penates.App_GlobalResources.Forms.ModelFormsResources.Description + ": " + x.ReceivingDate
                });
                return constraints.ToList();
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.DistributionCenterWArt, centerID), e.Message);
            }
        }

        public long getTransfersConstraintsNumber(DistributionCenter dc) {
            try {
                if (dc.Transfers == null && dc.TransfersSend == null) {
                    return 0;
                }
                return dc.Transfers.Count;
            } catch (NullReferenceException) {
                return 0;
            }
        }

        public List<Constraint> getTransfersConstraints(long centerID) {
            try {
                DistributionCenter dc = this.db.DistributionCenters.Find(centerID);
                var result = dc.Transfers.Take(this.itemsPerPage).ToList();
                var constraints = result.Select(x => new Constraint {
                    id = x.TransferID.ToString(),
                    name = Penates.App_GlobalResources.Forms.ModelFormsResources.OrderDate + ": " + x.TransferDepartureDate
                });
                return constraints.ToList();
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.ProductWArt, centerID), e.Message);
            }
        }

        public List<Constraint> getTransfers1Constraints(long centerID) {
            try {
                DistributionCenter dc = this.db.DistributionCenters.Find(centerID);
                var result = dc.TransfersSend.Take(this.itemsPerPage).ToList();
                var constraints = result.Select(x => new Constraint {
                    id = x.TransferID.ToString(),
                    name = Penates.App_GlobalResources.Forms.ModelFormsResources.OrderDate + ": " + x.TransferDepartureDate
                });
                return constraints.ToList();
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.ProductWArt, centerID), e.Message);
            }
        }


        public long getInventoriesConstraintsNumber(DistributionCenter dc) {
            try {
                if (dc.Inventories == null) {
                    return 0;
                }
                return dc.Inventories.Count;
            } catch (NullReferenceException) {
                return 0;
            }
        }

        public long getPoliciesConstraintsNumber(DistributionCenter dc) {
            try {
                if (dc.Policies == null) {
                    return 0;
                }
                return dc.Policies.Count;
            } catch (NullReferenceException) {
                return 0;
            }
        }

        public List<Constraint> getPoliciesConstraints(long centerID) {
            try {
                DistributionCenter dc = this.db.DistributionCenters.Find(centerID);
                var result = dc.Policies.Take(this.itemsPerPage).ToList();
                var constraints = result.Select(x => new Constraint {
                    id = x.PoliciesID.ToString(),
                    name = Penates.App_GlobalResources.Forms.ModelFormsResources.Description 
                });
                return constraints.ToList();
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.DistributionCenterWArt, centerID), e.Message);
            }
        }

        public long getSalesConstraintsNumber(DistributionCenter dc) {
            try {
                if (dc.Sales == null) {
                    return 0;
                }
                return dc.Sales.Count;
            } catch (NullReferenceException) {
                return 0;
            }
        }

        public List<Constraint> getSalesConstraints(long centerID) {
            try {
                DistributionCenter dc = this.db.DistributionCenters.Find(centerID);
                var result = dc.Sales.Take(this.itemsPerPage).ToList();
                var constraints = result.Select(x => new Constraint {
                    id = x.SaleID.ToString(),
                    name = Penates.App_GlobalResources.Forms.ModelFormsResources.Description + ": " + x.SaleDate
                });
                return constraints.ToList();
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.DistributionCenterWArt, centerID), e.Message);
            }
        }

        public IQueryable<DistributionCenter> search(IQueryable<DistributionCenter> query, string search) {
            return this.search(query, StringUtils.splitString(search));
        }

        public IQueryable<DistributionCenter> search(IQueryable<DistributionCenter> query, List<string> search) {
            try {
                foreach (string item in search) {
                    query = query.Where(p => p.Address.Contains(item) || p.City.Name.Contains(item) || SqlFunctions.StringConvert((double) p.DistributionCenterID).Contains(item));
                }
                return query;
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<DistributionCenter> countryFilter(IQueryable<DistributionCenter> query, long countryID) {
            return query.Where(p => p.City.Province.Country.CountryID == countryID).Select(p => p);
        }

        public IQueryable<DistributionCenter> stateFilter(IQueryable<DistributionCenter> query, long stateID) {
            return query.Where(p => p.City.Province.ProvinceID == stateID).Select(p => p);
        }

        public IQueryable<DistributionCenter> typeFilter(IQueryable<DistributionCenter> query, long typeID) {
            if (typeID == DistributionCenterTypes.INTERNAL.getTypeNumber()) {
                return query.OfType<InternalDistributionCenter>();
            } else {
                if (typeID == DistributionCenterTypes.EXTERNAL.getTypeNumber()) {
                    return query.OfType<ExternalDistributionCenter>();
                }
            }
            return query;
        }

        public IQueryable<DistributionCenter> sort(IQueryable<DistributionCenter> dc, Sorts sort) {
            switch (sort) { //Ordeno x lo que sea
                case Sorts.ID:
                    dc = dc.OrderBy(x => x.DistributionCenterID);
                    break;
                case Sorts.ID_DESC:
                    dc = dc.OrderByDescending(x => x.DistributionCenterID);
                    break;
                case Sorts.ADDRESS:
                    dc = dc.OrderBy(x => x.Address);
                    break;
                case Sorts.ADDRESS_DESC:
                    dc = dc.OrderByDescending(x => x.Address);
                    break;
                case Sorts.CITY:
                    dc = dc.OrderBy(x => x.City.Name);
                    break;
                case Sorts.CITY_DESC:
                    dc = dc.OrderByDescending(x => x.City.Name);
                    break;
                case Sorts.USEDPERCENT:
                    dc = dc.OrderBy(x => x.UsableUsedSpace);
                    break;
                case Sorts.USEDPERCENT_DESC:
                    dc = dc.OrderByDescending(x => x.UsableUsedSpace);
                    break;
                case Sorts.DEFAULT:
                    dc = dc.OrderBy(x => x.DistributionCenterID);
                    break;
                default:
                    throw new SortException(Resources.ExceptionMessages.InvalidSortException, sort.ToString());
            }
            return dc;
        }


        /// <summary> Checkea si puede eliminar el Producto con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> Id del producto a eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        public bool checkDeleteConstrains(long id) {
            DistributionCenter dc = db.DistributionCenters.Find(id);
            return this.checkDeleteConstrains(dc);
        }

        /// <summary> Checkea si puede eliminar el Producto con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> El Producto a Eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        public bool checkDeleteConstrains(DistributionCenter dc) {
            if (dc.Policies == null) {
                if(dc.Receptions.Where(r => r.ReceivingDate > System.DateTime.Now).Count()==0 && dc.Transfers.Where(t => t.TransferArrivalDate > System.DateTime.Now).Count()==0 &&
                    dc.TransfersSend.Where(t => t.TransferDepartureDate > System.DateTime.Now).Count() == 0 && dc.Sales.Where(s => s.SaleDate > System.DateTime.Now).Count() == 0) {
                        return true;
                    }
            } else {
                if (dc.Policies.Count == 0) {
                    if (dc.Receptions.Where(r => r.ReceivingDate > System.DateTime.Now).Count() == 0 && dc.Transfers.Where(t => t.TransferArrivalDate > System.DateTime.Now).Count() == 0 &&
                        dc.TransfersSend.Where(t => t.TransferDepartureDate > System.DateTime.Now).Count() == 0 && dc.Sales.Where(s => s.SaleDate > System.DateTime.Now).Count() == 0) {
                        return true;
                    }
                }
            }
            return false;
        }

        public IQueryable<DistributionCenter> getAutocomplete(string search) {
            try {
                var data = this.searchAndRank(search);
                return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
            } catch (DatabaseException de) {
                throw de;
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        public IQueryable<DistributionCenter> searchAndRank(string search) {
            var data = this.getData();
            return this.searchAndRank(data, search);
        }

        public IQueryable<DistributionCenter> searchAndRank(IQueryable<DistributionCenter> data, string search) {
            try {
                if (String.IsNullOrWhiteSpace(search)) {
                    data = data.OrderBy(x => x.DistributionCenterID);
                    return data;
                }
                List<string> searches = StringUtils.splitString(search);
                var aux = data.Select(x => new PageRankItem<DistributionCenter> {
                    table = x,
                    rankPoints = 0
                });
                foreach (string item in searches) {
                    aux = aux.Select(x => new PageRankItem<DistributionCenter> {
                        table = x.table,
                        rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double) x.table.DistributionCenterID).Trim() == item ? 1000 : 0) +
                                ((x.table.Address.Contains(item)) ? item.Length*2 : 0) + ((x.table.Address.StartsWith(item)) ? (item.Length * 4) : 0) +
                                ((x.table.City.Name.Contains(item)) ? item.Length : 0) + ((x.table.City.Name.StartsWith(item)) ? (item.Length * 2) : 0)
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

        internal bool deleteNotifications(DistributionCenter dc) {
            try {
                foreach (Notification not in dc.Notifications) {
                    this.db.Notifications.Remove(not);
                }
                this.db.SaveChanges();
                return true;
            }catch(UpdateException){
                return false;
            }
        }
    }
}