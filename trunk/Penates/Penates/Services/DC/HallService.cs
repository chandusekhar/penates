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
    public class HallService : IHallService {

        IHallRepository repository = new HallRepository();

        /// <summary> Guarda los datos de un Centro de Distribucion en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        public long Save(HallViewModel hall) {
            long value = this.repository.Save(hall);
            switch (value) {
                case -1:
                    throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.HallWArt));
                case -2:
                    var ex2 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.HallWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.Sector, hall.SectorID));
                    ex2.AttributeName = ReflectionExtension.getVarName(() => hall.SectorID);
                    throw ex2;
                case -3:
                    var ex3 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.HallWArt),
                        Resources.Errors.NewSpaceTooSmall);
                    ex3.AttributeName = ReflectionExtension.getVarName(() => hall.Size);
                    throw ex3;
            }
            return value;
        }

        /// <summary>Elimina un centro de Distribuciones</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        public bool Delete(long hallID) {
            return this.repository.Delete(hallID);
        }

        /// <summary>Obtiene los datos de un Hall</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>HallViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public HallViewModel getHallData(long hallID) {
            Hall hall = this.repository.getHallInfo(hallID);
            if (hall == null) {
                return null;
            }
            HallViewModel model = this.getHallViewModel(hall);
            List<SelectItem> categories = new List<SelectItem>();
            SelectItem item;
            model.Categories = null;
            foreach (ProductCategory category in hall.ProductCategories) {
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
        public HallViewModel getHallSummary(long hallID) {
            Hall hall = this.repository.getHallInfo(hallID);
            if (hall == null) {
                return null;
            }
            HallViewModel model = this.getHallViewModel(hall);
            foreach (ProductCategory category in hall.ProductCategories) {
                if (model.Categories != null) { //si no es la primera vez
                    model.Categories = model.Categories + ", ";
                }
                model.Categories = model.Categories + category.Description;
            }
            return model;
        }

        public IQueryable<Hall> getData() {
            return this.repository.getData();
        }

        public List<HallTableJson> toJsonArray(IQueryable<Hall> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<HallTableJson> toJsonArray(ICollection<Hall> list) {
            List<HallTableJson> result = new List<HallTableJson>();
            HallTableJson aux;
            foreach (Hall hall in list) {
                aux = new HallTableJson() {
                    HallID = hall.HallID,
                    Description = hall.Description,
                    DistributionCenter = hall.Sector.Deposit.IDDistributionCenter,
                    UsedPercentage = Math.Round((hall.Size == 0 ? 100 : ((hall.UsedSpace / hall.Size) * 100)),2),
                    UsablePercentage = Math.Round((hall.UsableSpace == 0 ? 100 : (hall.UsedUsableSpace / hall.UsableSpace) * 100),2)
                };
                result.Add(aux);
            }
            return result;
        }

        public IQueryable<Hall> search(IQueryable<Hall> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<Hall> sort(IQueryable<Hall> query, int index, string direction) {
            Sorts sort = this.toSort(index, direction);
            return this.sort(query, sort);
        }

        public IQueryable<Hall> sort(IQueryable<Hall> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        public IQueryable<Hall> filterByDistributionCenter(IQueryable<Hall> query, long? dcID) {
            if (dcID.HasValue) {
                query = this.repository.filterByDistributionCenter(query, dcID.Value);
            }
            return query;
        }

        public IQueryable<Hall> filterByDeposit(IQueryable<Hall> query, long? depositID) {
            if (depositID.HasValue) {
                query = this.repository.filterByDeposit(query, depositID.Value);
            }
            return query;
        }

        public IQueryable<Hall> filterBySector(IQueryable<Hall> query, long? hallID) {
            if (hallID.HasValue) {
                query = this.repository.filterBySector(query, hallID.Value);
            }
            return query;
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool addCategory(long hallID, long categoryID) {
            return this.repository.addCategory(hallID, categoryID);
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool unnasignCategory(long hallID, long categoryID) {
            return this.repository.unnasignCategory(hallID, categoryID);
        }

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool saveCategories(long hallID, string categoryiesIDs) {
            if (String.IsNullOrWhiteSpace(categoryiesIDs)) {
                Hall hall = this.repository.getHallInfo(hallID);
                if(hall != null){
                    ICollection<ProductCategory> cats = hall.Sector.ProductCategories;
                    bool error = true;
                    foreach(ProductCategory category in cats){
                        if (!this.repository.addCategory(hallID, category.ProductCategoriesID)) {
                            error = false;
                        }
                    }
                    return error;
                }
                return this.repository.deleteCategories(hallID);
            }
            categoryiesIDs = categoryiesIDs.Trim();
            List<string> categories = StringUtils.split(categoryiesIDs, ',');
            List<long> ids = new List<long>();
            foreach (string s in categories) {
                ids.Add(long.Parse(s));
            }
            return this.repository.saveCategories(hallID, ids);
        }

        public IQueryable<Hall> getAutocomplete(string search, long? dcID, long? depositID, long? sectorID) {
            return this.repository.getAutocomplete(search, dcID, depositID, sectorID);
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<Hall> query) {
            List<Hall> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (Hall hall in list) {
                aux = new AutocompleteItem() {
                    ID = hall.HallID,
                    Label = hall.Description,
                    Description = Resources.Resources.Sector + ": " + hall.Sector.Description,
                    aux = new { DistributionCenterID = hall.Sector.Deposit.IDDistributionCenter, 
                        DepositID = hall.Sector.IDDeposit, DepositName = hall.Sector.Deposit.Description,
                        SectorID = hall.IDSector, SectorName = hall.Sector.Description,
                        Height = hall.Sector.Deposit.Height }
                };
                result.Add(aux);
            }
            return result;
        }

        public string getHallDescription(long hallID) {
            Hall hall = this.repository.getHallInfo(hallID);
            if (hall == null) {
                return null;
            }
            return hall.Description;
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
        private HallViewModel getHallViewModel(Hall hall) {
            HallViewModel model = new HallViewModel() {
                HallID = hall.HallID,
                SectorID = hall.IDSector,
                SectorName = hall.Sector.Description,
                DepositID = hall.Sector.IDDeposit,
                DepositName = hall.Sector.Deposit.Description,
                Depth = hall.Depth,
                Description = hall.Description,
                Height = hall.Sector.Deposit.Height,
                Size = hall.Size,
                UsableSpace = hall.UsableSpace,
                UsableUsedSpace = hall.UsedUsableSpace,
                UsedSpace = hall.UsedSpace,
                Width = hall.Width, 
                MaxDepth = hall.Sector.Depth,
                MaxSize = hall.Sector.Size - hall.Sector.UsedSpace,
                MaxWidth = hall.Sector.Width,
                DistributionCenter = hall.Sector.Deposit.IDDistributionCenter
            };
            model.Categories = null;
            model.setPrecentages();
            return model;
        }

        public IQueryable<ProductCategory> getCategoryAutocomplete(string search, long? SectorID) {
            IProductCategoryRepository categoryRepo = new ProductCategoryRepository();
            IQueryable<ProductCategory> query;
            if (SectorID.HasValue) {
                ISectorRepository sectorRepo = new SectorRepository();
                Sector sector = sectorRepo.getSectorInfo(SectorID.Value);
                if (sector.ProductCategories == null || sector.ProductCategories.Count == 0) {
                    query = categoryRepo.getData();
                    query = categoryRepo.searchAndRank(query, search);
                } else {
                    ICollection<ProductCategory> list = categoryRepo.getAssignableCategories(sector.ProductCategories);
                    query = categoryRepo.searchAndRank(list, search).AsQueryable();
                }
            } else {
                query = categoryRepo.getData();
                query = categoryRepo.searchAndRank(query, search);
            }
            return query.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
        }

        public IQueryable<Rack> getRacks(long HallID)
        {
            return this.repository.getRacks(HallID);
        }
    }
}