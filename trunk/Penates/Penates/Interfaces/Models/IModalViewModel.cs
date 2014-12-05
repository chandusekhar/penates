using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Models {
    public interface IModalViewModel {

        bool isAjax { get; set; }

        string ViewId { get; set; }

        string concatWithID(string s);

        string getJqueryID(string s);
    }
}
