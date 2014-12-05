using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.JSON.TableObjects {
    public class UserTableJson {

        public string UserID { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }
    }
}