using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Models.ViewModels.Forms;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Penates.Models.ViewModels {
    public class TreeViewModel {

        public TreeViewModel() {
            this.treeID = "tree";
        }

        public TreeViewModel(string id) {
            this.treeID = id;
        }

        public string Message { get; set; }

        public bool? Error { get; set; }

        public Nodo<TreeItemViewModel> arbol { get; set; }

        public long filterID { get; set; }

        public string treeID { get; set; }

        public string SelectAction { get; set; }

        public string SelectController { get; set; }

        public string concatWithID(string s) {
            return s + this.treeID;
        }
    }
}