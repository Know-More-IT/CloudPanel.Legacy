<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="activesync.aspx.cs" Inherits="CloudPanel.company.exchange.activesync" %>
<%@ Register src="../../controls/notification.ascx" tagname="notification" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <link rel="stylesheet" href="../../css/uniform.css" />
    <link rel="stylesheet" href="../../css/select2.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphSideBar" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">
    <div id="content">
        <!--breadcrumbs-->
        <div id="content-header">
            <div id="breadcrumb">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/dashboard.aspx" title="Go to Dashboard" CssClass="tip-bottom"><i class="icon-home"></i>Dashboard</asp:HyperLink>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/resellers.aspx" title="Go to Resellers" CssClass="tip-bottom"><i class="icon-user"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="#" CssClass="tip-bottom"><i class="icon-building"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyCode %></asp:HyperLink>
                <a href="#" title="Exchange" class="tip-bottom"><i class="icon-cloud"></i>Exchange</a>
                <a href="#" title="Exchange" class="tip-bottom"><i class="icon-group"></i>ActiveSync Policies</a>
            </div>

            <uc1:notification ID="notification1" runat="server" />
            <h1><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyName %> ActiveSync Policies</h1>
        </div>
        <!--End-breadcrumbs-->

          <div class="container-fluid">
              <hr />
              <div class="row-fluid">

                  <asp:Panel ID="panelActiveSyncList" runat="server">
                      <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5>Current ActiveSync Policies</h5>
                                <span class="label label-info">
                                    <asp:LinkButton ID="lnkCreatePolicy" runat="server" ForeColor="White" OnClick="lnkCreatePolicy_Click">Click Here to Create New Policies</asp:LinkButton>
                                </span>
                            </div>
                            <div class="widget-content ">
                                <table class="table table-bordered table-striped">
                                    <thead>
                                        <tr>
                                            <th>
                                                Display Name
                                            </th>
                                            <th>
                                                Description
                                            </th>
                                            <th>
                                                &nbsp;
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="repeater" runat="server" OnItemCommand="repeater_ItemCommand">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <%# Eval("DisplayName") %>
                                                    </td>
                                                    <td>
                                                        <%# Eval("Description") %>
                                                    </td>
                                                    <td style="text-align: right">
                                                        <asp:Button ID="btnDeletePolicy" runat="server" CssClass="btn-danger" Text="Delete" CommandArgument='<%# Eval("ExchangeName") %>' CommandName="DeletePolicy" /> &nbsp;
                                                        <asp:Button ID="btnEditPolicy" runat="server" CssClass="btn-info" Text="Edit" CommandArgument='<%# Eval("ExchangeName") %>' CommandName="EditPolicy" />
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                  </asp:Panel>

                  <asp:Panel ID="panelActiveSyncEdit" runat="server" Visible="false">
                      <div class="widget-box">
                          <div class="widget-title">
                              <span class="icon"><i class="icon icon-edit"></i></span>
                              <h5>General</h5>
                          </div>
                          <div class="widget-content nopadding">
                              <div class="form-horizontal">
                                  <div class="control-group">
                                      <label class="control-label">Name</label>
                                      <div class="controls">
                                          <asp:TextBox ID="txtDisplayName" runat="server" Text=""></asp:TextBox>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Description</label>
                                      <div class="controls">
                                          <asp:TextBox ID="txtDescription" runat="server" Text="" TextMode="MultiLine" Rows="10" Width="95%" ></asp:TextBox>
                                      </div>
                                  </div>
                                  </div>
                              </div>
                          </div>

                        <div class="widget-box">
                          <div class="widget-title">
                              <span class="icon"><i class="icon icon-edit"></i></span>
                              <h5>Basic Settings</h5>
                          </div>
                          <div class="widget-content nopadding">
                              <div class="form-horizontal">
                                  <div class="control-group">
                                      <label class="control-label">Allow Bluetooth</label>
                                      <div class="controls">
                                          <asp:DropDownList ID="ddlAllowBluetooth" runat="server">
                                              <asp:ListItem Text="Disable" Value="Disable"></asp:ListItem>
                                              <asp:ListItem Text="HandsFree" Value="HandsFree"></asp:ListItem>
                                              <asp:ListItem Text="Allow" Value="Allow" Selected="True"></asp:ListItem>
                                          </asp:DropDownList>
                                          <a href="#" title="This setting specifies whether a mobile phone allows Bluetooth connections. The available options are Disable, HandsFree Only, and Allow." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow Browser</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowBrowser" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether Pocket Internet Explorer is allowed on the mobile phone. This setting doesn't affect third-party browsers installed on the phone." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow Camera</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowCamera" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether the mobile phone camera can be used." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow Consumer Mail</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowConsumerMail" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether the mobile phone user can configure a personal e-mail account (either POP3 or IMAP4) on the mobile phone." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow Desktop Sync</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowDesktopSync" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether the mobile phone can synchronize with a computer through a cable, Bluetooth, or IrDA connection." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow Internet Sharing</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowInternetSharing" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether the mobile phone can be used as a modem for a desktop or a portable computer." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow Simple Password</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowSimplePassword" runat="server" Checked="false" />
                                          <a href="#" title="This setting enables or disables the ability to use a simple password such as 1234." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow Text Messaging</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowTextMessaging" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether text messaging is allowed from the mobile phone. " class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow Wi-Fi</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowWIFI" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether wireless Internet access is allowed on the mobile phone." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Password Enabled</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbPasswordEnabled" runat="server" Checked="false" />
                                          <a href="#" title="This setting enables the mobile phone password." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Alphanumeric Password Required</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAlphanumericPwdRequired" runat="server" Checked="false" />
                                          <a href="#" title="This setting requires that a password contains numeric and non-numeric characters." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Maximum Failed Password Attempts</label>
                                      <div class="controls">
                                          <asp:DropDownList ID="ddlMaxFailedPasswordAttempts" runat="server">
                                              <asp:ListItem Value="Unlimited" Text="Unlimited" Selected="True"></asp:ListItem>
                                              <asp:ListItem Value="1" Text="1"></asp:ListItem>
                                              <asp:ListItem Value="2" Text="2"></asp:ListItem>
                                              <asp:ListItem Value="3" Text="3"></asp:ListItem>
                                              <asp:ListItem Value="4" Text="4"></asp:ListItem>
                                              <asp:ListItem Value="5" Text="5"></asp:ListItem>
                                              <asp:ListItem Value="6" Text="6"></asp:ListItem>
                                              <asp:ListItem Value="7" Text="7"></asp:ListItem>
                                              <asp:ListItem Value="8" Text="8"></asp:ListItem>
                                              <asp:ListItem Value="9" Text="9"></asp:ListItem>
                                              <asp:ListItem Value="10" Text="10"></asp:ListItem>
                                          </asp:DropDownList>
                                          <a href="#" title="This setting specifies how many times an incorrect password can be entered before the mobile phone performs a wipe of all data." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Minimum Password Length</label>
                                      <div class="controls">
                                          <asp:TextBox ID="txtMinPwdLength" runat="server" Text="4"></asp:TextBox>
                                          <a href="#" title="This setting specifies the minimum password length." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                              </div>
                          </div>
                     </div>

                      <div class="widget-box">
                          <div class="widget-title">
                              <span class="icon"><i class="icon icon-edit"></i></span>
                              <h5>Advanced Settings</h5>
                          </div>
                          <div class="widget-content nopadding">
                              <div class="form-horizontal">
                                  
                                  <div class="control-group">
                                      <label class="control-label">Allow HTML Email</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowHTMLEmail" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether e-mail synchronized to the mobile phone can be in HTML format." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow Infrared Connections</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowInfrared" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether infrared connections are allowed to and from the mobile phone." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow Non-Provisionable Devices</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowNonProvisionable" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether older phones that may not support application of all policy settings are allowed to connect to Exchange by using Exchange ActiveSync." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow POP or IMAP</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowPOPIMAP" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether the user can configure a POP3 or an IMAP4 e-mail account on the mobile phone." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow Remote Desktop</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowRemoteDesktop" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether the mobile phone can initiate a remote desktop connection." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow SMIME Encryption Algorithm Negotiation</label>
                                      <div class="controls">
                                          <asp:DropDownList ID="ddlAllowSMIMEEncryptionAlgorithmNeg" runat="server">
                                              <asp:ListItem Value="AllowAnyAlgorithmNegotiation"  Text="Allow Any Algorithm Negotiation" Selected="True"></asp:ListItem>
                                              <asp:ListItem Value="OnlyStrongAlgorithmNegotiation" Text="Only Strong Algorithm Negotiation"></asp:ListItem>
                                              <asp:ListItem Value="BlockNegotiation" Text="Block Negotiation"></asp:ListItem>
                                          </asp:DropDownList>
                                          <a href="#" title="This setting specifies whether S/MIME software certificates are allowed on the mobile phone." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow S/MIME Software Certificates</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowSMIME" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether S/MIME software certificates are allowed on the mobile phone." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow Storage Card</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowStorageCard" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether the mobile phone can access information that's stored on a storage card." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow Unsigned Applications</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowUnsignedApps" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether unsigned applications can be installed on the mobile phone. " class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Allow Unsigned Install Packages</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowUnsignedInstallPackages" runat="server" Checked="true" />
                                          <a href="#" title="This setting specifies whether an unsigned installation package can be run on the mobile phone." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Attached Enabled</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAttachmentsEnabled" runat="server" Checked="true" />
                                          <a href="#" title="This setting enables attachments to be downloaded to the mobile phone." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Device Encryption Enabled</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbDeviceEncryptionEnabled" runat="server" Checked="false" />
                                          <a href="#" title="This setting enables encryption on the mobile phone. Not all mobile phones can enforce encryption. For more information, see the phone and mobile operating system documentation." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Password Expiration</label>
                                      <div class="controls">
                                          <asp:TextBox ID="txtPasswordExpiration" runat="server" Text="Unlimited"></asp:TextBox>
                                          <a href="#" title="This setting enables the administrator to configure a length of time after which a mobile phone password must be changed. Format dd.hh.mm:ss" class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Password History</label>
                                      <div class="controls">
                                          <asp:TextBox ID="txtPasswordHistory" runat="server" Text="0"></asp:TextBox>
                                          <a href="#" title="This setting specifies the number of past passwords that can be stored in a user's mailbox. A user can't reuse a stored password." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Policy Refresh Interval</label>
                                      <div class="controls">
                                          <asp:TextBox ID="txtPolicyRefreshInterval" runat="server" Text="Unlimited"></asp:TextBox>
                                          <a href="#" title="This setting defines how frequently the mobile phone updates the Exchange ActiveSync policy from the server." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Maximum Attachment Size</label>
                                      <div class="controls">
                                          <asp:TextBox ID="txtMaxAttachmentSize" runat="server" Text="Unlimited"></asp:TextBox>
                                          <a href="#" title="This setting specifies the maximum size of attachments that are automatically downloaded to the mobile phone." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Maximum Calendar Age Filter</label>
                                      <div class="controls">
                                          <asp:DropDownList ID="ddlMaxCalendarAgeFilter" runat="server">
                                              <asp:ListItem Value="All" Text="All" Selected="True"></asp:ListItem>
                                              <asp:ListItem Value="TwoWeeks" Text="Two Weeks"></asp:ListItem>
                                              <asp:ListItem Value="OneMonth" Text="One Month"></asp:ListItem>
                                              <asp:ListItem Value="ThreeMonths" Text="Three Months"></asp:ListItem>
                                              <asp:ListItem Value="SixMonths" Text="Six Months"></asp:ListItem>
                                          </asp:DropDownList>
                                          <a href="#" title="This setting specifies the maximum range of calendar days that can be synchronized to the mobile phone. The value is specified in days." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Maximum Inactivity Time Lock</label>
                                      <div class="controls">
                                          <asp:TextBox ID="txtMaxInactivityLock" runat="server" Text="15"></asp:TextBox>
                                          <a href="#" title="This setting specifies the length of time that a mobile phone can go without user input before it locks. This setting is in minutes." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Maximum E-mail Age Filter</label>
                                      <div class="controls">
                                          <asp:DropDownList ID="ddlMaxEmailAgeFilter" runat="server">
                                              <asp:ListItem Value="All" Text="All"></asp:ListItem>
                                              <asp:ListItem Value="OneDay" Text="One Day"></asp:ListItem>
                                              <asp:ListItem Value="ThreeDays" Text="Three Days" Selected="True"></asp:ListItem>
                                              <asp:ListItem Value="OneWeek" Text="One Week"></asp:ListItem>
                                              <asp:ListItem Value="TwoWeeks" Text="Two Weeks"></asp:ListItem>
                                              <asp:ListItem Value="OneMonth" Text="One Month"></asp:ListItem>
                                              <asp:ListItem Value="ThreeMonths" Text="Three Months"></asp:ListItem>
                                              <asp:ListItem Value="SixMonths" Text="Six Months"></asp:ListItem>
                                          </asp:DropDownList>
                                          <a href="#" title="This setting specifies the maximum number of days' worth of e-mail items to synchronize to the mobile phone. The value is specified in days." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Maximum HTML E-mail Body Truncation Size</label>
                                      <div class="controls">
                                          <asp:TextBox ID="txtMaxHTMLBodyTruncSize" runat="server" Text="Unlimited"></asp:TextBox>
                                          <a href="#" title="This setting specifies the size beyond which HTML-formatted e-mail messages are truncated when they are synchronized to the mobile phone. The value is specified in kilobytes (KB)." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Minumum Device Password Complex Characters</label>
                                      <div class="controls">
                                          <asp:TextBox ID="txtMinPwdComplexChar" runat="server" Text="0"></asp:TextBox>
                                          <a href="#" title="This setting specifies the minimum number of complex characters required in a mobile phone password. A complex character is any character that is not a letter." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Maximum E-mail Body Truncation Size</label>
                                      <div class="controls">
                                          <asp:TextBox ID="txtMaxEmailBodyTruncSize" runat="server" Text="Unlimited"></asp:TextBox>
                                          <a href="#" title="This setting specifies the size beyond which e-mail messages are truncated when they are synchronized to the mobile phone. The value is specified in kilobytes (KB)." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Password Recovery</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbPasswordRecovery" runat="server" Checked="false" />
                                          <a href="#" title="When this setting is enabled, the mobile phone generates a recovery password that's sent to the server. If the user forgets their mobile phone password, the recovery password can be used to unlock the mobile phone and enable the user to create a new mobile phone password." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Require Device Encryption</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbRequireDeviceEncryption" runat="server" Checked="false" />
                                          <a href="#" title="This setting specifies whether device encryption is required. If set to $true, the mobile phone must be able to support and implement encryption to synchronize with the server." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Require encrypted S/MIME Messages</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbRequireEncryptedSMIMEMsg" runat="server" Checked="false" />
                                          <a href="#" title="This setting specifies whether S/MIME messages must be encrypted." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Require encryption SMIME Algorithm</label>
                                      <div class="controls">
                                          <asp:DropDownList ID="ddlRequireEncryptedSMIMEAlgorithm" runat="server">
                                              <asp:ListItem Value="TripleDES" Text="TripleDES" Selected="True"></asp:ListItem>
                                              <asp:ListItem Value="DES" Text="DES"></asp:ListItem>
                                              <asp:ListItem Value="RC2128bit" Text="RC2128bit"></asp:ListItem>
                                              <asp:ListItem Value="RC264bit" Text="RC264bit"></asp:ListItem>
                                              <asp:ListItem Value="RC240bit" Text="RC240bit"></asp:ListItem>
                                          </asp:DropDownList>
                                          <a href="#" title="The RequireEncryptionSMIMEAlgorithm parameter specifies what required algorithm must be used when encrypting a message." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Require signed S/MIME Messages</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbRequireSignedSMIMEMsg" runat="server" Checked="false" />
                                          <a href="#" title="The RequireSignedSMIMEMessages parameter specifies whether the device must send signed S/MIME messages." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Require signed SMIME Algorithm</label>
                                      <div class="controls">
                                          <asp:DropDownList ID="ddlRequireSignedSMIMEMsg" runat="server">
                                              <asp:ListItem Value="SHA1" Text="SHA1" Selected="True"></asp:ListItem>
                                              <asp:ListItem Value="MD5" Text="MD5"></asp:ListItem>
                                          </asp:DropDownList>
                                          <a href="#" title="The RequireSignedSMIMEAlgorithm parameter specifies what required algorithm must be used when signing a message." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Require manual synchronization while roaming</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbRequireManualSyncRoaming" runat="server" Checked="false" />
                                          <a href="#" title="This setting specifies whether the mobile phone must synchronize manually while roaming. Allowing automatic synchronization while roaming will frequently lead to larger-than-expected data costs for the mobile phone plan." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  <div class="control-group">
                                      <label class="control-label">Require Storage Card Encryption</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbRequireStorageCardEncryption" runat="server" Checked="false" />
                                          <a href="#" title="This setting specifies whether the storage card must be encrypted. Not all mobile phone operating systems support storage card encryption. For more information, see your mobile phone and mobile operating system for more information." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                              </div>
                          </div>
                      </div>

                      <div class="widget-box">
                          <div class="widget-title">
                              <span class="icon"><i class="icon icon-edit"></i></span>
                              <h5>Exchange 2013 Only ActiveSync Policies</h5>
                          </div>
                          <div class="widget-content nopadding">
                              <div class="form-horizontal">
                                  <div class="control-group">
                                      <label class="control-label">Allow Apple Push Notifications</label>
                                      <div class="controls">
                                          <asp:CheckBox ID="cbAllowApplePushNotifications" runat="server" Checked="true" />
                                          <a href="#" title="The AllowApplePushNotifications parameter specifies whether push notifications are allowed for Apple mobile devices." class="tip-right"><i class="icon-question-sign"></i></a>
                                      </div>
                                  </div>
                                  </div>
                              </div>
                          </div>

                      <div class="widget-box">
                          <div class="widget-content nopadding">
                              <div class="form-horizontal">
                                  <div class="control-group" style="text-align: right; margin: 20px">
                                      <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                                      &nbsp;
                                       <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnSave_Click" />
                                  </div>
                              </div>
                          </div>
                      </div>
                  </asp:Panel>

                </div>
        </div>
    </div>

    <script src="../../js/jquery.min.js"></script> 
    <script src="../../js/jquery.ui.custom.js"></script> 
    <script src="../../js/bootstrap.min.js"></script> 
    <script src="../../js/jquery.uniform.js"></script> 
    <script src="../../js/select2.min.js"></script> 
    <script src="../../js/jquery.dataTables.min.js"></script> 
    <script src="../../js/matrix.js"></script> 
    <script src="../../js/matrix.tables.js"></script>
    <script src="../../js/jquery.validate.js"></script>
    <script src="../../js/masked.js"></script>

</asp:Content>
