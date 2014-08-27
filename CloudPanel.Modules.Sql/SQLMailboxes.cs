using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudPanel.Modules.Base;
using System.Data.SqlClient;
using System.Configuration;
using CloudPanel.Modules.Base.Class;
using log4net;
using System.Reflection;
using System.Globalization;

namespace CloudPanel.Modules.Sql
{
    public class SQLMailboxes
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Gets a list of users and information from SQL for a specific company
        /// (For the mailbox users display)
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<MailboxUser> GetMailboxUsers(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT
		                                            u.ID,
		                                            u.UserGuid,
                                                    u.DistinguishedName,
		                                            u.UserPrincipalName,
		                                            u.Department,
		                                            u.DisplayName,
		                                            u.Email,
		                                            u.AdditionalMB,
                                                    u.sAMAccountName,
		                                            p.MailboxPlanName,
		                                            p.MailboxSizeMB,
		                                            p.MaxMailboxSizeMB,
                                                    u.ActiveSyncPlan,
		                                            (SELECT TOP 1 TotalItemSizeInKB FROM SvcMailboxSizes WHERE UserPrincipalName=u.UserPrincipalName ORDER BY Retrieved DESC) AS TotalItemSizeInKB
                                            FROM
	                                            Users u
                                            INNER JOIN
	                                            Plans_ExchangeMailbox p ON p.MailboxPlanID = u.MailboxPlan
                                            WHERE
	                                            u.CompanyCode=@CompanyCode AND u.MailboxPlan > 0
                                            ORDER BY
	                                            u.DisplayName", sql);

            // Initialize list
            List<MailboxUser> users = new List<MailboxUser>();

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                // Open Connection
                sql.Open();

                // Read all data
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        MailboxUser tmp = new MailboxUser();
                        tmp.UserID = int.Parse(r["ID"].ToString());
                        tmp.UserGuid = Guid.Parse(r["UserGuid"].ToString());
                        tmp.UserPrincipalName = r["UserPrincipalName"].ToString();
                        tmp.MailboxPlanName = r["MailboxPlanName"].ToString();
                        tmp.DisplayName = r["DisplayName"].ToString();

                        if (r["sAMAccountName"] != DBNull.Value)
                            tmp.SamAccountName = r["sAMAccountName"].ToString();
                        else
                            tmp.SamAccountName = "";

                        if (r["DistinguishedName"] != DBNull.Value)
                            tmp.DistinguishedName = r["DistinguishedName"].ToString().ToUpper(); // Upper case to compare with other lists
                        else
                            tmp.DistinguishedName = "";

                        if (r["Department"] == DBNull.Value)
                            tmp.Department = "";
                        else
                            tmp.Department = r["Department"].ToString();

                        if (r["AdditionalMB"] != DBNull.Value)
                            tmp.AdditionalMB = int.Parse(r["AdditionalMB"].ToString());
                        else
                            tmp.AdditionalMB = 0;

                        if (r["Email"] != DBNull.Value)
                            tmp.PrimarySmtpAddress = r["Email"].ToString();
                        else
                            tmp.PrimarySmtpAddress = "";

                        // Calcuate the current mailbox size
                        int current = int.Parse(r["MailboxSizeMB"].ToString());
                        int total = current + tmp.AdditionalMB;
                        decimal totalInGB = (decimal)total / 1024;

                        //tmp.MailboxSizeFormatted = string.Format("{0:##.##}{1}", totalInGB, "GB");

                        // Get the Exchange mailbox size information
                        if (r["TotalItemSizeInKB"] != DBNull.Value)
                            tmp.TotalItemSizeInKB = FormatBytes(r["TotalItemSizeInKB"].ToString());
                        else
                            tmp.TotalItemSizeInKB = "Unknown";

                        // Add to collections
                        users.Add(tmp);
                    }
                }

                // Close and dispose
                r.Close();
                r.Dispose();

                // Return list
                return users;
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
        /// Gets a list of users for a specific company that do not have a mailbox
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<BaseUser> GetNonMailboxUsers(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE CompanyCode=@CompanyCode AND (MailboxPlan=0 OR MailboxPlan IS NULL) ORDER BY DisplayName", sql);

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
                    tmp.CompanyCode = companyCode;

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
                        tmp.Email = r["Email"].ToString();
                    else
                        tmp.Email = "";

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

                    // Add to our collection
                    users.Add(tmp);
                }

                // Dispose
                r.Close();
                r.Dispose();
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

            // Return our collection
            return users;
        }

        /// <summary>
        /// Disables a user's mailbox in SQL
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <param name="companyCode"></param>
        public static void DisableMailbox(string userPrincipalName, string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("UPDATE Users SET MailboxPlan=@MailboxPlan WHERE UserPrincipalName=@UserPrincipalName AND CompanyCode=@CompanyCode", sql);
            
            try
            {
                // Add our parameter
                cmd.Parameters.AddWithValue("@UserPrincipalName", userPrincipalName);
                cmd.Parameters.AddWithValue("@MailboxPlan", DBNull.Value);
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                // Open connection 
                sql.Open();

                // Execute query
                cmd.ExecuteNonQuery();

                // Close connection
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
        /// Adds a resource mailbox to the database
        /// </summary>
        /// <param name="mbx"></param>
        public static void AddResourceMailbox(ResourceMailbox mbx, int mailboxPlanID, int additionalMB)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"INSERT INTO ResourceMailboxes 
                                            (DisplayName, CompanyCode, UserPrincipalName, PrimarySmtpAddress, ResourceType, MailboxPlan, AdditionalMB) 
                                            VALUES  
                                            (@DisplayName, @CompanyCode, @UserPrincipalName, @PrimarySmtpAddress, @ResourceType, @MailboxPlan, @AdditionalMB)", sql);


            try
            {
                // Add our parameter
                cmd.Parameters.AddWithValue("@DisplayName", mbx.DisplayName);
                cmd.Parameters.AddWithValue("@CompanyCode", mbx.CompanyCode);
                cmd.Parameters.AddWithValue("@UserPrincipalName", mbx.UserPrincipalName);
                cmd.Parameters.AddWithValue("@PrimarySmtpAddress", mbx.PrimarySmtpAddress);
                cmd.Parameters.AddWithValue("@ResourceType", mbx.ResourceType);
                cmd.Parameters.AddWithValue("@MailboxPlan", mbx.MailboxPlanID);
                cmd.Parameters.AddWithValue("@AdditionalMB", additionalMB);

                // Open connection 
                sql.Open();

                // Insert data
                cmd.ExecuteNonQuery();
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
        /// Updates a resource mailbox
        /// </summary>
        /// <param name="mbx"></param>
        /// <param name="mailboxPlanID"></param>
        /// <param name="additionalMB"></param>
        public static void UpdateResourceMailbox(ResourceMailbox mbx, int mailboxPlanID, int additionalMB)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE ResourceMailboxes SET DisplayName=@DisplayName, PrimarySmtpAddress=@PrimarySmtpAddress, MailboxPlan=@MailboxPlan, AdditionalMB=@AdditionalMB WHERE UserPrincipalName=@UserPrincipalName", sql);


            try
            {
                logger.Debug("Updating resource mailbox in SQL for " + mbx.UserPrincipalName);

                // Add our parameter
                cmd.Parameters.AddWithValue("@DisplayName", mbx.DisplayName);
                cmd.Parameters.AddWithValue("@UserPrincipalName", mbx.UserPrincipalName);
                cmd.Parameters.AddWithValue("@PrimarySmtpAddress", mbx.PrimarySmtpAddress);
                cmd.Parameters.AddWithValue("@MailboxPlan", mbx.MailboxPlanID);
                cmd.Parameters.AddWithValue("@AdditionalMB", additionalMB);

                // Open connection 
                sql.Open();

                // Update data
                cmd.ExecuteNonQuery();
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
        /// Gets a list of resource mailboxes
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<ResourceMailbox> GetResourceMailboxes(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM ResourceMailboxes WHERE CompanyCode=@CompanyCode ORDER BY ResourceType,DisplayName", sql);

            // Create our collection
            List<ResourceMailbox> mailboxes = new List<ResourceMailbox>();

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
                    ResourceMailbox tmp = new ResourceMailbox();
                    tmp.DisplayName = r["DisplayName"].ToString();
                    tmp.CompanyCode = r["CompanyCode"].ToString();
                    tmp.UserPrincipalName = r["UserPrincipalName"].ToString();
                    tmp.PrimarySmtpAddress = r["PrimarySmtpAddress"].ToString();
                    tmp.ResourceType = r["ResourceType"].ToString();

                    

                    mailboxes.Add(tmp);
                }

                // Dispose
                r.Close();
                r.Dispose();

                return mailboxes;
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
        /// Gets a specific resource mailbox
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public static ResourceMailbox GetResourceMailbox(string userPrincipalName)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM ResourceMailboxes WHERE UserPrincipalName=@UserPrincipalName", sql);

            ResourceMailbox mailbox = new ResourceMailbox();

            try
            {
                // Add our parameter
                cmd.Parameters.AddWithValue("@UserPrincipalName", userPrincipalName);

                // Open connection 
                sql.Open();

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    mailbox.DisplayName = r["DisplayName"].ToString();
                    mailbox.CompanyCode = r["CompanyCode"].ToString();
                    mailbox.UserPrincipalName = r["UserPrincipalName"].ToString();
                    mailbox.PrimarySmtpAddress = r["PrimarySmtpAddress"].ToString();
                    mailbox.ResourceType = r["ResourceType"].ToString();
                    mailbox.MailboxPlanID = int.Parse(r["MailboxPlan"].ToString());
                    mailbox.AdditionalMB = int.Parse(r["AdditionalMB"].ToString());
                }

                // Dispose
                r.Close();
                r.Dispose();

                return mailbox;
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
        /// Removes a resource mailbox from the database
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <param name="companyCode"></param>
        public static void RemoveResourceMailbox(string userPrincipalName, string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("DELETE FROM ResourceMailboxes WHERE UserPrincipalName=@UserPrincipalName AND CompanyCode=@CompanyCode", sql);

            try
            {
                // Add our parameters
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
                cmd.Parameters.AddWithValue("@UserPrincipalName", userPrincipalName);

                // Open connection 
                sql.Open();

                // Delete data
                cmd.ExecuteNonQuery();
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
        /// Formats KB
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string FormatBytes(string kiloBytes)
        {
            const int scale = 1024;

            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            
            decimal bytes = decimal.Parse(kiloBytes, CultureInfo.InvariantCulture) * 1024;
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##}{1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }

            return "Unknown";
        }
    }
}
