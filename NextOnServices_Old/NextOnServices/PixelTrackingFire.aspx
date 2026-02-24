<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PixelTrackingFire.aspx.cs" Inherits="PixelTrackingFire" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <iframe src="http://localhost:6753/VT/ProjectStatus.aspx?ID=0XZC2GA1VR85GCZ3ZFNJ3WFBF51205VE&Status=COMPLETE&RC=0&PX=1"></iframe>
        </div>
    </form>
    <script src="vendors/bower_components/jquery/dist/jquery.min.js"></script>
    <script>
        $(document).ready(function () {

            //$.ajax({
            //    type: "POST",
            //    contentType: "application/json; charset=utf-8",
            //    url: 'PixelTrackingFire.aspx/Check',
            //    data: JSON.stringify({}),
            //    cache: false,
            //    dataType: "json",
            //    success: function (data) {

            //    },
            //    error: function (result) {
            //        (result.statusText);
            //    }
            //});

        });

    </script>
</body>
</html>
