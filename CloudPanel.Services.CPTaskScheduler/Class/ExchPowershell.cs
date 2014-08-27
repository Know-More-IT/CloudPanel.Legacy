using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using CloudPanel.Services.Scheduler.Class;
using CloudPanel.Services.Scheduler.SQL;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Security;
using System.Text;

namespace CloudPanel.Services.CPScheduler.Class
{
    public class ExchPowershell : IDisposable
    {
        // Log utilitys
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // If everything is disposed or not
        internal bool disposed = false;

        // Our connection information to the Exchange srever
        internal WSManConnectionInfo wsConn;

        // Pipeline Information for the remote powershell
        internal Runspace runspace = null;

        // The domain controller that we communicate with when we run powershell commands
        internal string domainController = string.Empty;

        /// <summary>
        /// Creates a new class and stores the passed values containing connection information
        /// </summary>
        /// <param name="uri">The URI to the Exchange powershell virtual directory</param>
        /// <param name="username">DOMAIN\Username to authenticate with</param>
        /// <param name="password">Password for the domain user</param>
        /// <param name="isKerberos">True if using kerberos authentication, False if using basic authentication</param>
        /// <param name="domainController">The domain controller to communicate with</param>
        public ExchPowershell()
        {
            try
            {
                // Retrieve the settings from the database
                SchedulerRetrieve.GetSettings();

                // Set our domain controller to communicate with
                this.domainController = Config.PrimaryDC;

                // Get the type of Exchange connection
                bool isKerberos = false;
                if (Config.ExchangeConnectionType == Enumerations.ConnectionType.Kerberos)
                    isKerberos = true;

                // Create our connection
                this.wsConn = GetConnection(Config.ExchangeURI, Config.Username, Config.Password, isKerberos);

                // Create our runspace
                runspace = RunspaceFactory.CreateRunspace(wsConn);

                // Open our connection
                runspace.Open();
            }
            catch (Exception ex)
            {
                // ERROR
                logger.Fatal("Unable to establish connection to Exchange.", ex);
            }
        }

        /// <summary>
        /// Sets the mailbox default calendar permissions by removing the default and adding their company's ExchangeSecurity group to the AvailabilityOnly
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <param name="companyCode"></param>
        public void Set_MailboxCalendarPermission(string userPrincipalName, string companyCode)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // DEBUG
                logger.Debug("Removing default permissions for mailbox calendar and adding correct permissions for user " + userPrincipalName);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Enable the mailbox
                PSCommand cmd = new PSCommand();

                // Get the calendar name
                string calendarName = Get_CalendarName(userPrincipalName);

                // Remove default calendar permissions
                cmd.AddCommand("Set-MailboxFolderPermission");
                cmd.AddParameter("Identity", string.Format(@"{0}:\{1}", userPrincipalName, calendarName));
                cmd.AddParameter("User", "Default");
                cmd.AddParameter("AccessRights", "None");
                cmd.AddParameter("DomainController", this.domainController);

                // Add calendar permissions for the group
                cmd.AddStatement();
                cmd.AddCommand("Add-MailboxFolderPermission");
                cmd.AddParameter("Identity", string.Format(@"{0}:\{1}", userPrincipalName, calendarName));
                cmd.AddParameter("User", "ExchangeSecurity@" + companyCode);
                cmd.AddParameter("AccessRights", "AvailabilityOnly");
                cmd.AddParameter("DomainController", this.domainController);

                powershell.Commands = cmd;
                powershell.Invoke();

                // Log the powershell commands
                LogPowershellCommands(ref powershell);

                // Find all the error
                if (powershell.HadErrors)
                {
                    // Log all warning detected
                    foreach (WarningRecord warn in powershell.Streams.Warning)
                    {
                        string warnMessage = warn.Message;
                        if (!warnMessage.Contains("completed successfully but no permissions"))
                            logger.Warn("Warning was generated running the command to modify the mailbox permissions for " + userPrincipalName + ": " + warnMessage);
                    }

                    // Log all errors detected
                    foreach (ErrorRecord err in powershell.Streams.Error)
                    {
                        string exception = err.Exception.ToString();
                        if (exception.Contains("An existing permission entry was found for user"))
                            logger.Info("Attempted to modify permission on " + userPrincipalName + " but the permission already existed.");
                        else
                            throw err.Exception;
                    }
                }

                // Stop the clock
                stopwatch.Stop();

                // Log the success
                logger.Info("Successfully modified calendar permissions for " + userPrincipalName);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Gets the calendar name because it can be in a different language
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public string Get_CalendarName(string userPrincipalName)
        {
            PowerShell powershell = null;

            try
            {
                // DEBUG //
                logger.Debug("Getting calendar name for " + userPrincipalName);

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                //
                // First we need to remove the default calendar permissions and add the security group
                //
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Get-MailboxFolderStatistics");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("FolderScope", "Calendar");
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;

                // Default calendar name in English
                string calendarName = "Calendar";

                Collection<PSObject> obj = powershell.Invoke();
                if (obj != null && obj.Count > 0)
                {
                    foreach (PSObject ps in obj)
                    {
                        if (ps.Members["FolderType"] != null)
                        {
                            string folderType = ps.Members["FolderType"].Value.ToString();

                            if (folderType.Equals("Calendar"))
                            {
                                calendarName = ps.Members["Name"].Value.ToString();
                                break;
                            }
                        }
                    }
                }

                return calendarName;
            }
            catch (Exception ex)
            {
                logger.Error("Error getting calendar name for " + userPrincipalName, ex);

                // Return the default name
                return "Calendar";
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Gets a list of Exchange database names
        /// </summary>
        /// <returns></returns>
        public List<MailboxDatabase> Get_ExchangeDatabases()
        {
            PowerShell powershell = null;

            try
            {
                // DEBUG
                logger.Debug("Retrieving a list of Exchange databases...");

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Get Databases
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Get-MailboxDatabase");
                cmd.AddParameter("Status");
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;

                // Store what we find in this list so we can return the names of the databases
                List<MailboxDatabase> databases = new List<MailboxDatabase>();

                // Now read the returned values
                Collection<PSObject> foundDatabases = powershell.Invoke();
                if (foundDatabases != null)
                {
                    foreach (PSObject o in foundDatabases)
                    {
                        MailboxDatabase db = new MailboxDatabase();
                        db.Identity = o.Members["Identity"].Value.ToString();

                        // DEBUG
                        logger.Debug("Found database " + db.Identity);

                        db.LogFolderPath = o.Members["LogFolderPath"].Value.ToString();
                        db.EDBFilePath = o.Members["EdbFilePath"].Value.ToString();
                        db.IsMailboxDatabase = bool.Parse(o.Members["IsMailboxDatabase"].Value.ToString());
                        db.IsPublicFolderDatabase = bool.Parse(o.Members["IsPublicFolderDatabase"].Value.ToString());

                        if (o.Members["Server"].Value != null)
                            db.Server = o.Members["Server"].Value.ToString();

                        if (o.Members["DatabaseSize"].Value != null)
                        {
                            db.DatabaseSize = o.Members["DatabaseSize"].Value.ToString();

                            // DEBUG
                            logger.Debug("Size of the database is " + o.Members["DatabaseSize"].Value.ToString());
                            logger.Debug("Size of the database after formatted is " + db.DatabaseSize.ToString());
                        }


                        databases.Add(db);
                    }
                }

                // Log the powershell commands
                LogPowershellCommands(ref powershell);

                // Find all the error
                if (powershell.HadErrors)
                {
                    // Log all errors detected
                    foreach (ErrorRecord err in powershell.Streams.Error)
                    {
                        string exception = err.Exception.ToString();

                        // Log the exception
                        logger.Fatal("Error retrieving mailbox database sizes: " + exception);

                        throw err.Exception;
                    }
                }

                // Stop the clock
                stopwatch.Stop();

                // Log the success
                logger.Info("Successfully retrieved a list of databases from Exchange.");

                // Return values
                return databases;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Gets a list of mailbox sizes Exchange
        /// </summary>
        /// <returns></returns>
        public List<MailboxUser> Get_MailboxSizes()
        {
            PowerShell powershell = null;

            try
            {
                // DEBUG
                logger.Debug("Retrieving a list of mailbox users from the SQL database...");

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Get a list of users from the database
                List<ADUser> allUsers = DbSql.Get_Users();

                // Our return object of information
                List<MailboxUser> users = new List<MailboxUser>();

                // Now loop through the databases and query the information
                foreach (ADUser user in allUsers)
                {
                    if (user.MailboxPlanID > 0)
                    {
                        // DEBUG
                        logger.Debug("Retrieving mailbox statistics for user " + user);

                        // Our MailboxUser object to store this users information
                        MailboxUser currentUser = new MailboxUser();

                        try
                        {
                            // Run commands
                            powershell = PowerShell.Create();
                            powershell.Runspace = runspace;

                            // Get Databases
                            PSCommand cmd = new PSCommand();
                            cmd.AddCommand("Get-MailboxStatistics");
                            cmd.AddParameter("Identity", user.UserPrincipalName);
                            cmd.AddParameter("DomainController", this.domainController);
                            powershell.Commands = cmd;

                            // Now read the returned values
                            Collection<PSObject> foundStatistics = powershell.Invoke();
                            foreach (PSObject o in foundStatistics)
                            {
                                currentUser.UserPrincipalName = user.UserPrincipalName;
                                currentUser.ItemCount = int.Parse(o.Members["ItemCount"].Value.ToString());
                                currentUser.DeletedItemCount = int.Parse(o.Members["DeletedItemCount"].Value.ToString());
                                currentUser.TotalItemSize = o.Members["TotalItemSize"].Value.ToString();
                                currentUser.TotalDeletedItemSize = o.Members["TotalDeletedItemSize"].Value.ToString();
                                currentUser.Database = o.Members["Database"].Value.ToString();
                                currentUser.MailboxDataRetrieved = DateTime.Now;
                            }

                            // Log the powershell commands
                            LogPowershellCommands(ref powershell);

                            // Find all the errors
                            if (powershell.HadErrors)
                            {
                                // Log all errors detected
                                foreach (ErrorRecord err in powershell.Streams.Error)
                                {
                                    logger.Error("Error getting mailbox size for " + user, err.Exception);

                                    // Compile message
                                    StringBuilder sb = new StringBuilder();
                                    sb.AppendLine("Failed to get mailbox size for: " + user);
                                    sb.AppendLine("");
                                    sb.AppendLine("Recommended Action:");
                                    sb.AppendLine("This could be because the user no longer exists in Active Directory but still exists in the database.");
                                    sb.AppendLine("If that is the case simply delete the user from CloudPanel. If the issues stays the same please contact support.");
                                    sb.AppendLine("");
                                    sb.AppendLine("Error:");
                                    sb.AppendLine(err.Exception.ToString());

                                    // Send message
                                    Support.SendEmailMessage("Failed to get mailbox size for: " + user, sb.ToString());
                                }

                                // Log all warnings detected
                                foreach (WarningRecord err in powershell.Streams.Warning)
                                {
                                    logger.Error("Warning getting mailbox size for " + user + ": " + err.Message);
                                }
                            }
                            else
                            {
                                logger.Info("Successfully retrieved mailbox size information for " + currentUser.UserPrincipalName);

                                users.Add(currentUser);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Fatal("Failed to retrieve mailbox size for " + user, ex);
                        }
                    }
                }

                // Stop the clock
                stopwatch.Stop();

                // Log the success
                logger.Info("Successfully retrieved a complete list of mailbox sizes from Exchange.");

                // Return values
                return users;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }
        
        /// <summary>
        /// Checks for errors, logs to log file, and throws error
        /// </summary>
        /// <param name="errorCheck"></param>
        private void CheckErrors(ref PowerShell errorCheck)
        {
            //
            // Log the powershell commands that were ran if debugging is enabled
            //
            if (logger.IsDebugEnabled)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Command cmd in errorCheck.Commands.Commands)
                {
                    sb.AppendFormat("[Powershell Command]: {0} ", cmd.CommandText);

                    // Get parameters for this command
                    foreach (CommandParameter parm in cmd.Parameters)
                    {
                        sb.AppendFormat("-{0} {1} ", parm.Name, parm.Value);
                    }

                    // Add line
                    sb.AppendLine("");
                }

                // Log the command
                logger.Info(sb.ToString());
            }

            // Now check for errors
            if (errorCheck.HadErrors)
            {
                // Log all errors detected
                foreach (ErrorRecord err in errorCheck.Streams.Error)
                {
                    logger.Error("[Powershell Error]: " + err.Exception.ToString());
                }

                // Log all warning detected
                if (logger.IsWarnEnabled)
                {
                    foreach (WarningRecord warn in errorCheck.Streams.Warning)
                    {
                        logger.Warn("[Powershell Warning]: " + warn.Message);
                    }
                }

                throw errorCheck.Streams.Error[0].Exception;
            }
        }

        /// <summary>
        /// Logs all the powershell commands that are ran.
        /// </summary>
        /// <param name="commands"></param>
        private void LogPowershellCommands(ref PowerShell commands)
        {
            //
            // Log the powershell commands that were ran if debugging is enabled
            //
            StringBuilder sb = new StringBuilder();
            foreach (Command cmd in commands.Commands.Commands)
            {
                sb.AppendFormat("[Powershell Command]: {0} ", cmd.CommandText);

                // Get parameters for this command
                foreach (CommandParameter parm in cmd.Parameters)
                {
                    sb.AppendFormat("-{0} {1} ", parm.Name, parm.Value);
                }

                // Add line
                sb.AppendLine("");
            }

            // Log the command
            logger.Debug(sb.ToString());
        }

        /// <summary>
        /// Create our connection information
        /// </summary>
        /// <param name="uri">Uri to Exchange server powershell directory</param>
        /// <param name="username">Username to connect</param>
        /// <param name="password">Password to connect</param>
        /// <param name="kerberos">True to use Kerberos authentication, false to use Basic authentication</param>
        /// <returns></returns>
        private WSManConnectionInfo GetConnection(string uri, string username, string password, bool kerberos)
        {
            SecureString pwd = new SecureString();
            foreach (char x in password)
                pwd.AppendChar(x);

            PSCredential ps = new PSCredential(username, pwd);

            WSManConnectionInfo wsinfo = new WSManConnectionInfo(new Uri(uri), "http://schemas.microsoft.com/powershell/Microsoft.Exchange", ps);
            wsinfo.SkipCACheck = true;
            wsinfo.SkipCNCheck = true;
            wsinfo.SkipRevocationCheck = true;
            wsinfo.OpenTimeout = 9000;

            if (kerberos)
                wsinfo.AuthenticationMechanism = AuthenticationMechanism.Kerberos;
            else
                wsinfo.AuthenticationMechanism = AuthenticationMechanism.Basic;

            return wsinfo;
        }

        /// <summary>
        /// Disposing
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    wsConn = null;

                    if (runspace != null)
                        runspace.Dispose();
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
