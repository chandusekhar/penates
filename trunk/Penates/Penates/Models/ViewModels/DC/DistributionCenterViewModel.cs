using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Penates.App_GlobalResources.Forms;
using Penates.Database;
using Penates.Utils;
using System.Drawing;
using System.Web.Helpers;
using Penates.Services.ABMs;
using Penates.Models.Validators;
using System.Web.Mvc;
using Penates.Interfaces.Models;

namespace Penates.Models.ViewModels.DC {
    public abstract class DistributionCenterViewModel : IFormViewModel {

        public DistributionCenterViewModel() {
            this.UsableSpace = 0;
            this.UsableUsedSpace = 0;

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

        [Display(Name = "DistributionCenterID", ResourceType = typeof(ModelFormsResources))]
        public long DistributionCenterID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "CityID", ResourceType = typeof(ModelFormsResources))]
        public long CityID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DataType(DataType.Text)]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Address", ResourceType = typeof(ModelFormsResources))]
        public string Address { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = null, ErrorMessageResourceName = "PhoneError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Telephone", ResourceType = typeof(ModelFormsResources))]
        public string Telephone { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, 1E36, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "UsableSpace", ResourceType = typeof(ModelFormsResources))]
        public decimal UsableSpace { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, 1E36, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "UsableUsedSpace", ResourceType = typeof(ModelFormsResources))]
        public decimal UsableUsedSpace { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "UsableSpacePercentage", ResourceType = typeof(ModelFormsResources))]
        public decimal UsableSpacePercentage { get; set; }

        public IEnumerable<SelectListItem> CountryList { get; set; }
        
        public long CountryID { get; set; }

        public long StateID { get; set; }

        public string CountryName { get; set; }

        public string StateName { get; set; }

        [Display(Name = "error")]
        public string error { get; set; }

        public string CityName { get; set; }

        public IEnumerable<SelectListItem> StateList { get; set; }

        public IEnumerable<SelectListItem> CityList { get; set; }

        public void setPrecentages() {
            if (this.UsableSpace == 0) {
                this.UsableSpacePercentage = 100;
            } else {
                this.UsableSpacePercentage = (this.UsableUsedSpace / this.UsableSpace) * 100;
            }
        }
    }
}