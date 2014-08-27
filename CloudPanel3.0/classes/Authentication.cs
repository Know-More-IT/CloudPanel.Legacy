using CloudPanel.Modules.Base;
using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Security;

namespace CloudPanel.classes
{
    public class Authentication
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Sets the appropriate permissions for the user
        /// </summary>
        /// <param name="isSuperAdmin"></param>
        /// <param name="sqlUser"></param>
        /// <returns></returns>
        public static bool SetPermissions(bool isSuperAdmin, BaseUser sqlUser)
        {
            bool allowedLogin = false;

            try
            {
                // DEBUG //
                logger.Debug("Setting permissions for user. " + HttpContext.Current.User.Identity.Name);

                if (isSuperAdmin)
                {
                    // User is a super administrator. Set to true
                    IsSuperAdmin = true;

                    // Update to allow login
                    allowedLogin = true;
                }

                // Now check if they are a reseller administrator
                if (sqlUser.IsResellerAdmin)
                {
                    // User is a reseller administrator. Now set to true
                    IsResellerAdmin = true;

                    // Now we need to set the reseller code and name
                    CPContext.SelectedResellerCode = sqlUser.ResellerCode;
                    CPContext.SelectedResellerName = sqlUser.ResellerName;

                    // Update allow login
                    allowedLogin = true;
                }
                else
                    IsResellerAdmin = false;

                //  Now lets check if they are a company administrator
                if (sqlUser.IsCompanyAdmin)
                {
                    // User is a company administrator. Set to true
                    IsCompanyAdmin = true;

                    // Now we need to set the reseller code and name
                    CPContext.SelectedResellerCode = sqlUser.ResellerCode;
                    CPContext.SelectedResellerName = sqlUser.ResellerName;

                    // Now we need to set the company code and name
                    CPContext.SelectedCompanyCode = sqlUser.CompanyCode;
                    CPContext.SelectedCompanyName = sqlUser.CompanyName;

                    // Set permissions for company admin                    
                    ADUser userPerm = DbSql.Get_CompanyAdminPermissions(HttpContext.Current.User.Identity.Name);
                    if (userPerm != null)
                    {
                        PermEnableExchange      = userPerm.EnableExchangePermission;
                        PermDisableExchange     = userPerm.DisableExchangePermission;
                        PermAddDomain           = userPerm.AddDomainPermission;
                        PermDeleteDomain        = userPerm.DeleteDomainPermission;
                        PermModifyAcceptedDomain = userPerm.ModifyAcceptedDomainPermission;
                        PermImportUsers         = userPerm.ImportUsersPermission;
                    }

                    // Update allow login
                    allowedLogin = true;
                }
                else
                    IsCompanyAdmin = false;


                // If reseller is not enabled we need to change the selected reseller
                if (!Config.ResellersEnabled)
                {
                    CPContext.SelectedResellerCode = Config.HostersName;
                    CPContext.SelectedResellerName = Config.HostersName;
                }

                return allowedLogin;
            }
            catch (Exception ex)
            {
                logger.Fatal("Failure setting the permissions for the user " + HttpContext.Current.User.Identity.Name, ex);

                throw;
            }
        }

        /// <summary>
        /// Checks if a user is authenticated or not
        /// </summary>
        public static bool IsAuthenticated
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated && HttpContext.Current.Session["CPUsername"] != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Checks to see if the user is a Super Admin or not
        /// </summary>
        /// <returns></returns>
        public static bool IsSuperAdmin
        {
            get
            {
                if (HttpContext.Current.Session["CloudPanel.SuperAdmin"] == null)
                    return false;
                else
                {
                    bool setValue = false;
                    bool.TryParse(HttpContext.Current.Session["CloudPanel.SuperAdmin"].ToString(), out setValue);

                    return setValue;
                }
            }
            set
            {
                HttpContext.Current.Session["CloudPanel.SuperAdmin"] = value;
            }
        }

        /// <summary>
        /// Checks to see if the user is a Reseller Admin or not
        /// </summary>
        /// <returns></returns>
        public static bool IsResellerAdmin
        {
            get
            {
                if (HttpContext.Current.Session["CloudPanel.ResellerAdmin"] == null)
                    return false;
                else
                {
                    bool setValue = false;
                    bool.TryParse(HttpContext.Current.Session["CloudPanel.ResellerAdmin"].ToString(), out setValue);

                    return setValue;
                }
            }
            set
            {
                HttpContext.Current.Session["CloudPanel.ResellerAdmin"] = value;
            }
        }

        /// <summary>
        /// Checks to see if the user is a Company Admin or not
        /// </summary>
        /// <returns></returns>
        public static bool IsCompanyAdmin
        {
            get
            {
                if (HttpContext.Current.Session["CloudPanel.CompanyAdmin"] == null)
                    return false;
                else
                {
                    bool setValue = false;
                    bool.TryParse(HttpContext.Current.Session["CloudPanel.CompanyAdmin"].ToString(), out setValue);

                    return setValue;
                }
            }
            set
            {
                HttpContext.Current.Session["CloudPanel.CompanyAdmin"] = value;
            }
        }

        /// <summary>
        /// If the user has permission to enable Exchange (company admin only)
        /// </summary>
        public static bool PermEnableExchange
        {
            get
            {
                if (IsSuperAdmin || IsResellerAdmin)
                    return true;
                else
                {
                    if (HttpContext.Current.Session["CloudPanel.Permission.EnableExchange"] == null)
                        return false;
                    else
                    {
                        bool setValue = false;
                        bool.TryParse(HttpContext.Current.Session["CloudPanel.Permission.EnableExchange"].ToString(), out setValue);

                        return setValue;
                    }
                }
            }
            set
            {
                HttpContext.Current.Session["CloudPanel.Permission.EnableExchange"] = value;
            }
        }

        /// <summary>
        /// If the user has permission to disable Exchange (company admin only)
        /// </summary>
        public static bool PermDisableExchange
        {
            get
            {
                if (IsSuperAdmin || IsResellerAdmin)
                    return true;
                else
                {
                    if (HttpContext.Current.Session["CloudPanel.Permission.DisableExchange"] == null)
                        return false;
                    else
                    {
                        bool setValue = false;
                        bool.TryParse(HttpContext.Current.Session["CloudPanel.Permission.DisableExchange"].ToString(), out setValue);

                        return setValue;
                    }
                }
            }
            set
            {
                HttpContext.Current.Session["CloudPanel.Permission.DisableExchange"] = value;
            }
        }

        /// <summary>
        /// If the user has permission to add a domain (company admin only)
        /// </summary>
        public static bool PermAddDomain
        {
            get
            {
                if (IsSuperAdmin || IsResellerAdmin)
                    return true;
                else
                {
                    if (HttpContext.Current.Session["CloudPanel.Permission.AddDomain"] == null)
                        return false;
                    else
                    {
                        bool setValue = false;
                        bool.TryParse(HttpContext.Current.Session["CloudPanel.Permission.AddDomain"].ToString(), out setValue);

                        return setValue;
                    }
                }
            }
            set
            {
                HttpContext.Current.Session["CloudPanel.Permission.AddDomain"] = value;
            }
        }

        /// <summary>
        /// If the user has permission to delete a domain (company admin only)
        /// </summary>
        public static bool PermDeleteDomain
        {
            get
            {
                if (IsSuperAdmin || IsResellerAdmin)
                    return true;
                else
                {
                    if (HttpContext.Current.Session["CloudPanel.Permission.DeleteDomain"] == null)
                        return false;
                    else
                    {
                        bool setValue = false;
                        bool.TryParse(HttpContext.Current.Session["CloudPanel.Permission.DeleteDomain"].ToString(), out setValue);

                        return setValue;
                    }
                }
            }
            set
            {
                HttpContext.Current.Session["CloudPanel.Permission.DeleteDomain"] = value;
            }
        }

        /// <summary>
        /// If the user has permission to modify a accepted domain (company admin only)
        /// </summary>
        public static bool PermModifyAcceptedDomain
        {
            get
            {
                if (IsSuperAdmin || IsResellerAdmin)
                    return true;
                else
                {
                    if (HttpContext.Current.Session["CloudPanel.Permission.ModifyAcceptedDomain"] == null)
                        return false;
                    else
                    {
                        bool setValue = false;
                        bool.TryParse(HttpContext.Current.Session["CloudPanel.Permission.ModifyAcceptedDomain"].ToString(), out setValue);

                        return setValue;
                    }
                }
            }
            set
            {
                HttpContext.Current.Session["CloudPanel.Permission.ModifyAcceptedDomain"] = value;
            }
        }

        /// <summary>
        /// If the user has permission to import users (company admin only)
        /// </summary>
        public static bool PermImportUsers
        {
            get
            {
                if (IsSuperAdmin || IsResellerAdmin)
                    return true;
                else
                {
                    if (HttpContext.Current.Session["CloudPanel.Permission.ImportUsers"] == null)
                        return false;
                    else
                    {
                        bool setValue = false;
                        bool.TryParse(HttpContext.Current.Session["CloudPanel.Permission.ImportUsers"].ToString(), out setValue);

                        return setValue;
                    }
                }
            }
            set
            {
                HttpContext.Current.Session["CloudPanel.Permission.ImportUsers"] = value;
            }
        }

        /// <summary>
        /// Gets a list of groups the user belongs to from their ticket
        /// </summary>
        /// <returns></returns>
        private static string[] GetUserRoles()
        {
            try
            {
                FormsIdentity fi = HttpContext.Current.User.Identity as FormsIdentity;
                return fi.Ticket.UserData.Split(',');
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Resets the session settings for permissions
        /// </summary>
        public static void ResetPermissions()
        {
            HttpContext.Current.Session["CloudPanel.SuperAdmin"] = null;
            HttpContext.Current.Session["CloudPanel.ResellerAdmin"] = null;
            HttpContext.Current.Session["CloudPanel.CompanyAdmin"] = null;
        }

        /// <summary>
        /// Returns the list of groups that are super admins from the settings
        /// </summary>
        /// <returns></returns>
        public static string GetSuperAdminGroups
        {
            get
            {
                return HttpContext.Current.Application["SuperAdmins"].ToString().ToLower();
            }
        }

        /// <summary>
        /// Gets the reseller's name
        /// </summary>
        /// <param name="resellerCode"></param>
        /// <returns></returns>
        private static string GetResellerCompanyName(string resellerCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT CompanyName FROM Companies WHERE IsReseller=1 AND CompanyCode=@CompanyCode", sql);

            try
            {
                // Add parameter
                cmd.Parameters.AddWithValue("CompanyCode", resellerCode);

                // Open
                sql.Open();

                // Get value
                object value = cmd.ExecuteScalar();

                // Close
                sql.Close();

                // Return value
                return value.ToString();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();

                if (sql != null)
                    sql.Dispose();
            }
        }

        /// <summary>
        /// Gets the company's name and the reseller code
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        private static string[] GetCompanyName(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT CompanyName, ResellerCode FROM Companies WHERE IsReseller=0 AND CompanyCode=@CompanyCode", sql);

            try
            {
                // Add parameter
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open
                sql.Open();

                // Get value
                string[] values = new string[2];
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    values[0] = r["CompanyName"].ToString();
                    values[1] = r["ResellerCode"].ToString();
                }

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return value
                return values;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();

                if (sql != null)
                    sql.Dispose();
            }
        }
    }
}
