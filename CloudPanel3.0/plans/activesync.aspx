<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="activesync.aspx.cs" Inherits="CloudPanel.plans.activesync" %>

<%@ Register Src="../controls/notification.ascx" TagName="notification" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../css/uniform.css" />
    <link rel="stylesheet" href="../css/select2.css" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">
    <div id="content">
        <!--breadcrumbs-->
        <div id="content-header">
            <div id="breadcrumb">
                <a href="../dashboard.aspx" title="Go to Dashboard" class="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Plan %></a>
                <a href="#"><i class="icon-edit"></i><%= Resources.LocalizedText.Plans %></a>
                <a href="#" class="active"><i class="icon-building"></i><%= Resources.LocalizedText.ActiveSyncPlans %></a>
            </div>

            <uc1:notification ID="notification1" runat="server" />

            <h1><%= Resources.LocalizedText.ActiveSyncPlans %></h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <div class="row-fluid">
                <div class="span12">
                    <div class="widget-box">
                        <div class="widget-title">
                            <ul class="nav nav-tabs">
                                <li class="active"><a data-toggle="tab" href="#General"><%= Resources.LocalizedText.General %></a></li>
                                <li><a data-toggle="tab" href="#Password"><%= Resources.LocalizedText.Password %></a></li>
                                <li><a data-toggle="tab" href="#SyncSettings"><%= Resources.LocalizedText.SyncSettings %></a></li>
                                <li><a data-toggle="tab" href="#Device"><%= Resources.LocalizedText.Device %></a></li>
                                <li><a data-toggle="tab" href="#DeviceApplications"><%= Resources.LocalizedText.DeviceApplications %></a></li>
                            </ul>
                        </div>
                        <div class="widget-content tab-content">
                            <div id="General" class="tab-pane active">
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.Plan %></label>
                                        <div class="controls">
                                            <asp:DropDownList ID="ddlActiveSyncPlan" runat="server" OnSelectedIndexChanged="ddlActiveSyncPlan_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp1 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.DisplayName %></label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtDisplayName" runat="server"></asp:TextBox>
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp2 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.Description %></label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowNonProvDevices %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowNonProvisionableDevices" runat="server" Checked="true" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp3 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.RefreshIntervalInHours %></label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtRefreshInterval" runat="server"></asp:TextBox>
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp4 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="Password" class="tab-pane">
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.RequirePassword %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbRequirePassword" runat="server" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp5 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div id="IfRequirePassword" runat="server" style="display: none">
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.RequireAlphaNumericPassword %></label>
                                            <div class="controls">
                                                <asp:CheckBox ID="cbRequireAlphaNumericPassword" runat="server" />
                                                <a href="#" title="<%= Resources.LocalizedText.ASHelp6 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.MinimumNumberOfCharSets %></label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtMinimumNumberOfCharacterSets" runat="server" Text="1"></asp:TextBox>
                                                <a href="#" title="<%= Resources.LocalizedText.ASHelp7 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.EnablePasswordRecovery %></label>
                                            <div class="controls">
                                                <asp:CheckBox ID="cbEnablePasswordRecovery" runat="server" Checked="false" />
                                                <a href="#" title="<%= Resources.LocalizedText.ASHelp8 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.RequireEncryptionOnDevice %></label>
                                            <div class="controls">
                                                <asp:CheckBox ID="cbRequireEncryption" runat="server" />
                                                <a href="#" title="<%= Resources.LocalizedText.ASHelp9 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.RequireEncryptionOnStorageCard %></label>
                                            <div class="controls">
                                                <asp:CheckBox ID="cbRequireEncryptionOnStorageCard" runat="server" />
                                                <a href="#" title="<%= Resources.LocalizedText.ASHelp10 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.AllowSimplePassword %></label>
                                            <div class="controls">
                                                <asp:CheckBox ID="cbAllowSimplePassword" runat="server" Checked="true" />
                                                <a href="#" title="<%= Resources.LocalizedText.ASHelp11 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.NumberOfFailedAttemptsAllowed %></label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtNumberOfFailedAttemptsAllowed" runat="server"></asp:TextBox>
                                                <a href="#" title="<%= Resources.LocalizedText.ASHelp12 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.MinimumPasswordLength %></label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtMinimumPasswordLength" runat="server"></asp:TextBox>
                                                <a href="#" title="<%= Resources.LocalizedText.ASHelp13 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.InactivityTimeoutInMin %></label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtInactivityTimeout" runat="server"></asp:TextBox>
                                                <a href="#" title="<%= Resources.LocalizedText.ASHelp14 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.PasswordExpirationInDays %></label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtPasswordExpiration" runat="server"></asp:TextBox>
                                                <a href="#" title="<%= Resources.LocalizedText.ASHelp15 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.EnforcePasswordHistory %></label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtEnforcePasswordHistory" runat="server" Text="0"></asp:TextBox>
                                                <a href="#" title="<%= Resources.LocalizedText.ASHelp16 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="SyncSettings" class="tab-pane">
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.IncludePastCalendarItems %></label>
                                        <div class="controls">
                                            <asp:DropDownList ID="ddlPastCalendarItems" runat="server">
                                                <asp:ListItem Text="All" Value="All" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Two Weeks" Value="TwoWeeks"></asp:ListItem>
                                                <asp:ListItem Text="One Month" Value="OneMonth"></asp:ListItem>
                                                <asp:ListItem Text="Three Months" Value="ThreeMonths"></asp:ListItem>
                                                <asp:ListItem Text="Six Months" Value="SixMonths"></asp:ListItem>
                                            </asp:DropDownList>
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp17 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.IncludePastEmailItems %></label>
                                        <div class="controls">
                                            <asp:DropDownList ID="ddlPastEmailItems" runat="server">
                                                <asp:ListItem Text="All" Value="All" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="One Day" Value="OneDay"></asp:ListItem>
                                                <asp:ListItem Text="Three Days" Value="ThreeDays"></asp:ListItem>
                                                <asp:ListItem Text="One Week" Value="OneWeek"></asp:ListItem>
                                                <asp:ListItem Text="Two Weeks" Value="TwoWeeks"></asp:ListItem>
                                                <asp:ListItem Text="One Month" Value="OneMonth"></asp:ListItem>
                                            </asp:DropDownList>
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp18 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.LimitEmailSizeInKB %></label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtLimitEmailSize" runat="server"></asp:TextBox>
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp19 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.RequireManualSyncWhenRoaming %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowDirectPushWhenRoaming" runat="server" Checked="true" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp20 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowHTMLFormattedEmail %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowHTMLEmail" runat="server" Checked="true" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp21 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowAttachmentsDownload %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowAttachmentDownload" runat="server" Checked="true" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp22 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.MaximumAttachmentSizeInKB %></label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtMaximumAttachmentSize" runat="server"></asp:TextBox>
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp23 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="Device" class="tab-pane">
                                <h5><%= Resources.LocalizedText.ASRequireExchangeEnterpriseLicense %></h5>
                                <br />
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowRemovableStorage %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowRemovableStorage" runat="server" Checked="true" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp24 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowCamera %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowCamera" runat="server" Checked="true" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp25 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowWiFi %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowWiFi" runat="server" Checked="true" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp26 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowInfrared %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowInfrared" runat="server" Checked="true" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp27 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowInternetSharingFromDevice %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowInternetSharing" runat="server" Checked="true" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp28 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowRemoteDesktopFromDevice %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowRemoteDesktop" runat="server" Checked="true" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp29 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowDesktopSync %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowDesktopSync" runat="server" Checked="true" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp30 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowTextMessaging %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowTextMessaging" runat="server" Checked="true" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp31 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowBluetooth %></label>
                                        <div class="controls">
                                            <asp:DropDownList ID="ddlAllowBluetooth" runat="server">
                                                <asp:ListItem Text="Allow" Value="Allow" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Handsfree Only" Value="Handsfree"></asp:ListItem>
                                                <asp:ListItem Text="Disable" Value="Disable"></asp:ListItem>
                                            </asp:DropDownList>
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp32 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="DeviceApplications" class="tab-pane">
                                <h5><%= Resources.LocalizedText.ASRequireExchangeEnterpriseLicense %></h5>
                                <br />
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowBrowser %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowBrowser" runat="server" Checked="true" />
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowConsumerMail %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowConsumerMail" runat="server" Checked="true" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp34 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowUnsignedApplications %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowUnsignedApplications" runat="server" Checked="true" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp35 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.AllowUnsignedInstallPackages %></label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbAllowUnsignedInstallPackages" runat="server" Checked="true" />
                                            <a href="#" title="<%= Resources.LocalizedText.ASHelp36 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>


                        <div class="widget-box">
                            <div class="widget-content" style="text-align: right">
                                <asp:Button ID="btnDeletePlan" runat="server" Text="<%$ Resources:LocalizedText, Button_Delete %>"
                                    class="btn btn-danger cancel" OnClick="btnDeletePlan_Click" />
                                <asp:Button ID="btnUpdatePlan" runat="server" Text="<%$ Resources:LocalizedText, Save %>"
                                    class="btn btn-success" OnClick="btnUpdatePlan_Click" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <script src="../js/jquery.min.js"></script>
            <script src="../js/jquery.gritter.min.js"></script> 
            <script src="../js/jquery.gritter.min.js"></script>
            <script src="../js/jquery.ui.custom.js"></script>
            <script src="../js/bootstrap.min.js"></script>
            <script src="../js/jquery.uniform.js"></script>
            <script src="../js/select2.min.js"></script>
            <script src="../js/jquery.dataTables.min.js"></script>
            <script src="../js/matrix.js"></script>
            <script src="../js/matrix.tables.js"></script>
            <script src="../js/jquery.validate.js"></script>
            <script src="../js/masked.js"></script>
            <script src="../js/jquery.maskMoney.js"></script>
            <script type="text/javascript">

                $(document).ready(function () {
                    $('#<%= cbRequirePassword.ClientID %>').change(function () {
                if (this.checked)
                    $('#<%= IfRequirePassword.ClientID %>').fadeIn('slow');
                else
                    $('#<%= IfRequirePassword.ClientID %>').fadeOut('slow');

            });
        });

            </script>
        </div>
    </div>
</asp:Content>
