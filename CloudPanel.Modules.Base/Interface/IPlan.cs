using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Interface
{
    public class IPlan
    {
        /// <summary>
        /// Plan ID For the plan
        /// </summary>
        private int _planid;
        public int PlanID
        {
            get { return _planid; }
            set { _planid = value; }
        }

        /// <summary>
        /// Display Name for the plan
        /// </summary>
        private string _displayname;
        public string DisplayName
        {
            get { return _displayname; }
            set { _displayname = value; }
        }

        /// <summary>
        /// How much the plan cost you (the hoster)
        /// </summary>
        private string _cost;
        public string Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }

        /// <summary>
        /// How much the plan cost the customer
        /// </summary>
        private string _price;
        public string Price
        {
            get { return _price; }
            set { _price = value; }
        }

        /// <summary>
        /// The custom price if it is overridden on the customer level
        /// </summary>
        private string _customprice;
        public string CustomPrice
        {
            get { return _customprice; }
            set { _customprice = value; }
        }

        /// <summary>
        /// Descriptiong of the plan
        /// </summary>
        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Company code that the plan is assigned to.
        /// This can be null or empty
        /// </summary>
        private string _companycode;
        public string CompanyCode
        {
            get { return _companycode; }
            set { _companycode = value; }
        }

    }
}
