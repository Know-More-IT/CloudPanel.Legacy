using CloudPanel.Modules.ActiveDirectory;
using CloudPanel.Modules.Base;
using CloudPanel.Modules.Base.Class;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Security;
using System.Text;

namespace CloudPanel.Modules.Exchange
{
    public class ExchCmds : IDisposable
    {
        // Disposing information
        private bool disposed = false;

        // Our connection information to the Exchange server
        private WSManConnectionInfo wsConn;

        // Pipeline Information
        Runspace runspace = null;

        // Domain Controller to communciate with
        private string domainController = string.Empty;

        // Logger
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Address list types
        public enum AddressListType
        {
            ConferenceRoomMailbox,
            User,
            Contact,
            Group
        }

        /// <summary>
        /// Start new object
        /// </summary>
        /// <param name="uri">Uri to Exchange server powershell directory</param>
        /// <param name="username">Username to connect</param>
        /// <param name="password">Password to connect</param>
        /// <param name="kerberos">True to use Kerberos authentication, false to use Basic authentication</param>
        /// <param name="domainController">Domain controller to communicate with</param>
        public ExchCmds(string uri, string username, string password, Enumerations.ConnectionType connectionType, string domainController)
        {
            // DEBUG //
            this.logger.Debug(string.Format("Opened Exchange powershell class with {0}, {1}, {2}, {3}, {4}", uri, username, password, connectionType.ToString(), domainController));

            this.wsConn = GetConnection(uri, username, password, connectionType == Enumerations.ConnectionType.Kerberos ? true : false);
            this.domainController = domainController;

            // Create our runspace
            runspace = RunspaceFactory.CreateRunspace(wsConn);

            // Open the connection
            runspace.Open();
        }

        /// <summary>
        /// Enables a company for Exchange by creating all the address lists and other required objects
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="groupForDownloadRights"></param>
        public void Enable_Company(string companyCode, string groupForDownloadRights, string companyExchangeOU)
        {
            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // INFO //
                this.logger.Info("Starting to create new company in Exchange for " + companyCode);

                // Create the global address list first
                New_GlobalAddressList(companyCode);

                // Create all of the address lists next
                New_AddressList(companyCode, AddressListType.User);
                New_AddressList(companyCode, AddressListType.Group);
                New_AddressList(companyCode, AddressListType.Contact);
                New_AddressList(companyCode, AddressListType.ConferenceRoomMailbox);

                // Create the offline address book policy
                New_OfflineAddressBook(companyCode, groupForDownloadRights);

                // Create the address book policy
                New_AddressBookPolicy(companyCode);

                // Create our security distribution group
                New_SecurityDistributionGroup("ExchangeSecurity", companyCode, companyExchangeOU);
            }
            catch (Exception ex)
            {
                this.logger.Fatal("Failed to completely enable " + companyCode + " for Exchange.", ex);

                throw;
            }
        }

        /// <summary>
        /// Deletes all mailboxes, GAL, OAL, address lists, etc associated with the company
        /// </summary>
        /// <param name="companyCode"></param>
        public void Disable_Company(string companyCode, List<string> domains)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // INFO //
                this.logger.Warn("Starting to disable company in Exchange for " + companyCode);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = null;

                //
                // Lets first disable all mailboxes
                //
                cmd = new PSCommand();
                cmd.AddCommand("Get-Mailbox");
                cmd.AddParameter("Filter", string.Format("CustomAttribute1 -eq '{0}'", companyCode));
                cmd.AddParameter("DomainController", domainController);
                cmd.AddCommand("Disable-Mailbox");
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();
                this.logger.Info("Disabled all mailboxes for company " + companyCode);

                // Check for errors
                CheckErrors(ref powershell);

                //
                // Lets lets delete all contacts
                //
                cmd = new PSCommand();
                cmd.AddCommand("Get-MailContact");
                cmd.AddParameter("Filter", string.Format("CustomAttribute1 -eq '{0}'", companyCode));
                cmd.AddParameter("DomainController", domainController);
                cmd.AddCommand("Remove-MailContact");
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();
                this.logger.Info("Deleted all contacts for company " + companyCode);

                // Check for errors
                CheckErrors(ref powershell);

                //
                // Lets lets delete all distribution groups
                //
                cmd = new PSCommand();
                cmd.AddCommand("Get-DistributionGroup");
                cmd.AddParameter("Filter", string.Format("CustomAttribute1 -eq '{0}'", companyCode));
                cmd.AddParameter("DomainController", domainController);
                cmd.AddCommand("Remove-DistributionGroup");
                cmd.AddParameter("BypassSecurityGroupManagerCheck");
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();
                this.logger.Info("Deleted all distribution groups for company " + companyCode);

                // Check for errors
                CheckErrors(ref powershell);

                //
                // Delete Address Book Policy
                //
                cmd = new PSCommand();
                cmd.AddCommand("Remove-AddressBookPolicy");
                cmd.AddParameter("Identity", companyCode + " ABP");
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();
                this.logger.Info("Deleted address book policy for " + companyCode);

                // Check for errors
                CheckErrors(ref powershell, true);

                //
                // Delete Offline Address Book
                //
                cmd = new PSCommand();
                cmd.AddCommand("Remove-OfflineAddressBook");
                cmd.AddParameter("Identity", companyCode + " OAL");
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();
                this.logger.Info("Deleted offline address book for " + companyCode);

                // Check for errors
                CheckErrors(ref powershell, true);

                //
                // Delete Conference Room Mailbox List
                //
                cmd = new PSCommand();
                cmd.AddCommand("Remove-AddressList");
                cmd.AddParameter("Identity", companyCode + " - All Rooms");
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();
                this.logger.Info("Deleted conference room list for " + companyCode);

                // Check for errors
                CheckErrors(ref powershell, true);

                //
                // Delete Contact Address List
                //
                cmd = new PSCommand();
                cmd.AddCommand("Remove-AddressList");
                cmd.AddParameter("Identity", companyCode + " - All Contacts");
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();
                this.logger.Info("Deleted all contacts list for " + companyCode);

                // Check for errors
                CheckErrors(ref powershell, true);

                //
                // Delete Group Address List
                //
                cmd = new PSCommand();
                cmd.AddCommand("Remove-AddressList");
                cmd.AddParameter("Identity", companyCode + " - All Groups");
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();
                this.logger.Info("Deleted all groups list for " + companyCode);

                // Check for errors
                CheckErrors(ref powershell, true);

                //
                // Delete Users list
                //
                cmd = new PSCommand();
                cmd.AddCommand("Remove-AddressList");
                cmd.AddParameter("Identity", companyCode + " - All Users");
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();
                this.logger.Info("Deleted all users list for " + companyCode);

                // Check for errors
                CheckErrors(ref powershell, true);

                //
                // Delete Global Address List
                //
                cmd = new PSCommand();
                cmd.AddCommand("Remove-GlobalAddressList");
                cmd.AddParameter("Identity", companyCode + " GAL");
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();
                this.logger.Info("Deleted global address list for " + companyCode);

                // Check for errors
                CheckErrors(ref powershell, true);

                //
                // Delete all accepted domains
                //
                foreach (string d in domains)
                {
                    cmd = new PSCommand();
                    cmd.AddCommand("Remove-AcceptedDomain");
                    cmd.AddParameter("Identity", d);
                    cmd.AddParameter("Confirm", false);
                    cmd.AddParameter("DomainController", domainController);
                    powershell.Commands = cmd;
                    powershell.Invoke();
                    this.logger.Info("Deleted accepted domain for " + companyCode);

                    // Check for errors
                    CheckErrors(ref powershell, true);
                }

                // Stop the clock
                stopwatch.Stop();

                // INFO //
                logger.Info("Successfully disabled Exchange for company " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                this.logger.Fatal("Failed to disable copmany " + companyCode + " for Exchange.", ex);
                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Creates a new Exchange contact
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="emailAddress"></param>
        /// <param name="hidden"></param>
        /// <param name="companyDN"></param>
        /// <param name="companyCode"></param>
        public void New_Contact(string displayName, string emailAddress, bool hidden, string companyDN, string companyCode)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Starting to create new contact " + emailAddress + " for company code " + companyCode);

                // DEBUG //
                this.logger.Debug(string.Format("New contact parameters: {0}, {1}, {2}, {3}, {4}", displayName, emailAddress, hidden.ToString(), companyDN, companyCode));
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("New-MailContact");
                cmd.AddParameter("Alias", emailAddress.Replace("@", "_"));
                cmd.AddParameter("Name", emailAddress.Split('@')[0] + "_" + companyCode);
                cmd.AddParameter("PrimarySmtpAddress", emailAddress.Split('@')[0] + "_" + companyCode + "@" + emailAddress.Split('@')[1]);
                cmd.AddParameter("ExternalEmailAddress", emailAddress);
                cmd.AddParameter("DisplayName", displayName);
                cmd.AddParameter("OrganizationalUnit", companyDN);
                cmd.AddParameter("DomainController", domainController);

                cmd.AddCommand("Set-MailContact");
                cmd.AddParameter("CustomAttribute1", companyCode);
                cmd.AddParameter("HiddenFromAddressListsEnabled", hidden);
                cmd.AddParameter("EmailAddressPolicyEnabled", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                logger.Info("Successfully created new contact " + emailAddress + " in Exchange in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to create new contact " + emailAddress, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Gets a contact and its information from Exchange
        /// </summary>
        /// <param name="distinguishedName"></param>
        /// <returns></returns>
        public MailContact Get_Contact(string distinguishedName)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Retrieving contact " + distinguishedName + " from Exchange");

                // Object to return
                MailContact contact = new MailContact();

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Get-MailContact");
                cmd.AddParameter("Identity", distinguishedName);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;

                foreach (PSObject ps in powershell.Invoke())
                {
                    contact.DisplayName = ps.Members["DisplayName"].Value.ToString();
                    contact.DistinguishedName = distinguishedName;
                    contact.Hidden = bool.Parse(ps.Members["HiddenFromAddressListsEnabled"].Value.ToString());
                    contact.ExternalEmailAddress = ps.Members["ExternalEmailAddress"].Value.ToString().Replace("SMTP:", string.Empty);
                }

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully retrieved contact " + distinguishedName + " from Exchange in " + stopwatch.Elapsed.ToString() + " second(s)");

                // Return our object
                return contact;
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to retrieve contact " + distinguishedName + " from Exchange.", ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Updates a contact
        /// </summary>
        /// <param name="contact"></param>
        public void Set_Contact(MailContact contact)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Starting to update contact " + contact.DistinguishedName);

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Set-MailContact");
                cmd.AddParameter("Identity", contact.DistinguishedName);
                cmd.AddParameter("HiddenFromAddressListsEnabled", contact.Hidden);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                logger.Info("Successfully updated contact " + contact.DistinguishedName + " in Exchange in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to update contact " + contact.DistinguishedName, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Removes a mail contact from Exchange
        /// </summary>
        /// <param name="distinguishedName"></param>
        public void Remove_Contact(string distinguishedName)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Deleting contact " + distinguishedName + " from Exchange");
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Remove-MailContact");
                cmd.AddParameter("Identity", distinguishedName);
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully deleted contact " + distinguishedName + " from Exchange in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // Do not throw error if it cannot be found
                if (!ex.ToString().Contains("couldn't be found on"))
                {
                    // FATAL //
                    this.logger.Fatal("Failed to delete contact " + distinguishedName + " from Exchange.", ex);

                    throw;
                }
                else
                    this.logger.Info("Powershell error was thrown because the contact does not exist in Exchange. Continuing without error.");
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Creates a new accepted domain in Exchange
        /// </summary>
        /// <param name="domainName">DomainName to create</param>
        public void New_AcceptedDomain(string domainName)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Enabling " + domainName + " as an Accepted Domain in Exchange");
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("New-AcceptedDomain");
                cmd.AddParameter("Name", domainName);
                cmd.AddParameter("DomainName", domainName);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully enabled " + domainName + " as an Accepted Domain in Exchange in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // Do not throw error if it already exists
                if (!ex.ToString().Contains("already exists"))
                {
                    // FATAL //
                    this.logger.Fatal("Failed to enable " + domainName + " as an Accepted Domain.", ex);

                    throw;
                }
                else
                    this.logger.Info("Powershell error was thrown because the accepted domain already existed. Continuing without error.");
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Removes a new accepted domain in Exchange
        /// </summary>
        /// <param name="domainName">DomainName to remove</param>
        public void Remove_AcceptedDomain(string domainName)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Disabling " + domainName + " as an Accepted Domain in Exchange");
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Remove-AcceptedDomain");
                cmd.AddParameter("Identity", domainName);
                cmd.AddParameter("DomainController", domainController);
                cmd.AddParameter("Confirm", false);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully disabled " + domainName + " as an Accepted Domain in Exchange in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception)
            {
                // FATAL //
                this.logger.Fatal("Failed to disable " + domainName + " as an Accepted Domain in Exchange.");

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Creates a new global address list in Exchange
        /// </summary>
        /// <param name="companyCode">Company Code to create the GAL for</param>
        public void New_GlobalAddressList(string companyCode)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Creating new global address list for company code " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("New-GlobalAddressList");
                cmd.AddParameter("Name", companyCode + " GAL");
                cmd.AddParameter("RecipientFilter", string.Format("(Alias -ne $null) -and (CustomAttribute1 -eq '{0}')", companyCode));
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully created new global address list for company code " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // Do not throw error if it already exists
                if (!ex.ToString().Contains("already exists"))
                {
                    // FATAL //
                    this.logger.Fatal("Failed to create new global address list for company code " + companyCode, ex);

                    throw;
                }
                else
                    this.logger.Info("Powershell error was thrown because the global address list already existed. Continuing without error.");

            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Creates a new offline address book for a specific company
        /// </summary>
        /// <param name="companyCode">Company Code to create the OAB for</param>
        /// <param name="groupAllowedToDownload">Group granted rights to download OAB</param>
        public void New_OfflineAddressBook(string companyCode, string groupAllowedToDownload)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Creating new offline address book for company code " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("New-OfflineAddressBook");
                cmd.AddParameter("Name", companyCode + " OAL");
                cmd.AddParameter("AddressLists", companyCode + " GAL");
                cmd.AddParameter("DomainController", domainController);
                cmd.AddCommand("Add-ADPermission");
                cmd.AddParameter("User", groupAllowedToDownload.Replace(" ", string.Empty));
                cmd.AddParameter("ExtendedRights", "MS-EXCH-DOWNLOAD-OAB");
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully created new offline address book for company code " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // Do not throw error if it already exists
                if (!ex.ToString().Contains("already exists"))
                    throw;
                else
                    this.logger.Info("Powershell error was thrown because the offline address book already existed. Continuing without error.");
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Creates a new address book policy for a specific company
        /// </summary>
        /// <param name="companyCode">Company Code to create address book policy for</param>
        public void New_AddressBookPolicy(string companyCode)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Creating new address book policy for company code " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("New-AddressBookPolicy");
                cmd.AddParameter("Name", companyCode + " ABP");
                cmd.AddParameter("AddressLists", new string[] { companyCode + " - All Users", companyCode + " - All Contacts", companyCode + " - All Groups" });
                cmd.AddParameter("GlobalAddressList", companyCode + " GAL");
                cmd.AddParameter("OfflineAddressBook", companyCode + " OAL");
                cmd.AddParameter("RoomList", companyCode + " - All Rooms");
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully created new address book policy for company code " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // Do not throw error if it already exists
                if (!ex.ToString().Contains("already exists"))
                    throw;
                else
                    this.logger.Info("Powershell error was thrown because the address book policy already existed. Continuing without error.");
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Creates a new address list for a specific company
        /// </summary>
        /// <param name="companyCode">CompanyCode to create address list for</param>
        /// <param name="addrType">Type of address list to add</param>
        public void New_AddressList(string companyCode, AddressListType addrType)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Creating new address list [" + addrType.ToString() + "] for company code " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Figure out what address list we are creating
                string recipientFilter = string.Empty;
                string addressListName = string.Empty;

                switch (addrType)
                {
                    case AddressListType.ConferenceRoomMailbox:
                        recipientFilter = string.Format(@"((Alias -ne $null) -and (CustomAttribute1 -eq '{0}') -and (((RecipientDisplayType -eq 'ConferenceRoomMailbox') -or (RecipientDisplayType -eq 'SyncedConferenceRoomMailbox'))))", companyCode);
                        addressListName = string.Format("{0} - {1}", companyCode, "All Rooms");
                        break;
                    case AddressListType.User:
                        recipientFilter = string.Format(@"((Alias -ne $null) -and (CustomAttribute1 -eq '{0}') -and (((((ObjectCategory -like 'person') -and (ObjectClass -eq 'user') -and (-not(Database -ne $null)) -and (-not(ServerLegacyDN -ne $null)))) -or (((ObjectCategory -like 'person') -and (ObjectClass -eq 'user') -and (((Database -ne $null) -or (ServerLegacyDN -ne $null))))))))", companyCode);
                        addressListName = string.Format("{0} - {1}", companyCode, "All Users");
                        break;
                    case AddressListType.Contact:
                        recipientFilter = string.Format(@"((Alias -ne $null) -and (CustomAttribute1 -eq '{0}') -and (((ObjectCategory -like 'person') -and (ObjectClass -eq 'contact'))))", companyCode);
                        addressListName = string.Format("{0} - {1}", companyCode, "All Contacts");
                        break;
                    case AddressListType.Group:
                        recipientFilter = string.Format(@"((Alias -ne $null) -and (CustomAttribute1 -eq '{0}') -and (ObjectCategory -like 'group'))", companyCode);
                        addressListName = string.Format("{0} - {1}", companyCode, "All Groups");
                        break;
                    default:
                        throw new Exception("Invalid address list type was supplied. Supplied value: " + addrType.ToString());

                }

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("New-AddressList");
                cmd.AddParameter("Name", addressListName);
                cmd.AddParameter("RecipientFilter", recipientFilter);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully created new address list [" + addrType.ToString() + "] for company code " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // Do not throw error if it already exists
                if (!ex.ToString().Contains("already exists"))
                    throw;
                else
                    this.logger.Info("Powershell error was thrown because the address list already existed. Continuing without error.");
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Disables a mailbox in Exchange
        /// </summary>
        /// <param name="userPrincipalName"></param>
        public void Disable_Mailbox(string userPrincipalName)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Disabling " + userPrincipalName + "'s mailbox.");
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Disable-Mailbox");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully disabled " + userPrincipalName + "'s mailbox in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // Do not throw error if it already exists
                if (!ex.ToString().Contains("couldn't be found on"))
                    throw;
                else
                    this.logger.Info("Powershell error was thrown because the mailbox does not exist in Exchange. Continuing without error.");
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Enables a mailbox
        /// </summary>
        /// <param name="u"></param>
        /// <param name="p"></param>
        public void Enable_Mailbox(MailboxUser u, MailboxPlan p)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                u.CompanyCode = u.CompanyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Enabling mailbox for user [" + u.UserPrincipalName + "] for company code " + u.CompanyCode);

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Enable the mailbox
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Enable-Mailbox");
                cmd.AddParameter("Identity", u.UserPrincipalName);
                cmd.AddParameter("AddressBookPolicy", u.CompanyCode + " ABP");
                cmd.AddParameter("PrimarySmtpAddress", u.PrimarySmtpAddress);
                cmd.AddParameter("Alias", u.PrimarySmtpAddress.Replace("@", "_"));

                if (!string.IsNullOrEmpty(u.Database))
                    cmd.AddParameter("Database", u.Database); // If there was a certain database to enable the user in

                cmd.AddParameter("DomainController", domainController);

                 // Set the mailbox settings
                cmd.AddStatement();
                cmd.AddCommand("Set-Mailbox");
                cmd.AddParameter("Identity", u.UserPrincipalName);
                cmd.AddParameter("EmailAddressPolicyEnabled", false);
                cmd.AddParameter("IssueWarningQuota", p.WarningSizeInMB + "MB");
                cmd.AddParameter("MaxReceiveSize", p.MaxReceiveSizeInKB + "KB");
                cmd.AddParameter("MaxSendSize", p.MaxSendSizeInKB + "KB");
                cmd.AddParameter("OfflineAddressBook", u.CompanyCode + " OAL");
                cmd.AddParameter("ProhibitSendQuota", p.SetSizeInMB + "MB");
                cmd.AddParameter("ProhibitSendReceiveQuota", p.SetSizeInMB + "MB");
                cmd.AddParameter("RecipientLimits", p.MaxRecipients);
                cmd.AddParameter("RetainDeletedItemsFor", p.KeepDeletedItemsInDays);
                cmd.AddParameter("UseDatabaseQuotaDefaults", false);
                cmd.AddParameter("UseDatabaseRetentionDefaults", false);
                cmd.AddParameter("RetainDeletedItemsUntilBackup", true);
                cmd.AddParameter("CustomAttribute1", u.CompanyCode);
                cmd.AddParameter("RoleAssignmentPolicy", "Alternate Assignment Policy");
                cmd.AddParameter("DomainController", domainController);

                // Set the CAS mailbox settings
                cmd.AddStatement();
                cmd.AddCommand("Set-CASMailbox");
                cmd.AddParameter("Identity", u.UserPrincipalName);
                cmd.AddParameter("ActiveSyncEnabled", p.ActiveSyncEnabled);
                cmd.AddParameter("ECPEnabled", p.ECPEnabled);
                cmd.AddParameter("ImapEnabled", p.IMAPEnabled);
                cmd.AddParameter("MAPIEnabled", p.MAPIEnabled);
                cmd.AddParameter("OWAEnabled", p.OWAEnabled);
                cmd.AddParameter("PopEnabled", p.POP3Enabled);
                cmd.AddParameter("ActiveSyncMailboxPolicy", string.IsNullOrEmpty(u.ActiveSyncMailboxPolicy) ? null : u.ActiveSyncMailboxPolicy);
                cmd.AddParameter("DomainController", domainController);

                // Execute powershell
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors but skip if we get the message "is of type UserMailbox" because that 
                // means that the mailbox already existed. This could be due to failure enabling the mailbox in the past.
                CheckErrors(ref powershell, false, true);

                // DEBUG //
                logger.Debug("Successfully enabled mailbox for [" + u.UserPrincipalName + "] for company code " + u.CompanyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // Do not throw error if it already exists
                if (!ex.ToString().Contains("already exists"))
                    throw;
                else
                    this.logger.Info("Powershell error was thrown because the mailbox already existed. Continuing without error.");
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Sets the mailbox default calendar permissions by removing the default and adding their company's ExchangeSecurity group to the AvailabilityOnly
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <param name="companyCode"></param>
        public void Set_MailboxCalendarPermission(MailboxUser user)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                user.CompanyCode = user.CompanyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Setting up the Calendar permissions for mailbox [" + user.UserPrincipalName + "] for company code " + user.CompanyCode);

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Enable the mailbox
                PSCommand cmd = new PSCommand();

                // Get the calendar name
                string calendarName = Get_CalendarName(user.UserPrincipalName);

                // Remove default calendar permissions
                cmd.AddCommand("Set-MailboxFolderPermission");
                cmd.AddParameter("Identity", string.Format(@"{0}:\{1}", user.UserPrincipalName, calendarName));
                cmd.AddParameter("User", "Default");
                cmd.AddParameter("AccessRights", "None");
                cmd.AddParameter("DomainController", this.domainController);

                // Add calendar permissions for the group
                cmd.AddStatement();
                cmd.AddCommand("Add-MailboxFolderPermission");
                cmd.AddParameter("Identity", string.Format(@"{0}:\{1}", user.UserPrincipalName, calendarName));
                cmd.AddParameter("User", "ExchangeSecurity@" +user.CompanyCode);
                cmd.AddParameter("AccessRights", "AvailabilityOnly");
                cmd.AddParameter("DomainController", this.domainController);

                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully finished mailbox permissions for [" + user.UserPrincipalName + "] for company code " + user.CompanyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");

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
        /// Sets mailbox information
        /// </summary>
        /// <param name="user"></param>
        /// <param name="plan"></param>
        public void Set_Mailbox(MailboxUser user, MailboxPlan plan)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Setting details on mailbox for user [" + user.UserPrincipalName + "]");

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Set the mailbox information
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Set-Mailbox");
                cmd.AddParameter("Identity", user.UserPrincipalName);
                cmd.AddParameter("EmailAddressPolicyEnabled", false);
                cmd.AddParameter("IssueWarningQuota", plan.WarningSizeInMB + "MB");
                cmd.AddParameter("MaxReceiveSize", plan.MaxReceiveSizeInKB + "KB");
                cmd.AddParameter("MaxSendSize", plan.MaxSendSizeInKB + "KB");
                cmd.AddParameter("ProhibitSendQuota", plan.SetSizeInMB + "MB");
                cmd.AddParameter("ProhibitSendReceiveQuota", plan.SetSizeInMB + "MB");
                cmd.AddParameter("RecipientLimits", plan.MaxRecipients);
                cmd.AddParameter("RetainDeletedItemsFor", plan.KeepDeletedItemsInDays);
                cmd.AddParameter("UseDatabaseQuotaDefaults", false);
                cmd.AddParameter("UseDatabaseRetentionDefaults", false);
                cmd.AddParameter("RetainDeletedItemsUntilBackup", true);
                cmd.AddParameter("RoleAssignmentPolicy", "Alternate Assignment Policy");

                // Compile the emails addresses starts with the primary and then the aliases
                List<string> compiledEmails = new List<string>();
                compiledEmails.Add("SMTP:" + user.PrimarySmtpAddress);

                if (user.EmailAliases != null && user.EmailAliases.Count > 0)
                {
                    foreach (string e in user.EmailAliases)
                    {
                        if (e.StartsWith("sip:", StringComparison.CurrentCultureIgnoreCase))
                            compiledEmails.Add(e);
                        else if (e.StartsWith("x500:", StringComparison.CurrentCultureIgnoreCase))
                            compiledEmails.Add(e);
                        else if (e.StartsWith("x400:", StringComparison.CurrentCultureIgnoreCase))
                            compiledEmails.Add(e);
                        else
                            compiledEmails.Add("smtp:" + e);
                    }
                }
                cmd.AddParameter("EmailAddresses", compiledEmails.ToArray());
               

                //
                // Check the settings for forwarding emails
                //
                if (!string.IsNullOrEmpty(user.ForwardingAddress))
                {
                    cmd.AddParameter("ForwardingAddress", user.ForwardingAddress);
                    cmd.AddParameter("DeliverToMailboxAndForward", user.DeliverToMailboxAndForward);
                }
                else
                {
                    cmd.AddParameter("ForwardingAddress", null);
                    cmd.AddParameter("DeliverToMailboxAndForward", false);
                }

                cmd.AddParameter("DomainController", domainController);


                //
                // Check the other mailbox features and update them
                //
                cmd.AddStatement();
                cmd.AddCommand("Set-CASMailbox");
                cmd.AddParameter("Identity", user.UserPrincipalName);
                cmd.AddParameter("ActiveSyncEnabled", user.ActiveSyncEnabled);
                cmd.AddParameter("ECPEnabled", user.ECPEnabled);
                cmd.AddParameter("ImapEnabled", user.IMAPEnabled);
                cmd.AddParameter("MAPIEnabled", user.MAPIEnabled);
                cmd.AddParameter("OWAEnabled", user.OWAEnabled);
                cmd.AddParameter("PopEnabled", user.POP3Enabled);
                cmd.AddParameter("ActiveSyncMailboxPolicy", user.ActiveSyncMailboxPolicy);
                cmd.AddParameter("DomainController", domainController);

                
                //
                // Check if any full access permissions were added or removed
                //
                if (user.FullAccessPermissions != null && user.FullAccessPermissions.Count > 0)
                {
                    foreach (MailboxPermissions m in user.FullAccessPermissions)
                    {
                        if (m.Add)
                        {
                            cmd.AddStatement();
                            cmd.AddCommand("Add-MailboxPermission");
                            cmd.AddParameter("Identity", user.UserPrincipalName);
                            cmd.AddParameter("User", m.SamAccountName);
                            cmd.AddParameter("AccessRights", "FullAccess");
                            cmd.AddParameter("DomainController", domainController);
                            cmd.AddParameter("Confirm", false);
                            cmd.AddParameter("ErrorAction", "Continue");
                        }
                        else
                        {
                            cmd.AddStatement();
                            cmd.AddCommand("Remove-MailboxPermission");
                            cmd.AddParameter("Identity", user.UserPrincipalName);
                            cmd.AddParameter("User", m.SamAccountName);
                            cmd.AddParameter("AccessRights", "FullAccess");
                            cmd.AddParameter("DomainController", domainController);
                            cmd.AddParameter("Confirm", false);
                            cmd.AddParameter("ErrorAction", "Continue");
                        }
                    }
                }

                //
                // Check if any send as permissions were added or removed
                //
                if (user.SendAsPermissions != null && user.SendAsPermissions.Count > 0)
                {
                    foreach (MailboxPermissions m in user.SendAsPermissions)
                    {
                        if (m.Add)
                        {
                            cmd.AddStatement();
                            cmd.AddCommand("Add-ADPermission");
                            cmd.AddParameter("Identity", user.DistinguishedName);
                            cmd.AddParameter("User", m.SamAccountName);
                            cmd.AddParameter("AccessRights", "ExtendedRight");
                            cmd.AddParameter("ExtendedRights", "Send As");
                            cmd.AddParameter("DomainController", domainController);
                            cmd.AddParameter("Confirm", false);
                            cmd.AddParameter("ErrorAction", "Continue");
                        }
                        else
                        {
                            cmd.AddStatement();
                            cmd.AddCommand("Remove-ADPermission");
                            cmd.AddParameter("Identity", user.DistinguishedName);
                            cmd.AddParameter("User", m.SamAccountName);
                            cmd.AddParameter("AccessRights", "ExtendedRight");
                            cmd.AddParameter("ExtendedRights", "Send As");
                            cmd.AddParameter("DomainController", domainController);
                            cmd.AddParameter("Confirm", false);
                            cmd.AddParameter("ErrorAction", "Continue");
                        }
                    }
                }


                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully set details on mailbox for [" + user.UserPrincipalName + "] in " + stopwatch.Elapsed.ToString() + " second(s)");
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

        /// <summary>
        /// Changes the mailbox settings. This is used for the import users feature
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <param name="companyCode"></param>
        /// <param name="plan"></param>
        public void Set_Mailbox(string userPrincipalName, string companyCode, MailboxPlan plan)
        {
            PowerShell powershell = null;

            try
            {
                // Remove spaces from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Setting details on mailbox for user [" + userPrincipalName + "]");

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Set the mailbox information
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Set-Mailbox");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("CustomAttribute1", companyCode);
                cmd.AddParameter("AddressBookPolicy", companyCode + " ABP");
                cmd.AddParameter("EmailAddressPolicyEnabled", false);
                cmd.AddParameter("IssueWarningQuota", plan.WarningSizeInMB + "MB");
                cmd.AddParameter("MaxReceiveSize", plan.MaxReceiveSizeInKB + "KB");
                cmd.AddParameter("MaxSendSize", plan.MaxSendSizeInKB + "KB");
                cmd.AddParameter("ProhibitSendQuota", plan.SetSizeInMB + "MB");
                cmd.AddParameter("ProhibitSendReceiveQuota", plan.SetSizeInMB + "MB");
                cmd.AddParameter("RecipientLimits", plan.MaxRecipients);
                cmd.AddParameter("RetainDeletedItemsFor", plan.KeepDeletedItemsInDays);
                cmd.AddParameter("UseDatabaseQuotaDefaults", false);
                cmd.AddParameter("UseDatabaseRetentionDefaults", false);
                cmd.AddParameter("RetainDeletedItemsUntilBackup", true);
                cmd.AddParameter("RoleAssignmentPolicy", "Alternate Assignment Policy");
                cmd.AddParameter("DomainController", domainController);
                cmd.AddStatement();
                cmd.AddCommand("Set-CASMailbox");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("ActiveSyncEnabled", plan.ActiveSyncEnabled);
                cmd.AddParameter("ECPEnabled", plan.ECPEnabled);
                cmd.AddParameter("ImapEnabled", plan.IMAPEnabled);
                cmd.AddParameter("MAPIEnabled", plan.MAPIEnabled);
                cmd.AddParameter("OWAEnabled", plan.OWAEnabled);
                cmd.AddParameter("PopEnabled", plan.POP3Enabled);
                cmd.AddParameter("DomainController", domainController);

                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully set details on mailbox for [" + userPrincipalName + "] in " + stopwatch.Elapsed.ToString() + " second(s)");
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

        /// <summary>
        /// Gets mailbox information for a user
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public MailboxUser Get_Mailbox(string userPrincipalName)
        {
            // Our return object
            MailboxUser mbx = new MailboxUser();

            PowerShell powershell = null;
            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Getting mailbox for [" + userPrincipalName + "]");

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                //
                // Get the Mailbox
                //
                this.logger.Debug("Retrieving mailbox information for " + userPrincipalName);

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Get-Mailbox");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;

                // Invoke and get returned information
                mbx.EmailAliases = new List<string>();
                foreach (PSObject ps in powershell.Invoke())
                {
                    mbx.DistinguishedName = ps.Members["DistinguishedName"].Value.ToString();
                    mbx.Database = ps.Members["Database"].Value.ToString();
                    mbx.DeliverToMailboxAndForward = bool.Parse(ps.Members["DeliverToMailboxAndForward"].Value.ToString());
                    mbx.RetainDeletedItemsInDays = TimeSpan.Parse(ps.Members["RetainDeletedItemsFor"].Value.ToString()).Days;
                    mbx.SamAccountName = ps.Members["SamAccountName"].Value.ToString();
                    mbx.UserPrincipalName = ps.Members["UserPrincipalName"].Value.ToString();
                    mbx.Alias = ps.Members["Alias"].Value.ToString();
                    mbx.CompanyCode = ps.Members["CustomAttribute1"].Value.ToString();
                    mbx.DisplayName = ps.Members["DisplayName"].Value.ToString();
                    mbx.PrimarySmtpAddress = ps.Members["PrimarySmtpAddress"].Value.ToString();
                    mbx.IsHidden = bool.Parse(ps.Members["HiddenFromAddressListsEnabled"].Value.ToString());

                    if (ps.Members["RecipientLimits"].Value.ToString().Equals("Unlimited", StringComparison.CurrentCultureIgnoreCase))
                        mbx.RecipientLimit = 0;
                    else
                    {
                        int recipientLimit = 100;
                        int.TryParse(ps.Members["RecipientLimits"].Value.ToString(), out recipientLimit);

                        mbx.RecipientLimit = recipientLimit;
                    }

                    // Get the quota on the mailbox
                    mbx.MailboxSizeInMB = int.Parse(FormatExchangeSize(ps.Members["ProhibitSendReceiveQuota"].Value.ToString()));

                    // Get "EmailAddresses" which includes the alises
                    PSObject multiValue = (PSObject)ps.Members["EmailAddresses"].Value;
                    ArrayList emailAddresses = (ArrayList)multiValue.BaseObject;

                    for (int i = 0; i < emailAddresses.Count; i++)
                    {
                        if (!emailAddresses[i].ToString().StartsWith("SMTP:"))
                            mbx.EmailAliases.Add(emailAddresses[i].ToString().Replace("smtp:", string.Empty));
                    }

                    if (ps.Members["ForwardingAddress"].Value != null)
                        mbx.ForwardingAddress = ps.Members["ForwardingAddress"].Value.ToString();
                }

                //
                // Get the CAS Mailbox
                //
                this.logger.Debug("Retrieving CAS mailbox information for " + userPrincipalName);

                cmd = new PSCommand();
                cmd.AddCommand("Get-CASMailbox");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;

                foreach (PSObject ps in powershell.Invoke())
                {
                    if (ps.Members["ActiveSyncMailboxPolicy"].Value != null)
                        mbx.ActiveSyncMailboxPolicy = ps.Members["ActiveSyncMailboxPolicy"].Value.ToString();

                    if (ps.Members["OwaMailboxPolicy"].Value != null)
                        mbx.OWAMailboxPolicy = ps.Members["OwaMailboxPolicy"].Value.ToString();
                }

                //
                // Get Mailbox Permissions for FullAccess
                //
                this.logger.Debug("Retrieving mailbox full access permissions for " + userPrincipalName);

                cmd = new PSCommand();
                cmd.AddCommand("Get-MailboxPermission");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;

                mbx.FullAccessPermissions = new List<MailboxPermissions>();
                foreach (PSObject ps in powershell.Invoke())
                {
                    if (ps.Members["AccessRights"].Value != null)
                    {
                        MailboxPermissions tmp = new MailboxPermissions();
                        tmp.InheritanceType = ps.Members["InheritanceType"].Value.ToString();
                        tmp.Deny = bool.Parse(ps.Members["Deny"].Value.ToString());
                        tmp.IsInherited = bool.Parse(ps.Members["IsInherited"].Value.ToString());

                        // Get the SamAccountName
                        if (ps.Members["User"].Value.ToString().Contains("\\"))
                        {
                            // Need to make sure the value contains a black slash because if not then it is not a valid sAMAccountName (Could be like S-1-5-32-554)
                            tmp.SamAccountName = ps.Members["User"].Value.ToString().Split('\\')[1];

                            // Get the permissions that this user has
                            PSObject multiValue = (PSObject)ps.Members["AccessRights"].Value;
                            ArrayList accessRights = (ArrayList)multiValue.BaseObject;
                            tmp.AccessRights = (string[])accessRights.ToArray(typeof(string));

                            // Only add if it contains "FullAccess"
                            if (tmp.AccessRights.Contains("FullAccess"))
                                mbx.FullAccessPermissions.Add(tmp);
                        }
                    }
                }

                //
                // Get Mailbox Permissions for SendAs
                //
                this.logger.Debug("Retrieving mailbox send-as permissions for " + userPrincipalName);

                cmd = new PSCommand();
                cmd.AddCommand("Get-ADPermission");
                cmd.AddParameter("Identity", mbx.DistinguishedName);
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;
                
                mbx.SendAsPermissions = new List<MailboxPermissions>();
                foreach (PSObject ps in powershell.Invoke())
                {
                    if (ps.Members["ExtendedRights"].Value != null)
                    {
                        MailboxPermissions tmp = new MailboxPermissions();
                        tmp.InheritanceType = ps.Members["InheritanceType"].Value.ToString();
                        tmp.Deny = bool.Parse(ps.Members["Deny"].Value.ToString());
                        tmp.IsInherited = bool.Parse(ps.Members["IsInherited"].Value.ToString());

                        // Get the SamAccountName
                        if (ps.Members["User"].Value.ToString().Contains("\\"))
                        {
                            // Need to make sure the value contains a black slash because if not then it is not a valid sAMAccountName (Could be like S-1-5-32-554)
                            tmp.SamAccountName = ps.Members["User"].Value.ToString().Split('\\')[1];

                            // Get the permissions that this user has
                            PSObject multiValue = (PSObject)ps.Members["ExtendedRights"].Value;
                            ArrayList accessRights = (ArrayList)multiValue.BaseObject;
                            tmp.AccessRights = (string[])accessRights.ToArray(typeof(string));

                            if (tmp.AccessRights.Contains("Send-As"))
                                mbx.SendAsPermissions.Add(tmp);
                        }
                    }
                }

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully retrieved mailbox information for [" + userPrincipalName + "] in " + stopwatch.Elapsed.ToString() + " second(s)");

                // Return our object
                return mbx;
            }
            catch (Exception ex)
            {
                // DEBUG //
                this.logger.Error("Error getting mailbox information for " + userPrincipalName + ": " + ex.ToString());

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Sets information for a resource mailbox
        /// </summary>
        /// <param name="mbx"></param>
        /// <param name="plan"></param>
        public void Set_ResourceMailbox(ResourceMailbox mbx, MailboxPlan plan)
        {
            PowerShell powershell = null;
            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Setting resource mailbox for [" + mbx.UserPrincipalName + "]");

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Get the Mailbox
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Set-Mailbox");
                cmd.AddParameter("Identity", mbx.UserPrincipalName);
                cmd.AddParameter("DisplayName", mbx.DisplayName);
                cmd.AddParameter("PrimarySmtpAddress", mbx.PrimarySmtpAddress);
                cmd.AddParameter("HiddenFromAddressListsEnabled", mbx.IsHidden);
                cmd.AddParameter("IssueWarningQuota", plan.WarningSizeInMB + "MB");
                cmd.AddParameter("MaxReceiveSize", plan.MaxReceiveSizeInKB + "KB");
                cmd.AddParameter("MaxSendSize", plan.MaxSendSizeInKB + "KB");
                cmd.AddParameter("ProhibitSendQuota", plan.SetSizeInMB + "MB");
                cmd.AddParameter("ProhibitSendReceiveQuota", plan.SetSizeInMB + "MB");
                cmd.AddParameter("RecipientLimits", plan.MaxRecipients);
                cmd.AddParameter("RetainDeletedItemsFor", plan.KeepDeletedItemsInDays);

                if (!mbx.ResourceType.Equals("Shared"))
                {
                    if (mbx.ResourceCapacity == 0)
                        cmd.AddParameter("ResourceCapacity", null);
                    else
                        cmd.AddParameter("ResourceCapacity", mbx.ResourceCapacity);
                }

                cmd.AddParameter("DomainController", this.domainController);

                cmd.AddStatement();
                cmd.AddCommand("Set-CASMailbox");
                cmd.AddParameter("Identity", mbx.UserPrincipalName);
                cmd.AddParameter("ECPEnabled", plan.ECPEnabled);
                cmd.AddParameter("IMAPEnabled", plan.IMAPEnabled);
                cmd.AddParameter("MAPIEnabled", plan.MAPIEnabled);
                cmd.AddParameter("OWAEnabled", plan.OWAEnabled);
                cmd.AddParameter("POPEnabled", plan.POP3Enabled);

                if (!mbx.ResourceType.Equals("Shared"))
                {
                    cmd.AddStatement();
                    cmd.AddCommand("Set-CalendarProcessing");
                    cmd.AddParameter("Identity", mbx.UserPrincipalName);
                    cmd.AddParameter("AutomateProcessing", mbx.BookingAttendantEnabled ? "AutoAccept" : "None");
                    cmd.AddParameter("AllowConflicts", mbx.AllowConflicts);
                    cmd.AddParameter("BookingWindowInDays", mbx.BookingWindowInDays);
                    cmd.AddParameter("MaximumDurationInMinutes", mbx.MaximumDurationInMinutes);
                    cmd.AddParameter("AllowRecurringMeetings", mbx.AllowRepeatingMeetings);
                    cmd.AddParameter("ScheduleOnlyDuringWorkHours", mbx.SchedulingOnlyDuringWorkHours);
                    cmd.AddParameter("ConflictPercentageAllowed", mbx.ConflictPercentageAllowed);
                    cmd.AddParameter("MaximumConflictInstances", mbx.MaximumConflictInstances);
                    cmd.AddParameter("ForwardRequestsToDelegates", mbx.ForwardRequestsToDelegates);
                    cmd.AddParameter("DeleteAttachments", mbx.DeleteAttachments);
                    cmd.AddParameter("DeleteComments", mbx.DeleteComments);
                    cmd.AddParameter("DeleteSubject", mbx.DeleteSubject);
                    cmd.AddParameter("EnforceSchedulingHorizon", mbx.RejectOutsideBookingWindow);

                    if (mbx.ResourceDelegates != null && mbx.ResourceDelegates.Length > 0)
                        cmd.AddParameter("ResourceDelegates", mbx.ResourceDelegates);
                    else
                        cmd.AddParameter("ResourceDelegates", null);
                }

                //
                // Check if any full access permissions were added or removed
                //
                if (mbx.FullAccessPermissions != null && mbx.FullAccessPermissions.Count > 0)
                {
                    foreach (MailboxPermissions m in mbx.FullAccessPermissions)
                    {
                        if (m.Add)
                        {
                            cmd.AddStatement();
                            cmd.AddCommand("Add-MailboxPermission");
                            cmd.AddParameter("Identity", mbx.UserPrincipalName);
                            cmd.AddParameter("User", m.SamAccountName);
                            cmd.AddParameter("AccessRights", "FullAccess");
                            cmd.AddParameter("DomainController", domainController);
                            cmd.AddParameter("Confirm", false);
                            cmd.AddParameter("ErrorAction", "Continue");
                        }
                        else
                        {
                            cmd.AddStatement();
                            cmd.AddCommand("Remove-MailboxPermission");
                            cmd.AddParameter("Identity", mbx.UserPrincipalName);
                            cmd.AddParameter("User", m.SamAccountName);
                            cmd.AddParameter("AccessRights", "FullAccess");
                            cmd.AddParameter("DomainController", domainController);
                            cmd.AddParameter("Confirm", false);
                            cmd.AddParameter("ErrorAction", "Continue");
                        }
                    }
                }

                //
                // Check if any send as permissions were added or removed
                //
                if (mbx.SendAsPermissions != null && mbx.SendAsPermissions.Count > 0)
                {
                    foreach (MailboxPermissions m in mbx.SendAsPermissions)
                    {
                        if (m.Add)
                        {
                            cmd.AddStatement();
                            cmd.AddCommand("Add-ADPermission");
                            cmd.AddParameter("Identity", mbx.DistinguishedName);
                            cmd.AddParameter("User", m.SamAccountName);
                            cmd.AddParameter("AccessRights", "ExtendedRight");
                            cmd.AddParameter("ExtendedRights", "Send As");
                            cmd.AddParameter("DomainController", domainController);
                            cmd.AddParameter("Confirm", false);
                            cmd.AddParameter("ErrorAction", "Continue");
                        }
                        else
                        {
                            cmd.AddStatement();
                            cmd.AddCommand("Remove-ADPermission");
                            cmd.AddParameter("Identity", mbx.DistinguishedName);
                            cmd.AddParameter("User", m.SamAccountName);
                            cmd.AddParameter("AccessRights", "ExtendedRight");
                            cmd.AddParameter("ExtendedRights", "Send As");
                            cmd.AddParameter("DomainController", domainController);
                            cmd.AddParameter("Confirm", false);
                            cmd.AddParameter("ErrorAction", "Continue");
                        }
                    }
                }

                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully set mailbox information for [" + mbx.UserPrincipalName + "] in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // DEBUG //
                this.logger.Error("Error setting mailbox information for " + mbx.UserPrincipalName + ": " + ex.ToString());

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Gets information about a resource mailbox
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public ResourceMailbox Get_ResourceMailbox(string userPrincipalName)
        {
            // Our return object
            ResourceMailbox mbx = new ResourceMailbox();

            PowerShell powershell = null;
            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Getting resource mailbox for [" + userPrincipalName + "]");

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Get the Mailbox
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Get-Mailbox");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;

                // Invoke and get mailbox information
                foreach (PSObject ps in powershell.Invoke())
                {
                    mbx.UserPrincipalName = userPrincipalName;
                    mbx.CompanyCode = ps.Members["CustomAttribute1"].Value.ToString();
                    mbx.DisplayName = ps.Members["DisplayName"].Value.ToString();
                    mbx.DistinguishedName = ps.Members["DistinguishedName"].Value.ToString();
                    mbx.PrimarySmtpAddress = ps.Members["PrimarySmtpAddress"].Value.ToString();
                    mbx.KeepDeletedItemsFor = TimeSpan.Parse(ps.Members["RetainDeletedItemsFor"].Value.ToString());
                    mbx.IsHidden = bool.Parse(ps.Members["HiddenFromAddressListsEnabled"].Value.ToString());
                    mbx.IsResource = bool.Parse(ps.Members["IsResource"].Value.ToString());

                    if (ps.Members["ResourceType"].Value != null)
                        mbx.ResourceType = ps.Members["ResourceType"].Value.ToString();

                    if (ps.Members["RecipientTypeDetails"].Value != null)
                        mbx.RecipientTypeDetails = ps.Members["RecipientTypeDetails"].Value.ToString();

                    if (ps.Members["ResourceCapacity"].Value != null)
                        mbx.ResourceCapacity = int.Parse(ps.Members["ResourceCapacity"].Value.ToString());
                }

                // Log
                this.logger.Debug("Successfully received the first section of the resource mailbox.");

                // Get the calendar processing settings
                cmd = new PSCommand();
                cmd.AddCommand("Get-CalendarProcessing");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;

                foreach (PSObject ps in powershell.Invoke())
                {
                    mbx.AutomateProcessing = ps.Members["AutomateProcessing"].Value.ToString();
                    mbx.AllowConflicts = bool.Parse(ps.Members["AllowConflicts"].Value.ToString());
                    mbx.BookingWindowInDays = int.Parse(ps.Members["BookingWindowInDays"].Value.ToString());
                    mbx.MaximumDurationInMinutes = int.Parse(ps.Members["MaximumDurationInMinutes"].Value.ToString());
                    mbx.AllowRepeatingMeetings = bool.Parse(ps.Members["AllowRecurringMeetings"].Value.ToString());
                    mbx.SchedulingOnlyDuringWorkHours = bool.Parse(ps.Members["ScheduleOnlyDuringWorkHours"].Value.ToString());
                    mbx.ConflictPercentageAllowed = int.Parse(ps.Members["ConflictPercentageAllowed"].Value.ToString());
                    mbx.MaximumConflictInstances = int.Parse(ps.Members["MaximumConflictInstances"].Value.ToString());
                    mbx.ForwardRequestsToDelegates = bool.Parse(ps.Members["ForwardRequestsToDelegates"].Value.ToString());
                    mbx.DeleteAttachments = bool.Parse(ps.Members["DeleteAttachments"].Value.ToString());
                    mbx.DeleteComments = bool.Parse(ps.Members["DeleteComments"].Value.ToString());
                    mbx.RemovePrivateFlag = bool.Parse(ps.Members["RemovePrivateProperty"].Value.ToString());
                    mbx.DeleteSubject = bool.Parse(ps.Members["DeleteSubject"].Value.ToString());
                    mbx.AddOrganizerNameToSubject = bool.Parse(ps.Members["AddOrganizerToSubject"].Value.ToString());
                    mbx.DeleteNonCalendarItems = bool.Parse(ps.Members["DeleteNonCalendarItems"].Value.ToString());
                    mbx.AddAdditionalResponse = bool.Parse(ps.Members["AddAdditionalResponse"].Value.ToString());
                    mbx.MarkPendingRequestsAsTentative = bool.Parse(ps.Members["TentativePendingApproval"].Value.ToString());
                    mbx.RejectOutsideBookingWindow = bool.Parse(ps.Members["EnforceSchedulingHorizon"].Value.ToString());

                    if (ps.Members["AdditionalResponse"].Value != null)
                        mbx.AdditionalText = ps.Members["AdditionalResponse"].Value.ToString();

                    if (ps.Members["ResourceDelegates"].Value != null)
                    {
                        PSObject multiValue = (PSObject)ps.Members["ResourceDelegates"].Value;
                        ArrayList resourceDelegates = (ArrayList)multiValue.BaseObject;

                        mbx.ResourceDelegates = (string[])resourceDelegates.ToArray(typeof(string));
                    }
                }


                //
                // Get Mailbox Permissions for FullAccess
                //
                this.logger.Debug("Retrieving mailbox full access permissions for " + userPrincipalName);

                cmd = new PSCommand();
                cmd.AddCommand("Get-MailboxPermission");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;

                mbx.FullAccessPermissions = new List<MailboxPermissions>();
                foreach (PSObject ps in powershell.Invoke())
                {
                    if (ps.Members["AccessRights"].Value != null)
                    {
                        MailboxPermissions tmp = new MailboxPermissions();
                        tmp.InheritanceType = ps.Members["InheritanceType"].Value.ToString();
                        tmp.Deny = bool.Parse(ps.Members["Deny"].Value.ToString());
                        tmp.IsInherited = bool.Parse(ps.Members["IsInherited"].Value.ToString());

                        // Get the SamAccountName
                        if (ps.Members["User"].Value.ToString().Contains("\\"))
                        {
                            // Need to make sure the value contains a black slash because if not then it is not a valid sAMAccountName (Could be like S-1-5-32-554)
                            tmp.SamAccountName = ps.Members["User"].Value.ToString().Split('\\')[1];

                            // Get the permissions that this user has
                            PSObject multiValue = (PSObject)ps.Members["AccessRights"].Value;
                            ArrayList accessRights = (ArrayList)multiValue.BaseObject;
                            tmp.AccessRights = (string[])accessRights.ToArray(typeof(string));

                            // Only add if it contains "FullAccess"
                            if (tmp.AccessRights.Contains("FullAccess"))
                                mbx.FullAccessPermissions.Add(tmp);
                        }
                    }
                }

                //
                // Get Mailbox Permissions for SendAs
                //
                this.logger.Debug("Retrieving mailbox send-as permissions for " + userPrincipalName);

                cmd = new PSCommand();
                cmd.AddCommand("Get-ADPermission");
                cmd.AddParameter("Identity", mbx.DistinguishedName);
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;

                mbx.SendAsPermissions = new List<MailboxPermissions>();
                foreach (PSObject ps in powershell.Invoke())
                {
                    if (ps.Members["ExtendedRights"].Value != null)
                    {
                        MailboxPermissions tmp = new MailboxPermissions();
                        tmp.InheritanceType = ps.Members["InheritanceType"].Value.ToString();
                        tmp.Deny = bool.Parse(ps.Members["Deny"].Value.ToString());
                        tmp.IsInherited = bool.Parse(ps.Members["IsInherited"].Value.ToString());

                        // Get the SamAccountName
                        if (ps.Members["User"].Value.ToString().Contains("\\"))
                        {
                            // Need to make sure the value contains a black slash because if not then it is not a valid sAMAccountName (Could be like S-1-5-32-554)
                            tmp.SamAccountName = ps.Members["User"].Value.ToString().Split('\\')[1];

                            // Get the permissions that this user has
                            PSObject multiValue = (PSObject)ps.Members["ExtendedRights"].Value;
                            ArrayList accessRights = (ArrayList)multiValue.BaseObject;
                            tmp.AccessRights = (string[])accessRights.ToArray(typeof(string));

                            if (tmp.AccessRights.Contains("Send-As"))
                                mbx.SendAsPermissions.Add(tmp);
                        }
                    }
                }

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully retrieved mailbox information for [" + userPrincipalName + "] in " + stopwatch.Elapsed.ToString() + " second(s)");

                // Return mailbox data
                return mbx;
            }
            catch (Exception ex)
            {
                // DEBUG //
                this.logger.Error("Error getting mailbox information for " + userPrincipalName + ": " + ex.ToString());

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }


        /// <summary>
        /// Creates a new distribution group
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="companyCode"></param>
        /// <param name="emailAddress"></param>
        /// <param name="companyDN"></param>
        /// <param name="memberJoinRestriction"></param>
        /// <param name="memberDepartRestriction"></param>
        /// <param name="moderationEnabled"></param>
        /// <param name="sendModerationNotifications"></param>
        /// <param name="members"></param>
        /// <param name="managedBy"></param>
        /// <param name="moderatedBy"></param>
        /// <param name="allowedSenders"></param>
        /// <param name="bypassModerationSenders"></param>
        /// <param name="isHidden"></param>
        /// <param name="requireSenderAuthentication"></param>
        public void New_DistributionGroup(ExchangeGroup group, string companyExchangeDN)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                group.CompanyCode = group.CompanyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Creating a new distribution group named [" + group.DisplayName + "] for company code " + group.CompanyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Create the distribution group
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("New-DistributionGroup");
                cmd.AddParameter("Name", group.PrimarySmtpAddress);
                cmd.AddParameter("Alias", group.PrimarySmtpAddress.Replace("@", "_"));
                cmd.AddParameter("SamAccountName", group.SamAccountName);
                cmd.AddParameter("DisplayName", group.DisplayName);
                cmd.AddParameter("PrimarySmtpAddress", group.PrimarySmtpAddress);
                cmd.AddParameter("OrganizationalUnit", companyExchangeDN);
                cmd.AddParameter("MemberJoinRestriction", group.JoinRestriction);
                cmd.AddParameter("MemberDepartRestriction", group.DepartRestriction);
                cmd.AddParameter("ModerationEnabled", group.ModerationEnabled);
                cmd.AddParameter("SendModerationNotifications", group.SendModerationNotifications);
                cmd.AddParameter("Members", group.MembersArray);
                cmd.AddParameter("ManagedBy", group.ManagedBy);
                cmd.AddParameter("DomainController", this.domainController);

                // Only add ModeratedBy if it was checked
                if (group.ModerationEnabled)
                    cmd.AddParameter("ModeratedBy", group.GroupModerators);
                else
                    cmd.AddParameter("ModeratedBy", null);

                
                cmd.AddCommand("Set-DistributionGroup");
                cmd.AddParameter("BypassSecurityGroupManagerCheck");
                cmd.AddParameter("CustomAttribute1", group.CompanyCode);
                cmd.AddParameter("HiddenFromAddressListsEnabled", group.Hidden);
                cmd.AddParameter("RequireSenderAuthenticationEnabled", group.RequireSenderAuthentication);
                cmd.AddParameter("DomainController", this.domainController);

                // Check to see if allowed senders is set or not
                if (group.WhoCanSendToGroup != null && group.WhoCanSendToGroup.Length > 0)
                {
                    cmd.AddParameter("AcceptMessagesOnlyFromSendersOrMembers", group.WhoCanSendToGroup);
                }
                else if (group.RequireSenderAuthentication)
                {
                    // Didn't specify allowed senders and allowing inside only
                    cmd.AddParameter("AcceptMessagesOnlyFromSendersOrMembers", "ExchangeSecurity@" + group.CompanyCode);
                }
                else
                    cmd.AddParameter("AcceptMessagesOnlyFromSendersOrMembers", null);

                // Check if it must be approved by an administrator
                if (group.ModerationEnabled)
                {
                    if (group.SendersNotRequiringApproval != null && group.SendersNotRequiringApproval.Length > 0)
                        cmd.AddParameter("BypassModerationFromSendersOrMembers", group.SendersNotRequiringApproval);
                    else
                        cmd.AddParameter("BypassModerationFromSendersOrMembers", null);
                }
                else
                    cmd.AddParameter("BypassModerationFromSendersOrMembers", null);

                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

            }
            catch (Exception ex)
            {
                // FATAL //
                logger.Debug("Error creating a new distribution group named [" + group.DisplayName + "] for company code " + group.CompanyCode + ". Path: " + companyExchangeDN, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }


        /// <summary>
        /// Creates a new security distribution group. This is for assigning rights to users on their
        /// calendar and such
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="companyCode"></param>
        /// <param name="companyExchDN"></param>
        public void New_SecurityDistributionGroup(string displayName, string companyCode, string companyExchDN)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Modify the display name to include the ending company code
                displayName = displayName + "@" + companyCode;

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Creating a new security distribution group named [" + displayName + "] for company code " + companyCode);

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Create the distribution group
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("New-DistributionGroup");
                cmd.AddParameter("Name", displayName);
                cmd.AddParameter("DisplayName", displayName);
                cmd.AddParameter("PrimarySmtpAddress", displayName);
                cmd.AddParameter("OrganizationalUnit", companyExchDN);
                cmd.AddParameter("Members", "AllUsers@" + companyCode);
                cmd.AddParameter("Type", "Security");
                cmd.AddParameter("DomainController", this.domainController);
                cmd.AddCommand("Set-DistributionGroup");
                cmd.AddParameter("BypassSecurityGroupManagerCheck");
                cmd.AddParameter("CustomAttribute1", companyCode);
                cmd.AddParameter("HiddenFromAddressListsEnabled", true);
                cmd.AddParameter("DomainController", this.domainController);

                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

            }
            catch (Exception ex)
            {
                // FATAL //
                logger.Debug("Error creating a new security distribution group named [" + displayName + "] for company code " + companyCode + ". Path: " + companyExchDN, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Repairs Exchange permissions since they were missing off the last CloudPanel versions
        /// </summary>
        /// <param name="mailboxUsers"></param>
        /// <param name="distributionGroups"></param>
        public void Repair_ExchangePermissions(string companyCode)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Repairing Exchange permissions for company code " + companyCode);

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                //
                // First we need to remove the default calendar permissions and add the security group
                //
                #region Mailbox Fix
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Get-Mailbox");
                cmd.AddParameter("Filter", ScriptBlock.Create(string.Format("CustomAttribute1 -eq \"{0}\"", companyCode)));
                cmd.AddParameter("ResultSize", "Unlimited");
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;

                // Get the list of mailbox users
                List<MailboxUser> mailboxUsers = new List<MailboxUser>();
                
                Collection<PSObject> obj = powershell.Invoke();
                if (obj != null && obj.Count > 0)
                {
                    foreach (PSObject ps in obj)
                    {
                        mailboxUsers.Add(new MailboxUser()
                        {
                            UserPrincipalName = ps.Members["UserPrincipalName"].Value.ToString()
                        });

                        // DEBUG //
                        this.logger.Debug("Found user " + ps.Members["UserPrincipalName"].Value.ToString() + " that we will repair mailbox permissions on.");
                    }

                    // Dispose of commands
                    cmd = null;

                    // Now lets remove the default permissions
                    cmd = new PSCommand();
                    foreach (MailboxUser u in mailboxUsers)
                    {
                        // Get the calendar name for the user
                        string calendarName = Get_CalendarName(u.UserPrincipalName);

                        cmd.AddCommand("Set-MailboxFolderPermission");
                        cmd.AddParameter("Identity", string.Format(@"{0}:\{1}", u.UserPrincipalName, calendarName));
                        cmd.AddParameter("User", "Default");
                        cmd.AddParameter("AccessRights", "None");
                        cmd.AddParameter("DomainController", domainController);
                        cmd.AddParameter("Confirm", false);
                        cmd.AddStatement();

                        // Now lets add the correct permissions
                        cmd.AddCommand("Add-MailboxFolderPermission");
                        cmd.AddParameter("Identity", string.Format(@"{0}:\{1}", u.UserPrincipalName, calendarName));
                        cmd.AddParameter("User", "ExchangeSecurity@" + companyCode);
                        cmd.AddParameter("AccessRights", "AvailabilityOnly");
                        cmd.AddParameter("DomainController", domainController);
                        cmd.AddStatement();
                    }

                    // Only invoke if we have commands listed
                    if (cmd.Commands.Count > 0)
                    {
                        powershell.Commands = cmd;
                        powershell.Invoke();
                    }
                }
                #endregion

                //
                // Next we need to set the exchange security group for all distribution groups that are set to require sender authentication
                //
                #region Distribution Group Fix

                string filter = string.Format("(RequireAllSendersAreAuthenticated -eq 'True') -and (CustomAttribute1 -eq '{0}')", companyCode);

                cmd = new PSCommand();
                cmd.AddCommand("Get-DistributionGroup");
                cmd.AddParameter("Filter", ScriptBlock.Create(filter));
                cmd.AddParameter("ResultSize", "Unlimited");
                cmd.AddParameter("DomainController", this.domainController);
                cmd.AddCommand("Set-DistributionGroup");
                cmd.AddParameter("BypassSecurityGroupManagerCheck");
                cmd.AddParameter("AcceptMessagesOnlyFromSendersOrMembers", "ExchangeSecurity@" + companyCode);
                cmd.AddParameter("DomainController", this.domainController);
                cmd.AddParameter("Confirm", false);
                powershell.Commands = cmd;
                powershell.Invoke();
                
                #endregion

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Finished repairing Exchange permissions for company code " + companyCode);

            }
            catch (Exception ex)
            {
                // FATAL //
                logger.Debug("Error repairing Exchange permissions for company code " + companyCode, ex);

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
        /// Updates an existing distribution group
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="companyCode"></param>
        /// <param name="oldEmailAddress"></param>
        /// <param name="emailAddress"></param>
        /// <param name="companyDN"></param>
        /// <param name="memberJoinRestriction"></param>
        /// <param name="memberDepartRestriction"></param>
        /// <param name="moderationEnabled"></param>
        /// <param name="sendModerationNotifications"></param>
        /// <param name="moderatedBy"></param>
        /// <param name="allowedSenders"></param>
        /// <param name="bypassModerationSenders"></param>
        /// <param name="isHidden"></param>
        /// <param name="requireSenderAuthentication"></param>
        public void Set_DistributionGroup(string displayName, string companyCode, string oldEmailAddress, string emailAddress, string memberJoinRestriction, string memberDepartRestriction,
                                          bool moderationEnabled, string sendModerationNotifications, List<string> managedBy, List<string> moderatedBy, List<string> allowedSenders,
                                          List<string> bypassModerationSenders, bool isHidden, bool requireSenderAuthentication)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Updating existing distribution group. New name: [" + displayName + "] for company code " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Update the distribution group
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Set-DistributionGroup");
                cmd.AddParameter("Identity", oldEmailAddress);
                cmd.AddParameter("Name", emailAddress);
                cmd.AddParameter("DisplayName", displayName);
                cmd.AddParameter("PrimarySmtpAddress", emailAddress);
                cmd.AddParameter("MemberJoinRestriction", memberJoinRestriction);
                cmd.AddParameter("MemberDepartRestriction", memberDepartRestriction);
                cmd.AddParameter("ModerationEnabled", moderationEnabled);
                cmd.AddParameter("SendModerationNotifications", sendModerationNotifications);
                cmd.AddParameter("HiddenFromAddressListsEnabled", isHidden);
                cmd.AddParameter("RequireSenderAuthenticationEnabled", requireSenderAuthentication);
                cmd.AddParameter("ManagedBy", managedBy.ToArray());
                cmd.AddParameter("DomainController", this.domainController);
                cmd.AddParameter("BypassSecurityGroupManagerCheck");

                // Check to see if allowed senders is set or not
                if (allowedSenders != null && allowedSenders.Count > 0)
                    cmd.AddParameter("AcceptMessagesOnlyFromSendersOrMembers", allowedSenders.ToArray());
                else if (requireSenderAuthentication)
                    cmd.AddParameter("AcceptMessagesOnlyFromSendersOrMembers", "ExchangeSecurity@" + companyCode);
                else
                    cmd.AddParameter("AcceptMessagesOnlyFromSendersOrMembers", null);

                // Check if it must be approved by an administrator
                if (moderationEnabled)
                {
                    if (bypassModerationSenders != null && bypassModerationSenders.Count > 0)
                        cmd.AddParameter("BypassModerationFromSendersOrMembers", bypassModerationSenders.ToArray());
                    else
                        cmd.AddParameter("BypassModerationFromSendersOrMembers", null);
                }
                else
                    cmd.AddParameter("BypassModerationFromSendersOrMembers", null);

                // Only add ModeratedBy if it was checked
                if (moderationEnabled)
                    cmd.AddParameter("ModeratedBy", moderatedBy.ToArray());
                else
                    cmd.AddParameter("ModeratedBy", null);

                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
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

        /// <summary>
        /// Creates a new activsync policy in Exchange
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="companyCode"></param>
        /// <param name="exchangeVersion"></param>
        public void New_ActiveSyncPolicy(BaseActivesyncPolicy policy, Enumerations.ProductVersion version)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Adding new Activesync mailbox policy " + policy.DisplayName);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Update the distribution group
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("New-ActiveSyncMailboxPolicy");
                cmd.AddParameter("Name", policy.DisplayName); // Append company code to beginning
                cmd.AddParameter("AllowBluetooth", policy.AllowBluetooth);
                cmd.AddParameter("AllowBrowser", policy.AllowBrowser);
                cmd.AddParameter("AllowCamera", policy.AllowCamera);
                cmd.AddParameter("AllowConsumerEmail", policy.AllowConsumerEmail);
                cmd.AddParameter("AllowDesktopSync", policy.AllowDesktopSync);
                cmd.AddParameter("AllowHTMLEmail", policy.AllowHTMLEmail);
                cmd.AddParameter("AllowInternetSharing", policy.AllowInternetSharing);
                cmd.AddParameter("AllowIrDA", policy.AllowIrDA);
                cmd.AddParameter("AllowNonProvisionableDevices", policy.AllowNonProvisionableDevice);
                cmd.AddParameter("AllowRemoteDesktop", policy.AllowRemoteDesktop);
                cmd.AddParameter("AllowSimpleDevicePassword", policy.AllowSimpleDevicePassword);
                cmd.AddParameter("AllowStorageCard", policy.AllowStorageCard);
                cmd.AddParameter("AllowTextMessaging", policy.AllowTextMessaging);
                cmd.AddParameter("AllowUnsignedApplications", policy.AllowUnsignedApplications);
                cmd.AddParameter("AllowUnsignedInstallationPackages", policy.AllowUnsignedInstallationPackages);
                cmd.AddParameter("AllowWiFi", policy.AllowWiFi);
                cmd.AddParameter("AlphanumericDevicePasswordRequired", policy.AlphanumericDevicePasswordRequired);
                cmd.AddParameter("AttachmentsEnabled", policy.AttachmentsEnabled);
                cmd.AddParameter("DeviceEncryptionEnabled", policy.DeviceEncryptionEnabled);
                cmd.AddParameter("DevicePasswordEnabled", policy.DevicePasswordEnabled);
                cmd.AddParameter("MaxCalendarAgeFilter", policy.MaxCalendarAgeFilter);
                cmd.AddParameter("MaxEmailAgeFilter", policy.MaxEmailAgeFilter);
                cmd.AddParameter("PasswordRecoveryEnabled", policy.PasswordRecoveryEnabled);
                cmd.AddParameter("RequireStorageCardEncryption", policy.RequireStorageCardEncryption);
                cmd.AddParameter("RequireManualSyncWhenRoaming", policy.RequireManualSyncWhenRoaming);

                if (policy.DevicePolicyRefreshInterval > 0)
                    cmd.AddParameter("DevicePolicyRefreshInterval", new TimeSpan(policy.DevicePolicyRefreshInterval, 0, 0).ToString());
                else
                    cmd.AddParameter("DevicePolicyRefreshInterval", "Unlimited");

                if (policy.MaxAttachmentSize > 0)
                    cmd.AddParameter("MaxAttachmentSize", policy.MaxAttachmentSize + "KB");
                else
                    cmd.AddParameter("MaxAttachmentSize", "Unlimited");

                if (policy.MaxDevicePasswordFailedAttempts > 0)
                    cmd.AddParameter("MaxDevicePasswordFailedAttempts", policy.MaxDevicePasswordFailedAttempts);
                else
                    cmd.AddParameter("MaxDevicePasswordFailedAttempts", "Unlimited");

                if (policy.MaxEmailBodyTruncationSize > 0)
                    cmd.AddParameter("MaxEmailBodyTruncationSize", policy.MaxEmailBodyTruncationSize);
                else
                    cmd.AddParameter("MaxEmailBodyTruncationSize", "Unlimited");

                if (policy.MaxInactivityTimeDeviceLock > 0)
                    cmd.AddParameter("MaxInactivityTimeDeviceLock", new TimeSpan(0, policy.MaxInactivityTimeDeviceLock, 0).ToString());
                else
                    cmd.AddParameter("MaxInactivityTimeDeviceLock", "Unlimited");

                if (policy.MinDevicePasswordComplexCharacters > 0)
                    cmd.AddParameter("MinDevicePasswordComplexCharacters", policy.MinDevicePasswordComplexCharacters);
                else
                    cmd.AddParameter("MinDevicePasswordComplexCharacters", 1);

                if (policy.MinDevicePasswordLength > 0)
                    cmd.AddParameter("MinDevicePasswordLength", policy.MinDevicePasswordLength);
                else
                    cmd.AddParameter("MinDevicePasswordLength", null);

                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Info("Added new Activesync mailbox policy " + policy.DisplayName + " in " + stopwatch.Elapsed.ToString() + " second(s)");
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

        /// <summary>
        /// Updates an existing activesync policy
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="exchangeVersion"></param>
        public void Update_ActiveSyncPolicy(BaseActivesyncPolicy policy, Enumerations.ProductVersion version)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Updating Activesync mailbox policy " + policy.DisplayName);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Update the distribution group
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Set-ActiveSyncMailboxPolicy");

                if (!string.IsNullOrEmpty(policy.OldDisplayName))
                    cmd.AddParameter("Identity", policy.OldDisplayName);
                else
                    cmd.AddParameter("Identity", policy.DisplayName);

                cmd.AddParameter("Name", policy.DisplayName);
                cmd.AddParameter("AllowBluetooth", policy.AllowBluetooth);
                cmd.AddParameter("AllowBrowser", policy.AllowBrowser);
                cmd.AddParameter("AllowCamera", policy.AllowCamera);
                cmd.AddParameter("AllowConsumerEmail", policy.AllowConsumerEmail);
                cmd.AddParameter("AllowDesktopSync", policy.AllowDesktopSync);
                cmd.AddParameter("AllowHTMLEmail", policy.AllowHTMLEmail);
                cmd.AddParameter("AllowInternetSharing", policy.AllowInternetSharing);
                cmd.AddParameter("AllowIrDA", policy.AllowIrDA);
                cmd.AddParameter("AllowNonProvisionableDevices", policy.AllowNonProvisionableDevice);
                cmd.AddParameter("AllowRemoteDesktop", policy.AllowRemoteDesktop);
                cmd.AddParameter("AllowSimpleDevicePassword", policy.AllowSimpleDevicePassword);
                cmd.AddParameter("AllowStorageCard", policy.AllowStorageCard);
                cmd.AddParameter("AllowTextMessaging", policy.AllowTextMessaging);
                cmd.AddParameter("AllowUnsignedApplications", policy.AllowUnsignedApplications);
                cmd.AddParameter("AllowUnsignedInstallationPackages", policy.AllowUnsignedInstallationPackages);
                cmd.AddParameter("AllowWiFi", policy.AllowWiFi);
                cmd.AddParameter("AlphanumericDevicePasswordRequired", policy.AlphanumericDevicePasswordRequired);
                cmd.AddParameter("AttachmentsEnabled", policy.AttachmentsEnabled);
                cmd.AddParameter("DeviceEncryptionEnabled", policy.DeviceEncryptionEnabled);
                cmd.AddParameter("DevicePasswordEnabled", policy.DevicePasswordEnabled);
                cmd.AddParameter("MaxCalendarAgeFilter", policy.MaxCalendarAgeFilter);
                cmd.AddParameter("MaxEmailAgeFilter", policy.MaxEmailAgeFilter);
                cmd.AddParameter("PasswordRecoveryEnabled", policy.PasswordRecoveryEnabled);
                cmd.AddParameter("RequireStorageCardEncryption", policy.RequireStorageCardEncryption);
                cmd.AddParameter("RequireManualSyncWhenRoaming", policy.RequireManualSyncWhenRoaming);

                if (policy.DevicePolicyRefreshInterval > 0)
                    cmd.AddParameter("DevicePolicyRefreshInterval", new TimeSpan(policy.DevicePolicyRefreshInterval, 0, 0).ToString());
                else
                    cmd.AddParameter("DevicePolicyRefreshInterval", "Unlimited");

                if (policy.MaxAttachmentSize > 0)
                    cmd.AddParameter("MaxAttachmentSize", policy.MaxAttachmentSize + "KB");
                else
                    cmd.AddParameter("MaxAttachmentSize", "Unlimited");

                if (policy.MaxDevicePasswordFailedAttempts > 0)
                    cmd.AddParameter("MaxDevicePasswordFailedAttempts", policy.MaxDevicePasswordFailedAttempts);
                else
                    cmd.AddParameter("MaxDevicePasswordFailedAttempts", "Unlimited");

                if (policy.MaxEmailBodyTruncationSize > 0)
                    cmd.AddParameter("MaxEmailBodyTruncationSize", policy.MaxEmailBodyTruncationSize);
                else
                    cmd.AddParameter("MaxEmailBodyTruncationSize", "Unlimited");

                if (policy.MaxInactivityTimeDeviceLock > 0)
                    cmd.AddParameter("MaxInactivityTimeDeviceLock", new TimeSpan(0, policy.MaxInactivityTimeDeviceLock, 0).ToString());
                else
                    cmd.AddParameter("MaxInactivityTimeDeviceLock", "Unlimited");

                if (policy.MinDevicePasswordComplexCharacters > 0)
                    cmd.AddParameter("MinDevicePasswordComplexCharacters", policy.MinDevicePasswordComplexCharacters);
                else
                    cmd.AddParameter("MinDevicePasswordComplexCharacters", 1);

                if (policy.MinDevicePasswordLength > 0)
                    cmd.AddParameter("MinDevicePasswordLength", policy.MinDevicePasswordLength);
                else
                    cmd.AddParameter("MinDevicePasswordLength", null);

                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Info("Updated Activesync mailbox policy " + policy.DisplayName + " in " + stopwatch.Elapsed.ToString() + " second(s)");
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

        /// <summary>
        /// Adds a member to a distribution group
        /// </summary>
        /// <param name="groupEmailAddress"></param>
        /// <param name="userPrincipalName"></param>
        public void Add_DistributionGroupMember(string groupEmailAddress, string userPrincipalName)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Adding new member to distribution group " + groupEmailAddress + ". Member login name: " + userPrincipalName);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Update the distribution group
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Add-DistributionGroupMember");
                cmd.AddParameter("Identity", groupEmailAddress);
                cmd.AddParameter("Member", userPrincipalName);
                cmd.AddParameter("BypassSecurityGroupManagerCheck");
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
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

        /// <summary>
        /// Removes a member from distribution group
        /// </summary>
        /// <param name="groupEmailAddress"></param>
        /// <param name="userPrincipalName"></param>
        public void Remove_DistributionGroupMember(string groupEmailAddress, string userPrincipalName)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Removing member from distribution group " + groupEmailAddress + ". Member login name: " + userPrincipalName);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Update the distribution group
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Remove-DistributionGroupMember");
                cmd.AddParameter("Identity", groupEmailAddress);
                cmd.AddParameter("Member", userPrincipalName);
                cmd.AddParameter("BypassSecurityGroupManagerCheck");
                cmd.AddParameter("DomainController", this.domainController);
                cmd.AddParameter("Confirm", false);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
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

        /// <summary>
        /// Removes a distribution group from Exchange
        /// </summary>
        /// <param name="emailAddress"></param>
        public void Remove_DistributionGroup(string emailAddress)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Deleting distribution group " + emailAddress);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Remove-DistributionGroup");
                cmd.AddParameter("Identity", emailAddress);
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully deleted the distribution group " + emailAddress + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // Do not throw error if it already exists
                if (!ex.ToString().Contains("couldn't be found on"))
                    throw;
                else
                    this.logger.Info("Powershell error was thrown because the distribution group does not exist in Exchange. Continuing without error.");
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Gets a distribution group's information
        /// </summary>
        /// <param name="groupEmail"></param>
        /// <returns></returns>
        public ExchangeGroup Get_DistributionGroup(string groupEmail)
        {
            // Our return object
            ExchangeGroup group = new ExchangeGroup();

            PowerShell powershell = null;
            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Getting distribution group [" + groupEmail + "]");
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Enable the mailbox
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Get-DistributionGroup");
                cmd.AddParameter("Identity", groupEmail);
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;

                // Invoke and get returned information
                foreach (PSObject ps in powershell.Invoke())
                {
                    // Check and make sure it is a user
                    group.JoinRestriction = ps.Members["MemberJoinRestriction"].Value.ToString();
                    group.DepartRestriction = ps.Members["MemberDepartRestriction"].Value.ToString();
                    group.CompanyCode = ps.Members["CustomAttribute1"].Value.ToString();
                    group.DisplayName = ps.Members["DisplayName"].Value.ToString();
                    group.PrimarySmtpAddress = ps.Members["PrimarySmtpAddress"].Value.ToString();
                    group.Hidden = bool.Parse(ps.Members["HiddenFromAddressListsEnabled"].Value.ToString());
                    group.RequireSenderAuthentication = bool.Parse(ps.Members["RequireSenderAuthenticationEnabled"].Value.ToString());
                    group.SendModerationNotifications = ps.Members["SendModerationNotifications"].Value.ToString();
                    group.DistinguishedName = ps.Members["DistinguishedName"].Value.ToString();
                    group.ModerationEnabled = bool.Parse(ps.Members["ModerationEnabled"].Value.ToString());

                    // Get members of group
                    group.Members = new List<MailObject>();
                    group.Members = Get_DistributionGroupMembers(groupEmail);

                    // Sort the list
                    if (group.Members != null)
                    {
                        group.Members = group.Members.OrderBy(x => x.DisplayName).ToList();
                    }
                    
                    // Get managed by users (aka owners)
                    if (ps.Members["ManagedBy"] != null)
                    {
                        PSObject multiValue = (PSObject)ps.Members["ManagedBy"].Value;
                        ArrayList owners = (ArrayList)multiValue.BaseObject;

                        group.ManagedBy = (string[])owners.ToArray(typeof(string));
                    }

                    // Get if it is restricted to only certain people
                    if (ps.Members["AcceptMessagesOnlyFromSendersOrMembers"] != null)
                        group.WhoCanSendToGroup = ps.Members["AcceptMessagesOnlyFromSendersOrMembers"].Value.ToString().Split(',');

                    // Get group moderators
                    if (ps.Members["ModeratedBy"] != null)
                        group.GroupModerators = ps.Members["ModeratedBy"].Value.ToString().Split(',');

                    // Get the bypass list
                    if (ps.Members["BypassModerationFromSendersOrMembers"] != null)
                        group.SendersNotRequiringApproval = ps.Members["BypassModerationFromSendersOrMembers"].Value.ToString().Split(',');
                }

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully retrieved group information for [" + groupEmail + "] in " + stopwatch.Elapsed.ToString() + " second(s)");

                // Return list
                return group;
            }
            catch (Exception ex)
            {
                // DEBUG //
                this.logger.Error("Error getting distribution group " + groupEmail + ": " + ex.ToString());

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Gets a list of members in a distribution group
        /// </summary>
        /// <param name="groupEmail"></param>
        /// <returns></returns>
        public List<MailObject> Get_DistributionGroupMembers(string groupEmail)
        {
            // Our return object
            List<MailObject> foundObjects = new List<MailObject>();

            PowerShell powershell = null;
            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Getting members of distribution group [" + groupEmail + "]");
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Enable the mailbox
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Get-DistributionGroupMember");
                cmd.AddParameter("Identity", groupEmail);
                cmd.AddParameter("ResultSize", "Unlimited");
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;

                // Invoke and get returned information
                Collection<PSObject> results = powershell.Invoke();
                foreach (PSObject ps in results)
                {
                    // Get the type of recipient it is
                    string recipientType = ps.Members["RecipientType"].Value.ToString();

                    // Now lets go through and add to our object
                    switch (recipientType.ToLower())
                    {
                        case "user":
                        case "usermailbox":
                            foundObjects.Add(new MailObject()
                            {
                                CompanyCode = ps.Members["CustomAttribute1"].Value.ToString(),
                                DisplayName = ps.Members["DisplayName"].Value.ToString(),
                                DistinguishedName = ps.Members["DistinguishedName"].Value.ToString(),
                                EmailAddress = ps.Members["PrimarySmtpAddress"].Value.ToString(),
                                ObjectType = Enumerations.ObjectType.User
                            });
                            break;
                        case "distributiongroup": case "mailuniversaldistributiongroup":
                            foundObjects.Add(new MailObject()
                            {
                                CompanyCode = ps.Members["CustomAttribute1"].Value.ToString(),
                                DisplayName = ps.Members["DisplayName"].Value.ToString(),
                                DistinguishedName = ps.Members["DistinguishedName"].Value.ToString(),
                                EmailAddress = ps.Members["PrimarySmtpAddress"].Value.ToString(),
                                ObjectType = Enumerations.ObjectType.Group
                            });
                            break;
                        case "mailcontact":
                            foundObjects.Add(new MailObject()
                            {
                                CompanyCode = ps.Members["CustomAttribute1"].Value.ToString(),
                                DisplayName = ps.Members["DisplayName"].Value.ToString(),
                                DistinguishedName = ps.Members["DistinguishedName"].Value.ToString(),
                                EmailAddress = ps.Members["PrimarySmtpAddress"].Value.ToString(),
                                ObjectType = Enumerations.ObjectType.Contact
                            });
                            break;
                        default:
                            foundObjects.Add(new MailObject()
                            {
                                CompanyCode = ps.Members["CustomAttribute1"].Value.ToString(),
                                DisplayName = ps.Members["DisplayName"].Value.ToString(),
                                DistinguishedName = ps.Members["DistinguishedName"].Value.ToString(),
                                EmailAddress = ps.Members["PrimarySmtpAddress"].Value.ToString(),
                                ObjectType = Enumerations.ObjectType.User
                            });
                            break;
                    }
                }

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully retrieved members for group [" + groupEmail + "] in " + stopwatch.Elapsed.ToString() + " second(s)");

                // Return list
                return foundObjects;
            }
            catch (Exception ex)
            {
                // DEBUG //
                this.logger.Error("Error getting distribution group members for group " + groupEmail + ": " + ex.ToString());

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Enables a security group as a distribution group in Exchange
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="alias"></param>
        /// <param name="primarySmtpAddress"></param>
        public void Enable_DistributionGroup(string companyCode, string groupName, bool hidden)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Enabling security group " + groupName + " as a Distribution Group.");
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Enable-DistributionGroup");
                cmd.AddParameter("Identity", groupName);
                cmd.AddParameter("Alias", groupName.Replace("@", "_"));
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                cmd = new PSCommand();
                cmd.AddCommand("Set-DistributionGroup");
                cmd.AddParameter("Identity", groupName);
                cmd.AddParameter("BypassSecurityGroupManagerCheck");
                cmd.AddParameter("CustomAttribute1", companyCode);
                cmd.AddParameter("HiddenFromAddressListsEnabled", hidden);
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully enabled " + groupName + " as a Distribution Group in Exchange in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to enable " + groupName + " as a Distribution Group.", ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Gets the public folder hierarchy
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public List<BasePublicFolder> Get_PublicFolderHierarchy(string companyCode, CloudPanel.Modules.Base.Class.Enumerations.ProductVersion version)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Getting public folder hierarchy for  " + companyCode);

                // Create our list to return
                List<BasePublicFolder> publicFolders = new List<BasePublicFolder>();

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Get the hierarchy
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Get-PublicFolder");
                cmd.AddParameter("Identity", @"\" + companyCode);
                cmd.AddParameter("Recurse");
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;

                Collection<PSObject> obj = powershell.Invoke();

                // Now loop through
                foreach (PSObject o in obj)
                {
                    BasePublicFolder p = new BasePublicFolder();
                    p.Path = o.Members["Identity"].Value.ToString();

                    this.logger.Debug("Found public folder: " + p.Path);

                    p.Name = o.Members["Name"].Value.ToString();
                    p.MailEnabled = bool.Parse(o.Members["MailEnabled"].Value.ToString());
                    p.ParentPath = o.Members["ParentPath"].Value.ToString();

                    if (version == Enumerations.ProductVersion.Exchange2013)
                    {
                        if (o.Members["FolderClass"].Value != null)
                            p.FolderClass = o.Members["FolderClass"].Value.ToString();
                    }
                    else if (version == Enumerations.ProductVersion.Exchange2010)
                    {
                        if (o.Members["FolderType"].Value != null)
                            p.FolderClass = o.Members["FolderType"].Value.ToString();
                    }

                    // Add to our list
                    publicFolders.Add(p);
                }

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully retrieved hierarchy on public folders for " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");

                return publicFolders;
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to retrieve public folder hierarchy for " + companyCode, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Gets the public folder information for the specific public folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public BasePublicFolder Get_PublicFolder(string path)
        {
            PowerShell powershell = null;

            try
            {

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Getting public folder " + path);

                // Create our list to return
                BasePublicFolder p = new BasePublicFolder();

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Get the hierarchy
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Get-PublicFolder");
                cmd.AddParameter("Identity", path);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;

                Collection<PSObject> obj = powershell.Invoke();

                // Now loop through
                foreach (PSObject o in obj)
                {
                    p.Path = o.Members["Identity"].Value.ToString();
                    p.Name = o.Members["Name"].Value.ToString();
                    p.MailEnabled = bool.Parse(o.Members["MailEnabled"].Value.ToString());
                    p.ParentPath = o.Members["ParentPath"].Value.ToString();
                    p.PerUserReadStateEnabled = bool.Parse(o.Members["PerUserReadStateEnabled"].Value.ToString());
                    p.HasSubFolders = bool.Parse(o.Members["HasSubFolders"].Value.ToString());

                    if (o.Members["IssueWarningQuota"].Value != null)
                        p.IssueWarningQuota = FormatExchangeSize(o.Members["IssueWarningQuota"].Value.ToString());
                    else
                        p.IssueWarningQuota = "";

                    if (o.Members["ProhibitPostQuota"].Value != null)
                        p.ProhibitPostQuota = FormatExchangeSize(o.Members["ProhibitPostQuota"].Value.ToString());
                    else
                        p.ProhibitPostQuota = "";

                    if (o.Members["MaxItemSize"].Value != null)
                        p.MaxItemSize = FormatExchangeSize(o.Members["MaxItemSize"].Value.ToString());
                    else
                        p.MaxItemSize = "";

                    if (o.Members["RetainDeletedItemsFor"].Value != null)
                        p.RetainDeletedItemsFor = TimeSpan.Parse(o.Members["RetainDeletedItemsFor"].Value.ToString()).Days.ToString();
                    else
                        p.RetainDeletedItemsFor = "";

                    if (o.Members["AgeLimit"].Value != null)
                        p.AgeLimit = TimeSpan.Parse(o.Members["AgeLimit"].Value.ToString()).Days.ToString();
                    else
                        p.AgeLimit = string.Empty;
                }

                // Now check if it is mail enabled
                if (p.MailEnabled)
                {
                    p.MailFolderInfo = new BaseMailPublicFolder();

                    cmd = new PSCommand();
                    cmd.AddCommand("Get-MailPublicFolder");
                    cmd.AddParameter("Identity", path);
                    cmd.AddParameter("DomainController", domainController);
                    powershell.Commands = cmd;

                    obj = powershell.Invoke();

                    // Now get all the mail info
                    foreach (PSObject o in obj)
                    {
                        p.MailFolderInfo.DisplayName = o.Members["DisplayName"].Value.ToString();
                        p.MailFolderInfo.CompanyCode = o.Members["CustomAttribute1"].Value.ToString();
                        p.MailFolderInfo.Hidden = bool.Parse(o.Members["HiddenFromAddressListsEnabled"].Value.ToString());
                        p.MailFolderInfo.EmailAddress = o.Members["PrimarySmtpAddress"].Value.ToString();
                    }
                }

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully retrieved public folder information on path " + path + " in " + stopwatch.Elapsed.ToString() + " second(s)");

                return p;
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to retrieve public folder information on path " + path, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Creates a new public folder for Exchange 2013
        /// </summary>
        /// <param name="companyCode"></param>
        public void New_PublicFolder(string companyCode)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Creating new public folder for " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("New-PublicFolder");
                cmd.AddParameter("Name", companyCode);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully created a new public folder for " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // Do not throw error if it already exists
                if (!ex.ToString().Contains("already exists"))
                {
                    // FATAL //
                    this.logger.Fatal("Failed to create public folder for " + companyCode, ex);

                    throw;
                }
                else
                    this.logger.Info("Powershell error was thrown because the public folder already existed. Continuing without error.");
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Removes a public folder
        /// </summary>
        /// <param name="companyCode"></param>
        public void Remove_PublicFolder(string companyCode)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Removing public folder for " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Remove-PublicFolder");
                cmd.AddParameter("Identity", @"\" + companyCode);
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully removed public folder for " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to remove public folder for " + companyCode, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Disable mail on a public folder
        /// </summary>
        /// <param name="companyCode"></param>
        public void Disable_MailPublicFolder(string path)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Disabling mail on public folder " + path);

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Disable-MailPublicFolder");
                cmd.AddParameter("Identity", path);
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully disabled mail on public folder " + path + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to disable mail on public folder " + path, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Enable mail on a public folder
        /// </summary>
        /// <param name="companyCode"></param>
        public void Enable_MailPublicFolder(string path, bool hidden, string companyCode, string displayName, string primarySmtpAddress)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Enabling mail on public folder " + path);

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Enable-MailPublicFolder");
                cmd.AddParameter("Identity", path);
                cmd.AddParameter("HiddenFromAddressListsEnabled", hidden);
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                cmd.AddStatement();
                cmd.AddCommand("Set-MailPublicFolder");
                cmd.AddParameter("Identity", path);
                cmd.AddParameter("EmailAddressPolicyEnabled", false);
                cmd.AddParameter("CustomAttribute1", companyCode.Replace(" ", string.Empty));
                cmd.AddParameter("PrimarySmtpAddress", primarySmtpAddress);
                cmd.AddParameter("DisplayName", displayName);
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully enabled mail on public folder " + path + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to enable mail on public folder " + path, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Changes mail settings on a mail enabled public folder
        /// </summary>
        /// <param name="companyCode"></param>
        public void Set_MailPublicFolder(string path, bool hidden, string companyCode, string displayName, string primarySmtpAddress)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Changing mail settings on public folder " + path);

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Set-MailPublicFolder");
                cmd.AddParameter("Identity", path);
                cmd.AddParameter("EmailAddressPolicyEnabled", false);
                cmd.AddParameter("CustomAttribute1", companyCode.Replace(" ", string.Empty));
                cmd.AddParameter("PrimarySmtpAddress", primarySmtpAddress);
                cmd.AddParameter("DisplayName", displayName);
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully set mail settings on public folder " + path + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to set mail settings on public folder " + path, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Sets public folder information
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="issueWarningQuota"></param>
        /// <param name="prohibitPostQuota"></param>
        /// <param name="maxItemSize"></param>
        /// <param name="ageLimit"></param>
        /// <param name="retainDeletedItemsFor"></param>
        /// <param name="perUserReadStateEnabled"></param>
        /// <param name="exchangeVersion"></param>
        public void Set_PublicFolder(string path, int issueWarningQuota, int prohibitPostQuota, int maxItemSize, int ageLimit, int retainDeletedItemsFor, Enumerations.ProductVersion version)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                this.logger.Debug("Setting information on public folder for " + path);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Set-PublicFolder");
                cmd.AddParameter("Identity", path);

                if (ageLimit > 0)
                    cmd.AddParameter("AgeLimit", new TimeSpan(ageLimit, 0, 0, 0));
                else
                    cmd.AddParameter("AgeLimit", null);

                if (issueWarningQuota > 0)
                    cmd.AddParameter("IssueWarningQuota", issueWarningQuota + "MB");
                else
                    cmd.AddParameter("IssueWarningQuota", "Unlimited");

                if (maxItemSize > 0)
                    cmd.AddParameter("MaxItemSize", maxItemSize + "MB");
                else
                    cmd.AddParameter("MaxItemSize", "Unlimited");

                if (prohibitPostQuota > 0)
                    cmd.AddParameter("ProhibitPostQuota", prohibitPostQuota + "MB");
                else
                    cmd.AddParameter("ProhibitPostQuota", "Unlimited");

                if (retainDeletedItemsFor > 0)
                    cmd.AddParameter("RetainDeletedItemsFor", new TimeSpan(retainDeletedItemsFor, 0, 0, 0));
                else
                    cmd.AddParameter("RetainDeletedItemsFor", null);

                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);

                // Exchange 2010 parameters only
                if (version == Enumerations.ProductVersion.Exchange2010)
                {
                    cmd.AddParameter("UseDatabaseQuotaDefaults", false);
                    cmd.AddParameter("UseDatabaseAgeDefaults", false);
                }

                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully created a set public folder for " + path + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to set public folder for " + path, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Adds the PublicFolderAdmins and PublicFolderUsers group to the public folder
        /// </summary>
        /// <param name="companyCode"></param>
        public void Add_PublicFolderClientPermission(string companyCode)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                string companyCodeWithoutSpaces = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Adding public folder permissions for " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Get the current permissions
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Get-PublicFolderClientPermission");
                cmd.AddParameter("Identity", @"\" + companyCode);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;

                Collection<PSObject> obj = powershell.Invoke();
                var currentUsers = from o in obj
                                   select o.Members["User"].Value;

                // Log
                this.logger.Debug("Current users that have permissions on this public folder: " + String.Join(",", currentUsers));

                if (!(currentUsers.Any(x => x.ToString().Equals("PublicFolderAdmins@" + companyCodeWithoutSpaces, StringComparison.CurrentCultureIgnoreCase))))
                {
                    // Add the public folder owners
                    cmd = new PSCommand();
                    cmd.AddCommand("Add-PublicFolderClientPermission");
                    cmd.AddParameter("Identity", @"\" + companyCode);
                    cmd.AddParameter("User", "PublicFolderAdmins@" + companyCodeWithoutSpaces);
                    cmd.AddParameter("AccessRights", "Owner");
                    cmd.AddParameter("Confirm", false);
                    cmd.AddParameter("DomainController", domainController);
                    powershell.Commands = cmd;
                    powershell.Invoke();
                }

                if (!(currentUsers.Any(x => x.ToString().Equals("PublicFolderUsers@" + companyCodeWithoutSpaces, StringComparison.CurrentCultureIgnoreCase))))
                {
                    // Add the public folder users
                    cmd = new PSCommand();
                    cmd.AddCommand("Add-PublicFolderClientPermission");
                    cmd.AddParameter("Identity", @"\" + companyCode);
                    cmd.AddParameter("User", "PublicFolderUsers@" + companyCodeWithoutSpaces);
                    cmd.AddParameter("AccessRights", "Reviewer");
                    cmd.AddParameter("Confirm", false);
                    cmd.AddParameter("DomainController", domainController);
                    powershell.Commands = cmd;
                    powershell.Invoke();
                }

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully added permissions on public folder for " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to create public folder for " + companyCode, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Removes the default public folder permissions for a company's public folder
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="exchangeVersion"></param>
        public void Remove_PublicFolderDefaultPermissions(string companyCode, Enumerations.ProductVersion version)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                string companyCodeWithoutSpaces = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Removing default public folder permissions for " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();

                if (version == Enumerations.ProductVersion.Exchange2010)
                {
                    // Remove Default permissions
                    cmd.AddCommand("Remove-PublicFolderClientPermission");
                    cmd.AddParameter("Identity", @"\" + companyCode);
                    cmd.AddParameter("User", "Default");
                    cmd.AddParameter("AccessRights", "Author");
                    cmd.AddParameter("Confirm", false);
                    cmd.AddParameter("DomainController", domainController);
                    powershell.Commands = cmd;
                    powershell.Invoke();

                    // Remove Anonymous permissions
                    cmd = new PSCommand();
                    cmd.AddCommand("Remove-PublicFolderClientPermission");
                    cmd.AddParameter("Identity", @"\" + companyCode);
                    cmd.AddParameter("User", "Anonymous");
                    cmd.AddParameter("AccessRights", "CreateItems");
                    cmd.AddParameter("Confirm", false);
                    cmd.AddParameter("DomainController", domainController);
                    powershell.Commands = cmd;
                    powershell.Invoke();
                }
                else if (version == Enumerations.ProductVersion.Exchange2013)
                {
                    // Remove Default permissions
                    cmd.AddCommand("Remove-PublicFolderClientPermission");
                    cmd.AddParameter("Identity", @"\" + companyCode);
                    cmd.AddParameter("User", "Default");
                    cmd.AddParameter("Confirm", false);
                    cmd.AddParameter("DomainController", domainController);
                    powershell.Commands = cmd;
                    powershell.Invoke();

                    // Remove Anonymous permissions
                    cmd = new PSCommand();
                    cmd.AddCommand("Remove-PublicFolderClientPermission");
                    cmd.AddParameter("Identity", @"\" + companyCode);
                    cmd.AddParameter("User", "Anonymous");
                    cmd.AddParameter("Confirm", false);
                    cmd.AddParameter("DomainController", domainController);
                    powershell.Commands = cmd;
                    powershell.Invoke();
                }

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully removed permissions for public folder " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to create public folder for " + companyCode, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Creates a new room mailbox
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="companyCode"></param>
        /// <param name="companyExchDN"></param>
        public void New_RoomMailbox(ResourceMailbox mbxInfo, string companyCode, string companyExchDN, MailboxPlan plan)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                this.logger.Debug("Creating new room mailbox for " + companyCode);

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("New-Mailbox");
                cmd.AddParameter("Name", mbxInfo.DisplayName);
                cmd.AddParameter("DisplayName", mbxInfo.DisplayName);
                cmd.AddParameter("OrganizationalUnit", companyExchDN);
                cmd.AddParameter("UserPrincipalName", mbxInfo.UserPrincipalName);
                cmd.AddParameter("PrimarySmtpAddress", mbxInfo.PrimarySmtpAddress);
                cmd.AddParameter("Room");
                cmd.AddParameter("DomainController", domainController);
                cmd.AddStatement();

                cmd.AddCommand("Set-Mailbox");
                cmd.AddParameter("Identity", mbxInfo.UserPrincipalName);
                cmd.AddParameter("CustomAttribute1", companyCode);
                cmd.AddParameter("AddressBookPolicy", string.Format("{0} ABP", companyCode));
                cmd.AddParameter("EmailAddressPolicyEnabled", false);
                cmd.AddParameter("IssueWarningQuota", plan.WarningSizeInMB + "MB");
                cmd.AddParameter("MaxReceiveSize", plan.MaxReceiveSizeInKB + "KB");
                cmd.AddParameter("MaxSendSize", plan.MaxSendSizeInKB + "KB");
                cmd.AddParameter("OfflineAddressBook", companyCode + " OAL");
                cmd.AddParameter("ProhibitSendQuota", plan.SetSizeInMB + "MB");
                cmd.AddParameter("ProhibitSendReceiveQuota", plan.SetSizeInMB + "MB");
                cmd.AddParameter("RecipientLimits", plan.MaxRecipients);
                cmd.AddParameter("RetainDeletedItemsFor", plan.KeepDeletedItemsInDays);
                cmd.AddParameter("UseDatabaseQuotaDefaults", false);
                cmd.AddParameter("UseDatabaseRetentionDefaults", false);
                cmd.AddParameter("RetainDeletedItemsUntilBackup", true);
                cmd.AddParameter("RoleAssignmentPolicy", "Alternate Assignment Policy");
                cmd.AddParameter("DomainController", domainController);
                
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully created room mailbox for " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to create room mailbox for " + companyCode, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Creates a new equipment mailbox
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="companyCode"></param>
        /// <param name="companyExchDN"></param>
        public void New_EquipmentMailbox(ResourceMailbox mbxInfo, string companyCode, string companyExchDN, MailboxPlan plan)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                this.logger.Debug("Creating new equipment mailbox for " + companyCode);

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("New-Mailbox");
                cmd.AddParameter("Name", mbxInfo.DisplayName);
                cmd.AddParameter("DisplayName", mbxInfo.DisplayName);
                cmd.AddParameter("OrganizationalUnit", companyExchDN);
                cmd.AddParameter("UserPrincipalName", mbxInfo.UserPrincipalName);
                cmd.AddParameter("PrimarySmtpAddress", mbxInfo.PrimarySmtpAddress);
                cmd.AddParameter("Equipment");
                cmd.AddParameter("DomainController", domainController);
                cmd.AddStatement();

                cmd.AddCommand("Set-Mailbox");
                cmd.AddParameter("Identity", mbxInfo.UserPrincipalName);
                cmd.AddParameter("CustomAttribute1", companyCode);
                cmd.AddParameter("AddressBookPolicy", string.Format("{0} ABP", companyCode));
                cmd.AddParameter("EmailAddressPolicyEnabled", false);
                cmd.AddParameter("IssueWarningQuota", plan.WarningSizeInMB + "MB");
                cmd.AddParameter("MaxReceiveSize", plan.MaxReceiveSizeInKB + "KB");
                cmd.AddParameter("MaxSendSize", plan.MaxSendSizeInKB + "KB");
                cmd.AddParameter("OfflineAddressBook", companyCode + " OAL");
                cmd.AddParameter("ProhibitSendQuota", plan.SetSizeInMB + "MB");
                cmd.AddParameter("ProhibitSendReceiveQuota", plan.SetSizeInMB + "MB");
                cmd.AddParameter("RecipientLimits", plan.MaxRecipients);
                cmd.AddParameter("RetainDeletedItemsFor", plan.KeepDeletedItemsInDays);
                cmd.AddParameter("UseDatabaseQuotaDefaults", false);
                cmd.AddParameter("UseDatabaseRetentionDefaults", false);
                cmd.AddParameter("RetainDeletedItemsUntilBackup", true);
                cmd.AddParameter("RoleAssignmentPolicy", "Alternate Assignment Policy");
                cmd.AddParameter("DomainController", domainController);

                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully created equipment mailbox for " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to create equipment mailbox for " + companyCode, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Creates a new shared mailbox
        /// </summary>
        /// <param name="mbxInfo"></param>
        /// <param name="companyCode"></param>
        /// <param name="companyExchDN"></param>
        /// <param name="plan"></param>
        public void New_SharedMailbox(ResourceMailbox mbxInfo, string companyCode, string companyExchDN, MailboxPlan plan)
        {
            PowerShell powershell = null;

            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                this.logger.Debug("Creating new shared mailbox for " + companyCode);

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("New-Mailbox");
                cmd.AddParameter("Name", mbxInfo.DisplayName);
                cmd.AddParameter("DisplayName", mbxInfo.DisplayName);
                cmd.AddParameter("OrganizationalUnit", companyExchDN);
                cmd.AddParameter("UserPrincipalName", mbxInfo.UserPrincipalName);
                cmd.AddParameter("PrimarySmtpAddress", mbxInfo.PrimarySmtpAddress);
                cmd.AddParameter("Shared");
                cmd.AddParameter("DomainController", domainController);
                cmd.AddStatement();

                cmd.AddCommand("Set-Mailbox");
                cmd.AddParameter("Identity", mbxInfo.UserPrincipalName);
                cmd.AddParameter("CustomAttribute1", companyCode);
                cmd.AddParameter("AddressBookPolicy", string.Format("{0} ABP", companyCode));
                cmd.AddParameter("EmailAddressPolicyEnabled", false);
                cmd.AddParameter("IssueWarningQuota", plan.WarningSizeInMB + "MB");
                cmd.AddParameter("MaxReceiveSize", plan.MaxReceiveSizeInKB + "KB");
                cmd.AddParameter("MaxSendSize", plan.MaxSendSizeInKB + "KB");
                cmd.AddParameter("OfflineAddressBook", companyCode + " OAL");
                cmd.AddParameter("ProhibitSendQuota", plan.SetSizeInMB + "MB");
                cmd.AddParameter("ProhibitSendReceiveQuota", plan.SetSizeInMB + "MB");
                cmd.AddParameter("RecipientLimits", plan.MaxRecipients);
                cmd.AddParameter("RetainDeletedItemsFor", plan.KeepDeletedItemsInDays);
                cmd.AddParameter("UseDatabaseQuotaDefaults", false);
                cmd.AddParameter("UseDatabaseRetentionDefaults", false);
                cmd.AddParameter("RetainDeletedItemsUntilBackup", true);
                cmd.AddParameter("RoleAssignmentPolicy", "Alternate Assignment Policy");
                cmd.AddParameter("DomainController", domainController);
                cmd.AddStatement();

                cmd.AddCommand("Set-CASMailbox");
                cmd.AddParameter("Identity", mbxInfo.UserPrincipalName);
                cmd.AddParameter("ImapEnabled", plan.IMAPEnabled);
                cmd.AddParameter("ECPEnabled", plan.ECPEnabled);
                cmd.AddParameter("MAPIEnabled", plan.MAPIEnabled);
                cmd.AddParameter("ActiveSyncEnabled", plan.ActiveSyncEnabled);
                cmd.AddParameter("PopEnabled", plan.POP3Enabled);
                cmd.AddParameter("OWAEnabled", plan.OWAEnabled);
                cmd.AddParameter("DomainController", domainController);

                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully created shared mailbox for " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to create shared mailbox for " + companyCode, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Gets the mailbox permissions for a mailbox
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public List<MailboxPermissions> Get_MailboxPermission(string userPrincipalName)
        {
            PowerShell powershell = null;

            try
            {

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Getting mailbox permissions for " + userPrincipalName);

                // Create our list to return
                List<MailboxPermissions> permissions = new List<MailboxPermissions>();

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Get the hierarchy
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Get-MailboxPermission");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;

                Collection<PSObject> obj = powershell.Invoke();

                // Now loop through
                foreach (PSObject ps in obj)
                {
                    MailboxPermissions tmp = new MailboxPermissions();
                    tmp.SamAccountName = ps.Members["User"].Value.ToString();
                    tmp.Deny = bool.Parse(ps.Members["Deny"].Value.ToString());
                    tmp.IsInherited = bool.Parse(ps.Members["IsInherited"].Value.ToString());

                    if (ps.Members["AccessRights"].Value != null)
                    {
                        PSObject multiValue = (PSObject)ps.Members["AccessRights"].Value;
                        ArrayList rights = (ArrayList)multiValue.BaseObject;

                        tmp.AccessRights = (string[])rights.ToArray(typeof(string));
                    }

                    permissions.Add(tmp);
                }

                
                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully retrieved mailbox permissions for " + userPrincipalName + " in " + stopwatch.Elapsed.ToString() + " second(s)");

                // Return our object
                return permissions;
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to retrieve mailbox permissions for " + userPrincipalName, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        /// <summary>
        /// Removes a mailbox from Exchange.
        /// *** THIS DELTES THE USER ACCOUNT TOO ***
        /// </summary>
        /// <param name="userPrincipalName"></param>
        public void Remove_Mailbox(string userPrincipalName)
        {
            PowerShell powershell = null;

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                this.logger.Debug("Removing mailbox " + userPrincipalName);

                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Remove-Mailbox");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", this.domainController);

                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // INFO //
                this.logger.Info("Successfully removed mailbox and user account for " + userPrincipalName + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to remove mailbox for " + userPrincipalName, ex);

                throw;
            }
            finally
            {
                if (powershell != null)
                    powershell.Dispose();
            }
        }

        #region Formatting

        private string FormatEmailsCommaSeperated(List<string> emails)
        {
            string formatted = "";

            foreach (string email in emails)
            {
                formatted += email + ",";
            }

            // Get rid of ending comma
            formatted = formatted.Substring(formatted.Length - 1, 1);

            // Return
            return formatted;
        }

        #endregion

        /// <summary>
        /// Checks for errors, logs to log file, and throws error
        /// </summary>
        /// <param name="errorCheck"></param>
        private void CheckErrors(ref PowerShell errorCheck, bool skipCouldntBeFound = false, bool skipMailboxAlreadyEnabled = false)
        {
            // Loop through our commands to build in string builder
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
            this.logger.Debug(sb.ToString());

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
                else if (errorCheck.Streams.Error[0].Exception.ToString().Contains("is of type UserMailbox") && skipMailboxAlreadyEnabled)
                {
                    // Skip on errors because the mailbox already exists
                    this.logger.Debug("Skipped error about enabling new mailbox because the mailbox already existed. Going to run the rest of the commands: " + errorCheck.Streams.Error[0].Exception.ToString());

                    // Clear errors because if we are skipping that it couldn't be found then we are probably
                    // running commands back to back
                    errorCheck.Streams.Error.Clear();
                }
                else if (errorCheck.Streams.Error[0].Exception.ToString().Contains("existing permission entry was found for user"))
                {
                    // Skip on errors because the permission already existed that we tried to add
                    this.logger.Debug("Skipped error about existing permission already existing. Going to continue running: " + errorCheck.Streams.Error[0].Exception.ToString());

                    errorCheck.Streams.Error.Clear();
                }
                else
                    throw errorCheck.Streams.Error[0].Exception;
            }
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
        /// Formats the exchange size from 434.00GB (23,23,34,234 bytes) to just kilobytes
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private string FormatExchangeSize(string size)
        {
            if (string.IsNullOrEmpty(size))
                return "";
            else if (size.Equals("Unlimited"))
                return "";
            else
            {
                this.logger.Debug("Attempting to format size: " + size);

                string newSize = size;

                string[] stringSeparators = new string[] { "TB (", "GB (", "MB (", "KB (", "B (" };

                if (newSize.Contains("TB ("))
                    newSize = ConvertToMB(decimal.Parse(newSize.Split(stringSeparators, StringSplitOptions.None)[0].Trim(), CultureInfo.InvariantCulture), "TB");
                else if (newSize.Contains("GB ("))
                    newSize = ConvertToMB(decimal.Parse(newSize.Split(stringSeparators, StringSplitOptions.None)[0].Trim(), CultureInfo.InvariantCulture), "GB");
                else if (newSize.Contains("MB ("))
                    newSize = ConvertToMB(decimal.Parse(newSize.Split(stringSeparators, StringSplitOptions.None)[0].Trim(), CultureInfo.InvariantCulture), "MB");
                else if (newSize.Contains("KB ("))
                    newSize = ConvertToMB(decimal.Parse(newSize.Split(stringSeparators, StringSplitOptions.None)[0].Trim(), CultureInfo.InvariantCulture), "KB");
                else if (newSize.Contains("B ("))
                    newSize = ConvertToMB(decimal.Parse(newSize.Split(stringSeparators, StringSplitOptions.None)[0].Trim(), CultureInfo.InvariantCulture), "B");

                this.logger.Debug("Original size was: " + size + " and the new size in megabytes is: " + newSize);

                return newSize.Trim();
            }
        }

        /// <summary>
        /// Converts the size from TB,GB,MB to KB
        /// </summary>
        /// <param name="size"></param>
        /// <param name="sizeType"></param>
        /// <returns></returns>
        private string ConvertToMB(decimal size, string sizeType)
        {
            decimal newSize = 0;

            switch (sizeType)
            {
                case "TB":
                    this.logger.Debug("Size was in terabytes... converting to megabytes...");
                    newSize = size * 1024 * 1024;
                    break;
                case "GB":
                    this.logger.Debug("Size was in gigabytes... converting to megabytes...");
                    newSize = size * 1024;
                    break;
                case "MB":
                    this.logger.Debug("Size was in megabytes... no need to convert...");
                    newSize = size;
                    break;
                case "KB":
                    this.logger.Debug("Size was in kilobytes... converting to megabytes...");
                    newSize = size / 1024;
                    break;
                case "B":
                    this.logger.Debug("Size was in bytes... converting to megabytes...");
                    newSize = (size / 1024) / 1024;
                    break;
                default:
                    newSize = size;
                    break;
            }

            return decimal.Round(newSize, 0).ToString();
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
