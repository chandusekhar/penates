using DataAnnotationsExtensions;
using Penates.App_GlobalResources.Forms;
using Penates.Models.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.Users {
    public class SecurityParametersViewModel {

        public SecurityParametersViewModel() {
            this.HasPasswordDigits = false;
            this.HasPasswordLower = false;
            this.HasPasswordMax = false;
            this.HasPasswordMin = false;
            this.HasPasswordSymbols = false;
            this.HasPasswordUpper = false;
            this.HasUsernameDigits = false;
            this.HasUsernameLower = false;
            this.HasUsernameMax = false;
            this.HasUsernameMin = false;
            this.HasUsernameSymbols = false;
            this.HasUsernameUpper = false;
        }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [GraterOrEqualThanInteger(4, ErrorMessageResourceName = "GraterOrEqualThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "SessionTime", ResourceType = typeof(ModelFormsResources))]
        public int SessionTime { get; set; }

        [Display(Name = "HasUsernameMax", ResourceType = typeof(ModelFormsResources))]
        public bool HasUsernameMax { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(2, 64, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "UsernameMax", ResourceType = typeof(ModelFormsResources))]
        public int? UsernameMax { get; set; }

        [Display(Name = "HasUsernameMin", ResourceType = typeof(ModelFormsResources))]
        public bool HasUsernameMin { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, 64, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "UsernameMin", ResourceType = typeof(ModelFormsResources))]
        public int? UsernameMin { get; set; }

        [Display(Name = "HasUsernameLower", ResourceType = typeof(ModelFormsResources))]
        public bool HasUsernameLower { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, 64, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "UsernameLower", ResourceType = typeof(ModelFormsResources))]
        public long? UsernameLower { get; set; }

        [Display(Name = "HasUsernameUpper", ResourceType = typeof(ModelFormsResources))]
        public bool HasUsernameUpper { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, 64, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "UsernameUpper", ResourceType = typeof(ModelFormsResources))]
        public long? UsernameUpper { get; set; }

        [Display(Name = "HasUsernameDigits", ResourceType = typeof(ModelFormsResources))]
        public bool HasUsernameDigits { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, 64, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "UsernameDigits", ResourceType = typeof(ModelFormsResources))]
        public long? UsernameDigits { get; set; }

        [Display(Name = "HasUsernameSymbols", ResourceType = typeof(ModelFormsResources))]
        public bool HasUsernameSymbols { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Range(0, 64, ErrorMessageResourceName = "RangeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "UsernameSymbols", ResourceType = typeof(ModelFormsResources))]
        public long? UsernameSymbols { get; set; }

        [Display(Name = "HasPasswordMax", ResourceType = typeof(ModelFormsResources))]
        public bool HasPasswordMax { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [GraterOrEqualThanInteger(2, ErrorMessageResourceName = "GraterOrEqualThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "PasswordMax", ResourceType = typeof(ModelFormsResources))]
        public int? PasswordMax { get; set; }

        [Display(Name = "HasPasswordMin", ResourceType = typeof(ModelFormsResources))]
        public bool HasPasswordMin { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [GraterOrEqualThanInteger(0, ErrorMessageResourceName = "GraterOrEqualThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "PasswordMin", ResourceType = typeof(ModelFormsResources))]
        public int? PasswordMin { get; set; }

        [Display(Name = "HasPasswordLower", ResourceType = typeof(ModelFormsResources))]
        public bool HasPasswordLower { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [GraterOrEqualThanInteger(0, ErrorMessageResourceName = "GraterOrEqualThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "PasswordLower", ResourceType = typeof(ModelFormsResources))]
        public long? PasswordLower { get; set; }

        [Display(Name = "HasPasswordUpper", ResourceType = typeof(ModelFormsResources))]
        public bool HasPasswordUpper { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [GraterOrEqualThanInteger(0, ErrorMessageResourceName = "GraterOrEqualThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "PasswordUpper", ResourceType = typeof(ModelFormsResources))]
        public long? PasswordUpper { get; set; }

        [Display(Name = "HasPasswordDigits", ResourceType = typeof(ModelFormsResources))]
        public bool HasPasswordDigits { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [GraterOrEqualThanInteger(0, ErrorMessageResourceName = "GraterOrEqualThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "PasswordDigits", ResourceType = typeof(ModelFormsResources))]
        public long? PasswordDigits { get; set; }

        [Display(Name = "HasPasswordSymbols", ResourceType = typeof(ModelFormsResources))]
        public bool HasPasswordSymbols { get; set; }

        [Numeric(ErrorMessageResourceName = "NumericTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [GraterOrEqualThanInteger(0, ErrorMessageResourceName = "GraterOrEqualThanError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "PasswordSymbols", ResourceType = typeof(ModelFormsResources))]
        public long? PasswordSymbols { get; set; }

        [Display(Name = "Error", ResourceType = typeof(ModelFormsResources))]
        public string error { get; set; }
    }
}