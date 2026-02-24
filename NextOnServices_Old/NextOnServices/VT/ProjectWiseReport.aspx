<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProjectWiseReport.aspx.cs" Inherits="ProjectWiseReport" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
    <title></title>
    <style>
        table thead tr th {
            padding: 11px !important;
        }


        table tbody tr td {
            padding: 5px 15px !important;
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

                                    <div class="clearfix"></div>
                                    <div class="row">

                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Projects</label>
                                                <select id="ddProjects" class="ui search selection dropdown form-control">
                                                </select>
                                            </div>
                                        </div>

                                        <%--<div class="clearfix"></div>--%>
                                        <div class="col-sm-3 col-xs-12">
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
                    <div class="row" id="">
                        <div class="col-sm-12">
                            <div class="panel panel-default card-view">
                                <div class="panel-wrapper collapse in clear">
                                    <div class="panel-body" id="newDesign">
                                        <div class="table-wrap">
                                            <style>
                                                .header {
                                                    font-weight: 600;
                                                }
                                            </style>
                                            <div class="row" id="">
                                                <div class="panel-body">
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                            <input type="button" id="btnExport" onclick="tablesToExcel(['hprojects', 'tblprojectstotal', 'space', 'space', 'OverAll', 'tblTotalRevenue', 'space', 'space', 'hRespondents', 'tblRespondants', 'space', 'space', 'SupplierWise', 'tblSupplierWise'], ['first'], 'myfile.xls')" class="pull-right btn btn-sm" value="Export to Excel" />
                                                            <script src="../Scripts/cust.js"></script>
                                                        </div>
                                                    </div>


                                                </div>
                                                <div class="col-sm-6" id="mainDetails" style="display: none">
                                                    <h4 id="hprojects">Overview</h4>
                                                    <table class="table table-hover display table-bordered table-custom" id="tblprojectstotal">
                                                        <tr>
                                                            <td class="header w-30">Project Number</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblPNumber" Text="NXTABC"></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="header">Status</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblStatus" Text=""></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="header">Client</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblClient" Text="Some Client"></asp:Label></td>

                                                        </tr>
                                                        <tr>
                                                            <td class="header">Manager</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblManager" Text="Project Manager"></asp:Label></td>

                                                        </tr>
                                                        <tr>
                                                            <td class="header">Country</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblCountry" Text="Yet to Come"></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="header">Completes</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblCompletes" Text="Yet to Come"></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="header">CPC</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblCPC" Text="Yet to Come"></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="header">Revenue From Completes</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblRevenue" Text="Yet to Come"></asp:Label></td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <div class="col-sm-6" id="TotalDetails" style="display: none">
                                                    <h4 id="OverAll">Over All</h4>
                                                    <table class="table table-hover display table-bordered table-custom" id="tblTotalRevenue">
                                                        <tr>
                                                            <td class="header w-30">Total Revenue</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblTotalRevenue" Text="NXTABC"></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="header">Total Cost</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblCost" Text=""></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="header">Gross Profit</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblProfit" Text="Some Client"></asp:Label></td>

                                                        </tr>
                                                        <tr>
                                                            <td class="header">Margin</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblMargin" Text="Project Manager"></asp:Label></td>

                                                        </tr>
                                                    </table>
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
                                            <h4 id="hRespondents">Country wise Value</h4>
                                            <div class="table-wrap">
                                                <div class="">
                                                    <table id="tblRespondants" class="table table-hover display  pb-30 table-custom table-bordered">
                                                        <thead>
                                                            <tr>
                                                                <th>Country</th>
                                                                <th>Completes</th>
                                                                <th>CPC</th>
                                                                <th>Total</th>
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
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="panel panel-default card-view">
                                <div class="panel-wrapper collapse in">
                                    <div class="panel-body">

                                        <div class="col-sm-12">
                                            <h4 id="SupplierWise">Supplier wise Value</h4>
                                            <div class="table-wrap">
                                                <div class="">
                                                    <table id="tblSupplierWise" class="table table-hover display  pb-30 table-custom table-bordered">
                                                        <thead>
                                                            <tr>
                                                                <th>Country</th>
                                                                <th>Partner</th>
                                                                <th>Completes</th>
                                                                <th>CPC</th>
                                                                <th>Cost</th>
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
    <%--<script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js" type="text/javascript"></script>--%>
    <%--<script src="Scripts/UI/jquery-1.4.2.js"></script>--%>

    <%--<link href="Scripts/UI/themes/base/jquery-ui.css" rel="stylesheet" />--%>
    <%--<script src="Scripts/UI/ui/minified/jquery-ui.min.js"></script>--%>
    <script src="../Scripts/Dropdown/semantic.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.blockUI/2.70/jquery.blockUI.min.js"></script>
    <script type="text/javascript">

        jQuery(document).ready(function () {
            $(document).ajaxStart(jQuery.blockUI({ message: '<h1 style="font-size: 25px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' })).ajaxStop(jQuery.unblockUI);
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'ProjectWiseReport.aspx/GetProjects',
                cache: false,
                dataType: "json",
                success: function (data) {
                    $("#ddProjects").empty();
                    if (data.d.length > 0) {
                        $("#ddProjects").append('<option value="0">--Search Project--</option>');
                        for (var i = 0; i < data.d.length; i++) {
                            $("#ddProjects").append('<option value="' + data.d[i].ID + '">' + data.d[i].PName + '</option>');
                        }
                        $('#ddProjects').dropdown();
                    }
                    else {
                    }
                },
                error: function () {
                    alert("n");
                }
            });
            $('#btnSubmit').click(function () {
                $(document).ajaxStart(jQuery.blockUI({ message: '<h1 style="font-size: 25px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' })).ajaxStop(jQuery.unblockUI);
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'ProjectWiseReport.aspx/GetProjectDetails',
                    data: JSON.stringify({ Id: $('#ddProjects').val() }),
                    cache: false,
                    dataType: "json",
                    success: function (data) {

                        var tbody = "";
                        if (data.d[0].length > 0) {
                            $('#mainDetails').show();
                            $('#lblPNumber').text(data.d[0][0].PNumber);
                            $('#lblStatus').text(data.d[0][0].PStatus);
                            $('#lblClient').text(data.d[0][0].Company);
                            $('#lblManager').text(data.d[0][0].Manager);
                            $('#lblCountry').text(data.d[0][0].Country);
                            $('#lblCompletes').text(data.d[0][0].Complete);
                            $('#lblCPC').text(data.d[0][0].CPC);
                            $('#lblRevenue').text(data.d[0][0].RevenueFromCompletes);

                        }
                        else {
                            $('#mainDetails').hide();
                        }
                        if (data.d[3].length > 0) {
                            $('#TotalDetails').show();
                            $('#lblTotalRevenue').text(data.d[3][0].TotalRevenue);
                            $('#lblCost').text(data.d[3][0].TotalCost);
                            $('#lblProfit').text(data.d[3][0].GrossProfit);
                            $('#lblMargin').text(data.d[3][0].Margin);
                        }
                        else {
                            $('#TotalDetails').hide();
                        }

                        var tbodytotal = "";
                        for (var i = 0; i < data.d[1].length; i++) {
                            tbodytotal += '<tr><td>' + data.d[1][i].Country + '</td><td>' + data.d[1][i].Complete + '</td><td>' + data.d[1][i].CPC + '</td><td>' + data.d[1][i].TotalRevenue + '</td></tr>';
                        }
                        $("#tblRespondants tbody").html(tbodytotal);
                        var tbodySupplier = "";
                        for (var i = 0; i < data.d[2].length; i++) {
                            tbodySupplier += '<tr><td>' + data.d[2][i].Country + '</td><td>' + data.d[2][i].Supplier + '</td><td>' + data.d[2][i].Complete + '</td><td>' + data.d[2][i].CPC + '</td><td>' + data.d[2][i].Cost + '</td></tr>';
                        }
                        $("#tblSupplierWise tbody").html(tbodySupplier);

                    },

                    error: function () {
                        alert("n");
                    }
                });
            });

        });
    </script>
</body>
</html>
