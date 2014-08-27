using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudPanel.classes
{
    public class RepeaterOwnership
    {
        /// <summary>
        /// The image of the type of object it is
        /// </summary>
        private string _imageurl;
        public string ImageUrl
        {
            get { return _imageurl; }
            set { _imageurl = value; }
        }

        /// <summary>
        /// The display name of the object
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
        /// The email address of the object if it has one
        /// </summary>
        private string _primarysmtpaddress;
        public string PrimarySmtpAddress
        {
            get { return _primarysmtpaddress; }
            set { _primarysmtpaddress = value; }
        }

        /// <summary>
        /// The type of object it is
        /// </summary>
        private string _objecttype;
        public string ObjectType
        {
            get { return _objecttype; }
            set { _objecttype = value; }
        }
    }
}