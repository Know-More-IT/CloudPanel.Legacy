using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ExchangeDetails
{
    /// <summary>
    /// The unique code of the company
    /// </summary>
    private string _companycode;
    public string CompanyCode
    {
        get { return _companycode; }
        set { _companycode = value; }
    }

    /// <summary>
    /// The company's name
    /// </summary>
    private string _companyname;
    public string CompanyName
    {
        get { return _companyname; }
        set { _companyname = value; }
    }

    /// <summary>
    /// UPN of the user
    /// </summary>
    private string _userprincipalname;
    public string UserPrincipalName
    {
        get { return _userprincipalname; }
        set { _userprincipalname = value; }
    }

    #region Exchange

    /// <summary>
    /// The name of the Exchange plan
    /// </summary>
    private string _mailboxplanname;
    public string MailboxPlanName
    {
        get { return _mailboxplanname; }
        set { _mailboxplanname = value; }
    }

    /// <summary>
    /// The ID of the mailbox plan
    /// </summary>
    private int _mailboxplanid;
    public int MailboxPlanID
    {
        get { return _mailboxplanid; }
        set { _mailboxplanid = value; }
    }

    /// <summary>
    /// The number of mailboxes for this plan
    /// </summary>
    private int _mailboxplancount;
    public int MailboxPlanCount
    {
        get { return _mailboxplancount; }
        set { _mailboxplancount = value; }
    }

    /// <summary>
    /// Additional MB added to the plan
    /// </summary>
    private Decimal _additionalgb;
    public Decimal AdditionalGB
    {
        get { return _additionalgb; }
        set { _additionalgb = value; }
    }

    #endregion

    #region Pricing

    /// <summary>
    /// The cost for the Exchange plan for the user (Hosters cost)
    /// </summary>
    private decimal _exchplancost;
    public decimal ExchangePlanCost
    {
        get { return _exchplancost; }
        set { _exchplancost = value; }
    }

    /// <summary>
    /// The price for the Exchange plan for the user (customer's cost... customer could have custom pricing)
    /// </summary>
    private decimal _exchplanprice;
    public decimal ExchangePlanPrice
    {
        get { return _exchplanprice; }
        set { _exchplanprice = value; }
    }

    /// <summary>
    /// The price for additional GB added to the mailbox (customer's cost)
    /// </summary>
    private decimal _exchadditionalgbprice;
    public decimal ExchangeAdditionalGBPrice
    {
        get { return _exchadditionalgbprice; }
        set { _exchadditionalgbprice = value; }
    }

    #endregion
}
