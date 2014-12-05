using Penates.Database;
using Penates.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Jobs {
    public class InventoryReminderJob : IJob {

        PenatesEntities db = new PenatesEntities();
        Logger log = new Logger();

        public void Execute(IJobExecutionContext context) {
            log.Info("Inventory Reminder Notifications Started");
            log.Debug("Inventory Reminder Notifications Started");
            //aca la ejecucion
            log.Info("Inventory Reminder Notifications Finished Successfully");
            log.Debug("Inventory Reminder Notifications Finished Successfully");
        }
    }
}