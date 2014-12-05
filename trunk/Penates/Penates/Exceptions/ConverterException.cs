using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Exceptions {
    public class ConverterException: Exception {

        public ConverterException()
    {
    }

    public ConverterException(string message)
        : base(message)
    {
    }

    public ConverterException(string message, Exception inner)
        : base(message, inner)
    {
    }
    }
}