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

namespace CloudPanel.company.exchange
{
    public partial class groups : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Get a list of distribution groups and set the panel to show and others to hide
                GetDistributionGroups();
            }

            // Check group limit on each postback
            CheckLimit();
        }

        /// <summary>
        /// Checks the group limit
        /// </summary>
        private void CheckLimit()
        {
            // Check if they are at the max or not
            try
            {
                bool isAtMax = SQLLimits.IsCompanyAtDistGroupLimit(CPContext.SelectedCompanyCode);
                if (isAtMax)
                {
                    btnCreateNewGroup.Text = "Limit Reached";
                    btnCreateNewGroup.Enabled = false;
                }
                else
                {
                    btnCreateNewGroup.Text = "Create Group";
                    btnCreateNewGroup.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                // Log error //
                this.logger.Error("Error checking the limits on distribution groups for company " + CPContext.SelectedCompanyCode, ex);

                btnCreateNewGroup.Text = "Error";
                btnCreateNewGroup.Enabled = false;
            }
        }

        /// <summary>
        /// Retrieves the information for the fields and shows 
        /// the create new group section
        /// </summary>
        private void CreateNewGroup()
        {
            // Clear fields
            txtDisplayName.Text = string.Empty;
            txtEmailAddress.Text = string.Empty;
            cbGroupHidden.Checked = false;
            
            repeaterOwners.DataSource = null;
            repeaterOwners.DataBind();

            repeaterMembers.DataSource = null;
            repeaterMembers.DataBind();
            
            // Clear the hidden value fields
            hfCurrentEmailAddress.Value = null;
            hfEditGroupDistinguishedName.Value = null;
            hfModifiedMembership.Value = null;
            hfModifiedOwners.Value = null;
            hfOriginalMembership.Value = null;
            hfOriginalOwners.Value = null;
            hfRemovedMembership.Value = null;
            hfRemovedOwners.Value = null;

            // Get list of mailbox users  and groups for the ownership section
            GetMailObjects();

            // Get a list of domains
            GetDomains();

            // Get the popup list
            GetPopupList();

            // Hide and show the correct panels
            panelGroupList.Visible = false;
            panelGroupDelete.Visible = false;
            panelNewEditGroup.Visible = true;
        }

        /// <summary>
        /// Gets a list of distribution groups and hides the panels not needed
        /// </summary>
        private void GetDistributionGroups()
        {
            // Change panel views
            panelGroupDelete.Visible = false;
            panelNewEditGroup.Visible = false;
            panelGroupList.Visible = true;

            try
            {
                logger.DebugFormat("Getting distribution groups...");

                List<ExchangeGroup> groups = SQLExchange.GetGroups(CPContext.SelectedCompanyCode);

                logger.DebugFormat("Ordering distribution groups found...");
                groups.OrderBy(x => x.DisplayName);

                logger.DebugFormat("Binding distribution groups to the repeater...");
                repeaterGroups.DataSource = groups;
                repeaterGroups.DataBind();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Gets a list of current mailbox users
        /// </summary>
        private void GetMailObjects()
        {
            try
            {
                List<MailboxUser> users = SQLMailboxes.GetMailboxUsers(CPContext.SelectedCompanyCode);
                List<ExchangeGroup> groups = SQLExchange.GetDistributionGroups(CPContext.SelectedCompanyCode);

                List<ListBoxSorter> sorter = new List<ListBoxSorter>();
                
                // Add users to our sorter
                foreach (MailboxUser u in users)
                    sorter.Add(new ListBoxSorter() { String1 = u.DisplayName, String2 = u.CanonicalName.ToUpper() });

                // Add groups to our sorter
                foreach (ExchangeGroup g in groups)
                    sorter.Add(new ListBoxSorter() { String1 = g.DisplayName, String2 = g.CanonicalName.ToUpper() });

                // Sort values
                sorter = sorter.OrderBy(x => x.String1).ToList();

                // Bind values
                lstDeliveryManagementRestrict.DataSource = sorter;
                lstDeliveryManagementRestrict.DataBind();

                lstSendersDontRequireApproval.DataSource = sorter;
                lstSendersDontRequireApproval.DataBind();

                lstGroupModerators.DataSource = sorter;
                lstGroupModerators.DataBind();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.ToString());
            }
        }

        /// <summary>
        /// Gets a list of distribution groups, contacts, and mailbox users
        /// </summary>
        private void GetPopupList()
        {
            try
            {
                List<MailboxUser> users = SQLMailboxes.GetMailboxUsers(CPContext.SelectedCompanyCode);
                List<BaseContacts> contacts = SQLExchange.GetContacts(CPContext.SelectedCompanyCode);
                List<ExchangeGroup> groups = SQLExchange.GetDistributionGroups(CPContext.SelectedCompanyCode);

                List<RepeaterOwnership> repeaterData = new List<RepeaterOwnership>();
                foreach (MailboxUser u in users)
                    repeaterData.Add(new RepeaterOwnership()
                    {
                        DisplayName = u.DisplayName,
                        DistinguishedName = u.DistinguishedName,
                        PrimarySmtpAddress = u.PrimarySmtpAddress,
                        ObjectType = "User",
                        ImageUrl = "~/img/icons/16/user.png"
                    });


                foreach (ExchangeGroup g in groups)
                    repeaterData.Add(new RepeaterOwnership()
                    {
                        DisplayName = g.DisplayName,
                        DistinguishedName = g.DistinguishedName,
                        PrimarySmtpAddress = g.PrimarySmtpAddress,
                        ObjectType = "Group",
                        ImageUrl = "~/img/icons/16/people.png"
                    });

                foreach (BaseContacts c in contacts)
                    repeaterData.Add(new RepeaterOwnership()
                    {
                        DisplayName = c.DisplayName,
                        DistinguishedName = c.DistinguishedName,
                        PrimarySmtpAddress = c.Email,
                        ObjectType = "Contact",
                        ImageUrl = "~/img/icons/16/web.png"
                    });

                // Order list
                repeaterData.OrderBy(x => x.ImageUrl).ThenBy(x => x.DisplayName);

                // Bind to popup
                repeaterPopup.DataSource = repeaterData;
                repeaterPopup.DataBind();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.ToString());
            }
        }

        /// <summary>
        /// Gets a list of domains for the company
        /// </summary>
        private void GetDomains()
        {
            try
            {
                List<Domain> domains = SQLExchange.GetAcceptedDomains(CPContext.SelectedCompanyCode);

                ddlDomains.DataSource = domains;
                ddlDomains.DataBind();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Begins creating the group
        /// </summary>
        private void StartCreatingNewGroup()
        {
            ExchCmds powershell = null;
            ADUsers users = null;

            try
            {
                users = new ADUsers(Config.Username, Config.Password, Config.PrimaryDC);
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // User input minus the spaces
                string emailInput = txtEmailAddress.Text.Replace(" ", string.Empty);

                ExchangeGroup group = new ExchangeGroup();
                group.DisplayName = txtDisplayName.Text;
                group.PrimarySmtpAddress = string.Format("{0}@{1}", emailInput, ddlDomains.SelectedItem.Value);
                group.SamAccountName = emailInput;
                group.CompanyCode = CPContext.SelectedCompanyCode;
                group.ModerationEnabled = cbMustBeApprovedByAModerator.Checked;
                group.Hidden = cbGroupHidden.Checked;

                // Check the length of the sAMAccountName (cannot exceed 19 characters)
                if (group.SamAccountName.Length > 19)
                {
                        group.SamAccountName = group.SamAccountName.Substring(0, 18);
                        this.logger.Debug("User's sAMAccountName was to long and had to be shortened to: " + group.SamAccountName);
                }

                // Compile the sAMAccountName
                string finalSamAccountName = emailInput;
                for (int i = 1; i < 999; i++)
                {
                    if (users.DoesSamAccountNameExist(finalSamAccountName))
                    {
                        this.logger.Debug("SamAccountName " + finalSamAccountName + " is already in use. Trying to find another account name...");
                        finalSamAccountName = group.SamAccountName + i.ToString(); // We found a match so we need to increment the number

                        // Make sure the SamAccountName is less than 19 characters
                        if (finalSamAccountName.Length > 19)
                        {
                            finalSamAccountName = finalSamAccountName.Substring(0, 18 - i.ToString().Length) + i.ToString(); // Make sure it isn't above 19 characters
                            this.logger.Debug("New SamAccountName was too long and was trimmed to " + finalSamAccountName);
                        }
                    }
                    else
                    {
                        // No match was found which means we can continue and break out of the loop
                        group.SamAccountName = finalSamAccountName;
                        this.logger.Debug("Found SamAccountName not in use: " + group.SamAccountName);
                        break;
                    }
                }

                // Make sure to remove any spaces
                string ownersString = hfModifiedOwners.Value.Trim();
                string membersString = hfModifiedMembership.Value.Trim();

                // Our string seperator to split the owners and members information
                string[] separators = { "||" };

                // Collection of owners
                group.ManagedBy = ownersString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (group.ManagedBy == null || group.ManagedBy.Length < 1)
                    throw new Exception("You did not select a group owner. There must be at least one group owner to create a distribution group.");
               
                // Collection of members
                group.MembersArray = membersString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (group.MembersArray == null || group.MembersArray.Length < 1)
                    throw new Exception("You did not select a member. There must be at least one group member to create a distribution group.");
                                
                // Go through membership approval settings
                group.JoinRestriction = "Open";
                if (rbMembershipApprovalJoinClosed.Checked)
                    group.JoinRestriction = "Closed";
                else if (rbMembershipApprovalJoinApproval.Checked)
                    group.JoinRestriction = "ApprovalRequired";

                group.DepartRestriction = "Open";
                if (rbMembershipApprovalLeaveClosed.Checked)
                    group.DepartRestriction = "Closed";

                //
                // Go through delivery management settings
                //
                group.RequireSenderAuthentication = true;
                if (rbDeliveryManagementInsideOutside.Checked)
                    group.RequireSenderAuthentication = false;

                // Compile the list of "Allowed senders" (if any)
                List<string> allowedSenders = new List<string>();
                foreach (ListItem li in lstDeliveryManagementRestrict.Items)
                {
                    if (li.Selected)
                        allowedSenders.Add(li.Value);
                }
                if (allowedSenders.Count > 0)
                    group.WhoCanSendToGroup = allowedSenders.ToArray();


                //
                // Go through message approval settings
                //
                group.SendModerationNotifications = "Never";
                if (rbMessageApprovalNotifyAll.Checked)
                    group.SendModerationNotifications = "Always";
                else if (rbMessageApprovalNotifyInternal.Checked)
                    group.SendModerationNotifications = "Internal";

                // Compile the list of "Group Moderators" (if any)
                List<string> groupModerators = new List<string>();
                foreach (ListItem li in lstGroupModerators.Items)
                {
                    if (li.Selected)
                        groupModerators.Add(li.Value);
                }
                if (groupModerators.Count > 0)
                    group.GroupModerators = groupModerators.ToArray();


                // Compile the list of senders that don't require approval
                List<string> bypassModerationSenders = new List<string>();
                foreach (ListItem li in lstSendersDontRequireApproval.Items)
                {
                    if (li.Selected)
                        bypassModerationSenders.Add(li.Value);
                }
                if (bypassModerationSenders.Count > 0)
                    group.SendersNotRequiringApproval = bypassModerationSenders.ToArray();

                // Create group
                powershell.New_DistributionGroup(group, Retrieve.GetCompanyExchangeOU);

                // Add group to SQL
                SQLExchange.AddDistributionGroup("CN=" + group.PrimarySmtpAddress + "," + Retrieve.GetCompanyExchangeOU, CPContext.SelectedCompanyCode, group.DisplayName, group.PrimarySmtpAddress, cbGroupHidden.Checked);
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();

                // Refresh the distribution group view
                GetDistributionGroups();
            }
        }

        /// <summary>
        /// Updates the group
        /// </summary>
        private void StartUpdatingNewGroup()
        {
            ExchCmds powershell = null;

            try
            {
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                string displayName = txtDisplayName.Text;
                string emailAddress = txtEmailAddress.Text.Replace(" ", string.Empty) + "@" + ddlDomains.SelectedItem.Value;

                // Our string seperator to split the owners and members information
                string[] separators = { "||" };

                // Compile a list of owners
                List<string> newOwners = hfOriginalOwners.Value.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> ownerAdded = hfModifiedOwners.Value.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> ownerRemoved = hfRemovedOwners.Value.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();
                
                // Now lets see who was added or removed from the owners section
                newOwners.RemoveAll(ownerRemoved.Contains);  // removes everything from original owners that was in the removed list
                newOwners.AddRange(ownerAdded); // Adds everything that was in the ownerAdded list (except duplicates)
               

                // Compile a list of members
                List<string> newMembers = hfModifiedMembership.Value.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> removedMembers = hfRemovedMembership.Value.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();

                // Go through membership approval settings
                string memberJoinRestriction = "Open";
                if (rbMembershipApprovalJoinClosed.Checked)
                    memberJoinRestriction = "Closed";
                else if (rbMembershipApprovalJoinApproval.Checked)
                    memberJoinRestriction = "ApprovalRequired";

                string memberDepartRestriction = "Open";
                if (rbMembershipApprovalLeaveClosed.Checked)
                    memberDepartRestriction = "Closed";

                //
                // Go through delivery management settings
                //
                bool requireSenderAuthentication = true;
                if (rbDeliveryManagementInsideOutside.Checked)
                    requireSenderAuthentication = false;

                // Compile the list of "Allowed senders" (if any)
                List<string> allowedSenders = new List<string>();
                foreach (ListItem li in lstDeliveryManagementRestrict.Items)
                {
                    if (li.Selected)
                        allowedSenders.Add(li.Value);
                }
                if (allowedSenders.Count < 1)
                    allowedSenders = null; // Clear (null) the list if it is empty

                //
                // Go through message approval settings
                //
                string notify = "Never";
                if (rbMessageApprovalNotifyAll.Checked)
                    notify = "Always";
                else if (rbMessageApprovalNotifyInternal.Checked)
                    notify = "Internal";

                // Compile the list of "Group Moderators" (if any)
                List<string> groupModerators = new List<string>();
                foreach (ListItem li in lstGroupModerators.Items)
                {
                    if (li.Selected)
                        groupModerators.Add(li.Value);
                }
                if (groupModerators.Count < 1)
                    groupModerators = null; // Clear (null) the list if it is empty

                // Compile the list of senders that don't require approval
                List<string> bypassModerationSenders = new List<string>();
                foreach (ListItem li in lstSendersDontRequireApproval.Items)
                {
                    if (li.Selected)
                        bypassModerationSenders.Add(li.Value);
                }
                if (bypassModerationSenders.Count < 1)
                    bypassModerationSenders = null; // Clear (null) the list if it is empty

                // Update Group
                powershell.Set_DistributionGroup(displayName, CPContext.SelectedCompanyCode, hfCurrentEmailAddress.Value, emailAddress, memberJoinRestriction, memberDepartRestriction, 
                    cbMustBeApprovedByAModerator.Checked, notify, newOwners, groupModerators, allowedSenders, bypassModerationSenders, cbGroupHidden.Checked, requireSenderAuthentication);

                // Update SQL
                SQLExchange.UpdateDistributionGroup("CN=" + emailAddress + "," + Retrieve.GetCompanyExchangeOU, hfCurrentEmailAddress.Value, emailAddress, displayName, cbGroupHidden.Checked);

                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Warning, Resources.LocalizedText.NotificationWarningUpdateGroup);

                // Add the new members
                foreach (string added in newMembers)
                {
                    powershell.Add_DistributionGroupMember(emailAddress, added);
                }

                // Remove members that are no longer part of the group
                foreach (string removed in removedMembers)
                {
                    powershell.Remove_DistributionGroupMember(emailAddress, removed);
                }


                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.NotificationSuccessUpdateGroup);
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.ToString());
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();

                // Refresh the distribution group view
                GetDistributionGroups();
            }
        }

        /// <summary>
        /// Item command for our repeater for editing or deleting a group
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void repeaterGroups_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EditGroup")
            {
                // Get the popup list
                GetPopupList();

                // Get the other list
                GetMailObjects();

                // Get the detailed information
                GetDistributionGroup(e.CommandArgument.ToString());
            }
            else if (e.CommandName == "DeleteGroup")
            {
                // Hide all panels
                panelGroupList.Visible = false;
                panelNewEditGroup.Visible = false;

                // Set delete panel information
                string[] split = e.CommandArgument.ToString().Split('|');

                lbDeleteDisplayName.Text = split[0];
                hfDeleteDistributionGroup.Value = split[1];

                // Show panel
                panelGroupDelete.Visible = true;
            }
        }

        /// <summary>
        /// Retrieves detailed information about a particular distribution group
        /// </summary>
        /// <param name="groupEmailAddress"></param>
        private void GetDistributionGroup(string groupEmailAddress)
        {
            ExchCmds powershell = null;

            try
            {
                // Initialize powershell session
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // Get the distribution group
                ExchangeGroup group = powershell.Get_DistributionGroup(groupEmailAddress);

                // Clear fields
                hfModifiedOwners.Value = string.Empty;
                hfModifiedMembership.Value = string.Empty;
                hfRemovedOwners.Value = string.Empty;
                hfRemovedMembership.Value = string.Empty;

                // Populate fields
                GetDomains();
                GetMailObjects();

                // Populate information
                txtDisplayName.Text = group.DisplayName;
                txtEmailAddress.Text = group.PrimarySmtpAddress.Split('@')[0];
                ddlDomains.SelectedValue = group.PrimarySmtpAddress.Split('@')[1];
                cbGroupHidden.Checked = group.Hidden;
                hfCurrentEmailAddress.Value = group.PrimarySmtpAddress;


                //
                // Get the list of users, contacts, and groups that can be added
                //
                List<MailboxUser> groupOwners = SQLMailboxes.GetMailboxUsers(CPContext.SelectedCompanyCode);

                //
                // Find the owners
                //
                hfOriginalOwners.Value = String.Join("||", group.ManagedBy);

                if (groupOwners != null)
                {
                    List<RepeaterOwnership> owners = new List<RepeaterOwnership>();
                    foreach (string s in group.ManagedBy)
                    {
                        MailboxUser tmp = groupOwners.FirstOrDefault(m => m.CanonicalName.Equals(s, StringComparison.CurrentCultureIgnoreCase));
                        if (tmp != null)
                        {
                            owners.Add(new RepeaterOwnership()
                            {
                                DisplayName = tmp.DisplayName,
                                DistinguishedName = tmp.DistinguishedName,
                                ImageUrl = "~/img/icons/16/user.png"
                            });
                        }
                    }

                    // Order list
                    owners.OrderBy(x => x.ImageUrl).ThenBy(x => x.DisplayName);

                    repeaterOwners.DataSource = owners;
                    repeaterOwners.DataBind();
                }

                //
                // Find the members
                //
                hfOriginalMembership.Value = String.Join("||", group.Members);

                List<RepeaterOwnership> members = new List<RepeaterOwnership>();
                foreach (MailObject member in group.Members)
                {
                    if (member.ObjectType == Enumerations.ObjectType.User)
                    {
                        members.Add(new RepeaterOwnership()
                        {
                            DisplayName = member.DisplayName,
                            DistinguishedName = member.DistinguishedName,
                            ImageUrl = "~/img/icons/16/user.png"
                        });
                    }
                    else if (member.ObjectType == Enumerations.ObjectType.Group)
                    {
                        members.Add(new RepeaterOwnership()
                        {
                            DisplayName = member.DisplayName,
                            DistinguishedName = member.DistinguishedName,
                            ImageUrl = "~/img/icons/16/people.png"
                        });
                    }
                    else if (member.ObjectType == Enumerations.ObjectType.Contact)
                    {
                        members.Add(new RepeaterOwnership()
                        {
                            DisplayName = member.DisplayName,
                            DistinguishedName = member.DistinguishedName,
                            ImageUrl = "~/img/icons/16/web.png"
                        });
                    }
                }

                // Order list
                members.OrderBy(x => x.ImageUrl).ThenBy(x => x.DisplayName);

                repeaterMembers.DataSource = members;
                repeaterMembers.DataBind();

                //
                // Membership approval
                //
                switch (group.JoinRestriction.ToLower())
                {
                    case "approvalrequired":
                        rbMembershipApprovalJoinApproval.Checked = true;
                        break;
                    case "closed":
                        rbMembershipApprovalJoinClosed.Checked = true;
                        break;
                    default:
                        rbMembershipApprovalJoinOpen.Checked = true;
                        break;
                }

                switch (group.DepartRestriction.ToLower())
                {
                    case "closed":
                        rbMembershipApprovalLeaveClosed.Checked = true;
                        break;
                    default:
                        rbMembershipApprovalLeaveOpen.Checked = true;
                        break;
                }

                //
                // Delivery Management
                //
                if (group.RequireSenderAuthentication)
                    rbDeliveryManagementInsideOnly.Checked = true;
                else
                    rbDeliveryManagementInsideOutside.Checked = true;

                if (group.WhoCanSendToGroup != null)
                {
                    foreach (ListItem item in lstDeliveryManagementRestrict.Items)
                    {
                        foreach (string s in group.WhoCanSendToGroup)
                        {
                            if (s.IndexOf(item.Value, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                item.Selected = true;
                        }
                    }
                }

                //
                // Message Approval
                //
                if (group.ModerationEnabled)
                    cbMustBeApprovedByAModerator.Checked = true;
                else
                    cbMustBeApprovedByAModerator.Checked = false;

                if (group.GroupModerators != null)
                {
                    foreach (ListItem item in lstGroupModerators.Items)
                    {
                        foreach (string s in group.GroupModerators)
                        {
                            if (s.IndexOf(item.Value, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                item.Selected = true;
                        }
                    }
                }

                if (group.SendersNotRequiringApproval != null)
                {
                    foreach (ListItem item in lstSendersDontRequireApproval.Items)
                    {
                        foreach (string s in group.SendersNotRequiringApproval)
                        {
                            if (s.IndexOf(item.Value, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                item.Selected = true;
                        }
                    }
                }

                //
                // Last section
                //
                switch (group.SendModerationNotifications.ToLower())
                {
                    case "always":
                        rbMessageApprovalNotifyAll.Checked = true;
                        break;
                    case "internal":
                        rbMessageApprovalNotifyInternal.Checked = true;
                        break;
                    default:
                        rbMessageApprovalNotifyNone.Checked = true;
                        break;
                }

                // Show and hide correct panels
                panelGroupDelete.Visible = false;
                panelGroupList.Visible = false;
                panelNewEditGroup.Visible = true;
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error getting distribution group: " + ex.Message);
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Deletes the group from Exchange and SQL where from the panel that asks the user if they are sure they want to delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDeleteYes_Click(object sender, EventArgs e)
        {
            ExchCmds powershell = null;

            try
            {
                // Initialize
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // Delete group from Exchange
                powershell.Remove_DistributionGroup(hfDeleteDistributionGroup.Value);

                // Remove from SQL
                SQLExchange.RemoveGroup(hfDeleteDistributionGroup.Value);

                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.NotificationSuccessDeleteGroup);
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
            finally
            {
                // Refresh to the main distribution group list panel
                GetDistributionGroups();
            }
        }

        /// <summary>
        /// Reverts back to the distribution groups main screen from the Delete panel where it asks the user if they are sure they want to delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDeleteCancel_Click(object sender, EventArgs e)
        {
            // Get list of distribution groups since the user cancelled
            GetDistributionGroups();
        }

        /// <summary>
        /// Shows the screen to create a new group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCreateNewGroup_Click(object sender, EventArgs e)
        {
            // Clear the hidden fields
            hfCurrentEmailAddress.Value = string.Empty;

            // Show the new group field
            CreateNewGroup();
        }

        /// <summary>
        /// Actually inserts / updates Exchange and SQL depending on if the hidden field is empty or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCreateEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfCurrentEmailAddress.Value))
            {
                StartCreatingNewGroup();
            }
            else
            {
                StartUpdatingNewGroup();
            }
        }

        /// <summary>
        /// Cancels and goes back to the main list of distribution groups
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            GetDistributionGroups();
        }
    }
}