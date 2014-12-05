using Penates.Database;
using Penates.Exceptions;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.Forms;
using Penates.Repositories.ABMs;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Services.ABMs {
    public class BoxService : IBoxService {

        IBoxRepository repository = new BoxRepository();

        /// <summary> Guarda los datos de una caja en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        public long Add(BoxViewModel box) {
            long value = this.repository.Add(box);
            switch (value) {
                case -1:
                    throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.HallWArt));
                case -2:
                    var ex2 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.BoxesWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.Product, box.ProductID));
                    ex2.AttributeName = ReflectionExtension.getVarName(() => box.ProductName);
                    throw ex2;
                case -3:
                    var ex3 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.BoxesWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.Container, box.ContainerID));
                    ex3.AttributeName = ReflectionExtension.getVarName(() => box.ContainerCode);
                    throw ex3;
                case -4:
                    var ex4 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.BoxesWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.ItemState, box.StatusID));
                    ex4.AttributeName = ReflectionExtension.getVarName(() => box.StatusDescription);
                    throw ex4;
            }
            return value;
        }

        /// <summary> Guarda los datos de una caja en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        public long Add(ExternalBoxViewModel box) {
            long value = this.repository.Add(box);
            switch (value) {
                case -1:
                    throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.BoxWArt));
                case -2:
                    var ex2 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.BoxWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.Product, box.ProductID));
                    ex2.AttributeName = ReflectionExtension.getVarName(() => box.ProductName);
                    throw ex2;
                case -3:
                    var ex3 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.BoxWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.DistributionCenter, box.DistributionCenterID));
                    ex3.AttributeName = ReflectionExtension.getVarName(() => box.DistributionCenterID);
                    throw ex3;
                case -4:
                    var ex4 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.BoxWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.ItemState, box.StatusID));
                    ex4.AttributeName = ReflectionExtension.getVarName(() => box.StatusDescription);
                    throw ex4;
            }
            return value;
        }

        /// <summary> Guarda los datos de una caja en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        public long Edit(BoxViewModel box) {
            long value = this.repository.Edit(box);
            switch (value) {
                case -1:
                    throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.HallWArt));
                case -2:
                    var ex2 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.BoxesWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.Product, box.ProductID));
                    ex2.AttributeName = ReflectionExtension.getVarName(() => box.ProductName);
                    throw ex2;
                case -3:
                    var ex3 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.BoxesWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.Container, box.ContainerID));
                    ex3.AttributeName = ReflectionExtension.getVarName(() => box.ContainerCode);
                    throw ex3;
                case -4:
                    var ex4 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.BoxesWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.ItemState, box.StatusID));
                    ex4.AttributeName = ReflectionExtension.getVarName(() => box.StatusDescription);
                    throw ex4;
            }
            return value;
        }

        /// <summary> Guarda los datos de una caja en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como InternalDistributionCenterViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        /// <exception cref="StoredProcedureException">Error del SP</exception>
        /// <exception cref="ModelErrorException">Error a agregar al Model</exception>
        public long Edit(ExternalBoxViewModel box) {
            long value = this.repository.Edit(box);
            switch (value) {
                case -1:
                    throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.HallWArt));
                case -2:
                    var ex2 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.BoxesWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.Product, box.ProductID));
                    ex2.AttributeName = ReflectionExtension.getVarName(() => box.ProductName);
                    throw ex2;
                case -3:
                    var ex3 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.BoxesWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.DistributionCenter, box.DistributionCenterID));
                    ex3.AttributeName = ReflectionExtension.getVarName(() => box.DistributionCenterID);
                    throw ex3;
                case -4:
                    var ex4 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.BoxesWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.ItemState, box.StatusID));
                    ex4.AttributeName = ReflectionExtension.getVarName(() => box.StatusDescription);
                    throw ex4;
            }
            return value;
        }

        /// <summary>Obtiene los datos de una Caja</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>HallViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public ABoxViewModel getBoxInfo(long boxID) {
            Box box = this.repository.getBoxInfo(boxID);
            if (box == null) {
                return null;
            }
            BoxViewModel model = new BoxViewModel() {
                AdquisitionDate = box.AdquisitionDate,
                BoxID = box.BoxID,
                BuyerCost = box.BuyerCost,
                Depth = box.Depth,
                Height = box.Height,
                IsWaste = box.IsWaste.HasValue ? box.IsWaste.Value : false,
                PackID = box.IDPack,
                PackSerialCode = box.Pack != null ? box.Pack.SerialNumber : null,
                ProductID = box.IDProduct,
                ProductName = box.Product.Name,
                Quantity = box.Quantity,
                Reevaluate = box.Reevaluate.HasValue ? box.Reevaluate.Value : false,
                Reserved = box.Reserved.HasValue ? box.Reserved.Value : false,
                Size = box.Size.HasValue ? box.Size.Value : box.Depth * box.Width * box.Height,
                StatusID = box.IDStatus,
                StatusDescription = box.ItemsState.Description,
                Transit = box.Transit.HasValue ? box.Transit.Value : false,
                Width = box.Width
            };
            return model;
        }

        /// <summary>Obtiene los datos de una Caja</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>HallViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public ExternalBoxViewModel getExternalBoxInfo(long boxID) {
            Box box = this.repository.getBoxInfo(boxID);
            if (box == null) {
                return null;
            }
            if (box is ExternalBox) {
                ExternalBoxViewModel model = new ExternalBoxViewModel() {
                    AdquisitionDate = box.AdquisitionDate,
                    BoxID = box.BoxID,
                    BuyerCost = box.BuyerCost,
                    Depth = box.Depth,
                    Height = box.Height,
                    IsWaste = box.IsWaste.HasValue ? box.IsWaste.Value : false,
                    PackID = box.IDPack,
                    PackSerialCode = box.Pack != null ? box.Pack.SerialNumber : null,
                    ProductID = box.IDProduct,
                    ProductName = box.Product.Name,
                    Quantity = box.Quantity,
                    Reevaluate = box.Reevaluate.HasValue ? box.Reevaluate.Value : false,
                    Reserved = box.Reserved.HasValue ? box.Reserved.Value : false,
                    Size = box.Size.HasValue ? box.Size.Value : box.Depth * box.Width * box.Height,
                    StatusID = box.IDStatus,
                    StatusDescription = box.ItemsState.Description,
                    Transit = box.Transit.HasValue ? box.Transit.Value : false,
                    Width = box.Width
                };
                ExternalBox eBox = (ExternalBox) box;
                model.DistributionCenterID = eBox.ExternalDistributionCenterID;
                return model;
            } else {
                return null;
            }
            
        }

        /// <summary>Obtiene los datos de una Caja</summary>
        /// <param name="depositID">ID del deposito</param>
        /// <returns>HallViewModel</returns>
        /// <exception cref="IDNotFoundException"
        public BoxViewModel getInternalBoxInfo(long boxID) {
            Box box = this.repository.getBoxInfo(boxID);
            if (box == null) {
                return null;
            }
            if (box is InternalBox) {
                BoxViewModel model = new BoxViewModel() {
                    AdquisitionDate = box.AdquisitionDate,
                    BoxID = box.BoxID,
                    BuyerCost = box.BuyerCost,
                    Depth = box.Depth,
                    Height = box.Height,
                    IsWaste = box.IsWaste.HasValue ? box.IsWaste.Value : false,
                    PackID = box.IDPack,
                    PackSerialCode = box.Pack != null ? box.Pack.SerialNumber : null,
                    ProductID = box.IDProduct,
                    ProductName = box.Product.Name,
                    Quantity = box.Quantity,
                    Reevaluate = box.Reevaluate.HasValue ? box.Reevaluate.Value : false,
                    Reserved = box.Reserved.HasValue ? box.Reserved.Value : false,
                    Size = box.Size.HasValue ? box.Size.Value : box.Depth * box.Width * box.Height,
                    StatusID = box.IDStatus,
                    StatusDescription = box.ItemsState.Description,
                    Transit = box.Transit.HasValue ? box.Transit.Value : false,
                    Width = box.Width
                };
                InternalBox iBox = (InternalBox)box;
                model.ContainerID = iBox.IDContainer;
                model.ContainerCode = iBox.Container.Code;
                if (iBox.Container.IDTemporalDeposit.HasValue) {
                    model.DistributionCenterID = iBox.Container.TemporaryDeposit.IDDistributionCenter;
                } else {
                    if (iBox.Container.IDShelfSubdivition.HasValue) {
                        model.DistributionCenterID = iBox.Container.ShelfsSubdivision.Shelf.Rack.Hall.Sector.Deposit.IDDistributionCenter;
                    }
                }
                return model;
            } else {
                return null;
            }
        }

        public bool isExternal(long boxID) {
            Box box = this.repository.getBoxInfo(boxID);
            if (box is ExternalBox) {
                return true;
            }
            return false;
        }

        public IQueryable<Box> getData() {
            return this.repository.getData();
        }

        public List<BoxTableJson> toJsonArray(IQueryable<Box> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<BoxTableJson> toJsonArray(ICollection<Box> list) {
            List<BoxTableJson> result = new List<BoxTableJson>();
            BoxTableJson aux;
            foreach (Box box in list) {
                aux = new BoxTableJson() {
                    BoxID = box.BoxID,
                    ProductName = box.Product.Name,
                    Quantity = box.Quantity,
                    AdquisitionDate = box.AdquisitionDate.ToShortDateString(),
                    State = box.ItemsState.Description
                };
                result.Add(aux);
            }
            return result;
        }

        public IQueryable<Box> search(IQueryable<Box> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<Box> sort(IQueryable<Box> query, int index, string direction) {
            Sorts sort = this.toSort(index, direction);
            return this.sort(query, sort);
        }

        public IQueryable<Box> sort(IQueryable<Box> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        public IQueryable<Box> filterByDistributionCenter(IQueryable<Box> query, long? dcID) {
            if (dcID.HasValue) {
                query = this.repository.filterByDistributionCenter(query, dcID.Value);
            }
            return query;
        }

        public IQueryable<Box> filterByDeposit(IQueryable<Box> query, long? depositID) {
            if (depositID.HasValue) {
                query = this.repository.filterByDeposit(query, depositID.Value);
            }
            return query;
        }

        public IQueryable<Box> filterByTemporaryDeposit(IQueryable<Box> query, long? depositID) {
            if (depositID.HasValue) {
                query = this.repository.filterByTemporaryDeposit(query, depositID.Value);
            }
            return query;
        }

        public IQueryable<Box> filterBySector(IQueryable<Box> query, long? sectorID) {
            if (sectorID.HasValue) {
                query = this.repository.filterBySector(query, sectorID.Value);
            }
            return query;
        }

        public IQueryable<Box> filterByRack(IQueryable<Box> query, long? rackID) {
            if (rackID.HasValue) {
                query = this.repository.filterByRack(query, rackID.Value);
            }
            return query;
        }

        public IQueryable<Box> filterByPack(IQueryable<Box> query, long? packID) {
            if (packID.HasValue) {
                query = this.repository.filterByPack(query, packID.Value);
            }
            return query;
        }

        public IQueryable<Box> filterByState(IQueryable<Box> query, long? stateID) {
            if (stateID.HasValue) {
                query = this.repository.filterByStatus(query, stateID.Value);
            }
            return query;
        }

        public IQueryable<Box> filterByProduct(IQueryable<Box> query, long? productID) {
            if (productID.HasValue && productID.Value != 0) {
                query = this.repository.filterByProduct(query, productID.Value);
            }
            return query;
        }

        public Status sell(long boxID, long sellID) {
            return this.repository.sell(boxID, sellID);
        }

        public IQueryable<Box> getAutocomplete(string search, long? dcID, long? depositID, long? temporaryDepositID, long? sectorID, long? rackID){
            return this.repository.getAutocomplete(search, dcID, depositID, temporaryDepositID, sectorID, rackID);
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<Box> query) {
            List<Box> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (Box box in list) {
                aux = new AutocompleteItem() {
                    ID = box.BoxID,
                    Label = box.BoxID.ToString(),
                    Description = Resources.Resources.Product + ": " + box.Product.Name + "(" + box.AdquisitionDate.ToShortDateString()
                    + ")",
                    aux = new {
                        StatusDescription = box.ItemsState.Description,
                        PackCode = box.Pack!=null ? box.Pack.SerialNumber : null,
                        PackDate = box.Pack != null ? (box.Pack.ExpirationDate.HasValue ? box.Pack.ExpirationDate.Value.ToShortDateString() : null) : null,
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
                        return Sorts.PRODUCT;
                    } else {
                        return Sorts.PRODUCT_DESC;
                    }
                case 2:
                    if (sortDirection == "asc") {
                        return Sorts.DATE;
                    } else {
                        return Sorts.DATE_DESC;
                    }
                case 3:
                    if (sortDirection == "asc") {
                        return Sorts.STATUS;
                    } else {
                        return Sorts.STATUS_DESC;
                    }
                case 4:
                    if (sortDirection == "asc") {
                        return Sorts.QUANTITY;
                    } else {
                        return Sorts.QUANTITY_DESC;
                    }
                default:
                    return Sorts.ID;
            }
        }


        public bool Delete(long BoxID)
        {
            return this.repository.Delete(BoxID);
        }

        public bool setContainerID(long BoxID, long ContainerID)
        {
            return this.repository.setContainerID(BoxID, ContainerID); 
        }
    }
}