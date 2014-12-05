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
    public class ContainerService : IContainerService {

        IContainerRepository repository = new ContainerRepository();

        /// <summary> Guarda los datos de un Contenerdor en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        public long Save(ContainerViewModel container)
        {
            long value = this.repository.Save(container);
            switch (value) {
                case -1:
                    throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.ContainerWArt));
                case -2:
                    var ex2 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.ContainerWArt), String.Format(Resources.ExceptionMessages.UniqueRestrictionException,
                    Resources.Resources.ContainerType, container.ContainerTypeID));
                    ex2.AttributeName = ReflectionExtension.getVarName(() => container.Code);
                    throw ex2;
                case -3:
                    var ex3 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.ContainerWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.ContainerType, container.ContainerTypeID));
                    ex3.AttributeName = ReflectionExtension.getVarName(() => container.ContainerTypeName);
                    throw ex3;
                case -4:
                    var ex4 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.ContainerWArt), String.Format(Resources.ExceptionMessages.ParametersNulls,
                    Resources.Resources.TemporaryDepositWArt, Resources.Resources.DivitionWArt));
                    ex4.AttributeName = ReflectionExtension.getVarName(() => container.error);
                    throw ex4;
                case -5:
                    var ex5 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.ContainerWArt),
                        Resources.Errors.NewSpaceTooSmall);
                    ex5.AttributeName = ReflectionExtension.getVarName(() => container.Size);
                    throw ex5;
            }
            return value;
        }

        /// <summary>Elimina un Contenedor</summary>
        /// <param name="id">ID de la Orden a Eliminar</param>
        /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
        /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
        /// <exception cref="DeleteConstraintException" si no se puede borrar por restricciones
        public bool Delete(long containerID)
        {
            return this.repository.Delete(containerID);
        }

        /// <summary>Obtiene los datos de un container</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>ContainerViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public ContainerViewModel getContainerData(long containerID)
        {
            Container container = this.repository.getContainerInfo(containerID);
            if (container == null)
            {
                return null;
            }
            ContainerViewModel model = new ContainerViewModel()
            {
                ContainerID = container.ContainerID,
                ContainerTypeID = container.IDContainerTypes,
                Code = container.Code,
                UsedSpace = container.UsedSpace,
                TemporaryDepositID = container.IDTemporalDeposit,
                IDShelfSubdivision = container.IDShelfSubdivition,
                ContainerTypeName = container.ContainerType.Description,
                Size = container.ContainerType.Size.HasValue ? container.ContainerType.Size.Value : (container.ContainerType.Depth * container.ContainerType.Height * container.ContainerType.Width),
                Depth = container.ContainerType.Depth,
                Height = container.ContainerType.Height,
                Width = container.ContainerType.Width
            };
            decimal h = this.getMaxHeight(container.InternalBoxes);
            if(container.ContainerType.Height > h){
                h = container.ContainerType.Height;
            }
            model.Capacity = container.ContainerType.Depth * h * container.ContainerType.Width;
            if(container.ContainerType.Size.HasValue && container.ContainerType.Size.Value != 0){
                model.UsedPercentage = Math.Round(((container.UsedSpace/h)/(container.ContainerType.Size.Value/container.ContainerType.Height))*100 , 2);
            }else{
                model.UsedPercentage = 100;
            }
            if(container.IDTemporalDeposit.HasValue){
                model.TemporaryDepositName = container.TemporaryDeposit.Description;
                model.DistributionCenter = container.TemporaryDeposit.IDDistributionCenter;
            }else{
                if (container.IDShelfSubdivition.HasValue) {
                    model.ShelfSubdivisionName = container.ShelfsSubdivision.DivitionCode;
                    model.DistributionCenter = container.ShelfsSubdivision.Shelf.Rack.Hall.Sector.Deposit.IDDistributionCenter;
                    model.DepositID = container.ShelfsSubdivision.Shelf.Rack.Hall.Sector.IDDeposit;
                    model.DepositName = container.ShelfsSubdivision.Shelf.Rack.Hall.Sector.Deposit.Description;
                    model.SectorID = container.ShelfsSubdivision.Shelf.Rack.Hall.IDSector;
                    model.SectorName = container.ShelfsSubdivision.Shelf.Rack.Hall.Sector.Description;
                    model.HallID = container.ShelfsSubdivision.Shelf.Rack.IDHall;
                    model.HallName = container.ShelfsSubdivision.Shelf.Rack.Hall.Description;
                    model.RackID = container.ShelfsSubdivision.Shelf.IDRack;
                    model.RackName = container.ShelfsSubdivision.Shelf.Rack.RackCode;
                }
            }
            return model;
        }

        public IQueryable<Container> getData()
        {
            return this.repository.getData();
        }

        public List<ContainerTableJson> toJsonArray(IQueryable<Container> query)
        {
            return this.toJsonArray(query.ToList());
        }

        public List<ContainerTableJson> toJsonArray(ICollection<Container> list)
        {
            List<ContainerTableJson> result = new List<ContainerTableJson>();
            ContainerTableJson aux;
            foreach (Container container in list)
            {
                aux = new ContainerTableJson()
                {
                    ContainerID = container.ContainerID,
                    ContainerTypeName = container.ContainerType.Description,
                    TemporaryDeposit = container.IDTemporalDeposit.HasValue ? container.TemporaryDeposit.Description : null,
                    Divition = container.ShelfsSubdivision != null ? container.ShelfsSubdivision.DivitionCode : null

                };
                decimal h = this.getMaxHeight(container.InternalBoxes);
                if (container.ContainerType.Height > h) {
                    h = container.ContainerType.Height;
                }
                if (container.ContainerType.Size.HasValue && container.ContainerType.Size.Value != 0) {
                    aux.UsedPercentage = Math.Round(((container.UsedSpace / h) / (container.ContainerType.Size.Value / container.ContainerType.Height)) * 100, 2);
                } else {
                    aux.UsedPercentage = 100;
                }
                result.Add(aux);
            }
            return result;
        }

        public IQueryable<Container> search(IQueryable<Container> query, string search)
        {
            return this.repository.search(query, search);
        }

        public IQueryable<Container> sort(IQueryable<Container> query, int index, string direction)
        {
            Sorts sort = this.toSort(index, direction);
            return this.sort(query, sort);
        }

        public IQueryable<Container> sort(IQueryable<Container> query, Sorts sort)
        {
            return this.repository.sort(query, sort);
        }

 
        public string getContainerTypeDescription(long containerID)
        {
            Container container = this.repository.getContainerInfo(containerID);
            if (container == null)
            {
                return null;
            }
            return container.ContainerType.Description;
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
                        return Sorts.TEMPORARY_DEPOSIT;
                    } else {
                        return Sorts.TEMPORARY_DEPOSIT_DESC;
                    }
                case 3:
                    if (sortDirection == "asc") {
                        return Sorts.SHELF_SUBDIVISION;
                    } else {
                        return Sorts.SHELF_SUBDIVISION_DESC;
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

        public Container getEmptyContainer(long ContainerTypeID)
        {
            return this.repository.getEmptyContainer(ContainerTypeID);
        }


        public void setContainerUsedSpace(long ContainerID, decimal UsedSpace)
        {
            this.repository.setContainerUsedSpace(ContainerID, UsedSpace);
        }

        public IQueryable<Container> filterByDistributionCenter(IQueryable<Container> query, long? dcID) {
            if (dcID.HasValue) {
                query = this.repository.filterByDistributionCenter(query, dcID.Value);
            }
            return query;
        }

        public IQueryable<Container> filterByDeposit(IQueryable<Container> query, long? depositID) {
            if (depositID.HasValue) {
                query = this.repository.filterByDeposit(query, depositID.Value);
            }
            return query;
        }

        public IQueryable<Container> filterByTemporaryDeposit(IQueryable<Container> query, long? depositID) {
            if (depositID.HasValue) {
                query = this.repository.filterByTemporaryDeposit(query, depositID.Value);
            }
            return query;
        }

        public IQueryable<Container> filterBySector(IQueryable<Container> query, long? sectorID) {
            if (sectorID.HasValue) {
                query = this.repository.filterBySector(query, sectorID.Value);
            }
            return query;
        }

        public IQueryable<Container> filterByRack(IQueryable<Container> query, long? rackID) {
            if (rackID.HasValue) {
                query = this.repository.filterByRack(query, rackID.Value);
            }
            return query;
        }

        public IQueryable<Container> filterByType(IQueryable<Container> query, long? typeID) {
            if (typeID.HasValue && typeID.Value!=-1) {
                query = this.repository.filterByType(query, typeID.Value);
            }
            return query;
        }

        public IQueryable<Container> getAutocomplete(string search) {
            return this.getAutocomplete(search, null, null, null, null, null, null);
        }

        public IQueryable<Container> getAutocomplete(string search, long? dcID, long? depositID, long? temporaryDepositID, long? sectorID, long? rackID, long? typeID) {
            return this.repository.getAutocomplete(search, dcID, depositID, temporaryDepositID, sectorID, rackID, typeID);
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<Container> query) {
            List<Container> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (Container container in list) {
                aux = new AutocompleteItem() {
                    ID = container.ContainerID,
                    Label = container.Code,
                    Description = Math.Round(container.ContainerType.Size.Value,2) + "cm3",
                    aux = new {
                        UsedSpace = container.UsedSpace
                    }
                };
                result.Add(aux);
            }
            return result;
        }

        private decimal getMaxHeight(ICollection<InternalBox> boxes) {
            decimal h = 0;
            foreach(InternalBox box in boxes){
                if (box.Height > h) {
                    h = box.Height;
                }
            }
            return h;
        }
    }
}