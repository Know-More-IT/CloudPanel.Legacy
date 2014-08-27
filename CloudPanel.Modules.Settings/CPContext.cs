using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace CloudPanel.Modules.Settings
{
    public class CPContext
    {
        //******************************************
        // Returns the reseller code the current user
        // either selected or is part of. Returns null 
        // if it is not set
        //******************************************
        public static string SelectedResellerCode
        {
            get
            {
                if (HttpContext.Current.Session["SelectedResellerCode"] == null)
                    return null;
                else
                    return HttpContext.Current.Session["SelectedResellerCode"].ToString();
            }
            set
            {
                HttpContext.Current.Session["SelectedResellerCode"] = value;
            }
        }

        //******************************************
        // Returns the reseller name the current user
        // either selected or is part of. Returns null 
        // if it is not set
        //******************************************
        public static string SelectedResellerName
        {
            get
            {
                if (HttpContext.Current.Session["SelectedResellerName"] == null)
                    return null;
                else
                    return HttpContext.Current.Session["SelectedResellerName"].ToString();
            }
            set
            {
                HttpContext.Current.Session["SelectedResellerName"] = value;
            }
        }

        //******************************************
        // Returns the company code the current user
        // either selected or is part of. Returns null 
        // if it is not set
        //******************************************
        public static string SelectedCompanyCode
        {
            get
            {
                if (HttpContext.Current.Session["SelectedCompanyCode"] == null)
                    return null;
                else
                    return HttpContext.Current.Session["SelectedCompanyCode"].ToString();
            }
            set
            {
                HttpContext.Current.Session["SelectedCompanyCode"] = value;
            }
        }

        //******************************************
        // Returns the company name the current user
        // either selected or is part of. Returns null 
        // if it is not set
        //******************************************
        public static string SelectedCompanyName
        {
            get
            {
                if (HttpContext.Current.Session["SelectedCompanyName"] == null)
                    return null;
                else
                    return HttpContext.Current.Session["SelectedCompanyName"].ToString();
            }
            set
            {
                HttpContext.Current.Session["SelectedCompanyName"] = value;
            }
        }

        //******************************************
        // 
        // Returns the current user's login name
        // 
        //******************************************
        public static string LoggedInUserName
        {
            get
            {
                return HttpContext.Current.User.Identity.Name;
            }
        }

        //******************************************
        // Gets or sets the culture info for the
        // user's session for formatting purposes
        //******************************************
        public static CultureInfo UsersCulture
        {
            get
            {
                if (HttpContext.Current.Session["CPCulture"] == null)
                    return CultureInfo.InvariantCulture;
                else
                    return (CultureInfo)HttpContext.Current.Session["CPCulture"];
            }
            set
            {
                HttpContext.Current.Session["CPCulture"] = value;
            }
        }
    }
}
