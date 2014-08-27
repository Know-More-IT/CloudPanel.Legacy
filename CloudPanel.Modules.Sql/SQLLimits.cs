using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Sql
{
    public class SQLLimits
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SQLLimits)); 

        /// <summary>
        /// Checks if a company is at the mailbox limit
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static bool IsCompanyAtMailboxLimit(string companyCode, int add = 0)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT CompanyCode,
                                            (SELECT MaxExchangeMailboxes FROM Plans_Organization WHERE OrgPlanID=(SELECT OrgPlanID FROM Companies WHERE CompanyCode=Users.CompanyCode)) AS MaxMailboxes, 
                                            SUM(case when MailboxPlan > 0 THEN 1 ELSE 0 END) AS CurrentMailboxes 
                                            FROM Users WHERE CompanyCode=@CompanyCode GROUP BY Users.CompanyCode", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // If we are at max
                bool isAtMax = true;

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        int max = int.Parse(r["MaxMailboxes"].ToString());
                        int current = int.Parse(r["CurrentMailboxes"].ToString()) + add;

                        if (current < max)
                            isAtMax = false;
                    }
                }
                else
                    return false;

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return
                return isAtMax;
            }
            catch (Exception ex)
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
        /// Checks if a company is at the user limit
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static bool IsCompanyAtUserLimit(string companyCode, int add = 0)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT CompanyCode,
                                            (SELECT MaxUsers FROM Plans_Organization WHERE OrgPlanID=(SELECT OrgPlanID FROM Companies WHERE CompanyCode=Users.CompanyCode)) AS MaxUsers, 
                                            COUNT(UserPrincipalName) AS CurrentUsers
                                            FROM Users WHERE CompanyCode=@CompanyCode GROUP BY Users.CompanyCode", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // If we are at max
                bool isAtMax = true;

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        int max = int.Parse(r["MaxUsers"].ToString());
                        int current = int.Parse(r["CurrentUsers"].ToString()) + add;

                        if (current < max)
                            isAtMax = false;
                    }
                }
                else
                    return false;

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return
                return isAtMax;
            }
            catch (Exception ex)
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
        /// Checks if a company is at the domain limit
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static bool IsCompanyAtDomainLimit(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT COUNT(Domain) AS CurrentDomains,
                                                (SELECT MaxDomains FROM Plans_Organization WHERE OrgPlanID=(SELECT OrgPlanID FROM Companies WHERE CompanyCode=@CompanyCode AND IsReseller=0)) AS MaxDomains
                                                FROM Domains WHERE CompanyCode=@CompanyCode", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // If we are at max
                bool isAtMax = true;

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        int max = int.Parse(r["MaxDomains"].ToString());
                        int current = int.Parse(r["CurrentDomains"].ToString());

                        if (current < max)
                            isAtMax = false;
                    }
                }
                else
                    return false;

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return
                return isAtMax;
            }
            catch (Exception ex)
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
        /// Checks if the company is at the contact limit
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static bool IsCompanyAtContactLimit(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT
                                            (SELECT MaxExchangeContacts FROM Plans_Organization WHERE OrgPlanID=Companies.OrgPlanID) AS MaxContacts,
                                            (SELECT COUNT(*) FROM Contacts WHERE CompanyCode='KNO') AS CurrentContacts
                                            FROM Companies WHERE CompanyCode=@CompanyCode GROUP BY OrgPlanID", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // If we are at max
                bool isAtMax = true;

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        int max = int.Parse(r["MaxContacts"].ToString());
                        int current = int.Parse(r["CurrentContacts"].ToString());

                        if (current < max)
                            isAtMax = false;
                    }
                }
                else
                    return false;

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return
                return isAtMax;
            }
            catch (Exception ex)
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
        /// Checks if the company is at the distribution group limit
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static bool IsCompanyAtDistGroupLimit(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT
                                            (SELECT MaxExchangeDistLists FROM Plans_Organization WHERE OrgPlanID=Companies.OrgPlanID) AS MaxGroups,
                                            (SELECT COUNT(*) FROM DistributionGroups WHERE CompanyCode=@CompanyCode) AS CurrentGroups
                                            FROM Companies WHERE CompanyCode=@CompanyCode GROUP BY OrgPlanID", sql);

            try
            {
                logger.DebugFormat("Checking if company is at the distributiong group limit for {0}", companyCode);

                // Add parameters
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // If we are at max
                bool isAtMax = true;

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        int max = int.Parse(r["MaxGroups"].ToString());
                        int current = int.Parse(r["CurrentGroups"].ToString());

                        if (current < max)
                            isAtMax = false;
                    }
                }
                else
                    return false;

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return
                return isAtMax;
            }
            catch (Exception ex)
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
