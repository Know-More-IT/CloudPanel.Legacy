using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Sql;
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
    public class TaskRunner
    {
        // Log utilitys
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // To stop our thread if we need to
        private bool _StopNow = false;

        // Object for lock
        private readonly object sharedMonitor = new object();

        // How often to query again in minutes
        int queryInMin = 5;

        public TaskRunner(int queryEveryMinutes)
        {
            queryInMin = queryEveryMinutes;
        }

        /// <summary>
        /// Queries the database for new tasks and executes them
        /// </summary>
        public void QueryDatabase()
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
                            List<DatabaseTask> foundTasks = SchedulerRetrieve.NewTasks();

                            if (foundTasks != null && foundTasks.Count > 0)
                            {
                                // Loop through the found queues and execute
                                foreach (DatabaseTask task in foundTasks)
                                {
                                    try
                                    {
                                        switch (task.TaskType)
                                        {
                                            case Enumerations.TaskType.ReadMailboxSizes:
                                                ReadMailboxSizes(task);
                                                break;
                                            case Enumerations.TaskType.ReadMailboxDatabaseSizes:
                                                ReadMailboxDatabaseSizes(task);
                                                break;
                                            default:
                                                logger.Warn("Found an unknown task in the database: " + task.TaskID.ToString());
                                                break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error("Error on task id " + task.TaskID.ToString(), ex);
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
        /// Reads the size of each mailbox in the SQL database
        /// </summary>
        /// <param name="getTask"></param>
        /// <param name="database"></param>
        private void ReadMailboxSizes(DatabaseTask getTask)
        {
            ExchPowershell powershell = null;

            try
            {
                // DEBUG //
                logger.Debug("Found task to retrieve a list of all mailbox sizes from the Exchange environment.");

                // Start up powershell
                powershell = new ExchPowershell();

                // Update the database so we can reflect we tried to run it right now
                getTask.LastRun = DateTime.Now;
                if (getTask.IsReoccurringTask) // Only update next run if it is reoccurring
                    getTask.NextRun = DateTime.Now.AddMinutes(getTask.TaskDelayInMinutes);
                else
                    getTask.NextRun = null;

                // Update the next run time in the database
                DbSql.Update_DatabaseTask(getTask);

                // Run the powershell command and get the results back
                List<MailboxUser> users = powershell.Get_MailboxSizes();

                // Loop through the users we found and 
                foreach (MailboxUser u in users)
                {
                    if (!string.IsNullOrEmpty(u.UserPrincipalName))
                    {
                        // Add to database
                        DbSql.Add_MailboxSizeStat(u);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Fatal("Failed to retrieve all mailbox sizes.", ex);
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Gets a list of database sizes and add to the SQL database
        /// </summary>
        /// <param name="getTask"></param>
        /// <param name="database"></param>
        private void ReadMailboxDatabaseSizes(DatabaseTask getTask)
        {
            ExchPowershell powershell = null;

            try
            {
                // DEBUG //
                logger.Debug("Found task to retrieve a list of all mailbox database sizes from the Exchange environment.");

                // Start up powershell
                powershell = new ExchPowershell();

                // Update the database so we can reflect we tried to run it right now
                getTask.LastRun = DateTime.Now;
                if (getTask.IsReoccurringTask) // Only update next run if it is reoccurring
                    getTask.NextRun = DateTime.Now.AddMinutes(getTask.TaskDelayInMinutes);
                else
                    getTask.NextRun = null;

                // Update the next run time in the database
                DbSql.Update_DatabaseTask(getTask);

                // Run the powershell command and get the results back
                List<MailboxDatabase> edb = powershell.Get_ExchangeDatabases();

                // Loop through the databases and add to SQL
                foreach (MailboxDatabase db in edb)
                {
                    try
                    {
                        // Add to database
                        DbSql.Add_MailboxDatabaseStat(db);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Failed to add mailbox database " + db.Identity + " to SQL.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
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
