using DataAnnotationsExtensions;
using Penates.App_GlobalResources.Forms;
using Penates.Models.Validators;
using Penates.Utils.JSON;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.DC {
    public class RackViewModel {

        public RackViewModel() {
            this.Width = 10;
            this.Height = 10;
            this.Depth = 10;
            this.Size = 1000;
            this.UsedSpace = 0;
            this.setPrecentages();
            if (this.ShelfsNumber.HasValue && this.ShelfsNumber.Value > 0) {
                this.ShelfHeight = this.Height / this.ShelfsNumber.Value;
            }
            if (this.DivitionsNumber.HasValue && this.DivitionsNumber.Value > 0) {
                this.DivitionWidth = this.Width / this.DivitionsNumber.Value;
            }
        }

        [Display(Name = "RackID", ResourceType = typeof(ModelFormsResources))]
        public long RackID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "HallID", ResourceType = typeof(ModelFormsResources))]
        public long? HallID { get; set; }

        [Integer(ErrorMessageResourceName = "IntegerError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, Int32.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "ShelfsNumber", ResourceType = typeof(ModelFormsResources))]
        public int? ShelfsNumber { get; set; }

        [Integer(ErrorMessageResourceName = "IntegerError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, Int32.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "DivitionNumber", ResourceType = typeof(ModelFormsResources))]
        public int? DivitionsNumber { get; set; }

        [Display(Name = "HallDescription", ResourceType = typeof(ModelFormsResources))]
        public string HallName { get; set; }

        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "SectorID", ResourceType = typeof(ModelFormsResources))]
        public long? SectorID { get; set; }

        [Display(Name = "SectorDescription", ResourceType = typeof(ModelFormsResources))]
        public string SectorName { get; set; }

        [Display(Name = "DepositID", ResourceType = typeof(ModelFormsResources))]
        public long? DepositID { get; set; }

        [Display(Name = "DepositDescription", ResourceType = typeof(ModelFormsResources))]
        public string DepositName { get; set; }

        [DataType(DataType.Text)]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Description", ResourceType = typeof(ModelFormsResources))]
        public string Description { get; set; }

        [DataType(DataType.Text)]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Code", ResourceType = typeof(ModelFormsResources))]
        public string RackCode { get; set; }

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

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "UsedPercentage", ResourceType = typeof(ModelFormsResources))]
        public decimal UsedPercentage { get; set; }

        [Display(Name = "Categories", ResourceType = typeof(ModelFormsResources))]
        public string Categories { get; set; }

        [Display(Name = "DistributionCenterID", ResourceType = typeof(ModelFormsResources))]
        public long? DistributionCenter { get; set; }

        public IEnumerable<SelectItem> initialCategories { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "MaxDepth", ResourceType = typeof(ModelFormsResources))]
        public decimal? MaxDepth { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "MaxWidth", ResourceType = typeof(ModelFormsResources))]
        public decimal? MaxWidth { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "MaxHeight", ResourceType = typeof(ModelFormsResources))]
        public decimal? MaxHeight { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "MaxSize", ResourceType = typeof(ModelFormsResources))]
        public decimal? MaxSize { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "ShelfHeight", ResourceType = typeof(ModelFormsResources))]
        public decimal ShelfHeight { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "DivitionWidth", ResourceType = typeof(ModelFormsResources))]
        public decimal DivitionWidth { get; set; }

        [Display(Name = "error")]
        public string error { get; set; }

        public void setPrecentages() {
            if (this.Size == 0) {
                this.UsedPercentage = 100;
            } else {
                this.UsedPercentage = (this.UsedSpace / this.Size) * 100;
            }
        }

        public void calculateSize() {
            this.Size = this.Depth * this.Height * this.Width;
        }
    }
}