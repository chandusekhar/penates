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

namespace Penates.Services.ABMs {
    public class PackService : IPackService {

        IPackRepository repository = new PackRepository();

        /// <summary> Guarda los datos de una producto en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como ProductViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        public long Save(PackViewModel pack) {
            return repository.Save(pack);
        }

        public bool Delete(long id) {
            return this.repository.Delete(id);
        }

        /// <summary> Retorna el View Model Listo para mostrar en el Formulario</summary>
        /// <param name="id">Id de la categoria</param>
        /// <returns>View Model de la categoria</returns>
        public PackViewModel getData(long id) {
            Pack pack = repository.getData(id);

            if (pack == null) {
                return null;
            }
            PackViewModel aux = new PackViewModel() { 
                PackID = pack.PackID,
                Description = pack.Description,
                ExpirationDate = pack.ExpirationDate,
                SerialNumber = pack.SerialNumber,
                HasExpirationDate = pack.ExpirationDate.HasValue
            };
            return aux;
        }

        public IQueryable<Pack> getData() {
            return this.repository.getData();
        }

        public IQueryable<Pack> getData(bool includeDeleted) {
            return this.repository.getData(includeDeleted);
        }

        public IQueryable<Pack> getAutocomplete(string search) {
            return this.repository.getAutocomplete(search);
        }

        public IQueryable<Pack> search(IQueryable<Pack> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<Pack> sort(IQueryable<Pack> query, int index, string direction) {
            try {
                Sorts sort = this.toSort(index, direction);
                return this.repository.sort(query, sort);
            } catch (SortException) {
                return query; //Si no hay sort no hago nada
            }
        }

        public IQueryable<Pack> sort(IQueryable<Pack> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        public List<PackTableJson> toJsonArray(IQueryable<Pack> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<PackTableJson> toJsonArray(ICollection<Pack> list) {
            List<PackTableJson> result = new List<PackTableJson>();
            PackTableJson aux;
            foreach (Pack pack in list) {
                aux = new PackTableJson() {
                    PackID = pack.PackID,
                    SerialNumber = pack.SerialNumber,
                    Description = pack.Description,
                    ExpirationDate = pack.ExpirationDate.HasValue ? pack.ExpirationDate.Value.ToShortDateString() : ""
                };
                result.Add(aux);
            }
            return result;
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<Pack> query) {
            List<Pack> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (Pack pack in list) {
                aux = new AutocompleteItem() {
                    ID = pack.PackID,
                    Label = pack.SerialNumber,
                    Description = pack.Description
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
                        return Sorts.DATE;
                    } else {
                        return Sorts.DATE_DESC;
                    }
                default:
                    throw new SortException(Resources.ExceptionMessages.InvalidSortException, "Col: " + sortColumnIndex);
            }
        }
    }
}