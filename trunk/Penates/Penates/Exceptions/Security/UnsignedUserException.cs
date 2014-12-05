using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Security
{
    public class UnsignedUserException : MyException
    {

        public UnsignedUserException():base() {
        }

        public UnsignedUserException(string s):base(s) {
        }

        public UnsignedUserException(string title, string message)
            : base(title, message) {
        }

        public UnsignedUserException(string title, string message, Exception e)
            : base(title, message, e) {
        }

        public UnsignedUserException(string message, Exception e)
            : base(message,e) {
        }
    }
}
