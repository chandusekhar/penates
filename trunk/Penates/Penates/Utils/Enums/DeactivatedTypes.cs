using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.Enums {
    public enum DeactivatedTypes {

        ALL = -1,
        ACTIVE = 0,
        DEACTIVATED = 1
    }

    public static class DeactivatedMethods {

        public static int getTypeNumber(this DeactivatedTypes type) {
            switch (type) {
                case DeactivatedTypes.ACTIVE:
                    return 0;
                case DeactivatedTypes.DEACTIVATED:
                    return 1;
                default:
                    return -1;
            }
        }
    }
}