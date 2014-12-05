using DataAnnotationsExtensions;
using Penates.App_GlobalResources.Forms;
using Penates.Models.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.DC {
    public class ContainerTypeViewModel {

        [Display(Name = "ContainerTypeID", ResourceType = typeof(ModelFormsResources))]
        public long? ContainerTypeID { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Description", ResourceType = typeof(ModelFormsResources))]
        public string Description { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, 1E26, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [GraterThan(0, ErrorMessageResourceName = "GraterThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Depth", ResourceType = typeof(ModelFormsResources))]
        public decimal Depth { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, 1E26, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [GraterThan(0, ErrorMessageResourceName = "GraterThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Width", ResourceType = typeof(ModelFormsResources))]
        public decimal Width { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, 1E26, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [GraterThan(0, ErrorMessageResourceName = "GraterThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Height", ResourceType = typeof(ModelFormsResources))]
        public decimal Height { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "Capacity", ResourceType = typeof(ModelFormsResources))]
        public decimal Size { get; set; }

        [Display(Name = "error")]
        public string error { get; set; }
    }
}