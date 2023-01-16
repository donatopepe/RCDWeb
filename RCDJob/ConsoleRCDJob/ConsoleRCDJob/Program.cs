using Common.Logging;
using RCDWeb.Job;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRCDJob
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger<Program>();
        //private static void LogTraceMessage()
        //{
        //    Log.Trace("This is a trace...");
        //}

        //private static void LogDebugMessage()
        //{
        //    Log.Debug("This is a debug message...");
        //}

        //private static void LogInfoMessage()
        //{
        //    Log.Info("This is an info message...");
        //}

        //private static void LogWarningMessage()
        //{
        //    Log.Warn("This is a warning...");
        //}

        //private static void LogErrorMessage()
        //{
        //    Log.Error("This is an error...");
        //}

        //private static void LogFatalErrorMessage()
        //{
        //    Log.Fatal("This is a fatal error...");
        //}

        static void Main(string[] args)
        {
            //LogTraceMessage();
            //LogDebugMessage();
            //LogInfoMessage();
            //LogWarningMessage();
            //LogErrorMessage();
            //LogFatalErrorMessage();
            string host = ConfigurationManager.AppSettings["Host"];
            string nomePortale = ConfigurationManager.AppSettings["NomePortale"];
            string hostName = @"http://sso.example.com/Webapps/Portal/login.aspx?url=http://" + host + nomePortale;
            string fullUrl = hostName + @"/RCD/Richiesta/GestioneRichieste/";
            Log.Info(fullUrl);
            new CheckRichiesteJob().Check();
            var pippo = new JobManager();
            pippo.JobCheckRichieste("http://www.google.it");

        }
    }
}
