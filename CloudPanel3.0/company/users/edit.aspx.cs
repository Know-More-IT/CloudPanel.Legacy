using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Sql;
using log4net;
using System.Reflection;
using CloudPanel.Modules.ActiveDirectory;
using CloudPanel.Modules.Base.Class;

namespace CloudPanel.company.users
{
    public partial class edit : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Populate users
                GetUsers();

                // Populate the domains drop down box
                try
                {
                    List<Domain> domains = SQLDomains.GetDomains(CPContext.SelectedCompanyCode);
                    ddlLoginDomain.DataTextField = "DomainName";
                    ddlLoginDomain.DataValueField = "DomainName";
                    ddlLoginDomain.DataSource = domains;
                    ddlLoginDomain.DataBind();
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                    // Log Error //
                    this.logger.Error("Unable to retrieve domains for company code " + CPContext.SelectedCompanyCode, ex);
                }
            }

            // Hide user rights section based on security
            if (Master.IsSuperAdmin || Master.IsResellerAdmin)
            {
                companyAdminPermissions.Visible = true;
                editCompanyAdminPermissions.Visible = true;
            }
            else
            {
                companyAdminPermissions.Visible = false;
                editCompanyAdminPermissions.Visible = false;
            }
        }

        /// <summary>
        /// Gets list of users for the selected company
        /// </summary>
        private void GetUsers()
        {
            string companyCode = CPContext.SelectedCompanyCode;

            // Hide other panels except the users panel
            panelCreateUser.Visible = false;
            panelEditUser.Visible = false;
            panelDeleteUser.Visible = false;

            // Show users panel
            panelUsers.Visible = true;

            try
            {
                // Get list of users
                List<BaseUser> users = SQLUsers.GetUsers(companyCode);

                repeater.DataSource = users;
                repeater.DataBind();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                // Log Error //
                this.logger.Error("Error retrieving list of users for company code " + companyCode, ex);
            }
        }

        /// <summary>
        /// Cancels out of the create user panel and shows the users panel
        /// </summary>
        protected void btnSaveUserCancel_Click(object sender, EventArgs e)
        {
            // Repopulate Users
            GetUsers();
        }

        /// <summary>
        /// Create new user
        /// </summary>
        protected void btnSaveUser_Click(object sender, EventArgs e)
        {
            ADUsers users = new ADUsers(Config.Username, Config.Password, Config.PrimaryDC);

            try
            {
                // Create our user object
                ADUser user = new ADUser();
                user.CompanyCode = CPContext.SelectedCompanyCode;
                user.Firstname = txtFirstName.Text.Trim();
                user.Middlename = txtMiddleName.Text.Trim();
                user.Lastname = txtLastName.Text.Trim();
                user.DisplayName = txtDisplayName.Text.Trim();
                user.Department = txtDepartment.Text.Trim();
                user.UserPrincipalName = txtLoginName.Text.Replace(" ", string.Empty) + "@" + ddlLoginDomain.SelectedValue;
                user.IsCompanyAdmin = cbIsCompanyAdministrator.Checked;
                user.IsResellerAdmin = cbIsResellerAdministrator.Checked;
                user.PasswordNeverExpires = cbPasswordNeverExpires.Checked;
                
                // Check if we are using custom name attribute
                if (Config.CustomNameAttribute)
                    user.Name = txtFullName.Text.Trim();
                else
                    user.Name = string.Empty; // Set to empty so our class knows to use UPN instead 

                // Create our user
                ADUser returnedUser = users.Create(user, Retrieve.GetCompanyUsersOU, txtPassword1.Text);
            
                // Insert into SQL
                SQLUsers.AddUser(returnedUser);

                // Modify the users permissions if they are a company admin
                ModifyAdminPermissions(user.UserPrincipalName, cbIsCompanyAdministrator.Checked, true);

                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.NotificationSuccessCreateUser + " " + user.DisplayName + "!");

                // Refresh View
                GetUsers();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                // Log Error //
                this.logger.Error("There was an error creating an new user for company " + CPContext.SelectedCompanyName, ex);
            }
            finally
            {
                users.Dispose();
            }
        }

        /// <summary>
        /// Repeater item command to edit or delete a user
        /// </summary>
        protected void repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EditUser")
            {
                // Retrieve information
                GetUser(e.CommandArgument.ToString());
            }
            else if (e.CommandName == "DeleteUser")
            {
                // Split argument
                string[] split = e.CommandArgument.ToString().Split(';');

                lbDeleteDisplayName.Text = split[0];
                hfDeleteUserPrincipalName.Value = split[1];

                // Hide panels
                panelCreateUser.Visible = false;
                panelEditUser.Visible = false;
                panelUsers.Visible = false;

                // Show panel
                panelDeleteUser.Visible = true;
            }
        }

        /// <summary>
        /// Gets the user information for displaying
        /// </summary>
        /// <param name="userPrincipalName"></param>
        private void GetUser(string userPrincipalName)
        {
            ADUsers user = null;

            try
            {
                user = new ADUsers(Config.Username, Config.Password, Config.PrimaryDC);
                
                // Get User Details
                ADUser u = user.GetUser(userPrincipalName, CPContext.SelectedCompanyCode);

                // Set values
                txtEditLoginName.Text = userPrincipalName;
                txtEditFirstName.Text = u.Firstname;
                txtEditMiddleName.Text = u.Middlename;
                txtEditLastName.Text = u.Lastname;
                txtEditDisplayName.Text = u.DisplayName;
                txtEditDepartment.Text = u.Department;
                txtEditSamAccountName.Text = u.SamAccountName;
                cbEnableUser.Checked = u.IsEnabled;
                cbEditCompanyAdmin.Checked = u.IsCompanyAdmin;
                cbEditPwdNeverExpires.Checked = u.PasswordNeverExpires;

                // Check if the account is locked out
                if (u.IsLockedOut)
                {
                    cbUserLockedOut.Checked = true;
                    lnkUnlockAccount.Visible = true;
                }
                else
                {
                    cbUserLockedOut.Checked = false;
                    lnkUnlockAccount.Visible = false;
                }

                // Get extra details from SQL
                ADUser sqlUser = DbSql.Get_User(userPrincipalName);

                // Uncheck the checkboxes
                foreach (ListItem item in cbEditCompanyAdminPermissions.Items)
                        item.Selected = false;

                if (sqlUser.IsCompanyAdmin)
                {
                    editCompanyAdminPermissions.Style.Remove("display");

                    cbEditCompanyAdminPermissions.Items[0].Selected = sqlUser.EnableExchangePermission;
                    cbEditCompanyAdminPermissions.Items[1].Selected = sqlUser.DisableExchangePermission;
                    cbEditCompanyAdminPermissions.Items[2].Selected = sqlUser.AddDomainPermission;
                    cbEditCompanyAdminPermissions.Items[3].Selected = sqlUser.DeleteDomainPermission;
                    cbEditCompanyAdminPermissions.Items[4].Selected = sqlUser.ModifyAcceptedDomainPermission;
                    cbEditCompanyAdminPermissions.Items[5].Selected = sqlUser.ImportUsersPermission;
                }
                else
                    editCompanyAdminPermissions.Style.Add("display", "none");

                // Show the correct panels
                panelEditUser.Visible = true;
                panelCreateUser.Visible = false;
                panelDeleteUser.Visible = false;
                panelUsers.Visible = false;

                // Enable save user button
                btnEditSaveUser.Enabled = true;

                // Set default button
                this.Form.DefaultButton = this.btnEditSaveUser.UniqueID;
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                // Disable save user button
                btnSaveUser.Enabled = false;
            }
            finally
            {
                if (user != null)
                    user.Dispose();
            }
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        protected void btnEditSaveUser_Click(object sender, EventArgs e)
        {
            ADUsers users = new ADUsers(Config.Username, Config.Password, Config.PrimaryDC);

            try
            {
                // Create our user object
                ADUser user = new ADUser();
                user.UserPrincipalName = txtEditLoginName.Text;
                user.SamAccountName = txtEditSamAccountName.Text;
                user.CompanyCode = CPContext.SelectedCompanyCode;
                user.Firstname = txtEditFirstName.Text;
                user.Middlename = txtEditMiddleName.Text;
                user.Lastname = txtEditLastName.Text;
                user.DisplayName = txtEditDisplayName.Text;
                user.Department = txtEditDepartment.Text;
                user.IsCompanyAdmin = cbEditCompanyAdmin.Checked;
                user.IsResellerAdmin = cbEditResellerAdmin.Checked;
                user.IsEnabled = cbEnableUser.Checked;
                user.PasswordNeverExpires = cbEditPwdNeverExpires.Checked;

                if (!string.IsNullOrEmpty(txtEditPwd1.Text))
                    user.Password = txtEditPwd1.Text;

                // Update our user in Active Directory
                users.Edit(user);

                // Update SQL
                DbSql.Update_User(user);

                // Do not save if they are a super admin or reseller admin or company admin permissions were removed
                if (Master.IsSuperAdmin || Master.IsResellerAdmin || !cbEditCompanyAdmin.Checked)
                {
                    // Modify the users permissions if they are a company admin
                    ModifyAdminPermissions(user.UserPrincipalName, cbEditCompanyAdmin.Checked, false);
                }

                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.NotificationSuccessUpdateUser + user.DisplayName + "!");

                // Refresh View
                GetUsers();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                // Log Error //
                this.logger.Error("There was an error updating a user for company " + CPContext.SelectedCompanyName, ex);
            }
            finally
            {
                if (users != null)
                    users.Dispose();
            }
        }

        /// <summary>
        /// Clears the fields
        /// </summary>
        private void ClearFields()
        {
            // Clear create new user fields
            txtFirstName.Text = string.Empty;
            txtMiddleName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtDisplayName.Text = string.Empty;
            txtDepartment.Text = string.Empty;
            txtLoginName.Text = string.Empty;
            cbIsCompanyAdministrator.Checked = false;
            cbIsResellerAdministrator.Checked = false;
            cbUserLockedOut.Checked = false;
            lnkUnlockAccount.Visible = false;

            // Uncheck admin permissions
            foreach (ListItem item in cbCompanyAdminPermissions.Items)
                item.Selected = false;
        }

        /// <summary>
        /// Deletes a user from Active Directory and SQL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDeleteYes_Click(object sender, EventArgs e)
        {
            // Proceed with deleting the user
            ADUsers user = null;

            try
            {
                // Delete from AD
                user = new ADUsers(Config.Username, Config.Password, Config.PrimaryDC);
                user.Delete(hfDeleteUserPrincipalName.Value);

                // Delete from SQL
                SQLUsers.DeleteUser(hfDeleteUserPrincipalName.Value, CPContext.SelectedCompanyCode);

                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.NotificationSuccessDeleteUser + lbDeleteDisplayName.Text);
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
            finally
            {
                if (user != null)
                    user.Dispose();

                // Change back to users view
                GetUsers();
            }
        }

        /// <summary>
        /// Cancels deleting a user and refresh the view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDeleteCancel_Click(object sender, EventArgs e)
        {
            // Go back to the users screen
            GetUsers();
        }

        /// <summary>
        /// Unlocks the user account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkUnlockAccount_Click(object sender, EventArgs e)
        {
            ADUsers user = null;
            try
            {
                // Initialize object
                user = new ADUsers(Config.Username, Config.Password, Config.PrimaryDC);

                // Unlock the account
                user.UnlockAccount(txtEditLoginName.Text);

                // Change the checkbox and link
                cbUserLockedOut.Checked = false;
                lnkUnlockAccount.Visible = false;

                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Success, "Successfully unlocked account " + txtEditLoginName.Text);
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error unlocking account: " + ex.Message);
            }
            finally
            {
                if (user != null)
                    user.Dispose();
            }
        }

        /// <summary>
        /// Modifies the administrator's permissions
        /// </summary>
        /// <param name="userPrincipalName"></param>
        private void ModifyAdminPermissions(string userPrincipalName, bool isCompanyAdmin, bool creatingNewUser)
        {
            try
            {
                ADUser user = DbSql.Get_User(userPrincipalName);

                // Set the permissions
                user.EnableExchangePermission = cbEditCompanyAdminPermissions.Items[0].Selected;
                user.DisableExchangePermission = cbEditCompanyAdminPermissions.Items[1].Selected;
                user.AddDomainPermission = cbEditCompanyAdminPermissions.Items[2].Selected;
                user.DeleteDomainPermission = cbEditCompanyAdminPermissions.Items[3].Selected;
                user.ModifyAcceptedDomainPermission = cbEditCompanyAdminPermissions.Items[4].Selected;
                user.ImportUsersPermission = cbEditCompanyAdminPermissions.Items[5].Selected;

                // Check if they are a company admin
                if (!user.IsCompanyAdmin)
                {
                    // Disable all permissions
                    user.EnableExchangePermission = false;
                    user.DisableExchangePermission = false;
                    user.AddDomainPermission = false;
                    user.DeleteDomainPermission = false;
                    user.ModifyAcceptedDomainPermission = false;
                    user.ImportUsersPermission = false;
                }

                // Save
                DbSql.Update_UserPermissions(user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Shows the create user mailbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            // Hide all other panels
            panelUsers.Visible = false;

            // Show create user panel
            panelCreateUser.Visible = true;

            // Set default button
            this.Form.DefaultButton = this.btnSaveUser.UniqueID;

            // Clear fieelds
            ClearFields();
        }
    }
}