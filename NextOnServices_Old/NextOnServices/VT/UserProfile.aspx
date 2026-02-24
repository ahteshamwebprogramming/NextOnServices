<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserProfile.aspx.cs" Inherits="UserProfile" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <title>User Profile</title>
    <meta name="description" content="" />
    <meta name="keywords" content="" />
    <meta name="author" content="hencework" />

    <!-- Favicon -->
    <link rel="shortcut icon" href="favicon.ico" />
    <link rel="icon" href="favicon.png" type="image/x-icon" />

    <!-- vector map CSS -->
    <link href="../vendors/bower_components/jasny-bootstrap/dist/css/jasny-bootstrap.min.css" rel="stylesheet" type="text/css" />

    <!-- Custom CSS -->
    <link href="../dist/css/style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <!--Preloader-->




    <form id="form1" runat="server">
        <div class="preloader-it">
            <div class="la-anim-1"></div>
        </div>


        <div class="wrapper theme-1-active pimary-color-red">

            <uc1:Header ID="Header1" runat="server" />
            <div class="right-sidebar-backdrop"></div>

            <div class="page-wrapper" style="padding-top: 0px">
                <div class="container-fluid pt-25">

                    <!-- Title -->

                    <!-- /Breadcrumb -->
                    <div class="row">
                        <div class="table-struct full-width full-height card-view">
                            <div class="table-cell vertical-align-middle auth-form-wrap">
                                <div class="auth-form  ml-auto mr-auto no-float bdr_login">
                                    <div class="row">
                                        <div class="col-sm-12 col-xs-12">
                                            <div class="mb-30 text-center">
                                                <a href="index-2.html">
                                                    <img class="brand-img mr-10" src="dist/img/logo.png" alt="brand" />
                                                </a>
                                                <h6 class="text-center nonecase-font txt-grey">User Profile</h6>
                                            </div>
                                            <div class="profile_img">
                                                <img class="img-circle" src="dist/img/user1.png" />
                                            </div>
                                            <div class="table-responsive">
                                                <table class="table mb-0">
                                                    <thead>
                                                        <tr>
                                                            <th>
                                                                <asp:Label runat="server" ID="lblName" Text=""></asp:Label>
                                                            </th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td>User ID</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblUserId" Text="Wilsan"></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td>Mobile</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblMobile" Text="980 xxxx 001"></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td>Email</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblEmail" Text="jondo@extonservices.com"></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td>Address</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblAddress" Text="Venerable corner flower shop"></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td>Country</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblCountry" Text="America"></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td>Rating</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblRatings" Text="5 Star"></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td>Projects</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblProjects" Text="abc"></asp:Label></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>

                </div>
                <!-- Footer -->
                <uc2:Footer ID="Footer1" runat="server" />
                <!-- /Footer -->

            </div>

        </div>




    </form>


    <%-- ---------------------------------------------%>





    <!-- /#wrapper -->

    <!-- JavaScript -->

    <!-- jQuery -->
    <%--<script src="vendors/bower_components/jquery/dist/jquery.min.js"></script>

    <!-- Bootstrap Core JavaScript -->
    <script src="vendors/bower_components/bootstrap/dist/js/bootstrap.min.js"></script>
    <script src="vendors/bower_components/jasny-bootstrap/dist/js/jasny-bootstrap.min.js"></script>

    <!-- Slimscroll JavaScript -->
    <script src="dist/js/jquery.slimscroll.js"></script>

    <!-- Init JavaScript -->
    <script src="dist/js/init.js"></script>
    --%>
</body>
</html>
