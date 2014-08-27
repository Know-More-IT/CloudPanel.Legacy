using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Drawing;
using System.DirectoryServices;

namespace CloudPanel.Modules.ActiveDirectory
{
    [DirectoryObjectClass("user")]
    [DirectoryRdnPrefix("CN")]
    public class UserPrincipalExt : UserPrincipal
    {
        public UserPrincipalExt(PrincipalContext context) : base(context) { }
        public UserPrincipalExt(PrincipalContext context, string sAMAccountName, string password, bool enabled) : base(context, sAMAccountName, password, enabled) { }

        public static new UserPrincipalExt FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (UserPrincipalExt)FindByIdentityWithType(context, typeof(UserPrincipalExt), identityValue);
        }

        public static new UserPrincipalExt FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
        {
            return (UserPrincipalExt)FindByIdentityWithType(context, typeof(UserPrincipalExt), identityType, identityValue);
        }

        #region Extensions

        [DirectoryProperty("canonicalName")]
        public string CanonicalName
        {
            get
            {
                string dn = (string)ExtensionGet("distinguishedName")[0];

                string[] originalArray = dn.Split(',');
                string[] reversedArray = dn.Split(',');
                Array.Reverse(reversedArray);

                string canonicalName = string.Empty;
                foreach (string s in reversedArray)
                {
                    if (s.StartsWith("CN="))
                        canonicalName += s.Replace("CN=", string.Empty) + "/";
                    else if (s.StartsWith("OU="))
                        canonicalName += s.Replace("OU=", string.Empty) + "/";
                }

                // Remove the ending slash
                canonicalName = canonicalName.Substring(0, canonicalName.Length - 1);

                // Now our canonical name should be formatted except for the DC=
                // Lets do the DC= now
                string domain = string.Empty;
                foreach (string s in originalArray)
                {
                    if (s.StartsWith("DC="))
                        domain += s.Replace("DC=", string.Empty) + ".";
                }

                // Remove the ending period
                domain = domain.Substring(0, domain.Length - 1);
                
                return string.Format("{0}/{1}", domain, canonicalName);
            }
        }

        [DirectoryProperty("comment")]
        public string Comment
        {
            get
            {
                if (ExtensionGet("comment").Length != 1)
                    return null;

                return (string)ExtensionGet("comment")[0];
            }
            set
            {
                this.ExtensionSet("comment", value);
            }
        }

        [DirectoryProperty("mailNickname")]
        public string mailnickname
        {
            get
            {
                if (ExtensionGet("mailNickname").Length != 1)
                    return null;

                return (string)ExtensionGet("mailNickname")[0];
            }
            set
            {
                this.ExtensionSet("mailNickname", value);
            }
        }

        [DirectoryProperty("description")]
        public string Description
        {
            get
            {
                if (ExtensionGet("description").Length != 1)
                    return null;

                return (string)ExtensionGet("description")[0];
            }
            set
            {
                this.ExtensionSet("description", value);
            }
        }

        [DirectoryProperty("title")]
        public string JobTitle
        {
            get
            {
                if (ExtensionGet("title").Length != 1)
                    return null;

                return (string)ExtensionGet("title")[0];
            }
            set
            {
                this.ExtensionSet("title", value);
            }
        }

        [DirectoryProperty("company")]
        public string Company
        {
            get
            {
                if (ExtensionGet("company").Length != 1)
                    return null;

                return (string)ExtensionGet("company")[0];
            }
            set
            {
                this.ExtensionSet("company", value);
            }
        }

        [DirectoryProperty("physicalDeliveryOfficeName")]
        public string Office
        {
            get
            {
                if (ExtensionGet("physicalDeliveryOfficeName").Length != 1)
                    return null;

                return (string)ExtensionGet("physicalDeliveryOfficeName")[0];
            }
            set
            {
                this.ExtensionSet("physicalDeliveryOfficeName", value);
            }
        }

        [DirectoryProperty("department")]
        public string Department
        {
            get
            {
                if (ExtensionGet("department").Length != 1)
                    return null;

                return (string)ExtensionGet("department")[0];
            }
            set
            {
                this.ExtensionSet("department", value);
            }
        }

        [DirectoryProperty("streetAddress")]
        public string StreetAddress
        {
            get
            {
                if (ExtensionGet("streetAddress").Length != 1)
                    return null;

                return (string)ExtensionGet("streetAddress")[0];
            }
            set
            {
                this.ExtensionSet("streetAddress", value);
            }
        }

        [DirectoryProperty("l")]
        public string City
        {
            get
            {
                if (ExtensionGet("l").Length != 1)
                    return null;

                return (string)ExtensionGet("l")[0];
            }
            set
            {
                this.ExtensionSet("l", value);
            }
        }

        [DirectoryProperty("st")]
        public string State
        {
            get
            {
                if (ExtensionGet("st").Length != 1)
                    return null;

                return (string)ExtensionGet("st")[0];
            }
            set
            {
                this.ExtensionSet("st", value);
            }
        }

        [DirectoryProperty("postalCode")]
        public string ZipCode
        {
            get
            {
                if (ExtensionGet("postalCode").Length != 1)
                    return null;

                return (string)ExtensionGet("postalCode")[0];
            }
            set
            {
                this.ExtensionSet("postalCode", value);
            }
        }

        [DirectoryProperty("co")]
        public string Country
        {
            get
            {
                if (ExtensionGet("co").Length != 1)
                    return null;

                return (string)ExtensionGet("co")[0];
            }
            set
            {
                this.ExtensionSet("co", value);
            }
        }

        [DirectoryProperty("sn")]
        public string LastName
        {
            get
            {
                if (ExtensionGet("sn").Length != 1)
                    return null;

                return (string)ExtensionGet("sn")[0];
            }
            set
            {
                this.ExtensionSet("sn", value);
            }
        }

        [DirectoryProperty("whenCreated")]
        public string WhenCreated
        {
            get
            {
                if (ExtensionGet("whenCreated").Length != 1)
                    return "Unknown";

                return ExtensionGet("whenCreated").ToString();
            }
        }

        [DirectoryProperty("telephoneNumber")]
        public string TelephoneNumber
        {
            get
            {
                if (ExtensionGet("telephoneNumber").Length != 1)
                    return null;

                return (string)ExtensionGet("telephoneNumber")[0];
            }
            set
            {
                this.ExtensionSet("telephoneNumber", value);
            }
        }

        [DirectoryProperty("wWWHomePage")]
        public string Website
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

        [DirectoryProperty("manager")]
        public string Manager
        {
            get
            {
                if (ExtensionGet("manager").Length != 1)
                    return null;

                return (string)ExtensionGet("manager")[0];
            }
            set
            {
                this.ExtensionSet("manager", value);
            }
        }

        #endregion

        #region Exchange Extensions

        [DirectoryProperty("extensionAttribute1")]
        public string ExtensionAttribute1
        {
            get
            {
                if (ExtensionGet("extensionAttribute1").Length != 1)
                    return null;

                return (string)ExtensionGet("extensionAttribute1")[0];
            }
            set
            {
                this.ExtensionSet("extensionAttribute1", value);
            }
        }

        [DirectoryProperty("msExchGenericForwardingAddress")]
        public string ForwardingAddress
        {
            get
            {
                if (ExtensionGet("msExchGenericForwardingAddress").Length != 1)
                    return null;

                return ExtensionGet("msExchGenericForwardingAddress")[0].ToString();
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

        [DirectoryProperty("deliverAndRedirect")]
        public bool DeliverToAndForward
        {
            get
            {
                if (ExtensionGet("deliverAndRedirect").Length != 1)
                    return false;

                bool theValue;
                bool.TryParse(ExtensionGet("deliverAndRedirect")[0].ToString(), out theValue);

                return theValue;
            }
        }

        [DirectoryProperty("mDBUseDefaults")]
        public bool? MbxUseDefaults
        {
            get
            {
                if (ExtensionGet("mDBUseDefaults").Length != 1)
                    return null;

                bool theValue;
                bool.TryParse(ExtensionGet("mDBUseDefault")[0].ToString(), out theValue);

                return theValue;
            }
            set
            {
                this.ExtensionSet("mDBUseDefaults", value);
            }
        }

        [DirectoryProperty("mDBStorageQuota")]
        public int MbxSendWarning
        {
            get
            {
                if (ExtensionGet("mDBStorageQuota").Length != 1)
                    return 0;

                return (int)ExtensionGet("mDBStorageQuota")[0];
            }
            set
            {
                this.ExtensionSet("mDBStorageQuota", value);
            }
        }

        [DirectoryProperty("mDBOverQuotaLimit")]
        public int MbxProhibitSend
        {
            get
            {
                if (ExtensionGet("mDBOverQuotaLimit").Length != 1)
                    return 0;

                return (int)ExtensionGet("mDBOverQuotaLimit")[0];
            }
            set
            {
                this.ExtensionSet("mDBOverQuotaLimit", value);
            }
        }

        [DirectoryProperty("mDBOverHardQuotaLimit")]
        public int MbxProhibitSendReceive
        {
            get
            {

                if (ExtensionGet("mDBOverHardQuotaLimit").Length != 1)
                    return 0;

                return (int)ExtensionGet("mDBOverHardQuotaLimit")[0];
            }
            set
            {
                this.ExtensionSet("mDBOverHardQuotaLimit", value);
            }
        }

        [DirectoryProperty("msExchLitigationHoldDate")]
        public DateTime? msExchLitigationHoldDate
        {
            get
            {
                if (ExtensionGet("msExchLitigationHoldDate").Length != 1)
                    return null;

                return DateTime.Parse(ExtensionGet("msExchLitigationHoldDate")[0].ToString());
            }
        }

        [DirectoryProperty("msExchLitigationHoldOwner")]
        public string msExchLitigationHoldOwner
        {
            get
            {
                if (ExtensionGet("msExchLitigationHoldOwner").Length != 1)
                    return null;

                return ExtensionGet("msExchLitigationHoldOwner")[0].ToString();
            }
        }

        [DirectoryProperty("msExchRetentionComment")]
        public string msExchRetentionComment
        {
            get
            {
                if (ExtensionGet("msExchRetentionComment").Length != 1)
                    return null;

                return ExtensionGet("msExchRetentionComment")[0].ToString();
            }
        }

        [DirectoryProperty("msExchRetentionURL")]
        public string msExchRetentionURL
        {
            get
            {
                if (ExtensionGet("msExchRetentionURL").Length != 1)
                    return null;

                return ExtensionGet("msExchRetentionURL")[0].ToString();
            }
        }

        [DirectoryProperty("altRecipient")]
        public string altRecipient
        {
            get
            {
                if (ExtensionGet("altRecipient").Length != 1)
                    return null;

                return ExtensionGet("altRecipient")[0].ToString();
            }
        }

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

        [DirectoryProperty("msExchMobileMailboxPolicyLink")]
        public string msExchMobileMailboxPolicyLink
        {
            get
            {
                if (ExtensionGet("msExchMobileMailboxPolicyLink").Length != 1)
                    return null;
                else
                {
                    string attrDN = ExtensionGet("msExchMobileMailboxPolicyLink")[0].ToString();

                    // Now get the name of the policy
                    string[] attrDNSplit = attrDN.Split(',');

                    // First one contains the name of the policy
                    string attrDisplayName = attrDNSplit[0];

                    // Remove CN=
                    attrDisplayName = attrDisplayName.Replace("CN=", string.Empty);

                    // Return just the name of policy
                    return attrDisplayName;
                }
            }
        }

        [DirectoryProperty("msExchUserCulture")]
        public string msExchUserCulture
        {
            get
            {
                if (ExtensionGet("msExchUserCulture").Length != 1)
                    return "en-US";
                else
                {
                    return ExtensionGet("msExchUserCulture")[0].ToString();
                }
            }
        }

        [DirectoryProperty("thumbnailPhoto")]
        public Image thumbnailPhoto
        {
            get
            {
                if (ExtensionGet("thumbnailPhoto") != null && ExtensionGet("thumbnailPhoto").Length > 0)
                {
                    MemoryStream ms = null;
                    try
                    {
                        ms = new MemoryStream((byte[])ExtensionGet("thumbnailPhoto")[0]);
                        Image img = new Bitmap(ms);

                        return img;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                else
                    return null;
            }
            set
            {
                MemoryStream ms = null;
                try
                {
                    ms = new MemoryStream();
                    ((Image)value).Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                    this.ExtensionSet("thumbnailPhoto", ms.ToArray());
                }
                finally
                {
                    if (ms != null)
                        ms.Dispose();
                }
            }
        }


        #endregion
    }
}
