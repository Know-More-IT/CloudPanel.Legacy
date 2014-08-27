using CloudPanel.Modules.Base;
using CloudPanel.Modules.Exchange;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CloudPanel.plans
{
    public partial class activesync : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetPolicies();
            }
        }

        private void ResetFields()
        {
            // General
            txtDisplayName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            cbAllowNonProvisionableDevices.Checked = true;
            txtRefreshInterval.Text = string.Empty;

            // Password
            cbRequirePassword.Checked = false;
            cbRequireAlphaNumericPassword.Checked = false;
            txtMinimumNumberOfCharacterSets.Text = "1";
            cbEnablePasswordRecovery.Checked = false;
            cbRequireEncryption.Checked = false;
            cbRequireEncryptionOnStorageCard.Checked = false;
            cbAllowSimplePassword.Checked = true;
            txtNumberOfFailedAttemptsAllowed.Text = string.Empty;
            txtMinimumPasswordLength.Text = string.Empty;
            txtInactivityTimeout.Text = string.Empty;
            txtPasswordExpiration.Text = string.Empty;
            txtEnforcePasswordHistory.Text = "0";

            // Sync Settings
            ddlPastCalendarItems.SelectedValue = "All";
            ddlPastEmailItems.SelectedValue = "All";
            txtLimitEmailSize.Text = string.Empty;
            cbAllowDirectPushWhenRoaming.Checked = true;
            cbAllowHTMLEmail.Checked = true;
            cbAllowAttachmentDownload.Checked = true;
            txtMaximumAttachmentSize.Text = string.Empty;

            // Device
            cbAllowRemovableStorage.Checked = true;
            cbAllowCamera.Checked = true;
            cbAllowWiFi.Checked = true;
            cbAllowInfrared.Checked = true;
            cbAllowInternetSharing.Checked = true;
            cbAllowRemoteDesktop.Checked = true;
            cbAllowDesktopSync.Checked = true;
            cbAllowTextMessaging.Checked = true;
            ddlAllowBluetooth.SelectedValue = "Allow";

            // Device Applications
            cbAllowBrowser.Checked = true;
            cbAllowConsumerMail.Checked = true;
            cbAllowUnsignedApplications.Checked = true;
            cbAllowUnsignedInstallPackages.Checked = true;
        }

        
        /// <summary>
        /// Gets a list of policies from SQL
        /// </summary>
        private void GetPolicies()
        {
            try
            {
                // Reset all the fields
                ResetFields();

                List<BaseActivesyncPolicy> policies = SQLExchange.GetExchangeActiveSyncPolicies();

                // Clear all items
                ddlActiveSyncPlan.Items.Clear();

                // Add default
                ddlActiveSyncPlan.Items.Add(Resources.LocalizedText.CreateNew);

                // Add the rest
                foreach (BaseActivesyncPolicy p in policies)
                {
                    ListItem item = new ListItem();
                    item.Value = p.ASID.ToString();
                    item.Text = p.DisplayName;

                    // Add item
                    ddlActiveSyncPlan.Items.Add(item);
                }

                // Select first one
                ddlActiveSyncPlan.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.ToString());
            }
        }

        /// <summary>
        /// Gets a specific policy from the database
        /// </summary>
        /// <param name="policyID"></param>
        private void GetPolicy(int policyID)
        {
            try
            {
                BaseActivesyncPolicy policy = SQLExchange.GetExchangeActiveSyncPolicy(policyID);
                txtDisplayName.Text = policy.DisplayName;
                txtDescription.Text = policy.Description;

                //
                // Password TAB
                //
                if (policy.DevicePasswordEnabled)
                {
                    // Set the require password checkbox
                    cbRequirePassword.Checked = true;

                    // Show the additional DIV
                    IfRequirePassword.Style.Remove("display");

                    // Set all the values under require password
                    cbRequireAlphaNumericPassword.Checked = policy.AlphanumericDevicePasswordRequired;
                    cbEnablePasswordRecovery.Checked = policy.PasswordRecoveryEnabled;
                    cbRequireEncryption.Checked = policy.RequireDeviceEncryption;
                    cbRequireEncryptionOnStorageCard.Checked = policy.RequireStorageCardEncryption;
                    cbAllowSimplePassword.Checked = policy.AllowSimpleDevicePassword;

                    if (policy.MinDevicePasswordComplexCharacters > 0)
                        txtMinimumNumberOfCharacterSets.Text = policy.MinDevicePasswordComplexCharacters.ToString();
                    else
                        txtMinimumNumberOfCharacterSets.Text = string.Empty;

                    if (policy.MaxDevicePasswordFailedAttempts > 0)
                        txtNumberOfFailedAttemptsAllowed.Text = policy.MaxDevicePasswordFailedAttempts.ToString();
                    else
                        txtNumberOfFailedAttemptsAllowed.Text = string.Empty;

                    if (policy.MinDevicePasswordLength > 0)
                        txtMinimumPasswordLength.Text = policy.MinDevicePasswordLength.ToString();
                    else
                        txtMinimumPasswordLength.Text = string.Empty;

                    if (policy.MaxInactivityTimeDeviceLock > 0)
                        txtInactivityTimeout.Text = policy.MaxInactivityTimeDeviceLock.ToString();
                    else
                        txtInactivityTimeout.Text = string.Empty;

                    if (policy.DevicePasswordExpiration > 0)
                        txtPasswordExpiration.Text = policy.DevicePasswordExpiration.ToString();
                    else
                        txtPasswordExpiration.Text = string.Empty;

                    if (policy.DevicePasswordHistory > 0)
                        txtEnforcePasswordHistory.Text = policy.DevicePasswordHistory.ToString();
                    else
                        txtEnforcePasswordHistory.Text = string.Empty;

                }
                else
                {
                    // Hide div
                    IfRequirePassword.Style.Add("display", "none");

                    // Reset to defaults
                    cbRequirePassword.Checked = false;
                    cbRequireAlphaNumericPassword.Checked = false;
                    txtMinimumNumberOfCharacterSets.Text = "1";
                    cbEnablePasswordRecovery.Checked = false;
                    cbRequireEncryption.Checked = false;
                    cbRequireEncryptionOnStorageCard.Checked = false;
                    cbAllowSimplePassword.Checked = true;
                    txtNumberOfFailedAttemptsAllowed.Text = string.Empty;
                    txtMinimumPasswordLength.Text = string.Empty;
                    txtInactivityTimeout.Text = string.Empty;
                    txtPasswordExpiration.Text = string.Empty;
                    txtEnforcePasswordHistory.Text = "0";
                }

                //
                // Sync Settings
                //
                if (!string.IsNullOrEmpty(policy.MaxCalendarAgeFilter))
                {
                    ListItem item = ddlPastCalendarItems.Items.FindByValue(policy.MaxCalendarAgeFilter);
                    if (item != null)
                        ddlPastCalendarItems.SelectedValue = item.Value;
                    else
                        ddlPastCalendarItems.SelectedValue = "All";
                }
                else
                    ddlPastCalendarItems.SelectedValue = "All";

                if (!string.IsNullOrEmpty(policy.MaxEmailAgeFilter))
                {
                    ListItem item = ddlPastEmailItems.Items.FindByValue(policy.MaxEmailAgeFilter);
                    if (item != null)
                        ddlPastEmailItems.SelectedValue = item.Value;
                    else
                        ddlPastEmailItems.SelectedValue = "All";
                }
                else
                    ddlPastEmailItems.SelectedValue = "All";

                if (policy.MaxEmailBodyTruncationSize > 0)
                    txtLimitEmailSize.Text = policy.MaxEmailBodyTruncationSize.ToString();
                else
                    txtLimitEmailSize.Text = string.Empty;

                cbAllowDirectPushWhenRoaming.Checked = policy.RequireManualSyncWhenRoaming;
                cbAllowHTMLEmail.Checked = policy.AllowHTMLEmail;
                cbAllowAttachmentDownload.Checked = policy.AllowHTMLEmail;

                if (policy.MaxAttachmentSize > 0)
                    txtMaximumAttachmentSize.Text = policy.MaxAttachmentSize.ToString();
                else
                    txtMaximumAttachmentSize.Text = string.Empty;

                //
                // DEVICE TAB
                //
                cbAllowRemovableStorage.Checked = policy.AllowStorageCard;
                cbAllowCamera.Checked = policy.AllowCamera;
                cbAllowWiFi.Checked = policy.AllowWiFi;
                cbAllowInfrared.Checked = policy.AllowIrDA;
                cbAllowInternetSharing.Checked = policy.AllowInternetSharing;
                cbAllowRemoteDesktop.Checked = policy.AllowRemoteDesktop;
                cbAllowDesktopSync.Checked = policy.AllowDesktopSync;

                if (!string.IsNullOrEmpty(policy.AllowBluetooth))
                {
                    ListItem item = ddlAllowBluetooth.Items.FindByValue(policy.AllowBluetooth);
                    if (item != null)
                        ddlAllowBluetooth.SelectedValue = item.Value;
                    else
                        ddlAllowBluetooth.SelectedValue = "Allow";
                }
                else
                    ddlAllowBluetooth.SelectedValue = "Allow";


                //
                // DEVICE APPLICATIONS TAB
                //
                cbAllowBrowser.Checked = policy.AllowBrowser;
                cbAllowConsumerMail.Checked = policy.AllowConsumerEmail;
                cbAllowUnsignedApplications.Checked = policy.AllowUnsignedApplications;
                cbAllowUnsignedInstallPackages.Checked = policy.AllowUnsignedInstallationPackages;

                
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlActiveSyncPlan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlActiveSyncPlan.SelectedIndex > 0)
            {
                try
                {
                    GetPolicy(int.Parse(ddlActiveSyncPlan.SelectedValue));
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUpdatePlan_Click(object sender, EventArgs e)
        {
            BaseActivesyncPolicy policy = new BaseActivesyncPolicy();

            ExchCmds powershell = null;
            try
            {
                // Fill in custom object
                policy.CompanyCode = "";
                policy.DisplayName = txtDisplayName.Text;
                policy.Description = txtDescription.Text;
                policy.AllowNonProvisionableDevice = cbAllowNonProvisionableDevices.Checked;

                if (!string.IsNullOrEmpty(txtRefreshInterval.Text))
                    policy.DevicePolicyRefreshInterval = int.Parse(txtRefreshInterval.Text);
                else
                    policy.DevicePolicyRefreshInterval = 0;

                //
                // PASSWORD TAB
                //
                policy.DevicePasswordEnabled = cbRequirePassword.Checked;
                if (policy.DevicePasswordEnabled)
                {
                    policy.AlphanumericDevicePasswordRequired = cbRequireAlphaNumericPassword.Checked;

                    if (!string.IsNullOrEmpty(txtMinimumNumberOfCharacterSets.Text))
                        policy.MinDevicePasswordComplexCharacters = int.Parse(txtMinimumNumberOfCharacterSets.Text);
                    else
                        policy.MinDevicePasswordComplexCharacters = 0;

                    policy.PasswordRecoveryEnabled = cbEnablePasswordRecovery.Checked;
                    policy.RequireDeviceEncryption = cbRequireEncryption.Checked;
                    policy.RequireStorageCardEncryption = cbRequireEncryptionOnStorageCard.Checked;
                    policy.AllowSimpleDevicePassword = cbAllowSimplePassword.Checked;

                    if (!string.IsNullOrEmpty(txtNumberOfFailedAttemptsAllowed.Text))
                        policy.MaxDevicePasswordFailedAttempts = int.Parse(txtNumberOfFailedAttemptsAllowed.Text);
                    else
                        policy.MaxDevicePasswordFailedAttempts = 0;

                    if (!string.IsNullOrEmpty(txtMinimumPasswordLength.Text))
                        policy.MinDevicePasswordLength = int.Parse(txtMinimumPasswordLength.Text);
                    else
                        policy.MinDevicePasswordLength = 0;

                    if (!string.IsNullOrEmpty(txtInactivityTimeout.Text))
                        policy.MaxInactivityTimeDeviceLock = int.Parse(txtInactivityTimeout.Text);
                    else
                        policy.MaxInactivityTimeDeviceLock = 0;

                    if (!string.IsNullOrEmpty(txtPasswordExpiration.Text))
                        policy.DevicePasswordExpiration = int.Parse(txtPasswordExpiration.Text);
                    else
                        policy.DevicePasswordExpiration = 0;

                    if (!string.IsNullOrEmpty(txtEnforcePasswordHistory.Text))
                        policy.DevicePasswordHistory = int.Parse(txtEnforcePasswordHistory.Text);
                    else
                        policy.DevicePasswordHistory = 0;
                }


                //
                // SYNC SETTINGS
                //
                policy.MaxCalendarAgeFilter = ddlPastCalendarItems.SelectedValue;
                policy.MaxEmailAgeFilter = ddlPastEmailItems.SelectedValue;

                if (!string.IsNullOrEmpty(txtLimitEmailSize.Text))
                    policy.MaxEmailBodyTruncationSize = int.Parse(txtLimitEmailSize.Text);
                else
                    policy.MaxEmailBodyTruncationSize = 0;

                policy.RequireManualSyncWhenRoaming = cbAllowDirectPushWhenRoaming.Checked;
                policy.AllowHTMLEmail = cbAllowHTMLEmail.Checked;
                policy.AttachmentsEnabled = cbAllowAttachmentDownload.Checked;

                if (policy.AttachmentsEnabled)
                {
                    if (!string.IsNullOrEmpty(txtMaximumAttachmentSize.Text))
                        policy.MaxAttachmentSize = int.Parse(txtMaximumAttachmentSize.Text);
                    else
                        policy.MaxAttachmentSize = 0;
                }

                //
                // DEVICE
                //
                policy.AllowStorageCard = cbAllowRemovableStorage.Checked;
                policy.AllowCamera = cbAllowCamera.Checked;
                policy.AllowWiFi = cbAllowWiFi.Checked;
                policy.AllowIrDA = cbAllowInfrared.Checked;
                policy.AllowInternetSharing = cbAllowInternetSharing.Checked;
                policy.AllowRemoteDesktop = cbAllowRemoteDesktop.Checked;
                policy.AllowDesktopSync = cbAllowDesktopSync.Checked;
                policy.AllowBluetooth = ddlAllowBluetooth.SelectedValue;
                policy.AllowTextMessaging = cbAllowTextMessaging.Checked;

                //
                // DEVICE APPLICATIONS
                //
                policy.AllowBrowser = cbAllowBrowser.Checked;
                policy.AllowConsumerEmail = cbAllowConsumerMail.Checked;
                policy.AllowUnsignedApplications = cbAllowUnsignedApplications.Checked;
                policy.AllowUnsignedInstallationPackages = cbAllowUnsignedInstallPackages.Checked;
                


                // Initialize powershell
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // Check if we are creating a new policy or updating existing
                if (ddlActiveSyncPlan.SelectedIndex > 0)
                {
                    policy.ASID = int.Parse(ddlActiveSyncPlan.SelectedValue);

                    // Update Exchange
                    powershell.Update_ActiveSyncPolicy(policy, Config.ExchangeVersion);

                    // Update SQL database
                    SQLExchange.UpdateExchangeActiveSyncPolicy(policy);

                    // Update notification
                    notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.NotificationSuccessUpdateAS + policy.DisplayName);
                }
                else
                {
                    // Create new policy
                    powershell.New_ActiveSyncPolicy(policy, Config.ExchangeVersion);

                    // Add to SQL database
                    SQLExchange.AddExchangeActiveSyncPolicy(policy);

                    // Update notification
                    notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.NotificationSuccessCreatedAS + policy.DisplayName);
                }

            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();

                // Refresh view
                GetPolicies();
            }
        }
        
        /// <summary>
        /// Deletes a plan from the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDeletePlan_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlActiveSyncPlan.SelectedIndex > 0)
                {
                    int planId = 0;
                    int.TryParse(ddlActiveSyncPlan.SelectedValue, out planId);

                    if (planId > 0)
                    {
                        // Check if it is in use
                        List<string> whoIsUsingPlan = SQLPlans.IsPlanInUse(planId, SQLPlans.PlanType.ActiveSync);

                        if (whoIsUsingPlan != null)
                        {
                            notification1.SetMessage(controls.notification.MessageType.Warning, Resources.LocalizedText.NotificationFailedDeleteAS + String.Join(", ", whoIsUsingPlan));
                        }
                        else
                        {
                            // Delete plan because no one is using it
                            SQLPlans.DeleteActiveSyncPlan(planId);

                            // Refresh view
                            GetPolicies();

                            // Update notification
                            notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.NotificationSuccessDeleteAS);
                        }
                    }
                }
                else
                    notification1.SetMessage(controls.notification.MessageType.Warning, Resources.LocalizedText.NotificationWarningSelectAS);
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                // Log //
                this.logger.Error("Error deleting plan " + ddlActiveSyncPlan.SelectedItem.Text, ex);
            }
        }
    }
}