using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.JSON.TableObjects {
    public class ClientTableJson {

        public long ClientID { get; set; }

        public string CUIT { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public string Email { get; set; }

        public bool Deactivated { get; set; }
    }
}