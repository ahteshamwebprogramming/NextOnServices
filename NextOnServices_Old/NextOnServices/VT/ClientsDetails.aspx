<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ClientsDetails.aspx.cs" Inherits="ClientsDetails" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        #myTable1 thead tr th {
            padding: 11px;
        }

        #myTable1 tbody tr td {
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
                            <h5 class="txt-dark">Clients</h5>
                        </div>
                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <%--<li><a href="#"><span>Clients</span></a></li>--%>
                                <li class="active"><span>Clients List</span></li>
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
                                        <h6 class="panel-title txt-dark">Clients Details</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-wrapper collapse in">
                                    <div class="panel-body">
                                        <div class="table-wrap">
                                            <div class="">
                                                <table id="myTable1" class="table table-hover display  pb-30 table-custom">
                                                    <thead>
                                                        <tr>
                                                            <th>Company Name</th>
                                                            <th>Contact Person</th>
                                                            <th>Contact Number</th>
                                                            <th>Email</th>
                                                            <th>Country</th>
                                                            <th>Edit</th>
                                                            <th>Active (Yes/No)</th>
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
                <!-- Footer -->
                <uc2:Footer ID="Footer1" runat="server" />
                <!-- /Footer -->
            </div>
        </div>
    </form>
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        //$('body').on('focus', ".datepicker", function () {
        //    $(this).datepicker();
        //});
        jQuery(document).ready(function () {
            GetClients();
        });
        //Fetching Clients
        function GetClients() {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'ClientsDetails.aspx/GetClientDetails',
                cache: false,
                dataType: "json",
                success: function (data) {
                    var active = '';
                    var tbody = "";
                    for (var i = 0; i < data.d.length; i++) {
                        if (data.d[i].Active == 1) {
                            active = '<td><input type="button" style="background-color:#01c853;border: solid 1px #01c853;" value="Yes" id="btn' + data.d[i].ID + '" class="btn btn-success samewidth" onclick=Active("' + data.d[i].ID + '") /></td>';
                        }
                        else if (data.d[i].Active == 0) {
                            active = '<td><input type="button" style="background-color:#dd3333;border: solid 1px #dd3333;" value="No" id="btn' + data.d[i].ID + '" class="btn btn-success samewidth" onclick=Active("' + data.d[i].ID + '") /></td>';
                        }
                        tbody += '<tr><td><a href="ClientPopReport.aspx?ID=' + data.d[i].ID + '" class="hover_blue">' + data.d[i].Company + '</a></td><td>' + data.d[i].Person + '</td><td>' + data.d[i].Number + '</td><td>' + data.d[i].Email + '</td><td>' + data.d[i].Country + '</td><td><input type="button" class="btn btn-success samewidth" value="Edit" onclick=EDIT("' + data.d[i].ID + '") /></td>' + active + '</tr>';
                        //$("#myTable1 tbody").append('<tr><td>' + data.d[i].Company + '</td><td>' + data.d[i].Person + '</td><td>' + data.d[i].Number + '</td><td>' + data.d[i].Email + '</td><td>' + data.d[i].Country + '</td></tr>')
                    }
                    $("#myTable1 tbody").html(tbody);
                    $("#myTable1").DataTable();
                    //  $('#example').DataTable();
                },
                error: function (result) {
                    alert(result.d);
                }
            });
        }
        function EDIT(id) {
            var url = "UpdateClient.aspx?ID=" + id;
            window.location.href = url;
        }
        function Active(id) {
            var message;
            var flag;
            var btnid = 'btn' + id;
            if ($('#' + btnid).val() == 'Yes') {
                //$('#' + btnid).val('No');
                message = 'Are you sure you want to deactivate this Client.';
                flag = 0;
            }
            else {
                // $('#' + btnid).val('Yes');
                message = 'Are you sure you want to activate this Client.';
                flag = 1;
            }
            swal({
                title: 'Are you sure?',
                text: message,
                type: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#26d06c',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Proceed!'
            }).then((result) => {
                if (result.value) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: 'ClientsDetails.aspx/Delete',
                        data: JSON.stringify({ Flag: flag, Id: id }),
                        cache: false,
                        dataType: "json",
                        success: function (data) {
                            if (flag == 0) {
                                $('#' + btnid).val('No');
                                if (data.d == 1) {
                                    GetClients();
                                    swal(
                                        'Deactivated!',
                                        'Client has been deactivated.',
                                        'error'
                                    )
                                }
                            }
                            else {
                                $('#' + btnid).val('Yes');
                                GetClients();
                                swal(
                                    'Activated!',
                                    'Client has been activated.',
                                    'error'
                                )
                            }
                        },
                        error: function (result) {
                            alert(result.d);
                        }
                    });
                }
                //  $('#' + id).val(value);
            })
        }
    </script>
</body>
</html>
