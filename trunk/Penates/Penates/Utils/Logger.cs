using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils {
    public class Logger {

        private ILog log;

        public Logger() {
            log4net.Config.XmlConfigurator.Configure();
            this.log = log4net.LogManager.GetLogger(
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
               );
        }

        public bool Debug(string message) {
            try {
                this.log.Debug(message);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool Debug(string message, Exception e) {
            try {
                this.log.Debug(message, e);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool Info(string message) {
            try {
                this.log.Info(message);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool Info(string message, Exception e) {
            try {
                this.log.Info(message, e);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool Warn(string message) {
            try {
                this.log.Warn(message);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool Warn(string message, Exception e) {
            try {
                this.log.Warn(message, e);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool Error(string message) {
            try {
                this.log.Error(message);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool Error(string message, Exception e) {
            try {
                this.log.Error(message, e);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool Fatal(string message) {
            try {
                this.log.Fatal(message);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool Fatal(string message, Exception e) {
            try {
                this.log.Fatal(message, e);
                return true;
            } catch (Exception) {
                return false;
            }
        }
    }
}