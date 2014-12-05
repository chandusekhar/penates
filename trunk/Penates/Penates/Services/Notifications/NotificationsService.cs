using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Repositories.Notifications;
using Penates.Utils.JSON.TableObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Web;

namespace Penates.Services.Notifications
{
    public class NotificationsService
    {
         private INotificationRepository repository = new NotificationRepository();

         public IQueryable<Notification> getData(string username)
         {
             return this.repository.getData(username);
         }


         public List<NotificationTableJson> toJsonArray(IQueryable<Notification> query)
         {
             try {
                 return this.toJsonArray(query.ToList());
             } catch (EntityException) {
                 return new List<NotificationTableJson>();
             }
         }

         public List<NotificationTableJson> toJsonArray(ICollection<Notification> list)
         {
             List<NotificationTableJson> result = new List<NotificationTableJson>();
             NotificationTableJson aux;
             foreach (Notification notification in list)
             {
                 aux = new NotificationTableJson()
                 {
                     Date = notification.Date.ToShortDateString() + " - " + notification.Date.ToLongTimeString(),
                     Message = notification.Message,
                     NotificationID = notification.NotificationID
                 };
                 if (notification.DistributionCenter != null) {
                     aux.Address = notification.DistributionCenter.Address;
                 }
                 result.Add(aux);
             }
             return result;
         }


         /// <summary>Elimina una Notificacion</summary>
         /// <param name="id">ID de la Notificacion a Eliminar</param>
         /// <returns>True si logra eliminarlo, false si ya estaba eliminado</returns>
         /// <exception cref="DatabaseException" si ocurre un error de BD </exception>
         public bool Delete(long NotificationID)
         {
             return this.repository.Delete(NotificationID);
         }

         public decimal save(Notification notification)
         {
             long value = this.repository.Save(notification); //Capturo el ID o Errores del Sp
             switch (value)
             {
                 case -1:
                     throw new StoredProcedureException(String.Format(Resources.ExceptionMessages.SaveException,
                         Resources.Resources.OrderWArt));
             }
             return value;
         }

    }

    
    
}