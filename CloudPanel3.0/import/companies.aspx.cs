using CloudPanel.Modules.ActiveDirectory;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CloudPanel.import
{
    public partial class companies : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnSubmitCompany_Click(object sender, EventArgs e)
        {
            AddCompany();
        }


        private void AddCompany()
        {
            ADOrgUnit org = null;
            ADGroups groups = null;

            try
            {
                // Distinguished name the user put in
                string dn = txtDistinguishedName.Text.Trim();

                // Initialize our Active Directory connection object
                org = new ADOrgUnit(Config.Username, Config.Password, Config.PrimaryDC);
                groups = new ADGroups(Config.Username, Config.Password, Config.PrimaryDC);

                Company company = org.GetOU(dn);
                company.ResellerCode = CPContext.SelectedResellerCode;
                company.CompanyCode = company.CompanyName;
                company.Street = "";
                company.City = "";
                company.State = "";
                company.Country = "";
                company.ZipCode = "";
                company.PhoneNumber = "";
                company.Website = "";
                company.Description = "";
                company.AdministratorsName = "";
                company.AdministratorsEmail = "";

                // Check that some of the domains are not already in use
                if (company.Domains != null)
                {
                    foreach (string domain in company.Domains)
                    {
                        if (SQLDomains.DoesDomainExist(domain))
                            throw new Exception(Resources.LocalizedText.ImportCompanyError1 + domain);
                    }
                }

                // Store the company code with no whitespaces
                // We don't want to end up creating security groups with whitespaces in them
                string companyCodeWithRemovedWhitespace = company.CompanyCode.Replace(" ", string.Empty);

                // Make sure our other OU's exist
                org.CreateOU(company.DistinguishedName, "Exchange");
                org.CreateOU(company.DistinguishedName, "Applications");

                // Check for the users OU if specified
                if (!string.IsNullOrEmpty(Config.UsersOU))
                    org.CreateOU(company.DistinguishedName, Config.UsersOU);

                // Make sure our groups exists
                groups.Create(company.DistinguishedName, "Admins@" + companyCodeWithRemovedWhitespace, "", true, ADGroups.GroupType.Global, false);
                groups.Create(company.DistinguishedName, "AllTSUsers@" + companyCodeWithRemovedWhitespace, "", true, ADGroups.GroupType.Global, false);
                groups.Create(company.DistinguishedName, "AllUsers@" + companyCodeWithRemovedWhitespace, "", true, ADGroups.GroupType.Global, false);

                // Add the new AllTSUsers group for this company to the parent group
                groups.ModifyMembership("AllTSUsers@Hosting", "AllTSUsers@" + companyCodeWithRemovedWhitespace, System.DirectoryServices.AccountManagement.IdentityType.Name, System.DirectoryServices.AccountManagement.IdentityType.Name, false);

                // Remove Authenticated User rights
                org.RemoveAuthUsersRights(company.DistinguishedName);
                org.RemoveAuthUsersRights("OU=Exchange," + company.DistinguishedName);
                org.RemoveAuthUsersRights("OU=Applications," + company.DistinguishedName);

                // Add rights We want to strip the
                org.AddReadRights(company.DistinguishedName, "AllUsers@" + companyCodeWithRemovedWhitespace);
                org.AddReadRights("OU=Exchange," + company.DistinguishedName, "AllUsers@" + companyCodeWithRemovedWhitespace);
                org.AddReadRights("OU=Applications," + company.DistinguishedName, "AllUsers@" + companyCodeWithRemovedWhitespace);

                // Good! No Errors so lets insert this into SQL
                SQLCompanies.AddCompany(company);

                // Now lets add the domains
                if (company.Domains != null)
                {
                    foreach (string domain in company.Domains)
                    {
                        SQLDomains.AddDomain(company.CompanyCode, domain.Trim());
                    }
                }

                // Redirect to companies
                Response.Redirect("~/companies.aspx", false);
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.ToString());
            }
            finally
            {
                if (groups != null)
                    groups.Dispose();

                if (org != null)
                    org.Dispose();
            }
        }
    }
}