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
    public partial class ProductViewModel : IFormViewModel {
            public ProductViewModel() {
                String path = HttpContext.Current.Server.MapPath("~/Images/ABMs/NoImage.gif");
                var img = System.Drawing.Image.FromFile(path, true);
                this.ProdImage = Converters.gifToByteArray(img);
                this.validImage = true;
                this.SellPrice = 0;
                this.Width = 10;
                this.Height = 10;
                this.Depth = 10;
                this.Size = 1000;
                this.HasMinStock = false;

            }

            [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Required(ErrorMessageResourceName = "SellerPriceRequired", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Range(0, 1E20, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [DataType(DataType.Currency, ErrorMessageResourceName = "SellerPriceTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Display(Name = "SellerPrice", ResourceType = typeof(ModelFormsResources))]
            public decimal SellPrice { get; set; }

            [Display(Name = "ID", ResourceType = typeof(ModelFormsResources))]
            public long ProductID { get; set; }

            [Required(ErrorMessageResourceName = "BarcodeRequired", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [DataType(DataType.Text)]
            [StringLength(128, ErrorMessageResourceName = "BarcodeLength", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Display(Name = "Barcode", ResourceType = typeof(ModelFormsResources))]
            public string Barcode { get; set; }

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

            [DataType(DataType.Text, ErrorMessageResourceName = "DescriptionType", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [MaxLength(1024, ErrorMessageResourceName = "DescriptionLength", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Display(Name = "Description", ResourceType = typeof(ModelFormsResources))]
            public string Description { get; set; }

            [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [DataType(DataType.Text)]
            [StringLength(128, ErrorMessageResourceName = "NameLength", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Display(Name = "Name", ResourceType = typeof(ModelFormsResources))]
            public string Name { get; set; }

            [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [DisplayFormat(DataFormatString = "{0:F2}")]
            [Display(Name = "Size", ResourceType = typeof(ModelFormsResources))]
            public decimal? Size { get; set; }

            [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Display(Name = "Category", ResourceType = typeof(ModelFormsResources))]
            public long Category { get; set; }
            //public System.Web.Mvc.SelectList CategoriesList { get; set; }
            public IEnumerable<SelectListItem> CategoriesList { get; set; }

            [DataAnnotationsExtensions.FileExtensions("png,jpg,jpeg,gif",ErrorMessageResourceName = "ExtensionError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [ValidateFile]
            [Display(Name = "PhotoPath", ResourceType = typeof(ModelFormsResources))]
            public HttpPostedFileBase Image { get; set; }

            [Range(0, long.MaxValue, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
            [Display(Name = "MinStock", ResourceType = typeof(ModelFormsResources))]
            public long? MinStock { get; set; }

            [Display(Name = "HasMinStock", ResourceType = typeof(ModelFormsResources))]
            public bool HasMinStock { get; set; }
            
            [Display(Name = "IsBasic", ResourceType = typeof(ModelFormsResources))]
            public bool IsBasic{ get; set; }

            public byte[] ProdImage { get; set; }

            public bool validImage { get; set; }

            public string error { get; set; }

            public bool firstLoad { get; set; }

            public string CategoryName { get; set; }

            /// <summary> Validador para la Imagen, le saque la validacion de los atributos xq lo hago con file Extensions/// </summary>
           public class ValidateFileAttribute : ValidationAttribute {
                public override bool IsValid(object value) {
                    int MaxContentLength = 1024 * 1024 * 4;

                    var file = value as HttpPostedFileBase;

                    if (file == null)
                        return true;
                    else if (file.ContentLength > MaxContentLength) {
                        ErrorMessage = Resources.FormsErrors.ImageSizeError;
                        return false;
                    } else
                        return true;
                }
            }
        }
}