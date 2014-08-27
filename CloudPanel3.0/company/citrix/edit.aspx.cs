using CloudPanel.Modules.ActiveDirectory;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Base.Class;
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

namespace CloudPanel.company.citrix
{
    public partial class edit : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetCitrixPlans();
            }
        }

        /// <summary>
        /// Get all Citrix plans unless they are assigned to another company
        /// </summary>
        private void GetCitrixPlans()
        {
            try
            {
                List<CitrixPlan> plans = DbSql.Get_CitrixPlans(CPContext.SelectedCompanyCode);

                ddlCitrixApplications.Items.Clear();
                ddlCitrixApplications.Items.Add("--- Select One ---");

                if (plans != null)
                {
                    foreach (CitrixPlan p in plans)
                    {
                        ListItem item = new ListItem();
                        item.Text = p.DisplayName;
                        item.Value = p.PlanID.ToString();

                        ddlCitrixApplications.Items.Add(item);
                    }
                }
                else
                    notification1.SetMessage(controls.notification.MessageType.Warning, "No Citrix plans were found for your company.");
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error getting Citrix plans: " + ex.Message);
            }
        }

        /// <summary>
        /// Loads all users and sees if they are checked or not
        /// </summary>
        /// <param name="citrixPlanID"></param>
        private void LoadUsers(string citrixPlanID)
        {
            try
            {
                List<BaseUser> users = SQLCitrix.GetAssignedCitrixUsers(CPContext.SelectedCompanyCode, citrixPlanID);

                var sortedUsers = users.OrderBy(x => x.IsChecked ? 0 : 1).ThenBy(c => c.DisplayName);

                // Bind to repeater
                repeaterUsers.DataSource = sortedUsers;
                repeaterUsers.DataBind();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error getting users. Please contact support. Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Load the users and who currently has access or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCitrixApplications_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load the users
            LoadUsers(ddlCitrixApplications.SelectedValue);

            // Show panel
            panelUserList.Visible = true;
        }

        /// <summary>
        /// Save the information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            ADGroups groups = null;

            try
            {
                // Initialize
                groups = new ADGroups(Config.Username, Config.Password, Config.PrimaryDC);

                // Get application
                int ddlCitrixAppID = int.Parse(ddlCitrixApplications.SelectedValue);
                CitrixPlan citrixApp = DbSql.Get_CitrixPlan(ddlCitrixAppID);

                // Remove whitespace from Display Name and company the name of the group
                string displayName = citrixApp.DisplayName.Replace(" ", string.Empty);
                string groupName = string.Format("{0}@{1}", displayName, CPContext.SelectedCompanyCode.Replace(" ", string.Empty));

                // Make sure the name doesn't go beyond 64 characeters
                if (groupName.Length > 64)
                {
                    // See how much it is over
                    int overage = groupName.Length - 64;

                    // Take off that much
                    string[] split = groupName.Split('@');
                    string firstPart = split[0];
                    firstPart = groupName.Substring(0, groupName.Length - overage);

                    // Set the new name
                    groupName = string.Format("{0}@{1}", firstPart, split[1]);
                    this.logger.Debug("When evaluating to add users to the Citrix application " + citrixApp.DisplayName + ", we discovered the name of the security group would be beyong 64 characters. The new name is now: " + groupName);
                }

                // Create the group for the company
                if (!groups.DoesGroupExist(groupName))
                {
                    groups.Create(Retrieve.GetCompanyApplicationsOU, groupName, citrixApp.DisplayName, true, ADGroups.GroupType.Global, false);

                    // Join this group to the main group only if this application can be used for multiple
                    // companies.
                    if (string.IsNullOrEmpty(citrixApp.CompanyCode))
                    {
                        groups.ModifyMembership(citrixApp.GroupName, groupName, System.DirectoryServices.AccountManagement.IdentityType.Name, System.DirectoryServices.AccountManagement.IdentityType.Name, false, true);
                    }

                    // For group policy reasons we need to make sure we add this new group
                    // as a member of the AllTSUsers@ security group for the company
                    groups.ModifyMembership("AllTSUsers@" + CPContext.SelectedCompanyCode.Replace(" ", string.Empty), groupName, System.DirectoryServices.AccountManagement.IdentityType.Name,
                         System.DirectoryServices.AccountManagement.IdentityType.Name, false, true);
                }

                // Now loop through and see who needs to be added or removed
                Dictionary<string, string> users = new Dictionary<string, string>();

                // Loop through the repeater and find our users that need to be enabled
                foreach (RepeaterItem items in repeaterUsers.Items)
                {
                    if (items.ItemType == ListItemType.Item || items.ItemType == ListItemType.AlternatingItem)
                    {
                        var checkBox    = (CheckBox)items.FindControl("cbCitrixEnabledUsers");
                        var loginName   = (Label)items.FindControl("lbLoginName");

                        users.Add(loginName.Text, checkBox.Checked.ToString());
                    }
                }
                
                // Now loop through the users and add or remove them to the security group
                foreach (KeyValuePair<string, string> u in users)
                {
                    bool isAdding = bool.Parse(u.Value);

                    // Find the user in the database
                    ADUser foundUser = DbSql.Get_User(u.Key);

                    groups.ModifyMembership(groupName, foundUser.UserPrincipalName, System.DirectoryServices.AccountManagement.IdentityType.Name, 
                        System.DirectoryServices.AccountManagement.IdentityType.UserPrincipalName,
                        !isAdding); // We need to reverse the bool because True means to remove the user when False means to add the user

                    // Add to the SQL database
                    if (isAdding)
                    {
                        DbSql.Add_UserToPlan(citrixApp.PlanID, foundUser.UserID);
                    }
                    else
                    {
                        DbSql.Remove_UserFromPlan(citrixApp.PlanID, foundUser.UserID);
                    }
                }

                // Change selected index
                ddlCitrixApplications.SelectedIndex = 0;

                // Hide Panel
                panelUserList.Visible = false;

                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Success, "Successfully updated!");
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error: " + ex.Message);
            }
            finally
            {
                if (groups != null)
                    groups.Dispose();
            }
        }
    }
}