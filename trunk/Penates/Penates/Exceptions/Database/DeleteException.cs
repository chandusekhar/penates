using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Database
{
    public class DeleteException : DatabaseException
    {

        public string atributeName { get; set; }

        public DeleteException():base() {
        }

        public DeleteException(string s):base(s) {
        }

        public DeleteException(string title, string message)
            : base(title, message) {
        }

        public DeleteException(string title, string message, Exception e)
            : base(title, message,e) {
        }

        public DeleteException(string message, Exception e)
            : base(message,e) {
        }
    }
}
