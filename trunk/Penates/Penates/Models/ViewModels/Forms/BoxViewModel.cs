using Penates.App_GlobalResources.Forms;
using Penates.Models.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.Forms {
    public class BoxViewModel : ABoxViewModel {

        public BoxViewModel()
            : base() {
        }

        [GraterOrEqualThanInteger(0, ErrorMessageResourceName = "GraterOrEqualThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "ContainerID", ResourceType = typeof(ModelFormsResources))]
        public long? ContainerID { get; set; }

        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "ContainerCode", ResourceType = typeof(ModelFormsResources))]
        public string ContainerCode { get; set; }
    }
}