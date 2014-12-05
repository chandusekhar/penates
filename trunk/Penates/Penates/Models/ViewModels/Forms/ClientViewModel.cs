using DataAnnotationsExtensions;
using Penates.App_GlobalResources.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Models.ViewModels.Forms {
    public class ClientViewModel {

        public ClientViewModel() {
            List<SelectListItem> stateListInitialize = new List<SelectListItem>();
            SelectListItem stateIni = new SelectListItem();
            stateIni.Text = Resources.Messages.SelectState;
            stateIni.Value = "-1";
            stateListInitialize.Add(stateIni);
            this.StateList = new SelectList(stateListInitialize, "Value", "Text");

            List<SelectListItem> cityListInitialize = new List<SelectListItem>();
            SelectListItem cityIni = new SelectListItem();
            cityIni.Text = Resources.Messages.SelectCity;
            cityIni.Value = "-1";
            cityListInitialize.Add(cityIni);
            this.CityList = new SelectList(cityListInitialize, "Value", "Text");
        }

        [Display(Name = "ClientID", ResourceType = typeof(ModelFormsResources))]
        public long ClientID { get; set; }

        [DataType(DataType.Text)]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Address", ResourceType = typeof(ModelFormsResources))]
        public string Address { get; set; }

        [Email(ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [StringLength(255, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Email", ResourceType = typeof(ModelFormsResources))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DataType(DataType.Text)]
        [StringLength(128, ErrorMessageResourceName = "NameLength", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Name", ResourceType = typeof(ModelFormsResources))]
        public string Name { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = null, ErrorMessageResourceName = "PhoneError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [StringLength(64, MinimumLength = 8, ErrorMessageResourceName = "MinMaxLenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Phone", ResourceType = typeof(ModelFormsResources))]
        public string Phone { get; set; }

        [DataType(DataType.Text)]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "ContactName", ResourceType = typeof(ModelFormsResources))]
        public string ContactName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DataType(DataType.Text)]
        [StringLength(64, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "CUIT", ResourceType = typeof(ModelFormsResources))]
        public string CUIT { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "CityID", ResourceType = typeof(ModelFormsResources))]
        public long CityID { get; set; }

        [Display(Name = "CityName", ResourceType = typeof(ModelFormsResources))]
        public string CityName { get; set; }

        [Display(Name = "Active", ResourceType = typeof(ModelFormsResources))]
        public bool Active { get; set; }

        [Display(Name = "Error", ResourceType = typeof(ModelFormsResources))]
        public string error { get; set; }

        public IEnumerable<SelectListItem> CountryList { get; set; }

        public IEnumerable<SelectListItem> StateList { get; set; }

        public IEnumerable<SelectListItem> CityList { get; set; }

        public long CountryID { get; set; }

        public long StateID { get; set; }

        public string CountryName { get; set; }

        public string StateName { get; set; }
    }
}