using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CloudPanel.Modules.Base.Class;

namespace CloudPanel.Modules.Settings
{
    public static class Config
    {
        /// <summary>
        /// Security key that is used to encrypt / decrypt data
        /// </summary>
        private static string _key;
        public static string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        #region Active Directory Settings

        /// <summary>
        /// The organizational unit that holds all the resellers and company organizational units
        /// </summary>
        private static string _hostingou;
        public static string HostingOU
        {
            get { return _hostingou; }
            set { _hostingou = value; }
        }

        /// <summary>
        /// The primary domain controller that CloudPanel communicates with
        /// </summary>
        private static string _primarydc;
        public static string PrimaryDC
        {
            get { return _primarydc; }
            set { _primarydc = value; }
        }

        /// <summary>
        /// If customized to put the users in another OU located within the company.
        /// Only supports one level down and not multiple levels down within the company OU.
        /// </summary>
        private static string _usersou;
        public static string UsersOU
        {
            get { return _usersou; }
            set { _usersou = value; }
        }

        #endregion

        #region Authentication Settings

        /// <summary>
        /// The username for authenticating to AD and Exchange (in DOMAIN\Username format)
        /// </summary>
        private static string _username;
        public static string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        /// <summary>
        /// The password for the username field used to authenticate with AD and Exchange
        /// </summary>
        private static string _password;
        public static string Password
        {
            get 
            {
                return DataProtection.Decrypt(_password, Key); 
            }
            set { _password = value; }
        }

        /// <summary>
        /// The super admins for CloudPanel that have complete control to the system (except billing)
        /// </summary>
        private static string[] _superadministrators;
        public static string[] SuperAdministrators
        {
            get { return _superadministrators; }
            set { _superadministrators = value; }
        }

        /// <summary>
        /// The billing admins for CloudPanel that have access to billing information (Super admins do not have rights)
        /// </summary>
        private static string[] _billingadministrators;
        public static string[] BillingAdministrators
        {
            get { return _billingadministrators; }
            set { _billingadministrators = value; }
        }

        #endregion

        #region Exchange Settings

        /// <summary>
        /// The Exchange server FQDN that CloudPanel communicates with for powershell commands
        /// </summary>
        private static string _exchangeserver;
        public static string ExchangeServer
        {
            get { return _exchangeserver; }
            set { _exchangeserver = value; }
        }

        /// <summary>
        /// The Exchange server that CloudPanel communicates with for Public Folder commands
        /// </summary>
        private static string _publicfolderserver;
        public static string PublicFolderServer
        {
            get { return _publicfolderserver; }
            set { _publicfolderserver = value; }
        }

        /// <summary>
        /// The version of the Exchange server
        /// </summary>
        private static Enumerations.ProductVersion _exchangeversion;
        public static Enumerations.ProductVersion ExchangeVersion
        {
            get { return _exchangeversion; }
            set { _exchangeversion = value; }
        }

        /// <summary>
        /// The type of connection for Exchange server
        /// </summary>
        private static Enumerations.ConnectionType _exchangeconnectiontype;
        public static Enumerations.ConnectionType ExchangeConnectionType
        {
            get { return _exchangeconnectiontype; }
            set { _exchangeconnectiontype = value; }
        }

        /// <summary>
        /// If Exchange SSL is enabled or not when communicating with powershell
        /// </summary>
        private static bool _exchangesslenabled;
        public static bool ExchangeSSLEnabled
        {
            get { return _exchangesslenabled; }
            set { _exchangesslenabled = value; }
        }

        /// <summary>
        /// If Exchange Public Folders are enabled in CloudPanel or not
        /// </summary>
        private static bool _exchangepublicfoldersenabled;
        public static bool ExchangePublicFoldersEnabled
        {
            get { return _exchangepublicfoldersenabled; }
            set { _exchangepublicfoldersenabled = value; }
        }

        /// <summary>
        /// If Exchange Statistics are enabled for CloudPanel or not (Requires addon service)
        /// </summary>
        private static bool _exchangestatsenabled;
        public static bool ExchangeStatsEnabled
        {
            get { return _exchangestatsenabled; }
            set { _exchangestatsenabled = value; }
        }

        /// <summary>
        /// List of Exchange databases so you can choose which database to add
        /// </summary>
        private static List<string> _exchdatabases;
        public static List<string> ExchangeDatabases
        {
            get 
            {
                return _exchdatabases;
            }
            set { _exchdatabases = value; }
        }

        /// <summary>
        /// Returns the formatted Exchange URI
        /// </summary>
        public static string ExchangeURI
        {
            get
            {
                if (ExchangeSSLEnabled)
                    return string.Format("https://{0}/powershell", ExchangeServer);
                else
                    return string.Format("http://{0}/powershell", ExchangeServer);
            }
        }

        #endregion

        #region Lync Settings

        /// <summary>
        /// If Lync is enabled or not
        /// </summary>
        private static bool _lyncenabled;
        public static bool LyncEnabled
        {
            get { return _lyncenabled; }
            set { _lyncenabled = value; }
        }

        /// <summary>
        /// The frontend server we use to call remote powershell commands for Lync
        /// </summary>
        private static string _lyncfrontend;
        public static string LyncFrontEnd
        {
            get { return _lyncfrontend; }
            set { _lyncfrontend = value; }
        }

        /// <summary>
        /// The Lync user pool to assign the users to
        /// </summary>
        private static string _lyncuserpool;
        public static string LyncUserPool
        {
            get { return _lyncuserpool; }
            set { _lyncuserpool = value; }
        }

        /// <summary>
        /// The base lync meeting url for the hoster
        /// </summary>
        private static string _lyncmeeturi;
        public static string LyncMeetUri
        {
            get
            {
                if (!string.IsNullOrEmpty(_lyncmeeturi))
                    return string.Format("https://{0}", _lyncmeeturi.ToLower().Replace("http://", string.Empty).Replace("https://", string.Empty));
                else
                    return string.Empty;
            }
            set { _lyncmeeturi = value; }
        }

        /// <summary>
        /// The Lync dialin url for the hoster
        /// </summary>
        private static string _lyncdialinuri;
        public static string LyncdialinUri
        {
            get
            {
                if (!string.IsNullOrEmpty(_lyncdialinuri))
                    return string.Format("https://{0}", _lyncdialinuri.ToLower().Replace("http://", string.Empty).Replace("https://", string.Empty));
                else
                    return string.Empty;
            }
            set { _lyncdialinuri = value; }
        }

        /// <summary>
        /// Returns the formatted Lync URI
        /// </summary>
        public static string LyncURI
        {
            get
            {
                return string.Format("https://{0}/ocspowershell", LyncFrontEnd);
            }
        }


        #endregion

        #region Citrix Settings

        /// <summary>
        /// If Citrix is enabled or not
        /// </summary>
        private static bool _citrixenabled;
        public static bool CitrixEnabled
        {
            get { return _citrixenabled; }
            set { _citrixenabled = value; }
        }

        #endregion

        #region Currency Settings

        /// <summary>
        /// The currency symbol for CloudPanel
        /// </summary>
        private static string _currencysymbol;
        public static string CurrencySymbol
        {
            get { return _currencysymbol; }
            set { _currencysymbol = value; }
        }

        /// <summary>
        /// The name of the type of currency
        /// </summary>
        private static string _currencyenglishname;
        public static string CurrencyEnglishName
        {
            get { return _currencyenglishname; }
            set { _currencyenglishname = value; }
        }

        #endregion

        #region Brute Force Protection

        /// <summary>
        /// If brute force protection is enabled or not
        /// </summary>
        private static bool _bruteforceprotectionenabled;
        public static bool BruteForceProtectionEnabled
        {
            get { return _bruteforceprotectionenabled; }
            set { _bruteforceprotectionenabled = value; }
        }

        /// <summary>
        /// If lock down mode is enabled it will block ALL ip addresses except the local server
        /// and the IP addresses in the allowed IP addresses section
        /// </summary>
        private static bool _lockdownmodeenabled;
        public static bool LockedDownModeEnabled
        {
            get { return _lockdownmodeenabled; }
            set { _lockdownmodeenabled = value; }
        }

        /// <summary>
        /// The number of times an IP can fail login before it is blocked
        /// </summary>
        private static int _bruteforcefailedcount;
        public static int BruteForceFailedCount
        {
            get { return _bruteforcefailedcount; }
            set { _bruteforcefailedcount = value; }
        }

        /// <summary>
        /// How many minutes an IP is blocked before it is allowed to try again
        /// </summary>
        private static int _bruteforcelockoutinmin;
        public static int BruteForceLockoutInMin
        {
            get { return _bruteforcelockoutinmin; }
            set { _bruteforcelockoutinmin = value; }
        }

        /// <summary>
        /// List of IP addresses that are permanently blocked
        /// </summary>
        private static List<IPAddress> _blockedipaddresses;
        public static List<IPAddress> BlockedIPAddresses
        {
            get { return _blockedipaddresses; }
            set { _blockedipaddresses = value; }
        }

        /// <summary>
        /// List of IP addresses that are allowed (only in use when blocking ALL IP addresses)
        /// </summary>
        private static List<IPAddress> _allowedipaddresses;
        public static List<IPAddress> AllowedIPAddresses
        {
            get { return _allowedipaddresses; }
            set { _allowedipaddresses = value; }
        }

        #endregion

        #region Branding

        /// <summary>
        /// The name of the hoster for branding
        /// </summary>
        private static string _hostersname;
        public static string HostersName
        {
            get 
            {
                if (string.IsNullOrEmpty(_hostersname))
                    return "CloudPanel";
                else
                    return _hostersname; 
            }
            set { _hostersname = value; }
        }

        /// <summary>
        /// The path to the logo displayed on the login screen
        /// </summary>
        private static string _loginlogo;
        public static string LoginLogo
        {
            get 
            {
                if (string.IsNullOrEmpty(_loginlogo))
                    return "~/img/logo-default.png";
                else
                    return "~/img/branding/" + _loginlogo;
            }
            set { _loginlogo = value; }
        }

        /// <summary>
        /// The logo displayed in the top left corner after you login
        /// </summary>
        private static string _cornerlogo;
        public static string CornerLogo
        {
            get 
            {
                if (string.IsNullOrEmpty(_cornerlogo))
                    return "~/img/logo-default.png";
                else
                    return "~/img/branding/" + _cornerlogo; 
            }
            set { _cornerlogo = value; }
        }

        #endregion

        #region Mail Notifications

        /// <summary>
        /// If email notifications for errors are enabled or not
        /// </summary>
        private static bool _supportmailenabled;
        public static bool SupportMailEnabled
        {
            get { return _supportmailenabled; }
            set { _supportmailenabled = value; }
        }

        /// <summary>
        /// Who to send the email to when there is an error
        /// </summary>
        private static string _supportmailaddress;
        public static string SupportMailAddress
        {
            get { return _supportmailaddress; }
            set { _supportmailaddress = value; }
        }

        /// <summary>
        /// Who the message appears to be coming from
        /// </summary>
        private static string _supportmailfrom;
        public static string SupportMailFrom
        {
            get { return _supportmailfrom; }
            set { _supportmailfrom = value; }
        }

        /// <summary>
        /// The server to use when sending emails
        /// </summary>
        private static string _supportmailserver;
        public static string SupportMailServer
        {
            get { return _supportmailserver; }
            set { _supportmailserver = value; }
        }

        /// <summary>
        /// Username to use when sending emails
        /// </summary>
        private static string _supportmailusername;
        public static string SupportMailUsername
        {
            get { return _supportmailusername; }
            set { _supportmailusername = value; }
        }

        /// <summary>
        /// Password to use when sending emails
        /// </summary>
        private static string _supportmailpassword;
        public static string SupportMailPassword
        {
            get
            {
                if (string.IsNullOrEmpty(_supportmailpassword))
                    return string.Empty;
                else
                    return DataProtection.Decrypt(_supportmailpassword, Config.Key);
            }
            set { _supportmailpassword = value; }
        }

        /// <summary>
        /// Port to use when sending emails
        /// </summary>
        private static int _supportmailport;
        public static int SupportMailPort
        {
            get { return _supportmailport; }
            set { _supportmailport = value; }
        }

        #endregion

        #region Other Settings

        /// <summary>
        /// If resellers are enabled or not for CloudPanel
        /// </summary>
        private static bool _resellerenabled;
        public static bool ResellersEnabled
        {
            get { return _resellerenabled; }
            set { _resellerenabled = value; }
        }

        /// <summary>
        /// If CloudPanel allows you to enter the value for the 'Name' attribute in AD (shown as Full Name in CloudPanel)
        /// </summary>
        private static bool _customnameattribute;
        public static bool CustomNameAttribute
        {
            get { return _customnameattribute; }
            set { _customnameattribute = value; }
        }

        /// <summary>
        /// If logging into CloudPanel is allowed (Super Admins will always be allowed)
        /// </summary>
        private static bool _loginisallowed;
        public static bool LoginIsAllowed
        {
            get { return _loginisallowed; }
            set { _loginisallowed = value; }
        }

        #endregion
    }
}
