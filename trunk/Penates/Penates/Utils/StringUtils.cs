using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils
{
    public class StringUtils
    {

        public static List<string> splitString(string search)
        {
            search = search.Trim();
            var searches = search.Split(' ');

            for (int i = 0; i < searches.Count(); i++)
            {
                searches[i] = searches[i].Trim();
            }
            return new List<string>(searches);
        }

        public static List<string> split(string search, char separator) {
            if (String.IsNullOrEmpty(search)) {
                return new List<string>();
            }
            search = search.Trim();
            var searches = search.Split(separator);

            for (int i = 0; i < searches.Count(); i++) {
                searches[i] = searches[i].Trim();
            }
            return new List<string>(searches);
        }
    }
}