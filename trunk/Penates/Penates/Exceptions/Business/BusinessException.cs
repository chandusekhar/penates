using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Business
{
    public class BusinessException : MyException
    {

        public BusinessException():base() {
        }

        public BusinessException(string s):base(s) {
        }

        public BusinessException(string title, string message)
            : base(title, message) {
        }

        public BusinessException(string title, string message, Exception e)
            : base(title, message, e) {
        }

        public BusinessException(string message, Exception e)
            : base(message,e) {
        }
    }
}
