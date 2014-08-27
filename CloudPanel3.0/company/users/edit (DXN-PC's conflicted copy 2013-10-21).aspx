<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="CloudPanel.company.users.edit" %>
<%@ Register src="../../controls/notification.ascx" tagname="notification" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../../css/uniform.css" />
    <link rel="stylesheet" href="../../css/select2.css" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">
    <div id="content">
        <!--breadcrumbs-->
        <div id="content-header">
            <div id="breadcrumb">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/dashboard.aspx" title="Go to Dashboard"
                    CssClass="tip-bottom"><i class="icon-home"></i>Dashboard</asp:HyperLink>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/resellers.aspx" title="Go to Resellers"
                    CssClass="tip-bottom"><i class="icon-user"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/company/overview.aspx"
                    CssClass="tip-bottom"><i class="icon-building"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="#"><i class="icon-user"></i>Users</asp:HyperLink>
            </div>
            <uc1:notification ID="notification1" runat="server" />
            <h1><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyName %> Users</h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <div class="row-fluid">

                <!-- Current Users List -->
                <asp:Panel ID="panelUsers" runat="server" Visible="true">
                    <div class="span12">
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5>Users</h5>
                                <span class="label label-info">
                                    <asp:LinkButton ID="lnkCreateUsers" runat="server" ForeColor="White" 
                                    onclick="lnkCreateUsers_Click">Click Here to Create New User</asp:LinkButton>
                                </span>
                            </div>
                            <div class="widget-content ">
                                <table class="table table-bordered table-striped data-table">
                                    <thead>
                                        <tr>
                                            <th>
                                                Display Name
                                            </th>
                                            <th>
                                                Login Username
                                            </th>
                                            <th>
                                                sAMAccountName
                                            </th>
                                            <th>
                                                Department
                                            </th>
                                            <th>
                                                Created
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
                                                        <asp:Button ID="btnDeleteUser" runat="server" CssClass="btn-danger" Text="Delete" CommandArgument='<%# Eval("DisplayName") + ";" + Eval("UserPrincipalName") %>' CommandName="DeleteUser" /> &nbsp;
                                                        <asp:Button ID="btnEditUser" runat="server" CssClass="btn-info" Text="Edit" CommandArgument='<%# Eval("UserPrincipalName") %>' CommandName="EditUser" />
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
                <asp:Panel ID="panelCreateUser" runat="server" Visible="false">

                <div class="span12">
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5>User Information</h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label">First Name</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtFirstName" runat="server" TabIndex="20"></asp:TextBox>
                                        <a href="#" title="First name of the user. This field is required." class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Middle Name</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtMiddleName" runat="server" TabIndex="21"></asp:TextBox>
                                        <a href="#" title="Middle name of the user. This field is not required." class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Last Name</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtLastName" runat="server" TabIndex="22"></asp:TextBox>
                                        <a href="#" title="Last name of the user. This field is not required" class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Display Name</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtDisplayName" runat="server" name="required"  TabIndex="23"></asp:TextBox>
                                        <a href="#" title="The display name will appear in Outlook and other areas. This field IS required." class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Department</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtDepartment" runat="server" TabIndex="24" AutoCompleteType="None"></asp:TextBox>
                                        <a href="#" title="The department field can help you organize your users and make them easier to search." class="tip-right"><i class="icon-question-sign"></i></a>
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
                        <h5>Login Information</h5>
                    </div>
                    <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label">Login Name</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtLoginName" runat="server" name="required" TabIndex="24"></asp:TextBox>
                                        <a href="#" title="The login name is the characters before the @ symbol. Example: jdoe" class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Login Domain</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddlLoginDomain" runat="server" TabIndex="25">
                                        </asp:DropDownList>
                                        <a href="#" title="The login domain is the characters after the @ symbol. Example: knowmoreit.com" class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"></label>
                                    <div class="controls">
                                        <span style="color: Red; font-weight: bold">
                                            The login name cannot be changed. Please make sure you set it up correctly the first time.
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                <!-- Rights Information -->
                <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5>User Rights</h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label">Company Administrator</label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbIsCompanyAdministrator" runat="server" TabIndex="26" />
                                        <a href="#" title="This setting will give the user rights to manage the company they belong to." class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <% if (CloudPanel.Modules.Settings.Authentication.IsSuperAdmin()) { %>
                                <div class="control-group">
                                    <label class="control-label">Reseller Administrator</label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbIsResellerAdministrator" runat="server" />
                                        <a href="#" title="This setting will give the user rights to manage all companies within the same reseller." class="tip-right"><i class="icon-question-sign"></i></a>
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
                            <h5>Password</h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label">Password</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtPassword1" runat="server" name="passwords" TextMode="Password" TabIndex="27" AutoCompleteType="None"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Enter Again</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtPassword2" runat="server" name="passwords" TextMode="Password" TabIndex="28" AutoCompleteType="None"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-actions" style="text-align: right">
                                    <asp:Button ID="btnSaveUserCancel" runat="server" Text="Cancel" 
                                        CssClass="btn btn-warning" onclick="btnSaveUserCancel_Click"/>
                                    <asp:Button ID="btnSaveUser" runat="server" Text="Save User" 
                                        CssClass="btn btn-success" onclick="btnSaveUser_Click" TabIndex="29"/>
                                </div>
                            </div>
                        </div>
                    </div>

                </asp:Panel>

                <!-- Edit User -->
                <asp:Panel ID="panelEditUser" runat="server" Visible="false">
                <div class="span12">
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5>Edit User Information</h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label">Login Name</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditLoginName" runat="server" TabIndex="20"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">First Name</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditFirstName" runat="server" TabIndex="20"></asp:TextBox>
                                        <a href="#" title="First name of the user. This field is required." class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Middle Name</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditMiddleName" runat="server" TabIndex="21"></asp:TextBox>
                                        <a href="#" title="Middle name of the user. This field is not required." class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Last Name</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditLastName" runat="server" TabIndex="22"></asp:TextBox>
                                        <a href="#" title="Last name of the user. This field is not required" class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Display Name</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditDisplayName" runat="server" name="required"  TabIndex="23"></asp:TextBox>
                                        <a href="#" title="The display name will appear in Outlook and other areas. This field IS required." class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Department</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditDepartment" runat="server"  TabIndex="24"></asp:TextBox>
                                        <a href="#" title="The department field can help you organize your users and make them easier to search." class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Rights Information -->
                <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5>User Rights</h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label">Company Administrator</label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbEditCompanyAdmin" runat="server" TabIndex="26" />
                                        <a href="#" title="This setting will give the user rights to manage the company they belong to." class="tip-right"><i class="icon-question-sign"></i></a>
                                    </div>
                                </div>
                                <% if (CloudPanel.Modules.Settings.Authentication.IsSuperAdmin()) { %>
                                <div class="control-group">
                                    <label class="control-label">Reseller Administrator</label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbEditResellerAdmin" runat="server" />
                                        <a href="#" title="This setting will give the user rights to manage all companies within the same reseller." class="tip-right"><i class="icon-question-sign"></i></a>
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
                            <h5>Reset Password</h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label">Password</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditPwd1" runat="server" name="passwords" TextMode="Password" TabIndex="27"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Enter Again</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEditPwd2" runat="server" name="passwords" TextMode="Password" TabIndex="28"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-actions" style="text-align: right">
                                    <asp:Button ID="btnEditCancel" runat="server" Text="Cancel" 
                                        CssClass="btn btn-warning" onclick="btnSaveUserCancel_Click"/>
                                    <asp:Button ID="btnEditSaveUser" runat="server" Text="Save User" 
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
                        <h5>Delete <asp:Label ID="lbDeleteDisplayName" runat="server" Text=""></asp:Label>?</h5>
                        <asp:HiddenField ID="hfDeleteUserPrincipalName" runat="server" />
                      </div>
                      <div class="widget-content">
                        <div class="error_ex">
                          <h3>Are you sure you want to delete this user?</h3>
                          <p>Deleting this user is not reversable. Once the user is deleted, everything associated with the user will be gone forever.</p>
                          <br />
                          <br />

                          <asp:Button ID="btnDeleteCancel" runat="server" Text="No!" CssClass="btn btn-warning btn-big" onclick="btnDeleteCancel_Click" />
                          &nbsp;&nbsp;
                          <asp:Button ID="btnDeleteYes" runat="server" Text="Yes! Delete!" CssClass="btn btn-danger btn-big" onclick="btnDeleteYes_Click" />

                          </div>
                      </div>
                    </div>
                  </div>
                </asp:Panel>

            </div>
        </div>
    </div>

    <script src="../../js/jquery.min.js"></script> 
    <script src="../../js/jquery.validate.js"></script>
    <script src="../../js/jquery.ui.custom.js"></script> 
    <script src="../../js/bootstrap.min.js"></script> 
    <script src="../../js/jquery.uniform.js"></script> 
    <script src="../../js/select2.min.js"></script> 
    <script src="../../js/jquery.dataTables.min.js"></script> 
    <script src="../../js/matrix.js"></script> 
    <script src="../../js/matrix.tables.js"></script>

    <script type="text/javascript">
        $(document).ready(function() {

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
