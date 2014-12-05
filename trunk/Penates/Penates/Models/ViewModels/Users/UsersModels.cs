using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;
using DataAnnotationsExtensions;
using Penates.App_GlobalResources.Forms;
using Penates.Database;
using Penates.Utils;
using System.Drawing;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Drawing.Imaging;
using Penates.Utils.Attributes;

namespace Penates.Models.ViewModels.Users {
    public class Login 
    {
        [Required(ErrorMessage = "User Name is required")]
        [RegularExpression("([a-zA-Z0-9._]+)", ErrorMessageResourceName = "InvalidCharacters", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public string CaptchaText { get; set; }

        public ulong Attempts { get; set; }

        public ulong CaptchaAttempts { get; set; }
    }

    public class Roles
    {
        [Display(Name = "RoleID", ResourceType = typeof(ModelFormsResources))]
        public long RoleId { get; set; }

        [Display(Name = "Description", ResourceType = typeof(ModelFormsResources))]
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        public string RoleDesciption { get; set; }
        public SelectList RolesList { get; set; } 
    }
    
    public class UserRole
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }

        public System.Web.Mvc.SelectList UserList { get; set; }
        public System.Web.Mvc.SelectList RoleList { get; set; }
    }

    public class CaptchaImageResult : ActionResult
    {
        public string GetCaptchaString(int length)
        {
            int intZero = '0';
            int intNine = '9';
            int intA = 'A';
            int intZ = 'Z';
            int intCount = 0;
            int intRandomNumber = 0;
            string strCaptchaString = "";

            Random random = new Random(System.DateTime.Now.Millisecond);

            while (intCount < length)
            {
                intRandomNumber = random.Next(intZero, intZ);
                if (((intRandomNumber >= intZero) && (intRandomNumber <= intNine) || (intRandomNumber >= intA) && (intRandomNumber <= intZ)))
                {
                    strCaptchaString = strCaptchaString + (char)intRandomNumber;
                    intCount = intCount + 1;
                }
            }
            return strCaptchaString;
        }


        public override void ExecuteResult(ControllerContext context)
        {
            Bitmap bmp = new Bitmap(100, 30);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Navy);
            string randomString = GetCaptchaString(6);
            context.HttpContext.Session["captchastring"] = randomString;
            g.DrawString(randomString, new Font("Courier", 16), new SolidBrush(Color.WhiteSmoke), 2, 2);
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "image/jpeg";
            bmp.Save(response.OutputStream, ImageFormat.Jpeg);
            bmp.Dispose();
        }

    }

    public class UserViewModel {
        [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [StringLength(256, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "FileNumber", ResourceType = typeof(ModelFormsResources))]
        public string FileNumber { get; set; }

        [UsernameMaxLenght(ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [UsernameMinLenghtAttribute(ErrorMessageResourceName = "MinLengthError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [ValidateParamUserAttribute()]
        [RegularExpression("([a-zA-Z0-9._]+)", ErrorMessageResourceName = "InvalidCharacters", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Username", ResourceType = typeof(ModelFormsResources))]
        public string UserName { get; set; }

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
        [StringLength(64, ErrorMessageResourceName = "LenghtError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "Telephone", ResourceType = typeof(ModelFormsResources))]
        public string Telephone { get; set; }

        [DataType(DataType.DateTime, ErrorMessageResourceName = "DateTypeError", ErrorMessageResourceType = typeof(Resources.FormsErrors))]
        [Display(Name = "LastLoginDate", ResourceType = typeof(ModelFormsResources))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime LastLoginDate { get; set; }

        public bool Active { get; set; }
    }
}