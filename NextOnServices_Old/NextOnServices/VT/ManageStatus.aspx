<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ManageStatus.aspx.cs" Inherits="ManageStatus" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style>
        table thead tr th {
            padding: 11px;
        }

        table tbody tr td {
            padding: 5px 15px !important;
        }
    </style>
    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
    <title>Manage Status</title>
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
                            <h5 class="txt-dark">Respondents Report</h5>
                        </div>

                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <%--<li><a href="#"><span>Report</span></a></li>--%>
                                <li class="active"><span>Respondents Report</span></li>
                            </ol>
                        </div>
                        <!-- /Breadcrumb -->

                    </div>
                    <!-- /Title -->


                    <div class="row">
                        <div class="col-md-6">
                            <div class="panel panel-default card-view">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <%-- <h6 class="panel-title txt-dark">Filter</h6>--%>
                                    </div>
                                    <div class="clearfix"></div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:Label ID="lblmsg" runat="server" Font-Bold="true"></asp:Label>
                                            <div class="form-group">
                                                <label class="control-label mb-10">Select Type Of ID</label>
                                                <select id="ddlRespondents" class="form-control" name="ddlCountry">
                                                    <option value="0">--Select--</option>
                                                    <option value="1">Unique Id</option>
                                                    <option value="2">Supplier Id</option>
                                                </select>
                                            </div>
                                            <div class="form-group" id="divProjects">
                                                <label class="control-label mb-10">Projects</label>
                                                <select id="ddProjects" class="form-control ui search selection dropdown" name="ddProjects">
                                                </select>
                                            </div>
                                            <%--<div class="form-group">
                                                <label id="lbluniresp" class="control-label mb-10">Respondents Id</label>
                                                <input type="text" id="txtRespondents" class="form-control" name="Respondents" />
                                            </div>--%>
                                            <asp:Label ID="lblStatus" runat="server" Font-Bold="true"></asp:Label>
                                            <div class="form-group">
                                                <label class="control-label mb-10">Select Status</label>
                                                <select id="ddlStatus" class="form-control" name="ddlCountry">
                                                    <option value="0">--Select--</option>
                                                    <option value="1">Complete</option>
                                                    <%--<option value="2">Screened</option>--%>
                                                    <option value="3">Incomplete</option>
                                                    <option value="4">OVERQUOTA</option>
                                                    <option value="5">Terminate</option>
                                                    <option value="6">SEC_TERM</option>
                                                    <option value="6">F_ERROR</option>
                                                </select>
                                            </div>
                                            <div class="form-group">
                                                <button type="button" class="btn btn-success btn-anim" id="btnStatus" name="btnStatus">Change Status</button>
                                                <%--<button type="button" class="btn btn-success btn-anim" id="btnSubmit" name="btnSubmit">Search</button>--%>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="panel panel-default card-view">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Enter IDs</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                    <div class="row">
                                        <div class="col-sm-12">


                                            <%-- <div class="form-group">
                                                <label class="control-label mb-10">Respondents Id</label>
                                                <input type="text" id="Text2" class="form-control" name="Respondents" />
                                            </div>--%>
                                            <div class="form-group">
                                                <textarea id="txtIds" class="form-control" rows="9"></textarea>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>


                    <!-- Row -->


                    <div class="row" id="faildive">
                        <div class="col-sm-12">
                            <div class="panel panel-default card-view">
                                <div class="panel-wrapper collapse in" id="Div3">
                                    <div class="panel-body" id="">
                                        <div class="row">
                                            <div class="col-sm-12" id="Div1">
                                                <h3 style="font-size: 20px"><span id="spnFailed">0</span> out of <span id="spnTotal">0</span> Failed</h3>
                                                <%--<br />
                                                <h4>Failed</h4>--%>
                                                <div id="divfailed"></div>
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
                                                <h4>Succesfull</h4>
                                                <div style="overflow-x: scroll">
                                                    <div class="table-wrap">
                                                        <div class="">
                                                            <table id="tblRespondents" class="table table-hover display  pb-30">
                                                                <thead>
                                                                    <tr>
                                                                        <%--<th>
                                                                            <input type="checkbox" id="chkall" /></th>--%>
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
                <!-- Footer -->
                <uc2:Footer ID="Footer1" runat="server" />
                <!-- /Footer -->

            </div>

        </div>

    </form>
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js" type="text/javascript"></script>

    <%--<script src="Scripts/UI/jquery-1.4.2.js"></script>--%>
    <link href="../Scripts/UI/themes/base/jquery-ui.css" rel="stylesheet" />

    <script src="../Scripts/UI/ui/minified/jquery-ui.min.js"></script>
    <script src="../Scripts/Dropdown/semantic.js"></script>
    <script type="text/javascript">




        //$('body').on('focus', ".datepicker", function () {
        //    $(this).datepicker();
        //});


        jQuery(document).ready(function () {

            $('#btntest').click(function () {
                $('#divfortest').empty();
                var textvalue = $('#txttest').val();
                $('#divfortest').append(textvalue);


            });

            $('#faildive').hide();
            //  $('#divProjects').hide();

            $('#btnStatus').click(function () {
                // $('#faildive').empty();
                if ($('#txtIds').val() == '') {
                    alert("Please enter ID's")
                }
                else if ($('#ddlRespondents').val() == 0) { alert("Please select Id Type") }
                else if ($('#ddlRespondents').val() == 1) {
                    if ($('#ddProjects').val() == 0) {
                        alert("please select Projects")
                    }
                    else {
                        changestatus();
                        getids();
                    }
                }
                else if ($('#ddlRespondents').val() == 2) {
                    if ($('#ddProjects').val() == 0) {
                        alert("please select Projects")
                    }
                    else {
                        changestatus();
                        getids();
                    }
                }


            });



            $('#ddlRespondents').change(function () {
                if ($('#ddlRespondents').val() == 1) {
                    // $('#divProjects').hide();
                    $('#txtIds').attr("rows", "9");
                }
                else if ($('#ddlRespondents').val() == 2) {
                    $('#divProjects').show();
                    $('#txtIds').attr("rows", "14");
                }
                else {
                    $('#divProjects').hide();
                    $('#txtIds').attr("rows", "9");
                    abc();
                }
            });

            function changestatus() {
                $('#divfailed').empty();
                var failed = '';
                var total = 0;
                var failcnt = 0;
                $('#spnFailed').text(failcnt);
                if ($('#ddlStatus option:selected').text() != '--Select--') {
                    if ($('#ddlRespondents option:selected').text() != '--Select--') {
                        var text = $('#txtIds').val();
                        var pair;
                        var vars = text.split("\n");
                        $('#faildive').append("");
                        var tbody = "";
                        if ($('#ddlStatus option:selected').text() == 'Complete') {
                            $.ajax({
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                url: 'ManageStatus.aspx/ChangeAll',
                                async: false,
                                data: JSON.stringify({ Id: '0', status: $('#ddlStatus option:selected').text(), type: '3', PId: $('#ddProjects option:selected').val() }),
                                cache: false,
                                dataType: "json",
                                success: function (data) { },
                                error: function (result) { }
                            });
                        }
                        for (var i = 0; i < vars.length; i++) {
                            if (vars[i] != '') {
                                $.ajax({
                                    type: "POST",
                                    contentType: "application/json; charset=utf-8",
                                    url: 'ManageStatus.aspx/Updatestatus',
                                    async: false,
                                    data: JSON.stringify({ Id: vars[i], status: $('#ddlStatus option:selected').text(), type: $('#ddlRespondents option:selected').val(), PId: $('#ddProjects option:selected').val() }),
                                    cache: false,
                                    dataType: "json",
                                    success: function (data) {
                                        total = total + 1;
                                        $('#faildive').show();
                                        $('#spnTotal').text(total);
                                        for (var i = 0; i < data.d.length; i++) {
                                            if (data.d[i].Error == '') { tbody += '<tr><td>' + data.d[i].SupplierName + '</td><td id="sidcell">' + data.d[i].SID + '</td><td id="uidcell">' + data.d[i].UID + '</td><td>' + data.d[i].Country + '</td><td>' + data.d[i].SupplierId + '</td><td>' + data.d[i].Status + '</td><td>' + data.d[i].Sdate + '</td><td>' + data.d[i].Edate + '</td><td>' + data.d[i].Duration + '</td><td>' + data.d[i].ClientBrowser + '</td><td>' + data.d[i].ClientIP + '</td><td>' + data.d[i].Device + '</td></tr>'; }
                                            else {
                                                failcnt = failcnt + 1;
                                                failed = failed + '<label style="font-size: 15px">' + data.d[i].Error + '</label><br />';
                                                //alert(failed);
                                                $('#spnFailed').text(failcnt);
                                                $('#divfailed').empty();
                                                $('#divfailed').append(failed);
                                            }
                                        }
                                        $("#tblRespondents tbody").html(tbody);
                                        //alert("tbale");
                                        //getids();
                                    },
                                    error: function () {
                                        alert("n");
                                    }
                                });
                            }

                        }
                    }
                    else {
                        alert("Please select Type of ID");
                    }

                }
                else {
                    alert("Please select Status");
                }

            }
            function getids() {
                var i = 0;
                var customerId = [];
                var Uid = [];
                $('#tblRespondents tr').each(function () {
                    customerId[i] = $(this).find('#sidcell').html();
                    Uid[i] = $(this).find('#uidcell').html();
                    i = i + 1;
                    // alert($(this).find('#sidcell').html());
                });

                //alert(customerId);
                //alert(Uid);
                //alert($('#ddProjects').val())
            }


            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'ManageStatus.aspx/GetProjects',
                cache: false,
                dataType: "json",
                success: function (data) {
                    $("#ddProjects").empty();
                    if (data.d.length > 0) {
                        $("#ddProjects").append('<option value="0">--Select Project--</option>');
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
        });
    </script>

</body>
</html>
