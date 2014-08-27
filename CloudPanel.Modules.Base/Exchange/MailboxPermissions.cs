using CloudPanel.Modules.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class MailboxPermissions
    {
        /// <summary>
        /// Access rights for the mailbox
        /// </summary>
        private string[] _accessrights;
        public string[] AccessRights
        {
            get { return _accessrights; }
            set { _accessrights = value; }
        }

        /// <summary>
        /// The type of inheritance for the permission
        /// </summary>
        private string _inheritancetype;
        public string InheritanceType
        {
            get { return _inheritancetype; }
            set { _inheritancetype = value; }
        }

        /// <summary>
        /// The sAMAccountName of the user
        /// </summary>
        private string _samaccountname;
        public string SamAccountName
        {
            get { return _samaccountname; }
            set { _samaccountname = value; }
        }

        /// <summary>
        /// If the permission is deny permission or not
        /// </summary>
        private bool _deny;
        public bool Deny
        {
            get { return _deny; }
            set { _deny = value; }
        }

        /// <summary>
        /// If the permission is inherited or not
        /// </summary>
        private bool _isinherited;
        public bool IsInherited
        {
            get { return _isinherited; }
            set { _isinherited = value; }
        }

        private bool _add;
        public bool Add
        {
            get { return _add; }
            set { _add = value; }
        }
    }
}
