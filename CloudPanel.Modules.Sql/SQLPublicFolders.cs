using CloudPanel.Modules.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Sql
{
    public class SQLPublicFolders
    {
        /// <summary>
        /// If a company is enabled for public folders or not
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static bool IsPublicFolderEnabled(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT ExchPFPlan FROM Companies WHERE CompanyCode=@CompanyCode AND IsReseller=0", sql);

            try
            {
                // Add Parameter
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);
                
                // Open connection
                sql.Open();

                // Read data
                object enabled = cmd.ExecuteScalar();
                if (enabled == DBNull.Value)
                    return false;
                else
                {
                    if (int.Parse(enabled.ToString()) > 0)
                        return true;
                    else
                        return false;
                }
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
        /// Updates plan for a specific company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="planId"></param>
        public static void Update_PublicFolderForCompany(string companyCode, bool enablePublicFolders)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE Companies SET ExchPFPlan=@ExchPFPlan WHERE CompanyCode=@CompanyCode AND IsReseller=0", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("ExchPFPlan", enablePublicFolders ? 1 : 0);
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open
                sql.Open();

                // Update
                cmd.ExecuteNonQuery();

                // CLose
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
