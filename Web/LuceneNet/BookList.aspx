<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BookList.aspx.cs" Inherits="Web.LuceneNet.BookList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" method="get" action="BookList.aspx">
    <div>
        请输入搜索关键字:<input type="text" name="SearchKey" value="" />
        <input type="submit" name="btnSearch" value="测试" />
        <input type="submit" name="btnCreate" value="创建索引" />
        <br />
        <ul>
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <li>
                    <%# Eval("Id") %>
                </li>
                <li><%# Eval("MenuId") %> </li>
                <li><a href='#'>
                    <%# Eval("Description") %></a></li>
                <li><span>
                    <%# Eval("CreativeHTML")%></span></li>
            </ItemTemplate>
        </asp:Repeater>
        </ul>
    </div>
    </form>
</body>
</html>
