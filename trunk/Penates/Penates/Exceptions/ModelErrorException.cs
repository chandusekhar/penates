using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions
{
    public class ModelErrorException : MyException
    {
        public string AttributeName = "";

        public ModelErrorException():base() {
        }

        public ModelErrorException(string s):base(s) {
        }

        public ModelErrorException(string title, string message)
            : base(title, message) {
        }

        public ModelErrorException(string title, string message, Exception e)
            : base(title, message, e) {
        }

        public ModelErrorException(string message, Exception e)
            : base(message,e) {
        }
    }
}
