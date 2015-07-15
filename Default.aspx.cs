using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Web.UI.HtmlControls;

public partial class _Default : System.Web.UI.Page
{
    static string commandName = "select * from Orders;"; // the command that is sent to the SQL server
    static string connectionString = "Trusted_Connection=yes;server=NADAV-KEMPINSKI\\SQLEXPRESS;database=Computer;";
    static string searchInput = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) // Only on the first load of the page
        {
            query.Value = "select * from Orders;"; // necessary to stop loop
            changeVisibility(scriptType.Text); // changes the html tags to be shown at the appropriate times
            getTable();
        }
    }

    public void checkForPound(Object sender, EventArgs e)
    {
        test.InnerText = "Method called!"; // TEST HERE
        if (search.Value.Substring(search.Value.Length - 1).Equals("#"))
        {
            searchInput += search.Value;
            search.Value = "";
        }
    }

    protected void datalistHandler(Object sender, EventArgs e) // populates the datalist html 
    {
        commandName = "SELECT * FROM CLIENTBASE, ORDERS;";
        string[] suggestions = convertDatatableToStringArray(GetData()); // gets datatable based on what's already written
        for(int i=0;i<suggestions.Length;i++)
        {
            searchList.InnerHtml += String.Format("<option value=\"{0}\" />", suggestions[i]); // adds the suggesstion to the input
        }
    }

    public string[] convertDatatableToStringArray(DataTable dt) // converts a datatable given to an array of strings
    {
        int numColumns = dt.Columns.Count; // # of columns
        int numRows = dt.Rows.Count; // # of rows
        int size = numColumns * numRows; // ~# of strings to fill
        string[] partitions = new string[size]; 
        int a = 0;
        for (int i = 0; i < numColumns; i++)
        {
            foreach (DataRow row in dt.Rows) // takes each row
            {
                string colName = dt.Columns[i].ColumnName; // gets the name of the column for use in the search function

                string p = String.Format("{0} - {1}", colName, row[i].ToString()); // creates the format that looks nice and works well
                if (checkForDuplicates(partitions, p, colName, a)) // checks to see if we've already added this value in somewhere
                {
                    partitions[a++] = p; // adds it to the list
                }
            }
        }
        int n = partitions.Count<string>(s => s != null); // gets the size of the filled in portion of the array
        Array.Resize<string>(ref partitions, n); // resizes to get rid of nulls (which come up as spaces on the page)
        return partitions;
    }

    public bool checkForDuplicates(string[] list, string value, string col, int index)
    {
        for (int i = 0; i < index; i++ )
        {
            if(list[i].Equals(value) || col.Substring(col.Length-1).Equals("1")) // if the values are the same or the column is a duplicate (can't use those!)
            {
                return false;
            }
        }
        return true;
    }

    public string[] stripSpaces(string[] parts) // gets rid of the spaces around the special characters for the SQL command
    {
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Substring(0, 1).Equals(" ")) // spaces in front?
            {
                parts[i] = parts[i].Substring(1); // not anymore
            }
            if (parts[i].Substring(parts[i].Length - 1).Equals(" ")) // spaces in back?
            {
                parts[i] = parts[i].Substring(0, parts[i].Length - 1); // partaayyy
            }
        }
        return parts;
    }
    // First - Nadav, Last - Kempinski, Phone - 8588827116
    public string[] returnSplitSections(string searchText){ // splits the string into sections that we can use
        string[] parts = searchText.Split('#'); // (First - Nadav), (Last - Kempinski), (Phone - 8588827116)
        string[] sections = new string[parts.Length*2]; // size of the parts given x 2 (each part has two sides)
        int a = 0;
        foreach (string part in parts)
        {
            if (part.IndexOf('-') != -1) // Checks for the dividing -
            {
                string[] pieces = part.Split('-'); // ((First), (Nadav)), ((Last), (Kempinski)), ((Phone), (8588827116))
                foreach (string p in pieces) 
                {
                    sections[a++] = p;
                }
            }
        }
        return stripSpaces(sections); // returns the array that is stripped of spaces
    }

    protected void searchFunction(Object sender, EventArgs e) // gets the input and creates a command based of the # of elements
    {
        string searchText = searchInput + search.Value;
        
        if (searchText.IndexOf('#') != -1 || searchText.IndexOf('-') != -1) // Split based on pounds!
        {
            string[] sections = returnSplitSections(searchText);

            if (searchText.IndexOf('#') != -1)
            {
                int n = sections.Length;
                int i = 0;
                commandName = String.Format("SELECT * FROM ClientBase WHERE ({0} LIKE '%{1}%')", sections[i++], sections[i++]);
                while(i < n)
                {
                    commandName += String.Format("AND ({0} LIKE '%{1}%')", sections[i++], sections[i++]);
                }
                commandName += ";"; 
            }
            else
            {
                // commandName = String.Format("SELECT * FROM ClientBase WHERE {0} LIKE '%{1}%';", sections[0], sections[1]);
                switch (sections.Length)
                {
                    case 2: // columnName, value
                        commandName = String.Format("SELECT * FROM ClientBase WHERE {0} LIKE '%{1}%';", sections[0], sections[1]);
                        break;
                    case 3: // table, columnName, value
                        commandName = String.Format("SELECT * FROM {0} WHERE {1} LIKE '%{2}%';", sections[0], sections[1], sections[2]);
                        break;
                    case 4:
                        int s;
                        if (int.TryParse(sections[3], out s)) // table, columnName, operator, int value
                        {
                            commandName = String.Format("SELECT * FROM {0} WHERE {1} {2} {3};", sections[0], sections[1], sections[2], s);
                        }
                        else // getColumns, table, columnName, value
                        {
                            commandName = String.Format("SELECT {0] FROM {1} WHERE {2} LIKE '%{3}%';", sections[0], sections[1], sections[2], sections[3]);
                        }
                        break;
                    case 5: // getColumns, table, columnName, operator, value
                        commandName = String.Format("SELECT {0} FROM {1} WHERE {2} {3} {4};", sections[0], sections[1], sections[2], sections[3], sections[4]);
                        break;
                }
            }   
        } else { // value
            commandName = String.Format("SELECT * FROM ClientBase WHERE First LIKE '%{0}%';", searchText);
        }
        commandNameDiv.InnerText = "Command Being Sent: " + commandName; // prints command to commandNameDiv
        getTable();
    }
    
    protected void d0Handler(Object sender, EventArgs e) // whenever we change types of queries
    {
        changeVisibility(scriptType.Text);
    }

    protected void setCommandHandler(Object sender, EventArgs e) // sets the command based on our big system of inputs which is my kind of sanitary
    {
        switch (scriptType.Text) // what does kind of script are we talkin' bout here?
        {
            case "SELECT": // getcolumns, table, columnName, operator, value
                commandName = String.Format("SELECT {0} from {1} where {2} {3} {4};", selectColumn.Text, tableChoice.Text, whereColumn.Text, operatorChoice.Text, searchValue.Value);
                break;
            case "INSERT INTO": // table, newRow
                commandName = String.Format("INSERT INTO {0} VALUES ({1});", tableChoice.Text, insertValue.Value);
                break;
            case "UPDATE": // table, changeColumn, value, baseColumn, operator, baseValue
                commandName = String.Format("UPDATE {0} SET {1} = {2} WHERE {3} {4} {5}", tableChoice.Text, setColumn.Text, setValue.Value, whereColumn.Text, operatorChoice.Text, searchValue.Value);
                break;
            case "DELETE": // table, columnName, operator, value
                commandName = String.Format("DELETE FROM {0} WHERE {1} {2} {3};", tableChoice.Text, whereColumn.Text, operatorChoice.Text, searchValue.Value);
                break;
        }
        checkInputsAndGo(); // Make sure inputs don't have any rampaging illnesses
    }
    protected void queryCode(Object sender, EventArgs e) // send the command to table-press
    {
        commandName = query.Value;
        getTable();
    }

    public void checkInputsAndGo() // takes away usage of ; or - in order to sanitize totally free inputs
    {
        if(Regex.IsMatch(searchValue.Value, @"[;-]+$") || Regex.IsMatch(insertValue.Value, @"[;-]+$")) // ; and - are bad
        {
            output.InnerText = "Found invalid character(s)"; // tells the dude its not okay, and doesn't continue
        } else {
            getTable(); // healthy! let's continue
        }
    }

    public void getTable() // creates the table that we can see
    {
        DataTable dt = this.GetData(); // our beautiful SQL data

        StringBuilder html = new StringBuilder(); // yay Strings!

        html.Append("<table border = '1'>"); // Makes it look orderly

        html.Append("<tr>");
        foreach (DataColumn column in dt.Columns)
        {
            html.Append("<th>");
            html.Append(column.ColumnName);
            html.Append("</th>");
        }
        html.Append("</tr>");

        foreach (DataRow row in dt.Rows)
        {
            html.Append("<tr>");
            foreach (DataColumn column in dt.Columns)
            {
                html.Append("<td>");
                html.Append(row[column.ColumnName]);
                html.Append("</td>");
            }
            html.Append("</tr>");
        }
        html.Append("</table>");

        theTable.Controls.Clear(); // gets rid of other stuff in our table area
        theTable.Controls.Add(new Literal { Text = html.ToString() }); // Adds the text to the table we can see
    }

    private DataTable GetData() // creates the table that hold all the info
    {
        output.InnerText = "";
        using (SqlConnection con = new SqlConnection(connectionString)) // diligent usage of the connection
        {
            using (SqlCommand cmd = new SqlCommand(commandName)) // diligent usage of the command
            {
                using (SqlDataAdapter sda = new SqlDataAdapter()) // diligent usage of the dataAdapter (to change sql data to datatable data)
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    using (DataTable dt = new DataTable()) // create our table!
                    {
                        try // only if we can
                        {
                            int rowsChanged = sda.Fill(dt) + 1; // fills it while finding out how many we've changed
                            if (!scriptType.Text.Equals("SELECT")) // if we aren't dealing with a query
                            {
                                output.InnerText = String.Format("{0} row(s) affected.", rowsChanged); // tells us how much damage we've done 
                            }
                            return dt; // send the complete datatable out
                        }
                        catch (SqlException e) // so we don't have to close the webpage to deal with it
                        {
                            output.InnerHtml = "<p>Unable to Complete Request</p>" +  // i guess we can't do that for you
                                "<br /><p>" + e.Message +"</p>"; // tells us the message for debugging
                            commandName = "select * from Orders;"; // resets the commandName to avoid rebuild
                            return new DataTable(); // Send an empty table back
                        }
                    }
                }
            }
        }
    }

    protected  void getColumnsHandler(Object sender, EventArgs e) // populate our columns instead of typing them 100x!
    {
        DropDownList d = (DropDownList)sender; // beautiful use of the sender property
        d.Controls.Clear(); // get rid of it
        d.Items.Clear(); // get rid of it all!
        if (d.ID.Equals("selectColumn")) // do we deserve a gold star?
        {
            d.Items.Add(new ListItem("*")); // catch 'em all!
        }
        ListItem[] i = { // all of the columns from any/all tables. this could be done better? 
            new ListItem("id"),
            new ListItem("First"),
            new ListItem("Last"),
            new ListItem("CustomerID"),
            new ListItem("Age"),
            new ListItem("Payment"),
            new ListItem("Address"),
            new ListItem("ZipCode"),
            new ListItem("Phone"),
            new ListItem("State"),
            new ListItem("Date"),
            new ListItem("Type"),
            new ListItem("EmployeesInvolved"),
            new ListItem("DateModified")
        };
        d.Items.AddRange(i); // adds the range to the options
    }

    public void changeVisibility(string s) // changes the html parts based on whether they are needed or not.
    {
        selectColumn.Visible = s.Equals("SELECT");
        from.Visible = s.Equals("SELECT") || s.Equals("DELETE");
        tableChoice.Visible = true;
        set.Visible = s.Equals("UPDATE");
        setColumn.Visible = s.Equals("UPDATE");
        updateBreak.Visible = s.Equals("UPDATE");
        equal.Visible = s.Equals("UPDATE");
        setValue.Visible = s.Equals("UPDATE");
        where.Visible = !s.Equals("INSERT INTO");
        whereColumn.Visible = !s.Equals("INSERT INTO");
        operatorChoice.Visible = !s.Equals("INSERT INTO");
        searchValue.Visible = !s.Equals("INSERT INTO");
        insertValue.Visible = s.Equals("INSERT INTO");
    }

    public string[] getIP() // gets us the ip using two webservices
    {
        WebClient w = new WebClient();
        string ip = w.DownloadString("http://icanhazip.com"); // the actual ip of the user
        ip = w.DownloadString(String.Format("http://ip-api.com/line/{0}?fields=12287", ip)); // stalking info bout the user
        string[] sections = ip.Split('\n'); // thank god this works it splits it all up to sections for the IP class
        return sections;
    }
}
