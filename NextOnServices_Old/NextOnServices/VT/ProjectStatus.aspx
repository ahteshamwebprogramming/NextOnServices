<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProjectStatus.aspx.cs" Inherits="ProjectStatus" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../dist/css/Alert.css" rel="stylesheet" />
    <script>
        function alertt(message1, message2, message3) {
            swal(
                message1,
                message2,
                message3
            )
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">

        <%-- <asp:TextBox runat="server" ID="txtCode"></asp:TextBox>
        <asp:Label ID="lblResult" runat="server"></asp:Label>
        <asp:Button runat="server" ID="btnCode" Text="Code" OnClick="btnCode_Click" />

        <asp:Button runat="server" ID="btnDecode" Text="Decode" OnClick="btnDecode_Click" />--%>
        <div id="div" runat="server">
            <br />
            <br />
            <br />
            <table style="border: groove" width="80%" height="300px" align="center">
                <tr style="height: 50px">
                    <td></td>
                </tr>
                <tr align="center">
                    <td align="center">
                        <asp:Label ID="lblmsg" runat="server">testing</asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </form>
    <script src="../vendors/bower_components/jquery/dist/jquery.min.js"></script>
    <%--<script src="scripts/ui/jquery-1.4.2.js"></script>--%>
    <!-- Bootstrap Core JavaScript -->
    <script src="../vendors/bower_components/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- Counter Animation JavaScript -->
    <script src="../vendors/bower_components/waypoints/lib/jquery.waypoints.min.js"></script>
    <script src="../vendors/bower_components/jquery.counterup/jquery.counterup.min.js"></script>
    <script src="../Scripts/Alerts.js"></script>
    <script>
        $(document).ready(function () {
            // alert('c');

        });

    </script>
</body>
</html>
