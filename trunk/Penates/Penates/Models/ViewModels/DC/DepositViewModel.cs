using DataAnnotationsExtensions;
using Penates.App_GlobalResources.Forms;
using Penates.Models.Validators;
using Penates.Utils.JSON;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Models.ViewModels.DC {
    public class DepositViewModel {

        public DepositViewModel() {
            this.Width = 10;
            this.Height = 10;
            this.Depth = 10;
            this.Size = 1000;
            this.Floor = 0;
            this.UsedSpace = 0;
            this.UsableSpace = 0;
            this.UsableUsedSpace = 0;
            this.setPrecentages();
        }

        [Display(Name = "DepositID", ResourceType = typeof(ModelFormsResources))]
        public long DepositID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "DistributionCenterID", ResourceType = typeof(ModelFormsResources))]
        public long DistributionCenterID { get; set; }

        [DataType(DataType.Text)]
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

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "UsedSpace", ResourceType = typeof(ModelFormsResources))]
        public decimal UsedSpace { get; set; }

        [Integer(ErrorMessageResourceName = "IntegerError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, Int32.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Floor", ResourceType = typeof(ModelFormsResources))]
        public int Floor { get; set; }

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
        [Display(Name = "UsedPercentage", ResourceType = typeof(ModelFormsResources))]
        public decimal UsedPercentage { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "UsableSpacePercentage", ResourceType = typeof(ModelFormsResources))]
        public decimal UsableSpacePercentage { get; set; }

        [Display(Name = "Categories", ResourceType = typeof(ModelFormsResources))]
        public string Categories { get; set; }

        public IEnumerable<SelectItem> initialCategories { get; set; }

        [Display(Name = "error")]
        public string error { get; set; }

        public void setPrecentages() {
            if (this.Size == 0) {
                this.UsedPercentage = 100;
            } else {
                this.UsedPercentage = (this.UsedSpace / this.Size) * 100;
            }
            if (this.UsableSpace == 0) {
                this.UsableSpacePercentage = 100;
            } else {
                this.UsableSpacePercentage = (this.UsableUsedSpace / this.UsableSpace) * 100;
            }
        }
    }
}