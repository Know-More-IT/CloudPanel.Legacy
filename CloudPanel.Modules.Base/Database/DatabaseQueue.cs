using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class DatabaseQueue
    {
        /// <summary>
        /// The ID of the queue
        /// </summary>
        private int _queueid;
        public int QueueID
        {
            get { return _queueid; }
            set { _queueid = value; }
        }

        /// <summary>
        /// How long to delay the task before executing it
        /// </summary>
        private int _taskdelayinminutes;
        public int TaskDelayInMinutes
        {
            get { return _taskdelayinminutes; }
            set { _taskdelayinminutes = value; }
        }

        /// <summary>
        /// The status of the task
        /// </summary>
        private Enumerations.TaskSuccess _tasksuccess;
        public Enumerations.TaskSuccess TaskSuccess
        {
            get { return _tasksuccess; }
            set { _tasksuccess = value; }
        }

        /// <summary>
        /// The task assigned to the queue
        /// </summary>
        private Enumerations.TaskType _tasktype;
        public Enumerations.TaskType TaskType
        {
            get { return _tasktype; }
            set { _tasktype = value; }
        }

        /// <summary>
        /// The userprincipalname assigned to the queue
        /// </summary>
        private string _userprincipalname;
        public string UserPrincipalName
        {
            get { return _userprincipalname; }
            set { _userprincipalname = value; }
        }

        /// <summary>
        /// The company code for the queue
        /// </summary>
        private string _companycode;
        public string CompanyCode
        {
            get { return _companycode; }
            set { _companycode = value; }
        }

        /// <summary>
        /// The output of the task
        /// </summary>
        private string _taskoutput;
        public string TaskOutput
        {
            get { return _taskoutput; }
            set { _taskoutput = value; }
        }

        /// <summary>
        /// When the task was created
        /// </summary>
        private DateTime _taskcreated;
        public DateTime TaskCreated
        {
            get { return _taskcreated; }
            set { _taskcreated = value; }
        }

        /// <summary>
        /// When the task was completed
        /// </summary>
        private DateTime _taskcompleted;
        public DateTime TaskCompleted
        {
            get { return _taskcompleted; }
            set { _taskcompleted = value; }
        }
    }
}
