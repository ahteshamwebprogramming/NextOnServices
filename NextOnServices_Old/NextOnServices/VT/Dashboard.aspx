<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="Deshboard" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dashboard Details</title>

    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
    <link href="../dist/css/Tooltip.css" rel="stylesheet" />
    <link href="../dist/css/CustmAhte.css" rel="stylesheet" />
    <%--0000000000000000000000000000000000000000000000000000000000000--%>
    <link href="https://cdn.datatables.net/1.10.16/css/jquery.dataTables.min.css" rel="stylesheet" />
    <link href="https://cdn.datatables.net/buttons/1.5.1/css/buttons.dataTables.min.css" rel="stylesheet" />
    <%--0000000000000000000000000000000000000000000000000000000000000--%>


    <style>
        .mw {
            min-width: max-content;
        }

        .toolti_cust {
            font-weight: 700;
        }

        #myTable1 .tooltiptext1 {
            font-size: 10px;
        }

        .bordertophighlight {
            border-top: 1px solid #f1f1f1;
            box-shadow: 0px 1px 4px 1px #f1f1f1;
            padding: 12px;
        }

        #myTable1 thead tr th {
            padding: 11px;
        }

        .tooltip1 {
            border-bottom: 0px dotted black !important;
        }

        #myTable1 tbody tr td {
            padding: 10px 15px !important;
        }

            #myTable1 tbody tr td select {
                padding: 1px 10px 2px 2px;
                height: 35px;
            }

        .widthcust {
            width: 80px !important;
            text-align: center;
        }

        #myTable1 thead tr th, #myTable1 tbody tr td {
            text-align: center;
        }

            #myTable1 thead tr th:first-child, #myTable1 thead tr th:nth-child(2), #myTable1 thead tr th:nth-child(3), #myTable1 thead tr th:nth-child(4), #myTable1 thead tr th:nth-child(5), #myTable1 thead tr th:nth-child(9), #myTable1 tbody tr td:first-child, #myTable1 tbody tr td:nth-child(2), #myTable1 tbody tr td:nth-child(3), #myTable1 tbody tr td:nth-child(4), #myTable1 tbody tr td:nth-child(5), #myTable1 tbody tr td:nth-child(9) {
                text-align: left;
            }
    </style>
    <script>

        //function openWindow(code) {
        //    window.open('ProjectPageDetails.aspx?Id=' + code, 'open_window', ' width=1000, height=600, left=100, top=30');
        //}
    </script>
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
                <div class="container-fluid pt-25 ">
                    <!-- Row -->
                    <div class="row">
                        <div class="fourbox">
                            <div class="col-lg-3 col-md-6 col-sm-6 col-xs-12">
                                <div class="panel panel-default card-view pa-0">
                                    <div class="panel-wrapper collapse in">
                                        <div class="panel-body pa-0">
                                            <div class="sm-data-box bg-white">
                                                <div class="container-fluid">
                                                    <div class="row">
                                                        <div class="col-xs-7 text-center pl-0 pr-0 data-wrap-left">
                                                            <span class="txt-dark block counter"><span class="counter-anim" id="lblproject1">
                                                                <asp:Label ID="lblproject" runat="server"></asp:Label>
                                                            </span></span>
                                                            <span class="weight-500 uppercase-font txt-dark block font-13">Projects</span>
                                                        </div>
                                                        <div class="col-xs-5 text-center  pl-0 pr-0 data-wrap-right">
                                                            <i class="ti-package color1 data-right-rep-icon"></i>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-6 col-sm-6 col-xs-12">
                                <div class="panel panel-default card-view pa-0">
                                    <div class="panel-wrapper collapse in">
                                        <div class="panel-body pa-0">
                                            <div class="sm-data-box bg-white">
                                                <div class="container-fluid">
                                                    <div class="row">
                                                        <div class="col-xs-7 text-center pl-0 pr-0 data-wrap-left">
                                                            <span class="txt-dark block counter"><span class="counter-anim" id="lblActive1">
                                                                <asp:Label ID="lblActive" runat="server"></asp:Label>
                                                            </span></span>
                                                            <span class="weight-500 uppercase-font txt-dark block">Active</span>
                                                        </div>
                                                        <div class="col-xs-5 text-center  pl-0 pr-0 data-wrap-right">
                                                            <i class="ti-light-bulb color2 data-right-rep-icon"></i>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-6 col-sm-6 col-xs-12">
                                <div class="panel panel-default card-view pa-0">
                                    <div class="panel-wrapper collapse in">
                                        <div class="panel-body pa-0">
                                            <div class="sm-data-box bg-white">
                                                <div class="container-fluid">
                                                    <div class="row">
                                                        <div class="col-xs-7 text-center pl-0 pr-0 data-wrap-left">
                                                            <span class="txt-dark block counter"><span class="counter-anim" id="lblInactive1">
                                                                <asp:Label ID="lblInactive" runat="server"></asp:Label>
                                                            </span></span>
                                                            <span class="weight-500 uppercase-font txt-dark block">Inactive</span>
                                                        </div>
                                                        <div class="col-xs-5 text-center  pl-0 pr-0 data-wrap-right">
                                                            <i class="zmdi zmdi-file color3 data-right-rep-icon"></i>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-6 col-sm-6 col-xs-12">
                                <div class="panel panel-default card-view pa-0">
                                    <div class="panel-wrapper collapse in">
                                        <div class="panel-body pa-0">
                                            <div class="sm-data-box bg-white">
                                                <div class="container-fluid">
                                                    <div class="row">
                                                        <div class="col-xs-7 text-center pl-0 pr-0 data-wrap-left">
                                                            <span class="txt-dark block counter"><span class="counter-anim" id="lblArchived1">
                                                                <asp:Label ID="lblArchived" runat="server"></asp:Label>
                                                            </span></span>
                                                            <span class="weight-500 uppercase-font txt-dark block">Archived</span>
                                                        </div>
                                                        <div class="col-xs-5 text-center  pl-0 pr-0 pt-25  data-wrap-right">
                                                            <i class="ti-ruler-pencil color4 data-right-rep-icon"></i>
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
                    <!-- /Row -->

                    <style>
                        .full_width {
                            width: 100% !important;
                        }
                    </style>

                    <!-- Row -->

                    <div class="row">
                        <div class="col-sm-12">
                            <div class="panel panel-default card-view">
                                <div class="panel-heading">
                                    <div class="">
                                        <%-- <div class="col-md-6">
                                            <input type="button" value="Check Device" onclick="Device()" />
                                        </div>--%>
                                        <div class="col-md-5 col-md-offset-4">
                                            <span class="custom-dropdown">
                                                <select id="ddlManagers" class="" onchange="managedashboardmgr(this.id,'Manager')">
                                                    <%--<option>Sherlock Holmes</option>
                                                    <option>The Great Gatsby</option>
                                                    <option>V for Vendetta</option>
                                                    <option>The Wolf of Wallstreet</option>
                                                    <option>Quantum of Solace</option>--%>
                                                </select>
                                            </span>
                                            <span class="custom-dropdown">
                                                <select id="ddlMainStatus" class="" onchange="managedashboardmgr(this.id,'Status')">
                                                    <option value="0">--Select Status--</option>
                                                    <option value="1">Closed</option>
                                                    <option value="2">Live</option>
                                                    <option value="3">On Hold</option>
                                                    <option value="4">Cancelled</option>
                                                    <option value="5">Awarded</option>
                                                    <option value="6">Invoiced</option>
                                                </select>
                                            </span>
                                        </div>
                                        <div class="col-md-3">
                                            <div class="bordertophighlight" style="display: inline-block">
                                                <div class="col-md-4">
                                                    <div class="col-md-2" style="padding-left: 0px">
                                                        <label class="container1_ppd">
                                                            <input type="checkbox" id="chkBlue" onchange="ManageDashboardByFlag(this.id)" /><span class="checkmark_ppd"></span></label>
                                                    </div>
                                                    <span class="fa fa-flag tooltip1" style="color: blue; margin-left: 10px"><span class="tooltiptext1">IR Drop by 50%</span></span>
                                                </div>
                                                <div class="col-md-4">
                                                    <div class="col-md-2" style="padding-left: 0px">
                                                        <label class="container1_ppd">
                                                            <input type="checkbox" id="chkYellow" onchange="ManageDashboardByFlag(this.id)" /><span class="checkmark_ppd"></span></label>
                                                    </div>
                                                    <span class="fa fa-flag tooltip1" style="color: yellow; margin-left: 10px"><span class="tooltiptext1">LOI Raised by 35%</span></span>
                                                </div>
                                                <div class="col-md-4" style="padding-left: 0px">
                                                    <div class="col-md-2">
                                                        <label class="container1_ppd">
                                                            <input type="checkbox" id="chkRed" onchange="ManageDashboardByFlag(this.id)" /><span class="checkmark_ppd"></span></label>
                                                    </div>
                                                    <span class="fa fa-flag tooltip1" style="color: red; margin-left: 10px"><span class="tooltiptext1">IR drop by 50% and LOI Raised by 35%</span></span>
                                                </div>
                                            </div>
                                        </div>
                                        <%--<h6 class="panel-title txt-dark"><a href="#" onclick="chnmngr()">Change Manager</a></h6>--%>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-wrapper collapse in">
                                    <div class="panel-body">
                                        <div class="table-wrap">
                                            <div class="">
                                                <div style="overflow-x: scroll">
                                                    <table id="myTable1" class="table table-hover display  pb-30 table-custom">
                                                        <thead>
                                                            <tr>
                                                                <th class="tooltip1">PNO<span class="tooltiptext1">Project Number</span></th>
                                                                <th class="tooltip1">PName<span class="tooltiptext1">Project</span></th>
                                                                <th class="tooltip1">Client<span class="tooltiptext1">Client Name</span></th>
                                                                <th class="tooltip1">PM<span class="tooltiptext1">Project Manager</span></th>
                                                                <th class="tooltip1">Country<span class="tooltiptext1">Country</span></th>
                                                                <%--  <th>Sample Size</th>--%>
                                                                <th class="tooltip1">LOI<span class="tooltiptext1">Length Of Interview</span></th>
                                                                <th class="tooltip1">CPI<span class="tooltiptext1">Cost per Complete</span></th>
                                                                <th class="tooltip1">Irate<span class="tooltiptext1">IRate</span></th>
                                                                <%--<th>Start Date</th>--%>
                                                                <%--<th class="tooltip1">
                                                                    <label class="widthcust">Date</label><span class="tooltiptext1">Project End Date</span></th>--%>
                                                                <th class="tooltip1">Status<span class="tooltiptext1">Project Status</span></th>
                                                                <th class="tooltip1">Total<span class="tooltiptext1">Total Respondents</span></th>
                                                                <th class="tooltip1">CO<span class="tooltiptext1">Complete</span></th>
                                                                <th class="tooltip1">TR<span class="tooltiptext1">Terminate</span></th>
                                                                <th class="tooltip1">OQ<span class="tooltiptext1">Overquota</span></th>
                                                                <th class="tooltip1">
                                                                    <label class="">ST</label><span class="tooltiptext1">Security Term</span></th>
                                                                <th class="tooltip1">
                                                                    <label class="">FE</label><span class="tooltiptext1">Fraud Error</span></th>
                                                                <th class="tooltip1">IC<span class="tooltiptext1">Incomplete</span></th>
                                                                <th class="tooltip1">IR<span class="tooltiptext1">Incident Rate</span></th>
                                                                <th class="tooltip1">LOI<span class="tooltiptext1">Length of Interview</span></th>
                                                                <th class="tooltip1"><span class="fa fa-flag"></span><span class="tooltiptext1">Flag</span></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody></tbody>
                                                    </table>
                                                    <input type="hidden" id="hfdevice" name="hfdevice" runat="server" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- /Row -->

                </div>
                <!-- Footer -->
                <uc2:Footer ID="Footer1" runat="server" />
                <!-- /Footer -->

            </div>

        </div>

    </form>

    <%--<script src="../Scripts/UI/jquery-1.4.2.js"></script>--%>
    <script src="//cdn.datatables.net/1.10.16/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <%--<script src="scripts/ui/jquery-1.4.2.js"></script>--%>
    <%--<script src="scripts/dropdown/jquery.min.js"></script>--%>


    <script src="../Scripts/Dropdown/semantic.js"></script>
    <script src="../Scripts/DeviceDetection.js"></script>
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
    <%--<script src="../Scripts/UI/ui/minified/jquery-ui.min.js"></script>--%>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.blockUI/2.70/jquery.blockUI.min.js"></script>
    <script type="text/javascript">

        var table;

        jQuery(document).ready(function () {
            $(document).ajaxStart(jQuery.blockUI({ message: '<h1 style="font-size: 25px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' })).ajaxStop(jQuery.unblockUI);
            loadddl();
            var value = $('#ddlManagers').val();
            loadtable(value);
        });
        function statusccp(stat) {
            var len = stat.length;
            if ($.inArray(2, stat) > -1) {
                // $('#hstatus').text('Live');
                return 2;
            }
            else if ($.inArray(3, stat) > -1) {
                // $('#hstatus').text('On Hold');
                return 3;
            }
            else if ($.inArray(5, stat) > -1) {
                // $('#hstatus').text('Awarded');
                return 5;
            }
            else if ($.inArray(1, stat) > -1) {
                //  $('#hstatus').text('Closed');
                return 1;
            }
            else if ($.inArray(6, stat) > -1) {
                // $('#hstatus').text('Invoiced');
                return 6;
            }
            else if ($.inArray(4, stat) > -1) {
                // $('#hstatus').text('Cancelled');
                return 4
            }
            else {
                $('#hstatus').text('NAN');
                return 0;
            }
        }
        function getFlag(flag_code, operation) {
            if (operation == 'convertCode') {
                var style = '';
                if (flag_code != '') {
                    style = 'style="color:' + flag_code + '"';
                }
                else {
                    style = 'style="display:none"';
                }
                return style;
            }
        }
        function ManageDashboardByFlag(id) {
            jQuery.blockUI({ message: '<h1 style="font-size: 25px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' });
            loadtable();
        }
        function chckchckbox() {
            var UF = '0', B = '0', R = '0', Y = '0', flag;

            if ($('#chkBlue').is(':checked')) {
                B = '1';
            }
            if ($('#chkYellow').is(':checked')) {
                Y = '1';
            }
            if ($('#chkRed').is(':checked')) {
                R = '1';
            }
            var bit = B + Y + R;
            if (bit == '000')
                flag = 0;
            else if (bit == '100')
                flag = 1
            else if (bit == '010')
                flag = 2
            else if (bit == '001')
                flag = 3
            else if (bit == '110')
                flag = 4
            else if (bit == '101')
                flag = 5
            else if (bit == '011')
                flag = 6
            else if (bit == '111')
                flag = 7

            return flag;
        }
        function changestatus(id, value) {

            var status = $('#' + id + '').val();
            swal({
                title: 'Are you sure?',
                text: "You want to change the status?",
                type: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#26d06c',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, update it!'
            }).then((result) => {
                if (result.value) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: 'Dashboard.aspx/UpdateStatus',
                        data: JSON.stringify({ Id: id, Status: status }),
                        cache: false,
                        dataType: "json",
                        success: function (data) {
                            $('#' + id).val(status);
                            swal(
                                'Updated!',
                                'Your file has been Updated.',
                                'success'
                            )
                        },
                        error: function (result) {
                            alert(result.d);
                        }
                    });
                }
                $('#' + id).val(value);
            })
        }
        //manager dashboard
        function managedashboardmgr(id, type) {

            var value = $('#' + id).val();
            // alert(value);
            if (type == 'Manager') {

                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'Dashboard.aspx/GetDeshboardDetails',
                    data: JSON.stringify({ ID: value }),
                    cache: false,
                    dataType: "json",
                    success: function (response) {
                        $('#lblproject1').empty();
                        $('#lblActive1').empty();
                        $('#lblInactive1').empty();
                        $('#lblArchived1').empty();
                        $('#lblproject1').append(response.d[0].Project);
                        $('#lblActive1').append(response.d[0].Active);
                        $('#lblInactive1').append(response.d[0].Inactive);
                        $('#lblArchived1').append(response.d[0].Archived);


                    },
                    error: function (result) { alert('error'); }
                });
            }
            loadtable(value, type);
        }
        function loadtable(ID, type) {

            var Managevalue = $('#ddlManagers').val();
            var Statvalue = $('#ddlMainStatus').val();
            var flagstat = chckchckbox();
            // jQuery.blockUI({ message: '<h1 style="font-size: 25px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' });
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'ProjectDetails.aspx/GetProjectDetails1',
                data: JSON.stringify({ ManagerId: Managevalue, StatusId: Statvalue, Flag: flagstat }),
                async: false,
                cache: false,
                dataType: "json",
                success: function (data) {
                    // jQuery.unblockUI();
                    var tabledata = [];
                    var tbody = "";
                    var dropdown = "";
                    // var data = [];
                    //for (var i = 0; i < data.d.length; i++) {
                    //    data.push(data.d[i].Status);
                    //}
                    //var table;
                    for (var i = 0; i < data.d.length; i++) {
                        // alert(data.d[i].ID + ' ' + data.d[i].ProjectNumber);

                        if (data.d[i].Status == 1) {
                            dropdown = '<select name="status" id="' + data.d[i].ID + '"  class="ui dropdown abc" onchange="changestatus(this.id,' + data.d[i].Status + ')"><option value="1" selected="selected">Closed</option><option value="2" >Live</option><option value="3">On Hold</option><option value="4">Canceled</option><option value="5">Awarded</option><option value="6">Invoiced</option></select>';
                        }
                        else if (data.d[i].Status == 2) {
                            dropdown = '<select name="status" id="' + data.d[i].ID + '"  class="ui dropdown abc" onchange="changestatus(this.id,' + data.d[i].Status + ')"> <option value="1" >Closed</option><option value="2" selected="selected" >Live</option><option value="3">On Hold</option><option value="4">Canceled</option><option value="5">Awarded</option><option value="6">Invoiced</option></select>';
                        }
                        else if (data.d[i].Status == 3) {
                            dropdown = '<select name="status" id="' + data.d[i].ID + '"  class="ui dropdown abc" onchange="changestatus(this.id,' + data.d[i].Status + ')"><option value="1" >Closed</option><option value="2"  >Live</option><option value="3" selected="selected">On Hold</option><option value="4">Canceled</option><option value="5">Awarded</option><option value="6">Invoiced</option></select>';
                        }
                        else if (data.d[i].Status == 4) {
                            dropdown = '<select name="status" id="' + data.d[i].ID + '"  class="ui dropdown abc" onchange="changestatus(this.id,' + data.d[i].Status + ')"><option value="1" >Closed</option><option value="2"  >Live</option><option value="3">On Hold</option><option value="4" selected="selected">Canceled</option><option value="5">Awarded</option><option value="6">Invoiced</option></select>';
                        }
                        else if (data.d[i].Status == 5) {
                            dropdown = '<select name="status" id="' + data.d[i].ID + '"  class="ui dropdown abc" onchange="changestatus(this.id,' + data.d[i].Status + ')"><option value="1" >Closed</option><option value="2"  >Live</option><option value="3">On Hold</option><option value="4" >Canceled</option><option value="5" selected="selected">Awarded</option><option value="6">Invoiced</option></select>';
                        }
                        else if (data.d[i].Status == 6) {
                            dropdown = '<select name="status" id="' + data.d[i].ID + '"  class="ui dropdown abc" onchange="changestatus(this.id,' + data.d[i].Status + ')"><option value="1" >Closed</option><option value="2"  >Live</option><option value="3">On Hold</option><option value="4" >Canceled</option><option value="5" >Awarded</option><option value="6" selected="selected">Invoiced</option></select>';
                        }
                        else { dropdown = '<select name="status" id="' + data.d[i].ID + '"  class="ui dropdown abc" onchange="changestatus(this.id,' + data.d[i].Status + ')">option value="1" >Closed</option><option value="2"  >Live</option><option value="3">On Hold</option><option value="4" >Canceled</option><option value="5" >Awarded</option><option value="6" >Invoiced</option></select>'; }
                        var style = getFlag(data.d[i].Flag, 'convertCode');
                        tbody += '<tr><td><a href="ProjectPageDetails.aspx?ID=' + data.d[i].ID + '" class="hover_blue">' + data.d[i].ProjectNumber + '</a></td><td>' + data.d[i].PName + '</td><td><a href="ClientPopReport.aspx?ID=' + data.d[i].ClientID + '" class="hover_blue">' + data.d[i].ClientName + '</td><td>' + data.d[i].PManager + '</td><td>' + data.d[i].Country + '</td><td>' + data.d[i].LOI + '</td><td><label class="mw">$ ' + data.d[i].CPI + '</label></td><td><label class="mw">' + data.d[i].IRate + '%</label></td><td>' + dropdown + '</td><td>' + data.d[i].total + '</td><td>' + data.d[i].completed + '</td><td>' + data.d[i].terminate + '</td><td>' + data.d[i].overquota + '</td><td>' + data.d[i].securityterm + '</td><td>' + data.d[i].frauderror + '</td><td>' + data.d[i].incomplete + '</td><td>' + data.d[i].ActIR + '%</td><td>' + data.d[i].ActLOI + '</td><td><span class="fa fa-flag" ' + style + '></span></td></tr>';
                        // tbody += '<tr><td><a href="ProjectPageDetails.aspx?ID=' + data.d[i].ID + '" class="hover_blue">' + data.d[i].ProjectNumber + '_'+data.d[i].PName+'</a></td><td><a href="ClientPopReport.aspx?ID=' + data.d[i].ClientID + '" class="hover_blue">' + data.d[i].ClientName + '</td><td>' + data.d[i].PManager + '</td><td>' + data.d[i].Country + '</td><td>' + data.d[i].LOI + '</td><td>' + data.d[i].CPI + '</td><td>' + data.d[i].IRate + '</td><td>' + data.d[i].EDate + '</td> <td>' + dropdown + '</td><td>' + data.d[i].total + '</td><td>' + data.d[i].completed + '</td><td>' + data.d[i].terminate + '</td><td>' + data.d[i].overquota + '</td><td>' + data.d[i].securityterm + '</td><td>' + data.d[i].frauderror + '</td><td>' + data.d[i].incomplete + '</td></tr>';
                        //$("#myTable1 tbody").append('<tr><td>' + data.d[i].Company + '</td><td>' + data.d[i].Person + '</td><td>' + data.d[i].Number + '</td><td>' + data.d[i].Email + '</td><td>' + data.d[i].Country + '</td></tr>')
                    }


                    if ($.fn.dataTable.isDataTable('#myTable1')) {
                        table.destroy();
                        $("#myTable1 tbody").html(tbody);
                        table = $('#myTable1').DataTable({
                            dom: 'Bfrtip',
                            buttons: [
                                'copy', 'csv', 'excel', 'pdf', 'print'
                            ],
                            "aaSorting": [[0, 'desc']],

                            "aLengthMenu": [
                                [100, 200, 500, 1000, -1],
                                [100, 200, 500, 1000, "All"]
                            ],
                            "iDisplayLength": -1,
                        });
                    }
                    else {
                        $("#myTable1 tbody").html(tbody);
                        table = $('#myTable1').DataTable({
                            dom: 'Bfrtip',
                            buttons: [
                                'copy', 'csv', 'excel', 'pdf', 'print'
                            ],
                            "aaSorting": [[0, 'desc']],

                            "aLengthMenu": [
                                [100, 200, 500, 1000, -1],
                                [100, 200, 500, 1000, "All"]
                            ],
                            "iDisplayLength": -1,
                        });
                    }

                },
                error: function (result) {
                    alert(result.statusText);
                }
            });
        }
        function loadddl() {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                async: false,
                url: 'Dashboard.aspx/GetManager',
                cache: false,
                dataType: "json",
                success: function (data) {
                    if (data.d.length > 0) {
                        $("#ddlManagers").append('<option value="0">All</option>');
                        for (var i = 0; i < data.d.length; i++) {
                            $("#ddlManagers").append('<option value="' + data.d[i].Id + '">' + data.d[i].Manager + '</option>');
                        }
                    }
                    else {
                    }
                    $('#ddlManagers').val(<%=Session["userid"].ToString() %>);
                },
                error: function (result) {
                    alert('error');
                }
            });
        }
    </script>

</body>

</html>
