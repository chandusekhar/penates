using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Models {
    public class AutocompleteItem {

        public Object ID { get; set; }

        public string Label { get; set; }

        public string Description { get; set; }

        public object aux { get; set; }
    }
}