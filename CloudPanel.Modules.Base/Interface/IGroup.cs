using CloudPanel.Modules.Base.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Interface
{
    public class IGroup
    {
        public enum GroupType
        {
            DomainLocal,
            Global,
            Universal
        }

        /// <summary>
        /// The distinguished name of the group
        /// </summary>
        private string _distinguishedname;
        public string DistinguishedName
        {
            get { return _distinguishedname; }
            set {
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
        /// The company code the group belongs to
        /// </summary>
        private string _companycode;
        public string CompanyCode
        {
            get { return _companycode; }
            set { _companycode = value; }
        }

        /// <summary>
        /// Display Name of the group
        /// </summary>
        private string _displayname;
        public string DisplayName
        {
            get { return _displayname; }
            set { _displayname = value; }
        }

        /// <summary>
        /// The 'Name' attribute in Active Directory for the group
        /// </summary>
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The SamAccountName of the group
        /// </summary>
        private string _samaccountname;
        public string SamAccountName
        {
            get { return _samaccountname; }
            set { _samaccountname = value; }
        }

        /// <summary>
        /// The description of the group
        /// </summary>
        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// The manager of the group
        /// </summary>
        private string[] _managedby;
        public string[] ManagedBy
        {
            get { return _managedby; }
            set { _managedby = value; }
        }

        /// <summary>
        /// Members of the group
        /// </summary>
        private List<MailObject> _members;
        public List<MailObject> Members
        {
            get { return _members; }
            set { _members = value; }
        }

        /// <summary>
        /// Members of the group but with just the identifier of the member and no other information
        /// </summary>
        private string[] _membersarray;
        public string[] MembersArray
        {
            get { return _membersarray; }
            set { _membersarray = value; }
        }

        /// <summary>
        /// Groups this group is a member of
        /// </summary>
        private List<object> _memberof;
        public List<object> MemberOf
        {
            get { return _memberof; }
            set { _memberof = value; }
        }

        /// <summary>
        /// The group scope/type
        /// </summary>
        private GroupType _groupscope;
        public GroupType GroupScope
        {
            get { return _groupscope; }
            set { _groupscope = value; }
        }
        
    }
}
