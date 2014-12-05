using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Business
{
    public class InsufficientSpaceException : BusinessException
    {

        public InsufficientSpaceException():base() {
        }

        public InsufficientSpaceException(string s):base(s) {
        }

        public InsufficientSpaceException(string title, string message)
            : base(title, message) {
        }

        public InsufficientSpaceException(string title, string message, Exception e)
            : base(title, message, e) {
        }

        public InsufficientSpaceException(string message, Exception e)
            : base(message,e) {
        }
    }
}
