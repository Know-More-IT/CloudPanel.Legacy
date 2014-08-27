using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class Domain
    {
        /// <summary>
        /// The SQL ID of the domain
        /// </summary>
        private int _domainid;
        public int DomainID
        {
            get { return _domainid; }
            set { _domainid = value; }
        }

        /// <summary>
        /// The company code that the domain belongs to
        /// </summary>
        private string _companycode;
        public string CompanyCode
        {
            get { return _companycode; }
            set { _companycode = value; }
        }

        /// <summary>
        /// The domain name 
        /// </summary>
        private string _domainname;
        public string DomainName
        {
            get { return _domainname; }
            set { _domainname = value; }
        }

        /// <summary>
        /// If the domain is a sub domain (Not used currently)
        /// </summary>
        private bool _issubdomain;
        public bool IsSubDomain
        {
            get { return _issubdomain; }
            set { _issubdomain = value; }
        }

        /// <summary>
        /// If this is the default domain (Not used currently)
        /// </summary>
        private bool _isdefault;
        public bool IsDefault
        {
            get { return _isdefault; }
            set { _isdefault = value; }
        }

        /// <summary>
        /// If this domain is used for Exchange as an accepted domain
        /// </summary>
        private bool _isaccepteddomain;
        public bool IsAcceptedDomain
        {
            get { return _isaccepteddomain; }
            set { _isaccepteddomain = value; }
        }

        /// <summary>
        /// If this is used for Lync 
        /// </summary>
        private bool _islyncdomain;
        public bool IsLyncDomain
        {
            get { return _islyncdomain; }
            set { _islyncdomain = value; }
        }

        /// <summary>
        /// How many users, groups, contacts, etc that are using this domain
        /// </summary>
        private int _usercount;
        public int UserCount
        {
            get { return _usercount; }
            set { _usercount = value; }
        }

        /// <summary>
        /// Total objects (contacts, groups, etc) that are using this domain
        /// </summary>
        private int _objectcount;
        public int ObjectCount
        {
            get { return _objectcount; }
            set { _objectcount = value; }
        }

        /// <summary>
        /// Total count using this domain
        /// </summary>
        public int TotalCount
        {
            get
            {
                return _usercount + _objectcount;
            }
        }
    }
}
