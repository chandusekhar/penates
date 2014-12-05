using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Database
{
    public class DataRestrictionProcedureException : StoredProcedureException
    {
        public DataRestrictionProcedureException():base() {
        }

        public DataRestrictionProcedureException(string s):base(s) {
        }

        public DataRestrictionProcedureException(string title, string message)
            : base(title, message) {
        }

        public DataRestrictionProcedureException(string title, string message, Exception e)
            : base(title, message,e) {
        }

        public DataRestrictionProcedureException(string message, Exception e)
            : base(message,e) {
        }
    }
}
