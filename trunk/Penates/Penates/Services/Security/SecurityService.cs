using ApplicationSettingsWriter;
using Penates.Interfaces.Services;
using Penates.Models.ViewModels.Users;
using Penates.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Penates.Services.Security {
    public class SecurityService : ISecurityService{

        /// <summary>Trae los parametros de seguridad para mostrarlos</summary>
        /// <returns>SecurityParametersViewModel</returns>
        public SecurityParametersViewModel getParameters() {
            SecurityParametersViewModel model = new SecurityParametersViewModel();

            int? maxMin = this.getSessionTimeout();
            if (maxMin.HasValue && maxMin.Value > 0) {
                model.SessionTime = maxMin.Value;
            }

            //Traigo lo del Usuario
            maxMin = this.getUsernameMaxLenght();
            if (maxMin.HasValue && maxMin.Value > 0) {
                model.HasUsernameMax = true;
                model.UsernameMax = maxMin;
            }
            maxMin = this.getUsernameMinLenght();
            if (maxMin.HasValue && maxMin.Value >= 0) {
                model.HasUsernameMin = true;
                model.UsernameMin = maxMin;
            }
            long? constraint = this.getUserNumberOfLowerCase();
            if (constraint.HasValue && constraint.Value >= 0) {
                model.HasUsernameLower = true;
                model.UsernameLower = constraint;
            }
            constraint = this.getUserNumberOfUpperCase();
            if (constraint.HasValue && constraint.Value >= 0) {
                model.HasUsernameUpper = true;
                model.UsernameUpper = constraint;
            }
            constraint = this.getUserNumberOfDigits();
            if (constraint.HasValue && constraint.Value >= 0) {
                model.HasUsernameDigits = true;
                model.UsernameDigits = constraint;
            }
            constraint = this.getUserNumberOfSymbols();
            if (constraint.HasValue && constraint.Value >= 0) {
                model.HasUsernameSymbols = true;
                model.UsernameSymbols = constraint;
            }

            //Traigo lo de la Password
            maxMin = this.getPasswordMaxLenght();
            if (maxMin.HasValue && maxMin.Value > 0) {
                model.HasPasswordMax = true;
                model.PasswordMax = maxMin;
            }
            maxMin = this.getPasswordMinLenght();
            if (maxMin.HasValue && maxMin.Value >= 0) {
                model.HasPasswordMin = true;
                model.PasswordMin = maxMin;
            }
            constraint = this.getPasswordNumberOfLowerCase();
            if (constraint.HasValue && constraint.Value >= 0) {
                model.HasPasswordLower = true;
                model.PasswordLower = constraint;
            }
            constraint = this.getPasswordNumberOfUpperCase();
            if (constraint.HasValue && constraint.Value >= 0) {
                model.HasPasswordUpper = true;
                model.PasswordUpper = constraint;
            }
            constraint = this.getPasswordNumberOfDigits();
            if (constraint.HasValue && constraint.Value >= 0) {
                model.HasPasswordDigits = true;
                model.PasswordDigits = constraint;
            }
            constraint = this.getPasswordNumberOfSymbols();
            if (constraint.HasValue && constraint.Value >= 0) {
                model.HasPasswordSymbols = true;
                model.PasswordSymbols = constraint;
            }

            return model;
        }

        /// <summary>Guarda las Settings de la applicacion</summary>
        /// <param name="parameters">SecurityParametersViewModel</param>
        /// <returns>Status</returns>
        public Status setParameters(SecurityParametersViewModel parameters) {
            try {
                var configuration = WebConfigurationManager.OpenWebConfiguration(HttpRuntime.AppDomainAppVirtualPath);

                if (parameters.SessionTime >= 4) {
                    configuration.AppSettings.Settings.Remove("sessionTimeout");
                    configuration.AppSettings.Settings.Add("sessionTimeout", parameters.SessionTime.ToString());
                }

                //Trato lo de Usuarios
                if (parameters.HasUsernameMax && parameters.UsernameMax.HasValue) {
                    configuration.AppSettings.Settings.Remove("usernameMaxLenght");
                    if (parameters.UsernameMax.Value > 0) {
                        configuration.AppSettings.Settings.Add("usernameMaxLenght", parameters.UsernameMax.Value.ToString());
                    } else {
                        configuration.AppSettings.Settings.Add("usernameMaxLenght", "-1");
                    }
                }
                if (parameters.HasUsernameMin && parameters.UsernameMin.HasValue) {
                    configuration.AppSettings.Settings.Remove("usernameMinLenght");
                    if (parameters.UsernameMin.Value >= 0) {
                        configuration.AppSettings.Settings.Add("usernameMinLenght", parameters.UsernameMin.Value.ToString());
                    } else {
                        configuration.AppSettings.Settings.Add("usernameMinLenght", "-1");
                    }
                }
                if (parameters.HasUsernameLower && parameters.UsernameLower.HasValue) {
                    configuration.AppSettings.Settings.Remove("userNumberOfLowerCase");
                    if (parameters.UsernameLower.Value >= 0) {
                        configuration.AppSettings.Settings.Add("userNumberOfLowerCase", parameters.UsernameLower.Value.ToString());
                    } else {
                        configuration.AppSettings.Settings.Add("userNumberOfLowerCase", "-1");
                    }
                }
                if (parameters.HasUsernameUpper && parameters.UsernameUpper.HasValue) {
                    configuration.AppSettings.Settings.Remove("userNumberOfUpperCase");
                    if (parameters.UsernameUpper.Value >= 0) {
                        configuration.AppSettings.Settings.Add("userNumberOfUpperCase", parameters.UsernameUpper.Value.ToString());
                    } else {
                        configuration.AppSettings.Settings.Add("userNumberOfUpperCase", "-1");
                    }
                }
                if (parameters.HasUsernameDigits && parameters.UsernameDigits.HasValue) {
                    configuration.AppSettings.Settings.Remove("userNumberOfDigits");
                    if (parameters.UsernameDigits.Value >= 0) {
                        configuration.AppSettings.Settings.Add("userNumberOfDigits", parameters.UsernameDigits.Value.ToString());
                    } else {
                        configuration.AppSettings.Settings.Add("userNumberOfDigits", "-1");
                    }
                }
                if (parameters.HasUsernameSymbols && parameters.UsernameSymbols.HasValue) {
                    configuration.AppSettings.Settings.Remove("userNumberOfSymbols");
                    if (parameters.UsernameSymbols.Value >= 0) {
                        configuration.AppSettings.Settings.Add("userNumberOfSymbols", parameters.UsernameSymbols.Value.ToString());
                    } else {
                        configuration.AppSettings.Settings.Add("userNumberOfSymbols", "-1");
                    }
                }

                //Trato password
                if (parameters.HasPasswordMax && parameters.PasswordMax.HasValue) {
                    configuration.AppSettings.Settings.Remove("passwordMaxLenght");
                    if (parameters.PasswordMax.Value > 0) {
                        configuration.AppSettings.Settings.Add("passwordMaxLenght", parameters.PasswordMax.Value.ToString());
                    } else {
                        configuration.AppSettings.Settings.Add("passwordMaxLenght", "-1");
                    }
                }
                if (parameters.HasPasswordMin && parameters.PasswordMin.HasValue) {
                    configuration.AppSettings.Settings.Remove("passwordMinLenght");
                    if (parameters.PasswordMin.Value >= 0) {
                        configuration.AppSettings.Settings.Add("passwordMinLenght", parameters.PasswordMin.Value.ToString());
                    } else {
                        configuration.AppSettings.Settings.Add("passwordMinLenght", "-1");
                    }
                }
                if (parameters.HasPasswordLower && parameters.PasswordLower.HasValue) {
                    configuration.AppSettings.Settings.Remove("passwordNumberOfLowerCase");
                    if (parameters.PasswordLower.Value >= 0) {
                        configuration.AppSettings.Settings.Add("passwordNumberOfLowerCase", parameters.PasswordLower.Value.ToString());
                    } else {
                        configuration.AppSettings.Settings.Add("passwordNumberOfLowerCase", "-1");
                    }
                }
                if (parameters.HasPasswordUpper && parameters.PasswordUpper.HasValue) {
                    configuration.AppSettings.Settings.Remove("passwordNumberOfUpperCase");
                    if (parameters.PasswordUpper.Value >= 0) {
                        configuration.AppSettings.Settings.Add("passwordNumberOfUpperCase", parameters.PasswordUpper.Value.ToString());
                    } else {
                        configuration.AppSettings.Settings.Add("passwordNumberOfUpperCase", "-1");
                    }
                }
                if (parameters.HasPasswordDigits && parameters.PasswordDigits.HasValue) {
                    configuration.AppSettings.Settings.Remove("passwordNumberOfDigits");
                    if (parameters.PasswordDigits.Value >= 0) {
                        configuration.AppSettings.Settings.Add("passwordNumberOfDigits", parameters.PasswordDigits.Value.ToString());
                    } else {
                        configuration.AppSettings.Settings.Add("passwordNumberOfDigits", "-1");
                    }
                }
                if (parameters.HasPasswordSymbols && parameters.PasswordSymbols.HasValue) {
                    configuration.AppSettings.Settings.Remove("passwordNumberOfSymbols");
                    if (parameters.PasswordSymbols.Value >= 0) {
                        configuration.AppSettings.Settings.Add("passwordNumberOfSymbols", parameters.PasswordSymbols.Value.ToString());
                    } else {
                        configuration.AppSettings.Settings.Add("passwordNumberOfSymbols", "-1");
                    }
                }
                configuration.Save();

                return new Status() {
                    Success = true
                };
            }catch(Exception e){
                return new Status() {
                    Success = false,
                    Message = e.Message
                };
            }
        }

        private int? getIntParam(string s) {
            try {
                string value = ConfigurationManager.AppSettings[s];
                return Int32.Parse(value);
            }catch(Exception){
                return null;
            }
        }

        private long? getLongParam(string s) {
            try {
                string value = ConfigurationManager.AppSettings[s];
                return Int64.Parse(value);
            } catch (Exception) {
                return null;
            }
        }

        public int? getUsernameMaxLenght() {
            int? x = this.getIntParam("usernameMaxLenght");
            if (x.HasValue && x.Value > 0) {
                return x;
            }
            return null;
        }

        public int? getSessionTimeout() {
            int? x = this.getIntParam("sessionTimeout");
            if (!x.HasValue && x.Value < 0) {
                try {
                    Configuration conf = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                    SessionStateSection section = (SessionStateSection) conf.GetSection("system.web/sessionState");
                    x = (int) section.Timeout.TotalMinutes;
                }catch(Exception){
                    return null;
                }
            }
            return x;
        }

        public int? getUsernameMinLenght() {
            int? x = this.getIntParam("usernameMinLenght");
            if (!x.HasValue && x.Value < 0) {
                return null;
            }
            return x;
        }

        public long? getUserNumberOfLowerCase() {
            long? x = this.getLongParam("userNumberOfLowerCase");
            if (!x.HasValue && x.Value < 0) {
                return null;
            }
            return x;
        }

        public long? getUserNumberOfUpperCase() {
            long? x = this.getLongParam("userNumberOfUpperCase");
            if (!x.HasValue && x.Value < 0) {
                return null;
            }
            return x;
        }

        public long? getUserNumberOfDigits() {
            long? x = this.getLongParam("userNumberOfDigits");
            if (!x.HasValue && x.Value < 0) {
                return null;
            }
            return x;
        }

        public long? getUserNumberOfSymbols() {
            long? x = this.getLongParam("userNumberOfSymbols");
            if (!x.HasValue && x.Value < 0) {
                return null;
            }
            return x;
        }

        public int? getPasswordMaxLenght() {
            int? x = this.getIntParam("passwordMaxLenght");
            if (!x.HasValue && x.Value <= 0) {
                return null;
            }
            return x;
        }

        public int? getPasswordMinLenght() {
            int? x = this.getIntParam("passwordMinLenght");
            if (!x.HasValue && x.Value < 0) {
                return null;
            }
            return x;
        }

        public long? getPasswordNumberOfLowerCase() {
            long? x = this.getLongParam("passwordNumberOfLowerCase");
            if (!x.HasValue && x.Value < 0) {
                return null;
            }
            return x;
        }

        public long? getPasswordNumberOfUpperCase() {
            long? x = this.getLongParam("passwordNumberOfUpperCase");
            if (!x.HasValue && x.Value < 0) {
                return null;
            }
            return x;
        }

        public long? getPasswordNumberOfDigits() {
            long? x = this.getLongParam("passwordNumberOfDigits");
            if (!x.HasValue && x.Value < 0) {
                return null;
            }
            return x;
        }

        public long? getPasswordNumberOfSymbols() {
            long? x = this.getLongParam("passwordNumberOfSymbols");
            if (!x.HasValue && x.Value < 0) {
                return null;
            }
            return x;
        }

        public static int getStaticUsernameMaxLenght() {
            ISecurityService service = new SecurityService();
            int? x = service.getUsernameMaxLenght();
            if (!x.HasValue && x.Value <= 0) {
                return Int32.MaxValue;
            }
            return x.Value;
        }

        public static int getStaticUsernameMinLenght() {
            ISecurityService service = new SecurityService();
            int? x = service.getUsernameMinLenght();
            if (!x.HasValue && x.Value < 0) {
                return 0;
            }
            return x.Value;
        }

        public static int getStaticPasswordMaxLenght() {
            ISecurityService service = new SecurityService();
            int? x = service.getPasswordMaxLenght();
            if (!x.HasValue || x.Value <= 0) {
                return Int32.MaxValue;
            }
            return x.Value;
        }

        public static int getStaticPasswordMinLenght() {
            ISecurityService service = new SecurityService();
            int? x = service.getPasswordMinLenght();
            if (!x.HasValue && x.Value < 0) {
                return 0;
            }
            return x.Value;
        }
    }
}