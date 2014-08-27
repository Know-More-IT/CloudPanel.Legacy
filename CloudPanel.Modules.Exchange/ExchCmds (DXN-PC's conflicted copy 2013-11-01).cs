using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Management.Automation;
using log4net;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Collections.ObjectModel;
using CloudPanel.Modules.Base;

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
        PowerShell powershell = null;

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
        public ExchCmds(string uri, string username, string password, bool kerberos, string domainController)
        {
            // DEBUG //
            this.logger.Debug(string.Format("Opened Exchange powershell class with {0}, {1}, {2}, {3}, {4}", uri, username, password, kerberos.ToString(), domainController));

            this.wsConn = GetConnection(uri, username, password, kerberos);
            this.domainController = domainController;

            // Create our runspace
            runspace = RunspaceFactory.CreateRunspace(wsConn);
            runspace.ThreadOptions = PSThreadOptions.Default;

            // Open the connection
            runspace.Open();
        }

        /// <summary>
        /// Enables a company for Exchange by creating all the address lists and other required objects
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="groupForDownloadRights"></param>
        public void Enable_Company(string companyCode, string groupForDownloadRights)
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
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes all mailboxes, GAL, OAL, address lists, etc associated with the company
        /// </summary>
        /// <param name="companyCode"></param>
        public void Disable_Company(string companyCode, List<string> domains)
        {
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
                cmd.AddParameter("Identity", companyCode + " - GAL");
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
        /// Creates a new Exchange contact
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="emailAddress"></param>
        /// <param name="hidden"></param>
        /// <param name="companyDN"></param>
        /// <param name="companyCode"></param>
        public void New_Contact(string displayName, string emailAddress, bool hidden, string companyDN, string companyCode)
        {
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
        /// Removes a mail contact from Exchange
        /// </summary>
        /// <param name="distinguishedName"></param>
        public void Remove_Contact(string distinguishedName)
        {
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
        /// Enables a user for a mailbox
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <param name="companyCode"></param>
        /// <param name="email"></param>
        /// <param name="quotaWarning"></param>
        /// <param name="quotaProhibitSend"></param>
        /// <param name="quotaProhibitSendReceive"></param>
        /// <param name="maxReceiveSize"></param>
        /// <param name="maxSendSize"></param>
        /// <param name="recipientLimit"></param>
        /// <param name="retainDeletedItems"></param>
        /// <param name="activeSyncEnabled"></param>
        /// <param name="ecpEnabled"></param>
        /// <param name="imapEnabled"></param>
        /// <param name="mapiEnabled"></param>
        /// <param name="owaEnabled"></param>
        /// <param name="popEnabled"></param>
        public void Enable_Mailbox(string userPrincipalName, string companyCode, string email, int quotaWarning, int quotaProhibitSend, int quotaProhibitSendReceive,
            int maxReceiveSize, int maxSendSize, int recipientLimit, int retainDeletedItems, bool activeSyncEnabled, bool ecpEnabled, bool imapEnabled, bool mapiEnabled,
            bool owaEnabled, bool popEnabled, string activeSyncPolicyName = null)
        {
            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Enabling mailbox for user [" + userPrincipalName + "] for company code " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Enable the mailbox
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Enable-Mailbox");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("AddressBookPolicy", companyCode + " ABP");
                cmd.AddParameter("PrimarySmtpAddress", email);
                cmd.AddParameter("Alias", email.Replace("@", "_"));
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // Dispose of Powershell
                powershell.Dispose();

                // Set the details
                Set_Mailbox(userPrincipalName, quotaWarning, maxReceiveSize, maxSendSize, companyCode, quotaProhibitSend, quotaProhibitSendReceive, recipientLimit, retainDeletedItems);

                // Set more details using CAS Mailbox
                Set_CASMailbox(userPrincipalName, companyCode, activeSyncEnabled, ecpEnabled, imapEnabled, mapiEnabled, owaEnabled, popEnabled, activeSyncPolicyName);

                // DEBUG //
                logger.Debug("Successfully enabled mailbox for [" + userPrincipalName + "] for company code " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");

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
        /// Sets details on the mailbox
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <param name="quotaWarning"></param>
        /// <param name="maxReceiveSize"></param>
        /// <param name="maxSendSize"></param>
        /// <param name="companyCode"></param>
        /// <param name="quotaProhibitSend"></param>
        /// <param name="quotaProhibitSendReceive"></param>
        /// <param name="recipientLimit"></param>
        /// <param name="retainDeletedItems"></param>
        public void Set_Mailbox(string userPrincipalName, int quotaWarning, int maxReceiveSize, int maxSendSize, string companyCode, int quotaProhibitSend, int quotaProhibitSendReceive,
                                int recipientLimit, int retainDeletedItems)
        {
            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Setting details on mailbox for user [" + userPrincipalName + "] for company code " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Enable the mailbox
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Set-Mailbox");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("EmailAddressPolicyEnabled", false);
                cmd.AddParameter("IssueWarningQuota", quotaWarning + "MB");
                cmd.AddParameter("MaxReceiveSize", maxReceiveSize + "KB");
                cmd.AddParameter("MaxSendSize", maxSendSize + "KB");
                cmd.AddParameter("OfflineAddressBook", companyCode + " OAL");
                cmd.AddParameter("ProhibitSendQuota", quotaProhibitSend + "MB");
                cmd.AddParameter("ProhibitSendReceiveQuota", quotaProhibitSendReceive + "MB");
                cmd.AddParameter("RecipientLimits", recipientLimit);
                cmd.AddParameter("RetainDeletedItemsFor", retainDeletedItems);
                cmd.AddParameter("UseDatabaseQuotaDefaults", false);
                cmd.AddParameter("UseDatabaseRetentionDefaults", false);
                cmd.AddParameter("RetainDeletedItemsUntilBackup", true);
                cmd.AddParameter("CustomAttribute1", companyCode);
                cmd.AddParameter("RoleAssignmentPolicy", "Alternate Assignment Policy");
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully set details on mailbox for [" + userPrincipalName + "] for company code " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
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
        /// Sets mailbox information (used for updating an existing mailbox)
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <param name="quotaWarning"></param>
        /// <param name="maxReceiveSize"></param>
        /// <param name="maxSendSize"></param>
        /// <param name="quotaProhibitSend"></param>
        /// <param name="quotaProhibitSendReceive"></param>
        /// <param name="recipientLimit"></param>
        /// <param name="retainDeletedItems"></param>
        /// <param name="emailAddresses"></param>
        /// <param name="forwardTo"></param>
        /// <param name="deliverToAndForward"></param>
        public void Set_Mailbox(string userPrincipalName, int quotaWarning, int maxReceiveSize, int maxSendSize, int quotaProhibitSend, int quotaProhibitSendReceive,
                                int recipientLimit, int retainDeletedItems, string[] emailAddresses, string forwardTo, bool deliverToAndForward)
        {
            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Setting details on mailbox for user [" + userPrincipalName + "]");
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Enable the mailbox
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Set-Mailbox");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("EmailAddressPolicyEnabled", false);
                cmd.AddParameter("IssueWarningQuota", quotaWarning + "MB");
                cmd.AddParameter("MaxReceiveSize", maxReceiveSize + "KB");
                cmd.AddParameter("MaxSendSize", maxSendSize + "KB");
                cmd.AddParameter("ProhibitSendQuota", quotaProhibitSend + "MB");
                cmd.AddParameter("ProhibitSendReceiveQuota", quotaProhibitSendReceive + "MB");
                cmd.AddParameter("RecipientLimits", recipientLimit);
                cmd.AddParameter("RetainDeletedItemsFor", retainDeletedItems);
                cmd.AddParameter("UseDatabaseQuotaDefaults", false);
                cmd.AddParameter("UseDatabaseRetentionDefaults", false);
                cmd.AddParameter("RetainDeletedItemsUntilBackup", true);
                cmd.AddParameter("RoleAssignmentPolicy", "Alternate Assignment Policy");
                cmd.AddParameter("EmailAddresses", emailAddresses);

                if (!string.IsNullOrEmpty(forwardTo))
                {
                    cmd.AddParameter("ForwardingAddress", forwardTo);
                    cmd.AddParameter("DeliverToMailboxAndForward", deliverToAndForward);
                }
                else
                {
                    cmd.AddParameter("ForwardingAddress", null);
                    cmd.AddParameter("DeliverToMailboxAndForward", false);
                }

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
        /// Sets more details on the CAS mailbox
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <param name="companyCode"></param>
        /// <param name="activeSyncEnabled"></param>
        /// <param name="ecpEnabled"></param>
        /// <param name="imapEnabled"></param>
        /// <param name="mapiEnabled"></param>
        /// <param name="owaEnabled"></param>
        /// <param name="POPEnabled"></param>
        public void Set_CASMailbox(string userPrincipalName, string companyCode, bool activeSyncEnabled, bool ecpEnabled, bool imapEnabled, bool mapiEnabled, bool owaEnabled, bool POPEnabled, string activeSyncPolicyName = null)
        {
            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Setting details on CAS mailbox for user [" + userPrincipalName + "] for company code " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Enable the mailbox
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Set-CASMailbox");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("ActiveSyncEnabled", activeSyncEnabled);
                cmd.AddParameter("ECPEnabled", ecpEnabled);
                cmd.AddParameter("ImapEnabled", imapEnabled);
                cmd.AddParameter("MAPIEnabled", mapiEnabled);
                cmd.AddParameter("OWAEnabled", owaEnabled);
                cmd.AddParameter("PopEnabled", POPEnabled);

                if (!string.IsNullOrEmpty(activeSyncPolicyName))
                    cmd.AddParameter("ActiveSyncMailboxPolicy", activeSyncPolicyName);
                else
                    cmd.AddParameter("ActiveSyncMailboxPolicy", null);

                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully set details on CAS mailbox for [" + userPrincipalName + "] for company code " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
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
        public BaseMailbox Get_Mailbox(string userPrincipalName)
        {
            // Our return object
            BaseMailbox mailbox = new BaseMailbox();

            try
            {
                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Getting mailbox for [" + userPrincipalName + "]");
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Get the Mailbox
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Get-Mailbox");
                cmd.AddParameter("Identity", userPrincipalName);
                cmd.AddParameter("DomainController", this.domainController);
                powershell.Commands = cmd;

                // Invoke and get returned information
                foreach (PSObject ps in powershell.Invoke())
                {
                    // Debug information
                    foreach (PSMemberInfo psmi in ps.Members)
                    {
                        // DEBUG //
                        if (psmi.Value == null)
                            this.logger.Debug("Value for [" + psmi.Name + "] was null.");
                        else
                            this.logger.Debug("Value for [" + psmi.Name + "] is " + psmi.Value.ToString());
                    }

                    mailbox.DisplayName = ps.Members["DisplayName"].Value.ToString();
                    mailbox.PrimarySmtpAddress = ps.Members["PrimarySmtpAddress"].Value.ToString();
                    mailbox.HiddenFromAddressListsEnabled = bool.Parse(ps.Members["HiddenFromAddressListsEnabled"].Value.ToString());
                    mailbox.DeliverToMailboxAndForward = bool.Parse(ps.Members["DeliverToMailboxAndForward"].Value.ToString());

                    if (ps.Members["ForwardingAddress"] != null)
                    {
                        mailbox.ForwardingAddress = ps.Members["ForwardingAddress"].Value.ToString();
                    }

                    // Format the quota
                    string quota = ps.Members["ProhibitSendReceiveQuota"].Value.ToString();

                    // Strip data because Exchange resturns something like this: 4.25GB (111,111,1111 bytes).
                    // We need the bytes
                    int first = quota.IndexOf("(");
                    int last = quota.IndexOf(")");
                    quota = quota.Substring(first, last - first);

                    // Remove characters
                    quota = quota.Replace("bytes", string.Empty);
                    quota = quota.Replace(",", string.Empty);
                    quota = quota.Replace(" ", string.Empty);
                    quota = quota.Replace(")", string.Empty);
                    quota = quota.Replace("(", string.Empty);

                    // Log the quota
                    this.logger.Debug("Formatted quota is now: " + quota + " for user " + mailbox.DisplayName);

                    // Convert from bytes to MB
                    decimal mb = decimal.Parse(quota);
                    mb = ((mb / 1024) / 1024);
                    mailbox.ProhibitSendReceiveQuota = decimal.Round(mb, 2, MidpointRounding.AwayFromZero);

                    // Get email addresses
                    mailbox.EmailAddresses = ps.Members["EmailAddresses"].Value.ToString().Split(' ');
                }

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully retrieved mailbox information for [" + userPrincipalName + "] in " + stopwatch.Elapsed.ToString() + " second(s)");

                // Return mailbox data
                return mailbox;
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
        public void New_DistributionGroup(string displayName, string companyCode, string emailAddress, string companyDN, string memberJoinRestriction, string memberDepartRestriction,
                                          bool moderationEnabled, string sendModerationNotifications, List<string> members, List<string> managedBy, List<string> moderatedBy, List<string> allowedSenders,
                                          List<string> bypassModerationSenders, bool isHidden, bool requireSenderAuthentication)
        {
            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                logger.Debug("Creating a new distribution group named [" + displayName + "] for company code " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Create SamAccountName
                string samAccountName = emailAddress.Replace("@", "_");
                if (samAccountName.Length > 19)
                    samAccountName = samAccountName.Substring(0, 18);

                // Create the distribution group
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("New-DistributionGroup");
                cmd.AddParameter("Name", displayName);
                cmd.AddParameter("Alias", emailAddress.Replace("@", "_"));
                cmd.AddParameter("SamAccountName", samAccountName);
                cmd.AddParameter("DisplayName", displayName);
                cmd.AddParameter("PrimarySmtpAddress", emailAddress);
                cmd.AddParameter("OrganizationalUnit", companyDN);
                cmd.AddParameter("MemberJoinRestriction", memberJoinRestriction);
                cmd.AddParameter("MemberDepartRestriction", memberDepartRestriction);
                cmd.AddParameter("ModerationEnabled", moderationEnabled);
                cmd.AddParameter("SendModerationNotifications", sendModerationNotifications);
                cmd.AddParameter("Members", members.ToArray());
                cmd.AddParameter("ManagedBy", managedBy.ToArray());
                cmd.AddParameter("DomainController", this.domainController);

                // Only add ModeratedBy if it was checked
                if (moderationEnabled)
                    cmd.AddParameter("ModeratedBy", moderatedBy.ToArray());
                else
                    cmd.AddParameter("ModeratedBy", null);

                powershell.Commands = cmd;
                powershell.Invoke();

                // Set the other options for the distribution group
                cmd = new PSCommand();
                cmd.AddCommand("Set-DistributionGroup");
                cmd.AddParameter("Identity", emailAddress.Replace("@", "_"));
                cmd.AddParameter("CustomAttribute1", companyCode);
                cmd.AddParameter("HiddenFromAddressListsEnabled", isHidden);
                cmd.AddParameter("RequireSenderAuthenticationEnabled", requireSenderAuthentication);
                cmd.AddParameter("DomainController", this.domainController);

                // Check to see if allowed senders is set or not
                if (allowedSenders != null && allowedSenders.Count > 0)
                    cmd.AddParameter("AcceptMessagesOnlyFromSendersOrMembers", allowedSenders.ToArray());
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
                logger.Debug("Error creating a new distribution group named [" + displayName + "] for company code " + companyCode + ". Path: " + companyDN, ex);

                throw;
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

                // Check to see if allowed senders is set or not
                if (allowedSenders != null && allowedSenders.Count > 0)
                    cmd.AddParameter("AcceptMessagesOnlyFromSendersOrMembers", allowedSenders.ToArray());
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
        public void New_ActiveSyncPolicy(BaseActivesyncPolicy policy, string exchangeVersion)
        {
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
        public void Update_ActiveSyncPolicy(BaseActivesyncPolicy policy, string exchangeVersion)
        {
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
        public BaseDistributionGroup Get_DistributionGroup(string groupEmail)
        {
            // Our return object
            BaseDistributionGroup group = new BaseDistributionGroup();
            
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
                    // Debug information
                    foreach (PSMemberInfo psmi in ps.Members)
                    {
                        // DEBUG //
                        if (psmi.Value == null)
                            this.logger.Debug("Value for [" + psmi.Name + "] was null.");
                        else
                            this.logger.Debug("Value for [" + psmi.Name + "] is " + psmi.Value.ToString());
                    }

                    // Check and make sure it is a user
                    group.JoinRestriction = ps.Members["MemberJoinRestriction"].Value.ToString();
                    group.DepartRestriction = ps.Members["MemberDepartRestriction"].Value.ToString();
                    group.CompanyCode = ps.Members["CustomAttribute1"].Value.ToString();
                    group.DisplayName = ps.Members["DisplayName"].Value.ToString();
                    group.Email = ps.Members["PrimarySmtpAddress"].Value.ToString();
                    group.Hidden = bool.Parse(ps.Members["HiddenFromAddressListsEnabled"].Value.ToString());
                    group.RequireSenderAuthentication = bool.Parse(ps.Members["RequireSenderAuthenticationEnabled"].Value.ToString());
                    group.SendModerationNotifications = ps.Members["SendModerationNotifications"].Value.ToString();
                    group.DistinguishedName = ps.Members["DistinguishedName"].Value.ToString();
                    group.ModerationEnabled = bool.Parse(ps.Members["ModerationEnabled"].Value.ToString());

                    // Get members of group
                    group.Members = new List<BaseUser>();
                    group.Members = Get_DistributionGroupMembers(groupEmail);

                    // Get managed by users (this is retrieves on the calling method)

                    // Get if it is restricted to only certain people
                    if (ps.Members["AcceptMessagesOnlyFrom"] != null)
                        group.RestrictWhoCanSend = ps.Members["AcceptMessagesOnlyFrom"].Value.ToString().Split(',');

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
        public List<BaseUser> Get_DistributionGroupMembers(string groupEmail)
        {
            // Our return object
            List<BaseUser> members = new List<BaseUser>();

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
                    // BEGIN DEBUG
                    foreach (PSMemberInfo psmi in ps.Members)
                    {
                        // DEBUG //
                        if (psmi.Value == null)
                            this.logger.Debug("Value for [" + psmi.Name + "] was null.");
                        else
                            this.logger.Debug("Value for [" + psmi.Name + "] is " + psmi.Value.ToString());
                    }

                    // Check and make sure it is a user
                    string recipientType = ps.Members["RecipientType"].Value.ToString();
                    if (recipientType == "UserMailbox" || recipientType.Contains("DistributionGroup"))
                    {
                        BaseUser tmp = new BaseUser();
                        tmp.CompanyCode = ps.Members["CustomAttribute1"].Value.ToString();
                        tmp.DisplayName = ps.Members["DisplayName"].Value.ToString();
                        tmp.Email = ps.Members["PrimarySmtpAddress"].Value.ToString();
                        tmp.DistinguishedName = ps.Members["DistinguishedName"].Value.ToString().ToUpper(); // Upper case for comparisons with other lists
                        tmp.UserGuid = Guid.Parse(ps.Members["Guid"].Value.ToString());
                        tmp.sAMAccountName = ps.Members["sAMAccountName"].Value.ToString();

                        // Set the object type
                        if (recipientType == "UserMailbox")
                            tmp.ObjectType = "User";
                        else
                            tmp.ObjectType = "Group";

                        if (ps.Members["FirstName"].Value != null)
                            tmp.Firstname = ps.Members["Firstname"].Value.ToString();

                        if (ps.Members["LastName"].Value != null)
                            tmp.Lastname = ps.Members["Lastname"].Value.ToString();

                        // Add our object
                        members.Add(tmp);
                    }
                }

                // Stop the clock
                stopwatch.Stop();

                // Check for errors
                CheckErrors(ref powershell);

                // DEBUG //
                logger.Debug("Successfully retrieved members for group [" + groupEmail + "] in " + stopwatch.Elapsed.ToString() + " second(s)");

                // Return list
                return members;
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
        /// Creates a new public folder for Exchange 2013
        /// </summary>
        /// <param name="companyCode"></param>
        public void New_PublicFolder(string companyCode)
        {
            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

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
        public void Set_PublicFolder(string companyCode, int issueWarningQuota, int prohibitPostQuota, int maxItemSize, string ageLimit, int retainDeletedItemsFor, bool perUserReadStateEnabled, int exchangeVersion)
        {
            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // DEBUG //
                this.logger.Debug("Setting information on public folder for " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Set-PublicFolder");
                cmd.AddParameter("Identity", @"\" + companyCode);

                if (!ageLimit.Equals("Unlimited"))
                    cmd.AddParameter("AgeLimit", ageLimit);

                cmd.AddParameter("IssueWarningQuota", issueWarningQuota + "MB");
                cmd.AddParameter("MaxItemSize", maxItemSize + "MB");
                cmd.AddParameter("PerUserReadStateEnabled", perUserReadStateEnabled);
                cmd.AddParameter("ProhibitPostQuota", prohibitPostQuota + "MB");
                cmd.AddParameter("RetainDeletedItemsFor", retainDeletedItemsFor);
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);

                // Exchange 2010 parameters only
                if (exchangeVersion == 2010)
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
                this.logger.Info("Successfully created a set public folder for " + companyCode + " in " + stopwatch.Elapsed.ToString() + " second(s)");
            }
            catch (Exception ex)
            {
                // FATAL //
                this.logger.Fatal("Failed to set public folder for " + companyCode, ex);

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
            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Adding public folder permissions for " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                // Add the public folder owners
                PSCommand cmd = new PSCommand();
                cmd.AddCommand("Add-PublicFolderClientPermission");
                cmd.AddParameter("Identity", @"\" + companyCode);
                cmd.AddParameter("User", "PublicFolderAdmins@" + companyCode);
                cmd.AddParameter("AccessRights", "Owner");
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

                // Add the public folder users
                cmd = new PSCommand();
                cmd.AddCommand("Add-PublicFolderClientPermission");
                cmd.AddParameter("Identity", @"\" + companyCode);
                cmd.AddParameter("User", "PublicFolderUsers@" + companyCode);
                cmd.AddParameter("AccessRights", "Reviewer");
                cmd.AddParameter("Confirm", false);
                cmd.AddParameter("DomainController", domainController);
                powershell.Commands = cmd;
                powershell.Invoke();

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
        public void Remove_PublicFolderDefaultPermissions(string companyCode, int exchangeVersion)
        {
            try
            {
                // Strip whitespace from company code
                companyCode = companyCode.Replace(" ", string.Empty);

                // Start clock
                Stopwatch stopwatch = Stopwatch.StartNew();

                // INFO //
                this.logger.Info("Removing default public folder permissions for " + companyCode);
                
                // Run commands
                powershell = PowerShell.Create();
                powershell.Runspace = runspace;

                PSCommand cmd = new PSCommand();

                if (exchangeVersion == 2010)
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
                else if (exchangeVersion == 2013)
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
        private void CheckErrors(ref PowerShell errorCheck, bool skipCouldntBeFound = false)
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
            wsinfo.OpenTimeout = 1 * 60 * 1000;
            wsinfo.OperationTimeout = 4 * 60 * 1000;
            
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
                    if (powershell != null)
                        powershell.Dispose();

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
