using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;


public class IP // class for our IP needs
{
    public IP(string[] user) // constructor
    {
        PropertyInfo[] properties = this.GetType().GetProperties(); // sets the properties without having to list them
        for (int i = 0; i < properties.Length; i++)
        {
            properties[i].SetValue(this, user[i]); // sets the value based on how far we are and what we got
        }
    }
    public string country { get; set; }
    public string countryCode { get; set; }
    public string stateCode { get; set; }
    public string state { get; set; }
    public string city { get; set; }
    public string zip { get; set; }
    public string latitude { get; set; }
    public string longitude { get; set; }
    public string timeZone { get; set; }
    public string isp { get; set; }
    public string org { get; set; }
    public string num { get; set; }
    public string address { get; set; }

    public override string ToString() // for the html code
    {
        string s = "";
        s += String.Format("<p>Country Code: {0}</p>", countryCode);
        s += String.Format("<p>State Code: {0}</p>", stateCode);
        s += String.Format("<p>City: {0}</p>", city);
        s += String.Format("<p>ZipCode: {0}</p>", zip);
        s += String.Format("<p>Time Zone: {0}</p>", timeZone);
        s += String.Format("<p>Internet Service Provider: {0}</p>", isp);
        s += String.Format("<p>Organization: {0}</p>", org);
        s += String.Format("<p>IP Address: {0}</p>", address);
        return s;
    }
}
