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

public partial class _Default : System.Web.UI.Page
{
    static string commandName = "select * from Orders;";
    static string connectionString = "Trusted_Connection=yes;server=NADAV-KEMPINSKI\\SQLEXPRESS;database=Computer;";

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            query.Value = "select * from Orders;";
            changeVisibility(scriptType.Text);
            getTable();
        }
    }

    public void datalistHandler(Object sender, EventArgs e)
    {
        commandName = "SELECT * FROM CLIENTBASE, ORDERS;";
        string[] suggestions = convertDatatableToStringArray(GetData()); // provides info from last row
        string[] suggestions = convertDatatableToStringArray(GetData());
        for(int i=0;i<suggestions.Length;i++)
        {
            searchList.InnerHtml += String.Format("<option value=\"{0}\" />", suggestions[i]);
        }
    }

    public string[] convertDatatableToStringArray(DataTable dt)
    {
        int numColumns = dt.Columns.Count;
        int numRows = dt.Rows.Count;
        int size = numColumns * numRows;
        string[] partitions = new string[size];
        int a = 0;
        for (int i = 0; i < numColumns; i++)
        {
            foreach (DataRow row in dt.Rows)
            {
                string colName = dt.Columns[i].ColumnName;

                string p = String.Format("{0} - {1}", colName, row[i].ToString());
                if (checkForDuplicates(partitions, p, colName, a))
                {
                    partitions[a++] = p;
                }
            }
        }
        int n = partitions.Count<string>(s => s != null);
        Array.Resize<string>(ref partitions, n);
        return partitions;
    }

    public bool checkForDuplicates(string[] list, string value, string col, int index)
    {
        for (int i = 0; i < index; i++ )
        {
            if(list[i].Equals(value) || col.Substring(col.Length-1).Equals("1"))
            {
                return false;
            }
        }
        return true;
            
    }

    public void searchFunction(Object sender, EventArgs e)
    {
        string searchText = search.Value;

        if (searchText.IndexOf('-') != -1)
        {
            string[] sections = searchText.Split('-');
            for (int i = 0; i < sections.Length; i++)
            {
                if (sections[i].Substring(0, 1).Equals(" "))
                {
                    sections[i] = sections[i].Substring(1);
                }
                else if (sections[i].Substring(sections[i].Length - 1).Equals(" "))
                {
                    sections[i] = sections[i].Substring(0, sections[i].Length - 1);
                }
            }
            switch (sections.Length)
            {
                case 2:
                    commandName = String.Format("SELECT * FROM ClientBase, Orders WHERE {0} LIKE '%{1}%';", sections[0], sections[1]);
                    break;
                case 3:
                    commandName = String.Format("SELECT * FROM {0} WHERE {1} LIKE '%{2}%';", sections[0], sections[1], sections[2]);
                    break;
                case 4:
                    int s;
                    if (int.TryParse(sections[3], out s))
                    {
                        commandName = String.Format("SELECT * FROM {0} WHERE {1} {2} {3};", sections[0], sections[1], sections[2], s);
                    }
                    else
                    {
                        commandName = String.Format("SELECT {0] FROM {1} WHERE {2} LIKE '%{3}%';", sections[0], sections[1], sections[2], sections[3]);
                    }
                    break;
                case 5:
                    commandName = String.Format("SELECT {0} FROM {1} WHERE {2} {3} {4}", sections[0], sections[1], sections[2], sections[3], sections[4]);
                    break;
            }
        } else {
            commandName = String.Format("SELECT * FROM ClientBase, Orders WHERE First LIKE '%{0}%';", searchText);
        }
        commandNameDiv.InnerText = "Command Being Sent: " + commandName;
        getTable();
    }
    
    public void d0Handler(Object sender, EventArgs e)
    {
        changeVisibility(scriptType.Text);
    }

    public void setCommandHandler(Object sender, EventArgs e)
    {
        switch (scriptType.Text)
        {
            case "SELECT":
                commandName = String.Format("SELECT {0} from {1} where {2} {3} {4};", selectColumn.Text, tableChoice.Text, whereColumn.Text, operatorChoice.Text, searchValue.Value);
                break;
            case "INSERT INTO":
                commandName = String.Format("INSERT INTO {0} VALUES ({1});", tableChoice.Text, insertValue.Value);
                break;
            case "UPDATE":
                commandName = String.Format("UPDATE {0} SET {1} = {2} WHERE {3} {4} {5}", tableChoice.Text, setColumn.Text, setValue.Value, whereColumn.Text, operatorChoice.Text, searchValue.Value);
                break;
            case "DELETE":
                commandName = String.Format("DELETE FROM {0} WHERE {1} {2} {3};", tableChoice.Text, whereColumn.Text, operatorChoice.Text, searchValue.Value);
                break;
        }
        checkInputsAndGo();
    }
    public void queryCode(Object sender, EventArgs e)
    {
        commandName = query.Value;
        getTable();
    }

    public void checkInputsAndGo()
    {
        if(Regex.IsMatch(searchValue.Value, @"[;-]+$") || Regex.IsMatch(insertValue.Value, @"[;-]+$"))
        {
            output.InnerText = "Found invalid character(s)";
        } else {
            getTable();
        }
    }

    public void getTable()
    {
        DataTable dt = this.GetData();

        StringBuilder html = new StringBuilder();

        html.Append("<table border = '1'>");

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

        theTable.Controls.Clear();
        theTable.Controls.Add(new Literal { Text = html.ToString() });
    }

    private DataTable GetData()
    {
        output.InnerText = "";
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(commandName))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    using (DataTable dt = new DataTable())
                    {
                        try
                        {
                            int rowsChanged = sda.Fill(dt) + 1;
                            if (!scriptType.Text.Equals("SELECT"))
                            {
                                output.InnerText = String.Format("{0} row(s) affected.", rowsChanged);
                            }
                            return dt;
                        }
                        catch (SqlException e)
                        {
                            output.InnerHtml = "<p>Unable to Complete Request</p>" + 
                                "<br /><p>" + e.Message +"</p>";
                            commandName = "select * from Orders;";
                            return new DataTable();
                        }
                    }
                }
            }
        }
    }

    public void getColumnsHandler(Object sender, EventArgs e)
    {
        DropDownList d = (DropDownList)sender;
        d.Controls.Clear();
        d.Items.Clear();
        if (d.ID.Equals("selectColumn"))
        {
            d.Items.Add(new ListItem("*"));
        }
        ListItem[] i = { 
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
        d.Items.AddRange(i);
    }

    public void changeVisibility(string s)
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

    public string[] getIP()
    {
        WebClient w = new WebClient();
        string ip = w.DownloadString("http://icanhazip.com");
        ip = w.DownloadString(String.Format("http://ip-api.com/line/{0}?fields=12287", ip));
        string[] sections = ip.Split('\n');
        return sections;
    }
}
