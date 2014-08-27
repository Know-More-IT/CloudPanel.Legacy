using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TotalStats
{
    /// <summary>
    /// Total mailbox count in the database
    /// </summary>
    private int _mailboxcount;
    public int MailboxCount
    {
        get { return _mailboxcount; }
        set { _mailboxcount = value; }
    }

    /// <summary>
    /// Total amount of users that have Citrix applications or servers
    /// </summary>
    private int _citrixusercount;
    public int CitrixUserCount
    {
        get { return _citrixusercount; }
        set { _citrixusercount = value; }
    }

    /// <summary>
    /// Total user count in the database
    /// </summary>
    private int _usercount;
    public int UserCount
    {
        get { return _usercount; }
        set { _usercount = value; }
    }
}