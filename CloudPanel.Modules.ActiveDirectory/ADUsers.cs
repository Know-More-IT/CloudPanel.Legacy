using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;
using CloudPanel.Modules.Settings;
using log4net;
using System.Reflection;
using CloudPanel.Modules.Base;
using System.DirectoryServices;
using System.Text.RegularExpressions;
using CloudPanel.Modules.Base.Class;

namespace CloudPanel.Modules.ActiveDirectory
{
    public class ADUsers : IDisposable
    {
        // Disposing information
        private bool disposed = false;

        // Information for connecting
        private string username;
        private string password;
        private string domainController;

        // Logger
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public enum Attributes
        {
            msRTCSIPGroupingID,
            thumbnailPhoto
        }

        /// <summary>
        /// Create a new Users object instance
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="domainController"></param>
        public ADUsers(string username, string password, string domainController)
        {
            this.username = username;
            this.password = password;
            this.domainController = domainController;

            // DEBUG
            this.logger.Debug("Opened ADUser object with these values: " + username + ", " + domainController);
        }

        /// <summary>
        /// Authenticates the user against Active Directory
        /// </summary>
        /// <param name="username">Username (UserPrincipalName)</param>
        /// <param name="password">Password for user</param>
        /// <returns></returns>
        public bool Authenticate(string username, string password, string[] superAdminGroups, out bool isSuperAdmin)
        {
            PrincipalContext pc = null;
            UserPrincipal up = null;

            try
            {
                pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password);
                
                // Try to authenticate
                bool authenticated = pc.ValidateCredentials(username, password);

                if (!authenticated)
                {
                    isSuperAdmin = false;
                    return false;
                }
                else
                {
                    // Log that user authenticated
                    this.logger.Debug("User " + username + " successfully authenticated. Now checking to see if the user a super administrator.");

                    // Find the User
                    up = UserPrincipal.FindByIdentity(pc, IdentityType.UserPrincipalName, username);

                    if (up == null)
                        throw new Exception("Unable to find the user " + username + ". Please make sure you entered the correct login of the user.");
                    else
                    {
                        // Find super admin
                        bool foundSuperAdmin = false;

                        // Convert to directory entry
                        DirectoryEntry de = up.GetUnderlyingObject() as DirectoryEntry;
                        foreach (var dn in de.Properties["memberOf"])
                        {
                            foreach (string s in superAdminGroups)
                            {
                                if (dn.ToString().ToLower().Contains(s.ToLower().Trim()))
                                {
                                    foundSuperAdmin = true;

                                    // Log that the user is a super administrator
                                    this.logger.Debug("User " + username + " was found to be a super administrator.");

                                    break;
                                }
                            }
                        }

                        // Return true for authenticated
                        isSuperAdmin = foundSuperAdmin;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // ERROR //
                this.logger.Fatal("Error authenticating user " + username, ex);

                throw;
            }
            finally
            {
                if (up != null)
                    up.Dispose();

                if (pc != null)
                    pc.Dispose();
            }
        }

        /// <summary>
        /// Authenticates a user with Active Directory
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Authenticate(string userName, string userPassword)
        {
            PrincipalContext pc = null;

            try
            {
                pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password);

                // Try to authenticate
                bool authenticated = pc.ValidateCredentials(userName, userPassword);

                if (!authenticated)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (pc != null)
                    pc.Dispose();
            }
        }

        /// <summary>
        /// Gets a list of groups the user belongs to the user
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public List<string> GetGroups(string userPrincipalName)
        {
            PrincipalContext pc = null;
            UserPrincipal up = null;

            List<string> groups = new List<string>();
            try
            {
                pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password);

                // Find the User
                up = UserPrincipal.FindByIdentity(pc, IdentityType.UserPrincipalName, userPrincipalName);
                
                // Get groups
                foreach (Principal p in up.GetGroups())
                    groups.Add(p.Name);

                // Return list
                return groups;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (up != null)
                    up.Dispose();

                if (pc != null)
                    pc.Dispose();
            }
        }

        /// <summary>
        /// Creates a new user in Active Directory
        /// </summary>
        /// <param name="user"></param>
        /// <param name="parentOU"></param>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        public ADUser Create(ADUser user, string parentOU, string userPassword)
        {
            PrincipalContext pc = null;
            UserPrincipalExt up = null;

            // Groups object so we can add user to group
            ADGroups groups = new ADGroups(this.username, this.password, this.domainController);

            try
            {
                // DEBUG
                this.logger.Debug("Attempting to create new user " + user.UserPrincipalName + " on path " + parentOU);

                pc = new PrincipalContext(ContextType.Domain, this.domainController, parentOU, this.username, this.password);

                // Check if user exist
                up = UserPrincipalExt.FindByIdentity(pc, IdentityType.UserPrincipalName, user.UserPrincipalName);
                if (up == null)
                {
                    // Format the sAMAccountName with the first part of their login name
                    user.SamAccountName = user.UserPrincipalName.Split('@')[0];

                    // Check the length of the sAMAccountName (cannot exceed 19 characters)
                    if (user.SamAccountName.Length > 19)
                    {
                        user.SamAccountName = user.SamAccountName.Substring(0, 18);
                        this.logger.Debug("User's sAMAccountName was to long and had to be shortened to: " + user.SamAccountName);
                    }

                    // Now we need to make sure the sAMAccountName isn't already taken. If it is we will append a number
                    string finalSamAccountName = user.SamAccountName;
                    for (int i = 1; i < 1000; i++)
                    {
                        if (DoesSamAccountNameExist(finalSamAccountName))
                        {
                            // DEBUG
                            this.logger.Debug("SamAccountName " + finalSamAccountName + " is already in use. Trying to find another account name...");

                            finalSamAccountName = user.SamAccountName + i.ToString(); // We found a match so we need to increment the number

                            // Make sure the SamAccountName is less than 19 characters
                            if (finalSamAccountName.Length > 19)
                            {
                                finalSamAccountName = finalSamAccountName.Substring(0, 18 - i.ToString().Length) + i.ToString(); // Make sure it isn't above 19 characters
                                this.logger.Debug("New SamAccountName was too long and was trimmed to " + finalSamAccountName);
                            }
                        }
                        else
                        {
                            // No match found which means we can continue! Set the new value and break out of the loop!
                            user.SamAccountName = finalSamAccountName;

                            // DEBUG //
                            this.logger.Debug("Found samAccountName not in use: " + user.SamAccountName);

                            break;
                        }
                    }

                    // Create the user
                    up = new UserPrincipalExt(pc, user.SamAccountName, userPassword, true);

                    // Set values
                    if (string.IsNullOrEmpty(user.Name))
                        up.Name = user.UserPrincipalName;
                    else
                        up.Name = user.Name; // User is using a custom name attribute

                    up.UserPrincipalName = user.UserPrincipalName;
                    up.DisplayName = user.DisplayName;

                    if (up.PasswordNeverExpires != null)
                        up.PasswordNeverExpires = user.PasswordNeverExpires;

                    if (!string.IsNullOrEmpty(user.Firstname))
                        up.GivenName = user.Firstname;

                    if (!string.IsNullOrEmpty(user.Middlename))
                        up.MiddleName = user.Middlename;

                    if (!string.IsNullOrEmpty(user.Lastname))
                        up.Surname = user.Lastname;

                    if (!string.IsNullOrEmpty(user.Department))
                        up.Department = user.Department;

                    // Save user
                    up.Save(pc);

                    // DEBUG
                    this.logger.Debug("Successfully created new user " + user.UserPrincipalName + ". Now trying to modify security groups...");

                    // Get other information
                    user.DistinguishedName = up.DistinguishedName;
                    user.UserGuid = (Guid)up.Guid;

                    // Now lets add the user to the AllUsers@ group
                    groups.ModifyMembership("AllUsers@" + user.CompanyCode.Replace(" ", string.Empty), user.UserPrincipalName, IdentityType.Name, IdentityType.UserPrincipalName, false);

                    // Now lets check if the user is an administrator or not and add to that group
                    if (user.IsCompanyAdmin)
                    {
                        groups.ModifyMembership("Admins@" + user.CompanyCode.Replace(" ", string.Empty), user.UserPrincipalName, IdentityType.Name, IdentityType.UserPrincipalName, false, false);
                    }

                    // Return user object
                    return user;
                }
                else
                    throw new Exception("We couldn't create the user " + user.UserPrincipalName + " because the user already exists in Active Directory.");

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                groups.Dispose();

                if (up != null)
                    up.Dispose();

                if (pc != null)
                    pc.Dispose();
            }
        }

        /// <summary>
        /// Gets a list of all users in a particular OU
        /// </summary>
        /// <param name="parentOU"></param>
        /// <returns></returns>
        public List<ADUser> GetAllUsers(string parentOU, List<string> userPrincipalNames = null, bool includeUserPrincipalNames = false)
        {
            PrincipalContext pc = null;
            PrincipalSearcher pcSearch = null;
            UserPrincipalExt up = null;

            // Groups object so we can add user to group
            ADGroups groups = new ADGroups(this.username, this.password, this.domainController);

            try
            {
                // Initialize our return object
                List<ADUser> foundUsers = new List<ADUser>();

                // Initialize the principal context
                pc = new PrincipalContext(ContextType.Domain, this.domainController, parentOU, this.username, this.password);

                // Our UserPrincipalObject
                up = new UserPrincipalExt(pc);

                // Create a search
                pcSearch = new PrincipalSearcher(up);

                // Now find all matches
                foreach (var found in pcSearch.FindAll())
                {
                    // Cast our object
                    UserPrincipalExt uTmp = found as UserPrincipalExt;
                    ADUser tmpUser = new ADUser();

                    if (userPrincipalNames != null && !includeUserPrincipalNames)
                    {
                        // We are getting all users but exluding the users that are in the List<string> collection
                        if (!userPrincipalNames.Contains(found.UserPrincipalName))
                        {
                            tmpUser.UserGuid = (Guid)uTmp.Guid;
                            tmpUser.UserPrincipalName = uTmp.UserPrincipalName;
                            tmpUser.SamAccountName = uTmp.SamAccountName;
                            tmpUser.DisplayName = uTmp.DisplayName;
                            tmpUser.DistinguishedName = uTmp.DistinguishedName;

                            if (!string.IsNullOrEmpty(uTmp.GivenName))
                                tmpUser.Firstname = uTmp.GivenName;
                            else
                                tmpUser.Firstname = "";

                            if (!string.IsNullOrEmpty(uTmp.MiddleName))
                                tmpUser.Middlename = uTmp.MiddleName;
                            else
                                tmpUser.Middlename = "";

                            if (!string.IsNullOrEmpty(uTmp.Surname))
                                tmpUser.Lastname = uTmp.Surname;
                            else
                                tmpUser.Lastname = "";

                            if (!string.IsNullOrEmpty(uTmp.Department))
                                tmpUser.Department = uTmp.Department;
                            else
                                tmpUser.Department = "";

                            // Add to our list
                            foundUsers.Add(tmpUser);
                        }
                    }
                    else if (userPrincipalNames != null && includeUserPrincipalNames)
                    {
                        // We are getting all users but ONLY INCLUDING the users that are in the List<string> collection
                        if (userPrincipalNames.Contains(found.UserPrincipalName))
                        {
                            tmpUser.UserGuid = (Guid)uTmp.Guid;
                            tmpUser.UserPrincipalName = uTmp.UserPrincipalName;
                            tmpUser.SamAccountName = uTmp.SamAccountName;
                            tmpUser.DisplayName = uTmp.DisplayName;
                            tmpUser.DistinguishedName = uTmp.DistinguishedName;

                            if (!string.IsNullOrEmpty(uTmp.GivenName))
                                tmpUser.Firstname = uTmp.GivenName;
                            else
                                tmpUser.Firstname = "";

                            if (!string.IsNullOrEmpty(uTmp.MiddleName))
                                tmpUser.Middlename = uTmp.MiddleName;
                            else
                                tmpUser.Middlename = "";

                            if (!string.IsNullOrEmpty(uTmp.Surname))
                                tmpUser.Lastname = uTmp.Surname;
                            else
                                tmpUser.Lastname = "";

                            if (!string.IsNullOrEmpty(uTmp.Department))
                                tmpUser.Department = uTmp.Department;
                            else
                                tmpUser.Department = "";

                            // Add to our list
                            foundUsers.Add(tmpUser);
                        }
                    }
                    else
                    {
                        // We are getting all users regardless of what is included or excluded
                        tmpUser.UserGuid = (Guid)uTmp.Guid;
                        tmpUser.UserPrincipalName = uTmp.UserPrincipalName;
                        tmpUser.SamAccountName = uTmp.SamAccountName;
                        tmpUser.DisplayName = uTmp.DisplayName;
                        tmpUser.DistinguishedName = uTmp.DistinguishedName;

                        if (!string.IsNullOrEmpty(uTmp.GivenName))
                            tmpUser.Firstname = uTmp.GivenName;
                        else
                            tmpUser.Firstname = "";

                        if (!string.IsNullOrEmpty(uTmp.MiddleName))
                            tmpUser.Middlename = uTmp.MiddleName;
                        else
                            tmpUser.Middlename = "";

                        if (!string.IsNullOrEmpty(uTmp.Surname))
                            tmpUser.Lastname = uTmp.Surname;
                        else
                            tmpUser.Lastname = "";

                        if (!string.IsNullOrEmpty(uTmp.Department))
                            tmpUser.Department = uTmp.Department;
                        else
                            tmpUser.Department = "";

                        // Add to our list
                        foundUsers.Add(tmpUser);
                    }
                }

                return foundUsers;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                groups.Dispose();

                if (up != null)
                    up.Dispose();

                if (pcSearch != null)
                    pcSearch.Dispose();

                if (pc != null)
                    pc.Dispose();
            }
        }

        /// <summary>
        /// Checks if a sAMAccountName already exists or not
        /// </summary>
        /// <param name="sAMAccountName"></param>
        /// <returns></returns>
        public bool DoesSamAccountNameExist(string sAMAccountName)
        {
            PrincipalContext pc = null;
            UserPrincipal up = null;
            GroupPrincipal gp = null;

            // Groups object so we can add user to group
            ADGroups groups = new ADGroups(this.username, this.password, this.domainController);

            try
            {
                // DEBUG
                this.logger.Debug("Checking to see if sAMAccountName " + sAMAccountName + " exists...");

                pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password);
                
                // Check if user exist
                up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, sAMAccountName);
                gp = GroupPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, sAMAccountName);
                if (up == null && gp == null)
                {
                    // DEBUG
                    this.logger.Debug("sAMAccountName " + sAMAccountName + " does not exist.");

                    return false;
                }
                else
                {
                    // DEBUG
                    this.logger.Warn("sAMAccountName " + sAMAccountName + " exists!!");

                    return true;
                }
            }
            catch (Exception ex)
            {
                this.logger.Fatal("Error was thrown checking if a sAMAccountName existed", ex);

                throw;
            }
            finally
            {
                if (up != null)
                    up.Dispose();

                if (pc != null)
                    pc.Dispose();
            }
        }

        /// <summary>
        /// Edits a user in Active Directory
        /// </summary>
        /// <param name="user"></param>
        public void Edit(ADUser user)
        {
            PrincipalContext pc = null;
            UserPrincipalExt up = null;
            GroupPrincipal gp = null;

            // Reseller Admin Group
            GroupPrincipal rgp = null;
            DirectoryEntry deUser = null;
            ADGroups group = null;

            try
            {
                // Initialize
                pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password);

                // Find the User
                up = UserPrincipalExt.FindByIdentity(pc, IdentityType.UserPrincipalName, user.UserPrincipalName);

                // Find the Admin group
                gp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, "Admins@" + user.CompanyCode.Replace(" ", string.Empty));

                // Find the Reseller group
                rgp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, "ResellerAdmins@" + user.ResellerCode);          
                
                // Update the user
                up.GivenName = user.Firstname;
                up.DisplayName = user.DisplayName;
                up.Enabled = user.IsEnabled;
                up.PasswordNeverExpires = user.PasswordNeverExpires;

                if (!string.IsNullOrEmpty(user.Middlename))
                    up.MiddleName = user.Middlename;
                else
                    up.MiddleName = null;

                if (!string.IsNullOrEmpty(user.Lastname))
                    up.LastName = user.Lastname;
                else
                    up.LastName = null;

                if (!string.IsNullOrEmpty(user.Department))
                    up.Department = user.Department;
                else
                    up.Department = null;

                if (!string.IsNullOrEmpty(user.Password))
                    up.SetPassword(user.Password);

                // Check if they were added or removed from Company Admins
                if (user.IsCompanyAdmin && !up.IsMemberOf(gp))
                {
                    gp.Members.Add(up);
                }
                else if (!user.IsCompanyAdmin && up.IsMemberOf(gp))
                {
                    gp.Members.Remove(up);
                }

                // Check that the reseller group isn't null
                // If it is then create the group
                if (rgp == null)
                {
                    // Get the users underlying object
                    deUser = up.GetUnderlyingObject() as DirectoryEntry;

                    // Get the users parent ou
                    string parentOU = deUser.Parent.Parent.Properties["DistinguishedName"].Value.ToString();

                    // Initialize the group object
                    group = new ADGroups(this.username, this.password, this.domainController);

                    // Create the group
                    group.Create(parentOU, "ResellerAdmins@" + user.ResellerCode, "", true, ADGroups.GroupType.Global);

                    // Find the group again
                    rgp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, "ResellerAdmins@" + user.ResellerCode); 
                }

                // Check if they were added or removed from Reseller Admins
                if (user.IsResellerAdmin && !up.IsMemberOf(rgp))
                {
                    rgp.Members.Add(up);
                }
                else if (!user.IsResellerAdmin && up.IsMemberOf(rgp))
                {
                    rgp.Members.Remove(up);
                }

                // Save
                rgp.Save();
                gp.Save();
                up.Save();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (group != null)
                    group.Dispose();

                if (deUser != null)
                    deUser.Dispose();

                if (rgp != null)
                    rgp.Dispose();

                if (gp != null)
                    gp.Dispose();

                if (up != null)
                    up.Dispose();

                if (pc != null)
                    pc.Dispose();
            }
        }

        /// <summary>
        /// Resets a user's password
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool ResetPassword(string userPrincipalName, string newPassword)
        {
            PrincipalContext pc = null;
            UserPrincipal up = null;

            try
            {
                pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password);

                // Find the user
                up = UserPrincipal.FindByIdentity(pc, IdentityType.UserPrincipalName, userPrincipalName);
                if (up == null)
                    throw new Exception("User was not found in Active Directory");
                else
                {
                    up.SetPassword(newPassword);
                    up.Save();

                    // DEBUG
                    this.logger.Debug("Successfully reset user " + userPrincipalName + "'s password.");

                    // Return true
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (up != null)
                    up.Dispose();

                if (pc != null)
                    pc.Dispose();
            }
        }
        
        /// <summary>
        /// Deletes a user from active directory
        /// </summary>
        /// <param name="userPrincipalName"></param>
        public void Delete(string userPrincipalName)
        {
            PrincipalContext pc = null;
            UserPrincipalExt up = null;
            DirectoryEntry de = null;

            try
            {
                // DEBUG //
                this.logger.Warn("Attempting to delete user " + userPrincipalName + " from Active Directory.");

                pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password);

                // Find the User
                up = UserPrincipalExt.FindByIdentity(pc, IdentityType.UserPrincipalName, userPrincipalName);

                // Check if user exists
                if (up != null)
                {
                    // Convert to directory entry
                    de = up.GetUnderlyingObject() as DirectoryEntry;

                    // Delete user
                    de.DeleteTree();

                    // DEBUG //
                    this.logger.Warn("User " + userPrincipalName + " was successfully deleted from Active Directory.");
                }
                else
                    this.logger.Debug("User " + userPrincipalName + " does not exist when it was trying to be deleted from Active Directory.");
            }
            catch (Exception ex)
            {
                // DEBUG //
                this.logger.Fatal("Error deleting user " + userPrincipalName + " from Active Directory.", ex);

                throw;
            }
            finally
            {
                if (de != null)
                    de.Dispose();

                if (up != null)
                    up.Dispose();

                if (pc != null)
                    pc.Dispose();
            }
        }

        /// <summary>
        /// Gets details about a specific user
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public ADUser GetUser(string userPrincipalName, string companyCode)
        {
            PrincipalContext pc = null;
            UserPrincipalExt up = null;
            GroupPrincipal gp = null;

            try
            {
                pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password);

                // Find the User
                up = UserPrincipalExt.FindByIdentity(pc, IdentityType.UserPrincipalName, userPrincipalName);

                // Find the Admin group
                gp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, "Admins@" + companyCode.Replace(" ", string.Empty));

                // Populate the information
                ADUser user = new ADUser();
                user.UserGuid = (Guid)up.Guid;
                user.Firstname = up.GivenName;
                user.Middlename = up.MiddleName;
                user.Lastname = up.Surname;
                user.DisplayName = up.DisplayName;
                user.Department = up.Department;
                user.UserPrincipalName = up.UserPrincipalName;
                user.SamAccountName = up.SamAccountName;
                user.IsCompanyAdmin = up.IsMemberOf(gp);
                user.IsEnabled = up.Enabled == null ? true : (bool)up.Enabled;
                user.PasswordNeverExpires = up.PasswordNeverExpires;
                user.CanonicalName = up.CanonicalName;
                user.IsLockedOut = up.IsAccountLockedOut();

                return user;                
            }
            catch (Exception ex)
            {
                this.logger.Fatal("Error retrieving user " + userPrincipalName + ": " + ex.ToString());

                throw;
            }
            finally
            {
                if (gp != null)
                    gp.Dispose();

                if (up != null)
                    up.Dispose();

                if (pc != null)
                    pc.Dispose();
            }
        }

        /// <summary>
        /// Unlocks an account in active directory
        /// </summary>
        /// <param name="userPrincipalName"></param>
        public void UnlockAccount(string userPrincipalName)
        {
            PrincipalContext pc = null;
            UserPrincipalExt up = null;

            try
            {
                pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password);

                // Find the User
                up = UserPrincipalExt.FindByIdentity(pc, IdentityType.UserPrincipalName, userPrincipalName);

                // Unlock the account
                up.UnlockAccount();

                // DEBUG //
                this.logger.Debug("Unlocked account: " + userPrincipalName);
            }
            catch (Exception ex)
            {
                this.logger.Fatal("Error unlocking account " + userPrincipalName + ": " + ex.ToString());

                throw;
            }
            finally
            {
                if (up != null)
                    up.Dispose();

                if (pc != null)
                    pc.Dispose();
            }
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
