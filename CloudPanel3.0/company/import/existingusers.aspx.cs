using CloudPanel.classes;
using CloudPanel.Modules.ActiveDirectory;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Exchange;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;

namespace CloudPanel.company.import
{
    public partial class existingusers : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected int currentMailboxSize = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.IsSuperAdmin)
                Response.Redirect("~/login.aspx", true);
            else
            {
                if (!IsPostBack)
                {
                    // Check if the company is enabled for exchange or not
                    IsExchangeEnabled();

                    // Get Mailbox Plans
                    GetMailboxPlans();

                    // Get all users not currently in CloudPanel but found in AD
                    GetAllMissingUsers();
                }
            }
        }

        /// <summary>
        /// Checks if the user is enabled or not for Exchange
        /// IF not it will disable the checkbox for setting the Exchange plan
        /// </summary>
        private void IsExchangeEnabled()
        {
            try
            {
                bool isEnabled = SQLExchange.IsExchangeEnabled(CPContext.SelectedCompanyCode);

                if (isEnabled)
                {
                    cbUsersAlreadyEnabledExchange.Enabled = true;
                }
                else
                {
                    cbUsersAlreadyEnabledExchange.Checked = false;
                    cbUsersAlreadyEnabledExchange.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Unable to check if your company is enabled for Exchange: " + ex.Message);
            }

        }
        
        /// <summary>
        /// Check the user limit for importing
        /// </summary>
        /// <param name="add"></param>
        /// <returns></returns>
        private bool CheckUserLimit(int add = 0)
        {
            // Check if they are at the max or not
            try
            {
                bool isAtMax = SQLLimits.IsCompanyAtUserLimit(CPContext.SelectedCompanyCode, add);
                if (isAtMax)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "There was an error checking the limits: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Check the mailbox limit for importing
        /// </summary>
        /// <param name="add"></param>
        /// <returns></returns>
        private bool CheckMailboxLimit(int add = 0)
        {
            // Check if they are at the max or not
            try
            {
                bool isAtMax = SQLLimits.IsCompanyAtMailboxLimit(CPContext.SelectedCompanyCode, add);
                if (isAtMax)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "There was an error checking the limits: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets all users for this company from active directory
        /// and removes the ones that are already in CloudPanel
        /// </summary>
        private void GetAllMissingUsers()
        {
            ADUsers users = null;

            try
            {
                // Clear datasource
                repeater.DataSource = null;
                repeater.DataBind();

                // Get a list of current users so we do not show those
                List<BaseUser> currentUsers = SQLUsers.GetUsers(CPContext.SelectedCompanyCode);

                // Filter out the list of current users to get the UserPrincipalNames only
                var upns = new List<string>(currentUsers.Select(x => x.UserPrincipalName));

                // Initialize our users class
                users = new ADUsers(Config.Username, Config.Password, Config.PrimaryDC);

                // Find all users from the OU but exclude the existing ones
                List<ADUser> foundUsers = users.GetAllUsers(Retrieve.GetCompanyUsersOU, upns, false);
                if (foundUsers != null && foundUsers.Count > 0)
                {
                    // Bind to our repeater
                    repeater.DataSource = foundUsers;
                    repeater.DataBind();
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error getting users: " + ex.ToString() + "|" + Retrieve.GetCompanyUsersOU);
            }
            finally
            {
                if (users != null)
                    users.Dispose();
            }
        }

        /// <summary>
        /// Gets a list of mailbox plans for this company and excludes plans that do not belong to company
        /// </summary>
        private void GetMailboxPlans()
        {
            List<MailboxPlan> plans = null;

            try
            {
                plans = SQLPlans.GetMailboxPlans();

                // Now loop through and take out plans that don't belong to this company
                plans.RemoveAll(x => (!string.IsNullOrEmpty(x.CompanyCode) && !x.CompanyCode.Equals(CPContext.SelectedCompanyCode)));

                // Clear and add blank
                ddlMailboxPlans.Items.Clear();

                // Add rest of plans
                foreach (MailboxPlan b in plans)
                {
                    ListItem item = new ListItem();
                    item.Value = b.PlanID.ToString();
                    item.Text = b.DisplayName;
                    item.Attributes.Add("Description", b.Description);
                    item.Attributes.Add("Min", b.SizeInMB.ToString());
                    item.Attributes.Add("Max", b.MaximumSizeInMB.ToString());
                    item.Attributes.Add("Price", b.Price);
                    item.Attributes.Add("Extra", b.AdditionalGBPrice);

                    ddlMailboxPlans.Items.Add(item);
                }

                // Select first one
                ddlMailboxPlans.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error retrieving mailbox plans: " + ex.Message);
            }
            finally
            {
                plans = null;
            }
        }

        /// <summary>
        /// Gets a list of users that are selected
        /// </summary>
        /// <returns></returns>
        private string[] GetSelectedUsers()
        {
            List<string> userPrincipalNames = new List<string>();

            // Loop through the repeater
            foreach (RepeaterItem items in repeater.Items)
            {
                if (items.ItemType == ListItemType.Item || items.ItemType == ListItemType.AlternatingItem)
                {
                    var checkBox = (CheckBox)items.FindControl("cbImportUsers");
                    var upn = (Label)items.FindControl("lbUPN");

                    if (checkBox.Checked)
                    {
                        // User was selected. Add to list
                        userPrincipalNames.Add(upn.Text.Trim());

                        // DEBUG //
                        this.logger.Debug("User " + upn + " was selected to be imported into CloudPanel.");
                    }
                }
            }

            if (userPrincipalNames.Count > 0)
                return userPrincipalNames.ToArray();
            else
                return null; // Return null if nothing was selected
        }

        /// <summary>
        /// Starts the import process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImportSave_Click(object sender, EventArgs e)
        {
            ADUsers adUsers = null;
            ADGroups adGroups = null;

            try
            {
                string companyCode = CPContext.SelectedCompanyCode; // Get the current company code that is selected

                string[] selectedUsers = GetSelectedUsers();
                if (selectedUsers != null && selectedUsers.Length > 0)
                {
                    // Check the limits
                    if (CheckUserLimit(selectedUsers.Length))
                        throw new Exception("You have selected too many users and it would have caused you to go over the user limit set for your company. Please select fewer users and try again.");

                    // Check mailbox limits if checkbox is checked
                    if (cbUsersAlreadyEnabledExchange.Checked && CheckMailboxLimit(selectedUsers.Length))
                        throw new Exception("You have selected too many users that have mailboxes and it would have caused you to go over the mailbox limit set for your company. Please select fewer users and try again.");

                    // Initialize our Active Directory object
                    adUsers = new ADUsers(Config.Username, Config.Password, Config.PrimaryDC);
                    adGroups = new ADGroups(Config.Username, Config.Password, Config.PrimaryDC);

                    // Pull detailed information from ActiveDirectory on the selected users
                    List<ADUser> adUsersInfo = adUsers.GetAllUsers(Retrieve.GetCompanyOU, selectedUsers.ToList(), true);

                    // Validate the domains for the users login matches a domain in this company
                    ValidateDomains(adUsersInfo);

                    // Now insert into SQL
                    foreach (ADUser u in adUsersInfo)
                    {
                        // Set the company code of the user because it won't have it
                        u.CompanyCode = companyCode;

                        // Make sure the user belongs to the AllUsers@ group
                        adGroups.ModifyMembership("AllUsers@" + companyCode.Replace(" ", string.Empty), u.UserPrincipalName, System.DirectoryServices.AccountManagement.IdentityType.Name, System.DirectoryServices.AccountManagement.IdentityType.UserPrincipalName, false, false);

                        SQLUsers.AddUser(u);
                    }

                    // Now check if we are doing mailboxes
                    if (cbUsersAlreadyEnabledExchange.Checked)
                    {
                        ImportExchangeUsers(adUsersInfo);
                    }
                }

                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Success, "Successfully imported users.");
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error importing users: " + ex.Message);
            }
            finally
            {
                if (adGroups != null)
                    adGroups.Dispose();

                if (adUsers != null)
                    adUsers.Dispose();
            }

            // Retrieve a new list of useres
            GetAllMissingUsers();
        }

        /// <summary>
        /// Sets the user Exchange information
        /// </summary>
        /// <param name="adUsersInfo"></param>
        private void ImportExchangeUsers(List<ADUser> adUsersInfo)
        {
            ExchCmds powershell = null;

            try
            {
                // Get the plan that was selected
                MailboxPlan selectedPlan = SQLPlans.GetMailboxPlan(int.Parse(ddlMailboxPlans.SelectedValue));

                // Get the selected mailbox size based on the slider
                int selectedSize = int.Parse(hfMailboxSizeMB.Value);

                // Calculate the additional MB that was added
                int totalAdded = selectedSize - selectedPlan.SizeInMB;

                // Initialize our powershell object
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // Now we need to loop through and set the mailbox information
                foreach (ADUser u in adUsersInfo)
                {
                    // Set the mailbox information
                    powershell.Set_Mailbox(u.UserPrincipalName, CPContext.SelectedCompanyCode, selectedPlan);

                    // Update SQL
                    SQLUsers.UpdateUserMailbox(u, selectedPlan);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Validates that the domains for the user match a domain in this company
        /// </summary>
        /// <param name="adUsersInfo"></param>
        private void ValidateDomains(List<ADUser> adUsersInfo)
        {
            try
            {
                // Get a list of domains
                List<string> domains = SQLCompanies.GetCompanyDomains(CPContext.SelectedCompanyCode);

                // Now select any users where domain doesn't match
                var users = from a in adUsersInfo
                            where !domains.Any(d => d.ToLower().Equals(a.UserPrincipalName.ToLower().Split('@')[1]))
                            select a;

                if (users != null && users.ToList().Count > 0)
                {
                    // Get a list of user principal names
                    var upn = from u in users
                              select u.UserPrincipalName;

                    throw new Exception("Cannot import the following users because their domain doesn't match a domain belonging to this company: " + String.Join(", ", upn));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}