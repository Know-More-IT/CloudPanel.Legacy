using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Sql;
using CloudPanel.Modules.Exchange;
using CloudPanel.Modules.ActiveDirectory;
using CloudPanel.Modules.Base.Class;

namespace CloudPanel.company
{
    public partial class overview : System.Web.UI.Page
    {
        #region Stats

        protected int _statsMaxUsers = 10;
        protected int _statsUsers = 0;

        protected int _statsMaxDomains = 10;
        protected int _statsDomains = 0;

        protected int _statsMaxExchContacts = 10;
        protected int _statsExchContacts = 0;

        protected int _statsMaxExchMailboxes = 10;
        protected int _statsExchMailboxes = 0;

        protected int _statsMaxExchDistGroups = 10;
        protected int _statsExchDistGroups = 0;

        protected int _statsMaxCitrixUsers = 10;
        protected int _statsCitrixUsers = 0;

        protected int _statsMaxLyncUsers = 10;
        protected int _statsLyncUsers = 0;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Get list of company plans
                GetCompanyPlans();

                // Get company information
                GetCompanyInfo();

                // Get company statistics
                GetCompanyStats();
            }
        }

        /// <summary>
        /// Retrieves information about the company
        /// </summary>
        private void GetCompanyInfo()
        {
            string companyCode = CPContext.SelectedCompanyCode;

            try
            {
                // Get the company information
                Company company = SQLCompanies.GetCompany(companyCode);
                lbCompanyName.Text = company.CompanyName;
                lbCompanyCode.Text = company.CompanyCode;
                lbCompanyPhoneNumber.Text = company.PhoneNumber;

                if (company.WhenCreated != null)
                    lbCompanyCreated.Text = company.WhenCreated.ToString();

                if (company.OrgPlanID > 0)
                {
                    ddlCompanyPlans.SelectedValue = company.OrgPlanID.ToString();

                    // Get the plan information the company is part of
                    BasePlanCompany plan = SQLPlans.GetCompanyPlan(company.OrgPlanID);
                    _statsMaxUsers = plan.MaxUsers;
                    _statsMaxDomains = plan.MaxDomains;
                    _statsMaxExchContacts = plan.MaxExchangeContacts;
                    _statsMaxExchMailboxes = plan.MaxExchangeMailboxes;
                    _statsMaxExchDistGroups = plan.MaxExchangeDistLists;
                    _statsMaxCitrixUsers = plan.MaxCitrixUsers;
                }

                // Check if the company is Exchange enabled and the permissions are not fixed
                if (company.ExchangeEnabled && !company.ExchangePermissionsFixed)
                {
                    RepairExchPermissions(companyCode);
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// The earlier versions of CloudPanel didn't lock down the mailbox calendar permissions
        /// or things like the distribution groups to only the tenant (sending email, etc).
        /// 
        /// This checks the database if it has been updated and resolves it for all objects
        /// </summary>
        private void RepairExchPermissions(string companyCode)
        {
            ExchCmds powershell = null;
            ADGroups groups = null;

            try
            {
                // Initialize groups object
                groups = new ADGroups(Config.Username, Config.Password, Config.PrimaryDC);

                // Initialize powershell object
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // Check if the group exists or not
                if (!groups.DoesGroupExist("ExchangeSecurity@" + companyCode))
                {
                    powershell.New_SecurityDistributionGroup("ExchangeSecurity", CPContext.SelectedCompanyCode, Retrieve.GetCompanyExchangeOU);
                }

                // Now fix the permissions
                powershell.Repair_ExchangePermissions(companyCode);

                // Update permissions
                SQLCompanies.FixExchangePermissions(true, companyCode);
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error repairing Exchange permissions. Contact support!! Error: " + ex.Message);
            }
            finally
            {
                if (groups != null)
                    groups.Dispose();

                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Gets the company's statistics
        /// </summary>
        private void GetCompanyStats()
        {
            SQLStatistics stats = new SQLStatistics();

            try
            {
                stats.GetCompanyStatistics(CPContext.SelectedCompanyCode, out _statsUsers, out _statsDomains, out _statsExchContacts, out _statsExchMailboxes, out _statsExchDistGroups);
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
            finally
            {
                stats = null;
            }
        }

        /// <summary>
        /// Get a list of plans from the database
        /// </summary>
        private void GetCompanyPlans()
        {
            // Clear drop down and add default items
            ddlCompanyPlans.Items.Clear();
            ddlCompanyPlans.Items.Add(Resources.LocalizedText.NotSet);

            List<BasePlanCompany> plans = null;
            try
            {
                plans = SQLPlans.GetCompanyPlans();
                foreach (BasePlanCompany c in plans)
                {
                    ListItem item = new ListItem();
                    item.Text = c.PlanName;
                    item.Value = c.PlanID.ToString();

                    ddlCompanyPlans.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
            finally
            {
                plans = null;
            }
        }

        /// <summary>
        /// Change the company plan
        /// </summary>
        protected void ddlCompanyPlans_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCompanyPlans.SelectedIndex > 0)
            {
                try
                {
                    // Update Plan
                    SQLCompanies.UpdateCompanyPlan(CPContext.SelectedCompanyCode, int.Parse(ddlCompanyPlans.SelectedValue));

                    // Get list of company plans
                    GetCompanyPlans();

                    // Get company information
                    GetCompanyInfo();

                    // Get company statistics
                    GetCompanyStats();
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
                }
            }
        }

        #region Progress Panel Prerenders
        
            protected void panelStats_PreRender(object sender, EventArgs e)
            {
                // Get the panel that called the method
                Panel p = sender as Panel;

                int percentage = 0;

                if (p == panelStatsCompanyDomains)
                {
                    lbStatsCompanyDomains.Text = _statsDomains.ToString() + "/" + _statsMaxDomains.ToString();

                    if (_statsMaxDomains <= 0) // We can't divide by ZERO because it will throw exception. If max is 0 set to 100% used
                        percentage = 100;
                    else
                        percentage = _statsDomains * 100 / _statsMaxDomains;
                }
                else if (p == panelStatsCompanyUsers)
                {
                    lbStatsCompanyUsers.Text = _statsUsers.ToString() + "/" + _statsMaxUsers.ToString();

                    if (_statsMaxUsers <= 0)  // We can't divide by ZERO because it will throw exception. If max is 0 set to 100% used
                        percentage = 100;
                    else
                        percentage = _statsUsers * 100 / _statsMaxUsers;
                }
                else if (p == panelStatsExchangeContacts)
                {
                    lbStatsExchangeContacts.Text = _statsExchContacts.ToString() + "/" + _statsMaxExchContacts.ToString();

                    if (_statsMaxExchContacts <= 0) // We can't divide by ZERO because it will throw exception. If max is 0 set to 100% used
                        percentage = 100;
                    else
                        percentage = _statsExchContacts * 100 / _statsMaxExchContacts;
                }
                else if (p == panelStatsExchangeMailboxes)
                {
                    lbStatsExchangeMailboxes.Text = _statsExchMailboxes.ToString() + "/" + _statsMaxExchMailboxes.ToString();

                    if (_statsMaxExchMailboxes <= 0) // We can't divide by ZERO because it will throw exception. If max is 0 set to 100% used
                        percentage = 100;
                    else
                        percentage = _statsExchMailboxes * 100 / _statsMaxExchMailboxes;
                }
                else if (p == panelStatsExchangeDistGroups)
                {
                    lbStatsExchangeDistGroups.Text = _statsExchDistGroups.ToString() + "/" + _statsMaxExchDistGroups.ToString();

                    if (_statsMaxExchDistGroups <= 0) // We can't divide by ZERO because it will throw exception. If max is 0 set to 100% used
                        percentage = 100;
                    else
                        percentage = _statsExchDistGroups * 100 / _statsMaxExchDistGroups;
                }

                // Set the progress bar width based on the percentage
                HtmlGenericControl div = p.Controls[1] as HtmlGenericControl;
                div.Style.Clear();
                div.Style.Add("Width", percentage.ToString() + "%");

                // Set the style for the progress bar color based
                // on the percentage.
                // Under 50%: green
                // Over 49%: yellow
                // Over 89%: red
                if (percentage >= 90)
                    p.CssClass = "progress progress-striped progress-danger active";
                else if (percentage >= 50)
                    p.CssClass = "progress progress-striped progress-warning active";
                else
                    p.CssClass = "progress progress-striped progress-success active";
            }

        #endregion
    }
}