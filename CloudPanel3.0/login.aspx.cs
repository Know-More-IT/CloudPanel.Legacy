using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudPanel.Modules.ActiveDirectory;
using CloudPanel.Modules.Settings;
using System.Web.Security;
using System.Reflection;
using log4net;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Sql;
using System.Diagnostics;
using System.Globalization;
using CloudPanel.classes;

namespace CloudPanel3._0
{
    public partial class login : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        String ip = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set the company logo
                imgLogo.ImageUrl = ResolveUrl(Config.LoginLogo);
            }

            if (Config.LoginIsAllowed)
            {
                // Clear error message
                lbInfo.Text = string.Empty;

                // Enable logins
                lnkLogin.Enabled = true;

                // Get Client IP Info //
                ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip))
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                else
                    ip = ip.Split(',')[0];
            }
            else
            {
                // Set error message [This application has invalid stetings. Please contact support.]
                lbInfo.Text = Resources.LocalizedText.Login_InvalidSettings;

                // Disable logins
                lnkLogin.Enabled = false;
            }
        }

        protected void lnkLogin_Click(object sender, EventArgs e)
        {
            ADUsers users = null;

            try
            {
                // Initialize
                users = new ADUsers(Config.Username, Config.Password, Config.PrimaryDC);
                                
                // Check if IP Blocking is enabled and get the amount of logins for this IP
                if (Config.BruteForceProtectionEnabled && !Request.IsLocal)
                {
                    if (SqlCommon.IsIPLockedOut(ip, Config.BruteForceFailedCount, Config.BruteForceLockoutInMin))
                        throw new Exception(Resources.LocalizedText.Login_BruteForceBlock);
                }

                // Validate credentials
                bool isSuperAdmin = false;
                bool validLogin = users.Authenticate(txtUsername.Text, txtPassword.Text, Config.SuperAdministrators, out isSuperAdmin);

                // Audit the login
                SQLAudit.AuditLogin(ip, txtUsername.Text, validLogin);

                // Check if we are in lockdown mode
                if (!isSuperAdmin && Config.LockedDownModeEnabled)
                    throw new Exception(Resources.LocalizedText.Login_DownForMaintenace);

                if (!validLogin)
                {
                    lbInfo.Text = Resources.LocalizedText.Login_InvalidSettings;

                    // DEBUG //
                    this.logger.Warn("User " + txtUsername.Text + " failed to authentication. IP: " + ip);
                }
                else
                {
                    // Create our authentication ticket
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, txtUsername.Text, DateTime.Now, DateTime.Now.AddHours(8), true, "");
                    string cookieEncrypt = FormsAuthentication.Encrypt(ticket);
                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieEncrypt);

                    cookie.Path = FormsAuthentication.FormsCookiePath;

                    // Add Cookie 
                    Response.Cookies.Add(cookie);

                    // DEBUG //
                    this.logger.Debug("User " + txtUsername.Text + " successfully logged in. IP: " + ip);

                    // Set Permissions
                    bool allowedLogin = Authentication.SetPermissions(isSuperAdmin, SQLUsers.GetUser(txtUsername.Text));

                    // Check if the user is either a superadmin, reseller admin, or company admin
                    if (!allowedLogin)
                    {
                        // Show the user they don't have permission
                        lbInfo.Text = Resources.LocalizedText.Login_PermissionDenied;

                        // Clear session
                        Session.Abandon();

                        // Log this event
                        this.logger.Info("User " + txtUsername.Text + " tried to access the system but they were not a Super Admin, Reseller Admin, or Company Admin.");
                    }
                    else
                    {
                        // Set session for username
                        Session["CPUsername"] = txtUsername.Text;

                        // Redirect to dashboard
                        Response.Redirect("dashboard.aspx", false);
                    }
                }
            }
            catch (Exception ex)
            {
                lbInfo.Text = Resources.LocalizedText.Global_Error + ex.Message;

                // ERROR //
                this.logger.Fatal(txtUsername.Text + " failed to login.", ex);
            }
            finally
            {
                if (users != null)
                    users.Dispose();
            }
        }

    }
}