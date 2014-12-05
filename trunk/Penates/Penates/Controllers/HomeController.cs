using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Services;
using System.Web.Services;
using Penates.Models.ViewModels.Users;
using System.Web.Helpers;
using Newtonsoft.Json;
using Penates.Utils;
using Penates.Interfaces.Repositories;
using System.Web.Security;
using Penates.Models;
using Penates.Services.Users;
using Penates.Models.ViewModels.Home;
using Penates.Services;
using Penates.Utils.Enums;
using Penates.Services.Notifications;
using Penates.Database;
using Penates.Utils.JSON.TableObjects;
using Penates.Exceptions.Database;
using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using Penates.Repositories.Notifications;
using Penates.Interfaces.Services;
using Penates.Interfaces.Models;
using Penates.Utils.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Penates.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserService _userService = new UserService();
        private IFormsAuthenticationService _formsAuthenticationService;
        private ICookieUser _cookie = new CookieUser();

        public HomeController() {
        }

        public HomeController(IUserService userService) {
            _userService = userService;
        }

        public HomeController(IUserService userService, IFormsAuthenticationService formsAuthenticationService, ICookieUser cookie)
        {
            _userService = userService;
            _formsAuthenticationService = formsAuthenticationService;
            _cookie = cookie;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            var model = new Login();

            model.ErrorMessage = null;
            var userName = HttpContext.User.Identity.Name;
            
            if (_userService.validateUser(userName)) {
                _userService.updateLastLoginDate(userName);
                return this.HomeDashboards(null);
            }

            ViewBag.Message = "Home";
            return View("~/Views/Home/Index.cshtml", model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Página de descripción de la aplicación.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Página de contacto";
            return View();
        }

        [RoleValidation(RoleType.All)]
        public ActionResult HomeDashboards(DashBoardViewModel model)
        {
            if (model == null) {
                model = new DashBoardViewModel();
            }
            model.UserName = HttpContext.User.Identity.Name;
            return View("~/Views/Home/HomeDashboards.cshtml", model);
        }

        [HttpPost]
        //[MultipleButton(Name = "action", Argument = "Login")]
        public ActionResult Index(Login model)
        {
            var attempts = Session["Attempts"];
            if (attempts != null) {
                try {
                    ulong att = (ulong) attempts;
                    model.Attempts = att;
                } catch (InvalidCastException) {

                }
            }
            var captchaAttempts = Session["CaptchaAttempts"];
            if (captchaAttempts != null) {
                try {
                    ulong cAttempts = (ulong) captchaAttempts;
                    model.CaptchaAttempts = cAttempts;
                } catch (InvalidCastException) {

                }
            }
            var temp = TempData["Username"];
            if (temp != null) {
                try {
                    string u = (string) temp;
                    if (String.Compare(u, model.Username, true) != 0) {
                        model.Attempts = 0;
                    }
                } catch (InvalidCastException) {

                }
            }
            TempData["Username"] = model.Username;
            if (model.CaptchaAttempts >= 3 && !model.Username.Equals(ConfigurationManager.AppSettings["SuperUserName"].ToString(), StringComparison.OrdinalIgnoreCase))
            {
                if (model.Attempts >= 10)
                {
                    var user = _userService.GetUserByUserName(model.Username);

                    if (user != null)
                    {
                        INotificationRepository notificationRepo = new NotificationRepository();

                        _userService.InactiveUser(user.FileNumber);
                        notificationRepo.generateUserDeactivationNotifications(user.FileNumber, user.Username);
                    }
                }

                if (model.CaptchaText == HttpContext.Session["captchastring"].ToString())
                {
                    model.ErrorMessage = Resources.Errors.CaptchaSuccessfull;
                }
                else
                {
                    model.ErrorMessage = Resources.Errors.CaptchaFailed;
                    return View(model);

                }
            }

            if (ModelState.IsValid)
            {
                var result = _userService.Login(model.Username, model.Password);

                if (result)
                {
                    _userService.updateLastLoginDate(model.Username);
                    var json = JsonConvert.SerializeObject(result);

                    if (_formsAuthenticationService != null)
                        _formsAuthenticationService.SetAuthCookie(model.Username, true);
                    else
                        FormsAuthentication.SetAuthCookie(model.Username, true);

                    _cookie.ActualizarValor("userName", model.Username);
                    Session.Remove("CaptchaAttempts");
                    Session.Remove("Attempts");
                    try {
                        ValidateParamPasswordAttribute att = new ValidateParamPasswordAttribute();
                        ValidationContext context = new ValidationContext(model.Password);
                        context.DisplayName = Penates.App_GlobalResources.Forms.ModelFormsResources.Password;
                        var res = att.validate(model.Password, context);
                        if (res == ValidationResult.Success) {
                            return RedirectToAction("HomeDashboards");
                        } else {
                            string warning = Resources.Messages.PasswordsPolicyChange;
                            if (!String.IsNullOrWhiteSpace(res.ErrorMessage)) {
                                warning = warning + "\n" + res.ErrorMessage;
                            }
                            return RedirectToAction("HomeDashboards", new { Warning = warning });
                        }
                    } catch (Exception) {
                        return RedirectToAction("HomeDashboards");
                    }
                }

                model.ErrorMessage = Resources.Errors.InvalidLogin;
                model.Attempts = model.Attempts + 1;
                model.CaptchaAttempts = model.CaptchaAttempts + 1;
                Session["Attempts"] = model.Attempts;
                Session["CaptchaAttempts"] = model.CaptchaAttempts;
                return View(model);
            }
            
            return View();
        }


        [AllowAnonymous]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            if (Request.Cookies["datosUsuario"] != null)
            {
                Response.Cookies["datosUsuario"].Expires = DateTime.Now.AddDays(-1);
            }

            var user = User.Identity.Name;
            var cacheKey = string.Format("UserRoles_{0}", user);
            HttpRuntime.Cache.Remove(cacheKey);

            return RedirectToAction("Index", "Home", null);
        }

        
        public ActionResult GetNotifications(jQueryDataTableParamModel param, string UserName)
        {
            try
            {
                NotificationsService service = new NotificationsService();           
                IQueryable<Notification> query = service.getData(UserName);               
                List<NotificationTableJson> result = service.toJsonArray(query);
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = result
                },
                            JsonRequestBehavior.AllowGet);
            }
            catch (DatabaseException ex)
            {
                ErrorModel errorModel = new ErrorModel(ex.title, ex.Message, ex, "FormsController", "SubmitProduct");
                return View("~/Views/Error/ErrorDisplay.cshtml", errorModel);
            }           
        }
    
        public CaptchaImageResult ShowCaptchaImage()
        {
            return new CaptchaImageResult();
        }
        
    }
}
