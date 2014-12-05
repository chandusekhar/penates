using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Database
{
    public class SaveException : DatabaseException
    {

        public string atributeName { get; set; }

        public SaveException():base() {
        }

        public SaveException(string s):base(s) {
        }

        public SaveException(string title, string message)
            : base(title, message) {
        }

        public SaveException(string title, string message, Exception e)
            : base(title, message,e) {
        }

        public SaveException(string message, Exception e)
            : base(message,e) {
        }
    }
}
