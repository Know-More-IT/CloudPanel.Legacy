using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class ForwardingObject
    {
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
        /// The distinguished name of the user object
        /// </summary>
        private string _distinguishedname;
        public string DistinguishedName
        {
            get { return _distinguishedname; }
            set
            {
                _distinguishedname = value;

                string dn = _distinguishedname.ToUpper();

                // Now set the canonical name
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
                if (domain.EndsWith("."))
                    domain = domain.Substring(0, domain.Length - 1);

                // Now finally set it
                _canonicalname = string.Format("{0}/{1}", domain, canonicalName);
            }
        }

        /// <summary>
        /// The Canonical Name of the user
        /// </summary>
        private string _canonicalname;
        public string CanonicalName
        {
            get { return _canonicalname; }
            set { _canonicalname = value; }
        }

        /// <summary>
        /// The type of object this is
        /// Can be User, Contact, or DistributionGroup
        /// </summary>
        private string _objecttype;
        public string ObjectType
        {
            get { return _objecttype; }
            set { _objecttype = value; }
        }
    }
}
