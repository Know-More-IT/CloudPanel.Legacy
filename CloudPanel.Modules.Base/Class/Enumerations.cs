using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class Enumerations
    {
        /// <summary>
        /// Type of connection
        /// </summary>
        public enum ConnectionType
        {
            Basic,
            Kerberos,
            SSL
        }

        /// <summary>
        /// Product version
        /// </summary>
        public enum ProductVersion
        {
            Exchange2010,
            Exchange2013,
            Lync2010,
            Lync2013,
            Lync2013Hosting
        }

        /// <summary>
        /// The type of task to run
        /// </summary>
        public enum TaskType
        {
            MailboxCalendarPermissions = 10,
            ReadMailboxSizes = 200,
            ReadMailboxDatabaseSizes = 300
        }

        /// <summary>
        /// If the task succeeded or not
        /// </summary>
        public enum TaskSuccess
        {
            Completed = 0,
            NotStarted = 1,
            Running = 2,
            Warning = 3,
            Failed = 4
        }

        /// <summary>
        /// The type of object it is (Exchange related)
        /// </summary>
        public enum ObjectType
        {
            User,
            Group,
            Contact
        }

        /// <summary>
        /// The type of log type it was
        /// </summary>
        public enum LogType
        {
            Verbose = 3,
            Error = 2,
            Info = 1
        }

        public enum Status
        {
            Error,
            FailedLogin,
            NotAuthenticated,
            InvalidApiLogin,
            NotFound,
            Success
        }
    }
}
