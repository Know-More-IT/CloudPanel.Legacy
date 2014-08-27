<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="groups.aspx.cs" Inherits="CloudPanel.company.exchange.groups" %>

<%@ Register Src="../../controls/notification.ascx" TagName="notification" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../../css/uniform.css" />
    <link rel="stylesheet" href="../../css/select2.css" />
    <style type="text/css">
        tr.odd.row_selected td {
            background-color: skyblue;
        }

        tr.even.row_selected td {
            background-color: skyblue;
        }

        tr.row_selected td {
            background-color: skyblue;
        }

        .dataTables_filter {
            position: inherit;
        }

        table.table thead .sorting,
        table.table thead .sorting_asc,
        table.table thead .sorting_desc,
        table.table thead .sorting_asc_disabled,
        table.table thead .sorting_desc_disabled {
            cursor: pointer;
            *cursor: hand;
        }

        .datatable-scroll {
            overflow-x: auto;
            overflow-y: visible;
        }

        .dataTables_wrapper table thead {
        }

        .table-wrapper {
            position: relative;
        }

        .table-scroll {
            height: 250px;
            overflow: auto;
            margin-top: 20px;
        }

        .table-wrapper table {
            width: 100%;
        }

            .table-wrapper table * {
            }

            .table-wrapper table thead th .text {
                position: absolute;
                top: -20px;
                z-index: 2;
                height: 20px;
                width: 35%;
            }

        .hidden {
            display: none;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphSideBar" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">
    <div id="content">
        <!--breadcrumbs-->
        <div id="content-header">
            <div id="breadcrumb">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/dashboard.aspx" title="" CssClass="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/resellers.aspx" title="" CssClass="tip-bottom"><i class="icon-user"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="#" CssClass="tip-bottom"><i class="icon-building"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyCode %></asp:HyperLink>
                <a href="#" title="" class="tip-bottom"><i class="icon-cloud"></i><%= Resources.LocalizedText.Exchange %></a>
                <a href="#" title="" class="tip-bottom"><i class="icon-group"></i><%= Resources.LocalizedText.DistributionGroups %></a>
            </div>

            <uc1:notification ID="notification1" runat="server" />
            <h1><%= Resources.LocalizedText.DistributionGroups %></h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <div class="row-fluid">

                <asp:Panel ID="panelGroupList" runat="server">
                    <div class="span12">

                        <div style="float: right">
                            <asp:Button ID="btnCreateNewGroup" runat="server" Text="Add Group" CssClass="btn btn-success" OnClick="btnCreateNewGroup_Click" />
                        </div>

                        <br />

                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-group"></i></span>
                                <h5><%= Resources.LocalizedText.DistributionGroups %></h5>
                            </div>
                            <div class="widget-content">
                                <table class="table table-bordered table-striped">
                                    <thead>
                                        <tr>
                                            <th><%= Resources.LocalizedText.DisplayName %></th>
                                            <th><%= Resources.LocalizedText.EmailAddress %></th>
                                            <th><%= Resources.LocalizedText.Hidden %></th>
                                            <th>&nbsp;</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="repeaterGroups" runat="server" OnItemCommand="repeaterGroups_ItemCommand">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <asp:LinkButton ID="lnkEditGroup" runat="server" CommandName="EditGroup" CommandArgument='<%# Eval("PrimarySmtpAddress") %>'><%# Eval("DisplayName") %></asp:LinkButton>
                                                    </td>
                                                    <td>
                                                        <%# Eval("PrimarySmtpAddress") %>
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="cbIsGroupHidden" runat="server" Checked='<%# Eval("Hidden") %>' />
                                                    </td>
                                                    <td style="text-align: right">
                                                        <asp:Button ID="btnDeleteGroup" runat="server" Text="<%$ Resources:LocalizedText, Button_Delete %>" CssClass="btn btn-danger" CommandName="DeleteGroup" CommandArgument='<%# Eval("DisplayName") + "|" + Eval("PrimarySmtpAddress") %>' />
                                                        &nbsp;
                                                    <asp:Button ID="btnEditGroup" runat="server" Text="<%$ Resources:LocalizedText, Button_Edit %>" CssClass="btn btn-info" CommandName="EditGroup" CommandArgument='<%# Eval("PrimarySmtpAddress") %>' />
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

                <asp:Panel ID="panelGroupDelete" runat="server">
                    <div class="span12">
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5><%= Resources.LocalizedText.DeleteDistributionGroup %>
                                    <asp:Label ID="lbDeleteDisplayName" runat="server" Text=""></asp:Label></h5>
                                <asp:HiddenField ID="hfDeleteDistributionGroup" runat="server" />
                            </div>
                            <div class="widget-content">
                                <div class="error_ex">
                                    <h3><%= Resources.LocalizedText.DeleteDistributionGroupAreYouSure %></h3>
                                    <p><%= Resources.LocalizedText.DeleteDistributionGroupInfo %></p>
                                    <br />
                                    <br />

                                    <asp:Button ID="btnDeleteCancel" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" CssClass="btn btn-warning btn-big" OnClick="btnDeleteCancel_Click" />
                                    &nbsp;&nbsp;
                                    <asp:Button ID="btnDeleteYes" runat="server" Text="<%$ Resources:LocalizedText, Button_Delete %>" CssClass="btn btn-danger btn-big" OnClick="btnDeleteYes_Click" />

                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="panelNewEditGroup" runat="server">

                    <!-- GENERAL -->
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon-info-sign"></i></span>
                            <h5><%= Resources.LocalizedText.General %></h5>
                        </div>
                        <div class="widget-content">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.DisplayName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtDisplayName" runat="server" name="required"></asp:TextBox>
                                        <asp:HiddenField ID="hfEditGroupDistinguishedName" runat="server" />
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.EmailAddress %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEmailAddress" runat="server" name="required"></asp:TextBox>
                                        <asp:DropDownList ID="ddlDomains" runat="server" Width="300px" DataTextField="DomainName" DataValueField="DomainName"></asp:DropDownList>
                                        <asp:HiddenField ID="hfCurrentEmailAddress" runat="server" />
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.Hidden %></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbGroupHidden" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- OWNERSHIP -->
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon-user"></i></span>
                            <h5><%= Resources.LocalizedText.Ownership %></h5>
                            <span class="label label-info">
                                <a href="#" onclick="javascript: AddMembers('Owners');" style="color: white; cursor: pointer"><%= Resources.LocalizedText.Add %></a>
                            </span>
                        </div>
                        <div class="widget-content">
                            <div class="control-group">
                                <div class="controls">
                                    <div class="table-wrapper">
                                        <div class="table-scroll">
                                            <table id="tableOwners" class="table table-bordered table-striped" width="98%">
                                                <thead>
                                                    <tr>
                                                        <th style="width: 18px">&nbsp;</th>
                                                        <th><%= Resources.LocalizedText.DisplayName %></th>
                                                        <th><%= Resources.LocalizedText.DistinguishedName %></th>
                                                        <th style="width: 18px"></th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:Repeater ID="repeaterOwners" runat="server">
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td>
                                                                    <img src='<%# ResolveUrl(Eval("ImageUrl").ToString()) %>' alt="" /></td>
                                                                <td><%# Eval("DisplayName") %></td>
                                                                <td><%# Eval("DistinguishedName") %></td>
                                                                <td>
                                                                    <img src="../../img/icons/16/DeleteIcon_16x16.png" onclick="javascript: RemoveRow(this,'Owners');" style="cursor: pointer" alt="Remove" />
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>

                                    <asp:HiddenField ID="hfOriginalOwners" runat="server" />
                                    <asp:HiddenField ID="hfModifiedOwners" runat="server" />
                                    <asp:HiddenField ID="hfRemovedOwners" runat="server" />

                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- MEMBERSHIP -->
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon-group"></i></span>
                            <h5><%= Resources.LocalizedText.Membership %></h5>
                            <span class="label label-info">
                                <a href="#" onclick="javascript: AddMembers('Membership');" style="color: white; cursor: pointer"><%= Resources.LocalizedText.Add %></a>
                            </span>
                        </div>
                        <div class="widget-content">
                            <div class="control-group">
                                <div class="controls">
                                    <div class="table-wrapper">
                                        <div class="table-scroll">
                                            <table id="tableMembership" class="table table-bordered table-striped" width="98%">
                                                <thead>
                                                    <tr>
                                                        <th style="width: 18px">&nbsp;</th>
                                                        <th><%= Resources.LocalizedText.DisplayName %></th>
                                                        <th><%= Resources.LocalizedText.DistinguishedName %></th>
                                                        <th style="width: 18px"></th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:Repeater ID="repeaterMembers" runat="server">
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td>
                                                                    <img src='<%# ResolveUrl(Eval("ImageUrl").ToString()) %>' alt="" /></td>
                                                                <td><%# Eval("DisplayName") %></td>
                                                                <td><%# Eval("DistinguishedName") %></td>
                                                                <td>
                                                                    <img src="../../img/icons/16/DeleteIcon_16x16.png" onclick="javascript: RemoveRow(this,'Membership');" style="cursor: pointer" alt="Remove" />
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                    <asp:HiddenField ID="hfOriginalMembership" runat="server" />
                                    <asp:HiddenField ID="hfModifiedMembership" runat="server" />
                                    <asp:HiddenField ID="hfRemovedMembership" runat="server" />

                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- MEMBERSHIP APPROVAL -->
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon-ok"></i></span>
                            <h5><%= Resources.LocalizedText.MembershipApproval %></h5>
                        </div>
                        <div class="widget-content">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MembershipApproval_1 %></label>
                                    <div class="controls">
                                        <label>
                                            <asp:RadioButton ID="rbMembershipApprovalJoinOpen" runat="server" GroupName="MembershipApprovalJoin" Checked="true" />
                                            <%= Resources.LocalizedText.MembershipApproval_1_A %>
                                        </label>
                                        <label>
                                            <asp:RadioButton ID="rbMembershipApprovalJoinClosed" runat="server" GroupName="MembershipApprovalJoin" />
                                            <%= Resources.LocalizedText.MembershipApproval_1_B %>
                                        </label>
                                        <label>
                                            <asp:RadioButton ID="rbMembershipApprovalJoinApproval" runat="server" GroupName="MembershipApprovalJoin" />
                                            <%= Resources.LocalizedText.MembershipApproval_1_C %>
                                        </label>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MembershipApproval2 %></label>
                                    <div class="controls">
                                        <label>
                                            <asp:RadioButton ID="rbMembershipApprovalLeaveOpen" runat="server" GroupName="MembershipApprovalLeave" Checked="true" />
                                            <%= Resources.LocalizedText.MemberhsipApproval_2_A %>
                                        </label>
                                        <label>
                                            <asp:RadioButton ID="rbMembershipApprovalLeaveClosed" runat="server" GroupName="MembershipApprovalLeave" />
                                            <%= Resources.LocalizedText.MembershipApproval_2_B %>
                                        </label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- DELIVERY MANAGEMENT -->
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon-envelope"></i></span>
                            <h5><%= Resources.LocalizedText.DeliveryManagement %></h5>
                        </div>
                        <div class="widget-content">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"></label>
                                    <div class="controls">
                                        <label><%= Resources.LocalizedText.DeliveryManagementTotal %></label>
                                        <label>
                                            <asp:RadioButton ID="rbDeliveryManagementInsideOnly" runat="server" GroupName="Delivery Management" Checked="true" />
                                            <%= Resources.LocalizedText.DeliveryManagement_A %>
                                        </label>
                                        <label>
                                            <asp:RadioButton ID="rbDeliveryManagementInsideOutside" runat="server" GroupName="Delivery Management" />
                                            <%= Resources.LocalizedText.DeliveryManagement_B %>
                                        </label>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"></label>
                                    <div class="controls">
                                        <label><%= Resources.LocalizedText.DeliveryManagement_C %></label>
                                        <label>
                                            <asp:ListBox ID="lstDeliveryManagementRestrict" runat="server" SelectionMode="Multiple" DataTextField="String1" DataValueField="String2" multiple></asp:ListBox>
                                        </label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- MESSAGE APPROVAL -->
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon-lock"></i></span>
                            <h5><%= Resources.LocalizedText.MessageApproval %></h5>
                        </div>
                        <div class="widget-content">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label">&nbsp</label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbMustBeApprovedByAModerator" runat="server" />
                                        <%= Resources.LocalizedText.MessageApproval_A %>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MessageApproval_Column1 %></label>
                                    <div class="controls">
                                        <asp:ListBox ID="lstGroupModerators" runat="server" SelectionMode="Multiple" DataTextField="String1" DataValueField="String2" multiple></asp:ListBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MessageApproval_Column2 %></label>
                                    <div class="controls">
                                        <asp:ListBox ID="lstSendersDontRequireApproval" runat="server" SelectionMode="Multiple" DataTextField="String1" DataValueField="String2" multiple></asp:ListBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"></label>
                                    <div class="controls">
                                        <label>
                                            <asp:RadioButton ID="rbMessageApprovalNotifyAll" runat="server" GroupName="MessageApproval" />
                                            <%= Resources.LocalizedText.MessageApproval_B %>
                                        </label>
                                        <label>
                                            <asp:RadioButton ID="rbMessageApprovalNotifyInternal" runat="server" GroupName="MessageApproval" />
                                            <%= Resources.LocalizedText.MessageApproval_C %>
                                        </label>
                                        <label>
                                            <asp:RadioButton ID="rbMessageApprovalNotifyNone" runat="server" GroupName="MessageApproval" Checked="true" />
                                            <%= Resources.LocalizedText.MessageApproval_D %>
                                        </label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- SAVE -->
                    <div class="widget-box">
                            <div class="widget-content">
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <label class="control-label"></label>
                                        <div class="controls" style="text-align: right">
                                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-warning" OnClick="btnCancel_Click" /> &nbsp;
                                            <asp:Button ID="btnCreateEdit" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="btn btn-success" OnClick="btnCreateEdit_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                    </div>
                </asp:Panel>

                <div id="modalGroupAdd" class="modal hide" aria-hidden="true" style="display: none;">
                    <div class="modal-header">
                        <button data-dismiss="modal" class="close" type="button" onclick="javascript: CloseModal();">×</button>
                        <h3><span id="GroupAddLabel"></span></h3>
                    </div>
                    <div class="modal-body">
                        <div style="text-align: right">
                            <a onclick="javascript: DeselectAll();"><%= Resources.LocalizedText.DeselectAll %></a> &nbsp; <a onclick="javascript: SelectAll();"><%= Resources.LocalizedText.SelectAll %></a>
                        </div>
                        <br />
                        <div class="table-wrapper">
                            <div class="table-scroll">
                                <table id="tablePopup" class="table">
                                    <thead>
                                        <tr>
                                            <th><%= Resources.LocalizedText.DisplayName %></th>
                                            <th style="display: none"><%= Resources.LocalizedText.DistinguishedName %></th>
                                            <th><%= Resources.LocalizedText.Email %></th>
                                            <th><%= Resources.LocalizedText.Type %></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="repeaterPopup" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# Eval("DisplayName") %></td>
                                                    <td style="display: none"><%# Eval("DistinguishedName") %></td>
                                                    <td><%# Eval("PrimarySmtpAddress") %></td>
                                                    <td>
                                                        <%# Eval("ObjectType") %>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>

                        <br />
                        <hr />
                        <br />

                        <div style="text-align: right">
                            <input id="btnOK" type="button" value="<%= Resources.LocalizedText.OK %>" onclick="javascript: fnGetSelected();" />
                        </div>
                    </div>
                </div>


            </div>
        </div>
    </div>

    <script src="../../js/jquery.min.js"></script>
    <script src="../../js/jquery.gritter.min.js"></script>
    <script src="../../js/jquery.validate.js"></script>
    <script src="../../js/jquery.ui.custom.js"></script>
    <script src="../../js/bootstrap.min.js"></script>
    <script src="../../js/jquery.uniform.js"></script>
    <script src="../../js/select2.min.js"></script>
    <script src="../../js/jquery.dataTables.min.js"></script>
    <script src="../../js/matrix.js"></script>
    <script src="../../js/matrix.tables.js"></script>
    <script src="../../js/matrix.popover.js"></script>
    <script type="text/javascript">
        var oTable;
        var membersTable;
        var ownersTable;

        jQuery.expr[':'].contains = function (a, i, m) {
            return jQuery(a).text().toUpperCase().indexOf(m[3].toUpperCase()) >= 0;
        };

        function AddMembers(section) {
            $("#modalGroupAdd").fadeIn('fast');
            $("#GroupAddLabel").text(section);
        }

        function CloseModal() {
            $("#modalGroupAdd").fadeOut('fast');
        }

        function fnGetSelected() {

            var hiddenField = null;
            var removedHiddenField = null;
            var sectionValue = null;
            var theTable = null;

            if ($("#GroupAddLabel").text() == "Owners") {
                hiddenField = document.getElementById("cphMainContainer_hfModifiedOwners");
                removedHiddenField = document.getElementById("cphMainContainer_hfRemovedOwners");
                sectionValue = 'Owners';
                theTable = "tableOwners";
            }
            else if ($("#GroupAddLabel").text() == "Membership") {
                hiddenField = document.getElementById("cphMainContainer_hfModifiedMembership");
                removedHiddenField = document.getElementById("cphMainContainer_hfRemovedMembership");
                sectionValue = 'Membership';
                theTable = "tableMembership";
            }

            oTable.$("tr").filter(".row_selected").each(function (index, row) {
                var dn = $(row).find('td').eq(1).text();
                var objectType = $(row).find('td').eq(3).text().trim();

                var imgType = null;

                if (objectType == 'Group')
                    imgType = '../../img/icons/16/people.png';
                else if (objectType == 'Contact')
                    imgType = '../../img/icons/16/web.png';
                else
                    imgType = '../../img/icons/16/user.png';

                if ($("#" + theTable).not(':contains(\"' + dn + '\")').length == 1) {

                    if (sectionValue == "Owners" && objectType == "User") {
                        ownersTable.fnAddData([
                            "<img src=\"" + imgType + "\"/>",
                             $(row).find('td').eq(0).text(),
                             dn,
                             "<img src='../../img/icons/16/DeleteIcon_16x16.png' onclick='javascript: RemoveRow(this,\"" + sectionValue + "\");' style='cursor: pointer' />"
                        ]);
                        hiddenField.value += dn + "||";
                    }
                    else if (sectionValue == "Owners" && objectType != "User") {
                        alert("You can only add users to the ownership section.");
                    }
                    else if (sectionValue == "Membership") {
                        membersTable.fnAddData([
                            "<img src=\"" + imgType + "\"/>",
                             $(row).find('td').eq(0).text(),
                             dn,
                             "<img src='../../img/icons/16/DeleteIcon_16x16.png' onclick='javascript: RemoveRow(this,\"" + sectionValue + "\");' style='cursor: pointer' />"
                        ]);
                        hiddenField.value += dn + "||";
                    }
                }

                if (removedHiddenField.value.indexOf(dn) !== -1) {
                    removedHiddenField.value = removedHiddenField.value.replace(dn + "||", "");
                }
            });

            $('.row_selected').removeClass('row_selected');

            CloseModal();
        }

        function RemoveRow(row, section) {
            var table = $(row).closest('table');
            var tableRow = $(row).closest('tr');
            var dn = $(tableRow).find('td').eq(2).text();

            var hiddenField = null;
            var removedHiddenField = null;

            if (section == "Owners") {
                hiddenField = document.getElementById("cphMainContainer_hfModifiedOwners");
                removedHiddenField = document.getElementById("cphMainContainer_hfRemovedOwners");

                ownersTable.fnDeleteRow($(tableRow).index());
            }
            else if (section == "Membership") {
                hiddenField = document.getElementById("cphMainContainer_hfModifiedMembership");
                removedHiddenField = document.getElementById("cphMainContainer_hfRemovedMembership");

                membersTable.fnDeleteRow($(tableRow).index());
            }

            hiddenField.value = hiddenField.value.replace(dn + "||", "");

            if (removedHiddenField.value.indexOf(dn) == -1) {
                removedHiddenField.value += dn + "||";
            }
        }

        function SelectAll() {
            $('#tablePopup tr').addClass('row_selected');
        }

        function DeselectAll() {
            $('.row_selected').removeClass('row_selected');
        }

        $(document).ready(function () {

            oTable = $("#tablePopup").dataTable({
                "bJQueryUI": false,
                "bInfo": false,
                "bPaginate": false,
                "aLengthMenu": [10, 50, 100, 500, 1000, 2000],
            });

            membersTable = $("#tableMembership").dataTable({
                "bJQueryUI": false,
                "bInfo": false,
                "bPaginate": false,
                "aoColumns": [
                    {
                        "sWidth": "16px",
                        "bSearchable": false,
                        "bVisible": true
                    },
                    null,
                    {
                        "bSearchable": false,
                        "sClass": "hidden"
                    },
                    {
                        "sWidth": "16px",
                        "bSearchable": false,
                        "bVisible": true
                    },
                ]
            });

            ownersTable = $("#tableOwners").dataTable({
                "bJQueryUI": false,
                "bInfo": false,
                "bPaginate": false,
                "aoColumns": [
                    {
                        "sWidth": "16px",
                        "bSearchable": false,
                        "bVisible": true
                    },
                    null,
                    {
                        "bSearchable": false,
                        "sClass": "hidden"
                    },
                    {
                        "sWidth": "16px",
                        "bSearchable": false,
                        "bVisible": true
                    },
                ]
            });

            $("#tablePopup tr").click(function () {
                $(this).toggleClass('row_selected');
            });
        });
    </script>

</asp:Content>
