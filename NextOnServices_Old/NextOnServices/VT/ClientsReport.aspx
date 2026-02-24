<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ClientsReport.aspx.cs" Inherits="ClientsReport" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
    <link href="../dist/css/cust.css" rel="stylesheet" />
    <title></title>
    <style>
        #tblprojectstotal tbody tr td, #tblprojectstotal thead tr th, #tblRespondants tbody tr td, #tblRespondants thead tr th {
            text-align: center;
        }

        /*#tblprojectstotal thead tr th, #tblRespondants thead tr th {
            padding: 11px !important;
        }

        #tblprojectstotal tbody tr td, #tblRespondants thead tr th {
            padding: 5px 15px !important;
        }*/
    </style>
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
                <div class="container-fluid">

                    <!-- Title -->
                    <div class="row heading-bg">
                        <div class="col-lg-3 col-md-4 col-sm-4 col-xs-12">
                            <h5 class="txt-dark">Clients Report</h5>
                        </div>

                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <%--<li><a href="#"><span>Report</span></a></li>--%>
                                <li class="active"><span>Client Report</span></li>
                            </ol>
                        </div>
                        <!-- /Breadcrumb -->

                    </div>
                    <!-- /Title -->

                    <!-- Row -->
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="panel panel-default card-view">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Filter</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                    <div class="row">
                                        <div class="col-sm-2 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Start Date</label>
                                                <input type="text" id="txtSDate" class="form-control" autocomplete="off" />
                                            </div>
                                        </div>
                                        <div class="col-sm-2 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">End Date</label>
                                                <input type="text" id="txtEDate" class="form-control" autocomplete="off" />
                                            </div>
                                        </div>
                                        <div class="col-sm-2 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Client</label>
                                                <select id="ddlClients" class="ui search selection dropdown form-control" name="ddlstatus">
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-sm-2 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Country</label>
                                                <select id="ddlCountry" class="ui search selection dropdown form-control" name="ddlCountry"></select>
                                                <%-- <asp:DropDownList ID="ddlCountry" runat="server"  class="form-control" data-style="form-control btn-default btn-outline">

                                                    </asp:DropDownList>--%>
                                            </div>
                                        </div>
                                        <%--<div class="clearfix"></div>--%>
                                        <div class="col-sm-offset-0 col-sm-2 col-xs-12">
                                            <div class="form-group">
                                                <%-- <asp:Button ID="btnSubmit" runat="server" class="btn btn-success btn-anim" OnClientClick="return validation();"
                                                    Text="Submit"  />--%>
                                                <button type="button" class="btn btn-success btn-anim" id="btnSubmit" name="btnSubmit" style="margin-top: 31px">Apply</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <style>
                        .w_30 {
                            border: 1px solid black;
                            /*float: left;*/
                        }

                        .shadow {
                            -webkit-box-shadow: 1px 1px 38px -12px rgba(0,0,0,0.75);
                            -moz-box-shadow: 1px 1px 38px -12px rgba(0,0,0,0.75);
                            box-shadow: 1px 1px 38px -12px rgba(0,0,0,0.75);
                        }

                        .completes {
                            font-weight: 900;
                            font-size: 90px;
                            color: darkblue;
                        }

                        .align_left {
                            float: left;
                            width: 30%;
                            /*margin: 0 auto;*/
                        }

                        .clear {
                            clear: both;
                        }

                        .img {
                            height: 200px;
                            width: 350px;
                        }
                    </style>
                    <label id="space" style="display: none"></label>
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="panel panel-default card-view">
                                <div class="panel-wrapper collapse in clear">
                                    <div class="panel-body">
                                        <div class="row">
                                            <div class="col-sm-12">
                                                <input type="button" id="btnExport" onclick="tablesToExcel(['hprojects', 'tblprojectstotal', 'space', 'space', 'hRespondents', 'tblRespondants'], ['first'], 'myfile.xls')" class="pull-right btn btn-sm" value="Export to Excel" />
                                                <script src="Scripts/cust.js"></script>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-sm-12">
                                                <h4 id="hprojects">Projects</h4>
                                                <div class="table-wrap">
                                                    <div class="">
                                                        <table id="tblprojectstotal" class="table table-hover display  pb-30">
                                                            <thead>
                                                                <tr>
                                                                    <th>Total</th>
                                                                    <th>Closed</th>
                                                                    <th>In Progress</th>
                                                                    <th>On Hold</th>
                                                                    <th>Cancelled</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody></tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="panel panel-default card-view">
                                <div class="panel-wrapper collapse in">
                                    <div class="panel-body">

                                        <div class="col-sm-12">
                                            <h4 id="hRespondents">Respondents</h4>
                                            <div class="table-wrap">
                                                <div class="">
                                                    <table id="tblRespondants" class="table table-hover display  pb-30">
                                                        <thead>
                                                            <tr>
                                                                <th>Total</th>
                                                                <th>Complete</th>
                                                                <th>Incomplete</th>
                                                                <th>Screened</th>
                                                                <th>Quotafull</th>
                                                                <th>Terminate</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
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

                </div>
                <!-- Footer -->
                <uc2:Footer ID="Footer1" runat="server" />
                <!-- /Footer -->

            </div>

        </div>

    </form>
    <script src="../Scripts/Dropdown/semantic.js"></script>
    <script src="../Scripts/UI/jquery-1.4.2.js"></script>
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../Scripts/UI/ui/minified/jquery-ui.min.js"></script>
    <script src="../Scripts/jquery.table2excel.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.blockUI/2.70/jquery.blockUI.min.js"></script>
    <script src="../CustomJS/Common.js"></script>
    <script src="../CustomJS/OverAllReport.js"></script>


    <%-- <link href="../Scripts/UI/themes/base/jquery-ui.css" rel="stylesheet" />
    <script src="../Scripts/UI/ui/minified/jquery-ui.min.js"></script>
    <script src="../CustomJS/Common.js"></script>
    <script src="../CustomJS/OverAllReport.js"></script>--%>
    <script type="text/javascript">




        //$('body').on('focus', ".datepicker", function () {
        //    $(this).datepicker();
        //});


        jQuery(document).ready(function () {
            jQuery.noConflict();
            jQuery("#txtSDate").datepicker({ dateFormat: 'mm-dd-yy' });
            jQuery("#txtEDate").datepicker({ dateFormat: 'mm-dd-yy' });

            PageInitClient();

            //$.ajax({
            //    type: "POST",
            //    contentType: "application/json; charset=utf-8",
            //    url: 'OverallReport.aspx/GetReportForCompletion',
            //    cache: false,
            //    dataType: "json",
            //    success: function (data) {
            //        var tbodytotal = "";
            //        for (var i = 0; i < data.d[3].length; i++) {
            //            tbodytotal += '<tr><td>' + data.d[3][i].Total + '</td><td>' + data.d[3][i].Closed + '</td><td>' + data.d[3][i].Inprogress + '</td><td>' + data.d[3][i].Onhold + '</td><td>' + data.d[3][i].Cancelled + '</td></tr>';
            //        }
            //        $("#tblprojectstotal tbody").html(tbodytotal);
            //        //tblSupplierWise
            //        var tbodysupplier = "";
            //        for (var i = 0; i < data.d[0].length; i++) {
            //            tbodysupplier += '<tr><td>' + data.d[0][i].SupplierName + '</td><td>' + data.d[0][i].complete + '</td><td>' + data.d[0][i].percentage + '</td></tr>';
            //        }
            //        $("#tblSupplierWise tbody").html(tbodysupplier);
            //        //tblClientWise
            //        var tbodyClient = "";
            //        for (var i = 0; i < data.d[1].length; i++) {
            //            tbodyClient += '<tr><td>' + data.d[1][i].ClientName + '</td><td>' + data.d[1][i].complete + '</td><td>' + data.d[1][i].percentage + '</td></tr>';
            //        }
            //        $("#tblClientWise tbody").html(tbodyClient);
            //        //tblCountry
            //        var tbodyCountry = "";
            //        for (var i = 0; i < data.d[2].length; i++) {
            //            tbodyCountry += '<tr><td>' + data.d[2][i].Country + '</td><td>' + data.d[2][i].Total + '</td><td>' + data.d[2][i].Percentage + '</td></tr>';
            //        }
            //        $("#tblCountry tbody").html(tbodyCountry);
            //        var tbodyIrate = "";
            //        for (var i = 0; i < data.d[4].length; i++) {
            //            tbodyIrate += '<tr><td>' + data.d[4][i].MaxIRate + '</td><td>' + data.d[4][i].MinIRate + '</td><td>' + data.d[4][i].AvgIRate + '</td></tr>';
            //        }
            //        $("#tblIrate tbody").html(tbodyIrate);
            //        var tbodyLOI = "";
            //        for (var i = 0; i < data.d[5].length; i++) {
            //            tbodyLOI += '<tr><td>' + data.d[5][i].MaxLOI + '</td><td>' + data.d[5][i].MinLOI + '</td><td>' + data.d[5][i].AvgLOI + '</td></tr>';
            //        }
            //        $("#tblActualLOI tbody").html(tbodyLOI);
            //        var tbodyCPI = "";
            //        for (var i = 0; i < data.d[6].length; i++) {
            //            tbodyCPI += '<tr><td>' + data.d[6][i].MaxCPI + '</td><td>' + data.d[6][i].MinCPI + '</td><td>' + data.d[6][i].AvgCPI + '</td></tr>';
            //        }
            //        $("#tblCPI tbody").html(tbodyCPI);
            //        var tbodySuccessRate = "";
            //        for (var i = 0; i < data.d[7].length; i++) {
            //            tbodySuccessRate += '<tr><td>' + data.d[7][i].ProjectSuccessRate + '</td><td>' + data.d[7][i].IRSuccessRate + '</td></tr>';
            //        }
            //        $("#tblSuccesrate tbody").html(tbodySuccessRate);
            //        var tbodyRespondants = "";
            //        $('#lblCompletes').empty();
            //        for (var i = 0; i < data.d[8].length; i++) {

            //            tbodyRespondants += '<tr><td>' + data.d[8][i].Total + '</td><td>' + data.d[8][i].Complete + '</td><td>' + data.d[8][i].Incomplete + '</td><td>' + data.d[8][i].Screened + '</td><td>' + data.d[8][i].Quotafull + '</td><td>' + data.d[8][i].Terminate + '</td></tr>';
            //            $('#lblCompletes').text(data.d[8][i].Complete);
            //        }
            //        $("#tblRespondants tbody").html(tbodyRespondants);
            //        //$("#tblCountry").DataTable();
            //        //$("#tblClientWise").DataTable();
            //        //$("#tblSupplierWise").DataTable();
            //        //$("#tblprojectstotal").DataTable();
            //    },
            //    error: function (result) {
            //        alert(result.d);

            //    }
            //});

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'ClientReport.aspx/GetCountries',
                cache: false,
                dataType: "json",
                success: function (data) {
                    // alert(data.d.length);
                    // $("#OperatorMobile").append('<option value="SE">--Select Operator--</option>');
                    if (data.d.length > 0) {
                        $("#ddlCountry").append('<option value="0">--Search Country--</option>');
                        for (var i = 0; i < data.d.length; i++) {
                            $("#ddlCountry").append('<option value="' + data.d[i].ID + '">' + data.d[i].Country + '</option>');
                        }
                        $('#ddlCountry').dropdown();
                    }
                    else {
                    }
                },
                error: function (result) {
                    alert(result.d);
                }
            });

            // json for getting client
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'OverallReport.aspx/GetClients',
                cache: false,
                dataType: "json",
                success: function (data) {
                    // alert(data.d.length);
                    // $("#OperatorMobile").append('<option value="SE">--Select Operator--</option>');
                    if (data.d.length > 0) {
                        $("#ddlClients").append('<option value="0">--Search Clients--</option>');
                        for (var i = 0; i < data.d.length; i++) {
                            $("#ddlClients").append('<option value="' + data.d[i].Id + '">' + data.d[i].ClientName + '</option>');
                        }
                        $('#ddlClients').dropdown();
                    }
                    else {
                    }
                },
                error: function (result) {
                    alert(result.d);
                }
            });



            // var countryname = $('#ddlCountry').val();
            $('#btnSubmit').click(function () {
                jQuery.blockUI({ message: '<h1 style="font-size: 25px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' });
                //var countryname = $('#ddlCountry option:selected').text();
                var countryid = $('#ddlCountry').val();
                var clientid = $('#ddlClients').val();
                // var supplierid = $('#ddlSuppliers').val();

                //json for getting datain grid with the help of filters
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'OverallReport.aspx/GetDataForFilters',
                    data: JSON.stringify({ CountryId: countryid, Clientid: clientid, Supplierid: 0, sdate: $('#txtSDate').val(), edate: $('#txtEDate').val() }),
                    cache: false,
                    dataType: "json",
                    success: function (data) {
                        jQuery.unblockUI();
                        var tbodytotal = "";
                        for (var i = 0; i < data.d[3].length; i++) {
                            tbodytotal += '<tr><td>' + data.d[3][i].Total + '</td><td>' + data.d[3][i].Closed + '</td><td>' + data.d[3][i].Inprogress + '</td><td>' + data.d[3][i].Onhold + '</td><td>' + data.d[3][i].Cancelled + '</td></tr>';
                        }
                        $("#tblprojectstotal tbody").html(tbodytotal);
                        //tblSupplierWise
                        var tbodysupplier = "";
                        for (var i = 0; i < data.d[0].length; i++) {
                            tbodysupplier += '<tr><td>' + data.d[0][i].SupplierName + '</td><td>' + data.d[0][i].complete + '</td><td>' + data.d[0][i].percentage + '</td></tr>';

                        }
                        if (tbodysupplier == '') {
                            tbodysupplier = 'NO Records Found';
                        }
                        $("#tblSupplierWise tbody").html(tbodysupplier);
                        //tblClientWise
                        var tbodyClient = "";
                        for (var i = 0; i < data.d[1].length; i++) {
                            tbodyClient += '<tr><td>' + data.d[1][i].ClientName + '</td><td>' + data.d[1][i].complete + '</td><td>' + data.d[1][i].percentage + '</td></tr>';
                        }
                        $("#tblClientWise tbody").html(tbodyClient);
                        //tblCountry
                        var tbodyCountry = "";
                        for (var i = 0; i < data.d[2].length; i++) {
                            tbodyCountry += '<tr><td>' + data.d[2][i].Country + '</td><td>' + data.d[2][i].Total + '</td><td>' + data.d[2][i].Percentage + '</td></tr>';
                        }
                        $("#tblCountry tbody").html(tbodyCountry);
                        var tbodyIrate = "";
                        for (var i = 0; i < data.d[4].length; i++) {
                            tbodyIrate += '<tr><td>' + data.d[4][i].MaxIRate + '</td><td>' + data.d[4][i].MinIRate + '</td><td>' + data.d[4][i].AvgIRate + '</td></tr>';
                        }
                        $("#tblIrate tbody").html(tbodyIrate);
                        var tbodyLOI = "";
                        for (var i = 0; i < data.d[5].length; i++) {
                            tbodyLOI += '<tr><td>' + data.d[5][i].MaxLOI + '</td><td>' + data.d[5][i].MinLOI + '</td><td>' + data.d[5][i].AvgLOI + '</td></tr>';
                        }
                        $("#tblActualLOI tbody").html(tbodyLOI);
                        var tbodyCPI = "";
                        for (var i = 0; i < data.d[6].length; i++) {
                            tbodyCPI += '<tr><td>' + data.d[6][i].MaxCPI + '</td><td>' + data.d[6][i].MinCPI + '</td><td>' + data.d[6][i].AvgCPI + '</td></tr>';
                        }
                        $("#tblCPI tbody").html(tbodyCPI);
                        var tbodySuccessRate = "";
                        for (var i = 0; i < data.d[7].length; i++) {
                            tbodySuccessRate += '<tr><td>' + data.d[7][i].ProjectSuccessRate + '</td><td>' + data.d[7][i].IRSuccessRate + '</td></tr>';
                        }
                        $("#tblSuccesrate tbody").html(tbodySuccessRate);
                        var tbodyRespondants = "";
                        $('#lblCompletes').empty();
                        for (var i = 0; i < data.d[8].length; i++) {

                            tbodyRespondants += '<tr><td>' + data.d[8][i].Total + '</td><td>' + data.d[8][i].Complete + '</td><td>' + data.d[8][i].Incomplete + '</td><td>' + data.d[8][i].Screened + '</td><td>' + data.d[8][i].Quotafull + '</td><td>' + data.d[8][i].Terminate + '</td></tr>';
                            $('#lblCompletes').text(data.d[8][i].Complete);
                        }
                        $("#tblRespondants tbody").html(tbodyRespondants);

                        //$("#tblCountry").DataTable();
                        //$("#tblClientWise").DataTable();
                        //$("#tblSupplierWise").DataTable();
                        //$("#tblprojectstotal").DataTable();   
                    },
                    error: function (result) {
                        alert(result.d);
                    }
                });


            });



            $("#ddlstatus").change(function () {

                //var st = $("#ddlstatus").val();
                //alert(st);
                if ($("#ddlstatus").val() == 'S') {

                    $("#PStatus").show();
                    $("#Respodents").hide();
                }
                else {
                    $("#PStatus").hide();
                    $("#Respodents").show();
                }

            });



            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'ProjectDetails.aspx/GetProjectDetails',
                cache: false,
                dataType: "json",
                success: function (data) {
                    var tbody = "";
                    //tbody += '<tr><td>Respondents</td><td>4346</td><td>2335</td><td>2310</td><td>544</td><td>456</td></tr>';

                    for (var i = 0; i < data.d.length; i++) {

                        tbody += '<tr><td>' + data.d[i].PName + '</td><td>' + data.d[i].ClientName + '</td><td>' + data.d[i].PManager + '</td><td>' + data.d[i].LOI + '</td><td>' + data.d[i].SDate + '</td><td>' + data.d[i].EDate + '</td></tr>';
                        //$("#myTable1 tbody").append('<tr><td>' + data.d[i].Company + '</td><td>' + data.d[i].Person + '</td><td>' + data.d[i].Number + '</td><td>' + data.d[i].Email + '</td><td>' + data.d[i].Country + '</td></tr>')
                    }
                    $("#tblrespondents tbody").html(tbody);
                    $("#tblrespondents").DataTable();
                },
                error: function (result) {
                    alert(result.d);
                }
            });


        });





    </script>


</body>
</html>
