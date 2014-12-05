using Penates.Database;
using Penates.Interfaces.Repositories;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.Transactions.Sales;
using Penates.Repositories.Transactions;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Services.Transactions {
    public class SaleService : ISaleService {

        ISaleRepository repository = new SaleRepository();

        ///// <summary> Guarda los datos de un Centro de Distribucion en la base de datos </summary>
        ///// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        ///// <returns>True o false dependiendo si resulta o no la operacion</returns>
        ///// <exception cref="StoredProcedureException">Error del SP</exception>
        ///// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        //public long Save(RackViewModel rack) {
        //    long value = this.repository.Save(rack);
        //    switch (value) {
        //        case -1:
        //            throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
        //                Resources.Resources.RackWArt));
        //        case -2:
        //            var ex2 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
        //            Resources.Resources.RackWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
        //            Resources.Resources.Hall, rack.RackID));
        //            ex2.AttributeName = ReflectionExtension.getVarName(() => rack.RackID);
        //            throw ex2;
        //        case -3:
        //            var ex3 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.RackWArt),
        //                Resources.Errors.NewSpaceTooSmall);
        //            ex3.AttributeName = ReflectionExtension.getVarName(() => rack.Size);
        //            throw ex3;
        //        case -4:
        //            var ex4 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.RackWArt),
        //                Resources.Errors.OperationUnsaccessfull);
        //            ex4.AttributeName = "";
        //            throw ex4;
        //    }
        //    return value;
        //}

        /// <summary>Elimina un centro de Distribuciones</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        public Status Anulate(long saleID) {
            return this.repository.Annulate(saleID);
        }

        /// <summary>Obtiene los datos de un Hall</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>RackViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public SaleViewModel getRackData(long saleID) {
            Sale sale = this.repository.getSaleInfo(saleID);
            if (sale == null) {
                return null;
            }
            SaleViewModel model = new SaleViewModel() { 
                SaleID = saleID,
                BillNumber = sale.BillNumber,
                ClientID = sale.IDClient,
                ClientName = sale.Client != null ? sale.Client.Name : null,
                COT = sale.COT,
                DistributionCenterID = sale.IDDistributionCenter,
                IsSale = sale.IsIncome,
                SaleDate = sale.SaleDate
            };
            model.Boxes = sale.Boxes;
            return model;
        }

        public IQueryable<Sale> getData() {
            return this.repository.getData();
        }

        public List<SaleTableJson> toJsonArray(IQueryable<Sale> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<SaleTableJson> toJsonArray(ICollection<Sale> list) {
            List<SaleTableJson> result = new List<SaleTableJson>();
            SaleTableJson aux;
            foreach (Sale sale in list) {
                aux = new SaleTableJson() {
                    SaleID = sale.SaleID,
                    BillNumber = sale.BillNumber,
                    Client = sale.Client != null ? sale.Client.Name : null,
                    DistributionCenter = sale.IDDistributionCenter,
                    SaleDate = sale.SaleDate.ToShortDateString()
                };
                result.Add(aux);
            }
            return result;
        }

        public IQueryable<Sale> search(IQueryable<Sale> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<Sale> sort(IQueryable<Sale> query, int index, string direction) {
            Sorts sort = this.toSort(index, direction);
            return this.sort(query, sort);
        }

        public IQueryable<Sale> sort(IQueryable<Sale> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        public IQueryable<Sale> filterByDistributionCenter(IQueryable<Sale> query, long? dcID) {
            if (dcID.HasValue) {
                query = this.repository.filterByDistributionCenter(query, dcID.Value);
            }
            return query;
        }

        public IQueryable<Sale> filterByClient(IQueryable<Sale> query, long? clientID) {
            if (clientID.HasValue) {
                query = this.repository.filterByClient(query, clientID.Value);
            }
            return query;
        }

        public IQueryable<Sale> filterByAnulated(IQueryable<Sale> query, int? anulated) {
            if (anulated.HasValue) {
                if(anulated == 0){
                    query = this.repository.filterByAnnulated(query, false);
                } else {
                    if (anulated.Value == 1) {
                        query = this.repository.filterByAnnulated(query, true);
                    }
                }
            }
            return query;
        }

        public IQueryable<Sale> getAutocomplete(string search, long? dcID, long? clientID, bool? anulated) {
            return this.repository.getAutocomplete(search, dcID, clientID, anulated);
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<Sale> query) {
            List<Sale> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (Sale sale in list) {
                aux = new AutocompleteItem() {
                    ID = sale.SaleID,
                    Label = sale.BillNumber,
                    Description = sale.IDDistributionCenter + (sale.Client != null ? " ==> "+sale.Client.Name : "" ),
                    aux = new {
                        DistributionCenterID = sale.IDDistributionCenter,
                        ClientID = sale.IDClient,
                        ClientName = sale.Client != null ? sale.Client.Name : null
                    }
                };
                result.Add(aux);
            }
            return result;
        }

        private Sorts toSort(int sortColumnIndex, string sortDirection) {
            switch (sortColumnIndex) {
                case 0:
                    if (sortDirection == "asc") {
                        return Sorts.ID;
                    } else {
                        return Sorts.ID_DESC;
                    }
                case 1:
                    if (sortDirection == "asc") {
                        return Sorts.BILL;
                    } else {
                        return Sorts.BILL_DESC;
                    }
                case 2:
                    if (sortDirection == "asc") {
                        return Sorts.DISTRIBUTION_CENTER;
                    } else {
                        return Sorts.DISTRIBUTION_CENTER_DESC;
                    }
                case 3:
                    if (sortDirection == "asc") {
                        return Sorts.CLIENT;
                    } else {
                        return Sorts.CLIENT_DESC;
                    }
                case 4:
                    if (sortDirection == "asc") {
                        return Sorts.DATE;
                    } else {
                        return Sorts.DATE_DESC;
                    }
                default:
                    return Sorts.DATE_DESC;
            }
        }
    }
}