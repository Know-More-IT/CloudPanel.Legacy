using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public class BaseUser
    {
        public int ID { get; set; }
        public int MailboxPlan { get; set; }
        public int TSPlan { get; set; }
        public int LyncPlan { get; set; }
        public int ProhibitSendReceiveQuota { get; set; }
        public int ProhibitSendQuota { get; set; }
        public int WarningQuota { get; set; }
        public int AdditionalMB { get; set; }
        public int MailboxSizeUnformattedInMB { get; set; }

        public Guid UserGuid { get; set; }

        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string ResellerCode { get; set; }
        public string ResellerName { get; set; }
        public string sAMAccountName { get; set; }
        public string Name { get; set; }
        public string UserPrincipalName { get; set; }

        private string _distinguishedname;
        public string DistinguishedName 
        {
            get
            {
                return _distinguishedname;
            }
            set
            {
                _distinguishedname = value;
                CanonicalName = GetCanonicalName();
            }
        }

        public string DisplayName { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string MailboxPlanName { get; set; }
        public string MailboxSizeFormatted { get; set; }
        public string NewPassword { get; set; }
        public string ObjectType { get; set; }
        public string ForwardingAddress { get; set; }
        public string CanonicalName { get; set; }

        // Used to get the Exchange forwarding
        public string AltRecipient { get; set; }

        // Used to get the Exchange Activesync policy name
        public string ActiveSyncMailboxPolicyName { get; set; }

        // The language the mailbox is set to
        public string MailboxLanguage { get; set; }
        
        public string[] EmailAliases { get; set; }

        public bool IsCompanyAdmin { get; set; }
        public bool IsResellerAdmin { get; set; }
        public bool DeliverToAndForward { get; set; }
        public bool IsUserEnabled { get; set; }
        public bool PasswordNeverExpires { get; set; }
        public bool IsAccountLockedOut { get; set; }

        // Used to check a checkbox if the user contains a certain value or not
        public bool IsChecked { get; set; }

        public DateTime Created { get; set; }

        #region Exchange Mailbox Size Information

        public string MailboxDatabase { get; set; }
        public string TotalItemSizeInKB { get; set; }
        public string TotalDeletedItemSizeInKB { get; set; }

        public int ItemCount { get; set; }
        public int DeletedItemCount { get; set; }

        public DateTime Retrieved { get; set; }

        #endregion

        /// <summary>
        /// Gets the canonical name based on the distinguished name
        /// </summary>
        /// <returns></returns>
        private string GetCanonicalName()
        {
            string dn = DistinguishedName;

            string[] originalArray = dn.Split(',');
            string[] reversedArray = dn.Split(',');
            Array.Reverse(reversedArray);

            string canonicalName = string.Empty;
            foreach (string s in reversedArray)
            {
                if (s.StartsWith("CN="))
                    canonicalName += s.Replace("CN=", string.Empty) + "/";
                else if (s.StartsWith("OU="))
                    canonicalName += s.Replace("OU=", string.Empty) + "/";
            }

            // Remove the ending slash
            canonicalName = canonicalName.Substring(0, canonicalName.Length - 1);

            // Now our canonical name should be formatted except for the DC=
            // Lets do the DC= now
            string domain = string.Empty;
            foreach (string s in originalArray)
            {
                if (s.StartsWith("DC="))
                    domain += s.Replace("DC=", string.Empty) + ".";
            }

            // Remove the ending period
            domain = domain.Substring(0, domain.Length - 1);

            return string.Format("{0}/{1}", domain, canonicalName);
        }
    }
}
