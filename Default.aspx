<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>C# SQL</title> <!-- What a great name-->
<link rel="stylesheet" href="StyleSheet.css" /> <!-- doesn't do a thing -->
</head>
<body>
    
    <!-- You can use C# here -->
    <form id="form1" runat="server"> 
        <!-- kind of script -->
        <input runat="server" id="search" list="searchList" autocomplete="off" onchange="checkForPound"/> <!-- using the searchFunction -->
        <datalist id="searchList" runat="server" onload="datalistHandler"></datalist>
        <asp:Button runat="server" id="searchListButton" text="ListButton" OnClick="searchFunction"></asp:Button>
        <br />

        <asp:DropDownList runat="server" ID="scriptType" AutoPostBack="true" OnTextChanged="d0Handler">
            <asp:ListItem Text="SELECT"></asp:ListItem>
            <asp:ListItem Text="INSERT INTO"></asp:ListItem>
            <asp:ListItem Text="DELETE"></asp:ListItem>
            <asp:ListItem Text="UPDATE"></asp:ListItem>
        </asp:DropDownList>
        
        <!-- column you want -->
        <asp:DropDownList runat="server" ID="selectColumn" OnLoad="getColumnsHandler"></asp:DropDownList> 
        <span runat="server" id="from" >FROM</span>
        
        <!-- choice of table -->
        <asp:DropDownList runat="server" ID="tableChoice">
            <asp:ListItem Text="clientBase"></asp:ListItem>
            <asp:ListItem Text="Orders"></asp:ListItem>
        </asp:DropDownList>
        <br />
        <span runat="server" id="set">SET</span>
        <asp:DropDownList runat="server" ID="setColumn" OnLoad="getColumnsHandler"></asp:DropDownList>
        <span runat="server" id="equal">=</span>
        <input runat="server" id="setValue" autocomplete="off" placeholder="Set to: " />
        <div runat="server" id="updateBreak" ></div>

        <span runat="server" id="where">WHERE</span>
        <!-- filter column -->
        <asp:DropDownList runat="server" ID="whereColumn" onLoad="getColumnsHandler"></asp:DropDownList>
        
        <!-- operators -->
        <asp:DropDownList runat="server" ID="operatorChoice">
            <asp:ListItem Text=">"></asp:ListItem>
            <asp:ListItem Text="<"></asp:ListItem>
            <asp:ListItem Text="="></asp:ListItem>
            <asp:ListItem Text="LIKE"></asp:ListItem>
        </asp:DropDownList>
        <input size="25" runat="server" id="searchValue" autocomplete="off" placeholder="Value of search"/>
        <input size="100" runat="server" id="insertValue" autocomplete="off" placeholder="Values of INSERT INTO" />
        <asp:Button runat="server" Text="Parse Query" OnClick="setCommandHandler" />
        <br />

        <!-- Full Query -->
        <input size="125" runat="server" type="text" id="query" placeholder="SQL Code" />
        <asp:Button runat="server" ID="queryButton" Text="Run Query" OnClick="queryCode" />
        <br />
    </form>

    <!-- Testing Divs -->
    <div runat="server" id="output"></div>
    <div runat="server" id="commandNameDiv"></div>
    <div runat="server" id="test"></div>
    <asp:PlaceHolder runat="server" id="theTable"></asp:PlaceHolder>


    <!-- Greeting -->
    <div id="greeting">
        <h2>Client Info</h2>
        <h4 runat="server">Welcome, <% Response.Write("Nadav"); %></h4>
        <p runat="server">The date is <% Response.Write(DateTime.Now.ToShortDateString()); %></p>
        <p runat="server">The time is <% Response.Write(DateTime.Now.ToShortTimeString());%></p>
        <p runat="server">Your IP Address(in comparison to the server): <% Response.Write((Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? 
                                                  Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim()); %></p>
        <p runat="server">Your Host Name: <% Response.Write(System.Net.Dns.GetHostName()); %></p>
        <div>
            <p runat="server" id="ipTitle" onclick="collapseToggle()">Your real IP Address: (Click Here to Collapse Text)</p>
            <div id="ip"> <% Response.Write(new IP(getIP()).ToString()); %> </div> 
        </div>
        <script type="text/javascript">
            function collapseToggle() {
                document.getElementById("ip").hidden = !document.getElementById("ip").hidden;
            }
        </script>
    </div>
</body>
</html>
