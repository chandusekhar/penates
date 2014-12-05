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
using System.Web.Mvc;

namespace Penates.Interfaces.Services {
    public interface IStatusService {

        /// <summary> Guarda los datos de una producto en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como ProductViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        long Save(StatusViewModel pack);

        bool Delete(long id);

        /// <summary> Retorna el View Model Listo para mostrar en el Formulario</summary>
        /// <param name="id">Id de la categoria</param>
        /// <returns>View Model de la categoria</returns>
        StatusViewModel getData(long id);

        IQueryable<ItemsState> getData();

        IQueryable<ItemsState> getData(bool includeDeleted);

        IQueryable<ItemsState> getAutocomplete(string search);

        IQueryable<ItemsState> search(IQueryable<ItemsState> query, string search);

        IQueryable<ItemsState> sort(IQueryable<ItemsState> query, int index, string direction);

        IQueryable<ItemsState> sort(IQueryable<ItemsState> query, Sorts sort);

        List<StatusTableJson> toJsonArray(IQueryable<ItemsState> query);

        List<StatusTableJson> toJsonArray(ICollection<ItemsState> list);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<ItemsState> query);

        ItemsState getDefaultState();

        /// <summary> Obtiene la lista de Categorias de Producto para mostrar en el formulario </summary>
        /// <returns>SelectList - Lista de Categorias de Producto</returns>
        /// <exception cref="DatabaseException"
        SelectList getStatusList();

        /// <summary> Obtiene la lista de categorias y elije la categoria pasada por parametro </summary>
        /// <param name="categoryId"> ID de la categoria a elejir </param>
        /// <returns>Selectlist para mostrar en el DropDownList</returns>
        /// <exception cref="DatabaseException"
        SelectList getStatusList(long statusID);
    }
}
