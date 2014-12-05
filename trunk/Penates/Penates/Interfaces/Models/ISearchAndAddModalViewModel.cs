using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Models {
    public interface ISearchAndAddModalViewModel {

        string ModalURL { get; set; }

        string ModalEditURL { get; set; }
    }
}
