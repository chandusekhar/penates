using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Penates.Utils {
    public static class ReflectionExtension {

        public static string getVarName<T>(Expression<Func<T>> expr) {
            var body = ((MemberExpression) expr.Body);
            return body.Member.Name;
        }
    }
}