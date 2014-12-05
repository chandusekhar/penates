using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Repositories.Users;
using Penates.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Repositories.Notifications {
    public class NotificationRepository : INotificationRepository {

        private PenatesEntities db = new PenatesEntities();

        public bool generateUserDeactivationNotifications(string userID, string userName) {
            long? id = this.db.SP_Notification_Add(System.DateTime.Now, (int) NotiType.User, null, null,
                String.Format(Resources.Messages.DeactivatedUserNotification, userName, userID)).SingleOrDefault();
            IUserRepository userRepo = new UserRepository();
            IQueryable<User> admins = userRepo.getUsersByRole(RoleType.Admin);
            if (!id.HasValue) {
                return false;
            }
            Notification not = this.db.Notifications.Find(id);
            foreach (User admin in admins) {
                User a = this.db.Users.Find(admin.FileNumber);
                a.Notifications.Add(not);
            }
            this.db.SaveChanges();
            return true;
        }

        private enum NotiType {
            User = 1,
            TotalMin = 2,
            Min = 3,
            Inventory = 4,
            Reception = 5
        }

        public IQueryable<Notification> getData(string username)
        {
            try{
                if (String.Equals(username, "SU", StringComparison.OrdinalIgnoreCase)) { //si es su ve todo
                    return this.db.Notifications
                        .OrderByDescending(x => x.Date).Skip(0).Take(Properties.Settings.Default.notifications);
                }
                return this.db.Notifications.Where(x => x.Users.Any(y => String.Compare(y.Username, username, StringComparison.OrdinalIgnoreCase)==0))
                    .OrderByDescending(x => x.Date).Skip(0).Take(Properties.Settings.Default.notifications);
            }
            catch (Exception e){
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary> Devuelve true si lo elimina y false si no encuentra que eliminar. Tira una excepcion /// </summary>
        /// <param name="id">ID a eliminar</param>
        /// <returns>Devuelve true si logra eliminar o false si no sabe que eliminar.</returns>
        /// <exception cref="DatabaseException"
        public bool Delete(long NotificationID)
        {
            Notification notification = this.db.Notifications.Find(NotificationID);
            if (notification == null)
            {
                return false;
            }
            var tran = this.db.Database.BeginTransaction();
            try
            {
                Notification n = this.db.Notifications.Find(NotificationID);
                this.db.Notifications.Remove(n);
                db.SaveChanges();
                tran.Commit();
                return true;
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw new DeleteException(String.Format(Resources.ExceptionMessages.DeleteException,
                        Resources.Resources.UserWArt, NotificationID), e.Message);
            }
        }

        public long Save(Notification Notification)
        {
            try
            {
                Nullable<long> val = null;
                val = this.db.SP_Notification_Add(Notification.Date, Notification.IDNotificationType, Notification.IDDistributionCenter, null, 
                    Notification.Message).SingleOrDefault(); 
                if (!val.HasValue)
                {
                    return -1;
                }
                return val.Value;
            }
            catch (DatabaseException de)
            {
                throw de;
            }
            catch (Exception)
            {
                throw new DatabaseException(String.Format(Resources.ExceptionMessages.SaveException));
            }
        }


    }
}