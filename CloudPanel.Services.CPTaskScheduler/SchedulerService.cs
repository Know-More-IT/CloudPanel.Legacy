using CloudPanel.Services.CPScheduler.Class;
using CloudPanel.Services.Scheduler.SQL;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace CloudPanel.Services.Scheduler
{
    public partial class SchedulerService : ServiceBase
    {
        // Log utility
        private static readonly ILog logger = LogManager.GetLogger(typeof(SchedulerService));

        // Our thread objects
        Thread taskThread = null;
        Thread queueThread = null;

        // Our class objects
        TaskRunner tasks = null;
        QueueRunner queues = null;

        public SchedulerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                // Get settings from database
                SchedulerRetrieve.GetSettings();

                int taskQueryInMin = int.Parse(ConfigurationManager.AppSettings["TaskQueryInMin"]);
                int queueQueryInMin = int.Parse(ConfigurationManager.AppSettings["QueueQueryInMin"]);

                // Check that the task and queue query is equal to or greater then 5 minutes
                if ((taskQueryInMin + queueQueryInMin) < 10)
                    throw new Exception("You can only query for new queues and tasks every 5 minutes or greater. Please modify each value to be 5 minutes or greater.");

                tasks = new TaskRunner(taskQueryInMin);
                queues = new QueueRunner(queueQueryInMin);

                // Run the tasks on a new thread
                taskThread = new Thread(tasks.QueryDatabase);
                queueThread = new Thread(queues.QueryDatabase);

                // Start the threads
                logger.Debug("Starting thread to watch for new tasks");
                taskThread.Start();

                logger.Debug("Starting thread to watch for queued items in the database");
                queueThread.Start();
            }
            catch (Exception ex)
            {
                // Log the error
                logger.Fatal("There was an error starting the service.", ex);

                // Stop the service
                Stop();
            }
        }

        /// <summary>
        /// Stops the service
        /// </summary>
        protected override void OnStop()
        {
            // INFO //
            logger.Info("Received the signal to stop the service...");

            try
            {
                if (tasks != null)
                {
                    // DEBUG //
                    logger.Debug("Stopping the tasks thread...");
                    tasks.Stop();
                }

                if (queues != null)
                {
                    // DEBUG //
                    logger.Debug("Stopping the queue thread...");
                    queues.Stop();
                }
            }
            catch (Exception ex)
            {
                logger.Fatal("Error stopping processes", ex);
            }
        }
    }
}
