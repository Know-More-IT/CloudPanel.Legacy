<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="mailbox.aspx.cs" Inherits="CloudPanel.plans.mailbox" %>
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
                <a href="../dashboard.aspx" title="" class="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></a>
                <a href="#"><i class="icon-edit"></i><%= Resources.LocalizedText.Plans %></a>
                <a href="#" class="active"><i class="icon-envelope"></i><%= Resources.LocalizedText.MailboxPlans %></a>
            </div>

            <uc1:notification ID="notification1" runat="server" />

            <h1><%= Resources.LocalizedText.MailboxPlans %></h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <div class="row-fluid">
                <div class="span12">
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.Modify %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.Plan %></label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddlMailboxPlans" runat="server" Width="220px" 
                                            AutoPostBack="True" 
                                            onselectedindexchanged="ddlMailboxPlans_SelectedIndexChanged">
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
                                    <label class="control-label"><%= Resources.LocalizedText.Description %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtDescription" runat="server" name="required" TextMode="MultiLine"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MaxRecipients %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtMaxRecipients" runat="server" name="required"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.KeepDeletedItems %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtKeepDeletedItems" runat="server" name="required"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MailboxSize %></label>
                                    <div class="controls">
                                        <div class="input-prepend">
                                            <span class="add-on">MB</span>
                                            <asp:TextBox ID="txtMailboxSize" runat="server" name="required" CssClass="span11"></asp:TextBox>
                                        </div>
                                        <br />Converted Gigabytes: <asp:Label ID="lbMailboxSize" runat="server" Text="" ForeColor="Green"></asp:Label>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MaxMailboxSize %></label>
                                    <div class="controls">
                                        <div class="input-prepend">
                                            <span class="add-on">MB</span>
                                            <asp:TextBox ID="txtMaxMailboxSize" runat="server" name="required" CssClass="span11"></asp:TextBox>
                                        </div>
                                        <br />Converted Gigabytes: <asp:Label ID="lbMaxMailboxSize" runat="server" Text="" ForeColor="Green"></asp:Label>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MaxSendSize %></label>
                                    <div class="controls">
                                        <div class="input-prepend">
                                            <span class="add-on">KB</span>
                                            <asp:TextBox ID="txtSendSize" runat="server" name="required" CssClass="span11"></asp:TextBox>
                                        </div>
                                        <br />Converted Megabytes: <asp:Label ID="lbSendSize" runat="server" Text="" ForeColor="Green"></asp:Label>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MaxReceiveSize %></label>
                                    <div class="controls">
                                        <div class="input-prepend">
                                            <span class="add-on">KB</span>
                                            <asp:TextBox ID="txtReceiveSize" runat="server" name="required" CssClass="span11"></asp:TextBox>
                                        </div>
                                        <br />Converted Megabytes: <asp:Label ID="lbReceiveSize" runat="server" Text="" ForeColor="Green"></asp:Label>
                                    </div>
                                </div>
                                <div class="control-group">
                                  <label class="control-label"><%= Resources.LocalizedText.Features %></label>
                                  <div class="controls">
                                    <label>
                                      <asp:CheckBox ID="cbEnablePOP3" runat="server" />
                                      <%= Resources.LocalizedText.POP3 %>
                                    </label>
                                    <label>
                                      <asp:CheckBox ID="cbEnableIMAP" runat="server" />
                                      <%= Resources.LocalizedText.IMAP %>
                                        </label>
                                    <label>
                                      <asp:CheckBox ID="cbEnableOWA" runat="server" />
                                      <%= Resources.LocalizedText.OWA %>
                                        </label>
                                     <label>
                                      <asp:CheckBox ID="cbEnableMAPI" runat="server" />
                                      <%= Resources.LocalizedText.MAPI %>
                                         </label>
                                    <label>
                                      <asp:CheckBox ID="cbEnableAS" runat="server" />
                                       <%= Resources.LocalizedText.ActiveSync %>
                                        </label>
                                    <label>
                                      <asp:CheckBox ID="cbEnableECP" runat="server" />
                                      <%= Resources.LocalizedText.ECP %>
                                        </label>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.CostPerMailbox %></label>
                                    <div class="controls">
                                        <div class="input-prepend">
                                            <span class="add-on"><%= CloudPanel.Modules.Settings.Config.CurrencySymbol %></span>
                                            <asp:TextBox ID="txtCost" runat="server" name="required" CssClass="span11" Text="0.00"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.PricePerMailbox %></label>
                                    <div class="controls">
                                        <div class="input-prepend">
                                            <span class="add-on"><%= CloudPanel.Modules.Settings.Config.CurrencySymbol %></span>
                                            <asp:TextBox ID="txtPrice" runat="server" name="required" CssClass="span11" Text="0.00"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.PricePerAdditionalGB %></label>
                                    <div class="controls">
                                        <div class="input-prepend">
                                            <span class="add-on"><%= CloudPanel.Modules.Settings.Config.CurrencySymbol %></span>
                                            <asp:TextBox ID="txtPriceAdditionalGB" runat="server" name="required" CssClass="span11" Text="0.00"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.SpecificCompany %></label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddlSpecificCompany" runat="server" Width="220px">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-actions" style="text-align: right">
                                    <asp:Button ID="btnDeletePlan" runat="server" Text="<%$ Resources:LocalizedText, Button_Delete %>" 
                                        class="btn btn-danger cancel" onclick="btnDeletePlan_Click" />
                                    <asp:Button ID="btnUpdatePlan" runat="server" Text="<%$ Resources:LocalizedText, Save %>" 
                                        class="btn btn-success" onclick="btnUpdatePlan_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script src="../js/jquery.min.js"></script>
    <script src="../js/jquery.gritter.min.js"></script>  
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
            $('#<%= txtCost.ClientID %>').maskMoney({allowZero: true, defaultZero: true});
            $('#<%= txtPrice.ClientID %>').maskMoney({allowZero: true, defaultZero: true});
            $('#<%= txtPriceAdditionalGB.ClientID %>').maskMoney({allowZero: true, defaultZero: true});

            $('#<%= txtMailboxSize.ClientID %>').keyup(function() {
                var currentValue = this.value;
                document.getElementById('<%= lbMailboxSize.ClientID %>').innerText = (currentValue / 1024);
            });

            $('#<%= txtMaxMailboxSize.ClientID %>').keyup(function() {
                var currentValue = this.value;
                document.getElementById('<%= lbMaxMailboxSize.ClientID %>').innerText = (currentValue / 1024);
            });

            $('#<%= txtSendSize.ClientID %>').keyup(function() {
                var currentValue = this.value;
                document.getElementById('<%= lbSendSize.ClientID %>').innerText = (currentValue / 1024);
            });

            $('#<%= txtReceiveSize.ClientID %>').keyup(function() {
                var currentValue = this.value;
                document.getElementById('<%= lbReceiveSize.ClientID %>').innerText = (currentValue / 1024);
            });

            $("#<%= btnUpdatePlan.ClientID %>").click(function() {
                $("#form1").validate({
                    rules: {
                        <%= txtPlanName.UniqueID %>: {
                            required: true
                        },
                        <%= txtDescription.UniqueID %>: {
                            required: true
                        },
                        <%= txtMaxRecipients.UniqueID %>: {
                            required: true,
                            number: true
                        },
                        <%= txtKeepDeletedItems.UniqueID %>: {
                            required: true,
                            number: true
                        },
                        <%= txtMailboxSize.UniqueID %>: {
                            required: true,
                            number: true
                        },
                        <%= txtMaxMailboxSize.UniqueID %>: {
                            required: true,
                            number: true
                        },
                        <%= txtSendSize.UniqueID %>: {
                            required: true,
                            number: true
                        },
                        <%= txtReceiveSize.UniqueID %>: {
                            required: true,
                            number: true
                        },
                        <%= txtCost.UniqueID %>: {
                            required: true,
                            number: true,
                            maxlength: 7
                        },
                        <%= txtPrice.UniqueID %>: {
                            required: true,
                            number: true,
                            maxlength: 7
                        },
                        <%= txtPriceAdditionalGB.UniqueID %>: {
                            required: true,
                            number: true,
                            maxlength: 7
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
