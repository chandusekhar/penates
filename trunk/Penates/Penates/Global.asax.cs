using Penates.Interfaces.Services;
using Penates.Jobs;
using Penates.Services.Security;
using Penates.Utils;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Penates
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            HtmlHelper.ClientValidationEnabled = true;
            HtmlHelper.UnobtrusiveJavaScriptEnabled = true;
            try {
                ISecurityService service = new SecurityService();
                int? timeout = service.getSessionTimeout();
                if (timeout.HasValue) {
                    Session.Timeout = timeout.Value;
                }
            }catch(Exception){

            }
            this.startLogger();
            this.createJobs();
        }

        protected void Application_End() {
            try {
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                Logger log = new Logger();
                log.Info("Server Stopped");
                scheduler.Shutdown();
            } catch (SchedulerException se) {
                Console.WriteLine(se);
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e) {
            //Note everything hardcoded, for simplicity!
            if (Request.UserLanguages.Length == 0)
                return;

            string language = Request.UserLanguages[0];

            if (language.Substring(0, 2).ToLower() == "es")
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("es");
        }

        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{handler}.ashx", new { handler = @"captcha" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    "Default", // Route name
            //    ConfigurationManager.AppSettings["ControllerName"] + "/registro", // URL with parameters
            //    new { controller = "Home", action = "Index" });

            routes.MapRoute(
                "ProductForm", // Route name
                "ProductForm", // URL with parameters
                new { controller = "Home", action = "Index" });
        }

        private void createJobs() {
            try {
                Logger log = new Logger();

                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                scheduler.Start();
                log.Info("Job Scheduler Started");

                //Hacer schedule de los Jobs de usuarios
                IJobDetail job = JobBuilder.Create<UserJob>()
                    .WithIdentity("userJob", "group1")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("userTrigger", "group1")
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(00, 00))
                    .Build();

                scheduler.ScheduleJob(job, trigger);
                log.Info("User Job Scheduled");

                //Hacer schedule del Job para las Policies
                job = JobBuilder.Create<MinStockJob>()
                    .WithIdentity("policyJob", "policyGroup")
                    .Build();

                trigger = TriggerBuilder.Create()
                    .WithIdentity("policyTrigger", "policyGroup")
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(01, 00))
                    .Build();

                scheduler.ScheduleJob(job, trigger);
                log.Info("Policy Job Scheduled");

                //Hacer schedule del Job para las Policies
                job = JobBuilder.Create<InventoryReminderJob>()
                    .WithIdentity("inventoryJob", "inventoryGroup")
                    .Build();

                trigger = TriggerBuilder.Create()
                    .WithIdentity("inventoryTrigger", "inventoryGroup")
                    .WithSchedule(DailyTimeIntervalScheduleBuilder.Create().OnDaysOfTheWeek(DayOfWeek.Sunday)
                    .StartingDailyAt(new TimeOfDay(3,00)))
                    .Build();

                scheduler.ScheduleJob(job, trigger);
                log.Info("Inventory Job Scheduled");
            } catch (SchedulerException se) {
                Logger log = new Logger();
                log.Fatal(se.Message, se);
            }
        }

        private void startLogger(){
            Logger log = new Logger();
            log.Debug("Server Started");
            log.Info("Server Started");
        }
    }
}
