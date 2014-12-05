using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.Objects
{
    public class RequestedContainers
    {

        public RequestedContainers()
        {
            this.requestedContainersMap = new SortedDictionary<long, decimal>();
            this.Error = "";
            this.containersPerProduct = new SortedDictionary<long, string>();
        }

        public SortedDictionary<long, decimal> requestedContainersMap {set; get;}

        public string Error { set; get; }

        public SortedDictionary<long, string> containersPerProduct { get; set; }
    }
}