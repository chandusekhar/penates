using Penates.App_GlobalResources.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.Forms {
    public class PackViewModel {

        public PackViewModel() {
            this.HasExpirationDate = false;
        }

        [Display(Name = "PackID", ResourceType = typeof(ModelFormsResources))]
        public long PackID { get; set; }

        [DataType(DataType.Text, ErrorMessageResourceName = "DescriptionType", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [MaxLength(128, ErrorMessageResourceName = "DescriptionLength", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Description", ResourceType = typeof(ModelFormsResources))]
        public string Description { get; set; }

        [DataType(DataType.DateTime, ErrorMessageResourceName = "DateTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "ExpirationDate", ResourceType = typeof(ModelFormsResources))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? ExpirationDate { get; set; }

        [DataType(DataType.Text, ErrorMessageResourceName = "DescriptionType", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [MaxLength(128, ErrorMessageResourceName = "DescriptionLength", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "SerialNumber", ResourceType = typeof(ModelFormsResources))]
        public string SerialNumber { get; set; }

        [Display(Name = "HasExpirationDate", ResourceType = typeof(ModelFormsResources))]
        public bool HasExpirationDate { get; set; }

        [Display(Name = "error")]
        public string error { get; set; }
    }
}