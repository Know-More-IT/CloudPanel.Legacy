using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public class BaseDistributionGroup
    {
        public int ID { get; set; }

        public string DistinguishedName { get; set; }
        public string CompanyCode { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public string JoinRestriction { get; set; }
        public string DepartRestriction { get; set; }
        public string SendModerationNotifications { get; set; }

        public string[] ManagedBy { get; set; }
        public string[] RestrictWhoCanSend { get; set; }
        public string[] GroupModerators { get; set; }
        public string[] SendersNotRequiringApproval { get; set; }

        public List<BaseUser> Members { get; set; }

        public bool Hidden { get; set; }
        public bool RequireSenderAuthentication { get; set; }
        public bool ModerationEnabled { get; set; }
    }
}
