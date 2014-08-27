<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="settings.aspx.cs" Inherits="CloudPanel.settings" %>

<%@ Register src="controls/notification.ascx" tagname="notification" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="css/uniform.css" />
    <link rel="stylesheet" href="css/select2.css" />
    <style type="text/css">
        #errorContainer {
            display: none;
            overflow: auto;
            background-color: #FFDDDD;
            border: 1px solid #FF2323;
            padding-top: 0;
        }
 
        #errorContainer label {
            float: none;
            width: auto;
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">
<div id="content">
  <div id="content-header">
    <div id="breadcrumb"> <a href="dashboard.aspx" title="" class="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></a> <a href="#" class="current"><%= Resources.LocalizedText.Settings %></a> </div>
      
      <uc1:notification ID="notification1" runat="server" />

      <div id="errorContainer">
        <p><%= Resources.LocalizedText.Settings_CorrectErrors %></p>
        <ul />
    </div>

    <h1><%= Resources.LocalizedText.Global_Settings %></h1>
  </div>
  <div class="container-fluid"><hr />

    <div class="row-fluid">
      <div class="span12">

          <div class="widget-box">
              <div class="widget-title">
                  <ul class="nav nav-tabs">
                      <li class="active"><a data-toggle="tab" href="#General"><%= Resources.LocalizedText.Settings_General %></a></li>
                      <li><a data-toggle="tab" href="#ActiveDirectory"><%= Resources.LocalizedText.Settings_ActiveDirectory %></a></li>
                      <li><a data-toggle="tab" href="#SecurityGroups"><%= Resources.LocalizedText.Settings_SecurityGroups %></a></li>
                      <li><a data-toggle="tab" href="#Billing"><%= Resources.LocalizedText.Settings_Billing %></a></li>
                      <li><a data-toggle="tab" href="#Exchange"><%= Resources.LocalizedText.Settings_Exchange %></a></li>
                      <li><a data-toggle="tab" href="#Lync"><%= Resources.LocalizedText.Settings_Lync %></a></li>
                      <li><a data-toggle="tab" href="#Citrix"><%= Resources.LocalizedText.Settings_Citrix %></a></li>
                      <li><a data-toggle="tab" href="#Support"><%= Resources.LocalizedText.Settings_SupportNotifications %></a></li>
                      <li><a data-toggle="tab" href="#Advanced"><%= Resources.LocalizedText.Settings_Advanced %></a></li>
                  </ul>
              </div>
              <div class="widget-content tab-content">

                  <!-- GENERAL -->
                  <div id="General" class="tab-pane active">
                      <div class="form-horizontal">
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Global_Name %></label>
                              <div class="controls">
                                  <asp:TextBox ID="txtCompanysName" runat="server" CssClass="required"></asp:TextBox><br />
                                  <%= Resources.LocalizedText.Settings_CompanyNameInfo %>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_LogoLoginScreen %></label>
                              <div class="controls">
                                  <asp:FileUpload ID="uploadCompanyLoginLogo" runat="server" />
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_LogoCorner %></label>
                              <div class="controls">
                                  <asp:FileUpload ID="uploadCompanyCornerLogo" runat="server" /><br />
                                  <%= Resources.LocalizedText.Settings_LogoCornerRecommended %><br />
                                  <%= Resources.LocalizedText.Settings_LogoCornerRecommendedWarn %>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_EnableResellers %></label>
                              <div class="controls">
                                  <asp:CheckBox ID="cbResellersEnabled" runat="server" Checked="true" />
                              </div>
                          </div>
                      </div>
                  </div>

                  <!-- ACTIVE DIRECTORY -->
                  <div id="ActiveDirectory" class="tab-pane">
                      <div class="form-horizontal">
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_HostingOU %></label>
                              <div class="controls">
                                  <asp:TextBox ID="txtBaseOrganizationalUnit" runat="server" CssClass="required"></asp:TextBox>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_UsersOU %></label>
                              <div class="controls">
                                  <asp:TextBox ID="txtUsersOU" runat="server"></asp:TextBox>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_PrimaryDC %></label>
                              <div class="controls">
                                  <asp:TextBox ID="txtDomainController" runat="server" CssClass="required"></asp:TextBox>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_Username %></label>
                              <div class="controls">
                                  <asp:TextBox ID="txtUsername" runat="server" CssClass="required"></asp:TextBox>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_Password %></label>
                              <div class="controls">
                                  <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="required"></asp:TextBox>
                              </div>
                          </div>
                      </div>
                  </div>


                  <!-- SECURITY GROUPS -->
                  <div id="SecurityGroups" class="tab-pane">
                      <div class="form-horizontal">
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_SuperAdmins %></label>
                              <div class="controls">
                                  <asp:TextBox ID="txtSuperAdmins" runat="server" CssClass="required"></asp:TextBox>
                               </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_BillingAdmins %></label>
                              <div class="controls">
                                  <asp:TextBox ID="txtBillingAdmins" runat="server" CssClass="required"></asp:TextBox>
                              </div>
                          </div>
                      </div>
                  </div>


                  <!-- BILLING -->
                  <div id="Billing" class="tab-pane">
                      <div class="form-horizontal">
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_CurrencySymbol %></label>
                              <div class="controls">
                                  <asp:DropDownList ID="ddlCurrenciesSymbol" runat="server" Width="250px"></asp:DropDownList>
                              </div>
                          </div>
                      </div>
                  </div>


                  <!-- EXCHANGE -->
                  <div id="Exchange" class="tab-pane">
                      <div class="form-horizontal">
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_ExchangeConnectionType %></label>
                              <div class="controls">
                                  <asp:DropDownList ID="ddlExchangeConnectionType" runat="server" Width="250px">
                                      <asp:ListItem Value="Kerberos" Text="Kerberos Authentication"></asp:ListItem>
                                      <asp:ListItem Value="Basic" Text="Basic Authentication"></asp:ListItem>
                                  </asp:DropDownList>
                                  &nbsp
                               </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_ExchangeVersion %></label>
                              <div class="controls">
                                  <asp:DropDownList ID="ddlExchangeVersion" runat="server" Width="250px">
                                      <asp:ListItem Value="2010" Text="Exchange 2010"></asp:ListItem>
                                      <asp:ListItem Value="2013" Text="Exchange 2013"></asp:ListItem>
                                  </asp:DropDownList>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_ExchangeFqdn %></label>
                              <div class="controls">
                                  <asp:TextBox ID="txtExchangeServer" runat="server" CssClass="required"></asp:TextBox>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_ExchangePFFqdn %></label>
                              <div class="controls">
                                  <asp:TextBox ID="txtExchangePublicFolderServer" runat="server"></asp:TextBox>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Setting_ExchangePFEnabled %></label>
                              <div class="controls">
                                  <asp:CheckBox ID="cbExchangePFEnabled" runat="server" Text="" />
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_ExchangeDatabases %></label>
                              <div class="controls">
                                  <asp:TextBox ID="txtExchDatabases" runat="server"></asp:TextBox>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_ExchangeSSLEnabled %></label>
                              <div class="controls">
                                  <asp:CheckBox ID="cbExchangeSSL" runat="server" Text="" />
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_ExchangeStatsEnabled %></label>
                              <div class="controls">
                                  <asp:CheckBox ID="cbExchangeStats" runat="server" Text="" />
                              </div>
                          </div>
                          <div class="control-group" id="divExchStats" runat="server">
                              <label class="control-label"></label>
                              <div id="Div1" class="controls" runat="server">
                                  <%= Resources.LocalizedText.Settings_ExchangeMBSizes %>
                                  <asp:TextBox ID="txtQueryMailboxSizesEveryXDays" runat="server" Width="20px"></asp:TextBox>
                                  
                                  <br />
                                  [<asp:Label ID="lbQueryMailboxSizesEveryXDays" runat="server" Text=""></asp:Label>]
                      
                                  <br />
                                  <br />

                                  <%= Resources.LocalizedText.Settings_ExchangeDBSizes %>
                                  <asp:TextBox ID="txtQueryMailboxDatabaseSizesEveryXDays" runat="server" Width="20px"></asp:TextBox>
                                  
                                  <br />
                                  [<asp:Label ID="lbQueryMailboxDatabaseSizesEveryXDays" runat="server" Text=""></asp:Label>]
                              </div>
                          </div>
                      </div>
                  </div>

                  <!-- LYNC -->
                  <div id="Lync" class="tab-pane">
                      <div class="form-horizontal">
                          <div class="control-group">
                              <label class="control-label"></label>
                              <div class="controls">
                                  <asp:CheckBox ID="cbLyncEnabled" runat="server" /> <%= Resources.LocalizedText.Settings_LyncEnabled %>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_LyncFrontEnd %></label>
                              <div class="controls">
                                  <asp:TextBox ID="txtLyncFrontEnd" runat="server"></asp:TextBox>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_LyncUserPool %></label>
                              <div class="controls">
                                  <asp:TextBox ID="txtLyncUserPool" runat="server"></asp:TextBox>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_LyncMeeting %></label>
                              <div class="controls">
                                  <div class="input-prepend">
                                    <span class="add-on">https://</span>
                                    <asp:TextBox ID="txtLyncMeetUrl" runat="server"></asp:TextBox>
                                  </div>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_LyncDialin %></label>
                              <div class="controls">
                                  <div class="input-prepend">
                                    <span class="add-on">https://</span>
                                    <asp:TextBox ID="txtLyncDialinUrl" runat="server"></asp:TextBox>
                                  </div>
                              </div>
                          </div>
                      </div>
                  </div>


                  <!-- CITRIX -->
                  <div id="Citrix" class="tab-pane">
                      <div class="form-horizontal">
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_CitrixEnabled %></label>
                              <div class="controls">
                                  <asp:CheckBox ID="cbCitrixEnabled" runat="server" Text="" />
                              </div>
                          </div>
                      </div>
                  </div>


                  <!-- Advanced -->
                  <div id="Advanced" class="tab-pane">
                      <div class="form-horizontal">
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_OnlyAllowSuperAdmins %></label>
                              <div class="controls">
                                  <asp:CheckBox ID="cbOnlySuperAdminLogin" runat="server" Text="" />
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_UseCustomNameAttribute %></label>
                              <div class="controls">
                                  <asp:CheckBox ID="cbAllowCustomNameAttribute" runat="server" Text="" />
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_BruteForceBlocking %></label>
                              <div class="controls">
                                  <asp:CheckBox ID="cbIPBlockingEnabled" runat="server" Text="" />
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_BruteForceFailedCount %></label>
                              <div class="controls">
                                  <asp:TextBox ID="txtIPBlockingFailedCount" runat="server"></asp:TextBox>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label"><%= Resources.LocalizedText.Settings_BruteForceLockout %></label>
                              <div class="controls">
                                  <asp:TextBox ID="txtIPBlockingLockedOutInMin" runat="server"></asp:TextBox>
                              </div>
                          </div>
                      </div>
                  </div>

                  <!-- SUPPORT NOTIFICATIONS -->
                  <div id="Support" class="tab-pane">
                      <div class="form-horizontal">
                          <div class="control-group">
                              <label class="control-label">Enable Mail Notifications:</label>
                              <div class="controls">
                                  <asp:CheckBox ID="cbSupportMailEnabled" runat="server" Text="" />
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label">Support Email:</label>
                              <div class="controls">
                                  <asp:TextBox ID="txtSupportMailAddress" runat="server"></asp:TextBox>
                               </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label">From:</label>
                              <div class="controls">
                                  <asp:TextBox ID="txtSupportMailFrom" runat="server"></asp:TextBox>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label">Mail Server:</label>
                              <div class="controls">
                                  <asp:TextBox ID="txtSupportMailServer" runat="server"></asp:TextBox>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label">Port:</label>
                              <div class="controls">
                                  <asp:TextBox ID="txtSupportMailPort" runat="server" Text="25"></asp:TextBox>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label">Username:</label>
                              <div class="controls">
                                  <asp:TextBox ID="txtSupportMailUsername" runat="server"></asp:TextBox>
                              </div>
                          </div>
                          <div class="control-group">
                              <label class="control-label">Password:</label>
                              <div class="controls">
                                  <asp:TextBox ID="txtSupportMailPassword" runat="server" TextMode="Password"></asp:TextBox>
                              </div>
                          </div>
                      </div>
                  </div>

              </div>
              </div>
          

      <!-- Save -->
      <div style="text-align: right">
          <asp:Button ID="btnSave" runat="server" Text="<%$ Resources:LocalizedText, Button_Save %>" CssClass="btn btn-success" 
              onclick="btnSave_Click" />
      </div>
      <!-- END -->

    </div>

  </div>
</div>

<script src="js/jquery.min.js"></script>
<script src="js/jquery.gritter.min.js"></script> 
<script src="js/jquery.ui.custom.js"></script> 
<script src="js/bootstrap.min.js"></script> 
<script src="js/jquery.uniform.js"></script> 
<script src="js/select2.min.js"></script> 
<script src="js/jquery.validate.js"></script> 
<script src="js/matrix.js"></script> 
<script src="js/matrix.form_validation.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('input[type=checkbox],input[type=radio],input[type=file]').uniform();

            $('select').select2();

            $("#form1").validate({
                ignore: "",
                rules: {
                    required: {
                        required: true
                    },
                    <%= txtQueryMailboxSizesEveryXDays.UniqueID %>: {
                        number: true
                    },
                    <%= txtQueryMailboxDatabaseSizesEveryXDays.UniqueID %>: {
                        number: true
                    },
                    <%= txtIPBlockingFailedCount.UniqueID %>: {
                        number: true
                    },
                    <%= txtIPBlockingLockedOutInMin.UniqueID %>: {
                        number: true
                    },
                    <%= txtSupportMailPort.UniqueID %>: {
                        required: true,
                        number: true
                    },
                    <%= txtSupportMailAddress.UniqueID %>: {
                        email: true
                    },
                    <%= txtSupportMailFrom.UniqueID %>: {
                        email: true
                    }
                },
                messages: {
                    <%= txtCompanysName.UniqueID %>: "You must enter your company's name.",
                    <%= txtBaseOrganizationalUnit.UniqueID %>: "You must enter the base organizational unit that will host all your resellers and companies.",
                    <%= txtDomainController.UniqueID %>: "You must enter the domain controller (FQDN or NETBIOS) that CloudPanel will communicate with.",
                    <%= txtUsername.UniqueID %>:  "You must enter the username (DOMAIN\Username) of the user that has rights to AD and Exchange.",
                    <%= txtPassword.UniqueID %>: "You must enter the password for the username.",
                    <%= txtSuperAdmins.UniqueID %>: "You must enter the security group(s) that have full rights to CloudPanel.",
                    <%= txtBillingAdmins.UniqueID %>: "You must enter the security group(s) that have full rights to billing information.",
                    <%= txtExchangeServer.UniqueID %>: "Please enter the Exchange server FQDN or NETBIOS name.",
                    <%= txtIPBlockingFailedCount.UniqueID %>: "You must enter the failed IP blocking count.",
                    <%= txtIPBlockingLockedOutInMin.UniqueID %>: "You must enter the failed IP blocking minutes.",
                    <%= txtSupportMailPort.UniqueID %>: "You must enter an integer value for the port number under support notifications.",
                    <%= txtSupportMailAddress.UniqueID %>: "You must enter a valid email address in the to field under support notifications.",
                    <%= txtSupportMailFrom.UniqueID %>: "You must enter a valid email address in the from field under support notifications."
                },
                errorClass: "help-inline",
                errorElement: "span",
                highlight: function (element, errorClass, validClass) {
                    $(element).parents('.control-group').addClass('error');
                },
                unhighlight: function (element, errorClass, validClass) {
                    $(element).parents('.control-group').removeClass('error');
                    $(element).parents('.control-group').addClass('success');
                },
                errorContainer: $('#errorContainer'),
                errorLabelContainer: $('#errorContainer ul'),
                wrapper: 'li'
            });

            $('#<%= cbExchangeStats.ClientID %>').change(function () {
                if (this.checked)
                    $('#<%= divExchStats.ClientID %>').fadeIn('slow');
                else
                    $('#<%= divExchStats.ClientID %>').fadeOut('slow');

        });
    });
    </script>
    </div>
</asp:Content>
