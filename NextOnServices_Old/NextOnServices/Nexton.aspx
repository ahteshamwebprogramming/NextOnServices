<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Nexton.aspx.vb" Inherits="Nexton" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <title>index</title>
    <meta name="description" content="" />
    <meta name="keywords" content="" />
    <meta name="author" content="hencework" />

    <!-- Favicon -->
    <link rel="shortcut icon" href="favicon.ico" />
    <link rel="icon" href="../favicon.png" type="image/x-icon" />
    <link href="vendors/bower_components/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Data table CSS -->
    <link href="vendors/bower_components/datatables/media/css/jquery.dataTables.min.css" rel="stylesheet" type="text/css" />
    <link href="vendors/bower_components/datatables.net-responsive/css/responsive.dataTables.min.css" rel="stylesheet" type="text/css" />
    <!-- select2 CSS -->
    <link href="vendors/bower_components/select2/dist/css/select2.min.css" rel="stylesheet" type="text/css" />
    <!-- bootstrap-select CSS -->
    <link href="vendors/bower_components/bootstrap-select/dist/css/bootstrap-select.min.css" rel="stylesheet" type="text/css" />
    <!-- bootstrap-tagsinput CSS -->
    <link href="vendors/bower_components/bootstrap-tagsinput/dist/bootstrap-tagsinput.css" rel="stylesheet" type="text/css" />
    <!-- bootstrap-touchspin CSS -->
    <link href="vendors/bower_components/bootstrap-touchspin/dist/jquery.bootstrap-touchspin.min.css" rel="stylesheet" type="text/css" />
    <!-- Bootstrap Datetimepicker CSS -->
    <link href="vendors/bower_components/eonasdan-bootstrap-datetimepicker/build/css/bootstrap-datetimepicker.min.css" rel="stylesheet" type="text/css" />
    <!-- multi-select CSS -->
    <link href="vendors/bower_components/multiselect/css/multi-select.css" rel="stylesheet" type="text/css" />
    <!-- Bootstrap Switches CSS -->
    <link href="vendors/bower_components/bootstrap-switch/dist/css/bootstrap3/bootstrap-switch.min.css" rel="stylesheet" type="text/css" />
    <!-- Bootstrap Datetimepicker CSS -->
    <link href="vendors/bower_components/eonasdan-bootstrap-datetimepicker/build/css/bootstrap-datetimepicker.min.css" rel="stylesheet" type="text/css" />
    <link href="Scripts/UI/themes/base/jquery-ui.css" rel="stylesheet" />
    <link href="Scripts/UI/themes/base/jquery.ui.theme.css" rel="stylesheet" />
    <link href="dist/css/Alert.css" rel="stylesheet" />
    <%--  <link rel="stylesheet" href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.5/themes/base/jquery-ui.css" type="text/css" media="all" />
    <link rel="stylesheet" href="http://static.jquery.com/ui/css/demo-docs-theme/ui.theme.css" type="text/css" media="all" />--%>

    <!-- Custom CSS -->
    <link href="dist/css/style.css" rel="stylesheet" type="text/css" />
    <%--<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js" type="text/javascript"></script>--%>
    <style>
        .bg_n6 {
            background: url(dist/img/img10.jpg) no-repeat;
            background-position: center;
            background-size: cover;
            min-height: 100vh;
        }

        .overlay, .overlay-yellow, .overlay-skyblue, .overlay-green, .overlay-black {
            position: relative;
        }

            .overlay:before {
                content: "";
                /*background: #33658a;*/
                background: #76858f;
                position: absolute;
                top: 0;
                left: 0;
                width: 100%;
                height: 100vh;
                opacity: .5;
            }

        .company_logo {
            /*color: #234140;*/
            color: white;
            font-family: 'Buxton Sketch';
            font-size: 7vw;
            vertical-align: middle;
            align-items: center;
            align-content: center;
            align-self: center;
            margin-top: 30vh;
        }

        #contact span {
            color: white;
            font-size: 15px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="bg_n6 overlay">
            <div class="container" style="padding-top: 1%">
                <div class="row">
                    <div class="col-md-6 col-md-offset-6">
                        <div class="pull-right" id="contact">
                            <span>Email : info@nextonservices.com</span><br />
                            <span>Contact | USA:&nbsp&nbsp&nbsp(+1) 818-453-1793</span>
                            <br />
                            <span style="margin-left: 3.55em">INDIA:&nbsp&nbsp&nbsp(+91) 997-164-3131    </span><br />
                            <span style="margin-left: 4.2em">UAE:&nbsp&nbsp&nbsp(+971) 50-941-9689    </span>
                        </div>
                    </div>
                </div>
                <div class="" style="width: max-content; margin: auto; color: white; align-items: center; vertical-align: middle; height: 100%; visibility: visible; position: relative">
                    <%-- <img src="dist/img/Originals/NextonLogo.png" />--%>
                    <h1 class="company_logo">NEXT<span style="color: #53cc70">ON</span> SERVICES</h1>
                </div>
            </div>
        </div>
    </form>

</body>
<script src="vendors/bower_components/jquery/dist/jquery.min.js"></script>
<%--<script src="scripts/ui/jquery-1.4.2.js"></script>--%>
<!-- Bootstrap Core JavaScript -->
<script src="vendors/bower_components/bootstrap/dist/js/bootstrap.min.js"></script>
<!-- Counter Animation JavaScript -->
<script src="vendors/bower_components/waypoints/lib/jquery.waypoints.min.js"></script>
<script src="vendors/bower_components/jquery.counterup/jquery.counterup.min.js"></script>
<!-- Data table JavaScript -->
<%--<script src="vendors/bower_components/datatables/media/js/jquery.dataTables.min.js"></script>
	<script src="vendors/bower_components/datatables.net-buttons/js/dataTables.buttons.min.js"></script>
	<script src="vendors/bower_components/datatables.net-responsive/js/dataTables.responsive.min.js"></script>
	<script src="dist/js/responsive-datatable-data.js"></script>--%>

<!-- Owl JavaScript -->
<script src="vendors/bower_components/owl.carousel/dist/owl.carousel.min.js"></script>

<!-- Switchery JavaScript -->
<script src="vendors/bower_components/switchery/dist/switchery.min.js"></script>

<!-- Slimscroll JavaScript -->
<script src="dist/js/jquery.slimscroll.js"></script>

<!-- Fancy Dropdown JS -->
<script src="dist/js/dropdown-bootstrap-extended.js"></script>
</html>
