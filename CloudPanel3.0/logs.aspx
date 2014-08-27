<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="logs.aspx.cs" Inherits="CloudPanel.logs" %>
<%@ Register src="controls/notification.ascx" tagname="notification" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="css/uniform.css" />
    <link rel="stylesheet" href="css/select2.css" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">
    <div id="content">
        <!--breadcrumbs-->
        <div id="content-header">
            <div id="breadcrumb">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/dashboard.aspx" title="" CssClass="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="#" title="" CssClass="tip-bottom"><i class="icon-warning-sign"></i><%= Resources.LocalizedText.Global_Logs %></asp:HyperLink>
            </div>
            <uc1:notification ID="notification1" runat="server" />

            <h1>Log</h1>
        </div>
        <!--End-breadcrumbs-->
        <div class="container-fluid">
            
            <asp:Panel ID="panelLogs" runat="server">
                <div class="widget-box">
                    <div class="widget-title">
                        <span class="icon"><i class="icon-building"></i></span>
                        <h5><%= Resources.LocalizedText.Global_Logs %></h5>
                    </div>
                    <div class="widget-content" style="font-size: 14px">
                        <asp:Literal ID="ltrLog" runat="server"></asp:Literal>
                    </div>
                </div>
            </asp:Panel>

        </div>
    </div>

    <script src="js/jquery.min.js"></script>
    <script src="js/jquery.gritter.min.js"></script> 
    <script src="js/jquery.ui.custom.js"></script> 
    <script src="js/bootstrap.min.js"></script> 
    <script src="js/jquery.uniform.js"></script> 
    <script src="js/select2.min.js"></script> 
    <script src="js/matrix.js"></script>
</asp:Content>
