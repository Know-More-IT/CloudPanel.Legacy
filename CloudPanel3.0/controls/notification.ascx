<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="notification.ascx.cs" Inherits="CloudPanel.controls.notification" %>

<asp:Panel ID="panelNotification" runat="server" CssClass="alert alert-error alert-block" Visible="false">
    <a class="close" data-dismiss="alert" href="#">×</a>
    <h4 class="alert-heading"><asp:Label ID="lbTitle" runat="server" Text="Error!"></asp:Label></h4>
    <p><asp:Label ID="lbMsg" runat="server" Text=""></asp:Label></p>
</asp:Panel>