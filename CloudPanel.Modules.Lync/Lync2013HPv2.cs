using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Settings;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Security;
using System.Text;

namespace CloudPanel.Modules.Lync
{
    public class Lync2013HPv2 : IDisposable
    {
        #region Variables

        // Disposing information
        private bool disposed = false;

        // Our connection information to the Exchange server
        private WSManConnectionInfo wsConn;

        // Pipeline Information
        Runspace runspace = null;

        // Other Settings
        private string domainController, username, password = string.Empty;

        // Logger
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Constructors

        public Lync2013HPv2(string uri, string username, string password, Enumerations.ConnectionType connectionType, string domainController)
        {
            this.wsConn = GetConnection(uri, username, password, connectionType == Enumerations.ConnectionType.Kerberos ? true : false);
            this.domainController = domainController;
            this.username = username;
            this.password = password;

            // Create our runspace
            runspace = RunspaceFactory.CreateRunspace(wsConn);
            logger.Debug("Creating new runspace for Lync");

            // Open the connection
            runspace.Open();
            logger.Debug("Connection to Lync server has been opened");
        }

        #endregion

        #region Commands

        /// <summary>
        /// Enables a company for Lync
        /// </summary>
        /// <param name="companyOrgUnit"></param>
        /// <param name="companyCode"></param>
        /// <param name="plan"></param>
        public void Enable_Company(string companyOrgUnit, string companyCode, LyncPlan plan)
        {
            PowerShell powershell = null;
            DirectoryEntry de = null;

            try
            {
                // Connect to AD
                de = new DirectoryEntry("LDAP://" + this.domainController + "/" + companyOrgUnit, this.username, this.password);

                // Retrieve the objectGUID so we can set the values
                logger.Debug("Connecting to path " + de.Path);

                Guid objectGuid = de.Guid;

                // Retrieve the upnSuffixes from the OU
                List<string> upnSuffixes = new List<string>();
                if (de.Properties["upnSuffixes"].Value != null)
                {
                    foreach (object v in de.Properties["upnSuffixes"])
                        upnSuffixes.Add(v.ToString());
                }
                logger.Debug("Total upnSuffixes found for " + companyCode + ": " + upnSuffixes.Count.ToString());

                if (upnSuffixes.Count < 1)
                    throw new Exception("Unable to continue because the company code " + companyCode + " has zero upnSuffixes configured on the OU. Please contact support.");

                // Now set the TenantID, ObjectID, and the Lync Domain suffixes on the company
                de.Properties["msRTCSIP-TenantID"].Value = objectGuid.ToByteArray();
                de.Properties["msRTCSIP-ObjectID"].Value = objectGuid.ToByteArray();
                de.Properties["msRTCSIP-Domains"].Value = upnSuffixes.ToArray();
                logger.Debug("Setting TenantID and ObjectID on the company's organizational unit.");

                foreach (string upn in upnSuffixes)
                    de.Properties["msRTCSIP-DomainUrlMap"].Add(string.Format("{0}#{1}/{2}", upn, Config.LyncMeetUri, upn));
                logger.Debug("Setting msRTCSIP-DomainUrlMap on the company's organizational unit.");

                // Save changes to OU
                de.CommitChanges();
                logger.Debug("Successfully committed changes to the Active Directory store.");

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("New-CsExternalAccessPolicy");
                cmd.AddParameter("Identity", companyCode);
                cmd.AddParameter("EnableFederationAccess", plan.EnableFederation);
                cmd.AddParameter("EnableOutsideAccess", plan.EnableOutsideAccess);
                cmd.AddParameter("EnablePublicCloudAccess", plan.EnablePublicCloudAccess);
                cmd.AddParameter("EnablePublicCloudAudioVideoAccess", plan.EnablePublicCloudAudioVideoAccess);
                powershell.Commands = cmd;
                powershell.Invoke();

                CheckErrors(ref powershell, false, true); // Check for errors after command
                logger.Debug("Successfully created new external access policy for " + companyCode);

                cmd = new PSCommand();
                cmd.AddCommand("New-CsConferencingPolicy");
                cmd.AddParameter("Identity", companyCode);
                cmd.AddParameter("AllowIPVideo", plan.AllowIPVideo);
                cmd.AddParameter("MaxMeetingSize", plan.MaxMeetingSize);
                powershell.Commands = cmd;
                powershell.Invoke();

                CheckErrors(ref powershell, false, true); // Check for errors after command
                logger.Debug("Successfully created new conferencing policy for " + companyCode);

                cmd = new PSCommand();
                cmd.AddCommand("New-CsMobilityPolicy");
                cmd.AddParameter("Identity", companyCode);
                cmd.AddParameter("EnableMobility", plan.EnableMobility);
                cmd.AddParameter("EnableOutsideVoice", plan.EnableOutsideVoice);
                powershell.Commands = cmd;
                powershell.Invoke();

                CheckErrors(ref powershell, false, true); // Check for errors after command
                logger.Debug("Successfully created new mobility policy for " + companyCode);

                cmd = new PSCommand();
                cmd.AddCommand("New-CsVoicePolicy");
                cmd.AddParameter("Identity", companyCode);
                powershell.Commands = cmd;
                powershell.Invoke();

                CheckErrors(ref powershell, false, true);
                logger.Debug("Successfully created new voice policy for " + companyCode);

                cmd = new PSCommand();
                cmd.AddCommand("New-CsDialPlan");
                cmd.AddParameter("Identity", companyCode);
                powershell.Commands = cmd;
                powershell.Invoke();

                CheckErrors(ref powershell, false, true);
                logger.Debug("Successfully created new dial plan for " + companyCode);

                // Now invoke replication
                cmd = new PSCommand();
                cmd.AddCommand("Invoke-CsManagementStoreReplication");
                powershell.Commands = cmd;
                powershell.Invoke();

                CheckErrors(ref powershell, false, true); // Check for errors after command
                logger.Debug("Successfully invoked replication of the management store.");

                // Create our simple urls
                Create_CompanySimpleUrls(objectGuid, upnSuffixes);

                logger.Info("Successfully enabled company code " + companyCode + " for Lync");
            }
            catch (Exception ex)
            {
                logger.Error("Error enabling company " + companyCode + " for Lync", ex);

                throw;
            }
            finally
            {
                if (de != null)
                    de.Dispose();

                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Creates the company's simple url
        /// </summary>
        /// <param name="companyGuid"></param>
        /// <param name="upnSuffixes"></param>
        internal void Create_CompanySimpleUrls(Guid companyGuid, List<string> upnSuffixes)
        {
            PowerShell powershell = null;

            try
            {
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = null;

                // Keep our list of PSObjects to add to the simple url configuration
                List<PSObject> newSimpleUrls = new List<PSObject>();

                // Loop through and create a simple url for each domain
                foreach (string domain in upnSuffixes)
                {
                    string meetUrl = string.Format("{0}/{1}", Config.LyncMeetUri, domain);

                    cmd = new PSCommand();
                    cmd.AddCommand("New-CsSimpleUrlEntry");
                    cmd.AddParameter("Url", meetUrl);
                    powershell.Commands = cmd;
                    Collection<PSObject> newSimpleUrlEntry = powershell.Invoke();
                    logger.Debug("Successfully created New-CsSimpleUrlEntry for guid " + companyGuid.ToString());

                    cmd = new PSCommand();
                    cmd.AddCommand("New-CsSimpleUrl");
                    cmd.AddParameter("Component", "meet");
                    cmd.AddParameter("Domain", domain);
                    cmd.AddParameter("SimpleUrl", newSimpleUrlEntry[0]);
                    cmd.AddParameter("ActiveUrl", meetUrl);
                    powershell.Commands = cmd;
                    Collection<PSObject> newCsSimpleUrl = powershell.Invoke();
                    logger.Debug("Successfully created New-CsSimpleUrl for guid " + companyGuid.ToString());

                    // Add to our list
                    newSimpleUrls.Add(newCsSimpleUrl[0]);
                }

                // Recreate our dialin url
                cmd = new PSCommand();
                cmd.AddCommand("New-CsSimpleUrlEntry");
                cmd.AddParameter("Url", Config.LyncdialinUri);
                powershell.Commands = cmd;
                Collection<PSObject> newDialinUrl = powershell.Invoke();
                logger.Debug("Successfully created New-CsSimpleUrlEntry for the Dialin URL for guid " + companyGuid.ToString());

                cmd = new PSCommand();
                cmd.AddCommand("New-CsSimpleUrl");
                cmd.AddParameter("Component", "dialin");
                cmd.AddParameter("Domain", "*");
                cmd.AddParameter("SimpleUrl", newDialinUrl[0]);
                cmd.AddParameter("ActiveUrl", Config.LyncdialinUri);
                powershell.Commands = cmd;
                Collection<PSObject> newCsDialinSimpleUrl = powershell.Invoke();
                logger.Debug("Successfully created New-CsSimpleUrl for the Dialin URL for guid " + companyGuid.ToString());

                // Add to our list
                newSimpleUrls.Add(newCsDialinSimpleUrl[0]);

                // Now set the simple url configuration for the tenant
                cmd = new PSCommand();
                cmd.AddCommand("Set-CsSimpleUrlConfiguration");
                cmd.AddParameter("Tenant", companyGuid);
                cmd.AddParameter("SimpleUrl", newSimpleUrls);
                cmd.AddParameter("UseBackendDatabase", true);
                powershell.Commands = cmd;
                powershell.Invoke();
                logger.Debug("Successfully set the simple url configuration for guid " + companyGuid.ToString());

                CheckErrors(ref powershell);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        #endregion

        #region Error checking

        /// <summary>
        /// Checks for errors, logs to log file, and throws error
        /// </summary>
        /// <param name="errorCheck"></param>
        private void CheckErrors(ref PowerShell errorCheck, bool skipCouldntBeFound = false, bool skipAlreadyExists = false)
        {
            // Now check for errors
            if (errorCheck.HadErrors)
            {
                foreach (ErrorRecord errs in errorCheck.Streams.Error)
                {
                    this.logger.Error("[Powershell Error]: " + errs.Exception.ToString());

                    if (errs.ErrorDetails != null)
                    {
                        if (!string.IsNullOrEmpty(errs.ErrorDetails.RecommendedAction))
                            this.logger.Info("[Recommended Action]: " + errs.ErrorDetails.RecommendedAction);
                    }
                }

                if (errorCheck.Streams.Error[0].Exception.ToString().Contains("couldn't be found on") && skipCouldntBeFound)
                {
                    // Skip on errors about it not being found
                    this.logger.Debug("Skipped error about the object not being found: " + errorCheck.Streams.Error[0].Exception.ToString());

                    // Clear errors because if we are skipping that it couldn't be found then we are probably
                    // running commands back to back
                    errorCheck.Streams.Error.Clear();
                }
                else
                    throw errorCheck.Streams.Error[0].Exception;
            }
        }

        #endregion

        #region Connection Information
        /// <summary>
        /// Create our connection information
        /// </summary>
        /// <param name="uri">Uri to Lync server</param>
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

            WSManConnectionInfo wsinfo = new WSManConnectionInfo(new Uri("https://lync.lab.local/OCSPowershell"), "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", ps);
            wsinfo.AuthenticationMechanism = AuthenticationMechanism.Default;
            wsinfo.SkipCACheck = true;
            wsinfo.SkipCNCheck = true;
            wsinfo.SkipRevocationCheck = true;
            wsinfo.OpenTimeout = 9000;

            return wsinfo;
        }

        #endregion

        #region Disposing

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

        #endregion
    }
}
