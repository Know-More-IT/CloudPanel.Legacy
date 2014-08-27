using CloudPanel.Modules.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class LyncPlan : IPlan
    {
        /// <summary>
        /// The maximum number of participants allowed to a conference
        /// </summary>
        private int _maxmeetingsize;
        public int MaxMeetingSize
        {
            get 
            {
                if (_maxmeetingsize == -1)
                    return 250;
                else
                    return _maxmeetingsize; 
            }
            set { _maxmeetingsize = value; }
        }

        /// <summary>
        /// If federation is allowed or not
        /// </summary>
        private bool _enablefederation;
        public bool EnableFederation
        {
            get { return _enablefederation; }
            set { _enablefederation = value; }
        }

        /// <summary>
        /// If they are allowed to communite with public providers such as Skype, MSN, etc
        /// </summary>
        private bool _enablepubliccloudaccess;
        public bool EnablePublicCloudAccess
        {
            get { return _enablepubliccloudaccess; }
            set { _enablepubliccloudaccess = value; }
        }

        /// <summary>
        /// If they are allowed to use audio and video with public providers
        /// </summary>
        private bool _enablepubliccloudaudiovideoaccess;
        public bool EnablePublicCloudAudioVideoAccess
        {
            get { return _enablepubliccloudaudiovideoaccess; }
            set { _enablepubliccloudaudiovideoaccess = value; }
        }

        /// <summary>
        /// If they are allowed to use Lync Mobile
        /// </summary>
        private bool _enablemobility;
        public bool EnableMobility
        {
            get { return _enablemobility; }
            set { _enablemobility = value; }
        }

        /// <summary>
        /// If they can use the Call via Work feature on their mobile device
        /// </summary>
        private bool _enableoutsidevoice;
        public bool EnableOutsideVoice
        {
            get { return _enableoutsidevoice; }
            set { _enableoutsidevoice = value; }
        }

        /// <summary>
        /// If they are allowed to use IP Video for conferences
        /// </summary>
        private bool _allowipvideo;
        public bool AllowIPVideo
        {
            get { return _allowipvideo; }
            set { _allowipvideo = value; }
        }

        /// <summary>
        /// This will always be true because in a Lync hosting
        /// pack everyone is supposed to be external
        /// </summary>
        public bool EnableOutsideAccess
        {
            get { return true; }
        }

    }
}
