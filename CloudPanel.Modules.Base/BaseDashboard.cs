using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public class BaseDashboard
    {
        public int UserCount { get; set; }
        public int MailboxCount { get; set; }
        public int CompanyCount { get; set; }
        public int CitrixCount { get; set; }
        public int DomainCount { get; set; }
        public int AcceptedDomainCount { get; set; }
    }
}
