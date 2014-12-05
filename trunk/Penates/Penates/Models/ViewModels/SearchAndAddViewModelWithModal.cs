using DataAnnotationsExtensions;
using Penates.App_GlobalResources.Forms;
using Penates.Interfaces;
using Penates.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels {
    public class SearchAndAddViewModelWithModal : ISearchAndAddViewModel, ISearchAndAddModalViewModel {

        public SearchAndAddViewModelWithModal() : this("searchAndAdd") {
        
        }

        public SearchAndAddViewModelWithModal(string id) {
            this.ConfirmMessage = @String.Format(Resources.Messages.SureToAdd, "");
            this.UseConfirmMessage = true;
            this.TableServerProcessing = true;
            this.TableUpdateFunctionName = "refreshTable"+id;
            this.TableId = id;
        }

        public string ConfirmMessage { get; set; }

        public string TablePartialView { get; set; }

        public Object Params { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "SearchID", ResourceType = typeof(ModelFormsResources))]
        public long? AddID { get; set; }

        public string AjaxRequest { get; set; }

        public string TableAjaxRequest { get; set; }

        public string SubmitController { get; set; }

        public string SubmitAction { get; set; }

        public string TableDeleteController { get; set; }

        public string TableDeleteAction { get; set; }

        public string TableDeleteText { get; set; }

        public string TableDeleteConfirmMessage { get; set; }

        public bool TableServerProcessing { get; set; }

        public bool UseConfirmMessage { get; set; }

        public string TableUpdateFunctionName { get; set; }

        public string TableId { get; set; }

        public string ModalURL { get; set; }

        public string ModalEditURL { get; set; }

        public string EditController { get; set; }

        public string EditAction { get; set; }

        public bool useJQueryEditFunction { get; set; }

        public string JQueryEditFunction { get; set; }

        public string concatWithId(string s){
            return s + this.TableId;
        }

        public string getJqueryID(string s) {
            return "#" + this.concatWithId(s);
        }
    }
}