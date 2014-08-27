<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="CloudPanel.dashboard" %>
<%@ MasterType VirtualPath="~/Default.Master" %>
<%@ Register src="controls/notification.ascx" tagname="notification" tagprefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title><%= Resources.LocalizedText.Global_Dashboard %></title>
    <script src="js/jquery.min.js"></script>
    <script src="js/jquery.gritter.min.js"></script> 
    <script src="js/jquery.ui.custom.js"></script>
    <script src="js/highcharts.js"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">

    <div id="content">
        <!--breadcrumbs-->
        <div id="content-header">
            <div id="breadcrumb">
                <a href="dashboard.aspx" title="Go to Dashboard" class="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></a>
            </div>

            <uc1:notification ID="notification1" runat="server" />
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">

            <!--Chart-box-->
            <div class="row-fluid">
                <div class="widget-box">
                    <div class="widget-title bg_lg">
                        <span class="icon"><i class="icon-signal"></i></span>
                        <h5></h5>
                    </div>
                    <div class="widget-content">
                        <div class="row-fluid">
                            <div class="span9">
                                <asp:Literal ID="ltrChart" runat="server"></asp:Literal>
                            </div>
                            <div class="span3">
                                <ul class="site-stats">
                                    <li class="bg_lh"><i class="icon-user"></i><strong><asp:Label ID="lbStatisticsTotalUsers" runat="server" Text="0"></asp:Label></strong><small><%= Resources.LocalizedText.Global_Users %></small></li>
                                    <li class="bg_lh"><i class="icon-plus"></i><strong><asp:Label ID="lbStatisticsTotalMailboxes" runat="server" Text="0"></asp:Label></strong> <small><%= Resources.LocalizedText.Global_Mailboxes %></small></li>
                                    <li class="bg_lh"><i class="icon-building"></i><strong><asp:Label ID="lbStatisticsTotalCompanies" runat="server" Text="0"></asp:Label></strong> <small><%= Resources.LocalizedText.Global_Companies %></small></li>
                                    <li class="bg_lh"><i class="icon-hdd"></i><strong><asp:Label ID="lbStatisticsTotalExchangeStorage" runat="server" Text="0"></asp:Label></strong> <small><%= Resources.LocalizedText.Dashboard_MailboxSpaceAllocated %></small></li>
                                    <li class="bg_lh"><i class="icon-globe"></i><strong><asp:Label ID="lbStatisticsDomains" runat="server" Text="0"></asp:Label></strong> <small><%= Resources.LocalizedText.Global_Domains %></small></li>
                                    <li class="bg_lh"><i class="icon-envelope"></i><strong><asp:Label ID="lbStatisticsAcceptedDomains" runat="server" Text="0"></asp:Label></strong> <small><%= Resources.LocalizedText.Global_AcceptedDomains %></small></li>
                                    <% if (CloudPanel.Modules.Settings.Config.CitrixEnabled) { %>
                                        <li class="bg_lh"><i class="icon-desktop"></i><strong><asp:Label ID="lbStatisticsTotalCitrixUsers" runat="server" Text="0"></asp:Label></strong> <small><%= Resources.LocalizedText.Global_Citrix %></small></li> 
                                    <% } %>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!--End-Chart-box-->
            <hr />
            <div class="row-fluid">

                <% if (Master.IsSuperAdmin && Master.ExchangeStatistics) { %>
                <div class="span6">
                    <div class="widget-box">
                        <div class="widget-title bg_ly" data-toggle="collapse" href="#collapseG2">
                            <span class="icon"><i class="icon-chevron-down"></i></span>
                            <h5></h5>
                        </div>
                        <div class="widget-content nopadding collapse in" id="Div2">
                            <asp:Literal ID="ltrDbSize" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>
                <% } %>
                <% if (Master.IsSuperAdmin || Master.IsResellerAdmin) { %>
                <div class="span6">
                    <div class="widget-box">
                        <div class="widget-title bg_ly" data-toggle="collapse" href="#collapseG3">
                            <span class="icon"><i class="icon-chevron-down"></i></span>
                            <h5></h5>
                        </div>
                        <div class="widget-content nopadding collapse in" id="Div1">
                            <asp:Literal ID="ltrBar" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>
                <% } %>

            </div>
        </div>
    </div>

    <style type="text/css">
        #flotcontainer {
            width: 600px;
            height: 200px;
            text-align: center;
            margin: 0 auto;
        }
     </style>

    <script src="js/bootstrap.min.js"></script> 
    <script src="js/jquery.uniform.js"></script> 
    <script src="js/select2.min.js"></script> 
    <script src="js/jquery.dataTables.min.js"></script> 
    <script src="js/matrix.js"></script> 
    <script src="js/matrix.tables.js"></script>

</asp:Content>


