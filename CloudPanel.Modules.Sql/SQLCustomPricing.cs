using CloudPanel.Modules.Base;
using CloudPanel.Modules.Base.Class;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Sql
{
    public class SQLCustomPricing
    {
        /// <summary>
        /// Gets the custom pricing for each mailbox plan
        /// </summary>
        /// <param name="product"></param>
        /// <param name="companyCode"></param>
        /// <param name="mailboxPlans"></param>
        public static void GetCustomPricing(string product, string companyCode, string currencySymbol, ref List<MailboxPlan> plans)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT PlanID, Product, Price FROM PriceOverride WHERE CompanyCode=@CompanyCode AND Product=@Product", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("Product", product);
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);
                
                // Open connection
                sql.Open();

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        int planId = int.Parse(r["PlanID"].ToString());

                        var findItem = plans.FindLast(x => x.PlanID == planId);
                        if (findItem != null)
                        {
                            plans[plans.IndexOf(findItem)].CustomPrice = String.Format(currencySymbol + r["Price"].ToString());
                        }
                    }
                }

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();
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
        /// Gets the custom pricing for each citrix plan
        /// </summary>
        /// <param name="product"></param>
        /// <param name="companyCode"></param>
        /// <param name="currencySymbol"></param>
        /// <param name="plans"></param>
        public static void GetCustomPricing(string product, string companyCode, string currencySymbol, ref List<BaseCitrixApp> plans)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT PlanID, Product, Price FROM PriceOverride WHERE CompanyCode=@CompanyCode AND Product=@Product", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("Product", product);
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        int planId = int.Parse(r["PlanID"].ToString());

                        var findItem = plans.FindLast(x => x.ID == planId);
                        if (findItem != null)
                        {
                            plans[plans.IndexOf(findItem)].CustomPrice = String.Format(currencySymbol + r["Price"].ToString());
                        }
                    }
                }

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();
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
        /// Updates or inserts data into the custom pricing table
        /// </summary>
        /// <param name="product"></param>
        /// <param name="companyCode"></param>
        /// <param name="customPrice"></param>
        /// <param name="planID"></param>
        public static void UpdateCustomPricing(string product, string companyCode, string customPrice, string planID)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("UpdatePriceOverride", sql);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            try
            {
                // Parse decimal to make sure it is legit
                decimal.Parse(customPrice, CultureInfo.InvariantCulture);

                // Add parameters
                cmd.Parameters.AddWithValue("Product", product);
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);
                cmd.Parameters.AddWithValue("CustomPrice", customPrice);
                cmd.Parameters.AddWithValue("PlanID", planID);

                // Open connection
                sql.Open();

                // Update / Insert
                cmd.ExecuteNonQuery();
                
                // Close
                sql.Close();
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
