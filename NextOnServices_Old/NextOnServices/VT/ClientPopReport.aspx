<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ClientPopReport.aspx.cs" Inherits="ClientPopReport" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

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
                            <h5 class="txt-dark">Client Preview</h5>
                        </div>

                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Client Details</a></li>
                                <li class="active"><a href="#"><span>Overview</span></a></li>
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
                                        <h6 class="panel-title txt-dark">Client Details</h6>
                                        <br />
                                    </div>

                                </div>
                                <div class="panel-wrapper collapse in">
                                    <div class="panel-body">
                                        <div class="table-wrap status_r w_60">

                                            <%--<asp:GridView ID="GrdClientDetails" CssClass="table table-hover display  pb-30  table-bordered table-custom" runat="server" Width="100%" AutoGenerateColumns="false">
                                                <Columns>
                                                    <asp:BoundField DataField="ID" HeaderText="Id" Visible="false" HeaderStyle-Font-Bold="true" />
                                                    <asp:BoundField DataField="Company" HeaderText="Client Name" HeaderStyle-Font-Bold="true" />
                                                    <asp:BoundField DataField="CNumber" HeaderText="Contact Number" HeaderStyle-Font-Bold="true" />
                                                    <asp:BoundField DataField="CEmail" HeaderText="Email" HeaderStyle-Font-Bold="true" />
                                                    <asp:BoundField DataField="Country" HeaderText="Country" HeaderStyle-Font-Bold="true" />
                                                </Columns>
                                            </asp:GridView>--%>
                                            <style>
                                                .header {
                                                    font-weight: 600;
                                                }
                                            </style>
                                            <table class="table table-hover display pb-30  table-bordered table-custom">
                                                <tr>
                                                    <td class="header">Client Name</td>
                                                    <td>
                                                        <asp:Label runat="server" ID="lblClientName"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td class="header">Contact Number</td>
                                                    <td>
                                                        <asp:Label runat="server" ID="lblContactNumber"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td class="header">Email</td>
                                                    <td>
                                                        <asp:Label runat="server" ID="lblEmail"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td class="header">Country</td>
                                                    <td>
                                                        <asp:Label runat="server" ID="lblCountry"></asp:Label></td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField runat="server" ID="hfCLientID" />
                                        </div>
                                    </div>
                                </div>
                                <br />
                            </div>
                        </div>
                    </div>
                    <%--<div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Supplier Specs</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-body">

                                    <asp:GridView ID="GrdSupplierSpecs" CssClass="table table-hover display  pb-30" runat="server" Width="100%" AutoGenerateColumns="false">
                                        <Columns>
                                            <asp:BoundField DataField="ID" HeaderText="ID" Visible="false" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Country" HeaderText="Country" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="PSize" HeaderText="Panel Size" HeaderStyle-Font-Bold="true" />
                                        </Columns>
                                    </asp:GridView>


                                </div>

                            </div>
                        </div>
                    </div>--%>
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Client Delivery Details</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-body status_r">

                                    <asp:GridView ID="GrdOverview" CssClass="table table-hover display  pb-30  table-bordered table-custom" runat="server" Width="100%" AutoGenerateColumns="false">
                                        <Columns>
                                            <%-- <asp:BoundField DataField="id" HeaderText="id" Visible="false" HeaderStyle-Font-Bold="true" />--%>
                                            <asp:BoundField DataField="pname" HeaderText="Project Name" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Country" HeaderText="Country" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Quota" HeaderText="Quota" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="LOI" HeaderText="LOI" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="CPI" HeaderText="CPI" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="IRate" HeaderText="IRate" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Status" HeaderText="Status" HeaderStyle-Font-Bold="true" />
                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </div>
                        </div>
                    </div>
                    <%--<div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Supplier Redirects</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-body">

                                    <asp:GridView ID="GrdSurveyLinks" CssClass="table table-hover display  pb-30" runat="server" Width="100%" AutoGenerateColumns="false">
                                        <Columns>
                                            <asp:BoundField DataField="Country" HeaderText="Country" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="OLink" HeaderText="Client Survey Link" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="MLink" HeaderText="Supplier Link" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="StatusLink" HeaderText="Redirects for Supplier" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="RedirectLink" HeaderText="Redirects for client" HeaderStyle-Font-Bold="true" />

                                        </Columns>
                                    </asp:GridView>




                                </div>




                                <br />
                            </div>
                        </div>
                    </div>--%>
                </div>
                <!-- Footer -->
                <uc2:Footer ID="Footer1" runat="server" />
                <!-- /Footer -->

            </div>

        </div>




    </form>



    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <%-- <script type="text/javascript">
        //$('body').on('focus', ".datepicker", function () {
        //    $(this).datepicker();
        //});
        var pair;

        jQuery(document).ready(function () {
            //alert("hi");
            var query = window.location.search.substring(1);
            var vars = query.split("&");
            for (var i = 0; i < vars.length; i++) {
                pair = vars[i].split("=");
                //alert(pair[1]);
            }



            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'SupplierPopReport.aspx/GetDetails',
                cache: false,
                dataType: "json",
                success: function () {
                    alert(pair[1]);
                    //var tbody = "";
                    //for (var i = 0; i < data.d.length; i++) {

                    //  tbody += '<tr><td>' + data.d[i].PName + '</td><td>' + data.d[i].ClientName + '</td><td>' + data.d[i].PManager + '</td><td>' + data.d[i].LOI + '</td><td>' + data.d[i].SDate + '</td><td>' + data.d[i].EDate + '</td><td><input type="button" value="Edit" onclick=EDIT("' + data.d[i].ID + '") /></td><td><input type="button" value="Url Mapping" onclick=Mapping("' + data.d[i].ID + '","' + data.d[i].PName + '") /></td></tr>';
                    //$("#myTable1 tbody").append('<tr><td>' + data.d[i].Company + '</td><td>' + data.d[i].Person + '</td><td>' + data.d[i].Number + '</td><td>' + data.d[i].Email + '</td><td>' + data.d[i].Country + '</td></tr>')
                    // }
                    $("#myTable1 tbody").html(tbody);
                    $("#myTable1").DataTable();
                },
                error: function (result) {
                    alert(result.d);
                }
            });

            // $('#myTable1').DataTable();

        });
    </script>--%>
</body>
</html>
