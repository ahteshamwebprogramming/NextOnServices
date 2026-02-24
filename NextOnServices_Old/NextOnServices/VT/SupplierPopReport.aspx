<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SupplierPopReport.aspx.cs" Inherits="SupplierPopReport" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="preloader-it">
            <div class="la-anim-1"></div>
        </div>


        <div class="wrapper theme-1-active pimary-color-red">

            <uc1:Header ID="Header1" runat="server" />
            <div class="right-sidebar-backdrop"></div>

            <div class="page-wrapper">
                <div class="container-fluid pt-25">

                    <!-- Title -->
                    <div class="row heading-bg">
                        <div class="col-lg-3 col-md-4 col-sm-4 col-xs-12">
                            <h5 class="txt-dark">Supplier Preview</h5>
                        </div>

                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <li><a href="#"><span>Supplier</span></a></li>
                                <li><a href="SupplierDetails.aspx"><span>Supplier List</span></a></li>
                                <li><a href="#"><span>Supplier Specification</span></a></li>
                                <%--<li class="active"><span>Update Project</span></li>--%>
                            </ol>
                        </div>
                        <!-- /Breadcrumb -->

                    </div>
                    <!-- /Title -->

                    <!-- Row -->
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Supplier Details</h6>
                                        <br />
                                    </div>

                                </div>
                                <div class="panel-wrapper collapse in">
                                    <div class="panel-body">
                                        <div class="table-wrap w_60">
                                            <%-- <asp:GridView ID="GrdSupplierDetails" CssClass="table table-hover display  pb-30  table-bordered table-custom" runat="server" Width="100%" AutoGenerateColumns="false">
                                                <Columns>
                                                    <asp:BoundField DataField="ID" HeaderText="Id" Visible="false" HeaderStyle-Font-Bold="true" />
                                                    <asp:BoundField DataField="Name" HeaderText="Name" HeaderStyle-Font-Bold="true" />
                                                    <asp:BoundField DataField="Description" HeaderText="Description" HeaderStyle-Font-Bold="true" />
                                                    <asp:BoundField DataField="Number" HeaderText="Number" HeaderStyle-Font-Bold="true" />
                                                    <asp:BoundField DataField="Email" HeaderText="Email" HeaderStyle-Font-Bold="true" />
                                                </Columns>
                                            </asp:GridView>--%>
                                            <style>
                                                .header {
                                                    font-weight: 600;
                                                }
                                            </style>
                                            <table class="table table-hover display pb-30  table-bordered table-custom">
                                                <tr>
                                                    <td class="header">Name</td>
                                                    <td>
                                                        <asp:Label runat="server" ID="lblName"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td class="header">Number</td>
                                                    <td>
                                                        <asp:Label runat="server" ID="lblNumber"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td class="header">Email</td>
                                                    <td>
                                                        <asp:Label runat="server" ID="lblEmail"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td class="header">Discription</td>
                                                    <td>
                                                        <asp:Label runat="server" ID="lblDiscription"></asp:Label></td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField runat="server" ID="hfSuppID" />
                                        </div>
                                    </div>
                                </div>
                                <br />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Supplier Specs</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-body w_60">

                                    <asp:GridView ID="GrdSupplierSpecs" CssClass="table table-hover display pb-30  table-bordered table-custom" runat="server" Width="100%" AutoGenerateColumns="false">
                                        <Columns>
                                            <asp:BoundField DataField="ID" HeaderText="ID" Visible="false" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Country" HeaderText="Country" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="PSize" HeaderText="Panel Size" HeaderStyle-Font-Bold="true" />
                                        </Columns>
                                    </asp:GridView>


                                </div>

                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Supplier Delivery Details</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-body">

                                    <asp:GridView ID="GrdOverview" CssClass="table table-hover display  pb-30  table-bordered table-custom" runat="server" Width="100%" AutoGenerateColumns="false">
                                        <Columns>
                                            <asp:BoundField DataField="id" HeaderText="id" Visible="false" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Country" HeaderText="Country" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Total" HeaderText="Total" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Complete" HeaderText="Completes" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Incompletes" HeaderText="Incompletes" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Screened" HeaderText="Screened" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Quotafull" HeaderText="Quotafull" HeaderStyle-Font-Bold="true" />
                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Status & Redirects</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-body">


                                    <div class="">
                                        <table class="table table-hover display pb-30  table-bordered table-custom" cellspacing="0" rules="all" border="1" id="GrdSurveyLinks" style="width: 100%; border-collapse: collapse;">
                                            <tbody>
                                                <tr>
                                                    <th style="font-weight: bold;">Status</th>
                                                    <th style="font-weight: bold;">Redirects</th>
                                                </tr>
                                                <tr>
                                                    <td style="font-weight: bold;">Completes</td>
                                                    <td>http://nextonservices.com/smt/views/redirection_operations.php?survey_id=12999&amp;identifier=[respondentID]&amp;redirected_from=survey&amp;status=complete</td>
                                                </tr>
                                                <tr>
                                                    <td style="font-weight: bold;">Terminate</td>
                                                    <td>http://nextonservices.com/smt/views/redirection_operations.php?survey_id=12999&amp;identifier=[respondentID]&amp;redirected_from=survey&amp;status=Terminate</td>
                                                </tr>
                                                <tr>
                                                    <td style="font-weight: bold;">Quota Full</td>
                                                    <td>http://nextonservices.com/smt/views/redirection_operations.php?survey_id=12999&amp;identifier=[respondentID]&amp;redirected_from=survey&amp;status=Quotafull</td>
                                                </tr>
                                                <tr>
                                                    <td style="font-weight: bold;">Screened</td>
                                                    <td>http://nextonservices.com/smt/views/redirection_operations.php?survey_id=12999&amp;identifier=[respondentID]&amp;redirected_from=survey&amp;status=Screened</td>
                                                </tr>
                                                <tr>
                                                    <td style="font-weight: bold;">Over Quota</td>
                                                    <td>http://nextonservices.com/smt/views/redirection_operations.php?survey_id=12999&amp;identifier=[respondentID]&amp;redirected_from=survey&amp;status=Quotafull</td>
                                                </tr>


                                            </tbody>
                                        </table>

                                    </div>

                                    <div style="overflow-x: scroll">
                                        <!-- <asp:GridView ID="GrdSurveyLinks" CssClass="table table-hover display  pb-30" runat="server" Width="100%" AutoGenerateColumns="false">
                                            <Columns>
                                                <asp:BoundField DataField="completes" HeaderText="Completes" HeaderStyle-Font-Bold="true" />
                                                <asp:BoundField DataField="Terminate" HeaderText="Terminate" HeaderStyle-Font-Bold="true" />
                                                <asp:BoundField DataField="quotafull" HeaderText="Quota Full" HeaderStyle-Font-Bold="true" />
                                                <asp:BoundField DataField="screened" HeaderText="Screened" HeaderStyle-Font-Bold="true" />
                                                <asp:BoundField DataField="overquota" HeaderText="Over Quota" HeaderStyle-Font-Bold="true" />

                                            </Columns>
                                        </asp:GridView>
                                        -->
                                    </div>



                                </div>




                                <br />
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
    <script src="../vendors/bower_components/jquery/dist/jquery.min.js"></script>

    <!-- Bootstrap Core JavaScript -->
    <script src="../vendors/bower_components/bootstrap/dist/js/bootstrap.min.js"></script>

    <!-- Counter Animation JavaScript -->
    <script src="../vendors/bower_components/waypoints/lib/jquery.waypoints.min.js"></script>
    <script src="../vendors/bower_components/jquery.counterup/jquery.counterup.min.js"></script>

    <!-- Data table JavaScript -->
    <%--<script src="vendors/bower_components/datatables/media/js/jquery.dataTables.min.js"></script>
	<script src="vendors/bower_components/datatables.net-buttons/js/dataTables.buttons.min.js"></script>
	<script src="vendors/bower_components/datatables.net-responsive/js/dataTables.responsive.min.js"></script>
	<script src="dist/js/responsive-datatable-data.js"></script>--%>

    <!-- Owl JavaScript -->
    <script src="../vendors/bower_components/owl.carousel/dist/owl.carousel.min.js"></script>

    <!-- Switchery JavaScript -->
    <script src="../vendors/bower_components/switchery/dist/switchery.min.js"></script>

    <!-- Slimscroll JavaScript -->
    <script src="../dist/js/jquery.slimscroll.js"></script>

    <!-- Fancy Dropdown JS -->
    <script src="../dist/js/dropdown-bootstrap-extended.js"></script>


    <!-- Init JavaScript -->
    <script src="../dist/js/init.js"></script>
    <script src="../dist/js/dashboard3-data.js"></script>
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        //$('body').on('focus', ".datepicker", function () {
        //    $(this).datepicker();
        //});
        var pair;

        jQuery(document).ready(function () {
            //alert("hi");

        });
    </script>
</body>
</html>
