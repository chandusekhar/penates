using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Database
{
    public class ForeignKeyConstraintException : DatabaseException
    {

        public string atributeName { get; set; }

        public ForeignKeyConstraintException():base() {
        }

        public ForeignKeyConstraintException(string s):base(s) {
        }

        public ForeignKeyConstraintException(string title, string message)
            : base(title, message) {
        }

        public ForeignKeyConstraintException(string title, string message, Exception e)
            : base(title, message,e) {
        }

        public ForeignKeyConstraintException(string message, Exception e)
            : base(message,e) {
        }
    }
}
