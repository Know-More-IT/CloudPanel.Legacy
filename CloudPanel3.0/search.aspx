<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="search.aspx.cs" Inherits="CloudPanel.search" %>
<%@ MasterType VirtualPath="~/Default.Master" %>
<%@ Register Src="controls/notification.ascx" TagName="notification" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="css/uniform.css" />
    <link rel="stylesheet" href="css/select2.css" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">
    <div id="content">
        <!--breadcrumbs-->
        <div id="content-header">
            <div id="breadcrumb">
                <a href="dashboard.aspx" title="" class="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></a>
                <a href="#" class="current"><%= Resources.LocalizedText.Global_Search %></a>
            </div>

            <uc1:notification ID="notification1" runat="server" />

            <h1>Search Users</h1>
        </div>
        <!--End-breadcrumbs-->
        <div class="container-fluid">
            <hr />
            
                <div class="widget-box">
                    <div class="widget-title">
                        <span class="icon"><i class="icon-user"></i></span>
                        <h5><%= Resources.LocalizedText.Global_Results%></h5>
                    </div>
                    <div class="widget-content">
                        <table class="table table-bordered">
                            <thead>
                                <tr>
                                    <th>
                                        <%= Resources.LocalizedText.Global_DisplayName %>
                                    </th>
                                    <th>
                                        <%= Resources.LocalizedText.Global_LoginName %>
                                    </th>
                                    <th>
                                        <%= Resources.LocalizedText.Global_Name %>
                                    </th>
                                    <th>
                                        <%= Resources.LocalizedText.Global_Reseller %>
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="searchRepeater" runat="server" OnItemCommand="searchRepeater_ItemCommand">
                                    <ItemTemplate>
                                        <tr class="gradeX">
                                            <td>
                                                <asp:LinkButton ID="lnkDisplayName" runat="server" CommandName="SelectUser" CommandArgument='<%# Eval("UserPrincipalName")  %>'><%# Eval("DisplayName") %></asp:LinkButton>
                                            </td>
                                            <td>
                                                <%# Eval("UserPrincipalName") %>
                                            </td>
                                            <td>
                                                <%# Eval("CompanyName") %>
                                            </td>
                                            <td>
                                                <%# Eval("ResellerName") %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </div>

            <script src="js/jquery.min.js"></script>
            <script src="js/jquery.ui.custom.js"></script>
            <script src="js/bootstrap.min.js"></script>
            <script src="js/jquery.uniform.js"></script>
            <script src="js/select2.min.js"></script>
            <script src="js/jquery.dataTables.min.js"></script>
            <script src="js/matrix.js"></script>
            <script src="js/matrix.tables.js"></script>
            <script src="js/jquery.validate.js"></script>
            <script src="js/masked.js"></script>

</div>
    </div>
</asp:Content>
