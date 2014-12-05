using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penates.Repositories.Geography;
using Penates.Interfaces.Repositories;
using Penates.Database;
using System.Linq;

namespace Penates.Tests.Repositories.Geography {
    /// <summary>Test de Paises</summary>
    [TestClass]
    public class GeographyRepositoryTest {
        [TestMethod]
        public void retrieveCountries() {
            ICountryRepository repo = new CountryRepository();

            IQueryable<Country> countries = repo.getData();

            foreach(Country country in countries){
                Console.WriteLine(country.CountryID + ": " + country.Name.Trim());
            }
        }

        /// <summary>Test de Estados</summary>
        [TestMethod]
        public void retrieveStates() {
            IStateRepository repo = new StateRepository();

            IQueryable<Province> provinces = repo.getProvinces(11);

            foreach (Province province in provinces) {
                Console.WriteLine(province.ProvinceID + ": " + province.Name.Trim());
            }
        }

        /// <summary>Test de Ciudades</summary>
        [TestMethod]
        public void retrieveCities() {
            ICityRepository repo = new CityRepository();

            IQueryable<City> cities = repo.getCities(355);

            foreach (City city in cities) {
                Console.WriteLine(city.CityID + ": " + city.Name.Trim());
            }
        }

        /// <summary>Test: Obtener datos de la Ciudad</summary>
        [TestMethod]
        public void retrieveCityInfo() {
            ICityRepository repo = new CityRepository();

            City city = repo.getCityData(17657);

            Console.WriteLine(city.CityID + ": " + city.Name.Trim() + ", " + city.ISO);
        }
    }
}
