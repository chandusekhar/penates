using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils {
    public class Paginator<T> {

        IQueryable<T> query;

        public Paginator(IQueryable<T> query){
            this.query = query;
        }

        public IQueryable<T> page(int start, int lenght) {
            if (start < 0 || lenght < 0) {
                return this.query;
            }
            return this.query.Skip(start).Take(lenght);
        }
    }
}