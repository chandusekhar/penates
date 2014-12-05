using Penates.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Penates.Services {
    public class FormsAuth : IFormsAuthenticationService {
        public void SetAuthCookie(string userName, bool remember) {
            FormsAuthentication.SetAuthCookie(userName, remember);
        }
    }
}