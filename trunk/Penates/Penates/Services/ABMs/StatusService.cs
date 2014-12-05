using Penates.Database;
using Penates.Exceptions.Views;
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
    public class StatusService : IStatusService {

        IItemStateRepository repository = new ItemStateRepository();

        /// <summary> Guarda los datos de una producto en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como ProductViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        public long Save(StatusViewModel pack) {
            return repository.Save(pack);
        }

        public bool Delete(long id) {
            return this.repository.Delete(id);
        }

        /// <summary> Retorna el View Model Listo para mostrar en el Formulario</summary>
        /// <param name="id">Id de la categoria</param>
        /// <returns>View Model de la categoria</returns>
        public StatusViewModel getData(long id) {
            ItemsState state = repository.getData(id);

            if (state == null) {
                return null;
            }
            StatusViewModel aux = new StatusViewModel() {
                StatusID = state.ItemStateID,
                Description = state.Description
            };
            return aux;
        }

        public IQueryable<ItemsState> getData() {
            return this.repository.getData();
        }

        public IQueryable<ItemsState> getData(bool includeDeleted) {
            return this.repository.getData(includeDeleted);
        }

        public IQueryable<ItemsState> getAutocomplete(string search) {
            return this.repository.getAutocomplete(search);
        }

        public IQueryable<ItemsState> search(IQueryable<ItemsState> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<ItemsState> sort(IQueryable<ItemsState> query, int index, string direction) {
            try {
                Sorts sort = this.toSort(index, direction);
                return this.repository.sort(query, sort);
            } catch (SortException) {
                return query; //Si no hay sort no hago nada
            }
        }

        public IQueryable<ItemsState> sort(IQueryable<ItemsState> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        public List<StatusTableJson> toJsonArray(IQueryable<ItemsState> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<StatusTableJson> toJsonArray(ICollection<ItemsState> list) {
            List<StatusTableJson> result = new List<StatusTableJson>();
            StatusTableJson aux;
            foreach (ItemsState status in list) {
                aux = new StatusTableJson() {
                    StatusID = status.ItemStateID,
                    Description = status.Description
                };
                result.Add(aux);
            }
            return result;
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<ItemsState> query) {
            List<ItemsState> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (ItemsState status in list) {
                aux = new AutocompleteItem() {
                    ID = status.ItemStateID,
                    Label = status.Description
                };
                result.Add(aux);
            }
            return result;
        }

        /// <summary> Obtiene la lista de Categorias de Producto para mostrar en el formulario </summary>
        /// <returns>SelectList - Lista de Categorias de Producto</returns>
        /// <exception cref="DatabaseException"
        public SelectList getStatusList() {
            var states = this.getData().ToList();
            return new SelectList(states, "ItemStateID", "Description");
        }

        /// <summary> Obtiene la lista de categorias y elije la categoria pasada por parametro </summary>
        /// <param name="categoryId"> ID de la categoria a elejir </param>
        /// <returns>Selectlist para mostrar en el DropDownList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getStatusList(long statusID) {
            var states = this.getData().ToList();
            return new SelectList(states, "ItemStateID", "Description", statusID);
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
                default:
                    throw new SortException(Resources.ExceptionMessages.InvalidSortException, "Col: " + sortColumnIndex);
            }
        }


        public ItemsState getDefaultState()
        {
            return this.repository.getDefaultState();
        }
    }
}