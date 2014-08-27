using CloudPanel.Modules.Base.Class;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Sql
{
    public class ExchangeSql
    {
        /// <summary>
        /// Gets a list of current mailboxes for the company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<MailboxUser> Get_MailboxUsersForCompany(string companyCode)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT  DisplayName,
                                               UserPrincipalName,
	                                           Email,
	                                           Department,
	                                           sAMAccountName,
	                                           (SELECT TOP 1 TotalItemSizeInKB FROM SvcMailboxSizes s WHERE s.UserPrincipalName=u.UserPrincipalName ORDER BY Retrieved DESC) AS TotalItemSize,
	                                           p.MailboxPlanName,
	                                           p.MailboxSizeMB,
                                               AdditionalMB
                                        FROM Users u
                                        INNER JOIN Plans_ExchangeMailbox p ON p.MailboxPlanID=u.MailboxPlan
                                        WHERE u.MailboxPlan > 0 AND u.CompanyCode=@CompanyCode
                                        ORDER BY u.DisplayName", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // Create our object to return
                List<MailboxUser> mailboxUsers = new List<MailboxUser>();

                // Read the data
                r = cmd.ExecuteReader();
                while (r.Read())
                {
                    MailboxUser tmp = new MailboxUser();
                    tmp.DisplayName         = r["DisplayName"].ToString();
                    tmp.UserPrincipalName   = r["UserPrincipalName"].ToString();
                    tmp.PrimarySmtpAddress  = r["Email"].ToString();
                    tmp.Department          = r["Department"] == DBNull.Value ? "" : r["Department"].ToString();
                    tmp.SamAccountName      = r["sAMAccountName"] == DBNull.Value ? "" : r["sAMAccountName"].ToString();
                    tmp.TotalItemSizeInKB   = r["TotalItemSize"] == DBNull.Value ? "0" : r["TotalItemSize"].ToString();
                    tmp.MailboxPlanName     = r["MailboxPlanName"].ToString();
                    tmp.MailboxSizeInMB     = int.Parse(r["MailboxSizeMB"].ToString());

                    if (r["AdditionalMB"] == DBNull.Value)
                        tmp.AdditionalMB = 0;
                    else
                        tmp.AdditionalMB = int.Parse(r["AdditionalMB"].ToString());


                    mailboxUsers.Add(tmp);
                }

                // Return our object
                return mailboxUsers;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (r != null)
                    r.Dispose();

                if (cmd != null)
                    cmd.Dispose();

                if (sql != null)
                    sql.Dispose();
            }
        }

        /// <summary>
        /// Gets a list of users that do not have mailboxes for a company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<ADUser> Get_NonMailboxUsersForCompany(string companyCode)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT  DisplayName,
                                               Firstname,
	                                           Lastname,
	                                           UserPrincipalName,
                                               Department,
                                               Created
                                        FROM Users
                                        WHERE CompanyCode=@CompanyCode AND (MailboxPlan = 0 OR MailboxPlan IS NULL)
                                        ORDER BY DisplayName", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // Create our object to return
                List<ADUser> users = new List<ADUser>();

                // Read the data
                r = cmd.ExecuteReader();
                while (r.Read())
                {
                    ADUser tmp = new ADUser();
                    tmp.DisplayName = r["DisplayName"].ToString();
                    tmp.Firstname = r["Firstname"].ToString();
                    tmp.Lastname = r["Lastname"] == DBNull.Value ? "" : r["Lastname"].ToString();
                    tmp.UserPrincipalName = r["UserPrincipalName"].ToString();
                    tmp.Department = r["Department"] == DBNull.Value ? "" : r["Department"].ToString();

                    if (r["Created"] == DBNull.Value)
                        tmp.Created = null;
                    else
                        tmp.Created = DateTime.Parse(r["Created"].ToString());

                    users.Add(tmp);
                }

                // Return our object
                return users;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (r != null)
                    r.Dispose();

                if (cmd != null)
                    cmd.Dispose();

                if (sql != null)
                    sql.Dispose();
            }
        }
    }
}
