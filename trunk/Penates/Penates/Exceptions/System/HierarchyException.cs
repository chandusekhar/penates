using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.System
{
    public class HierarchyException : MyException
    {

        public HierarchyException():base() {
        }

        public HierarchyException(string s):base(s) {
        }

        public HierarchyException(string title, string message)
            : base(title, message) {
        }

        public HierarchyException(string title, string message, Exception e)
            : base(title, message, e) {
        }

        public HierarchyException(string message, Exception e)
            : base(message,e) {
        }
    }
}
