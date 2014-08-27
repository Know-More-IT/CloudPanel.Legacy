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
    public class SQLCompanies
    {
        /// <summary>
        /// Get a list of ALL companies in the database
        /// </summary>
        /// <returns></returns>
        public static List<Company> GetCompanies()
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM Companies WHERE IsReseller=0 ORDER BY CompanyName", sql);

            List<Company> companies = new List<Company>();
            try
            {
                // Open Connection
                sql.Open();

                // Read all data
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Company tmp = new Company();
                    tmp.CompanyID = int.Parse(r["CompanyId"].ToString());
                    tmp.ResellerCode = r["ResellerCode"].ToString();
                    tmp.CompanyName = r["CompanyName"].ToString();
                    tmp.CompanyCode = r["CompanyCode"].ToString();
                    tmp.Street = r["Street"].ToString();
                    tmp.City = r["City"].ToString();
                    tmp.State = r["State"].ToString();

                    if (r["Country"] != DBNull.Value)
                        tmp.Country = r["Country"].ToString();

                    tmp.ZipCode = r["ZipCode"].ToString();

                    if (r["PhoneNumber"] != DBNull.Value)
                        tmp.PhoneNumber = r["PhoneNumber"].ToString();

                    if (r["Website"] != DBNull.Value)
                        tmp.Website = r["Website"].ToString();

                    if (r["Description"] != DBNull.Value)
                        tmp.Description = r["Description"].ToString();

                    if (r["AdminName"] != DBNull.Value)
                        tmp.AdministratorsName = r["AdminName"].ToString();

                    if (r["AdminEmail"] != DBNull.Value)
                        tmp.AdministratorsEmail = r["AdminEmail"].ToString();

                    if (r["DistinguishedName"] != DBNull.Value)
                        tmp.DistinguishedName = r["DistinguishedName"].ToString();

                    if (r["Created"] != DBNull.Value)
                        tmp.WhenCreated = DateTime.Parse(r["Created"].ToString());

                    if (r["ExchEnabled"] != DBNull.Value)
                        tmp.ExchangeEnabled = bool.Parse(r["ExchEnabled"].ToString());

                    if (r["LyncEnabled"] != DBNull.Value)
                        tmp.LyncEnabled = bool.Parse(r["LyncEnabled"].ToString());

                    if (r["CitrixEnabled"] != DBNull.Value)
                        tmp.CitrixEnabled = bool.Parse(r["CitrixEnabled"].ToString());

                    if (r["ExchPFPlan"] != DBNull.Value)
                        tmp.ExchPublicFolderPlan = int.Parse(r["ExchPFPlan"].ToString());

                    // Get list of company domains
                    tmp.Domains = GetCompanyDomains(r["CompanyCode"].ToString());

                    companies.Add(tmp);
                }

                // Close and dispose
                r.Close();
                r.Dispose();
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

            // Return data
            return companies;
        }

        /// <summary>
        /// Gets a list of companies for a specific reseller
        /// </summary>
        /// <param name="resellerCode">ResellerCode to search</param>
        /// <returns></returns>
        public static List<Company> GetCompanies(string resellerCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM Companies WHERE IsReseller=0 AND ResellerCode=@ResellerCode ORDER BY CompanyName", sql);

            List<Company> companies = new List<Company>();
            try
            {
                // Add SQL parameters
                cmd.Parameters.AddWithValue("@ResellerCode", resellerCode);

                // Open Connection
                sql.Open();

                // Read all data
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Company tmp = new Company();
                    tmp.CompanyID = int.Parse(r["CompanyId"].ToString());
                    tmp.ResellerCode = resellerCode;
                    tmp.CompanyName = r["CompanyName"].ToString();
                    tmp.CompanyCode = r["CompanyCode"].ToString();
                    tmp.Street = r["Street"].ToString();
                    tmp.City = r["City"].ToString();
                    tmp.State = r["State"].ToString();

                    if (r["Country"] != DBNull.Value)
                        tmp.Country = r["Country"].ToString();

                    if (r["ZipCode"] != DBNull.Value)
                        tmp.ZipCode = r["ZipCode"].ToString();

                    if (r["PhoneNumber"] != DBNull.Value)
                        tmp.PhoneNumber = r["PhoneNumber"].ToString();

                    if (r["Website"] != DBNull.Value)
                        tmp.Website = r["Website"].ToString();

                    if (r["Description"] != DBNull.Value)
                        tmp.Description = r["Description"].ToString();

                    if (r["AdminName"] != DBNull.Value)
                        tmp.AdministratorsName = r["AdminName"].ToString();

                    if (r["AdminEmail"] != DBNull.Value)
                        tmp.AdministratorsEmail = r["AdminEmail"].ToString();

                    if (r["DistinguishedName"] != DBNull.Value)
                        tmp.DistinguishedName = r["DistinguishedName"].ToString();

                    if (r["Created"] != DBNull.Value)
                        tmp.WhenCreated = DateTime.Parse(r["Created"].ToString());

                    if (r["ExchEnabled"] != DBNull.Value)
                        tmp.ExchangeEnabled = bool.Parse(r["ExchEnabled"].ToString());

                    if (r["LyncEnabled"] != DBNull.Value)
                        tmp.LyncEnabled = bool.Parse(r["LyncEnabled"].ToString());

                    if (r["CitrixEnabled"] != DBNull.Value)
                        tmp.CitrixEnabled = bool.Parse(r["CitrixEnabled"].ToString());

                    if (r["ExchPFPlan"] != DBNull.Value)
                        tmp.ExchPublicFolderPlan = int.Parse(r["ExchPFPlan"].ToString());

                    // Get list of company domains
                    tmp.Domains = GetCompanyDomains(r["CompanyCode"].ToString());

                    companies.Add(tmp);
                }

                // Close and dispose
                r.Close();
                r.Dispose();
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

            // Return data
            return companies;
        }

        /// <summary>
        /// Gets details about a specific company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static Company GetCompany(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM Companies WHERE IsReseller=0 AND CompanyCode=@CompanyCode", sql);

            Company company = new Company();
            try
            {
                // Add SQL parameters
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                // Open Connection
                sql.Open();

                // Read all data
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    company.CompanyID = int.Parse(r["CompanyId"].ToString());
                    company.ResellerCode = r["ResellerCode"].ToString();
                    company.CompanyName = r["CompanyName"].ToString();
                    company.CompanyCode = r["CompanyCode"].ToString();
                    company.Street = r["Street"].ToString();
                    company.City = r["City"].ToString();
                    company.State = r["State"].ToString();

                    if (r["OrgPlanID"] != DBNull.Value)
                        company.OrgPlanID = int.Parse(r["OrgPlanID"].ToString());

                    if (r["Country"] != DBNull.Value)
                        company.Country = r["Country"].ToString();

                    company.ZipCode = r["ZipCode"].ToString();

                    if (r["PhoneNumber"] != DBNull.Value)
                        company.PhoneNumber = r["PhoneNumber"].ToString();

                    if (r["Website"] != DBNull.Value)
                        company.Website = r["Website"].ToString();

                    if (r["Description"] != DBNull.Value)
                        company.Description = r["Description"].ToString();

                    if (r["AdminName"] != DBNull.Value)
                        company.AdministratorsName = r["AdminName"].ToString();

                    if (r["AdminEmail"] != DBNull.Value)
                        company.AdministratorsEmail = r["AdminEmail"].ToString();

                    if (r["DistinguishedName"] != DBNull.Value)
                        company.DistinguishedName = r["DistinguishedName"].ToString();

                    if (r["Created"] != DBNull.Value)
                        company.WhenCreated = DateTime.Parse(r["Created"].ToString());

                    if (r["ExchEnabled"] != DBNull.Value)
                        company.ExchangeEnabled = bool.Parse(r["ExchEnabled"].ToString());

                    if (r["LyncEnabled"] != DBNull.Value)
                        company.LyncEnabled = bool.Parse(r["LyncEnabled"].ToString());

                    if (r["CitrixEnabled"] != DBNull.Value)
                        company.CitrixEnabled = bool.Parse(r["CitrixEnabled"].ToString());

                    if (r["ExchPFPlan"] != DBNull.Value)
                        company.ExchPublicFolderPlan = int.Parse(r["ExchPFPlan"].ToString());

                    if (r["ExchPermFixed"] != DBNull.Value)
                        company.ExchangePermissionsFixed = bool.Parse(r["ExchPermFixed"].ToString());
                    else
                        company.ExchangePermissionsFixed = false;

                    // Get list of company domains
                    company.Domains = GetCompanyDomains(r["CompanyCode"].ToString());
                }

                // Close and dispose
                r.Close();
                r.Dispose();
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

            // Return data
            return company;
        }

        /// <summary>
        /// Adds a new company to the database
        /// </summary>
        /// <param name="companyObject"></param>
        public static void AddCompany(Company companyObject)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"INSERT INTO Companies (IsReseller, ResellerCode, OrgPlanID, CompanyName, CompanyCode, Street, City, State, Country, ZipCode,
                                                                     PhoneNumber, Website, Description, AdminName, AdminEmail, DistinguishedName, Created, ExchEnabled, LyncEnabled,
                                                                     CitrixEnabled, ExchPFPlan) VALUES
                                                                    (@IsReseller, @ResellerCode, @OrgPlanID, @CompanyName, @CompanyCode, @Street, @City, @State, @Country, @ZipCode,
                                                                     @PhoneNumber, @Website, @Description, @AdminName, @AdminEmail, @DistinguishedName, @Created, @ExchEnabled, @LyncEnabled,
                                                                     @CitrixEnabled, @ExchPFPlan)", sql);

            try
            {
                // Set parameters
                cmd.Parameters.AddWithValue("@IsReseller", false);
                cmd.Parameters.AddWithValue("@ResellerCode", companyObject.ResellerCode);
                cmd.Parameters.AddWithValue("@OrgPlanID", 0);
                cmd.Parameters.AddWithValue("@CompanyCode", companyObject.CompanyCode);
                cmd.Parameters.AddWithValue("@CompanyName", companyObject.CompanyName);
                cmd.Parameters.AddWithValue("@Street", companyObject.Street);
                cmd.Parameters.AddWithValue("@City", companyObject.City);
                cmd.Parameters.AddWithValue("@State", companyObject.State);
                cmd.Parameters.AddWithValue("@Country", companyObject.Country);
                cmd.Parameters.AddWithValue("@ZipCode", companyObject.ZipCode);
                cmd.Parameters.AddWithValue("@PhoneNumber", companyObject.PhoneNumber);
                cmd.Parameters.AddWithValue("@Website", companyObject.Website);
                cmd.Parameters.AddWithValue("@Description", companyObject.Description);
                cmd.Parameters.AddWithValue("@AdminName", companyObject.AdministratorsName);
                cmd.Parameters.AddWithValue("@AdminEmail", companyObject.AdministratorsEmail);
                cmd.Parameters.AddWithValue("@DistinguishedName", companyObject.DistinguishedName);
                cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                cmd.Parameters.AddWithValue("@ExchEnabled", false);
                cmd.Parameters.AddWithValue("@LyncEnabled", false);
                cmd.Parameters.AddWithValue("@CitrixEnabled", false);
                cmd.Parameters.AddWithValue("@ExchPFPlan", 0);

                // Open connection
                sql.Open();

                // Update
                cmd.ExecuteNonQuery();

                // Close
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
        /// Updates a company object in the database
        /// </summary>
        /// <param name="companyObject"></param>
        public static void UpdateCompany(Company companyObject)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE Companies SET CompanyName=@CompanyName, Street=@Street, City=@City, State=@State, Country=@Country, ZipCode=@ZipCode,
                                                                   PhoneNumber=@PhoneNumber, Website=@Website, AdminName=@AdminName, AdminEmail=@AdminEmail WHERE IsReseller=0 AND CompanyCode=@CompanyCode", sql);

            try
            {
                // Set parameters
                cmd.Parameters.AddWithValue("@CompanyCode", companyObject.CompanyCode);
                cmd.Parameters.AddWithValue("@CompanyName", companyObject.CompanyName);
                cmd.Parameters.AddWithValue("@Street", companyObject.Street);
                cmd.Parameters.AddWithValue("@City", companyObject.City);
                cmd.Parameters.AddWithValue("@State", companyObject.State);
                cmd.Parameters.AddWithValue("@Country", companyObject.Country);
                cmd.Parameters.AddWithValue("@ZipCode", companyObject.ZipCode);
                cmd.Parameters.AddWithValue("@PhoneNumber", companyObject.PhoneNumber);
                cmd.Parameters.AddWithValue("@Website", companyObject.Website);
                cmd.Parameters.AddWithValue("@AdminName", companyObject.AdministratorsName);
                cmd.Parameters.AddWithValue("@AdminEmail", companyObject.AdministratorsEmail);

                // Open connection
                sql.Open();

                // Update
                cmd.ExecuteNonQuery();

                // Close
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
        /// Updates the company if the Exchange permissions have been fixed
        /// </summary>
        /// <param name="isFixed"></param>
        /// <param name="companyCode"></param>
        public static void FixExchangePermissions(bool isFixed, string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE Companies SET ExchPermFixed=@ExchPermFixed WHERE IsReseller=0 AND CompanyCode=@CompanyCode", sql);

            try
            {
                // Set parameters
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
                cmd.Parameters.AddWithValue("@ExchPermFixed", isFixed);

                // Open connection
                sql.Open();

                // Update
                cmd.ExecuteNonQuery();

                // Close
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
        /// Deletes a company and all associated objects from SQL
        /// </summary>
        /// <param name="companyCode"></param>
        public static void DeleteCompany(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("DeleteCompany", sql);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            try
            {
                // Set parameters
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // Delete
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
                cmd.Dispose();
                sql.Dispose();
            }
        }

        /// <summary>
        /// Updates the plan for a specific company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="planID"></param>
        public static void UpdateCompanyPlan(string companyCode, int planID)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE Companies SET OrgPlanID=@OrgPlanID WHERE IsReseller=0 AND CompanyCode=@CompanyCode", sql);

            try
            {
                // Set parameters
                cmd.Parameters.AddWithValue("@OrgPlanID", planID);
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // Update
                cmd.ExecuteNonQuery();

                // Close
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
        /// Gets a list of company domains from the database
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<string> GetCompanyDomains(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT Domain FROM Domains WHERE CompanyCode=@CompanyCode", sql);

            List<string> domains = new List<string>();
            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // Read
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    domains.Add(r["Domain"].ToString());
                }

                // Close and dispose
                r.Close();
                r.Dispose();
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

            return domains;
        }

    }
}
