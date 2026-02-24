<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProjectInfoSearc.aspx.cs" Inherits="ProjectInfoSearc" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        table thead tr th {
            padding: 11px;
        }

        table tbody tr td {
            padding: 5px 15px !important;
        }
    </style>
    <%--0000000000000000000000000000000000000000000000000000000000000--%>
    <link href="https://cdn.datatables.net/1.10.16/css/jquery.dataTables.min.css" rel="stylesheet" />
    <link href="https://cdn.datatables.net/buttons/1.5.1/css/buttons.dataTables.min.css" rel="stylesheet" />
    <%--0000000000000000000000000000000000000000000000000000000000000--%>
</head>
<body>
    <form id="form1" runat="server">
        <div class="preloader-it">
            <div class="la-anim-1"></div>
        </div>
        <!--/Preloader-->
        <div class="wrapper theme-1-active pimary-color-red">

            <uc1:Header ID="Header1" runat="server" />
            <div class="right-sidebar-backdrop"></div>

            <div class="page-wrapper">
                <div class="container-fluid">

                    <!-- Title -->
                    <div class="row heading-bg">
                        <div class="col-lg-3 col-md-4 col-sm-4 col-xs-12">
                            <h5 class="txt-dark">Project Summary</h5>
                        </div>

                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Project</a></li>
                                <%--<li><a href="#"><span>Project</span></a></li>--%>
                                <li class="active"><span>Project Summary</span></li>
                            </ol>
                        </div>
                        <!-- /Breadcrumb -->

                    </div>
                    <!-- /Title -->

                    <!-- Row -->
                    <div class="row">
                        <div class="col-md-12">
                            <div class="panel panel-default card-view">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Filter</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <asp:Label ID="lblmsg" runat="server" Font-Bold="true"></asp:Label>
                                            <div class="form-group">
                                                <label class="control-label mb-10">Select</label>
                                                <select id="ddlRespondents" class="form-control" name="ddlCountry">
                                                    <option value="0">--Select--</option>
                                                    <option value="1">Unique Id</option>
                                                    <option value="2">Supplier Id</option>
                                                </select>
                                            </div>
                                            <div class="form-group">
                                                <button type="button" class="btn btn-success btn-anim" id="btnSubmit" name="btnSubmit">Search</button>
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Respondents Id</label>
                                                <div class="form-group">
                                                    <textarea id="txtIds" class="form-control" rows="9"></textarea>
                                                </div>
                                            </div>
                                        </div>



                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>


                    <!-- Row -->


                    <%--<div class="row" id="faildive">
                        <div class="col-sm-12">
                            <div class="panel panel-default card-view">
                                <div class="panel-wrapper collapse in" id="Div3">
                                    <div class="panel-body" id="">
                                        <div class="row">
                                            <div class="col-sm-12" id="Div1">
                                                <h3 style="font-size: 20px"><span id="spnFailed">0</span> out of <span id="spnTotal">0</span> Failed</h3>                                       
                                                <div id="divfailed"></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>--%>
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
                                    <div class="panel-body">
                                        <div class="row">
                                            <div class="col-sm-12">
                                                <h4>Respondents</h4>
                                                <div style="overflow-x: scroll">
                                                    <div class="table-wrap">
                                                        <div class="">
                                                            <table id="tblRespondents" class="table table-hover display  pb-30">
                                                                <thead>
                                                                    <tr>
                                                                        <th>Project Name</th>
                                                                        <th>Project Number</th>
                                                                        <th>Supplier Name</th>
                                                                        <th>SID</th>
                                                                        <th>UID</th>
                                                                        <th>Country</th>
                                                                        <th>Supplier ID</th>
                                                                        <th>Status</th>
                                                                        <th>StartDate</th>
                                                                        <th>EndDate</th>
                                                                        <th>Duration(Mins)</th>
                                                                        <th>ClientBrowser</th>
                                                                        <th>ClientIP</th>
                                                                        <th>Device</th>
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
                </div>

            </div>


        </div>
        <!-- Footer -->
        <uc2:Footer ID="Footer1" runat="server" />
        <!-- /Footer -->



    </form>
    <script src="//cdn.datatables.net/1.10.16/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <link href="../Scripts/UI/themes/base/jquery-ui.css" rel="stylesheet" />
    <%--0000000000000000000000000000000--%>
    <%--<script src="https://code.jquery.com/jquery-1.12.4.js"></script>--%>
    <script src="https://cdn.datatables.net/1.10.16/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.5.1/js/dataTables.buttons.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.5.1/js/buttons.flash.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jszip/3.1.3/jszip.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.32/pdfmake.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.32/vfs_fonts.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.5.1/js/buttons.html5.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.5.1/js/buttons.print.min.js"></script>
    <%--0000000000000000000000000000000--%>
    <script type="text/javascript">
        var table;
        jQuery(document).ready(function () {

            $('#btnSubmit').click(function () {
                if ($('#txtIds').val() == '') {
                    alert("Please enter ID's")
                }
                else if ($('#ddlRespondents').val() == 0) {
                    alert("Please select Id Type")
                }

                else {
                    GetStatus();
                }
            });
        });
        function GetStatus() {

            var failed = '';
            var total = 0;
            var failcnt = 0;
            var text = $('#txtIds').val();
            var Opt = 0;
            if ($('#ddlRespondents').val() == 2) {
                var Opt = 1;
            }
            else if ($('#ddlRespondents').val() == 1) { var Opt = 2; }
            var pair;
            var vars = text.split("\n");
            $('#faildive').append('');
            var tbody = "";
            for (var i = 0; i < vars.length; i++) {
                if (vars[i] != '') {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: 'ProjectInfoSearc.aspx/Getstatus',
                        async: false,
                        data: JSON.stringify({ opt: Opt, id: vars[i] }),
                        cache: false,
                        dataType: "json",
                        success: function (data) {
                            //total = total + 1;
                            //$('#faildive').show();
                            //$('#spnTotal').text(total);
                            for (var i = 0; i < data.d.length; i++) {
                                tbody += '<tr><td>' + data.d[i].PName + '</td><td>' + data.d[i].PID + '</td><td>' + data.d[i].SupplierName + '</td><td id="sidcell">' + data.d[i].SID + '</td><td id="uidcell">' + data.d[i].UID + '</td><td>' + data.d[i].Country + '</td><td>' + data.d[i].SupplierId + '</td><td>' + data.d[i].Status + '</td><td>' + data.d[i].Sdate + '</td><td>' + data.d[i].Edate + '</td><td>' + data.d[i].Duration + '</td><td>' + data.d[i].ClientBrowser + '</td><td>' + data.d[i].ClientIP + '</td><td>' + data.d[i].Device + '</td></tr>';
                            }
                            // $("#tblRespondents tbody").html(tbody);
                            if ($.fn.dataTable.isDataTable('#tblRespondents')) {
                                //table = $('#myTable1').DataTable();
                                table.destroy();
                                $("#tblRespondents tbody").html(tbody);
                                table = $('#tblRespondents').DataTable({
                                    "aaSorting": [[0, 'desc']],
                                    "iDisplayLength": 50,
                                    dom: 'Bfrtip',
                                    buttons: [
                                        'copy', 'csv', 'excel', 'pdf', 'print'
                                    ]

                                });
                            }
                            else {
                                $("#tblRespondents tbody").html(tbody);
                                table = $('#tblRespondents').DataTable({
                                    "aaSorting": [[0, 'desc']],
                                    "iDisplayLength": 50,
                                    dom: 'Bfrtip',
                                    buttons: [
                                        'copy', 'csv', 'excel', 'pdf', 'print'
                                    ]
                                });
                            }
                        },
                        error: function () {
                            alert("n");
                        }
                    });
                }
            }
        }
    </script>
</body>
</html>
