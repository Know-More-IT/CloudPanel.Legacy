using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base
{
    public class BasePublicFolder
    {
        /// <summary>
        /// The Identity of the public folder (also the path)
        /// </summary>
        private string _path;
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        /// <summary>
        /// Name of the public folder
        /// </summary>
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        /// <summary>
        /// The parent path of the public folder
        /// </summary>
        private string _parentpath;
        public string ParentPath
        {
            get { return _parentpath; }
            set { _parentpath = value; }
        }

        /// <summary>
        /// Age limit of the public folder ( in days )
        /// </summary>
        private string _agelimit;
        public string AgeLimit
        {
            get { return _agelimit; }
            set { _agelimit = value; }
        }

        /// <summary>
        /// How long to retain deleted items ( in days )
        /// </summary>
        private string _retaindeleteditemsfor;
        public string RetainDeletedItemsFor
        {
            get { return _retaindeleteditemsfor; }
            set { _retaindeleteditemsfor = value; }
        }

        /// <summary>
        /// When they cannot post anymore in MB
        /// </summary>
        private string _prohibitpostquota;
        public string ProhibitPostQuota
        {
            get { return _prohibitpostquota; }
            set { _prohibitpostquota = value; }
        }

        /// <summary>
        /// When to issue a warning in MB
        /// </summary>
        private string _issuewarningquota;
        public string IssueWarningQuota
        {
            get { return _issuewarningquota; }
            set { _issuewarningquota = value; }
        }

        /// <summary>
        /// Max size of an item
        /// </summary>
        private string _maxitemsize;
        public string MaxItemSize
        {
            get { return _maxitemsize; }
            set { _maxitemsize = value; }
        }

        /// <summary>
        /// The type of public folder it is
        /// </summary>
        private string _folderclass;
        public string FolderClass
        {
            get { return _folderclass; }
            set { _folderclass = value; }
        }

        /// <summary>
        /// If the public folder is mail enabled or not
        /// </summary>
        private bool _mailenabled;
        public bool MailEnabled
        {
            get { return _mailenabled; }
            set { _mailenabled = value; }
        }

        /// <summary>
        /// Per user read state from the public folder
        /// </summary>
        private bool _peruserreadstateenabled;
        public bool PerUserReadStateEnabled
        {
            get { return _peruserreadstateenabled; }
            set { _peruserreadstateenabled = value; }
        }

        /// <summary>
        /// If the folder has any sub folders
        /// </summary>
        private bool _hassubfolders;
        public bool HasSubFolders
        {
            get { return _hassubfolders; }
            set { _hassubfolders = value; }
        }


        /// <summary>
        /// Holds information about the mail public folder if it is enabled
        /// </summary>
        private BaseMailPublicFolder _mailfolderinfo;
        public BaseMailPublicFolder MailFolderInfo
        {
            get { return _mailfolderinfo; }
            set { _mailfolderinfo = value; }
        }

    }
}
