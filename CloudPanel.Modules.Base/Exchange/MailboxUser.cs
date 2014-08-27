using CloudPanel.Modules.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class MailboxUser : IMailbox
    {
        /// <summary>
        /// The type of the mailbox user (Could be a contact, distribution group, etc)
        /// </summary>
        private string _objecttype;
        public string ObjectType
        {
            get { return _objecttype; }
            set { _objecttype = value; }
        }

        /// <summary>
        /// First name of the user for this mailbox
        /// </summary>
        private string _firstname;
        public string FirstName
        {
            get { return _firstname; }
            set { _firstname = value; }
        }

        /// <summary>
        /// Last name of the user for this mailbox
        /// </summary>
        private string _lastname;
        public string LastName
        {
            get { return _lastname; }
            set { _lastname = value; }
        }

        /// <summary>
        /// The mailbox database the user belongs to
        /// </summary>
        private string _database;
        public string Database
        {
            get { return _database; }
            set { _database = value; }
        }

        /// <summary>
        /// How many items are currently in the mailbox
        /// </summary>
        private int _itemcount;
        public int ItemCount
        {
            get { return _itemcount; }
            set { _itemcount = value; }
        }

        /// <summary>
        /// How many deleted items are in the mailbox
        /// </summary>
        private int _deleteditemcount;
        public int DeletedItemCount
        {
            get { return _deleteditemcount; }
            set { _deleteditemcount = value; }
        }

        /// <summary>
        /// The total item size of the mailbox
        /// </summary>
        private string _totalitemsize;
        public string TotalItemSize
        {
            get 
            { 
                if (!string.IsNullOrEmpty(_totalitemsize))
                    return FormatExchangeSize(_totalitemsize);
                else
                    return _totalitemsize; 
            }
            set { _totalitemsize = value; }
        }

        /// <summary>
        /// The total deleted item size of the mailbox
        /// </summary>
        private string _totaldeleteditemsize;
        public string TotalDeletedItemSize
        {
            get 
            {
                if (!string.IsNullOrEmpty(_totaldeleteditemsize))
                    return FormatExchangeSize(_totaldeleteditemsize);
                else
                    return _totaldeleteditemsize; 
            }
            set { _totaldeleteditemsize = value; }
        }

        /// <summary>
        /// When the mailbox data was retrieved
        /// </summary>
        private DateTime _mailboxdataretrieved;
        public DateTime MailboxDataRetrieved
        {
            get { return _mailboxdataretrieved; }
            set { _mailboxdataretrieved = value; }
        }
    }
}
