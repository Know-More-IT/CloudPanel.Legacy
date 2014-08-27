using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class Company
    {
        /// <summary>
        /// Object guid for the company (from the OU)
        /// </summary>
        private Guid _objectguid;
        public Guid ObjectGuid
        {
            get { return _objectguid; }
            set { _objectguid = value; }
        }

        /// <summary>
        /// The SQL ID of the company
        /// </summary>
        private int _companyid;
        public int CompanyID
        {
            get { return _companyid; }
            set { _companyid = value; }
        }

        /// <summary>
        /// The plan set on the organization
        /// </summary>
        private int _orgplanid;
        public int OrgPlanID
        {
            get { return _orgplanid; }
            set { _orgplanid = value; }
        }

        /// <summary>
        /// The public folder plan assigned
        /// </summary>
        private int _exchpfplan;
        public int ExchPublicFolderPlan
        {
            get { return _exchpfplan; }
            set { _exchpfplan = value; }
        }

        /// <summary>
        /// Additional value for whatever is needed
        /// </summary>
        private int _additionalvalue1;
        public int AdditionalValue1
        {
            get { return _additionalvalue1; }
            set { _additionalvalue1 = value; }
        }

        /// <summary>
        /// The reseller code that the company belongs to
        /// </summary>
        private string _resellercode;
        public string ResellerCode
        {
            get { return _resellercode; }
            set { _resellercode = value; }
        }

        /// <summary>
        /// The company's name
        /// </summary>
        private string _companyname;
        public string CompanyName
        {
            get { return _companyname; }
            set { _companyname = value; }
        }

        /// <summary>
        /// The company's company code
        /// </summary>
        private string _companycode;
        public string CompanyCode
        {
            get { return _companycode; }
            set { _companycode = value; }
        }

        /// <summary>
        /// Street address of the company
        /// </summary>
        private string _street;
        public string Street
        {
            get { return _street; }
            set { _street = value; }
        }

        /// <summary>
        /// City of the company
        /// </summary>
        private string _city;
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        /// <summary>
        /// State of the company
        /// </summary>
        private string _state;
        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// The country that the company is in
        /// </summary>
        private string _country;
        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        /// <summary>
        /// The phone number for the company
        /// </summary>
        private string _phonenumber;
        public string PhoneNumber
        {
            get { return _phonenumber; }
            set { _phonenumber = value; }
        }

        /// <summary>
        /// Website of the company
        /// </summary>
        private string _website;
        public string Website
        {
            get { return _website; }
            set { _website = value; }
        }

        /// <summary>
        /// Any description or notes
        /// </summary>
        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        
        /// <summary>
        /// The administrators name
        /// </summary>
        private string _adminname;
        public string AdministratorsName
        {
            get { return _adminname; }
            set { _adminname = value; }
        }

        /// <summary>
        /// The administrator's email
        /// </summary>
        private string _adminemail;
        public string AdministratorsEmail
        {
            get { return _adminemail; }
            set { _adminemail = value; }
        }

        /// <summary>
        /// The zip code of the comapny
        /// </summary>
        private string _zipcode;
        public string ZipCode
        {
            get { return _zipcode; }
            set { _zipcode = value; }
        }

        /// <summary>
        /// The distinguished name of the company
        /// </summary>
        private string _distinguishedname;
        public string DistinguishedName
        {
            get { return _distinguishedname; }
            set { _distinguishedname = value; }
        }

        /// <summary>
        /// The domains for the company
        /// </summary>
        private string[] _upnsuffixes;
        public string[] UPNSuffixes
        {
            get { return _upnsuffixes; }
            set { _upnsuffixes = value; }
        }

        /// <summary>
        /// When the company was created
        /// </summary>
        private DateTime _whencreated;
        public DateTime WhenCreated
        {
            get { return _whencreated; }
            set { _whencreated = value; }
        }

        /// <summary>
        /// If this company is a reseller or not
        /// </summary>
        private bool _isreseller;
        public bool IsReseller
        {
            get { return _isreseller; }
            set { _isreseller = value; }
        }

        /// <summary>
        /// If Exchange is enabled or not
        /// </summary>
        private bool _exchangeenabled;
        public bool ExchangeEnabled
        {
            get { return _exchangeenabled; }
            set { _exchangeenabled = value; }
        }

        /// <summary>
        /// If Lync is enabled or not
        /// </summary>
        private bool _lyncenabled;
        public bool LyncEnabled
        {
            get { return _lyncenabled; }
            set { _lyncenabled = value; }
        }

        /// <summary>
        /// If Citrix is enabled or not
        /// </summary>
        private bool _citrixenabled;
        public bool CitrixEnabled
        {
            get { return _citrixenabled; }
            set { _citrixenabled = value; }
        }

        /// <summary>
        /// If the Exchange permissions have been fixed
        /// </summary>
        private bool _exchpermissionsfixed;
        public bool ExchangePermissionsFixed
        {
            get { return _exchpermissionsfixed; }
            set { _exchpermissionsfixed = value; }
        }

        /// <summary>
        /// List of domains for the company
        /// </summary>
        private List<string> _domains;
        public List<string> Domains
        {
            get { return _domains; }
            set { _domains = value; }
        }
    }
}
