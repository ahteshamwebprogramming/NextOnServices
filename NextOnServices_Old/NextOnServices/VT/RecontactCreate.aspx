<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RecontactCreate.aspx.cs" EnableEventValidation="false" Inherits="VT_RecontactCreate" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
    <title></title>
    <link href="../dist/css/CustmAhte.css" rel="stylesheet" />
    <%--0000000000000000000000000000000000000000000000000000000000000--%>
    <link href="https://cdn.datatables.net/1.10.16/css/jquery.dataTables.min.css" rel="stylesheet" />
    <link href="https://cdn.datatables.net/buttons/1.5.1/css/buttons.dataTables.min.css" rel="stylesheet" />
    <%--0000000000000000000000000000000000000000000000000000000000000--%>
    <style>
        #ProjectRpt thead tr th {
            padding: 11px;
        }

        #ProjectRpt tbody tr td {
            padding: 5px 15px !important;
        }

        .cust_w_modal {
            width: 1100px;
            margin: 30px auto;
        }
    </style>
    <script type="text/javascript">
        function validation() {
            var txtSLink = document.getElementById('txtSLink');
            var ddlCountry = document.getElementById('ddlCountry');
            var ddlPN = document.getElementById('ddlPN');
            var ddlsupplier = document.getElementById('ddlsupplier');
            var txtrespondent = document.getElementById('txtRCQ');
            var txtCPI = document.getElementById('txtCPI');

            var txtRecontactName = document.getElementById('txtRecontactName');
            var txtRecontactDescription = document.getElementById('txtRecontactDescription');
            var ddlStatus = document.getElementById('ddlStatus');
            var txtLOI = document.getElementById('txtLOI');
            var txtIR = document.getElementById('txtIR');
            var txtCPI = document.getElementById('txtCPI');
            var txtRCQ = document.getElementById('txtRCQ');
            var ddlVarSelection = document.getElementById('ddlVarSelection');

            var result;

            if (ddlsupplier.value == '0') {
                alertt('', 'Please select Supplier', 'error');
                document.getElementById('ddlsupplier').focus();
                return false;
            }
            // alert(txtrespondent.value);
            if (txtrespondent.value == '') {
                alertt('Respondant Quota', 'Field cannot left blank', 'error');
                document.getElementById('txtRCQ').focus();
                return false;
            }
            return true;
        }

        function confirmDelete() {
            return confirm('Are you sure you want to delete?');
        }
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
        function echeck(str) {
            var filter = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i
            if (filter.test(str))
                testresults = true;
            else {
                testresults = false;
            }
            return (testresults);
        }
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode; if (charCode != 46 && charCode != 45 && charCode > 31 && (charCode < 48 || charCode > 57)) { return false; }
            return true;
        };

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
                <div class="container-fluid">

                    <!-- Title -->
                    <div class="row heading-bg">
                        <div class="col-lg-3 col-md-4 col-sm-4 col-xs-12">
                            <h5 class="txt-dark">Recontact</h5>
                        </div>
                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <li><a href="ProjectDetails.aspx"><span>Projects</span></a></li>
                                <li class="active"><span>Recontact</span></li>
                            </ol>
                        </div>
                        <!-- /Breadcrumb -->
                    </div>
                    <!-- /Title -->
                    <!-- Row -->
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Recontact</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="form-wrap">
                                    <div class="row">
                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <asp:Label ID="lblmsg" runat="server" Font-Bold="true"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-4 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Recontact Name</label>
                                                <input type="text" class="form-control" id="txtRecontactName" />
                                            </div>
                                        </div>
                                        <div class="col-sm-4 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Recontact Description</label>
                                                <input type="text" class="form-control" id="txtRecontactDescription" />
                                            </div>
                                        </div>
                                        <div class="col-sm-4 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Project Status</label>
                                                <select class="form-control" id="ddlStatus">
                                                    <option value="1">Closed</option>
                                                    <option value="2">Live</option>
                                                    <option value="3">On hold</option>
                                                    <option value="4">Cancelled</option>
                                                    <option value="5" selected="selected">Awarded</option>
                                                    <option value="6">Invoiced</option>
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-sm-4 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">LOI (Mins)</label>
                                                <input type="text" id="txtLOI" class="form-control" onkeypress="return isNumberKey(event)" />
                                            </div>
                                        </div>
                                        <div class="col-sm-4 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Incident Rate (%)</label>
                                                <input type="text" id="txtIR" class="form-control" onkeypress="return isNumberKey(event)" />
                                            </div>
                                        </div>
                                        <div class="col-sm-4 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">CPI ($)</label>
                                                <input type="text" id="txtCPI" class="form-control" onkeypress="return isNumberKey(event)" />
                                            </div>
                                        </div>
                                        <div class="col-sm-4 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Respondent Quota</label>
                                                <input type="text" id="txtRCQ" class="form-control" onkeypress="return isNumberKey(event)" />
                                            </div>
                                        </div>
                                        <div class="col-sm-4 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Select No of Variables</label>
                                                <select id="ddlVarSelection" class="form-control ui dropdown">
                                                    <option value="0">select</option>
                                                    <option value="1">1</option>
                                                    <option value="2">2</option>
                                                    <option value="3">3</option>
                                                    <option value="4">4</option>
                                                    <option value="5">5</option>
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="Varibales_div">
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-4 col-xs-12">
                                            <div class="row" style="margin-left: 0px; margin-right: 0px">
                                                <div class="col-md-12">
                                                    <div class="row">
                                                        <div class="form-group">
                                                            <label class="control-label mb-10">Upload CSV</label>
                                                            <input type="file" id="csvRecontacts" class="form-control" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <asp:Button runat="server" ID="btnExampleCsv" CssClass="btn btn-sm btn-default" OnClick="btnExampleCsv_Click" Text="Example File" />
                                                    </div>
                                                </div>

                                            </div>

                                        </div>
                                        <div class="col-sm-8 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Notes</label>
                                                <textarea id="txtNotes" class="form-control" placeholder="Enter your notes here" cols="4" rows="4"></textarea>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <input type="button" id="btnSubmit" class="btn btn-success btn-anim" value="Submit" />
                                                <%--   <input type="button" value="Back" id="btnBack" class="btn btn-danger" />--%>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <div class="panel-wrapper collapse in">
                                                <div class="panel-body">
                                                    <div class="table-wrap">
                                                        <div class="" style="overflow-x: scroll">
                                                            <table id="ProjectRpt" class="table table-hover display  pb-30">
                                                                <thead>
                                                                    <tr>
                                                                        <th>RPID</th>
                                                                        <th>
                                                                            <label>Recontact Name</label></th>
                                                                        <th>CPI</th>
                                                                        <th>SID</th>
                                                                        <th>Vendor Links</th>
                                                                        <th>Notes</th>
                                                                        <th>View</th>
                                                                        <th>Delete</th>
                                                                        <th>Add More</th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody></tbody>
                                                            </table>
                                                            <asp:GridView runat="server" ID="gdvProjectDownloadDetails">
                                                            </asp:GridView>
                                                            <asp:HiddenField runat="server" ID="hfPID" />
                                                        </div>
                                                        <%--Modal--%>
                                                        <%--Modal to view UIDS--%>
                                                        <div class="modal fade" id="mdlRecontacts" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                                                            <div class="modal-dialog cust_w_modal">
                                                                <div class="modal-content">
                                                                    <!-- Modal Header -->
                                                                    <div class="modal-header">
                                                                        <button type="button" class="close" data-dismiss="modal">
                                                                            <span aria-hidden="true">&times;</span>
                                                                            <span class="sr-only">Close</span>
                                                                        </button>
                                                                        <h4 class="modal-title" id="myModalTokans">Recontacts</h4>
                                                                    </div>
                                                                    <!-- Modal Body -->
                                                                    <div class="modal-body">
                                                                        <div class="form-horizontal" role="form">
                                                                            <div class="form-group">
                                                                                <input type="hidden" id="hiddenIDTokens" />
                                                                            </div>
                                                                            <div class="row">
                                                                                <div class="col-md-12 col-sm-12 col-xs-12">
                                                                                    <div style="overflow-x: scroll">
                                                                                        <table id="tblRecontactsForDownload" class="table table-hover table-responsive ">
                                                                                            <thead>
                                                                                                <tr>
                                                                                                    <th>UID</th>
                                                                                                    <th>Client Links</th>
                                                                                                    <th>Vendor Links</th>
                                                                                                    <th>PMID</th>
                                                                                                    <th>Used</th>
                                                                                                </tr>
                                                                                            </thead>
                                                                                            <tbody></tbody>
                                                                                        </table>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <!-- Modal Footer -->
                                                                    <div class="modal-footer">
                                                                        <button type="button" class="btn btn-default" id="btnCloseRecontactViewModal" data-dismiss="modal">
                                                                            Close
                                                                        </button>
                                                                        <%--<input type="button" id="btnSaveTokensView" class="btn btn-primary" value="Save" style="display: none" />--%>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <input type="button" id="displayRecontactDownloadmodal" style="display: none" data-toggle="modal" data-target="#mdlRecontacts" />
                                                        <%--Modal to view UIDS end--%>
                                                        <%-- Modal to delete UIDS--%>
                                                        <div class="modal fade" id="mdlDeleteRecontact" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                                                            <div class="modal-dialog cust_w_modal">
                                                                <div class="modal-content">
                                                                    <!-- Modal Header -->
                                                                    <div class="modal-header">
                                                                        <button type="button" class="close" data-dismiss="modal">
                                                                            <span aria-hidden="true">&times;</span>
                                                                            <span class="sr-only">Close</span>
                                                                        </button>
                                                                        <h4 class="modal-title" id="mymodalrecontact">Recontacts</h4>
                                                                    </div>
                                                                    <!-- Modal Body -->
                                                                    <div class="modal-body">
                                                                        <div class="form-horizontal" role="form">
                                                                            <div class="form-group">
                                                                                <input type="hidden" id="hfid" />
                                                                                <input type="hidden" id="hfoperation" />
                                                                            </div>
                                                                            <div class="row">
                                                                                <div class="col-md-12 col-sm-12 col-xs-12">
                                                                                    <label for="uploaderdeleteduid" class="label-control">Upload csv</label>
                                                                                    <input type="file" id="uploaderdeleteduid" class="form-control" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <!-- Modal Footer -->
                                                                    <div class="modal-footer">
                                                                        <button type="button" class="btn btn-default" id="btnCloseRecontactDeleteModal" data-dismiss="modal">
                                                                            Close
                                                                        </button>
                                                                        <input type="button" id="btnDeleteUIDs" onclick="DeleteAndMoreModalbtn()" class="btn btn-primary" value="OK" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <input type="button" id="displayRecontactDeletemodal" style="display: none" data-toggle="modal" data-target="#mdlDeleteRecontact" />
                                                        <%--Modal to delete UIDS end--%>
                                                        <%--Modal end--%>
                                                        <%-- <asp:Button runat="server" ID="btntest" CommandName="abc" CommandArgument="abcd" Text="Test" OnCommand="btntest_Click" />
                                                        <asp:Button runat="server" ID="Button1" CommandName="a" Text="Test1" CommandArgument="a" OnCommand="btntest_Click" />--%>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <br />
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Footer -->
                <uc2:Footer ID="Footer1" runat="server" />
                <!-- /Footer -->
            </div>
            <input type="hidden" runat="server" id="OrgUrl" />
        </div>
    </form>
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../Scripts/Dropdown/semantic.js"></script>




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
        var RecontactProjectTable;
        jQuery(document).ready(function () {
            //New Part Start
            LoadProjects();
            // LoadRecontacts(0);
            $('#ddlPN').change(function () {
                LoadCountries(0);
                //$('#ddlC').change();
            });
            $('#ddlC').change(function () {
                LoadCountries(1);
            });
            $('#ddlS').change(function () {
                LoadRecontacts(0);
            });

            $('#btnSubmit').click(function () {
                $('#btnSubmit').attr('disabled', 'disabled');
                SaveData();
            });
            //New Part End
            LoadRecontacts(2);
            //$('#ddlPN').dropdown();
            //$('#ddlCountry').dropdown();
            //$('#ddlsupplier').dropdown();
            $('#ddlVarSelection').change(function () {
                var value = $('#ddlVarSelection').val();
                VariablesTextBoxShow(value);
            });
        });
        function VariablesTextBoxShow(value) {
            $('#Varibales_div').empty();
            var divcontent = '<div class="row">';
            for (var i = 0; i < value; i++) {
                var j = i + 1;
                divcontent = divcontent + '<div class="col-sm-4 col--xs-12"><div class="form-group"><label class="control-label mb-10">Variable ' + j + '</label><input type="text" class="form-control" id="txtVar' + j + '" /> </div> </div>';
            }
            divcontent = divcontent + '</div>';
            $('#Varibales_div').append(divcontent);
        }
        //New Part Start
        function LoadRecontacts(opt) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'RecontactCreate.aspx/LoadRecontactCountries',
                cache: false,
                data: JSON.stringify({ PID: '0', CID: '0', SID: '0', OPT: opt }),
                dataType: "json",
                success: function (data) {
                    var tbody = "";
                    $("#ProjectRpt tbody").empty();
                    for (var i = 0; i < data.d.length; i++) {
                        tbody += '<tr><td>' + data.d[i].PID + '</td><td>' + data.d[i].RecontactName + '</td><td>' + data.d[i].CPI + '</td><td>' + data.d[i].SID + '</td><td>' + data.d[i].MURL + '</td><td>' + data.d[i].Notes + '</td><td> <input type="button" class="btn btn-success samewidth clr2" value="View Data" onclick="btnDownloadData(' + data.d[i].ID + ')" /></td><td><input type="button" class="btn btn-success samewidth" onclick="popupDeleteandAddmoreModal(this.id,0)" id="' + data.d[i].ID + '" value="Delete" /></td><td><input type="button" class="btn btn-success samewidth" onclick="popupDeleteandAddmoreModal(this.id,1)" id="' + data.d[i].ID + '" value="Add More" /></td></tr>';
                    }
                    $("#ProjectRpt tbody").html(tbody);
                    if ($.fn.dataTable.isDataTable('#ProjectRpt')) {
                        RecontactProjectTable.destroy();
                        $("#ProjectRpt tbody").html(tbody);
                        RecontactProjectTable = $('#ProjectRpt').DataTable({
                            "aaSorting": [[0, 'desc']],
                            "iDisplayLength": 50,
                        });
                    }
                    else {
                        $("#ProjectRpt tbody").html(tbody);
                        RecontactProjectTable = $('#ProjectRpt').DataTable({
                            "aaSorting": [[0, 'desc']],
                            "iDisplayLength": 50,
                        });
                    }
                    //$("#ProjectRpt").DataTable({
                    //    "aaSorting": [[0, 'desc']]
                    //});

                },
                error: function (result) {
                    alert(result.d);
                }
            });

        }
        function LoadProjects() {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'RecontactCreate.aspx/GetProjectsDetails',
                cache: false,
                dataType: "json",
                success: function (data) {
                    $("#ddlPN").append('<option value="0">--Select Project--</option>');
                    if (data.d.length > 0) {
                        // $(".mapclass").append('<option value="0">--Search Country--</option>');
                        for (var i = 0; i < data.d.length; i++) {
                            $("#ddlPN").append('<option value="' + data.d[i].ID + '">' + data.d[i].PName + '</option>');
                        }
                        // $('#ddlPN').dropdown();
                    }
                    else {
                    }
                },
                error: function (result) {
                    alert(result.d);
                }
            });
        }

        function LoadCountries(opt) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'RecontactCreate.aspx/GetCountries',
                cache: false,
                data: JSON.stringify({ PID: $("#ddlPN").val(), CID: $("#ddlC").val(), SID: '', RCQ: '', CPI: '', Notes: '', OPT: opt }),
                dataType: "json",
                success: function (data) {
                    var id;
                    if (opt == 0) {
                        id = 'ddlC';
                    }
                    else if (opt == 1) {
                        id = 'ddlS';
                    }
                    $("#" + id + "").empty();
                    $("#" + id + "").append('<option value="0">--Select--</option>');
                    if (data.d.length > 0) {
                        for (var i = 0; i < data.d.length; i++) {
                            $("#" + id + "").append('<option value="' + data.d[i].ID + '">' + data.d[i].PName + '</option>');
                        }
                        $('#' + id).dropdown();
                        // LoadRecontacts(0);
                    }
                    else {
                    }
                },
                error: function (result) {
                    alert(result.d);
                }
            });
        }

        function SaveData() {
            var regex = /^([a-zA-Z0-9\s_\\.\-:])+(.csv)$/;
            var recontactname = $('#txtRecontactName').val();
            //var countryid = $('#ddlC').val();
            var supplierid = $('#ddlS').val();
            var cpi = $('#txtCPI').val();
            var notes = $('#txtNotes').val();
            // var respondantquota = $('#txtRCQ').val();
            var formData = {};
            var VarCount = $('#ddlVarSelection').val();
            for (var j = 1; j <= VarCount; j++) {
                formData['Var' + j] = $('#txtVar' + j).val();
            }
            if (ValidateKeywords(VarCount, formData) == false) {
                var keywords = '<%=ConfigurationSettings.AppSettings["ReservedKeyWords"].ToString()%>';
                //keywords = keywords.replace(',', '\n')

                alertt('Dont use these keywords as variables', keywords, 'error');
                $('#btnSubmit').removeAttr('disabled');
                return;
            }
            else {
                formData['recontactname'] = recontactname;
                formData['cpi'] = cpi;
                formData['notes'] = notes;
                formData['NoOfVars'] = VarCount;
                formData['Recontact_Description'] = $('#txtRecontactDescription').val();
                formData['Status'] = $('#ddlStatus').val();
                formData['LOI'] = $('#txtLOI').val();
                formData['IR'] = $('#txtIR').val();
                formData['RCQ'] = $('#txtRCQ').val();
                if (regex.test($("#csvRecontacts").val().toLowerCase())) {
                    if (typeof (FileReader) != "undefined") {
                        var reader = new FileReader();
                        reader.onload = function (e) {
                            var mystr = e.target.result;
                            var rows = mystr.substring(mystr.indexOf('\n') + 1);
                            formData['exceldata'] = rows;
                            formData['opt'] = 0;
                            var strdata = JSON.stringify(formData);
                            $.ajax({
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                url: 'RecontactCreate.aspx/SaveData',
                                data: JSON.stringify({ dataa: strdata }),
                                //async: false,
                                cache: false,
                                dataType: "json",
                                success: function (data) {
                                    if (data.d.length > 0) {
                                        for (var i = 0; i <= data.d.length; i++) {
                                            if (data.d[i].ErrorReturnDBStatus != "1") {
                                                success(data.d[i].ErrorReturnDBMessage, 'error');
                                            }
                                            else {
                                                success('Some UIDs are failed', 'error');
                                            }
                                        }

                                    }
                                    else {
                                        success('Success', 'success');
                                        // window.location.href = "RecontactCreate.aspx";
                                        LoadRecontacts(0);
                                        $("#csvRecontacts").val('');
                                        $('#txtRecontactName').val('');
                                        $('#txtCPI').val('');
                                        $('#ddlVarSelection').val(0);
                                        $('#txtNotes').val('');
                                        $('#Varibales_div').empty();
                                        $('#txtRecontactDescription').val('');
                                        $('#ddlStatus').val(0);
                                        $('#txtLOI').val('');
                                        $('#txtIR').val('');
                                        $('#txtRCQ').val('');
                                    }
                                    LoadRecontacts(2);
                                    $('#btnSubmit').attr('disabled', 'disabled');
                                },
                                error: function (result) {
                                    return "error";
                                    $('#btnSubmit').removeAttr('disabled');
                                }
                            });
                        }
                        reader.readAsText($("#csvRecontacts")[0].files[0]);
                    }
                }
                else {
                    alertt('', 'Document you uploaded is invalid. Please upload .csv', 'error')
                }
                //$.ajax({
                //    type: "POST",
                //    contentType: "application/json; charset=utf-8",
                //    url: 'RecontactCreate.aspx/SaveData',
                //    data: JSON.stringify({ PID: projectid, CID: countryid, SID: supplierid, RCQ: "0", CPI: cpi, Notes: notes, IDs: ids, OPT: 2 }),
                //    cache: false,
                //    dataType: "json",
                //    success: function (data) {
                //        success('Success', 'success');
                //        LoadRecontacts(0);
                //        $('#txtUIDSWithURL').val('');
                //        $('#ddlPN').val(0);
                //        $('#ddlC').val(0);
                //        $('#ddlS').val(0);
                //        $('#txtRCQ').val('');
                //        $('#txtCPI').val('');
                //        $('#txtNotes').val('');
                //        //$('#txtcomplete').val(data.d[0].Completes);
                //        //$('#txtTerminate').val(data.d[0].Terminate);
                //        //$('#txtOverquota').val(data.d[0].Overquota);
                //        //$('#txtSecurity').val(data.d[0].Security);
                //        //$('#txtFraud').val(data.d[0].Fraud);
                //    },
                //    error: function (result) {
                //        (result.d);
                //    }
                //});
            }
        }

        //New Part End

        function getQuerystringID() {
            var id = 0;
            var query = window.location.search.substring(1);
            var vars = query.split("&");
            for (var i = 0; i < vars.length; i++) {
                pair = vars[i].split("=");
                //alert(pair[1]);
                id = pair[1];
            }
            return id;
        }
        function popupDeleteandAddmoreModal(id, opt) {
            $("#uploaderdeleteduid").val('');
            $('#displayRecontactDeletemodal').click();
            $('#hfid').val(id);
            $('#hfoperation').val(opt);
        }
        function DeleteAndMoreModalbtn() {
            var regex = /^([a-zA-Z0-9\s_\\.\-:])+(.csv)$/;
            if (regex.test($("#uploaderdeleteduid").val().toLowerCase())) {
                if (typeof (FileReader) != "undefined") {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        var mystr = e.target.result;
                        var rows = mystr.substring(mystr.indexOf('\n') + 1);
                        if ($('#hfoperation').val() == 0) {
                            deleteUIDWise(rows, $('#hfid').val())
                        }
                        else if ($('#hfoperation').val() == 1) {
                            Addmore(rows, $('#hfid').val());
                        }
                    }
                    reader.readAsText($("#uploaderdeleteduid")[0].files[0]);
                }
            }

            function deleteUIDWise(rows, id) {
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'RecontactCreate.aspx/DeleteData1',
                    data: JSON.stringify({ ROWS: rows, ID: id }),
                    //async: false,
                    cache: false,
                    dataType: "json",
                    success: function (data) {

                        if (data.d == 0) {
                            success('Deleted successfully', 'success');
                            LoadRecontacts(2);
                        }
                        else if (data.d > 0) {
                            success('Unable to delete ' + data.d + ' records', 'error');
                        }
                        else {
                            success('Error', 'error');
                        }
                    },
                    error: function (result) {
                        return "error";
                    }
                });

            }
            function Addmore(rows, reconid) {
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'RecontactCreate.aspx/AddMore',
                    data: JSON.stringify({ ROWS: rows, ID: reconid }),
                    //async: false,
                    cache: false,
                    dataType: "json",
                    success: function (data) {
                        if (data.d == 0) {
                            success('Updated successfully', 'success');
                            LoadRecontacts(2);
                        }
                        else if (data.d > 0) {
                            success('Unable to update ' + data.d + ' records', 'error');
                        }
                        else {
                            success('Error', 'error');
                        }
                    },
                    error: function (result) {
                        return "error";
                    }
                });
            }
            //$.ajax({
            //    type: "POST",
            //    contentType: "application/json; charset=utf-8",
            //    url: 'RecontactCreate.aspx/DeleteData',
            //    data: JSON.stringify({ ID: id }),
            //    //async: false,
            //    cache: false,
            //    dataType: "json",
            //    success: function (data) {
            //        if (data.d == '1') {
            //            success('Deleted successfully', 'success');
            //            LoadRecontacts(0);
            //        }
            //        else if (data.d == '0') {
            //            success('Unable to delete', 'error');
            //        }
            //        else {
            //            success('Error', 'error');
            //        }
            //    },
            //    error: function (result) {
            //        return "error";
            //    }
            //});
        }
        var tablefordownload;
        function btnDownloadData(id) {
            $('#displayRecontactDownloadmodal').click();
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'RecontactCreate.aspx/DownloadData1',
                data: JSON.stringify({ ID: id }),
                //async: false,
                cache: false,
                dataType: "json",
                success: function (data) {
                    var tbody = "";
                    $("#tblRecontactsForDownload tbody").empty();
                    for (var i = 0; i < data.d.length; i++) {
                        tbody += '<tr><td>' + data.d[i].UID + '</a></td><td>' + data.d[i].RedirectLink + '</td><td>' + data.d[i].MaskingURL + '</td><td>' + data.d[i].PMID + '</td><td>' + data.d[i].IsUsed + '</td></tr>';
                    }
                    // $("#tblRecontactsForDownload tbody").html(tbody);
                    // $("#tblRecontactsForDownload").DataTable();
                    if ($.fn.dataTable.isDataTable('#tblRecontactsForDownload')) {
                        //table = $('#myTable1').DataTable();
                        tablefordownload.destroy();
                        $("#tblRecontactsForDownload tbody").html(tbody);
                        tablefordownload = $('#tblRecontactsForDownload').DataTable({
                            "iDisplayLength": 50,
                            dom: 'Bfrtip',
                            "aaSorting": [[1, 'desc']],
                            buttons: [
                                'copy', 'csv', 'excel', 'pdf', 'print'
                            ]
                        });
                    }
                    else {
                        $("#tblRecontactsForDownload tbody").html(tbody);
                        tablefordownload = $('#tblRecontactsForDownload').DataTable({
                            "iDisplayLength": 50,
                            dom: 'Bfrtip',
                            "aaSorting": [[1, 'desc']],
                            buttons: [
                                'copy', 'csv', 'excel', 'pdf', 'print'
                            ]
                        });
                    }
                },
                error: function (result) {
                    return "error";
                }
            });
        }

        function ValidateKeywords(Varcount, formData) {
            var keyword = '<%=ConfigurationSettings.AppSettings["ReservedKeyWords"].ToString() %>'

            var keywords = keyword.split(',');
            for (var j = 1; j <= Varcount; j++) {
                var index = keywords.indexOf(formData['Var' + j]);
                if (index > -1) {
                    return false
                }
            }
        }

    </script>


</body>
</html>

