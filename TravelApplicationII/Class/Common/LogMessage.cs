using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TravelApplication.Class.Common
{
    public class LogMessage
    {
        public static void Log(string logMessage)
        {
            string strLogFilePath = System.Web.Hosting.HostingEnvironment.MapPath(System.Configuration.ConfigurationManager.AppSettings["logFilePath"].ToString());
            string strLogMessage = string.Empty;            
            string strLogFile = strLogFilePath + "log1.txt";
            StreamWriter swLog;
            strLogMessage = string.Format("{0}: {1}", DateTime.Now, logMessage);

            if (!Directory.Exists(strLogFile)) Directory.CreateDirectory(strLogFilePath);
            
            // check if log file exists
            if (!File.Exists(strLogFile))
                { swLog = new StreamWriter(strLogFile); }
            else
                { swLog = File.AppendText(strLogFile); }

            swLog.WriteLine(strLogMessage);

            swLog.Close();
        }
    }
}