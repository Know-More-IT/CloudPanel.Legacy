using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;

namespace CloudPanel.Modules.Mail
{
    public class MailCmds
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
         
        #region Variables
            private string _server;
            private string _from;
            private string _to;
            private string _username;
            private string _password;
            private int _port = 25;
        #endregion

        /// <summary>
        /// Initializes and sets the values
        /// </summary>
        /// <param name="server"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public MailCmds(string server, string from, string to, int port, string username, string password)
        {
            _server = server;
            _from = from;
            _to = to;
            _username = username;
            _password = password;
            _port = port;
        }

        /// <summary>
        /// Sends a email message
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        public void SendMessage(string subject, string message)
        {
            SmtpClient sc = null;
            MailMessage mm = null;

            try
            {
                sc = new SmtpClient(_server, _port);
                sc.DeliveryMethod = SmtpDeliveryMethod.Network;

                // Check if we need to supply credentials
                if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
                {
                    sc.UseDefaultCredentials = false;
                    sc.Credentials = new System.Net.NetworkCredential(_username, _password);
                }

                // Check and see if we need SSL
                if (_port == 587)
                    sc.EnableSsl = true;

                mm = new MailMessage(_from, _to, subject, message);
                mm.IsBodyHtml = false;

                // Send the message
                sc.Send(mm);

                // Log
                this.logger.Debug("Successfully send email message to: " + _to + " with message: " + message);
            }
            catch (Exception ex)
            {
                logger.Error("Failed to send message: " + message, ex);
            }
            finally
            {
                if (mm != null)
                    mm.Dispose();

                if (sc != null)
                    sc.Dispose();
            }
        }
    }
}
