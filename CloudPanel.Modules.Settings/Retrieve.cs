using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace CloudPanel.Modules.Settings
{
    public class Retrieve
    {
        /// <summary>
        /// Gets the Citrix App OU (Applications) at the base hosting OU level
        /// </summary>
        public static string GetApplicationsOU
        {
            get
            {
                return string.Format("OU=Applications,{0}", Config.HostingOU);
            }
        }

        /// <summary>
        /// Gets the application OU for a company
        /// </summary>
        public static string GetCompanyApplicationsOU
        {
            get
            {
                return string.Format("OU=Applications,{0}", GetCompanyOU);
            }
        }

        /// <summary>
        /// Gets the company OU from the selected reseller and company code
        /// </summary>
        public static string GetCompanyOU
        {
            get
            {
                if (CPContext.SelectedResellerCode == null)
                    throw new Exception("Unable to get company OU because the reseller wasn't selected");

                if (CPContext.SelectedCompanyCode == null)
                    throw new Exception("Unable to get company OU because the company wasn't selected");

                // Check if resellers are enabled or not
                if (Config.ResellersEnabled)
                    return string.Format("OU={0},OU={1},{2}", CPContext.SelectedCompanyCode, CPContext.SelectedResellerCode, Config.HostingOU);
                else
                    return string.Format("OU={0},{1}", CPContext.SelectedCompanyCode, Config.HostingOU);
            }
        }

        /// <summary>
        /// Gets the OU to store the user object
        /// </summary>
        public static string GetCompanyUsersOU
        {
            get
            {
                if (string.IsNullOrEmpty(Config.UsersOU))
                    return GetCompanyOU;
                else
                    return string.Format("OU={0},{1}", Config.UsersOU, GetCompanyOU);
            }
        }

        /// <summary>
        /// Gets the Exchange OU in the selected company's OU
        /// </summary>
        public static string GetCompanyExchangeOU
        {
            get
            {
                if (CPContext.SelectedResellerCode == null)
                    throw new Exception("Unable to get company OU because the reseller wasn't selected");

                if (CPContext.SelectedCompanyCode == null)
                    throw new Exception("Unable to get company OU because the company wasn't selected");

                return string.Format("OU=Exchange,{0}", GetCompanyOU);
            }
        }

        /// <summary>
        /// Returns a random string for verification
        /// </summary>
        public static string RandomString
        {
            get
            {
                // Set our random string to match
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                var result = new string(
                    Enumerable.Repeat(chars, 8)
                              .Select(s => s[random.Next(s.Length)])
                              .ToArray());

                return result;
            }
        }
    }
}
