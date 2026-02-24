<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RecontactProjects.aspx.cs" Inherits="VT_RecontactProjects" %>

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
    <%-- <link href="https://cdn.datatables.net/1.10.16/css/jquery.dataTables.min.css" rel="stylesheet" />
    <link href="https://cdn.datatables.net/buttons/1.5.1/css/buttons.dataTables.min.css" rel="stylesheet" />--%>
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
                        <%--<div class="fourbox">
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
                        </div>--%>
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
                                                                <th class="tooltip1">P Number<span class="tooltiptext1">Project</span></th>
                                                                <th class="tooltip1">PName<span class="tooltiptext1">Project</span></th>

                                                                <th class="tooltip1">CPI<span class="tooltiptext1">Cost per Complete</span></th>
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
    <script src="//cdn.datatables.net/1.10.16/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <%--<script src="scripts/ui/jquery-1.4.2.js"></script>--%>
    <%--<script src="scripts/dropdown/jquery.min.js"></script>--%>
    <script src="../Scripts/Dropdown/semantic.js"></script>
    <script src="../Scripts/DeviceDetection.js"></script>
    <%--0000000000000000000000000000000--%>
    <%-- <script src="https://code.jquery.com/jquery-1.12.4.js"></script>
    <script src="https://cdn.datatables.net/1.10.16/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.5.1/js/dataTables.buttons.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.5.1/js/buttons.flash.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jszip/3.1.3/jszip.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.32/pdfmake.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.32/vfs_fonts.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.5.1/js/buttons.html5.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.5.1/js/buttons.print.min.js"></script>--%>
    <%--0000000000000000000000000000000--%>

    <script type="text/javascript">

        function loadtable() {

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'RecontactProjects.aspx/GetRecontactProjects',
                // data: JSON.stringify({ ManagerId: Managevalue, StatusId: Statvalue, Flag: flagstat }),
                async: false,
                cache: false,
                dataType: "json",
                success: function (data) {
                    var tabledata = [];
                    var tbody = "";
                    var dropdown = "";

                    for (var i = 0; i < data.d.length; i++) {
                        if (data.d[i].Statusint == 1) {
                            dropdown = '<select name="status" id="' + data.d[i].ID + '"  class="ui dropdown abc" onchange="changestatus(this.id,' + data.d[i].Statusint + ')"><option value="1" selected="selected">Closed</option><option value="2" >Live</option><option value="3">On Hold</option><option value="4">Cancelled</option><option value="5">Awarded</option><option value="6">Invoiced</option></select>';
                        }
                        else if (data.d[i].Statusint == 2) {
                            dropdown = '<select name="status" id="' + data.d[i].ID + '"  class="ui dropdown abc" onchange="changestatus(this.id,' + data.d[i].Statusint + ')"> <option value="1" >Closed</option><option value="2" selected="selected" >Live</option><option value="3">On Hold</option><option value="4">Cancelled</option><option value="5">Awarded</option><option value="6">Invoiced</option></select>';
                        }
                        else if (data.d[i].Statusint == 3) {
                            dropdown = '<select name="status" id="' + data.d[i].ID + '"  class="ui dropdown abc" onchange="changestatus(this.id,' + data.d[i].Statusint + ')"><option value="1" >Closed</option><option value="2"  >Live</option><option value="3" selected="selected">On Hold</option><option value="4">Cancelled</option><option value="5">Awarded</option><option value="6">Invoiced</option></select>';
                        }
                        else if (data.d[i].Statusint == 4) {
                            dropdown = '<select name="status" id="' + data.d[i].ID + '"  class="ui dropdown abc" onchange="changestatus(this.id,' + data.d[i].Statusint + ')"><option value="1" >Closed</option><option value="2"  >Live</option><option value="3">On Hold</option><option value="4" selected="selected">Cancelled</option><option value="5">Awarded</option><option value="6">Invoiced</option></select>';
                        }
                        else if (data.d[i].Statusint == 5) {
                            dropdown = '<select name="status" id="' + data.d[i].ID + '"  class="ui dropdown abc" onchange="changestatus(this.id,' + data.d[i].Statusint + ')"><option value="1" >Closed</option><option value="2"  >Live</option><option value="3">On Hold</option><option value="4" >Cancelled</option><option value="5" selected="selected">Awarded</option><option value="6">Invoiced</option></select>';
                        }
                        else if (data.d[i].Statusint == 6) {
                            dropdown = '<select name="status" id="' + data.d[i].ID + '"  class="ui dropdown abc" onchange="changestatus(this.id,' + data.d[i].Statusint + ')"><option value="1" >Closed</option><option value="2"  >Live</option><option value="3">On Hold</option><option value="4" >Cancelled</option><option value="5" >Awarded</option><option value="6" selected="selected">Invoiced</option></select>';
                        }
                        else { dropdown = '<select name="status" id="' + data.d[i].ID + '"  class="ui dropdown abc" onchange="changestatus(this.id,' + data.d[i].Statusint + ')">option value="1" >Closed</option><option value="2"  >Live</option><option value="3">On Hold</option><option value="4" >Cancelled</option><option value="5" >Awarded</option><option value="6" >Invoiced</option></select>'; }

                        // var data = [];
                        //for (var i = 0; i < data.d.length; i++) {
                        //    data.push(data.d[i].Status);
                        //}
                        //var table;

                        tbody += '<tr><td><a href="RecontactPageDetails.aspx?ID=' + data.d[i].ID + '" class="hover_blue">' + data.d[i].PID + '</a></td><td><a href="RecontactPageDetails.aspx?ID=' + data.d[i].ID + '" class="hover_blue">' + data.d[i].RecontactName + '</a></td><td>' + data.d[i].CPI + '</td><td>' + dropdown + '</td><td>' + data.d[i].Total + '</td><td>' + data.d[i].Complete + '</td><td>' + data.d[i].Terminate + '</td><td>' + data.d[i].Overquota + '</td><td>' + data.d[i].S_term + '</td><td>' + data.d[i].F_error + '</td><td>' + data.d[i].Incomplete + '</td></tr>';
                    }


                    if ($.fn.dataTable.isDataTable('#myTable1')) {
                        //table = $('#myTable1').DataTable();
                        table.destroy();
                        $("#myTable1 tbody").html(tbody);
                        table = $('#myTable1').DataTable({
                            "aaSorting": [[0, 'desc']],
                            "iDisplayLength": 50,


                        });
                    }
                    else {
                        $("#myTable1 tbody").html(tbody);
                        table = $('#myTable1').DataTable({
                            "aaSorting": [[0, 'desc']],
                            "iDisplayLength": 50,
                        });
                    }
                    //$("#myTable1").DataTable({
                    //    responsive: true,
                    //    "sDom": 'T<"clear">lfrtip',
                    //    "oTableTools": {
                    //        "sSwfPath": "//cdn.datatables.net/tabletools/2.2.2/swf/copy_csv_xls_pdf.swf",
                    //        "aButtons": ["copy", "csv", "xls", "pdf", "print"]
                    //    },
                    //    "aaSorting": [[0, 'desc']],
                    //    "sPaginationType": "full_numbers",
                    //    bServerSide: false,
                    //    bJQueryUI: true,
                    //    "bDestroy": true,
                    //    "aoColumnDefs": [{
                    //        'bSortable': false,
                    //        'aTargets': [0, 1]
                    //    }],
                    //    "aLengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                    //    "iDisplayLength": 50

                    //});


                },
                error: function (result) {
                    alert(result.statusText);
                }
            });
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
                        url: 'RecontactProjects.aspx/UpdateStatus',
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

        jQuery(document).ready(function () {
            //loadddl();
            //var value = $('#ddlManagers').val();
            loadtable();
        });

    </script>

</body>

</html>
