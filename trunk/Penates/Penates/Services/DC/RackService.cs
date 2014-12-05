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
    public class RackService : IRackService {

        IRackRepository repository = new RackRepository();

        /// <summary> Guarda los datos de un Centro de Distribucion en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        public long Save(RackViewModel rack) {
            long value = this.repository.Save(rack);
            switch (value) {
                case -1:
                    throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.RackWArt));
                case -2:
                    var ex2 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.RackWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.Hall, rack.RackID));
                    ex2.AttributeName = ReflectionExtension.getVarName(() => rack.RackID);
                    throw ex2;
                case -3:
                    var ex3 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.RackWArt),
                        Resources.Errors.NewSpaceTooSmall);
                    ex3.AttributeName = ReflectionExtension.getVarName(() => rack.Size);
                    throw ex3;
                case -4:
                    var ex4 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.RackWArt),
                        Resources.Errors.OperationUnsaccessfull);
                    ex4.AttributeName = "";
                    throw ex4;
            }
            return value;
        }

        /// <summary>Elimina un centro de Distribuciones</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        public bool Delete(long rackID) {
            return this.repository.Delete(rackID);
        }

        /// <summary>Obtiene los datos de un Hall</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>RackViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public RackViewModel getRackData(long rackID) {
            Rack rack = this.repository.getRackInfo(rackID);
            if (rack == null) {
                return null;
            }
            RackViewModel model = this.getRackViewModel(rack);
            List<SelectItem> categories = new List<SelectItem>();
            SelectItem item;
            model.Categories = null;
            foreach (ProductCategory category in rack.ProductCategories) {
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
        public RackViewModel getRackSummary(long rackID) {
            Rack rack = this.repository.getRackInfo(rackID);
            if (rack == null) {
                return null;
            }
            RackViewModel model = this.getRackViewModel(rack);
            foreach (ProductCategory category in rack.ProductCategories) {
                if (model.Categories != null) { //si no es la primera vez
                    model.Categories = model.Categories + ", ";
                }
                model.Categories = model.Categories + category.Description;
            }
            return model;
        }

        public IQueryable<Rack> getData() {
            return this.repository.getData();
        }

        public List<RackTableJson> toJsonArray(IQueryable<Rack> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<RackTableJson> toJsonArray(ICollection<Rack> list) {
            List<RackTableJson> result = new List<RackTableJson>();
            RackTableJson aux;
            foreach (Rack rack in list) {
                aux = new RackTableJson() {
                    RackID = rack.RackID,
                    Description = rack.Description,
                    RackCode = rack.RackCode,
                    Deposit = rack.Hall.Sector.Deposit.Description,
                    UsedPercentage = Math.Round((rack.Size.HasValue ? ((rack.UsedSpace / rack.Size.Value) * 100) : 0),2)
                };
                result.Add(aux);
            }
            return result;
        }

        public IQueryable<Rack> search(IQueryable<Rack> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<Rack> sort(IQueryable<Rack> query, int index, string direction) {
            Sorts sort = this.toSort(index, direction);
            return this.sort(query, sort);
        }

        public IQueryable<Rack> sort(IQueryable<Rack> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        public IQueryable<Rack> filterByDistributionCenter(IQueryable<Rack> query, long? dcID) {
            if (dcID.HasValue) {
                query = this.repository.filterByDistributionCenter(query, dcID.Value);
            }
            return query;
        }

        public IQueryable<Rack> filterByDeposit(IQueryable<Rack> query, long? depositID) {
            if (depositID.HasValue) {
                query = this.repository.filterByDeposit(query, depositID.Value);
            }
            return query;
        }

        public IQueryable<Rack> filterBySector(IQueryable<Rack> query, long? sectorID) {
            if (sectorID.HasValue) {
                query = this.repository.filterBySector(query, sectorID.Value);
            }
            return query;
        }

        public IQueryable<Rack> filterByHall(IQueryable<Rack> query, long? hallID) {
            if (hallID.HasValue) {
                query = this.repository.filterByHall(query, hallID.Value);
            }
            return query;
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool addCategory(long rackID, long categoryID) {
            return this.repository.addCategory(rackID, categoryID);
        }

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool unnasignCategory(long rackID, long categoryID) {
            return this.repository.unnasignCategory(rackID, categoryID);
        }

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        public bool saveCategories(long rackID, string categoryiesIDs) {
            if (String.IsNullOrWhiteSpace(categoryiesIDs)) {
                Rack rack = this.repository.getRackInfo(rackID);
                if (rack != null) {
                    ICollection<ProductCategory> cats = rack.Hall.ProductCategories;
                    bool error = true;
                    foreach (ProductCategory category in cats) {
                        if (!this.repository.addCategory(rackID, category.ProductCategoriesID)) {
                            error = false;
                        }
                    }
                    return error;
                }
                return this.repository.deleteCategories(rackID);
            }
            categoryiesIDs = categoryiesIDs.Trim();
            List<string> categories = StringUtils.split(categoryiesIDs, ',');
            List<long> ids = new List<long>();
            foreach (string s in categories) {
                ids.Add(long.Parse(s));
            }
            return this.repository.saveCategories(rackID, ids);
        }

        public IQueryable<Rack> getAutocomplete(string search, long? dcID, long? depositID, long? sectorID, long? hallID) {
            return this.repository.getAutocomplete(search, dcID, depositID, sectorID, hallID);
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<Rack> query) {
            List<Rack> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (Rack rack in list) {
                aux = new AutocompleteItem() {
                    ID = rack.RackID,
                    Label = rack.RackCode,
                    Description = rack.Description,
                    aux = new {
                        DistributionCenterID = rack.Hall.Sector.Deposit.IDDistributionCenter,
                        DepositID = rack.Hall.Sector.IDDeposit,
                        DepositName = rack.Hall.Sector.Deposit.Description,
                        SectorID = rack.Hall.IDSector,
                        SectorName = rack.Hall.Sector.Description,
                        HallID = rack.IDHall,
                        HallName = rack.Hall.Description
                    }
                };
                result.Add(aux);
            }
            return result;
        }

        public string getRackDescription(long rackID) {
            Rack rack = this.repository.getRackInfo(rackID);
            if (rack == null) {
                return null;
            }
            return rack.Description;
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
                        return Sorts.CODE;
                    } else {
                        return Sorts.CODE_DESC;
                    }
                case 2:
                    if (sortDirection == "asc") {
                        return Sorts.DESCRIPTION;
                    } else {
                        return Sorts.DESCRIPTION_DESC;
                    }
                case 3:
                    if (sortDirection == "asc") {
                        return Sorts.DISTRIBUTION_CENTER;
                    } else {
                        return Sorts.DISTRIBUTION_CENTER_DESC;
                    }
                case 4:
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
        private RackViewModel getRackViewModel(Rack rack) {
            RackViewModel model = new RackViewModel() {
                RackID = rack.RackID,
                SectorID = rack.Hall.IDSector,
                SectorName = rack.Hall.Sector.Description,
                DepositID = rack.Hall.Sector.IDDeposit,
                DepositName = rack.Hall.Sector.Deposit.Description,
                Depth = rack.Depth,
                Description = rack.Description,
                Height = rack.Height,
                Size = rack.Size.HasValue ? rack.Size.Value : (rack.Width * rack.Height * rack.Depth),
                UsedSpace = rack.UsedSpace,
                Width = rack.Width,
                MaxDepth = rack.Hall.Depth,
                MaxHeight = rack.Hall.Sector.Deposit.Height,
                MaxSize = rack.Hall.Size - rack.Hall.UsedSpace,
                MaxWidth = rack.Hall.Width,
                DistributionCenter = rack.Hall.Sector.Deposit.IDDistributionCenter,
                DivitionsNumber = rack.Shelfs.Count == 0 ? 0 : rack.Shelfs.FirstOrDefault().ShelfsSubdivisions.Count,
                HallID = rack.IDHall,
                HallName = rack.Hall.Description,
                RackCode = rack.RackCode,
                ShelfsNumber = rack.Shelfs.Count,
            };
            model.Categories = null;
            if (model.ShelfsNumber.HasValue && model.ShelfsNumber.Value > 0) {
                model.ShelfHeight = model.Height / model.ShelfsNumber.Value;
            }
            if (model.DivitionsNumber.HasValue && model.DivitionsNumber.Value > 0) {
                model.DivitionWidth = model.Width / model.DivitionsNumber.Value;
            }
            model.setPrecentages();
            return model;
        }

        public IQueryable<ProductCategory> getCategoryAutocomplete(string search, long? HallID) {
            IProductCategoryRepository categoryRepo = new ProductCategoryRepository();
            IQueryable<ProductCategory> query;
            if (HallID.HasValue) {
                IHallRepository hallRepo = new HallRepository();
                Hall hall = hallRepo.getHallInfo(HallID.Value);
                if (hall.ProductCategories == null || hall.ProductCategories.Count == 0) {
                    query = categoryRepo.getData();
                    query = categoryRepo.searchAndRank(query, search);
                } else {
                    ICollection<ProductCategory> list = categoryRepo.getAssignableCategories(hall.ProductCategories);
                    query = categoryRepo.searchAndRank(list, search).AsQueryable();
                }
            } else {
                query = categoryRepo.getData();
                query = categoryRepo.searchAndRank(query, search);
            }
            return query.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
        }


        public void updateUsedSpace(long ShelfSubdivisionID, decimal UsedSpaceInCm)
        {
            this.repository.updateUsedSpace(ShelfSubdivisionID, UsedSpaceInCm);
        }
    }
}