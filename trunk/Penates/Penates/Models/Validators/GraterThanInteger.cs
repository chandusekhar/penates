using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.Validators {
    public class GraterOrEqualThanInteger : ValidationAttribute{
        private long minimum;

        public GraterOrEqualThanInteger(long min):base(){
            this.minimum = min;
        }

        public GraterOrEqualThanInteger(long min, string error)
            : base(error) {
                this.minimum = min;
        }

        public GraterOrEqualThanInteger(long min, Func<string> errorAccesor)
            : base(errorAccesor) {
                this.minimum = min;
        }

        public override bool IsValid(object value)
        {
            try { 
                if(value == null){
                    return true;
                }
                if (value is long) {
                    long num = (long) value;
                    if (num >= this.minimum) {
                        return true;
                    }
                    return false;
                } else {
                    int num = (int) value;
                    if (num >= this.minimum) {
                        return true;
                    }
                    return false;
                }
            } catch (Exception) {
                return false;
            }
        }

        public override string FormatErrorMessage(string name) {
            return string.Format(ErrorMessageString, new object[] { name, this.minimum});
        }
    }
}