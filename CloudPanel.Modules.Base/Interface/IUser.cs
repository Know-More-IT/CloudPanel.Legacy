using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Interface
{
    public class IUser
    {
        /// <summary>
        /// The ID of the user in the SQL database
        /// </summary>
        private int _userid;
        public int UserID
        {
            get { return _userid; }
            set { _userid = value; }
        }

        /// <summary>
        /// The name of the company that the user belongs to
        /// </summary>
        private string _companyname;
        public string CompanyName
        {
            get { return _companyname; }
            set { _companyname = value; }
        }

        /// <summary>
        /// The company code that the company belongs to
        /// </summary>
        private string _companycode;
        public string CompanyCode
        {
            get { return _companycode; }
            set { _companycode = value; }
        }

        /// <summary>
        /// The name of the reseller that the user's company belongs to
        /// </summary>
        private string _resellername;
        public string ResellerName
        {
            get { return _resellername; }
            set { _resellername = value; }
        }

        /// <summary>
        /// The reseller's code that the user's company belongs to
        /// </summary>
        private string _resellercode;
        public string ResellerCode
        {
            get { return _resellercode; }
            set { _resellercode = value; }
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
        /// The Active Directory attribute 'Name' of the user
        /// </summary>
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The display name of the user
        /// </summary>
        private string _displayname;
        public string DisplayName
        {
            get { return _displayname; }
            set { _displayname = value; }
        }

        /// <summary>
        /// The first name of the user 
        /// </summary>
        private string _firstname;
        public string Firstname
        {
            get { return _firstname; }
            set { _firstname = value; }
        }

        /// <summary>
        /// The middle name of the user
        /// </summary>
        private string _middlename;
        public string Middlename
        {
            get { return _middlename; }
            set { _middlename = value; }
        }

        /// <summary>
        /// The last name of the user
        /// </summary>
        private string _lastname;
        public string Lastname
        {
            get { return _lastname; }
            set { _lastname = value; }
        }

        /// <summary>
        /// The UserPrincipalName of the user object
        /// </summary>
        private string _userprincipalname;
        public string UserPrincipalName
        {
            get { return _userprincipalname; }
            set { _userprincipalname = value; }
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

                string dn = _distinguishedname;

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
        /// The department the user belongs to
        /// </summary>
        private string _department;
        public string Department
        {
            get { return _department; }
            set { _department = value; }
        }

        /// <summary>
        /// Used to reset the password for the user
        /// </summary>
        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// Email address for the user
        /// </summary>
        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        /// <summary>
        /// The unique identifier of the user in Active Directory
        /// </summary>
        private Guid _userguid;
        public Guid UserGuid
        {
            get { return _userguid; }
            set { _userguid = value; }
        }

        /// <summary>
        /// When the user was created
        /// </summary>
        private DateTime? _created;
        public DateTime? Created
        {
            get { return _created; }
            set { _created = value; }
        }

        /// <summary>
        /// If the user has the ability to enable Exchange or not
        /// </summary>
        private bool _enableexchangepermission;
        public bool EnableExchangePermission
        {
            get { return _enableexchangepermission; }
            set { _enableexchangepermission = value; }
        }

        /// <summary>
        /// If the user has the ability to disable Exchange or not
        /// </summary>
        private bool _disableexchangepermission;
        public bool DisableExchangePermission
        {
            get { return _disableexchangepermission; }
            set { _disableexchangepermission = value; }
        }

        /// <summary>
        /// If the user has the ability to add a new domain or not
        /// </summary>
        private bool _adddomainpermission;
        public bool AddDomainPermission
        {
            get { return _adddomainpermission; }
            set { _adddomainpermission = value; }
        }

        /// <summary>
        /// If the user has the ability to delete a domain or not
        /// </summary>
        private bool _deletedomainpermission;
        public bool DeleteDomainPermission
        {
            get { return _deletedomainpermission; }
            set { _deletedomainpermission = value; }
        }

        /// <summary>
        /// If the user can enable or disable accepted domains for their company in Exchange
        /// </summary>
        private bool _modifyaccepteddomainpermission;
        public bool ModifyAcceptedDomainPermission
        {
            get { return _modifyaccepteddomainpermission; }
            set { _modifyaccepteddomainpermission = value; }
        }

        /// <summary>
        /// If the user can import other users or not
        /// </summary>
        private bool _importuserspermission;
        public bool ImportUsersPermission
        {
            get { return _importuserspermission; }
            set { _importuserspermission = value; }
        }

        /// <summary>
        /// If the user is enabled or not
        /// </summary>
        private bool _isenabled;
        public bool IsEnabled
        {
            get { return _isenabled; }
            set { _isenabled = value; }
        }

    }
}
