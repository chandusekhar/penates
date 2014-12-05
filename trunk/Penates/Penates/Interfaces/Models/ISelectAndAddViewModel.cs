using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Penates.Interfaces.Models {
    public interface ISelectAndAddViewModel {

        string ConfirmMessage { get; set; }

        string TablePartialView { get; set; }

        Object Params { get; set; }

        long? AddID { get; set; }

        string TableAjaxRequest { get; set; }

        string SubmitController { get; set; }

        string SubmitAction { get; set; }

        string TableDeleteController { get; set; }

        string TableDeleteAction { get; set; }

        string TableDeleteText { get; set; }

        string TableDeleteConfirmMessage { get; set; }

        bool TableServerProcessing { get; set; }

        bool UseConfirmMessage { get; set; }

        string TableUpdateFunctionName { get; set; }

        string TableId { get; set; }

        string EditController { get; set; }

        string EditAction { get; set; }

        IEnumerable<SelectListItem> Selectlist { get; set; }

        bool useJQueryEditFunction { get; set; }

        string JQueryEditFunction { get; set; }

        string concatWithId(string s);

        string getJqueryID(string s);
    }
}
