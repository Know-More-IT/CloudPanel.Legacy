using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudPanel.Modules.Exchange;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using CloudPanel.Modules.Base;
using CloudPanel.classes;
using CloudPanel.Modules.Base.Class;

namespace CloudPanel.company.exchange
{
    public partial class enable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Set the user rights
            SetUserRights();

            if (!IsPostBack)
            {
                // Check if Exchange Enabled
                try
                {
                    if (!SQLExchange.IsExchangeEnabled(CPContext.SelectedCompanyCode))
                    {
                        enableExchange.Visible = true;
                        disableExchange.Visible = false;
                    }
                    else
                    {
                        enableExchange.Visible = false;
                        disableExchange.Visible = true;

                        lbDeleteLabel.Text = Retrieve.RandomString;
                    }
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Warning, ex.Message);
                }
            }
        }

        /// <summary>
        /// Sets what the user has rights to or not
        /// </summary>
        private void SetUserRights()
        {
            if (Authentication.PermEnableExchange)
                btnEnableExchange.Visible = true;
            else
                btnEnableExchange.Visible = false;

            if (Authentication.PermDisableExchange)
                btnDisableExchange.Visible = true;
            else
                btnDisableExchange.Visible = false;
        }

        protected void btnEnableExchange_Click(object sender, EventArgs e)
        {
            EnableExchange();
        }

        protected void btnDisableExchange_Click(object sender, EventArgs e)
        {
            if (lbDeleteLabel.Text.Equals(txtDeleteLabel.Text))
                DisableExchange();
            else
                notification1.SetMessage(controls.notification.MessageType.Warning, Resources.LocalizedText.SecurityCodeNotMatched);
        }

        /// <summary>
        /// Enables Exchange and updates the database
        /// </summary>
        private void EnableExchange()
        {
            ExchCmds cmds = null;

            try
            {
                cmds = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // Create Exchange objects for company
                cmds.Enable_Company(CPContext.SelectedCompanyCode, "AllUsers@" + CPContext.SelectedCompanyCode, Retrieve.GetCompanyExchangeOU);

                // Update SQL
                SQLExchange.SetCompanyExchangeEnabled(CPContext.SelectedCompanyCode, true);

                // Update Status Message
                notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.NotificationSuccessEnableExchange);

                // Change panel
                enableExchange.Visible = false;
                disableExchange.Visible = true;

                // Set Random string
                lbDeleteLabel.Text = Retrieve.RandomString;
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
            finally
            {
                if (cmds != null)
                    cmds.Dispose();
            }
        }

        /// <summary>
        /// Disables Exchange and updates the database
        /// </summary>
        private void DisableExchange()
        {
            ExchCmds cmds = null;

            try
            {
                cmds = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // Get a list of accepted domains
                var convertedDomains = new List<string>();
                List<Domain> domains = SQLExchange.GetAcceptedDomains(CPContext.SelectedCompanyCode);
                if (domains != null && domains.Count > 0)
                {
                    // Extracts just the domain name from the list
                    convertedDomains = (from d in domains select d.DomainName as string).ToList();
                }

                // Delete from Exchange
                cmds.Disable_Company(CPContext.SelectedCompanyCode, convertedDomains);

                // Delete from SQL
                SQLExchange.DisableExchange(CPContext.SelectedCompanyCode);

                // Update Status Message
                notification1.SetMessage(controls.notification.MessageType.Success, Resources.LocalizedText.NotificationSuccessDisableExchange);

                // Change panel
                enableExchange.Visible = true;
                disableExchange.Visible = false;
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
            finally
            {
                if (cmds != null)
                    cmds.Dispose();
            }
        }
    }
}