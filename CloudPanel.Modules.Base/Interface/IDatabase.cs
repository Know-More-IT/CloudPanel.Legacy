using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Interface
{
    public class IDatabase
    {
        /// <summary>
        /// The identity of the database
        /// </summary>
        private string _identity;
        public string Identity
        {
            get { return _identity; }
            set { _identity = value; }
        }

        /// <summary>
        /// The server the database is on
        /// </summary>
        private string _server;
        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }

        /// <summary>
        /// When the database was created
        /// </summary>
        private DateTime _whencreated;
        public DateTime WhenCreated
        {
            get { return _whencreated; }
            set { _whencreated = value; }
        }

        /// <summary>
        /// When the database was information was retrieved
        /// </summary>
        private DateTime _whenretrieved;
        public DateTime WhenRetrieved
        {
            get { return _whenretrieved; }
            set { _whenretrieved = value; }
        }
    }
}
