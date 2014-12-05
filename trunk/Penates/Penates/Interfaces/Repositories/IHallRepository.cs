using Penates.Database;
using Penates.Models.ViewModels.DC;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories {
    public interface IHallRepository {

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>* long > 0 si el ID es valido</returns>
        /// <exception cref=></exception>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Save(HallViewModel hall);

        Hall getHallInfo(long id);

        IQueryable<Hall> getData();

        IQueryable<Hall> search(IQueryable<Hall> query, string search);

        IQueryable<Hall> search(IQueryable<Hall> query, List<string> search);

        IQueryable<Hall> sort(IQueryable<Hall> query, Sorts sort);

        IQueryable<Hall> filterByDistributionCenter(IQueryable<Hall> query, long dcID);

        IQueryable<Hall> filterByDeposit(IQueryable<Hall> query, long depositID);

        IQueryable<Hall> filterBySector(IQueryable<Hall> query, long sectorID);

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        bool Delete(long hallID);

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool addCategory(long hallID, long categoryID);

        /// <summary>Salva las nuevas categorias y elimina las viejas</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Lista con los ids de las categorias</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool saveCategories(long hallID, List<long> categoryiesIDs);

        IQueryable<Hall> getAutocomplete(string search, long? dcID, long? depositID, long? sectorID);

        /// <summary>Agrega categoria</summary>
        /// <param name="depositID">Id del deposito</param>
        /// <param name="categoryID">Id de la categoria a agregar</param>
        /// <returns>true si lo logra o excepcion</returns>
        /// <exception cref="ForeignKeyConstraintException">Hay que capturarla en el controller y pasar el error</exception>
        bool unnasignCategory(long hallID, long categoryID);

        bool deleteCategories(long hallID);

        IQueryable<Rack> getRacks(long HallID);
    }
}
