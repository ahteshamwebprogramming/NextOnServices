<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RespondentsReport.aspx.cs" Inherits="RespondentsReport" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Respondents Report</title>
    <link href="../dist/css/CustmAhte.css" rel="stylesheet" />
    <style>
        table thead tr th {
            padding: 11px;
        }

        table tbody tr td {
            padding: 5px 15px !important;
        }
    </style>
    <script>
        function alertt(message1, message2, message3) {
            swal(
                message1,
                message2,
                message3
            )
        }
        function success(message, typee) {
            swal({
                position: 'top-center',
                type: typee,
                title: message,
                showConfirmButton: false,
                timer: 2000
            })
        }
    </script>
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
                                        <h6 class="panel-title txt-dark">Filter</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                    <div class="row">
                                        <div class="col-sm-12">
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
                                                <label class="control-label mb-10">Respondents Id</label>
                                                <input type="text" id="txtRespondents" class="form-control" name="Respondents" />
                                            </div>
                                            <div class="form-group">
                                                <button type="button" class="btn btn-success btn-anim" id="btnSubmit" name="btnSubmit">Search</button>
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
                                        <h6 class="panel-title txt-dark">Status Change</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <asp:Label ID="lblStatus" runat="server" Font-Bold="true"></asp:Label>
                                            <div class="form-group">
                                                <label class="control-label mb-10">Select</label>
                                                <select id="ddlStatus" class="form-control" name="ddlCountry">
                                                    <option value="0">--Select--</option>
                                                    <option value="1">Complete</option>
                                                    <%-- <option value="2">Screened</option>--%>
                                                    <option value="3">Incomplete</option>
                                                    <option value="4">OVERQUOTA</option>
                                                    <option value="5">Terminate</option>
                                                    <option value="6">SEC_TERM</option>
                                                    <option value="6">F_ERROR</option>
                                                </select>
                                            </div>

                                            <%-- <div class="form-group">
                                                <label class="control-label mb-10">Respondents Id</label>
                                                <input type="text" id="Text2" class="form-control" name="Respondents" />
                                            </div>--%>
                                            <div class="form-group">
                                                <button type="button" class="btn btn-success btn-anim" id="btnStatus" name="btnStatus">Change Status</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>


                    <!-- Row -->



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
                                                                        <th>
                                                                            <label class="container1">
                                                                                <input type="checkbox" id="chkall" /><span class="checkmark"></span>
                                                                            </label>
                                                                        </th>
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
    <%--<script src="../Scripts/UI/jquery-1.4.2.js"></script>--%>

    <link href="../Scripts/UI/themes/base/jquery-ui.css" rel="stylesheet" />

    <script src="../Scripts/UI/ui/minified/jquery-ui.min.js"></script>
    <script type="text/javascript">




        //$('body').on('focus', ".datepicker", function () {
        //    $(this).datepicker();
        //});


        jQuery(document).ready(function () {


            $('#chkall').change(function () {
                if ($('#chkall').is(':checked')) {
                    // alert("C");
                    $('#tblRespondents').find('tr').each(function () {
                        var row = $(this);
                        var id = $(row.find('input[type="checkbox"]')).attr("id");
                        //  alert(id)
                        $("#" + id + "").attr("checked", "checked");
                        //  alert("d");
                    });
                }
                else {
                    // alert("U");
                    $('#tblRespondents').find('tr').each(function () {
                        var row = $(this);
                        var id = $(row.find('input[type="checkbox"]')).attr("id");
                        $("#" + id + "").removeAttr("checked");
                    });
                }
            });


            $('#btnStatus').click(function () {
                // alert("hi");
                var result = true;
                if ($('#ddlStatus option:selected').text() != '--Select--') {
                    $('#tblRespondents').find('tr').each(function () {
                        var row = $(this);
                        if (row.find('input[type="checkbox"]').is(':checked')) {
                            var id = $(row.find('input[type="checkbox"]')).attr("id");
                            $.ajax({
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                url: 'RespondentsReport.aspx/Updatestatus',
                                data: JSON.stringify({ Id: id, status: $('#ddlStatus option:selected').text() }),
                                cache: false,
                                dataType: "json",
                                success: function () {
                                    //alert(id + "-" + $('#ddlStatus option:selected').text());
                                },
                                error: function () {
                                    result = false;
                                    alert(result.d);
                                }
                            });
                        }
                    });
                    // alert("Updated Succesfully");
                    success('Updated successfully', 'success');
                    $("#chkall").removeAttr("checked");
                }
                else { alertt('', 'Please select Status', 'error'); }
                if (result = true) {
                    // alert("s");
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: 'RespondentsReport.aspx/GetRespondents',
                        data: JSON.stringify({ id: $('#txtRespondents').val(), type: $('#ddlRespondents').val() }),
                        cache: false,
                        dataType: "json",
                        success: function (data) {
                            // alert(data.d.length);
                            var tbody = "";
                            for (var i = 0; i < data.d.length; i++) {
                                tbody += '<tr><td> <label class="container1"><input type="checkbox" id="' + data.d[i].UID + '" /><span class="checkmark"></span></label></td><td>' + data.d[i].SupplierName + '</td><td>' + data.d[i].SID + '</td><td>' + data.d[i].UID + '</td><td>' + data.d[i].Country + '</td><td>' + data.d[i].SupplierId + '</td><td>' + data.d[i].Status + '</td><td>' + data.d[i].Sdate + '</td><td>' + data.d[i].Edate + '</td><td>' + data.d[i].Duration + '</td><td>' + data.d[i].ClientBrowser + '</td><td>' + data.d[i].ClientIP + '</td><td>' + data.d[i].Device + '</td></tr>';
                            }
                            $("#tblRespondents tbody").html(tbody);
                            $("#tblRespondents").DataTable();
                        },
                        error: function (result) {
                            alert(result.d);
                        }
                    });
                }
            });

            $('#btnSubmit').click(function () {
                // alert("2");
                if ($('#ddlRespondents').val() == 0) {
                    //alert($('#txtRespondents').val());
                    alertt('', 'Please select UniqueId or Supplier ID from Dropdown', 'error');
                    // $('#txtRespondents').val('');
                }
                else {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: 'RespondentsReport.aspx/GetRespondents',
                        data: JSON.stringify({ id: $('#txtRespondents').val(), type: $('#ddlRespondents').val() }),
                        cache: false,
                        dataType: "json",
                        success: function (data) {
                            // alert(data.d.length);
                            var tbody = "";
                            for (var i = 0; i < data.d.length; i++) {
                                tbody += '<tr><td><label class="container1"><input type="checkbox" id="' + data.d[i].UID + '" /><span class="checkmark"></span></label></td><td>' + data.d[i].SupplierName + '</td><td>' + data.d[i].SID + '</td><td>' + data.d[i].UID + '</td><td>' + data.d[i].Country + '</td><td>' + data.d[i].SupplierId + '</td><td>' + data.d[i].Status + '</td><td>' + data.d[i].Sdate + '</td><td>' + data.d[i].Edate + '</td><td>' + data.d[i].Duration + '</td><td>' + data.d[i].ClientBrowser + '</td><td>' + data.d[i].ClientIP + '</td><td>' + data.d[i].Device + '</td></tr>';
                            }
                            $("#tblRespondents tbody").html(tbody);
                            $("#tblRespondents").DataTable();
                        },
                        error: function (result) {
                            alert(result.d);
                        }
                    });
                }



            });
        });
    </script>

</body>
</html>
