using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;
using CloudPanel.Modules.Settings;
using log4net;
using System.Reflection;
using System.DirectoryServices;
using CloudPanel.Modules.Base;

namespace CloudPanel.Modules.ActiveDirectory
{
    public class ADGroups : IDisposable
    {
        // Disposing information
        private bool disposed = false;

        // Information for connecting
        private string username;
        private string password;
        private string domainController;

        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public enum Attributes
        {
            msRTCSIPGroupingID
        }

        public enum GroupType
        {
            Local,
            Global,
            Universal
        }

        /// <summary>
        /// Creates a new Groups object
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="domainController"></param>
        public ADGroups(string username, string password, string domainController)
        {
            this.username = username;
            this.password = password;
            this.domainController = domainController;
        }

        /// <summary>
        /// Creates a new group
        /// </summary>
        /// <param name="oupath"></param>
        /// <param name="groupname"></param>
        /// <param name="description"></param>
        /// <param name="securityGroup"></param>
        /// <param name="groupType"></param>
        public void Create(string oupath, string groupname, string description, bool securityGroup, GroupType groupType, bool throwIfExists = true)
        {
            PrincipalContext pc = null;
            GroupPrincipal gp = null;

            try
            {
                // Remove whitspace from group name since we added the feature to include the company name as company code
                groupname = groupname.Replace(" ", string.Empty);

                // DEBUG
                this.logger.Debug("Creating new security group " + groupname + " on path " + oupath);

                pc = new PrincipalContext(ContextType.Domain, this.domainController, oupath, this.username, this.password);

                gp = GroupPrincipal.FindByIdentity(pc, groupname);
                if (gp == null)
                {
                    gp = new GroupPrincipal(pc, groupname);
                    gp.IsSecurityGroup = securityGroup;

                    if (!string.IsNullOrEmpty(description))
                        gp.Description = description;

                    switch (groupType)
                    {
                        case GroupType.Local:
                            gp.GroupScope = GroupScope.Local;
                            break;
                        case GroupType.Global:
                            gp.GroupScope = GroupScope.Global;
                            break;
                        case GroupType.Universal:
                            gp.GroupScope = GroupScope.Universal;
                            break;
                        default:
                            throw new Exception("Invalid group type specified.");
                    }

                    // Save
                    gp.Save();

                    // INFO //
                    this.logger.Info("Created security group " + groupname);
                }
                else if (gp != null && throwIfExists)
                    throw new Exception("The group " + groupname + " already exists in Active Directory. Unable to create.");
            }
            catch (Exception ex)
            {
                // LOG //
                this.logger.Fatal("Error creating security group " + groupname, ex);

                throw;
            }
            finally
            {
                if (gp != null)
                    gp.Dispose();

                if (pc != null)
                    pc.Dispose();
            }
        }

        /// <summary>
        /// Deletes a group from Active Directory
        /// </summary>
        /// <param name="groupname"></param>
        /// <param name="errorOnNotFound"></param>
        /// <param name="removeRecursive"></param>
        public void Delete(string groupname, bool errorOnNotFound = true, bool removeRecursive = false)
        {
            PrincipalContext pc = null;
            GroupPrincipal gp = null;

            try
            {
                // Remove whitspace from group name since we added the feature to include the company name as company code
                groupname = groupname.Replace(" ", string.Empty);

                pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password);

                gp = GroupPrincipal.FindByIdentity(pc, groupname);
                if (gp == null && errorOnNotFound)
                    throw new Exception("Unable to find the group " + groupname + " in Active Directory");
                else if (gp != null)
                {
                    if (removeRecursive)
                    {
                        foreach (Principal p in gp.Members)
                        {
                            // Only delete members if they are groups.. don't want to accidently delete users
                            if (p is GroupPrincipal)
                                p.Delete();
                        }
                    }

                    // Delete the main group
                    gp.Delete();

                    this.logger.Info("Deleted security group " + groupname);
                }
            }
            catch (Exception ex)
            {
                this.logger.Fatal("Error deleting security group " + groupname, ex);

                throw;
            }
            finally
            {
                if (gp != null)
                    gp.Dispose();

                if (pc != null)
                    pc.Dispose();
            }
        }

        /// <summary>
        /// Adds or remove group from another group
        /// </summary>
        /// <param name="groupToModify"></param>
        /// <param name="objectToAddOrRemove"></param>
        /// <param name="groupType"></param>
        /// <param name="objectType"></param>
        /// <param name="remove"></param>
        public void ModifyMembership(string groupToModify, string objectToAddOrRemove, IdentityType groupType, IdentityType objectType, bool remove, bool throwIfDoesntExist = true)
        {
            PrincipalContext pc = null;
            GroupPrincipal gp = null;

            try
            {
                // Remove whitspace from group name since we added the feature to include the company name as company code
                groupToModify = groupToModify.Replace(" ", string.Empty);
                objectToAddOrRemove = objectToAddOrRemove.Replace(" ", string.Empty);

                pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password);

                if (remove)
                    this.logger.Debug("Attempting to remove user/group " + objectToAddOrRemove + " from " + groupToModify);
                else
                    this.logger.Debug("Attempting to add user/group " + objectToAddOrRemove + " to " + groupToModify);

                gp = GroupPrincipal.FindByIdentity(pc, groupType, groupToModify);
                if (gp != null)
                {
                    bool groupAlreadyIsMember = gp.Members.Contains(pc, objectType, objectToAddOrRemove);
                    
                    if (remove)
                    {
                        if (groupAlreadyIsMember)
                            gp.Members.Remove(pc, objectType, objectToAddOrRemove);
                    }
                    else
                    {
                        if (!groupAlreadyIsMember)
                            gp.Members.Add(pc, objectType, objectToAddOrRemove);
                    }

                    // Save
                    gp.Save();

                    // INFO //
                    if (remove)
                        this.logger.Info("Successfully removed " + objectToAddOrRemove + " from " + groupToModify);
                    else
                        this.logger.Info("Successfully added " + objectToAddOrRemove + " to " + groupToModify);
                }
                else
                {
                    // If we throw an error or not if the group doesn't exist
                    if (throwIfDoesntExist)
                        throw new Exception("Unable to find the group " + groupToModify + " in Active Directory");
                }
            }
            catch (Exception ex)
            {
                this.logger.Fatal("Unable to modify group " + groupToModify, ex);

                throw;
            }
            finally
            {
                if (gp != null)
                    gp.Dispose();

                if (pc != null)
                    pc.Dispose();
            }
        }

        /// <summary>
        /// Adds or remove users from a group in bulk
        /// </summary>
        /// <param name="groupToModify"></param>
        /// <param name="userPrincipalNames">Dictionary with the key being the userPrincipalName and the value being True if adding or False if removing</param>
        /// <param name="groupType"></param>
        /// <param name="objectType"></param>
        /// <param name="remove"></param>
        /// <param name="throwIfDoesntExist"></param>
        public void ModifyMembership(string groupToModify, Dictionary<string, bool> userPrincipalNames, IdentityType groupType, IdentityType objectType, bool remove, bool throwIfDoesntExist = true)
        {
            PrincipalContext pc = null;
            GroupPrincipal gp = null;

            try
            {
                // Remove whitspace from group name since we added the feature to include the company name as company code
                groupToModify = groupToModify.Replace(" ", string.Empty);

                pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password);

                gp = GroupPrincipal.FindByIdentity(pc, groupType, groupToModify);
                if (gp != null)
                {
                    foreach (KeyValuePair<string, bool> usrs in userPrincipalNames)
                    {
                        // Check if the group contains the user or not first
                        bool groupContainsUser = gp.Members.Contains(pc, objectType, usrs.Key);

                        if (groupContainsUser && !usrs.Value)
                        {
                            this.logger.Info("Attempting to remove user " + usrs.Key + " from group " + groupToModify);
                            gp.Members.Remove(pc, objectType, usrs.Key); // If the group contains the user and we want to remove the user then remove it
                        }
                        else if (!groupContainsUser && usrs.Value)
                        {
                            this.logger.Info("Attempting to add user " + usrs.Key + " from group " + groupToModify);
                            gp.Members.Add(pc, objectType, usrs.Key); // If the group  doesn't contain the user and we want to add then add it
                        }
                    }

                    // Save
                    gp.Save();
                }
                else
                {
                    // If we throw an error or not if the group doesn't exist
                    if (throwIfDoesntExist)
                        throw new Exception("Unable to find the group " + groupToModify + " in Active Directory");
                }
            }
            catch (Exception ex)
            {
                this.logger.Fatal("Unable to modify group " + groupToModify, ex);

                throw;
            }
            finally
            {
                if (gp != null)
                    gp.Dispose();

                if (pc != null)
                    pc.Dispose();
            }
        }

        /// <summary>
        /// Gets all the group owners for a specific group
        /// </summary>
        /// <param name="groupDistinguishedName"></param>
        /// <returns></returns>
        public string[] GetGroupOwners(string groupDistinguishedName)
        {
            PrincipalContext pc = null;
            GroupPrincipalExt gp = null;

            try
            {
                pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password);

                gp = GroupPrincipalExt.FindByIdentity(pc, IdentityType.DistinguishedName, groupDistinguishedName);
                if (gp != null)
                {
                    List<string> ldapDistinguishedNames = new List<string>();

                    if (gp.ManagedBy != null)
                        ldapDistinguishedNames.Add(gp.ManagedBy.ToUpper());

                    if (gp.msExchCoManagedByLink != null)
                    {
                        foreach (object o in gp.msExchCoManagedByLink)
                        {
                            if (!string.IsNullOrEmpty(o.ToString()))
                                ldapDistinguishedNames.Add(o.ToString().ToUpper());
                        }
                    }

                    return ldapDistinguishedNames.ToArray();
                }
                else
                    throw new Exception("Unable to find the group " + groupDistinguishedName + " in Active Directory");

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
            }
        }

        /// <summary>
        /// Checks if a group exists
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public bool DoesGroupExist(string groupName)
        {
            PrincipalContext pc = null;
            GroupPrincipal gp = null;

            try
            {
                // Remove whitspace from group name
                groupName = groupName.Replace(" ", string.Empty);

                pc = new PrincipalContext(ContextType.Domain, this.domainController, this.username, this.password);

                gp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, groupName);
                if (gp != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (gp != null)
                    gp.Dispose();

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
