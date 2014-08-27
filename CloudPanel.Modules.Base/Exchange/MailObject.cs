using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class MailObject
    {
        /// <summary>
        /// Company code the mail object belongs to
        /// </summary>
        private string _companycode;
        public string CompanyCode
        {
            get { return _companycode; }
            set { _companycode = value; }
        }

        /// <summary>
        /// Display name of the object
        /// </summary>
        private string _displayname;
        public string DisplayName
        {
            get { return _displayname; }
            set { _displayname = value; }
        }

        /// <summary>
        /// The distinguished name of the object
        /// </summary>
        private string _distinguishedname;
        public string DistinguishedName
        {
            get { return _distinguishedname; }
            set { _distinguishedname = value; }
        }

        /// <summary>
        /// Image url of the object
        /// </summary>
        private string _imageurl;
        public string ImageUrl
        {
            get { return _imageurl; }
            set { _imageurl = value; }
        }

        /// <summary>
        /// The email address of the object
        /// </summary>
        private string _emailaddress;
        public string EmailAddress
        {
            get { return _emailaddress; }
            set { _emailaddress = value; }
        }

        /// <summary>
        /// The type of object
        /// </summary>
        private Enumerations.ObjectType _objecttype;
        public Enumerations.ObjectType ObjectType
        {
            get { return _objecttype; }
            set { _objecttype = value; }
        }

    }
}
