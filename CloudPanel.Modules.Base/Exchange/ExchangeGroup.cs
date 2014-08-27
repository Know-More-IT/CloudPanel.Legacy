using CloudPanel.Modules.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class ExchangeGroup : IGroup
    {
        /// <summary>
        /// The SQL ID of the distribution group
        /// </summary>
        private int _sqlid;
        public int SQLID
        {
            get { return _sqlid; }
            set { _sqlid = value; }
        }

        /// <summary>
        /// The primary SMTP address of the group
        /// </summary>
        private string _primarysmtpaddress;
        public string PrimarySmtpAddress
        {
            get { return _primarysmtpaddress; }
            set { _primarysmtpaddress = value; }
        }

        /// <summary>
        /// The type of join restriction in place
        /// </summary>
        private string _joinrestriction;
        public string JoinRestriction
        {
            get { return _joinrestriction; }
            set { _joinrestriction = value; }
        }

        /// <summary>
        /// The type of depart restriction in place
        /// </summary>
        private string _departrestriction;
        public string DepartRestriction
        {
            get { return _departrestriction; }
            set { _departrestriction = value; }
        }

        /// <summary>
        /// The type of send moderation notification that is in place
        /// </summary>
        private string _sendmoderationnotifications;
        public string SendModerationNotifications
        {
            get { return _sendmoderationnotifications; }
            set { _sendmoderationnotifications = value; }
        }

        /// <summary>
        /// List of users who can send to the distributino group
        /// </summary>
        private string[] _whocansendtogroup;
        public string[] WhoCanSendToGroup
        {
            get { return _whocansendtogroup; }
            set { _whocansendtogroup = value; }
        }

        /// <summary>
        /// List of group moderators
        /// </summary>
        private string[] _groupmoderators;
        public string[] GroupModerators
        {
            get { return _groupmoderators; }
            set { _groupmoderators = value; }
        }

        /// <summary>
        /// List of senders that do not require approval when sending to this group
        /// </summary>
        private string[] _sendersnotrequiringapproval;
        public string[] SendersNotRequiringApproval
        {
            get { return _sendersnotrequiringapproval; }
            set { _sendersnotrequiringapproval = value; }
        }


        /// <summary>
        /// Any email aliases for the distribution group
        /// </summary>
        private List<string> _emailaliases;
        public List<string> EmailAliases
        {
            get { return _emailaliases; }
            set { _emailaliases = value; }
        }

        /// <summary>
        /// If the manager can update the membership list or not
        /// </summary>
        private bool _managercanupdatelist;
        public bool ManagerCanUpdateList
        {
            get { return _managercanupdatelist; }
            set { _managercanupdatelist = value; }
        }

        /// <summary>
        /// If the group is hidden or not
        /// </summary>
        private bool _hidden;
        public bool Hidden
        {
            get { return _hidden; }
            set { _hidden = value; }
        }

        /// <summary>
        /// If authentication is required to send to this group
        /// </summary>
        private bool _requiresenderauthentication;
        public bool RequireSenderAuthentication
        {
            get { return _requiresenderauthentication; }
            set { _requiresenderauthentication = value; }
        }

        /// <summary>
        /// If moderation is enabled or not
        /// </summary>
        private bool _moderationenabled;
        public bool ModerationEnabled
        {
            get { return _moderationenabled; }
            set { _moderationenabled = value; }
        }
    }
}
