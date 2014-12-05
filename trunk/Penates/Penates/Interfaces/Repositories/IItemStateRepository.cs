using Penates.Database;
using Penates.Models.ViewModels.Forms;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories {
    public interface IItemStateRepository {

        /// <summary> Guarda los datos de un Estado</summary>
        /// <param name="prod">El viewmodel con los datos del producto</param>
        /// <returns>true si funciona, sino una excepcion</returns>
        /// <exception cref="DatabaseException">Se lanza cuando hay un error con la BD</exception>
        long Save(StatusViewModel status);

        bool Delete(long id);

        ItemsState getData(long id);

        IQueryable<ItemsState> getData();

        IQueryable<ItemsState> getData(bool includeDeleted);

        IQueryable<ItemsState> getAutocomplete(string search);

        IQueryable<ItemsState> searchAndRank(string search);

        IQueryable<ItemsState> searchAndRank(IQueryable<ItemsState> data, string search);

        IQueryable<ItemsState> search(IQueryable<ItemsState> query, string search);

        IQueryable<ItemsState> search(IQueryable<ItemsState> query, List<string> search);

        IQueryable<ItemsState> sort(IQueryable<ItemsState> query, Sorts sort);

        ItemsState getDefaultState();
    }
}
