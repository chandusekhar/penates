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
    public interface IPackService {

        /// <summary> Guarda los datos de una producto en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como ProductViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        long Save(PackViewModel pack);

        bool Delete(long id);

        /// <summary> Retorna el View Model Listo para mostrar en el Formulario</summary>
        /// <param name="id">Id de la categoria</param>
        /// <returns>View Model de la categoria</returns>
        PackViewModel getData(long id);

        IQueryable<Pack> getData();

        IQueryable<Pack> getData(bool includeDeleted);

        IQueryable<Pack> getAutocomplete(string search);

        IQueryable<Pack> search(IQueryable<Pack> query, string search);

        IQueryable<Pack> sort(IQueryable<Pack> query, int index, string direction);

        IQueryable<Pack> sort(IQueryable<Pack> query, Sorts sort);

        List<PackTableJson> toJsonArray(IQueryable<Pack> query);

        List<PackTableJson> toJsonArray(ICollection<Pack> list);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<Pack> query);
    }
}
