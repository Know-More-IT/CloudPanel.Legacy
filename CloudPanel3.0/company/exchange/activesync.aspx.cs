using CloudPanel.Modules.Base;
using CloudPanel.Modules.Exchange;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CloudPanel.company.exchange
{
    public partial class activesync : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                GetPolicies();
        }

        /// <summary>
        /// Gets a list of policies from SQL
        /// </summary>
        private void GetPolicies()
        {
            // Show list policies panel
            panelActiveSyncList.Visible = true;

            // Hide other panels
            panelActiveSyncEdit.Visible = false;

            try
            {
                List<BaseActivesyncPolicy> policies = SQLExchange.GetExchangeActiveSyncPolicies(CPContext.SelectedCompanyCode);
                repeater.DataSource = policies;
                repeater.DataBind();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error retrieving policies from the database: " + ex.Message);
            }
        }

        /// <summary>
        /// Shows the create new policy panel
        /// </summary>
        protected void lnkCreatePolicy_Click(object sender, EventArgs e)
        {
            // Hide panel
            panelActiveSyncList.Visible = false;

            // Show edit/new policy panel
            panelActiveSyncEdit.Visible = true;
        }

        protected void repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Get current list of policies and refresh view
            GetPolicies();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Create a new policy
            CreateNewPolicy();
        }

        private void CreateNewPolicy()
        {
            ExchCmds powershell = null;

            try
            {
                BaseActivesyncPolicy policy = new BaseActivesyncPolicy();

                // Compile list of general settings
                policy.DisplayName = txtDisplayName.Text;
                policy.Description = txtDescription.Text;

                // Compile list of basic settings
                policy.AllowBluetooth = ddlAllowBluetooth.SelectedValue;
                policy.AllowBrowser = cbAllowBrowser.Checked;
                policy.AllowCamera = cbAllowCamera.Checked;
                policy.AllowConsumerEmail = cbAllowConsumerMail.Checked;
                policy.AllowDesktopSync = cbAllowDesktopSync.Checked;
                policy.AllowInternetSharing = cbAllowInternetSharing.Checked;
                policy.AllowSimpleDevicePassword = cbAllowSimplePassword.Checked;
                policy.AllowTextMessaging = cbAllowTextMessaging.Checked;
                policy.AllowWiFi = cbAllowWIFI.Checked;
                policy.DevicePasswordEnabled = cbPasswordEnabled.Checked;
                policy.AlphanumericDevicePasswordRequired = cbAlphanumericPwdRequired.Checked;
                policy.MaxDevicePasswordFailedAttempts = ddlMaxFailedPasswordAttempts.SelectedValue;
                policy.AllowHTMLEmail = cbAllowHTMLEmail.Checked;
                policy.AllowIrDA = cbAllowInfrared.Checked;
                policy.AllowNonProvisionableDevice = cbAllowNonProvisionable.Checked;
                policy.AllowPOPIMAPEmail = cbAllowPOPIMAP.Checked;
                policy.AllowRemoteDesktop = cbAllowRemoteDesktop.Checked;
                policy.AllowSMIMEEncryptionAlgorithmNegotiation = ddlAllowSMIMEEncryptionAlgorithmNeg.SelectedValue;
                policy.AllowSMIMESoftCerts = cbAllowSMIME.Checked;
                policy.AllowStorageCard = cbAllowStorageCard.Checked;
                policy.AllowUnsignedApplications = cbAllowUnsignedApps.Checked;
                policy.AllowUnsignedInstallationPackages = cbAllowUnsignedInstallPackages.Checked;
                policy.AttachmentsEnabled = cbAttachmentsEnabled.Checked;
                policy.DeviceEncryptionEnabled = cbDeviceEncryptionEnabled.Checked;
                policy.DevicePasswordExpiration = txtPasswordExpiration.Text;
                policy.DevicePolicyRefreshInterval = txtPolicyRefreshInterval.Text;
                policy.MaxAttachmentSize = txtMaxAttachmentSize.Text;
                policy.MaxCalendarAgeFilter = ddlMaxCalendarAgeFilter.SelectedValue;
                policy.MaxEmailAgeFilter = ddlMaxEmailAgeFilter.SelectedValue;
                policy.PasswordRecoveryEnabled = cbPasswordRecovery.Checked;
                policy.RequireDeviceEncryption = cbRequireDeviceEncryption.Checked;
                policy.RequireEncryptedSMIMEMessages = cbRequireEncryptedSMIMEMsg.Checked;
                policy.RequireEncryptionSMIMEAlgorithm = ddlRequireEncryptedSMIMEAlgorithm.SelectedValue;
                policy.RequireSignedSMIMEMessages = cbRequireSignedSMIMEMsg.Checked;
                policy.RequireSignedSMIMEAlgorithm = ddlRequireSignedSMIMEMsg.SelectedValue;
                policy.RequireManualSyncWhenRoaming = cbRequireManualSyncRoaming.Checked;
                policy.RequireStorageCardEncryption = cbRequireStorageCardEncryption.Checked;

                policy.MaxEmailHTMLBodyTruncationSize = txtMaxHTMLBodyTruncSize.Text;
                policy.MaxEmailBodyTruncationSize = txtMaxEmailBodyTruncSize.Text;

                int minComplexChar = 0;
                int.TryParse(txtMinPwdComplexChar.Text, out minComplexChar);
                policy.MinDevicePasswordComplexCharacters = minComplexChar;
                
                int inactivityLock = 0;
                int.TryParse(txtMaxInactivityLock.Text, out inactivityLock);
                policy.MaxInactivityTimeDeviceLock = inactivityLock;

                int passwordHistory = 0;
                int.TryParse(txtPasswordHistory.Text, out passwordHistory);
                policy.DevicePasswordHistory = passwordHistory;

                int minPwdLength = 0;
                int.TryParse(txtMinPwdLength.Text, out minPwdLength);
                policy.MinDevicePasswordLength = minPwdLength;

                // Exchange 2013
                policy.AllowApplePushNotifications = cbAllowApplePushNotifications.Checked;

                // Initilaze powershell
                powershell = new ExchCmds(Retrieve.ExchangeUri, Retrieve.Username, Retrieve.Password, Retrieve.ExchangeKerberos, Retrieve.PrimaryDC);

                // Insert into Exchange
                powershell.New_ActiveSyncPolicy(policy, CPContext.SelectedCompanyCode, Retrieve.ExchangeVersion);

                // Insert into SQL
                SQLExchange.AddExchangeActiveSyncPolicy(CPContext.SelectedCompanyCode, txtDisplayName.Text, txtDescription.Text);

                // Notify
                notification1.SetMessage(controls.notification.MessageType.Success, "Successfully added new activesync policy");
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error creating activesync policy: " + ex.Message);
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();

                // Refresh view
                GetPolicies();
            }
        }
    }
}