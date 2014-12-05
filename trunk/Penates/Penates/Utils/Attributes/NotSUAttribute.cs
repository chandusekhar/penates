using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Penates.Utils.Attributes {
    public class NotSUAttribute : ValidationAttribute {
        
        private string su;

        public NotSUAttribute():base(){
            try {
                this.su = ConfigurationManager.AppSettings["SuperUserName"].ToString();
            }catch(Exception){
                this.su = "SU";
            }
        }

        public NotSUAttribute(string error)
            : base(error) {
                try {
                    this.su = ConfigurationManager.AppSettings["SuperUserName"].ToString();
                } catch (Exception) {
                    this.su = "SU";
                }
        }

        public NotSUAttribute(Func<string> errorAccesor)
            : base(errorAccesor) {
                try {
                    this.su = ConfigurationManager.AppSettings["SuperUserName"].ToString();
                } catch (Exception) {
                    this.su = "SU";
                }
        }

        public override bool IsValid(object value)
        {
            try { 
                string val = (string)value;
                if (String.Equals(this.su, val, StringComparison.OrdinalIgnoreCase)) {
                    return false;
                }
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public override string FormatErrorMessage(string name) {
            return string.Format(ErrorMessageString, new object[] { name, this.su});
        }
    }
}