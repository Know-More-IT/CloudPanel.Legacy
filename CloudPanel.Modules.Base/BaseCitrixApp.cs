using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public class BaseCitrixApp
    {
        public int ID { get; set; }
        public int CurrentUsers { get; set; }

        public string DisplayName { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string CompanyCode { get; set; }
        public string Cost { get; set; }
        public string Price { get; set; }
        public string CustomPrice { get; set; }
        public string PictureURL { get; set; }

        public bool IsServer { get; set; }
    }
}
