using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions
{
    public class MyException : Exception
    {
        public string title;

        public MyException():base() {
        }

        public MyException(string s):base(s) {
        }

        public MyException(string s, Exception e)
            : base(s, e) {
        }

        public MyException(string title, string message)
            : base(message) {
                this.title = title;
        }

        public MyException(string title, string message, Exception e)
            : base(message, e) {
            this.title = title;
        }

        public override string ToString() {
            if (title == null) {
                return base.Message;
            } else {
                return this.title + ": " + base.Message;
            }
        }
    }
}
