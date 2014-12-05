using Penates.Database;
using Penates.Models.ViewModels.DC;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories {
    public interface IRackRepository {

        /// <summary> Guarda los datos de un Rack en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Save(RackViewModel rack);

        Rack getRackInfo(long id);

        IQueryable<Rack> getData();

        IQueryable<Rack> search(IQueryable<Rack> query, string search);

        IQueryable<Rack> search(IQueryable<Rack> query, List<string> search);

        IQueryable<Rack> sort(IQueryable<Rack> query, Sorts sort);

        IQueryable<Rack> filterByDistributionCenter(IQueryable<Rack> query, long dcID);

        IQueryable<Rack> filterByDeposit(IQueryable<Rack> query, long depositID);

        IQueryable<Rack> filterBySector(IQueryable<Rack> query, long sectorID);

        IQueryable<Rack> filterByHall(IQueryable<Rack> query, long hallID);

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        bool Delete(long rackID);

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool addCategory(long rackID, long categoryID);

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool saveCategories(long rackID, List<long> categoryiesIDs);

        IQueryable<Rack> getAutocomplete(string search, long? dcID, long? depositID, long? sectorID, long? hallID);

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool unnasignCategory(long rackID, long categoryID);

        bool deleteCategories(long rackID);

        void updateUsedSpace(long ShelfSubdivisionID, decimal UsedSpaceInCm);

        decimal getShelfSubdivisionSize(long ShelfSubdivisionID);

        decimal getShelfSubdivisionUsedSpace(long ShelfSubdivisionID);
    }
}
