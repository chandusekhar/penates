using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Database
{
    public class UniqueRestrictionException : DatabaseException
    {

        public string atributeName { get; set; }

        public UniqueRestrictionException():base() {
        }

        public UniqueRestrictionException(string s):base(s) {
        }

        public UniqueRestrictionException(string title, string message)
            : base(title, message) {
        }

        public UniqueRestrictionException(string title, string message, Exception e)
            : base(title, message,e) {
        }

        public UniqueRestrictionException(string message, Exception e)
            : base(message,e) {
        }
    }
}
