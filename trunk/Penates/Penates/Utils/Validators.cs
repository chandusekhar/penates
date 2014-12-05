using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils {
    public static class Validators {

        /// <summary> Valida si el archivo pasado es una imagen /// </summary>
        /// <param name="file"> Archivo a balidar</param>
        /// <returns>true si es una imagen. Sino false</returns>
        public static bool IsImage(HttpPostedFileBase file) {
            if (file.ContentType.Contains("image")) {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };

            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}