using Penates.Database;
using Penates.Models;
using Penates.Models.ViewModels.DC;
using Penates.Utils;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Penates.Interfaces.Services {
    public interface IContainerTypeService {

        /// <summary> Guarda los datos de una producto en la base de datos </summary>
        /// <param name="prod">Los datos a guardar como ProductViewModel</param>
        /// <returns>True o false dependiendo si resulta o no la operacion</returns>
        long Save(ContainerTypeViewModel type);

        bool Delete(long id);

        /// <summary> Retorna el View Model Listo para mostrar en el Formulario</summary>
        /// <param name="id">Id de la categoria</param>
        /// <returns>View Model de la categoria</returns>
        ContainerTypeViewModel getData(long id);

        IQueryable<ContainerType> getData();

        IQueryable<ContainerType> getAutocomplete(string search);

        List<AutocompleteItem> toJsonAutocomplete(IQueryable<ContainerType> query);

        IQueryable<ContainerType> search(IQueryable<ContainerType> query, string search);

        IQueryable<ContainerType> sort(IQueryable<ContainerType> query, int index, string direction);

        IQueryable<ContainerType> sort(IQueryable<ContainerType> query, Sorts sort);

        List<ContainerTypeTableJson> toJsonArray(IQueryable<ContainerType> query);

        List<ContainerTypeTableJson> toJsonArray(ICollection<ContainerType> list);

        bool areThereEnoughEmptyContainers(long ContainerTypeID, decimal Quantity);

        string getContainerTypeName(long ContainerTypeID);

        decimal getContainerArea(long ContainerID);

        decimal getContainerSize(long ContainerID);

        decimal getContainerHeight(long ContainerTypeID);

        /// <summary> Obtiene la lista de Categorias de Producto para mostrar en el formulario </summary>
        /// <returns>SelectList - Lista de Categorias de Producto</returns>
        /// <exception cref="DatabaseException"
        SelectList getTypeList();

        /// <summary> Obtiene la lista de Categorias de Producto para mostrar en el formulario </summary>
        /// <returns>SelectList - Lista de Categorias de Producto</returns>
        /// <exception cref="DatabaseException"
        SelectList getTypeList(bool includeAll);

        /// <summary> Obtiene la lista de categorias y elije la categoria pasada por parametro </summary>
        /// <param name="categoryId"> ID de la categoria a elejir </param>
        /// <returns>Selectlist para mostrar en el DropDownList</returns>
        /// <exception cref="DatabaseException"
        SelectList getTypeList(long categoryId);

        /// <summary> Obtiene la lista de categorias y elije la categoria pasada por parametro </summary>
        /// <param name="categoryId"> ID de la categoria a elejir </param>
        /// <returns>Selectlist para mostrar en el DropDownList</returns>
        /// <exception cref="DatabaseException"
        SelectList getTypeList(bool includeAll, long categoryId);
    }
}
