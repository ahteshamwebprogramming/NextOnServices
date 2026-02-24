<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MaskingUrl.aspx.cs" Inherits="MaskingUrl" %>

<!doctype html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
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
</body>
</html>
<script src="../vendors/bower_components/jquery/dist/jquery.min.js"></script>
<script src="../Scripts/DeviceDetection.js"></script>
<script>

    $(document).ready(function () {
        var hardware = Device();
        $('#hfdevice').val(hardware);

    });
    function IPFinder(ip) {
        return $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: 'https://ipfind.co?ip=8.8.8.8',
            data: JSON.stringify({ ProjectURLID: projecturlid, CountryID: countryid, OPT: 2 }),
            async: false,
            cache: false,
            dataType: "json",
            success: function (data) {
                // return data.d[0].country_code;
                $('#hfCountrycode').val(data.d[0].country_code);
            },
            error: function (result) {
                return "error";
            }
        });

    }

</script>
