using CloudPanel.Modules.ActiveDirectory;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Exchange;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;

namespace CloudPanel.company.exchange
{
    public partial class mailboxes : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private List<BaseEmailAliases> emailAliases;
        protected int currentMailboxSize = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Get a list of mailbox users
                GetMailboxUsers();

                // Get a list of activesync policies
                GetActiveSyncPlans();
                
                // Get a list of domains
                GetAcceptedDomains();

                // Get a list of mailbox plans
                GetMailboxPlans();

                // Get a list of mailbox databases
                GetExchangeDatabases();

                // Generate new email alias list
                emailAliases = new List<BaseEmailAliases>();

                // Add to view state
                ViewState["CPEmailAliases"] = emailAliases;
            }
            else
            {
                if (panelEditMailbox.Visible)
                {
                    // Get E-mail Aliases
                    emailAliases = (List<BaseEmailAliases>)ViewState["CPEmailAliases"];

                    // Bind e-mail aliases to grid view
                    gridEmailAliases.DataSource = emailAliases;
                    gridEmailAliases.DataBind();
                }
            }

            // Check out limits
            CheckLimit(0);
        }

        /// <summary>
        /// Checks the limits and gives option of returning value
        /// </summary>
        /// <param name="add"></param>
        /// <returns></returns>
        private bool CheckLimit(int add = 0)
        {
            // Check if they are at the max or not
            try
            {
                bool isAtMax = SQLLimits.IsCompanyAtMailboxLimit(CPContext.SelectedCompanyCode, add);
                if (isAtMax)
                {
                    btnAddUserMailboxes.Enabled = false;
                    return true;
                }
                else
                {
                    btnAddUserMailboxes.Enabled = true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                btnAddUserMailboxes.Enabled = false;

                // Return false
                return false;
            }
        }

        #region Populating Data

        /// <summary>
        /// Gets a list of accepted domains from the database
        /// </summary>
        private void GetAcceptedDomains()
        {
            try
            {
                DataTable dt = SqlLibrary.ReadSql("SELECT * FROM Domains WHERE CompanyCode=@CompanyCode AND IsAcceptedDomain=1 ORDER BY Domain",
                    new SqlParameter("CompanyCode", CPContext.SelectedCompanyCode));

                // Create our data view
                DataView dw = dt.AsDataView();

                // Bind to our objects
                ddlDomains.DataSource = dw;
                ddlDomains.DataBind();

                ddlEditPrimaryEmailDomain.DataSource = dw;
                ddlEditPrimaryEmailDomain.DataBind();

                ddlAddEmailAliasDomain.DataSource = dw;
                ddlAddEmailAliasDomain.DataBind();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error getting a list of accepted domains: " + ex.Message);
            }
        }

        /// <summary>
        /// Get all users already enabled for mailboxes
        /// </summary>
        private void GetMailboxUsers()
        {
            try
            {
                // Change the panel
                ShowPanel(panelMailboxes);

                // Get list of current mailboxu sers
                List<MailboxUser> mailboxUsers = ExchangeSql.Get_MailboxUsersForCompany(CPContext.SelectedCompanyCode);

                //
                // Bind to repeater
                //
                repeaterMailboxes.DataSource = mailboxUsers;
                repeaterMailboxes.DataBind();

                //
                // Bind to our full access and send as group but only if they have a sAMAccountName
                //
                var validUsers = (from u in mailboxUsers
                                  where !string.IsNullOrEmpty(u.SamAccountName)
                                  select u).ToList();

                lstFullAccessPermissions.DataSource = validUsers;
                lstFullAccessPermissions.DataBind();

                lstSendAsPermissions.DataSource = validUsers;
                lstSendAsPermissions.DataBind();

            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error getting list of mailbox users: " + ex.ToString());
            }
        }

        /// <summary>
        /// Get all users not enabled for mailboxes
        /// </summary>
        private void GetNonMailboxUsers()
        {
            try
            {
                List<ADUser> users = ExchangeSql.Get_NonMailboxUsersForCompany(CPContext.SelectedCompanyCode);

                // Show the correct panel
                ShowPanel(panelEnableUsers);

                //
                // Bind to repeater
                //
                repeaterNonMailboxUsers.DataSource = users;
                repeaterNonMailboxUsers.DataBind();

            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.ToString());
            }
        }

        /// <summary>
        /// Gets a list of mailbox plans for this company and excludes plans that do not belong to company
        /// </summary>
        private void GetMailboxPlans()
        {
            try
            {
                List<MailboxPlan> plans = DbSql.Get_MailboxPlans(CPContext.SelectedCompanyCode);

                // Clear and add blank
                ddlEnableMailboxPlans.Items.Clear();
                ddlEditMailboxPlan.Items.Clear();

                // Add rest of plans
                foreach (MailboxPlan p in plans)
                {
                    ListItem item = new ListItem();
                    item.Value = p.PlanID.ToString();
                    item.Text = p.DisplayName;
                    item.Attributes.Add("Description", p.Description);
                    item.Attributes.Add("Min", p.SizeInMB.ToString());
                    item.Attributes.Add("Max", p.MaximumSizeInMB.ToString());
                    item.Attributes.Add("Price", p.Price);
                    item.Attributes.Add("Extra", p.AdditionalGBPrice);

                    ddlEnableMailboxPlans.Items.Add(item);
                    ddlEditMailboxPlan.Items.Add(item);
                }

                // Select first one
                ddlEnableMailboxPlans.SelectedIndex = 0;
                ddlEditMailboxPlan.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error getting mailbox plans: " + ex.Message);
            }
        }
        
        /// <summary>
        /// Gets a list of activesync plans from the database
        /// </summary>
        private void GetActiveSyncPlans()
        {
            // Clear list
            ddlActiveSyncPlan.Items.Clear();
            ddlActiveSyncPlan.Items.Add(new ListItem("None", "0"));

            ddlActiveSyncPlanEditMailbox.Items.Clear();
            ddlActiveSyncPlanEditMailbox.Items.Add(new ListItem("None", "0"));

            try
            {
                List<BaseActivesyncPolicy> policies = SQLExchange.GetExchangeActiveSyncPolicies();

                // Add to our drop down list
                foreach (BaseActivesyncPolicy tmp in policies)
                {
                    ListItem item = new ListItem();
                    item.Text = tmp.DisplayName;
                    item.Value = tmp.ASID.ToString();

                    ddlActiveSyncPlan.Items.Add(item);
                    ddlActiveSyncPlanEditMailbox.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                this.logger.Error("Unable to get active sync plans from database.", ex);
            }
        }

        /// <summary>
        /// Gets a list of forwarding addresses
        /// </summary>
        private void GetForwardingAddresses()
        {
            try
            {
                ddlForwardTo.Items.Clear();
                ddlForwardTo.Items.Add("None");

                List<ForwardingObject> forwardingObjects = new List<ForwardingObject>();

                // Get the list of mailbox users
                logger.DebugFormat("Getting list of mailbox users");
                DataTable dt = SqlLibrary.ReadSql("SELECT DisplayName,DistinguishedName FROM Users WHERE MailboxPlan>0 AND CompanyCode=@CompanyCode", new SqlParameter("CompanyCode", CPContext.SelectedCompanyCode));
                if (dt != null)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        forwardingObjects.Add(new ForwardingObject()
                        {
                            DisplayName = r["DisplayName"].ToString(),
                            DistinguishedName = r["DistinguishedName"].ToString(),
                            ObjectType = "Users"
                        });
                    }
                }

                // Get the list of Exchange contacts
                logger.DebugFormat("Getting list of exchange contacts");
                dt = SqlLibrary.ReadSql("SELECT DisplayName,DistinguishedName FROM Contacts WHERE CompanyCode=@CompanyCode", new SqlParameter("CompanyCode", CPContext.SelectedCompanyCode));
                if (dt != null)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        forwardingObjects.Add(new ForwardingObject()
                        {
                            DisplayName = r["DisplayName"].ToString(),
                            DistinguishedName = r["DistinguishedName"].ToString(),
                            ObjectType = "Contacts"
                        });
                    }
                }


                // Get the list of distribution groups
                logger.DebugFormat("Getting list of exchange groups");
                dt = SqlLibrary.ReadSql("SELECT DisplayName,DistinguishedName FROM Contacts WHERE CompanyCode=@CompanyCode", new SqlParameter("CompanyCode", CPContext.SelectedCompanyCode));
                if (dt != null)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        forwardingObjects.Add(new ForwardingObject()
                        {
                            DisplayName = r["DisplayName"].ToString(),
                            DistinguishedName = r["DistinguishedName"].ToString(),
                            ObjectType = "DistributionGroups"
                        });
                    }
                }

                // Add to our drop down list
                if (forwardingObjects != null)
                {
                    foreach (ForwardingObject bfi in forwardingObjects)
                    {
                        ListItem item = new ListItem();
                        item.Text = bfi.DisplayName;
                        item.Value = bfi.CanonicalName;
                        item.Attributes.Add("Classification", bfi.ObjectType);

                        ddlForwardTo.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting forwarding objects: {0}", ex.ToString());
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Gets a list of Exchange databases that they can choose from when enabling
        /// a mailbox user. Only Super Admins can do this
        /// </summary>
        private void GetExchangeDatabases()
        {
            List<string> databases = Config.ExchangeDatabases;

            ddlExchangeDatabases.Items.Clear();
            ddlExchangeDatabases.Items.Add("Auto");

            if (databases != null)
            {
                foreach (string d in databases)
                {
                    ddlExchangeDatabases.Items.Add(d);
                }
            }

            // Select the first value which is 'Auto'
            ddlExchangeDatabases.SelectedIndex = 0;
        }

        #endregion

        #region Disable User

        protected void btnDisableYes_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hfDisableUserPrincipalName.Value))
            {
                ExchCmds powershell = null;

                try
                {
                    powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                    // Disable the mailbox
                    powershell.Disable_Mailbox(hfDisableUserPrincipalName.Value);

                    // Delete from SQL
                    SQLMailboxes.DisableMailbox(hfDisableUserPrincipalName.Value, CPContext.SelectedCompanyCode);

                    // Notify
                    notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.NotificationSuccessMailboxDisable + hfDisableUserPrincipalName.Value);

                    // Wipe
                    hfDisableUserPrincipalName.Value = null;
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
                }
                finally
                {
                    if (powershell != null)
                        powershell.Dispose();

                    // Go back to mailbox user view
                    GetMailboxUsers();
                }
            }
        }

        protected void btnDisableCancel_Click(object sender, EventArgs e)
        {
            // Go back to mailbox user view
            GetMailboxUsers();
        }

        #endregion

        #region Edit Mailbox

        private void EditMailbox(string userPrincipalName, string mailboxPlanName)
        {
            // Hide all panels except edit panel
            panelDisableMailbox.Visible = false;
            panelEnableUsers.Visible = false;
            panelMailboxes.Visible = false;

            // Show edit panel
            panelEditMailbox.Visible = true;

            // Now get the mailbox information
            ExchCmds powershell = null;
            try
            {
                // Refresh plans again otherwise the jQuery won't work
                GetMailboxPlans();

                // Get list of forwarding addresses
                GetForwardingAddresses();

                // Initilize 
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // Get user object
                MailboxUser user = powershell.Get_Mailbox(userPrincipalName);

                // Set plan information
                ddlEditMailboxPlan.SelectedIndex = ddlEditMailboxPlan.Items.IndexOf(ddlEditMailboxPlan.Items.FindByText(mailboxPlanName));

                // Populate information
                hfUserPrincipalName.Value = userPrincipalName;
                hfDistinguishedName.Value = user.DistinguishedName;
                txtDisplayName.Text = user.DisplayName;
                txtEditPrimaryEmail.Text = user.PrimarySmtpAddress.Split('@')[0];
                ddlEditPrimaryEmailDomain.SelectedValue = user.PrimarySmtpAddress.Split('@')[1];
                cbDeliverToMailboxAndFoward.Checked = user.DeliverToMailboxAndForward;

                // Get activesync policy
                if (string.IsNullOrEmpty(user.ActiveSyncMailboxPolicy))
                    ddlActiveSyncPlanEditMailbox.SelectedIndex = 0;
                else
                {
                    ListItem item = ddlActiveSyncPlanEditMailbox.Items.FindByText(user.ActiveSyncMailboxPolicy);
                    if (item != null)
                        ddlActiveSyncPlanEditMailbox.SelectedIndex = ddlActiveSyncPlanEditMailbox.Items.IndexOf(item);
                    else
                        ddlActiveSyncPlanEditMailbox.SelectedIndex = 0;
                }

                //
                // Populate any forwarding address
                //
                if (!string.IsNullOrEmpty(user.ForwardingAddress))
                {
                    string upper = user.ForwardingAddress.ToUpper();

                    var item = ddlForwardTo.Items.Cast<ListItem>().Where(i => i.Value.ToUpper() == upper).First();
                    if (item != null)
                        ddlForwardTo.SelectedValue = item.Value;
                }
                else
                    ddlForwardTo.SelectedIndex = 0;

                // Set current mailbox size
                currentMailboxSize = user.MailboxSizeInMB;

                //
                // Set the email aliases
                //
                txtAddEmailAlias.Text = string.Empty;
                emailAliases = new List<BaseEmailAliases>();
                if (user.EmailAliases != null)
                {
                    foreach (string email in user.EmailAliases)
                    {
                        emailAliases.Add(new BaseEmailAliases() { emailAddress = email });
                    }
                }

                // Add to ViewState
                ViewState["CPEmailAliases"] = emailAliases;


                //
                // Populate the mailbox permissions for FullAccess
                //
                hfFullAccessOriginal.Value = string.Empty;
                foreach (MailboxPermissions m in user.FullAccessPermissions)
                {
                    ListItem item = lstFullAccessPermissions.Items.FindByValue(m.SamAccountName);
                    if (item != null)
                    {
                        int index = lstFullAccessPermissions.Items.IndexOf(item);
                        lstFullAccessPermissions.Items[index].Selected = true;

                        // Add to our hidden value field for a list of original values
                        hfFullAccessOriginal.Value += m.SamAccountName + ",";
                    }
                }
                this.logger.Debug("Full access permissions for " + user.UserPrincipalName + " when loaded is: " + hfFullAccessOriginal.Value);

                //
                // Populate the mailbox permissions for SendAs
                //
                hfSendAsOriginal.Value = string.Empty;
                foreach (MailboxPermissions m in user.SendAsPermissions)
                {
                    ListItem item = lstSendAsPermissions.Items.FindByValue(m.SamAccountName);
                    if (item != null)
                    {
                        int index = lstSendAsPermissions.Items.IndexOf(item);
                        lstSendAsPermissions.Items[index].Selected = true;

                        // Add to our hidden value field for a list of original values
                        hfSendAsOriginal.Value += m.SamAccountName + ",";
                    }
                }
                this.logger.Debug("Send-As permissions for " + user.UserPrincipalName + " when loaded is: " + hfSendAsOriginal.Value);

                // Bind gridview
                gridEmailAliases.DataSource = emailAliases;
                gridEmailAliases.DataBind();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.ToString());

                // Reset view
                GetMailboxUsers();
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Adds an alias to the gridview and rebinds the grid view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnInsertEmailAlias_Click(object sender, EventArgs e)
        {
            // Make sure the values are not null
            if (string.IsNullOrEmpty(txtAddEmailAlias.Text) || string.IsNullOrEmpty(ddlAddEmailAliasDomain.SelectedValue))
                return;

            string enteredEmail = (Server.HtmlEncode(txtAddEmailAlias.Text) + "@" + ddlAddEmailAliasDomain.SelectedValue).Replace(" ", string.Empty);

            // Make sure it doesn't already exist
            if (emailAliases.Find(x => x.emailAddress.Equals(enteredEmail, StringComparison.CurrentCultureIgnoreCase)) != null)
                lbAddAliasError.Text = Resources.LocalizedText.EmailAliasAlreadyExists + enteredEmail;
            else
            {
                // Add to our list
                emailAliases.Add(new BaseEmailAliases()
                {
                    emailAddress = enteredEmail
                });

                // Read to view state
                ViewState["CPEmailAliases"] = emailAliases;

                // Rebind grid view
                gridEmailAliases.DataBind();

                // Clear errors
                lbAddAliasError.Text = string.Empty;
            }
        }

        /// <summary>
        /// Prerenders the grid view so it will be compatible with the DataTables jQuery plugin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridEmailAliases_PreRender(object sender, EventArgs e)
        {
            if (gridEmailAliases.Rows.Count > 0)
            {
                gridEmailAliases.UseAccessibleHeader = true;
                gridEmailAliases.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridEmailAliases_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                string selectedEmail = e.CommandArgument.ToString();

                // Remove from our list
                emailAliases.RemoveAll(x => x.emailAddress.Equals(selectedEmail, StringComparison.CurrentCultureIgnoreCase));

                // Set view state
                ViewState["CPEmailAliases"] = emailAliases;

                // Rebind
                gridEmailAliases.DataBind();

            }
        }

        /// <summary>
        /// Removes an alias from the grid view and rebinds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridEmailAliases_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            TableCell cell = gridEmailAliases.Rows[e.RowIndex].Cells[0];

            // Remove from our list
            emailAliases.RemoveAll(x => x.emailAddress.Equals(cell.Text, StringComparison.CurrentCultureIgnoreCase));

            // Set view state
            ViewState["CPEmailAliases"] = emailAliases;

            // Rebind
            gridEmailAliases.DataBind();
        }

        /// <summary>
        /// Cancels and goes back to the main Mailboxes screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEditMailboxCancel_Click(object sender, EventArgs e)
        {
            // User clicked cancel on a mailbox. Revert back to main screen
            GetMailboxUsers();
        }

        /// <summary>
        /// Saves a mailbox after it has been edited
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEditMailboxSave_Click(object sender, EventArgs e)
        {
            ExchCmds powershell = null;

            try
            {
                // Initialize powershell
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // Get mailbox plan
                MailboxPlan selectedPlan = SQLPlans.GetMailboxPlan(int.Parse(ddlEditMailboxPlan.SelectedValue));

                // Initialize our collection
                MailboxUser user = new MailboxUser();
                user.UserPrincipalName = hfUserPrincipalName.Value;
                user.DistinguishedName = hfDistinguishedName.Value;
                user.PrimarySmtpAddress = string.Format("{0}@{1}", txtEditPrimaryEmail.Text.Replace(" ", string.Empty), ddlEditPrimaryEmailDomain.SelectedValue);
                user.DeliverToMailboxAndForward = cbDeliverToMailboxAndFoward.Checked;
                user.ActiveSyncMailboxPolicy = ddlActiveSyncPlanEditMailbox.SelectedIndex == 0 ? null : ddlActiveSyncPlanEditMailbox.SelectedItem.Text;

                // Get our forwrading address
                if (ddlForwardTo.SelectedIndex > 0)
                    user.ForwardingAddress = ddlForwardTo.SelectedValue;

                // Get our list of email aliases
                user.EmailAliases = new List<string>();
                if (emailAliases != null && emailAliases.Count > 0)
                {
                    foreach (BaseEmailAliases email in emailAliases)
                    {
                        user.EmailAliases.Add(email.emailAddress);
                    }
                }

                // Get the selected mailbox size based on the slider
                int selectedSize = int.Parse(hfEditMailboxSize.Value);
                int totalAdded = selectedSize - selectedPlan.SizeInMB;
                selectedPlan.SetSizeInMB = selectedSize;

                // Get the permissions and see if anything has changed or not
                GetChangedMailboxPermissions(ref user);


                //
                // Check plan features or if the plan was overridden
                //
                user.ActiveSyncEnabled = cbOverrideOptions.Checked ? cbEnableActiveSync.Checked : selectedPlan.ActiveSyncEnabled;
                user.ECPEnabled = cbOverrideOptions.Checked ? cbEnableECP.Checked : selectedPlan.ECPEnabled;
                user.IMAPEnabled = cbOverrideOptions.Checked ? cbEnableIMAP.Checked : selectedPlan.IMAPEnabled;
                user.MAPIEnabled = cbOverrideOptions.Checked ? cbEnableMAPI.Checked : selectedPlan.MAPIEnabled;
                user.OWAEnabled = cbOverrideOptions.Checked ? cbEnableOWA.Checked : selectedPlan.OWAEnabled;
                user.POP3Enabled = cbOverrideOptions.Checked ? cbEnablePOP3.Checked : selectedPlan.POP3Enabled;

                // Update the mailbox
                powershell.Set_Mailbox(user, selectedPlan);

                // Update mailbox in SQL
                SQLUsers.UpdateUserMailbox(user, selectedPlan);

                // Set notification
                notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.NotificationSuccessMailboxUpdate + user.UserPrincipalName);
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();

                // Revert to main screen
                GetMailboxUsers();
            }
        }

        /// <summary>
        /// Gets if any changes were made to the full access or send as permissions
        /// </summary>
        /// <param name="user"></param>
        private void GetChangedMailboxPermissions(ref MailboxUser user)
        {
            //
            // Figure out the full access permissions
            //
            List<string> fullAccessOriginal = hfFullAccessOriginal.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            user.FullAccessPermissions = new List<MailboxPermissions>();
            foreach (ListItem li in lstFullAccessPermissions.Items)
            {
                // Check if the item is selected first, then we need to see if they were added
                if (li.Selected && !fullAccessOriginal.Contains(li.Value))
                {
                    // Permission was added
                    user.FullAccessPermissions.Add(new MailboxPermissions()
                    {
                        Add = true,
                        SamAccountName = li.Value
                    });
                }
            }

            // Now see if any were removed
            foreach (string s in fullAccessOriginal)
            {
                // Check if the item exist in the original but isn't selected in the listbox. That means they were removed
                if (!lstFullAccessPermissions.Items.FindByValue(s).Selected)
                {
                    // Permission was removed
                    user.FullAccessPermissions.Add(new MailboxPermissions()
                    {
                        Add = false,
                        SamAccountName = s
                    });
                }
            }


            //
            // Figure out the send as permissions
            //
            List<string> sendAsOriginal = hfSendAsOriginal.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            user.SendAsPermissions = new List<MailboxPermissions>();
            foreach (ListItem li in lstSendAsPermissions.Items)
            {
                // Check if the item is selected first, then we need to see if they were added
                if (li.Selected && !sendAsOriginal.Contains(li.Value))
                {
                    // Permission was added
                    user.SendAsPermissions.Add(new MailboxPermissions()
                    {
                        Add = true,
                        SamAccountName = li.Value
                    });
                }
            }

            // Now see if any were removed
            foreach (string s in sendAsOriginal)
            {
                // Check if the item exists in the original but isn't selected in the listbox. That means they were removed
                if (!lstSendAsPermissions.Items.FindByValue(s).Selected)
                {
                    // Permission was removed
                    user.SendAsPermissions.Add(new MailboxPermissions()
                    {
                        Add = false,
                        SamAccountName = s
                    });
                }
            }
        }

        /// <summary>
        /// Cancels and goes back to the mailbox users screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnableUsersCancel_Click(object sender, EventArgs e)
        {
            GetMailboxUsers();
        }

        #endregion

        /// <summary>
        /// For commands such as edit or delete in the repeater
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void repeaterMailboxes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DisableMailbox")
            {
                string[] split = e.CommandArgument.ToString().Split(';');

                // Set information
                lbDisableDisplayName.Text = split[0];
                hfDisableUserPrincipalName.Value = split[1];

                // Show and hide panels
                panelDisableMailbox.Visible = true;
                panelEditMailbox.Visible = false;
                panelEnableUsers.Visible = false;
                panelMailboxes.Visible = false;
            }
            else if (e.CommandName == "EditMailbox")
            {
                // Split the string because we pass both the UserPrincipalName AND the Mailbox Plan
                string[] splitValue = e.CommandArgument.ToString().Split('|');

                // Populate information from Exchange
                EditMailbox(splitValue[0], splitValue[1]);
            }
        }

        /// <summary>
        /// Button to start creating mailboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnableUsers_Click(object sender, EventArgs e)
        {
            // Check the limits
            List<MailboxUser> selectedNewUsers = GetUsersToEnable();
            if (CheckLimit(selectedNewUsers.Count))
            {
                // Set notification
                notification1.SetMessage(controls.notification.MessageType.Warning, Resources.LocalizedText.NotificationWarningMailboxTooManyUsers);

                // Refresh view
                GetMailboxUsers();
            }

            // Dispose of list
            selectedNewUsers = null;

            // Start enabling the new mailboxes
            EnableNewMailboxes(CPContext.SelectedCompanyCode);
        }

        /// <summary>
        /// Begins the process of creating the mailboxes
        /// </summary>
        private void EnableNewMailboxes(string companyCode)
        {
            ExchCmds powershell = null;

            try
            {
                // Initialize powershell and sql
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                //
                // Get all the users and validate they selected a user and it won't put the company over their allowed limit
                //
                List<MailboxUser> usersToEnable = GetUsersToEnable();
                if (usersToEnable.Count < 1)
                    throw new Exception("You must select at least one user to enable for a mailbox. You selected zero users.");
                if (SQLLimits.IsCompanyAtMailboxLimit(companyCode, usersToEnable.Count))
                    throw new Exception("You have selected too many users that would have caused your company to have more mailboxes that your company is allowed to have. Selected fewer users and try again.");

                //
                // Get the mailbox plan
                //
                int planId = int.Parse(ddlEnableMailboxPlans.SelectedValue);
                int setSizeInMB = int.Parse(hfMailboxSizeMB.Value);
                                
                MailboxPlan plan = DbSql.Get_MailboxPlan(planId);
                plan.SetSizeInMB = setSizeInMB; // set the size that the user selected



                //
                // Format all the users email addresses based on what they selected
                //
                FormatAllUsersEmail(usersToEnable, ddlDomains.SelectedValue);


                // A string of all our problem email accounts that gave an error when enabling
                string errorEnabling = string.Empty;


                // Now loop through each user
                foreach (MailboxUser user in usersToEnable)
                {
                    if (string.IsNullOrEmpty(user.PrimarySmtpAddress.Split('@')[0]))
                        errorEnabling += user.DisplayName + ", ";
                    else
                    {
                        user.CompanyCode             = companyCode;
                        user.ActiveSyncMailboxPolicy = ddlActiveSyncPlan.SelectedIndex > 0 ? ddlActiveSyncPlan.SelectedItem.Text : null;
                        user.Database                = ddlExchangeDatabases.SelectedIndex > 0 ? ddlExchangeDatabases.SelectedValue : string.Empty;
                        

                        // Enabling the mailbox
                        powershell.Enable_Mailbox(user, plan);

                        //
                        // Insert into SQL
                        //
                        DbSql.Update_UserMailboxInfo(user.UserPrincipalName, plan.PlanID, user.PrimarySmtpAddress, (setSizeInMB - plan.SizeInMB), user.ActiveSyncMailboxPolicy);
                        
                        //
                        // Add to the queue to modify the calendar permissions 10 minutes after it has been enabled
                        //
                        DbSql.Add_DatabaseQueue(Enumerations.TaskType.MailboxCalendarPermissions, user.UserPrincipalName, user.CompanyCode, 10, Enumerations.TaskSuccess.NotStarted);
                        
                    }
                }

                // Update the notification
                if (!string.IsNullOrEmpty(errorEnabling))
                    notification1.SetMessage(controls.notification.MessageType.Warning, "Error enabling the following users because of missing data (normally first or last name): " + errorEnabling);
                else
                    notification1.SetMessage(controls.notification.MessageType.Success, "Successfully enabled " + usersToEnable.Count.ToString() + " user(s).");

            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                // Refresh back to the main screen
                GetMailboxUsers();
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();

                // Refresh the view even if there is an error
                GetMailboxUsers();
            }
        }

        /// <summary>
        /// Goes to the add users mailbox screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddUserMailboxes_Click(object sender, EventArgs e)
        {
            // Get list of non mailbox users
            GetNonMailboxUsers();

            // Get a list of mailbox plans again so we can bind the attributes to the drop down list
            GetMailboxPlans();
        }

        #region Common

        /// <summary>
        /// Hides all panels then shows the correct one
        /// </summary>
        /// <param name="panel"></param>
        private void ShowPanel(Panel panel)
        {
            // Hide all panels
            panelDisableMailbox.Visible = false;
            panelEditMailbox.Visible = false;
            panelEnableUsers.Visible = false;
            panelMailboxes.Visible = false;

            if (panel == panelMailboxes)
                panelMailboxes.Visible = true;
            else if (panel == panelEnableUsers)
                panelEnableUsers.Visible = true;
            else if (panel == panelEditMailbox)
                panelEditMailbox.Visible = true;
            else if (panel == panelDisableMailbox)
                panelDisableMailbox.Visible = true;

        }

        /// <summary>
        /// Formats all the users email 
        /// </summary>
        /// <param name="users"></param>
        /// <param name="domainName"></param>
        /// <returns></returns>
        private List<MailboxUser> FormatAllUsersEmail(List<MailboxUser> users, string domainName)
        {
            // Loop through each object and format the users email for them
            foreach (MailboxUser user in users)
            {
                // Format the users email
                user.PrimarySmtpAddress = FormatUserEmail(user, domainName);
            }

            // Return the collections
            return users;
        }

        /// <summary>
        /// Formarts a single user email address
        /// </summary>
        /// <param name="user"></param>
        /// <param name="domainName"></param>
        /// <returns></returns>
        private string FormatUserEmail(MailboxUser user, string domainName)
        {
            // Remove spaces
            string firstName = "", lastName = "";

            if (!string.IsNullOrEmpty(user.FirstName))
                firstName = user.FirstName.Replace(" ", string.Empty);

            if (!string.IsNullOrEmpty(user.LastName))
                lastName = user.LastName.Replace(" ", string.Empty);

            // Validate the values are correct
            if ((string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName)) && !rbFormatOther.Checked)
                throw new Exception(Resources.LocalizedText.NotificationErrorMailboxCannotEnable1 + user.UserPrincipalName);

            if (string.IsNullOrEmpty(firstName) && (!rbFormatOther.Checked && !rbFormatLastName.Checked))
                throw new Exception(Resources.LocalizedText.NotificationErrorMailboxCannotEnable1 + user.UserPrincipalName);

            if (string.IsNullOrEmpty(lastName) && (!rbFormatOther.Checked && !rbFormatFirstName.Checked))
                throw new Exception(Resources.LocalizedText.NotificationErrorMailboxCannotEnable1 + user.UserPrincipalName);

            if (rbFormatFirstDotLast.Checked)
                return string.Format("{0}.{1}@{2}", firstName, lastName, domainName);
            else if (rbFormatFirstLast.Checked)
                return string.Format("{0}{1}@{2}", firstName, lastName, domainName);
            else if (rbFormatLastDotFirst.Checked)
                return string.Format("{0}.{1}@{2}", lastName, firstName, domainName);
            else if (rbFormatLastFirst.Checked)
                return string.Format("{0}{1}@{2}", lastName, firstName, domainName);
            else if (rbFormatFirstInitialLast.Checked)
                return string.Format("{0}{1}@{2}", firstName.Substring(0, 1), lastName, domainName);
            else if (rbFormatLastFirstInitial.Checked)
                return string.Format("{0}{1}@{2}", lastName, firstName.Substring(0, 1), domainName);
            else if (rbFormatFirstName.Checked)
                return string.Format("{0}@{1}", firstName, domainName);
            else if (rbFormatLastName.Checked)
                return string.Format("{0}@{1}", lastName, domainName);
            else
            {
                // User has selected other so now we need to parse the input
                string entered = txtFormatOther.Text.Replace(" ", string.Empty).ToLower();

                // Replace the basic variables
                entered = entered.Replace("%g", firstName);
                entered = entered.Replace("%s", lastName);

                // Replace the advanced variables
                if (entered.Contains("%1s"))
                    entered = entered.Replace("%1s", lastName.Substring(0, 1));
                else if (entered.Contains("%2s"))
                    entered = entered.Replace("%2s", lastName.Substring(0, 2));
                else if (entered.Contains("%3s"))
                    entered = entered.Replace("%3s", lastName.Substring(0, 3));
                else if (entered.Contains("%4s"))
                    entered = entered.Replace("%4s", lastName.Substring(0, 4));
                else if (entered.Contains("%5s"))
                    entered = entered.Replace("%5s", lastName.Substring(0, 5));
                else if (entered.Contains("%6s"))
                    entered = entered.Replace("%6s", lastName.Substring(0, 6));
                else if (entered.Contains("%7s"))
                    entered = entered.Replace("%7s", lastName.Substring(0, 7));
                else if (entered.Contains("%8s"))
                    entered = entered.Replace("%8s", lastName.Substring(0, 8));
                else if (entered.Contains("%9s"))
                    entered = entered.Replace("%9s", lastName.Substring(0, 9));

                if (entered.Contains("%1g"))
                    entered = entered.Replace("%1g", firstName.Substring(0, 1));
                else if (entered.Contains("%2g"))
                    entered = entered.Replace("%2g", firstName.Substring(0, 2));
                else if (entered.Contains("%3g"))
                    entered = entered.Replace("%3g", firstName.Substring(0, 3));
                else if (entered.Contains("%4g"))
                    entered = entered.Replace("%4g", firstName.Substring(0, 4));
                else if (entered.Contains("%5g"))
                    entered = entered.Replace("%5g", firstName.Substring(0, 5));
                else if (entered.Contains("%6g"))
                    entered = entered.Replace("%6g", firstName.Substring(0, 6));
                else if (entered.Contains("%7g"))
                    entered = entered.Replace("%7g", firstName.Substring(0, 7));
                else if (entered.Contains("%8g"))
                    entered = entered.Replace("%8g", firstName.Substring(0, 8));
                else if (entered.Contains("%9g"))
                    entered = entered.Replace("%9g", firstName.Substring(0, 9));


                // Return the finished product
                return string.Format("{0}@{1}", entered.Replace(" ", string.Empty), domainName);
            }
        }

        /// <summary>
        /// Gets a list of users selected from the repeater to enable
        /// </summary>
        private List<MailboxUser> GetUsersToEnable()
        {
            List<MailboxUser> users = new List<MailboxUser>();

            // Loop through the repeater
            foreach (RepeaterItem items in repeaterNonMailboxUsers.Items)
            {
                if (items.ItemType == ListItemType.Item || items.ItemType == ListItemType.AlternatingItem)
                {
                    var checkBox = (CheckBox)items.FindControl("cbExchEnableUsers");
                    var displayName = (Label)items.FindControl("lbDisplayName");
                    var loginName = (Label)items.FindControl("lbLoginName");
                    var firstName = (Label)items.FindControl("lbFirstName");
                    var lastName = (Label)items.FindControl("lbLastName");

                    if (checkBox.Checked)
                    {
                        // User was selected
                        // Now add to our list to return
                        users.Add(new MailboxUser()
                        {
                            DisplayName = displayName.Text,
                            UserPrincipalName = loginName.Text,
                            FirstName = firstName.Text,
                            LastName = lastName.Text
                        });

                        // DEBUG //
                        this.logger.Debug("User " + loginName.Text + " was selected to be enabled for Exchange.");
                    }
                }
            }

            // DEBUG //
            this.logger.Debug("Total users selected to be enabled for Exchange: " + users.Count.ToString() + ". Company: " + CPContext.SelectedCompanyName);

            // Return our collection
            return users;
        }

        #endregion

    }
}