<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestPage.aspx.cs" Inherits="TestPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <input type="button" value="runcode" onclick="run();" />
            <select class="ui search selection dropdown" id="search-select">
                <option value="">State</option>
                <option value="AL">Alabama</option>
                <option value="AK">Alaska</option>
                <option value="AZ">Arizona</option>
                <option value="AR">Arkansas</option>
                <option value="CA">California</option>
                <!-- Saving your scroll sanity !-->
                <option value="OH">Ohio</option>
                <option value="OK">Oklahoma</option>
                <option value="OR">Oregon</option>
                <option value="PA">Pennsylvania</option>
                <option value="RI">Rhode Island</option>
                <option value="SC">South Carolina</option>
                <option value="SD">South Dakota</option>
                <option value="TN">Tennessee</option>
                <option value="TX">Texas</option>
                <option value="UT">Utah</option>
                <option value="VT">Vermont</option>
                <option value="VA">Virginia</option>
                <option value="WA">Washington</option>
                <option value="WV">West Virginia</option>
                <option value="WI">Wisconsin</option>
                <option value="WY">Wyoming</option>
            </select>
        </div>
    </form>
</body>
<script src="../vendors/bower_components/jquery/dist/jquery.min.js"></script>
<script src="../vendors/bower_components/bootstrap/dist/js/bootstrap.min.js"></script>
<script src="../vendors/bower_components/waypoints/lib/jquery.waypoints.min.js"></script>
<script src="../vendors/bower_components/jquery.counterup/jquery.counterup.min.js"></script>
<script src="../vendors/bower_components/owl.carousel/dist/owl.carousel.min.js"></script>
<script src="../vendors/bower_components/switchery/dist/switchery.min.js"></script>
<script src="../dist/js/jquery.slimscroll.js"></script>
<script src="../dist/js/dropdown-bootstrap-extended.js"></script>
<script src="../dist/js/init.js"></script>
<%--<script src="dist/js/dashboard3-data.js"></script>--%>
<script src="../Scripts/Dropdown/semantic.js"></script>
<script type="text/javascript">
    function run() {
        $('#search-select')
            .dropdown()
            ;
    }
    jQuery(document).ready(function () {
        $('#search-select')
            .dropdown()
            ;
    });
</script>
</html>
