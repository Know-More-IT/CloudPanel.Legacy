using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public class BaseTasks
    {
        public static readonly int TaskReadMailboxSizes = 200;
        public static readonly int TaskReadMailboxDatabaseSizes = 300;

        public int TaskType { get; set; }
        public int StartAgainInXDays { get; set; }
        public int TaskStatus { get; set; }

        public bool Reoccurring { get; set; }

        public DateTime NextRun { get; set; }
        public DateTime LastRun { get; set; }
        public DateTime Created { get; set; }

        public string TaskOutput { get; set; }
    }
}
