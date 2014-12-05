using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.Enums {
    public enum PredifinedMethodsTypes {

        Retail = 0,
        WeightedAverage = 1,
        Fifo = 2
    }

    public static class PredifinedMethodsTypesMethods
    {
        public static int getTypeNumber(this PredifinedMethodsTypes type)
        {
            switch (type) {
                case PredifinedMethodsTypes.Retail:
                    return 0;
                case PredifinedMethodsTypes.WeightedAverage:
                    return 1;
                default:
                    return 2;
            }
        }
    }
}