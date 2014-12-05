using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Database
{
    public class GeneralException : MyException
    {

        public GeneralException():base() {
        }

        public GeneralException(string s):base(s) {
        }

        public GeneralException(string title, string message)
            : base(title, message) {
        }

        public GeneralException(string title, string message, Exception e)
            : base(title, message, e) {
        }

        public GeneralException(string message, Exception e)
            : base(message,e) {
        }
    }
}
