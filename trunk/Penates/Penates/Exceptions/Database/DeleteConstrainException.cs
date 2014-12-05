using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Database
{
    public class DeleteConstrainException : DatabaseException
    {

        public DeleteConstrainException():base() {
        }

        public DeleteConstrainException(string s):base(s) {
        }

        public DeleteConstrainException(string title, string message)
            : base(title, message) {
        }

        public DeleteConstrainException(string title, string message, Exception e)
            : base(title, message,e) {
        }

        public DeleteConstrainException(string message, Exception e)
            : base(message,e) {
        }
    }
}
