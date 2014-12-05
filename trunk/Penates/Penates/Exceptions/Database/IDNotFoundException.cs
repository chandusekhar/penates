using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Database
{
    public class IDNotFoundException : DatabaseException
    {

        public string atributeName { get; set; }

        public IDNotFoundException():base() {
        }

        public IDNotFoundException(string s):base(s) {
        }

        public IDNotFoundException(string title, string message)
            : base(title, message) {
        }

        public IDNotFoundException(string title, string message, Exception e)
            : base(title, message,e) {
        }

        public IDNotFoundException(string message, Exception e)
            : base(message,e) {
        }
    }
}
