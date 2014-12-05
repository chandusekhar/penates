using Penates.App_GlobalResources.Forms;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.ViewModels.Forms {
    public class CategoryHierarchyViewModel {

        public CategoryHierarchyViewModel() {
            this.Error = false;
        }

        public Nodo<TreeItemViewModel> padres { get; set; }

        public Nodo<TreeItemViewModel> hijos { get; set; }

        public CategoryViewModel actual { get; set; }

        public string Message { get; set; }

        public string Title { get; set; }

        public bool Error { get; set; }
    }
}