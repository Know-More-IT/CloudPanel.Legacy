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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CloudPanel.company.exchange
{
    public partial class publicfolders : System.Web.UI.Page
    {
        // Logger
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnEnablePublicFolders.Enabled = true;

                // Check if a company is enabled or not
                if (IsCompanyEnabled())
                {
                    panelDisablePublicFolders.Visible = false;
                    panelEnablePublicFolders.Visible = false;
                    panelEditPublicFolders.Visible = true;

                    GetPublicFolders();
                    GetEmailDomains();
                }
                else
                {
                    panelDisablePublicFolders.Visible = false;
                    panelEditPublicFolders.Visible = false;
                    panelEnablePublicFolders.Visible = true;
                }
            }
        }

        /// <summary>
        /// Gets the public folder hierarchy
        /// </summary>
        private void GetPublicFolders()
        {
            ExchCmds powershell = null;

            try
            {
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);
                
                // Get public folders
                List<BasePublicFolder> pf = powershell.Get_PublicFolderHierarchy(CPContext.SelectedCompanyCode, Config.ExchangeVersion);

                this.logger.Debug("Sorting public folder hierarchy for " + CPContext.SelectedCompanyCode);
                pf.Sort((pf1, pf2) => pf1.ParentPath.CompareTo(pf2.ParentPath));

                foreach (BasePublicFolder p in pf)
                {
                    this.logger.Debug("Processing public folder " + p.Path);

                    if (p.ParentPath.Equals(@"\"))
                    {
                        TreeNode tmp = new TreeNode();
                        tmp.Text = " - " + p.Name;
                        tmp.Value = p.Path;
                        tmp.ImageUrl = "~/img/icons/16/folder.png";

                        treePublicFolders.Nodes.Add(tmp);
                    }
                    else
                    {
                        // It isn't a parent node so we need to find its parent then add it
                        FindNode(treePublicFolders.Nodes, p);
                    }
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
            }
        }
        
        /// <summary>
        /// Checks if a company is enabled for public folders or not
        /// </summary>
        /// <returns></returns>
        private bool IsCompanyEnabled()
        {
            try
            {
                return SQLPublicFolders.IsPublicFolderEnabled(CPContext.SelectedCompanyCode);
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error checking if your company is enabled for public folders: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Enables public folders for a company
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnablePublicFolders_Click(object sender, EventArgs e)
        {
            ADGroups groups = null;
            ExchCmds powershell = null;

            try
            {
                // Initialize
                groups = new ADGroups(Config.Username, Config.Password, Config.PrimaryDC);
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // Get company code
                string companyCodeWithSpaces = CPContext.SelectedCompanyCode;
                string companyCodeWithoutSpaces = companyCodeWithSpaces.Replace(" ", string.Empty);

                // Create groups
                groups.Create(Retrieve.GetCompanyExchangeOU, "PublicFolderAdmins@" + companyCodeWithoutSpaces, "Public Folder Administrators", true, ADGroups.GroupType.Universal, false);
                groups.Create(Retrieve.GetCompanyExchangeOU, "PublicFolderUsers@" + companyCodeWithoutSpaces, "Public Folder Users", true, ADGroups.GroupType.Universal, false);

                // Modify membership
                groups.ModifyMembership("PublicFolderAdmins@" + companyCodeWithoutSpaces, "Admins@" + companyCodeWithoutSpaces, System.DirectoryServices.AccountManagement.IdentityType.Name,
                     System.DirectoryServices.AccountManagement.IdentityType.Name, false, false);
                groups.ModifyMembership("PublicFolderUsers@" + companyCodeWithoutSpaces, "AllUsers@" + companyCodeWithoutSpaces, System.DirectoryServices.AccountManagement.IdentityType.Name,
                     System.DirectoryServices.AccountManagement.IdentityType.Name, false, false);

                // Enable security groups as distribution groups so we can add to public folders
                powershell.Enable_DistributionGroup(companyCodeWithoutSpaces, "PublicFolderAdmins@" + companyCodeWithoutSpaces, true);
                powershell.Enable_DistributionGroup(companyCodeWithoutSpaces, "PublicFolderUsers@" + companyCodeWithoutSpaces, true);

                // Create public folder
                powershell.New_PublicFolder(companyCodeWithSpaces);

                // Remove the default permissions
                powershell.Remove_PublicFolderDefaultPermissions(companyCodeWithSpaces, Config.ExchangeVersion);

                // Add permissions
                powershell.Add_PublicFolderClientPermission(companyCodeWithSpaces);

                // Update the database
                SQLPublicFolders.Update_PublicFolderForCompany(companyCodeWithSpaces, true);

                // Set new view
                panelDisablePublicFolders.Visible = false;
                panelEnablePublicFolders.Visible = false;
                panelEditPublicFolders.Visible = true;

                // Get public folders
                GetPublicFolders();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Could not enable public folders: " + ex.Message);
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();

                if (groups != null)
                    groups.Dispose();
            }
        }
        
        /// <summary>
        /// Removes a public folder from a company
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDisableYes_Click(object sender, EventArgs e)
        {
            ExchCmds powershell = null;

            try
            {
                // Initialize
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // Remove public folder
                powershell.Remove_PublicFolder(CPContext.SelectedCompanyCode);

                // Remove from SQL
                SQLPublicFolders.Update_PublicFolderForCompany(CPContext.SelectedCompanyCode, false);

                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Success, "Successfully disabled public folders for your company");

                // Change view
                panelDisablePublicFolders.Visible = false;
                panelEditPublicFolders.Visible = false;
                panelEnablePublicFolders.Visible = true;
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Failed to remove public folders. Please contact support: " + ex.Message);
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Cancels and reverts view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDisableCancel_Click(object sender, EventArgs e)
        {
            panelDisablePublicFolders.Visible = false;
            panelEnablePublicFolders.Visible = false;
            panelEditPublicFolders.Visible = true;
        }

        /// <summary>
        /// Gets the list of domains for the company
        /// </summary>
        private void GetEmailDomains()
        {
            try
            {
                List<Domain> domains = SQLDomains.GetDomains(CPContext.SelectedCompanyCode);

                var acceptedDomains = from d in domains
                                      where d.IsAcceptedDomain
                                      select d.DomainName;

                ddlEmailDomains.DataSource = acceptedDomains;
                ddlEmailDomains.DataBind();

                ddlEmailDomains.SelectedValue = (from d in domains
                                                 where d.IsAcceptedDomain
                                                 where d.IsDefault
                                                 select d.DomainName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves the public folder information when the user
        /// selects one of the nodes from the treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void treePublicFolders_SelectedNodeChanged(object sender, EventArgs e)
        {
            string path = treePublicFolders.SelectedNode.Value;

            GetPublicFolder(path);
        }

        /// <summary>
        /// Gets a specific public folder from Exchange and displays the information
        /// </summary>
        /// <param name="path"></param>
        private void GetPublicFolder(string path)
        {
            ExchCmds powershell = null;

            try
            {
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                BasePublicFolder pf = powershell.Get_PublicFolder(path);
                txtPublicFolderName.Text = pf.Name;
                txtMaxItemSize.Text = pf.MaxItemSize;
                txtWarningSizeMB.Text = pf.IssueWarningQuota;
                txtProhibitPostMB.Text = pf.ProhibitPostQuota;
                txtAgeLimit.Text = pf.AgeLimit;
                txtKeepDeletedItems.Text = pf.RetainDeletedItemsFor;

                // Set our hidden field for the selected public folder
                hfPublicFolderPath.Value = pf.Path;

                if (pf.MailFolderInfo == null)
                {
                    // Set that the public folder currently isn't enabled for email
                    cbPFCurrentEmailEnabled.Checked = false;

                    txtDisplayName.Text = string.Empty;
                    txtPrimaryEmail.Text = string.Empty;
                    cbPFEnableEmail.Checked = false;
                }
                else
                {
                    // Set the the public folder IS currently enabled for email
                    cbPFCurrentEmailEnabled.Checked = true;

                    cbPFEnableEmail.Checked = true;
                    txtDisplayName.Text = pf.MailFolderInfo.DisplayName;
                    cbPFHiddenFromAddressLists.Checked = pf.MailFolderInfo.Hidden;

                    string[] emailSplit = pf.MailFolderInfo.EmailAddress.Split('@');
                    txtPrimaryEmail.Text = emailSplit[0];

                    ListItem item = ddlEmailDomains.Items.FindByValue(emailSplit[1]);
                    if (item != null)
                        ddlEmailDomains.SelectedValue = item.Value;
                }

                // Check if the user is a super admin or reseller admin.
                // If they are not we need to diasble the fields
                if (Master.IsSuperAdmin || Master.IsResellerAdmin)
                {
                    txtMaxItemSize.ReadOnly = false;
                    txtWarningSizeMB.ReadOnly = false;
                    txtProhibitPostMB.ReadOnly = false;
                    txtAgeLimit.ReadOnly = false;
                    txtKeepDeletedItems.ReadOnly = false;
                }
                else
                {
                    txtMaxItemSize.ReadOnly = true;
                    txtWarningSizeMB.ReadOnly = true;
                    txtProhibitPostMB.ReadOnly = true;
                    txtAgeLimit.ReadOnly = true;
                    txtKeepDeletedItems.ReadOnly = true;
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
            }
        }

        /// <summary>
        /// Loops through the nodes and adds the child nodes when it finds it
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="pf"></param>
        private void FindNode(TreeNodeCollection nodes, BasePublicFolder pf)
        {
            foreach (TreeNode n in nodes)
            {
                if (n.Value.Equals(pf.ParentPath))
                {
                    TreeNode tmp = new TreeNode();
                    tmp.Text = " - " + pf.Name;
                    tmp.Value = pf.Path;

                    if (string.IsNullOrEmpty(pf.FolderClass))
                        tmp.ImageUrl = "~/img/icons/16/folder.png";
                    else
                    {
                        switch (pf.FolderClass.ToLower().Trim())
                        {
                            case "ipf.appointment":
                                tmp.ImageUrl = "~/img/icons/16/calendar.png";
                                break;
                            case "ipf.contact":
                                tmp.ImageUrl = "~/img/icons/16/contacts.png";
                                break;
                            case "ipf.task":
                                tmp.ImageUrl = "~/img/icons/16/tasks.png";
                                break;
                            case "ipf.note.infopathform":
                                tmp.ImageUrl = "~/img/icons/16/onenote.png";
                                break;
                            case "ipf.journal":
                                tmp.ImageUrl = "~/img/icons/16/journal.png";
                                break;
                            case "ipf.stickynote":
                                tmp.ImageUrl = "~/img/icons/16/stickynote.png";
                                break;
                            default:
                                tmp.ImageUrl = "~/img/icons/16/folder.png";
                                break;
                        }
                    }

                    n.ChildNodes.Add(tmp);
                }

                if (n.ChildNodes.Count > 0)
                    FindNode(n.ChildNodes, pf);
            }
        }

        /// <summary>
        /// Saves public folder information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPFSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfPublicFolderPath.Value))
                notification1.SetMessage(controls.notification.MessageType.Warning, "You must first select a public folder before trying to save properties.");
            else
            {
                ExchCmds powershell = null;

                try
                {
                    // Initialize the powershell module
                    powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                    // Set the public folder
                    int warningSize, prohibitSize, maxItemSize, ageLimit, keepDeletedItems = 0;

                    // Parse the values
                    int.TryParse(txtWarningSizeMB.Text, out warningSize);
                    int.TryParse(txtProhibitPostMB.Text, out prohibitSize);
                    int.TryParse(txtMaxItemSize.Text, out maxItemSize);
                    int.TryParse(txtAgeLimit.Text, out ageLimit);
                    int.TryParse(txtKeepDeletedItems.Text, out keepDeletedItems);

                    powershell.Set_PublicFolder(hfPublicFolderPath.Value, warningSize, prohibitSize, maxItemSize, ageLimit, keepDeletedItems, Config.ExchangeVersion);

                    // Show saved message
                    notification1.SetMessage(controls.notification.MessageType.Warning, "Successfully saved public folder information but was unable to modify the email settings.");

                    // Now continue to the mail section
                    if (!cbPFCurrentEmailEnabled.Checked && cbPFEnableEmail.Checked)
                    {
                        // User is enabling email on the public folder
                        if (string.IsNullOrEmpty(txtDisplayName.Text) || string.IsNullOrEmpty(txtPrimaryEmail.Text))
                            throw new Exception("You must fill out the display name and email address if you want to mail enable a public folder.");

                        powershell.Enable_MailPublicFolder(hfPublicFolderPath.Value, cbPFHiddenFromAddressLists.Checked, CPContext.SelectedCompanyCode, txtDisplayName.Text, string.Format("{0}@{1}", txtPrimaryEmail.Text.Replace(" ", string.Empty), ddlEmailDomains.SelectedValue));
                    }
                    else if (cbPFCurrentEmailEnabled.Checked && !cbPFEnableEmail.Checked)
                    {
                        // User is disabling email on the public folder
                        powershell.Disable_MailPublicFolder(hfPublicFolderPath.Value);
                    }
                    else if (cbPFCurrentEmailEnabled.Checked && cbPFEnableEmail.Checked)
                    {
                        // User is just modifing settings on the mail public folder
                        if (string.IsNullOrEmpty(txtDisplayName.Text) || string.IsNullOrEmpty(txtPrimaryEmail.Text))
                            throw new Exception("You must fill out the display name and email address if you want to change a mail enabled public folder.");

                        powershell.Set_MailPublicFolder(hfPublicFolderPath.Value, cbPFHiddenFromAddressLists.Checked, CPContext.SelectedCompanyCode, txtDisplayName.Text, string.Format("{0}@{1}", txtPrimaryEmail.Text.Replace(" ", string.Empty), ddlEmailDomains.SelectedValue));
                    }

                    // Show success message
                    notification1.SetMessage(controls.notification.MessageType.Success, "Successfully updated all public folder information.");
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, "Error saving public folder information: " + ex.Message);
                }
                finally
                {
                    if (powershell != null)
                        powershell.Dispose();

                    // Refresh the current public folder
                    GetPublicFolder(hfPublicFolderPath.Value);
                }
            }
        }
    }
}