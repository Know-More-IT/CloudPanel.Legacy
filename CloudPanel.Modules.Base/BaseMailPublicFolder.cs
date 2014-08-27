using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public class BaseMailPublicFolder
    {

        /// <summary>
        /// The company code from the custom attribute
        /// </summary>
        private string _customattribute1;
        public string CompanyCode
        {
            get { return _customattribute1; }
            set { _customattribute1 = value; }
        }

        /// <summary>
        /// Display name of the mail enabled public folder
        /// </summary>
        private string _displayname;
        public string DisplayName
        {
            get { return _displayname; }
            set { _displayname = value; }
        }

        /// <summary>
        /// Email address of the public folder
        /// </summary>
        private string _emailaddress;
        public string EmailAddress
        {
            get { return _emailaddress; }
            set { _emailaddress = value; }
        }

        /// <summary>
        /// If it is hidden from the address list
        /// </summary>
        private bool _hidden;
        public bool Hidden
        {
            get { return _hidden; }
            set { _hidden = value; }
        }

    }
}
