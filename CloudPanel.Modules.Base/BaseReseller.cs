using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public class BaseReseller
    {
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public string AdminName { get; set; }
        public string AdminEmail { get; set; }

        public DateTime Created { get; set; }
    }
}
