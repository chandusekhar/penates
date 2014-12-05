using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Penates.App_GlobalResources.Forms;
using Penates.Interfaces.Models;

namespace Penates.Models.ViewModels.Forms {
    public partial class CategoryViewModel : IFormViewModel{

        [Range(0, long.MaxValue, ErrorMessageResourceName = "ProductIDRange", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "CategoryID", ResourceType = typeof(ModelFormsResources))]
        public long ProductCategoriesID { get; set; }

        [DataType(DataType.Text, ErrorMessageResourceName = "TextTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [StringLength(128, ErrorMessageResourceName = "StringLenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Description", ResourceType = typeof(ModelFormsResources))]
        public string Description { get; set; }

        public string error { get; set; }
    }
}