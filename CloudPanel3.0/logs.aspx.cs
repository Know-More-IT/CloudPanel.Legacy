using CloudPanel.classes;
using System;
using System.IO;

namespace CloudPanel
{
    public partial class logs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.IsSuperAdmin)
                Response.Redirect("~/dashboard.aspx", true);
            else
            {
                RetrieveLogs();
            }
        }

        private void RetrieveLogs()
        {
            FileStream fs = null;
            StreamReader sr = null;

            try
            {
                fs = new FileStream(Server.MapPath("~/log/CloudPanelLogger.log"), FileMode.Open, FileAccess.Read);
                sr = new StreamReader(fs);

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("ERROR") || line.Contains("FATAL"))
                        ltrLog.Text += string.Format("<p style=\"color: red\">{0}{1}</p>", line, Environment.NewLine);
                    else if (!line.Contains("ERROR") && !line.Contains("FATAL") && !line.Contains("DEBUG") && !line.Contains("INFO") && !line.Contains("WARN")) // Doesn't contain these probably exception message
                        ltrLog.Text += string.Format("<p style=\"color: red\">{0}{1}</p>", line, Environment.NewLine);
                    else if (line.Contains("WARN"))
                        ltrLog.Text += string.Format("<p style=\"color: DarkOrange\">{0}{1}</p>", line, Environment.NewLine);
                    else
                        ltrLog.Text += string.Format("<p>{0}{1}</p>", line, Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.ToString());
            }
            finally
            {
                if (sr != null)
                    sr.Dispose();

                if (fs != null)
                    fs.Dispose();
            }
        }
    }
}