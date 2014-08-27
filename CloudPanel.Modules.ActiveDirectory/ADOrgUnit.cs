using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudPanel.Modules.Base;
using System.DirectoryServices;
using CloudPanel.Modules.Settings;
using System.DirectoryServices.AccountManagement;
using System.Security.AccessControl;
using log4net;
using System.Reflection;
using System.Security.Principal;
using System.Collections;
using CloudPanel.Modules.Base.Class;

namespace CloudPanel.Modules.ActiveDirectory
{
    public class ADOrgUnit
    {
        // Disposing information
        private bool disposed = false;

        // Information for connecting
        private string username;
        private string password;
        private string domainController;

        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ADOrgUnit(string username, string password, string domainController)
        {
            this.username = username;
            this.password = password;
            this.domainController = domainController;
        }

        #region Active Directory Only Related Commands

        /// <summary>
        /// Checks if the OU exists or not
        /// </summary>
        /// <param name="orgUnitDN"></param>
        /// <returns></returns>
        public Company GetOU(string orgUnitDN)
        {
            DirectoryEntry de = null;

            try
            {
                Company company = new Company();

                // Connect to AD
                de = new DirectoryEntry("LDAP://" + this.domainController + "/" + orgUnitDN, this.username, this.password);

                // Retrieve attributes
                company.ObjectGuid  = Guid.Parse(de.Properties["objectGuid"].Value.ToString());
                company.CompanyName = de.Properties["name"].Value.ToString();
                company.CompanyCode = de.Properties["name"].Value.ToString();
                company.DistinguishedName = de.Properties["distinguishedName"].Value.ToString();

                if (de.Properties["uPNSuffixes"] != null && de.Properties["uPNSuffixes"].Count > 0)
                {
                    List<string> domains = new List<string>();

                    for (int i = 0; i < de.Properties["uPNSuffixes"].Count; i++)
                    {
                        if (!string.IsNullOrEmpty(de.Properties["uPNSuffixes"][i].ToString()))
                            domains.Add(de.Properties["uPNSuffixes"][i].ToString());
                    }

                    // Set company domains
                    company.Domains = domains;
                }

                // If we got here then the OU exist because it didn't throw an error
                return company;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (de != null)
                    de.Dispose();
            }
        }

        /// <summary>
        /// Gets a list of all child OU's
        /// </summary>
        /// <param name="orgUnitDN"></param>
        /// <returns></returns>
        public List<Company> GetChildOUs(string orgUnitDN)
        {
            DirectoryEntry de = null;
            DirectorySearcher ds = null;

            try
            {
                List<Company> foundChildOUs = new List<Company>();

                // DEBUG //
                this.logger.Debug("Checking for child OUs in " + orgUnitDN);

                // Connect to AD
                de = new DirectoryEntry("LDAP://" + this.domainController + "/" + orgUnitDN, this.username, this.password);
                ds = new DirectorySearcher(de);
                ds.Filter = "(objectCategory=organizationalUnit)";

                foreach (SearchResult sr in ds.FindAll())
                {
                    Company company = new Company();
                   
                    // Create our DirectoryEntry object
                    DirectoryEntry found = sr.GetDirectoryEntry();
                    company.CompanyCode = found.Properties["name"].Value.ToString();
                    company.DistinguishedName = found.Properties["DistinguishedName"].Value.ToString();

                    if (found.Properties["DisplayName"].Value != null)
                        company.CompanyName = found.Properties["DisplayName"].Value.ToString();
                    else
                        company.CompanyName = company.CompanyCode;

                    // Add to our list but not if it is the parent ou
                    if (!company.DistinguishedName.Equals(orgUnitDN, StringComparison.CurrentCultureIgnoreCase))
                        foundChildOUs.Add(company);
                }

                // Return list of child OU's
                return foundChildOUs;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (ds != null)
                    ds.Dispose();

                if (de != null)
                    de.Dispose();
            }
        }

        /// <summary>
        /// Adds a domain to the list
        /// </summary>
        /// <param name="parentOrgUnit"></param>
        /// <param name="domainName"></param>
        public void AddDomain(string parentOrgUnit, string domainName)
        {
            DirectoryEntry de = null;

            try
            {
                de = new DirectoryEntry("LDAP://" + this.domainController + "/" + parentOrgUnit, this.username, this.password);
                de.Properties["uPNSuffixes"].Add(domainName);

                // Commit all the changes to the OU
                de.CommitChanges();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (de != null)
                    de.Dispose();
            }
        }

        /// <summary>
        /// Removes a domain from the OU
        /// </summary>
        /// <param name="parentOrgUnit"></param>
        /// <param name="domainName"></param>
        public void RemoveDomain(string parentOrgUnit, string domainName)
        {
            DirectoryEntry de = null;

            try
            {
                de = new DirectoryEntry("LDAP://" + this.domainController + "/" + parentOrgUnit, this.username, this.password);
                de.Properties["uPNSuffixes"].Remove(domainName);

                // Commit all the changes to the OU
                de.CommitChanges();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (de != null)
                    de.Dispose();
            }
        }

        /// <summary>
        /// Creates a new organizational unit
        /// </summary>
        /// <param name="parentOrgUnit">The OU to add the new organizational unit to</param>
        /// <param name="companyInfo">Company class containing information about the organizational unit to add</param>
        public void CreateOU(string parentOrgUnit, Company companyInfo)
        {
            DirectoryEntry de = null;
            DirectoryEntry newOrg = null;

            try
            {
                de = new DirectoryEntry("LDAP://" + this.domainController + "/" + parentOrgUnit, this.username, this.password);

                // Add organizational unit
                newOrg = de.Children.Add("OU=" + companyInfo.CompanyCode, "OrganizationalUnit");

                // Set additional information
                newOrg.Properties["description"].Add(companyInfo.CompanyName);
                newOrg.Properties["displayName"].Add(companyInfo.CompanyName);

                // These values may not be set so only set if they are valid
                if (!string.IsNullOrEmpty(companyInfo.Street))
                    newOrg.Properties["street"].Add(companyInfo.Street);

                if (!string.IsNullOrEmpty(companyInfo.City))
                    newOrg.Properties["l"].Add(companyInfo.City);

                if (!string.IsNullOrEmpty(companyInfo.State))
                    newOrg.Properties["st"].Add(companyInfo.State);

                if (!string.IsNullOrEmpty(companyInfo.ZipCode))
                    newOrg.Properties["postalCode"].Add(companyInfo.ZipCode);

                if (!string.IsNullOrEmpty(companyInfo.PhoneNumber))
                    newOrg.Properties["telephoneNumber"].Add(companyInfo.PhoneNumber);

                if (!string.IsNullOrEmpty(companyInfo.Website))
                    newOrg.Properties["wWWHomePage"].Add(companyInfo.Website);

                if (!string.IsNullOrEmpty(companyInfo.AdministratorsName))
                    newOrg.Properties["adminDisplayName"].Add(companyInfo.AdministratorsName);

                if (!string.IsNullOrEmpty(companyInfo.AdministratorsEmail))
                    newOrg.Properties["adminDescription"].Add(companyInfo.AdministratorsEmail);

                // Now loop through the domainsand see if there are any (reseller OU's won't have any)
                if (companyInfo.Domains != null && companyInfo.Domains.Count > 0)
                {
                    foreach (string upn in companyInfo.Domains)
                    {
                        if (!string.IsNullOrEmpty(upn))
                        {
                            newOrg.Properties["uPNSuffixes"].Add(upn);
                        }
                    }
                }

                // Commit all the changes to the new OU
                newOrg.CommitChanges();

                // Remove Authenticated Users Rights
                string formattedOU = newOrg.Path.Replace("LDAP://" + this.domainController + "/", "");
                RemoveAuthUsersRights(formattedOU);

                // Commit the changes to the parent OU
                de.CommitChanges();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (newOrg != null)
                    newOrg.Dispose();

                if (de != null)
                    de.Dispose();
            }
        }

        /// <summary>
        /// Creates a organizational unit with no extra values or attributes
        /// </summary>
        /// <param name="parentOrgUnit"></param>
        /// <param name="OUName"></param>
        public void CreateOU(string parentOrgUnit, string OUName)
        {
            DirectoryEntry de = null;
            DirectoryEntry newOrg = null;

            try
            {
                de = new DirectoryEntry("LDAP://" + this.domainController + "/" + parentOrgUnit, this.username, this.password);

                // See if the OU already exists
                bool alreadyExists = false;

                DirectoryEntries children = de.Children;
                if (children != null)
                {
                    foreach (DirectoryEntry child in children)
                    {
                        if (child.Name.Equals(OUName, StringComparison.CurrentCultureIgnoreCase))
                            alreadyExists = true;
                    }
                }

                if (!alreadyExists)
                {
                    // Create new OU
                    newOrg = de.Children.Add("OU=" + OUName, "OrganizationalUnit");

                    // Save changes
                    newOrg.CommitChanges();

                    // SAve changes
                    de.CommitChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (newOrg != null)
                    newOrg.Dispose();

                if (de != null)
                    de.Dispose();
            }
        }

        /// <summary>
        /// Deletes the OU and everything in the OU.
        /// **** DESTRUCTIVE!!! ****
        /// </summary>
        /// <param name="distinguishedName"></param>
        public void DeleteOUAll(string distinguishedName)
        {
            DirectoryEntry de = null;

            try
            {
                de = new DirectoryEntry("LDAP://" + this.domainController + "/" + distinguishedName, this.username, this.password);
                
                // Delete the OU
                de.DeleteTree();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (de != null)
                    de.Dispose();
            }
        }

        /// <summary>
        /// Grants the group red and list object rights but denies list content
        /// </summary>
        /// <param name="ouPath"></param>
        /// <param name="groupName"></param>
        public void AddGPOAccessRights(string ouPath, string groupName)
        {
            DirectoryEntry de = null;
            PrincipalContext pc = null;
            GroupPrincipal gp = null;

            try
            {
                de = new DirectoryEntry("LDAP://" + this.domainController + "/" + ouPath, this.username, this.password);
                pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password);

                // Find the group
                gp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, groupName.Replace(" ", string.Empty));
                if (gp == null)
                    throw new Exception("Unable to find the group " + groupName + " in Active Directory");
                else
                {
                    // Add Read Property
                    de.ObjectSecurity.AddAccessRule(new ActiveDirectoryAccessRule(gp.Sid, ActiveDirectoryRights.ReadProperty, AccessControlType.Allow));

                    // Add List Object
                    de.ObjectSecurity.AddAccessRule(new ActiveDirectoryAccessRule(gp.Sid, ActiveDirectoryRights.ListObject, AccessControlType.Allow));

                    // Deny List Content
                    de.ObjectSecurity.AddAccessRule(new ActiveDirectoryAccessRule(gp.Sid, ActiveDirectoryRights.ListChildren, AccessControlType.Deny));

                    de.CommitChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (gp != null)
                    gp.Dispose();

                if (pc != null)
                    pc.Dispose();

                if (de != null)
                    de.Dispose();
            }
        }

        /// <summary>
        /// Removes authenticated users from the OU
        /// </summary>
        /// <param name="oupath"></param>
        /// <param name="taskId"></param>
        public void RemoveAuthUsersRights(string oupath)
        {
            using (DirectoryEntry de = new DirectoryEntry("LDAP://" + this.domainController + "/" + oupath, this.username, this.password))
            {
                try
                {
                    AuthorizationRuleCollection arc = de.ObjectSecurity.GetAccessRules(true, true, typeof(NTAccount));
                    foreach (ActiveDirectoryAccessRule adar in arc)
                    {
                        if (adar.IdentityReference.Value.Equals("NT AUTHORITY\\AUTHENTICATED USERS", StringComparison.CurrentCultureIgnoreCase))
                        {
                            bool modified = false;
                            de.ObjectSecurity.ModifyAccessRule(AccessControlModification.RemoveAll, adar, out modified);
                            de.CommitChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Add read rights to the OU
        /// </summary>
        /// <param name="oupath"></param>
        /// <param name="groupName"></param>
        public void AddReadRights(string oupath, string groupName)
        {
            using (DirectoryEntry de = new DirectoryEntry("LDAP://" + this.domainController + "/" + oupath, this.username, this.password))
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password))
                {
                    try
                    {
                        GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, groupName.Replace(" ", string.Empty));
                        if (gp == null)
                            throw new Exception("Unable to find the group or user specified.");
                        else
                        {
                            ActiveDirectoryAccessRule ar = new ActiveDirectoryAccessRule(gp.Sid, ActiveDirectoryRights.GenericRead, AccessControlType.Allow);
                            de.ObjectSecurity.AddAccessRule(ar);
                            de.CommitChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Removes rights from the OU
        /// </summary>
        /// <param name="oupath"></param>
        /// <param name="accountname"></param>
        /// <param name="taskId"></param>
        public void RemoveRights(string oupath, string accountname, int taskId = 0)
        {
            using (DirectoryEntry de = new DirectoryEntry("LDAP://" + this.domainController + "/" + oupath, this.username, this.password))
            {
                try
                {
                    AuthorizationRuleCollection arc = de.ObjectSecurity.GetAccessRules(true, true, typeof(NTAccount));
                    foreach (ActiveDirectoryAccessRule adar in arc)
                    {
                        if (adar.IdentityReference.Value.Equals(accountname, StringComparison.CurrentCultureIgnoreCase))
                        {
                            bool modified = false;

                            de.ObjectSecurity.ModifyAccessRule(AccessControlModification.RemoveAll, adar, out modified);
                            de.CommitChanges();
                        }
                    }

                    arc = null;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        #endregion

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
                    username = null;
                    password = null;
                    domainController = null;
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
