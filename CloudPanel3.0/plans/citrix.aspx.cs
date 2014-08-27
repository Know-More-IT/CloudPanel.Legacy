using CloudPanel.Modules.ActiveDirectory;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CloudPanel.plans
{
    public partial class citrix : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Get a list of companies
                GetAllCompanies();

                // Get a list of plans
                GetAllPlans();
            }
        }

        /// <summary>
        /// Gets all companies from the database
        /// </summary>
        private void GetAllCompanies()
        {
            try
            {
                List<Company> companies = DbSql.Get_Companies();

                // Clear field
                ddlSpecificCompany.Items.Clear();
                ddlSpecificCompany.Items.Add("Not Selected");

                // Add all companies to the list
                if (companies != null)
                {
                    foreach (Company c in companies)
                    {
                        ListItem item = new ListItem();
                        item.Text = c.CompanyName;
                        item.Value = c.CompanyCode;

                        ddlSpecificCompany.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Failed to retrieve companies from database: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets all the plans from the database
        /// </summary>
        private void GetAllPlans()
        {
            try
            {
                // Reset View
                panelAddEditApp.Visible = false;
                panelViewApps.Visible = true;

                List<BaseCitrixApp> citrixApps = SQLCitrix.GetCitrixPlans();

                // Bind all virtual applications
                gridViewVirtApps.DataSource = citrixApps.Where(x => x.IsServer == false);
                gridViewVirtServers.DataSource = citrixApps.Where(x => x.IsServer == true);

                // Clear edit because we do not edit grid view
                gridViewVirtApps.EditIndex = -1;
                gridViewVirtServers.EditIndex = -1;

                // Refresh grid view
                gridViewVirtApps.DataBind();
                gridViewVirtServers.DataBind();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Unable to get plans: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets a specific plan from the database
        /// </summary>
        /// <param name="id"></param>
        private void GetPlan(string id)
        {
            try
            {
                BaseCitrixApp citrixApp = SQLCitrix.GetCitrixPlan(id);

                if (citrixApp == null || string.IsNullOrEmpty(citrixApp.DisplayName))
                    throw new Exception("Could not find plan in the database. Please contact support.");

                // Set values
                hfCitrixPlanID.Value = id;
                txtDisplayName.Text = citrixApp.DisplayName;
                txtDescription.Text = citrixApp.Description;
                txtCost.Text = citrixApp.Cost;
                txtPrice.Text = citrixApp.Price;
                cbIsServer.Checked = citrixApp.IsServer;

                if (!string.IsNullOrEmpty(citrixApp.CompanyCode))
                    ddlSpecificCompany.SelectedValue = citrixApp.CompanyCode;
                else
                    ddlSpecificCompany.SelectedIndex = 0;

                // Disable changing the assigned company
                ddlSpecificCompany.Enabled = false;

                // Show panel
                panelAddEditApp.Visible = true;
                panelViewApps.Visible = false;

                // Disable checkbox
                cbCreateGroups.Checked = false;
                cbCreateGroups.Enabled = false;
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error getting plan information: " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes a plan from the database
        /// </summary>
        /// <param name="id"></param>
        private void DeletePlan(string id)
        {
            ADGroups group = null;

            try
            {
                CitrixPlan foundPlan = DbSql.Get_CitrixPlan(int.Parse(id));
                if (foundPlan != null)
                {
                    group = new ADGroups(Config.Username, Config.Password, Config.PrimaryDC);

                    // Check if it is for a specific company or not
                    if (string.IsNullOrEmpty(foundPlan.CompanyCode))
                    {
                        // This is a public application. Need to delete the group and all members
                        group.Delete(foundPlan.GroupName, false, true);
                    }
                    else
                    {
                        // This is a private application. Just need to delete the one group
                        group.Delete(foundPlan.GroupName, false, false);
                    }

                    // Remove from database
                    DbSql.Delete_CitrixPlan(int.Parse(id));
                }
                
                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Success, "Successfully removed " + foundPlan.DisplayName + " from Active Directory and the database.");
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error getting plan information: " + ex.ToString());
            }
            finally
            {
                if (group != null)
                    group.Dispose();

                GetAllPlans();
            }
        }

        /// <summary>
        /// Show the create plan section
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddCitrixApp_Click(object sender, EventArgs e)
        {
            // Clear hidden field
            hfCitrixPlanID.Value = null;

            // Enable company code drop down
            ddlSpecificCompany.Enabled = true;
            ddlSpecificCompany.SelectedIndex = 0;

            // Show panel
            panelViewApps.Visible = false;
            panelAddEditApp.Visible = true;

            // Enable checkbox
            cbCreateGroups.Enabled = true;
            cbCreateGroups.Checked = true;

            // Clear all values
            txtDisplayName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtCost.Text = "0.00";
            txtPrice.Text = "0.00";
        }

        /// <summary>
        /// Cancels creating or updating and reverts back to main view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Go back to the view citrix plan section
            GetAllPlans();
        }

        /// <summary>
        /// Creates or updates a citrix application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Create object
            CitrixPlan plan = new CitrixPlan();
            plan.DisplayName = txtDisplayName.Text;
            plan.Description = txtDescription.Text;
            plan.Cost = txtCost.Text;
            plan.Price = txtPrice.Text;
            plan.IsServer = cbIsServer.Checked;
            plan.CompanyCode = ddlSpecificCompany.SelectedIndex > 0 ? ddlSpecificCompany.SelectedValue : "";
            
            ADGroups groups = null;
            try
            {

                //
                // Save the picture for this specific application
                //
                string guid = Guid.NewGuid().ToString();
                if (fileUploadImg.HasFile && fileUploadImg.PostedFile != null)
                {
                    // Get Extension
                    string fileExt = Path.GetExtension(fileUploadImg.FileName);

                    // Save File
                    fileUploadImg.SaveAs(Server.MapPath("~/img/icons/Citrix/") + guid + fileExt);

                    // Set picture URL
                    plan.PictureUrl = "~/img/icons/Citrix/" + guid + fileExt;
                }

                //
                // Check and see if are updating an existing application or creating a new one
                //
                if (!string.IsNullOrEmpty(hfCitrixPlanID.Value))
                {
                    // We are updating an application
                    int existingPlanId = int.Parse(hfCitrixPlanID.Value);
                    plan.PlanID = existingPlanId;

                    // Save
                    DbSql.Save_CitrixPlan(plan);
                }
                else
                {
                    // Initialize 
                    groups = new ADGroups(Config.Username, Config.Password, Config.PrimaryDC);
                    if (cbCreateGroups.Checked)
                    {
                        string groupName = string.Empty;
                        string selectedCompanyCode = plan.CompanyCode;

                        if (string.IsNullOrEmpty(selectedCompanyCode))
                        {
                            groupName = string.Format("{0}@Hosting", plan.DisplayName.Replace(" ", string.Empty));
                            this.logger.Debug("Group name for new application is now " + groupName);
                        }
                        else
                        {
                            groupName = string.Format("{0}@{1}", plan.DisplayName.Replace(" ", string.Empty), selectedCompanyCode.Replace(" ", string.Empty));
                            this.logger.Debug("Group name for new application is now " + groupName);
                        }

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
                            this.logger.Debug("When evaluating to add users to the Citrix application " + plan.DisplayName + ", we discovered the name of the security group would be beyong 64 characters. The new name is now: " + groupName);
                        }

                        // Set the group name
                        plan.GroupName = groupName;

                        // Now create the group
                        if (string.IsNullOrEmpty(selectedCompanyCode))
                            groups.Create(Retrieve.GetApplicationsOU, plan.GroupName, plan.Description, true, ADGroups.GroupType.Global);
                        else
                        {
                            // This is for a specific company so we are going to place it in the company's application OU instead
                            // of the main Hosting applications OU
                            if (!Config.ResellersEnabled)
                            {
                                // Resellers are not enabled so we need to modify the path
                                this.logger.Debug("Resellers are not enabled and the application " + plan.DisplayName + " is being created for a specific company. Placing it in the company's Applications OU");
                                groups.Create(string.Format("OU=Applications,OU={0},{1}", selectedCompanyCode, Config.HostingOU), plan.GroupName, plan.Description, true, ADGroups.GroupType.Global);
                            
                                // Now add the group to a member of the AllTSUsers@CompanyCode group
                                groups.ModifyMembership("AllTSUsers@" + selectedCompanyCode, plan.GroupName, System.DirectoryServices.AccountManagement.IdentityType.Name, System.DirectoryServices.AccountManagement.IdentityType.Name, false);
                            }
                            else
                            {
                                this.logger.Debug("Resellers are enabled and the application " + plan.DisplayName + " is being created for a specific company. Placing it in the company's Applications OU");

                                // Resellers are enabled so we have to query SQL for the path to the company
                                Company c = DbSql.Get_Company(selectedCompanyCode);

                                this.logger.Debug("Found company's distinguished name: " + c.DistinguishedName);

                                groups.Create(string.Format("OU=Applications,{0}", c.DistinguishedName), plan.GroupName, plan.Description, true, ADGroups.GroupType.Global);

                                // Now add the group to a member of the AllTSUsers@CompanyCode group
                                groups.ModifyMembership("AllTSUsers@" + selectedCompanyCode, plan.GroupName, System.DirectoryServices.AccountManagement.IdentityType.Name, System.DirectoryServices.AccountManagement.IdentityType.Name, false);
                            }
                        }
                    }
                    else
                        plan.GroupName = txtGroupName.Text;


                    // Add to SQL
                    DbSql.New_CitrixPlan(plan);

                    // Update notification
                    notification1.SetMessage(controls.notification.MessageType.Success, "Successfully created new Citrix application " + plan.DisplayName);
                }
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Error saving or updating Citrix Plan", ex);

                // Set message
                notification1.SetMessage(controls.notification.MessageType.Error, "There was an error creating the citrix application: " + ex.Message);
            }
            finally
            {
                if (groups != null)
                    groups.Dispose();

                // Refresh the view
                GetAllPlans();
            }
        }


        #region Grid View Renders

        protected void gridViewVirtServers_PreRender(object sender, EventArgs e)
        {
            if (gridViewVirtServers.Rows.Count > 0)
            {
                gridViewVirtServers.UseAccessibleHeader = true;
                gridViewVirtServers.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        protected void gridViewVirtApps_PreRender(object sender, EventArgs e)
        {
            if (gridViewVirtApps.Rows.Count > 0)
            {
                gridViewVirtApps.UseAccessibleHeader = true;
                gridViewVirtApps.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        protected void gridViewVirtServers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            if (e.CommandName == "Edit")
            {
                GetPlan(id);
            }
            else if (e.CommandName == "Delete")
            {
                DeletePlan(id);
            }
        }

        protected void gridViewVirtApps_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            if (e.CommandName == "Edit")
            {
                GetPlan(id);
            }
            else if (e.CommandName == "Delete")
            {
                DeletePlan(id);
            }
        }

        protected void gridViewVirtServers_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }

        protected void gridViewVirtServers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void gridViewVirtApps_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }

        protected void gridViewVirtApps_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        #endregion

    }
}