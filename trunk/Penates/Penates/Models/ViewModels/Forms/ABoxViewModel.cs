using DataAnnotationsExtensions;
using Penates.App_GlobalResources.Forms;
using Penates.Models.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Models.ViewModels.Forms {
    public abstract class ABoxViewModel {

        public ABoxViewModel() {
            this.AdquisitionDate = DateTime.Now;
        }

        [Display(Name = "BoxID", ResourceType = typeof(ModelFormsResources))]
        public long BoxID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "IsWaste", ResourceType = typeof(ModelFormsResources))]
        public bool IsWaste { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Reevaluate", ResourceType = typeof(ModelFormsResources))]
        public bool Reevaluate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Reserved", ResourceType = typeof(ModelFormsResources))]
        public bool Reserved { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Transit", ResourceType = typeof(ModelFormsResources))]
        public bool Transit { get; set; }

        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "ProductID", ResourceType = typeof(ModelFormsResources))]
        public long? ProductID { get; set; }

        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "ProductName", ResourceType = typeof(ModelFormsResources))]
        public string ProductName { get; set; }

        [DataType(DataType.DateTime, ErrorMessageResourceName = "DateTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "AdquisitionDate", ResourceType = typeof(ModelFormsResources))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime AdquisitionDate { get; set; }

        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Quantity", ResourceType = typeof(ModelFormsResources))]
        public long Quantity { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, 1E20, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "BuyerCost", ResourceType = typeof(ModelFormsResources))]
        public decimal BuyerCost { get; set; }

        [Display(Name = "UnitCost", ResourceType = typeof(ModelFormsResources))]
        public decimal UnitCost { get; set; }

        [Display(Name = "DistributionCenterID", ResourceType = typeof(ModelFormsResources))]
        public long? DistributionCenterID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [GraterOrEqualThanInteger(0, ErrorMessageResourceName = "GraterOrEqualThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "StatusID", ResourceType = typeof(ModelFormsResources))]
        public long? StatusID { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }

        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "StatusDescription", ResourceType = typeof(ModelFormsResources))]
        public string StatusDescription { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [GraterOrEqualThanInteger(0, ErrorMessageResourceName = "GraterOrEqualThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "PackID", ResourceType = typeof(ModelFormsResources))]
        public long? PackID { get; set; }

        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "PackSerialCode", ResourceType = typeof(ModelFormsResources))]
        public string PackSerialCode { get; set; }

        [Range(0, 1E26, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "Depth", ResourceType = typeof(ModelFormsResources))]
        public decimal Depth { get; set; }

        [Range(0, 1E26, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "Width", ResourceType = typeof(ModelFormsResources))]
        public decimal Width { get; set; }

        [Range(0, 1E26, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "Height", ResourceType = typeof(ModelFormsResources))]
        public decimal Height { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "Capacity", ResourceType = typeof(ModelFormsResources))]
        public decimal Size { get; set; }

        [Display(Name = "error")]
        public string error { get; set; }
    }
}