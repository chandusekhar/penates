using Penates.Database;
using Penates.Models;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Services {
    public interface IClientService {

        /// <summary> Guarda los datos de una producto en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como ProductViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        long Save(ClientViewModel client);

        Status Deactivate(long id);

        /// <summary> Retorna el View Model Listo para mostrar en el Formulario</summary>
        /// <param name="id">Id de la categoria</param>
        /// <returns>View Model de la categoria</returns>
        ClientViewModel getData(long id);

        IQueryable<Client> getData();

        IQueryable<Client> getData(bool includeDeleted);

        IQueryable<Client> getAutocomplete(string search);

        IQueryable<Client> getAutocomplete(string search, bool includeDeleted);

        IQueryable<Client> search(IQueryable<Client> query, string search);

        IQueryable<Client> sort(IQueryable<Client> query, int index, string direction);

        IQueryable<Client> sort(IQueryable<Client> query, Sorts sort);

        IQueryable<Client> filterByCity(IQueryable<Client> query, long? cityID);

        IQueryable<Client> filterByState(IQueryable<Client> query, long? stateID);

        IQueryable<Client> filterByCountry(IQueryable<Client> query, long? countryID);

        IQueryable<Client> filterByDeactivated(IQueryable<Client> query, int? active);

        List<ClientTableJson> toJsonArray(IQueryable<Client> query);

        List<ClientTableJson> toJsonArray(ICollection<Client> list);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<Client> query);
    }
}
