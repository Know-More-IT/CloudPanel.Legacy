using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using System.Reflection;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Sql;

namespace CloudPanel.plans
{
    public partial class organization : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulatePlans();
            }
        }

        /// <summary>
        /// Get a list of plans from the database
        /// </summary>
        private void PopulatePlans()
        {
            // Clear all fields
            ClearTextBoxes();

            // Clear drop down and add default items
            ddlCompanyPlan.Items.Clear();
            ddlCompanyPlan.Items.Add(Resources.LocalizedText.CreateNew);

            List<BasePlanCompany> plans = null;
            try
            {
                plans = SQLPlans.GetCompanyPlans();
                foreach (BasePlanCompany c in plans)
                {
                    ListItem item = new ListItem();
                    item.Text = c.PlanName;
                    item.Value = c.PlanID.ToString();

                    ddlCompanyPlan.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                // Log the error
                logger.Error("Error retrieving company plans", ex);
            }
            finally
            {
                plans = null;
            }
        }

        /// <summary>
        /// Clear all text boxes in each panel
        /// </summary>
        private void ClearTextBoxes()
        {
            txtPlanName.Text = "";
            txtMaxUsers.Text = "";
            txtMaxDomains.Text = "";
            txtMaxMailboxes.Text = "";
            txtMaxContacts.Text = "";
            txtMaxDistributionLists.Text = "";
            txtMaxMailPublicFolders.Text = "";
            txtMaxResourceMailboxes.Text = "";
            txtMaxCitrixUsers.Text = "";
            txtMaxCitrixAppPerUser.Text = "";
            txtMaxCitrixSvrPerUser.Text = "";
        }

        /// <summary>
        /// Get detailed information about the plan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCompanyPlan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCompanyPlan.SelectedIndex == 0)
                ClearTextBoxes();
            else
            {
                try
                {
                    BasePlanCompany plan = SQLPlans.GetCompanyPlan(int.Parse(ddlCompanyPlan.SelectedValue));

                    txtPlanName.Text = plan.PlanName;
                    txtMaxUsers.Text = plan.MaxUsers.ToString();
                    txtMaxDomains.Text = plan.MaxDomains.ToString();
                    txtMaxMailboxes.Text = plan.MaxExchangeMailboxes.ToString();
                    txtMaxContacts.Text = plan.MaxExchangeContacts.ToString();
                    txtMaxDistributionLists.Text = plan.MaxExchangeDistLists.ToString();
                    txtMaxResourceMailboxes.Text = plan.MaxExchangeResourceMailboxes.ToString();
                    txtMaxMailPublicFolders.Text = plan.MaxExchangeMailPublicFolders.ToString();
                    txtMaxCitrixUsers.Text = plan.MaxCitrixUsers.ToString();

                    plan = null;
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                    // Log Error //
                    this.logger.Error("Error getting company plan id " + ddlCompanyPlan.SelectedIndex.ToString() + ".", ex);
                }
            }
        }

        /// <summary>
        /// Add New plan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUpdatePlan_Click(object sender, EventArgs e)
        {
            // Gather our data
            BasePlanCompany plan = new BasePlanCompany();
            plan.PlanName = txtPlanName.Text;
            plan.MaxUsers = int.Parse(txtMaxUsers.Text);
            plan.MaxDomains = int.Parse(txtMaxDomains.Text);
            plan.MaxExchangeMailboxes = int.Parse(txtMaxMailboxes.Text);
            plan.MaxExchangeContacts = int.Parse(txtMaxContacts.Text);
            plan.MaxExchangeDistLists = int.Parse(txtMaxDistributionLists.Text);
            plan.MaxExchangeResourceMailboxes = int.Parse(txtMaxResourceMailboxes.Text);
            plan.MaxExchangeMailPublicFolders = int.Parse(txtMaxMailPublicFolders.Text);
            plan.MaxCitrixUsers = int.Parse(txtMaxCitrixUsers.Text);
            

            // Check if we are adding or updating
            try
            {
                if (ddlCompanyPlan.SelectedIndex > 0)
                {
                    // We are updating an existing plan
                    plan.PlanID = int.Parse(ddlCompanyPlan.SelectedValue);

                    SQLPlans.UpdateCompanyPlan(plan);

                    // Refresh
                    PopulatePlans();
                }
                else
                {
                    // We are adding a new plan
                    SQLPlans.AddCompanyPlan(plan);

                    // Refresh
                    PopulatePlans();
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                // Log //
                this.logger.Error("There was an error adding / updating company plan.", ex);
            }
        }

        /// <summary>
        /// Deletes a plan
        /// </summary>
        protected void btnDeletePlan_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlCompanyPlan.SelectedIndex > 0)
                {
                    int planId = 0;
                    int.TryParse(ddlCompanyPlan.SelectedValue, out planId);

                    if (planId > 0)
                    {
                        int deletedRows = SQLPlans.DeleteCompanyPlan(planId);
                        if (deletedRows == 0)
                            notification1.SetMessage(controls.notification.MessageType.Warning, Resources.LocalizedText.DeleteWarningCompanyPlan);
                        else
                        {
                            // Refresh view
                            PopulatePlans();

                            // Update notication
                            notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.DeleteSuccessPlan);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                // Log //
                this.logger.Error("Error deleting plan " + ddlCompanyPlan.SelectedItem.Text, ex);
            }
        }
    }
}