using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Views
{
    public class SortException : MyException
    {

        public SortException():base() {
        }

        public SortException(string s):base(s) {
        }

        public SortException(string title, string message)
            : base(title, message) {
        }

        public SortException(string title, string message, Exception e)
            : base(title, message, e) {
        }

        public SortException(string message, Exception e)
            : base(message,e) {
        }
    }
}
