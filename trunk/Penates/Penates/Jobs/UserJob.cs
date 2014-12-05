using Penates.Database;
using Penates.Interfaces.Repositories;
using Penates.Repositories.Notifications;
using Penates.Repositories.Users;
using Penates.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Penates.Jobs {
    public class UserJob : IJob {

        PenatesEntities db = new PenatesEntities();
        Logger log = new Logger();

        public void Execute(IJobExecutionContext context) {
            Mail mail = new Mail();
            mail.send("User Inactivity", "User Inactivity Deactivation Started");
            log.Info("User Inactivity Deactivation Started");
            log.Debug("User Inactivity Deactivation Started");
            this.disableUsers();
            log.Info("User Inactivity Deactivation Finished Successfully");
            log.Debug("User Inactivity Deactivation Finished Successfully");
            mail.send("User Inactivity", "User Inactivity Deactivation Finished Successfully");
        }

        /// <summary>Elimina los usuarios y genera las notificaciones necesarias</summary>
        private void disableUsers() {
            using (PenatesEntities context = new PenatesEntities()) {
                IQueryable<User> users = this.db.Users.
                    Where(x => x.LastLoginDate < DbFunctions.AddDays(DateTime.Now, -Properties.Settings.Default.DaysToDeactivateUser)
                    && x.Active == true);
                IUserRepository userRepo = new UserRepository();
                INotificationRepository notificationRepo = new NotificationRepository();
                foreach (User user in users) {
                    var tran = context.Database.BeginTransaction();
                    try {
                        userRepo.Deactivate(user.FileNumber);
                        notificationRepo.generateUserDeactivationNotifications(user.FileNumber, user.Username);
                        tran.Commit();
                    } catch (Exception e) {
                        log.Error(e.Message);
                        tran.Rollback();
                    }
                }
            }
        }
    }
}