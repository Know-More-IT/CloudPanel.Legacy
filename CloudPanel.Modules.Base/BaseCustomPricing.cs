using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public class BaseCustomPricing
    {
        public int PlanID { get; set; }

        public string Product { get; set; }
        public string CustomPrice { get; set; }
    }
}
