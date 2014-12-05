using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Penates.Utils.Attributes {
    public class ConfigRegularExpressionAttribute : RegularExpressionAttribute {
        public ConfigRegularExpressionAttribute(string patternConfigKey)
            : base((string)Properties.Settings.Default[patternConfigKey]) { 
        }
    }
}