using Penates.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penates.Interfaces.Repositories {
    public interface INotificationRepository {

        bool generateUserDeactivationNotifications(string userID, string userName);

        IQueryable<Notification> getData(string username);

        bool Delete(long NotificationID);

        long Save(Notification Notification);
    }
}
