using Penates.Database;
using Penates.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Penates.Utils {
    public static class CryptoUtils {


        public static UserPassword HashPassword(string FileNumber, string userName, string password) {
            return CryptoUtils.HashPassword(new UserPassword() { FileNumber = FileNumber, UserName = userName, Password = password,});
        }

        public static UserPassword HashPassword(UserPassword user) {
            user.Salt = Crypto.GenerateSalt();
            string password = user.FileNumber + user.UserName + user.Password + user.Salt;
            user.Password = Crypto.HashPassword(password);
            return user;
        }

        public static bool VerifyHash(string FileNumber, string userName, string password, string salt, string hashedPassword) {
            return CryptoUtils.VerifyHash(new UserPassword() { FileNumber = FileNumber, UserName = userName, Password = password, Salt = salt }, hashedPassword);
        }

        public static bool VerifyHash(UserPassword user, string hashedPassword) {
            string password = user.FileNumber + user.UserName + user.Password + user.Salt;
            return Crypto.VerifyHashedPassword(hashedPassword, password);
        }
    }
}