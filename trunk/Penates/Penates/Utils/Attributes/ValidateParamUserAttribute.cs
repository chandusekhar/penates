using Penates.Interfaces.Services;
using Penates.Services.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Penates.Utils.Attributes {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateParamUserAttribute : ValidationAttribute {
        private int maxLenght;
        private int minLenght;
        private long numberOfUpper;
        private long numberOfLower;
        private long numberOfDigits;
        private long numberOfSymbols;

        public ValidateParamUserAttribute()
            : base(Resources.FormsErrors.PasswordErrorMessage) {
                base.ErrorMessage = Resources.FormsErrors.PasswordErrorMessage;
                ISecurityService service = new SecurityService();
                int? auxInt = service.getUsernameMaxLenght();
                if (auxInt.HasValue && auxInt.Value > 0) {
                    this.maxLenght = auxInt.Value;
                } else {
                    this.maxLenght = -1;
                }
                auxInt = service.getUsernameMinLenght();
                if (auxInt.HasValue && auxInt.Value >= 0) {
                    this.minLenght = auxInt.Value;
                } else {
                    this.minLenght = 0;
                }
                long? auxLong = service.getUserNumberOfUpperCase();
                if (auxLong.HasValue && auxLong.Value >= 0) {
                    this.numberOfUpper = auxLong.Value;
                } else {
                    this.numberOfUpper = 0;
                }
                auxLong = service.getUserNumberOfLowerCase();
                if (auxLong.HasValue && auxLong.Value >= 0) {
                    this.numberOfLower = auxLong.Value;
                } else {
                    this.numberOfLower = 0;
                }
                auxLong = service.getUserNumberOfDigits();
                if (auxLong.HasValue && auxLong.Value >= 0) {
                    this.numberOfDigits = auxLong.Value;
                } else {
                    this.numberOfDigits = 0;
                }
                auxLong = service.getUserNumberOfSymbols();
                if (auxLong.HasValue && auxLong.Value >= 0) {
                    this.numberOfSymbols = auxLong.Value;
                } else {
                    this.numberOfSymbols = 0;
                }
        }

        public override string FormatErrorMessage(string name) {
            return string.Format(ErrorMessageString, name, this.minLenght, this.maxLenght, this.numberOfLower, this.numberOfUpper,
                this.numberOfDigits, this.numberOfSymbols);
        }

        protected override ValidationResult IsValid(object value,
                              ValidationContext validationContext) {
            if (value == null) {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            if (value is string) {
                string pass = (string) value;
                if(pass.Length < this.minLenght){
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                if(this.maxLenght > 0 && pass.Length > this.maxLenght){
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                long count = 0;
                if(this.numberOfLower > 0){
                    count = pass.Count(x => Char.IsLower(x));
                    if(count < this.numberOfLower){
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                }
                if(this.numberOfUpper > 0){
                    count = pass.Count(x => Char.IsUpper(x));
                    if(count < this.numberOfUpper){
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                }
                if(this.numberOfDigits > 0){
                    count = pass.Count(x => Char.IsDigit(x));
                    if(count < this.numberOfDigits){
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                }
                if(this.numberOfSymbols > 0){
                    count = pass.Count(x => Char.IsSymbol(x) || Char.IsPunctuation(x));
                    if(count < this.numberOfSymbols){
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                }
                return ValidationResult.Success;
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}