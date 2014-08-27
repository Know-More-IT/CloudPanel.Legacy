<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="contacts.aspx.cs" Inherits="CloudPanel.company.exchange.contacts" %>
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
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/resellers.aspx" title="" CssClass="tip-bottom"><i class="icon-user"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="#" CssClass="tip-bottom"><i class="icon-building"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyCode %></asp:HyperLink>
                <a href="#" title="Exchange" class="tip-bottom"><i class="icon-cloud"></i><%= Resources.LocalizedText.Exchange %></a>
                <a href="#" title="Contacts" class="tip-bottom"><i class="icon-globe"></i><%= Resources.LocalizedText.Contacts %></a>
            </div>

            <uc1:notification ID="notification1" runat="server" />
            <h1><%= Resources.LocalizedText.Contacts %></h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <div class="row-fluid">

                 <asp:Panel ID="panelContacts" runat="server" Visible="true">
                    <div class="span12">

                        <div style="float: right">
                            <asp:Button ID="btnAddContact" runat="server" Text="Add Contact" CssClass="btn btn-success" OnClick="btnAddContact_Click"  />
                        </div>

                        <br />

                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5><%= Resources.LocalizedText.Contacts %></h5>
                            </div>
                            <div class="widget-content ">
                                <table class="table table-bordered table-striped data-table">
                                    <thead>
                                        <tr>
                                            <th>
                                                <%= Resources.LocalizedText.DisplayName %>
                                            </th>
                                            <th>
                                                <%= Resources.LocalizedText.Email %>
                                            </th>
                                            <th>
                                                <%= Resources.LocalizedText.Hidden %>
                                            </th>
                                            <th>
                                                &nbsp;
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="repeater" runat="server" OnItemCommand="repeater_ItemCommand">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <%# Eval("DisplayName") %>
                                                    </td>
                                                    <td>
                                                        <%# Eval("Email") %>
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="cbExchContactHidden" runat="server" Enabled="false" Checked='<%# Eval("Hidden") %>' />
                                                    </td>
                                                    <td style="text-align: right">
                                                        <asp:Button ID="btnEditContact" runat="server" CssClass="btn btn-info" Text="<%$ Resources:LocalizedText, Button_Edit %>" CommandArgument='<%# Eval("DistinguishedName") %>' CommandName="Edit" /> &nbsp;
                                                        <asp:Button ID="btnDeleteContact" runat="server" CssClass="btn btn-danger" Text="<%$ Resources:LocalizedText, Button_Delete %>" CommandArgument='<%# Eval("DistinguishedName") %>' CommandName="Delete" /> &nbsp;
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

                 <asp:Panel ID="panelCreateContact" runat="server" Visible="false">
                 <div class="span12">
                     <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.Create %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.DisplayName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtDisplayName" runat="server" TabIndex="27"></asp:TextBox>
                                        <asp:HiddenField ID="hfDistinguishedName" runat="server" />
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.Email %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEmailAddress" runat="server" TabIndex="28"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.Hidden %></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbContactHidden" runat="server" TabIndex="29" />
                                    </div>
                                </div>
                                <div class="form-actions" style="text-align: right">
                                    <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" CssClass="btn btn-warning" OnClick="btnCancel_Click" /> &nbsp;
                                    <asp:Button ID="btnSaveContact" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="btn btn-success" TabIndex="30" onclick="btnSaveContact_Click"/>
                                </div>
                            </div>
                        </div>
                    </div>

                 </asp:Panel>

            </div>
        </div>
    </div>

    <script src="../../js/jquery.min.js"></script><script src="../../js/jquery.gritter.min.js"></script>  
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

            $("#<%= btnSaveContact.ClientID %>").click(function() {
                $("#form1").validate({
                    rules: {
                        <%= txtDisplayName.UniqueID %>: {
                            required: true,
                            maxlength: 50
                        },
                        <%= txtEmailAddress.UniqueID %>: {
                            required: true,
                            email: true
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
