using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Exceptions.Database
{
    public class ItemDeletedException : DatabaseException
    {

        public ItemDeletedException():base() {
        }

        public ItemDeletedException(string s):base(s) {
        }

        public ItemDeletedException(string title, string message)
            : base(title, message) {
        }

        public ItemDeletedException(string title, string message, Exception e)
            : base(title, message,e) {
        }

        public ItemDeletedException(string message, Exception e)
            : base(message,e) {
        }
    }
}
