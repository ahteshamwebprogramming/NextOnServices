<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PixelTrackingtest.aspx.cs" Inherits="PixelTrackingtest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <iframe src="http://nextonservices.go2cloud.org/aff_l?offer_id=5&adv_sub=SUB_ID" scrolling="no" frameborder="0" width="1" height="1"></iframe>
    </form>
    <script src="vendors/bower_components/jquery/dist/jquery.min.js"></script>
    <script>

        $(document).ready(function () {
            // alert('h');
            var querystringid;
            var id;
            var query = window.location.search.substring(1);
            var vars = query.split("&");
            for (var i = 0; i < vars.length; i++) {
                pair = vars[i].split("=");
                //alert(pair[1]);
                id = pair[1];
                querystringid = id;
            }
            // alert('h');
            // top.window.location.href = 'https://www.google.com';

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'PixelTrackingtest.aspx/Check',
                data: JSON.stringify({}),
                cache: false,
                dataType: "json",
                success: function (data) {

                },
                error: function (result) {
                    (result.statusText);
                }
            });


        });

    </script>
</body>


</html>
