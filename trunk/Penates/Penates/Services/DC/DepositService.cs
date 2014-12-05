using Penates.Database;
using Penates.Exceptions;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.DC;
using Penates.Repositories.ABMs;
using Penates.Repositories.DC;
using Penates.Utils;
using Penates.Utils.JSON;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Services.DC {
    public class DepositService : IDepositService {
        IDepositRepository repository = new DepositRepository();

        /// <summary> Guarda los datos de un Centro de Distribucion en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        public long Save(DepositViewModel depo) {
            long value = this.repository.Save(depo);
            switch (value) {
                case -1:
                    throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.DepositWArt));
                case -2:
                    var ex2 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.DepositWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.DistributionCenter, depo.DistributionCenterID));
                    ex2.AttributeName = ReflectionExtension.getVarName(() => depo.DistributionCenterID);
                    throw ex2;
                case -3:
                    var ex3 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.DepositWArt),
                        Resources.Errors.NewSpaceTooSmall);
                    ex3.AttributeName = ReflectionExtension.getVarName(() => depo.Size);
                    throw ex3;
            }
            return value;
        }

        /// <summary>Elimina un centro de Distribuciones</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        public bool Delete(long depositID) {
            return this.repository.Delete(depositID);
        }

        /// <summary>Obtiene los datos de un Deposito Temporal</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public DepositViewModel getDepositData(long depositID) {
            Deposit deposit = this.repository.getDepositInfo(depositID);
            if (deposit == null) {
                return null;
            }
            DepositViewModel model = this.getDepositViewModel(deposit);
            List<SelectItem> categories = new List<SelectItem>();
            SelectItem item;
            model.Categories = null;
            foreach (ProductCategory category in deposit.ProductCategories) {
                if (!String.IsNullOrEmpty(model.Categories)) {
                    model.Categories = model.Categories + ",";
                }
                model.Categories = model.Categories + category.ProductCategoriesID;
                item = new SelectItem {
                    id = category.ProductCategoriesID,
                    label = category.Description
                };
                categories.Add(item);
            }
            model.initialCategories = categories;
            return model;
        }

        /// <summary>Obtiene los datos de un Deposito Temporal, pero con los nombres de las categorias en lugar de los IDs</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public DepositViewModel getDepositSummery(long depositID) {
            Deposit deposit = this.repository.getDepositInfo(depositID);
            if (deposit == null) {
                return null;
            }
            DepositViewModel model = this.getDepositViewModel(deposit);
            foreach (ProductCategory category in deposit.ProductCategories) {
                if (model.Categories != null) { //si no es la primera vez
                    model.Categories = model.Categories + ", ";
                }
                model.Categories = model.Categories + category.Description;
            }
            return model;
        }

        public IQueryable<Deposit> getData(long? depositID) {
            if (!depositID.HasValue) {
                return new List<Deposit>().AsQueryable();
            }
            return this.repository.getData(depositID.Value);
        }

        public List<DepositTableJson> toJsonArray(IQueryable<Deposit> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<DepositTableJson> toJsonArray(ICollection<Deposit> list) {
            List<DepositTableJson> result = new List<DepositTableJson>();
            DepositTableJson aux;
            foreach (Deposit depo in list) {
                aux = new DepositTableJson() {
                    DepositID = depo.DepositID,
                    Description = depo.Description,
                    Floor = (depo.Floor.HasValue) ? depo.Floor.Value : 0,
                    UsablePercentage = Math.Round((depo.UsableSpace == 0 ? 100 : (depo.UsedUsableSpace / depo.UsableSpace) * 100),2)
                };
                if (depo.Size == 0) {
                    aux.UsedPercentage = 100;
                } else {
                    aux.UsedPercentage = Math.Round(((depo.UsedSpace / depo.Size.Value) * 100),2);
                }
                result.Add(aux);
            }
            return result;
        }

        public IQueryable<Deposit> search(IQueryable<Deposit> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<Deposit> sort(IQueryable<Deposit> query, int index, string direction) {
            Sorts sort = this.toSort(index, direction);
            return this.sort(query, sort);
        }

        public IQueryable<Deposit> sort(IQueryable<Deposit> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool addCategory(long depositID, long categoryID) {
            return this.repository.addCategory(depositID, categoryID);
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool unnasignCategory(long depositID, long categoryID) {
            return this.repository.unnasignCategory(depositID, categoryID);
        }

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool saveCategories(long depositID, string categoryiesIDs) {
            if (String.IsNullOrWhiteSpace(categoryiesIDs)) {
                return this.repository.deleteCategories(depositID);
            }
            categoryiesIDs = categoryiesIDs.Trim();
            List<string> categories = StringUtils.split(categoryiesIDs, ',');
            List<long> ids = new List<long>();
            foreach (string s in categories) {
                ids.Add(long.Parse(s));
            }
            return this.repository.saveCategories(depositID, ids);
        }

        public IQueryable<Deposit> getAutocomplete(string search, long? dcID) {
            return this.repository.getAutocomplete(search, dcID);
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<Deposit> query) {
            List<Deposit> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (Deposit deposit in list) {
                aux = new AutocompleteItem() {
                    ID = deposit.DepositID,
                    Label = deposit.Description,
                    Description = Resources.Resources.DistributionCenter + ": " + deposit.IDDistributionCenter,
                    aux = new {DistributionCenterID = deposit.IDDistributionCenter, Height = deposit.Height}
                };
                result.Add(aux);
            }
            return result;
        }

        public string getDepositDescription(long depositID) {
            Deposit dep = this.repository.getDepositInfo(depositID);
            if (dep == null) {
                return null;
            }
            return dep.Description;
        }

        public InternalDistributionCenter getDistributionCenter(long depositID) {
            Deposit dep = this.repository.getDepositInfo(depositID);
            if (dep == null) {
                return null;
            }
            return dep.InternalDistributionCenter;
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

                /// <summary>Obtiene los datos de un Deposito Temporal</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        private DepositViewModel getDepositViewModel(Deposit deposit) {
            DepositViewModel model = new DepositViewModel() {
                DepositID = deposit.DepositID,
                Depth = deposit.Depth,
                Description = deposit.Description,
                DistributionCenterID = deposit.IDDistributionCenter,
                Height = deposit.Height,
                Size = deposit.Size.HasValue ? Math.Round(deposit.Size.Value,2) : Math.Round((deposit.Width * deposit.Height * deposit.Depth),2),
                UsedSpace = deposit.UsedSpace,
                Width = deposit.Width,
                UsableSpace = deposit.UsableSpace,
                UsableUsedSpace = deposit.UsedUsableSpace
            };
            if (deposit.Floor.HasValue) {
                model.Floor = deposit.Floor.Value;
            }
            model.Categories = null;
            model.setPrecentages();
            return model;
        }


        public IQueryable<Sector> getSectors(long depositId)
        { 
            return this.repository.getSectors(depositId); 
        }
    }
}