using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils {
    public static class SizeUtils {

        public static decimal fromCm3ToM3(decimal size) {
            return size / 1000000;
        }

        public static decimal fromM3ToCm3(decimal size) {
            return size * 1000000;
        }

        public static decimal fromCm3ToM3(decimal? size) {
            if (!size.HasValue) {
                return 0;
            }
            return SizeUtils.fromCm3ToM3(size.Value);
        }

        public static decimal fromM3ToCm3(decimal? size) {
            if (!size.HasValue) {
                return 0;
            }
            return SizeUtils.fromM3ToCm3(size.Value);
        }

        public static decimal fromCmToM(decimal size) {
            return size / 100;
        }

        public static decimal fromMToCm(decimal size) {
            return size * 100;
        }
    }
}