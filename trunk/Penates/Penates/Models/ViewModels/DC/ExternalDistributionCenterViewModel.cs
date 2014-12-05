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
    public partial class ExternalDistributionCenterViewModel : DistributionCenterViewModel {

        public ExternalDistributionCenterViewModel() {
            this.UsableUsedSpace = 0;
            this.UsableSpace = 0;
            this.HasMaxCapacity = true;
            this.setPrecentages();
        }

            [DataType(DataType.Text)]
            [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Display(Name = "ContactName", ResourceType = typeof(ModelFormsResources))]
            public string ContactName { get; set; }

            [DataType(DataType.Text)]
            [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Display(Name = "Telephone2", ResourceType = typeof(ModelFormsResources))]
            public string Telephone2 { get; set; }

            [Display(Name = "HasMaxCapacity", ResourceType = typeof(ModelFormsResources))]
            public bool HasMaxCapacity { get; set; }
    
    }
}