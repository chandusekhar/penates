using Penates.Utils.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Penates.Utils.Enums {
    public enum RoleType {
        [Description("All")]
        All = -1,
        [Description("Super User")]
        SU = 0,
        [Description("Administrator")]
        Admin = 1,
        [Description("Executive")]
        Executive = 2,
        [Description("Inventory Chief")]
        InventoryChief = 3,
        [Description("Common")]
        Common = 4
    }

    public static class RoleTypeMethods {
        public static string GetStringValue(this RoleType value) {
            return value.ToDescriptionString();
        }
    }
}