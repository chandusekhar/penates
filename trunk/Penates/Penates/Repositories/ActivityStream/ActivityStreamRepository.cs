using Penates.Database;
using Penates.Exceptions.Database;
using Penates.Interfaces.Repositories;
using Penates.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Repositories.ActivityStream
{
    public class ActivityStreamRepository : IActivityStreamRepository
    {
        private PenatesEntities db = new PenatesEntities();

        public IQueryable<Sale> getSales(string userName)
        {               
            try
            {
                if (userName.Equals("SU", StringComparison.OrdinalIgnoreCase)) {
                    return this.db.Sales
                    .OrderByDescending(x => x.SaleDate).Skip(0).Take(Properties.Settings.Default.activitiesNumber);
                }
                UserService userService = new UserService();
                User u = userService.GetUserByUserName(userName);
                List<long> dcs = u.DistributionCenters.Select(x => x.DistributionCenterID).ToList();
                return this.db.Sales
                    .Where(x => dcs.Contains(x.IDDistributionCenter))
                    .OrderByDescending(x => x.SaleDate).Skip(0).Take(Properties.Settings.Default.activitiesNumber);
            }
            catch (Exception e)
            {
                throw new DatabaseException(e.Message, e);
            }  
        }

        public IQueryable<Transfer> getTransfers(string userName)
        {
            try
            {
                if (userName.Equals("SU", StringComparison.OrdinalIgnoreCase)) {
                    return this.db.Transfers
                    .OrderByDescending(x => x.TransferArrivalDate).Skip(0).Take(Properties.Settings.Default.activitiesNumber);
                }
                UserService userService = new UserService();
                User u = userService.GetUserByUserName(userName);
                List<long> dcs = u.DistributionCenters.Select(x => x.DistributionCenterID).ToList();
                return this.db.Transfers
                    .Where(x => dcs.Any(y => y == x.IDReciever || y == x.IDSender))
                    .OrderByDescending(x => x.TransferArrivalDate).Skip(0).Take(Properties.Settings.Default.activitiesNumber);
            }
            catch (Exception e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }
        
        public IQueryable<Reception> getReceptions(string userName)
        {
            try
            {
                if (userName.Equals("SU", StringComparison.OrdinalIgnoreCase)) {
                    return this.db.Receptions
                    .OrderByDescending(x => x.ReceivingDate).Skip(0).Take(Properties.Settings.Default.activitiesNumber);
                }
                UserService userService = new UserService();
                User u = userService.GetUserByUserName(userName);
                List<long> dcs = u.DistributionCenters.Select(x => x.DistributionCenterID).ToList();
                return this.db.Receptions
                    .Where(x => dcs.Contains(x.IDDistributionCenter))
                    .OrderByDescending(x => x.ReceivingDate).Skip(0).Take(Properties.Settings.Default.activitiesNumber);
            }
            catch (Exception e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }

    }
}