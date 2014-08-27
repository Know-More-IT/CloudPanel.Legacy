using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.ActiveDirectory
{
    [DirectoryObjectClass("group")]
    [DirectoryRdnPrefix("CN")]
    public class GroupPrincipalExt : GroupPrincipal
    {
        public GroupPrincipalExt(PrincipalContext context) : base(context) { }
        public GroupPrincipalExt(PrincipalContext context, string groupName) : base(context, groupName) { }

        public static new GroupPrincipalExt FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (GroupPrincipalExt)FindByIdentityWithType(context, typeof(GroupPrincipalExt), identityValue);
        }

        public static new GroupPrincipalExt FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
        {
            return (GroupPrincipalExt)FindByIdentityWithType(context, typeof(GroupPrincipalExt), identityType, identityValue);
        }

        #region Extensions

        [DirectoryProperty("wWWHomePage")]
        public string WWWHomePage
        {
            get
            {
                if (ExtensionGet("wWWHomePage").Length != 1)
                    return null;

                return (string)ExtensionGet("wWWHomePage")[0];
            }
            set
            {
                this.ExtensionSet("wWWHomePage", value);
            }
        }

        #endregion

        #region Exchange Flags

        [DirectoryProperty("msExchGroupDepartRestriction")]
        public int? msExchGroupDepartRestriction
        {
            get
            {
                if (ExtensionGet("msExchGroupDepartRestriction").Length != 1)
                    return null;

                return (int)ExtensionGet("msExchGroupDepartRestriction")[0];
            }
            set
            {
                this.ExtensionSet("msExchGroupDepartRestriction", value);
            }
        }

        [DirectoryProperty("msExchGroupJoinRestriction")]
        public int? msExchGroupJoinRestriction
        {
            get
            {
                if (ExtensionGet("msExchGroupJoinRestriction").Length != 1)
                    return null;

                return (int)ExtensionGet("msExchGroupJoinRestriction")[0];
            }
            set
            {
                this.ExtensionSet("msExchGroupJoinRestriction", value);
            }
        }

        [DirectoryProperty("msExchModerationFlags")]
        public int? msExchModerationFlags
        {
            get
            {
                if (ExtensionGet("msExchModerationFlags").Length != 1)
                    return null;

                return (int)ExtensionGet("msExchModerationFlags")[0];
            }
        }

        [DirectoryProperty("msExchHideFromAddressLists")]
        public bool? msExchHideFromAddressLists
        {
            get
            {
                if (ExtensionGet("msExchHideFromAddressLists").Length != 1)
                    return null;

                bool hide;
                bool.TryParse(ExtensionGet("msExchHideFromAddressLists")[0].ToString(), out hide);

                return hide;
            }
        }

        [DirectoryProperty("msExchEnableModeration")]
        public bool? msExchEnableModeration
        {
            get
            {
                if (ExtensionGet("msExchEnableModeration").Length != 1)
                    return null;

                bool hide;
                bool.TryParse(ExtensionGet("msExchEnableModeration")[0].ToString(), out hide);

                return hide;
            }
        }

        [DirectoryProperty("msExchRequireAuthToSendTo")]
        public bool? msExchRequireAuthToSendTo
        {
            get
            {
                if (ExtensionGet("msExchRequireAuthToSendTo").Length != 1)
                    return null;

                bool hide;
                bool.TryParse(ExtensionGet("msExchRequireAuthToSendTo")[0].ToString(), out hide);

                return hide;
            }
        }

        [DirectoryProperty("msExchCoManagedByLink")]
        public object[] msExchCoManagedByLink
        {
            get
            {
                if (ExtensionGet("msExchCoManagedByLink").Length < 1)
                    return null;

                return (object[])ExtensionGet("msExchCoManagedByLink");
            }
        }

        [DirectoryProperty("authOrig")]
        public object[] authOrig
        {
            get
            {
                if (ExtensionGet("authOrig").Length < 1)
                    return null;

                return (object[])ExtensionGet("authOrig");
            }
        }

        [DirectoryProperty("msExchBypassModerationLink")]
        public object[] msExchBypassModerationLink
        {
            get
            {
                if (ExtensionGet("msExchBypassModerationLink").Length < 1)
                    return null;

                return (object[])ExtensionGet("msExchBypassModerationLink");
            }
        }

        [DirectoryProperty("msExchModeratedByLink")]
        public object[] msExchModeratedByLink
        {
            get
            {
                if (ExtensionGet("msExchModeratedByLink").Length < 1)
                    return null;

                return (object[])ExtensionGet("msExchModeratedByLink");
            }
        }

        [DirectoryProperty("managedBy")]
        public string ManagedBy
        {
            get
            {
                if (ExtensionGet("managedBy").Length != 1)
                    return null;

                return ExtensionGet("managedBy")[0].ToString();
            }
            set
            {
                this.ExtensionSet("managedBy", value);
            }
        }

        [DirectoryProperty("mail")]
        public string Mail
        {
            get
            {
                if (ExtensionGet("mail").Length != 1)
                    return null;

                return ExtensionGet("mail")[0].ToString();
            }
            set
            {
                this.ExtensionSet("mail", value);
            }
        }

        [DirectoryProperty("proxyAddresses")]
        public object[] ProxyAddresses
        {
            get
            {
                if (ExtensionGet("proxyAddresses").Length < 1)
                    return null;

                return (object[])ExtensionGet("proxyAddresses");
            }
            set
            {
                this.ExtensionSet("proxyAddresses", value);
            }
        }
        #endregion

        #region Lync Flags

        [DirectoryProperty("msRTCSIP-GroupingID")]
        public Guid msRTCSIPGroupingID
        {
            get
            {
                if (ExtensionGet("msRTCSIP-GroupingID") != null && ExtensionGet("msRTCSIP-GroupingID").Length != 1)
                {
                    Guid guid = new Guid((byte[])ExtensionGet("msRTCSIP-GroupingID")[0]);

                    return guid;
                }
                else
                    return System.Guid.Empty;
            }
            set
            {
                ((DirectoryEntry)this.GetUnderlyingObject()).Properties["msRTCSIP-GroupingID"].Value = value.ToByteArray();
            }
        }

        #endregion
    }
}