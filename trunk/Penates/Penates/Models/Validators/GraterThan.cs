using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Models.Validators {
    public class GraterThan : ValidationAttribute{
        private decimal minimum;

        public GraterThan(double min):base(){
            this.minimum = (decimal)min;
        }

        public GraterThan(double min, string error)
            : base(error) {
                this.minimum = (decimal) min;
        }

        public GraterThan(double min, Func<string> errorAccesor)
            : base(errorAccesor) {
                this.minimum = (decimal) min;
        }

        public override bool IsValid(object value)
        {
            try { 
                decimal num = (decimal)value;
                if (num > this.minimum) {
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