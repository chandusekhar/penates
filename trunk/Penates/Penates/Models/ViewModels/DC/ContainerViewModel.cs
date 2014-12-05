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
    public class ContainerViewModel {

        public ContainerViewModel()
        {
            this.UsedSpace = 0;
        }

        [Display(Name = "ContainerID", ResourceType = typeof(ModelFormsResources))]
        public long? ContainerID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "ContainerTypesID", ResourceType = typeof(ModelFormsResources))]
        public long ContainerTypeID { get; set; }

        [Display(Name = "ContainerTypeDescription", ResourceType = typeof(ModelFormsResources))]
        public string ContainerTypeName { get; set; }

        [DataType(DataType.Text)]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Code", ResourceType = typeof(ModelFormsResources))]
        public string Code { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "UsedSpace", ResourceType = typeof(ModelFormsResources))]
        public decimal UsedSpace { get; set; }

        [Display(Name = "TemporaryDepositID", ResourceType = typeof(ModelFormsResources))]
        public long? TemporaryDepositID { get; set; }

        [Display(Name = "TemporaryDepositName", ResourceType = typeof(ModelFormsResources))]
        public string TemporaryDepositName { get; set; }

        [Display(Name = "IDShelfSubdivision", ResourceType = typeof(ModelFormsResources))]
        public long? IDShelfSubdivision { get; set; }

        [Display(Name = "ShelfSubdivisionName", ResourceType = typeof(ModelFormsResources))]
        public string ShelfSubdivisionName { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "Capacity", ResourceType = typeof(ModelFormsResources))]
        public decimal Size { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal Capacity { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "UsedPercentage", ResourceType = typeof(ModelFormsResources))]
        public decimal UsedPercentage { get; set; }

        [Display(Name = "error", ResourceType = typeof(ModelFormsResources))]
        public string error { get; set; }

        public void setPercentages()
        {
            if (this.Size == 0)
            {
                this.UsedPercentage = 100;
            }
            else
            {
                this.UsedPercentage = (this.UsedSpace / this.Size) * 100;
            }
        }

        [Display(Name = "RackID", ResourceType = typeof(ModelFormsResources))]
        public long? RackID { get; set; }

        [Display(Name = "RackCode", ResourceType = typeof(ModelFormsResources))]
        public string RackName { get; set; }

        [Display(Name = "HallID", ResourceType = typeof(ModelFormsResources))]
        public long? HallID { get; set; }

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

        [Display(Name = "DistributionCenterID", ResourceType = typeof(ModelFormsResources))]
        public long? DistributionCenter { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Depth", ResourceType = typeof(ModelFormsResources))]
        public decimal Depth { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Width", ResourceType = typeof(ModelFormsResources))]
        public decimal Width { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Height", ResourceType = typeof(ModelFormsResources))]
        public decimal Height { get; set; }
    }
}