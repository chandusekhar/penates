using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Database
{
    public class StoredProcedureException : DatabaseException
    {

        public string atributeName { get; set; }

        public StoredProcedureException():base() {
        }

        public StoredProcedureException(string s):base(s) {
        }

        public StoredProcedureException(string title, string message)
            : base(title, message) {
        }

        public StoredProcedureException(string title, string message, Exception e)
            : base(title, message,e) {
        }

        public StoredProcedureException(string message, Exception e)
            : base(message,e) {
        }
    }
}
