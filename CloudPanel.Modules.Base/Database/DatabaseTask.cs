using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class DatabaseTask
    {
        /// <summary>
        /// The ID of the task in the database
        /// </summary>
        private int _taskid;
        public int TaskID
        {
            get { return _taskid; }
            set { _taskid = value; }
        }

        /// <summary>
        /// The type of task it is
        /// </summary>
        private Enumerations.TaskType _tasktype;
        public Enumerations.TaskType TaskType
        {
            get { return _tasktype; }
            set { _tasktype = value; }
        }

        /// <summary>
        /// The last time the task was ran
        /// </summary>
        private DateTime _lastrun;
        public DateTime LastRun
        {
            get { return _lastrun; }
            set { _lastrun = value; }
        }

        /// <summary>
        /// When the task should run again
        /// </summary>
        private DateTime? _nextrun;
        public DateTime? NextRun
        {
            get { return _nextrun; }
            set { _nextrun = value; }
        }

        /// <summary>
        /// When the task was created
        /// </summary>
        private DateTime _whencreated;
        public DateTime WhenCreated
        {
            get { return _whencreated; }
            set { _whencreated = value; }
        }

        /// <summary>
        /// Output from the task
        /// </summary>
        private string _taskoutput;
        public string TaskOutput
        {
            get { return _taskoutput; }
            set { _taskoutput = value; }
        }

        /// <summary>
        /// How long to delay the task in minutes
        /// </summary>
        private int _taskdelayinminutes;
        public int TaskDelayInMinutes
        {
            get { return _taskdelayinminutes; }
            set { _taskdelayinminutes = value; }
        }

        /// <summary>
        /// If the task is reoccurring or just one time
        /// </summary>
        private bool _isreoccurringtask;
        public bool IsReoccurringTask
        {
            get { return _isreoccurringtask; }
            set { _isreoccurringtask = value; }
        }

    }
}
