<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OverallReport.aspx.cs" Inherits="OverallReport" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Overall</title>
    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
    <link href="../dist/css/cust.css" rel="stylesheet" />
    <style>
        #tables table thead tr th {
            padding: 11px !important;
        }

        #tables table tbody tr td {
            padding: 5px 15px !important;
        }

        #chartdiv2 {
            width: 85%;
            height: 150px;
            margin: auto;
        }

        #chartdiv {
            width: 74%;
            height: 110px;
            margin: auto;
            border: 1px dotted #ccc;
        }

        svg g:nth-child(8) > g:first-child path {
            fill: blue;
        }

        svg g:nth-child(8) > g:nth-child(2) path {
            fill: orange;
        }

        svg g:nth-child(8) > g:nth-child(3) path {
            fill: gray;
        }

        .amcharts-chart-div a {
            display: none !important;
        }

        .amcharts-export-menu a {
            display: none !important;
        }

        .amcharts-value-axis text {
            display: none !important;
        }

        #tblprojectstotal thead tr th, #tblprojectstotal tbody tr td, #tblSuccesrate tbody tr td, #tblSuccesrate thead tr th, #tblRespondants tbody tr td, #tblRespondants thead tr th, #tblIrate thead tr th, #tblIrate tbody tr td {
            text-align: center;
        }

            #tblIrate thead tr th:first-child, #tblIrate tbody tr td:first-child {
                text-align: left;
            }
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
                            <h5 class="txt-dark">Overall Report</h5>
                        </div>

                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <%--<li><a href="#"><span>Report</span></a></li>--%>
                                <li class="active"><span>Overall Report</span></li>
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

                                        <div class="col-xs-12">
                                            <asp:Label ID="lblmsg" runat="server" Font-Bold="true"></asp:Label>
                                        </div>

                                        <div class="col-sm-2 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Start Date</label>
                                                <%--<asp:TextBox ID="txtSDate" runat="server" class="form-control" placeholder="Enter Start Date"></asp:TextBox>--%>
                                                <input type="text" id="txtSDate1" class="form-control" />

                                            </div>
                                        </div>
                                        <div class="col-sm-2 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">End Date</label>
                                                <input type="text" id="txtEDate1" class="form-control" />
                                                <%--  <asp:TextBox ID="txtEDate" runat="server" class="form-control" placeholder="Enter End Date"></asp:TextBox>
                                                --%>
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
                                        <div class="col-sm-2 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Client</label>
                                                <select id="ddlClients" class="ui search selection dropdown form-control" name="ddlstatus">
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-sm-2 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Supplier</label>
                                                <select id="ddlSuppliers" class="ui search selection dropdown form-control" name="ddlstatus">
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-sm-2 col-xs-12 text-right">
                                            <div class="form-group">
                                                <%-- <asp:Button ID="btnSubmit" runat="server" class="btn btn-success btn-anim" OnClientClick="return validation();"
                                                    Text="Submit"  />--%>
                                                <label for="button"></label>
                                                <button type="button" style="margin-top: 30px" class="btn btn-success btn-anim" id="btnSubmit" name="btnSubmit">Apply</button>
                                            </div>
                                        </div>
                                        <div class="clearfix"></div>
                                        <div class="col-xs-12">
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="tables">
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="panel panel-default card-view">
                                    <div class="panel-wrapper collapse in" id="charts">
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-sm-3" style="text-align: center">

                                                    <script src="https://www.amcharts.com/lib/3/amcharts.js"></script>
                                                    <script src="https://www.amcharts.com/lib/3/pie.js"></script>
                                                    <div style="margin-top: 80px;">
                                                        <label class="control-label mb-10" style="font-size: 25px; font-weight: 700">CPI</label>
                                                        <div id="chartdiv"></div>
                                                        <div>
                                                            <label style="background-color: #0000ff; width: 11px; height: 10px"></label>
                                                            <label>Min</label>
                                                            <label style="background-color: #ffa500; width: 11px; height: 10px"></label>
                                                            <label>Max</label>
                                                            <label style="background-color: #808080; width: 11px; height: 10px"></label>
                                                            <label>Mean</label>

                                                        </div>
                                                    </div>
                                                    <%--<img src="Imgs/pie.jpg" class="img" />--%>
                                                </div>
                                                <div class="col-sm-6" style="text-align: center">
                                                    <label id="labelll">
                                                        <input id="chkswitch" type="checkbox" checked="checked" />
                                                        <div>
                                                            <span class="on" style="font-size: 38px; font-weight: bolder">$</span>
                                                            <span class="off" style="font-size: 38px; font-weight: bolder">N</span>
                                                        </div>
                                                        <i></i>
                                                    </label>
                                                    <label id="lblCompletes" class="completes"></label>
                                                    <label id="lblValue" class="completes"></label>
                                                    <h6 id="hHeading">Completes</h6>
                                                </div>
                                                <div class="col-sm-3" style="text-align: center">
                                                    <script src="https://www.amcharts.com/lib/3/amcharts.js"></script>
                                                    <script src="https://www.amcharts.com/lib/3/serial.js"></script>
                                                    <script src="https://www.amcharts.com/lib/3/plugins/export/export.min.js"></script>
                                                    <link rel="stylesheet" href="https://www.amcharts.com/lib/3/plugins/export/export.css" type="text/css" media="all" />
                                                    <script src="https://www.amcharts.com/lib/3/themes/light.js"></script>
                                                    <div style="margin-top: 80px;">
                                                        <label class="control-label mb-10" style="font-size: 25px; font-weight: 700">Complete</label>
                                                        <div id="chartdiv2"></div>
                                                    </div>
                                                    <%--<img src="Imgs/bar.png" class="img" />--%>
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
                                    <div class="panel-wrapper collapse in" id="Respodents">
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
                                                color: #006767;
                                                font-size: 110px;
                                                line-height: 110px;
                                            }

                                            h6#hHeading {
                                                font-size: 40px;
                                                margin-bottom: 15px;
                                                color: #006767;
                                                text-transform: uppercase;
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
                                        <div class="panel-body">
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <input type="button" id="btnExport" onclick="tablesToExcel(['hSupplierWise', 'tblSupplierWise', 'space', 'space', 'hClientWise', 'tblClientWise', 'space', 'space', 'hCountryWise', 'tblCountry', 'space', 'space', 'hprojects', 'tblprojectstotal', 'space', 'space', 'hsuccessrate', 'tblSuccesrate', 'space', 'space', 'hRespondents', 'tblRespondants', 'space', 'space', 'hIcident', 'tblIrate'], ['first', 'second', 'Third'], 'myfile.xls')" class="pull-right btn btn-sm" value="Export to Excel" />
                                                    <script src="Scripts/cust.js"></script>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-sm-3">
                                                    <h4 id="hSupplierWise">Supplier</h4>
                                                    <div class="revenuetable">
                                                        <div class="table-wrap w_30">
                                                            <div class="">
                                                                <table id="tblSupplierwiseRev" class="table table-hover display  pb-30">
                                                                    <thead>
                                                                        <tr>
                                                                            <th>Supplier Name</th>
                                                                            <th>Cost</th>
                                                                            <th>%</th>
                                                                        </tr>
                                                                    </thead>
                                                                    <tbody></tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="completetable">
                                                        <div class="table-wrap w_30">
                                                            <div class="">
                                                                <table id="tblSupplierWise" class="table table-hover display  pb-30">
                                                                    <thead>
                                                                        <tr>
                                                                            <th>Supplier Name</th>
                                                                            <th>Complete</th>
                                                                            <th>%</th>
                                                                        </tr>
                                                                    </thead>
                                                                    <tbody></tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>
                                                <div class="col-sm-3">
                                                    <h4 id="hClientWise">Client</h4>
                                                    <div class="revenuetable">
                                                        <div class="table-wrap w_30">
                                                            <div class="">
                                                                <table id="tblClientwiseRev" class="table table-hover display  pb-30">
                                                                    <thead>
                                                                        <tr>
                                                                            <th>Client Name</th>
                                                                            <th>Value</th>
                                                                            <th>%</th>
                                                                        </tr>
                                                                    </thead>
                                                                    <tbody></tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="completetable">
                                                        <div class="table-wrap w_30">
                                                            <div class="">
                                                                <table id="tblClientWise" class="table table-hover display  pb-30">
                                                                    <thead>
                                                                        <tr>
                                                                            <th>Client Name</th>
                                                                            <th>Complete</th>
                                                                            <th>%</th>
                                                                        </tr>
                                                                    </thead>
                                                                    <tbody></tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>
                                                <div class="col-sm-3">
                                                    <h4 id="hCountryWise">Country</h4>
                                                    <div class="revenuetable">
                                                        <div class="table-wrap w_30">
                                                            <div class="">
                                                                <table id="tblCountryRev" class="table table-hover display  pb-30">
                                                                    <thead>
                                                                        <tr>
                                                                            <th>Country</th>
                                                                            <th>Value</th>
                                                                            <th>%</th>
                                                                        </tr>
                                                                    </thead>
                                                                    <tbody></tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="completetable">
                                                        <div class="table-wrap w_30">
                                                            <div class="">
                                                                <table id="tblCountry" class="table table-hover display  pb-30">
                                                                    <thead>
                                                                        <tr>
                                                                            <th>Country</th>
                                                                            <th>Total</th>
                                                                            <th>%</th>
                                                                        </tr>
                                                                    </thead>
                                                                    <tbody></tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>
                                                <div class="col-sm-3">
                                                    <h4 id="hManagerWise">Manager</h4>
                                                    <div class="revenuetable">
                                                        <div class="table-wrap w_30">
                                                            <div class="">
                                                                <table id="tblManagerWiseRev" class="table table-hover display  pb-30">
                                                                    <thead>
                                                                        <tr>
                                                                            <th>Manager</th>
                                                                            <th>Value</th>
                                                                            <th>%</th>
                                                                        </tr>
                                                                    </thead>
                                                                    <tbody></tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="completetable">
                                                        <div class="table-wrap w_30">
                                                            <div class="">
                                                                <table id="tblManagerWiseCom" class="table table-hover display  pb-30">
                                                                    <thead>
                                                                        <tr>
                                                                            <th>Manager</th>
                                                                            <th>Total</th>
                                                                            <th>%</th>
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
                        </div>
                        <div class="row" style="display:none">
                            <div class="col-sm-12">
                                <div class="panel panel-default card-view">
                                    <div class="panel-wrapper collapse in clear">
                                        <div class="panel-body">

                                            <div class="row">
                                                <div class="col-sm-7">
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
                                                <div class="col-sm-4">
                                                    <h4 id="hsuccessrate">Success Rate</h4>
                                                    <div class="table-wrap">
                                                        <table id="tblSuccesrate" class="table table-hover display  pb-30">
                                                            <thead>
                                                                <tr>
                                                                    <th>Project Success Rate</th>
                                                                    <th>IR Success Rate</th>
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
                        <div class="row" style="display:none">
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
                        <div class="row" style="display:none">
                            <div class="col-sm-12">
                                <div class="panel panel-default card-view">
                                    <div class="panel-wrapper collapse in">
                                        <div class="panel-body">
                                            <div class="col-sm-10 col-sm-offset-1">
                                                <h4 id="hIcident">Rate</h4>
                                                <div class="table-wrap">
                                                    <div class="">
                                                        <table id="tblIrate" class="table table-hover display  pb-30">
                                                            <thead>
                                                                <tr>
                                                                    <th>Parameters</th>
                                                                    <th>Min</th>
                                                                    <th>Max</th>
                                                                    <th>Average</th>
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


    <%--<link href="Scripts/UI/themes/base/jquery.ui.all.css" rel="stylesheet" />--%>
    <%--  <link rel="stylesheet" href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.5/themes/base/jquery-ui.css" type="text/css" media="all" />
    <link rel="stylesheet" href="http://static.jquery.com/ui/css/demo-docs-theme/ui.theme.css" type="text/css" media="all" />--%>
    <script src="../Scripts/UI/ui/minified/jquery-ui.min.js"></script>
    <%-- <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.5/jquery-ui.min.js" type="text/javascript"></script>--%>

    <script src="../Scripts/jquery.table2excel.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.blockUI/2.70/jquery.blockUI.min.js"></script>
    <script src="../CustomJS/Common.js"></script>
    <script src="../CustomJS/OverAllReport.js"></script>
    <script type="text/javascript">
        jQuery.noConflict();
        //$('body').on('focus', ".datepicker", function () {
        //    $(this).datepicker();
        //});
        jQuery(document).ready(function () {
            PageInitOverAll();
            $(document).ajaxStart(jQuery.blockUI({ message: '<h1 style="font-size: 25px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' })).ajaxStop(jQuery.unblockUI);


            $('#btnSubmit').click(function () {
                if ($('#txtSDate1').val() != '') {
                    if ($('#txtEDate1').val() == '' || $('#txtEDate1').val() == undefined) {
                        alert("Please select end date also");
                        return;
                        // ManageButtonClick();
                    }
                    else {
                        ManageButtonClick();
                    }
                }
                else if ($('#txtEDate1').val() != '') {
                    if ($('#txtSDate1').val() == '' || $('#txtSDate1').val() == undefined) {
                        alert("Please select Start date also");
                        return;
                    }
                    else {
                        ManageButtonClick();
                    }
                }
                else {
                    ManageButtonClick();
                }

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
                    if (data.d == null) {
                        return;
                    }
                    var tbody = "";
                    //tbody += '<tr><td>Respondents</td><td>4346</td><td>2335</td><td>2310</td><td>544</td><td>456</td></tr>';

                    for (var i = 0; i < data.d.length; i++) {

                        tbody += '<tr><td>' + data.d[i].PName + '</td><td>' + data.d[i].ClientName + '</td><td>' + data.d[i].PManager + '</td><td>' + data.d[i].LOI + '</td><td>' + data.d[i].SDate + '</td><td>' + data.d[i].EDate + '</td></tr>';
                        //$("#myTable1 tbody").append('<tr><td>' + data.d[i].Company + '</td><td>' + data.d[i].Person + '</td><td>' + data.d[i].Number + '</td><td>' + data.d[i].Email + '</td><td>' + data.d[i].Country + '</td></tr>')
                    }
                    $("#tblrespondents tbody").html(tbody);
                    //$("#tblrespondents").DataTable();
                },
                error: function (result) {
                    alert(result.d);
                }
            });
            //function exporttable() {
            //    $(".table2excel2").table2excel({
            //        exclude: ".noExl",
            //        name: "Excel Document",
            //        filename: "Iffco Bazaar Indent",
            //        fileext: ".xls",
            //        exclude_img: true,
            //        exclude_links: true,
            //        exclude_inputs: true
            //    });
            //    $("#tblCPI").table2excel({
            //        exclude: ".noExl",
            //        name: "Excel Document",
            //        filename: "Iffco Bazaar Indent",
            //        fileext: ".xls",
            //        exclude_img: true,
            //        exclude_links: true,
            //        exclude_inputs: true
            //    });
            //};

            //$('#btnExport').click(function () {
            //    // alert("hi");
            //    exporttable();
            //});

        });
        function getformdata(type, opt) {
            var attr = new Array();
            var object = {};
            if (opt == 'filters') {
                if (type == 'onload') {
                    object['sdate'] = $('#txtSDate1').val();;
                    attr.push(object);
                    object['edate'] = $('#txtEDate1').val();
                    attr.push(object);
                    object['countryid'] = 0;
                    attr.push(object);
                    object['clientid'] = 0;
                    attr.push(object);
                    object['supplierid'] = 0;
                    attr.push(object);
                }
                else if (type == 'onchange') {
                    object['sdate'] = $('#txtSDate1').val();
                    attr.push(object);
                    object['edate'] = $('#txtEDate1').val();
                    attr.push(object);
                    object['countryid'] = $('#ddlCountry').val();
                    attr.push(object);
                    object['clientid'] = $('#ddlClients').val();
                    attr.push(object);
                    object['supplierid'] = $('#ddlSuppliers').val();
                    attr.push(object);
                }
            }
            return object;
        }
        function loadManagerwisereport(type) {
            var FormData = getformdata(type, 'filters');
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'OverallReport.aspx/ManagerWiseReport',
                data: JSON.stringify({ formdata: FormData }),
                cache: false,
                dataType: "json",
                success: function (data) {
                    if (data.d == null) {
                        return;
                    }
                    var tbodyManagerWiseRev = "";
                    var tbodyManagerWiseCom = "";
                    for (var i = 0; i < data.d.length; i++) {
                        tbodyManagerWiseRev += '<tr><td>' + data.d[i].Name + '</td><td>' + data.d[i].Revenue + '</td><td>' + data.d[i].Revenuepercentage + '</td></tr>';
                        tbodyManagerWiseCom += '<tr><td>' + data.d[i].Name + '</td><td>' + data.d[i].complete + '</td><td>' + data.d[i].Completepercentage + '</td></tr>';
                    }
                    $("#tblManagerWiseRev tbody").html(tbodyManagerWiseRev);
                    $("#tblManagerWiseCom tbody").html(tbodyManagerWiseCom);
                    //jQuery.unblockUI();
                },
                error: function (result) {
                    alert(result.d);
                }
            });
        }



    </script>


</body>
</html>
