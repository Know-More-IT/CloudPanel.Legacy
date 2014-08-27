using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public class BaseMailbox
    {
        public string DisplayName { get; set; }
        public string PrimarySmtpAddress { get; set; }
        public string ForwardingAddress { get; set; }
        public string ForwardingSmtpAddress { get; set; }

        public bool HiddenFromAddressListsEnabled { get; set; }
        public bool DeliverToMailboxAndForward { get; set; }

        public string[] EmailAddresses { get; set; }

        public decimal ProhibitSendReceiveQuota { get; set; }
    }
}
