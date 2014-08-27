<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="customprices.aspx.cs" Inherits="CloudPanel.company.billing.customprices" %>
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
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/resellers.aspx" title=""
                    CssClass="tip-bottom"><i class="icon-user"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/company/overview.aspx"
                    CssClass="tip-bottom"><i class="icon-building"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="#"><i class="icon-money"></i><%= Resources.LocalizedText.CustomPrices %></asp:HyperLink>
            </div>

            <uc1:notification ID="notification1" runat="server" />
            <h1><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyName %> <%= Resources.LocalizedText.CustomPrices %></h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <div class="row-fluid">
                <div class="span12">

                    <!-- Exchange Custom Pricing -->
                    <asp:Panel ID="panelCustomPricingExchange" runat="server">
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5><%= Resources.LocalizedText.Exchange %></h5>
                            </div>
                            <div class="widget-content">
                                <table width="100%" class="table table-bordered table-striped">
                                    <thead>
                                        <tr>
                                            <th><%= Resources.LocalizedText.Plan %></th>
                                            <th><%= Resources.LocalizedText.DefaultPrice %></th>
                                            <th><%= Resources.LocalizedText.CustomPrice %></th>
                                            <th> </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:ListView ID="lstExchangePricing" runat="server" 
                                            onitemupdating="lstExchangePricing_ItemUpdating" 
                                            onitemediting="lstExchangePricing_ItemEditing"
                                            DataKeyNames="PlanID">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# Eval("DisplayName") %></td>
                                                    <td style="text-align: center"><%= CloudPanel.Modules.Settings.Config.CurrencySymbol %><%# Eval("Price") %></td>
                                                    <td style="text-align: center"><%# Eval("CustomPrice") %></td>
                                                    <td style="text-align: right"><asp:Button ID="btnExchEdit" runat="server" Text="<%$ Resources:LocalizedText, Button_Edit %>" CommandName="Edit" CssClass="btn btn-info" /></td>
                                                </tr>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <tr>
                                                    <td><%# Eval("DisplayName") %></td>
                                                    <td style="text-align: center">$<%# Eval("Price") %></td>
                                                    <td style="text-align: center">
                                                        <asp:TextBox ID="txtExchCustomPrice" runat="server" Text='<%# Eval("CustomPrice") %>' CssClass="MaskMoney"></asp:TextBox>
                                                    </td>
                                                    <td style="text-align: right"><asp:Button ID="btnExchEdit" runat="server" Text="<%$ Resources:LocalizedText, Update %>" CommandName="Update" CssClass="btn btn-success" /></td>
                                                </tr>
                                            </EditItemTemplate>
                                        </asp:ListView>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </asp:Panel>

                    <!-- Citrix Custom Pricing -->
                    <asp:Panel ID="panelCustomPricingCitrix" runat="server">
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5><%= Resources.LocalizedText.Citrix %></h5>
                            </div>
                            <div class="widget-content">
                                <table width="100%" class="table table-bordered table-striped">
                                    <thead>
                                        <tr>
                                            <th><%= Resources.LocalizedText.Plan %></th>
                                            <th><%= Resources.LocalizedText.DefaultPrice %></th>
                                            <th><%= Resources.LocalizedText.CustomPrice %></th>
                                            <th> </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:ListView ID="lstCitrixCustomPricing" runat="server" DataKeyNames="ID" OnItemUpdating="lstCitrixCustomPricing_ItemUpdating" OnItemEditing="lstCitrixCustomPricing_ItemEditing">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# Eval("DisplayName") %></td>
                                                    <td style="text-align: center"><%= CloudPanel.Modules.Settings.Config.CurrencySymbol %><%# Eval("Price") %></td>
                                                    <td style="text-align: center"><%# Eval("CustomPrice") %></td>
                                                    <td style="text-align: right"><asp:Button ID="btnCitrixEdit" runat="server" Text="<%$ Resources:LocalizedText, Button_Edit %>" CommandName="Edit" CssClass="btn btn-info" /></td>
                                                </tr>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <tr>
                                                    <td><%# Eval("DisplayName") %></td>
                                                    <td style="text-align: center"><%= CloudPanel.Modules.Settings.Config.CurrencySymbol %><%# Eval("Price") %></td>
                                                    <td style="text-align: center">
                                                        <asp:TextBox ID="txtCitrixCustomPrice" runat="server" Text='<%# Eval("CustomPrice") %>' CssClass="MaskMoney"></asp:TextBox>
                                                    </td>
                                                    <td style="text-align: right"><asp:Button ID="btnCitrixEdit" runat="server" Text="<%$ Resources:LocalizedText, Update %>" CommandName="Update" CssClass="btn btn-success" /></td>
                                                </tr>
                                            </EditItemTemplate>
                                        </asp:ListView>
                                    </tbody>
                                </table>
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
    <script src="../../js/masked.js"></script>
    <script src="../../js/jquery.maskMoney.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('.MaskMoney').maskMoney({ allowZero: true, defaultZero: true });
        });
    </script>
</asp:Content>
