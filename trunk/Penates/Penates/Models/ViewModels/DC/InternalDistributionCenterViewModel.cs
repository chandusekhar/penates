using DataAnnotationsExtensions;
using Penates.App_GlobalResources.Forms;
using Penates.Models.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.DC {
    public class InternalDistributionCenterViewModel : DistributionCenterViewModel {

        public InternalDistributionCenterViewModel() {
            this.Width = 10;
            this.Height = 10;
            this.Depth = 10;
            this.Size = 1000;
            this.Floors = 0;
            this.UsableUsedSpace = 0;
            this.UsableSpace = 0;
            this.UsedSpace = 0;
            this.setPrecentages();
        }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Integer(ErrorMessageResourceName = "IntegerError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, Int32.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Floors", ResourceType = typeof(ModelFormsResources))]
        public int Floors { get; set; }

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
        [Display(Name = "Size", ResourceType = typeof(ModelFormsResources))]
        public decimal Size { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "UsedSpace", ResourceType = typeof(ModelFormsResources))]
        public decimal UsedSpace { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "UsedPercentage", ResourceType = typeof(ModelFormsResources))]
        public decimal UsedPercentage { get; set; }

        [Display(Name = "AmountOfProducts", ResourceType = typeof(ModelFormsResources))]
        public long AmountOfProducts { get; set; }

        public new void setPrecentages() {
            if (this.Size == 0) {
                this.UsedPercentage = 100;
            } else {
                this.UsedPercentage = (this.UsedSpace / this.Size) * 100;
            }
            base.setPrecentages();
        }
    }
}