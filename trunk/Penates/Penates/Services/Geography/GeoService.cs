using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Interfaces.Services;
using Penates.Models;
using Penates.Repositories.Geography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Services.Geography {
    public class GeoService: IGeoService {

        ICountryRepository countryRepo = new CountryRepository();
        IStateRepository stateRepo = new StateRepository();
        ICityRepository cityRepo = new CityRepository();

        /// <summary>Retorna el Pais con los datos para mostrar</summary>
        /// <param name="id">Id del Pais</param>
        /// <returns>Country</returns>
        public Country getCountryData(long id) {
            return this.countryRepo.getData(id);
        }

        /// <summary>Retorna el Estado con los datos para mostrar</summary>
        /// <param name="id">Id del Estado</param>
        /// <returns>Province</returns>
        public Province getStateData(long id) {
            return this.stateRepo.getProvinceData(id);
        }

        /// <summary>Retorna la Ciudad con los datos para mostrar</summary>
        /// <param name="id">Id de la Ciudad</param>
        /// <returns>City</returns>
        public City getCityData(long id) {
            return this.cityRepo.getCityData(id);
        }

        /// <summary>Obtiene la lista de paises</summary>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getCountryList() {
            return this.getCountryList(false);
        }

        /// <summary>Obtiene la lista de paises</summary>
        /// <param name="includeAll">Indica si se agrega o no la opcion de All</param>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getCountryList(bool includeAll) {
            var countries = this.getCountries();
            if (includeAll) {
                countries.Insert(0, new Country { CountryID = -1, Name = Resources.Resources.All });
                return new SelectList(countries, "CountryID", "Name", -1);
            }
            return new SelectList(countries, "CountryID", "Name");
        }

        /// <summary>Obtiene la lista de paises con el mensaje de Seleccione</summary>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getCountryListWMessage() {
            var countries = this.getCountries();
            countries.Insert(0, new Country { CountryID = -1, Name = Resources.Messages.SelectCountry });
            return new SelectList(countries, "CountryID", "Name", -1);
        }

        /// <summary>Obtiene la lista de paises y elije el pais pasado por parametro</summary>
        /// <param name="categoryId">ID del pais a elegir</param>
        /// <returns>Selectlist para mostrar en el DropDownList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getCountryList(long countryID) {
            var list = this.getCountries();
            return new SelectList(list, "CountryID", "Name", countryID);
        }

        /// <summary>Obtiene la lista de Estados</summary>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getStateList(long countryID) {
            return this.getStateList(countryID, false);
        }

        /// <summary>Obtiene la lista de Estados</summary>
        /// <param name="includeAll">Indica si se agrega o no la opcion de All</param>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getStateList(long countryID, bool includeAll) {
            var states = this.getStates(countryID);
            if (includeAll) {
                states.Insert(0, new Province { ProvinceID = -1, Name = Resources.Resources.All });
                return new SelectList(states, "ProvinceID", "Name", -1);
            }
            return new SelectList(states, "ProvinceID", "Name");
        }

        /// <summary>Obtiene la lista de estados y elije el estado pasado por parametro</summary>
        /// <param name="categoryId">ID del estado a elegir</param>
        /// <returns>Selectlist para mostrar en el DropDownList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getStateList(long countryID, long stateID) {
            var list = this.getStates(countryID);
            return new SelectList(list, "ProvinceID", "Name", stateID);
        }

        /// <summary>Obtiene la lista de Ciudades</summary>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getCityList(long stateID) {
            return this.getCityList(stateID, false);
        }

        /// <summary>Obtiene la lista de Estados</summary>
        /// <param name="includeAll">Indica si se agrega o no la opcion de All</param>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getCityList(long stateID, bool includeAll) {
            var cities = this.getCities(stateID);
            if (includeAll) {
                cities.Insert(0, new City { CityID = -1, Name = Resources.Resources.All });
                return new SelectList(cities, "CityID", "Name", -1);
            }
            return new SelectList(cities, "CityID", "Name");
        }

        /// <summary>Obtiene la lista de ciudades y elije la ciudad pasado por parametro</summary>
        /// <param name="categoryId">ID de la ciudad a elegir</param>
        /// <returns>Selectlist para mostrar en el DropDownList</returns>
        /// <exception cref="DatabaseException"
        public SelectList getCityList(long stateID, long cityID) {
            var list = this.getCities(stateID);
            return new SelectList(list, "CityID", "Name", cityID);
        }

        /// <summary>Obtiene los paises existentes para mostrar</summary>
        /// <returns>SelectList: Lista de las categorias</returns>
        /// <exception cref="DatabaseException"
        private List<Country> getCountries() {
            try {
                return this.countryRepo.getData().ToList();
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        /// <summary>Obtiene las provincias existentes para mostrar</summary>
        /// <returns>SelectList: Lista de las categorias</returns>
        /// <exception cref="DatabaseException"
        private List<Province> getStates(long countryID) {
            try {
                return this.stateRepo.getProvinces(countryID).ToList();
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }

        /// <summary>Obtiene las ciudades existentes para mostrar</summary>
        /// <returns>SelectList: Lista de las categorias</returns>
        /// <exception cref="DatabaseException"
        private List<City> getCities(long stateID) {
            try {
                return this.cityRepo.getCities(stateID).ToList();
            } catch (Exception e) {
                throw new DatabaseException(e.Message);
            }
        }
    }
}