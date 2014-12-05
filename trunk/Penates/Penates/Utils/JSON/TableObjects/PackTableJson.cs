using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.JSON.TableObjects {
    public class PackTableJson {

        public long PackID { get; set; }

        public string SerialNumber { get; set; }

        public string Description { get; set; }

        public string ExpirationDate { get; set; }
    }
}