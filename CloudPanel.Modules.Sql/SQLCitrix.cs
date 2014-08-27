using CloudPanel.Modules.Base;
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
    public class SQLCitrix
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Gets a particular Citrix plan
        /// </summary>
        /// <param name="citrixAppID"></param>
        /// <returns></returns>
        public static BaseCitrixApp GetCitrixPlan(string citrixAppID)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM Plans_Citrix WHERE CitrixPlanID=@CitrixPlanID", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("CitrixPlanID", citrixAppID);

                // Open connection
                sql.Open();

                // Create our return object
                BaseCitrixApp tmp = new BaseCitrixApp();

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        tmp.ID = int.Parse(r["CitrixPlanID"].ToString());
                        tmp.DisplayName = r["Name"].ToString();
                        tmp.GroupName = r["GroupName"].ToString();
                        tmp.IsServer = bool.Parse(r["IsServer"].ToString());

                        if (r["CompanyCode"] != DBNull.Value)
                            tmp.CompanyCode = r["CompanyCode"].ToString();

                        if (r["Description"] != DBNull.Value)
                            tmp.Description = r["Description"].ToString();

                        if (r["Cost"] != DBNull.Value)
                            tmp.Cost = r["Cost"].ToString();
                        else
                            tmp.Cost = "0.00";

                        if (r["Price"] != DBNull.Value)
                            tmp.Price = r["Price"].ToString();
                        else
                            tmp.Price = "0.00";

                        if (r["PictureURL"] != DBNull.Value)
                            tmp.PictureURL = r["PictureURL"].ToString();
                    }
                }

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return
                return tmp;
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
        /// Gets all citrix plans in the database
        /// </summary>
        /// <returns></returns>
        public static List<BaseCitrixApp> GetCitrixPlans()
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT *,(SELECT COUNT(UserID) FROM UserPlansCitrix WHERE CitrixPlanID=p.CitrixPlanID) AS UserCount  
                                                     FROM Plans_Citrix p ORDER BY Name", sql);

            try
            {
                // Open connection
                sql.Open();

                // Create collection
                List<BaseCitrixApp> citrixApps = new List<BaseCitrixApp>();

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        BaseCitrixApp tmp = new BaseCitrixApp();
                        tmp.ID = int.Parse(r["CitrixPlanID"].ToString());
                        tmp.DisplayName = r["Name"].ToString();
                        tmp.GroupName = r["GroupName"].ToString();
                        tmp.IsServer = bool.Parse(r["IsServer"].ToString());

                        if (r["CompanyCode"] != DBNull.Value)
                            tmp.CompanyCode = r["CompanyCode"].ToString();

                        if (r["Description"] != DBNull.Value)
                            tmp.Description = r["Description"].ToString();

                        if (r["Cost"] != DBNull.Value)
                            tmp.Cost = r["Cost"].ToString();
                        else
                            tmp.Cost = "0.00";

                        if (r["Price"] != DBNull.Value)
                            tmp.Price = r["Price"].ToString();
                        else
                            tmp.Price = "0.00";

                        if (r["PictureURL"] != DBNull.Value)
                            tmp.PictureURL = r["PictureURL"].ToString();

                        if (r["UserCount"] != DBNull.Value)
                            tmp.CurrentUsers = int.Parse(r["UserCount"].ToString());

                        citrixApps.Add(tmp);
                    }
                }

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return
                return citrixApps;
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
        /// Gets all citrix plans for a specific company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<BaseCitrixApp> GetCitrixPlans(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM Plans_Citrix WHERE CompanyCode=@CompanyCode ORDER BY Name", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // Create collection
                List<BaseCitrixApp> citrixApps = new List<BaseCitrixApp>();

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        BaseCitrixApp tmp = new BaseCitrixApp();
                        tmp.ID = int.Parse(r["CitrixPlanID"].ToString());
                        tmp.DisplayName = r["Name"].ToString();
                        tmp.GroupName = r["GroupName"].ToString();
                        tmp.IsServer = bool.Parse(r["IsServer"].ToString());

                        if (r["CompanyCode"] != DBNull.Value)
                            tmp.CompanyCode = r["CompanyCode"].ToString();

                        if (r["Description"] != DBNull.Value)
                            tmp.Description = r["Description"].ToString();

                        if (r["Cost"] != DBNull.Value)
                            tmp.Cost = r["Cost"].ToString();
                        else
                            tmp.Cost = "0.00";

                        if (r["Price"] != DBNull.Value)
                            tmp.Price = r["Price"].ToString();
                        else
                            tmp.Price = "0.00";

                        if (r["PictureURL"] != DBNull.Value)
                            tmp.PictureURL = r["PictureURL"].ToString();

                        citrixApps.Add(tmp);
                    }
                }

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return
                return citrixApps;
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
        /// Adds a citrix plan to the database
        /// </summary>
        /// <param name="citrixApp"></param>
        public static void AddCitrixPlan(BaseCitrixApp citrixApp)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"INSERT INTO Plans_Citrix (Name, GroupName, Description, IsServer, CompanyCode, Price, Cost, PictureURL) VALUES
                                                                       (@Name, @GroupName, @Description, @IsServer, @CompanyCode, @Price, @Cost, @PictureURL)", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("Name", citrixApp.DisplayName);
                cmd.Parameters.AddWithValue("GroupName", citrixApp.GroupName);
                cmd.Parameters.AddWithValue("Description", citrixApp.Description);
                cmd.Parameters.AddWithValue("IsServer", citrixApp.IsServer);
                cmd.Parameters.AddWithValue("CompanyCode", citrixApp.CompanyCode);
                cmd.Parameters.AddWithValue("Price", citrixApp.Price);
                cmd.Parameters.AddWithValue("Cost", citrixApp.Cost);

                if (!string.IsNullOrEmpty(citrixApp.PictureURL))
                    cmd.Parameters.AddWithValue("PictureURL", citrixApp.PictureURL);
                else
                    cmd.Parameters.AddWithValue("PictureURL", string.Empty);

                // Open connection
                sql.Open();

                // Insert
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

        /// <summary>
        /// Updates a citrix plan in the database
        /// </summary>
        /// <param name="citrixApp"></param>
        public static void UpdateCitrixPlan(BaseCitrixApp citrixApp)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"", sql);

            try
            {
                if (!string.IsNullOrEmpty(citrixApp.PictureURL))
                {
                    cmd.CommandText = @"UPDATE Plans_Citrix SET Name=@Name, Description=@Description, IsServer=@IsServer,
                                                                      CompanyCode=@CompanyCode, Price=@Price, Cost=@Cost, PictureURL=@PictureURL
                                                                WHERE 
                                                                      CitrixPlanID=@CitrixPlanID";
                    cmd.Parameters.AddWithValue("PictureURL", citrixApp.PictureURL);
                }
                else
                {
                    cmd.CommandText = @"UPDATE Plans_Citrix SET Name=@Name, Description=@Description, IsServer=@IsServer,
                                                                      CompanyCode=@CompanyCode, Price=@Price, Cost=@Cost
                                                                WHERE 
                                                                      CitrixPlanID=@CitrixPlanID";
                }

                // Add parameters
                cmd.Parameters.AddWithValue("CitrixPlanID", citrixApp.ID);
                cmd.Parameters.AddWithValue("Name", citrixApp.DisplayName);
                cmd.Parameters.AddWithValue("Description", citrixApp.Description);
                cmd.Parameters.AddWithValue("IsServer", citrixApp.IsServer);
                cmd.Parameters.AddWithValue("CompanyCode", citrixApp.CompanyCode);
                cmd.Parameters.AddWithValue("Price", citrixApp.Price);
                cmd.Parameters.AddWithValue("Cost", citrixApp.Cost);

                // Open connection
                sql.Open();

                // Update
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

        /// <summary>
        /// Deletes a citrix plan from the database
        /// </summary>
        /// <param name="citrixPlanID"></param>
        public static void DeleteCitrixPlan(int citrixPlanID)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"DELETE FROM Plans_Citrix WHERE CitrixPlanID=@CitrixPlanID;
                                              DELETE FROM UserPlansCitrix WHERE CitrixPlanID=@CitrixPlanID;", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("CitrixPlanID", citrixPlanID);

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
                if (cmd != null)
                    cmd.Dispose();

                if (sql != null)
                    sql.Dispose();
            }
        }

        /// <summary>
        /// Gets a list of users and then gets if they have access to the application or not
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="citrixPlanID"></param>
        /// <returns></returns>
        public static List<BaseUser> GetAssignedCitrixUsers(string companyCode, string citrixPlanID)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Users WHERE CompanyCode=@CompanyCode ORDER BY DisplayName;
                                              SELECT * FROM UserPlansCitrix WHERE CitrixPlanID=@CitrixPlanID AND (UserID IN (SELECT UserID FROM Users WHERE CompanyCode=@CompanyCode))", sql);

            // Create our collection
            List<BaseUser> users = new List<BaseUser>();

            try
            {
                // DEBUG //
                logger.Debug("Getting Citrix users for " + companyCode + " and Citrix plan id " + citrixPlanID);

                // Add our parameter
                cmd.Parameters.AddWithValue("CompanyCode", companyCode);
                cmd.Parameters.AddWithValue("CitrixPlanID", citrixPlanID);
                
                // Open connection
                sql.Open();

                // Read data and get all users
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        BaseUser tmp = new BaseUser();
                        tmp.ID = int.Parse(r["ID"].ToString());
                        tmp.UserGuid = Guid.Parse(r["UserGuid"].ToString());
                        tmp.UserPrincipalName = r["UserPrincipalName"].ToString();
                        tmp.DistinguishedName = r["DistinguishedName"].ToString();
                        tmp.DisplayName = r["DisplayName"].ToString();
                        tmp.CompanyCode = companyCode;

                        if (r["sAMAccountName"] != DBNull.Value)
                            tmp.sAMAccountName = r["sAMAccountName"].ToString();
                        else
                            tmp.sAMAccountName = "";

                        if (r["Firstname"] != DBNull.Value)
                            tmp.Firstname = r["Firstname"].ToString();
                        else
                            tmp.Firstname = "";

                        if (r["Middlename"] != DBNull.Value)
                            tmp.Middlename = r["MiddleName"].ToString();
                        else
                            tmp.Middlename = "";

                        if (r["Lastname"] != DBNull.Value)
                            tmp.Lastname = r["Lastname"].ToString();
                        else
                            tmp.Lastname = "";

                        if (r["Email"] != DBNull.Value)
                            tmp.Email = r["Email"].ToString();
                        else
                            tmp.Email = "";

                        if (r["Department"] != DBNull.Value)
                            tmp.Department = r["Department"].ToString();
                        else
                            tmp.Department = "";

                        if (r["IsResellerAdmin"] != DBNull.Value)
                            tmp.IsResellerAdmin = bool.Parse(r["IsResellerAdmin"].ToString());
                        else
                            tmp.IsResellerAdmin = false;

                        if (r["IsCompanyAdmin"] != DBNull.Value)
                            tmp.IsCompanyAdmin = bool.Parse(r["IsCompanyAdmin"].ToString());
                        else
                            tmp.IsCompanyAdmin = false;

                        if (r["MailboxPlan"] != DBNull.Value)
                            tmp.MailboxPlan = int.Parse(r["MailboxPlan"].ToString());
                        else
                            tmp.MailboxPlan = 0;

                        if (r["TSPlan"] != DBNull.Value)
                            tmp.TSPlan = int.Parse(r["TSPlan"].ToString());
                        else
                            tmp.TSPlan = 0;

                        if (r["LyncPlan"] != DBNull.Value)
                            tmp.LyncPlan = int.Parse(r["LyncPlan"].ToString());
                        else
                            tmp.LyncPlan = 0;

                        if (r["Created"] != DBNull.Value)
                            tmp.Created = DateTime.Parse(r["Created"].ToString());

                        // Add to our collection
                        users.Add(tmp);
                    }
                }

                // DEBUG //
                logger.Debug("Successfully retrieved all Citrix users for " + companyCode);

                // Go to our next result
                r.NextResult();
                
                // Now see who has what application
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        int userID = int.Parse(r["UserID"].ToString());
                        int citrixID = int.Parse(r["CitrixPlanID"].ToString());

                        // Find the value in our list
                        BaseUser found = users.Find(u => u.ID == userID);
                        if (found != null)
                            found.IsChecked = true;
                    }
                }

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return
                return users;
            }
            catch (Exception ex)
            {
                // FATAL //
                logger.Fatal("Error getting Citrix users for " + companyCode + " and Citrix plan id " + citrixPlanID, ex);

                throw;
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }
        }

        /// <summary>
        /// Adds the Citrix plan to the user
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="citrixPlanID"></param>
        public static void AddCitrixPlanToUser(int userID, int citrixPlanID)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"INSERT INTO UserPlansCitrix (UserID, CitrixPlanID) VALUES (@UserID, @CitrixPlanID)", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("CitrixPlanID", citrixPlanID);

                // Open connection
                sql.Open();

                // Insert
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

        /// <summary>
        /// Removes the Citrix plan from the user
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="citrixPlanID"></param>
        public static void DeleteCitrixPlanFromUser(int userID, int citrixPlanID)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"DELETE FROM UserPlansCitrix WHERE UserID=@UserID AND CitrixPlanID=@CitrixPlanID", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("CitrixPlanID", citrixPlanID);

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
                if (cmd != null)
                    cmd.Dispose();

                if (sql != null)
                    sql.Dispose();
            }
        }
    }
}
