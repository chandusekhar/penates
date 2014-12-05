using Penates.Services.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Penates.Utils.Attributes {
    public class UsernameMinLenghtAttribute : MinLengthAttribute {
        public UsernameMinLenghtAttribute()
            : base(SecurityService.getStaticUsernameMinLenght()) {
        }
    }
}