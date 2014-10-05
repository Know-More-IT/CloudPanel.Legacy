<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="CloudPanel.company.domains.edit" %>
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
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/dashboard.aspx" CssClass="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/resellers.aspx" CssClass="tip-bottom"><i class="icon-user"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="#" CssClass="tip-bottom"><i class="icon-building"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyCode %></asp:HyperLink>
                <a href="#" title="Exchange" class="tip-bottom"><i class="icon-globe"></i><%= Resources.LocalizedText.Global_Domains %></a>
            </div>

            <uc1:notification ID="notification1" runat="server" />
            <h1><%= Resources.LocalizedText.Global_Domains %></h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <div class="row-fluid">
                <div class="span12">

                 <div style="float: right; margin-bottom: 15px;">
                    <asp:Button ID="btnAddDomain" runat="server" Text="Add Domain" CssClass="btn btn-success" OnClick="btnAddDomain_Click"/>
                 </div>

                 <asp:Panel ID="panelCurrentDomains" runat="server">
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-globe"></i></span>
                                <h5><%= Resources.LocalizedText.CurrentDomains %></h5>
                            </div>
                            <div class="widget-content nopadding">
                                <table class="table table-bordered table-striped">
                                    <thead>
                                        <tr>
                                            <th><%= Resources.LocalizedText.DomainName %></th>
                                            <th style="width: 75px; text-align: right">&nbsp;</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="repeaterDomains" runat="server" OnItemCommand="repeaterDomains_ItemCommand">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <%# Eval("DomainName") %>
                                                    </td>
                                                    <td>
                                                        <% if (CloudPanel.classes.Authentication.PermDeleteDomain) { %>
                                                        <asp:Button ID="btnDeleteDomain" runat="server" Text="<%$ Resources:LocalizedText, Button_Delete %>" CssClass="btn btn-danger" CommandName="Delete" CommandArgument='<%# Eval("DomainName") %>' />
                                                        <% } %>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </asp:Panel>

                 <asp:Panel ID="panelAddDomain" runat="server" Visible="false">
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-globe"></i></span>
                                <h5><%= Resources.LocalizedText.Add %></h5>
                            </div>
                            <div class="widget-content">
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.DomainName %></label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtDomainName" runat="server" name="required"></asp:TextBox>
                                            <a href="#" title="<%= Resources.LocalizedText.DOMAINHelp1 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="widget-box">
                            <div class="widget-content" style="text-align: right">
                                <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                                &nbsp;
                                    <asp:Button ID="btnSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="btn btn-info" OnClick="btnSave_Click" />
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
    <script src="../../js/jquery.validate.js"></script>
    <script src="../../js/bootstrap.min.js"></script> 
    <script src="../../js/jquery.uniform.js"></script> 
    <script src="../../js/select2.min.js"></script> 
    <script src="../../js/jquery.flot.min.js" type="text/javascript"></script>
    <script src="../../js/jquery.dataTables.min.js"></script> 
    <script src="../../js/matrix.js"></script> 
    <script src="../../js/matrix.tables.js"></script>

    <script type="text/javascript">
        $(document).ready(function() {

            $("#<%= btnSave.ClientID %>").click(function () {
                $("#form1").validate({
                    rules: {
                        <%= txtDomainName.UniqueID %>: { required: true }
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
