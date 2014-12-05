using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Tests.Utils {
    class RandomStringGenerator {

        public static string RandomString(int size) {
            //StringBuilder builder = new StringBuilder();
            //Random random = new Random((int) DateTime.Now.Ticks);
            //char ch;
            //for (int i = 0; i < size; i++) {
            //    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
            //    builder.Append(ch);
            //}
            //return builder.ToString();
            return RandomStringGenerator.generateString("ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789", size);
        }

        public static string RandomNumericString(int size) {
            return generateString("0123456789", size);
        }

        public static string RandomLetersString(int size) {
            return generateString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", size);
        }

        private static string generateString(string alphabet, int size){
            var chars = alphabet;
            var random = new Random((int) DateTime.Now.Ticks);
            var result = new string(
                Enumerable.Repeat(chars, size)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}
