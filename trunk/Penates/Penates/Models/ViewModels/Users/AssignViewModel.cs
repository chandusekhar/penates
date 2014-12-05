using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Models.ViewModels.Users {
    public class AssignViewModel {

        public string UserFileNumber { get; set; }

        public string UserName { get; set; }

        public IEnumerable<SelectListItem> Selectlist { get; set; }
    }
}