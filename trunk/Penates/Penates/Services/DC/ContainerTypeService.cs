using Penates.Database;
using Penates.Exceptions.Views;
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
using System.Web.Mvc;

namespace Penates.Services.DC {
    public class ContainerTypeService : IContainerTypeService {

        IContainerTypeRepository repository = new ContainerTypeRepository();

        /// <summary> Guarda los datos de una producto en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como ProductViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        public long Save(ContainerTypeViewModel type) {
            return repository.Save(type);
        }

        public bool Delete(long id) {
            return this.repository.Delete(id);
        }

        /// <summary> Retorna el View Model Listo para mostrar en el Formulario</summary>
        /// <param name="id">Id de la categoria</param>
        /// <returns>View Model de la categoria</returns>
        public ContainerTypeViewModel getData(long id) {
            ContainerType type = repository.getData(id);

            if (type == null) {
                return null;
            }
            ContainerTypeViewModel aux = new ContainerTypeViewModel() {
                ContainerTypeID = type.ContainerTypesID,
                Description = type.Description,
                Depth = type.Depth,
                Height = type.Height,
                Width = type.Width,
                Size = Math.Round((type.Size.HasValue ? type.Size.Value : (type.Depth * type.Height * type.Width)),2)
            };
            return aux;
        }

        public IQueryable<ContainerType> getData() {
            return this.repository.getData();
        }

        public IQueryable<ContainerType> getAutocomplete(string search) {
            return this.repository.getAutocomplete(search);
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<ContainerType> query) {
            List<ContainerType> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (ContainerType pc in list) {
                aux = new AutocompleteItem() {
                    ID = pc.ContainerTypesID,
                    Label = pc.Description,
                    aux = new { Size = pc.Size, Width = pc.Width, Height = pc.Height, Depth = pc.Depth }
                };
                if (!pc.Size.HasValue) {
                    pc.Size = pc.Depth * pc.Height * pc.Width;
                }
                aux.Description = Resources.FormsResources.Size + ": " + pc.Size.ToString() + " cm3";
                result.Add(aux);
            }
            return result;
        }

        public IQueryable<ContainerType> search(IQueryable<ContainerType> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<ContainerType> sort(IQueryable<ContainerType> query, int index, string direction) {
            try {
                Sorts sort = this.toSort(index, direction);
                return this.repository.sort(query, sort);
            } catch (SortException) {
                return query; //Si no hay sort no hago nada
            }
        }

        public IQueryable<ContainerType> sort(IQueryable<ContainerType> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        public List<ContainerTypeTableJson> toJsonArray(IQueryable<ContainerType> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<ContainerTypeTableJson> toJsonArray(ICollection<ContainerType> list) {
            List<ContainerTypeTableJson> result = new List<ContainerTypeTableJson>();
            ContainerTypeTableJson aux;
            foreach (ContainerType type in list) {
                aux = new ContainerTypeTableJson() {
                    ContainerTypeID = type.ContainerTypesID,
                    Description = type.Description,
                    Size = type.Size.HasValue ? type.Size.Value : (type.Depth * type.Height * type.Width)
                };
                result.Add(aux);
            }
            return result;
        }

        public bool areThereEnoughEmptyContainers(long ContainerTypeID, decimal Quantity)
        {
            if (this.repository.getAvaibleContainersQuantity(ContainerTypeID) >= Quantity)
            {               
                return true;
            }
            return false; 
        }


        public string getContainerTypeName(long ContainerTypeID)
        {
            try {
                return this.repository.getContainerTypeName(ContainerTypeID);
            } catch (Exception) {
                return "";
            }
        }

        public decimal getContainerArea(long ContainerID)
        {
            return this.repository.getContainerArea(ContainerID);
        }

        public decimal getContainerSize(long ContainerID)
        {
            return this.repository.getContainerSize(ContainerID);
        }

        public decimal getContainerHeight(long ContainerTypeID)
        {
            return this.repository.getContainerHeight(ContainerTypeID);
        }

        /// <summary> Obtiene la lista de Categorias de Producto para mostrar en el formulario </summary>
        /// <returns>SelectList - Lista de Categorias de Producto</returns>
        /// <exception cref="DatabaseException"
        public SelectList getTypeList() {
            return this.getTypeList(false);
        }

        /// <summary> Obtiene la lista de Categorias de Producto para mostrar en el formulario </summary>
        /// <returns>SelectList - Lista de Categorias de Producto</returns>
        /// <exception cref="DatabaseException"
        public SelectList getTypeList(bool includeAll) {
            var types = this.getData().ToList();
            if (includeAll) {
                types.Insert(0, new ContainerType { 
                    ContainerTypesID = -1, 
                    Description = Resources.Resources.All,
                    Depth = 0, Height = 0, Width = 0, Size = 0
                });
                return new SelectList(types, "ContainerTypesID", "Description", -1);
            }
            return new SelectList(types, "ContainerTypesID", "Description");
        }

        /// <summary> Obtiene la lista de categorias y elije la categoria pasada por parametro </summary>
        /// <param name="categoryId"> ID de la categoria a elejir </param>
        /// <returns>Selectlist para mostrar en el DropDownList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getTypeList(long categoryId) {
            var list = this.getTypeList();
            return new SelectList(list, "ContainerTypesID", "Description", categoryId);
        }

        /// <summary> Obtiene la lista de categorias y elije la categoria pasada por parametro </summary>
        /// <param name="categoryId"> ID de la categoria a elejir </param>
        /// <returns>Selectlist para mostrar en el DropDownList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getTypeList(bool includeAll, long categoryId) {
            var types = this.getData().ToList();
            if (includeAll) {
                types.Insert(0, new ContainerType {
                    ContainerTypesID = -1,
                    Description = Resources.Resources.All,
                    Depth = 0,
                    Height = 0,
                    Width = 0,
                    Size = 0
                });
                return new SelectList(types, "ContainerTypesID", "Description", categoryId);
            }
            return new SelectList(types, "ContainerTypesID", "Description", categoryId);
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
                        return Sorts.SIZE;
                    } else {
                        return Sorts.SIZE_DESC;
                    }
                default:
                    throw new SortException(Resources.ExceptionMessages.InvalidSortException, "Col: " + sortColumnIndex);
            }
        }
    }
}