using Penates.Database;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories {
    public interface IClientRepository {

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>true si funciona, sino una excepcion</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Save(ClientViewModel client);

        Status Deactivate(long id);

        Client getData(long id);

        IQueryable<Client> getData();

        IQueryable<Client> getData(bool includeDeleted);

        IQueryable<Client> getAutocomplete(string search);

        IQueryable<Client> getAutocomplete(string search, bool includeDeleted);

        IQueryable<Client> searchAndRank(string search, bool includeDeleted);

        IQueryable<Client> searchAndRank(IQueryable<Client> data, string search);

        IQueryable<Client> search(IQueryable<Client> query, string search);

        IQueryable<Client> search(IQueryable<Client> query, List<string> search);

        IQueryable<Client> sort(IQueryable<Client> query, Sorts sort);

        IQueryable<Client> filterByCity(IQueryable<Client> query, long cityID);

        IQueryable<Client> filterByState(IQueryable<Client> query, long provinceID);

        IQueryable<Client> filterByCountry(IQueryable<Client> query, long countryID);

        IQueryable<Client> filterByDisabled(IQueryable<Client> query, bool disabled);
    }
}
