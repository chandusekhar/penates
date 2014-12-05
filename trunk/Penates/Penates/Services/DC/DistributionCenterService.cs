using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Exceptions.Views;
using Penates.Interfaces.Repositories;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.ABMs;
using Penates.Models.ViewModels.DC;
using Penates.Models.ViewModels.Transactions;
using Penates.Repositories.DC;
using Penates.Services.ABMs;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace Penates.Services.DC {
    public class DistributionCenterService : IDistributionCenterService {

        IDistributionCenterRepository repository = new DistributionCenterRepository();
        IInternalDistributionCenterRepository internalRepo = new InternalDistributionCenterRepository();
        IExternalDistributionCenterRepository externalRepo = new ExternalDistributionCenterRepository();

        /// <summary> Guarda los datos de un Centro de Distribucion en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException"
        /// <exception cref="ForeignKeyConstraintException"
        /// <exception cref="DataRestrictionProcedureException"
        public long SaveInternal(InternalDistributionCenterViewModel dc) {
            long value = this.internalRepo.Save(dc);
            switch (value) {
                case -1:
                    throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.DistributionCenterWArt));
                case -2:
                    var ex2 = new ForeignKeyConstraintException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.DistributionCenterWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.City, dc.CityID));
                    ex2.atributeName = ReflectionExtension.getVarName(() => dc.CityID);
                    throw ex2;
                case -3:
                    var ex3 = new DataRestrictionProcedureException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.DistributionCenterWArt),
                        Resources.Errors.NewSpaceTooSmall);
                    ex3.atributeName = ReflectionExtension.getVarName(() => dc.Size);
                    throw ex3;
            }
            return value;
        }

        /// <summary> Guarda los datos de un Centro de Distribucion en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como ExternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException"
        /// <exception cref="ForeignKeyConstraintException"
        /// <exception cref="DataRestrictionProcedureException"
        public long SaveExternal(ExternalDistributionCenterViewModel dc) {
            long value = this.externalRepo.Save(dc);
            switch (value) {
                case -1:
                    throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.DistributionCenterWArt));
                case -2:
                    var ex2 = new ForeignKeyConstraintException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.DistributionCenterWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.City, dc.CityID));
                    ex2.atributeName = ReflectionExtension.getVarName(() => dc.CityID);
                    throw ex2;
                case -3:
                    var ex3 = new DataRestrictionProcedureException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.DistributionCenterWArt),
                        Resources.Errors.NewSpaceTooSmall);
                    ex3.atributeName = ReflectionExtension.getVarName(() => dc.UsableSpace);
                    throw ex3;
            }
            return value;
        }

        /// <summary>Elimina un centro de Distribuciones</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        public bool Delete(long dcID) 
        {
            return this.repository.Delete(dcID);
        }

        /// <summary>Obtiene los datos de un centro de distribucion Interno</summary>
        /// <param name="dcID"></param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public InternalDistributionCenterViewModel getInternalData(long dcID) {
            InternalDistributionCenter idc = this.internalRepo.getData(dcID);
            if (idc == null) {
                return null;
            }
            InternalDistributionCenterViewModel model = new InternalDistributionCenterViewModel() {
                DistributionCenterID = idc.DistributionCenterID,
                Address = idc.Address,
                CityID = idc.City.CityID,
                CityName = idc.City.Name,
                Depth = idc.Depth, 
                Floors = idc.Floors,
                UsedSpace = idc.UsedSpace,
                Height = idc.Height,
                Telephone = idc.Telephone,
                UsableUsedSpace = idc.UsableUsedSpace,
                UsableSpace = idc.UsableSpace,
                Width = idc.Width,
                CountryID = idc.City.Province.Country.CountryID,
                CountryName = idc.City.Province.Country.Name,
                StateID = idc.City.Province.ProvinceID,
                StateName = idc.City.Province.Name,              
            };
            if(idc.Capacity.HasValue){
                model.Size = idc.Capacity.Value;
            }else{
                model.Size = idc.Height * idc.Width * idc.Depth;
            }
            model.setPrecentages();
            return model;
        }

        /// <summary>Obtiene los datos de un centro de distribucion Externo</summary>
        /// <param name="dcID"></param>
        /// <returns>InternalDistributionCenterViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public ExternalDistributionCenterViewModel getExternalData(long dcID) {
            ExternalDistributionCenter edc = this.externalRepo.getData(dcID);
            if (edc == null) {
                return null;
            }
            ExternalDistributionCenterViewModel model = new ExternalDistributionCenterViewModel() {
                DistributionCenterID = edc.DistributionCenterID,
                Address = edc.Address,
                CityID = edc.City.CityID,
                CityName = edc.City.Name,
                Telephone = edc.Telephone,
                UsableUsedSpace = edc.UsableUsedSpace,
                UsableSpace = edc.UsableSpace,
                CountryID = edc.City.Province.Country.CountryID,
                CountryName = edc.City.Province.Country.Name,
                StateID = edc.City.Province.ProvinceID,
                StateName = edc.City.Province.Name,
                ContactName = edc.ContactName,
                Telephone2 = edc.Telephone2,
                HasMaxCapacity = edc.HasMaxCapacity
            };
            model.setPrecentages();
            return model;
        }

        public IQueryable<DistributionCenter> getData() {
            return this.repository.getData();
        }

        public List<DistributionCenterTableJson> toJsonArray(IQueryable<DistributionCenter> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<DistributionCenterTableJson> toJsonArray(ICollection<DistributionCenter> list) {
            List<DistributionCenterTableJson> result = new List<DistributionCenterTableJson>();
            DistributionCenterTableJson aux;
            foreach (DistributionCenter dc in list) {
                aux = new DistributionCenterTableJson() {
                    DistributionCenterID = dc.DistributionCenterID,
                    Address = dc.Address,
                    City = dc.City.Name
                };
                if (dc.UsableSpace == 0) {
                    aux.UsableSpacePercentage = 100;
                } else {
                    aux.UsableSpacePercentage = Math.Round(((dc.UsableUsedSpace / dc.UsableSpace) * 100),2);
                }
                if (dc is InternalDistributionCenter) {
                    InternalDistributionCenter idc = (InternalDistributionCenter) dc;
                    aux.UsedPercentage = Math.Round((idc.Capacity.HasValue ? (idc.UsedSpace / idc.Capacity.Value) * 100 : 100),2);
                } else {
                    aux.UsedPercentage = aux.UsableSpacePercentage;
                }
                result.Add(aux);
            }
            return result;
        }

        public IQueryable<DistributionCenter> filterByCountry(IQueryable<DistributionCenter> query, long? countryID) {
            if (countryID.HasValue && countryID.Value != -1) {
                return this.repository.countryFilter(query, countryID.Value);
            }
            return query;
        }

        public IQueryable<DistributionCenter> filterByState(IQueryable<DistributionCenter> query, long? stateID) {
            if (stateID.HasValue && stateID.Value != -1) {
                return this.repository.stateFilter(query, stateID.Value);
            }
            return query;
        }

        public IQueryable<DistributionCenter> filterByType(IQueryable<DistributionCenter> query, long? typeID) {
            if (typeID.HasValue && typeID.Value != -1) {
                return this.repository.typeFilter(query, typeID.Value);
            }
            return query;
        }

        public IQueryable<DistributionCenter> getAutocomplete(string search) {
            return this.repository.getAutocomplete(search);
        }

        public IQueryable<InternalDistributionCenter> getInternalAutocomplete(string search) {
            return this.internalRepo.getAutocomplete(search);
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<DistributionCenter> query) {
            List<DistributionCenter> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (DistributionCenter dc in list) {
                aux = new AutocompleteItem() { ID = dc.DistributionCenterID, 
                    Label = dc.DistributionCenterID.ToString(),
                    Description = dc.Address + ", " + dc.City.Name + ", " + dc.City.Province.Country.Name};
                result.Add(aux);
            }
            return result;
        }

        public Sorts toSort(int sortColumnIndex, string sortDirection) {

            switch (sortColumnIndex) {
                case 0:
                    if (sortDirection == "asc") {
                        return Sorts.ID;
                    } else {
                        return Sorts.ID_DESC;
                    }
                case 1:
                    if (sortDirection == "asc") {
                        return Sorts.ADDRESS;
                    } else {
                        return Sorts.ADDRESS_DESC;
                    }
                case 2:
                    if (sortDirection == "asc") {
                        return Sorts.CITY;
                    } else {
                        return Sorts.CITY_DESC;
                    }
                case 3:
                    if (sortDirection == "asc") {
                        return Sorts.USEDPERCENT;
                    } else {
                        return Sorts.USEDPERCENT_DESC;
                    }
                default:
                    throw new SortException(Resources.ExceptionMessages.InvalidSortException, "Col: " + sortColumnIndex);
            }
        }

        public IQueryable<DistributionCenter> search(IQueryable<DistributionCenter> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<DistributionCenter> sort(IQueryable<DistributionCenter> query, int index, string direction) {
            try {
                Sorts sort = this.toSort(index, direction);
                return this.repository.sort(query, sort);
            } catch (SortException) {
                return query;
            }
        }

        public IQueryable<DistributionCenter> sort(IQueryable<DistributionCenter> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        /// <summary>Retorna ture si es un Centro de Distribucion Interno, sino false</summary>
        /// <param name="DistributionCenterID">Id a buscar</param>
        /// <returns>True si es interno, sino false</returns>
        public bool isInternal(long DistributionCenterID) {
            DistributionCenter dc = this.repository.getData(DistributionCenterID);
            if (dc != null) {
                if (dc is InternalDistributionCenter) {
                    return true;
                }
            }
            return false;
        }

        public long getInternalDistributionCenterProductTypesQuantity(long DistributionCenterID)
        {
            return internalRepo.getProductTypesQuantity(DistributionCenterID);            
        }

        public long getExternalDistributionCenterProductsQuantity(long ExternalDistributionCenterID)
        {
            return externalRepo.getExternalDistributionCenterProductsQuantity(ExternalDistributionCenterID);
        }

        public InternalDistributionCenterDetails getIternalDistributionCenterDetails(long DistributionCenterID)
        {
            return internalRepo.getInternalDistributionCenterDetails(DistributionCenterID); 
        }

        public long getExternallDistributionCenterProductTypesQuantity(long DistributionCenterID)
        {
            return externalRepo.getProductTypesQuantity(DistributionCenterID);
        }

        public long getExternalDistributionCenterDetails(long DistributionCenterID)
        {
            return 0;
        }

        public ICollection<Deposit> getDeposits(long DistributionCenterID)
        {
            return internalRepo.getDeposits(DistributionCenterID);
        }

        public bool validDeposit(long DistributionCenterID)
        {
            return internalRepo.getDeposits(DistributionCenterID).Count > 0;
        }
        
        public List<Rack> getAllRacks(long DistributionCenterID)
        {
            IEnumerable<Rack> allRacks = new List<Rack>().AsQueryable();

            ICollection<Deposit> deposits = this.getDeposits(DistributionCenterID);
            foreach (Deposit deposit in deposits)
            {
                IDepositService depositService = new DepositService();
                IQueryable<Sector> sectors = depositService.getSectors(deposit.DepositID);
                foreach (Sector sector in sectors)
                {
                    ISectorService sectorService = new SectorService();
                    IQueryable<Hall> halls = sectorService.getHalls(sector.SectorID);
                    foreach (Hall hall in halls)
                    {
                        IHallService hallService = new HallService();
                        IQueryable<Rack> racks = hallService.getRacks(hall.HallID);
                        allRacks = allRacks.Union(racks);
                    }
                }
            }
            return new List<Rack>(allRacks);
        }

        public TempDepositLocation findTemporaryDepositWithSpace(long DistributionCenterID, decimal spaceNeeded){            
            ICollection<TemporaryDeposit> temporalDeposits = this.internalRepo.getTemporalDeposits(DistributionCenterID);
            foreach (TemporaryDeposit deposit in temporalDeposits)
            {
                if (deposit.Size - deposit.UsedSpace >= spaceNeeded)
                {
                    TempDepositLocation  location = new TempDepositLocation()
                    {
                        DepositDescription = deposit.Description,
                        DepositID = deposit.TemporaryDepositID
                    };
                    return location;                    
                }
            }
            return null;             
        }


        public Location findLocationForContainer(long InternalDistributionCenerID, long ProductCategory, decimal ContainerNeededSpace, decimal VolumeNeeded, decimal HeightNeeded)
        {
            Location location = null;
            List<Rack> distributionCenterRacks = this.getAllRacks(InternalDistributionCenerID);
            
            ICategoryService categoryService = new CategoryService();
                
            int i = 0;
            bool continueSearching = true; 
            while ( i < distributionCenterRacks.Count && continueSearching)
            {
                List<ProductCategory> rackCategories = new List<ProductCategory>();
                foreach ( ProductCategory productCategory in distributionCenterRacks[i].ProductCategories) {
                    categoryService.getAllChildren(productCategory.ProductCategoriesID, rackCategories);
                }
                if(findCategory(rackCategories,ProductCategory)){
                    ShelfsSubdivision shelfLocation = findSpaceInRack(distributionCenterRacks[i].Shelfs.ToList(), SizeUtils.fromCm3ToM3(ContainerNeededSpace),VolumeNeeded,HeightNeeded);
                    if (shelfLocation != null)
                    {
                        continueSearching = false;
                        location = new Location();
                        location.ShelfDivisionCode = shelfLocation.DivitionCode;
                        location.ShelfNumber = (int) shelfLocation.Shelf.ShelfNumber;
                        location.ShelfSubdivisionID = shelfLocation.ShelfSubdivisionID;
                        location.RackID = shelfLocation.Shelf.IDRack;
                        location.RackDescription = shelfLocation.Shelf.Rack.Description;
                        location.HallID = shelfLocation.Shelf.Rack.Hall.HallID;
                        location.HallDescription = shelfLocation.Shelf.Rack.Hall.Description;
                        location.SectorID = shelfLocation.Shelf.Rack.Hall.IDSector;
                        location.SectorDescription = shelfLocation.Shelf.Rack.Hall.Sector.Description;
                        location.DepositID = shelfLocation.Shelf.Rack.Hall.Sector.IDDeposit;
                        location.DepositDescription = shelfLocation.Shelf.Rack.Hall.Sector.Deposit.Description;                        
                    }
                    else
                    {
                        i++;
                    }                     
                }else{
                    i++;
                }  
            }
            return location; 
        }


        public ShelfsSubdivision findSpaceInRack(List<Shelf> RackShelfs, decimal AreaNeeded, decimal VolumeNeeded, decimal HeightNeeded )
        {
            int i = 0;
            bool continueSearching = true; 
            while (i < RackShelfs.Count && continueSearching)
            {
                if  ((RackShelfs[i].Rack.Width * RackShelfs[i].Rack.Depth) - (RackShelfs[i].UsedSpace / RackShelfs[i].Height ) >= AreaNeeded && 
                    ((RackShelfs[i].Size - RackShelfs[i].UsedSpace) >= VolumeNeeded) &&
                    (RackShelfs[i].Height >= HeightNeeded))
                    {
                        foreach (ShelfsSubdivision shelfSubdivision in RackShelfs[i].ShelfsSubdivisions)
                        {
                            IRackRepository rackRepository = new RackRepository();
                            if (rackRepository.getShelfSubdivisionSize(shelfSubdivision.ShelfSubdivisionID) - rackRepository.getShelfSubdivisionUsedSpace(shelfSubdivision.ShelfSubdivisionID) >= VolumeNeeded)
                            {
                                continueSearching = false;
                                return shelfSubdivision; 
                            }
                        }
                    }
                i++;
            }
            return null;
        }


        private bool findCategory(List<ProductCategory> CategoriesList, long CategoryID)
        {
            bool result = false;
            int i = 0;
            while (i < CategoriesList.Count && !result)
            {
                if (CategoriesList[i].ProductCategoriesID == CategoryID)
                {
                    result = true;
                }
                i++;
            }
            return result;
        }


        public void setExternalDCUsableUsedSpace(long DistributionCenterID, decimal Space)
        {
            this.externalRepo.setExternalDCUsableUsedSpace(DistributionCenterID, Space);
        }

        public decimal getExternalDCUsableUsedSpace(long ExternalDistributionCenterID)
        {
            return this.externalRepo.getData(ExternalDistributionCenterID).UsableUsedSpace; 
        }

        public void updateTempDepositUsedSpace(long DistributionCenterID, long DepositID, decimal SpaceInCm3)
        {
            this.internalRepo.updateTempDepositUsedSpace(DistributionCenterID, DepositID, SpaceInCm3);
        }
    }
}