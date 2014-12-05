using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Models {
    public interface ITableViewModel {

        string AjaxRequest { get; set; }

        string DeleteController { get; set; }

        string DeleteAction { get; set; }

        bool ToggleDeleteMessage { get; set; }

        string DeleteText { get; set; }

        string DeleteConfirmMessage { get; set; }

        string DeleteText2 { get; set; }

        string DeleteConfirmMessage2 { get; set; }

        bool ServerProcessing { get; set; }

        string RefreshFunction { get; set; }

        string tableID { get; set; }

        Object Params { get; set; }

        string EditController { get; set; }

        string EditAction { get; set; }

        bool useJQueryEditFunction { get; set; }

        string JQueryEditFunction { get; set; }

        bool useDefault { get; set; }

        string concatWithID(string s);
    }
}
