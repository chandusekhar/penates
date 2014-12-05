using Penates.Database;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories {
    public interface IPackRepository {

        /// <summary> Guarda los datos de un Producto en la Base de Datos</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>true si funciona, sino una excepcion</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Save(PackViewModel pack);

        bool Delete(long id);

        Pack getData(long id);

        IQueryable<Pack> getData();

        IQueryable<Pack> getData(bool includeDeleted);

        IQueryable<Pack> getAutocomplete(string search);

        IQueryable<Pack> searchAndRank(string search);

        IQueryable<Pack> searchAndRank(IQueryable<Pack> data, string search);

        IQueryable<Pack> search(IQueryable<Pack> query, string search);

        IQueryable<Pack> search(IQueryable<Pack> query, List<string> search);

        IQueryable<Pack> sort(IQueryable<Pack> query, Sorts sort);
    }
}
