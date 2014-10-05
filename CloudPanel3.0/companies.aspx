<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="companies.aspx.cs" Inherits="CloudPanel.companies" %>

<%@ Register src="controls/notification.ascx" tagname="notification" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="css/uniform.css" />
    <link rel="stylesheet" href="css/select2.css" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMainContainer" runat="server">
    <div id="content">
        <!--breadcrumbs-->
        <div id="content-header">
            <div id="breadcrumb">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/dashboard.aspx" title="Go to Dashboard" CssClass="tip-bottom"><i class="icon-home"></i><%= Resources.LocalizedText.Global_Dashboard %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/resellers.aspx" title="Go to Resellers" CssClass="tip-bottom"><i class="icon-user"></i><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerCode %></asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="#" CssClass="tip-bottom"><i class="icon-building"></i><%= Resources.LocalizedText.Global_Companies %></asp:HyperLink>
            </div>
            <uc1:notification ID="notification1" runat="server" />

            <h1><%= CloudPanel.Modules.Settings.CPContext.SelectedResellerName %> <%= Resources.LocalizedText.Global_Companies %></h1>
        </div>
        <!--End-breadcrumbs-->
        <div class="container-fluid">
            
            <asp:Panel ID="panelCompanyList" runat="server">
                <div style="float: right; margin: 0px 0px 20px 0px">
                    <asp:Button ID="btnAddCompany" runat="server" Text="<%$ Resources:LocalizedText, Add %>" 
                        CssClass="btn btn-success cancel" onclick="btnAddCompany_Click" />
                </div>

                <div class="widget-box">
                    <div class="widget-title">
                        <span class="icon"><i class="icon-building"></i></span>
                        <h5><%= Resources.LocalizedText.Global_Companies %></h5>
                    </div>
                    <div class="widget-content">
                        <table class="table table-bordered data-table">
                            <thead>
                                <tr>
                                    <th>
                                        <%= Resources.LocalizedText.Global_Name %>
                                    </th>
                                    <th>
                                        <%= Resources.LocalizedText.Global_CompanyCode %>
                                    </th>
                                    <th>
                                        <%= Resources.LocalizedText.Global_Administrator %>
                                    </th>
                                    <th>
                                        <%= Resources.LocalizedText.Global_Address %>
                                    </th>
                                    <th>
                                        <%= Resources.LocalizedText.Global_Domains %>
                                    </th>
                                    <th>
                                        <%= Resources.LocalizedText.Global_Created %>
                                    </th>
                                    <th>
                                        &nbsp;
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="companyRepeater" runat="server" 
                                    onitemcommand="companyRepeater_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <asp:LinkButton ID="lnkSelectCompany" runat="server"  ForeColor="Blue" Font-Underline="true" CommandName="SelectCompany" CommandArgument='<%# Eval("CompanyCode") + "|" + Eval("CompanyName") %>'><%# Eval("CompanyName") %></asp:LinkButton>
                                            </td>
                                            <td>
                                                <%# Eval("CompanyCode") %>
                                            </td>
                                            <td>
                                                <a href='mailto:<%# Eval("AdministratorsEmail") %>'>
                                                    <%# Eval("AdministratorsName") %></a>
                                            </td>
                                            <td>
                                                    <%# Eval("Street") %><br />
                                                    <%# Eval("City") %>,
                                                    <%# Eval("State") %>
                                                    &nbsp;&nbsp;
                                                    <%# Eval("ZipCode") %>
                                            </td>
                                            <td>
                                                <%# String.Join("<br/>", (List<string>)Eval("Domains")) %>
                                            </td>
                                            <td>
                                                <%# Eval("WhenCreated") %>
                                            </td>
                                            <td style="text-align: right">
                                                <asp:Button ID="btnEditCompany" runat="server" CssClass="btn btn-info" Text="<%$ Resources:LocalizedText, Button_Edit %>" CommandArgument='<%# Eval("CompanyCode") + "|" + Eval("CompanyName") %>' CommandName="EditCompany" /> &nbsp;
                                                <asp:Button ID="btnDeleteCompany" runat="server" CssClass="btn btn-danger" Text="<%$ Resources:LocalizedText, Button_Delete %>" CommandArgument='<%# Eval("CompanyCode") %>' CommandName="DeleteCompany" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="panelCompanyEdit" runat="server" CssClass="row-fluid" Visible="false">
                <div class="span12">
                    <div class="widget-box">
                        <div class="widget-title">
                            <span class="icon"><i class="icon icon-user"></i></span>
                            <h5><asp:Label ID="lbEditTitle" runat="server" Text=""></asp:Label></h5>
                        </div>
                        <div class="widget-content nopadding">
                            <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">* <%= Resources.LocalizedText.Global_Name %></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtCompanyName" runat="server" name="required" MaxLength="40"></asp:TextBox><br />
                                    <asp:CheckBox ID="cbUserCompanysNameAsCode" runat="server" /> [Check to use the company's name as the company code instead of a generated one]
                                    <asp:HiddenField ID="hfModifyCompanyCode" runat="server" />
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                    <%= Resources.LocalizedText.Companies_PointofContact %></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtPointofContact" runat="server" name="required"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                    <%= Resources.LocalizedText.Global_Email %></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtEmail" runat="server" name="txtEmail"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                    <%= Resources.LocalizedText.Global_Phone %></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtTelephone" runat="server" MaxLength="50"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                    <%= Resources.LocalizedText.Global_Address %></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtStreetAddress" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                   <%= Resources.LocalizedText.Global_City %></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtCity" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                    <%= Resources.LocalizedText.Global_State %></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtState" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                    <%= Resources.LocalizedText.Global_ZipCode %></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtZipCode" runat="server" MaxLength="50"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                    <%= Resources.LocalizedText.Global_Country %></label>
                                <div class="controls">
                                    <asp:DropDownList id="ddlCountry" runat="server" Width="220px">
                                        <asp:ListItem Value="AF">Afghanistan</asp:ListItem>
                                        <asp:ListItem Value="AL">Albania</asp:ListItem>
                                        <asp:ListItem Value="DZ">Algeria</asp:ListItem>
                                        <asp:ListItem Value="AS">American Samoa</asp:ListItem>
                                        <asp:ListItem Value="AD">Andorra</asp:ListItem>
                                        <asp:ListItem Value="AO">Angola</asp:ListItem>
                                        <asp:ListItem Value="AI">Anguilla</asp:ListItem>
                                        <asp:ListItem Value="AQ">Antarctica</asp:ListItem>
                                        <asp:ListItem Value="AG">Antigua And Barbuda</asp:ListItem>
                                        <asp:ListItem Value="AR">Argentina</asp:ListItem>
                                        <asp:ListItem Value="AM">Armenia</asp:ListItem>
                                        <asp:ListItem Value="AW">Aruba</asp:ListItem>
                                        <asp:ListItem Value="AU">Australia</asp:ListItem>
                                        <asp:ListItem Value="AT">Austria</asp:ListItem>
                                        <asp:ListItem Value="AZ">Azerbaijan</asp:ListItem>
                                        <asp:ListItem Value="BS">Bahamas</asp:ListItem>
                                        <asp:ListItem Value="BH">Bahrain</asp:ListItem>
                                        <asp:ListItem Value="BD">Bangladesh</asp:ListItem>
                                        <asp:ListItem Value="BB">Barbados</asp:ListItem>
                                        <asp:ListItem Value="BY">Belarus</asp:ListItem>
                                        <asp:ListItem Value="BE">Belgium</asp:ListItem>
                                        <asp:ListItem Value="BZ">Belize</asp:ListItem>
                                        <asp:ListItem Value="BJ">Benin</asp:ListItem>
                                        <asp:ListItem Value="BM">Bermuda</asp:ListItem>
                                        <asp:ListItem Value="BT">Bhutan</asp:ListItem>
                                        <asp:ListItem Value="BO">Bolivia</asp:ListItem>
                                        <asp:ListItem Value="BA">Bosnia And Herzegowina</asp:ListItem>
                                        <asp:ListItem Value="BW">Botswana</asp:ListItem>
                                        <asp:ListItem Value="BV">Bouvet Island</asp:ListItem>
                                        <asp:ListItem Value="BR">Brazil</asp:ListItem>
                                        <asp:ListItem Value="IO">British Indian Ocean Territory</asp:ListItem>
                                        <asp:ListItem Value="BN">Brunei Darussalam</asp:ListItem>
                                        <asp:ListItem Value="BG">Bulgaria</asp:ListItem>
                                        <asp:ListItem Value="BF">Burkina Faso</asp:ListItem>
                                        <asp:ListItem Value="BI">Burundi</asp:ListItem>
                                        <asp:ListItem Value="KH">Cambodia</asp:ListItem>
                                        <asp:ListItem Value="CM">Cameroon</asp:ListItem>
                                        <asp:ListItem Value="CA">Canada</asp:ListItem>
                                        <asp:ListItem Value="CV">Cape Verde</asp:ListItem>
                                        <asp:ListItem Value="KY">Cayman Islands</asp:ListItem>
                                        <asp:ListItem Value="CF">Central African Republic</asp:ListItem>
                                        <asp:ListItem Value="TD">Chad</asp:ListItem>
                                        <asp:ListItem Value="CL">Chile</asp:ListItem>
                                        <asp:ListItem Value="CN">China</asp:ListItem>
                                        <asp:ListItem Value="CX">Christmas Island</asp:ListItem>
                                        <asp:ListItem Value="CC">Cocos (Keeling) Islands</asp:ListItem>
                                        <asp:ListItem Value="CO">Colombia</asp:ListItem>
                                        <asp:ListItem Value="KM">Comoros</asp:ListItem>
                                        <asp:ListItem Value="CG">Congo</asp:ListItem>
                                        <asp:ListItem Value="CK">Cook Islands</asp:ListItem>
                                        <asp:ListItem Value="CR">Costa Rica</asp:ListItem>
                                        <asp:ListItem Value="CI">Cote D'Ivoire</asp:ListItem>
                                        <asp:ListItem Value="HR">Croatia (Local Name: Hrvatska)</asp:ListItem>
                                        <asp:ListItem Value="CU">Cuba</asp:ListItem>
                                        <asp:ListItem Value="CY">Cyprus</asp:ListItem>
                                        <asp:ListItem Value="CZ">Czech Republic</asp:ListItem>
                                        <asp:ListItem Value="DK">Denmark</asp:ListItem>
                                        <asp:ListItem Value="DJ">Djibouti</asp:ListItem>
                                        <asp:ListItem Value="DM">Dominica</asp:ListItem>
                                        <asp:ListItem Value="DO">Dominican Republic</asp:ListItem>
                                        <asp:ListItem Value="TP">East Timor</asp:ListItem>
                                        <asp:ListItem Value="EC">Ecuador</asp:ListItem>
                                        <asp:ListItem Value="EG">Egypt</asp:ListItem>
                                        <asp:ListItem Value="SV">El Salvador</asp:ListItem>
                                        <asp:ListItem Value="GQ">Equatorial Guinea</asp:ListItem>
                                        <asp:ListItem Value="ER">Eritrea</asp:ListItem>
                                        <asp:ListItem Value="EE">Estonia</asp:ListItem>
                                        <asp:ListItem Value="ET">Ethiopia</asp:ListItem>
                                        <asp:ListItem Value="FK">Falkland Islands (Malvinas)</asp:ListItem>
                                        <asp:ListItem Value="FO">Faroe Islands</asp:ListItem>
                                        <asp:ListItem Value="FJ">Fiji</asp:ListItem>
                                        <asp:ListItem Value="FI">Finland</asp:ListItem>
                                        <asp:ListItem Value="FR">France</asp:ListItem>
                                        <asp:ListItem Value="GF">French Guiana</asp:ListItem>
                                        <asp:ListItem Value="PF">French Polynesia</asp:ListItem>
                                        <asp:ListItem Value="TF">French Southern Territories</asp:ListItem>
                                        <asp:ListItem Value="GA">Gabon</asp:ListItem>
                                        <asp:ListItem Value="GM">Gambia</asp:ListItem>
                                        <asp:ListItem Value="GE">Georgia</asp:ListItem>
                                        <asp:ListItem Value="DE">Germany</asp:ListItem>
                                        <asp:ListItem Value="GH">Ghana</asp:ListItem>
                                        <asp:ListItem Value="GI">Gibraltar</asp:ListItem>
                                        <asp:ListItem Value="GR">Greece</asp:ListItem>
                                        <asp:ListItem Value="GL">Greenland</asp:ListItem>
                                        <asp:ListItem Value="GD">Grenada</asp:ListItem>
                                        <asp:ListItem Value="GP">Guadeloupe</asp:ListItem>
                                        <asp:ListItem Value="GU">Guam</asp:ListItem>
                                        <asp:ListItem Value="GT">Guatemala</asp:ListItem>
                                        <asp:ListItem Value="GN">Guinea</asp:ListItem>
                                        <asp:ListItem Value="GW">Guinea-Bissau</asp:ListItem>
                                        <asp:ListItem Value="GY">Guyana</asp:ListItem>
                                        <asp:ListItem Value="HT">Haiti</asp:ListItem>
                                        <asp:ListItem Value="HM">Heard And Mc Donald Islands</asp:ListItem>
                                        <asp:ListItem Value="VA">Holy See (Vatican City State)</asp:ListItem>
                                        <asp:ListItem Value="HN">Honduras</asp:ListItem>
                                        <asp:ListItem Value="HK">Hong Kong</asp:ListItem>
                                        <asp:ListItem Value="HU">Hungary</asp:ListItem>
                                        <asp:ListItem Value="IS">Iceland</asp:ListItem>
                                        <asp:ListItem Value="IN">India</asp:ListItem>
                                        <asp:ListItem Value="ID">Indonesia</asp:ListItem>
                                        <asp:ListItem Value="IR">Iran (Islamic Republic Of)</asp:ListItem>
                                        <asp:ListItem Value="IQ">Iraq</asp:ListItem>
                                        <asp:ListItem Value="IE">Ireland</asp:ListItem>
                                        <asp:ListItem Value="IL">Israel</asp:ListItem>
                                        <asp:ListItem Value="IT">Italy</asp:ListItem>
                                        <asp:ListItem Value="JM">Jamaica</asp:ListItem>
                                        <asp:ListItem Value="JP">Japan</asp:ListItem>
                                        <asp:ListItem Value="JO">Jordan</asp:ListItem>
                                        <asp:ListItem Value="KZ">Kazakhstan</asp:ListItem>
                                        <asp:ListItem Value="KE">Kenya</asp:ListItem>
                                        <asp:ListItem Value="KI">Kiribati</asp:ListItem>
                                        <asp:ListItem Value="KP">Korea, Dem People'S Republic</asp:ListItem>
                                        <asp:ListItem Value="KR">Korea, Republic Of</asp:ListItem>
                                        <asp:ListItem Value="KW">Kuwait</asp:ListItem>
                                        <asp:ListItem Value="KG">Kyrgyzstan</asp:ListItem>
                                        <asp:ListItem Value="LA">Lao People'S Dem Republic</asp:ListItem>
                                        <asp:ListItem Value="LV">Latvia</asp:ListItem>
                                        <asp:ListItem Value="LB">Lebanon</asp:ListItem>
                                        <asp:ListItem Value="LS">Lesotho</asp:ListItem>
                                        <asp:ListItem Value="LR">Liberia</asp:ListItem>
                                        <asp:ListItem Value="LY">Libyan Arab Jamahiriya</asp:ListItem>
                                        <asp:ListItem Value="LI">Liechtenstein</asp:ListItem>
                                        <asp:ListItem Value="LT">Lithuania</asp:ListItem>
                                        <asp:ListItem Value="LU">Luxembourg</asp:ListItem>
                                        <asp:ListItem Value="MO">Macau</asp:ListItem>
                                        <asp:ListItem Value="MK">Macedonia</asp:ListItem>
                                        <asp:ListItem Value="MG">Madagascar</asp:ListItem>
                                        <asp:ListItem Value="MW">Malawi</asp:ListItem>
                                        <asp:ListItem Value="MY">Malaysia</asp:ListItem>
                                        <asp:ListItem Value="MV">Maldives</asp:ListItem>
                                        <asp:ListItem Value="ML">Mali</asp:ListItem>
                                        <asp:ListItem Value="MT">Malta</asp:ListItem>
                                        <asp:ListItem Value="MH">Marshall Islands</asp:ListItem>
                                        <asp:ListItem Value="MQ">Martinique</asp:ListItem>
                                        <asp:ListItem Value="MR">Mauritania</asp:ListItem>
                                        <asp:ListItem Value="MU">Mauritius</asp:ListItem>
                                        <asp:ListItem Value="YT">Mayotte</asp:ListItem>
                                        <asp:ListItem Value="MX">Mexico</asp:ListItem>
                                        <asp:ListItem Value="FM">Micronesia, Federated States</asp:ListItem>
                                        <asp:ListItem Value="MD">Moldova, Republic Of</asp:ListItem>
                                        <asp:ListItem Value="MC">Monaco</asp:ListItem>
                                        <asp:ListItem Value="MN">Mongolia</asp:ListItem>
                                        <asp:ListItem Value="MS">Montserrat</asp:ListItem>
                                        <asp:ListItem Value="MA">Morocco</asp:ListItem>
                                        <asp:ListItem Value="MZ">Mozambique</asp:ListItem>
                                        <asp:ListItem Value="MM">Myanmar</asp:ListItem>
                                        <asp:ListItem Value="NA">Namibia</asp:ListItem>
                                        <asp:ListItem Value="NR">Nauru</asp:ListItem>
                                        <asp:ListItem Value="NP">Nepal</asp:ListItem>
                                        <asp:ListItem Value="NL">Netherlands</asp:ListItem>
                                        <asp:ListItem Value="AN">Netherlands Ant Illes</asp:ListItem>
                                        <asp:ListItem Value="NC">New Caledonia</asp:ListItem>
                                        <asp:ListItem Value="NZ">New Zealand</asp:ListItem>
                                        <asp:ListItem Value="NI">Nicaragua</asp:ListItem>
                                        <asp:ListItem Value="NE">Niger</asp:ListItem>
                                        <asp:ListItem Value="NG">Nigeria</asp:ListItem>
                                        <asp:ListItem Value="NU">Niue</asp:ListItem>
                                        <asp:ListItem Value="NF">Norfolk Island</asp:ListItem>
                                        <asp:ListItem Value="MP">Northern Mariana Islands</asp:ListItem>
                                        <asp:ListItem Value="NO">Norway</asp:ListItem>
                                        <asp:ListItem Value="OM">Oman</asp:ListItem>
                                        <asp:ListItem Value="PK">Pakistan</asp:ListItem>
                                        <asp:ListItem Value="PW">Palau</asp:ListItem>
                                        <asp:ListItem Value="PA">Panama</asp:ListItem>
                                        <asp:ListItem Value="PG">Papua New Guinea</asp:ListItem>
                                        <asp:ListItem Value="PY">Paraguay</asp:ListItem>
                                        <asp:ListItem Value="PE">Peru</asp:ListItem>
                                        <asp:ListItem Value="PH">Philippines</asp:ListItem>
                                        <asp:ListItem Value="PN">Pitcairn</asp:ListItem>
                                        <asp:ListItem Value="PL">Poland</asp:ListItem>
                                        <asp:ListItem Value="PT">Portugal</asp:ListItem>
                                        <asp:ListItem Value="PR">Puerto Rico</asp:ListItem>
                                        <asp:ListItem Value="QA">Qatar</asp:ListItem>
                                        <asp:ListItem Value="RE">Reunion</asp:ListItem>
                                        <asp:ListItem Value="RO">Romania</asp:ListItem>
                                        <asp:ListItem Value="RU">Russian Federation</asp:ListItem>
                                        <asp:ListItem Value="RW">Rwanda</asp:ListItem>
                                        <asp:ListItem Value="KN">Saint K Itts And Nevis</asp:ListItem>
                                        <asp:ListItem Value="LC">Saint Lucia</asp:ListItem>
                                        <asp:ListItem Value="VC">Saint Vincent, The Grenadines</asp:ListItem>
                                        <asp:ListItem Value="WS">Samoa</asp:ListItem>
                                        <asp:ListItem Value="SM">San Marino</asp:ListItem>
                                        <asp:ListItem Value="ST">Sao Tome And Principe</asp:ListItem>
                                        <asp:ListItem Value="SA">Saudi Arabia</asp:ListItem>
                                        <asp:ListItem Value="SN">Senegal</asp:ListItem>
                                        <asp:ListItem Value="SC">Seychelles</asp:ListItem>
                                        <asp:ListItem Value="SL">Sierra Leone</asp:ListItem>
                                        <asp:ListItem Value="SG">Singapore</asp:ListItem>
                                        <asp:ListItem Value="SK">Slovakia (Slovak Republic)</asp:ListItem>
                                        <asp:ListItem Value="SI">Slovenia</asp:ListItem>
                                        <asp:ListItem Value="SB">Solomon Islands</asp:ListItem>
                                        <asp:ListItem Value="SO">Somalia</asp:ListItem>
                                        <asp:ListItem Value="ZA">South Africa</asp:ListItem>
                                        <asp:ListItem Value="GS">South Georgia , S Sandwich Is.</asp:ListItem>
                                        <asp:ListItem Value="ES">Spain</asp:ListItem>
                                        <asp:ListItem Value="LK">Sri Lanka</asp:ListItem>
                                        <asp:ListItem Value="SH">St. Helena</asp:ListItem>
                                        <asp:ListItem Value="PM">St. Pierre And Miquelon</asp:ListItem>
                                        <asp:ListItem Value="SD">Sudan</asp:ListItem>
                                        <asp:ListItem Value="SR">Suriname</asp:ListItem>
                                        <asp:ListItem Value="SJ">Svalbard, Jan Mayen Islands</asp:ListItem>
                                        <asp:ListItem Value="SZ">Sw Aziland</asp:ListItem>
                                        <asp:ListItem Value="SE">Sweden</asp:ListItem>
                                        <asp:ListItem Value="CH">Switzerland</asp:ListItem>
                                        <asp:ListItem Value="SY">Syrian Arab Republic</asp:ListItem>
                                        <asp:ListItem Value="TW">Taiwan</asp:ListItem>
                                        <asp:ListItem Value="TJ">Tajikistan</asp:ListItem>
                                        <asp:ListItem Value="TZ">Tanzania, United Republic Of</asp:ListItem>
                                        <asp:ListItem Value="TH">Thailand</asp:ListItem>
                                        <asp:ListItem Value="TG">Togo</asp:ListItem>
                                        <asp:ListItem Value="TK">Tokelau</asp:ListItem>
                                        <asp:ListItem Value="TO">Tonga</asp:ListItem>
                                        <asp:ListItem Value="TT">Trinidad And Tobago</asp:ListItem>
                                        <asp:ListItem Value="TN">Tunisia</asp:ListItem>
                                        <asp:ListItem Value="TR">Turkey</asp:ListItem>
                                        <asp:ListItem Value="TM">Turkmenistan</asp:ListItem>
                                        <asp:ListItem Value="TC">Turks And Caicos Islands</asp:ListItem>
                                        <asp:ListItem Value="TV">Tuvalu</asp:ListItem>
                                        <asp:ListItem Value="UG">Uganda</asp:ListItem>
                                        <asp:ListItem Value="UA">Ukraine</asp:ListItem>
                                        <asp:ListItem Value="AE">United Arab Emirates</asp:ListItem>
                                        <asp:ListItem Value="GB">United Kingdom</asp:ListItem>
                                        <asp:ListItem Value="US" Selected="True">United States</asp:ListItem>
                                        <asp:ListItem Value="UM">United States Minor Is.</asp:ListItem>
                                        <asp:ListItem Value="UY">Uruguay</asp:ListItem>
                                        <asp:ListItem Value="UZ">Uzbekistan</asp:ListItem>
                                        <asp:ListItem Value="VU">Vanuatu</asp:ListItem>
                                        <asp:ListItem Value="VE">Venezuela</asp:ListItem>
                                        <asp:ListItem Value="VN">Viet Nam</asp:ListItem>
                                        <asp:ListItem Value="VG">Virgin Islands (British)</asp:ListItem>
                                        <asp:ListItem Value="VI">Virgin Islands (U.S.)</asp:ListItem>
                                        <asp:ListItem Value="WF">Wallis And Futuna Islands</asp:ListItem>
                                        <asp:ListItem Value="EH">Western Sahara</asp:ListItem>
                                        <asp:ListItem Value="YE">Yemen</asp:ListItem>
                                        <asp:ListItem Value="YU">Yugoslavia</asp:ListItem>
                                        <asp:ListItem Value="ZR">Zaire</asp:ListItem>
                                        <asp:ListItem Value="ZM">Zambia</asp:ListItem>
                                        <asp:ListItem Value="ZW">Zimbabwe</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                    <%= Resources.LocalizedText.Global_Website %></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtWebsite" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                    * <%= Resources.LocalizedText.Global_DomainName %></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtDomainName" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnCancelSubmitCompany" runat="server" Text="<%$ Resources:LocalizedText, Button_Cancel %>" 
                                    class="btn btn-warning cancel" onclick="btnCancelSubmitCompany_Click" />
                                <asp:Button ID="btnSubmitCompany" runat="server" Text="<%$ Resources:LocalizedText, Button_Add %>" 
                                    class="btn btn-success" onclick="btnSubmitCompany_Click" />
                            </div>
                            </div>
                        </div>
                    </div>
                 </div>
            </asp:Panel>

            <asp:Panel ID="panelCompanyDelete" runat="server">
                <div class="widget-box">
                  <div class="widget-title"> <span class="icon"> <i class="icon-info-sign"></i> </span>
                    <h5><%= Resources.LocalizedText.Button_Delete %></h5>
                  </div>
                  <div class="widget-content ">
                    <div style="text-align: center">
                        <%= Resources.LocalizedText.Companies_DeleteWarning %>
                    </div>
                     
                    <div style="text-align: center">
                        <br />
                        <%= Resources.LocalizedText.Companies_DeleteOUPath %>
                        <br />
                        <asp:Label ID="lbDeleteCompanyDN" runat="server" Text=""></asp:Label>
                        <asp:HiddenField ID="hfDeleteCompanyCode" runat="server" />
                    </div>

                    <br />
                    <br />

                    <div style="text-align: center">
                        <%= Resources.LocalizedText.Companies_DeleteVerification %> <asp:Label ID="lbDeleteCompany" runat="server" Text=""></asp:Label><br />
                        <asp:TextBox ID="txtDeleteCompany" runat="server" Width="220px"></asp:TextBox>
                    </div>

                    <div class="clearfix"></div>
                    <br />

                    <div style="text-align: center">
                        <asp:Button ID="btnDeleteCompany" runat="server" Text="<%$ Resources:LocalizedText, Button_Delete %>" CssClass="btn btn-danger" OnClick="btnDeleteCompany_Click"/>
                    </div>

                  </div>
                </div>
            </asp:Panel>

        </div>
    </div>

    <script src="js/jquery.min.js"></script>
    <script src="js/jquery.gritter.min.js"></script> 
    <script src="js/jquery.ui.custom.js"></script> 
    <script src="js/bootstrap.min.js"></script> 
    <script src="js/jquery.uniform.js"></script> 
    <script src="js/select2.min.js"></script> 
    <script src="js/jquery.dataTables.min.js"></script> 
    <script src="js/matrix.js"></script> 
    <script src="js/matrix.tables.js"></script>
    <script src="js/jquery.validate.js"></script>
    <script src="js/masked.js"></script>

    <script type="text/javascript">
        $(document).ready(function() {

	        $(".mask-date").mask("99/99/9999");
	        $(".mask-ssn").mask("999-99-9999");
	        $(".mask-productKey").mask("a*-999-a999", { placeholder: "*" });
	        $(".mask-percent").mask("99%");

            $("#<%= btnSubmitCompany.ClientID %>").click(function() {
                $("#form1").validate({
                    rules: {
                        <%= txtCompanyName.UniqueID %>: {
                            required: true,
                            minlength: 3
                        },
                        <%= txtEmail.UniqueID %>: {
                            email: true,
                        },
                        <%= txtWebsite.UniqueID %>: {
                            url: true
                        },
                        <%= txtDomainName.UniqueID %>: {
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
