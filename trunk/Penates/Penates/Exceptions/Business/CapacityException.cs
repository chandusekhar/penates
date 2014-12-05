using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Business
{
    public class CapacityException : BusinessException
    {

        public CapacityException():base() {
        }

        public CapacityException(string s):base(s) {
        }

        public CapacityException(string title, string message)
            : base(title, message) {
        }

        public CapacityException(string title, string message, Exception e)
            : base(title, message, e) {
        }

        public CapacityException(string message, Exception e)
            : base(message,e) {
        }
    }
}
