using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public class BaseForwardingInfo
    {
        public string DisplayName { get; set; }
        public string DistinguishedName { get; set; }
        public string UserPrincipalName { get; set; }
        public string Classification { get; set; }
    }
}
