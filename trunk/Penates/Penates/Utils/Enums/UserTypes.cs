using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.Enums {
    public enum UserTypes {

        ALL = -1,
        ACTIVE = 0,
        INACTIVE = 1
    }

    public static class UserTypesMethods{

        public static int getTypeNumber(this UserTypes type) {
            switch (type) {
                case UserTypes.ACTIVE:
                    return 0;
                case UserTypes.INACTIVE:
                    return 1;
                default:
                    return -1;
            }
        }
    }
}