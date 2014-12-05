using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.ABMs {
    public class ConstraintViewModel {

        public ConstraintViewModel(string title) {
            this.Title = title;
        }

        public ConstraintViewModel(string title, string message) {
            this.Title = title;
            this.Message = message;
        }

        public string Title{get; set;}

        public string Message { get; set; }

        public long Count { get; set; }

        public string TableWithConstrain { get; set; }

        public List<Constraint> constraints { get; set; }
    }
}