using Penates.App_GlobalResources.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.Forms {
    public class StatusViewModel {

        [Display(Name = "StatusID", ResourceType = typeof(ModelFormsResources))]
        public long? StatusID { get; set; }

        [DataType(DataType.Text, ErrorMessageResourceName = "DescriptionType", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [MaxLength(128, ErrorMessageResourceName = "DescriptionLength", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Description", ResourceType = typeof(ModelFormsResources))]
        public string Description { get; set; }

        [Display(Name = "error")]
        public string error { get; set; }
    }
}