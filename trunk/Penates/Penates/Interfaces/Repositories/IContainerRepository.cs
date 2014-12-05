using Penates.Database;
using Penates.Models.ViewModels.DC;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories
{
    public interface IContainerRepository
    {

        /// <summary> Guarda los datos de un Container en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del container</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Save(ContainerViewModel container);

        Container getContainerInfo(long id);

        IQueryable<Container> getData();

        IQueryable<Container> search(IQueryable<Container> query, string search);

        
        IQueryable<Container> search(IQueryable<Container> query, List<string> search);

        IQueryable<Container> sort(IQueryable<Container> query, Sorts sort);


        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        bool Delete(long ContainerID);

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        IQueryable<Container> getAutocomplete(string search, long? dcID, long? depositID, long? temporaryDepositID, long? sectorID, long? rackID, long? typeID);

        Container getEmptyContainer(long ContainerTypeID);

        void setContainerUsedSpace(long ContainerID, decimal UsedSapce);

        IQueryable<Container> filterByDistributionCenter(IQueryable<Container> query, long dcID);

        IQueryable<Container> filterByDeposit(IQueryable<Container> query, long depositID);

        IQueryable<Container> filterByTemporaryDeposit(IQueryable<Container> query, long depositID);

        IQueryable<Container> filterBySector(IQueryable<Container> query, long sectorID);

        IQueryable<Container> filterByRack(IQueryable<Container> query, long rackID);

        IQueryable<Container> filterByType(IQueryable<Container> query, long typeID);
    }
}
