using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Utils.Attributes {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class NotEqualToIgnoreCaseAttribute : ValidationAttribute {
        private const string DefaultErrorMessage = "{0} cannot be the same as {1}.";
        private object typeId = new object();

        public string OtherProperty { get; private set; }

        public NotEqualToIgnoreCaseAttribute(string otherProperty)
            : base(DefaultErrorMessage) {
            if (string.IsNullOrEmpty(otherProperty)) {
                throw new ArgumentNullException("otherProperty");
            }

            OtherProperty = otherProperty;
        }

        public override string FormatErrorMessage(string name) {
            return string.Format(ErrorMessageString, name, OtherProperty);
        }

        protected override ValidationResult IsValid(object value,
                              ValidationContext validationContext) {
            if (value != null) {
                var otherProperty = validationContext.ObjectInstance.GetType()
                                   .GetProperty(OtherProperty);

                var otherPropertyValue = otherProperty
                              .GetValue(validationContext.ObjectInstance, null);

                if(value is string && otherPropertyValue is string){
                    string s = (string) value;
                    string t = (string) otherPropertyValue;
                    if (String.Compare(s, t, true) == 0) {
                        return new ValidationResult(
                          FormatErrorMessage(validationContext.DisplayName));
                    }
                }else{
                    if (value.Equals(otherPropertyValue)) {
                        return new ValidationResult(
                            FormatErrorMessage(validationContext.DisplayName));
                    }
                }
            }

            return ValidationResult.Success;
        }

        public override object TypeId {
            get {
                return this.typeId;
            }
        }
    }
}