using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penates.Services.Geography;
using Penates.Interfaces.Services;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Penates.Database;
using System.Linq;

namespace Penates.Tests.Services.Geography {
    [TestClass]
    public class GeoServiceTest {
        /// <summary>Obtiene la lista de Paises</summary>
        [TestMethod]
        public void getCountryList() {
            this.getCountries(false);
        }

        /// <summary>Obtiene la lista de Paises incluyendo All</summary>
        [TestMethod]
        public void getCountryListAll() {
            this.getCountries(true);
        }

        private void getCountries(bool showAll) {
            IGeoService service = new GeoService();
            SelectList countries = service.getCountryList(showAll);

            foreach (Country item in countries.Items) {
                Console.WriteLine(item.CountryID + ": " + item.Name.Trim());
            }
        }

        /// <summary>Obtiene la lista de Provincias</summary>
        [TestMethod]
        public void getProvinceList() {
            IGeoService service = new GeoService();
            SelectList provinces = service.getStateList(11);

            foreach (Province province in provinces.Items) {
                Console.WriteLine(province.ProvinceID + ": " + province.Name.Trim());
            }
        }

        /// <summary>Obtiene la lista de Ciudades</summary>
        [TestMethod]
        public void getCityList() {
            IGeoService service = new GeoService();
            SelectList cities = service.getCityList(355);

            foreach (City city in cities.Items) {
                Console.WriteLine(city.CityID + ": " + city.Name.Trim());
            }
        }

        /// <summary>Test: Obtener datos del pais</summary>
        [TestMethod]
        public void retrieveCountryInfo() {
            IGeoService service = new GeoService();

            Country country = service.getCountryData(11);

            Console.WriteLine(country.CountryID + ": " + country.Name.Trim() + ", " + country.ISO);
        }

        /// <summary>Test: Obtener datos de la Provincia</summary>
        [TestMethod]
        public void retrieveStateInfo() {
            IGeoService service = new GeoService();

            Province state = service.getStateData(355);

            Console.WriteLine(state.ProvinceID + ": " + state.Name.Trim() + ", " + state.ISO);
        }

        /// <summary>Test: Obtener datos de la Provincia</summary>
        [TestMethod]
        public void retrieveStateInfoError() {
            IGeoService service = new GeoService();

            Province state = service.getStateData(11);

            Assert.AreEqual(null, state);
        }

        /// <summary>Test: Obtener datos de la Ciudad</summary>
        [TestMethod]
        public void retrieveCityInfo() {
            IGeoService service = new GeoService();

            City city = service.getCityData(17657);

            Console.WriteLine(city.CityID + ": " + city.Name.Trim() + ", " + city.ISO);
        }
    }
}
