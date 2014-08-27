<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="resourcemailbox.aspx.cs" Inherits="CloudPanel.company.exchange.resourcemailboxes" %>
<%@ Register src="../../controls/notification.ascx" tagname="notification" tagprefix="uc1" %>
<%@ MasterType VirtualPath="~/Default.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../../css/uniform.css" />
    <link rel="stylesheet" href="../../css/select2.css" />
    <link rel="stylesheet" href="../../css/jquery-ui.css" />
<style type="text/css">
    .auto-style1 {
        height: 45px;
    }
</style>
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
                <a href="#" title="Contacts" class="tip-bottom"><i class="icon-globe"></i><%= Resources.LocalizedText.ResourceMailboxes %></a>
            </div>

            <uc1:notification ID="notification1" runat="server" />
            <h1><%= Resources.LocalizedText.ResourceMailboxes %></h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <div class="row-fluid">

                 <asp:Panel ID="panelResourceMailboxes" runat="server" Visible="true">
                    <div class="span12">

                        <div style="float: right; margin-bottom: 15px;">
                            <asp:Button ID="btnCreateMailbox" runat="server" Text="Add Mailbox" CssClass="btn btn-success" OnClick="btnCreateMailbox_Click"/>
                         </div>

                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5><%= Resources.LocalizedText.ResourceMailboxes %></h5>
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
                                                <%= Resources.LocalizedText.ResourceType %>
                                            </th>
                                            <th>
                                                &nbsp;
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="repeaterResourceMailboxes" runat="server" OnItemCommand="repeaterResourceMailboxes_ItemCommand">
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <%# Eval("DisplayName") %>
                                                    </td>
                                                    <td>
                                                        <%# Eval("PrimarySmtpAddress") %>
                                                    </td>
                                                    <td>
                                                        <%# Eval("ResourceType") %>
                                                    </td>
                                                    <td style="text-align: right">
                                                        <asp:Button ID="btnEditMailbox" runat="server" CssClass="btn btn-info" Text="<%$ Resources:LocalizedText, Button_Edit %>" CommandArgument='<%# Eval("UserPrincipalName") %>' CommandName="Edit" /> &nbsp; 
                                                        <asp:Button ID="btnDeleteMailbox" runat="server" CssClass="btn btn-danger" Text="<%$ Resources:LocalizedText, Button_Delete %>" CommandArgument='<%# Eval("UserPrincipalName") %>' CommandName="Delete" />
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

                <asp:Panel ID="panelCreateMailbox" runat="server" Visible="false">
                 <div class="span12">

                     <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.Create %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"></label>
                                    <div class="controls">
                                        <label>
                                            <div class="radio" id="uniform-undefined"><span class=""><asp:RadioButton ID="rbRoom" runat="server" GroupName="Resources" Checked="true" /></span></div> Room Mailbox
                                        </label>
                                        <label>
                                            <div class="radio" id="uniform-undefined"><span class=""><asp:RadioButton ID="rbEquipment" runat="server" GroupName="Resources" /></span></div> Equipment Mailbox
                                        </label>
                                        <label>
                                            <div class="radio" id="uniform-undefined"><span class=""><asp:RadioButton ID="rbShared" runat="server" GroupName="Resources" /></span></div> Shared Mailbox
                                        </label>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.DisplayName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtCreateDisplayName" runat="server" TabIndex="27"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.Email %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtCreatePrimarySmtpAddress" runat="server" TabIndex="28"></asp:TextBox> &nbsp; @ &nbsp;
                                        <asp:DropDownList ID="ddlCreateDomains" runat="server" Width="250px"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.Plan %></label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddlCreateMailboxPlans" runat="server" Width="250px"></asp:DropDownList><br />
                                        <span id="PlanCreateDescription"> </span>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MailboxSize %></label>
                                    <div class="controls">
                                        <p>
                                            <label for="amount"><%= Resources.LocalizedText.CurrentSize %>
                                                <asp:Label ID="lbCreateMailboxSizeMB" runat="server" Text=""></asp:Label>
                                                <asp:HiddenField ID="hfCreateMailboxSizeMB" runat="server" />
                                            </label>
                                        </p>
                                        <div id="slider-create-mailbox-size" style="width: 90%"></div>
                                    </div>
                                </div>
                                <div class="form-actions" style="text-align: right">
                                    <asp:Button ID="btnCreateCancel" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" class="btn btn-danger cancel" OnClick="btnCreateCancel_Click" />
                                    <asp:Button ID="btnCreate" runat="server" Text="<%$ Resources:LocalizedText, Create %>" class="btn btn-success" OnClick="btnCreate_Click" />
                                </div>
                            </div>
                        </div>
                    </div>

                 </div>
                 </asp:Panel>

                <asp:Panel ID="panelEditMailbox" runat="server" Visible="false">
                 <div class="span12">

                     <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.General %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label">Resource Type</label>
                                    <div class="controls">
                                        <asp:Label ID="lbResourceType" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.DisplayName %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtDisplayName" runat="server" TabIndex="27"></asp:TextBox>
                                        <asp:HiddenField ID="hfEditUserPrincipalName" runat="server" />
                                        <asp:HiddenField ID="hfEditDistinguishedName" runat="server" />
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.Email %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtEmailAddress" runat="server" TabIndex="28"></asp:TextBox> &nbsp; @ &nbsp;
                                        <asp:DropDownList ID="ddlEmailDomains" runat="server" Width="250px"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.Plan %></label>
                                    <div class="controls">
                                        <asp:DropDownList ID="ddlEditMailboxPlans" runat="server" Width="250px"></asp:DropDownList><br />
                                        <span id="PlanEditDescription"> </span>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.MailboxSize %></label>
                                    <div class="controls">
                                        <p>
                                            <label for="amount"><%= Resources.LocalizedText.CurrentSize %>
                                                <asp:Label ID="lbEditMailboxSizeMB" runat="server" Text=""></asp:Label>
                                                <asp:HiddenField ID="hfEditMailboxSizeMB" runat="server" />
                                            </label>
                                        </p>
                                        <div id="slider-edit-mailbox-size" style="width: 90%"></div>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbHidden" runat="server" TabIndex="30" /> <%= Resources.LocalizedText.Hidden %>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                     <div class="widget-box">
                         <div class="widget-title">
                             <span class="icon"><i class="icon-info-sign"></i></span>
                             <h5>Mailbox Permissions</h5>
                         </div>
                         <div class="widget-content">
                             <div class="form-horizontal">
                                 <div class="control-group">
                                     <label class="control-label">Full Access</label>
                                     <div class="controls">
                                         <asp:ListBox ID="lstFullAccessPermissions" runat="server" SelectionMode="Multiple" DataTextField="DisplayName" DataValueField="SamAccountName" multiple></asp:ListBox><asp:HiddenField ID="hfFullAccessOriginal" runat="server" />
                                     </div>
                                 </div>
                                 <div class="control-group">
                                     <label class="control-label">Send As</label>
                                     <div class="controls">
                                         <asp:ListBox ID="lstSendAsPermissions" runat="server" SelectionMode="Multiple" DataTextField="DisplayName" DataValueField="SamAccountName" multiple></asp:ListBox><asp:HiddenField ID="hfSendAsOriginal" runat="server" />
                                     </div>
                                 </div>
                             </div>
                         </div>
                     </div>

                     <div class="widget-box" id="panelResourceGeneral" runat="server">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.ResourceGeneral %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"><%= Resources.LocalizedText.Capacity %></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtCapacity" runat="server" TabIndex="29"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbEnableResourceBookingAttendant" runat="server" /> <%= Resources.LocalizedText.EnableResourceBookingAttendant %>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                     <div class="widget-box" id="panelResourcePolicy" runat="server">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-edit"></i></span>
                            <h5><%= Resources.LocalizedText.ResourcePolicy %></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <label class="control-label"></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbAllowConflictingMeeting" runat="server" /> Allow conflicting meeting requests
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbAllowRepeatingMeetings" runat="server" /> Allow repeating meetings
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbAllowScheduleDuringWorkHoursOnly" runat="server" /> Allow scheduling only during work hours
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbRejectMeetingBeyondBookingWindow" runat="server" /> Reject repeating meetings that have an end date beyond the booking window
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Booking Window</label>
                                    <div class="controls">
                                        <div class="input-prepend">
                                            <span class="add-on">Days</span>
                                            <asp:TextBox ID="txtBookingWindowInDays" runat="server" name="required"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Maximum Duration</label>
                                    <div class="controls">
                                        <div class="input-prepend">
                                            <span class="add-on">Min</span>
                                            <asp:TextBox ID="txtMaximumDuration" runat="server" name="required"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Maximum Conflict Instances</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtMaxConflictInstances" runat="server" name="required"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Conflict Percentage Allowed</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtConflictPercentageAllowed" runat="server" name="required"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Resource Delegates</label>
                                    <div class="controls">
                                        <asp:ListBox ID="lstResourceDelegates" runat="server" SelectionMode="Multiple" DataTextField="DisplayName" DataValueField="CanonicalName" multiple>
                                        </asp:ListBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label"></label>
                                    <div class="controls">
                                        <asp:CheckBox ID="cbForwardMeetingRequestsToDelegates" runat="server" /> Forward meeting requests to delegates
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                 </div>

                 <div class="widget-box">
                     <div class="widget-content" style="text-align: right">
                         <span>
                             <asp:Button ID="btnSaveCancel" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" CssClass="btn btn-danger" OnClick="btnSaveCancel_Click" /> &nbsp;
                             <asp:Button ID="btnSaveMailbox" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="btn btn-success" OnClick="btnSaveMailbox_Click" />
                         </span>
                     </div>
                 </div>
                 </asp:Panel>

                <asp:Panel ID="panelDisableMailbox" runat="server" Visible="false">
                    <div class="span12">
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5><%= Resources.LocalizedText.DisableUsersMailbox %></h5>
                                <asp:HiddenField ID="hfDisableResourcePrincipalName" runat="server" />
                            </div>
                            <div class="widget-content">
                                <div class="error_ex">
                                    <h3><%= Resources.LocalizedText.DisableUsersMailboxAreYouSure %></h3>
                                    <p><%= Resources.LocalizedText.DisableUsersMailboxAreYouSureDesc %></p>
                                    <br />
                                    <br />

                                    <asp:Button ID="btnDisableCancel" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" CssClass="btn btn-warning btn-big" OnClick="btnDisableCancel_Click" />
                                    &nbsp;&nbsp;
                                    <asp:Button ID="btnDisableYes" runat="server" Text="<%$ Resources:LocalizedText, Disable %>" CssClass="btn btn-danger btn-big" OnClick="btnDisableYes_Click" />

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
    <script src="../../js/jquery.validate.js"></script>

    <script type="text/javascript">
        var selected = "";
        var currentSize = "<%= currentMailboxSize %>";

        $(document).ready(function () {

            $newSelected = $("#<%= ddlCreateMailboxPlans.ClientID %> option:selected");
            CalculateNew($newSelected.attr("Description"), $newSelected.attr("Extra"), $newSelected.attr("Min"), $newSelected.attr("Max"));

            $editSelected = $("#<%= ddlEditMailboxPlans.ClientID %> option:selected");
            CalculateEdit($editSelected.attr("Description"), $editSelected.attr("Extra"), $editSelected.attr("Min"), $editSelected.attr("Max"));

            $("#<%= ddlCreateMailboxPlans.ClientID %>").change(function () {

                $("#<%= ddlCreateMailboxPlans.ClientID %> option:selected").each(function () {
                    CalculateNew($(this).attr("Description"), $(this).attr("Extra"), $(this).attr("Min"), $(this).attr("Max"));
                });
            });

            $("#<%= ddlEditMailboxPlans.ClientID %>").change(function () {

                $("#<%= ddlEditMailboxPlans.ClientID %> option:selected").each(function () {
                    CalculateEdit($(this).attr("Description"), $(this).attr("Extra"), $(this).attr("Min"), $(this).attr("Max"));
                });
            });

            $("#<%= btnCreate.ClientID %>").click(function() {
                $("#form1").validate({
                    rules: {
                        <%= txtCreateDisplayName.UniqueID %>: {
                            required: true,
                            maxlength: 50
                        },
                        <%= txtCreatePrimarySmtpAddress.UniqueID %>: {
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

            $("#<%= btnSaveMailbox.ClientID %>").click(function() {
                $("#form1").validate({
                    rules: {
                        <%= txtDisplayName.UniqueID %>: {
                            required: true
                        },
                        <%= txtEmailAddress.UniqueID %>: {
                            required: true
                        },
                        <%= txtCapacity.UniqueID %>: {
                            required: true,
                            number: true
                        },
                        <%= txtBookingWindowInDays.UniqueID %>: {
                            required: true,
                            number: true
                        },
                        <%= txtMaximumDuration.UniqueID %>: {
                            required: true,
                            number: true
                        },
                        <%= txtMaxConflictInstances.UniqueID %>: {
                            required: true,
                            number: true
                        },
                        <%= txtConflictPercentageAllowed.UniqueID %>: {
                            required: true,
                            number: true,
                            min: 0,
                            max: 100
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

        function CalculateNew(description, extra, min, max) {
            selected = description;

            var extra = parseFloat(extra);
            var minRange = parseInt(min);
            var maxRange = parseInt(max);

            // Store original value in hidden field for post back
            $("#<%= hfCreateMailboxSizeMB.ClientID %>").val(minRange);

            $("#slider-create-mailbox-size").slider("destroy");
            $("#slider-create-mailbox-size").slider({
                range: "max",
                min: minRange,
                max: maxRange,
                value: minRange,
                step: 256,
                slide: function (event, ui) {
                    $("#<%= lbCreateMailboxSizeMB.ClientID %>").text(ui.value + "MB (" + ui.value / 1024 + "GB)");

                        // Store in hidden field for post back
                        $("#<%= hfCreateMailboxSizeMB.ClientID %>").val(ui.value);
                    }
                });
                $("#<%= lbCreateMailboxSizeMB.ClientID %>").text(minRange.toString() + "MB (" + minRange / 1024 + "GB)");

            $("#PlanCreateDescription").text(selected);
        }

        function CalculateEdit(description, extra, min, max) {
            selected = description;

            var extra = parseFloat(extra);
            var minRange = parseInt(min);
            var maxRange = parseInt(max);

            // Store original value in hidden field for post back
            $("#<%= hfEditMailboxSizeMB.ClientID %>").val(currentSize);

            $("#slider-edit-mailbox-size").slider("destroy");
            $("#slider-edit-mailbox-size").slider({
                range: "max",
                min: minRange,
                max: maxRange,
                value: currentSize,
                step: 256,
                slide: function (event, ui) {
                    $("#<%= lbEditMailboxSizeMB.ClientID %>").text(ui.value + "MB (" + ui.value / 1024 + "GB)");

                    // Store in hidden field for post back
                    $("#<%= hfEditMailboxSizeMB.ClientID %>").val(ui.value);
                }
            });

            $("#<%= lbEditMailboxSizeMB.ClientID %>").text(currentSize.toString() + "MB (" + currentSize / 1024 + "GB)");
            $("#PlanEditDescription").text(selected);
        }
    </script>
</asp:Content>
