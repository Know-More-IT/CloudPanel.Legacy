<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="CloudPanel.company.users.edit" %>
<%@ Register src="../../controls/notification.ascx" tagname="notification" tagprefix="uc1" %>
<%@ MasterType VirtualPath="~/Default.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../../css/uniform.css" />
    <link rel="stylesheet" href="../../css/select2.css" />
    <style type="text/css">
        .CenterStyle {
            text-align: center;
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">
    <div id="content">
        <!--breadcrumbs-->
        <div id="content-header">
            <div id="breadcrumb">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/dashboard.aspx" title=""
                    CssClass="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/resellers.aspx" title=""
                    CssClass="tip-bottom"><i class="icon-user"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/company/overview.aspx"
                    CssClass="tip-bottom"><i class="icon-building"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="#"><i class="icon-user"></i><%= Resources.LocalizedText.Users %></asp:HyperLink>
            </div>
            <uc1:notification ID="notification1" runat="server" />
            <h1><%= Resources.LocalizedText.Users %></h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <div class="row-fluid">

                <!-- Current Users List -->
                <asp:Panel ID="panelUsers" runat="server" Visible="true" DefaultButton="btnAddUser">
                    <div class="span12">

                        <div style="float: right; margin-bottom: 15px;">
                            <asp:Button ID="btnAddUser" runat="server" Text="Add User" CssClass="btn btn-success" OnClick="btnAddUser_Click" />
                         </div>

                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5><%= Resources.LocalizedText.Users %></h5>
                            </div>
                            <div class="widget-content">
                                <table class="table table-bordered table-striped data-table">
                                    <thead>
                                        <tr>
                                            <th>
                                                <%= Resources.LocalizedText.DisplayName %>
                                            </th>
                                            <th>
                                                <%= Resources.LocalizedText.Username %>
                                            </th>
                                            <th>
                                                <%= Resources.LocalizedText.sAMAccountName %>
                                            </th>
                                            <th>
                                                <%= Resources.LocalizedText.Department %>
                                            </th>
                                            <th>
                                                <%= Resources.LocalizedText.Global_Created %>
                                            </th>
                                            <th>
                                                &nbsp;
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="repeater" runat="server" onitemcommand="repeater_ItemCommand">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <%# Eval("DisplayName") %>
                                                    </td>
                                                    <td>
                                                        <%# Eval("UserPrincipalName") %>
                                                    </td>
                                                    <td>
                                                        <%# Eval("sAMAccountName") %>
                                                    </td>
                                                    <td>
                                                        <%# Eval("Department") %>
                                                    </td>
                                                    <td>
                                                        <%# Eval("Created") %>
                                                    </td>
                                                    <td style="text-align: right">
                                                        <asp:Button ID="btnEditUser" runat="server" CssClass="btn btn-info" Text="<%$ Resources:LocalizedText, Button_Edit %>" CommandArgument='<%# Eval("UserPrincipalName") %>' CommandName="EditUser" />
                                                        <asp:Button ID="btnDeleteUser" runat="server" CssClass="btn btn-danger" Text="<%$ Resources:LocalizedText, Button_Delete %>" CommandArgument='<%# Eval("DisplayName") + ";" + Eval("UserPrincipalName") %>' CommandName="DeleteUser" /> &nbsp;
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Create User -->
                <asp:Panel ID="panelCreateUser" runat="server" Visible="false" DefaultButton="btnEditSaveUser">

                <div class="span12">
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.UserInformation %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label">* <%= Resources.LocalizedText.FirstName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtFirstName" runat="server" TabIndex="20"></asp:TextBox>
                                        <a href="#" title="<%= Resources.LocalizedText.USERHelp1 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MiddleName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtMiddleName" runat="server" TabIndex="21"></asp:TextBox>
                                        <a href="#" title="<%= Resources.LocalizedText.USERHelp2 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.LastName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtLastName" runat="server" TabIndex="22"></asp:TextBox>
                                        <a href="#" title="<%= Resources.LocalizedText.USERHelp3 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <% if (CloudPanel.Modules.Settings.Config.CustomNameAttribute) { %>
                                <div class="control-group">
                                    <label class="control-label">* <%= Resources.LocalizedText.FullName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtFullName" runat="server" TabIndex="22" onfocus="FocusSetInput(this)"></asp:TextBox>
                                        <a href="#" title="<%= Resources.LocalizedText.USERHelp4 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <% } %>
                                <div class="control-group">
                                    <label class="control-label">* <%= Resources.LocalizedText.DisplayName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtDisplayName" runat="server" name="required"  TabIndex="23" onfocus="FocusSetInput(this)"></asp:TextBox>
                                        <a href="#" title="<%= Resources.LocalizedText.USERHelp5 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.Department %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtDepartment" runat="server" TabIndex="24"></asp:TextBox>
                                        <a href="#" title="<%= Resources.LocalizedText.USERHelp6 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Login Information -->
                <div class="widget-box">
                    <div class="widget-title">
                        <span class="icon"><i class="icon icon-edit"></i></span>
                        <h5><%= Resources.LocalizedText.LoginInformation %></h5>
                    </div>
                    <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label">* <%= Resources.LocalizedText.LoginName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtLoginName" runat="server" name="required" TabIndex="24" onfocus="FocusSetLoginName()"></asp:TextBox>
                                        <asp:DropDownList ID="ddlLoginDomain" runat="server" TabIndex="25" Width="300px">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                <!-- Rights Information -->
                <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.UserRights %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.CompanyAdmin %></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbIsCompanyAdministrator" runat="server" TabIndex="26" />
                                        <a href="#" title="<%= Resources.LocalizedText.USERHelp9 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <div id="companyAdminPermissions" runat="server" style="display: none" class="control-group">
                                        <label class="control-label">Admin Rights:</label>
                                        <div class="controls">
                                            <asp:CheckBoxList ID="cbCompanyAdminPermissions" runat="server" CellPadding="15" CssClass="CenterStyle"
                                                AutoPostBack="false" RepeatDirection="Horizontal" RepeatColumns="4">
                                                <asp:ListItem Value="EnableExchange">Enable Exchange</asp:ListItem>
                                                <asp:ListItem Value="DisableExchange">Disable Exchange</asp:ListItem>
                                                <asp:ListItem Value="AddDomain">Add Domain</asp:ListItem>
                                                <asp:ListItem Value="DeleteDomain">Delete Domain</asp:ListItem>
                                                <asp:ListItem Value="ModifyAcceptedDomain">Modify Accepted Domain</asp:ListItem>
                                                <asp:ListItem Value="ImportUsers">Import Users</asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>
                                </div>
                                <% if (Master.IsSuperAdmin && CloudPanel.Modules.Settings.Config.ResellersEnabled) { %>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.ResellerAdmin %></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbIsResellerAdministrator" runat="server" />
                                        <a href="#" title="<%= Resources.LocalizedText.USERHelp10 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <% } %>
                            </div>
                        </div>
                    </div>
                
                <!-- Password Information -->
                <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.Password %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label">* <%= Resources.LocalizedText.Password %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtPassword1" runat="server" name="passwords" TextMode="Password" TabIndex="27"  AutoCompleteType="None" Autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">* <%= Resources.LocalizedText.PwdEnterAgain %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtPassword2" runat="server" name="passwords" TextMode="Password" TabIndex="28"  AutoCompleteType="None" Autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.PasswordNeverExpires %></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbPasswordNeverExpires" runat="server" TabIndex="29" />
                                    </div>
                                </div>
                                <div class="form-actions" style="text-align: right">
                                    <asp:Button ID="btnSaveUserCancel" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" 
                                        CssClass="btn btn-warning" onclick="btnSaveUserCancel_Click"/>
                                    <asp:Button ID="btnSaveUser" runat="server" Text="<%$ Resources:LocalizedText, Save %>" 
                                        CssClass="btn btn-success" onclick="btnSaveUser_Click" TabIndex="29"/>
                                </div>
                            </div>
                        </div>
                    </div>

                </asp:Panel>

                <!-- Edit User -->
                <asp:Panel ID="panelEditUser" runat="server" Visible="false" DefaultButton="btnEditSaveUser">
                <div class="span12">
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.Edit %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.LoginName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditLoginName" runat="server" TabIndex="20" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">SamAccountName</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditSamAccountName" runat="server" TabIndex="20" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">* <%= Resources.LocalizedText.FirstName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditFirstName" runat="server" TabIndex="20"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MiddleName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditMiddleName" runat="server" TabIndex="21"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.LastName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditLastName" runat="server" TabIndex="22"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">* <%= Resources.LocalizedText.DisplayName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditDisplayName" runat="server" name="required"  TabIndex="23"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.Department %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditDepartment" runat="server"  TabIndex="24"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- User Status Information -->
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.UserStatus %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.UserEnable %></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbEnableUser" runat="server" TabIndex="26" />
                                        <a href="#" title="<%= Resources.LocalizedText.USERHelp11 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">User Locked Out?</label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbUserLockedOut" runat="server" TabIndex="26" Enabled="false" /> &nbsp; 
                                        <asp:LinkButton ID="lnkUnlockAccount" runat="server" Visible="false" OnClick="lnkUnlockAccount_Click" Font-Underline="true" ForeColor="Blue" >Unlock Account</asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                <!-- Rights Information -->
                <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.UserRights %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.CompanyAdmin %></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbEditCompanyAdmin" runat="server" TabIndex="26" />
                                    </div>
                                </div>
                                <div id="editCompanyAdminPermissions" runat="server" style="display: none" class="control-group">
                                        <label class="control-label">Admin Rights:</label>
                                        <div class="controls">
                                            <asp:CheckBoxList ID="cbEditCompanyAdminPermissions" runat="server" CellPadding="15" CssClass="CenterStyle"
                                                AutoPostBack="false" RepeatDirection="Horizontal" RepeatColumns="4">
                                                <asp:ListItem Value="EnableExchange">Enable Exchange</asp:ListItem>
                                                <asp:ListItem Value="DisableExchange">Disable Exchange</asp:ListItem>
                                                <asp:ListItem Value="AddDomain">Add Domain</asp:ListItem>
                                                <asp:ListItem Value="DeleteDomain">Delete Domain</asp:ListItem>
                                                <asp:ListItem Value="ModifyAcceptedDomain">Modify Accepted Domain</asp:ListItem>
                                                <asp:ListItem Value="ImportUsers">Import Users</asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>
                                </div>
                                <% if (Master.IsSuperAdmin && CloudPanel.Modules.Settings.Config.ResellersEnabled) { %>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.ResellerAdmin %></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbEditResellerAdmin" runat="server" />
                                    </div>
                                </div>
                                <% } %>
                            </div>
                        </div>
                    </div>

                    <!-- Password Information -->
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.ResetPassword %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"></label>
                                    <div class="controls">
                                        <%= Resources.LocalizedText.IfYouDontWantToResetPassword %>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.Password %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditPwd1" runat="server" name="passwords" TextMode="Password" TabIndex="27" AutoCompleteType="None" Autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.PwdEnterAgain %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditPwd2" runat="server" name="passwords" TextMode="Password" TabIndex="28" AutoCompleteType="None" Autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.PasswordNeverExpires %></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbEditPwdNeverExpires" runat="server" TabIndex="29" />
                                    </div>
                                </div>
                                <div class="form-actions" style="text-align: right">
                                    <asp:Button ID="btnEditCancel" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" 
                                        CssClass="btn btn-warning" onclick="btnSaveUserCancel_Click"/>
                                    <asp:Button ID="btnEditSaveUser" runat="server" Text="<%$ Resources:LocalizedText, Save %>" 
                                        CssClass="btn btn-success" TabIndex="29" onclick="btnEditSaveUser_Click"/>
                                </div>
                            </div>
                        </div>
                    </div>

                </asp:Panel>

                <!-- Delete User -->
                <asp:Panel ID="panelDeleteUser" runat="server" Visible="false">
                <div class="span12">
                    <div class="widget-box">
                      <div class="widget-title"> <span class="icon"> <i class="icon-info-sign"></i> </span>
                        <h5><%= Resources.LocalizedText.Delete %> <asp:Label ID="lbDeleteDisplayName" runat="server" Text=""></asp:Label>?</h5>
                        <asp:HiddenField ID="hfDeleteUserPrincipalName" runat="server" />
                      </div>
                      <div class="widget-content">
                        <div class="error_ex">
                          <h3><%= Resources.LocalizedText.UserDeleteTitle %></h3>
                          <p><%= Resources.LocalizedText.UserDeleteInfo %></p>
                          <br />
                          <br />

                          <asp:Button ID="btnDeleteCancel" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" CssClass="btn btn-warning btn-big" onclick="btnDeleteCancel_Click" />
                          &nbsp;&nbsp;
                          <asp:Button ID="btnDeleteYes" runat="server" Text="<%$ Resources:LocalizedText, Button_Delete %>" CssClass="btn btn-danger btn-big" onclick="btnDeleteYes_Click" />

                          </div>
                      </div>
                    </div>
                  </div>
                </asp:Panel>

            </div>
        </div>
    </div>

    <script src="../../js/jquery.min.js"></script>
    <script src="../../js/jquery.gritter.min.js"></script>  
    <script src="../../js/jquery.validate.js"></script>
    <script src="../../js/jquery.ui.custom.js"></script> 
    <script src="../../js/bootstrap.min.js"></script> 
    <script src="../../js/jquery.uniform.js"></script> 
    <script src="../../js/select2.min.js"></script> 
    <script src="../../js/jquery.dataTables.min.js"></script> 
    <script src="../../js/matrix.js"></script> 
    <script src="../../js/matrix.tables.js"></script>

    <script type="text/javascript">

        $(function () {
            $('#gv1').dataTable({
                'bProcessing': true,
                'bServerSide': true,
                'sAjaxSource': 'Test1.ashx'
            });
        });

        function FocusSetInput(ctrl)
        {
            var firstName = $("#<%= txtFirstName.ClientID %>").val();
            var lastName = $("#<%= txtLastName.ClientID %>").val();

            $(ctrl).val(firstName + " " + lastName).toString().trim();
        }

        function FocusSetLoginName()
        {
            var firstName = $("#<%= txtFirstName.ClientID %>").val();
            var lastName = $("#<%= txtLastName.ClientID %>").val();

            $("#<%= txtLoginName.ClientID %>").val(firstName.toLowerCase().substring(0,1).trim() + lastName.toLowerCase().trim());
        }

        $(document).ready(function() {

            $('#<%= cbIsCompanyAdministrator.ClientID %>').change(function () {
                if (this.checked)
                    $('#<%= companyAdminPermissions.ClientID %>').fadeIn('slow');
                else
                    $('#<%= companyAdminPermissions.ClientID %>').fadeOut('slow');

            });

            $('#<%= cbEditCompanyAdmin.ClientID %>').change(function () {
                if (this.checked)
                    $('#<%= editCompanyAdminPermissions.ClientID %>').fadeIn('slow');
                else
                    $('#<%= editCompanyAdminPermissions.ClientID %>').fadeOut('slow');

            });

            $("#<%= btnSaveUser.ClientID %>").click(function() {
                $("#form1").validate({
                    rules: {
                        <%= txtFirstName.UniqueID %>: {
                            required: true,
                            maxlength: 50
                        },
                        <%= txtDisplayName.UniqueID %>: {
                            required: true,
                            maxlength: 100
                        },
                        <%= txtFullName.UniqueID %>: {
                            required: true
                        },
                        <%= txtLoginName.UniqueID %>: {
                            required: true,
                            maxlength: 35
                        },
                        <%= txtPassword1.UniqueID %>: {
                            required: true
                        },
                        <%= txtPassword2.UniqueID %>: {
                            required: true,
                            equalTo: "#<%= txtPassword1.ClientID %>"
                        }
                    },
                    errorClass: "help-inline",
                    errorElement: "span",
                    highlight: function (element, errorClass, validClass) {
                        $(element).parents('.control-group').removeClass('success');
                        $(element).parents('.control-group').addClass('error');
                    },
                    unhighlight: function (element, errorClass, validClass) {
                        $(element).parents('.control-group').removeClass('error');
                        $(element).parents('.control-group').addClass('success');
                    }
                });
            });

            $("#<%= btnEditSaveUser.ClientID %>").click(function() {
                $("#form1").validate({
                    rules: {
                        <%= txtEditFirstName.UniqueID %>: {
                            required: true,
                            maxlength: 50
                        },
                        <%= txtEditDisplayName.UniqueID %>: {
                            required: true,
                            maxlength: 100
                        },
                        <%= txtEditPwd2.UniqueID %>: {
                            equalTo: "#<%= txtEditPwd1.ClientID %>"
                        }
                    },
                    errorClass: "help-inline",
                    errorElement: "span",
                    highlight: function (element, errorClass, validClass) {
                        $(element).parents('.control-group').removeClass('success');
                        $(element).parents('.control-group').addClass('error');
                    },
                    unhighlight: function (element, errorClass, validClass) {
                        $(element).parents('.control-group').removeClass('error');
                        $(element).parents('.control-group').addClass('success');
                    }
                });
            });
        });
    </script>

</asp:Content>
