using CloudPanel.Modules.Mail;
using CloudPanel.Modules.Settings;
using CloudPanel.Services.CPScheduler.Class;
using CloudPanel.Services.Scheduler.SQL;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CloudPanel.Services.Scheduler.Class
{
    public class Support
    {
        // Log utility
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public static void SendEmailMessage(string subject, string message)
        {
            try
            {
                SchedulerRetrieve.GetSettings();

                if ((bool)Config.SupportMailEnabled)
                {
                    int port = (int)Config.SupportMailPort;

                    // Decrypt password if it exists
                    string decrypted = string.Empty;
                    if (!string.IsNullOrEmpty(Config.SupportMailPassword))
                        decrypted = Config.SupportMailPassword;

                    // Initialize mail
                    MailCmds cmds = new MailCmds(Config.SupportMailServer, Config.SupportMailFrom, Config.SupportMailAddress,
                        port, Config.SupportMailUsername, decrypted);

                    // Send message
                    cmds.SendMessage(subject, message);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to send email message", ex);
            }
        }
    }
}
