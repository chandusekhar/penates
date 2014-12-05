using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.JSON.TableObjects {
    public class ProductTableJson {

        public long ProductID { get; set; }

        public string Barcode { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }
    }
}