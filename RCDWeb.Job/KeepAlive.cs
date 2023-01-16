
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;

namespace RCDWeb.Job
{
    [PersistJobDataAfterExecution]
    public class KeepAlive : IJob
    {
        private static readonly ILog Log = LogManager.GetLogger<KeepAlive>();

        public void Execute(IJobExecutionContext context)
        {

            try
            {
                JobKey key = context.JobDetail.Key;
                Log.Info(key);
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                string url = dataMap.GetString("url");

                WebRequest req = WebRequest.Create(url);
                Log.Info(req.GetResponse().ResponseUri);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
    }
}
