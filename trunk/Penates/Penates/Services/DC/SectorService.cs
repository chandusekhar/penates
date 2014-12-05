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
    public class SectorService : ISectorService {

        ISectorRepository repository = new SectorRepository();

        /// <summary> Guarda los datos de un Centro de Distribucion en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        public long Save(SectorViewModel sector) {
            long value = this.repository.Save(sector);
            switch (value) {
                case -1:
                    throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.SectorWArt));
                case -2:
                    var ex2 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.SectorWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.Deposit, sector.DepositID));
                    ex2.AttributeName = ReflectionExtension.getVarName(() => sector.DepositID);
                    throw ex2;
                case -3:
                    var ex3 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.SectorWArt),
                        Resources.Errors.NewSpaceTooSmall);
                    ex3.AttributeName = ReflectionExtension.getVarName(() => sector.Size);
                    throw ex3;
            }
            return value;
        }

        /// <summary>Elimina un centro de Distribuciones</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        public bool Delete(long sectorID) {
            return this.repository.Delete(sectorID);
        }

        /// <summary>Obtiene los datos de un Deposito Temporal</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>SectorViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public SectorViewModel getSectorData(long sectorID) {
            Sector sector = this.repository.getSectorInfo(sectorID);
            if (sector == null) {
                return null;
            }
            SectorViewModel model = this.getSectorViewModel(sector);
            List<SelectItem> categories = new List<SelectItem>();
            SelectItem item;
            model.Categories = null;
            foreach (ProductCategory category in sector.ProductCategories) {
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

        /// <summary>Obtiene los datos de un Sector, pero con los nombres de las categorias en lugar de los IDs</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public SectorViewModel getSectorSummary(long sectorID) {
            Sector sector = this.repository.getSectorInfo(sectorID);
            if (sector == null) {
                return null;
            }
            SectorViewModel model = this.getSectorViewModel(sector);
            foreach (ProductCategory category in sector.ProductCategories) {
                if (model.Categories != null) { //si no es la primera vez
                    model.Categories = model.Categories + ", ";
                }
                model.Categories = model.Categories + category.Description;
            }
            return model;
        }

        public IQueryable<Sector> getData() {
            return this.repository.getData();
        }

        public List<SectorTableJson> toJsonArray(IQueryable<Sector> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<SectorTableJson> toJsonArray(ICollection<Sector> list) {
            List<SectorTableJson> result = new List<SectorTableJson>();
            SectorTableJson aux;
            foreach (Sector sector in list) {
                aux = new SectorTableJson() {
                    SectorID = sector.SectorID,
                    Description = sector.Description,
                    Deposit = sector.Deposit.Description,
                    DistributionCenter = sector.Deposit.IDDistributionCenter,
                    UsedPercentage = Math.Round((sector.Size == 0 ? 0 : ((sector.UsedSpace / sector.Size) * 100)),2),
                    UsablePercentage = Math.Round((sector.UsableSpace == 0 ? 100 : (sector.UsedUsableSpace / sector.UsableSpace) * 100),2)
                };
                result.Add(aux);
            }
            return result;
        }

        public IQueryable<Sector> search(IQueryable<Sector> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<Sector> sort(IQueryable<Sector> query, int index, string direction) {
            Sorts sort = this.toSort(index, direction);
            return this.sort(query, sort);
        }

        public IQueryable<Sector> sort(IQueryable<Sector> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        public IQueryable<Sector> filterByDistributionCenter(IQueryable<Sector> query, long? dcID) {
            if (dcID.HasValue) {
                query = this.repository.filterByDistributionCenter(query, dcID.Value);
            }
            return query;
        }

        public IQueryable<Sector> filterByDeposit(IQueryable<Sector> query, long? depositID) {
            if (depositID.HasValue) {
                query = this.repository.filterByDeposit(query, depositID.Value);
            }
            return query;
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool addCategory(long sectorID, long categoryID) {
            return this.repository.addCategory(sectorID, categoryID);
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool unnasignCategory(long sectorID, long categoryID) {
            return this.repository.unnasignCategory(sectorID, categoryID);
        }

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool saveCategories(long sectorID, string categoryiesIDs) {
            if (String.IsNullOrWhiteSpace(categoryiesIDs)) {
                Sector sector = this.repository.getSectorInfo(sectorID);
                if(sector != null){
                    ICollection<ProductCategory> cats = sector.Deposit.ProductCategories;
                    bool error = true;
                    foreach(ProductCategory category in cats){
                        if (!this.repository.addCategory(sectorID, category.ProductCategoriesID)) {
                            error = false;
                        }
                    }
                    return error;
                }
                return this.repository.deleteCategories(sectorID);
            }
            categoryiesIDs = categoryiesIDs.Trim();
            List<string> categories = StringUtils.split(categoryiesIDs, ',');
            List<long> ids = new List<long>();
            foreach (string s in categories) {
                ids.Add(long.Parse(s));
            }
            return this.repository.saveCategories(sectorID, ids);
        }

        public IQueryable<Sector> getAutocomplete(string search, long? dcID, long? depositID) {
            return this.repository.getAutocomplete(search, dcID, depositID);
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<Sector> query) {
            List<Sector> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (Sector sector in list) {
                aux = new AutocompleteItem() {
                    ID = sector.SectorID,
                    Label = sector.Description,
                    Description = Resources.Resources.DistributionCenter + ": " + sector.Deposit.IDDistributionCenter,
                    aux = new { DistributionCenterID = sector.Deposit.IDDistributionCenter, DepositID = sector.IDDeposit, DepositName = sector.Deposit.Description, Height = sector.Deposit.Height }
                };
                result.Add(aux);
            }
            return result;
        }

        public string getSectorDescription(long sectorID) {
            Sector sec = this.repository.getSectorInfo(sectorID);
            if (sec == null) {
                return null;
            }
            return sec.Description;
        }

        public long? getDistributionCenter(long sectorID) {
            Sector sec = this.repository.getSectorInfo(sectorID);
            if (sec == null) {
                return null;
            }
            return sec.Deposit.IDDistributionCenter;
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
                        return Sorts.DISTRIBUTION_CENTER;
                    } else {
                        return Sorts.DISTRIBUTION_CENTER_DESC;
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
        private SectorViewModel getSectorViewModel(Sector sector) {
            SectorViewModel model = new SectorViewModel() {
                SectorID = sector.SectorID,
                DepositID = sector.IDDeposit,
                DepositName = sector.Deposit.Description,
                Depth = sector.Depth,
                Description = sector.Description,
                Height = sector.Deposit.Height,
                Size = sector.Size,
                UsableSpace = sector.UsableSpace,
                UsableUsedSpace = sector.UsedUsableSpace,
                UsedSpace = sector.UsedSpace,
                Width = sector.Width, 
                MaxDepth = sector.Deposit.Depth,
                MaxSize = sector.Deposit.Size.HasValue ? (sector.Deposit.Size.Value - sector.Deposit.UsedSpace) : default(decimal?),
                MaxWidth = sector.Deposit.Width,
                DistributionCenter = sector.Deposit.IDDistributionCenter
            };
            model.Categories = null;
            model.setPrecentages();
            return model;
        }

        public IQueryable<ProductCategory> getCategoryAutocomplete(string search, long? DepositID) {
            IProductCategoryRepository categoryRepo = new ProductCategoryRepository();
            IQueryable<ProductCategory> query;
            if (DepositID.HasValue) {
                IDepositRepository depositRepo = new DepositRepository();
                Deposit depo = depositRepo.getDepositInfo(DepositID.Value);
                if (depo.ProductCategories == null || depo.ProductCategories.Count == 0) {
                    query = categoryRepo.getData();
                    query = categoryRepo.searchAndRank(query, search);
                } else {
                    ICollection<ProductCategory> list = categoryRepo.getAssignableCategories(depo.ProductCategories);
                    query = categoryRepo.searchAndRank(list, search).AsQueryable();
                }
            } else {
                query = categoryRepo.getData();
                query = categoryRepo.searchAndRank(query, search);
            }
            return query.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
        }

        public IQueryable<Hall> getHalls(long SectorID)
        {
            return this.repository.getHalls(SectorID);
        }
    }
}