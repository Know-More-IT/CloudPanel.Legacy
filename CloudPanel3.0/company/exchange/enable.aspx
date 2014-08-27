<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="enable.aspx.cs" Inherits="CloudPanel.company.exchange.enable" %>
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
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/dashboard.aspx" title="" CssClass="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/resellers.aspx" title="" CssClass="tip-bottom"><i class="icon-user"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="#" CssClass="tip-bottom"><i class="icon-building"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyCode %></asp:HyperLink>
                <a href="#" title="Exchange" class="tip-bottom"><i class="icon-cloud"></i><%= Resources.LocalizedText.Exchange %></a>
                <a href="#" title="Exchange" class="tip-bottom"><i class="icon-envelope"></i><%= Resources.LocalizedText.Modify %></a>
            </div>

            <uc1:notification ID="notification1" runat="server" />
            <h1><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyName %> </h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />

            <div id="enableExchange" class="row-fluid" runat="server">
                <div class="span12">
                <div class="widget-box">
                  <div class="widget-title"> <span class="icon"> <i class="icon-info-sign"></i> </span>
                    <h5><%= Resources.LocalizedText.EnableExchange %></h5>
                  </div>
                  <div class="widget-content ">
                    <div style="text-align: center">
                        <%= Resources.LocalizedText.EnableExchangeDesc %>
                    </div>

                    <div class="clearfix"></div>
                    <br />

                    <div style="text-align: center">
                        <asp:Button ID="btnEnableExchange" runat="server" Text="<%$ Resources:LocalizedText, Enable %>" CssClass="btn btn-success" onclick="btnEnableExchange_Click" />
                    </div>

                  </div>
                </div>
              </div>
            </div>

            <div id="disableExchange" class="row-fluid" runat="server" visible="false">
                <div class="span12">
                <div class="widget-box">
                  <div class="widget-title"> <span class="icon"> <i class="icon-info-sign"></i> </span>
                    <h5><%= Resources.LocalizedText.DisableExchange %></h5>
                  </div>
                  <div class="widget-content ">
                    <div style="text-align: center">
                        <%= Resources.LocalizedText.DisableExchangeDesc %>
                    </div>

                    <br />
                    <br />
                    <div style="text-align: center">
                        <%= Resources.LocalizedText.DisableExchangeDesc2 %> <asp:Label ID="lbDeleteLabel" runat="server" Text="Label"></asp:Label><br />
                        <asp:TextBox ID="txtDeleteLabel" runat="server" Width="220px"></asp:TextBox>
                    </div>

                    <div class="clearfix"></div>
                    <br />

                    <div style="text-align: center">
                        <asp:Button ID="btnDisableExchange" runat="server" Text="<%$ Resources:LocalizedText, Disable %>" CssClass="btn btn-danger" OnClick="btnDisableExchange_Click" />
                    </div>

                  </div>
                </div>
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
    <script src="../../js/matrix.popover.js"></script>
</asp:Content>
