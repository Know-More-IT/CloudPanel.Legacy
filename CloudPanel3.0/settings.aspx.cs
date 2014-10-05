using CloudPanel.classes;
using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;

namespace CloudPanel
{
    public partial class settings : System.Web.UI.Page
    {
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsLocal && !Authentication.IsSuperAdmin)
            {
                // Redirect to login since this isn't a local request
                Response.Redirect("~/login.aspx", true);
            }

            if (!IsPostBack)
            {
                // Populate the currency symbols
                PopulateCurrencies();

                // Get all the settings from the database
                PopulateSettings();
            }
           
        }

        private void PopulateCurrencies()
        {
            List<ListItem> items = new List<ListItem>();

            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                if (ci.IsNeutralCulture != true)
                {
                    RegionInfo region = new RegionInfo(ci.LCID);

                    string currencyName = region.CurrencyEnglishName;
                    string currencySymbol = region.CurrencySymbol;

                    string format = string.Format("{0} [{1}]", currencyName, currencySymbol);

                    ListItem item = new ListItem();
                    item.Text = format;
                    item.Value = format;

                    // Add to our list
                    if (!items.Contains(item))
                        items.Add(item);
                }
            }

            // Sort array
            items = items.OrderBy(li => li.Text).ToList();

            // Add to our list
            ddlCurrenciesSymbol.Items.AddRange(items.ToArray());
        }

        /// <summary>
        /// Populates the settings from the database
        /// </summary>
        private void PopulateSettings()
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM Settings", sql);

            try
            {
                sql.Open();

                SqlDataReader r = cmd.ExecuteReader();
                if (r.HasRows)
                {
                    while (r.Read())
                    {
                        if (r["CompanysName"] != DBNull.Value)
                            txtCompanysName.Text = r["CompanysName"].ToString();

                        if (r["ResellersEnabled"] != DBNull.Value)
                            cbResellersEnabled.Checked = bool.Parse(r["ResellersEnabled"].ToString());

                        if (r["BaseOU"] != DBNull.Value)
                            txtBaseOrganizationalUnit.Text = r["BaseOU"].ToString();

                        if (r["UsersOU"] != DBNull.Value)
                            txtUsersOU.Text = r["UsersOU"].ToString();

                        if (r["PrimaryDC"] != DBNull.Value)
                            txtDomainController.Text = r["PrimaryDC"].ToString();

                        if (r["Username"] != DBNull.Value)
                            txtUsername.Text = r["Username"].ToString();

                        if (r["Password"] != DBNull.Value)
                            txtPassword.Text = DataProtection.Decrypt(r["Password"].ToString(), Config.Key);

                        if (r["SuperAdmins"] != DBNull.Value)
                            txtSuperAdmins.Text = r["SuperAdmins"].ToString();

                        if (r["BillingAdmins"] != DBNull.Value)
                            txtBillingAdmins.Text = r["BillingAdmins"].ToString();

                        if (r["ExchangeConnectionType"] != DBNull.Value)
                            ddlExchangeConnectionType.SelectedValue = r["ExchangeConnectionType"].ToString();

                        if (r["ExchangeFqdn"] != DBNull.Value)
                            txtExchangeServer.Text = r["ExchangeFqdn"].ToString();

                        if (r["ExchangePFServer"] != DBNull.Value)
                            txtExchangePublicFolderServer.Text = r["ExchangePFServer"].ToString();

                        if (r["ExchangeVersion"] != DBNull.Value)
                            ddlExchangeVersion.SelectedValue = r["ExchangeVersion"].ToString();

                        if (r["ExchangeSSLEnabled"] != DBNull.Value)
                            cbExchangeSSL.Checked = bool.Parse(r["ExchangeSSLEnabled"].ToString());

                        if (r["CitrixEnabled"] != DBNull.Value)
                            cbCitrixEnabled.Checked = bool.Parse(r["CitrixEnabled"].ToString());

                        if (r["PublicFolderEnabled"] != DBNull.Value)
                            cbExchangePFEnabled.Checked = bool.Parse(r["PublicFolderEnabled"].ToString());

                        if (r["ExchDatabases"] != DBNull.Value)
                            txtExchDatabases.Text = r["ExchDatabases"].ToString();

                        if (r["CurrencySymbol"] != DBNull.Value && r["CurrencyEnglishName"] != DBNull.Value)
                        {
                            string name = r["CurrencyEnglishName"].ToString();

                            foreach (ListItem item in ddlCurrenciesSymbol.Items)
                            {
                                if (item.Text.Equals(name))
                                {
                                    item.Selected = true;
                                    break;
                                }
                            }
                        }

                        if (r["AllowCustomNameAttrib"] != DBNull.Value)
                            cbAllowCustomNameAttribute.Checked = bool.Parse(r["AllowCustomNameAttrib"].ToString());

                        if (r["ExchStats"] != DBNull.Value)
                            cbExchangeStats.Checked = bool.Parse(r["ExchStats"].ToString());

                        if (r["IPBlockingEnabled"] != DBNull.Value)
                            cbIPBlockingEnabled.Checked = bool.Parse(r["IPBlockingEnabled"].ToString());

                        if (r["IPBlockingFailedCount"] != DBNull.Value)
                            txtIPBlockingFailedCount.Text = r["IPBlockingFailedCount"].ToString();
                        else
                            txtIPBlockingFailedCount.Text = "1";

                        if (r["IPBlockingLockedMinutes"] != DBNull.Value)
                            txtIPBlockingLockedOutInMin.Text = r["IPBlockingLockedMinutes"].ToString();
                        else
                            txtIPBlockingLockedOutInMin.Text = "1";

                        if (r["LockdownEnabled"] != DBNull.Value)
                            cbOnlySuperAdminLogin.Checked = bool.Parse(r["LockdownEnabled"].ToString());
                        else
                            cbOnlySuperAdminLogin.Checked = false;

                        if (r["LyncEnabled"] != DBNull.Value)
                            cbLyncEnabled.Checked = bool.Parse(r["LyncEnabled"].ToString());
                        else
                            cbLyncEnabled.Checked = false;

                        if (r["LyncFrontEnd"] != DBNull.Value)
                            txtLyncFrontEnd.Text = r["LyncFrontEnd"].ToString();

                        if (r["LyncUserPool"] != DBNull.Value)
                            txtLyncUserPool.Text = r["LyncUserPool"].ToString();

                        if (r["LyncMeetingUrl"] != DBNull.Value)
                            txtLyncMeetUrl.Text = r["LyncMeetingUrl"].ToString();

                        if (r["LyncDialinUrl"] != DBNull.Value)
                            txtLyncDialinUrl.Text = r["LyncDialinUrl"].ToString();

                        if (r["SupportMailEnabled"] != DBNull.Value)
                            cbSupportMailEnabled.Checked = bool.Parse(r["SupportMailEnabled"].ToString());

                        if (r["SupportMailAddress"] != DBNull.Value)
                            txtSupportMailAddress.Text = r["SupportMailAddress"].ToString();

                        if (r["SupportMailPort"] != DBNull.Value)
                            txtSupportMailPort.Text = r["SupportMailPort"].ToString();

                        if (r["SupportMailUsername"] != DBNull.Value)
                            txtSupportMailUsername.Text = r["SupportMailUsername"].ToString();

                        if (r["SupportMailServer"] != DBNull.Value)
                            txtSupportMailServer.Text = r["SupportMailServer"].ToString();

                        try
                        {
                            if (r["SupportMailPassword"] != DBNull.Value)
                            {
                                if (!string.IsNullOrEmpty(r["SupportMailPassword"].ToString()))
                                    txtSupportMailPassword.Text = DataProtection.Decrypt(r["SupportMailPassword"].ToString(), Config.Key);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log that we were unable to decrypt password
                            this.logger.Error("Unable to decrypt password for the support mail notifications.", ex);
                        }

                        if (r["SupportMailFrom"] != DBNull.Value)
                            txtSupportMailFrom.Text = r["SupportMailFrom"].ToString();

                        // Get tasks from database
                        GetTasks();
                    }
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.Message);
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }
        }

        /// <summary>
        /// Gets the tasks from the database
        /// </summary>
        private void GetTasks()
        {
            try
            {
                List<DatabaseTask> tasks = DbSql.Get_DatabaseTasksReoccurring();
                if (tasks != null)
                {
                    foreach (DatabaseTask task in tasks)
                    {
                        TimeSpan days = new TimeSpan(0, task.TaskDelayInMinutes, 0);

                        if (task.TaskType == Enumerations.TaskType.ReadMailboxSizes)
                        {
                            txtQueryMailboxSizesEveryXDays.Text = days.Days.ToString();
                            lbQueryMailboxSizesEveryXDays.Text = task.TaskOutput;
                        }
                        else if (task.TaskType == Enumerations.TaskType.ReadMailboxDatabaseSizes)
                        {
                            txtQueryMailboxDatabaseSizesEveryXDays.Text = days.Days.ToString();
                            lbQueryMailboxDatabaseSizesEveryXDays.Text = task.TaskOutput;
                        }
                    }
                }
                else
                {
                    txtQueryMailboxSizesEveryXDays.Text = "0";
                    txtQueryMailboxDatabaseSizesEveryXDays.Text = "0";
                }
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, "There was an error retrieving the tasks from the database: " + ex.ToString());
            }
        }

        /// <summary>
        /// Saves / Updates the tasks in the database
        /// </summary>
        private void SaveTasks()
        {
            try
            {
                int startXMailboxSizes = 0;
                int.TryParse(txtQueryMailboxSizesEveryXDays.Text, out startXMailboxSizes);

                int startXMailboxDBSizes = 0;
                int.TryParse(txtQueryMailboxDatabaseSizesEveryXDays.Text, out startXMailboxDBSizes);

                if (!cbExchangeStats.Checked)
                {
                    DbSql.Remove_DatabaseTask(new DatabaseTask()
                    {
                        TaskType = Enumerations.TaskType.ReadMailboxSizes,
                        IsReoccurringTask = true
                    });
                    
                    DbSql.Remove_DatabaseTask(new DatabaseTask()
                    {
                        TaskType = Enumerations.TaskType.ReadMailboxDatabaseSizes,
                        IsReoccurringTask = true
                    });
                }
                else
                {
                    if (startXMailboxSizes == 0)
                        DbSql.Remove_DatabaseTask(new DatabaseTask()
                        {
                            TaskType = Enumerations.TaskType.ReadMailboxSizes,
                            IsReoccurringTask = true
                        });

                    if (startXMailboxDBSizes == 0)
                        DbSql.Remove_DatabaseTask(new DatabaseTask()
                        {
                            TaskType = Enumerations.TaskType.ReadMailboxDatabaseSizes,
                            IsReoccurringTask = true
                        });

                    if (startXMailboxSizes > 0)
                    {
                        // Convert to minutes
                        TimeSpan minutes = new TimeSpan(startXMailboxSizes, 0, 0, 0);

                        DatabaseTask task = new DatabaseTask();
                        task.TaskType = Enumerations.TaskType.ReadMailboxSizes;
                        task.LastRun = DateTime.Now;
                        task.NextRun = DateTime.Now.Add(minutes);
                        task.TaskDelayInMinutes = int.Parse(minutes.TotalMinutes.ToString());
                        task.WhenCreated = DateTime.Now;
                        task.IsReoccurringTask = true;

                        DbSql.Update_DatabaseTask(task);
                    }

                    if (startXMailboxDBSizes > 0)
                    {
                        // Convert to minutes
                        TimeSpan minutes = new TimeSpan(startXMailboxDBSizes, 0, 0, 0);

                        DatabaseTask task = new DatabaseTask();
                        task.TaskType = Enumerations.TaskType.ReadMailboxDatabaseSizes;
                        task.LastRun = DateTime.Now;
                        task.NextRun = DateTime.Now.Add(minutes);
                        task.TaskDelayInMinutes = int.Parse(minutes.TotalMinutes.ToString());
                        task.WhenCreated = DateTime.Now;
                        task.IsReoccurringTask = true;

                        DbSql.Update_DatabaseTask(task);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Saves the values to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"IF NOT EXISTS (SELECT * FROM Settings)
                                              BEGIN
                                                     INSERT INTO Settings
                                                     (BaseOU, PrimaryDC, ExchangeFqdn, Username, Password, SuperAdmins, BillingAdmins, ExchangeConnectionType,
                                                      ExchangePFServer, ExchangeVersion, PasswordMinLength, PasswordComplexityType, ExchangeSSLEnabled, ExchStats,
                                                      CitrixEnabled, PublicFolderEnabled, LyncEnabled, WebsiteEnabled, SQLEnabled, CurrencySymbol, CurrencyEnglishName, ResellersEnabled, 
                                                      CompanysName, AllowCustomNameAttrib, IPBlockingEnabled, IPBlockingFailedCount, IPBlockingLockedMinutes, ExchDatabases, UsersOU, LockdownEnabled,
                                                      LyncFrontEnd, LyncUserPool, LyncMeetingUrl, LyncDialinUrl, SupportMailEnabled, SupportMailAddress, SupportMailServer, SupportMailPort, SupportMailUsername, SupportMailPassword, SupportMailFrom) VALUES
                                                     (@BaseOU, @PrimaryDC, @ExchangeFqdn, @Username, @Password, @SuperAdmins, @BillingAdmins, @ExchangeConnectionType,
                                                      @ExchangePFServer, @ExchangeVersion, @PasswordMinLength, @PasswordComplexityType, @ExchangeSSLEnabled, @ExchStats,
                                                      @CitrixEnabled, @PublicFolderEnabled, @LyncEnabled, @WebsiteEnabled, @SQLEnabled, @CurrencySymbol, @CurrencyEnglishName, @ResellersEnabled, 
                                                      @CompanysName, @AllowCustomNameAttrib, @IPBlockingEnabled, @IPBlockingFailedCount, @IPBlockingLockedMinutes, @ExchDatabases, @UsersOU, @LockdownEnabled,
                                                      @LyncFrontEnd, @LyncUserPool, @LyncMeetingUrl, @LyncDialinUrl, @SupportMailEnabled, @SupportMailAddress, @SupportMailServer, @SupportMailPort, @SupportMailUsername, @SupportMailPassword, @SupportMailFrom)
                                              END
                                              ELSE
                                              BEGIN
                                                     UPDATE Settings SET BaseOU=@BaseOU, PrimaryDC=@PrimaryDC, ExchangeFqdn=@ExchangeFqdn, Username=@Username, Password=@Password, SuperAdmins=@SuperAdmins,
                                                     BillingAdmins=@BillingAdmins, ExchangeConnectionType=@ExchangeConnectionType, ExchangePFServer=@ExchangePFServer, ExchangeVersion=@ExchangeVersion, PasswordMinLength=@PasswordMinLength,
                                                     PasswordComplexityType=@PasswordComplexityType, ExchangeSSLEnabled=@ExchangeSSLEnabled, ExchStats=@ExchStats, CitrixEnabled=@CitrixEnabled, PublicFolderEnabled=@PublicFolderEnabled,
                                                     LyncEnabled=@LyncEnabled, WebsiteEnabled=@WebsiteEnabled, SQLEnabled=@SQLEnabled, CurrencySymbol=@CurrencySymbol, CurrencyEnglishName=@CurrencyEnglishName, ResellersEnabled=@ResellersEnabled,
                                                     CompanysName=@CompanysName, AllowCustomNameAttrib=@AllowCustomNameAttrib, IPBlockingEnabled=@IPBlockingEnabled, IPBlockingFailedCount=@IPBlockingFailedCount, IPBlockingLockedMinutes=@IPBlockingLockedMinutes,
                                                     ExchDatabases=@ExchDatabases, UsersOU=@UsersOU, LockDownEnabled=@LockDownEnabled, LyncFrontEnd=@LyncFrontEnd, LyncUserPool=@LyncUserPool, LyncMeetingUrl=@LyncMeetingUrl, LyncDialinUrl=@LyncDialinUrl,
                                                     SupportMailEnabled=@SupportMailEnabled, SupportMailAddress=@SupportMailAddress, SupportMailServer=@SupportMailServer, SupportMailPort=@SupportMailPort, SupportMailUsername=@SupportMailUsername, SupportMailPassword=@SupportMailPassword, SupportMailFrom=@SupportMailFrom
                                              END
                                              ", sql);



            try
            {
                // Set Values
                cmd.Parameters.AddWithValue("CompanysName", txtCompanysName.Text);
                cmd.Parameters.AddWithValue("BaseOU", txtBaseOrganizationalUnit.Text);
                cmd.Parameters.AddWithValue("PrimaryDC", txtDomainController.Text);
                cmd.Parameters.AddWithValue("Username", txtUsername.Text);
                cmd.Parameters.AddWithValue("Password", DataProtection.Encrypt(txtPassword.Text, Config.Key));
                cmd.Parameters.AddWithValue("SuperAdmins", txtSuperAdmins.Text);
                cmd.Parameters.AddWithValue("BillingAdmins", txtBillingAdmins.Text);
                cmd.Parameters.AddWithValue("ExchangeFqdn", txtExchangeServer.Text);
                cmd.Parameters.AddWithValue("ExchangeConnectionType", ddlExchangeConnectionType.SelectedValue);
                cmd.Parameters.AddWithValue("ExchangePFServer", txtExchangePublicFolderServer.Text);
                cmd.Parameters.AddWithValue("ExchangeVersion", ddlExchangeVersion.SelectedValue);
                cmd.Parameters.AddWithValue("PasswordMinLength", 4);
                cmd.Parameters.AddWithValue("PasswordComplexityType", 1);
                cmd.Parameters.AddWithValue("ExchangeSSLEnabled", cbExchangeSSL.Checked);
                cmd.Parameters.AddWithValue("CitrixEnabled", cbCitrixEnabled.Checked);
                cmd.Parameters.AddWithValue("PublicFolderEnabled", cbExchangePFEnabled.Checked);
                cmd.Parameters.AddWithValue("LyncEnabled", cbLyncEnabled.Checked);
                cmd.Parameters.AddWithValue("WebsiteEnabled", false);
                cmd.Parameters.AddWithValue("SQLEnabled", false);
                cmd.Parameters.AddWithValue("CurrencySymbol", Between(ddlCurrenciesSymbol.SelectedValue, "[", "]"));
                cmd.Parameters.AddWithValue("CurrencyEnglishName", ddlCurrenciesSymbol.SelectedItem.Text);
                cmd.Parameters.AddWithValue("ResellersEnabled", cbResellersEnabled.Checked);
                cmd.Parameters.AddWithValue("AllowCustomNameAttrib", cbAllowCustomNameAttribute.Checked);
                cmd.Parameters.AddWithValue("ExchStats", cbExchangeStats.Checked);
                cmd.Parameters.AddWithValue("IPBlockingEnabled", cbIPBlockingEnabled.Checked);
                cmd.Parameters.AddWithValue("ExchDatabases", txtExchDatabases.Text);
                cmd.Parameters.AddWithValue("UsersOU", txtUsersOU.Text);
                cmd.Parameters.AddWithValue("LockdownEnabled", cbOnlySuperAdminLogin.Checked);
                cmd.Parameters.AddWithValue("LyncFrontEnd", txtLyncFrontEnd.Text);
                cmd.Parameters.AddWithValue("LyncUserPool", txtLyncUserPool.Text);
                cmd.Parameters.AddWithValue("LyncMeetingUrl", txtLyncMeetUrl.Text);
                cmd.Parameters.AddWithValue("LyncDialinUrl", txtLyncDialinUrl.Text);
                cmd.Parameters.AddWithValue("SupportMailEnabled", cbSupportMailEnabled.Checked);
                cmd.Parameters.AddWithValue("SupportMailAddress", txtSupportMailAddress.Text);
                cmd.Parameters.AddWithValue("SupportMailServer", txtSupportMailServer.Text);
                cmd.Parameters.AddWithValue("SupportMailUsername", txtSupportMailUsername.Text);
                cmd.Parameters.AddWithValue("SupportMailFrom", txtSupportMailFrom.Text);

                int mailPort = 25;
                int.TryParse(txtSupportMailPort.Text, out mailPort);
                cmd.Parameters.AddWithValue("SupportMailPort", mailPort);

                if (!string.IsNullOrEmpty(txtSupportMailPassword.Text))
                    cmd.Parameters.AddWithValue("SupportMailPassword", DataProtection.Encrypt(txtSupportMailPassword.Text, Config.Key));
                else
                    cmd.Parameters.AddWithValue("SupportMailPassword", string.Empty);

                int ipBlockingFailedCount = 1;
                int.TryParse(txtIPBlockingFailedCount.Text, out ipBlockingFailedCount);
                cmd.Parameters.AddWithValue("IPBlockingFailedCount", ipBlockingFailedCount);

                int ipBlockingLockedMin = 1;
                int.TryParse(txtIPBlockingLockedOutInMin.Text, out ipBlockingLockedMin);
                cmd.Parameters.AddWithValue("IPBlockingLockedMinutes", ipBlockingLockedMin);

                // Open
                sql.Open();

                // Update
                cmd.ExecuteNonQuery();

                // Close
                sql.Close();

                // Update the logos
                string loginLogo = string.Empty;
                string cornerLogo = string.Empty;

                // Set the login logo
                if (uploadCompanyLoginLogo.HasFile)
                {
                    // Save the file
                    uploadCompanyLoginLogo.SaveAs(Server.MapPath("~/img/branding/") + uploadCompanyLoginLogo.FileName);

                    // Update the database
                    UpdateLoginLogo(uploadCompanyLoginLogo.FileName);

                    // Set the variable so we can update the config
                    loginLogo = uploadCompanyLoginLogo.FileName;
                }

                // Set the corner logo
                if (uploadCompanyCornerLogo.HasFile)
                {
                    // Save the file
                    uploadCompanyCornerLogo.SaveAs(Server.MapPath("~/img/branding/") + uploadCompanyCornerLogo.FileName);

                    // Update the database
                    UpdateCornerLogo(uploadCompanyCornerLogo.FileName);

                    // Set the variable so we can update the config
                    cornerLogo = uploadCompanyCornerLogo.FileName;
                }

                // Update Application Settings
                UpdateApplicationSettings(DataProtection.Encrypt(txtPassword.Text, Config.Key), loginLogo, cornerLogo);

                // Save the tasks
                SaveTasks();

                // Update Message
                notification1.SetMessage(controls.notification.MessageType.Success, "Successfully changed your CloudPanel settings.");
                
                // Get new data from SQL
                PopulateSettings();
            }
            catch (Exception ex)
            {
                notification1.SetMessage(controls.notification.MessageType.Error, ex.ToString());
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }
        }

        /// <summary>
        /// Updates the login logo
        /// </summary>
        /// <param name="filename"></param>
        private void UpdateLoginLogo(string filename)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE Settings SET BrandingLoginLogo=@BrandingLoginLogo", sql);

            try
            {
                // Set Values
                cmd.Parameters.AddWithValue("BrandingLoginLogo", filename);

                // Open
                sql.Open();

                // Update
                cmd.ExecuteNonQuery();

                // Close
                sql.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }
        }

        /// <summary>
        /// Updates the corner logo
        /// </summary>
        /// <param name="filename"></param>
        private void UpdateCornerLogo(string filename)
        {
            SqlConnection sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            SqlCommand cmd = new SqlCommand(@"UPDATE Settings SET BrandingCornerLogo=@BrandingCornerLogo", sql);

            try
            {
                // Set Values
                cmd.Parameters.AddWithValue("BrandingCornerLogo", filename);

                // Open
                sql.Open();

                // Update
                cmd.ExecuteNonQuery();

                // Close
                sql.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Dispose();
                sql.Dispose();
            }
        }

        /// <summary>
        /// Update the Application settings
        /// </summary>
        /// <param name="password"></param>
        private void UpdateApplicationSettings(string password, string loginLogo, string cornerLogo)
        {
            Config.HostingOU = txtBaseOrganizationalUnit.Text;
            Config.PrimaryDC = txtDomainController.Text;
            Config.ExchangeServer = txtExchangeServer.Text;
            Config.Username = txtUsername.Text;
            Config.Password = password;
            Config.SuperAdministrators = txtSuperAdmins.Text.Split(',');
            Config.BillingAdministrators = txtBillingAdmins.Text.Split(',');
            Config.PublicFolderServer = txtExchangePublicFolderServer.Text;
            Config.ExchangeSSLEnabled = cbExchangeSSL.Checked;
            Config.CitrixEnabled = cbCitrixEnabled.Checked;
            Config.ExchangePublicFoldersEnabled = cbExchangePFEnabled.Checked;
            Config.CurrencySymbol = Between(ddlCurrenciesSymbol.SelectedValue, "[", "]");
            Config.CurrencyEnglishName = ddlCurrenciesSymbol.SelectedItem.Text;
            Config.ResellersEnabled = cbResellersEnabled.Checked;
            Config.HostersName = txtCompanysName.Text;
            Config.CustomNameAttribute = cbAllowCustomNameAttribute.Checked;
            Config.ExchangeStatsEnabled = cbExchangeStats.Checked;
            Config.BruteForceProtectionEnabled = cbIPBlockingEnabled.Checked;
            Config.UsersOU = txtUsersOU.Text;
            Config.LockedDownModeEnabled = cbOnlySuperAdminLogin.Checked;
            Config.LyncEnabled = cbLyncEnabled.Checked;
            Config.LyncFrontEnd = txtLyncFrontEnd.Text;
            Config.LyncUserPool = txtLyncUserPool.Text;
            Config.LyncMeetUri = txtLyncMeetUrl.Text;
            Config.LyncdialinUri = txtLyncDialinUrl.Text;
            Config.SupportMailEnabled = cbSupportMailEnabled.Checked;
            Config.SupportMailAddress = txtSupportMailAddress.Text;
            Config.SupportMailFrom = txtSupportMailFrom.Text;
            Config.SupportMailPort = int.Parse(txtSupportMailPort.Text);
            Config.SupportMailServer = txtSupportMailServer.Text;
            Config.SupportMailUsername = txtSupportMailUsername.Text;

            int ipBlockingFailedCount = 10;
            int.TryParse(txtIPBlockingFailedCount.Text, out ipBlockingFailedCount);
            Config.BruteForceFailedCount = ipBlockingFailedCount;

            int ipBlockingLockedOutInMin = 10;
            int.TryParse(txtIPBlockingLockedOutInMin.Text, out ipBlockingLockedOutInMin);
            Config.BruteForceLockoutInMin = ipBlockingLockedOutInMin;

            if (!string.IsNullOrEmpty(txtSupportMailPassword.Text))
                Config.SupportMailPassword = DataProtection.Encrypt(txtSupportMailPassword.Text, Config.Key);
            else
                Config.SupportMailPassword = string.Empty;

            // Don't update logo settings if they are null
            if (!string.IsNullOrEmpty(loginLogo))
                Config.LoginLogo = loginLogo;

            if (!string.IsNullOrEmpty(cornerLogo))
                Config.CornerLogo = cornerLogo;

            switch (ddlExchangeConnectionType.SelectedValue)
            {
                case "Kerberos":
                    Config.ExchangeConnectionType = Enumerations.ConnectionType.Kerberos;
                    break;
                default:
                    Config.ExchangeConnectionType = Enumerations.ConnectionType.Basic;
                    break;
            }

            switch (ddlExchangeVersion.SelectedValue)
            {
                case "2013":
                    Config.ExchangeVersion = Enumerations.ProductVersion.Exchange2013;
                    break;
                default:
                    Config.ExchangeVersion = Enumerations.ProductVersion.Exchange2010;
                    break;
            }

            Config.LoginIsAllowed = true;
        }

        /// <summary>
        /// Find information between two characters
        /// </summary>
        /// <param name="src"></param>
        /// <param name="findfrom"></param>
        /// <param name="findto"></param>
        /// <returns></returns>
        public static string Between(string src, string findfrom, string findto)
        {
            int start = src.IndexOf(findfrom);
            int to = src.IndexOf(findto, start + findfrom.Length);
            if (start < 0 || to < 0) return "";
            string s = src.Substring(
                           start + findfrom.Length,
                           to - start - findfrom.Length);
            return s;
        }
    }
}