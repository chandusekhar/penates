using Penates.Database;
using Penates.Exceptions;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.DC;
using Penates.Repositories.DC;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Services.DC {
    public class TemporaryDepositService : ITemporaryDepositService {
        ITemporaryDepositRepository repository = new TemporaryDepositRepository();

        /// <summary> Guarda los datos de un Centro de Distribucion en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        public long Save(DepositViewModel dc) {
            long value = this.repository.Save(dc);
            switch (value) {
                case -1:
                    throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.TemporaryDepositWArt));
                case -2:
                    var ex2 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.TemporaryDepositWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.DistributionCenter, dc.DistributionCenterID));
                    ex2.AttributeName = ReflectionExtension.getVarName(() => dc.DistributionCenterID);
                    throw ex2;
                case -3:
                    var ex3 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.TemporaryDepositWArt),
                        Resources.Errors.NewSpaceTooSmall);
                    ex3.AttributeName = ReflectionExtension.getVarName(() => dc.Size);
                    throw ex3;
            }
            return value;
        }

        /// <summary>Elimina un centro de Distribuciones</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        public bool Delete(long dcID) {
            return this.repository.Delete(dcID);
        }

        /// <summary>Obtiene los datos de un Deposito Temporal</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public DepositViewModel getDepositData(long depositID) {
            TemporaryDeposit temporaryDeposit = this.repository.getDepositInfo(depositID);
            if (temporaryDeposit == null) {
                return null;
            }
            DepositViewModel model = new DepositViewModel() {
                DepositID = temporaryDeposit.TemporaryDepositID,
                Depth = temporaryDeposit.Depth,
                Description = temporaryDeposit.Description,
                DistributionCenterID = temporaryDeposit.IDDistributionCenter,
                Height = temporaryDeposit.Height,
                Size = temporaryDeposit.Size.HasValue ? temporaryDeposit.Size.Value : (temporaryDeposit.Width * temporaryDeposit.Height * temporaryDeposit.Depth),
                UsedSpace = temporaryDeposit.UsedSpace,
                Width = temporaryDeposit.Width
            };
            if (temporaryDeposit.Floor.HasValue) {
                model.Floor = temporaryDeposit.Floor.Value;
            }
            model.setPrecentages();
            return model;
        }

        public IQueryable<TemporaryDeposit> getData(long? dcID) {
            if (!dcID.HasValue) {
                return new List<TemporaryDeposit>().AsQueryable();
            }
            return this.repository.getData(dcID.Value);
        }

        public List<DepositTableJson> toJsonArray(IQueryable<TemporaryDeposit> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<DepositTableJson> toJsonArray(ICollection<TemporaryDeposit> list) {
            List<DepositTableJson> result = new List<DepositTableJson>();
            DepositTableJson aux;
            foreach (TemporaryDeposit depo in list) {
                aux = new DepositTableJson() {
                    DepositID = depo.TemporaryDepositID,
                    Description = depo.Description,
                    Floor = (depo.Floor.HasValue) ? depo.Floor.Value : 0
                };
                if (depo.Size == 0) {
                    aux.UsedPercentage = 0;
                } else {
                    aux.UsedPercentage = Math.Round(((depo.UsedSpace / depo.Size.Value) * 100),2);
                }
                result.Add(aux);
            }
            return result;
        }

        public IQueryable<TemporaryDeposit> search(IQueryable<TemporaryDeposit> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<TemporaryDeposit> sort(IQueryable<TemporaryDeposit> query, int index, string direction) {
            Sorts sort = this.toSort(index, direction);
            return this.sort(query, sort);
        }

        public IQueryable<TemporaryDeposit> sort(IQueryable<TemporaryDeposit> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        public IQueryable<TemporaryDeposit> getAutocomplete(string search, long? dcID) {
            return this.repository.getAutocomplete(search, dcID);
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<TemporaryDeposit> query) {
            List<TemporaryDeposit> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (TemporaryDeposit deposit in list) {
                aux = new AutocompleteItem() {
                    ID = deposit.TemporaryDepositID,
                    Label = deposit.Description,
                    Description = Resources.Resources.DistributionCenter + ": " + deposit.IDDistributionCenter,
                    aux = new { DistributionCenterID = deposit.IDDistributionCenter, Height = deposit.Height }
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
                        return Sorts.DESCRIPTION;
                    } else {
                        return Sorts.DESCRIPTION_DESC;
                    }
                case 2:
                    if (sortDirection == "asc") {
                        return Sorts.FLOOR;
                    } else {
                        return Sorts.FLOOR_DESC;
                    }
                case 3:
                    if (sortDirection == "asc") {
                        return Sorts.USEDPERCENT;
                    } else {
                        return Sorts.USEDPERCENT_DESC;
                    }
                default:
                    return Sorts.ID;
            }
        }


        public TemporaryDeposit findDepositWithSpace(long DistributionCenterID, decimal SpaceNeededInM3)
        {
            IQueryable<TemporaryDeposit> deposits = getData(DistributionCenterID);
            foreach (TemporaryDeposit deposit in deposits)
            {
                if (deposit.Size - deposit.UsedSpace >= SpaceNeededInM3)
                {
                    return deposit;
                }
            }
            return null; 
        }


        public void increaseUsedSpace(long DepositID, decimal SpaceInM3)
        {
            this.repository.increaseUsedSpace(DepositID, SpaceInM3);
        }
    }
}