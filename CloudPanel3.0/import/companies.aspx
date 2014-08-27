<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="companies.aspx.cs" Inherits="CloudPanel.import.companies" %>
<%@ Register src="../controls/notification.ascx" tagname="notification" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <link rel="stylesheet" href="css/uniform.css" />
    <link rel="stylesheet" href="css/select2.css" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">
      <div id="content">
        <!--breadcrumbs-->
        <div id="content-header">
            <div id="breadcrumb">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/dashboard.aspx" title="" CssClass="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.ImportCompany %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/resellers.aspx" title="" CssClass="tip-bottom"><i class="icon-user"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="#" CssClass="tip-bottom"><i class="icon-building"></i><%= Resources.LocalizedText.Global_Companies %></asp:HyperLink>
            </div>
            <uc1:notification ID="notification1" runat="server" />

            <h1><%= Resources.LocalizedText.ImportCompany %></h1>
        </div>
        <!--End-breadcrumbs-->
        <div class="container-fluid">
            <asp:Panel ID="panelCompanyEdit" runat="server" CssClass="row-fluid">
                <div class="span12">

                    <h5>
                        Before you import a company you MUST make sure the organizational unit is under the currently selected reseller. Also be sure that the uPNSuffixes attribute
                        under the OU contains the domains that the company uses. CloudPanel uses the uPNSuffixes attribute and will import the domains the company has.
                    </h5>

                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-user"></i></span>
                            <h5><%= Resources.LocalizedText.ImportCompany %></h5><br />
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">* <%= Resources.LocalizedText.DistinguishedName %></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtDistinguishedName" runat="server" name="required"></asp:TextBox><br />
                                </div>
                            </div>
                            <div class="form-actions" style="text-align: right">
                                <asp:Button ID="btnSubmitCompany" runat="server" Text="<%$ Resources:LocalizedText, Add %>" class="btn btn-success" OnClick="btnSubmitCompany_Click" />
                            </div>
                            </div>
                        </div>
                    </div>
                 </div>
            </asp:Panel>

            

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
    <script type="text/javascript">
        $(document).ready(function() {

            $("#<%= btnSubmitCompany.ClientID %>").click(function() {
                $("#form1").validate({
                    rules: {
                        <%= txtDistinguishedName.UniqueID %>: {
                            required: true
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
