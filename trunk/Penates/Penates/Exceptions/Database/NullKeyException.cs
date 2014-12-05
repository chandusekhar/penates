using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Database
{
    public class NullKeyException : DatabaseException
    {

        public string atributeName { get; set; }

        public NullKeyException():base() {
        }

        public NullKeyException(string s):base(s) {
        }

        public NullKeyException(string title, string message)
            : base(title, message) {
        }

        public NullKeyException(string title, string message, Exception e)
            : base(title, message,e) {
        }

        public NullKeyException(string message, Exception e)
            : base(message,e) {
        }
    }
}
