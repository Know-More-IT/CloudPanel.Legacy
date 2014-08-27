using CloudPanel.classes;
using CloudPanel.Modules.ActiveDirectory;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace CloudPanel
{
    public partial class companies : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if the user has selected a reseller and has rights
            if (Authentication.IsSuperAdmin || Authentication.IsResellerAdmin)
            {
                if (string.IsNullOrEmpty(CloudPanel.Modules.Settings.CPContext.SelectedResellerCode))
                    Response.Redirect("~/resellers.aspx", true);
                if (!IsPostBack)
                {
                    // Get a list of companies
                    GetCompanies();
                }
            }
            else
                Response.Redirect("~/dashboard.aspx", true);
        }

        /// <summary>
        /// Gets a list of companies from the database
        /// </summary>
        private void GetCompanies()
        {
            // Reset the panels
            panelCompanyEdit.Visible = false;
            panelCompanyDelete.Visible = false;
            panelCompanyList.Visible = true;

            string resellerCode = CPContext.SelectedResellerCode;

            try
            {
                // DEBUG //
                this.logger.Debug("User " + CloudPanel.Modules.Settings.CPContext.LoggedInUserName + " is retrieving a list of companies for reseller " + resellerCode);

                // Get list of companies
                List<Company> companyList = SQLCompanies.GetCompanies(resellerCode);

                // Bind to repeater
                companyRepeater.DataSource = companyList;
                companyRepeater.DataBind();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                // ERROR //
                this.logger.Error("Error trying to retrieve a list of companies for reseller code " + resellerCode + ": " + ex.ToString());
            }

        }

        /// <summary>
        /// Shows the Add Company section
        /// </summary>
        protected void btnAddCompany_Click(object sender, EventArgs e)
        {
            // Hide other panels
            panelCompanyDelete.Visible = false;
            panelCompanyList.Visible = false;

            // Clear out the hidden field option for the company
            hfModifyCompanyCode.Value = string.Empty;

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
            txtDomainName.Text = string.Empty;
            txtDomainName.ReadOnly = false;

            // Show the panel
            panelCompanyEdit.Visible = true;

            // Set button to show that we are adding a new company and not modifying
            btnSubmitCompany.Text = Resources.LocalizedText.Save;
        }

        /// <summary>
        /// Reverts back to the main screen
        /// </summary>
        protected void btnCancelSubmitCompany_Click(object sender, EventArgs e)
        {
            GetCompanies();
        }

        /// <summary>
        /// Selects or edits a certain company
        /// </summary>
        protected void companyRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectCompany")
            {
                // Split the command argument into two strings
                string[] split = e.CommandArgument.ToString().Split(new char[] { '|' });

                // INFO //
                this.logger.Info("User " + HttpContext.Current.User.Identity.Name + " selected company " + split[1]);

                // Set the reseller session variable
                CPContext.SelectedCompanyCode = split[0];
                CPContext.SelectedCompanyName = split[1];

                // Redirect to companies
                HttpContext.Current.Response.Redirect("~/company/overview.aspx", false);
            }
            else if (e.CommandName == "EditCompany")
            {
                // Hide panels not needed
                panelCompanyDelete.Visible = false;
                panelCompanyList.Visible = false;

                // Show the modify company panel
                panelCompanyEdit.Visible = true;

                // Place a null value in the domain box
                txtDomainName.Text = "can.not.change.com";

                // Change the button text
                btnSubmitCompany.Text = Resources.LocalizedText.Save;

                // Split the command argument into two strings
                string[] split = e.CommandArgument.ToString().Split('|');

                try
                {
                    // INFO //
                    this.logger.Info("User " + HttpContext.Current.User.Identity.Name + " entered the editing section for company " + split[1]);

                    Company companyInfo = SQLCompanies.GetCompany(split[0]);

                    // Update the title
                    lbEditTitle.Text = split[1];

                    // Add values to fields
                    hfModifyCompanyCode.Value = split[0];
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

                    // Set the value to read only
                    txtDomainName.ReadOnly = true;
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                    // ERROR //
                    this.logger.Error("Error retrieving the company information for editing: " + ex.ToString());
                }
            }
            else if (e.CommandName.Equals("DeleteCompany"))
            {
                try
                {
                    // Reset field
                    hfDeleteCompanyCode.Value = string.Empty;

                    // INFO //
                    this.logger.Info("User " + HttpContext.Current.User.Identity.Name + " selected to delete company " + e.CommandArgument.ToString());
                
                    // Get company information from SQL
                    Company companyInfo = SQLCompanies.GetCompany(e.CommandArgument.ToString());

                    // Check if the company is enabled for Exchange
                    if (companyInfo.ExchangeEnabled)
                        throw new Exception(Resources.LocalizedText.DeleteCompanyExchEnabled);

                    // Set information
                    lbDeleteCompanyDN.Text = companyInfo.DistinguishedName;
                    hfDeleteCompanyCode.Value = e.CommandArgument.ToString();

                    // Set random string to compare
                    lbDeleteCompany.Text = Retrieve.RandomString;

                    // Show and hide correct panels
                    panelCompanyDelete.Visible = true;
                    panelCompanyEdit.Visible = false;
                    panelCompanyList.Visible = false;
                }
                catch (Exception ex)
                {
                    // Show error message
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                    // ERROR //
                    this.logger.Error("Error retrieving the company information for deleting: " + ex.ToString());

                    // Revert back to main screen
                    GetCompanies();
                }
            }
        }

        /// <summary>
        /// Creates a new Company or Updates a company
        /// </summary>
        protected void btnSubmitCompany_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfModifyCompanyCode.Value))
            {
                // DEBUG //
                this.logger.Debug("Attempting to create a new company named " + txtCompanyName.Text);

                // Used for the company code
                string companyCodeVar = string.Empty;

                // Strip everything except letters, numbers, or whitespace
                char[] arr = txtCompanyName.Text.Trim().ToCharArray();
                arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))));

                if (!cbUserCompanysNameAsCode.Checked)
                {
                    companyCodeVar = new string(arr).Replace(" ", string.Empty).Substring(0, 3).ToUpper();

                    this.logger.Debug("User chose not to use the company's display name as the company code. New code is: " + companyCodeVar);
                }
                else
                {
                    companyCodeVar = new string(arr);

                    // Remove extra spaces (like if there is two or three spaces together
                    // it will replace it will one space)
                    companyCodeVar = Regex.Replace(companyCodeVar, @"\s+", " ");

                    this.logger.Debug("User chose to use the company's display name as the company code. New code is: " + companyCodeVar);
                }

                ADGroups group = new ADGroups(Config.Username, Config.Password, Config.PrimaryDC);
                ADOrgUnit organizationalUnit = new ADOrgUnit(Config.Username, Config.Password, Config.PrimaryDC);
                try
                {
                    // Check to see if company code exists or not
                    bool codeExists = SqlCommon.DoesCompanyCodeExist(companyCodeVar);

                    // Check to see if the domain already exists or not
                    bool domainExists = SQLDomains.DoesDomainExist(txtDomainName.Text.Trim());
                    if (domainExists)
                        throw new Exception(Resources.LocalizedText.DomainInUse);

                    if (codeExists)
                    {
                        // DEBUG //
                        this.logger.Debug("Company's company code already existed. Trying alternative code: " + txtCompanyName.Text + ", " + companyCodeVar);

                        string convertedCompanyCode = companyCodeVar;

                        // Uh Oh! Company code existed so we need to append a number and try again

                        for (int i = 0; i < 9999998; i++)
                        {
                            // Append a number at the end
                            convertedCompanyCode = companyCodeVar + i.ToString();

                            // DEBUG //
                            this.logger.Debug("Try number " + i.ToString() + ". New Company Code: " + convertedCompanyCode);

                            // Check if it exists
                            codeExists = SqlCommon.DoesCompanyCodeExist(convertedCompanyCode);
                            if (!codeExists)
                            {
                                // New company code doesn't exist. Replace the companyCode string
                                // with the new company code and break out of the loop
                                companyCodeVar = convertedCompanyCode;
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
                    this.logger.Debug("Found an available company code! New Code: " + companyCodeVar);

                    // Now that we have gotten here we apparently have found a company code 
                    // we can use. So lets create our Company object
                    Company companyInfo = new Company();
                    companyInfo.ResellerCode = CPContext.SelectedResellerCode;
                    companyInfo.CompanyCode = companyCodeVar;
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
                    companyInfo.Domains = new List<string>() { txtDomainName.Text.Trim() };
                    companyInfo.Description = "";

                    // Check if resellers is enabled or not
                    // If resellers are not enabled then we don't need to append the reseller code 
                    // in the distinguished name
                    string newCompanysOU = string.Empty;
                    string newCompanysResellerOU = string.Empty;

                    if (Config.ResellersEnabled)
                    {
                        // Compile the distinguished Name
                        companyInfo.DistinguishedName = "OU=" + companyCodeVar + ",OU=" + CPContext.SelectedResellerCode + "," + Config.HostingOU;
                    
                        // Set the reseller ou
                        newCompanysResellerOU = "OU=" + CPContext.SelectedResellerCode + "," + Config.HostingOU;

                        // Set the company ou which contains the reseller OU and base OU
                        newCompanysOU = "OU=" + companyInfo.CompanyCode + "," + newCompanysResellerOU;
                    }
                    else
                    {
                        // Compile the distinguished name without a reseller
                        companyInfo.DistinguishedName = "OU=" + companyInfo.CompanyCode + "," + Config.HostingOU;

                        // Set the reseller ou to just the base ou
                        newCompanysResellerOU = Config.HostingOU;

                        // Set the company ou which contains just the company OU and base OU
                        newCompanysOU = "OU=" + companyInfo.CompanyCode + "," + newCompanysResellerOU;
                    }

                    // Store the company code with no whitespaces
                    // We don't want to end up creating security groups with whitespaces in them
                    string companyCodeWithRemovedWhitespace = companyCodeVar.Replace(" ", string.Empty);
                    
                    this.logger.Debug("Creating the company's base organizational unit: " + newCompanysOU);
                    organizationalUnit.CreateOU(newCompanysResellerOU, companyInfo);

                    this.logger.Debug("Creating the company's Exchange OU: " + "OU=Exchange," + newCompanysOU);
                    organizationalUnit.CreateOU(newCompanysOU, "Exchange");

                    this.logger.Debug("Creating the company's Applications OU: " + "OU=Applications," + newCompanysOU);
                    organizationalUnit.CreateOU(newCompanysOU, "Applications");

                    this.logger.Debug("Are we putting the users in a custom organizational unit within the company: " + string.IsNullOrEmpty(Config.UsersOU).ToString());
                    if (!string.IsNullOrEmpty(Config.UsersOU))
                        organizationalUnit.CreateOU(newCompanysOU, Config.UsersOU);

                    this.logger.Debug("Creating the Admins security group for company: " + companyInfo.CompanyName);
                    group.Create(companyInfo.DistinguishedName, "Admins@" + companyCodeWithRemovedWhitespace, "", true, ADGroups.GroupType.Global);

                    this.logger.Debug("Creating the AllUsers security group for company: " + companyInfo.CompanyName);
                    group.Create(companyInfo.DistinguishedName, "AllUsers@" + companyCodeWithRemovedWhitespace, "", true, ADGroups.GroupType.Global);

                    this.logger.Debug("Creating the AllTSUsers security group for company: " + companyInfo.CompanyName);
                    group.Create(companyInfo.DistinguishedName, "AllTSUsers@" + companyCodeWithRemovedWhitespace, "", true, ADGroups.GroupType.Global);

                    this.logger.Debug("Adding the AllTSUsers security group to the AllTSUsers@Hosting security group: " + companyInfo.CompanyName);
                    group.ModifyMembership("AllTSUsers@Hosting", "AllTSUsers@" + companyCodeWithRemovedWhitespace, System.DirectoryServices.AccountManagement.IdentityType.Name, System.DirectoryServices.AccountManagement.IdentityType.Name, false);

                    this.logger.Debug("Adding the AllTSUsers security group to the GPOAccess security group: " + companyInfo.CompanyName);
                    if (!Config.ResellersEnabled)
                        group.ModifyMembership("GPOAccess@Hosting", "AllTSUsers@" + companyCodeWithRemovedWhitespace, System.DirectoryServices.AccountManagement.IdentityType.Name, System.DirectoryServices.AccountManagement.IdentityType.Name, false);
                    else
                        group.ModifyMembership("GPOAccess@" + companyInfo.ResellerCode, "AllTSUsers@" + companyCodeWithRemovedWhitespace, System.DirectoryServices.AccountManagement.IdentityType.Name, System.DirectoryServices.AccountManagement.IdentityType.Name, false);

                    // Remove Authenticated User rights
                    organizationalUnit.RemoveAuthUsersRights(companyInfo.DistinguishedName);
                    organizationalUnit.RemoveAuthUsersRights("OU=Exchange," + companyInfo.DistinguishedName);
                    organizationalUnit.RemoveAuthUsersRights("OU=Applications," + companyInfo.DistinguishedName);

                    // Add rights We want to strip the
                    organizationalUnit.AddReadRights(companyInfo.DistinguishedName, "AllUsers@" + companyCodeWithRemovedWhitespace);
                    organizationalUnit.AddReadRights("OU=Exchange," + companyInfo.DistinguishedName, "AllUsers@" + companyCodeWithRemovedWhitespace);
                    organizationalUnit.AddReadRights("OU=Applications," + companyInfo.DistinguishedName, "AllUsers@" + companyCodeWithRemovedWhitespace);

                    // DEBUG //
                    this.logger.Debug("Successfully created new organizational unit for company. Attempting to insert into SQL: " + txtCompanyName.Text + ", " + companyCodeVar);

                    // Good! No Errors so lets insert this into SQL
                    SQLCompanies.AddCompany(companyInfo);

                    // DEBUG //
                    this.logger.Debug("Attempting to add the domain into the database: " + txtCompanyName.Text + ", " + txtDomainName.Text);

                    // Add the domain to the system
                    SQLDomains.AddDomain(companyInfo.CompanyCode, txtDomainName.Text.Trim());

                    // DEBUG //
                    this.logger.Debug("Successfully created new company: " + txtCompanyName.Text);

                    // Update notification saying that we created it
                    notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.NotificationSuccessNewCompany);

                    // Reset View
                    GetCompanies();
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                    // ERROR //
                    this.logger.Error("There was an error creating company " + txtCompanyName.Text + ": " + ex.ToString());
                }
                finally
                {
                    group.Dispose();
                    organizationalUnit.Dispose();
                }
            }
            else
            {
                // We are updating a company and not creating a new one so we need to create our object
                // and update it in the database (the companycode cannot be changed)
                Company companyInfo = new Company();
                companyInfo.CompanyCode = hfModifyCompanyCode.Value;
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
                companyInfo.Description = "";

                try
                {
                    // DEBUG //
                    this.logger.Debug("Attempting to update company " + companyInfo.CompanyName + " information.");

                    // Update the database
                    SQLCompanies.UpdateCompany(companyInfo);

                    // DEBUG //
                    this.logger.Debug("Successfully updated company " + companyInfo.CompanyName + " information.");

                    // Show to the user we successfully updated
                    notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.NotificationSuccessUpdateCompany);

                    // Retrieve list of resellers
                    GetCompanies();
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                    // ERROR //
                    this.logger.Error("Error updating company " + companyInfo.CompanyName + ": " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Deletes the company from the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDeleteCompany_Click(object sender, EventArgs e)
        {
            if (txtDeleteCompany.Text.Equals(lbDeleteCompany.Text))
            {
                ADOrgUnit org = null;

                try
                {
                    // Initialize our org object
                    org = new ADOrgUnit(Config.Username, Config.Password, Config.PrimaryDC);

                    try
                    {
                        // Delete the company OU and everything in it
                        org.DeleteOUAll(lbDeleteCompanyDN.Text);

                        // Delete from SQL
                        SQLCompanies.DeleteCompany(hfDeleteCompanyCode.Value);
                    }
                    catch (Exception ex)
                    {
                        // Check to see if the error is because it doesn't exist
                        if (ex.Message.Contains("no such object on the server"))
                        {
                            // OU doesn't exist. Delete from SQL
                            SQLCompanies.DeleteCompany(hfDeleteCompanyCode.Value);
                        }
                        else
                            throw;
                    }

                    // Reset selected company variables
                    CPContext.SelectedCompanyCode = null;
                    CPContext.SelectedCompanyName = null;

                    // Show notification
                    notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.NotificationSuccessDeleteCompany);
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
                }
                finally
                {
                    if (org != null)
                        org.Dispose();

                    GetCompanies();
                }
            }
            else
            {
                // User did not enter the random string right. Show a warning!
                notification1.SetMessage(controls.notification.MessageType.Warning, Resources.LocalizedText.InvalidSecurityCode);
            }
        }
    }
}