using Penates.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Penates.Interfaces.Services {
    public interface IGeoService {

        /// <summary>Retorna el Pais con los datos para mostrar</summary>
        /// <param name="id">Id del Pais</param>
        /// <returns>Country</returns>
        Country getCountryData(long id);

        /// <summary>Retorna el Estado con los datos para mostrar</summary>
        /// <param name="id">Id del Estado</param>
        /// <returns>Province</returns>
        Province getStateData(long id);

        /// <summary>Retorna la Ciudad con los datos para mostrar</summary>
        /// <param name="id">Id de la Ciudad</param>
        /// <returns>City</returns>
        City getCityData(long id);

        /// <summary>Obtiene la lista de paises</summary>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        SelectList getCountryList();

        /// <summary>Obtiene la lista de paises</summary>
        /// <param name="includeAll">Indica si se agrega o no la opcion de All</param>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        SelectList getCountryList(bool includeAll);

        /// <summary>Obtiene la lista de paises con el mensaje de Seleccione</summary>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        SelectList getCountryListWMessage();

        /// <summary>Obtiene la lista de paises y elije el pais pasado por parametro</summary>
        /// <param name="categoryId">ID del pais a elegir</param>
        /// <returns>Selectlist para mostrar en el DropDownList</returns>
        /// <exception cref="DatabaseException"
        SelectList getCountryList(long countryID);

        /// <summary>Obtiene la lista de Estados</summary>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        SelectList getStateList(long countryID);

        /// <summary>Obtiene la lista de Estados</summary>
        /// <param name="includeAll">Indica si se agrega o no la opcion de All</param>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        SelectList getStateList(long countryID, bool includeAll);

        /// <summary>Obtiene la lista de estados y elije el estado pasado por parametro</summary>
        /// <param name="categoryId">ID del estado a elegir</param>
        /// <returns>Selectlist para mostrar en el DropDownList</returns>
        /// <exception cref="DatabaseException"
        SelectList getStateList(long countryID, long stateID);

        /// <summary>Obtiene la lista de Ciudades</summary>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        SelectList getCityList(long stateID);

        /// <summary>Obtiene la lista de Estados</summary>
        /// <param name="includeAll">Indica si se agrega o no la opcion de All</param>
        /// <returns>SelectList</returns>
        /// <exception cref="DatabaseException"
        SelectList getCityList(long stateID, bool includeAll);

        /// <summary>Obtiene la lista de ciudades y elije la ciudad pasado por parametro</summary>
        /// <param name="categoryId">ID de la ciudad a elegir</param>
        /// <returns>Selectlist para mostrar en el DropDownList</returns>
        /// <exception cref="DatabaseException"
        SelectList getCityList(long stateID, long cityID);
    }
}
