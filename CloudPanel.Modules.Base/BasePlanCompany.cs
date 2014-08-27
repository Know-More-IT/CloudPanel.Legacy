using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public class BasePlanCompany
    {
        public int PlanID { get; set; }
        public int ProductID { get; set; }
        public int MaxUsers { get; set; }
        public int MaxDomains { get; set; }
        public int MaxExchangeMailboxes { get; set; }
        public int MaxExchangeContacts { get; set; }
        public int MaxExchangeDistLists { get; set; }
        public int MaxExchangePublicFolders { get; set; }
        public int MaxExchangeMailPublicFolders { get; set; }
        public int MaxExchangeKeepDeletedItems { get; set; }
        public int MaxExchangeResourceMailboxes { get; set; }
        public int MaxCitrixUsers { get; set; }

        public string PlanName { get; set; }
    }
}
