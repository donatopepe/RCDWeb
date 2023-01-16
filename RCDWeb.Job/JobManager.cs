using Quartz;
using Quartz.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Configuration;
using Common.Logging;

namespace RCDWeb.Job
{
    public class JobManager
    {
        private static readonly ILog Log = LogManager.GetLogger<JobManager>();

        public void JobCheckRichieste(string url)
        {
            
            //NameValueCollection properties = new NameValueCollection();
            //properties["quartz.scheduler.instanceName"] = "RemoteServer";

            //////// set thread pool info
            //properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            //properties["quartz.threadPool.threadCount"] = "5";
            //properties["quartz.threadPool.threadPriority"] = "Normal";

            //properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
            //properties["quartz.jobStore.useProperties"] = "true";
            //properties["quartz.jobStore.dataSource"] = "default";
            //properties["quartz.jobStore.tablePrefix"] = "QRTZ_";
            //properties["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz";

            //properties["quartz.dataSource.default.connectionString"] = "Data Source=127.0.0.1;Database=QRZ;Trusted_Connection=False;MultipleActiveResultSets=true;User ID=rcd_user;$1=[removed]";
            //properties["quartz.dataSource.default.provider"] = "SqlServer-20";

            // job initialization plugin handles our xml reading, without it defaults are used
            //properties["quartz.plugin.xml.type"] = "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz";
            //properties["quartz.plugin.xml.fileNames"] = "~/quartz_jobs.xml";

            ISchedulerFactory schedFact = new StdSchedulerFactory();//properties);
            IScheduler quartzScheduler = schedFact.GetScheduler();

            string withCronSchedule = "0 0 7-18 ? * * *";
            //string withCronSchedule = "0 */2 * ? * * *";

            IJobDetail job = JobBuilder.Create<CheckRichiesteJob>()
                .WithIdentity("CheckRichiesteJob", "JOB_RCD")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("RCD", "IDG_RCD")
                .WithCronSchedule(withCronSchedule) //dal lun-ven, tutti i mesi alle 8.30                                                                             
                                      //.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(ora_start, min_start))
                .StartAt(DateTimeOffset.Now)
                .WithPriority(1)
                .Build();

            if (quartzScheduler.CheckExists(trigger.Key))
            {
                quartzScheduler.UnscheduleJob(trigger.Key);
            }
            if (!quartzScheduler.CheckExists(trigger.Key))
            {
                quartzScheduler.ScheduleJob(job, trigger);
            }



            //string withKeepAlive = "0 */2 * * * ?";


            //IJobDetail jobKeepAlive = JobBuilder.Create<KeepAlive>()
            //    .WithIdentity("KeepAlive", "JOB_RCD")
            //    .UsingJobData("url", url)
            //    .Build();

            //ITrigger triggerKeepAlive = TriggerBuilder.Create()
            //    .WithIdentity("KeepAlive", "IDG_RCD")
            //    .WithCronSchedule(withKeepAlive) //dal lun-ven, tutti i mesi alle 8.30                                                                             
            //                                     //.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(ora_start, min_start))
            //    .StartAt(DateTimeOffset.Now)
            //    .WithPriority(1)
            //    .Build();


            //if (quartzScheduler.CheckExists(triggerKeepAlive.Key))
            //{
            //    quartzScheduler.UnscheduleJob(triggerKeepAlive.Key);
            //}
            //if (!quartzScheduler.CheckExists(triggerKeepAlive.Key))
            //{
            //    quartzScheduler.ScheduleJob(jobKeepAlive, triggerKeepAlive);
            //}

            quartzScheduler.Start();
        }


    }
}
