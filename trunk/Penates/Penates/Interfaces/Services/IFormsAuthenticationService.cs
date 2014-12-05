using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Services {
    public interface IFormsAuthenticationService {
        void SetAuthCookie(string username, bool remember);
    }
}
