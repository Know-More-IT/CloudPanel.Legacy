<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="publicfolders.aspx.cs" Inherits="CloudPanel.company.exchange.publicfolders" %>
<%@ Register src="../../controls/notification.ascx" tagname="notification" tagprefix="uc1" %>
<%@ MasterType VirtualPath="~/Default.Master" %>

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
                <a href="#" title="" class="tip-bottom"><i class="icon-cloud"></i><%= Resources.LocalizedText.Exchange %></a>
                <a href="#" title="" class="tip-bottom"><i class="icon-globe"></i><%= Resources.LocalizedText.PublicFolders %></a>
            </div>

            <uc1:notification ID="notification1" runat="server" />
            <h1><%= Resources.LocalizedText.PublicFolders %></h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <div class="row-fluid">

                 <asp:Panel ID="panelEnablePublicFolders" runat="server" Visible="true">
                    <div class="span12">
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5><%= Resources.LocalizedText.EnablePublicFolders %></h5>
                            </div>
                            <div class="widget-content">
                                <div class="error_ex">
                                    <h3><%= Resources.LocalizedText.EnablePublicFoldersTitle %></h3>
                                    <p><%= Resources.LocalizedText.EnablePublicFoldersDesc %></p>
                                    <br />
                                    <br />

                                    <asp:Button ID="btnEnablePublicFolders" runat="server" Text="<%$ Resources:LocalizedText, Enable %>" CssClass="btn btn-info btn-big" OnClick="btnEnablePublicFolders_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                 <asp:Panel ID="panelEditPublicFolders" runat="server" Visible="false">
                     <div class="span4">
                         <div class="widget-box">
                             <div class="widget-title">
                                  <span class="icon"> <i class="icon-info-sign"></i> </span>
                                  <h5><%= Resources.LocalizedText.PublicFolders %></h5>
                             </div>
                             <div class="widget-content">
                                 <asp:TreeView ID="treePublicFolders" runat="server" OnSelectedNodeChanged="treePublicFolders_SelectedNodeChanged">

                                 </asp:TreeView>
                             </div>
                         </div>
                     </div>

                     <div class="span7">
                         <div class="widget-box">
                             <div class="widget-title">
                                 <ul class="nav nav-tabs">
                                     <li class="active"><a data-toggle="tab" href="#General"><%= Resources.LocalizedText.Properties %></a></li>
                                     <li><a data-toggle="tab" href="#Email"><%= Resources.LocalizedText.Email %></a></li>
                                 </ul>
                             </div>
                             <div class="widget-content tab-content">
                                 <div id="General" class="tab-pane active">
                                     <div class="form-horizontal">
                                         <div class="control-group">
                                             <label class="control-label"><%= Resources.LocalizedText.Name %></label>
                                             <div class="controls">
                                                 <asp:TextBox ID="txtPublicFolderName" runat="server" ReadOnly="true"></asp:TextBox>
                                                 <asp:HiddenField ID="hfPublicFolderPath" runat="server" />
                                             </div>
                                         </div>
                                         <div class="control-group">
                                             <label class="control-label"><%= Resources.LocalizedText.MaxItemSize %></label>
                                             <div class="controls">
                                                 <div class="input-prepend">
                                                    <span class="add-on">MB</span>
                                                    <asp:TextBox ID="txtMaxItemSize" runat="server" name="required"></asp:TextBox>
                                                </div>
                                             </div>
                                         </div>
                                         <div class="control-group">
                                             <label class="control-label"><%= Resources.LocalizedText.WarningSizeInMB %></label>
                                             <div class="controls">
                                                 <div class="input-prepend">
                                                    <span class="add-on">MB</span>
                                                    <asp:TextBox ID="txtWarningSizeMB" runat="server"></asp:TextBox>
                                                </div>
                                             </div>
                                         </div>
                                         <div class="control-group">
                                             <label class="control-label"><%= Resources.LocalizedText.ProhibitPostInMB %></label>
                                             <div class="controls">
                                                 <div class="input-prepend">
                                                    <span class="add-on">MB</span>
                                                    <asp:TextBox ID="txtProhibitPostMB" runat="server"></asp:TextBox>
                                                </div>
                                             </div>
                                         </div>
                                         <div class="control-group">
                                             <label class="control-label"><%= Resources.LocalizedText.AgeLimit %></label>
                                             <div class="controls">
                                                 <div class="input-prepend">
                                                    <span class="add-on">Days</span>
                                                    <asp:TextBox ID="txtAgeLimit" runat="server" name="required"></asp:TextBox>
                                                </div>
                                             </div>
                                         </div>
                                         <div class="control-group">
                                             <label class="control-label"><%= Resources.LocalizedText.KeepDeletedItems %></label>
                                             <div class="controls">
                                                 <div class="input-prepend">
                                                    <span class="add-on">Days</span>
                                                    <asp:TextBox ID="txtKeepDeletedItems" runat="server" name="required"></asp:TextBox>
                                                </div>
                                             </div>
                                         </div>
                                     </div>
                                 </div>
                                 <div id="Email" class="tab-pane">
                                     <div class="form-horizontal">
                                         <div class="control-group">
                                             <label class="control-label">Currently Mail Enabled?</label>
                                             <div class="controls">
                                                 <asp:CheckBox ID="cbPFCurrentEmailEnabled" runat="server" Enabled="false" />
                                             </div>
                                         </div>
                                         <div class="control-group">
                                             <label class="control-label"></label>
                                             <div class="controls">
                                                 <asp:CheckBox ID="cbPFEnableEmail" runat="server" /> Check this box to enable or uncheck to disable
                                             </div>
                                         </div>
                                         <div class="control-group">
                                             <label class="control-label"><%= Resources.LocalizedText.DisplayName %></label>
                                             <div class="controls">
                                                 <asp:TextBox ID="txtDisplayName" runat="server"></asp:TextBox>
                                             </div>
                                         </div>
                                         <div class="control-group">
                                             <label class="control-label"><%= Resources.LocalizedText.Email %></label>
                                             <div class="controls">
                                                 <asp:TextBox ID="txtPrimaryEmail" runat="server"></asp:TextBox>
                                                 <asp:DropDownList ID="ddlEmailDomains" runat="server" Width="200px"></asp:DropDownList>
                                             </div>
                                         </div>
                                         <div class="control-group">
                                             <label class="control-label"></label>
                                             <div class="controls">
                                                 <asp:CheckBox ID="cbPFHiddenFromAddressLists" runat="server" /> Hide from address lists?
                                             </div>
                                         </div>
                                     </div>
                                 </div>
                             </div>
                         </div>

                         <div style="text-align: right">
                            <asp:Button ID="btnPFSave" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnPFSave_Click"/>
                         </div>

                     </div>
                 </asp:Panel>

                <asp:Panel ID="panelDisablePublicFolders" runat="server">
                    <div class="span12">
                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon-info-sign"></i></span>
                                <h5><%= Resources.LocalizedText.DisablePublicFolders %></h5>
                                <asp:HiddenField ID="hfDisableUserPrincipalName" runat="server" />
                            </div>
                            <div class="widget-content">
                                <div class="error_ex">
                                    <h3><%= Resources.LocalizedText.DisablePublicFoldersTitle %></h3>
                                    <p><%= Resources.LocalizedText.DisablePublicFoldersDesc %></p>
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
    <script src="../../js/jquery.validate.js"></script>
    <script src="../../js/jquery.ui.custom.js"></script> 
    <script src="../../js/bootstrap.min.js"></script> 
    <script src="../../js/jquery.uniform.js"></script> 
    <script src="../../js/select2.min.js"></script> 
    <script src="../../js/jquery.dataTables.min.js"></script> 
    <script src="../../js/matrix.js"></script> 
    <script src="../../js/matrix.tables.js"></script>
</asp:Content>
