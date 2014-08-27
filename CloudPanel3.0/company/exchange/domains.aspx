<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="domains.aspx.cs" Inherits="CloudPanel.company.exchange.domains" %>
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
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/dashboard.aspx" title="" CssClass="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/resellers.aspx" title="Go to Resellers" CssClass="tip-bottom"><i class="icon-user"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="#" CssClass="tip-bottom"><i class="icon-building"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyCode %></asp:HyperLink>
                <a href="#" title="" class="tip-bottom"><i class="icon-cloud"></i><%= Resources.LocalizedText.Exchange %></a>
                <a href="#" title="" class="tip-bottom"><i class="icon-globe"></i><%= Resources.LocalizedText.AcceptedDomains %></a>
            </div>

            <uc1:notification ID="notification1" runat="server" />
            <h1><%= Resources.LocalizedText.AcceptedDomains %></h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <div class="row-fluid">
                <div class="span12">

                <div class="widget-box">
                  <div class="widget-title"> <span class="icon"> <i class="icon-globe"></i> </span>
                    <h5><%= Resources.LocalizedText.AcceptedDomains%></h5>
                  </div>
                  <div class="widget-content nopadding">
                    <table class="table table-bordered table-striped">
                      <thead>
                        <tr>
                          <th><%= Resources.LocalizedText.Domain %></th>
                          <th><%= Resources.LocalizedText.Modify%></th>
                        </tr>
                      </thead>
                      <tbody>
                          <asp:Repeater ID="repeaterAcceptedDomains" runat="server" onitemcommand="repeaterAcceptedDomains_ItemCommand">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="lbDomainName" runat="server" Text='<%# Eval("DomainName") %>'></asp:Label>
                                    </td>
                                    <td style="text-align: right">
                                        <% if (CloudPanel.classes.Authentication.PermModifyAcceptedDomain) { %>
                                            <asp:Button ID="btnChangeAcceptedDomain" runat="server" CssClass='<%# Eval("IsAcceptedDomain").ToString() == "True" ? "btn btn-warning" : "btn btn-success" %>' Text='<%# Eval("IsAcceptedDomain").ToString() == "True" ? "Disable" : "Enable" %>' CommandArgument='<%# Eval("DomainName") %>' CommandName="ChangeAcceptedDomainStatus" />
                                        <% } %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                          </asp:Repeater>
                      </tbody>
                    </table>
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
    <script src="../../js/jquery.flot.min.js" type="text/javascript"></script>
    <script src="../../js/jquery.dataTables.min.js"></script> 
    <script src="../../js/matrix.js"></script> 
    <script src="../../js/matrix.tables.js"></script>

</asp:Content>
