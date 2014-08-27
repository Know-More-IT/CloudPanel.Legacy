using CloudPanel.Modules.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class CitrixPlan : IPlan
    {
        /// <summary>
        /// The name of the security group for the plan
        /// </summary>
        private string _groupname;
        public string GroupName
        {
            get { return _groupname; }
            set { _groupname = value; }
        }

        /// <summary>
        /// Picture for the Citrix Plan
        /// </summary>
        private string _pictureurl;
        public string PictureUrl
        {
            get { return _pictureurl; }
            set { _pictureurl = value; }
        }

        /// <summary>
        /// If the Citrix plan is a application or a server/desktop
        /// </summary>
        private bool _isserver;
        public bool IsServer
        {
            get { return _isserver; }
            set { _isserver = value; }
        }
    }
}
