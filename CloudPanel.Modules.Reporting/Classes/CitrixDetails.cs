using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CitrixDetails
{
    /// <summary>
    /// Display name of the application
    /// </summary>
    private string _displayname;
    public string DiplayName
    {
        get { return _displayname; }
        set { _displayname = value; }
    }

    /// <summary>
    /// The name of the company
    /// </summary>
    private string _companyname;
    public string CompanyName
    {
        get { return _companyname; }
        set { _companyname = value; }
    }

    /// <summary>
    /// The unique identifier of the company
    /// </summary>
    private string _companycode;
    public string CompanyCode
    {
        get { return _companycode; }
        set { _companycode = value; }
    }

    /// <summary>
    /// The number of users for the application
    /// </summary>
    private int _usercount;
    public int UserCount
    {
        get { return _usercount; }
        set { _usercount = value; }
    }

    /// <summary>
    /// If it is a server or not
    /// </summary>
    private bool _isserver;
    public bool IsServer
    {
        get { return _isserver; }
        set { _isserver = value; }
    }

    /// <summary>
    /// The cost of the application/server per user
    /// </summary>
    private Decimal _cost;
    public Decimal Cost
    {
        get { return _cost; }
        set { _cost = value; }
    }

    /// <summary>
    /// The price of the application/server per user
    /// </summary>
    private Decimal _price;
    public Decimal Price
    {
        get { return _price; }
        set { _price = value; }
    }

}
