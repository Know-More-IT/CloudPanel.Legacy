using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudPanel.Modules.Base;
using System.Data.SqlClient;
using System.Configuration;
using CloudPanel.Modules.Base.Class;

namespace CloudPanel.Modules.Sql
{
    public class SQLUsers
    {
        public static void GetUserInfoForAuthentication(string userPrincipalName)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT
	                                                (SELECT CompanyName FROM Companies WHERE CompanyCode=u.CompanyCode) AS CompanyName,
	                                                u.CompanyCode,
	                                                (SELECT CompanyName FROM Companies WHERE IsReseller=1 AND CompanyCode=(SELECT ResellerCode FROM Companies WHERE CompanyCode=u.CompanyCode)) AS ResellerName,
	                                                (SELECT ResellerCode FROM Companies WHERE CompanyCode=u.CompanyCode) AS ResellerCode,
	                                                IsResellerAdmin,
                                                    IsCompanyAdmin
                                                FROM 
	                                                Users u
                                                WHERE 
	                                                UserPrincipalName=@UserPrincipalName", sql);

            try
            {
                // Add our parameter
                cmd.Parameters.AddWithValue("@UserPrincipalName", userPrincipalName);

                // Open connection 
                sql.Open();

                // Read our data
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {

                    }
                }

                // Close
                sql.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }
        }

        /// <summary>
        /// Checks if the user is a reseller admin or not
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public static bool IsUserResellerAdmin(string userPrincipalName)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT
	                                                IsResellerAdmin
                                               FROM 
	                                                Users
                                                WHERE 
	                                                UserPrincipalName=@UserPrincipalName", sql);

            try
            {
                // Add our parameter
                cmd.Parameters.AddWithValue("UserPrincipalName", userPrincipalName);

                // Open connection 
                sql.Open();

                // Read data
                object returnedData = cmd.ExecuteScalar();
                if (returnedData != DBNull.Value)
                    return bool.Parse(returnedData.ToString());
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }
        }

        /// <summary>
        /// Gets detailed information about a specific user
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public static BaseUser GetUser(string userPrincipalName)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT
	                                                *,
	                                                (SELECT CompanyName FROM Companies WHERE CompanyCode=u.CompanyCode) AS CompanyName,
	                                                (SELECT CompanyName FROM Companies WHERE IsReseller=1 AND CompanyCode=(SELECT ResellerCode FROM Companies WHERE CompanyCode=u.CompanyCode)) AS ResellerName,
	                                                (SELECT ResellerCode FROM Companies WHERE CompanyCode=u.CompanyCode) AS ResellerCode
                                                FROM 
	                                                Users u
                                                WHERE 
	                                                UserPrincipalName=@UserPrincipalName", sql);

            try
            {
                // Our object to return
                BaseUser tmp = new BaseUser();

                // Add our parameter
                cmd.Parameters.AddWithValue("UserPrincipalName", userPrincipalName);

                // Open connection 
                sql.Open();

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    tmp.ID = int.Parse(r["ID"].ToString());
                    tmp.UserGuid = Guid.Parse(r["UserGuid"].ToString());
                    tmp.UserPrincipalName = r["UserPrincipalName"].ToString();
                    tmp.DistinguishedName = r["DistinguishedName"].ToString();
                    tmp.DisplayName = r["DisplayName"].ToString();
                    tmp.CompanyCode = r["CompanyCode"].ToString();
                    tmp.CompanyName = r["CompanyName"].ToString();
                    tmp.ResellerCode = r["ResellerCode"].ToString();
                    tmp.ResellerName = r["ResellerName"].ToString();

                    if (r["sAMAccountName"] != DBNull.Value)
                        tmp.sAMAccountName = r["sAMAccountName"].ToString();
                    else
                        tmp.sAMAccountName = "";

                    if (r["Firstname"] != DBNull.Value)
                        tmp.Firstname = r["Firstname"].ToString();
                    else
                        tmp.Firstname = "";

                    if (r["Middlename"] != DBNull.Value)
                        tmp.Middlename = r["MiddleName"].ToString();
                    else
                        tmp.Middlename = "";

                    if (r["Lastname"] != DBNull.Value)
                        tmp.Lastname = r["Lastname"].ToString();
                    else
                        tmp.Lastname = "";

                    if (r["Email"] != DBNull.Value)
                        tmp.Lastname = r["Email"].ToString();
                    else
                        tmp.Lastname = "";

                    if (r["Department"] != DBNull.Value)
                        tmp.Department = r["Department"].ToString();
                    else
                        tmp.Department = "";

                    if (r["IsResellerAdmin"] != DBNull.Value)
                        tmp.IsResellerAdmin = bool.Parse(r["IsResellerAdmin"].ToString());
                    else
                        tmp.IsResellerAdmin = false;

                    if (r["IsCompanyAdmin"] != DBNull.Value)
                        tmp.IsCompanyAdmin = bool.Parse(r["IsCompanyAdmin"].ToString());
                    else
                        tmp.IsCompanyAdmin = false;

                    if (r["MailboxPlan"] != DBNull.Value)
                        tmp.MailboxPlan = int.Parse(r["MailboxPlan"].ToString());
                    else
                        tmp.MailboxPlan = 0;

                    if (r["TSPlan"] != DBNull.Value)
                        tmp.TSPlan = int.Parse(r["TSPlan"].ToString());
                    else
                        tmp.TSPlan = 0;

                    if (r["LyncPlan"] != DBNull.Value)
                        tmp.LyncPlan = int.Parse(r["LyncPlan"].ToString());
                    else
                        tmp.LyncPlan = 0;

                    if (r["Created"] != DBNull.Value)
                        tmp.Created = DateTime.Parse(r["Created"].ToString());

                    if (r["AdditionalMB"] != DBNull.Value)
                        tmp.AdditionalMB = int.Parse(r["AdditionalMB"].ToString());
                    else
                        tmp.AdditionalMB = 0;
                }

                // Dispose
                r.Close();
                r.Dispose();

                // return
                return tmp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }
        }
        
        /// <summary>
        /// Gets a list of users for a specific company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<BaseUser> GetUsers(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT
	                                                *,
	                                                (SELECT CompanyName FROM Companies WHERE CompanyCode=u.CompanyCode) AS CompanyName,
	                                                (SELECT CompanyName FROM Companies WHERE IsReseller=1 AND CompanyCode=(SELECT ResellerCode FROM Companies WHERE CompanyCode=u.CompanyCode)) AS ResellerName,
	                                                (SELECT ResellerCode FROM Companies WHERE CompanyCode=u.CompanyCode) AS ResellerCode
                                                FROM 
	                                                Users u
                                                WHERE
                                                    CompanyCode=@CompanyCode", sql);

            // Create our collection
            List<BaseUser> users = new List<BaseUser>();

            try
            {
                // Add our parameter
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                // Open connection 
                sql.Open();

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    BaseUser tmp = new BaseUser();
                    tmp.ID = int.Parse(r["ID"].ToString());
                    tmp.UserGuid = Guid.Parse(r["UserGuid"].ToString());
                    tmp.UserPrincipalName = r["UserPrincipalName"].ToString();
                    tmp.DistinguishedName = r["DistinguishedName"].ToString();
                    tmp.DisplayName = r["DisplayName"].ToString();
                    tmp.CompanyCode = r["CompanyCode"].ToString();
                    tmp.CompanyName = r["CompanyName"].ToString();
                    tmp.ResellerCode = r["ResellerCode"].ToString();
                    tmp.ResellerName = r["ResellerName"].ToString();

                    if (r["sAMAccountName"] != DBNull.Value)
                        tmp.sAMAccountName = r["sAMAccountName"].ToString();
                    else
                        tmp.sAMAccountName = "";

                    if (r["Firstname"] != DBNull.Value)
                        tmp.Firstname = r["Firstname"].ToString();
                    else
                        tmp.Firstname = "";

                    if (r["Middlename"] != DBNull.Value)
                        tmp.Middlename = r["MiddleName"].ToString();
                    else
                        tmp.Middlename = "";

                    if (r["Lastname"] != DBNull.Value)
                        tmp.Lastname = r["Lastname"].ToString();
                    else
                        tmp.Lastname = "";

                    if (r["Email"] != DBNull.Value)
                        tmp.Lastname = r["Email"].ToString();
                    else
                        tmp.Lastname = "";

                    if (r["Department"] != DBNull.Value)
                        tmp.Department = r["Department"].ToString();
                    else
                        tmp.Department = "";

                    if (r["IsResellerAdmin"] != DBNull.Value)
                        tmp.IsResellerAdmin = bool.Parse(r["IsResellerAdmin"].ToString());
                    else
                        tmp.IsResellerAdmin = false;

                    if (r["IsCompanyAdmin"] != DBNull.Value)
                        tmp.IsCompanyAdmin = bool.Parse(r["IsCompanyAdmin"].ToString());
                    else
                        tmp.IsCompanyAdmin = false;

                    if (r["MailboxPlan"] != DBNull.Value)
                        tmp.MailboxPlan = int.Parse(r["MailboxPlan"].ToString());
                    else
                        tmp.MailboxPlan = 0;

                    if (r["TSPlan"] != DBNull.Value)
                        tmp.TSPlan = int.Parse(r["TSPlan"].ToString());
                    else
                        tmp.TSPlan = 0;

                    if (r["LyncPlan"] != DBNull.Value)
                        tmp.LyncPlan = int.Parse(r["LyncPlan"].ToString());
                    else
                        tmp.LyncPlan = 0;

                    if (r["Created"] != DBNull.Value)
                        tmp.Created = DateTime.Parse(r["Created"].ToString());

                    if (r["AdditionalMB"] != DBNull.Value)
                        tmp.AdditionalMB = int.Parse(r["AdditionalMB"].ToString());
                    else
                        tmp.AdditionalMB = 0;

                    // Add to our collection
                    users.Add(tmp);
                }

                // Dispose
                r.Close();
                r.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }

            // Return our collection
            return users;
        }

        /// <summary>
        /// Adds a new user to SQL
        /// </summary>
        /// <param name="user"></param>
        public static void AddUser(ADUser user)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"INSERT INTO Users 
                                              (UserGuid, CompanyCode, sAMAccountName, UserPrincipalName, DistinguishedName, DisplayName, Firstname, Middlename,
                                               Lastname, Email, Department, IsResellerAdmin, IsCompanyAdmin, MailboxPlan, TSPlan, LyncPlan, Created, AdditionalMB) 
                                              VALUES 
                                              (@UserGuid, @CompanyCode, @sAMAccountName, @UserPrincipalName, @DistinguishedName, @DisplayName, @Firstname, @Middlename,
                                               @Lastname, @Email, @Department, @IsResellerAdmin, @IsCompanyAdmin, @MailboxPlan, @TSPlan, @LyncPlan, GETDATE(), 0)", sql);

            try
            {
                // Add our parameters
                cmd.Parameters.AddWithValue("@UserGuid", user.UserGuid);
                cmd.Parameters.AddWithValue("@CompanyCode", user.CompanyCode);
                cmd.Parameters.AddWithValue("@sAMAccountName", user.SamAccountName);
                cmd.Parameters.AddWithValue("@UserPrincipalName", user.UserPrincipalName);
                cmd.Parameters.AddWithValue("@DistinguishedName", user.DistinguishedName);
                cmd.Parameters.AddWithValue("@DisplayName", user.DisplayName);
                cmd.Parameters.AddWithValue("@Firstname", user.Firstname);
                cmd.Parameters.AddWithValue("@Middlename", string.IsNullOrEmpty(user.Middlename) ? "" : user.Middlename);
                cmd.Parameters.AddWithValue("@Lastname", string.IsNullOrEmpty(user.Lastname) ? "" : user.Lastname);
                cmd.Parameters.AddWithValue("@Email", string.Empty);
                cmd.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(user.Department) ? "" : user.Department);
                cmd.Parameters.AddWithValue("@IsResellerAdmin", user.IsResellerAdmin);
                cmd.Parameters.AddWithValue("@IsCompanyAdmin", user.IsCompanyAdmin);
                cmd.Parameters.AddWithValue("@MailboxPlan", 0);
                cmd.Parameters.AddWithValue("@TSPlan", 0);
                cmd.Parameters.AddWithValue("@LyncPlan", 0);

                // Open connection 
                sql.Open();

                // Insert data
                cmd.ExecuteNonQuery();

                // Close
                sql.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }
        }

        /// <summary>
        /// Updates a user in SQL
        /// </summary>
        /// <param name="user"></param>
        public static void UpdateUser(BaseUser user)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE Users SET
                                              DisplayName=@DisplayName, Firstname=@Firstname, Middlename=@Middlename, Lastname=@Lastname, Department=@Department,
                                              IsCompanyAdmin=@IsCompanyAdmin, IsResellerAdmin=@IsResellerAdmin WHERE UserPrincipalName=@UserPrincipalName", sql);
            try
            {
                // Add our parameters
                cmd.Parameters.AddWithValue("@UserPrincipalName", user.UserPrincipalName);
                cmd.Parameters.AddWithValue("@DisplayName", user.DisplayName);
                cmd.Parameters.AddWithValue("@Firstname", user.Firstname);
                cmd.Parameters.AddWithValue("@Middlename", user.Middlename);
                cmd.Parameters.AddWithValue("@Lastname", user.Lastname);
                cmd.Parameters.AddWithValue("@Department", user.Department);
                cmd.Parameters.AddWithValue("@IsCompanyAdmin", user.IsCompanyAdmin);
                cmd.Parameters.AddWithValue("@IsResellerAdmin", user.IsResellerAdmin);

                // Open connection 
                sql.Open();

                // Insert data
                cmd.ExecuteNonQuery();

                // Close
                sql.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }
        }

        /// <summary>
        /// Deletes a user from SQL
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <param name="companyCode"></param>
        public static void DeleteUser(string userPrincipalName, string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"DELETE FROM Users WHERE UserPrincipalName=@UserPrincipalName AND CompanyCode=@CompanyCode", sql);

            try
            {
                // Add our parameters
                cmd.Parameters.AddWithValue("@UserPrincipalName", userPrincipalName);
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                // Open connection 
                sql.Open();

                // Insert data
                cmd.ExecuteNonQuery();

                // Close
                sql.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }
        }

        /// <summary>
        /// Searches the system for users
        /// </summary>
        /// <param name="searchName"></param>
        /// <returns></returns>
        public static List<BaseSearchResults> SearchUsers(string searchName, string isResellerAdmin)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT
	                                                (SELECT CompanyName FROM Companies WHERE CompanyCode=u.CompanyCode) AS CompanyName,
                                                    (SELECT ResellerCode FROM Companies WHERE CompanyCode=u.CompanyCode) AS ResellerCode,	                                                
                                                    (SELECT CompanyName FROM Companies WHERE IsReseller=1 AND CompanyCode=(SELECT ResellerCode FROM Companies WHERE CompanyCode=u.CompanyCode)) AS ResellerName,
	                                                UserPrincipalName,
	                                                DisplayName,
	                                                CompanyCode
                                                FROM 
	                                                Users u
                                                WHERE 
	                                                (DisplayName LIKE @Search OR 
                                                    FirstName LIKE @Search OR 
                                                    LastName LIKE @Search OR
                                                    UserPrincipalName LIKE @Search)", sql);

            try
            {
                // Check if this is a reseller searching then limit their search to only their users
                if (!string.IsNullOrEmpty(isResellerAdmin))
                {
                    cmd.CommandText += " AND CompanyCode IN (SELECT CompanyCode FROM Companies WHERE ResellerCode=@ResellerCode)";
                    cmd.Parameters.AddWithValue("ResellerCode", isResellerAdmin);
                }

                // Create our object to return
                List<BaseSearchResults> returnedData = new List<BaseSearchResults>();

                // Add our parameters
                cmd.Parameters.AddWithValue("@Search", "%" + searchName + "%");

                // Open connection 
                sql.Open();

                // Start reading data
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        BaseSearchResults tmp = new BaseSearchResults();
                        tmp.UserPrincipalName = r["UserPrincipalName"].ToString();
                        tmp.DisplayName = r["DisplayName"].ToString();

                        if (r["ResellerName"] != DBNull.Value)
                            tmp.ResellerName = r["ResellerName"].ToString();
                        else
                            tmp.ResellerName = "Unknown";

                        if (r["CompanyName"] != DBNull.Value)
                            tmp.CompanyName = r["CompanyName"].ToString();
                        else
                            tmp.CompanyName = "Unknown";

                        returnedData.Add(tmp);
                    }
                }

                // Close
                sql.Close();

                // Return data
                return returnedData;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="plan"></param>
        public static void UpdateUserMailbox(MailboxUser user, MailboxPlan plan)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE Users SET 
                                                MailboxPlan=@MailboxPlan, 
                                                Email=@Email, 
                                                AdditionalMB=@AdditionalMB
                                              WHERE 
                                                UserPrincipalName=@UserPrincipalName", sql);

            try
            {
                // Check if the ActiveSync plan is not null
                if (!string.IsNullOrEmpty(user.ActiveSyncMailboxPolicy))
                {
                    cmd.CommandText = @"UPDATE Users SET 
                                                MailboxPlan=@MailboxPlan, 
                                                Email=@Email, 
                                                AdditionalMB=@AdditionalMB, 
                                                ActiveSyncPlan=(SELECT TOP 1 ASID FROM Plans_ExchangeActiveSync WHERE DisplayName=@ActiveSyncPlan)
                                              WHERE 
                                                UserPrincipalName=@UserPrincipalName";
                    cmd.Parameters.AddWithValue("@ActiveSyncPlan", user.ActiveSyncMailboxPolicy);
                }
                else
                    cmd.Parameters.AddWithValue("@ActiveSyncPlan", DBNull.Value);

                // Add our parameters
                cmd.Parameters.AddWithValue("@MailboxPlan", plan.PlanID);
                cmd.Parameters.AddWithValue("@Email", user.PrimarySmtpAddress);
                cmd.Parameters.AddWithValue("@AdditionalMB", plan.AdditionalMBAdded);
                cmd.Parameters.AddWithValue("@UserPrincipalName", user.UserPrincipalName);

                // Open connection 
                sql.Open();

                // Insert data
                cmd.ExecuteNonQuery();

                // Close
                sql.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }
        }

        /// <summary>
        /// Used for the import feature
        /// </summary>
        /// <param name="user"></param>
        /// <param name="plan"></param>
        public static void UpdateUserMailbox(ADUser user, MailboxPlan plan)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE Users SET 
                                                MailboxPlan=@MailboxPlan,
                                                AdditionalMB=@AdditionalMB
                                              WHERE 
                                                UserPrincipalName=@UserPrincipalName", sql);

            try
            {
                // Add our parameters
                cmd.Parameters.AddWithValue("@MailboxPlan", plan.PlanID);
                cmd.Parameters.AddWithValue("@AdditionalMB", plan.AdditionalMBAdded);
                cmd.Parameters.AddWithValue("@UserPrincipalName", user.UserPrincipalName);

                // Open connection 
                sql.Open();

                // Insert data
                cmd.ExecuteNonQuery();

                // Close
                sql.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }
        }
    }
}
