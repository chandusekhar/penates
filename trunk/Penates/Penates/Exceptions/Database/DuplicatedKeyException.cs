using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Database
{
    public class DuplicatedKeyException : DatabaseException
    {
        public List<string> Attributes { get; set; }
        public DuplicatedKeyException():base() {
            this.Attributes = new List<string>();
        }

        public DuplicatedKeyException(string s):base(s) {
            this.Attributes = new List<string>();
        }

        public DuplicatedKeyException(string title, string message)
            : base(title, message) {
                this.Attributes = new List<string>();
        }

        public DuplicatedKeyException(string title, string message, Exception e)
            : base(title, message,e) {
                this.Attributes = new List<string>();
        }

        public DuplicatedKeyException(string message, Exception e)
            : base(message,e) {
                this.Attributes = new List<string>();
        }
    }
}
