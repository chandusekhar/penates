using Penates.Database;
using Penates.Models.ViewModels.DC;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories {
    public interface IContainerTypeRepository {

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>true si funciona, sino una excepcion</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Save(ContainerTypeViewModel type);

        bool Delete(long id);

        ContainerType getData(long id);

        IQueryable<ContainerType> getData();

        decimal getContainerArea(long ContainerTypeID);

        decimal getAvaibleContainersQuantity(long ContainerTypeID);

        string getContainerTypeName(long ContainerTypeID);

        IQueryable<ContainerType> getAutocomplete(string search);

        IQueryable<ContainerType> searchAndRank(string search);

        IQueryable<ContainerType> searchAndRank(IQueryable<ContainerType> data, string search);

        IQueryable<ContainerType> search(IQueryable<ContainerType> query, string search);

        IQueryable<ContainerType> search(IQueryable<ContainerType> query, List<string> search);

        IQueryable<ContainerType> sort(IQueryable<ContainerType> query, Sorts sort);
       
        decimal getContainerSize(long ContainerTypeID);

        decimal getContainerHeight(long ContainerTypeID);
    }
}
