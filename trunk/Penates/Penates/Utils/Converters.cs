using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Penates.Exceptions;
using System.Web.Mvc;

namespace Penates.Utils {
    public static class Converters {
        public static byte[] gifToByteArray(System.Drawing.Image imageIn) {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public static byte[] ToByteArray(HttpPostedFileBase file) {
            if (file != null) {
                try {
                    using (MemoryStream ms = new MemoryStream()) {
                        file.InputStream.CopyTo(ms);
                        return ms.GetBuffer();
                    }
                } catch (Exception e) {
                    throw new ConverterException(e.Message, e);
                }
            } else {
                throw new ConverterException(Resources.Errors.ConverterFileNull);
            }
        }
    }
}