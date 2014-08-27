<%@ Page Language="C#" UICulture="auto" Culture="auto" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="CloudPanel3._0.login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%= CloudPanel.Modules.Settings.Config.HostersName %></title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

	<!-- Font Awesome and Bootstrap -->
    <link href="//netdna.bootstrapcdn.com/twitter-bootstrap/2.3.2/css/bootstrap-combined.no-icons.min.css" rel="stylesheet">
    <link href="//netdna.bootstrapcdn.com/font-awesome/3.2.1/css/font-awesome.css" rel="stylesheet">

    <!-- Google Fonts -->
    <link href='http://fonts.googleapis.com/css?family=Open+Sans:400,700,800' rel='stylesheet' type='text/css'>

    <!-- Other Styles -->
    <link rel="stylesheet" href="css/matrix-login.css" />
</head>
<body>
    <form id="form1" runat="server">

        <div id="loginbox">   
            <asp:Panel ID="loginform" runat="server" CssClass="form-vertical" DefaultButton="lnkLogin">
                <br />
				<div class="control-group normal_text"><h3><asp:Image ID="imgLogo" runat="server" ImageUrl="~/img/logo-default.png"/></h3></div>
                <div class="control-group">
                    <div class="controls">
                        <div class="main_input_box">
                            <span class="add-on bg_lg"><i class="icon-user"></i></span>
                            <asp:TextBox ID="txtUsername" runat="server" placeholder="<%$ Resources:LocalizedText, Global_LoginName %>"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="control-group">
                    <div class="controls">
                        <div class="main_input_box">
                            <span class="add-on bg_ly"><i class="icon-lock"></i></span>
                            <asp:TextBox ID="txtPassword" runat="server" placeholder="<%$ Resources:LocalizedText, Global_Password %>" TextMode="Password"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="control-group">
                    <div class="controls">
                        <div class="main_input_box">
                            <asp:Label ID="lbInfo" runat="server" Text="" ForeColor="Red"></asp:Label>
                        </div>
                    </div>
                </div>
                <div class="form-actions">
                    <span class="pull-right">
                        <asp:LinkButton ID="lnkLogin" runat="server" CssClass="btn btn-success" onclick="lnkLogin_Click" Text="<%$ Resources:LocalizedText, Global_Signin %>"></asp:LinkButton>
                    </span>
                </div>
            </asp:Panel>

        </div>
        
        <script src="js/jquery.min.js"></script>

    </form>
</body>
</html>
