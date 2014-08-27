using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CloudPanel.Modules.Sql
{
    public class SqlLibrary
    {
        // Logger
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Executes a SQL command against the database
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        public static void ExecuteSql(string commandText, SqlParameter[] parameters)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(commandText, sql);

                // Open the connection
                sql.Open();

                // Add our parameters
                cmd.Parameters.AddRange(parameters);

                // Execute
                cmd.ExecuteNonQuery();
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
        /// Reads from the database
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static DataTable ReadSql(string commandText)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataAdapter sda = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(commandText, sql);
                
                // Set our Adapter
                sda = new SqlDataAdapter();
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();

                // Open the connection
                sql.Open();

                // Fill the dataset
                sda.Fill(dt);

                // Return
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (sda != null)
                    sda.Dispose();

                if (cmd != null)
                    cmd.Dispose();

                if (sql != null)
                    sql.Dispose();
            }
        }

        /// <summary>
        /// Reads SQL and returns a dataset
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static DataTable ReadSql(string commandText, SqlParameter parameter)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataAdapter sda = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(commandText, sql);

                // Add our parameters if there is any
                if (parameter != null)
                    cmd.Parameters.Add(parameter);

                // Set our Adapter
                sda = new SqlDataAdapter();
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();

                // Open the connection
                sql.Open();

                // Fill the dataset
                sda.Fill(dt);

                // Return
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (sda != null)
                    sda.Dispose();

                if (cmd != null)
                    cmd.Dispose();

                if (sql != null)
                    sql.Dispose();
            }
        }

        /// <summary>
        /// Reads SQL and returns a dataset
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataTable ReadSql(string commandText, SqlParameter[] parameters)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataAdapter sda = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(commandText, sql);

                // Add our parameters if there is any
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                // Set our Adapter
                sda = new SqlDataAdapter();
                sda.SelectCommand = cmd;

                DataTable dt = new DataTable();

                // Open the connection
                sql.Open();

                // Fill the dataset
                sda.Fill(dt);

                // Return
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (sda != null)
                    sda.Dispose();

                if (cmd != null)
                    cmd.Dispose();

                if (sql != null)
                    sql.Dispose();
            }
        }
    }
}
