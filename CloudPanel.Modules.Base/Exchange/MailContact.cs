using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class MailContact
    {
        /// <summary>
        /// Distinguished name of the contact
        /// </summary>
        private string _distinguishedname;
        public string DistinguishedName
        {
            get { return _distinguishedname; }
            set { _distinguishedname = value; }
        }

        /// <summary>
        /// The display name of the contact
        /// </summary>
        private string _displayname;
        public string DisplayName
        {
            get { return _displayname; }
            set { _displayname = value; }
        }

        /// <summary>
        /// The external email address of the contact
        /// </summary>
        private string _externalemailaddress;
        public string ExternalEmailAddress
        {
            get 
            { 
                return _externalemailaddress.Replace("SMTP:", string.Empty); 
            }
            set { _externalemailaddress = value; }
        }

        /// <summary>
        /// The primary email address of the contact
        /// </summary>
        private string _primarysmtpaddress;
        public string PrimarySmtpAddress
        {
            get { return _primarysmtpaddress; }
            set { _primarysmtpaddress = value; }
        }

        /// <summary>
        /// The company code of the contact
        /// </summary>
        private string _companycode;
        public string CompanyCode
        {
            get { return _companycode; }
            set { _companycode = value; }
        }

        /// <summary>
        /// If the contact is hidden or not
        /// </summary>
        private bool _hidden;
        public bool Hidden
        {
            get { return _hidden; }
            set { _hidden = value; }
        }

        /// <summary>
        /// When the contact was created
        /// </summary>
        private DateTime _whencreated;
        public DateTime WhenCreated
        {
            get { return _whencreated; }
            set { _whencreated = value; }
        }

    }
}
