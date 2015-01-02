using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CloudPanel.Services.Scheduler.SQL
{
    public static class SchedulerRetrieve
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal static readonly string SecureKey = ConfigurationManager.AppSettings["Key"];
        private static DateTime _cachedsettingsretrieved;

        // Gets the queues from the database
        public static List<DatabaseQueue> NewQueues()
        {
            List<DatabaseQueue> result;
            try
            {
                SchedulerRetrieve.logger.Debug("Fixing to retrieve new queues from the database...");
                List<DatabaseQueue> databaseQueuesWaiting = DbSql.Get_DatabaseQueuesWaiting();
                if (databaseQueuesWaiting != null)
                {
                    SchedulerRetrieve.logger.Debug("Found a total of " + databaseQueuesWaiting.Count<DatabaseQueue>() + " queues in the database that need to be ran.");
                    result = databaseQueuesWaiting;
                }
                else
                {
                    SchedulerRetrieve.logger.Debug("Found a total of 0 queues in the database that need to be ran.");
                    result = null;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
        public static List<DatabaseTask> NewTasks()
        {
            List<DatabaseTask> result;
            try
            {
                SchedulerRetrieve.logger.Debug("Fixing to retrieve new tasks from the database...");
                List<DatabaseTask> databaseTasksReoccurringReady = DbSql.Get_DatabaseTasksReoccurringReady();
                if (databaseTasksReoccurringReady != null)
                {
                    SchedulerRetrieve.logger.Debug("Found a total of " + databaseTasksReoccurringReady.Count<DatabaseTask>() + " tasks in the database that need to be ran.");
                    result = databaseTasksReoccurringReady;
                }
                else
                {
                    SchedulerRetrieve.logger.Debug("Found a total of 0 tasks in the database that need to be ran.");
                    result = null;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
        public static void GetSettings()
        {
            try
            {
                TimeSpan value = new TimeSpan(2, 0, 0);
                DateTime arg_0F_0 = SchedulerRetrieve._cachedsettingsretrieved;
                DateTime arg_15_0 = SchedulerRetrieve._cachedsettingsretrieved;
                if (SchedulerRetrieve._cachedsettingsretrieved < DateTime.Now.Subtract(value))
                {
                    Config.Key = ConfigurationManager.AppSettings["Key"];

                    DataTable dataTable = SqlLibrary.ReadSql("SELECT TOP 1 * FROM Settings");
                    Config.HostingOU = dataTable.Rows[0]["BaseOU"].ToString();
                    SchedulerRetrieve.logger.Debug("Hosting OU: " + Config.HostingOU);
                    Config.PrimaryDC = dataTable.Rows[0]["PrimaryDC"].ToString();
                    SchedulerRetrieve.logger.Debug("Primary DC: " + Config.PrimaryDC);
                    Config.ExchangeServer = dataTable.Rows[0]["ExchangeFqdn"].ToString();
                    SchedulerRetrieve.logger.Debug("Exchange Server: " + Config.ExchangeServer);
                    Config.Username = dataTable.Rows[0]["Username"].ToString();
                    SchedulerRetrieve.logger.Debug("Username: " + Config.Username);
                    Config.Password = dataTable.Rows[0]["Password"].ToString();
                    SchedulerRetrieve.logger.Debug("Password: **********");
                    Config.ExchangeSSLEnabled = bool.Parse(dataTable.Rows[0]["ExchangeSSLEnabled"].ToString());
                    SchedulerRetrieve.logger.Debug("Exchange SSL Enabled: " + Config.ExchangeSSLEnabled.ToString());
                    Config.ExchangeStatsEnabled = bool.Parse(dataTable.Rows[0]["ExchStats"].ToString());
                    SchedulerRetrieve.logger.Debug("Exchange Stats Enabled: " + Config.ExchangeStatsEnabled.ToString());
                    Config.ExchangeVersion = ((int.Parse(dataTable.Rows[0]["ExchangeVersion"].ToString()) == 2013) ? Enumerations.ProductVersion.Exchange2013 : Enumerations.ProductVersion.Exchange2010);
                    SchedulerRetrieve.logger.Debug("Exchange Version: " + Config.ExchangeVersion.ToString());
                    Config.ExchangeConnectionType = ((dataTable.Rows[0]["ExchangeConnectionType"].ToString() == "Kerberos") ? Enumerations.ConnectionType.Kerberos : Enumerations.ConnectionType.Basic);
                    SchedulerRetrieve.logger.Debug("Exchange Connection Type: " + Config.ExchangeConnectionType.ToString());
                    Config.SupportMailEnabled = bool.Parse(dataTable.Rows[0]["SupportMailEnabled"].ToString());
                    SchedulerRetrieve.logger.Debug("Support mail enabled: " + Config.SupportMailEnabled.ToString());
                    Config.SupportMailAddress = dataTable.Rows[0]["SupportMailAddress"].ToString();
                    SchedulerRetrieve.logger.Debug("Support mail address: " + Config.SupportMailAddress);
                    Config.SupportMailServer = dataTable.Rows[0]["SupportMailServer"].ToString();
                    SchedulerRetrieve.logger.Debug("Support mail server: " + Config.SupportMailServer);
                    Config.SupportMailPort = int.Parse(dataTable.Rows[0]["SupportMailPort"].ToString());
                    SchedulerRetrieve.logger.Debug("Support mail port: " + Config.SupportMailPort.ToString());
                    Config.SupportMailUsername = dataTable.Rows[0]["SupportMailUsername"].ToString();
                    SchedulerRetrieve.logger.Debug("Support mail username: " + Config.SupportMailUsername.ToString());
                    Config.SupportMailFrom = dataTable.Rows[0]["SupportMailFrom"].ToString();
                    SchedulerRetrieve.logger.Debug("Support mail from: " + Config.SupportMailFrom);
                    SchedulerRetrieve._cachedsettingsretrieved = DateTime.Now;
                }
            }
            catch (Exception exception)
            {
                SchedulerRetrieve.logger.Fatal("Failed to retrieve settings from the database.", exception);
                throw;
            }
        }
    }
}
