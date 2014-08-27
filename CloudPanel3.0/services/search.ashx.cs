using CloudPanel.classes;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;

namespace CloudPanel.services
{
    /// <summary>
    /// Summary description for search
    /// </summary>
    public class search : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    {
        public bool IsReusable { 
            get { return true; } 
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = "application/json";

            if (Authentication.IsSuperAdmin || Authentication.IsResellerAdmin)
            {
                List<BaseSearchResults> found = null;

                if (Authentication.IsResellerAdmin)
                    found = SQLUsers.SearchUsers(context.Request.QueryString["query"], CPContext.SelectedResellerCode);
                else
                    found = SQLUsers.SearchUsers(context.Request.QueryString["query"], null);


                if (found != null)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    context.Response.Write(js.Serialize(found));
                }
            }

            context.Response.End();
        }
    }
}