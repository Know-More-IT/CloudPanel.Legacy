using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Base.Class;

namespace CloudPanel.Modules.Sql
{
    public class SQLStatistics
    {
        /// <summary>
        /// Get lines chart information
        /// </summary>
        /// <returns></returns>
        public static List<Dictionary<string, object>> GetSuperAdminLineChart()
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT StatDate, UserCount FROM Stats_UserCount WHERE StatDate >= DATEADD(year, -1, GetDate()) ORDER BY StatDate ASC;
                                              SELECT StatDate, UserCount FROM Stats_ExchCount WHERE StatDate >= DATEADD(year, -1, GetDate()) ORDER BY StatDate ASC;
                                              SELECT StatDate, UserCount FROM Stats_CitrixCount WHERE StatDate >= DATEADD(year, -1, GetDate()) ORDER BY StatDate ASC;", sql);

            try
            {
                // Open connection to database
                sql.Open();

                // Keep track months to years
                Dictionary<string, object> userMonthYearValue = new Dictionary<string, object>();
                Dictionary<string, object> exchMonthYearValue = new Dictionary<string, object>();
                Dictionary<string, object> citrixMonthYearValue = new Dictionary<string, object>();

                // Formatter for months
                DateTimeFormatInfo mfi = new DateTimeFormatInfo();

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        DateTime statDate = DateTime.Parse(r["StatDate"].ToString());
                        int value = int.Parse(r["UserCount"].ToString());

                        string keyValue = mfi.GetAbbreviatedMonthName(statDate.Month) + " " + statDate.Year;

                        if (!userMonthYearValue.ContainsKey(keyValue))
                        {
                            // Add to our dictionary
                            userMonthYearValue.Add(keyValue, r["UserCount"]);
                        }
                        else
                        {
                            // Update our dictionary (because we want the last value in the month)
                            userMonthYearValue[keyValue] = r["UserCount"];
                        }
                    }

                    // Next result set
                    r.NextResult();

                    if (r.HasRows)
                    {
                        while (r.Read())
                        {
                            DateTime statDate = DateTime.Parse(r["StatDate"].ToString());
                            int value = int.Parse(r["UserCount"].ToString());

                            string keyValue = mfi.GetAbbreviatedMonthName(statDate.Month) + " " + statDate.Year;

                            if (!exchMonthYearValue.ContainsKey(keyValue))
                            {
                                // Add to our dictionary
                                exchMonthYearValue.Add(keyValue, r["UserCount"]);
                            }
                            else
                            {
                                // Update our dictionary (because we want the last value in the month)
                                exchMonthYearValue[keyValue] = r["UserCount"];
                            }
                        }
                    }

                    // Last result set
                    r.NextResult();

                    if (r.HasRows)
                    {
                        while (r.Read())
                        {
                            DateTime statDate = DateTime.Parse(r["StatDate"].ToString());
                            int value = int.Parse(r["UserCount"].ToString());

                            string keyValue = mfi.GetAbbreviatedMonthName(statDate.Month) + " " + statDate.Year;

                            if (!citrixMonthYearValue.ContainsKey(keyValue))
                            {
                                // Add to our dictionary
                                citrixMonthYearValue.Add(keyValue, r["UserCount"]);
                            }
                            else
                            {
                                // Update our dictionary (because we want the last value in the month)
                                citrixMonthYearValue[keyValue] = r["UserCount"];
                            }
                        }
                    }
                }


                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return our custom object
                return new List<Dictionary<string, object>>() { userMonthYearValue, exchMonthYearValue, citrixMonthYearValue };
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
        /// Gets the information for the bar chart
        /// </summary>
        /// <returns></returns>
        public static List<MailboxDatabase> GetSuperAdminDbSizeChart()
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT
	                                                DatabaseName,
	                                                DatabaseSize,
	                                                Retrieved
                                                FROM
	                                                SvcMailboxDatabaseSizes
                                                WHERE
	                                                Retrieved IN (SELECT MAX(Retrieved) FROM SvcMailboxDatabaseSizes) AND Retrieved >= DATEADD(d, -30, getdate())", sql);

            try
            {
                // Open connection to database
                sql.Open();

                // Our data to return to the calling method
                List<MailboxDatabase> mdbStats = new List<MailboxDatabase>();

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        mdbStats.Add(new MailboxDatabase()
                        {
                            Identity = r["DatabaseName"].ToString(),
                            DatabaseSize = r["DatabaseSize"].ToString(),
                            WhenRetrieved = DateTime.Parse(r["Retrieved"].ToString())
                        });
                    }
                }


                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return our custom object
                return mdbStats;
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
        /// <param name="isSuperAdmin"></param>
        /// <param name="isResellerAdmin"></param>
        /// <returns></returns>
        public BaseDashboard GetDashboardStatistics(bool isSuperAdmin, bool isResellerAdmin, string resellerOrCompanyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("", sql);

            if (isSuperAdmin)
            {
                cmd.CommandText = @"SELECT COUNT(*) AS UserCount FROM Users; 
                                    SELECT COUNT(*) AS MailboxCount FROM Users WHERE MailboxPlan IS NOT NULL AND MailboxPlan > 0;
                                    SELECT COUNT(DISTINCT(CompanyCode)) AS CompanyCount FROM Users;
                                    SELECT COUNT(UserPrincipalName) AS CitrixCount FROM Users WHERE ID IN (SELECT UserID FROM UserPlansCitrix);
                                    SELECT COUNT(Domain) AS DomainCount FROM Domains;
                                    SELECT COUNT(Domain) AS AcceptedDomainCount FROM Domains WHERE IsAcceptedDomain=1;";
            }
            else if (isResellerAdmin)
            {
                cmd.CommandText = @"SELECT COUNT(*) AS UserCount FROM Users WHERE CompanyCode IN (SELECT CompanyCode FROM Companies WHERE ResellerCode=@ResellerCode); 
                                    SELECT COUNT(*) AS MailboxCount FROM Users WHERE (MailboxPlan IS NOT NULL AND MailboxPlan > 0) AND CompanyCode IN (SELECT CompanyCode FROM Companies WHERE ResellerCode=@ResellerCode);
                                    SELECT COUNT(DISTINCT(CompanyCode)) AS CompanyCount FROM Users WHERE CompanyCode IN (SELECT CompanyCode FROM Companies WHERE ResellerCode=@ResellerCode);
                                    SELECT COUNT(UserPrincipalName) AS CitrixCount FROM Users WHERE CompanyCode IN (SELECT CompanyCode From Companies WHERE ResellerCode=@ResellerCode) AND ID IN (SELECT UserID FROM UserPlansCitrix);
                                    SELECT COUNT(Domain) AS DomainCount FROM Domains WHERE CompanyCode IN (SELECT CompanyCode FROM Companies WHERE ResellerCode=@ResellerCode);
                                    SELECT COUNT(Domain) AS AcceptedDomainCount FROM Domains WHERE CompanyCode IN (SELECT CompanyCode FROM Companies WHERE ResellerCode=@ResellerCode) AND IsAcceptedDomain=1;";
                cmd.Parameters.AddWithValue("ResellerCode", resellerOrCompanyCode);
            }
            else
            {
                cmd.CommandText = @"SELECT COUNT(*) AS UserCount FROM Users WHERE CompanyCode=@CompanyCode; 
                                    SELECT COUNT(*) AS MailboxCount FROM Users WHERE (MailboxPlan IS NOT NULL AND MailboxPlan > 0) AND CompanyCode=@CompanyCode;
                                    SELECT COUNT(DISTINCT(CompanyCode)) AS CompanyCount FROM Users WHERE CompanyCode=@CompanyCode;
                                    SELECT COUNT(UserPrincipalName) AS CitrixCount FROM Users WHERE CompanyCode=@CompanyCode AND ID IN (SELECT UserID FROM UserPlansCitrix);
                                    SELECT COUNT(Domain) AS DomainCount FROM Domains WHERE CompanyCode=@CompanyCode;
                                    SELECT COUNT(Domain) AS AcceptedDomainCount FROM Domains WHERE CompanyCode=@CompanyCode AND IsAcceptedDomain=1;";
                cmd.Parameters.AddWithValue("CompanyCode", resellerOrCompanyCode);
            }

            BaseDashboard dashboardStats = new BaseDashboard();
            try
            {
                // Open connection to database
                sql.Open();

                // Read Data
                SqlDataReader r = cmd.ExecuteReader();
                do
                {
                    while (r.Read())
                    {
                        if (r.GetName(0).Equals("UserCount"))
                            dashboardStats.UserCount = int.Parse(r["UserCount"].ToString());
                        else if (r.GetName(0).Equals("MailboxCount"))
                            dashboardStats.MailboxCount = int.Parse(r["MailboxCount"].ToString());
                        else if (r.GetName(0).Equals("CompanyCount"))
                            dashboardStats.CompanyCount = int.Parse(r["CompanyCount"].ToString());
                        else if (r.GetName(0).Equals("CitrixCount"))
                            dashboardStats.CitrixCount = int.Parse(r["CitrixCount"].ToString());
                        else if (r.GetName(0).Equals("DomainCount"))
                            dashboardStats.DomainCount = int.Parse(r["DomainCount"].ToString());
                        else if (r.GetName(0).Equals("AcceptedDomainCount"))
                            dashboardStats.AcceptedDomainCount = int.Parse(r["AcceptedDomainCount"].ToString());
                    }
                }
                while (r.NextResult());

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return
                return dashboardStats;
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
        /// Get statistics for a specific company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="users"></param>
        /// <param name="domains"></param>
        /// <param name="contacts"></param>
        /// <param name="mailboxes"></param>
        /// <param name="groups"></param>
        public void GetCompanyStatistics(string companyCode, out int users, out int domains, out int contacts, out int mailboxes, out int groups)
        {
            // Set default values
            users = 0;
            domains = 0;
            contacts = 0;
            mailboxes = 0;
            groups = 0;

            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT COUNT(*) AS UserCount FROM Users WHERE CompanyCode=@CompanyCode;
                                              SELECT COUNT(*) AS DomainCount FROM Domains WHERE CompanyCode=@CompanyCode;
                                              SELECT COUNT(*) AS MailboxCount FROM Users WHERE CompanyCode=@CompanyCode AND MailboxPlan > 0;
                                              SELECT COUNT(*) AS ContactCount FROM Contacts WHERE CompanyCode=@CompanyCode;
                                              SELECT COUNT(*) AS GroupCount FROM DistributionGroups WHERE CompanyCode=@CompanyCode;", sql);

            try
            {
                // Add parameter
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                // Open
                sql.Open();

                // Initilize reader
                SqlDataReader r = cmd.ExecuteReader();

                // Read user count
                r.Read();
                users = int.Parse(r["UserCount"].ToString());

                // Read domain count
                r.NextResult();
                r.Read();
                domains = int.Parse(r["DomainCount"].ToString());

                // Read mailbox count
                r.NextResult();
                r.Read();
                mailboxes = int.Parse(r["MailboxCount"].ToString());

                // Read Contact count
                r.NextResult();
                r.Read();
                contacts = int.Parse(r["ContactCount"].ToString());

                // Read Group count
                r.NextResult();
                r.Read();
                groups = int.Parse(r["GroupCount"].ToString());

                // Close and dispose reader
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
        }

        /// <summary>
        /// Get the top 5 largest companies for resellers and super admins
        /// </summary>
        /// <param name="isReseller"></param>
        /// <param name="resellerCode"></param>
        public static BaseLargestCustomers GetLargestCustomers(bool isReseller, string resellerCode = null)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("", sql);

            BaseLargestCustomers largest = new BaseLargestCustomers();
            largest.CompanyNames = new List<string>();
            largest.UserCount = new List<object>();
            largest.MailboxCount = new List<object>();

            if (!isReseller)
            {
                cmd.CommandText = @"SELECT TOP 5 (SELECT CompanyName FROM Companies WHERE CompanyCode=Users.CompanyCode) AS CompanyName, 
                                    COUNT(*) AS UserCount, 
                                    SUM(CASE WHEN MailboxPlan > 0 THEN 1 ELSE 0 END) AS MailboxCount 
                                    FROM Users GROUP BY CompanyCode ORDER BY UserCount DESC";
            }
            else
            {
                cmd.CommandText = @"SELECT TOP 5 (SELECT CompanyName FROM Companies WHERE CompanyCode=Users.CompanyCode) AS CompanyName, 
                                    COUNT(*) AS UserCount, 
                                    SUM(CASE WHEN MailboxPlan > 0 THEN 1 ELSE 0 END) AS MailboxCount 
                                    FROM Users WHERE CompanyCode IN (SELECT CompanyCode FROM Companies WHERE ResellerCode=@ResellerCode) 
                                    GROUP BY CompanyCode ORDER BY UserCount DESC";
                cmd.Parameters.AddWithValue("ResellerCode", resellerCode);
            }

            try
            {
                // Open connection
                sql.Open();

                // Read Data
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    largest.CompanyNames.Add(r["CompanyName"].ToString());
                    largest.UserCount.Add(int.Parse(r["UserCount"].ToString()));
                    largest.MailboxCount.Add(int.Parse(r["MailboxCount"].ToString()));
                }

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return our object
                return largest;
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
        /// Gets the total Exchange assigned in TB, GB, or MB
        /// </summary>
        /// <param name="isSuperAdmin"></param>
        /// <param name="isResellerAdmin"></param>
        /// <param name="isCompanyAdmin"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetTotalAssignedExchangeStorage(bool isSuperAdmin, bool isResellerAdmin, bool isCompanyAdmin, string code)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("", sql);

            if (isSuperAdmin)
            {
                cmd.CommandText = @"SELECT UserPrincipalName, MailboxPlan, AdditionalMB, 
                                    (SELECT MailboxSizeMB FROM Plans_ExchangeMailbox WHERE MailboxPlanID=Users.MailboxPlan) AS MailboxPlanSize,
                                    ((SELECT MailboxSizeMB FROM Plans_ExchangeMailbox WHERE MailboxPlanID=Users.MailboxPlan) + AdditionalMB) AS TotalSize
                                     FROM Users WHERE MailboxPlan > 0";
            }
            else if (isResellerAdmin)
            {
                cmd.CommandText = @"SELECT UserPrincipalName, MailboxPlan, AdditionalMB, 
                                    (SELECT MailboxSizeMB FROM Plans_ExchangeMailbox WHERE MailboxPlanID=Users.MailboxPlan) AS MailboxPlanSize,
                                    ((SELECT MailboxSizeMB FROM Plans_ExchangeMailbox WHERE MailboxPlanID=Users.MailboxPlan) + AdditionalMB) AS TotalSize
                                     FROM Users WHERE MailboxPlan > 0 AND CompanyCode IN (SELECT CompanyCode FROM Companies WHERE ResellerCode=@ResellerCode)";
                cmd.Parameters.AddWithValue("ResellerCode", code);
            }
            else
            {
                cmd.CommandText = @"SELECT UserPrincipalName, MailboxPlan, AdditionalMB, 
                                    (SELECT MailboxSizeMB FROM Plans_ExchangeMailbox WHERE MailboxPlanID=Users.MailboxPlan) AS MailboxPlanSize,
                                    ((SELECT MailboxSizeMB FROM Plans_ExchangeMailbox WHERE MailboxPlanID=Users.MailboxPlan) + AdditionalMB) AS TotalSize
                                     FROM Users WHERE MailboxPlan > 0 AND CompanyCode=@CompanyCode";
                cmd.Parameters.AddWithValue("CompanyCode", code);
            }

            try
            {
                // Open connection
                sql.Open();

                // Keep track of total
                decimal total = 0;

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (r["TotalSize"] == DBNull.Value)
                        total += decimal.Parse(r["MailboxPlanSize"].ToString(), CultureInfo.InvariantCulture);
                    else
                        total += decimal.Parse(r["TotalSize"].ToString(), CultureInfo.InvariantCulture);
                }

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Convert to GB, TB, etc
                string formattedSize = total.ToString() + "MB";
                if (total >= 1048576)
                {
                    formattedSize = ((total / 1024) / 1024).ToString("#.##") + "TB";
                }
                else if (total >= 1024)
                {
                    formattedSize = (total / 1024).ToString("#.##") + "GB";
                }

                // Return value
                return formattedSize;
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
