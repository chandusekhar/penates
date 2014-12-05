using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.Objects {
    public class PageRankItem<T> {

        public T table{get;set;}

        public int rankPoints { get; set; }
    }
}