using CloudPanel.Modules.Base.Class;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CloudPanel.Modules.Sql
{
    public class DbSql
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Company

        /// <summary>
        /// Gets the permissions for a specific company admin
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public static ADUser Get_CompanyAdminPermissions(string userPrincipalName)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT  *
                                        FROM Users u
                                        LEFT JOIN UserPermissions p on p.UserID=u.ID
                                        WHERE u.UserPrincipalName=@UserPrincipalName", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("UserPrincipalName", userPrincipalName);

                // Open connection
                sql.Open();

                // Create our object to return
                ADUser foundUser = new ADUser();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        foundUser.UserPrincipalName = userPrincipalName;
                        foundUser.EnableExchangePermission      = r["EnableExchange"] == DBNull.Value ? false : bool.Parse(r["EnableExchange"].ToString());
                        foundUser.DisableExchangePermission     = r["DisableExchange"] == DBNull.Value ? false : bool.Parse(r["DisableExchange"].ToString());
                        foundUser.AddDomainPermission           = r["AddDomain"] == DBNull.Value ? false : bool.Parse(r["AddDomain"].ToString());
                        foundUser.DeleteDomainPermission        = r["DeleteDomain"] == DBNull.Value ? false : bool.Parse(r["DeleteDomain"].ToString());
                        foundUser.ModifyAcceptedDomainPermission = r["ModifyAcceptedDomain"] == DBNull.Value ? false : bool.Parse(r["ModifyAcceptedDomain"].ToString());
                        foundUser.ImportUsersPermission         = r["ImportUsers"] == DBNull.Value ? false : bool.Parse(r["ImportUsers"].ToString());
                    }
                }
                else
                    foundUser = null;

                // Return our object
                return foundUser;
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
        /// Gets information about a certain company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static Company Get_Company(string companyCode)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT  *
                                        FROM Companies
                                        WHERE IsReseller=0 AND CompanyCode=@CompanyCode
                                        ORDER BY CompanyName", sql);
                
                // Add parameter
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // Create our object to return
                Company tmp = new Company();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        tmp.CompanyID = int.Parse(r["CompanyId"].ToString());
                        tmp.CompanyName = r["CompanyName"].ToString();
                        tmp.CompanyCode = r["CompanyCode"].ToString();
                        tmp.Street = r["Street"].ToString();
                        tmp.City = r["City"].ToString();
                        tmp.State = r["State"].ToString();
                        tmp.ZipCode = r["ZipCode"].ToString();
                        tmp.PhoneNumber = r["PhoneNumber"].ToString();
                        tmp.AdministratorsName = r["AdminName"].ToString();
                        tmp.AdministratorsEmail = r["AdminEmail"].ToString();
                        tmp.DistinguishedName = r["DistinguishedName"].ToString();
                        tmp.WhenCreated = DateTime.Parse(r["Created"].ToString());

                        if (r["ResellerCode"] != DBNull.Value)
                            tmp.ResellerCode = r["ResellerCode"].ToString();

                        if (r["OrgPlanID"] != DBNull.Value)
                            tmp.OrgPlanID = int.Parse(r["OrgPlanID"].ToString());

                        if (r["Website"] != DBNull.Value)
                            tmp.Website = r["Website"].ToString();

                        if (r["Description"] != DBNull.Value)
                            tmp.Description = r["Description"].ToString();

                        if (r["ExchEnabled"] != DBNull.Value)
                            tmp.ExchangeEnabled = bool.Parse(r["ExchEnabled"].ToString());

                        if (r["LyncEnabled"] != DBNull.Value)
                            tmp.LyncEnabled = bool.Parse(r["LyncEnabled"].ToString());

                        if (r["CitrixEnabled"] != DBNull.Value)
                            tmp.CitrixEnabled = bool.Parse(r["CitrixEnabled"].ToString());

                        if (r["ExchPFPlan"] != DBNull.Value)
                            tmp.ExchPublicFolderPlan = int.Parse(r["ExchPFPlan"].ToString());

                        if (r["Country"] != DBNull.Value)
                            tmp.Country = r["Country"].ToString();

                        if (r["ExchPermFixed"] != DBNull.Value)
                            tmp.ExchangePermissionsFixed = bool.Parse(r["ExchPermFixed"].ToString());
                    }
                }
                else
                    tmp = null;

                // Return our object
                return tmp;
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
        /// Gets a list of companies from the database from ALL resellers
        /// </summary>
        /// <returns></returns>
        public static List<Company> Get_Companies()
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT  *
                                        FROM Companies
                                        WHERE IsReseller=0
                                        ORDER BY CompanyName", sql);

                // Open connection
                sql.Open();

                // Create our object to return
                List<Company> companies = new List<Company>();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Company tmp = new Company();
                        tmp.CompanyID = int.Parse(r["CompanyId"].ToString());
                        tmp.CompanyName = r["CompanyName"].ToString();
                        tmp.CompanyCode = r["CompanyCode"].ToString();
                        tmp.Street = r["Street"].ToString();
                        tmp.City = r["City"].ToString();
                        tmp.State = r["State"].ToString();
                        tmp.ZipCode = r["ZipCode"].ToString();
                        tmp.PhoneNumber = r["PhoneNumber"].ToString();
                        tmp.AdministratorsName = r["AdminName"].ToString();
                        tmp.AdministratorsEmail = r["AdminEmail"].ToString();
                        tmp.DistinguishedName = r["DistinguishedName"].ToString();
                        tmp.WhenCreated = DateTime.Parse(r["Created"].ToString());

                        if (r["ResellerCode"] != DBNull.Value)
                            tmp.ResellerCode = r["ResellerCode"].ToString();

                        if (r["OrgPlanID"] != DBNull.Value)
                            tmp.OrgPlanID = int.Parse(r["OrgPlanID"].ToString());

                        if (r["Website"] != DBNull.Value)
                            tmp.Website = r["Website"].ToString();

                        if (r["Description"] != DBNull.Value)
                            tmp.Description = r["Description"].ToString();

                        if (r["ExchEnabled"] != DBNull.Value)
                            tmp.ExchangeEnabled = bool.Parse(r["ExchEnabled"].ToString());

                        if (r["LyncEnabled"] != DBNull.Value)
                            tmp.LyncEnabled = bool.Parse(r["LyncEnabled"].ToString());

                        if (r["CitrixEnabled"] != DBNull.Value)
                            tmp.CitrixEnabled = bool.Parse(r["CitrixEnabled"].ToString());

                        if (r["ExchPFPlan"] != DBNull.Value)
                            tmp.ExchPublicFolderPlan = int.Parse(r["ExchPFPlan"].ToString());

                        if (r["Country"] != DBNull.Value)
                            tmp.Country = r["Country"].ToString();

                        if (r["ExchPermFixed"] != DBNull.Value)
                            tmp.ExchangePermissionsFixed = bool.Parse(r["ExchPermFixed"].ToString());

                        companies.Add(tmp);
                    }
                }
                else
                    companies = null;

                // Return our object
                return companies;
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

        #endregion

        #region Resellers

        /// <summary>
        /// Gets a list of resellers from the database
        /// </summary>
        /// <returns></returns>
        public static List<Company> GetResellers()
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT CompanyName, CompanyCode, Street, City, State, ZipCode, PhoneNumber, Website,
                                                     AdminName, AdminEmail, Created, (SELECT COUNT(*) FROM Companies WHERE ResellerCode=c.CompanyCode AND IsReseller=0) AS CompanyCount FROM Companies c WHERE IsReseller=1", sql);

            List<Company> resellers = new List<Company>();
            try
            {
                // Open Connection
                sql.Open();

                // Read and put in List
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Company tmp = new Company();
                    tmp.CompanyName = r["CompanyName"].ToString();
                    tmp.CompanyCode = r["CompanyCode"].ToString();
                    tmp.AdditionalValue1 = int.Parse(r["CompanyCount"].ToString());

                    if (r["Street"] != DBNull.Value)
                        tmp.Street = r["Street"].ToString();

                    if (r["City"] != DBNull.Value)
                        tmp.City = r["City"].ToString();

                    if (r["State"] != DBNull.Value)
                        tmp.State = r["State"].ToString();

                    if (r["ZipCode"] != DBNull.Value)
                        tmp.ZipCode = r["ZipCode"].ToString();

                    if (r["PhoneNumber"] != DBNull.Value)
                        tmp.PhoneNumber = r["PhoneNumber"].ToString();

                    if (r["Website"] != DBNull.Value)
                        tmp.Website = r["Website"].ToString();

                    if (r["AdminName"] != DBNull.Value)
                        tmp.AdministratorsName = r["AdminName"].ToString();

                    if (r["AdminEmail"] != DBNull.Value)
                        tmp.AdministratorsEmail = r["AdminEmail"].ToString();

                    if (r["Created"] != DBNull.Value)
                        tmp.WhenCreated = DateTime.Parse(r["Created"].ToString());

                    resellers.Add(tmp);
                }

                // Close and Dispose Reader
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

            // Return our list
            return resellers;
        }

        /// <summary>
        /// Deletes a reseller from AD
        /// </summary>
        /// <param name="resellerCode"></param>
        public static void DeleteReseller(string resellerCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"DELETE FROM Companies WHERE CompanyCode=@ResellerCode AND IsReseller=1", sql);

            try
            {
                // Add our parameter
                cmd.Parameters.AddWithValue("ResellerCode", resellerCode);

                // Open Connection
                sql.Open();

                // Delete the reseller from SQL
                cmd.ExecuteNonQuery();
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
        /// Gets a certain reseller
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static Company GetReseller(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT CompanyName, CompanyCode, Street, City, State, ZipCode, PhoneNumber, Website,
                                                     AdminName, AdminEmail, Created, Country, DistinguishedName FROM Companies WHERE IsReseller=1 AND CompanyCode=@CompanyCode", sql);

            Company companyInfo = new Company();
            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open Connection
                sql.Open();

                // Read and put in List
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    companyInfo.CompanyName = r["CompanyName"].ToString();
                    companyInfo.CompanyCode = r["CompanyCode"].ToString();
                    companyInfo.DistinguishedName = r["DistinguishedName"].ToString();

                    if (r["Street"] != DBNull.Value)
                        companyInfo.Street = r["Street"].ToString();

                    if (r["City"] != DBNull.Value)
                        companyInfo.City = r["City"].ToString();

                    if (r["State"] != DBNull.Value)
                        companyInfo.State = r["State"].ToString();

                    if (r["ZipCode"] != DBNull.Value)
                        companyInfo.ZipCode = r["ZipCode"].ToString();

                    if (r["PhoneNumber"] != DBNull.Value)
                        companyInfo.PhoneNumber = r["PhoneNumber"].ToString();

                    if (r["Website"] != DBNull.Value)
                        companyInfo.Website = r["Website"].ToString();

                    if (r["AdminName"] != DBNull.Value)
                        companyInfo.AdministratorsName = r["AdminName"].ToString();

                    if (r["AdminEmail"] != DBNull.Value)
                        companyInfo.AdministratorsEmail = r["AdminEmail"].ToString();

                    if (r["Created"] != DBNull.Value)
                        companyInfo.WhenCreated = DateTime.Parse(r["Created"].ToString());

                    if (r["Country"] != DBNull.Value)
                        companyInfo.Country = r["Country"].ToString();

                }

                // Close and Dispose Reader
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

            // Return our object
            return companyInfo;
        }

        /// <summary>
        /// Updates a resellers information in the database
        /// </summary>
        /// <param name="companyInfo"></param>
        public static void UpdateReseller(Company companyInfo)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE Companies SET CompanyName=@CompanyName, Street=@Street, City=@City, State=@State, ZipCode=@ZipCode,
                                              Country=@Country, PhoneNumber=@PhoneNumber, Website=@Website, Description=@Description, AdminName=@AdminName, 
                                              AdminEmail=@AdminEmail WHERE IsReseller=1 AND CompanyCode=@CompanyCode", sql);

            try
            {
                // Add the parameters
                cmd.Parameters.AddWithValue("CompanyCode", companyInfo.CompanyCode);
                cmd.Parameters.AddWithValue("CompanyName", companyInfo.CompanyName);
                cmd.Parameters.AddWithValue("Street", companyInfo.Street);
                cmd.Parameters.AddWithValue("City", companyInfo.City);
                cmd.Parameters.AddWithValue("State", companyInfo.State);
                cmd.Parameters.AddWithValue("ZipCode", companyInfo.ZipCode);
                cmd.Parameters.AddWithValue("PhoneNumber", companyInfo.PhoneNumber);
                cmd.Parameters.AddWithValue("Website", companyInfo.Website);
                cmd.Parameters.AddWithValue("Description", companyInfo.Description);
                cmd.Parameters.AddWithValue("AdminName", companyInfo.AdministratorsName);
                cmd.Parameters.AddWithValue("AdminEmail", companyInfo.AdministratorsEmail);
                cmd.Parameters.AddWithValue("Country", companyInfo.Country);

                // Open connection
                sql.Open();

                // Update database
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
        /// Adds a new reseller to the database
        /// </summary>
        /// <param name="companyInfo"></param>
        public static void AddReseller(Company companyInfo)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"INSERT INTO Companies 
                                              (IsReseller, ResellerCode, OrgPlanID, CompanyName, CompanyCode, Street, City, State, ZipCode,
                                               PhoneNumber, Website, Description, AdminName, AdminEmail, DistinguishedName, Created, ExchEnabled,
                                               LyncEnabled, CitrixEnabled) VALUES
                                              (@IsReseller, @ResellerCode, @OrgPlanID, @CompanyName, @CompanyCode, @Street, @City, @State, @ZipCode,
                                               @PhoneNumber, @Website, @Description, @AdminName, @AdminEmail, @DistinguishedName, @Created, @ExchEnabled,
                                               @LyncEnabled, @CitrixEnabled)", sql);

            try
            {
                // Add all of the parameters
                cmd.Parameters.AddWithValue("@IsReseller", 1);
                cmd.Parameters.AddWithValue("@ResellerCode", DBNull.Value);
                cmd.Parameters.AddWithValue("@OrgPlanID", 0);
                cmd.Parameters.AddWithValue("@CompanyName", companyInfo.CompanyName);
                cmd.Parameters.AddWithValue("@CompanyCode", companyInfo.CompanyCode);
                cmd.Parameters.AddWithValue("@Street", companyInfo.Street);
                cmd.Parameters.AddWithValue("@City", companyInfo.City);
                cmd.Parameters.AddWithValue("@State", companyInfo.State);
                cmd.Parameters.AddWithValue("@ZipCode", companyInfo.ZipCode);
                cmd.Parameters.AddWithValue("@PhoneNumber", companyInfo.PhoneNumber);
                cmd.Parameters.AddWithValue("@Website", companyInfo.Website);
                cmd.Parameters.AddWithValue("@Description", companyInfo.Description);
                cmd.Parameters.AddWithValue("@AdminName", companyInfo.AdministratorsName);
                cmd.Parameters.AddWithValue("@AdminEmail", companyInfo.AdministratorsEmail);
                cmd.Parameters.AddWithValue("@DistinguishedName", companyInfo.DistinguishedName);
                cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                cmd.Parameters.AddWithValue("@ExchEnabled", false);
                cmd.Parameters.AddWithValue("@LyncEnabled", false);
                cmd.Parameters.AddWithValue("@CitrixEnabled", false);

                // Open the connection
                sql.Open();

                // Insert data
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

        #endregion

        #region Citrix

        /// <summary>
        /// Gets a specific Citrix plan from the database
        /// </summary>
        /// <param name="citrixPlanId"></param>
        /// <returns></returns>
        public static CitrixPlan Get_CitrixPlan(int citrixPlanId)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT  *
                                        FROM Plans_Citrix
                                        WHERE CitrixPlanID=@PlanID", sql);

                // Add our parameter
                cmd.Parameters.AddWithValue("PlanID", citrixPlanId);

                // Open connection
                sql.Open();

                // Create our object to return
                CitrixPlan plan = new CitrixPlan();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        plan.PlanID = citrixPlanId;
                        plan.DisplayName    = r["Name"].ToString();
                        plan.GroupName      = r["GroupName"].ToString();
                        plan.Description    = r["Description"] == DBNull.Value ? "" : r["Description"].ToString();
                        plan.IsServer       = bool.Parse(r["IsServer"].ToString());
                        plan.CompanyCode    = r["CompanyCode"] == DBNull.Value ? "" : r["CompanyCode"].ToString();
                    }
                }
                else
                    plan = null;

                // Return our object
                return plan;
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
        /// Gets all Citrix plans that a company has access too
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<CitrixPlan> Get_CitrixPlans(string companyCode)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT  *
                                        FROM Plans_Citrix
                                        WHERE (CompanyCode IS NULL OR CompanyCode=@CompanyCode OR CompanyCode = '')", sql);

                // Add our parameter
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // Create our object to return
                List<CitrixPlan> plans = new List<CitrixPlan>();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        CitrixPlan tmp = new CitrixPlan();
                        tmp.PlanID = int.Parse(r["CitrixPlanID"].ToString());
                        tmp.DisplayName = r["Name"].ToString();
                        tmp.GroupName = r["GroupName"].ToString();
                        tmp.Description = r["Description"] == DBNull.Value ? "" : r["Description"].ToString();
                        tmp.IsServer = bool.Parse(r["IsServer"].ToString());
                        tmp.CompanyCode = r["CompanyCode"] == DBNull.Value ? "" : r["CompanyCode"].ToString();

                        plans.Add(tmp);
                    }
                }
                else
                    plans = null;

                // Return our object
                return plans;
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
        /// Adds a new Citrix plan
        /// </summary>
        /// <param name="plan"></param>
        public static void New_CitrixPlan(CitrixPlan plan)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"INSERT INTO Plans_Citrix (Name, GroupName, Description, IsServer, CompanyCode, Price, Cost, PictureURL) VALUES (@Name, @GroupName, @Description, @IsServer, @CompanyCode, @Price, @Cost, @PictureURL)", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("Name", plan.DisplayName);
                cmd.Parameters.AddWithValue("GroupName", plan.GroupName);
                cmd.Parameters.AddWithValue("Description", plan.Description);
                cmd.Parameters.AddWithValue("Cost", plan.Cost);
                cmd.Parameters.AddWithValue("Price", plan.Price);
                cmd.Parameters.AddWithValue("IsServer", plan.IsServer);
                cmd.Parameters.AddWithValue("CompanyCode", string.IsNullOrEmpty(plan.CompanyCode) ? "" : plan.CompanyCode);
                cmd.Parameters.AddWithValue("PictureURL", string.IsNullOrEmpty(plan.PictureUrl) ? "" : plan.PictureUrl);

                // Open connection
                sql.Open();

                // Update plan
                int r = cmd.ExecuteNonQuery();
                logger.Info("Inserted new Citrix plan " + plan.DisplayName);
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
        /// Saves an existing Citrix plan
        /// </summary>
        /// <param name="plan"></param>
        public static void Save_CitrixPlan(CitrixPlan plan)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);

                if (string.IsNullOrEmpty(plan.PictureUrl))
                    cmd = new SqlCommand(@"UPDATE Plans_Citrix SET Name=@DisplayName, Description=@Description, Cost=@Cost, Price=@Price, IsServer=@IsServer WHERE CitrixPlanID=@PlanID", sql);
                else
                {
                    cmd = new SqlCommand(@"UPDATE Plans_Citrix SET Name=@DisplayName, Description=@Description, Cost=@Cost, Price=@Price, IsServer=@IsServer, PictureURL=@PictureURL WHERE CitrixPlanID=@PlanID", sql);
                    cmd.Parameters.AddWithValue("PictureURL", string.IsNullOrEmpty(plan.PictureUrl) ? "" : plan.PictureUrl);
                }                  

                // Add our parameters
                cmd.Parameters.AddWithValue("PlanID", plan.PlanID);
                cmd.Parameters.AddWithValue("DisplayName", plan.DisplayName);
                cmd.Parameters.AddWithValue("Description", plan.Description);
                cmd.Parameters.AddWithValue("Cost", plan.Cost);
                cmd.Parameters.AddWithValue("Price", plan.Price);
                cmd.Parameters.AddWithValue("IsServer", plan.IsServer);

                // Open connection
                sql.Open();

                // Update plan
                int r = cmd.ExecuteNonQuery();
                logger.Info("Updated Citrix plan " + plan.DisplayName + " with plan ID " + plan.PlanID.ToString());
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
        /// Deletes a Citrix plan from the database
        /// </summary>
        /// <param name="citrixPlanId"></param>
        public static void Delete_CitrixPlan(int citrixPlanId)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"DELETE FROM Plans_Citrix WHERE CitrixPlanID=@PlanID", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("PlanID", citrixPlanId);

                // Open connection
                sql.Open();

                // Delete and log how many were deleted
                int r = cmd.ExecuteNonQuery();
                logger.Warn("Deleted a total of " + r.ToString() + " Citrix plan(s) from the database when trying to delete plan ID " + citrixPlanId.ToString());
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
        /// Adds a user to a Citrix plan
        /// </summary>
        /// <param name="citrixPlanId"></param>
        /// <param name="userId"></param>
        public static void Add_UserToPlan(int citrixPlanId, int userId)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"IF NOT EXISTS (SELECT * FROM UserPlansCitrix WHERE UserID=@UserID AND CitrixPlanID=@CitrixPlanID)
                                       BEGIN
                                            INSERT INTO UserPlansCitrix (UserID, CitrixPlanID) VALUES (@UserID, @CitrixPlanID)
                                       END", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("UserID", userId);
                cmd.Parameters.AddWithValue("CitrixPlanID", citrixPlanId);

                // Open connection
                sql.Open();

                // Update plan
                int r = cmd.ExecuteNonQuery();
                if (r > 0)
                    logger.Info("Added user ID " + userId.ToString() + " to Citrix plan ID " + citrixPlanId.ToString());
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
        /// Removes a user from a citrix plan
        /// </summary>
        /// <param name="citrixPlanId"></param>
        /// <param name="userId"></param>
        public static void Remove_UserFromPlan(int citrixPlanId, int userId)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"IF EXISTS (SELECT * FROM UserPlansCitrix WHERE UserID=@UserID AND CitrixPlanID=@CitrixPlanID)
                                       BEGIN
                                            DELETE FROM UserPlansCitrix WHERE UserID=@UserID AND CitrixPlanID=@CitrixPlanID
                                       END", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("UserID", userId);
                cmd.Parameters.AddWithValue("CitrixPlanID", citrixPlanId);

                // Open connection
                sql.Open();

                // Update plan
                int r = cmd.ExecuteNonQuery();
                if (r > 0)
                    logger.Info("Removed user ID " + userId.ToString() + " to Citrix plan ID " + citrixPlanId.ToString());
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

        #endregion

        #region Lync

        /// <summary>
        /// Checks to see if Lync is enabled or not for a company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static bool IsLyncEnabled(string companyCode)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT LyncEnabled
                                       FROM Companies
                                       WHERE CompanyCode=@CompanyCode AND IsReseller=0", sql);
                
                // Add parameter
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open connection
                sql.Open();

                object sqlvalue = cmd.ExecuteScalar();
                if (sqlvalue == null || sqlvalue == DBNull.Value)
                    return false;
                else
                    return bool.Parse(sqlvalue.ToString());
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
        /// Modifies the Lync status of a company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="isLyncEnabled"></param>
        public static void ModifyLyncStatus(string companyCode, bool isLyncEnabled)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"UPDATE Companies
                                       SET LyncEnabled=@LyncEnabled
                                       WHERE CompanyCode=@CompanyCode AND IsReseller=0", sql);

                // Add parameter
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);
                cmd.Parameters.AddWithValue("LyncEnabled", isLyncEnabled);

                // Open connection
                sql.Open();

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

        #endregion

        #region Domains

        /// <summary>
        /// Adds a new domain to the database
        /// </summary>
        /// <param name="domain"></param>
        public static void Add_Domain(Domain domain)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"INSERT INTO Domains (CompanyCode, Domain, IsSubDomain, IsDefault, IsAcceptedDomain, IsLyncDomain) VALUES (@CompanyCode, @Domain, @IsSubDomain, @IsDefault, @IsAcceptedDomain, @IsLyncDomain)", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("CompanyCode", domain.CompanyCode);
                cmd.Parameters.AddWithValue("Domain", domain.DomainName);
                cmd.Parameters.AddWithValue("IsSubDomain", domain.IsSubDomain);
                cmd.Parameters.AddWithValue("IsDefault", domain.IsDefault);
                cmd.Parameters.AddWithValue("IsAcceptedDomain", domain.IsAcceptedDomain);
                cmd.Parameters.AddWithValue("IsLyncDomain", domain.IsLyncDomain);

                // Open connection
                sql.Open();

                // Insert domain
                int r = cmd.ExecuteNonQuery();
                if (r > 0)
                    logger.Info("Added new domain to database: " + domain.DomainName);
                else
                    throw new Exception("Failed to add new domain to database: " + domain.DomainName);
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
        /// Gets information about a specific domain
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public static Domain Get_Domain(string domainName)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT  
                                            DomainID, CompanyCode, Domain, IsSubDomain, IsDefault, IsAcceptedDomain, IsLyncDomain,
                                            (SELECT COUNT(*) FROM Users WHERE UserPrincipalName LIKE '%' + d.Domain) AS UserCount,
                                            (SELECT COUNT(*) FROM Contacts c,DistributionGroups g WHERE (c.Email LIKE '%' + d.Domain OR g.Email LIKE '%' + d.Domain)) AS ObjectCount
                                            FROM Domains d
                                            WHERE Domain=@DomainName", sql);
            
                // Add our parameter
                cmd.Parameters.AddWithValue("DomainName", domainName);

                // Open connection
                sql.Open();

                // Create our object to return
                Domain domain = new Domain();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        domain.DomainID = int.Parse(r["DomainID"].ToString());
                        domain.DomainName = r["Domain"].ToString();
                        domain.IsDefault = bool.Parse(r["IsDefault"].ToString());
                        domain.IsAcceptedDomain = bool.Parse(r["IsAcceptedDomain"].ToString());

                        if (r["CompanyCode"] != DBNull.Value)
                            domain.CompanyCode = r["CompanyCode"].ToString();

                        if (r["IsSubDomain"] != DBNull.Value)
                            domain.IsSubDomain = bool.Parse(r["IsSubDomain"].ToString());

                        if (r["IsLyncDomain"] != DBNull.Value)
                            domain.IsLyncDomain = bool.Parse(r["IsLyncDomain"].ToString());

                    }
                }
                else
                    domain = null;

                // Return our object
                return domain;
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
        /// Gets all domains in the system
        /// </summary>
        /// <returns></returns>
        public static List<Domain> Get_Domains()
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT  *
                                        FROM Domains
                                        ORDER BY Domain", sql);
            
                // Open connection
                sql.Open();

                // Create our object to return
                List<Domain> domains = new List<Domain>();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Domain tmp = new Domain();
                        tmp.DomainID = int.Parse(r["DomainID"].ToString());
                        tmp.DomainName = r["Domain"].ToString();
                        tmp.IsDefault = bool.Parse(r["IsDefault"].ToString());
                        tmp.IsAcceptedDomain = bool.Parse(r["IsAcceptedDomain"].ToString());

                        if (r["CompanyCode"] != DBNull.Value)
                            tmp.CompanyCode = r["CompanyCode"].ToString();

                        if (r["IsSubDomain"] != DBNull.Value)
                            tmp.IsSubDomain = bool.Parse(r["IsSubDomain"].ToString());

                        if (r["IsLyncDomain"] != DBNull.Value)
                            tmp.IsLyncDomain = bool.Parse(r["IsLyncDomain"].ToString());

                        domains.Add(tmp);

                    }
                }
                else
                    domains = null;

                // Return our object
                return domains;
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
        /// Gets all domains for a specific company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<Domain> Get_Domains(string companyCode)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT  *
                                        FROM Domains
                                        WHERE CompanyCode=@CompanyCode
                                        ORDER BY Domain", sql);
            
                // Add our parameter
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // Create our object to return
                List<Domain> domains = new List<Domain>();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        Domain tmp = new Domain();
                        tmp.DomainID = int.Parse(r["DomainID"].ToString());
                        tmp.DomainName = r["Domain"].ToString();
                        tmp.IsDefault = bool.Parse(r["IsDefault"].ToString());
                        tmp.IsAcceptedDomain = bool.Parse(r["IsAcceptedDomain"].ToString());

                        if (r["CompanyCode"] != DBNull.Value)
                            tmp.CompanyCode = r["CompanyCode"].ToString();

                        if (r["IsSubDomain"] != DBNull.Value)
                            tmp.IsSubDomain = bool.Parse(r["IsSubDomain"].ToString());

                        if (r["IsLyncDomain"] != DBNull.Value)
                            tmp.IsLyncDomain = bool.Parse(r["IsLyncDomain"].ToString());

                        domains.Add(tmp);

                    }
                }
                else
                    domains = null;

                // Return our object
                return domains;
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
        /// Deletes a domain from the database
        /// </summary>
        /// <param name="domainID"></param>
        public static void Delete_Domain(int domainID)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"DELETE FROM Domains WHERE DomainID=@DomainID", sql);

                // Add our parameter
                cmd.Parameters.AddWithValue("DomainID", domainID);

                // Open connection
                sql.Open();

                // Delete domain
                int r = cmd.ExecuteNonQuery();
                if (r > 0)
                    logger.Info("Removed a total of " + r.ToString() + " domain(s) from the database.");
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
        /// Updates a domain and changes the Lync status
        /// </summary>
        /// <param name="domainname"></param>
        /// <param name="isLyncEnabled"></param>
        public static void Update_DomainLyncStatus(string domainname, bool isLyncEnabled)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"UPDATE Domains SET IsLyncDomain=@IsLyncDomain WHERE Domain=@DomainName", sql);

                // Add our parameter
                cmd.Parameters.AddWithValue("DomainName", domainname);
                cmd.Parameters.AddWithValue("IsLyncDomain", isLyncEnabled);
                
                // Open connection
                sql.Open();

                // Update domain
                int r = cmd.ExecuteNonQuery();
                logger.Info("Updated Domain " + domainname + " and changed the Lync status to: " + isLyncEnabled.ToString());
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
        /// Updates a domain and changes the accepted domain status
        /// </summary>
        /// <param name="domainname"></param>
        /// <param name="isAcceptedDomain"></param>
        public static void Update_DomainAcceptedDomainStatus(string domainname, bool isAcceptedDomain)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"UPDATE Domains SET IsAcceptedDomain=@IsAcceptedDomain WHERE Domain=@DomainName", sql);

                // Add our parameter
                cmd.Parameters.AddWithValue("DomainName", domainname);
                cmd.Parameters.AddWithValue("IsAcceptedDomain", isAcceptedDomain);

                // Open connection
                sql.Open();

                // Update domain
                int r = cmd.ExecuteNonQuery();
                logger.Info("Updated Domain " + domainname + " and changed the Accepted Domain status to: " + isAcceptedDomain.ToString());
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

        #endregion

        #region Users

        /// <summary>
        /// Updates a user in the database
        /// </summary>
        /// <param name="user"></param>
        public static void Update_User(ADUser user)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"UPDATE Users SET sAMAccountName=@sAMAccountName, DisplayName=@DisplayName, Firstname=@Firstname, Middlename=@Middlename, Lastname=@Lastname, Department=@Department,
                                                        IsResellerAdmin=@IsResellerAdmin, IsCompanyAdmin=@IsCompanyAdmin WHERE UserPrincipalName=@UserPrincipalName", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("sAMAccountName", user.SamAccountName);
                cmd.Parameters.AddWithValue("DisplayName", user.DisplayName);
                cmd.Parameters.AddWithValue("Firstname", user.Firstname);
                cmd.Parameters.AddWithValue("Middlename", user.Middlename);
                cmd.Parameters.AddWithValue("Lastname", user.Lastname);
                cmd.Parameters.AddWithValue("Department", user.Department);
                cmd.Parameters.AddWithValue("IsResellerAdmin", user.IsResellerAdmin);
                cmd.Parameters.AddWithValue("IsCompanyAdmin", user.IsCompanyAdmin);
                cmd.Parameters.AddWithValue("UserPrincipalName", user.UserPrincipalName);

                // Open connection
                sql.Open();

                // Insert domain
                int r = cmd.ExecuteNonQuery();
                if (r > 0)
                    logger.Info("Updated one user in the database: " + user.UserPrincipalName);
                else
                    throw new Exception("Failed to update one user in the database " + user.UserPrincipalName);
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
        /// Updates user permissions in the database
        /// </summary>
        /// <param name="user"></param>
        public static void Update_UserPermissions(ADUser user)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"IF NOT EXISTS (SELECT * FROM UserPermissions WHERE UserID=@UserID)
                                       BEGIN
                                            INSERT INTO UserPermissions (UserID, EnableExchange, DisableExchange, AddDomain, DeleteDomain, ModifyAcceptedDomain, ImportUsers) VALUES
                                                                        (@UserID, @EnableExchange, @DisableExchange, @AddDomain, @DeleteDomain, @ModifyAcceptedDomain, @ImportUsers)
                                       END
                                       ELSE
                                       BEGIN
                                            UPDATE UserPermissions SET EnableExchange=@EnableExchange, DisableExchange=@DisableExchange, AddDomain=@AddDomain, DeleteDomain=@DeleteDomain,
                                                                       ModifyAcceptedDomain=@ModifyAcceptedDomain, ImportUsers=@ImportUsers WHERE UserID=@UserID
                                       END", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("UserID", user.UserID);
                cmd.Parameters.AddWithValue("EnableExchange", user.EnableExchangePermission);
                cmd.Parameters.AddWithValue("DisableExchange", user.DisableExchangePermission);
                cmd.Parameters.AddWithValue("AddDomain", user.AddDomainPermission);
                cmd.Parameters.AddWithValue("DeleteDomain", user.DeleteDomainPermission);
                cmd.Parameters.AddWithValue("ModifyAcceptedDomain", user.ModifyAcceptedDomainPermission);
                cmd.Parameters.AddWithValue("ImportUsers", user.ImportUsersPermission);

                // Open connection
                sql.Open();

                // Insert domain
                int r = cmd.ExecuteNonQuery();
                if (r > 0)
                    logger.Info("Updated user permissions for user ID: " + user.UserID.ToString());
                else
                    throw new Exception("Failed to update user permissions for user ID: " + user.UserID.ToString());
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
        /// Gets a user from the datbase
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public static ADUser Get_User(string userPrincipalName)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT * FROM Users u
                                       LEFT JOIN UserPermissions p ON p.UserID=u.ID
                                       WHERE u.UserPrincipalName=@UserPrincipalName", sql);

                // Add our parameter
                cmd.Parameters.AddWithValue("UserPrincipalName", userPrincipalName);

                // DEBUG
                logger.Debug("Attempting to find user " + userPrincipalName + " in the database.");

                // Open connection
                sql.Open();

                // Create our object to return
                ADUser user = new ADUser();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    logger.Debug("Found user " + userPrincipalName + " in the database.");

                    while (r.Read())
                    {
                        user.UserID = int.Parse(r["ID"].ToString());
                        user.UserGuid = Guid.Parse(r["UserGuid"].ToString());
                        user.UserPrincipalName = r["UserPrincipalName"].ToString();
                        user.DisplayName = r["DisplayName"].ToString();
                        user.CompanyCode = r["CompanyCode"].ToString();

                        if (r["sAMAccountName"] != DBNull.Value)
                            user.SamAccountName = r["sAMAccountName"].ToString();

                        if (r["DistinguishedName"] != DBNull.Value)
                            user.DistinguishedName = r["DistinguishedName"].ToString();

                        if (r["Firstname"] != DBNull.Value)
                            user.Firstname = r["Firstname"].ToString();

                        if (r["Middlename"] != DBNull.Value)
                            user.Middlename = r["Middlename"].ToString();

                        if (r["Lastname"] != DBNull.Value)
                            user.Lastname = r["Lastname"].ToString();

                        if (r["Department"] != DBNull.Value)
                            user.Department = r["Department"].ToString();

                        if (r["IsResellerAdmin"] != DBNull.Value)
                            user.IsResellerAdmin = bool.Parse(r["IsResellerAdmin"].ToString());

                        if (r["IsCompanyADmin"] != DBNull.Value)
                            user.IsCompanyAdmin = bool.Parse(r["IsCompanyAdmin"].ToString());

                        if (r["Created"] != DBNull.Value)
                            user.Created = DateTime.Parse(r["Created"].ToString());

                        if (r["EnableExchange"] != DBNull.Value)
                            user.EnableExchangePermission = bool.Parse(r["EnableExchange"].ToString());

                        if (r["DisableExchange"] != DBNull.Value)
                            user.DisableExchangePermission = bool.Parse(r["DisableExchange"].ToString());

                        if (r["AddDomain"] != DBNull.Value)
                            user.AddDomainPermission = bool.Parse(r["AddDomain"].ToString());

                        if (r["DeleteDomain"] != DBNull.Value)
                            user.DeleteDomainPermission = bool.Parse(r["DeleteDomain"].ToString());

                        if (r["ModifyAcceptedDomain"] != DBNull.Value)
                            user.ModifyAcceptedDomainPermission = bool.Parse(r["ModifyAcceptedDomain"].ToString());

                        if (r["ImportUsers"] != DBNull.Value)
                            user.ImportUsersPermission = bool.Parse(r["ImportUsers"].ToString());
                    }

                    logger.Debug("Finished reading values for user " + userPrincipalName + " in the database.");
                }
                else
                    user = null;

                // Return our object
                return user;
            }
            catch (Exception ex)
            {
                logger.Error("Failed to retrieve user " + userPrincipalName + " from the database.", ex);

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
        /// Get a list of all users in the system
        /// </summary>
        /// <returns></returns>
        public static List<ADUser> Get_Users()
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT * FROM Users u
                                       LEFT JOIN UserPermissions p ON p.UserID=u.ID", sql);

                // Open connection
                sql.Open();

                // Create our object to return
                List<ADUser> users = new List<ADUser>();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        ADUser user = new ADUser();
                        user.UserID = int.Parse(r["ID"].ToString());
                        user.UserGuid = Guid.Parse(r["UserGuid"].ToString());
                        user.UserPrincipalName = r["UserPrincipalName"].ToString();
                        user.DisplayName = r["DisplayName"].ToString();

                        if (r["sAMAccountName"] != DBNull.Value)
                            user.SamAccountName = r["sAMAccountName"].ToString();

                        if (r["DistinguishedName"] != DBNull.Value)
                            user.DistinguishedName = r["DistinguishedName"].ToString();

                        if (r["Firstname"] != DBNull.Value)
                            user.Firstname = r["Firstname"].ToString();

                        if (r["Middlename"] != DBNull.Value)
                            user.Middlename = r["Middlename"].ToString();

                        if (r["Lastname"] != DBNull.Value)
                            user.Lastname = r["Lastname"].ToString();

                        if (r["Email"] != DBNull.Value)
                            user.Email = r["Email"].ToString();

                        if (r["MailboxPlan"] != DBNull.Value)
                            user.MailboxPlanID = int.Parse(r["MailboxPlan"].ToString());

                        if (r["Department"] != DBNull.Value)
                            user.Department = r["Department"].ToString();

                        if (r["IsResellerAdmin"] != DBNull.Value)
                            user.IsResellerAdmin = bool.Parse(r["IsResellerAdmin"].ToString());

                        if (r["IsCompanyADmin"] != DBNull.Value)
                            user.IsCompanyAdmin = bool.Parse(r["IsCompanyAdmin"].ToString());

                        if (r["Created"] != DBNull.Value)
                            user.Created = DateTime.Parse(r["Created"].ToString());

                        if (r["EnableExchange"] != DBNull.Value)
                            user.EnableExchangePermission = bool.Parse(r["EnableExchange"].ToString());

                        if (r["DisableExchange"] != DBNull.Value)
                            user.DisableExchangePermission = bool.Parse(r["DisableExchange"].ToString());

                        if (r["AddDomain"] != DBNull.Value)
                            user.AddDomainPermission = bool.Parse(r["AddDomain"].ToString());

                        if (r["DeleteDomain"] != DBNull.Value)
                            user.DeleteDomainPermission = bool.Parse(r["DeleteDomain"].ToString());

                        if (r["ModifyAcceptedDomain"] != DBNull.Value)
                            user.ModifyAcceptedDomainPermission = bool.Parse(r["ModifyAcceptedDomain"].ToString());

                        if (r["ImportUsers"] != DBNull.Value)
                            user.ImportUsersPermission = bool.Parse(r["ImportUsers"].ToString());

                        // Add to our list
                        users.Add(user);
                    }
                }
                else
                    users = null;

                // Return our object
                return users;
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
        /// Gets a list of all users from a company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<ADUser> Get_Users(string companyCode)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT * FROM Users u
                                       LEFT JOIN UserPermissions p ON p.UserID=u.ID
                                       WHERE u.CompanyCode=@CompanyCode", sql);

                // Add our parameter
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // Create our object to return
                List<ADUser> users = new List<ADUser>();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        ADUser user = new ADUser();
                        user.UserID = int.Parse(r["ID"].ToString());
                        user.UserGuid = Guid.Parse(r["UserGuid"].ToString());
                        user.UserPrincipalName = r["UserPrincipalName"].ToString();
                        user.DisplayName = r["DisplayName"].ToString();

                        if (r["sAMAccountName"] != DBNull.Value)
                            user.SamAccountName = r["sAMAccountName"].ToString();

                        if (r["DistinguishedName"] != DBNull.Value)
                            user.DistinguishedName = r["DistinguishedName"].ToString();

                        if (r["Firstname"] != DBNull.Value)
                            user.Firstname = r["Firstname"].ToString();

                        if (r["Middlename"] != DBNull.Value)
                            user.Middlename = r["Middlename"].ToString();

                        if (r["Lastname"] != DBNull.Value)
                            user.Lastname = r["Lastname"].ToString();

                        if (r["Email"] != DBNull.Value)
                            user.Email = r["Email"].ToString();

                        if (r["MailboxPlan"] != DBNull.Value)
                            user.MailboxPlanID = int.Parse(r["MailboxPlan"].ToString());

                        if (r["Department"] != DBNull.Value)
                            user.Department = r["Department"].ToString();

                        if (r["IsResellerAdmin"] != DBNull.Value)
                            user.IsResellerAdmin = bool.Parse(r["IsResellerAdmin"].ToString());

                        if (r["IsCompanyADmin"] != DBNull.Value)
                            user.IsCompanyAdmin = bool.Parse(r["IsCompanyAdmin"].ToString());

                        if (r["Created"] != DBNull.Value)
                            user.Created = DateTime.Parse(r["Created"].ToString());

                        if (r["EnableExchange"] != DBNull.Value)
                            user.EnableExchangePermission = bool.Parse(r["EnableExchange"].ToString());

                        if (r["DisableExchange"] != DBNull.Value)
                            user.DisableExchangePermission = bool.Parse(r["DisableExchange"].ToString());

                        if (r["AddDomain"] != DBNull.Value)
                            user.AddDomainPermission = bool.Parse(r["AddDomain"].ToString());

                        if (r["DeleteDomain"] != DBNull.Value)
                            user.DeleteDomainPermission = bool.Parse(r["DeleteDomain"].ToString());

                        if (r["ModifyAcceptedDomain"] != DBNull.Value)
                            user.ModifyAcceptedDomainPermission = bool.Parse(r["ModifyAcceptedDomain"].ToString());

                        if (r["ImportUsers"] != DBNull.Value)
                            user.ImportUsersPermission = bool.Parse(r["ImportUsers"].ToString());

                        // Add to our list
                        users.Add(user);
                    }
                }
                else
                    users = null;

                // Return our object
                return users;
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
        /// Updates a user mailbox information
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <param name="mailboxPlanID"></param>
        /// <param name="primarySmtpAddress"></param>
        /// <param name="additionaMBAdded"></param>
        /// <param name="activeSyncPlan"></param>
        public static void Update_UserMailboxInfo(string userPrincipalName, int mailboxPlanID, string primarySmtpAddress, int additionaMBAdded, string activeSyncPlan)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);

                if (string.IsNullOrEmpty(activeSyncPlan))
                    cmd = new SqlCommand(@"UPDATE Users SET MailboxPlan=@MailboxPlanID, Email=@PrimarySmtpAddress, AdditionalMB=@AdditionalMB, ActiveSyncPlan=0 WHERE UserPrincipalName=@UserPrincipalName", sql);
                else
                {
                    cmd = new SqlCommand(@"UPDATE Users SET MailboxPlan=@MailboxPlanID, Email=@PrimarySmtpAddress, AdditionalMB=@AdditionalMB, 
                                           ActiveSyncPlan=(SELECT ASID FROM Plans_ExchangeActiveSync WHERE DisplayName=@ActiveSyncPlan)
                                           WHERE UserPrincipalName=@UserPrincipalName", sql);
                    cmd.Parameters.AddWithValue("ActiveSyncPlan", activeSyncPlan);
                }

                // Add our parameters
                cmd.Parameters.AddWithValue("MailboxPlanID", mailboxPlanID);
                cmd.Parameters.AddWithValue("PrimarySmtpAddress", primarySmtpAddress);
                cmd.Parameters.AddWithValue("AdditionalMB", additionaMBAdded);
                cmd.Parameters.AddWithValue("UserPrincipalName", userPrincipalName);

                // Open connection
                sql.Open();

                // Insert domain
                int r = cmd.ExecuteNonQuery();
                if (r > 0)
                    logger.Info("Updated one user in the database for mailbox information: " + userPrincipalName);
                else
                    throw new Exception("Failed to update one user in the database " + userPrincipalName);
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

        #endregion

        #region Plans

        /// <summary>
        /// Gets a specific mailbox plan from the database
        /// </summary>
        /// <param name="mailboxPlanID"></param>
        /// <returns></returns>
        public static MailboxPlan Get_MailboxPlan(int mailboxPlanID)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT  *
                                        FROM    Plans_ExchangeMailbox
                                        WHERE MailboxPlanID=@MailboxPlanID", sql);

                // Add parameter
                cmd.Parameters.AddWithValue("MailboxPlanID", mailboxPlanID);

                // Open connection
                sql.Open();

                // Create our object to return
                MailboxPlan plan = new MailboxPlan();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        plan.PlanID = int.Parse(r["MailboxPlanID"].ToString());
                        plan.DisplayName = r["MailboxPlanName"].ToString();
                        plan.SizeInMB = int.Parse(r["MailboxSizeMB"].ToString());
                        plan.MaxSendSizeInKB = int.Parse(r["MaxSendKB"].ToString());
                        plan.MaxReceiveSizeInKB = int.Parse(r["MaxReceiveKB"].ToString());
                        plan.MaxRecipients = int.Parse(r["MaxRecipients"].ToString());
                        plan.POP3Enabled = bool.Parse(r["EnablePOP3"].ToString());
                        plan.IMAPEnabled = bool.Parse(r["EnableIMAP"].ToString());
                        plan.OWAEnabled = bool.Parse(r["EnableOWA"].ToString());
                        plan.MAPIEnabled = bool.Parse(r["EnableMAPI"].ToString());
                        plan.ActiveSyncEnabled = bool.Parse(r["EnableAS"].ToString());
                        plan.ECPEnabled = bool.Parse(r["EnableECP"].ToString());
                        plan.KeepDeletedItemsInDays = int.Parse(r["MaxKeepDeletedItems"].ToString());

                        if (r["MaxMailboxSizeMB"] != DBNull.Value)
                            plan.MaximumSizeInMB = int.Parse(r["MaxMailboxSizeMB"].ToString());
                        else
                            plan.MaximumSizeInMB = plan.SizeInMB;

                        if (r["MailboxPlanDesc"] != DBNull.Value)
                            plan.Description = r["MailboxPlanDesc"].ToString();

                        if (r["Price"] != DBNull.Value)
                            plan.Price = r["Price"].ToString();
                        else
                            plan.Price = "0";

                        if (r["Cost"] != DBNull.Value)
                            plan.Cost = r["Cost"].ToString();
                        else
                            plan.Cost = "0";

                        if (r["AdditionalGBPrice"] != DBNull.Value)
                            plan.AdditionalGBPrice = r["AdditionalGBPrice"].ToString();
                        else
                            plan.AdditionalGBPrice = "0";

                        if (r["CompanyCode"] != DBNull.Value)
                            plan.CompanyCode = r["CompanyCode"].ToString();
                    }
                }
                else
                    plan = null;

                // Return our object
                return plan;
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
        /// Gets a list of mailbox plans for a company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<MailboxPlan> Get_MailboxPlans(string companyCode)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT  *
                                        FROM    Plans_ExchangeMailbox
                                        WHERE (CompanyCode=@CompanyCode OR CompanyCode IS NULL OR CompanyCode = '')
                                        ORDER BY MailboxPlanName", sql);

                // Add parameter
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // Create our object to return
                List<MailboxPlan> plans = new List<MailboxPlan>();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        MailboxPlan tmp = new MailboxPlan();
                        tmp.PlanID = int.Parse(r["MailboxPlanID"].ToString());
                        tmp.DisplayName = r["MailboxPlanName"].ToString();
                        tmp.SizeInMB = int.Parse(r["MailboxSizeMB"].ToString());
                        tmp.MaxSendSizeInKB = int.Parse(r["MaxSendKB"].ToString());
                        tmp.MaxReceiveSizeInKB = int.Parse(r["MaxReceiveKB"].ToString());
                        tmp.MaxRecipients = int.Parse(r["MaxRecipients"].ToString());
                        tmp.POP3Enabled = bool.Parse(r["EnablePOP3"].ToString());
                        tmp.IMAPEnabled = bool.Parse(r["EnableIMAP"].ToString());
                        tmp.OWAEnabled = bool.Parse(r["EnableOWA"].ToString());
                        tmp.MAPIEnabled = bool.Parse(r["EnableMAPI"].ToString());
                        tmp.ActiveSyncEnabled = bool.Parse(r["EnableAS"].ToString());
                        tmp.ECPEnabled = bool.Parse(r["EnableECP"].ToString());
                        tmp.KeepDeletedItemsInDays = int.Parse(r["MaxKeepDeletedItems"].ToString());
                        
                        if (r["MaxMailboxSizeMB"] != DBNull.Value)
                            tmp.MaximumSizeInMB = int.Parse(r["MaxMailboxSizeMB"].ToString());
                        else
                            tmp.MaximumSizeInMB = tmp.SizeInMB;

                        if (r["MailboxPlanDesc"] != DBNull.Value)
                            tmp.Description = r["MailboxPlanDesc"].ToString();

                        if (r["Price"] != DBNull.Value)
                            tmp.Price = r["Price"].ToString();
                        else
                            tmp.Price = "0";

                        if (r["Cost"] != DBNull.Value)
                            tmp.Cost = r["Cost"].ToString();
                        else
                            tmp.Cost = "0";

                        if (r["AdditionalGBPrice"] != DBNull.Value)
                            tmp.AdditionalGBPrice = r["AdditionalGBPrice"].ToString();
                        else
                            tmp.AdditionalGBPrice = "0";

                        if (r["CompanyCode"] != DBNull.Value)
                            tmp.CompanyCode = r["CompanyCode"].ToString();

                        plans.Add(tmp);
                    }
                }
                else
                    plans = null;

                // Return our object
                return plans;
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

        #endregion

        #region Tasks & Queues

        /// <summary>
        /// Adds a database queue to the database
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="userPrincipalName"></param>
        /// <param name="companyCode"></param>
        /// <param name="delayInMinutes"></param>
        /// <param name="taskStatus"></param>
        public static void Add_DatabaseQueue(Enumerations.TaskType taskType, string userPrincipalName, string companyCode, int delayInMinutes, Enumerations.TaskSuccess taskStatus)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"INSERT INTO SvcQueue (TaskID, UserPrincipalName, CompanyCode, TaskCreated, TaskDelayInMinutes, TaskSuccess) VALUES 
                                                            (@TaskID, @UserPrincipalName, @CompanyCode, @TaskCreated, @TaskDelayInMinutes, @TaskSuccess)", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("TaskID", (int)taskType);
                cmd.Parameters.AddWithValue("UserPrincipalName", userPrincipalName);
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);
                cmd.Parameters.AddWithValue("TaskCreated", DateTime.Now);
                cmd.Parameters.AddWithValue("TaskDelayInMinutes", delayInMinutes);
                cmd.Parameters.AddWithValue("TaskSuccess", (int)taskStatus);

                // Open connection
                sql.Open();

                // Insert domain
                int r = cmd.ExecuteNonQuery();
                if (r > 0)
                    logger.Info("Added queue to the database for: " + userPrincipalName);
                else
                    throw new Exception("Failed to add queue to the database for: " + userPrincipalName);
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
        /// Gets a list of database tasks from the database rather they are ready or not
        /// </summary>
        public static List<DatabaseTask> Get_DatabaseTasksReoccurring()
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT * FROM SvcTask WHERE Reoccurring=1", sql);

                // Open connection
                sql.Open();

                // Create our object to return
                List<DatabaseTask> tasks = new List<DatabaseTask>();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        DatabaseTask tmp = new DatabaseTask();
                        tmp.TaskID = int.Parse(r["SvcTaskID"].ToString());
                        tmp.TaskType = (Enumerations.TaskType)int.Parse(r["TaskType"].ToString());
                        tmp.TaskDelayInMinutes = int.Parse(r["TaskDelayInMinutes"].ToString());
                        tmp.IsReoccurringTask = bool.Parse(r["Reoccurring"].ToString());

                        if (r["LastRun"] != DBNull.Value)
                            tmp.LastRun = DateTime.Parse(r["LastRun"].ToString());

                        if (r["NextRun"] != DBNull.Value)
                            tmp.NextRun = DateTime.Parse(r["NextRun"].ToString());

                        if (r["TaskOutput"] != DBNull.Value)
                            tmp.TaskOutput = r["TaskOutput"].ToString();

                        if (r["TaskCreated"] != DBNull.Value)
                            tmp.WhenCreated = DateTime.Parse(r["TaskCreated"].ToString());

                        tasks.Add(tmp);
                    }
                }
                else
                    tasks = null;

                // Return our object
                return tasks;
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
        /// Gets al ist of database tasks from the database that are ready to execute
        /// </summary>
        /// <returns></returns>
        public static List<DatabaseTask> Get_DatabaseTasksReoccurringReady()
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT * FROM SvcTask s WHERE Reoccurring=1 AND GetDate() >= dateadd(MINUTE, s.TaskDelayInMinutes, s.LastRun) ORDER BY LastRun", sql);

                // Open connection
                sql.Open();

                // Create our object to return
                List<DatabaseTask> tasks = new List<DatabaseTask>();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        DatabaseTask tmp = new DatabaseTask();
                        tmp.TaskID = int.Parse(r["SvcTaskID"].ToString());
                        tmp.TaskType = (Enumerations.TaskType)int.Parse(r["TaskType"].ToString());
                        tmp.TaskDelayInMinutes = int.Parse(r["TaskDelayInMinutes"].ToString());
                        tmp.IsReoccurringTask = bool.Parse(r["Reoccurring"].ToString());

                        if (r["LastRun"] != DBNull.Value)
                            tmp.LastRun = DateTime.Parse(r["LastRun"].ToString());

                        if (r["NextRun"] != DBNull.Value)
                            tmp.NextRun = DateTime.Parse(r["NextRun"].ToString());

                        if (r["TaskOutput"] != DBNull.Value)
                            tmp.TaskOutput = r["TaskOutput"].ToString();

                        if (r["TaskCreated"] != DBNull.Value)
                            tmp.WhenCreated = DateTime.Parse(r["TaskCreated"].ToString());

                        tasks.Add(tmp);
                    }
                }
                else
                    tasks = null;

                // Return our object
                return tasks;
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
        /// Adds or updates a task in the database
        /// </summary>
        /// <param name="task"></param>
        public static void Update_DatabaseTask(DatabaseTask task)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"IF NOT EXISTS (SELECT * FROM SvcTask WHERE TaskType=@TaskType AND Reoccurring=1)
                                       BEGIN
                                           INSERT INTO SvcTask (TaskType,LastRun,NextRun,TaskDelayInMinutes,TaskCreated,Reoccurring) VALUES (@TaskType,@LastRun,@NextRun,@TaskDelayInMinutes,@TaskCreated,@Reoccurring)
                                       END
                                       ELSE
                                       BEGIN
                                           UPDATE SvcTask SET LastRun=@LastRun,NextRun=@NextRun,TaskDelayInMinutes=@TaskDelayInMinutes WHERE TaskType=@TaskType AND Reoccurring=1
                                       END", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("@TaskType", (int)task.TaskType);
                cmd.Parameters.AddWithValue("@LastRun", DateTime.Now);
                cmd.Parameters.AddWithValue("@NextRun", task.NextRun);
                cmd.Parameters.AddWithValue("@TaskDelayInMinutes", task.TaskDelayInMinutes);
                cmd.Parameters.AddWithValue("@TaskCreated", DateTime.Now);
                cmd.Parameters.AddWithValue("@Reoccurring", task.IsReoccurringTask);

                // Open connection
                sql.Open();

                // Insert or update task
                int r = cmd.ExecuteNonQuery();
                if (r > 0)
                    logger.Info("Updated task in database: " + task.TaskType.ToString());
                else
                    throw new Exception("Failed update task in database: " + task.TaskType.ToString());
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
        /// Removes a task from the database
        /// </summary>
        /// <param name="task"></param>
        public static void Remove_DatabaseTask(DatabaseTask task)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"DELETE FROM SvcTask WHERE TaskType=@TaskType AND Reoccurring=@Reoccurring", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("@TaskType", (int)task.TaskType);
                cmd.Parameters.AddWithValue("@Reoccurring", task.IsReoccurringTask);

                // Open connection
                sql.Open();

                // Insert or update task
                int r = cmd.ExecuteNonQuery();
                if (r > 0)
                    logger.Info("Deleted task in database: " + task.TaskType.ToString());
                else
                    logger.Error("Failed to delete task in database because it didn't exist: " + task.TaskType.ToString());
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
        /// Gets a list of queues waiting to be completed that are ready to be executed
        /// </summary>
        public static List<DatabaseQueue> Get_DatabaseQueuesWaiting()
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"SELECT * FROM SvcQueue s WHERE TaskCompleted IS NULL AND GetDate() >= dateadd(MINUTE, s.TaskDelayInMinutes, s.TaskCreated) ORDER BY TaskCreated", sql);

                // Open connection
                sql.Open();

                // Create our object to return
                List<DatabaseQueue> queues = new List<DatabaseQueue>();

                // Read the data
                r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        DatabaseQueue tmp = new DatabaseQueue();
                        tmp.QueueID = int.Parse(r["SvcQueueID"].ToString());
                        tmp.TaskType = (Enumerations.TaskType)int.Parse(r["TaskID"].ToString());
                        tmp.TaskDelayInMinutes = int.Parse(r["TaskDelayInMinutes"].ToString());

                        if (r["TaskSuccess"] != DBNull.Value)
                            tmp.TaskSuccess = (Enumerations.TaskSuccess)int.Parse(r["TaskSuccess"].ToString());

                        if (r["UserPrincipalName"] != DBNull.Value)
                            tmp.UserPrincipalName = r["UserPrincipalName"].ToString();

                        if (r["CompanyCode"] != DBNull.Value)
                            tmp.CompanyCode = r["CompanyCode"].ToString();

                        if (r["TaskOutput"] != DBNull.Value)
                            tmp.TaskOutput = r["TaskOutput"].ToString();

                        if (r["TaskCreated"] != DBNull.Value)
                            tmp.TaskCreated = DateTime.Parse(r["TaskCreated"].ToString());

                        queues.Add(tmp);
                    }
                }
                else
                    queues = null;

                // Return our object
                return queues;
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
        /// Updates or inserts a new database queue
        /// </summary>
        /// <param name="queue"></param>
        public static void Update_DatabaseQueue(DatabaseQueue queue)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"IF NOT EXISTS (SELECT * FROM SvcQueue WHERE SvcQueueID=@QueueID)
                                       BEGIN
                                           INSERT INTO SvcQueue (TaskID, UserPrincipalName, CompanyCode, TaskOutput, TaskCreated, TaskCompleted, TaskDelayInMinutes, TaskSuccess) VALUES 
                                                               (@TaskID, @UserPrincipalName, @CompanyCode, @TaskOutput, @TaskCreated, @TaskCompleted, @TaskDelayInMinutes, @TaskSuccess)
                                       END
                                       ELSE
                                       BEGIN
                                           UPDATE SvcQueue SET TaskOutput=@TaskOutput, TaskCompleted=@TaskCompleted, TaskSuccess=@TaskSuccess WHERE SvcQueueID=@QueueID
                                       END", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("@QueueID", queue.QueueID);
                cmd.Parameters.AddWithValue("@TaskID", (int)queue.TaskType);
                cmd.Parameters.AddWithValue("@UserPrincipalName", queue.UserPrincipalName);
                cmd.Parameters.AddWithValue("@CompanyCode", queue.CompanyCode);
                cmd.Parameters.AddWithValue("@TaskOutput", queue.TaskOutput);
                cmd.Parameters.AddWithValue("@TaskCreated", queue.TaskCreated);
                cmd.Parameters.AddWithValue("@TaskCompleted", queue.TaskCompleted);
                cmd.Parameters.AddWithValue("@TaskDelayInMinutes", queue.TaskDelayInMinutes);
                cmd.Parameters.AddWithValue("@TaskSuccess", queue.TaskSuccess);

                // Open connection
                sql.Open();

                // Insert or update task
                int r = cmd.ExecuteNonQuery();
                if (r > 0)
                    logger.Info("Updated task in database: " + queue.TaskType.ToString());
                else
                    throw new Exception("Failed update task in database: " + queue.TaskType.ToString());
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

        #endregion

        #region Statistics

        /// <summary>
        /// Adds mailbox stats to the database
        /// </summary>
        /// <param name="usr"></param>
        public static void Add_MailboxSizeStat(MailboxUser usr)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"INSERT INTO SvcMailboxSizes (UserPrincipalName, MailboxDatabase, TotalItemSizeInKB, TotalDeletedItemSizeInKB, ItemCount, DeletedItemCount, Retrieved) VALUES
                                                                   (@UserPrincipalName, @MailboxDatabase, @TotalItemSizeInKB, @TotalDeletedItemSizeInKB, @ItemCount, @DeletedItemCount, @Retrieved)", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("UserPrincipalName", usr.UserPrincipalName);
                cmd.Parameters.AddWithValue("MailboxDatabase", usr.Database);
                cmd.Parameters.AddWithValue("TotalItemSizeInKB", usr.TotalItemSize);
                cmd.Parameters.AddWithValue("TotalDeletedItemSizeInKB", usr.TotalDeletedItemSize);
                cmd.Parameters.AddWithValue("ItemCount", usr.ItemCount);
                cmd.Parameters.AddWithValue("DeletedItemCount", usr.DeletedItemCount);
                cmd.Parameters.AddWithValue("Retrieved", usr.MailboxDataRetrieved);

                // Open connection
                sql.Open();

                // Insert data
                int r = cmd.ExecuteNonQuery();
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
        /// Adds mailbox database information to database
        /// </summary>
        /// <param name="db"></param>
        public static void Add_MailboxDatabaseStat(MailboxDatabase db)
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand(@"INSERT INTO SvcMailboxDatabaseSizes (DatabaseName, Server, DatabaseSize, Retrieved) VALUES (@DatabaseName, @Server, @DatabaseSize, @Retrieved)", sql);

                // Add our parameters
                cmd.Parameters.AddWithValue("DatabaseName", db.Identity);
                cmd.Parameters.AddWithValue("Server", db.Server);
                cmd.Parameters.AddWithValue("DatabaseSize", db.DatabaseSize);
                cmd.Parameters.AddWithValue("Retrieved", DateTime.Now);

                // Open connection
                sql.Open();

                // Insert data
                int r = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.Error("Failed to add mailbox database stat: " + ex.ToString());

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

        #endregion

        #region Settings



        #endregion
    }
}
