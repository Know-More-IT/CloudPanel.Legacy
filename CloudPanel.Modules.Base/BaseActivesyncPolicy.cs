using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public class BaseActivesyncPolicy
    {
        public bool AlphanumericDevicePasswordRequired { get; set; }
        public bool DevicePasswordEnabled { get; set; }
        public bool AllowSimpleDevicePassword { get; set; }
        public bool AllowNonProvisionableDevice { get; set; }
        public bool AttachmentsEnabled { get; set; }
        public bool DeviceEncryptionEnabled { get; set; }
        public bool RequireStorageCardEncryption { get; set; }
        public bool PasswordRecoveryEnabled { get; set; }
        public bool WSSAccessEnabled { get; set; }
        public bool UNCAccessEnabled { get; set; }
        public bool AllowStorageCard { get; set; }
        public bool AllowCamera { get; set; }
        public bool RequireDeviceEncryption { get; set; }
        public bool AllowUnsignedApplications { get; set; }
        public bool AllowUnsignedInstallationPackages { get; set; }
        public bool AllowWiFi { get; set; }
        public bool AllowTextMessaging { get; set; }
        public bool AllowPOPIMAPEmail{ get; set; }
        public bool AllowIrDA { get; set; }
        public bool RequireManualSyncWhenRoaming { get; set; }
        public bool AllowDesktopSync { get; set; }
        public bool AllowHTMLEmail { get; set; }
        public bool RequireSignedSMIMEMessages { get; set; }
        public bool RequireEncryptedSMIMEMessages { get; set; }
        public bool AllowSMIMESoftCerts { get; set; }
        public bool AllowBrowser { get; set; }
        public bool AllowConsumerEmail { get; set; }
        public bool AllowRemoteDesktop { get; set; }
        public bool AllowInternetSharing { get; set; }
        public bool AllowExternalDeviceManagement { get; set; }
        public bool AllowMobileOTAUpdate { get; set; }
        public bool IrmEnabled { get; set; }
        public bool AllowApplePushNotifications { get; set; }
        public bool IsEnterpriseCAL { get; set; }

        public int ASID { get; set; }
        public int MinDevicePasswordLength { get; set; }
        public int DevicePasswordHistory { get; set; }
        public int MinDevicePasswordComplexCharacters { get; set; }
        public int MaxInactivityTimeDeviceLock { get; set; }
        public int MaxDevicePasswordFailedAttempts { get; set; }
        public int DevicePasswordExpiration { get; set; }
        public int MaxEmailBodyTruncationSize { get; set; }
        public int MaxEmailHTMLBodyTruncationSize { get; set; }
        public int MaxAttachmentSize { get; set; }
        public int DevicePolicyRefreshInterval { get; set; }

        public string OldDisplayName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string CompanyCode { get; set; }
        public string ExchangeName { get; set; }
        public string AllowBluetooth { get; set; }
        public string MaxCalendarAgeFilter { get; set; }
        public string MaxEmailAgeFilter { get; set; }
        public string RequireSignedSMIMEAlgorithm { get; set; }
        public string RequireEncryptionSMIMEAlgorithm { get; set; }
        public string AllowSMIMEEncryptionAlgorithmNegotiation { get; set; }
        public string MobileOTAUpdateMode { get; set; }

        public string[] ApprovedApplicationList { get; set; }
        public string[] UnapprovedInROMApplicationList { get; set; }

    }
}
