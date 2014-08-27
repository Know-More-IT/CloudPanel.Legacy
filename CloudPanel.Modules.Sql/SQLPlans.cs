using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Base.Class;
using System.Data.SqlClient;
using System.Configuration;

namespace CloudPanel.Modules.Sql
{
    public class SQLPlans
    {
        public enum PlanType
        {
            Company,
            Mailbox,
            ActiveSync,
            PublicFolder,
            Citrix
        }

        #region Mailbox Plans

        /// <summary>
        /// Gets a list of mailbox plans
        /// </summary>
        /// <returns></returns>
        public static List<MailboxPlan> GetMailboxPlans()
        {
            // Initialize new collection
            List<MailboxPlan> plans = new List<MailboxPlan>();

            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM Plans_ExchangeMailbox ORDER BY CompanyCode DESC", sql);

            try
            {
                // Open Connection
                sql.Open();

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
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
                    tmp.OWAEnabled= bool.Parse(r["EnableOWA"].ToString());
                    tmp.MAPIEnabled = bool.Parse(r["EnableMAPI"].ToString());
                    tmp.ActiveSyncEnabled = bool.Parse(r["EnableAS"].ToString());
                    tmp.ECPEnabled = bool.Parse(r["EnableECP"].ToString());
                    tmp.KeepDeletedItemsInDays = int.Parse(r["MaxKeepDeletedItems"].ToString());

                    if (r["MaxMailboxSizeMB"] != DBNull.Value)
                        tmp.MaximumSizeInMB = int.Parse(r["MaxMailboxSizeMB"].ToString());
                    else
                        tmp.MaximumSizeInMB = tmp.SizeInMB;

                    if (r["Price"] != DBNull.Value)
                        tmp.Price = r["Price"].ToString();
                    else
                        tmp.Price = "0.00";

                    if (r["AdditionalGBPrice"] != DBNull.Value)
                        tmp.AdditionalGBPrice = r["AdditionalGBPrice"].ToString();
                    else
                        tmp.AdditionalGBPrice = "0.00";

                    if (r["MailboxPlanDesc"] != DBNull.Value)
                        tmp.Description = r["MailboxPlanDesc"].ToString();

                    if (r["CompanyCode"] != DBNull.Value)
                        tmp.CompanyCode = r["CompanyCode"].ToString();

                    // Add to our list
                    plans.Add(tmp);
                }

                // Close and dispose
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
            return plans;
        }

        /// <summary>
        /// Gets a specific mailbox plan from the database
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public static MailboxPlan GetMailboxPlan(int planId)
        {
            // Initialize new collection
            MailboxPlan plan = new MailboxPlan();

            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM Plans_ExchangeMailbox WHERE MailboxPlanID=@MailboxPlanID", sql);

            try
            {
                // Add parameter
                cmd.Parameters.AddWithValue("@MailboxPlanID", planId);

                // Open Connection
                sql.Open();

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
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
                    
                    if (r["Cost"] != DBNull.Value)
                        plan.Cost = r["Cost"].ToString();
                    else
                        plan.Cost = "0.00";

                    if (r["Price"] != DBNull.Value)
                        plan.Price = r["Price"].ToString();
                    else
                        plan.Price = "0.00";

                    if (r["AdditionalGBPrice"] != DBNull.Value)
                        plan.AdditionalGBPrice = r["AdditionalGBPrice"].ToString();
                    else
                        plan.AdditionalGBPrice = "0.00";

                    if (r["MaxMailboxSizeMB"] != DBNull.Value)
                        plan.MaximumSizeInMB = int.Parse(r["MaxMailboxSizeMB"].ToString());
                    else
                        plan.MaximumSizeInMB = plan.SizeInMB;

                    if (r["MailboxPlanDesc"] != DBNull.Value)
                        plan.Description = r["MailboxPlanDesc"].ToString();

                    if (r["CompanyCode"] != DBNull.Value)
                        plan.CompanyCode = r["CompanyCode"].ToString();
                }

                // Close and dispose
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

            // Return our object
            return plan;
        }

        /// <summary>
        /// Adds a new mailbox plan to the database
        /// </summary>
        /// <param name="plan"></param>
        public static void AddMailboxPlan(BasePlanMailbox plan)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"INSERT INTO Plans_ExchangeMailbox (MailboxPlanName, CompanyCode, MailboxSizeMB, MaxMailboxSizeMB,
                                                     MaxSendKB, MaxReceiveKB, MaxRecipients, EnablePOP3, EnableIMAP, EnableOWA, EnableMAPI, EnableAS,
                                                     EnableECP, MaxKeepDeletedItems, MailboxPlanDesc, Price, Cost, AdditionalGBPrice) VALUES 
                                                     (@MailboxPlanName, @CompanyCode, @MailboxSizeMB, @MaxMailboxSizeMB,
                                                     @MaxSendKB, @MaxReceiveKB, @MaxRecipients, @EnablePOP3, @EnableIMAP, @EnableOWA, @EnableMAPI, @EnableAS,
                                                     @EnableECP, @MaxKeepDeletedItems, @MailboxPlanDesc, @Price, @Cost, @AdditionalGBPrice)", sql);

            try
            {
                // Add parameter
                cmd.Parameters.AddWithValue("@MailboxPlanName", plan.MailboxPlanName);
                cmd.Parameters.AddWithValue("@CompanyCode", plan.CompanyCode);
                cmd.Parameters.AddWithValue("@MailboxSizeMB", plan.MailboxSize);
                cmd.Parameters.AddWithValue("@MaxMailboxSizeMB", plan.MaxMailboxSize);
                cmd.Parameters.AddWithValue("@MaxSendKB", plan.MaxSendKB);
                cmd.Parameters.AddWithValue("@MaxReceiveKB", plan.MaxReceiveKB);
                cmd.Parameters.AddWithValue("@MaxRecipients", plan.MaxRecipients);
                cmd.Parameters.AddWithValue("@EnablePOP3", plan.EnablePOP3);
                cmd.Parameters.AddWithValue("@EnableIMAP", plan.EnableIMAP);
                cmd.Parameters.AddWithValue("@EnableOWA", plan.EnableOWA);
                cmd.Parameters.AddWithValue("@EnableMAPI", plan.EnableMAPI);
                cmd.Parameters.AddWithValue("@EnableAS", plan.EnableAS);
                cmd.Parameters.AddWithValue("@EnableECP", plan.EnableECP);
                cmd.Parameters.AddWithValue("@MaxKeepDeletedItems", plan.MaxKeepDeletedItems);
                cmd.Parameters.AddWithValue("@MailboxPlanDesc", plan.MailboxPlanDesc);
                cmd.Parameters.AddWithValue("@Price", plan.Price);
                cmd.Parameters.AddWithValue("@Cost", plan.Cost);
                cmd.Parameters.AddWithValue("@AdditionalGBPrice", plan.AdditionalGBPrice);

                // Open Connection
                sql.Open();

                // Insert
                cmd.ExecuteNonQuery();

                // Close
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

        /// <summary>
        /// Updates an existing mailbox plan
        /// </summary>
        /// <param name="plan"></param>
        public static void UpdateMailboxPlan(BasePlanMailbox plan)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE Plans_ExchangeMailbox SET MailboxPlanName=@MailboxPlanName, MailboxSizeMB=@MailboxSizeMB, MaxMailboxSizeMB=@MaxMailboxSizeMB,
                                                    MaxSendKB=@MaxSendKB, MaxReceiveKB=@MaxReceiveKB, MaxRecipients=@MaxRecipients, EnablePOP3=@EnablePOP3, EnableIMAP=@EnableIMAP,
                                                    EnableOWA=@EnableOWA, EnableMAPI=@EnableMAPI, EnableAS=@EnableAS, EnableECP=@EnableECP, MaxKeepDeletedItems=@MaxKeepDeletedItems,
                                                    MailboxPlanDesc=@MailboxPlanDesc, Price=@Price, Cost=@Cost, AdditionalGBPrice=@AdditionalGBPrice WHERE MailboxPlanID=@MailboxPlanID", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@MailboxPlanID", plan.MailboxPlanID);
                cmd.Parameters.AddWithValue("@MailboxPlanName", plan.MailboxPlanName);
                cmd.Parameters.AddWithValue("@MailboxSizeMB", plan.MailboxSize);
                cmd.Parameters.AddWithValue("@MaxMailboxSizeMB", plan.MaxMailboxSize);
                cmd.Parameters.AddWithValue("@MaxSendKB", plan.MaxSendKB);
                cmd.Parameters.AddWithValue("@MaxReceiveKB", plan.MaxReceiveKB);
                cmd.Parameters.AddWithValue("@MaxRecipients", plan.MaxRecipients);
                cmd.Parameters.AddWithValue("@EnablePOP3", plan.EnablePOP3);
                cmd.Parameters.AddWithValue("@EnableIMAP", plan.EnableIMAP);
                cmd.Parameters.AddWithValue("@EnableOWA", plan.EnableOWA);
                cmd.Parameters.AddWithValue("@EnableMAPI", plan.EnableMAPI);
                cmd.Parameters.AddWithValue("@EnableAS", plan.EnableAS);
                cmd.Parameters.AddWithValue("@EnableECP", plan.EnableECP);
                cmd.Parameters.AddWithValue("@MaxKeepDeletedItems", plan.MaxKeepDeletedItems);
                cmd.Parameters.AddWithValue("@MailboxPlanDesc", plan.MailboxPlanDesc);
                cmd.Parameters.AddWithValue("@Price", plan.Price);
                cmd.Parameters.AddWithValue("@Cost", plan.Cost);
                cmd.Parameters.AddWithValue("@AdditionalGBPrice", plan.AdditionalGBPrice);

                // Open Connection
                sql.Open();

                // Update
                cmd.ExecuteNonQuery();

                // Close
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

        /// <summary>
        /// Deletes a mailbox plan from the database if no users are assigned to it
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public static int DeleteMailboxPlan(int planId)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"IF NOT EXISTS (SELECT 1 FROM Users WHERE MailboxPlan=@MailboxPlanID)
                                              BEGIN
                                                    DELETE FROM Plans_ExchangeMailbox WHERE MailboxPlanID=@MailboxPlanID
                                              END", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@MailboxPlanID", planId);

                // Open connection
                sql.Open();

                // Insert
                int rows = cmd.ExecuteNonQuery();

                // Close connection
                sql.Close();

                // Check if any rows were affected
                return rows;
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

        #endregion

        #region Activesync Plans

        public static int DeleteActiveSyncPlan(int planId)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"IF NOT EXISTS (SELECT * FROM Users WHERE ActiveSyncPlan=@ActiveSyncPlanID)
                                              BEGIN
                                                    DELETE FROM Plans_ExchangeActiveSync WHERE ASID=@ActiveSyncPlanID
                                              END", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@ActiveSyncPlanID", planId);

                // Open connection
                sql.Open();

                // Delete
                int rows = cmd.ExecuteNonQuery();

                // Close connection
                sql.Close();

                // Check if any rows were affected
                return rows;
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

        #endregion

        #region Company Plans
        /// <summary>
        /// Gets a list of company plans
        /// </summary>
        /// <returns></returns>
        public static List<BasePlanCompany> GetCompanyPlans()
        {
            // Initialize new collection
            List<BasePlanCompany> plans = new List<BasePlanCompany>();

            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM Plans_Organization ORDER BY OrgPlanName", sql);

            try
            {
                // Open Connection
                sql.Open();

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    BasePlanCompany pc = new BasePlanCompany();
                    pc.PlanID = int.Parse(r["OrgPlanID"].ToString());
                    pc.PlanName = r["OrgPlanName"].ToString();
                    pc.MaxUsers = int.Parse(r["MaxUsers"].ToString());
                    pc.MaxDomains = int.Parse(r["MaxDomains"].ToString());
                    pc.MaxExchangeMailboxes = int.Parse(r["MaxExchangeMailboxes"].ToString());
                    pc.MaxExchangeContacts = int.Parse(r["MaxExchangeContacts"].ToString());
                    pc.MaxExchangeDistLists = int.Parse(r["MaxExchangeDistLists"].ToString());
                    pc.MaxExchangePublicFolders = int.Parse(r["MaxExchangePublicFolders"].ToString());
                    pc.MaxExchangeMailPublicFolders = int.Parse(r["MaxExchangeMailPublicFolders"].ToString());
                    pc.MaxExchangeKeepDeletedItems = int.Parse(r["MaxExchangeKeepDeletedItems"].ToString());
                    pc.MaxCitrixUsers = int.Parse(r["MaxTerminalServerUsers"].ToString());

                    // Add to our list
                    plans.Add(pc);
                }

                // Close and dispose
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
            return plans;
        }

        /// <summary>
        /// Gets plan information for a specific company
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public static BasePlanCompany GetCompanyPlan(int planId)
        {
            // Initialize new collection
            BasePlanCompany pc = new BasePlanCompany();

            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM Plans_Organization WHERE OrgPlanID=@OrgPlanID", sql);
            cmd.Parameters.AddWithValue("@OrgPlanID", planId);

            try
            {
                // Open Connection
                sql.Open();

                // Read data
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    pc.PlanID = int.Parse(r["OrgPlanID"].ToString());
                    pc.PlanName = r["OrgPlanName"].ToString();
                    pc.MaxUsers = int.Parse(r["MaxUsers"].ToString());
                    pc.MaxDomains = int.Parse(r["MaxDomains"].ToString());
                    pc.MaxExchangeMailboxes = int.Parse(r["MaxExchangeMailboxes"].ToString());
                    pc.MaxExchangeContacts = int.Parse(r["MaxExchangeContacts"].ToString());
                    pc.MaxExchangeDistLists = int.Parse(r["MaxExchangeDistLists"].ToString());

                    if (r["MaxExchangeMailPublicFolders"] != DBNull.Value)
                        pc.MaxExchangeMailPublicFolders = int.Parse(r["MaxExchangeMailPublicFolders"].ToString());
                    
                    if (r["MaxExchangeResourceMailboxes"] != DBNull.Value)
                        pc.MaxExchangeResourceMailboxes = int.Parse(r["MaxExchangeResourceMailboxes"].ToString());

                    pc.MaxCitrixUsers = int.Parse(r["MaxTerminalServerUsers"].ToString());
                }

                // Close and dispose
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

            // Return our object
            return pc;
        }

        /// <summary>
        /// Adds a new company plan
        /// </summary>
        /// <param name="company"></param>
        public static void AddCompanyPlan(BasePlanCompany company)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"INSERT INTO Plans_Organization 
                                                     (OrgPlanName, ProductID, MaxUsers, MaxDomains, MaxExchangeMailboxes, MaxExchangeContacts,
                                                      MaxExchangeDistLists, MaxExchangePublicFolders, MaxExchangeMailPublicFolders, MaxExchangeKeepDeletedItems, 
                                                      MaxExchangeResourceMailboxes, MaxTerminalServerUsers) VALUES
                                                     (@OrgPlanName, @ProductID, @MaxUsers, @MaxDomains, @MaxExchangeMailboxes, @MaxExchangeContacts,
                                                      @MaxExchangeDistLists, @MaxExchangePublicFolders, @MaxExchangeMailPublicFolders, @MaxExchangeKeepDeletedItems, 
                                                      @MaxExchangeResourceMailboxes, @MaxTerminalServerUsers)
                                              ", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@OrgPlanName", company.PlanName);
                cmd.Parameters.AddWithValue("@ProductID", 0);
                cmd.Parameters.AddWithValue("@MaxUsers", company.MaxUsers);
                cmd.Parameters.AddWithValue("@MaxDomains", company.MaxDomains);
                cmd.Parameters.AddWithValue("@MaxExchangeMailboxes", company.MaxExchangeMailboxes);
                cmd.Parameters.AddWithValue("@MaxExchangeContacts", company.MaxExchangeContacts);
                cmd.Parameters.AddWithValue("@MaxExchangeDistLists", company.MaxExchangeDistLists);
                cmd.Parameters.AddWithValue("@MaxExchangePublicFolders", company.MaxExchangeMailPublicFolders);
                cmd.Parameters.AddWithValue("@MaxExchangeMailPublicFolders", company.MaxExchangeMailPublicFolders);
                cmd.Parameters.AddWithValue("@MaxExchangeKeepDeletedItems", 0);
                cmd.Parameters.AddWithValue("@MaxExchangeResourceMailboxes", company.MaxExchangeResourceMailboxes);
                cmd.Parameters.AddWithValue("@MaxTerminalServerUsers", company.MaxCitrixUsers);

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

        /// <summary>
        /// Updates an existing company plan
        /// </summary>
        /// <param name="company"></param>
        public static void UpdateCompanyPlan(BasePlanCompany company)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE Plans_Organization SET OrgPlanName=@OrgPlanName, MaxUsers=@MaxUsers, MaxDomains=@MaxDomains,
                                                     MaxExchangeMailboxes=@MaxExchangeMailboxes, MaxExchangeContacts=@MaxExchangeContacts, 
                                                     MaxExchangeDistLists=@MaxExchangeDistLists, MaxExchangePublicFolders=@MaxExchangePublicFolders,
                                                     MaxTerminalServerUsers=@MaxTerminalServerUsers, MaxExchangeMailPublicFolders=@MaxExchangeMailPublicFolders,
                                                     MaxExchangeResourceMailboxes=@MaxExchangeResourceMailboxes
                                              WHERE
                                                     OrgPlanID=@OrgPlanID", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@OrgPlanID", company.PlanID);
                cmd.Parameters.AddWithValue("@OrgPlanName", company.PlanName);
                cmd.Parameters.AddWithValue("@MaxUsers", company.MaxUsers);
                cmd.Parameters.AddWithValue("@MaxDomains", company.MaxDomains);
                cmd.Parameters.AddWithValue("@MaxExchangeMailboxes", company.MaxExchangeMailboxes);
                cmd.Parameters.AddWithValue("@MaxExchangeContacts", company.MaxExchangeContacts);
                cmd.Parameters.AddWithValue("@MaxExchangeDistLists", company.MaxExchangeDistLists);
                cmd.Parameters.AddWithValue("@MaxExchangePublicFolders", company.MaxExchangeMailPublicFolders);
                cmd.Parameters.AddWithValue("@MaxExchangeMailPublicFolders", company.MaxExchangeMailPublicFolders);
                cmd.Parameters.AddWithValue("@MaxExchangeResourceMailboxes", company.MaxExchangeResourceMailboxes);
                cmd.Parameters.AddWithValue("@MaxTerminalServerUsers", company.MaxCitrixUsers);

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

        /// <summary>
        /// Deletes a company plan from the database
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public static int DeleteCompanyPlan(int planId)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"IF NOT EXISTS (SELECT 1 FROM Companies WHERE OrgPlanID=@OrgPlanID)
                                              BEGIN
                                                    DELETE FROM Plans_Organization WHERE OrgPlanID=@OrgPlanID
                                              END", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@OrgPlanID", planId);

                // Open connection
                sql.Open();

                // Insert
                int rows = cmd.ExecuteNonQuery();

                // Close connection
                sql.Close();

                // Check if any rows were affected
                return rows;
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
        #endregion

        #region Common

        /// <summary>
        /// Checks to see if anyone is using a plan or not
        /// </summary>
        /// <param name="planID"></param>
        /// <param name="planType"></param>
        /// <returns></returns>
        public static List<string> IsPlanInUse(int planID, PlanType planType)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("", sql);

            // How string that we send back for who is using the plan
            // If we have no data this will be returned NULL
            List<string> whoIsUsingPlan = null;
            try
            {
                // Figure out what plan it is then generate the command
                switch (planType)
                {
                    case PlanType.Company:
                        cmd.CommandText = "SELECT CompanyName AS Name FROM Companies WHERE OrgPlanID=@PlanID AND IsReseller=0";
                        break;
                    case PlanType.Mailbox:
                        cmd.CommandText = "SELECT UserPrincipalName AS Name FROM Users WHERE MailboxPlan=@PlanID";
                        break;
                    case PlanType.ActiveSync:
                        cmd.CommandText = "SELECT UserPrincipalName AS Name FROM Users WHERE ActiveSyncPlan=@PlanID";
                        break;
                    case PlanType.Citrix:
                        cmd.CommandText = "SELECT UserPrincipalName AS Name FROM Users WHERE ID IN (SELECT UserID FROM UserPlansCitrix WHERE CitrixPlanID=@PlanID)";
                        break;
                    case PlanType.PublicFolder:
                        cmd.CommandText = "SELECT CompanyName AS Name FROM Companies WHERE ExchPFPlan=@PlanID";
                        break;
                    default:
                        throw new Exception("Invalid plan type was specified.");
                }

                // Add parameter
                cmd.Parameters.AddWithValue("PlanID", planID);

                // Open connection
                sql.Open();

                // Read Data
                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    whoIsUsingPlan = new List<string>();

                    while (r.Read())
                    {
                        whoIsUsingPlan.Add(r["Name"].ToString());
                    }
                }

                // Close
                r.Close();
                sql.Close();

                // Return data
                return whoIsUsingPlan;
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

        #endregion
    }
}
