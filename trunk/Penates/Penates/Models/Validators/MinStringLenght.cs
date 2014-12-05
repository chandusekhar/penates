using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.Validators {
    public class MinStringLenght : ValidationAttribute{
        private long minimum;

        public MinStringLenght(long min):base(){
            this.minimum = min;
        }

        public MinStringLenght(long min, string error)
            : base(error) {
                this.minimum = min;
        }

        public MinStringLenght(long min, Func<string> errorAccesor)
            : base(errorAccesor) {
                this.minimum = min;
        }

        public override bool IsValid(object value)
        {
            try { 
                string s = (string)value;
                if (s.Length >= this.minimum) {
                    return true;
                }
                return false;
            } catch (Exception) {
                return false;
            }
        }

        public override string FormatErrorMessage(string name) {
            return string.Format(ErrorMessageString, new object[] { name, this.minimum});
        }
    }
}