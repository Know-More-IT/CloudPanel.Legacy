using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Sql;
using CloudPanel.Services.Scheduler.Class;
using CloudPanel.Services.Scheduler.SQL;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace CloudPanel.Services.CPScheduler.Class
{
    public class QueueRunner
    {
        // Log utility
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // To stop our thread if we need to
        private bool _StopNow = false;

        // Object for lock
        private readonly object sharedMonitor = new object();

        // How often to query again in minutes
        int queryInMin = 5;

        public QueueRunner(int queryEveryMinutes)
        {
            queryInMin = queryEveryMinutes;
        }

        /// <summary>
        /// Queries the database for new tasks and executes them
        /// </summary>
        internal void QueryDatabase()
        {
            while (true)
            {
                lock (sharedMonitor)
                {
                    try
                    {
                        if (_StopNow)
                            return;
                        else
                        {
                            // Get queues from the database
                            List<DatabaseQueue> foundQueues = SchedulerRetrieve.NewQueues();

                            if (foundQueues != null && foundQueues.Count > 0)
                            {
                                // Loop through the found queues and execute
                                foreach (DatabaseQueue queue in foundQueues)
                                {
                                    try
                                    {
                                        switch (queue.TaskType)
                                        {
                                            case Enumerations.TaskType.MailboxCalendarPermissions:
                                                SetCalendarPermissions(queue);
                                                break;
                                            default:
                                                logger.Warn("Unknown queue found in database: " + queue.TaskType.ToString());
                                                break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error("Failed to run queue " + queue.QueueID.ToString(), ex);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // LOG //
                        logger.Fatal("Fatal error has occurred.", ex);
                    }

                    // DEBUG //
                    logger.Debug("Waiting for another " + queryInMin.ToString() + " minutes before checking again...");

                    Monitor.Wait(sharedMonitor, queryInMin * 60000);
                }
            }
        }

        /// <summary>
        /// Modifies the mailbox permissions for the user and updates the database that it was completed
        /// </summary>
        /// <param name="getQueue"></param>
        /// <param name="database"></param>
        private void SetCalendarPermissions(DatabaseQueue getQueue)
        {
            ExchPowershell powershell = null;

            try
            {
                // DEBUG // 
                logger.Debug("Found queue to modify mailbox calendar permissions for " + getQueue.UserPrincipalName);

                // Start up powershell
                powershell = new ExchPowershell();

                // Run powershell command
                powershell.Set_MailboxCalendarPermission(getQueue.UserPrincipalName, getQueue.CompanyCode);

                // Update database
                getQueue.TaskOutput = "Completed Successfully";
                getQueue.TaskCompleted = DateTime.Now;
                getQueue.TaskSuccess = (int)Enumerations.TaskSuccess.Completed;

                DbSql.Update_DatabaseQueue(getQueue);
            }
            catch (Exception ex)
            {
                // Update database
                getQueue.TaskOutput = ex.Message;
                getQueue.TaskCompleted = DateTime.Now;
                getQueue.TaskSuccess = Enumerations.TaskSuccess.Failed;

                DbSql.Update_DatabaseQueue(getQueue);

                // Compile message
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Failed to set the correct calendar permissions on user: " + getQueue.UserPrincipalName);
                sb.AppendLine("");
                sb.AppendLine("Recommended Action:");
                sb.AppendLine("Not having the correct permissions on the mailbox calendar can cause leakage of information between tenants. Please contact support or try the recommended action below.");
                sb.AppendLine("");
                sb.AppendLine("Open Exchange Shell and run the following commands:");
                sb.AppendLine(string.Format(@"Set-MailboxFolderPermission -Identity '{0}:\Calendar' -User Default -AccessRights None", getQueue.UserPrincipalName));
                sb.AppendLine("");
                sb.AppendLine(string.Format(@"Add-MailboxFolderPermission -Identity '{0}:\Calendar' -User 'ExchangeSecurity@{1}' -AccessRights AvailabilityOnly", getQueue.UserPrincipalName, getQueue.CompanyCode));
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("Please remember that the word 'Calendar' may be in a differnet language depending on the language the mailbox is set for.");
                sb.AppendLine("If you have any issues after attempting to repair please contact support.");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("Error:");
                sb.AppendLine(ex.ToString());

                // Send message
                Support.SendEmailMessage("Failed to set calendar permissions on user: "+  getQueue.UserPrincipalName, sb.ToString());
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Requests the thread to stop
        /// </summary>
        public void Stop()
        {
            // Tell it should stop
            _StopNow = true;

            lock (sharedMonitor)
            {
                // Pulse the lock object
                Monitor.Pulse(sharedMonitor);
            }
        }
    }
}
