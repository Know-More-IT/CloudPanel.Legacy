using CloudPanel.classes;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace CloudPanel
{
    public partial class search : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Master.IsSuperAdmin)
                Response.Redirect("~/dashboard.aspx", false);
            else
            {
                GetSearchResults();
            }
        }

        private void GetSearchResults()
        {
            if (Request.QueryString["search"] != null)
            {
                try
                {
                    // If this is a reseller searching then make sure they only pull users for their environment
                    string isResellerCode = string.Empty;
                    if (Authentication.IsResellerAdmin)
                        isResellerCode = CPContext.SelectedResellerCode;

                    List<BaseSearchResults> users = SQLUsers.SearchUsers(Request.QueryString["search"], isResellerCode);
                    searchRepeater.DataSource = users;
                    searchRepeater.DataBind();
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
                }
            }
        }

        protected void searchRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectUser")
            {
                try
                {
                    BaseUser user = SQLUsers.GetUser(e.CommandArgument.ToString());
                    if (user != null)
                    {
                        // Set the session variables
                        CPContext.SelectedResellerCode = user.ResellerCode;
                        CPContext.SelectedResellerName = user.ResellerName;
                        CPContext.SelectedCompanyCode = user.CompanyCode;
                        CPContext.SelectedCompanyName = user.CompanyName;

                        // Redirect to company users
                        Response.Redirect("~/company/users/edit.aspx", false);
                    }
                }
                catch (Exception ex)
                {
                    notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
                }
            }
        }
    }
}