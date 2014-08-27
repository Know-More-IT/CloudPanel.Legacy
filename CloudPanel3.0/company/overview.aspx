<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="overview.aspx.cs" Inherits="CloudPanel.company.overview" %>
<%@ Register src="../controls/notification.ascx" tagname="notification" tagprefix="uc1" %>
<%@ MasterType VirtualPath="~/Default.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../css/uniform.css" />
    <link rel="stylesheet" href="../css/select2.css" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">
    <div id="content">
        <!--breadcrumbs-->
        <div id="content-header">
            <div id="breadcrumb">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/dashboard.aspx" title="" CssClass="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/resellers.aspx" title="" CssClass="tip-bottom"><i class="icon-user"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="#" CssClass="tip-bottom"><i class="icon-building"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyCode %></asp:HyperLink>
            </div>

            <uc1:notification ID="notification1" runat="server" />
            <h1><%= Resources.LocalizedText.Overview %></h1>

        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
                <div class="widget-box">
                    <div class="widget-title">
                        <span class="icon"><i class="icon-book"></i></span>
                        <h5><%= Resources.LocalizedText.Details %></h5>
                    </div>
                    <div class="widget-content" style="font-size: 16px">
                        <table>
                            <tr>
                                <td><strong><%= Resources.LocalizedText.Global_Name %></strong></td>
                                <td style="padding-left: 20px"> <asp:Label ID="lbCompanyName" runat="server" Text="Know More IT"></asp:Label></td>
                            </tr>
                             <tr>
                                <td><strong><%= Resources.LocalizedText.Global_CompanyCode %></strong></td>
                                <td style="padding-left: 20px"> <asp:Label ID="lbCompanyCode" runat="server" Text="01/01/0001"></asp:Label></td>
                            </tr>
                             <tr>
                                <td><strong><%= Resources.LocalizedText.Telephone %></strong></td>
                                <td style="padding-left: 20px"> <asp:Label ID="lbCompanyPhoneNumber" runat="server" Text="1-000-000-0000"></asp:Label></td>
                            </tr>
                             <tr>
                                <td><strong><%= Resources.LocalizedText.Global_Created %></strong></td>
                                <td style="padding-left: 20px"> <asp:Label ID="lbCompanyCreated" runat="server" Text="01/01/0001"></asp:Label></td>
                            </tr>
                            <% if (Master.IsSuperAdmin || Master.IsResellerAdmin)
                               { %>
                            <tr>
                                <td><strong><%= Resources.LocalizedText.Plan %></strong></td>
                                <td style="padding-left: 20px">
                                    <asp:DropDownList ID="ddlCompanyPlans" runat="server" Width="220px" 
                                        onselectedindexchanged="ddlCompanyPlans_SelectedIndexChanged" AutoPostBack="True">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <% } %>
                        </table>
                    </div>
                </div>

                <div class="row-fluid">
                  <div class="span6">
                    <div class="widget-box">
                      <div class="widget-title"> <span class="icon"> <i class="icon-dashboard"></i> </span>
                        <h5><%= Resources.LocalizedText.Company %></h5>
                      </div>
                      <div class="widget-content">
                            <ul class="unstyled">
                              <li> 
                                <span class="icon24 icomoon-icon-arrow-up-2 green"></span> <%= Resources.LocalizedText.Users %> <span class="pull-right strong">
                                <asp:Label ID="lbStatsCompanyUsers" runat="server" Text="20/30"></asp:Label></span>

                                <asp:Panel ID="panelStatsCompanyUsers" runat="server" onprerender="panelStats_PreRender">
                                  <div runat="server" id="progressStatsCompanyUsers" style="width: 0%;" class="bar"></div>
                                </asp:Panel>
                              </li>
                              <li>
                                <span class="icon24 icomoon-icon-arrow-up-2 green"></span> <%= Resources.LocalizedText.Global_Domains %> <span class="pull-right strong">
                                <asp:Label ID="lbStatsCompanyDomains" runat="server" Text="20/30"></asp:Label></span>

                                <asp:Panel ID="panelStatsCompanyDomains" runat="server" onprerender="panelStats_PreRender">
                                  <div runat="server" id="progressStatsCompanyDomains" style="width: 0%;" class="bar"></div>
                                </asp:Panel>
                              </li>
                              <li>
                                <p>&nbsp;</p>
                                <p>&nbsp;</p>
                              </li>
                            </ul>
                        </div>
                    </div>
                  </div>
                  <div class="span6">
                    <div class="widget-box">
                      <div class="widget-title"> <span class="icon"> <i class="icon-dashboard"></i> </span>
                        <h5><%= Resources.LocalizedText.Exchange %></h5>
                      </div>
                      <div class="widget-content">
                            <ul class="unstyled">
                              <li> 
                                <span class="icon24 icomoon-icon-arrow-up-2 green"></span> <%= Resources.LocalizedText.Contacts %> <span class="pull-right strong">
                                <asp:Label ID="lbStatsExchangeContacts" runat="server" Text="20/30"></asp:Label></span>

                                <asp:Panel ID="panelStatsExchangeContacts" runat="server" onprerender="panelStats_PreRender">
                                  <div runat="server" id="progressStatsExchangeContacts" style="width: 0%;" class="bar"></div>
                                </asp:Panel>
                              </li>
                              <li>
                                <span class="icon24 icomoon-icon-arrow-up-2 green"></span> <%= Resources.LocalizedText.Mailboxes %> <span class="pull-right strong">
                                <asp:Label ID="lbStatsExchangeMailboxes" runat="server" Text="20/30"></asp:Label></span>

                                <asp:Panel ID="panelStatsExchangeMailboxes" runat="server" onprerender="panelStats_PreRender">
                                  <div runat="server" id="progressStatsExchangeMailboxes" style="width: 0%;" class="bar"></div>
                                </asp:Panel>
                              </li>
                              <li>
                                <span class="icon24 icomoon-icon-arrow-up-2 green"></span> <%= Resources.LocalizedText.DistributionGroups %> <span class="pull-right strong">
                                <asp:Label ID="lbStatsExchangeDistGroups" runat="server" Text="20/30"></asp:Label></span>

                                <asp:Panel ID="panelStatsExchangeDistGroups" runat="server" onprerender="panelStats_PreRender">
                                  <div runat="server" id="progressStatsExchangeDistGroups" style="width: 0%;" class="bar"></div>
                                </asp:Panel>
                              </li>
                            </ul>
                        </div>
                    </div>
                  </div>
                </div>

                <br />
                <hr />
                <br />
                
                <div class="row-fluid">
                  <div style="width: 75%; text-align: center; margin: 0 auto;">
                      <div class="widget-box">
                          <div class="widget-title"> <span class="icon"> <i class="icon-signal"></i> </span>
                            <h5><%= Resources.LocalizedText.ProductOverview %></h5>
                          </div>
                          <div class="widget-content">
                            <div class="bars"></div>
                          </div>
                        </div>
                  </div>
                </div>
        </div>
    </div>

    <script src="../js/jquery.min.js"></script><script src="../js/jquery.gritter.min.js"></script>  
    <script src="../js/jquery.ui.custom.js"></script> 
    <script src="../js/bootstrap.min.js"></script> 
    <script src="../js/jquery.uniform.js"></script> 
    <script src="../js/select2.min.js"></script> 
    <script src="../js/jquery.flot.min.js" type="text/javascript"></script>
    <script src="../js/jquery.dataTables.min.js"></script> 
    <script src="../js/matrix.js"></script> 
    <script src="../js/matrix.tables.js"></script>

    <script type="text/javascript">
        var data = [
                [0, <%= _statsUsers %>],
                [1, <%= _statsDomains %>],
                [2, <%= _statsExchMailboxes %>],
                [3, <%= _statsExchDistGroups %>],
                [4, <%= _statsExchContacts %>],
                [5, <%= _statsCitrixUsers %>],
                [6, <%= _statsLyncUsers %>]
            ];

            var dataset = [
                { data: data, color: "#5482FF" }
            ];

            var ticks = [
                [0, "<%= Resources.LocalizedText.Users %>"],
                [1, "<%= Resources.LocalizedText.Global_Domains %>"],
                [2, "<%= Resources.LocalizedText.Mailboxes %>"],
                [3, "<%= Resources.LocalizedText.DistributionGroups %>"],
                [4, "<%= Resources.LocalizedText.Contacts %>"],
                [5, "<%= Resources.LocalizedText.CitrixUsers %>"],
                [6, "<%= Resources.LocalizedText.LyncUsers %>"],
            ];

            var options = {
                series: {
                    bars: {
                        show: true
                    }
                },
                bars: {
                    align: "center",
                    barWidth: 0.8,
                    fillColor: { colors: [{ opacity: 0.5 }, { opacity: 1}] },
                    lineWidth: 2
                },
                xaxis: {
                    axisLabel: "<%= Resources.LocalizedText.ProductOverview %>",
                    axisLabelUseCanvas: true,
                    axisLabelFontSizePixels: 14,
                    axisLabelFontFamily: 'Verdana, Arial',
                    axisLabelPadding: 5,
                    ticks: ticks,
                    tickLength: 0,
                    autoscaleMargin: 0.01
                },
                yaxis: {
                    axisLabel: "<%= Resources.LocalizedText.Count %>",
                    axisLabelUseCanvas: true,
                    axisLabelPadding: 3
                },
                legend: {
                    noColumns: 0,
                    labelBoxBorderColor: "#000000",
                    position: "nw"
                },
                grid: {
                    hoverable: true,
                    borderWidth: 2,
                    backgroundColor: { colors: ["#ffffff", "#EDF5FF"] }
                }
            };

        $(document).ready(function () {
            $.plot($(".bars"), dataset, options);
        });
    </script>

</asp:Content>