using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Models
{
    public class NotFoundModel : HandleErrorInfo
    {
        public NotFoundModel(Exception exception, string controllerName, string actionName)
            : base(exception, controllerName, actionName)
        {
        }
        public string RequestedUrl { get; set; }
        public string ReferrerUrl { get; set; }
    }

    public class ErrorModel : HandleErrorInfo {

        public ErrorModel(Exception exception, string controllerName, string actionName)
            : base(exception, controllerName, actionName) {
        }

        public ErrorModel(string message, Exception exception, string controllerName, string actionName)
            : this("", message, exception, controllerName, actionName) {
                this.ErrorMessage = message;
        }

        public ErrorModel(string title, string message, Exception exception, string controllerName, string actionName)
            : base(exception, controllerName, actionName) {
            this.ErrorMessage = message;
            this.ErrorTitle = title;
        }

        public ErrorModel(string title, string message, string controllerName, string actionName)
            : this(title, message, new Exception(), controllerName, actionName) {
            this.ErrorMessage = message;
            this.ErrorTitle = title;
        }

        public ErrorModel(string message, string controllerName, string actionName)
            : this(message, new Exception(), controllerName, actionName) {
        }
        public string ErrorMessage { get; set; }
        public string ErrorTitle { get; set; }
    }
}
