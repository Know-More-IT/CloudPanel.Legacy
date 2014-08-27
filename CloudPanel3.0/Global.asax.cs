using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using log4net.Config;
using log4net;
using System.Reflection;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Sql;

namespace CloudPanel
{
    public class Global : System.Web.HttpApplication
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Pull all the settings from the database when the application starts and store in static variables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Start(object sender, EventArgs e)
        {
            // Start logger
            XmlConfigurator.Configure();

            try
            {
                // DEBUG //
                this.logger.Debug("Starting application. Retrieving settings from SQL.");

                // Get security key from Config. This is used to encrypt the password
                Config.Key = ConfigurationManager.AppSettings["Key"];

                // Get the data
                DataTable dt = SqlLibrary.ReadSql("SELECT TOP 1 * FROM Settings");
                
                Config.HostingOU                    = dt.Rows[0]["BaseOU"].ToString();
                this.logger.Debug("Hosting OU: " + Config.HostingOU);

                Config.PrimaryDC                    = dt.Rows[0]["PrimaryDC"].ToString();
                this.logger.Debug("Primary DC: " + Config.PrimaryDC);

                Config.ExchangeServer               = dt.Rows[0]["ExchangeFqdn"].ToString();
                this.logger.Debug("Exchange Server: " +  Config.ExchangeServer);

                Config.Username                     = dt.Rows[0]["Username"].ToString();
                this.logger.Debug("Username: " + Config.Username);

                Config.Password                     = dt.Rows[0]["Password"].ToString();
                this.logger.Debug("Password: **********");

                Config.SuperAdministrators          = dt.Rows[0]["SuperAdmins"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                this.logger.Debug("Super Admins: " + String.Join(", ", Config.SuperAdministrators));

                Config.BillingAdministrators        = dt.Rows[0]["BillingAdmins"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                this.logger.Debug("Billing Admins: " + String.Join(", ", Config.BillingAdministrators));

                Config.PublicFolderServer           = dt.Rows[0]["ExchangePFServer"].ToString();
                this.logger.Debug("Public Folder Server: " + Config.PublicFolderServer);

                Config.ExchangeSSLEnabled           = bool.Parse(dt.Rows[0]["ExchangeSSLEnabled"].ToString());
                this.logger.Debug("Exchange SSL Enabled: " + Config.ExchangeSSLEnabled.ToString());

                Config.CitrixEnabled                = bool.Parse(dt.Rows[0]["CitrixEnabled"].ToString());
                this.logger.Debug("Citrix Enabled: " + Config.CitrixEnabled.ToString());

                Config.ExchangePublicFoldersEnabled = bool.Parse(dt.Rows[0]["PublicFolderEnabled"].ToString());
                this.logger.Debug("Exchange Public Folders Enabled: " + Config.ExchangePublicFoldersEnabled.ToString());

                Config.CurrencySymbol               = dt.Rows[0]["CurrencySymbol"].ToString();
                this.logger.Debug("Currency Symbol: " + Config.CurrencySymbol);

                Config.CurrencyEnglishName          = dt.Rows[0]["CurrencyEnglishName"].ToString();
                this.logger.Debug("Currency English Name: " + Config.CurrencyEnglishName);

                Config.ResellersEnabled             = bool.Parse(dt.Rows[0]["ResellersEnabled"].ToString());
                this.logger.Debug("Resellers Enabled: " + Config.ResellersEnabled.ToString());

                Config.HostersName                  = dt.Rows[0]["CompanysName"].ToString();
                this.logger.Debug("Hosters Name: " + Config.HostersName);

                Config.CustomNameAttribute          = bool.Parse(dt.Rows[0]["AllowCustomNameAttrib"].ToString());
                this.logger.Debug("Custom Name Attribute Enabled: " + Config.CustomNameAttribute.ToString());

                Config.ExchangeStatsEnabled         = bool.Parse(dt.Rows[0]["ExchStats"].ToString());
                this.logger.Debug("Exchange Stats Enabled: " + Config.ExchangeStatsEnabled.ToString());

                Config.BruteForceProtectionEnabled  = bool.Parse(dt.Rows[0]["IPBlockingEnabled"].ToString());
                this.logger.Debug("Brute Force Protection Enabled: " + Config.BruteForceProtectionEnabled.ToString());

                Config.BruteForceFailedCount        = int.Parse(dt.Rows[0]["IPBlockingFailedCount"].ToString());
                this.logger.Debug("Brute Force Failed Count: " + Config.BruteForceFailedCount.ToString());

                Config.BruteForceLockoutInMin       = int.Parse(dt.Rows[0]["IPBlockingLockedMinutes"].ToString());
                this.logger.Debug("Brute Force Lockout In Minutes: " + Config.BruteForceLockoutInMin.ToString());

                if (dt.Rows[0]["BrandingLoginLogo"] != null)
                {
                    Config.LoginLogo = dt.Rows[0]["BrandingLoginLogo"].ToString();
                    this.logger.Debug("Login Logo: " + Config.LoginLogo);
                }

                if (dt.Rows[0]["BrandingCornerLogo"] != null)
                {
                    Config.CornerLogo = dt.Rows[0]["BrandingCornerLogo"].ToString();
                    this.logger.Debug("Corner Logo: " + Config.CornerLogo);
                }

                if (dt.Rows[0]["UsersOU"] != null)
                {
                    Config.UsersOU = dt.Rows[0]["UsersOU"].ToString();
                    this.logger.Debug("Custom Users OU: " + Config.UsersOU);
                }

                Config.LockedDownModeEnabled        = bool.Parse(dt.Rows[0]["LockdownEnabled"].ToString());
                this.logger.Debug("Lock Down Mode Enabled: " + Config.LockedDownModeEnabled.ToString());

                if (dt.Rows[0]["ExchDatabases"] != null)
                {
                    Config.ExchangeDatabases = dt.Rows[0]["ExchDatabases"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    this.logger.Debug("Exchange Databases: " + String.Join(", ", Config.ExchangeDatabases));
                }

                Config.ExchangeVersion              = int.Parse(dt.Rows[0]["ExchangeVersion"].ToString()) == 2013 ? Enumerations.ProductVersion.Exchange2013 : Enumerations.ProductVersion.Exchange2010;
                this.logger.Debug("Exchange Version: " + Config.ExchangeVersion.ToString());

                Config.ExchangeConnectionType       = dt.Rows[0]["ExchangeConnectionType"].ToString() == "Kerberos" ? 
                                                        Enumerations.ConnectionType.Kerberos : 
                                                        Enumerations.ConnectionType.Basic;
                this.logger.Debug("Exchange Connection Type: " + Config.ExchangeConnectionType.ToString());

                Config.LyncEnabled                  = bool.Parse(dt.Rows[0]["LyncEnabled"].ToString());
                this.logger.Debug("Lync Enabled: " + Config.LyncEnabled.ToString());

                Config.LyncFrontEnd                 = dt.Rows[0]["LyncFrontEnd"].ToString();
                this.logger.Debug("Lync Front End: " + Config.LyncFrontEnd);

                Config.LyncUserPool                 = dt.Rows[0]["LyncUserPool"].ToString();
                this.logger.Debug("Lync User Pool: " + Config.LyncUserPool);

                Config.LyncMeetUri                  = dt.Rows[0]["LyncMeetingUrl"].ToString();
                this.logger.Debug("Lync Meeting Url: " + Config.LyncMeetUri);

                Config.LyncdialinUri                = dt.Rows[0]["LyncDialinUrl"].ToString();
                this.logger.Debug("Lync Dialin Url: " + Config.LyncdialinUri);

                Config.SupportMailEnabled           = bool.Parse(dt.Rows[0]["SupportMailEnabled"].ToString());
                this.logger.Debug("Support mail enabled: " + Config.SupportMailEnabled.ToString());

                Config.SupportMailAddress           = dt.Rows[0]["SupportMailAddress"].ToString();
                this.logger.Debug("Support mail address: " + Config.SupportMailAddress);

                Config.SupportMailServer            = dt.Rows[0]["SupportMailServer"].ToString();
                this.logger.Debug("Support mail server: " + Config.SupportMailServer);

                Config.SupportMailPort              = int.Parse(dt.Rows[0]["SupportMailPort"].ToString());
                this.logger.Debug("Support mail port: " + Config.SupportMailPort.ToString());

                Config.SupportMailUsername          = dt.Rows[0]["SupportMailUsername"].ToString();
                this.logger.Debug("Support mail username: " + Config.SupportMailUsername.ToString());

                Config.SupportMailFrom              = dt.Rows[0]["SupportMailFrom"].ToString();
                this.logger.Debug("Support mail from: " + Config.SupportMailFrom);

                // Allow Application Login
                Config.LoginIsAllowed = true;
            }
            catch (Exception ex)
            {
                this.logger.Error("Error retrieving application settings: " + ex.ToString());

                // Do not allow login
                Config.LoginIsAllowed = false;
            }
        }

        /// <summary>
        /// When a session starts we need to get the culture information of the browser and store it in the users session
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Session_Start(object sender, EventArgs e)
        {
            // When a session starts set the culture information for the user
            try
            {
                var userLanguages = Request.UserLanguages;
                if (userLanguages.Count() > 0)
                {
                    logger.Debug("Detected browsers language is " + userLanguages[0]);

                    // Set the culture information
                    CPContext.UsersCulture = new System.Globalization.CultureInfo(userLanguages[0]);
                }
                else
                    logger.Warn("Culture information was not found i nthe user's browser.");
            }
            catch (Exception ex)
            {
                logger.Warn("Unable to get culture information from the browser of a user's session.", ex);
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Log the application error that occurred
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            // Get the last error
            Exception ex = Server.GetLastError();

            // ERROR //
            this.logger.Error("Application error occurred: " + ex.ToString());
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}