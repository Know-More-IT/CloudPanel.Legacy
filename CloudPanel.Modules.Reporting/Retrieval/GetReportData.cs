using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Reporting
{
    public class GetReportData
    {
        /// <summary>
        /// Gets data for the Exchange report
        /// </summary>
        /// <returns></returns>
        public static BindingList<ExchangeDetails> Get_ExchangeDetails()
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT
                                                U.UserPrincipalName,
	                                            C.CompanyName,
	                                            C.CompanyCode,
	                                            U.AdditionalMB,
	                                            E.MailboxPlanName,
	                                            E.MailboxPlanID,
	                                            COUNT(1) * CAST(dbo.GetExchangeMailboxPlanPrice(C.CompanyCode, E.MailboxPlanID) AS decimal(18, 2)) AS Price,
	                                            COUNT(1) * CAST((SELECT Cost FROM Plans_ExchangeMailbox WHERE MailboxPlanID=E.MailboxPlanID) AS decimal(18, 2)) AS Cost,
	                                            COUNT(1) Cnt,
	                                            CAST(AdditionalGBPrice AS decimal(18,2)) AS AdditionalGBPrice
                                            FROM Companies C
	                                            JOIN Users U
		                                            ON C.CompanyCode = U.CompanyCode
	                                            JOIN Plans_ExchangeMailbox E
		                                            ON U.MailboxPlan = E.MailboxPlanID
                                            GROUP BY 
	                                            C.CompanyCode,
	                                            C.CompanyName,
	                                            E.MailboxPlanID,
	                                            E.MailboxPlanName,
	                                            C.ExchPFPlan,
	                                            E.AdditionalGBPrice,
	                                            U.AdditionalMB,
                                                U.UserPrincipalName
                                            ORDER BY
	                                            C.CompanyName,
	                                            E.MailboxPlanName", sql);

                // Object to return
                BindingList<ExchangeDetails> report = new BindingList<ExchangeDetails>();

                // Open connection
                sql.Open();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        ExchangeDetails tmp = new ExchangeDetails();
                        tmp.UserPrincipalName = r["UserPrincipalName"].ToString();
                        tmp.CompanyName = r["CompanyName"].ToString();
                        tmp.CompanyCode = r["CompanyCode"].ToString();
                        tmp.MailboxPlanName = r["MailboxPlanName"].ToString();
                        tmp.MailboxPlanID = int.Parse(r["MailboxPlanID"].ToString());
                        tmp.MailboxPlanCount = int.Parse(r["Cnt"].ToString());

                        if (r["AdditionalMB"] != DBNull.Value)
                        {
                            int added = int.Parse(r["AdditionalMB"].ToString());
                            if (added > 0)
                                tmp.AdditionalGB = (decimal.Parse(r["AdditionalMB"].ToString(), CultureInfo.InvariantCulture) / 1024);
                            else
                                tmp.AdditionalGB = 0;

                        }
                        else
                            tmp.AdditionalGB = 0;

                        if (r["Cost"] != DBNull.Value)
                            tmp.ExchangePlanCost = decimal.Parse(r["Cost"].ToString(), CultureInfo.InvariantCulture);
                        else
                            tmp.ExchangePlanCost = 0;

                        if (r["Price"] != DBNull.Value)
                            tmp.ExchangePlanPrice = decimal.Parse(r["Price"].ToString(), CultureInfo.InvariantCulture);
                        else
                            tmp.ExchangePlanPrice = 0;

                        if (r["AdditionalGBPrice"] != DBNull.Value)
                            tmp.ExchangeAdditionalGBPrice = decimal.Parse(r["AdditionalGBPrice"].ToString(), CultureInfo.InvariantCulture);
                        else
                            tmp.ExchangeAdditionalGBPrice = 0;

                        report.Add(tmp);
                    }
                }

                // Return our object
                return report;
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
        /// Totals from the database
        /// </summary>
        /// <returns></returns>
        public static BindingList<TotalStats> Get_TotalStatistics()
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT
	                                        (SELECT COUNT(*) FROM Users WHERE MailboxPlan > 0) AS MailboxCount,
	                                        (SELECT COUNT(*) FROM Users) AS UserCount", sql);

                // Object to return
                BindingList<TotalStats> stats = new BindingList<TotalStats>();

                // Open connection
                sql.Open();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        stats.Add(new TotalStats()
                        {
                            MailboxCount = int.Parse(r["MailboxCount"].ToString()),
                            UserCount = int.Parse(r["UserCount"].ToString())
                        });
                    }
                }

                // Return our object
                return stats;
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
        /// Gets data for the Citrix report
        /// </summary>
        /// <returns></returns>
        public static BindingList<CitrixDetails> Get_CitrixDetails()
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT
	                                        DISTINCT(Name) AS CitrixApp,
	                                        c.IsServer,
	                                        COUNT(Name) AS UserCount,
	                                        com.CompanyName AS Company,
											com.CompanyCode,
	                                        COUNT(1) * CAST(dbo.GetCitrixPlanPrice(com.CompanyCode, upc.CitrixPlanID) AS decimal(18, 2)) AS Price,
	                                        COUNT(1) * CAST((SELECT Cost FROM Plans_Citrix WHERE CitrixPlanID=upc.CitrixPlanID) AS decimal(18, 2)) AS Cost
                                        FROM 
	                                        Plans_Citrix c
                                        INNER JOIN
	                                        UserPlansCitrix upc
                                        ON
	                                        upc.CitrixPlanID = c.CitrixPlanID
                                        INNER JOIN
	                                        Users u
                                        ON
	                                        u.ID = upc.UserID
                                        INNER JOIN
	                                        Companies com
                                        ON
	                                        com.CompanyCode = u.CompanyCode
                                        GROUP BY
	                                        c.Name,
	                                        com.CompanyName,
	                                        c.IsServer,
	                                        com.CompanyCode,
	                                        upc.CitrixPlanID", sql);

                // Object to return
                BindingList<CitrixDetails> report = new BindingList<CitrixDetails>();

                // Open connection
                sql.Open();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        CitrixDetails tmp = new CitrixDetails();
                        tmp.DiplayName = r["CitrixApp"].ToString();
                        tmp.CompanyCode = r["CompanyCode"].ToString();
                        tmp.CompanyName = r["Company"].ToString();
                        tmp.IsServer = bool.Parse(r["IsServer"].ToString());
                        tmp.UserCount = int.Parse(r["UserCount"].ToString());

                        if (r["Cost"] != DBNull.Value)
                            tmp.Cost = Decimal.Parse(r["Cost"].ToString(), CultureInfo.InvariantCulture);
                        else
                            tmp.Cost = 0;

                        if (r["Price"] != DBNull.Value)
                            tmp.Price = Decimal.Parse(r["Price"].ToString(), CultureInfo.InvariantCulture);
                        else
                            tmp.Price = 0;

                        report.Add(tmp);
                    }
                }

                // Return our object
                return report;
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
