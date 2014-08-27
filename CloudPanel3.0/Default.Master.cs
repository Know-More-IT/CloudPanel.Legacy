using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using CloudPanel.classes;

namespace CloudPanel
{
    public partial class Default : System.Web.UI.MasterPage
    {
        public bool IsSuperAdmin        = Authentication.IsSuperAdmin;
        public bool IsResellerAdmin     = Authentication.IsResellerAdmin;
        public bool IsCompanyAdmin      = Authentication.IsCompanyAdmin;
        public bool ExchangeStatistics  = Config.ExchangeStatsEnabled;

        public int timeoutWarningInMilliseconds;
        public int expireSessionInMilliseconds;
        public int timeoutInSeconds;

        private readonly int whenToWarnInMinutes = 2;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in or not (not for settings page)
            if (!HttpContext.Current.Request.Path.Contains("settings.aspx"))
            {
                if (!Authentication.IsAuthenticated)
                    Response.Redirect("~/login.aspx?url=" + HttpContext.Current.Request.Path, true);
            }
            else
            {
                // Make sure only super admins can get to the plans section
                if (HttpContext.Current.Request.Path.Contains("/plans/") && !IsSuperAdmin)
                    Response.Redirect("~/dashboard.aspx", true);

                // Make sure only super admins and reseller admins can access the reports section
                if (HttpContext.Current.Request.Path.Contains("/reporting/") && !IsSuperAdmin)
                    Response.Redirect("~/dashboard.aspx", true);

                // Make sure only super admins, reseller admins, and company admins access the company section
                if (HttpContext.Current.Request.Path.Contains("/company/") && (!IsSuperAdmin && !IsCompanyAdmin && !IsResellerAdmin))
                    Response.Redirect("~/dashboard.aspx", true);
            }
            
            // Set logo on the top right
            headerh1.Style.Remove("background");
            headerh1.Style.Add("background", string.Format("url('{0}') no-repeat scroll 0 0 transparent", ResolveUrl(Config.CornerLogo)));
        }
        
        protected void btnExtendSession_Click(object sender, EventArgs e)
        {
            // Refresh ticket
            FormsIdentity fi = (FormsIdentity)HttpContext.Current.User.Identity;
            FormsAuthentication.RenewTicketIfOld(fi.Ticket);

            // Update values
            Session.Timeout = 60;
            timeoutInSeconds = fi.Ticket.Expiration.Subtract(DateTime.Now).Minutes * 60;
            timeoutWarningInMilliseconds = (timeoutInSeconds - (whenToWarnInMinutes * 60)) * 1000;
            expireSessionInMilliseconds = timeoutInSeconds * 1000;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/search.aspx?search=" + txtSearch.Text, false);
        } 

        /// <summary>
        /// Log out and kill session
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkLogOut_Click1(object sender, EventArgs e)
        {
            HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();
            
            // Redirect to login
            Response.Redirect("~/login.aspx", false);
        }
    }
}