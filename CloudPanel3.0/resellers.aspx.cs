using CloudPanel.classes;
using CloudPanel.Modules.ActiveDirectory;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using log4net;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI.WebControls;

namespace CloudPanel
{
    public partial class resellers : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if the user has rights
            if (!Authentication.IsSuperAdmin)
                Response.Redirect("~/dashboard.aspx", true);

            if (!IsPostBack)
            {
                GetResellers();
            }
        }

        /// <summary>
        /// Get list of resellers from the database
        /// </summary>
        private void GetResellers()
        {
            // Reset the panels
            panelModifyReseller.Visible = false;
            panelDeleteReseller.Visible = false;
            panelResellerList.Visible = true;

            try
            {
                List<Company> r = DbSql.GetResellers();

                // Only bind if there is data
                if (r != null && r.Count > 0)
                {
                    // Bind 
                    resellerRepeater.DataSource = r;
                    resellerRepeater.DataBind();
                }
            }
            catch (Exception ex)
            {
                this.logger.Error("Error retrieve list of resellers: " + ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Shows the add reseller panel
        /// </summary>
        protected void btnAddReseller_Click(object sender, EventArgs e)
        {
            // Hide other panels
            panelDeleteReseller.Visible = false;
            panelResellerList.Visible = false;

            // Clear out the hidden field option for the reseller
            hfModifyResellerCode.Value = string.Empty;

            // Update title
            lbEditTitle.Text = Resources.LocalizedText.Button_Add;

            // Clear all other fields
            txtCompanyName.Text = string.Empty;
            txtPointofContact.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtTelephone.Text = string.Empty;
            txtStreetAddress.Text = string.Empty;
            txtCity.Text = string.Empty;
            txtState.Text = string.Empty;
            txtZipCode.Text = string.Empty;
            txtWebsite.Text = string.Empty;

            // Show the panel
            panelModifyReseller.Visible = true;

            // Set button to show that we are adding a new reseller and not modifying
            btnSubmitReseller.Text = Resources.LocalizedText.Button_Add;
        }

        /// <summary>
        /// Cancels adding a new reseller or updating a reseller and goes
        /// back to the reseller list
        /// </summary>
        protected void btnCancelSubmitReseller_Click(object sender, EventArgs e)
        {
            // GetResellers method will also reset the view
            GetResellers();
        }

        /// <summary>
        /// Adds a new reseller to Active Directory and the
        /// database.
        /// </summary>
        protected void btnSubmitReseller_Click(object sender, EventArgs e)
        {
            // Check if we are updating or adding a new reseller
            if (string.IsNullOrEmpty(hfModifyResellerCode.Value))
            {
                // DEBUG //
                this.logger.Debug("Attempting to create new reseller " + txtCompanyName.Text);

                // Strip everything except letters
                char[] stripped = txtCompanyName.Text.Where(c => char.IsLetter(c)).ToArray();
                string strippedCompanyName = new string(stripped); // This should contain only letters and no whitespaces

                // Convert to company code
                string companyCode = strippedCompanyName.Substring(0, 3).ToUpper();

                // DEBUG //
                this.logger.Debug("Attempting to format the reseller's company name into a company code: " + txtCompanyName.Text + ", " + strippedCompanyName + ", " + companyCode);


                ADGroups groups = new ADGroups(Config.Username, Config.Password, Config.PrimaryDC);
                ADOrgUnit organizationalUnit = new ADOrgUnit(Config.Username, Config.Password, Config.PrimaryDC);
                try
                {
                    // Check to see if company code exists or not
                    bool exists = SqlCommon.DoesCompanyCodeExist(companyCode);
                    if (exists)
                    {
                        // DEBUG //
                        this.logger.Debug("Resellers converted company code already existed. Trying alternative code: " + txtCompanyName.Text + ", " + companyCode);

                        string convertedCompanyCode = companyCode;

                        // Uh Oh! Company code existed so we need to append a number and try again

                        for (int i = 0; i < 9999998; i++)
                        {
                            // Append a number at the end
                            convertedCompanyCode = companyCode + i.ToString();

                            // DEBUG //
                            this.logger.Debug("Try number " + i.ToString() + ". New Company Code: " + convertedCompanyCode);

                            // Check if it exists
                            exists = SqlCommon.DoesCompanyCodeExist(convertedCompanyCode);
                            if (!exists)
                            {
                                // New company code doesn't exist. Replace the companyCode string
                                // with the new company code and break out of the loop
                                companyCode = convertedCompanyCode;
                                break;
                            }
                            else
                            {
                                // If we got here then you have a CRAP load of companies and probably
                                // should pay me for CloudPanel haha
                                if (i >= 9999998)
                                    throw new Exception("Unable to find a company code. Last tried was: " + convertedCompanyCode);
                            }
                        }
                    }

                    // DEBUG //
                    this.logger.Debug("Found an available reseller code! New Code: " + companyCode);

                    // Now that we have gotten here we apparently have found a company code 
                    // we can use. So lets create our Company object
                    Company companyInfo = new Company();
                    companyInfo.CompanyCode = companyCode;
                    companyInfo.CompanyName = txtCompanyName.Text;
                    companyInfo.AdministratorsName = txtPointofContact.Text;
                    companyInfo.AdministratorsEmail = txtEmail.Text;
                    companyInfo.PhoneNumber = txtTelephone.Text;
                    companyInfo.Street = txtStreetAddress.Text;
                    companyInfo.City = txtCity.Text;
                    companyInfo.State = txtState.Text;
                    companyInfo.ZipCode = txtZipCode.Text;
                    companyInfo.Country = ddlCountry.SelectedValue;
                    companyInfo.Website = txtWebsite.Text;
                    companyInfo.Description = "Reseller";
                    companyInfo.DistinguishedName = "OU=" + companyCode + "," + Config.HostingOU;

                    // DEBUG //
                    this.logger.Debug("Creating organizational unit for reseller " + txtCompanyName.Text);

                    // Now lets create our organizational unit
                    organizationalUnit.CreateOU(Config.HostingOU, companyInfo);

                    // DEBUG //
                    this.logger.Debug("Creating reseller groups required by CloudPanel");

                    groups.Create(companyInfo.DistinguishedName, "GPOAccess@" + companyInfo.CompanyCode, "", true, ADGroups.GroupType.Global);
                    this.logger.Debug("Successfully created GPOAccess@" + companyInfo.CompanyCode + " in path " + companyInfo.DistinguishedName);

                    groups.Create(companyInfo.DistinguishedName, "ResellerAdmins@" + companyInfo.CompanyCode, "", true, ADGroups.GroupType.Global);
                    this.logger.Debug("Successfully created ResellerAdmins@" + companyInfo.CompanyCode + " in path " + companyInfo.DistinguishedName);

                    groups.ModifyMembership("GPOAccess@Hosting", "GPOAccess@" + companyInfo.CompanyCode, IdentityType.Name, IdentityType.Name, false);
                    this.logger.Debug("Successfully added GPOAccess@" + companyInfo.CompanyCode + " to group GPOAccess@Hosting");
                    
                    // Add the GPOAcess group rights to the OU
                    organizationalUnit.AddGPOAccessRights(companyInfo.DistinguishedName, "GPOAccess@" + companyInfo.CompanyCode);
                    
                    // DEBUG //
                    this.logger.Debug("Attempting to insert into SQL: " + txtCompanyName.Text + ", " + companyCode);

                    // Good! No Errors so lets insert this into SQL
                    DbSql.AddReseller(companyInfo);

                    // DEBUG //
                    this.logger.Debug("Successfully created new reseller: " + txtCompanyName.Text);

                    // Update notification saying that we created it
                    notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.Reseller_AddedSuccessfully);

                    // Reset View
                    GetResellers();
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                    // ERROR //
                    this.logger.Error("There was an error creating reseller " + txtCompanyName.Text + ": " + ex.ToString());
                }
                finally
                {
                    if (groups != null)
                        groups.Dispose();

                    if (organizationalUnit != null)
                        organizationalUnit.Dispose();
                }
            }
            else
            {
                // We are updating a reseller and not creating a new one so we need to create our object
                // and update it in the database (the companycode cannot be changed
                Company companyInfo = new Company();
                companyInfo.CompanyCode = hfModifyResellerCode.Value;
                companyInfo.CompanyName = txtCompanyName.Text;
                companyInfo.AdministratorsName = txtPointofContact.Text;
                companyInfo.AdministratorsEmail = txtEmail.Text;
                companyInfo.PhoneNumber = txtTelephone.Text;
                companyInfo.Street = txtStreetAddress.Text;
                companyInfo.City = txtCity.Text;
                companyInfo.State = txtState.Text;
                companyInfo.ZipCode = txtZipCode.Text;
                companyInfo.Country = ddlCountry.SelectedValue;
                companyInfo.Website = txtWebsite.Text;
                companyInfo.Description = "Reseller";

                try
                {
                    // DEBUG //
                    this.logger.Debug("Attempting to update reseller " + companyInfo.CompanyName + " information.");

                    // Update the database
                    DbSql.UpdateReseller(companyInfo);

                    // DEBUG //
                    this.logger.Debug("Successfully updated reseller " + companyInfo.CompanyName + " information.");

                    // Show to the user we successfully updated
                    notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.Reseller_UpdatedSuccessfully);

                    // Retrieve list of resellers
                    GetResellers();
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                    // ERROR //
                    this.logger.Error("Error updating reseller " + companyInfo.CompanyName + ": " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Repeater item commands to edit or delete a reseller
        /// </summary>
        protected void resellerRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectReseller")
            {
                // Split the command argument into two strings
                string[] split = e.CommandArgument.ToString().Split(new char[] { '|' });

                // INFO //
                this.logger.Debug("User " + HttpContext.Current.User.Identity.Name + " selected reseller " + split[1]);
                
                // Set the reseller session variable
                CPContext.SelectedResellerCode = split[0];
                CPContext.SelectedResellerName = split[1];

                // Redirect to companies
                HttpContext.Current.Response.Redirect("~/companies.aspx", false);
            }
            else if (e.CommandName == "EditReseller")
            {
                // Hide panels not needed
                panelDeleteReseller.Visible = false;
                panelResellerList.Visible = false;

                // Show the modify reseller panel
                panelModifyReseller.Visible = true;

                // Change the button text
                btnSubmitReseller.Text = Resources.LocalizedText.Button_Save;

                // Split the command argument into two strings
                string[] split = e.CommandArgument.ToString().Split(new char[] { '|' });

                try
                {
                    // INFO //
                    this.logger.Info("User " + HttpContext.Current.User.Identity.Name + " entered the editing section for reseller " + split[1]);

                    Company companyInfo = DbSql.GetReseller(split[0]);
                    
                    // Update the title
                    lbEditTitle.Text = split[1];

                    // Add values to fields
                    hfModifyResellerCode.Value = split[0];
                    txtCompanyName.Text = companyInfo.CompanyName;
                    txtPointofContact.Text = companyInfo.AdministratorsName;
                    txtEmail.Text = companyInfo.AdministratorsEmail;
                    txtTelephone.Text = companyInfo.PhoneNumber;
                    txtStreetAddress.Text = companyInfo.Street;
                    txtCity.Text = companyInfo.City;
                    txtState.Text = companyInfo.State;
                    txtZipCode.Text = companyInfo.ZipCode.ToString();
                    ddlCountry.SelectedValue = companyInfo.Country;
                    txtWebsite.Text = companyInfo.Website;
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                    // ERROR //
                    this.logger.Error("Error retrieving the reseller information for editing: " + ex.ToString());
                }
            }
            else if (e.CommandName == "DeleteReseller")
            {
                // Set the DN field
                hfDeleteReseller.Value = e.CommandArgument.ToString();

                // Show the warning panel
                panelDeleteReseller.Visible = true;

                // Hide other panels
                panelModifyReseller.Visible = false;
                panelResellerList.Visible = false;
            }
        }

        /// <summary>
        /// Cancels deleting a reseller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDeleteCancel_Click(object sender, EventArgs e)
        {
            GetResellers();
        }

        /// <summary>
        /// Deletes the current selected reseller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDeleteYes_Click(object sender, EventArgs e)
        {
            string resellerCompanyCode = hfDeleteReseller.Value;

            // Try to delete the reseller
            StartDeletingReseller(resellerCompanyCode);
        }

        private void StartDeletingReseller(string resellerCode)
        {
            ADOrgUnit org = null;

            try
            {
                // Initialize 
                org = new ADOrgUnit(Config.Username, Config.Password, Config.PrimaryDC);

                // Get the reseller information
                Company resellerInfo = DbSql.GetReseller(resellerCode);

                // Check SQL to make sure there are no companies
                List<Company> companies = SQLCompanies.GetCompanies(resellerCode);
                if (companies != null && companies.Count > 0)
                    throw new Exception("You cannot delete this reseller because it contains a total of " + companies.Count.ToString() + " companies.");

                // Check Active Directory for OU's that exist under this reseller
                List<Company> foundCompanies = org.GetChildOUs(resellerInfo.DistinguishedName);
                if (foundCompanies.Count > 0)
                {
                    // DEBUG //
                    this.logger.Debug("Unable to delete reseller " + resellerInfo.CompanyCode + " due to organizational units existing under the reseller:");
                    foreach (Company c in foundCompanies)
                    {
                        this.logger.Debug("OU found under reseller: " + c.DistinguishedName);
                    }

                    throw new Exception("Although we did not find any companies under this reseller in CloudPanel, we have found that the reseller organizational unit contains company OU's. Please move or remove these before trying to delete the reseller.");
                }

                // Delete the OU since nothing is under it
                org.DeleteOUAll(resellerInfo.DistinguishedName);

                // We should be good here to delete the reseller!
                DbSql.DeleteReseller(resellerInfo.CompanyCode);

                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Success, "Successfully deleting the reseller: " + resellerInfo.CompanyCode);
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error deleting reseller: " + ex.Message);
            }
            finally
            {
                if (org != null)
                    org.Dispose();

                // Change the view back to the reseller list
                GetResellers();
            }
        }
    }
}