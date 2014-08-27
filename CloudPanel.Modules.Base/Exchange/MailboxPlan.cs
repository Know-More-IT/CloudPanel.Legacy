using CloudPanel.Modules.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class MailboxPlan : IPlan
    {
        /// <summary>
        /// The size of the mailbox that it is currently set to
        /// </summary>
        private int _setsizeinmb;
        public int SetSizeInMB
        {
            get { return _setsizeinmb; }
            set { _setsizeinmb = value; }
        }

        /// <summary>
        /// Gets the warning size in megabytes
        /// Can only call this if the 'SetSizeInMB' is set
        /// </summary>
        private int _warningsizeinmb;
        public int WarningSizeInMB
        {
            get
            {
                decimal percentConverted = decimal.Divide(WarningSizeInPercent, 100);
                decimal total = decimal.Multiply(SetSizeInMB, percentConverted);

                return decimal.ToInt32(total);
            }
        }

        /// <summary>
        /// The warning size when to trigger a warning to the user
        /// </summary>
        private int _warningsizeinpercent;
        public int WarningSizeInPercent
        {
            get
            {
                if (_warningsizeinpercent < 50)
                    return 90;
                else
                    return _warningsizeinpercent;
            }
            set { _warningsizeinpercent = value; }
        }

        /// <summary>
        /// The size of the mailbox plan in megabytes
        /// </summary>
        private int _sizeinmb;
        public int SizeInMB
        {
            get { return _sizeinmb; }
            set { _sizeinmb = value; }
        }

        /// <summary>
        /// Gets how much additional MB was added based on the plan
        /// </summary>
        private int _additionalmbadded;
        public int AdditionalMBAdded
        {
            get
            {
                // Subject the set size minus the default size
                // to get how much was added
                return SetSizeInMB - SizeInMB; 
            }
        }

        /// <summary>
        /// The maximum size of the mailbox plan
        /// </summary>
        private int _maximumsizeinmb;
        public int MaximumSizeInMB
        {
            get { return _maximumsizeinmb; }
            set { _maximumsizeinmb = value; }
        }

        /// <summary>
        /// The maximum email size the mailbox can send in kilobytes
        /// </summary>
        private int _maxsendsizeinkb;
        public int MaxSendSizeInKB
        {
            get { return _maxsendsizeinkb; }
            set { _maxsendsizeinkb = value; }
        }

        /// <summary>
        /// The maximum receive size in kilobytes
        /// </summary>
        private int _maxreceivesizeinkb;
        public int MaxReceiveSizeInKB
        {
            get { return _maxreceivesizeinkb; }
            set { _maxreceivesizeinkb = value; }
        }

        /// <summary>
        /// The maximum amount of people a user can send in a single email message
        /// </summary>
        private int _maxrecipients;
        public int MaxRecipients
        {
            get { return _maxrecipients; }
            set { _maxrecipients = value; }
        }

        /// <summary>
        /// How long to retain deleted items (in days)
        /// </summary>
        private int _keepdeleteditems;
        public int KeepDeletedItemsInDays
        {
            get { return _keepdeleteditems; }
            set { _keepdeleteditems = value; }
        }

        /// <summary>
        /// If POP3 is enabled or not for this plan
        /// </summary>
        private bool _pop3enabled;
        public bool POP3Enabled
        {
            get { return _pop3enabled; }
            set { _pop3enabled = value; }
        }

        /// <summary>
        /// If IMAP is enabled for this plan or not
        /// </summary>
        private bool _imapenabled;
        public bool IMAPEnabled
        {
            get { return _imapenabled; }
            set { _imapenabled = value; }
        }

        /// <summary>
        /// If OWA is enabled or not for this plan
        /// </summary>
        private bool _owaenabled;
        public bool OWAEnabled
        {
            get { return _owaenabled; }
            set { _owaenabled = value; }
        }

        /// <summary>
        /// If MAPI is enabled or not for this plan
        /// </summary>
        private bool _mapienabled;
        public bool MAPIEnabled
        {
            get { return _mapienabled; }
            set { _mapienabled = value; }
        }

        /// <summary>
        /// If ActiveSync is enabled for this plan or not
        /// </summary>
        private bool _activesyncenabled;
        public bool ActiveSyncEnabled
        {
            get { return _activesyncenabled; }
            set { _activesyncenabled = value; }
        }

        /// <summary>
        /// If ECP is enabled for this plan or not
        /// </summary>
        private bool _ecpenabled;
        public bool ECPEnabled
        {
            get { return _ecpenabled; }
            set { _ecpenabled = value; }
        }

        /// <summary>
        /// The cost for each additional gigabyte of storage for this plan
        /// </summary>
        private string _additionalgbprice;
        public string AdditionalGBPrice
        {
            get { return _additionalgbprice; }
            set { _additionalgbprice = value; }
        }
    }
}
