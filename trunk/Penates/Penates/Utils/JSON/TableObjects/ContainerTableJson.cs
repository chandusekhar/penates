using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.JSON.TableObjects {
    public class ContainerTableJson {

        public long ContainerID { get; set; }

        public string ContainerTypeName { get; set; }

        public string TemporaryDeposit { get; set; }

        public string Divition { get; set; }

        public decimal UsedPercentage { get; set; }
    }
}