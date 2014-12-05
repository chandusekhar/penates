using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.JSON.TableObjects {
    public class ValSecurityTableJson {

        public string FileNumber { get; set; }

        public string Username { get; set; }

        public decimal? MaxProductPrice { get; set; }

        public decimal? MinProductPrice { get; set; }

        public decimal? MaxOrderTotal { get; set; }

        public decimal? MinOrderTotal { get; set; }
    }
}