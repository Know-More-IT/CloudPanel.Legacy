using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CloudPanel.controls
{
    public partial class notification : System.Web.UI.UserControl
    {
        public enum MessageType
        {
            Error,
            Warning,
            Success,
            Info
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void SetMessage(MessageType type, string msg)
        {
            panelNotification.CssClass = GetMsgType(type);
            lbTitle.Text = GetMsgTitle(type);
            lbMsg.Text = msg;

            panelNotification.Visible = true;
        }

        public void HidePanel()
        {
            panelNotification.Visible = false;
        }

        private string GetMsgType(MessageType type)
        {
            switch (type)
            {
                case MessageType.Error:
                    return "alert alert-error alert-block";
                case MessageType.Info:
                    return "alert alert-info alert-block";
                case MessageType.Success:
                    return "alert alert-success alert-block";
                case MessageType.Warning:
                    return "alert alert-block";
                default:
                    return "";
            }
        }

        private string GetMsgTitle(MessageType type)
        {
            switch (type)
            {
                case MessageType.Error:
                    return "Error!";
                case MessageType.Info:
                    return "Info!";
                case MessageType.Success:
                    return "Success!";
                case MessageType.Warning:
                    return "Warning!";
                default:
                    return "";
            }
        }
    }
}