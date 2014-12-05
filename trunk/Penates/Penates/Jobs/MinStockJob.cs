using Penates.Database;
using Penates.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Jobs {
    public class MinStockJob : IJob {

        PenatesEntities db = new PenatesEntities();
        Logger log = new Logger();

        public void Execute(IJobExecutionContext context) {
            Mail mail = new Mail();
            mail.send("Minimum Stock Analysis", "Minimum Stock Analysis Started");
            log.Info("Minimum Stock Analysis Started");
            log.Debug("Minimum Stock Analysis Started");
            //aca la ejecucion
            log.Info("Minimum Stock Analysis Finished Successfully");
            log.Debug("Minimum Stock Analysis Finished Successfully");
            mail.send("Minimum Stock Analysis", "Minimum Stock Analysis Finished Successfully");
        }
    }
}