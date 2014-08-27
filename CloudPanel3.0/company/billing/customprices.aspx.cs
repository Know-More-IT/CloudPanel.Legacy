using CloudPanel.classes;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace CloudPanel.company.billing
{
    public partial class customprices : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.IsSuperAdmin)
                Response.Redirect("~/company/overview.aspx", true);
            else
            {
                if (!IsPostBack)
                {
                    string companyCode = CPContext.SelectedCompanyCode;

                    // Load Exchange pricing
                    LoadExchangeCustomPricing(companyCode);

                    // Load Citrix plans
                    LoadCitrixCustomPricing(companyCode);
                }
            }
        }

        #region Exchange

        /// <summary>
        /// Gets a list of Exchange plans with the default price and custom price
        /// </summary>
        /// <param name="companyCode"></param>
        private void LoadExchangeCustomPricing(string companyCode)
        {
            try
            {
                List<MailboxPlan> mailboxPlans = SQLPlans.GetMailboxPlans();
                if (mailboxPlans != null)
                {
                    mailboxPlans.RemoveAll(p => !string.IsNullOrEmpty(p.CompanyCode) && !p.CompanyCode.Equals(companyCode, StringComparison.CurrentCultureIgnoreCase));

                    // Get custom prices
                    SQLCustomPricing.GetCustomPricing("Exchange", companyCode, Config.CurrencySymbol, ref mailboxPlans);

                    // Bind to the list view
                    lstExchangePricing.DataSource = mailboxPlans;
                    lstExchangePricing.DataBind();
                }
                else
                    notification1.SetMessage(controls.notification.MessageType.Warning, "Unable to find any mailbox plans in the database.");
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Sets it to edit view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lstExchangePricing_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            // Set the editing index
            lstExchangePricing.EditIndex = e.NewEditIndex;

            // Get custom pricing
            LoadExchangeCustomPricing(CPContext.SelectedCompanyCode);
        }

        /// <summary>
        /// Updates the price in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lstExchangePricing_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            string companyCode = CPContext.SelectedCompanyCode;

            try
            {
                TextBox txt = lstExchangePricing.Items[e.ItemIndex].FindControl("txtExchCustomPrice") as TextBox;

                string planID = lstExchangePricing.DataKeys[e.ItemIndex].Value.ToString();
                string customPrice = txt.Text;

                // Remove currency symbol if there
                customPrice = customPrice.Replace(Config.CurrencySymbol, string.Empty);
                
                // Update or Insert data
                SQLCustomPricing.UpdateCustomPricing("Exchange", CPContext.SelectedCompanyCode, customPrice, planID);

                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Success, "Successfully set custom price for this company to: " + Config.CurrencySymbol + customPrice);

                // Reset grid index
                lstExchangePricing.EditIndex = -1;
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error: " + ex.Message);
            }
            finally
            {
                LoadExchangeCustomPricing(companyCode);
            }
        }

        #endregion

        #region Citrix

        /// <summary>
        /// Gets a list of Citrix plans with the default price and custom price
        /// </summary>
        /// <param name="companyCode"></param>
        private void LoadCitrixCustomPricing(string companyCode)
        {
            try
            {
                List<BaseCitrixApp> plans = SQLCitrix.GetCitrixPlans();
                if (plans != null)
                {
                    // Remove ones assigned to other companies
                    plans.RemoveAll(p => !string.IsNullOrEmpty(p.CompanyCode) && !p.CompanyCode.Equals(companyCode, StringComparison.CurrentCultureIgnoreCase));

                    // Get custom prices
                    SQLCustomPricing.GetCustomPricing("Citrix", companyCode, Config.CurrencySymbol, ref plans);

                    // Bind
                    lstCitrixCustomPricing.DataSource = plans;
                    lstCitrixCustomPricing.DataBind();
                }
                else
                    notification1.SetMessage(controls.notification.MessageType.Warning, "Unable to find Citrix plans in the database.");
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error: " + ex.Message);
            }
        }

        protected void lstCitrixCustomPricing_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            string companyCode = CPContext.SelectedCompanyCode;

            try
            {
                TextBox txt = lstCitrixCustomPricing.Items[e.ItemIndex].FindControl("txtCitrixCustomPrice") as TextBox;

                string planID = lstCitrixCustomPricing.DataKeys[e.ItemIndex].Value.ToString();
                string customPrice = txt.Text;

                // Remove $ sign if there
                customPrice = customPrice = Regex.Replace(customPrice, "[^0-9.]", string.Empty);

                // Update or Insert data
                SQLCustomPricing.UpdateCustomPricing("Citrix", CPContext.SelectedCompanyCode, customPrice, planID);

                // Update notification
                notification1.SetMessage(controls.notification.MessageType.Success, "Successfully set custom price for this company to: " + Config.CurrencySymbol + customPrice);

                // Reset grid index
                lstCitrixCustomPricing.EditIndex = -1;
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "Error: " + ex.Message);
            }
            finally
            {
                LoadCitrixCustomPricing(companyCode);
            }
        }

        protected void lstCitrixCustomPricing_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            // Set index
            lstCitrixCustomPricing.EditIndex = e.NewEditIndex;

            // Reload data
            LoadCitrixCustomPricing(CPContext.SelectedCompanyCode);
        }

        #endregion
    }
}