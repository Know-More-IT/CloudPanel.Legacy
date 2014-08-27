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

namespace CloudPanel.company.exchange
{
    public partial class resourcemailboxes : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected int currentMailboxSize = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Populates the drop down lists with the clients current email domains
                GetEmailDomains();
                
                // Gets a list of the current resource mailboxes for the client
                GetResourceMailboxes();

                // Gets a list of mailbox users
                GetMailboxUsers();
            }
        }

        /// <summary>
        /// Gets a list of resource mailboxes from the database
        /// </summary>
        private void GetResourceMailboxes()
        {
            try
            {
                List<ResourceMailbox> mailboxes = SQLMailboxes.GetResourceMailboxes(CPContext.SelectedCompanyCode);
                repeaterResourceMailboxes.DataSource = mailboxes;
                repeaterResourceMailboxes.DataBind();

                // Show panel
                panelEditMailbox.Visible = false;
                panelCreateMailbox.Visible = false;
                panelDisableMailbox.Visible = false;
                panelResourceMailboxes.Visible = true;
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Gets a list of mailbox plans from the database
        /// </summary>
        private void GetMailboxPlans()
        {
            try
            {
                List<MailboxPlan> plans = SQLPlans.GetMailboxPlans();

                // Remove ones that shouldn't show
                var showPlans = from p in plans
                                where string.IsNullOrEmpty(p.CompanyCode) || p.CompanyCode.Equals(CPContext.SelectedCompanyCode)
                                select p;

                // Clear and add blank
                ddlCreateMailboxPlans.Items.Clear();
                ddlEditMailboxPlans.Items.Clear();

                // Add rest of plans
                foreach (MailboxPlan p in showPlans)
                {
                    ListItem item = new ListItem();
                    item.Value = p.PlanID.ToString();
                    item.Text = p.DisplayName;
                    item.Attributes.Add("Description", p.Description);
                    item.Attributes.Add("Min", p.SizeInMB.ToString());
                    item.Attributes.Add("Max", p.MaximumSizeInMB.ToString());
                    item.Attributes.Add("Price", p.Price);
                    item.Attributes.Add("Extra", p.AdditionalGBPrice);

                    ddlCreateMailboxPlans.Items.Add(item);
                    ddlEditMailboxPlans.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Gets email domains from the database for the selected company
        /// </summary>
        private void GetEmailDomains()
        {
            try
            {
                List<Domain> domains = SQLDomains.GetAcceptedDomains(CPContext.SelectedCompanyCode);
                ddlCreateDomains.DataSource = domains;
                ddlCreateDomains.DataTextField = "DomainName";
                ddlCreateDomains.DataValueField = "DomainName";
                ddlCreateDomains.DataBind();

                ddlEmailDomains.DataSource = domains;
                ddlEmailDomains.DataTextField = "DomainName";
                ddlEmailDomains.DataValueField = "DomainName";
                ddlEmailDomains.DataBind();
                
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Gets a list of current mailbox users
        /// </summary>
        private void GetMailboxUsers()
        {
            try
            {
                List<MailboxUser> mailboxUsers = SQLMailboxes.GetMailboxUsers(CPContext.SelectedCompanyCode);

                // Sort our list first
                lstResourceDelegates.DataSource = mailboxUsers;
                lstResourceDelegates.DataBind();

                //
                // Bind to our full access and send as group but only if they have a sAMAccountName
                //
                var validUsers = from u in mailboxUsers
                                 where !string.IsNullOrEmpty(u.SamAccountName)
                                 select u;

                lstFullAccessPermissions.DataSource = validUsers;
                lstFullAccessPermissions.DataBind();

                lstSendAsPermissions.DataSource = validUsers;
                lstSendAsPermissions.DataBind();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.ToString());
            }
        }

        /// <summary>
        /// Cancels creating a mailbox and goes back to the resource mailbox screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCreateCancel_Click(object sender, EventArgs e)
        {
            GetResourceMailboxes();
        }

        /// <summary>
        /// Creates a new resource mailbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            ExchCmds powershell = null;

            try
            {
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // Generate the email
                string email = string.Format("{0}@{1}", txtCreatePrimarySmtpAddress.Text.Replace(" ", string.Empty), ddlCreateDomains.SelectedItem.Text);

                // Get the selected plan from the drop down list
                int planId = int.Parse(ddlCreateMailboxPlans.SelectedValue);

                // Get the values of the plan from the database
                MailboxPlan plan = SQLPlans.GetMailboxPlan(planId);
                plan.SetSizeInMB = int.Parse(hfCreateMailboxSizeMB.Value);

                // Generate our custom object
                ResourceMailbox mbx = new ResourceMailbox();
                mbx.CompanyCode = CPContext.SelectedCompanyCode;
                mbx.DisplayName = txtCreateDisplayName.Text;
                mbx.PrimarySmtpAddress = email;
                mbx.UserPrincipalName = email;
                mbx.ResourceType = rbRoom.Checked ? "Room" : "Equipment";
                mbx.MailboxPlanID = planId;

                // Figure out what type of resource mailbox is being created
                // and create it
                if (rbRoom.Checked)
                {
                    mbx.ResourceType = "Room";

                    // Create the room mailbox
                    powershell.New_RoomMailbox(mbx, CPContext.SelectedCompanyCode, Retrieve.GetCompanyExchangeOU, plan);
                }
                else if (rbEquipment.Checked)
                {
                    mbx.ResourceType = "Equipment";

                    // Create the equipment mailbox
                    powershell.New_EquipmentMailbox(mbx, CPContext.SelectedCompanyCode, Retrieve.GetCompanyExchangeOU, plan);
                }
                else
                {
                    // What is left is a shared mailbox
                    mbx.ResourceType = "Shared";

                    // Create the shared mailbox
                    powershell.New_SharedMailbox(mbx, CPContext.SelectedCompanyCode, Retrieve.GetCompanyExchangeOU, plan);
                }

                // Add to sql
                SQLMailboxes.AddResourceMailbox(mbx, plan.PlanID, plan.SetSizeInMB - plan.SizeInMB); // pass the plan ID and calculate the additional MB that were added

                // Update notificatoin
                notification1.SetMessage(controls.notification.MessageType.Success, "Successfully created new resource mailbox");
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();

                GetResourceMailboxes();
            }
        }

        /// <summary>
        /// Deletes the mailbox and the user account (since it is a resource mailbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDisableYes_Click(object sender, EventArgs e)
        {
            ExchCmds powershell = null;

            try
            {
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // Delete the mailbox
                powershell.Remove_Mailbox(hfDisableResourcePrincipalName.Value);

                // Delete from SQL
                SQLMailboxes.RemoveResourceMailbox(hfDisableResourcePrincipalName.Value, CPContext.SelectedCompanyCode);
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();

                GetResourceMailboxes();
            }
        }

        /// <summary>
        /// Cancels removing a mailbox and goes back to the resource screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDisableCancel_Click(object sender, EventArgs e)
        {
            GetResourceMailboxes();
        }

        /// <summary>
        /// Populates the view with information from the Exchange server for the resource mailbox
        /// </summary>
        /// <param name="userPrincipalName"></param>
        private void PopulateExistingMailbox(string userPrincipalName)
        {
            ExchCmds powershell = null;

            try
            {
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // Get information from SQL such as the plan size
                ResourceMailbox mbxSQL = SQLMailboxes.GetResourceMailbox(userPrincipalName);

                // Get the mailbox plan from the database
                MailboxPlan plan = SQLPlans.GetMailboxPlan(mbxSQL.MailboxPlanID);

                // 
                // Set SQL Information
                //
                ddlEditMailboxPlans.SelectedValue = mbxSQL.MailboxPlanID.ToString();
                currentMailboxSize = plan.SizeInMB + mbxSQL.AdditionalMB;
                lbResourceType.Text = mbxSQL.ResourceType;

                
                // Get resource mailbox information from Exchange
                MailboxUser mbx = null;
                if (mbxSQL.ResourceType.Equals("Shared"))
                    mbx = powershell.Get_Mailbox(userPrincipalName);
                else
                    mbxSQL = powershell.Get_ResourceMailbox(userPrincipalName);


                // See if it is resource mailbox or a shared mailbox
                if (mbx != null)
                {
                    #region Is a resource mailbox (not shared)
                    panelResourceGeneral.Visible = false;
                    panelResourcePolicy.Visible = false;

                    //
                    // General
                    //
                    hfEditDistinguishedName.Value = mbx.DistinguishedName;
                    hfEditUserPrincipalName.Value = mbx.UserPrincipalName;
                    txtDisplayName.Text = mbx.DisplayName;
                    txtEmailAddress.Text = mbx.PrimarySmtpAddress.Split('@')[0];
                    ddlEmailDomains.SelectedValue = mbx.PrimarySmtpAddress.Split('@')[1];
                    cbHidden.Checked = mbx.IsHidden;

                    //
                    // Populate the mailbox permissions for FullAccess
                    //
                    hfFullAccessOriginal.Value = string.Empty;
                    foreach (MailboxPermissions m in mbx.FullAccessPermissions)
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
                    this.logger.Debug("Full access permissions for " + mbx.UserPrincipalName + " when loaded is: " + hfFullAccessOriginal.Value);

                    //
                    // Populate the mailbox permissions for SendAs
                    //
                    hfSendAsOriginal.Value = string.Empty;
                    foreach (MailboxPermissions m in mbx.SendAsPermissions)
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
                    this.logger.Debug("Send-As permissions for " + mbx.UserPrincipalName + " when loaded is: " + hfSendAsOriginal.Value);
                    #endregion
                }
                else
                {
                    #region Is a shared mailbox
                    panelResourceGeneral.Visible = true;
                    panelResourcePolicy.Visible = true;

                    //
                    // General
                    //
                    hfEditDistinguishedName.Value = mbxSQL.DistinguishedName;
                    hfEditUserPrincipalName.Value = mbxSQL.UserPrincipalName;
                    txtDisplayName.Text = mbxSQL.DisplayName;
                    txtEmailAddress.Text = mbxSQL.PrimarySmtpAddress.Split('@')[0];
                    ddlEmailDomains.SelectedValue = mbxSQL.PrimarySmtpAddress.Split('@')[1];
                    cbHidden.Checked = mbxSQL.IsHidden;

                    //
                    // Populate the mailbox permissions for FullAccess
                    //
                    hfFullAccessOriginal.Value = string.Empty;
                    foreach (MailboxPermissions m in mbxSQL.FullAccessPermissions)
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
                    this.logger.Debug("Full access permissions for " + mbxSQL.UserPrincipalName + " when loaded is: " + hfFullAccessOriginal.Value);

                    //
                    // Populate the mailbox permissions for SendAs
                    //
                    hfSendAsOriginal.Value = string.Empty;
                    foreach (MailboxPermissions m in mbxSQL.SendAsPermissions)
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
                    this.logger.Debug("Send-As permissions for " + mbxSQL.UserPrincipalName + " when loaded is: " + hfSendAsOriginal.Value);

                    //
                    // Resource General
                    //
                    txtCapacity.Text = mbxSQL.ResourceCapacity.ToString();
                    if (mbxSQL.AutomateProcessing.Equals("AutoAccept"))
                        cbEnableResourceBookingAttendant.Checked = true;
                    else
                        cbEnableResourceBookingAttendant.Checked = false;

                    //
                    // Resource Policy
                    //
                    cbAllowConflictingMeeting.Checked = mbxSQL.AllowConflicts;
                    cbAllowRepeatingMeetings.Checked = mbxSQL.AllowRepeatingMeetings;
                    cbAllowScheduleDuringWorkHoursOnly.Checked = mbxSQL.SchedulingOnlyDuringWorkHours;
                    cbRejectMeetingBeyondBookingWindow.Checked = mbxSQL.RejectOutsideBookingWindow;

                    txtBookingWindowInDays.Text = mbxSQL.BookingWindowInDays.ToString();
                    txtMaximumDuration.Text = mbxSQL.MaximumDurationInMinutes.ToString();
                    txtMaxConflictInstances.Text = mbxSQL.MaximumConflictInstances.ToString();
                    txtConflictPercentageAllowed.Text = mbxSQL.ConflictPercentageAllowed.ToString();

                    cbForwardMeetingRequestsToDelegates.Checked = mbxSQL.ForwardRequestsToDelegates;

                    //
                    // Delegates
                    //
                    if (mbxSQL.ResourceDelegates != null && mbxSQL.ResourceDelegates.Length > 0)
                    {
                        foreach (ListItem item in lstResourceDelegates.Items)
                        {
                            item.Selected = false;

                            foreach (string s in mbxSQL.ResourceDelegates)
                            {
                                if (s.IndexOf(item.Value, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                    item.Selected = true;
                            }
                        }
                    }
                    #endregion
                }

                // Show the edit panel
                panelCreateMailbox.Visible = false;
                panelEditMailbox.Visible = true;
                panelResourceMailboxes.Visible = false;
                panelDisableMailbox.Visible = false;
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.ToString());
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Editing or deleting a resource mailbox
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void repeaterResourceMailboxes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                hfDisableResourcePrincipalName.Value = e.CommandArgument.ToString().Trim();

                // Show the disable panel
                panelCreateMailbox.Visible = false;
                panelEditMailbox.Visible = false;
                panelResourceMailboxes.Visible = false;
                panelDisableMailbox.Visible = true;
            }
            else if (e.CommandName == "Edit")
            {
                // Set UserPrincipalName value
                hfEditUserPrincipalName.Value = e.CommandArgument.ToString();

                // Get mailbox plans
                GetMailboxPlans();

                // Get updated mailbox user list
                GetMailboxUsers();

                // Gets information for the mailbox
                PopulateExistingMailbox(e.CommandArgument.ToString().Trim());
            }
        }

        /// <summary>
        /// Cancels from the save screen and goes back to the list of resource mailboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSaveCancel_Click(object sender, EventArgs e)
        {
            GetResourceMailboxes();
        }

        /// <summary>
        /// Saves the mailbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSaveMailbox_Click(object sender, EventArgs e)
        {
            ExchCmds powershell = null;

            try
            {
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                //
                // Get Exchange plan information
                //
                MailboxPlan plan = SQLPlans.GetMailboxPlan(int.Parse(ddlEditMailboxPlans.SelectedValue));
                int totalSelectedMB = int.Parse(hfEditMailboxSizeMB.Value);
                int totalAdditionalMB = totalSelectedMB - plan.SizeInMB;

                plan.SetSizeInMB = totalSelectedMB; // Log the actual size the user selected (with additional MB)

                //
                // Gather our information and store in our custom object
                //
                ResourceMailbox mbx = new ResourceMailbox();
                mbx.IsHidden = cbHidden.Checked;
                mbx.ResourceType = lbResourceType.Text;
                mbx.UserPrincipalName = hfEditUserPrincipalName.Value;
                mbx.DistinguishedName = hfEditDistinguishedName.Value;
                mbx.MailboxPlanID = int.Parse(ddlEditMailboxPlans.SelectedValue);
                mbx.DisplayName = txtDisplayName.Text;
                mbx.PrimarySmtpAddress = string.Format("{0}@{1}", txtEmailAddress.Text.Replace(" ", string.Empty), ddlEmailDomains.SelectedValue);

                if (!mbx.ResourceType.Equals("Shared"))
                {
                    mbx.ResourceCapacity = int.Parse(txtCapacity.Text);
                    mbx.BookingAttendantEnabled = cbEnableResourceBookingAttendant.Checked;
                    mbx.AllowConflicts = cbAllowConflictingMeeting.Checked;
                    mbx.AllowRepeatingMeetings = cbAllowRepeatingMeetings.Checked;
                    mbx.SchedulingOnlyDuringWorkHours = cbAllowScheduleDuringWorkHoursOnly.Checked;
                    mbx.RejectOutsideBookingWindow = cbRejectMeetingBeyondBookingWindow.Checked;
                    mbx.BookingWindowInDays = int.Parse(txtBookingWindowInDays.Text);
                    mbx.MaximumDurationInMinutes = int.Parse(txtMaximumDuration.Text);
                    mbx.MaximumConflictInstances = int.Parse(txtConflictPercentageAllowed.Text);
                    mbx.ConflictPercentageAllowed = int.Parse(txtConflictPercentageAllowed.Text);
                    mbx.ForwardRequestsToDelegates = cbForwardMeetingRequestsToDelegates.Checked;

                    List<string> resourceDelegates = new List<string>();
                    foreach (ListItem item in lstResourceDelegates.Items)
                    {
                        if (item.Selected)
                            resourceDelegates.Add(item.Value);
                    }
                    mbx.ResourceDelegates = resourceDelegates.ToArray();
                }

                //
                // Get what has changed for full access and send-as permissions
                //
                GetChangedMailboxPermissions(ref mbx);

                //
                // Update Exchange
                //
                powershell.Set_ResourceMailbox(mbx, plan);

                //
                // Update SQL
                //
                SQLMailboxes.UpdateResourceMailbox(mbx, plan.PlanID, totalAdditionalMB);

                // Show notificatoin
                notification1.SetMessage(controls.notification.MessageType.Success, "Successfully updated resource mailbox.");
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                // ERROR
                this.logger.Error("Error saving resource mailbox " + hfEditUserPrincipalName.Value, ex);
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();

                GetResourceMailboxes();
            }
        }

        /// <summary>
        /// Gets what has been changed for the full access and send-as permissions
        /// </summary>
        /// <param name="user"></param>
        private void GetChangedMailboxPermissions(ref ResourceMailbox user)
        {
            // DEBUG
            this.logger.Debug("Getting a list of full access permissions for " + user.UserPrincipalName);

            //
            // Figure out the full access permissions
            //
            List<string> fullAccessOriginal = hfFullAccessOriginal.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();


            // DEBUG
            this.logger.Debug("Accessing full access permissions for " + user.UserPrincipalName);

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
            this.logger.Debug("Checking if any full access permissions were removed for " + user.UserPrincipalName);

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
            this.logger.Debug("Getting a list of send as permissions for " + user.UserPrincipalName);
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
            this.logger.Debug("Checking if any send as permissions were removed for " + user.UserPrincipalName);
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

        protected void btnCreateMailbox_Click(object sender, EventArgs e)
        {
            // Populate mailbox plans
            GetMailboxPlans();

            // Clear the values
            txtCreateDisplayName.Text = string.Empty;
            txtCreatePrimarySmtpAddress.Text = string.Empty;

            panelEditMailbox.Visible = false;
            panelResourceMailboxes.Visible = false;
            panelDisableMailbox.Visible = false;
            panelCreateMailbox.Visible = true;
        }
    }
}