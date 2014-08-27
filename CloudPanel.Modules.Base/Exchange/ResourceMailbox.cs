using CloudPanel.Modules.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class ResourceMailbox : IMailbox
    {

        #region Integers

        /// <summary>
        /// The capacity of the resource mailbox
        /// </summary>
        private int _resourcecapacity;
        public int ResourceCapacity
        {
            get { return _resourcecapacity; }
            set { _resourcecapacity = value; }
        }

        /// <summary>
        /// Number of days for which meetings are allowed to be booked in advance
        /// </summary>
        private int _bookingwindowindays;
        public int BookingWindowInDays
        {
            get { return _bookingwindowindays; }
            set { _bookingwindowindays = value; }
        }

        /// <summary>
        /// Maximum duration of a meeting in minutes
        /// </summary>
        private int _maximumdurationinmin;
        public int MaximumDurationInMinutes
        {
            get { return _maximumdurationinmin; }
            set { _maximumdurationinmin = value; }
        }

        /// <summary>
        /// How many conflict instances are allowed
        /// </summary>
        private int _maximumconflictinstances;
        public int MaximumConflictInstances
        {
            get { return _maximumconflictinstances; }
            set { _maximumconflictinstances = value; }
        }

        /// <summary>
        /// Maximum percentage of meeting conflicts for new recurring meeting requests.
        /// Values: 0 through 100
        /// Default: 0 (no conflicts)
        /// </summary>
        private int _conflictpercentageallowed;
        public int ConflictPercentageAllowed
        {
            get { return _conflictpercentageallowed; }
            set { _conflictpercentageallowed = value; }
        }

        #endregion

        #region Strings
        /// <summary>
        /// Custom resource properties that are searchable by users
        /// </summary>
        private string _resourcecustom;
        public string ResourceCustom
        {
            get { return _resourcecustom; }
            set { _resourcecustom = value; }
        }

        /// <summary>
        /// The type of resource (Room or Equipment)
        /// </summary>
        private string _resourcetype;
        public string ResourceType
        {
            get { return _resourcetype; }
            set { _resourcetype = value; }
        }

        /// <summary>
        /// Enables or disables calendar processing
        /// Values: None, AutoUpdate, AutoAccept
        /// Default: AutoUpdate
        /// </summary>
        private string _automateprocessing;
        public string AutomateProcessing
        {
            get { return _automateprocessing; }
            set { _automateprocessing = value; }
        }

        /// <summary>
        /// First Name for the mailbox
        /// </summary>
        private string _firstname;
        public string FirstName
        {
            get { return _firstname; }
            set { _firstname = value; }
        }

        /// <summary>
        /// Last name for the mailbox
        /// </summary>
        private string _lastname;
        public string LastName
        {
            get { return _lastname; }
            set { _lastname = value; }
        }

        /// <summary>
        /// Custom response message that the meeting organizer will receive
        /// </summary>
        private string _additionaltext;
        public string AdditionalText
        {
            get { return _additionaltext; }
            set { _additionaltext = value; }
        }

        #endregion

        #region Arrays

        /// <summary>
        /// List of users who are allowed to submit inpolicy meeting requests
        /// Any requests from these users are automatically approved
        /// </summary>
        private string[] _bookinpolicy;
        public string[] BookInPolicy
        {
            get { return _bookinpolicy; }
            set { _bookinpolicy = value; }
        }

        /// <summary>
        /// Resource delegates
        /// </summary>
        private string[] _resourcedelegates;
        public string[] ResourceDelegates
        {
            get { return _resourcedelegates; }
            set { _resourcedelegates = value; }
        }

        #endregion

        #region Boolean

        /// <summary>
        /// If you can allow conflicts for this resource mailbox or not
        /// </summary>
        private bool _allowconflicts;
        public bool AllowConflicts
        {
            get { return _allowconflicts; }
            set { _allowconflicts = value; }
        }

        /// <summary>
        /// If you can allow meetings that repeat
        /// </summary>
        private bool _allowrepeatingmeetings;
        public bool AllowRepeatingMeetings
        {
            get { return _allowrepeatingmeetings; }
            set { _allowrepeatingmeetings = value; }
        }

        /// <summary>
        /// If the Resource Booking Attendant is activated or not
        /// </summary>
        private bool _enablebookingattendant;
        public bool BookingAttendantEnabled
        {
            get { return _enablebookingattendant; }
            set { _enablebookingattendant = value; }
        }

        /// <summary>
        /// If the resource mailbox only schedules during work hours
        /// </summary>
        private bool _schedulingduringworkhours;
        public bool SchedulingOnlyDuringWorkHours
        {
            get { return _schedulingduringworkhours; }
            set { _schedulingduringworkhours = value; }
        }

        /// <summary>
        /// If it automatically rejects meetings outside booking window
        /// </summary>
        private bool _rejectoutsidebookingwindow;
        public bool RejectOutsideBookingWindow
        {
            get { return _rejectoutsidebookingwindow; } 
            set { _rejectoutsidebookingwindow = value; }
        }

        /// <summary>
        /// If the mailbox is a resource mailbox or not
        /// </summary>
        private bool _isresource;
        public bool IsResource
        {
            get { return _isresource; }
            set { _isresource = value; }
        }

        /// <summary>
        /// If attachments are automatically deleted
        /// </summary>
        private bool _deleteattachments;
        public bool DeleteAttachments
        {
            get { return _deleteattachments; }
            set { _deleteattachments = value; }
        }

        /// <summary>
        /// If comments are automatically deleted
        /// </summary>
        private bool _deletecomments;
        public bool DeleteComments
        {
            get { return _deletecomments; }
            set { _deletecomments = value; }
        }

        /// <summary>
        /// If subject is automatically deleted
        /// </summary>
        private bool _deletesubject;
        public bool DeleteSubject
        {
            get { return _deletesubject; }
            set { _deletesubject = value; }
        }

        /// <summary>
        /// If non calendar items are automatically deleted
        /// </summary>
        private bool _deletenoncalendaritems;
        public bool DeleteNonCalendarItems
        {
            get { return _deletenoncalendaritems; }
            set { _deletenoncalendaritems = value; }
        }

        /// <summary>
        /// If the person's name scheduling is automatically added to the subject
        /// </summary>
        private bool _addorganizernametosubject;
        public bool AddOrganizerNameToSubject
        {
            get { return _addorganizernametosubject; }
            set { _addorganizernametosubject = value; }
        }

        /// <summary>
        /// If the private flag is automatically removed from an accepted meeting
        /// </summary>
        private bool _removeprivateflag;
        public bool RemovePrivateFlag
        {
            get { return _removeprivateflag; }
            set { _removeprivateflag = value; }
        }

        /// <summary>
        /// Sends orgnaizer info when a meeting is declined because of conflicts
        /// </summary>
        private bool _sendorganizerinfoondecline;
        public bool SendOrganizerInfoOnDecline
        {
            get { return _sendorganizerinfoondecline; }
            set { _sendorganizerinfoondecline = value; }
        }

        /// <summary>
        /// If pending requests are automatically marked as Tentative
        /// </summary>
        private bool _markpendingastentative;
        public bool MarkPendingRequestsAsTentative
        {
            get { return _markpendingastentative; }
            set { _markpendingastentative = value; }
        }

        /// <summary>
        /// If forwarding all requests to delegates is enabled or not
        /// </summary>
        private bool _forwardrequeststodelegates;
        public bool ForwardRequestsToDelegates
        {
            get { return _forwardrequeststodelegates; }
            set { _forwardrequeststodelegates = value; }
        }

        /// <summary>
        /// If an additional response is added or not
        /// </summary>
        private bool _addadditionalresponse;
        public bool AddAdditionalResponse
        {
            get { return _addadditionalresponse; }
            set { _addadditionalresponse = value; }
        }

        #endregion
    }
}
