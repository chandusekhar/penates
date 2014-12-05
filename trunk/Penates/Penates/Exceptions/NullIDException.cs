using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions
{
    public class NullIDException : MyException
    {

        public NullIDException():base() {
        }

        public NullIDException(string s):base(s) {
        }

        public NullIDException(string title, string message)
            : base(title, message) {
        }

        public NullIDException(string title, string message, Exception e)
            : base(title, message, e) {
        }

        public NullIDException(string message, Exception e)
            : base(message,e) {
        }
    }
}
