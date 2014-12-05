using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Penates.Utils {
    public class Mail {

        private string sender;
        private string password;
        SmtpClient smtp;
        public string addresses;
        public string title;

        public Mail()
            : this("pegasus.enterprise.soft@gmail.com", "santy34905964") {
        }

        public Mail(string sender, string pass) {
            this.sender = sender;
            this.password = pass;
            this.addresses = Properties.Settings.Default.emailAddresses;
            this.title = "[Penates Inventory System]";
            this.smtp = new SmtpClient {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(sender, pass)
            }; 
        }

        public void send(string subject, string body) {
            try {
                using (var message = new MailMessage() {
                    Subject = this.title + subject,
                    Body = body
                }) {
                    message.From = new MailAddress(this.sender, "Penates Inventory System");
                    message.To.Add(this.addresses);
                    smtp.Send(message);
                }
            } catch (Exception e) {
                Logger log = new Logger();
                log.Error(e.Message, e);
            }
        }
    }
}