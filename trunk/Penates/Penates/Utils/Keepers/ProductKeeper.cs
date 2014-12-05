using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.Keepers {
    public class ProductKeeper {

        private static ProductKeeper instance = new ProductKeeper(); //Instancia
        
        public byte[] productImage { get; set; } //Aca guardo la imagen del producto ya que el controller se pierde
        public long productID { get; set; }


        protected ProductKeeper() {

        }

        public static ProductKeeper getInstance(){
            return instance;
        }
    }
}