<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="reports.aspx.cs" Inherits="CloudPanel.reporting.reports" %>
<%@ Register src="../controls/notification.ascx" tagname="notification" tagprefix="uc1" %>
<%@ MasterType VirtualPath="~/Default.Master" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../css/uniform.css" />
    <link rel="stylesheet" href="../css/select2.css" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">
       <div id="content">
        <!--breadcrumbs-->
        <div id="content-header">
            <div id="breadcrumb">
                <a href="../dashboard.aspx" title="Go to Dashboard" class="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></a>
                <a href="#"><i class="icon-edit"></i><%= Resources.LocalizedText.Reports %></a>
            </div>

            <uc1:notification ID="notification1" runat="server" />
            <h1><%= Resources.LocalizedText.Reports %></h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <rsweb:ReportViewer ID="dummyReportViewer" runat="server" Visible="false" en></rsweb:ReportViewer>


            <div class="row-fluid">
                <div class="span12">

                    <!-- EXCHANGE -->
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.Global_Reports %></h5>
                        </div>
                        <div class="widget-content">
                            <div class="form-horizontal">

                                <div class="control-group">
                                    <label class="control-label">
                                        <a href="#"><asp:Image ID="Image1" runat="server" ImageUrl="~/img/reporting/dynamic_reports.jpg" Width="250px" BorderWidth="1px" /></a>
                                    </label>
                                    <div class="controls">
                                        <h4>Exchange Report</h4>
                                        <p>
                                            This report will give you detailed information about the Exchange plans that each company is using.
                                        </p>
                                        <p>
                                            <asp:Button ID="btnRunExchangeReport" runat="server" Text="Run Report" CssClass="btn btn-success" OnClick="btnRunExchangeReport_Click" />
                                        </p>
                                    </div>

                                    <div style="clear: both"></div>
                                </div>

                                <div class="control-group">
                                    <label class="control-label">
                                        <a href="#"><asp:Image ID="Image2" runat="server" ImageUrl="~/img/reporting/dynamic_reports.jpg" Width="250px" BorderWidth="1px" /></a>
                                    </label>
                                    <div class="controls">
                                        <h4>Citrix Report</h4>
                                        <p>
                                            This report will give you detailed information about the Citrix applications and servers that each company is using
                                        </p>
                                        <p>
                                            <asp:Button ID="btnRunCitrixReport" runat="server" Text="Run Report" CssClass="btn btn-success" OnClick="btnRunCitrixReport_Click" />
                                        </p>
                                    </div>

                                    <div style="clear: both"></div>
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
    <script type="text/javascript">
        $(document).ready(function () {
            $('#lightbox').click(function () {
                $('#lightbox').hide(200);
            });
        });
    </script>
</asp:Content>
