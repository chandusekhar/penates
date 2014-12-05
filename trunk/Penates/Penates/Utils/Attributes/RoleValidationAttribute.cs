namespace Penates.Services
{
    using Penates.Repositories.Users;
    using Penates.Models.ViewModels.Users;
    using System.Web.Mvc;
    using System.Web;
    using System;
    using Penates.Interfaces.Repositories;
    using Penates.Database;
    using System.Configuration;
    using System.Collections.Generic;
    using Penates.Controllers;
    using System.Web.UI.WebControls;
    using Penates.Exceptions.Security;
    using Penates.Utils.Enums;

    public class RoleValidationAttribute : AuthorizeAttribute
    {
        public new List<RoleType> Roles { get; private set; }

        public RoleValidationAttribute(params RoleType[] roles) {
            this.Roles = new List<RoleType>(roles);
        }

        public string HandleErrorPath { get; set; }
        public IUserRepository userRepository = new UserRepository();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var userName = httpContext.User.Identity.Name;
            if (String.Equals(userName, ConfigurationManager.AppSettings["SuperUserName"].ToString(), StringComparison.OrdinalIgnoreCase)) {
                return true;
            }
            var user = userRepository.GetUserByUserName(userName);
            if (user == null) {
                throw new UnsignedUserException();
            }

            var userRoles = userRepository.GetRolesForUser(user.FileNumber);
            if (this.Roles == null) {
                return true;
            }
            //if (userRoles.Count > 0){} // ya que si no tiene solo accede a los de all
                foreach (var role in this.Roles)
                {
                    if((int)role == -1 || String.Equals(RoleType.All.GetStringValue(), role.GetStringValue(), StringComparison.OrdinalIgnoreCase)){
                        return true;
                    }
                    foreach (var userRole in userRoles)
                    {
                        if ((int)role == userRole.RoleID || String.Equals(userRole.Description, role.GetStringValue(), StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            //}

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            ErrorController errorController = new ErrorController();
            filterContext.Result = errorController.PermissionDenied();
        }

        protected void HandleUnsignedUser(AuthorizationContext filterContext) {
            ErrorController errorController = new ErrorController();
            filterContext.Result = errorController.UnsignedUser();
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try {
                if (this.AuthorizeCore(filterContext.HttpContext)) {
                    base.OnAuthorization(filterContext);
                } else {
                    this.HandleUnauthorizedRequest(filterContext);
                }
            } catch (UnsignedUserException) {
                this.HandleUnsignedUser(filterContext);
            }
        }
    }
}