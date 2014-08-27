<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="organization.aspx.cs" Inherits="CloudPanel.plans.organization" %>
<%@ Register src="../controls/notification.ascx" tagname="notification" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../css/uniform.css" />
    <link rel="stylesheet" href="../css/select2.css" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">
    <div id="content">
        <!--breadcrumbs-->
        <div id="content-header">
            <div id="breadcrumb">
                <a href="../dashboard.aspx" title="Go to Dashboard" class="tip-bottom"><i class="icon-home"></i>Dashboard</a>
                <a href="#"><i class="icon-edit"></i>Plans</a>
                <a href="#" class="active"><i class="icon-building"></i>Company Plans</a>
            </div>

            <uc1:notification ID="notification1" runat="server" />

            <h1><%= CloudPanel.Modules.Settings.Config.HostersName %> Company Plans</h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <div class="row-fluid">
                <div class="span12">
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.Details %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.Plan %></label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddlCompanyPlan" runat="server" Width="220px" 
                                            AutoPostBack="True" 
                                            onselectedindexchanged="ddlCompanyPlan_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.DisplayName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtPlanName" runat="server" name="required"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MaxUsers %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtMaxUsers" runat="server" name="number"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MaxDomains %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtMaxDomains" runat="server" name="number"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Exchange Information -->
                <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.Exchange %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MaxMailboxes %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtMaxMailboxes" runat="server" name="number"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MaxContacts %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtMaxContacts" runat="server" name="number"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MaxDistributionLists %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtMaxDistributionLists" runat="server" name="number"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MaxResourceMailboxes %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtMaxResourceMailboxes" runat="server" name="number"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MaxMailEnabledPublicFolders %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtMaxMailPublicFolders" runat="server" name="number"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                <!-- END Exchange Information -->

                <!-- Citrix Information -->
                <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.Citrix %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MaxUsers %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtMaxCitrixUsers" runat="server" CssClass="valNumber"></asp:TextBox>
                                    </div>
                                </div>
                                <% if (false) { %>
                                <div class="control-group">
                                    <label class="control-label">Max Apps per User</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtMaxCitrixAppPerUser" runat="server" name="number" Text="0"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Max Servers per User</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtMaxCitrixSvrPerUser" runat="server" name="number" Text="0"></asp:TextBox>
                                    </div>
                                </div>
                                <% } %>
                                <div class="form-actions" style="text-align: right">
                                    <asp:Button ID="btnDeletePlan" runat="server" Text="<%$ Resources:LocalizedText, Button_Delete %>" 
                                        class="btn btn-danger cancel" onclick="btnDeletePlan_Click" />
                                    <asp:Button ID="btnUpdatePlan" runat="server" Text="<%$ Resources:LocalizedText, Save %>" 
                                        class="btn btn-success" onclick="btnUpdatePlan_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                <!-- END Citrix Information -->

             </div>
        </div>
    </div>

    <script src="../js/jquery.min.js"></script><script src="../js/jquery.gritter.min.js"></script>  
    <script src="../js/jquery.ui.custom.js"></script> 
    <script src="../js/bootstrap.min.js"></script> 
    <script src="../js/jquery.uniform.js"></script> 
    <script src="../js/select2.min.js"></script> 
    <script src="../js/jquery.dataTables.min.js"></script> 
    <script src="../js/matrix.js"></script> 
    <script src="../js/matrix.tables.js"></script>
    <script src="../js/jquery.validate.js"></script>
    <script src="../js/masked.js"></script>
    <script src="../js/jquery.maskMoney.js"></script>

    <script type="text/javascript">
        $(document).ready(function() {

            $("#<%= btnUpdatePlan.ClientID %>").click(function() {
                $("#form1").validate({
                    rules: {
                        <%= txtPlanName.UniqueID %>: {
                            required: true
                        },
                        <%= txtMaxUsers.UniqueID %>: {
                            required: true,
                            number: true,
                            min: 1
                        },
                        <%= txtMaxDomains.UniqueID %>: {
                            required: true,
                            number: true,
                            min: 1
                        },
                        <%= txtMaxMailboxes.UniqueID %>: {
                            required: true,
                            number: true
                        },
                        <%= txtMaxContacts.UniqueID %>: {
                            required: true,
                            number: true
                        },
                        <%= txtMaxDistributionLists.UniqueID %>: {
                            required: true,
                            number: true
                        },
                        <%= txtMaxResourceMailboxes.UniqueID %>: {
                            required: true,
                            number: true
                        },
                        <%= txtMaxMailPublicFolders.UniqueID %>: {
                            required: true,
                            number: true
                        },
                        <%= txtMaxCitrixUsers.UniqueID %>: {
                            required: true,
                            number: true
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
