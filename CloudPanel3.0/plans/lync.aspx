<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="lync.aspx.cs" Inherits="CloudPanel.plans.lync" %>
<%@ Register src="../controls/notification.ascx" tagname="notification" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../css/uniform.css" />
    <link rel="stylesheet" href="../css/select2.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphSideBar" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">
        <div id="content">
        <!--breadcrumbs-->
        <div id="content-header">
            <div id="breadcrumb">
                <a href="../dashboard.aspx" title="Go to Dashboard" class="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></a>
                <a href="#"><i class="icon-edit"></i><%= Resources.LocalizedText.Global_Plans %></a>
                <a href="#" class="active"><i class="icon-comment"></i><%= Resources.LocalizedText.Global_Lync %></a>
            </div>

            <uc1:notification ID="notification1" runat="server" />

            <h1><%= CloudPanel.Modules.Settings.Config.HostersName %> <%= Resources.LocalizedText.Global_Lync %> <%= Resources.LocalizedText.Global_Plans %></h1>
        </div>
        <!--End-breadcrumbs-->

            <div class="container-fluid">
                <hr />
                <div class="row-fluid">
                    <div class="span12">

                        <!-- External Access Policy -->
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon icon-edit"></i></span>
                                <h5>External Access Policy</h5>
                            </div>
                            <div class="widget-content nopadding">
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <label class="control-label">Enable Federation Access</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbEnableFederationAccess" runat="server" Checked="false" />
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Enable Public Cloud Access</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbEnablePublicCloudAccess" runat="server" Checked="false" />
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Enable Public Cloud Audio/Video Access</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbEnablePublicCloudAudioVideoAccess" runat="server" Checked="false" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Conferencing Policy -->
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon icon-edit"></i></span>
                                <h5>Conferencing Policy</h5>
                            </div>
                            <div class="widget-content nopadding">
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <label class="control-label">Enable IP Video</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbConferencingEnableIPVideo" runat="server" Checked="false" />
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Max Meeting Size</label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtConferencingMaxMeetingSize" runat="server" Text="250"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Conferencing Policy -->
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon icon-edit"></i></span>
                                <h5>Mobility Policy</h5>
                            </div>
                            <div class="widget-content nopadding">
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <label class="control-label">Enable Mobility</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbMobilityEnable" runat="server" Checked="true" />
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Enable IP Audio / Video</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbMobilityEnableIPAudioVide" runat="server" Checked="true" />
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Enable Call via Work</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbMobilityEnableCallViaWork" runat="server" Checked="true" />
                                        </div>
                                    </div>
                                </div>
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
