using CloudPanel.classes;
using CloudPanel.Modules.ActiveDirectory;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CloudPanel.company.domains
{
    public partial class edit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Get all the domains for the company
                GetDomains();
            }

            // Check the domain limit
            CheckLimit();

            // Check the user rights
            SetUserRights();
        }

        /// <summary>
        /// Sets what the user has rights to or not
        /// </summary>
        private void SetUserRights()
        {
            if (Authentication.PermAddDomain)
                btnAddDomain.Visible = true;
            else
                btnAddDomain.Visible = false;
        }

        /// <summary>
        /// Checks if the domain is at the domain limit
        /// </summary>
        private void CheckLimit()
        {
            // Check if they are at the max or not
            try
            {
                bool isAtMax = SQLLimits.IsCompanyAtDomainLimit(CPContext.SelectedCompanyCode);
                if (isAtMax)
                {
                    btnAddDomain.Enabled = false;
                    notification1.SetMessage(controls.notification.MessageType.Warning, "You cannot add any more domains because you have reached the limit.");
                }
                else
                {
                    btnAddDomain.Enabled = true;
                    notification1.HidePanel();
                }
            }
            catch (Exception ex)
            {
                btnAddDomain.Enabled = false;
            }
        }

        /// <summary>
        /// Gets the domains for the company and puts it in the repeater
        /// </summary>
        private void GetDomains()
        {
            try
            {
                // Check the limit first
                CheckLimit();

                panelAddDomain.Visible = false;
                panelCurrentDomains.Visible = true;

                List<Domain> domains = DbSql.Get_Domains(CPContext.SelectedCompanyCode);
                repeaterDomains.DataSource = domains;
                repeaterDomains.DataBind();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
        }

        protected void btnAddDomain_Click(object sender, EventArgs e)
        {
            // Hide and show the correct panels
            panelCurrentDomains.Visible = false;
            panelAddDomain.Visible = true;
        }

        /// <summary>
        /// Adds a new domain to the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            ADOrgUnit org = null;

            try
            {
                string domainName = txtDomainName.Text.Replace(" ", string.Empty);

                Domain foundDomain = DbSql.Get_Domain(domainName);

                if (foundDomain != null)
                    throw new Exception("This domain is already in use and cannot be added to your account.");
                else
                {
                    // Initialize AD so we can add value to OU
                    org = new ADOrgUnit(Config.Username, Config.Password, Config.PrimaryDC);

                    // Add the domain to the organizational unit
                    org.AddDomain(Retrieve.GetCompanyOU, domainName);

                    // Add the domain to SQL
                    Domain newDomain = new Domain();
                    newDomain.DomainName = domainName;
                    newDomain.IsAcceptedDomain = false;
                    newDomain.IsDefault = false;
                    newDomain.IsSubDomain = false;
                    newDomain.CompanyCode = CPContext.SelectedCompanyCode;

                    DbSql.Add_Domain(newDomain);
                }
            }
            catch (Exception ex)
            {
                this.notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
            finally
            {
                if (org != null)
                    org.Dispose();

                GetDomains();
            }
        }

        /// <summary>
        /// Cancels and goes back to the main screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            GetDomains();
        }

        protected void repeaterDomains_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                // Get the number of domains
                if (repeaterDomains.Items.Count > 1)
                {
                    DeleteDomain(e.CommandArgument.ToString());
                }
                else
                {
                    notification1.SetMessage(controls.notification.MessageType.Warning, Resources.LocalizedText.CannotDeleteOnlyDomain);
                }
            }
        }

        /// <summary>
        /// Removes a domain if it is not in use by users or an accepted domain
        /// </summary>
        /// <param name="domainName"></param>
        private void DeleteDomain(string domainName)
        {
            ADOrgUnit org = null;

            try
            {
                Domain domain = DbSql.Get_Domain(domainName);

                if (domain.TotalCount == 0 && !domain.IsAcceptedDomain)
                {
                    // Remove from OU
                    org = new ADOrgUnit(Config.Username, Config.Password, Config.PrimaryDC);
                    org.RemoveDomain(Retrieve.GetCompanyOU, domainName);

                    // Delete from SQL
                    DbSql.Delete_Domain(domain.DomainID);
                }
                else
                    notification1.SetMessage(controls.notification.MessageType.Error, "You cannot delete this domain because it is either in use by users or is enabled in Exchange");
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
            finally
            {
                if (org != null)
                    org.Dispose();

                GetDomains();
            }
        }
    }
}