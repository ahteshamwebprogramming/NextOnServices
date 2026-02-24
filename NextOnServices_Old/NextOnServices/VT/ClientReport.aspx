<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ClientReport.aspx.cs" Inherits="ClientReport" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Client Report</title>
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
                            <h5 class="txt-dark">Client Report</h5>
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

                                        <div class="col-xs-12">
                                            <asp:Label ID="lblmsg" runat="server" Font-Bold="true"></asp:Label>
                                        </div>

                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Start Date</label>
                                                <%--<asp:TextBox ID="txtSDate" runat="server" class="form-control" placeholder="Enter Start Date"></asp:TextBox>--%>
                                                <input type="date" id="txtSDate" name="txtSDate" class="form-control" />

                                            </div>
                                        </div>
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">End Date</label>
                                                <input type="date" id="txtEDate" name="txtEDate" class="form-control" />
                                                <%--  <asp:TextBox ID="txtEDate" runat="server" class="form-control" placeholder="Enter End Date"></asp:TextBox>
                                                --%>
                                            </div>
                                        </div>
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Country</label>
                                                <select id="ddlCountry" class="form-control" name="ddlCountry"></select>
                                                <%-- <asp:DropDownList ID="ddlCountry" runat="server"  class="form-control" data-style="form-control btn-default btn-outline">

                                                    </asp:DropDownList>--%>
                                            </div>
                                        </div>
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Project Status</label>
                                                <select id="ddlstatus" class="form-control" name="ddlstatus">
                                                    <option value="S">Status</option>
                                                    <option value="R">Respondents</option>

                                                </select>
                                                <%-- <asp:DropDownList ID="ddlstatus" runat="server"  class="form-control" 
                                                    data-style="form-control btn-default btn-outline" AutoPostBack="True" 
                                                    OnSelectedIndexChanged="ddlstatus_SelectedIndexChanged">
                                                        <asp:ListItem Selected="True" Value="S">Project Status</asp:ListItem>
                                                        <asp:ListItem Value="R">Respondents</asp:ListItem>
                                                        
                                                    </asp:DropDownList>--%>
                                            </div>
                                        </div>
                                        <div class="clearfix"></div>

                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <%-- <asp:Button ID="btnSubmit" runat="server" class="btn btn-success btn-anim" OnClientClick="return validation();"
                                                    Text="Submit"  />--%>
                                                <button type="button" class="btn btn-success btn-anim" id="btnSubmit" name="btnSubmit">submit</button>
                                            </div>
                                        </div>
                                    </div>





                                </div>
                                <div class="panel-wrapper collapse in" id="PStatus">
                                    <div class="panel-body">
                                        <div class="table-wrap">
                                            <div class="">
                                                <table id="myTable1" class="table table-hover display  pb-30">
                                                    <thead>
                                                        <tr>
                                                            <th></th>
                                                            <th>Total</th>
                                                            <th>Closed</th>
                                                            <%-- <th>Sample Size</th>--%>
                                                            <th>Inprogress</th>
                                                            <th>On hold</th>
                                                            <th>Cancelled</th>

                                                        </tr>
                                                    </thead>

                                                    <tbody></tbody>


                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="panel-wrapper collapse in" id="Respodents" style="display: none">
                                    <div class="panel-body">
                                        <div class="table-wrap">
                                            <div class="">
                                                <table id="tblrespondents" class="table table-hover display  pb-30">
                                                    <thead>
                                                        <tr>
                                                            <th></th>
                                                            <th>Total</th>
                                                            <th>Completes</th>
                                                            <%-- <th>Sample Size</th>--%>
                                                            <th>Incompletes</th>
                                                            <th>Screened</th>
                                                            <th>Quotafull</th>

                                                        </tr>
                                                    </thead>

                                                    <tbody>
                                                        <h1>Page Under Construction</h1>
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
                <!-- Footer -->
                <uc2:Footer ID="Footer1" runat="server" />
                <!-- /Footer -->

            </div>

        </div>

    </form>
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js" type="text/javascript"></script>

    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js" type="text/javascript"></script>
    <link rel="stylesheet" href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.5/themes/base/jquery-ui.css" type="text/css" media="all" />
    <link rel="stylesheet" href="http://static.jquery.com/ui/css/demo-docs-theme/ui.theme.css" type="text/css" media="all" />

    <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.5/jquery-ui.min.js" type="text/javascript"></script>
    <script type="text/javascript">




        //$('body').on('focus', ".datepicker", function () {
        //    $(this).datepicker();
        //});


        jQuery(document).ready(function () {

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
                        $("#ddlCountry").append('<option value="0">--Select Country--</option>');
                        for (var i = 0; i < data.d.length; i++) {


                            $("#ddlCountry").append('<option value="' + data.d[i].ID + '">' + data.d[i].Country + '</option>');
                        }
                    }
                    else {
                    }
                },
                error: function (result) {
                    alert(result.d);
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

            $("#btnSubmit").click(function () {
            });
            //$.ajax({
            //    type: "POST",
            //    contentType: "application/json; charset=utf-8",
            //    url: 'ProjectDetails.aspx/GetProjectDetails',
            //    cache: false,
            //    dataType: "json",
            //    success: function (data) {
            //        var tbody = "";
            //        tbody += '<tr><td>Project</td><td>46</td><td>25</td><td>10</td><td>5</td><td>6</td></tr>';

            //        //for (var i = 0; i < data.d.length; i++) {

            //        //    tbody += '<tr><td>' + data.d[i].PName + '</td><td>' + data.d[i].ClientName + '</td><td>' + data.d[i].PManager + '</td><td>' + data.d[i].LOI + '</td><td>' + data.d[i].SDate + '</td><td>' + data.d[i].EDate + '</td></tr>';
            //        //    //$("#myTable1 tbody").append('<tr><td>' + data.d[i].Company + '</td><td>' + data.d[i].Person + '</td><td>' + data.d[i].Number + '</td><td>' + data.d[i].Email + '</td><td>' + data.d[i].Country + '</td></tr>')
            //        //}
            //        $("#myTable1 tbody").html(tbody);
            //        $("#myTable1").DataTable();
            //    },
            //    error: function (result) {
            //        alert(result.d);
            //    }
            //});




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

    <%-- <script type="text/javascript">
        $(function () {


            $('#txtSDate').datepicker(
             {
                 dateFormat: 'dd/mm/yy',
                 changeMonth: true,
                 changeYear: true,
                 yearRange: '1950:2100'

             });





            $('#txtEDate').datepicker(
             {
                 dateFormat: 'dd/mm/yy',
                 changeMonth: true,
                 changeYear: true,
                 yearRange: '1950:2100'

             });



        });
    </script>--%>
</body>
</html>
