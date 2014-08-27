using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudPanel.Modules.Sql;
using CloudPanel.Modules.Base;
using log4net;
using System.Reflection;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Exchange;
using CloudPanel.classes;
using CloudPanel.Modules.Base.Class;

namespace CloudPanel.company.exchange
{
    public partial class domains : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateDomains();
        }

        /// <summary>
        /// Gets a list of domains for the selected company
        /// </summary>
        private void PopulateDomains()
        {
            try
            {
                List<Domain> domains = DbSql.Get_Domains(CPContext.SelectedCompanyCode);

                repeaterAcceptedDomains.DataSource = domains;
                repeaterAcceptedDomains.DataBind();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                // ERROR //
                logger.Error("Error populating domains for company code " + CPContext.SelectedCompanyCode, ex);
            }
        }

        /// <summary>
        /// Changes the domain from to or from an Accepted Domain
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void repeaterAcceptedDomains_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ChangeAcceptedDomainStatus")
            {
                string domainname = e.CommandArgument.ToString();

                // Find the controls
                Button btn = (Button)e.CommandSource;

                ExchCmds powershell = null;

                try
                {
                    powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);
                    
                    if (btn.Text == "Enable")
                    {
                        // DEBUG //
                        this.logger.Debug("Attempting to enable accepted domain " + domainname);

                        // Add accepted domain to Exchange
                        powershell.New_AcceptedDomain(domainname);

                        // Update SQL
                        DbSql.Update_DomainAcceptedDomainStatus(domainname, true);

                        // Update notication
                        notification1.SetMessage(controls.notification.MessageType.Success, "Successfully enabled domain for Exchange: " + domainname);
                    }
                    else
                    {
                        // Remove accepted domain from Exchange
                        powershell.Remove_AcceptedDomain(domainname);

                        // Update SQL
                        DbSql.Update_DomainAcceptedDomainStatus(domainname, false);

                        // Update notication
                        notification1.SetMessage(controls.notification.MessageType.Success, "Successfully disabled domain from Exchange: " + domainname);
                    }
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);

                    // Log Error //
                    this.logger.Error("Error changing the accepted domain status.", ex);
                }
                finally
                {
                    if (powershell != null)
                        powershell.Dispose();

                    // Rebind
                    PopulateDomains();
                }
            }
        }
    }
}