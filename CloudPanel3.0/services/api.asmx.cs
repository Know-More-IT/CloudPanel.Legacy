using CloudPanel.Modules.ActiveDirectory;
using CloudPanel.Modules.Base.Class;
using CloudPanel.Modules.Settings;
using CloudPanel.Modules.Sql;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Linq;
using CloudPanel.Modules.Exchange;

namespace CloudPanel.services
{
    /// <summary>
    /// Summary description for api
    /// </summary>
    [WebService(Namespace = "CloudPanel.services", Name="CloudPanel API", Description="Provides API Access to the CloudPanel system")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class api : System.Web.Services.WebService
    {
        /// <summary>
        /// Authentication header containing the customers information
        /// </summary>
        public AuthHeader Credentials;

        /// <summary>
        /// Log utility to log any errors or debug messages
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initialize and log the caller's IP address
        /// </summary>
        public api()
        {
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            else
                ip = ip.Split(',')[0];

            // Log the IP connecting
            this.logger.Info("Client connected to the CloudPanel API service: " + ip);
        }

        [WebMethod (Description="Authenticates the user and returns an 'Authenticated' object containing if it was successful or not")]
        [SoapHeader("Credentials")]
        public Authenticated AuthenticateUser(string username, string password)
        {            
            ADUsers userAuth = null;
            try
            {
                string companyCode = GetApiCompanyCode();

                this.logger.Debug("Checking authentication...");

                if (string.IsNullOrEmpty(companyCode))
                    throw new Exception("NOT AUTHENTICATED");

                // User is validated now get the user from SQL
                ADUser getUser = DbSql.Get_User(username);
                if (getUser == null || !getUser.CompanyCode.Equals(companyCode, StringComparison.CurrentCultureIgnoreCase))
                    throw new Exception("User was not found.");
                else
                    this.logger.Debug("Succesfully retrieved " + username + " from the database.");

                // Now authenticate with Active Directory
                userAuth = new ADUsers(Config.Username, Config.Password, Config.PrimaryDC);

                // DEBUG
                this.logger.Debug("Attempting to authenticate user " + username);

                bool isAuthenticated = userAuth.Authenticate(getUser.UserPrincipalName, password);
                if (isAuthenticated)
                    return new Authenticated() { Username = getUser.UserPrincipalName, Status = Enumerations.Status.Success, ErrorMessage = "Sucessfully authenticated user " + getUser.UserPrincipalName };
                else
                    return new Authenticated() { Username = getUser.UserPrincipalName, Status = Enumerations.Status.FailedLogin, ErrorMessage = "Failed to log in user " + getUser.UserPrincipalName };
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (userAuth != null)
                    userAuth.Dispose();
            }
        }

        [WebMethod(Description = "Retrieves a list of all users in the system")]
        [SoapHeader("Credentials")]
        public ADUser[] GetUsers()
        {
            try
            {
                string companyCode = GetApiCompanyCode();

                if (string.IsNullOrEmpty(companyCode))
                    throw new Exception("NOT AUTHENTICATED");

                // User is validated now get the user from SQL
                return DbSql.Get_Users(companyCode).ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [WebMethod(Description = "Resets a user's password")]
        [SoapHeader("Credentials")]
        public void ResetPassword(string userPrincipalName, string newPassword)
        {
            ADUsers adUser = null;

            try
            {
                string companyCode = GetApiCompanyCode();

                if (string.IsNullOrEmpty(companyCode))
                    throw new Exception("NOT AUTHENTICATED");

                if (string.IsNullOrEmpty(userPrincipalName) || string.IsNullOrEmpty(newPassword))
                    throw new Exception("You must supply a user's UserPrincipalName and a new password to reset a user's password.");

                // Get the user from SQL so we can make sure they are part of the same company
                ADUser user = DbSql.Get_User(userPrincipalName);
                if (user == null || !user.CompanyCode.Equals(companyCode, StringComparison.CurrentCultureIgnoreCase))
                {
                    logger.Debug("User " + userPrincipalName + " companycode " + user.CompanyCode + " does not match company code " + companyCode + " or the object was null");

                    throw new Exception("User was not found.");
                }
                else
                {
                    // Reset the user's password
                    adUser = new ADUsers(Config.Username, Config.Password, Config.PrimaryDC);
                    adUser.ResetPassword(userPrincipalName, newPassword);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (adUser != null)
                    adUser.Dispose();
            }
        }

        [WebMethod(Description = "Checks if a user exists or not")]
        [SoapHeader("Credentials")]
        public bool UserExists(string userPrincipalName)
        {
            try
            {
                string companyCode = GetApiCompanyCode();

                if (string.IsNullOrEmpty(companyCode))
                    throw new Exception("NOT AUTHENTICATED");

                if (string.IsNullOrEmpty(userPrincipalName))
                    throw new Exception("You must supply a user's UserPrincipalName to see if it exists or not");

                // Get the user from SQL so we can make sure they are part of the same company
                ADUser user = DbSql.Get_User(userPrincipalName);
                if (!user.CompanyCode.Equals(companyCode, StringComparison.CurrentCultureIgnoreCase))
                    throw new Exception("Invalid user account provided. Please try again.");
                else
                {
                    if (user != null)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [WebMethod(Description = "Checks if a user exists or not")]
        [SoapHeader("Credentials")]
        public void NewUser(ADUser newUser)
        {
            ADUsers users = null;

            try
            {
                string companyCode = GetApiCompanyCode();

                if (string.IsNullOrEmpty(companyCode))
                    throw new Exception("NOT AUTHENTICATED");

                // Check all the variables
                if (string.IsNullOrEmpty(newUser.UserPrincipalName))
                    throw new Exception("To create a new user you must provide the UserPrincipalName");

                if (string.IsNullOrEmpty(newUser.DisplayName))
                    throw new Exception("To create a new user you must provide the Display Name of the user");

                if (string.IsNullOrEmpty(newUser.Firstname))
                    throw new Exception("To create a new user you must provide the First Name of the user");

                if (string.IsNullOrEmpty(newUser.Password))
                    throw new Exception("To create a new user you must provide a password for the user");

                // Check their limits
                bool isAtLimit = SQLLimits.IsCompanyAtUserLimit(companyCode, 1);
                if (isAtLimit)
                    throw new Exception("You are not allowed to create any more users because you are at the limit for your company.");
                else
                {
                    // Get the company's distinguished name
                    Company getCompany = DbSql.Get_Company(companyCode);
                    if (getCompany == null)
                        throw new Exception("Unable to find your company in the database. Please contact support.");

                    // Check if we are using a custom ou. If we are then we need to modify the path
                    if (!string.IsNullOrEmpty(Config.UsersOU))
                        getCompany.DistinguishedName = string.Format("OU={0},{1}", Config.UsersOU, getCompany.DistinguishedName);

                    // Get domains for the company
                    List<Domain> companyDomains = DbSql.Get_Domains(companyCode);

                    // Lower the email for comparison
                    string loweredEmail = newUser.UserPrincipalName.ToLower();

                    // Make sure email address ends with a valid domain
                    bool isValidDomain = false;
                    foreach (Domain d in companyDomains)
                    {
                        if (loweredEmail.EndsWith("@" + d.DomainName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            // The domain in the email address matches the list of allowed domains
                            isValidDomain = true;
                            break;
                        }
                    }

                    if (!isValidDomain)
                        throw new Exception("You provided an invalid domain.");

                    // Initialize class
                    users = new ADUsers(Config.Username, Config.Password, Config.PrimaryDC);

                    // Set the company code
                    newUser.CompanyCode = companyCode;

                    // Now create the user
                    ADUser returnedUser = users.Create(newUser, getCompany.DistinguishedName, newUser.Password);

                    // Add to SQL
                    SQLUsers.AddUser(returnedUser);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (users != null)
                    users.Dispose();
            }
        }

        [WebMethod(Description = "Enables a user for a mailbox")]
        [SoapHeader("Credentials")]
        public void EnableMailbox(MailboxUser mailboxUser, int mailboxPlanID)
        {
            ADUsers users = null;
            ExchCmds powershell = null;

            try
            {
                string companyCode = GetApiCompanyCode();

                if (string.IsNullOrEmpty(companyCode))
                    throw new Exception("NOT AUTHENTICATED");

                // Check all the variables
                if (string.IsNullOrEmpty(mailboxUser.UserPrincipalName))
                    throw new Exception("To create a mailbox for a user you must provide the UserPrincipalName");

                if (string.IsNullOrEmpty(mailboxUser.PrimarySmtpAddress))
                    throw new Exception("To create a mailbox for a user you must provide the PrimarySmtpAddress");

                if (mailboxPlanID < 1)
                    throw new Exception("You must provide a mailbox plan for the user");

                // Check their limits
                bool isAtLimit = SQLLimits.IsCompanyAtMailboxLimit(companyCode, 1);
                if (isAtLimit)
                    throw new Exception("You are not allowed to create any more mailboxes because you are at the limit for your company.");
                else
                {
                    // Get the company and make sure they are enabled
                    Company company = DbSql.Get_Company(companyCode);
                    if (!company.ExchangeEnabled)
                        throw new Exception("Your company is not enabled for Exchange. You must first enabled Exchange and then create mailboxes.");

                    // Get mailbox plans for the company
                    List<MailboxPlan> mailboxPlans = DbSql.Get_MailboxPlans(companyCode);

                    // Get domains for the company
                    List<Domain> exchangeDomains = DbSql.Get_Domains(companyCode);

                    // Lower the email for comparison
                    string loweredEmail = mailboxUser.PrimarySmtpAddress.ToLower();

                    // Make sure email address ends with a valid domain
                    bool isValidDomain = false;
                    foreach (Domain d in exchangeDomains)
                    {
                        if (loweredEmail.EndsWith("@" + d.DomainName, StringComparison.CurrentCultureIgnoreCase) && d.IsAcceptedDomain)
                        {
                            // The domain in the email address matches the list of allowed domains
                            isValidDomain = true;
                            break;
                        }
                    }

                    if (!isValidDomain)
                        throw new Exception("The domain name you provided is not valid. Please make sure the domain you provided is listed in the accepted domains section.");

                    // Make sure the mailbox plan ID is valid
                    var foundPlan = (from p in mailboxPlans
                                     where p.PlanID == mailboxPlanID
                                     select p).FirstOrDefault();

                    if (foundPlan == null)
                        throw new Exception("You have provided an invalid mailbox plan. Please try again.");
                    else
                    {
                        // Initialize powershell
                        powershell = new ExchCmds(Config.ExchangeURI, Config.Username, Config.Password, Config.ExchangeConnectionType, Config.PrimaryDC);

                        // Set the size to what the plan is set to and set the company code
                        foundPlan.SetSizeInMB = foundPlan.SizeInMB;
                        foundPlan.CompanyCode = companyCode;
                        mailboxUser.CompanyCode = companyCode;

                        // Clear out invalid fields
                        mailboxUser.Database = null;

                        // Enable user
                        powershell.Enable_Mailbox(mailboxUser, foundPlan);

                        // Update user in SQL
                        DbSql.Update_UserMailboxInfo(mailboxUser.UserPrincipalName, foundPlan.PlanID, mailboxUser.PrimarySmtpAddress, 0, null);

                        // Add to database queue for permission fixing
                        DbSql.Add_DatabaseQueue(Enumerations.TaskType.MailboxCalendarPermissions, mailboxUser.UserPrincipalName, companyCode, 10, Enumerations.TaskSuccess.NotStarted);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (users != null)
                    users.Dispose();
            }
        }

        [WebMethod(Description = "Gets a list of domains")]
        [SoapHeader("Credentials")]
        public Domain[] GetDomains()
        {
            try
            {
                string companyCode = GetApiCompanyCode();

                if (string.IsNullOrEmpty(companyCode))
                    throw new Exception("NOT AUTHENTICATED");

                return DbSql.Get_Domains(companyCode).ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [WebMethod(Description = "Gets a list of valid mailbox plans")]
        [SoapHeader("Credentials")]
        public MailboxPlan[] GetMailboxPlans()
        {
            try
            {
                string companyCode = GetApiCompanyCode();

                if (string.IsNullOrEmpty(companyCode))
                    throw new Exception("NOT AUTHENTICATED");

                MailboxPlan[] plans = DbSql.Get_MailboxPlans(companyCode).ToArray();
                if (plans == null)
                    return null;
                else
                {
                    for (int i = 0; i < plans.Length; i++)
                    {
                        plans[i].Cost = null;
                        plans[i].Price = null;
                        plans[i].AdditionalGBPrice = null;
                        plans[i].CustomPrice = null;
                    }
                    
                    // Return array
                    return plans;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the company code from the database.
        /// If the value is null or empty then it couldn't find it in the database (probably doesn't have access)
        /// </summary>
        /// <param name="userkey"></param>
        /// <param name="usersecret"></param>
        /// <returns></returns>
        private string GetApiCompanyCode()
        {
            SqlConnection sql = null;
            SqlCommand cmd = null;

            try
            {
                if (Credentials == null)
                    throw new Exception("You must provide the AuthHeaderValue.");

                sql = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                cmd = new SqlCommand("SELECT CompanyCode FROM ApiAccess WHERE CustomerKey=@CustomerKey AND CustomerSecret=@CustomerSecret", sql);

                // Add parameters
                cmd.Parameters.AddWithValue("CustomerKey", Credentials.UserKey);
                cmd.Parameters.AddWithValue("CustomerSecret", Credentials.UserSecret);

                // Open connection
                sql.Open();

                // Get the company code
                object companyCode = cmd.ExecuteScalar();
                if (companyCode != null)
                {
                    this.logger.Debug("API User belongs to company code " + companyCode);

                    return companyCode.ToString();
                }
                else
                {
                    this.logger.Debug("API User failed to login.");

                    throw new Exception("Invalid API Login");
                }
            }
            catch (Exception ex)
            {
                this.logger.Error("Error looking up customer API information.", ex);
                
                // Set companyCode and authentication header to null
                Credentials = null;

                throw;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();

                if (sql != null)
                    sql.Dispose();
            }
        }
    }

    public class AuthHeader : SoapHeader
    {
        public string UserKey;
        public string UserSecret;
    }
}
