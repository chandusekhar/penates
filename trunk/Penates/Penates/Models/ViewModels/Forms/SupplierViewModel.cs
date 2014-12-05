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

namespace Penates.Models.ViewModels.Forms {
    public partial class SupplierViewModel : IFormViewModel {

            [Display(Name = "SupplierID", ResourceType = typeof(ModelFormsResources))]
            public long SupplierID { get; set; }

            [DataType(DataType.Text)]
            [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Display(Name = "Address", ResourceType = typeof(ModelFormsResources))]
            public string Address { get; set; }

            [Email(ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
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

            public string error { get; set; }
        }
}