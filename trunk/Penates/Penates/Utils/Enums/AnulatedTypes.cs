using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.Enums {
    public enum AnulatedTypes {

        ALL = -1,
        VALID = 0,
        ANULATED = 1
    }

    public static class AnulatedTypesMethods {

        public static int getTypeNumber(this AnulatedTypes type) {
            switch (type) {
                case AnulatedTypes.VALID:
                    return 0;
                case AnulatedTypes.ANULATED:
                    return 1;
                default:
                    return -1;
            }
        }
    }
}