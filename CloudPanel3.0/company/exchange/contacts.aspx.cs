using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Sql;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Exchange;
using CloudPanel.classes;
using CloudPanel.Modules.Base.Class;

namespace CloudPanel.company.exchange
{
    public partial class contacts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetContacts();
            }
        }

        /// <summary>
        /// Gets a list of contacts for the company
        /// </summary>
        private void GetContacts()
        {
            // Hide Panels
            panelCreateContact.Visible = false;

            // Show Contact Panel
            panelContacts.Visible = true;

            try
            {
                List<BaseContacts> contacts = SQLExchange.GetContacts(CPContext.SelectedCompanyCode);
                contacts.OrderBy(x => x.DisplayName);

                // Bind to our list
                repeater.DataSource = contacts;
                repeater.DataBind();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Creates a new contact
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSaveContact_Click(object sender, EventArgs e)
        {
            ExchCmds powershell = null;

            try
            {
                // Initialize Exchange Powershell
                powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                // Trim email
                txtEmailAddress.Text = txtEmailAddress.Text.Trim();

                // Check if we are updating or creating a new contact
                if (string.IsNullOrEmpty(hfDistinguishedName.Value))
                {
                    // Add Contact
                    powershell.New_Contact(txtDisplayName.Text, txtEmailAddress.Text, cbContactHidden.Checked, Retrieve.GetCompanyExchangeOU, CPContext.SelectedCompanyCode);

                    // Format that DN
                    string contactDN = string.Format("CN={0},{1}", txtEmailAddress.Text.Split('@')[0] + "_" + CPContext.SelectedCompanyCode.Replace(" ", string.Empty),
                        Retrieve.GetCompanyExchangeOU);

                    // Insert into SQL
                    SQLExchange.AddContact(contactDN, CPContext.SelectedCompanyCode, txtDisplayName.Text.Trim(), txtEmailAddress.Text, cbContactHidden.Checked);
                }
                else
                {
                    MailContact contact = new MailContact();
                    contact.DistinguishedName = hfDistinguishedName.Value;
                    contact.Hidden = cbContactHidden.Checked;

                    // Update exchange
                    powershell.Set_Contact(contact);

                    // Update sql
                    SQLExchange.UpdateContact(contact);

                    contact = null;
                }

                // Refresh View
                GetContacts();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error,  ex.Message);
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Goes into the delete or edit screen for the selected contact
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                ExchCmds powershell = null;

                try
                {
                    string distinguishedName = e.CommandArgument.ToString();

                    // Initialize
                    powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                    // Remove from exchange
                    powershell.Remove_Contact(distinguishedName);

                    // Now remove from SQL
                    SQLExchange.RemoveContact(distinguishedName);
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
                }
                finally
                {
                    if (powershell != null)
                        powershell.Dispose();

                    // Refresh view
                    GetContacts();
                }
            }
            else if (e.CommandName == "Edit")
            {
                ExchCmds powershell = null;

                try
                {
                    string distinguishedName = e.CommandArgument.ToString();

                    // Initialize
                    powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                    // Remove from exchange
                    MailContact contact = powershell.Get_Contact(distinguishedName);
                    hfDistinguishedName.Value = contact.DistinguishedName;
                    txtDisplayName.Text = contact.DisplayName;
                    txtEmailAddress.Text = contact.ExternalEmailAddress;
                    cbContactHidden.Checked = contact.Hidden;

                    // Show panels
                    panelContacts.Visible = false;
                    panelCreateContact.Visible = true;

                    // Disable fields
                    txtDisplayName.ReadOnly = true;
                    txtEmailAddress.ReadOnly = true;

                    // Now remove from SQL
                    SQLExchange.UpdateContact(contact);

                    // Dispose
                    contact = null;
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
                }
                finally
                {
                    if (powershell != null)
                        powershell.Dispose();
                }
            }
        }

        /// <summary>
        /// Shows the create new contact page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddContact_Click(object sender, EventArgs e)
        {
            // Hide Panels
            panelContacts.Visible = false;

            // Show Create Contact Panel
            panelCreateContact.Visible = true;

            // Clear fields
            hfDistinguishedName.Value = string.Empty;
            txtDisplayName.Text = string.Empty;
            txtEmailAddress.Text = string.Empty;
            cbContactHidden.Checked = false;

            // Enable fields
            txtDisplayName.ReadOnly = false;
            txtEmailAddress.ReadOnly = false;
        }

        /// <summary>
        /// Cancels creating or editing a contact
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Get the list of contacts again and show the page
            GetContacts();
        }
    }
}