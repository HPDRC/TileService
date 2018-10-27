<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="vector_stat.aspx.cs" Inherits="TileService.vector_stat" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Statistics</title>
    <style type="text/css">
        th {
            width: 80px;
            text-align:left;
        }
    </style>
</head>
<body>
    <form id="Form1" runat="server">
        <asp:LinkButton ID="btnResetStatus" runat="server" OnClick="btnResetStatus_Click">Reset Statistics</asp:LinkButton>
    </form>
    <%=description%>
</body>
</html>
