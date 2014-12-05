using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Models.ViewModels.DC;
using Penates.Utils;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Repositories.DC {
    public class ExternalDistributionCenterRepository : IExternalDistributionCenterRepository {
        PenatesEntities db = new PenatesEntities();
        public int itemsPerPage {get;set;}

        public ExternalDistributionCenterRepository() {
            this.itemsPerPage = 50;
        }

        public ExternalDistributionCenterRepository(int itemsPerPage) {
            this.itemsPerPage = itemsPerPage;
        }

        /// <summary>Obtiene los datos de un Centro de Distribucion Externo. Omite los ya eliminados</summary>
        /// <param name="id">Id del Centro de Distribucion</param>
        /// <returns></returns>
        public ExternalDistributionCenter getData(long id) {
            try {
                DistributionCenter aux = db.DistributionCenters.Find(id);
                if (aux == null || aux.Deleted) {
                    return null;
                }
                return (ExternalDistributionCenter) aux;
            } catch (InvalidCastException) {
                return null;
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        /// <summary>Obtiene los datos de un Centro de Distribucion Externo</summary>
        /// <param name="id">Id del CD</param>
        /// <param name="includeDeleted">True para incluir a los ya eliminados, de lo contrario false</param>
        /// <returns></returns>
        public ExternalDistributionCenter getData(long id, bool includeDeleted) {
            if (includeDeleted) {
                try {
                    return (ExternalDistributionCenter) db.DistributionCenters.Find(id);
                } catch (InvalidCastException) {
                    return null;
                } catch (NullReferenceException) {
                    return null;
                } catch (Exception e) {
                    throw new DatabaseException(e.Message);
                }
            } else {
                return getData(id);
            }
        }

        /// <summary>Obtiene todos los centros de Distribuciones Externos</summary>
        /// <returns></returns>
        public IQueryable<ExternalDistributionCenter> getData() {
            try {
                return this.db.DistributionCenters.OfType<ExternalDistributionCenter>().Select(p => p).Where(p => p.Deleted == false);
            } catch (Exception e) {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary> Guarda los datos de un Centro de Distribucion en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del Centro</param>
        /// <returns>* long > 0 si el ID es valido
        ///         *Error del SP</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        public long Save(ExternalDistributionCenterViewModel dc) {
            try {
                DistributionCenter aux = db.DistributionCenters.Find(dc.DistributionCenterID);
                Nullable<long> val = null;
                if (aux == null) {
                    val = db.SP_ExternalDistributionCenter_Add(dc.CityID, dc.UsableSpace, dc.Address, dc.Telephone,
                        dc.Telephone2, dc.ContactName, dc.HasMaxCapacity).SingleOrDefault();
                } else {
                    if (dc.HasMaxCapacity) {
                        ExternalDistributionCenter edc = (ExternalDistributionCenter) aux;
                        decimal usedSpace = 0;
                        foreach (ExternalBox box in edc.ExternalBoxes) { // sumo todos los 
                            if (box.Product.Size.HasValue) {
                                usedSpace = usedSpace + SizeUtils.fromCm3ToM3(box.Product.Size.Value);
                            } else {
                                usedSpace = usedSpace + SizeUtils.fromCm3ToM3(box.Product.Width * box.Product.Height * box.Product.Depth);
                            }
                        }
                        if (dc.UsableSpace - usedSpace < 0) {
                            var ex3 = new DataRestrictionProcedureException(String.Format(Resources.ExceptionMessages.SaveException, Resources.Resources.DistributionCenterWArt),
                                String.Format(Resources.Errors.NewSpaceTooSmallWSize, usedSpace));
                            ex3.atributeName = ReflectionExtension.getVarName(() => dc.UsableSpace);
                            throw ex3;
                        }
                        val = db.SP_ExternalDistributionCenter_Edit(dc.DistributionCenterID, dc.CityID, dc.UsableSpace,
                            dc.Address, dc.Telephone, dc.Telephone2, dc.ContactName, dc.HasMaxCapacity).SingleOrDefault();
                    } else {
                        val = db.SP_ExternalDistributionCenter_Edit(dc.DistributionCenterID, dc.CityID, dc.UsableSpace,
                            dc.Address, dc.Telephone, dc.Telephone2, dc.ContactName, dc.HasMaxCapacity).SingleOrDefault();
                    }
                }
                if (!val.HasValue) {
                    return -1;
                }
                return val.Value;
            } catch (DataRestrictionProcedureException e) {
                throw e;
            } catch (Exception) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.DistributionCenter + ": " + dc.DistributionCenterID));
            }
        }

        /// <summary>Obtiene los depositos de un Centro de Distribuciones</summary>
        /// <param name="id">Id del Centro de Distribucion a Buscar</param>
        /// <returns>Collection de Depositos</returns>
        public ICollection<ExternalBox> getBoxes(long id) {
            try {
                ExternalDistributionCenter aux = this.db.DistributionCenters.OfType<ExternalDistributionCenter>().FirstOrDefault(x => x.DistributionCenterID == id);
                if (aux == null) {
                    return new List<ExternalBox>();
                }
                return aux.ExternalBoxes;
            } catch (Exception e) {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.GetConstraintException,
                        Resources.Resources.DistributionCenterWArt, id), e.Message);
            }
        }

        /// <summary>Obtiene la cantidad de cajas que impiden que se elimine el CD</summary>
        /// <param name="centerID"></param>
        /// <returns></returns>
        public long getBoxesConstraintsNumber(long centerID) {
            try {
                ExternalDistributionCenter dc = (ExternalDistributionCenter) this.db.DistributionCenters.Find(centerID);
                if (dc.ExternalBoxes == null) {
                    return 0;
                }
                return dc.ExternalBoxes.Count;
            } catch (InvalidCastException) {
                return 0;
            } catch (NullReferenceException) {
                return 0;
            }
        }

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        internal bool Delete(ExternalDistributionCenter edc) {
            DistributionCenterRepository dcRepo = new DistributionCenterRepository();
            if (this.checkDeleteConstrains(edc)) {
                var tran = this.db.Database.BeginTransaction();
                try {
                    if (this.checkPhysicalDelete(edc)) {
                        dcRepo.deleteNotifications(edc);
                        var dcExt = db.DistributionCenters.Find(edc.DistributionCenterID);
                        this.db.DistributionCenters.Remove(dcExt);
                    } else {
                        edc.Deleted = true;
                        db.DistributionCenters.Attach(edc);
                        var entry = db.Entry(edc);
                        entry.Property(e => e.Deleted).IsModified = true;
                        dcRepo.deleteNotifications(edc);
                    }
                    db.SaveChanges();
                    tran.Commit();
                    return true;
                } catch (Exception e) {
                    tran.Rollback();
                    throw new DeleteException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.CategoryWArt, edc.DistributionCenterID), e.Message);
                }
            } else {
                throw new DeleteConstrainException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.CategoryWArt, edc.DistributionCenterID),
                    String.Format(Resources.ExceptionMessages.DeleteConstraintMessage, Resources.Resources.DistributionCenterWArt,
                    Resources.Resources.Boxes, Resources.Resources.BoxesWArt));
            }
        }

        /// <summary>Checkea si puede eliminar el Centro de Distribuciones con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id">Id del Centro a eliminar</param>
        /// <returns>True si puede eliminar el Centro de Distribuciones, false si no lo puede eliminar</returns>
        /// <exception cref="InvalidCastException">Si el ID no pertenece a un InternalDC</exception>
        private bool checkDeleteConstrains(long id) {
            try {
                ExternalDistributionCenter dc = (ExternalDistributionCenter) db.DistributionCenters.Find(id);
                return this.checkDeleteConstrains(dc);
            } catch (NullReferenceException) {
                return true;
            }
        }

        /// <summary> Checkea si puede eliminar el Producto con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> El Producto a Eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        private bool checkDeleteConstrains(ExternalDistributionCenter dc) {
            DistributionCenterRepository dcRepo = new DistributionCenterRepository();
            if (dcRepo.checkDeleteConstrains(dc)) {
                if (dc.ExternalBoxes == null || dc.ExternalBoxes.Count == 0) {
                    return true;
                }
            }
            return false;
        }

        /// <summary> Checkea si puede eliminar el Producto con determinado Id y retorna true si puede o false si no puede </summary>
        /// <param name="id"> El Producto a Eliminar </param>
        /// <returns> True si puede eliminar el producto, false si no lo puede eliminar </returns>
        private bool checkPhysicalDelete(ExternalDistributionCenter dc) {
            DistributionCenterRepository dcRepo = new DistributionCenterRepository();
            if (dcRepo.checkFisicalDelete(dc)) {
                if (dc.ExternalBoxes == null || dc.ExternalBoxes.Count == 0) {
                    return true;
                }
            }
            return false;
        }

        public long getExternalDistributionCenterProductsQuantity(long ExternalDistributionCenterID)
        {
            ExternalDistributionCenter dc = this.getData(ExternalDistributionCenterID);
            return dc.ExternalBoxes.Count;
        }


        public long getProductTypesQuantity(long DistributionCenterID)
        {            
            ExternalDistributionCenter dc = this.getData(DistributionCenterID);
            ICollection<ProductCategory> listOfProductTypes = new List<ProductCategory>();

            if (dc.ExternalBoxes != null)
            {
                foreach (ExternalBox box in dc.ExternalBoxes)
                {
                    if (!listOfProductTypes.Contains(box.Product.ProductCategory)){
                        listOfProductTypes.Add(box.Product.ProductCategory);
                    }
                }
            }            
            return listOfProductTypes.Count; 
        }

        /// <summary>Aumenta el espacio a incrementar o disminuye dependiendo si es + o -</summary>
        /// <param name="dcID">Centro de Distribuciones Externo a Editar</param>
        /// <param name="spaceToIncrement">Espacio a incrementar</param>
        /// <returns>true si lo logra, false si no</returns>
        public bool setUsedSpace(long dcID, decimal spaceToIncrement) {
            ExternalDistributionCenter edc = this.getData(dcID);
            if (dcID == null) {
                return false;
            }
            edc.UsableSpace = edc.UsableSpace + spaceToIncrement;
            this.db.DistributionCenters.Attach(edc);
            var entry = db.Entry(edc);
            entry.Property(e => e.UsableSpace).IsModified = true;
            this.db.SaveChanges();
            return true;
        }


        public void setExternalDCUsableUsedSpace(long DistributionCenterID, decimal Space)
        {
            ExternalDistributionCenter edc = this.getData(DistributionCenterID);          
            edc.UsableUsedSpace = Space;
            this.db.DistributionCenters.Attach(edc);
            var entry = db.Entry(edc);
            entry.Property(e => e.UsableUsedSpace).IsModified = true;
            this.db.SaveChanges();                    
        }

        //private List<string> splitString(string search) {
        //    search = search.Trim();
        //    var searches = search.Split(' ');

        //    for (int i = 0; i < searches.Count(); i++) {
        //        searches[i] = searches[i].Trim();
        //    }
        //    return new List<string>(searches);
        //}

        //public IQueryable<DistributionCenter> getAutocomplete(string search)
        //{
        //    try
        //    {
        //        var data = this.searchAndRank(search);
        //        return data.Skip(0).Take(Properties.Settings.Default.autocompleteItems);
        //    }
        //    catch (DatabaseException de)
        //    {
        //        throw de;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new DatabaseException(e.Message, e);
        //    }
        //}

        //public IQueryable<DistributionCenter> searchAndRank(string search)
        //{
        //    var data = this.getData();
        //    return this.searchAndRank(data, search);
        //}

        //public IQueryable<DistributionCenter> searchAndRank(IQueryable<DistributionCenter> data, string search)
        //{
        //    try
        //    {
        //        if (String.IsNullOrEmpty(search) || String.IsNullOrWhiteSpace(search))
        //        {
        //            data = data.OrderBy(x => x.DistributionCenterID);
        //            return data;
        //        }
        //        List<string> searches = StringUtils.splitString(search);
        //        var aux = data.Select(x => new PageRankItem<DistributionCenter>
        //        {
        //            table = x,
        //            rankPoints = 0
        //        });
        //        foreach (string item in searches)
        //        {
        //            aux = aux.Select(x => new PageRankItem<DistributionCenter>
        //            {
        //                table = x.table,
        //                rankPoints = x.rankPoints + (SqlFunctions.StringConvert((double)x.table.DistributionCenterID).Trim() == item ? 1000 : 0) +
        //                        ((x.table.Address.Contains(item)) ? item.Length : 0) + ((x.table.Address.StartsWith(item)) ? (item.Length * 2) : 0)
        //            });
        //        }
        //        aux = aux.Where(x => x.rankPoints > 0);
        //        return aux.OrderByDescending(x => x.rankPoints)
        //            .ThenBy(x => x.table.DistributionCenterID)
        //            .Select(x => x.table);
        //    }
        //    catch (DatabaseException de)
        //    {
        //        throw de;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new DatabaseException(e.Message, e);
        //    }
        //}
    }
}