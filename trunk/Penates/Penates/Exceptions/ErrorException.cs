using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Database
{
    public class ErrorException : MyException
    {

        public ErrorException():base() {
        }

        public ErrorException(string s):base(s) {
        }

        public ErrorException(string title, string message)
            : base(title, message) {
        }

        public ErrorException(string title, string message, Exception e)
            : base(title, message, e) {
        }

        public ErrorException(string message, Exception e)
            : base(message,e) {
        }
    }
}
