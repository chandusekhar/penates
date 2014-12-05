using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.Enums {
    public enum DistributionCenterTypes {

        ALL = -1,
        INTERNAL = 0,
        EXTERNAL = 1
    }

    public static class DistributionCenterTypesMethods{

        public static int getTypeNumber(this DistributionCenterTypes type) {
            switch (type) {
                case DistributionCenterTypes.INTERNAL:
                    return 0;
                case DistributionCenterTypes.EXTERNAL:
                    return 1;
                default:
                    return -1;
            }
        }
    }
}