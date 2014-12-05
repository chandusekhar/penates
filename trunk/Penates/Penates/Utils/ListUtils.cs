using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils {
    public static class ListUtils<T>{

        public static List<T> interlay(List<T> firstList, List<T> secondList) {
            List<T> aux = new List<T>();
            int i = 0;
            while (i < firstList.Count) {
                aux.Add(firstList[i]);
                if (i < secondList.Count) {
                    aux.Add(secondList[i]);
                }
                i++;
            }
            if (i < secondList.Count) {
                for (int j = i; i < secondList.Count; i++) {
                    aux.Add(secondList[j]);
                }
            }
            return aux;
        }
    }
}