using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Lync;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using System;

namespace CloudPanel.company.lync
{
    public partial class enable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if Exchange Enabled
                try
                {
                    if (!DbSql.IsLyncEnabled(CPContext.SelectedCompanyCode))
                    {
                        enableLync.Visible = true;
                        disableLync.Visible = false;
                    }
                    else
                    {
                        enableLync.Visible = false;
                        disableLync.Visible = true;

                        lbDeleteLabel.Text = Retrieve.RandomString;
                    }
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Warning, ex.Message);
                }
            }
        }

        protected void btnEnableLync_Click(object sender, EventArgs e)
        {
            Lync2013HPv2 lync = null;

            try
            {
                lync = new Lync2013HPv2(Config.LyncURI, Config.Username, Config.Password, Modules.Base.Class.Enumerations.ConnectionType.SSL, Config.PrimaryDC);

                // Get the selected company
                Company selectedCompany = DbSql.Get_Company(CPContext.SelectedCompanyCode);

                // Enable Lync
                lync.Enable_Company(selectedCompany.DistinguishedName, CPContext.SelectedCompanyCode, null);

                // Update SQL
                DbSql.ModifyLyncStatus(CPContext.SelectedCompanyCode, true);

                // Update Status Message
                notification1.SetMessage(controls.notification.MessageType.Success, "Successfully enabled Lync for your company.");

                // Change panel
                enableLync.Visible = false;
                disableLync.Visible = true;

                // Set Random string
                lbDeleteLabel.Text = Retrieve.RandomString;
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
            finally
            {
                if (lync != null)
                    lync.Dispose();
            }
        }

        protected void btnDisableLync_Click(object sender, EventArgs e)
        {

        }
    }
}