using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Database
{
    public class DatabaseException : MyException
    {

        public DatabaseException():base() {
        }

        public DatabaseException(string s):base(s) {
        }

        public DatabaseException(string title, string message)
            : base(title, message) {
        }

        public DatabaseException(string title, string message, Exception e)
            : base(title, message, e) {
        }

        public DatabaseException(string message, Exception e)
            : base(message,e) {
        }
    }
}
