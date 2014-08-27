using CloudPanel.Modules.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class ADUser : IUser
    {
        /// <summary>
        /// If the user is a company administrator or not
        /// </summary>
        private bool _iscompanyadmin;
        public bool IsCompanyAdmin
        {
            get { return _iscompanyadmin; }
            set { _iscompanyadmin = value; }
        }

        /// <summary>
        /// If the user is a reseller admin or not
        /// </summary>
        private bool _isreselleradmin;
        public bool IsResellerAdmin
        {
            get { return _isreselleradmin; }
            set { _isreselleradmin = value; }
        }

        /// <summary>
        /// If the user is locked out or not
        /// </summary>
        private bool _islockedout;
        public bool IsLockedOut
        {
            get { return _islockedout; }
            set { _islockedout = value; }
        }

        /// <summary>
        /// If the password never expires or not
        /// </summary>
        private bool _passwordneverexpires;
        public bool PasswordNeverExpires
        {
            get { return _passwordneverexpires; }
            set { _passwordneverexpires = value; }
        }

        /// <summary>
        /// The ID of the mailbox plan
        /// </summary>
        private int _mailboxplanid;
        public int MailboxPlanID
        {
            get { return _mailboxplanid; }
            set { _mailboxplanid = value; }
        }
    }
}
