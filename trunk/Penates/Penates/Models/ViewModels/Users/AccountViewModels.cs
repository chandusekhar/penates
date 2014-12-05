using Penates.App_GlobalResources.Forms;
using Penates.Utils.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Penates.Models.ViewModels.Users
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        [RegularExpression("([a-zA-Z0-9._]+)", ErrorMessageResourceName = "InvalidCharacters", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        public string UserName { get; set; }
    }

    public class ChangePasswordViewModel {
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [RegularExpression("([a-zA-Z0-9._]+)", ErrorMessageResourceName = "InvalidCharacters", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [UsernameMaxLenght(ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Username", ResourceType = typeof(ModelFormsResources))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [PasswordMinLenghtAttribute(ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [PasswordMaxLenghtAttribute( ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [ValidateParamPasswordAttribute()]
        [DataType(DataType.Password)]
        [NotEqualToIgnoreCase("UserName", ErrorMessageResourceName = "PasswordSameAsUser", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "NewPassword", ResourceType = typeof(ModelFormsResources))]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(ModelFormsResources))]
        [Compare("NewPassword", ErrorMessageResourceName = "PasswordsDontMatch", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        public string ConfirmPassword { get; set; }
    }

    public class ChangeMyPasswordViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [UsernameMaxLenght(ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [RegularExpression("([a-zA-Z0-9._]+)", ErrorMessageResourceName = "InvalidCharacters", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Username", ResourceType = typeof(ModelFormsResources))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(ModelFormsResources))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [PasswordMinLenghtAttribute(ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [PasswordMaxLenghtAttribute(ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [ValidateParamPasswordAttribute()]
        [DataType(DataType.Password)]
        [NotEqualToIgnoreCase("UserName", ErrorMessageResourceName = "PasswordSameAsUser", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [NotEqualToIgnoreCase("OldPassword", ErrorMessageResourceName = "PasswordSameAsOld", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "NewPassword", ResourceType = typeof(ModelFormsResources))]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(ModelFormsResources))]
        [Compare("NewPassword", ErrorMessageResourceName = "PasswordsDontMatch", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [RegularExpression("([a-zA-Z0-9._]+)", ErrorMessageResourceName = "InvalidCharacters", ErrorMessageResourceType = typeof(Resources.FormsErrors))]              
        [Display(Name = "UserName", ResourceType = typeof(Resources.FormsResources))]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.FormsResources))]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [StringLength(256, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "FileNumber", ResourceType = typeof(ModelFormsResources))]
        public string FileNumber { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [UsernameMaxLenght(ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [UsernameMinLenghtAttribute(ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [ValidateParamUserAttribute()]
        [RegularExpression("([a-zA-Z0-9._]+)", ErrorMessageResourceName = "InvalidCharacters", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [NotSU(ErrorMessageResourceName = "IvalidUsername", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Username", ResourceType = typeof(ModelFormsResources))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [PasswordMinLenghtAttribute(ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [PasswordMaxLenghtAttribute(ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [ValidateParamPasswordAttribute()]
        [DataType(DataType.Password)]
        [NotEqualToIgnoreCase("UserName", ErrorMessageResourceName = "PasswordSameAsUser", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Password", ResourceType = typeof(ModelFormsResources))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(ModelFormsResources))]
        [Compare("Password", ErrorMessageResourceName = "PasswordsDontMatch", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DataType(DataType.Text)]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "FirstName", ResourceType = typeof(ModelFormsResources))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DataType(DataType.Text)]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "LastName", ResourceType = typeof(ModelFormsResources))]
        public string LastName { get; set; }

        [DataType(DataType.Text)]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Address", ResourceType = typeof(ModelFormsResources))]
        public string Address { get; set; }

        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [StringLength(255, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = null, ErrorMessageResourceName = "EmailError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Email", ResourceType = typeof(ModelFormsResources))]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = null, ErrorMessageResourceName = "PhoneError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [StringLength(128, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Telephone", ResourceType = typeof(ModelFormsResources))]
        public string Telephone { get; set; }
    }
}
