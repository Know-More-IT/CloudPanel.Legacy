<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="mailboxes.aspx.cs" Inherits="CloudPanel.company.exchange.mailboxes" %>
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
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/dashboard.aspx" title="" CssClass="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/resellers.aspx" title="" CssClass="tip-bottom"><i class="icon-user"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="#" CssClass="tip-bottom"><i class="icon-building"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedCompanyCode %></asp:HyperLink>
                <a href="#" title="" class="tip-bottom"><i class="icon-cloud"></i><%= Resources.LocalizedText.Exchange %></a>
                <a href="#" title="" class="tip-bottom"><i class="icon-envelope"></i><%= Resources.LocalizedText.Mailboxes %></a>
            </div>

            <uc1:notification ID="notification1" runat="server" />
            <h1><%= Resources.LocalizedText.Mailboxes %></h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />

            <div class="row-fluid">
                
                <!-- MAILBOX LIST -->
                <asp:Panel ID="panelMailboxes" runat="server">
                <div class="span12">

                <div style="float: right">
                    <asp:Button ID="btnAddUserMailboxes" runat="server" Text="Add User Mailboxes" CssClass="btn btn-success" OnClick="btnAddUserMailboxes_Click"  />
                </div>

                <br />

                <div class="widget-box">
                  <div class="widget-title"> <span class="icon"> <i class="icon-info-sign"></i> </span>
                    <h5><%= Resources.LocalizedText.Mailboxes %> [<asp:Label ID="lbTotalMailboxStorage" runat="server" Text="0.00"></asp:Label>GB]</h5>
                  </div>
                  <div class="widget-content ">
                    <table class="table table-bordered data-table">
                      <thead>
                        <tr>
                          <th><%= Resources.LocalizedText.DisplayName %></th>
                          <th><%= Resources.LocalizedText.EmailAddress %></th>
                          <th><%= Resources.LocalizedText.Plan %></th>
                          <% if (Master.ExchangeStatistics) { %><th><%= Resources.LocalizedText.MailboxSize %></th> <% } %>
                          <th><%= Resources.LocalizedText.MailboxSizeLimit %></th>
                          <th><%= Resources.LocalizedText.Department %></th>
                          <th>&nbsp;</th>
                        </tr>
                      </thead>
                      <tbody>
                          <asp:Repeater ID="repeaterMailboxes" runat="server" OnItemCommand="repeaterMailboxes_ItemCommand">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <%# Eval("DisplayName") %>
                                    </td>
                                    <td>
                                        <%# Eval("PrimarySmtpAddress") %>
                                    </td>
                                    <td>
                                        <%# Eval("MailboxPlanName") %>
                                    </td>

                                    <% if (Master.ExchangeStatistics) { %>
                                    <td>
                                        <%# Eval("CurrentMailboxSizeFormatted") %>
                                    </td>
                                    <% } %>

                                    <td>
                                        <%# Eval("MailboxSizeFormatted")%>
                                    </td>
                                    <td>
                                        <%# Eval("Department") %>
                                    </td>
                                    <td style="text-align: right">
                                        <asp:Button ID="btnEditMailbox" runat="server" CssClass="btn btn-info" Text="<%$ Resources:LocalizedText, Button_Edit %>" CommandArgument='<%# Eval("UserPrincipalName") + "|" + Eval("MailboxPlanName")  %>' CommandName="EditMailbox" /> &nbsp;
                                        <asp:Button ID="btnDisableMailbox" runat="server" CssClass="btn btn-danger" Text="<%$ Resources:LocalizedText, Disable %>" CommandArgument='<%# Eval("DisplayName") + ";" + Eval("UserPrincipalName") %>' CommandName="DisableMailbox" />
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

                <!-- ENABLE USERS FOR EXCHANGE -->
                <asp:Panel ID="panelEnableUsers" runat="server" Visible="false">
                <div class="span12">
                <div class="widget-box">
                  <div class="widget-title"> <span class="icon"> <i class="icon-info-sign"></i> </span>
                    <h5><%= Resources.LocalizedText.Users %></h5>
                  </div>
                  <div class="widget-content ">
                    <table class="table table-bordered table-striped with-check data-table">
                      <thead>
                        <tr>
                          <th><input type="checkbox" id="title-checkbox" name="title-checkbox"></th>
                          <th><%= Resources.LocalizedText.DisplayName %></th>
                          <th><%= Resources.LocalizedText.FirstName %></th>
                          <th><%= Resources.LocalizedText.LastName %></th>
                          <th><%= Resources.LocalizedText.LoginName %></th>
                          <th><%= Resources.LocalizedText.Department %></th>
                          <th><%= Resources.LocalizedText.Global_Created %></th>
                        </tr>
                      </thead>
                      <tbody>
                          <asp:Repeater ID="repeaterNonMailboxUsers" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="cbExchEnableUsers" runat="server" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lbDisplayName" runat="server" Text='<%# Eval("DisplayName") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lbFirstName" runat="server" Text='<%# Eval("Firstname") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lbLastName" runat="server" Text='<%# Eval("Lastname") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lbLoginName" runat="server" Text='<%# Eval("UserPrincipalName") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <%# Eval("Department") %>
                                    </td>
                                    <td>
                                        <%# Eval("Created") %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                          </asp:Repeater>
                      </tbody>
                    </table>
                  </div>
                </div>
                </div>

                <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-user"></i></span>
                            <h5><span id="cphMainContainer_lbEditTitle"><%= Resources.LocalizedText.Settings %></span></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label"><%= Resources.LocalizedText.Plan %></label>
                                <div class="controls">
                                    <asp:DropDownList ID="ddlEnableMailboxPlans" runat="server" Width="225px">
                                    </asp:DropDownList><br />
                                    <span id="PlanDescription"> </span>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label"></label>
                                <div class="controls">
                                     <h4>Please choose from the list below on how to format the e-mail address:</h4>
                                     <br />
                                     <label>
                                      <div class="radio" id="uniform-undefined"><span class=""><asp:RadioButton ID="rbFormatFirstName" runat="server" GroupName="EnableUser" /></span></div>
                                      <%= Resources.LocalizedText.FirstName %> [%g] <a href="#" title="Example: John@domain.com" class="tip-right"><i class="icon-question-sign"></i></a></label> 
                                    <label>
                                      <div class="radio" id="uniform-undefined"><span class=""><asp:RadioButton ID="rbFormatLastName" runat="server" GroupName="EnableUser" /></span></div>
                                      <%= Resources.LocalizedText.LastName %>[%s] <a href="#" title="Example: Doe@domain.com" class="tip-right"><i class="icon-question-sign"></i></a></label> 
                                    <label>
                                      <div class="radio" id="uniform-undefined"><span class=""><asp:RadioButton ID="rbFormatFirstDotLast" runat="server" GroupName="EnableUser" /></span></div>
                                      <%= Resources.LocalizedText.FirstName %>.<%= Resources.LocalizedText.LastName %>[%g.%s] <a href="#" title="Example: John.Doe@domain.com" class="tip-right"><i class="icon-question-sign"></i></a></label> 
                                    <label>
                                      <div class="radio" id="uniform-undefined"><span class=""><asp:RadioButton ID="rbFormatFirstLast" runat="server" GroupName="EnableUser" /></span></div>
                                      <%= Resources.LocalizedText.FirstName %><%= Resources.LocalizedText.LastName %> [%g%s] <a href="#" title="Example: JohnDoe@domain.com" class="tip-right"><i class="icon-question-sign"></i></a></label> 
                                    <label>
                                      <div class="radio" id="uniform-undefined"><span class=""><asp:RadioButton ID="rbFormatLastDotFirst" runat="server" GroupName="EnableUser" /></span></div>
                                      <%= Resources.LocalizedText.LastName %>.<%= Resources.LocalizedText.FirstName %>[%s.%g] <a href="#" title="Example: Doe.John@domain.com" class="tip-right"><i class="icon-question-sign"></i></a></label> 
                                    <label>
                                      <div class="radio" id="uniform-undefined"><span class=""><asp:RadioButton ID="rbFormatLastFirst" runat="server" GroupName="EnableUser" /></span></div>
                                      <%= Resources.LocalizedText.LastName %><%= Resources.LocalizedText.FirstName %> [%s%g] <a href="#" title="Example: DoeJohn@domain.com" class="tip-right"><i class="icon-question-sign"></i></a></label> 
                                    <label>
                                      <div class="radio" id="uniform-undefined"><span class=""><asp:RadioButton ID="rbFormatFirstInitialLast" runat="server" GroupName="EnableUser" Checked="true" /></span></div>
                                      <%= Resources.LocalizedText.FirstInitialLastName %> [%1g%s] <a href="#" title="Example: JDoe@domain.com" class="tip-right"><i class="icon-question-sign"></i></a></label> 
                                    <label>
                                      <div class="radio" id="uniform-undefined"><span class=""><asp:RadioButton ID="rbFormatLastFirstInitial" runat="server" GroupName="EnableUser" /></span></div>
                                      <%= Resources.LocalizedText.LastNameFirstInitial %> [%s%1g] <a href="#" title="Example: DoeJ@domain.com" class="tip-right"><i class="icon-question-sign"></i></a></label>
                                    <label>
                                        <div class="radio" id="uniform-undefined"><span class=""><asp:RadioButton ID="rbFormatOther" runat="server" GroupName="EnableUser" /></span></div>
                                        <%= Resources.LocalizedText.Other %> <a href="#modalExchangeVariables" data-toggle="modal" style="text-decoration: underline"><%= Resources.LocalizedText.Help %></a></label>
                                    <div>
                                        &nbsp;&nbsp;<asp:TextBox ID="txtFormatOther" runat="server"></asp:TextBox> 
                                    </div>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label"><%= Resources.LocalizedText.Domain %></label>
                                <div class="controls">
                                    <asp:DropDownList ID="ddlDomains" runat="server" Width="225px" DataTextField="Domain" DataValueField="Domain">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Activesync Plan</label>
                                <div class="controls">
                                    <asp:DropDownList ID="ddlActiveSyncPlan" runat="server" Width="225px">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <% if (Master.IsSuperAdmin) { %>
                            <div class="control-group">
                                <label class="control-label">Database</label>
                                <div class="controls">
                                    <asp:DropDownList ID="ddlExchangeDatabases" runat="server" Width="225px">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <% } %>
                            <div class="control-group">
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
                        </div>
                    </div>
                 </div>

                <div class="widget-box">
                        <div class="widget-content" style="text-align: right">
                        <span>
                            <asp:Button ID="btnEnableUsersCancel" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" CssClass="btn btn-danger" OnClick="btnEnableUsersCancel_Click" />
                            <asp:Button ID="btnEnableUsers" runat="server" Text="<%$ Resources:LocalizedText, Enable %>" CssClass="btn btn-success" onclick="btnEnableUsers_Click" />
                        </span>
                        </div>
                 </div>

                <div id="modalExchangeVariables" class="modal hide" aria-hidden="true" style="display: none;">
                  <div class="modal-header">
                    <button data-dismiss="modal" class="close" type="button">×</button>
                    <h3><%= Resources.LocalizedText.ExchangeVariables %></h3>
                  </div>
                     <div class="modal-body">
                         <p>
                            <%= Resources.LocalizedText.ExchangeVariablesInfo %>
                         </p>
                         <table>
                             <thead>
                                 <tr>
                                     <th>
                                         Variable
                                     </th>
                                     <th>
                                         Value
                                     </th>
                                 </tr>
                             </thead>
                             <tbody>
                                 <tr>
                                     <td>
                                         %g
                                     </td>
                                     <td>
                                         Given name (first name)
                                     </td>
                                 </tr>
                                 <tr>
                                     <td>
                                         %s
                                     </td>
                                     <td>
                                         Surname (last name)
                                     </td>
                                 </tr>
                                 <tr>
                                     <td>
                                         %<em>x</em>s
                                     </td>
                                     <td>
                                         Uses the first <em>x</em> letters of the surname. For example, if <em>x </em>= 2,
                                         the first two letters of the surname are used.
                                     </td>
                                 </tr>
                                 <tr>
                                     <td>
                                         %<em>x</em>g
                                     </td>
                                     <td>
                                         Uses the first <em>x</em> letters of the given name. For example, if <em>x </em>
                                         = 2, the first two letters of the given name are used.
                                     </td>
                                 </tr>
                             </tbody>
                         </table>
                     </div>
                </div>
                </asp:Panel>

                <!-- EDIT MAILBOX -->
                <asp:Panel ID="panelEditMailbox" runat="server" Visible="false">
                    <div class="span12">

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
                                            <asp:TextBox ID="txtDisplayName" runat="server" TabIndex="27"></asp:TextBox>
                                            <asp:HiddenField ID="hfUserPrincipalName" runat="server" />
                                            <asp:HiddenField ID="hfDistinguishedName" runat="server" />
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.Plan %></label>
                                        <div class="controls">
                                            <asp:DropDownList ID="ddlEditMailboxPlan" runat="server" Width="200px"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.ActiveSyncPlans %></label>
                                        <div class="controls">
                                            <asp:DropDownList ID="ddlActiveSyncPlanEditMailbox" runat="server" Width="225px">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.EmailAddress %></label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtEditPrimaryEmail" runat="server"></asp:TextBox> @ 
                                            <asp:DropDownList ID="ddlEditPrimaryEmailDomain" runat="server" Width="200px" DataTextField="Domain" DataValueField="Domain"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <label class="control-label"><%= Resources.LocalizedText.MailboxSize %></label>
                                        <div class="controls">
                                            <p>
                                                <label for="amount"><%= Resources.LocalizedText.Size %>
                                                    <asp:Label ID="lbEditMailboxSize" runat="server" Text=""></asp:Label>
                                                    <asp:HiddenField ID="hfEditMailboxSize" runat="server" />
                                                </label>
                                            </p>
                                            <p>
                                                <label for="amount"><%= Resources.LocalizedText.EstimatedCost %>
                                                    <%= CloudPanel.Modules.Settings.Config.CurrencySymbol %><span id="editMailboxSizeCost"></span>
                                                </label>
                                            </p>
                                            <div id="slider-editmailbox-size" style="width: 90%"></div>
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
                                            <asp:ListBox ID="lstFullAccessPermissions" runat="server" SelectionMode="Multiple" DataTextField="DisplayName" DataValueField="SamAccountName"  multiple>
                                            </asp:ListBox><asp:HiddenField ID="hfFullAccessOriginal" runat="server" />
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Send As</label>
                                        <div class="controls">
                                            <asp:ListBox ID="lstSendAsPermissions" runat="server" SelectionMode="Multiple" DataTextField="DisplayName" DataValueField="SamAccountName"  multiple>
                                            </asp:ListBox><asp:HiddenField ID="hfSendAsOriginal" runat="server" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5><%= Resources.LocalizedText.EmailAliases %></h5>
                            </div>
                            <div class="widget-content">
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.Aliases %></label>
                                        <div class="controls">
                                            <asp:UpdatePanel ID="updatePanelEmailAlias" runat="server">
                                               <ContentTemplate>
                                                    <asp:GridView ID="gridEmailAliases" runat="server" OnPreRender="gridEmailAliases_PreRender" CssClass="table table-bordered table-striped" 
                                                        OnRowCommand="gridEmailAliases_RowCommand" AutoGenerateColumns="false" OnRowDeleting="gridEmailAliases_RowDeleting">
                                                        <Columns>
                                                            <asp:BoundField HeaderText="Email" DataField="emailAddress"/>
                                                            <asp:CommandField DeleteImageUrl="~/img/icons/32/DeleteIcon_32x32.png" ButtonType="Image" ShowDeleteButton="true" HeaderStyle-Width="32px" />
                                                        </Columns>
                                                    </asp:GridView>
                                                   <br />
                                                   <asp:Label ID="lbAddAliasError" runat="server" Text="" ForeColor="Red"></asp:Label>
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="btnInsertEmailAlias" EventName="Click" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.Add %></label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtAddEmailAlias" runat="server"></asp:TextBox> @ 
                                            <asp:DropDownList ID="ddlAddEmailAliasDomain" runat="server" Width="200px" DataTextField="Domain" DataValueField="Domain"></asp:DropDownList> 
                                            &nbsp;
                                            <asp:Button ID="btnInsertEmailAlias" runat="server" Text="<%$ Resources:LocalizedText, Add %>" CssClass="btn btn-info" OnClick="btnInsertEmailAlias_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5><%= Resources.LocalizedText.Forwarding %></h5>
                            </div>
                            <div class="widget-content">
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <label class="control-label"><%= Resources.LocalizedText.ForwardTo %></label>
                                        <div class="controls">
                                            <asp:DropDownList ID="ddlForwardTo" runat="server" Width="200px" CssClass="grouped"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">&nbsp;</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbDeliverToMailboxAndFoward" runat="server" />
                                            <span><%= Resources.LocalizedText.DeliverAndForward %></span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <% if (CloudPanel.classes.Authentication.IsSuperAdmin) { %>
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5><%= Resources.LocalizedText.OverrideFeatures %></h5>
                                <span class="label">
                                    <asp:CheckBox ID="cbOverrideOptions" runat="server"/>
                                    Check to override plan options
                                </span>
                            </div>
                            <div class="widget-content" id="planOverrideDIV" style="display: none">
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <label class="control-label"></label>
                                        <div class="controls">
                                            <h4>If you check to override plan options it will set the mailbox with what you have checked (or unchecked) below and ignore these settings in the plan.</h4>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">POP3</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbEnablePOP3" runat="server" />
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">IMAP</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbEnableIMAP" runat="server" />
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">OWA</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbEnableOWA" runat="server" />
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">MAPI</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbEnableMAPI" runat="server" />
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">ActiveSync</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbEnableActiveSync" runat="server" />
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">ECP</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="cbEnableECP" runat="server" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <% } %>

                        <div class="widget-box">
                            <div class="widget-content">
                                <div class="form-horizontal">
                                    <div class="control-group">
                                        <label class="control-label"></label>
                                        <div class="controls" style="text-align: right">
                                            <asp:Button ID="btnEditMailboxCancel" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" CssClass="btn btn-danger" OnClick="btnEditMailboxCancel_Click" />
                                            &nbsp;
                                            <asp:Button ID="btnEditMailboxSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="btn btn-info" OnClick="btnEditMailboxSave_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </asp:Panel>

                <!-- DISABLE MAILBOX -->
                <asp:Panel ID="panelDisableMailbox" runat="server" Visible="false">
                    <div class="span12">
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5><%= Resources.LocalizedText.DisableUsersMailbox %> <asp:Label ID="lbDisableDisplayName" runat="server" Text=""></asp:Label></h5>
                                <asp:HiddenField ID="hfDisableUserPrincipalName" runat="server" />
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
    <script type="text/javascript">
        var selected = "";
        var currentSize = "<%= currentMailboxSize %>";

        $(document).ready(function () {

            $("select.grouped option[classification='Users']").wrapAll("<optgroup label='Users'>");
            $("select.grouped option[classification='Contacts']").wrapAll("<optgroup label='Contacts'>");
            $("select.grouped option[classification='DistributionGroups']").wrapAll("<optgroup label='Distribution Groups'>");

            $("#<%= cbOverrideOptions.ClientID %>").change(function () {
                if (this.checked) {
                    $("#planOverrideDIV").fadeIn('slow');
                }
                else
                    $("#planOverrideDIV").fadeOut('slow');
            });

            $newSelected = $("#<%= ddlEnableMailboxPlans.ClientID %> option:selected");
            CalculateNew($newSelected.attr("Description"), $newSelected.attr("Price"), $newSelected.attr("Extra"), $newSelected.attr("Min"), $newSelected.attr("Max"));

            $editSelected = $("#<%= ddlEditMailboxPlan.ClientID %> option:selected");
            CalculateEdit($editSelected.attr("Description"), $editSelected.attr("Price"), $editSelected.attr("Extra"), $editSelected.attr("Min"), $editSelected.attr("Max"), currentSize);


            $("#<%= ddlEnableMailboxPlans.ClientID %>").change(function () {

                $("#<%= ddlEnableMailboxPlans.ClientID %> option:selected").each(function () {
                    CalculateNew($(this).attr("Description"), $(this).attr("Price"), $(this).attr("Extra"), $(this).attr("Min"), $(this).attr("Max"));
                });
            });

            $("#<%= ddlEditMailboxPlan.ClientID %>").change(function () {

                $("#<%= ddlEditMailboxPlan.ClientID %> option:selected").each(function () {
                    CalculateEdit($(this).attr("Description"), $(this).attr("Price"), $(this).attr("Extra"), $(this).attr("Min"), $(this).attr("Max"), null);
                });
            });
        });

        function CalculateNew(description, price, extra, min, max) {
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

        function CalculateEdit(description, price, extra, min, max, current) {
            selected = description;

            var price = parseFloat(price);
            var extra = parseFloat(extra);
            var minRange = parseInt(min);
            var maxRange = parseInt(max);

            if (current == null)
                currentSize = minRange;

            // Store in hidden field for post back
            $("#<%= hfEditMailboxSize.ClientID %>").val(currentSize);

            $("#slider-editmailbox-size").slider("destroy");
            $("#slider-editmailbox-size").slider({
                range: "max",
                min: minRange,
                max: maxRange,
                value: currentSize,
                step: 256,
                slide: function (event, ui) {
                        $("#<%= lbEditMailboxSize.ClientID %>").text(ui.value + "MB (" + ui.value / 1024 + "GB)");

                        // Store in hidden field for post back
                        $("#<%= hfEditMailboxSize.ClientID %>").val(ui.value);
                        
                        var total = CalculatePrice(ui.value, minRange, extra, price);
                        $("#editMailboxSizeCost").text(total.toString());
                    }
             });

             $("#<%= lbEditMailboxSize.ClientID %>").text(currentSize.toString() + "MB (" + currentSize / 1024 + "GB)");
             $("#editMailboxSizeCost").text(price.toString());
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
