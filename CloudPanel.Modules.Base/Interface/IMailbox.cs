using CloudPanel.Modules.Base.Class;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Interface
{
    public class IMailbox : IUser
    {
        /// <summary>
        /// The primary email address of the mailbox
        /// </summary>
        private string _primarysmtpaddress;
        public string PrimarySmtpAddress
        {
            get { return _primarysmtpaddress; }
            set { _primarysmtpaddress = value; }
        }

        /// <summary>
        /// The name of the ActiveSync policy for the mailbox
        /// </summary>
        private string _activesyncpolicy;
        public string ActivesyncPolicy
        {
            get { return _activesyncpolicy; }
            set { _activesyncpolicy = value; }
        }

        /// <summary>
        /// The name of the retention policy for the mailbox
        /// </summary>
        private string _retentionpolicy;
        public string RetentionPolicy
        {
            get { return _retentionpolicy; }
            set { _retentionpolicy = value; }
        }

        /// <summary>
        /// The name of the database for the mailbox
        /// </summary>
        private string _database;
        public string Database
        {
            get { return _database; }
            set { _database = value; }
        }

        /// <summary>
        /// The name of the mailbox plan the mailbox is associated with
        /// </summary>
        private string _mailboxplanname;
        public string MailboxPlanName
        {
            get { return _mailboxplanname; }
            set { _mailboxplanname = value; }
        }

        /// <summary>
        /// The name of the ActiveSync policy assigned to the user
        /// </summary>
        private string _activesyncmailboxpolicy;
        public string ActiveSyncMailboxPolicy
        {
            get { return _activesyncmailboxpolicy; }
            set { _activesyncmailboxpolicy = value; }
        }

        /// <summary>
        /// The name of the OWA mailbox policy
        /// </summary>
        private string _owamailboxpolicy;
        public string OWAMailboxPolicy
        {
            get { return _owamailboxpolicy; }
            set { _owamailboxpolicy = value; }
        }

        /// <summary>
        /// If the mailbox is hidden from the GAL or not
        /// </summary>
        private bool _hidden;
        public bool IsHidden
        {
            get { return _hidden; }
            set { _hidden = value; }
        }
        
        /// <summary>
        /// Mailbox plan ID from the database that this mailbox is assigned
        /// </summary>
        private int _mailboxplanid;
        public int MailboxPlanID
        {
            get { return _mailboxplanid; }
            set { _mailboxplanid = value; }
        }

        /// <summary>
        /// Additional MB added to the plan
        /// </summary>
        private int _additionalmb;
        public int AdditionalMB
        {
            get { return _additionalmb; }
            set { _additionalmb = value; }
        }

        /// <summary>
        /// Current size limit of the mailbox according to the plan
        /// </summary>
        private int _mailboxsizeinmb;
        public int MailboxSizeInMB
        {
            get { return _mailboxsizeinmb; }
            set { _mailboxsizeinmb = value; }
        }

        /// <summary>
        /// The recipient limit (how many emails a person can send to in an email)
        /// </summary>
        private int _recipientlimit;
        public int RecipientLimit
        {
            get { return _recipientlimit; }
            set { _recipientlimit = value; }
        }

        /// <summary>
        /// How long deleted items are retained (in days)
        /// </summary>
        private int _retaindeleteditemsindays;
        public int RetainDeletedItemsInDays
        {
            get { return _retaindeleteditemsindays; }
            set { _retaindeleteditemsindays = value; }
        }

        /// <summary>
        /// Formats the size in readable format
        /// </summary>
        public string CurrentMailboxSizeFormatted
        {
            get
            {
                if (!string.IsNullOrEmpty(_totalitemsizeinkb))
                    return FormatKilobytes(_totalitemsizeinkb);
                else
                    return "Unknown";
            }
        }

        /// <summary>
        /// Formats the max mailbox size in readable format according to the plan
        /// </summary>
        public string MailboxSizeFormatted
        {
            get
            {
                string mailboxSizeInString = ((_mailboxsizeinmb + _additionalmb) * 1024).ToString();
                return FormatKilobytes(mailboxSizeInString);
            }
        }

        /// <summary>
        /// The total item size in kilobytes
        /// </summary>
        private string _totalitemsizeinkb;
        public string TotalItemSizeInKB
        {
            get { return _totalitemsizeinkb; }
            set { _totalitemsizeinkb = value; }
        }

        /// <summary>
        /// The canonical name of where the mailbox is forwarding
        /// </summary>
        private string _forwardingaddress;
        public string ForwardingAddress
        {
            get { return _forwardingaddress; }
            set { _forwardingaddress = value; }
        }

        /// <summary>
        /// The alias of the mailbox
        /// </summary>
        private string _alias;
        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }

        /// <summary>
        /// The type of mailbox (etc. SharedMailbox)
        /// </summary>
        private string _recipienttypedetails;
        public string RecipientTypeDetails
        {
            get { return _recipienttypedetails; }
            set { _recipienttypedetails = value; }
        }

        /// <summary>
        /// Email Aliases for the mailbox with the SMTP: and smtp: removed
        /// </summary>
        private List<string> _emailaliases;
        public List<string> EmailAliases
        {
            get { return _emailaliases; }
            set
            {
                // Original aliases
                List<string> aliases = value;

                // New list after formatting
                List<string> newAliases = new List<string>();

                foreach (string a in aliases)
                {
                    if (!a.StartsWith("SMTP:"))
                        newAliases.Add(a.Replace("smtp:", string.Empty));
                }

                _emailaliases = newAliases;
            }
        }

        /// <summary>
        /// If the mailbox is delivering to mailbox AND forwarding
        /// </summary>
        private bool _delivertomailboxandforward;
        public bool DeliverToMailboxAndForward
        {
            get { return _delivertomailboxandforward; }
            set { _delivertomailboxandforward = value; }
        }

        /// <summary>
        /// If ActiveSync is enabled for the mailbox
        /// </summary>
        private bool _activesyncenabled;
        public bool ActiveSyncEnabled
        {
            get { return _activesyncenabled; }
            set { _activesyncenabled = value; }
        }

        /// <summary>
        /// If ECP is enabled or not
        /// </summary>
        private bool _ecpenabled;
        public bool ECPEnabled
        {
            get { return _ecpenabled; }
            set { _ecpenabled = value; }
        }

        /// <summary>
        /// If IMAP is enabled or not
        /// </summary>
        private bool _imapenabled;
        public bool IMAPEnabled
        {
            get { return _imapenabled; }
            set { _imapenabled = value; }
        }

        /// <summary>
        /// If MAPI is enabled or not
        /// </summary>
        private bool _mapienabled;
        public bool MAPIEnabled
        {
            get { return _mapienabled; }
            set { _mapienabled = value; }
        }

        /// <summary>
        /// If the full version of OWA is enabled or not
        /// </summary>
        private bool _owaenabled;
        public bool OWAEnabled
        {
            get { return _owaenabled; }
            set { _owaenabled = value; }
        }

        /// <summary>
        /// If POP3 is enabled or not
        /// </summary>
        private bool _pop3enabled;
        public bool POP3Enabled
        {
            get { return _pop3enabled; }
            set { _pop3enabled = value; }
        }

        /// <summary>
        /// Mailbox permissions for the mailbox
        /// </summary>
        private List<MailboxPermissions> _fullaccesspermissions;
        public List<MailboxPermissions> FullAccessPermissions
        {
            get { return _fullaccesspermissions; }
            set { _fullaccesspermissions = value; }
        }

        /// <summary>
        /// Mailbox permissions for the mailbox
        /// </summary>
        private List<MailboxPermissions> _sendaspermissions;
        public List<MailboxPermissions> SendAsPermissions
        {
            get { return _sendaspermissions; }
            set { _sendaspermissions = value; }
        }

        /// <summary>
        /// How long to keep deleted items for
        /// </summary>
        private TimeSpan _keepdeleteditemsfor;
        public TimeSpan KeepDeletedItemsFor
        {
            get { return _keepdeleteditemsfor; }
            set { _keepdeleteditemsfor = value; }
        }

        /// <summary>
        /// Formats kilobytes to the correct size
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string FormatKilobytes(string kilobytes)
        {
            // Convert to decimal
            decimal _convertedsize = decimal.Parse(kilobytes, CultureInfo.InvariantCulture);

            // Set our values
            const long KB = 1;
            const long MB = 1024 * KB;
            const long GB = 1024 * MB;
            const long TB = 1024 * GB;

            if (_convertedsize > TB)
                return (_convertedsize / TB).ToString("0.00", CultureInfo.InvariantCulture) + " TB";
            else if (_convertedsize > GB)
                return (_convertedsize / GB).ToString("0.00", CultureInfo.InvariantCulture) + " GB";
            else if (_convertedsize > MB)
                return (_convertedsize / MB).ToString("0.00", CultureInfo.InvariantCulture) + " MB";
            else
                return (_convertedsize).ToString("0.00", CultureInfo.InvariantCulture) + " KB";
        }

        /// <summary>
        /// Formats the exchange size from 434.00GB (23,23,34,234 bytes) to just kilobytes
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string FormatExchangeSize(string size)
        {
            if (string.IsNullOrEmpty(size))
                return "0";
            else
            {
                string newSize = size;

                string[] stringSeparators = new string[] { "TB (", "GB (", "MB (", "KB (", "B (" };

                if (newSize.Contains("TB ("))
                    newSize = ConvertToKB(decimal.Parse(newSize.Split(stringSeparators, StringSplitOptions.None)[0].Trim(), CultureInfo.InvariantCulture), "TB");
                else if (newSize.Contains("GB ("))
                    newSize = ConvertToKB(decimal.Parse(newSize.Split(stringSeparators, StringSplitOptions.None)[0].Trim(), CultureInfo.InvariantCulture), "GB");
                else if (newSize.Contains("MB ("))
                    newSize = ConvertToKB(decimal.Parse(newSize.Split(stringSeparators, StringSplitOptions.None)[0].Trim(), CultureInfo.InvariantCulture), "MB");
                else if (newSize.Contains("KB ("))
                    newSize = ConvertToKB(decimal.Parse(newSize.Split(stringSeparators, StringSplitOptions.None)[0].Trim(), CultureInfo.InvariantCulture), "KB");
                else if (newSize.Contains("B ("))
                    newSize = ConvertToKB(decimal.Parse(newSize.Split(stringSeparators, StringSplitOptions.None)[0].Trim(), CultureInfo.InvariantCulture), "B");

                return newSize.Trim();
            }
        }

        /// <summary>
        /// Converts the size from TB,GB,MB to KB
        /// </summary>
        /// <param name="size"></param>
        /// <param name="sizeType"></param>
        /// <returns></returns>
        public string ConvertToKB(decimal size, string sizeType)
        {
            decimal newSize = 0;

            switch (sizeType)
            {
                case "TB":
                    newSize = size * 1024 * 1024 * 1024;
                    break;
                case "GB":
                    newSize = size * 1024 * 1024;
                    break;
                case "MB":
                    newSize = size * 1024;
                    break;
                default:
                    newSize = size;
                    break;
            }

            return newSize.ToString(CultureInfo.InvariantCulture);
        }
    }
}
