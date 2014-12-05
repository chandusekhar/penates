using Penates.Database;
using Penates.Exceptions;
using Penates.Exceptions.Database;
using Penates.Exceptions.Views;
using Penates.Interfaces.Repositories;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Models.ViewModels.Forms;
using Penates.Repositories.ABMs;
using Penates.Utils;
using Penates.Utils.Enums;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Services.ABMs {
    public class ClientService : IClientService{

        IClientRepository repository = new ClientRepository();

        /// <summary> Guarda los datos de una producto en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como ProductViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        public long Save(ClientViewModel client) {
            long value = repository.Save(client);
            switch (value) {
                case -1:
                    throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                        Resources.Resources.HallWArt));
                case -2:
                    var ex2 = new ModelErrorException(String.Format(Resources.ExceptionMessages.SaveException,
                    Resources.Resources.BoxesWArt), String.Format(Resources.ExceptionMessages.ForeignKeyConstraintException,
                    Resources.Resources.City, client.CityID));
                    ex2.AttributeName = ReflectionExtension.getVarName(() => client.CityID);
                    throw ex2;
            }
            return value;
        }

        public Status Deactivate(long id) {
            try {
                return this.repository.Deactivate(id);
            } catch (Exception e) {
                return new Status() {
                    Success = false,
                    Message = e.Message
                };
            }
        }

        /// <summary> Retorna el View Model Listo para mostrar en el Formulario</summary>
        /// <param name="id">Id de la categoria</param>
        /// <returns>View Model de la categoria</returns>
        public ClientViewModel getData(long id) {
            Client client = repository.getData(id);

            if (client == null) {
                return null;
            }
            ClientViewModel aux = new ClientViewModel() {
                ClientID = client.ClientsID,
                Active = !client.Deleted,
                Address = client.Address,
                CityID = client.City,
                ContactName = client.ContactName,
                CUIT = client.CUIT,
                Email = client.Email,
                Name = client.Name,
                Phone = client.Phone
            };
            if(client.City1 != null){
                aux.CityName = client.City1.Name;
                aux.StateID = client.City1.IDProvince;
                aux.StateName = client.City1.Province.Name;
                aux.CountryID = client.City1.Province.IDCountry;
                aux.CountryName = client.City1.Province.Country.Name;
            }
            return aux;
        }

        public IQueryable<Client> getData() {
            return this.repository.getData();
        }

        public IQueryable<Client> getData(bool includeDeleted) {
            return this.repository.getData(includeDeleted);
        }

        public IQueryable<Client> getAutocomplete(string search) {
            return this.repository.getAutocomplete(search);
        }

        public IQueryable<Client> getAutocomplete(string search, bool includeDeleted) {
            return this.repository.getAutocomplete(search, includeDeleted);
        }

        public IQueryable<Client> search(IQueryable<Client> query, string search) {
            return this.repository.search(query, search);
        }

        public IQueryable<Client> sort(IQueryable<Client> query, int index, string direction) {
            try {
                Sorts sort = this.toSort(index, direction);
                return this.repository.sort(query, sort);
            } catch (SortException) {
                return query; //Si no hay sort no hago nada
            }
        }

        public IQueryable<Client> sort(IQueryable<Client> query, Sorts sort) {
            return this.repository.sort(query, sort);
        }

        public IQueryable<Client> filterByCity(IQueryable<Client> query, long? cityID) {
            if(cityID.HasValue && cityID.Value != -1){
                query = this.repository.filterByCity(query, cityID.Value);
            }
            return query;
        }

        public IQueryable<Client> filterByState(IQueryable<Client> query, long? stateID) {
            if (stateID.HasValue && stateID.Value != -1) {
                query = this.repository.filterByState(query, stateID.Value);
            }
            return query;
        }

        public IQueryable<Client> filterByCountry(IQueryable<Client> query, long? countryID) {
            if (countryID.HasValue && countryID.Value != -1) {
                query = this.repository.filterByCountry(query, countryID.Value);
            }
            return query;
        }

        public IQueryable<Client> filterByDeactivated(IQueryable<Client> query, int? active) {
            if (active.HasValue) {
                if (active.Value == DeactivatedTypes.ACTIVE.getTypeNumber()) {
                    query = this.repository.filterByDisabled(query, false);
                } else {
                    if (active.Value == DeactivatedTypes.DEACTIVATED.getTypeNumber()) {
                        query = this.repository.filterByDisabled(query, true);
                    }
                }
            }
            return query;
        }

        public List<ClientTableJson> toJsonArray(IQueryable<Client> query) {
            return this.toJsonArray(query.ToList());
        }

        public List<ClientTableJson> toJsonArray(ICollection<Client> list) {
            List<ClientTableJson> result = new List<ClientTableJson>();
            ClientTableJson aux;
            foreach (Client client in list) {
                aux = new ClientTableJson() {
                    ClientID = client.ClientsID,
                    City = client.City1 == null ? null : client.City1.Name,
                    CUIT = client.CUIT,
                    Email = client.Email,
                    Name = client.Name,
                    Deactivated = client.Deleted
                };
                result.Add(aux);
            }
            return result;
        }

        public List<AutocompleteItem> toJsonAutocomplete(IQueryable<Client> query) {
            List<Client> list = query.ToList();
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            AutocompleteItem aux;
            foreach (Client client in list) {
                aux = new AutocompleteItem() {
                    ID = client.ClientsID,
                    Label = client.Name,
                    Description = client.Address + (client.City1 == null ? "" : (", " + client.City1.Name))
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
                        return Sorts.CUIT;
                    } else {
                        return Sorts.CUIT_DESC;
                    }
                case 2:
                    if (sortDirection == "asc") {
                        return Sorts.NAME;
                    } else {
                        return Sorts.NAME_DESC;
                    }
                case 3:
                    if (sortDirection == "asc") {
                        return Sorts.CITY;
                    } else {
                        return Sorts.CITY_DESC;
                    }
                case 4:
                    if (sortDirection == "asc") {
                        return Sorts.EMAIL;
                    } else {
                        return Sorts.EMAIL_DESC;
                    }
                default:
                    throw new SortException(Resources.ExceptionMessages.InvalidSortException, "Col: " + sortColumnIndex);
            }
        }
    }
}