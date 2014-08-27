using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudPanel.Modules.Sql;
using CloudPanel.Modules.Base;
using log4net;
using System.Reflection;
using CloudPanel.Modules.Base.Class;

namespace CloudPanel.plans
{
    public partial class mailbox : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Get list of mailbox plans
                GetMailboxPlans();

                // Get list of companies
                GetCompanies();
            }
        }

        /// <summary>
        /// Populate the drop down box with a list of mailbox plan
        /// </summary>
        private void GetMailboxPlans()
        {
            try
            {
                // Clear all items
                ClearTextBoxes();

                // Clear the current list box
                ddlMailboxPlans.Items.Clear();

                // Add the create new section
                ddlMailboxPlans.Items.Add("--- Create New ---");

                // Get new list of plans
                List<MailboxPlan> plans = SQLPlans.GetMailboxPlans();
                foreach (MailboxPlan p in plans)
                {
                    ListItem item = new ListItem();

                    if (string.IsNullOrEmpty(p.CompanyCode))
                        item.Text = p.DisplayName;
                    else
                        item.Text = "[" + p.CompanyCode + "] " + p.DisplayName;

                    item.Value = p.PlanID.ToString();

                    // Add to drop down list
                    ddlMailboxPlans.Items.Add(item);

                }

                // Dispose
                plans = null;
            }
            catch (Exception ex)
            {
                // ERROR //
                this.logger.Error("There was an error getting a list of mailbox plans: " + ex.ToString());

                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Populate the drop down box with a list of companies
        /// </summary>
        private void GetCompanies()
        {
            try
            {
                // Clear drop down box
                ddlSpecificCompany.Items.Clear();

                // Add blank for first value
                ddlSpecificCompany.Items.Add(string.Empty);

                // Get list of companies
                List<Company> companies = SQLCompanies.GetCompanies();

                // Populate drop down list
                foreach (Company c in companies)
                {
                    ListItem item = new ListItem();
                    item.Text = c.CompanyName;
                    item.Value = c.CompanyCode;

                    // Add to drop down list
                    ddlSpecificCompany.Items.Add(item);
                }

            }
            catch (Exception ex)
            {
                // ERROR //
                this.logger.Error("There was an error getting a list of companies plans: " + ex.ToString());

                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Clears checkboxes and textboxes
        /// </summary>
        private void ClearTextBoxes()
        {
            // Hide notification
            notification1.HidePanel();

            // Clear text boxes
            txtPlanName.Text = "";
            txtDescription.Text = "";
            txtMaxRecipients.Text = "";
            txtKeepDeletedItems.Text = "";
            txtMailboxSize.Text = "";
            txtMaxMailboxSize.Text = "";
            txtSendSize.Text = "";
            txtReceiveSize.Text = "";
            txtCost.Text = "0.00";
            txtPrice.Text = "0.00";
            txtPriceAdditionalGB.Text = "0.00";

            // Reset drop down to first
            ddlSpecificCompany.SelectedIndex = 0;

            // Clear features
            cbEnableAS.Checked = false;
            cbEnableECP.Checked = false;
            cbEnableIMAP.Checked = false;
            cbEnableMAPI.Checked = false;
            cbEnableOWA.Checked = false;
            cbEnablePOP3.Checked = false;

            // Enable company code field
            ddlSpecificCompany.Enabled = true;
        }

        /// <summary>
        /// Choose between plans
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlMailboxPlans_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlMailboxPlans.SelectedIndex == 0)
                ClearTextBoxes();
            else
            {
                try
                {
                    MailboxPlan plan = SQLPlans.GetMailboxPlan(int.Parse(ddlMailboxPlans.SelectedValue));
                    txtPlanName.Text = plan.DisplayName;
                    txtDescription.Text = plan.Description;
                    txtMaxRecipients.Text = plan.MaxRecipients.ToString();
                    txtKeepDeletedItems.Text = plan.KeepDeletedItemsInDays.ToString();
                    txtMailboxSize.Text = plan.SizeInMB.ToString();
                    txtMaxMailboxSize.Text = plan.MaximumSizeInMB.ToString();
                    txtSendSize.Text = plan.MaxSendSizeInKB.ToString();
                    txtReceiveSize.Text = plan.MaxReceiveSizeInKB.ToString();
                    txtCost.Text = plan.Cost;
                    txtPrice.Text = plan.Price;
                    txtPriceAdditionalGB.Text = plan.AdditionalGBPrice;

                    cbEnablePOP3.Checked = plan.POP3Enabled;
                    cbEnableIMAP.Checked = plan.IMAPEnabled;
                    cbEnableOWA.Checked = plan.OWAEnabled;
                    cbEnableMAPI.Checked = plan.MAPIEnabled;
                    cbEnableAS.Checked = plan.ActiveSyncEnabled;
                    cbEnableECP.Checked = plan.ECPEnabled;

                    if (!string.IsNullOrEmpty(plan.CompanyCode))
                        ddlSpecificCompany.SelectedValue = plan.CompanyCode;
                    else
                        ddlSpecificCompany.SelectedIndex = 0;

                    // Disable changing company code because this isn't supported
                    ddlSpecificCompany.Enabled = false;

                    plan = null;
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, "Error: " + ex.Message);

                    // Log Error //
                    this.logger.Error("Error getting company plan id " + ddlMailboxPlans.SelectedValue + ".", ex);
                }
            }
        }

        /// <summary>
        /// Adds or updates new/existing mailbox plans
        /// </summary>
        protected void btnUpdatePlan_Click(object sender, EventArgs e)
        {
            try
            {
                // Gather our data
                BasePlanMailbox plan = new BasePlanMailbox();
                plan.MailboxPlanName = txtPlanName.Text;
                plan.MailboxPlanDesc = txtDescription.Text;
                plan.MaxRecipients = int.Parse(txtMaxRecipients.Text);
                plan.MaxKeepDeletedItems = int.Parse(txtKeepDeletedItems.Text);
                plan.MailboxSize = int.Parse(txtMailboxSize.Text);
                plan.MaxMailboxSize = int.Parse(txtMaxMailboxSize.Text);
                plan.MaxSendKB = int.Parse(txtSendSize.Text);
                plan.MaxReceiveKB = int.Parse(txtReceiveSize.Text);
                plan.Cost = txtCost.Text;
                plan.Price = txtPrice.Text;
                plan.AdditionalGBPrice = txtPriceAdditionalGB.Text;

                plan.EnablePOP3 = cbEnablePOP3.Checked;
                plan.EnableIMAP = cbEnableIMAP.Checked;
                plan.EnableOWA = cbEnableOWA.Checked;
                plan.EnableMAPI = cbEnableMAPI.Checked;
                plan.EnableAS = cbEnableAS.Checked;
                plan.EnableECP = cbEnableECP.Checked;

                if (ddlSpecificCompany.SelectedIndex > 0)
                    plan.CompanyCode = ddlSpecificCompany.SelectedValue;
                else
                    plan.CompanyCode = string.Empty;

                if (ddlMailboxPlans.SelectedIndex > 0)
                {
                    // We are updating an existing plan
                    plan.MailboxPlanID = int.Parse(ddlMailboxPlans.SelectedValue);

                    SQLPlans.UpdateMailboxPlan(plan);

                    // Refresh
                    GetMailboxPlans();

                    // Update notication
                    notification1.SetMessage(controls.notification.MessageType.Success, "Successfully updated existing mailbox plan.");
                }
                else
                {
                    // We are adding a new plan
                    SQLPlans.AddMailboxPlan(plan);

                    // Refresh
                    GetMailboxPlans();

                    // Update notication
                    notification1.SetMessage(controls.notification.MessageType.Success, "Successfully created new mailbox plan.");
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error: " + ex.Message);

                // Log //
                this.logger.Error("There was an error adding / updating mailbox plan.", ex);
            }
        }

        /// <summary>
        /// Deletes a mailbox plan if it is not in use
        /// </summary>
        protected void btnDeletePlan_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlMailboxPlans.SelectedIndex > 0)
                {
                    int planId = 0;
                    int.TryParse(ddlMailboxPlans.SelectedValue, out planId);

                    if (planId > 0)
                    {
                        int deletedRows = SQLPlans.DeleteMailboxPlan(planId);
                        if (deletedRows == 0)
                            notification1.SetMessage(controls.notification.MessageType.Warning, "Unable to delete the mailbox plan because there are users currently assigned to this plan.");
                        else
                        {
                            // Refresh view
                            GetMailboxPlans();

                            // Update notification
                            notification1.SetMessage(controls.notification.MessageType.Success, "Successfully deleted mailbox plan.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error: " + ex.Message);

                // Log //
                this.logger.Error("Error deleting plan " + ddlMailboxPlans.SelectedItem.Text, ex);
            }
        }
    }
}