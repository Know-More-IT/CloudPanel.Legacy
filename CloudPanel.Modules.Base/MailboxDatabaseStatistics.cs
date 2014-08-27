using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public class BaseMailboxDatabaseStatistics
    {
        public string Server { get; set; }
        public string Name { get; set; }
        public string DatabaseSize { get; set; }

        public DateTime Retrieved { get; set; }
    }
}
