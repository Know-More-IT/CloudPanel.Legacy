using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public class BaseContacts
    {
        public string DistinguishedName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public bool Hidden { get; set; }
    }
}
