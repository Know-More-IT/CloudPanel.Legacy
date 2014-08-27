using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudPanel.Modules.Base;
using System.Data.SqlClient;
using System.Configuration;
using CloudPanel.Modules.Base.Class;

namespace CloudPanel.Modules.Sql
{
    public class SQLDomains
    {
        /// <summary>
        /// Gets a list of all domains 
        /// </summary>
        /// <returns></returns>
        public static List<Domain> GetDomains(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM Domains WHERE CompanyCode=@CompanyCode", sql);
            cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

            // Create our collection
            List<Domain> domains = new List<Domain>();

            try
            {
                // Open connection 
                sql.Open();

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Domain tmp = new Domain();
                    tmp.DomainID = int.Parse(r["DomainID"].ToString());
                    tmp.CompanyCode = r["CompanyCode"].ToString();
                    tmp.DomainName = r["Domain"].ToString();
                    tmp.IsDefault = bool.Parse(r["IsDefault"].ToString());
                    tmp.IsAcceptedDomain = bool.Parse(r["IsAcceptedDomain"].ToString());


                    if (r["IsSubDomain"] == DBNull.Value)
                        tmp.IsSubDomain = bool.Parse(r["IsSubDomain"].ToString());

                    // Add to our collection
                    domains.Add(tmp);
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
            return domains;
        }

        /// <summary>
        /// Gets only the accepted domains from the database for a specific company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<Domain> GetAcceptedDomains(string companyCode)
        {
            try
            {
                List<Domain> domains = GetDomains(companyCode);

                // Find only the accepted domains
                var acceptedDomains = from d in domains
                                      where d.IsAcceptedDomain = true
                                      select d;

                return acceptedDomains.ToList(); ;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Checks to see if a domain exists or not
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static bool DoesDomainExist(string domainName)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Domains WHERE Domain=@DomainName", sql);

            try
            {
                // Add our parameters
                cmd.Parameters.AddWithValue("@DomainName", domainName);

                // Open connection 
                sql.Open();

                // Read data
                int count = int.Parse(cmd.ExecuteScalar().ToString());

                if (count > 0)
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

        /// <summary>
        /// Adds a domain to the database
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="domainName"></param>
        public static void AddDomain(string companyCode, string domainName)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"INSERT INTO Domains (CompanyCode, Domain, IsSubDomain, IsDefault, IsAcceptedDomain) VALUES
                                                                  (@CompanyCode, @Domain, @IsSubDomain, @IsDefault, @IsAcceptedDomain)", sql);

            try
            {
                // Add our parameters
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
                cmd.Parameters.AddWithValue("@Domain", domainName);
                cmd.Parameters.AddWithValue("@IsSubDomain", false);
                cmd.Parameters.AddWithValue("@IsDefault", false);
                cmd.Parameters.AddWithValue("@IsAcceptedDomain", false);

                // Open connection 
                sql.Open();

                // Insert Data
                cmd.ExecuteNonQuery();

                // Close connection
                sql.Close();
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
        /// Deletes a domain from the database
        /// </summary>
        /// <param name="domainName"></param>
        public static void DeleteDomain(string domainName)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"DELETE FROM Domains WHERE Domain=@Domain", sql);

            try
            {
                // Add our parameters
                cmd.Parameters.AddWithValue("@Domain", domainName);

                // Open connection 
                sql.Open();

                // Delete Data
                cmd.ExecuteNonQuery();

                // Close connection
                sql.Close();
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
        /// Updates the domain to set it as an accepted domain or not
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="isAcceptedDomain"></param>
        /// <returns>True if more than one row changes, false if no rows were changed</returns>
        public static bool UpdateAcceptedDomain(int domainId, bool isAcceptedDomain)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("UPDATE Domains SET IsAcceptedDomain=@IsAcceptedDomain WHERE DomainID=@DomainID", sql);

            try
            {
                // Add our parameters
                cmd.Parameters.AddWithValue("@IsAcceptedDomain", isAcceptedDomain);
                cmd.Parameters.AddWithValue("@DomainID", domainId);

                // Open connection 
                sql.Open();

                // Update Data
                int count = cmd.ExecuteNonQuery();

                if (count > 0)
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

        /// <summary>
        /// Checks the database if the domain is in use or not
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static bool IsDomainInUse(string domainName)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT 
                                                *, 
                                                (SELECT COUNT(UserPrincipalName) FROM Users WHERE UserPrincipalName LIKE '%@" + domainName + @"') AS UserCount 
                                              FROM 
                                                Domains d 
                                              WHERE 
                                                Domain=@DomainName", sql);

            try
            {
                // Open connection 
                sql.Open();

                // Add our parameters
                cmd.Parameters.AddWithValue("@DomainName", domainName);

                bool isDomainInUse = true;

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    bool isAcceptedDomain = bool.Parse(r["IsAcceptedDomain"].ToString());
                    int userCount = int.Parse(r["UserCount"].ToString());

                    if (!isAcceptedDomain && userCount == 0)
                        isDomainInUse = false;
                }

                // Close and dispose
                r.Close();
                r.Dispose();

                // Return if the domain is in use or not
                return isDomainInUse;
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
