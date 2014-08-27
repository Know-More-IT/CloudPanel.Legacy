<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="existingusers.aspx.cs" Inherits="CloudPanel.company.import.existingusers" %>
<%@ Register src="../../controls/notification.ascx" tagname="notification" tagprefix="uc1" %>
<%@ MasterType VirtualPath="~/Default.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../../css/uniform.css" />
    <link rel="stylesheet" href="../../css/select2.css" />
    <link rel="stylesheet" href="../../css/jquery-ui.css" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">
    <div id="content">
        <!--breadcrumbs-->
        <div id="content-header">
            <div id="breadcrumb">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/dashboard.aspx" title=""
                    CssClass="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/resellers.aspx" title="Go to Resellers"
                    CssClass="tip-bottom"><i class="icon-user"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/company/overview.aspx"
                    CssClass="tip-bottom"><i class="icon-building"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="#"><i class="icon-user"></i><%= Resources.LocalizedText.Users %></asp:HyperLink>
            </div>
            <uc1:notification ID="notification1" runat="server" />
            <h1><%= Resources.LocalizedText.Users %></h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <div class="row-fluid">

                <!-- Current Users List -->
                <asp:Panel ID="panelUsers" runat="server" Visible="true" DefaultButton="btnImportSave">
                    <div class="span12">

                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5>Existing Users Found</h5>
                            </div>
                            <div class="widget-content">
                                <table class="table table-bordered table-striped">
                                    <thead>
                                        <tr>
                                            <th style="width: 16px; text-align: left"><input type="checkbox" id="title-checkbox" name="title-checkbox" /></th>
                                            <th>
                                                <%= Resources.LocalizedText.DisplayName %>
                                            </th>
                                            <th>
                                                <%= Resources.LocalizedText.Username %>
                                            </th>
                                            <th>
                                                <%= Resources.LocalizedText.sAMAccountName %>
                                            </th>
                                            <th>
                                                <%= Resources.LocalizedText.Department %>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="repeater" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <asp:CheckBox ID="cbImportUsers" runat="server" />
                                                    </td>
                                                    <td>
                                                        <%# Eval("DisplayName") %>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lbUPN" runat="server" Text='<%# Eval("UserPrincipalName") %>'></asp:Label>
                                                    </td>
                                                    <td>
                                                        <%# Eval("sAMAccountName") %>
                                                    </td>
                                                    <td>
                                                        <%# Eval("Department") %>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>

                        <div class="widget-box">
                            <div class="widget-content">
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <label class="control-label">Are these users already enabled for Exchange?</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbUsersAlreadyEnabledExchange" runat="server" />
                                        </div>
                                    </div>
                                    <div class="control-group" id="usersAlreadyExchEnabled4" style="display: none">
                                        <label class="control-label"></label>
                                        <div class="controls">
                                            When you import users and set that they are Exchange enabled, it will reconfigure those mailboxes according to the plan that you set.
                                            Also it will associate the users with this company and apply them to the new Address Book Policy, Global Address List, and other address lists.<br />
                                            Doing this will remove them from their current address lists and add them to the new ones.
                                        </div>
                                    </div>
                                    <div class="control-group" id="usersAlreadyExchEnabled" style="display: none">
                                        <label class="control-label"><%= Resources.LocalizedText.Plan %></label>
                                        <div class="controls">
                                            <asp:DropDownList ID="ddlMailboxPlans" runat="server" Width="225px"></asp:DropDownList><br />
                                            <span id="PlanDescription"> </span>
                                        </div>
                                    </div>
                                    <div class="control-group" id="usersAlreadyExchEnabled3" style="display: none">
                                        <label class="control-label"><%= Resources.LocalizedText.MailboxSize %></label>
                                        <div class="controls">
                                            <p>
                                                <label for="amount"><%= Resources.LocalizedText.CurrentSize %>
                                                    <asp:Label ID="lbMailboxSizeMB" runat="server" Text=""></asp:Label>
                                                    <asp:HiddenField ID="hfMailboxSizeMB" runat="server" />
                                                </label>
                                            </p>
                                            <p>
                                                <label for="amount"><%= Resources.LocalizedText.EstimatedCost %>
                                                    <%= CloudPanel.Modules.Settings.Config.CurrencySymbol %><span id="EstimatedCost"></span>
                                                </label>
                                            </p>
                                            <div id="slider-mailbox-size" style="width: 90%"></div>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"></label>
                                        <div class="controls" style="text-align: right">
                                            <asp:Button ID="btnImportSave" runat="server" Text="<%$ Resources:LocalizedText, Import %>" CssClass="btn btn-info" OnClick="btnImportSave_Click"/>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </asp:Panel>
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
    <script type="text/javascript">
        var selected = "";
        var currentSize = "<%= currentMailboxSize %>";

        $(document).ready(function () {
            
            $("#<%= cbUsersAlreadyEnabledExchange.ClientID %>").change(function () {
                if (this.checked) {
                    $("#usersAlreadyExchEnabled").fadeIn('slow');
                    $("#usersAlreadyExchEnabled2").fadeIn('slow');
                    $("#usersAlreadyExchEnabled3").fadeIn('slow');
                    $("#usersAlreadyExchEnabled4").fadeIn('slow');
                }
                else {
                    $("#usersAlreadyExchEnabled").fadeOut('slow');
                    $("#usersAlreadyExchEnabled2").fadeOut('slow');
                    $("#usersAlreadyExchEnabled3").fadeOut('slow');
                    $("#usersAlreadyExchEnabled4").fadeOut('slow');
                }
            });

            $newSelected = $("#<%= ddlMailboxPlans.ClientID %> option:selected");
            Calculate($newSelected.attr("Description"), $newSelected.attr("Price"), $newSelected.attr("Extra"), $newSelected.attr("Min"), $newSelected.attr("Max"));
            
            $("#<%= ddlMailboxPlans.ClientID %>").change(function () {

                $("#<%= ddlMailboxPlans.ClientID %> option:selected").each(function () {
                    Calculate($(this).attr("Description"), $(this).attr("Price"), $(this).attr("Extra"), $(this).attr("Min"), $(this).attr("Max"));
                });
            });

        });

        function Calculate(description, price, extra, min, max) {
            selected = description;

            var price = parseFloat(price);
            var extra = parseFloat(extra);
            var minRange = parseInt(min);
            var maxRange = parseInt(max);

            // Store original value in hidden field for post back
            $("#<%= hfMailboxSizeMB.ClientID %>").val(minRange);

            $("#slider-mailbox-size").slider("destroy");
            $("#slider-mailbox-size").slider({
                range: "max",
                min: minRange,
                max: maxRange,
                value: minRange,
                step: 256,
                slide: function (event, ui) {
                    $("#<%= lbMailboxSizeMB.ClientID %>").text(ui.value + "MB (" + ui.value / 1024 + "GB)");

                        // Store in hidden field for post back
                        $("#<%= hfMailboxSizeMB.ClientID %>").val(ui.value);

                        var total = CalculatePrice(ui.value, minRange, extra, price);
                        $("#EstimatedCost").text(total.toString());
                    }
                });
                $("#<%= lbMailboxSizeMB.ClientID %>").text(minRange.toString() + "MB (" + minRange / 1024 + "GB)");

            $("#EstimatedCost").text(price.toString());

            $("#PlanDescription").text(selected);
        }

        function CalculatePrice(currentValue, minValue, extraCostPerGB, price) {
            var added = parseInt(currentValue) - minValue;
            if (added > 0) {
                var perMB = ((extraCostPerGB / 1024) * (parseInt(currentValue) - minValue));
                var total = perMB + price;

                return total;
            }
            else {
                return price.toString();
            }
        }
    </script>
</asp:Content>
