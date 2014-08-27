using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace CloudPanel.Modules.Sql
{
    public class SqlCommon
    {
        /// <summary>
        /// Checks the database to see if the company code exists or not
        /// </summary>
        /// <param name="companyCode">CompanyCode to check if it exists or not</param>
        /// <returns></returns>
        public static bool DoesCompanyCodeExist(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Companies WHERE CompanyCode=@CompanyCode", sql);

            try
            {
                // Add company code to parameters
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // Get the number of rows returned
                int count = int.Parse(cmd.ExecuteScalar().ToString());

                // Close connection
                sql.Close();

                if (count > 0)
                    return true;
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
        /// Determines if the account is locked out or not
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="failedCount"></param>
        /// <param name="failedMinutes"></param>
        /// <returns></returns>
        public static bool IsIPLockedOut(string ipAddress, int failedCount, int failedMinutes)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM AuditLogin WHERE IPAddress=@IPAddress AND LoginStatus=0 AND AuditTimeStamp >= DATEADD(minute, @Minutes, GETDATE())", sql);

            try
            {
                // Add company code to parameters
                cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                cmd.Parameters.AddWithValue("@Minutes", failedMinutes * -1);

                // Open connection
                sql.Open();

                // Get the number of rows returned
                int count = int.Parse(cmd.ExecuteScalar().ToString());

                // Close connection
                sql.Close();

                if (count >= failedCount)
                    return true;
                else
                    return false;
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
