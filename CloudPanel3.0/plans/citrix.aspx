<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="citrix.aspx.cs" Inherits="CloudPanel.plans.citrix" %>
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
                <a href="#" class="active"><i class="icon-cloud"></i><%= Resources.LocalizedText.CitrixPlans %></a>
            </div>

            <uc1:notification ID="notification1" runat="server" />

            <h1><%= Resources.LocalizedText.CitrixPlans %></h1>
        </div>
        <!--End-breadcrumbs-->

        <div class="container-fluid">
            <hr />
            <div class="row-fluid">
                <div class="span12">

                    <asp:Panel ID="panelViewApps" runat="server">

                        <div style="float: right; margin: 0px 0px 20px 0px">
                            <asp:Button ID="btnAddCitrixApp" runat="server" Text="<%$ Resources:LocalizedText, Add %>" 
                                CssClass="btn btn-success cancel" OnClick="btnAddCitrixApp_Click" />
                        </div>

                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon icon-edit"></i></span>
                                <h5><%= Resources.LocalizedText.VirtualServers %></h5>
                            </div>
                            <div class="widget-content">

                                <asp:GridView ID="gridViewVirtServers" runat="server" CssClass="table table-bordered data-table" 
                                    OnPreRender="gridViewVirtServers_PreRender" AutoGenerateColumns="false" OnRowCommand="gridViewVirtServers_RowCommand" OnRowDeleting="gridViewVirtServers_RowDeleting" OnRowEditing="gridViewVirtServers_RowEditing">
                                    <Columns>
                                        <asp:ImageField DataImageUrlField="PictureURL"  NullImageUrl="~/img/icons/Citrix/CitrixServer.png" HeaderStyle-Width="32px"></asp:ImageField>
                                        <asp:BoundField DataField="ID" HeaderText="ID" Visible="false" />
                                        <asp:BoundField DataField="DisplayName" HeaderText="<%$ Resources:LocalizedText, DisplayName %>" />
                                        <asp:BoundField DataField="GroupName" HeaderText="<%$ Resources:LocalizedText, GroupName %>" />
                                        <asp:BoundField DataField="Description" HeaderText="<%$ Resources:LocalizedText, Description %>" />
                                        <asp:BoundField DataField="CompanyCode" HeaderText="<%$ Resources:LocalizedText, AssignedToCompany %>" />
                                        <asp:BoundField DataField="Cost" HeaderText="<%$ Resources:LocalizedText, Cost %>" />
                                        <asp:BoundField DataField="Price" HeaderText="<%$ Resources:LocalizedText, Price %>" />
                                        <asp:BoundField DataField="CurrentUsers" HeaderText="<%$ Resources:LocalizedText, CurrentUsers %>" />
                                        <asp:TemplateField HeaderStyle-Width="50px">
                                            <ItemTemplate>
                                                <asp:ImageButton ID="imgEditApp" runat="server" CommandName="Edit" CommandArgument='<%# Eval("ID") %>' ImageUrl="~/img/icons/16/text_edit.png" />
                                                &nbsp; &nbsp;
                                                <asp:ImageButton ID="imgDeleteApp" runat="server" CommandName="Delete" CommandArgument='<%# Eval("ID") %>' ImageUrl="~/img/icons/16/DeleteIcon_16x16.png" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>

                        <div class="widget-box">
                            <div class="widget-title">
                                <span class="icon"><i class="icon icon-edit"></i></span>
                                <h5><%= Resources.LocalizedText.VirtualApplications %></h5>
                            </div>
                            <div class="widget-content">

                                <asp:GridView ID="gridViewVirtApps" runat="server" CssClass="table table-bordered data-table" 
                                    OnPreRender="gridViewVirtApps_PreRender" AutoGenerateColumns="false" OnRowCommand="gridViewVirtApps_RowCommand" OnRowDeleting="gridViewVirtApps_RowDeleting" OnRowEditing="gridViewVirtApps_RowEditing">
                                    <Columns>
                                        <asp:ImageField DataImageUrlField="PictureURL" NullImageUrl="~/img/icons/Citrix/CitrixApp.png" HeaderStyle-Width="32px"></asp:ImageField>
                                        <asp:BoundField DataField="ID" HeaderText="ID" Visible="false" />
                                        <asp:BoundField DataField="DisplayName" HeaderText="<%$ Resources:LocalizedText, DisplayName %>" />
                                        <asp:BoundField DataField="GroupName" HeaderText="<%$ Resources:LocalizedText, GroupName %>" />
                                        <asp:BoundField DataField="Description" HeaderText="<%$ Resources:LocalizedText, Description %>" />
                                        <asp:BoundField DataField="CompanyCode" HeaderText="<%$ Resources:LocalizedText, AssignedToCompany %>" />
                                        <asp:BoundField DataField="Cost" HeaderText="<%$ Resources:LocalizedText, Cost %>" />
                                        <asp:BoundField DataField="Price" HeaderText="<%$ Resources:LocalizedText, Price %>" />
                                        <asp:BoundField DataField="CurrentUsers" HeaderText="<%$ Resources:LocalizedText, CurrentUsers %>"/>
                                        <asp:TemplateField HeaderStyle-Width="40px">
                                            <ItemTemplate>
                                                <asp:ImageButton ID="imgEditApp" runat="server" CommandName="Edit" CommandArgument='<%# Eval("ID") %>' ImageUrl="~/img/icons/16/text_edit.png" />
                                                &nbsp;
                                                <asp:ImageButton ID="imgDeleteApp" runat="server" CommandName="Delete" CommandArgument='<%# Eval("ID") %>' ImageUrl="~/img/icons/16/DeleteIcon_16x16.png" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>

                    </asp:Panel>


                    <asp:Panel ID="panelAddEditApp" runat="server">

                        <div class="span12">
                            <div class="widget-box">
                                <div class="widget-title">
                                    <span class="icon"><i class="icon icon-edit"></i></span>
                                    <h5><%= Resources.LocalizedText.CitrixSettings %></h5>
                                </div>
                                <div class="widget-content nopadding">
                                    <div class="form-horizontal">
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.DisplayName %></label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtDisplayName" runat="server" MaxLength="40"></asp:TextBox>
                                                <a href="#" title="<%= Resources.LocalizedText.CITRIXHelp1 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                                <asp:HiddenField ID="hfCitrixPlanID" runat="server" />
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.Image %></label>
                                            <div class="controls">
                                                <asp:FileUpload ID="fileUploadImg" runat="server"  />
                                                <a href="#" title="<%= Resources.LocalizedText.CITRIXHelp2 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.Description %></label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine"></asp:TextBox>
                                                <a href="#" title="<%= Resources.LocalizedText.CITRIXHelp3 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.Cost %></label>
                                            <div class="controls">
                                                <div class="input-prepend">
                                                    <span class="add-on"><%= CloudPanel.Modules.Settings.Config.CurrencySymbol %></span>
                                                    <asp:TextBox ID="txtCost" runat="server"></asp:TextBox>
                                                </div>
                                                <a href="#" title="<%= Resources.LocalizedText.CITRIXHelp4 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.Price %></label>
                                            <div class="controls">
                                                <div class="input-prepend">
                                                    <span class="add-on"><%= CloudPanel.Modules.Settings.Config.CurrencySymbol %></span>
                                                    <asp:TextBox ID="txtPrice" runat="server"></asp:TextBox>
                                                </div>
                                                <a href="#" title="<%= Resources.LocalizedText.CITRIXHelp5 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.SpecificCompany %></label>
                                            <div class="controls">
                                                <asp:DropDownList ID="ddlSpecificCompany" runat="server" Width="250px"></asp:DropDownList>
                                                <a href="#" title="<%= Resources.LocalizedText.CITRIXHelp6 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.IsServer %></label>
                                            <div class="controls">
                                                <asp:CheckBox ID="cbIsServer" runat="server" />
                                                <a href="#" title="<%= Resources.LocalizedText.CITRIXHelp7 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label"><%= Resources.LocalizedText.CreateGroups %></label>
                                            <div class="controls">
                                                <asp:CheckBox ID="cbCreateGroups" runat="server" Checked="true"/>
                                                <a href="#" title="<%= Resources.LocalizedText.CITRIXHelp8 %>" class="tip-right"><i class="icon-question-sign"></i></a>
                                                <br />
                                                <span id="groupname" style="display: none"><%= Resources.LocalizedText.CitrixGroupName %> <asp:TextBox ID="txtGroupName" runat="server"></asp:TextBox></span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="widget-box">
                            <div class="widget-content" style="text-align: right">
                                <asp:Button ID="btnCancel" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                                &nbsp;
                                <asp:Button ID="btnSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="btn btn-info" OnClick="btnSave_Click"  />
                            </div>
                        </div>

                    </asp:Panel>

                    
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
    <script src="../js/masked.js"></script>
    <script src="../js/jquery.maskMoney.js"></script>

    <script type="text/javascript">

        $(document).ready(function () {

            $('#<%= txtCost.ClientID %>').maskMoney({ allowZero: true, defaultZero: true });
            $('#<%= txtPrice.ClientID %>').maskMoney({allowZero: true, defaultZero: true});

            $("#<%= cbCreateGroups.ClientID %>").change(function () {
                if (this.checked) {
                    $("#groupname").fadeOut('slow');
                }
                else
                    $("#groupname").fadeIn('slow');
            });

        });

    </script>

</asp:Content>
