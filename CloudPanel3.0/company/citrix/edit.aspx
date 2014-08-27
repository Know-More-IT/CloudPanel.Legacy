<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="CloudPanel.company.citrix.edit" %>
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
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/dashboard.aspx" title=""
                    CssClass="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/resellers.aspx" title="Go to Resellers"
                    CssClass="tip-bottom"><i class="icon-user"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/company/overview.aspx"
                    CssClass="tip-bottom"><i class="icon-building"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="#"><i class="icon-cloud"></i><%= Resources.LocalizedText.Citrix %></asp:HyperLink>
            </div>
            <uc1:notification ID="notification1" runat="server" />
            <h1><%= Resources.LocalizedText.Citrix %></h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <div class="row-fluid">
                <div class="span12">

                    <!-- CHOOSE CITRIX APP -->
                    <asp:Panel ID="panelEnableUsers" runat="server">
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5><%= Resources.LocalizedText.ChooseCitrixApplication %></h5>
                            </div>
                            <div class="widget-content" style="text-align: center">
                                <asp:DropDownList ID="ddlCitrixApplications" runat="server" AutoPostBack="true"
                                     OnSelectedIndexChanged="ddlCitrixApplications_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                        </div>
                    </asp:Panel>

                    <!-- USER LIST -->
                    <asp:Panel ID="panelUserList" runat="server" Visible="false">
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-group"></i></span>
                                <h5><%= Resources.LocalizedText.Users %></h5>
                            </div>
                            <div class="widget-content">
                                <table class="table table-bordered table-striped with-check">
                                    <thead>
                                        <tr>
                                            <th>
                                                <input type="checkbox" id="title-checkbox" name="title-checkbox"></th>
                                                <th><%= Resources.LocalizedText.DisplayName %></th>
                                                <th><%= Resources.LocalizedText.FirstName %></th>
                                                <th><%= Resources.LocalizedText.LastName %></th>
                                                <th><%= Resources.LocalizedText.LoginName %></th>
                                                <th><%= Resources.LocalizedText.Department %></th>
                                                <th><%= Resources.LocalizedText.Global_Created %></th>
                                            </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="repeaterUsers" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <asp:CheckBox ID="cbCitrixEnabledUsers" runat="server" Checked='<%# Eval("IsChecked") %>' />
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lbDisplayName" runat="server" Text='<%# Eval("DisplayName") %>'></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lbFirstName" runat="server" Text='<%# Eval("Firstname") %>'></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lbLastName" runat="server" Text='<%# Eval("Lastname") %>'></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lbLoginName" runat="server" Text='<%# Eval("UserPrincipalName") %>'></asp:Label>
                                                        <asp:Label ID="lbUserID" runat="server" Text='<%# Eval("ID") %>' Visible="false"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <%# Eval("Department") %>
                                                    </td>
                                                    <td>
                                                        <%# Eval("Created") %>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>

                        <div class="widget-box">
                                <div class="widget-content" style="text-align: right">
                                    <asp:Button ID="btnSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="btn btn-info" OnClick="btnSave_Click"/>
                                </div>
                        </div>
                    </asp:Panel>

                </div>
            </div>
        </div>
    </div>

    <script src="../../js/jquery.min.js"></script>
    <script src="../../js/jquery.gritter.min.js"></script> 
    <script src="../../js/jquery.ui.custom.js"></script> 
    <script src="../../js/bootstrap.min.js"></script> 
    <script src="../../js/jquery.uniform.js"></script> 
    <script src="../../js/select2.min.js"></script> 
    <script src="../../js/jquery.flot.min.js"></script>
    <script src="../../js/jquery.dataTables.min.js"></script> 
    <script src="../../js/matrix.js"></script> 
    <script src="../../js/matrix.tables.js"></script>

</asp:Content>
