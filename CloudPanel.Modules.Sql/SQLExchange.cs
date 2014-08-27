using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using CloudPanel.Modules.Base;
using System.Data;
using CloudPanel.Modules.Base.Class;
using log4net;

namespace CloudPanel.Modules.Sql
{
    public class SQLExchange
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SQLExchange)); 

        /// <summary>
        /// Checks to see if a company is enabled or not
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static bool IsExchangeEnabled(string companyCode)
        {
            if (!string.IsNullOrEmpty(companyCode))
            {
                // Initialize SQL
                SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                SqlCommand cmd = new SqlCommand("SELECT ExchEnabled FROM Companies WHERE CompanyCode=@CompanyCode AND IsReseller=0", sql);

                try
                {
                    // Add parameters
                    cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                    // Open Connection
                    sql.Open();

                    object enabled = cmd.ExecuteScalar();

                    if (enabled == null || enabled == DBNull.Value)
                        return false;
                    else
                    {
                        return bool.Parse(enabled.ToString());
                    }
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
            else
                return false;
        }

        /// <summary>
        /// Disables Exchange for a company.
        /// This removes all Exchange related items from the database
        /// </summary>
        /// <param name="companyCode"></param>
        public static void DisableExchange(string companyCode)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"DisableExchange", sql);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                // Add parameter
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                // Open Connection
                sql.Open();

                // Update Database
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
        /// Gets a list of Exchange contacts from SQL
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<BaseContacts> GetContacts(string companyCode)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM Contacts WHERE CompanyCode=@CompanyCode ORDER BY DisplayName", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                // Open Connection
                sql.Open();

                List<BaseContacts> contacts = new List<BaseContacts>();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    contacts.Add(new BaseContacts()
                    {
                        DisplayName = r["DisplayName"].ToString(),
                        Email = r["Email"].ToString(),
                        Hidden = bool.Parse(r["Hidden"].ToString()),
                        DistinguishedName = r["DistinguishedName"].ToString()
                    });
                }

                r.Close();
                r.Dispose();

                // Return collection
                return contacts;
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
        /// Adds a new contact to Exchange
        /// </summary>
        /// <param name="distinguishedName"></param>
        /// <param name="companyCode"></param>
        /// <param name="displayName"></param>
        /// <param name="email"></param>
        /// <param name="hidden"></param>
        public static void AddContact(string distinguishedName, string companyCode, string displayName, string email, bool hidden)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"INSERT INTO Contacts 
                                                (DistinguishedName, CompanyCode, DisplayName, Email, Hidden) 
                                              VALUES 
                                                (@DistinguishedName, @CompanyCode, @DisplayName, @Email, @Hidden) ", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@DistinguishedName", distinguishedName);
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
                cmd.Parameters.AddWithValue("@DisplayName", displayName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Hidden", hidden);

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
        /// Updates a contact
        /// </summary>
        /// <param name="contact"></param>
        public static void UpdateContact(MailContact contact)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE Contacts SET Hidden=@Hidden WHERE DistinguishedName=@DistinguishedName", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@DistinguishedName", contact.DistinguishedName);
                cmd.Parameters.AddWithValue("@Hidden", contact.Hidden);

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
        /// Removes a contact from Exchange
        /// </summary>
        /// <param name="distinguishedName"></param>
        public static void RemoveContact(string distinguishedName)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"DELETE FROM Contacts WHERE DistinguishedName=@DistinguishedName", sql);

            try
            {
                // Add parameter
                cmd.Parameters.AddWithValue("@DistinguishedName", distinguishedName);

                // Open Connection
                sql.Open();

                // Delete
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
        /// Gets a list of ActiveSync policies from SQL
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<BaseActivesyncPolicy> GetExchangeActiveSyncPolicies()
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Plans_ExchangeActiveSync ORDER BY DisplayName", sql);

            // Initialize collection
            List<BaseActivesyncPolicy> policies = new List<BaseActivesyncPolicy>();
            string blah = "";
            try
            {
                // Open Connection
                sql.Open();


                // Read
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    BaseActivesyncPolicy tmp = new BaseActivesyncPolicy();
                    tmp.ASID = int.Parse(r["ASID"].ToString());
                    tmp.DisplayName = r["DisplayName"].ToString();

                    if (r["CompanyCode"] != DBNull.Value)
                        tmp.CompanyCode = r["CompanyCode"].ToString();
                    else
                        tmp.CompanyCode = string.Empty;
                   
                    if (r["Description"] != DBNull.Value)
                        tmp.Description = r["Description"].ToString();
                    else
                        tmp.Description = string.Empty;
                    
                    if (r["AllowNonProvisionableDevices"] != DBNull.Value)
                        tmp.AllowNonProvisionableDevice = bool.Parse(r["AllowNonProvisionableDevices"].ToString());
                    else
                        tmp.AllowNonProvisionableDevice = true;
                    
                    if (r["RefreshIntervalInHours"] != DBNull.Value)
                        tmp.DevicePolicyRefreshInterval = int.Parse(r["RefreshIntervalInHours"].ToString());
                    else
                        tmp.DevicePolicyRefreshInterval = 0;
                    
                    if (r["RequirePassword"] != DBNull.Value)
                        tmp.DevicePasswordEnabled = bool.Parse(r["RequirePassword"].ToString());
                    else
                        tmp.DevicePasswordEnabled = false;
                   
                    if (r["RequireAlphanumericPassword"] != DBNull.Value)
                        tmp.AlphanumericDevicePasswordRequired = bool.Parse(r["RequireAlphanumericPassword"].ToString());
                    else
                        tmp.AlphanumericDevicePasswordRequired = false;
                   
                    if (r["EnablePasswordRecovery"] != DBNull.Value)
                        tmp.PasswordRecoveryEnabled = bool.Parse(r["EnablePasswordRecovery"].ToString());
                    else
                        tmp.PasswordRecoveryEnabled = false;
                  
                    if (r["RequireEncryptionOnDevice"] != DBNull.Value)
                        tmp.RequireDeviceEncryption = bool.Parse(r["RequireEncryptionOnDevice"].ToString());
                    else
                        tmp.RequireDeviceEncryption = false;
                    
                    if (r["RequireEncryptionOnStorageCard"] != DBNull.Value)
                        tmp.DeviceEncryptionEnabled = bool.Parse(r["RequireEncryptionOnStorageCard"].ToString());
                    else
                        tmp.DeviceEncryptionEnabled = false;
                    
                    if (r["AllowSimplePassword"] != DBNull.Value)
                        tmp.AllowSimpleDevicePassword = bool.Parse(r["AllowSimplePassword"].ToString());
                    else
                        tmp.AllowSimpleDevicePassword = true;

                    if (r["NumberOfFailedAttempted"] != DBNull.Value)
                        tmp.MaxDevicePasswordFailedAttempts = int.Parse(r["NumberOfFailedAttempted"].ToString());
                    else
                        tmp.MaxDevicePasswordFailedAttempts = 0;
                   
                    if (r["MinimumPasswordLength"] != DBNull.Value)
                        tmp.MinDevicePasswordLength = int.Parse(r["MinimumPasswordLength"].ToString());
                    else
                        tmp.MinDevicePasswordLength = 0;

                    if (r["InactivityTimeoutInMinutes"] != DBNull.Value)
                        tmp.MaxInactivityTimeDeviceLock = int.Parse(r["InactivityTimeoutInMinutes"].ToString());
                    else
                        tmp.MaxInactivityTimeDeviceLock = 0;

                    if (r["PasswordExpirationInDays"] != DBNull.Value)
                        tmp.DevicePasswordExpiration = int.Parse(r["PasswordExpirationInDays"].ToString());
                    else
                        tmp.DevicePasswordExpiration = 0;

                    if (r["EnforcePasswordHistory"] != DBNull.Value)
                        tmp.DevicePasswordHistory = int.Parse(r["EnforcePasswordHistory"].ToString());
                    else
                        tmp.DevicePasswordHistory = 0;
                   
                    if (r["IncludePastCalendarItems"] != DBNull.Value)
                        tmp.MaxCalendarAgeFilter = r["IncludePastCalendarItems"].ToString();
                    else
                        tmp.MaxCalendarAgeFilter = "All";

                    if (r["IncludePastEmailItems"] != DBNull.Value)
                        tmp.MaxEmailAgeFilter = r["IncludePastEmailItems"].ToString();
                    else
                        tmp.MaxEmailAgeFilter = "All";

                    if (r["LimitEmailSizeInKB"] != DBNull.Value)
                        tmp.MaxEmailBodyTruncationSize = int.Parse(r["LimitEmailSizeInKB"].ToString());
                    else
                        tmp.MaxEmailBodyTruncationSize = 0;

                    if (r["AllowDirectPushWhenRoaming"] != DBNull.Value)
                        tmp.RequireManualSyncWhenRoaming = bool.Parse(r["AllowDirectPushWhenRoaming"].ToString());
                    else
                        tmp.RequireManualSyncWhenRoaming = false;
                    
                    if (r["AllowHTMLEmail"] != DBNull.Value)
                        tmp.AllowHTMLEmail = bool.Parse(r["AllowHTMLEmail"].ToString());
                    else
                        tmp.AllowHTMLEmail = true;
                    
                    if (r["AllowAttachmentsDownload"] != DBNull.Value)
                        tmp.AttachmentsEnabled = bool.Parse(r["AllowAttachmentsDownload"].ToString());
                    else
                        tmp.AttachmentsEnabled = true;
                   
                    if (r["MaximumAttachmentSizeInKB"] != DBNull.Value)
                        tmp.MaxAttachmentSize = int.Parse(r["MaximumAttachmentSizeInKB"].ToString());
                    else
                        tmp.MaxAttachmentSize = 0;
                    
                    if (r["AllowRemovableStorage"] != DBNull.Value)
                        tmp.AllowStorageCard = bool.Parse(r["AllowRemovableStorage"].ToString());
                    else
                        tmp.AllowStorageCard = true;
                   
                    if (r["AllowCamera"] != DBNull.Value)
                        tmp.AllowCamera = bool.Parse(r["AllowCamera"].ToString());
                    else
                        tmp.AllowCamera = true;

                    if (r["AllowWiFi"] != DBNull.Value)
                        tmp.AllowWiFi = bool.Parse(r["AllowWiFi"].ToString());
                    else
                        tmp.AllowWiFi = true;

                    if (r["AllowInfrared"] != DBNull.Value)
                        tmp.AllowIrDA = bool.Parse(r["AllowInfrared"].ToString());
                    else
                        tmp.AllowIrDA = true;

                    if (r["AllowInternetSharing"] != DBNull.Value)
                        tmp.AllowInternetSharing = bool.Parse(r["AllowInternetSharing"].ToString());
                    else
                        tmp.AllowInternetSharing = true;

                    if (r["AllowRemoteDesktop"] != DBNull.Value)
                        tmp.AllowRemoteDesktop = bool.Parse(r["AllowRemoteDesktop"].ToString());
                    else
                        tmp.AllowRemoteDesktop = true;

                    if (r["AllowDesktopSync"] != DBNull.Value)
                        tmp.AllowDesktopSync = bool.Parse(r["AllowDesktopSync"].ToString());
                    else
                        tmp.AllowDesktopSync = true;
                    
                    if (r["AllowBluetooth"] != DBNull.Value)
                        tmp.AllowBluetooth = r["AllowBluetooth"].ToString();
                    else
                        tmp.AllowBluetooth = "Allow";

                    if (r["AllowBrowser"] != DBNull.Value)
                        tmp.AllowBrowser = bool.Parse(r["AllowBrowser"].ToString());
                    else
                        tmp.AllowBrowser = true;

                    if (r["AllowConsumerMail"] != DBNull.Value)
                        tmp.AllowConsumerEmail = bool.Parse(r["AllowConsumerMail"].ToString());
                    else
                        tmp.AllowConsumerEmail = true;

                    if (r["AllowUnsignedApplications"] != DBNull.Value)
                        tmp.AllowUnsignedApplications = bool.Parse(r["AllowUnsignedApplications"].ToString());
                    else
                        tmp.AllowUnsignedApplications = true;

                    if (r["AllowUnsignedInstallationPackages"] != DBNull.Value)
                        tmp.AllowUnsignedInstallationPackages = bool.Parse(r["AllowUnsignedInstallationPackages"].ToString());
                    else
                        tmp.AllowUnsignedInstallationPackages = true;
                    
                    if (r["IsEnterpriseCAL"] != DBNull.Value)
                        tmp.IsEnterpriseCAL = bool.Parse(r["IsEnterpriseCAL"].ToString());
                    else
                        tmp.IsEnterpriseCAL = false;

                    if (r["AllowTextMessaging"] != DBNull.Value)
                        tmp.AllowTextMessaging = bool.Parse(r["AllowTextMessaging"].ToString());
                    else
                        tmp.AllowTextMessaging = true;

                    // Add to our collection
                    policies.Add(tmp);
                }

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return
                return policies;
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
        /// Gets a specific policy from SQL
        /// </summary>
        /// <param name="policyID"></param>
        /// <returns></returns>
        public static BaseActivesyncPolicy GetExchangeActiveSyncPolicy(int policyID)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT * FROM Plans_ExchangeActiveSync WHERE ASID=@ASID", sql);

            // Initialize object
            BaseActivesyncPolicy tmp = new BaseActivesyncPolicy();

            try
            {
                // Add parameter
                cmd.Parameters.AddWithValue("ASID", policyID);

                // Open Connection
                sql.Open();

                // Read
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    tmp.ASID = int.Parse(r["ASID"].ToString());
                    tmp.DisplayName = r["DisplayName"].ToString();

                    if (r["CompanyCode"] != DBNull.Value)
                        tmp.CompanyCode = r["CompanyCode"].ToString();
                    else
                        tmp.CompanyCode = string.Empty;

                    if (r["Description"] != DBNull.Value)
                        tmp.Description = r["Description"].ToString();
                    else
                        tmp.Description = string.Empty;

                    if (r["AllowNonProvisionableDevices"] != DBNull.Value)
                        tmp.AllowNonProvisionableDevice = bool.Parse(r["AllowNonProvisionableDevices"].ToString());
                    else
                        tmp.AllowNonProvisionableDevice = true;

                    if (r["RefreshIntervalInHours"] != DBNull.Value)
                        tmp.DevicePolicyRefreshInterval = int.Parse(r["RefreshIntervalInHours"].ToString());
                    else
                        tmp.DevicePolicyRefreshInterval = 0;

                    if (r["RequirePassword"] != DBNull.Value)
                        tmp.DevicePasswordEnabled = bool.Parse(r["RequirePassword"].ToString());
                    else
                        tmp.DevicePasswordEnabled = false;

                    if (r["RequireAlphaNumericPassword"] != DBNull.Value)
                        tmp.AlphanumericDevicePasswordRequired = bool.Parse(r["RequireAlphaNumericPassword"].ToString());
                    else
                        tmp.AlphanumericDevicePasswordRequired = false;

                    if (r["EnablePasswordRecovery"] != DBNull.Value)
                        tmp.PasswordRecoveryEnabled = bool.Parse(r["EnablePasswordRecovery"].ToString());
                    else
                        tmp.PasswordRecoveryEnabled = false;

                    if (r["RequireEncryptionOnDevice"] != DBNull.Value)
                        tmp.RequireDeviceEncryption = bool.Parse(r["RequireEncryptionOnDevice"].ToString());
                    else
                        tmp.RequireDeviceEncryption = false;

                    if (r["RequireEncryptionOnStorageCard"] != DBNull.Value)
                        tmp.DeviceEncryptionEnabled = bool.Parse(r["RequireEncryptionOnStorageCard"].ToString());
                    else
                        tmp.DeviceEncryptionEnabled = false;

                    if (r["AllowSimplePassword"] != DBNull.Value)
                        tmp.AllowSimpleDevicePassword = bool.Parse(r["AllowSimplePassword"].ToString());
                    else
                        tmp.AllowSimpleDevicePassword = true;

                    if (r["NumberOfFailedAttempted"] != DBNull.Value)
                        tmp.MaxDevicePasswordFailedAttempts = int.Parse(r["NumberOfFailedAttempted"].ToString());
                    else
                        tmp.MaxDevicePasswordFailedAttempts = 0;

                    if (r["MinimumPasswordLength"] != DBNull.Value)
                        tmp.MinDevicePasswordLength = int.Parse(r["MinimumPasswordLength"].ToString());
                    else
                        tmp.MinDevicePasswordLength = 0;

                    if (r["InactivityTimeoutInMinutes"] != DBNull.Value)
                        tmp.MaxInactivityTimeDeviceLock = int.Parse(r["InactivityTimeoutInMinutes"].ToString());
                    else
                        tmp.MaxInactivityTimeDeviceLock = 0;

                    if (r["PasswordExpirationInDays"] != DBNull.Value)
                        tmp.DevicePasswordExpiration = int.Parse(r["PasswordExpirationInDays"].ToString());
                    else
                        tmp.DevicePasswordExpiration = 0;

                    if (r["EnforcePasswordHistory"] != DBNull.Value)
                        tmp.DevicePasswordHistory = int.Parse(r["EnforcePasswordHistory"].ToString());
                    else
                        tmp.DevicePasswordHistory = 0;

                    if (r["IncludePastCalendarItems"] != DBNull.Value)
                        tmp.MaxCalendarAgeFilter = r["IncludePastCalendarItems"].ToString();
                    else
                        tmp.MaxCalendarAgeFilter = "All";

                    if (r["IncludePastEmailItems"] != DBNull.Value)
                        tmp.MaxEmailAgeFilter = r["IncludePastEmailItems"].ToString();
                    else
                        tmp.MaxEmailAgeFilter = "All";

                    if (r["LimitEmailSizeInKB"] != DBNull.Value)
                        tmp.MaxEmailBodyTruncationSize = int.Parse(r["LimitEmailSizeInKB"].ToString());
                    else
                        tmp.MaxEmailBodyTruncationSize = 0;

                    if (r["AllowDirectPushWhenRoaming"] != DBNull.Value)
                        tmp.RequireManualSyncWhenRoaming = bool.Parse(r["AllowDirectPushWhenRoaming"].ToString());
                    else
                        tmp.RequireManualSyncWhenRoaming = false;

                    if (r["AllowHTMLEmail"] != DBNull.Value)
                        tmp.AllowHTMLEmail = bool.Parse(r["AllowHTMLEmail"].ToString());
                    else
                        tmp.AllowHTMLEmail = true;

                    if (r["AllowAttachmentsDownload"] != DBNull.Value)
                        tmp.AttachmentsEnabled = bool.Parse(r["AllowAttachmentsDownload"].ToString());
                    else
                        tmp.AttachmentsEnabled = true;

                    if (r["MaximumAttachmentSizeInKB"] != DBNull.Value)
                        tmp.MaxAttachmentSize = int.Parse(r["MaximumAttachmentSizeInKB"].ToString());
                    else
                        tmp.MaxAttachmentSize = 0;

                    if (r["AllowRemovableStorage"] != DBNull.Value)
                        tmp.AllowStorageCard = bool.Parse(r["AllowRemovableStorage"].ToString());
                    else
                        tmp.AllowStorageCard = true;

                    if (r["AllowCamera"] != DBNull.Value)
                        tmp.AllowCamera = bool.Parse(r["AllowCamera"].ToString());
                    else
                        tmp.AllowCamera = true;

                    if (r["AllowWiFi"] != DBNull.Value)
                        tmp.AllowWiFi = bool.Parse(r["AllowWiFi"].ToString());
                    else
                        tmp.AllowWiFi = true;

                    if (r["AllowInfrared"] != DBNull.Value)
                        tmp.AllowIrDA = bool.Parse(r["AllowInfrared"].ToString());
                    else
                        tmp.AllowIrDA = true;

                    if (r["AllowInternetSharing"] != DBNull.Value)
                        tmp.AllowInternetSharing = bool.Parse(r["AllowInternetSharing"].ToString());
                    else
                        tmp.AllowInternetSharing = true;

                    if (r["AllowRemoteDesktop"] != DBNull.Value)
                        tmp.AllowRemoteDesktop = bool.Parse(r["AllowRemoteDesktop"].ToString());
                    else
                        tmp.AllowRemoteDesktop = true;

                    if (r["AllowDesktopSync"] != DBNull.Value)
                        tmp.AllowDesktopSync = bool.Parse(r["AllowDesktopSync"].ToString());
                    else
                        tmp.AllowDesktopSync = true;

                    if (r["AllowBluetooth"] != DBNull.Value)
                        tmp.AllowBluetooth = r["AllowBluetooth"].ToString();
                    else
                        tmp.AllowBluetooth = "Allow";

                    if (r["AllowBrowser"] != DBNull.Value)
                        tmp.AllowBrowser = bool.Parse(r["AllowBrowser"].ToString());
                    else
                        tmp.AllowBrowser = true;

                    if (r["AllowConsumerMail"] != DBNull.Value)
                        tmp.AllowConsumerEmail = bool.Parse(r["AllowConsumerMail"].ToString());
                    else
                        tmp.AllowConsumerEmail = true;

                    if (r["AllowUnsignedApplications"] != DBNull.Value)
                        tmp.AllowUnsignedApplications = bool.Parse(r["AllowUnsignedApplications"].ToString());
                    else
                        tmp.AllowUnsignedApplications = true;

                    if (r["AllowUnsignedInstallationPackages"] != DBNull.Value)
                        tmp.AllowUnsignedInstallationPackages = bool.Parse(r["AllowUnsignedInstallationPackages"].ToString());
                    else
                        tmp.AllowUnsignedInstallationPackages = true;

                    if (r["IsEnterpriseCAL"] != DBNull.Value)
                        tmp.IsEnterpriseCAL = bool.Parse(r["IsEnterpriseCAL"].ToString());
                    else
                        tmp.IsEnterpriseCAL = false;

                    if (r["AllowTextMessaging"] != DBNull.Value)
                        tmp.AllowTextMessaging = bool.Parse(r["AllowTextMessaging"].ToString());
                    else
                        tmp.AllowTextMessaging = true;
                }

                // Close
                r.Close();
                sql.Close();

                // Dispose
                r.Dispose();

                // Return
                return tmp;
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
        /// Adds a new Exchange ActiveSync policy
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="displayName"></param>
        /// <param name="description"></param>
        public static void AddExchangeActiveSyncPolicy(BaseActivesyncPolicy policy)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"INSERT INTO Plans_ExchangeActiveSync (
                       CompanyCode, DisplayName, Description, AllowNonProvisionableDevices, RefreshIntervalInHours, RequirePassword,
                       RequireAlphanumericPassword, EnablePasswordRecovery, RequireEncryptionOnDevice, RequireEncryptionOnStorageCard,
                       AllowSimplePassword, NumberOfFailedAttempted, MinimumPasswordLength, InactivityTimeoutInMinutes, PasswordExpirationInDays,
                       EnforcePasswordHistory, IncludePastCalendarItems, IncludePastEmailItems, LimitEmailSizeInKB, AllowDirectPushWhenRoaming,
                       AllowHTMLEmail, AllowAttachmentsDownload, MaximumAttachmentSizeInKB, AllowRemovableStorage, AllowCamera, AllowWiFi, AllowInfrared,
                       AllowInternetSharing, AllowRemoteDesktop, AllowDesktopSync, AllowBluetooth, AllowBrowser, AllowConsumerMail, AllowUnsignedApplications,
                       AllowUnsignedInstallationPackages, IsEnterpriseCAL, AllowTextMessaging) VALUES (
                       @CompanyCode, @DisplayName, @Description, @AllowNonProvisionableDevices, @RefreshIntervalInHours, @RequirePassword,
                       @RequireAlphanumericPassword, @EnablePasswordRecovery, @RequireEncryptionOnDevice, @RequireEncryptionOnStorageCard,
                       @AllowSimplePassword, @NumberOfFailedAttempted, @MinimumPasswordLength, @InactivityTimeoutInMinutes, @PasswordExpirationInDays,
                       @EnforcePasswordHistory, @IncludePastCalendarItems, @IncludePastEmailItems, @LimitEmailSizeInKB, @AllowDirectPushWhenRoaming,
                       @AllowHTMLEmail, @AllowAttachmentsDownload, @MaximumAttachmentSizeInKB, @AllowRemovableStorage, @AllowCamera, @AllowWiFi, @AllowInfrared,
                       @AllowInternetSharing, @AllowRemoteDesktop, @AllowDesktopSync, @AllowBluetooth, @AllowBrowser, @AllowConsumerMail, @AllowUnsignedApplications,
                       @AllowUnsignedInstallationPackages, @IsEnterpriseCAL, @AllowTextMessaging)", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("CompanyCode", policy.CompanyCode);
                cmd.Parameters.AddWithValue("DisplayName", policy.DisplayName);
                cmd.Parameters.AddWithValue("Description", policy.Description);
                cmd.Parameters.AddWithValue("AllowNonProvisionableDevices", policy.AllowNonProvisionableDevice);
                cmd.Parameters.AddWithValue("RefreshIntervalInHours", policy.DevicePolicyRefreshInterval);
                cmd.Parameters.AddWithValue("RequirePassword", policy.DevicePasswordEnabled);
                cmd.Parameters.AddWithValue("RequireAlphanumericPassword", policy.AlphanumericDevicePasswordRequired);
                cmd.Parameters.AddWithValue("EnablePasswordRecovery", policy.PasswordRecoveryEnabled);
                cmd.Parameters.AddWithValue("RequireEncryptionOnDevice", policy.RequireDeviceEncryption);
                cmd.Parameters.AddWithValue("RequireEncryptionOnStorageCard", policy.RequireStorageCardEncryption);
                cmd.Parameters.AddWithValue("AllowSimplePassword", policy.AllowSimpleDevicePassword);
                cmd.Parameters.AddWithValue("NumberOfFailedAttempted", policy.MaxDevicePasswordFailedAttempts);
                cmd.Parameters.AddWithValue("MinimumPasswordLength", policy.MinDevicePasswordLength);
                cmd.Parameters.AddWithValue("InactivityTimeoutInMinutes", policy.MaxInactivityTimeDeviceLock);
                cmd.Parameters.AddWithValue("PasswordExpirationInDays", policy.DevicePasswordExpiration);
                cmd.Parameters.AddWithValue("EnforcePasswordHistory", policy.DevicePasswordHistory);
                cmd.Parameters.AddWithValue("IncludePastCalendarItems", policy.MaxCalendarAgeFilter);
                cmd.Parameters.AddWithValue("IncludePastEmailItems", policy.MaxEmailAgeFilter);
                cmd.Parameters.AddWithValue("LimitEmailSizeInKB", policy.MaxEmailBodyTruncationSize);
                cmd.Parameters.AddWithValue("AllowDirectPushWhenRoaming", policy.RequireManualSyncWhenRoaming);
                cmd.Parameters.AddWithValue("AllowHTMLEmail", policy.AllowHTMLEmail);
                cmd.Parameters.AddWithValue("AllowAttachmentsDownload", policy.AttachmentsEnabled);
                cmd.Parameters.AddWithValue("MaximumAttachmentSizeInKB", policy.MaxAttachmentSize);
                cmd.Parameters.AddWithValue("AllowRemovableStorage", policy.AllowStorageCard);
                cmd.Parameters.AddWithValue("AllowCamera", policy.AllowCamera);
                cmd.Parameters.AddWithValue("AllowWiFi", policy.AllowWiFi);
                cmd.Parameters.AddWithValue("AllowInfrared", policy.AllowIrDA);
                cmd.Parameters.AddWithValue("AllowInternetSharing", policy.AllowInternetSharing);
                cmd.Parameters.AddWithValue("AllowRemoteDesktop", policy.AllowRemoteDesktop);
                cmd.Parameters.AddWithValue("AllowDesktopSync", policy.AllowDesktopSync);
                cmd.Parameters.AddWithValue("AllowBluetooth", policy.AllowBluetooth);
                cmd.Parameters.AddWithValue("AllowBrowser", policy.AllowBrowser);
                cmd.Parameters.AddWithValue("AllowConsumerMail", policy.AllowConsumerEmail);
                cmd.Parameters.AddWithValue("AllowUnsignedApplications", policy.AllowUnsignedApplications);
                cmd.Parameters.AddWithValue("AllowUnsignedInstallationPackages", policy.AllowUnsignedInstallationPackages);
                cmd.Parameters.AddWithValue("IsEnterpriseCAL", policy.IsEnterpriseCAL);
                cmd.Parameters.AddWithValue("AllowTextMessaging", policy.AllowTextMessaging);


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
        /// Updates an existing plan in the database
        /// </summary>
        /// <param name="policy"></param>
        public static void UpdateExchangeActiveSyncPolicy(BaseActivesyncPolicy policy)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE Plans_ExchangeActiveSync SET
                       CompanyCode=@CompanyCode, DisplayName=@DisplayName, Description=@Description, AllowNonProvisionableDevices=@AllowNonProvisionableDevices,
                       RefreshIntervalInHours=@RefreshIntervalInHours, RequirePassword=@RequirePassword, RequireAlphanumericPassword=@RequireAlphanumericPassword,
                       EnablePasswordRecovery=@EnablePasswordRecovery, RequireEncryptionOnDevice=@RequireEncryptionOnDevice, RequireEncryptionOnStorageCard=@RequireEncryptionOnStorageCard,
                       AllowSimplePassword=@AllowSimplePassword, NumberOfFailedAttempted=@NumberOfFailedAttempted, MinimumPasswordLength=@MinimumPasswordLength,
                       InactivityTimeoutInMinutes=@InactivityTimeoutInMinutes, PasswordExpirationInDays=@PasswordExpirationInDays, EnforcePasswordHistory=@EnforcePasswordHistory,
                       IncludePastCalendarItems=@IncludePastCalendarItems, IncludePastEmailItems=@IncludePastEmailItems, LimitEmailSizeInKB=@LimitEmailSizeInKB, AllowDirectPushWhenRoaming=@AllowDirectPushWhenRoaming,
                       AllowHTMLEmail=@AllowHTMLEmail, AllowAttachmentsDownload=@AllowAttachmentsDownload, MaximumAttachmentSizeInKB=@MaximumAttachmentSizeInKB,
                       AllowRemovableStorage=@AllowRemovableStorage, AllowCamera=@AllowCamera, AllowWiFi=@AllowWiFi, AllowInfrared=@AllowInfrared, AllowInternetSharing=@AllowInternetSharing,
                       AllowRemoteDesktop=@AllowRemoteDesktop, AllowDesktopSync=@AllowDesktopSync, AllowBluetooth=@AllowBluetooth, AllowBrowser=@AllowBrowser,
                       AllowConsumerMail=@AllowConsumerMail, AllowUnsignedApplications=@AllowUnsignedApplications, AllowUnsignedInstallationPackages=@AllowUnsignedInstallationPackages,
                       IsEnterpriseCAL=@IsEnterpriseCAL, AllowTextMessaging=@AllowTextMessaging WHERE ASID=@ASID", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("ASID", policy.ASID);
                cmd.Parameters.AddWithValue("CompanyCode", policy.CompanyCode);
                cmd.Parameters.AddWithValue("DisplayName", policy.DisplayName);
                cmd.Parameters.AddWithValue("Description", policy.Description);
                cmd.Parameters.AddWithValue("AllowNonProvisionableDevices", policy.AllowNonProvisionableDevice);
                cmd.Parameters.AddWithValue("RefreshIntervalInHours", policy.DevicePolicyRefreshInterval);
                cmd.Parameters.AddWithValue("RequirePassword", policy.DevicePasswordEnabled);
                cmd.Parameters.AddWithValue("RequireAlphanumericPassword", policy.AlphanumericDevicePasswordRequired);
                cmd.Parameters.AddWithValue("EnablePasswordRecovery", policy.PasswordRecoveryEnabled);
                cmd.Parameters.AddWithValue("RequireEncryptionOnDevice", policy.RequireDeviceEncryption);
                cmd.Parameters.AddWithValue("RequireEncryptionOnStorageCard", policy.RequireStorageCardEncryption);
                cmd.Parameters.AddWithValue("AllowSimplePassword", policy.AllowSimpleDevicePassword);
                cmd.Parameters.AddWithValue("NumberOfFailedAttempted", policy.MaxDevicePasswordFailedAttempts);
                cmd.Parameters.AddWithValue("MinimumPasswordLength", policy.MinDevicePasswordLength);
                cmd.Parameters.AddWithValue("InactivityTimeoutInMinutes", policy.MaxInactivityTimeDeviceLock);
                cmd.Parameters.AddWithValue("PasswordExpirationInDays", policy.DevicePasswordExpiration);
                cmd.Parameters.AddWithValue("EnforcePasswordHistory", policy.DevicePasswordHistory);
                cmd.Parameters.AddWithValue("IncludePastCalendarItems", policy.MaxCalendarAgeFilter);
                cmd.Parameters.AddWithValue("IncludePastEmailItems", policy.MaxEmailAgeFilter);
                cmd.Parameters.AddWithValue("LimitEmailSizeInKB", policy.MaxEmailBodyTruncationSize);
                cmd.Parameters.AddWithValue("AllowDirectPushWhenRoaming", policy.RequireManualSyncWhenRoaming);
                cmd.Parameters.AddWithValue("AllowHTMLEmail", policy.AllowHTMLEmail);
                cmd.Parameters.AddWithValue("AllowAttachmentsDownload", policy.AttachmentsEnabled);
                cmd.Parameters.AddWithValue("MaximumAttachmentSizeInKB", policy.MaxAttachmentSize);
                cmd.Parameters.AddWithValue("AllowRemovableStorage", policy.AllowStorageCard);
                cmd.Parameters.AddWithValue("AllowCamera", policy.AllowCamera);
                cmd.Parameters.AddWithValue("AllowWiFi", policy.AllowWiFi);
                cmd.Parameters.AddWithValue("AllowInfrared", policy.AllowIrDA);
                cmd.Parameters.AddWithValue("AllowInternetSharing", policy.AllowInternetSharing);
                cmd.Parameters.AddWithValue("AllowRemoteDesktop", policy.AllowRemoteDesktop);
                cmd.Parameters.AddWithValue("AllowDesktopSync", policy.AllowDesktopSync);
                cmd.Parameters.AddWithValue("AllowBluetooth", policy.AllowBluetooth);
                cmd.Parameters.AddWithValue("AllowBrowser", policy.AllowBrowser);
                cmd.Parameters.AddWithValue("AllowConsumerMail", policy.AllowConsumerEmail);
                cmd.Parameters.AddWithValue("AllowUnsignedApplications", policy.AllowUnsignedApplications);
                cmd.Parameters.AddWithValue("AllowUnsignedInstallationPackages", policy.AllowUnsignedInstallationPackages);
                cmd.Parameters.AddWithValue("IsEnterpriseCAL", policy.IsEnterpriseCAL);
                cmd.Parameters.AddWithValue("AllowTextMessaging", policy.AllowTextMessaging);


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
        /// Removes a Exchange ActiveSync Policy
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="exchangeName"></param>
        public static void RemoveExchangeActiveSyncPolicy(string companyCode, string exchangeName)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"DELETE FROM Plans_ExchangeActiveSync WHERE CompanyCode=@CompanyCode AND ExchangeName=@ExchangeName", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
                cmd.Parameters.AddWithValue("@ExchangeName", exchangeName);

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
        /// Gets a list of groups for a specific company code
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<ExchangeGroup> GetGroups(string companyCode)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM DistributionGroups WHERE CompanyCode=@CompanyCode ORDER BY DisplayName", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                // Open Connection
                logger.DebugFormat("Opening connection to retrieve distribution groups");
                sql.Open();

                List<ExchangeGroup> groups = new List<ExchangeGroup>();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    logger.DebugFormat("Found distribution group {0}", r["DistinguishedName"].ToString());

                    ExchangeGroup newGroup = new ExchangeGroup();
                    newGroup.SQLID = int.Parse(r["ID"].ToString());
                    logger.DebugFormat("Group ID is {0}", newGroup.SQLID);

                    newGroup.DistinguishedName = r["DistinguishedName"].ToString();
                    logger.DebugFormat("Group distinguished name is {0}", newGroup.DistinguishedName);

                    newGroup.DisplayName = r["DisplayName"].ToString();
                    logger.DebugFormat("Group Display Name is {0}", newGroup.DisplayName);

                    newGroup.PrimarySmtpAddress = r["Email"].ToString();
                    logger.DebugFormat("Group email is {0}", newGroup.PrimarySmtpAddress);

                    bool hidden = false;
                    bool.TryParse(r["Hidden"].ToString(), out hidden);
                    newGroup.Hidden = hidden;
                    logger.DebugFormat("Group hidden: {0}", newGroup.Hidden);

                    newGroup.CompanyCode = r["CompanyCode"].ToString();
                    logger.DebugFormat("Group company code is {0}", newGroup.CompanyCode);

                    groups.Add(newGroup);
                }

                r.Close();
                r.Dispose();

                logger.DebugFormat("Finished getting groups for {0}", companyCode);

                // Return collection
                return groups;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting groups for {0}. Error: {1}", companyCode, ex.ToString());
                throw;
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }
        }

        /// <summary>
        /// Removes a group from SQL
        /// </summary>
        /// <param name="emailAddress"></param>
        public static void RemoveGroup(string emailAddress)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"DELETE FROM DistributionGroups WHERE Email=@Email", sql);

            try
            {
                // Add parameter
                cmd.Parameters.AddWithValue("@Email", emailAddress);

                // Open Connection
                sql.Open();

                // Delete
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
        /// Updates a company's Exchange status
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="isEnabled"></param>
        public static void SetCompanyExchangeEnabled(string companyCode, bool isEnabled)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("UPDATE Companies SET ExchEnabled=@Enabled WHERE CompanyCode=@CompanyCode AND IsReseller=0", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
                cmd.Parameters.AddWithValue("@Enabled", isEnabled);

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
        /// Adds a new distribution group to SQL
        /// </summary>
        /// <param name="distinguishedName"></param>
        /// <param name="companyCode"></param>
        /// <param name="displayName"></param>
        /// <param name="email"></param>
        /// <param name="hidden"></param>
        public static void AddDistributionGroup(string distinguishedName, string companyCode, string displayName, string email, bool hidden)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"INSERT INTO DistributionGroups (DistinguishedName, CompanyCode, DisplayName, Email, Hidden) VALUES
                                                     (@DistinguishedName, @CompanyCode, @DisplayName, @Email, @Hidden)", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@DistinguishedName", distinguishedName);
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
                cmd.Parameters.AddWithValue("@DisplayName", displayName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Hidden", hidden);

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
        /// Updates an existing distribution group
        /// </summary>
        /// <param name="oldEmail"></param>
        /// <param name="newEmail"></param>
        /// <param name="displayName"></param>
        /// <param name="hidden"></param>
        public static void UpdateDistributionGroup(string newDistinguishedName, string oldEmail, string newEmail, string displayName, bool hidden)
        {
            // Initialize SQL
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE DistributionGroups SET DistinguishedName=@DistinguishedName, Email=@NewEmail, DisplayName=@DisplayName, Hidden=@Hidden WHERE Email=@OldEmail", sql);

            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@DistinguishedName", newDistinguishedName);
                cmd.Parameters.AddWithValue("@NewEmail", newEmail);
                cmd.Parameters.AddWithValue("@DisplayName", displayName);
                cmd.Parameters.AddWithValue("@Hidden", hidden);
                cmd.Parameters.AddWithValue("@OldEmail", oldEmail);

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
        /// Gets a list of currently accepted domains for a specific company
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<Domain> GetAcceptedDomains(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM Domains WHERE CompanyCode=@CompanyCode AND IsAcceptedDomain=1 ORDER BY Domain", sql);

            List<Domain> domains = new List<Domain>();
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
                    domains.Add(new Domain()
                    {
                        CompanyCode = companyCode,
                        DomainID = int.Parse(r["DomainID"].ToString()),
                        DomainName = r["Domain"].ToString(),
                        IsAcceptedDomain = true
                    });
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

        /// <summary>
        /// Gets a list of forwarding addresses 
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<ForwardingObject> GetForwardingAddresses(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT DistinguishedName, UserPrincipalName, DisplayName FROM Users WHERE MailboxPlan > 0 AND CompanyCode=@CompanyCode;
                                              SELECT DistinguishedName, DisplayName, Email FROM Contacts WHERE CompanyCode=@CompanyCode;
                                              SELECT DistinguishedName, DisplayName, Email FROM DistributionGroups WHERE CompanyCode=@CompanyCode", sql);

            List<ForwardingObject> forwardingList = new List<ForwardingObject>();
            try
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);

                // Open connection
                sql.Open();

                // Read
                string section = "Users";
                SqlDataReader r = cmd.ExecuteReader();
                do
                {
                    while (r.Read())
                    {
                        ForwardingObject tmp = new ForwardingObject();
                        tmp.DistinguishedName = r["DistinguishedName"].ToString();
                        tmp.DisplayName = r["DisplayName"].ToString();
                        tmp.ObjectType = section;

                        // Add to our list
                        forwardingList.Add(tmp);
                    }

                    if (section == "Users")
                        section = "Contacts";
                    else if (section == "Contacts")
                        section = "DistributionGroups";
                }
                while (r.NextResult());

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

            return forwardingList;
        }

        /// <summary>
        /// Gets a list of distribution groups from the database
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<ExchangeGroup> GetDistributionGroups(string companyCode)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"SELECT DistinguishedName, DisplayName, Email, Hidden FROM DistributionGroups WHERE CompanyCode=@CompanyCode ORDER BY DisplayName", sql);

            List<ExchangeGroup> groups = new List<ExchangeGroup>();
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
                    ExchangeGroup tmp = new ExchangeGroup();
                    tmp.DistinguishedName = r["DistinguishedName"].ToString();
                    tmp.CompanyCode = companyCode;
                    tmp.DisplayName = r["DisplayName"].ToString();
                    tmp.PrimarySmtpAddress = r["Email"].ToString();
                    tmp.Hidden = bool.Parse(r["Hidden"].ToString());

                    groups.Add(tmp);
                }

                // Close and dispose
                r.Close();
                r.Dispose();

                // Return our object
                return groups;
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
    }
}
