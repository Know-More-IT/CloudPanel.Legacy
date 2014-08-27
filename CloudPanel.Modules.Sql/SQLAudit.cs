using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Sql
{
    public class SQLAudit
    {
        /// <summary>
        /// Determines if the account is locked out or not
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="failedCount"></param>
        /// <param name="failedMinutes"></param>
        /// <returns></returns>
        public static void AuditLogin(string ipAddress, string username, bool successLogin)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("INSERT INTO AuditLogin (IPAddress, Username, LoginStatus, AuditTimeStamp) VALUES (@IPAddress, @Username, @LoginStatus, GETDATE())", sql);

            try
            {
                // Add company code to parameters
                cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@LoginStatus", successLogin);

                // Open connection
                sql.Open();

                // Insert
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
    }
}
