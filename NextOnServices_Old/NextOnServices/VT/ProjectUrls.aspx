<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProjectUrls.aspx.cs" Inherits="ProjectUrls" EnableEventValidation="false" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
    <link href="../dist/css/CustmAhte.css" rel="stylesheet" />
    <style>
        #btnAdd {
            background-color: #006767;
            padding: 4px 30px 4px 30px;
            color: white !important;
        }

        #myTable1 thead tr th {
            padding: 11px;
        }

        #myTable1 tbody tr td {
            padding: 5px 15px !important;
        }

        #chkTokens {
            position: unset;
            width: 30px;
        }
    </style>
    <script type="text/javascript">

        function validation() {
            var txtSize = document.getElementById('txtUrl');
            var ddlCountry = document.getElementById('ddlCountry');
            var txtCPI = document.getElementById('txtCPI');
            var txtQuota = document.getElementById('txtRQ');
            //var txtUrl = document.getElementById('txtUrl');
            // alert('Asif');
            if (ddlCountry.value == '0') {
                alertt('Please select country', '', '');
                document.getElementById('ddlCountry').focus();
                return false;
            }
            // alert(txtSize.value);
            //var result;
            if (txtSize.value == '') {
                alertt('Project Url cannot be', ' left blank', 'error');
                document.getElementById('txtUrl').focus();
                return false;
            }
            if (txtQuota.value == '') {
                alertt('Quota cannot be ', 'left blank', 'error');
                document.getElementById('txtRQ').focus();
                return false;
            }
            if (txtCPI.value == '') {
                alertt('CPI cannot be ', ' left blank', 'error');
                document.getElementById('txtCPI').focus();
                return false;
            }

            return true;
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
                            <h5 class="txt-dark">Url Mapping</h5>
                        </div>

                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <%--<li><a href="#"><span>Project</span></a></li>--%>
                                <li class="active"><span>Project Mapping</span></li>
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
                                        <asp:Label runat="server" ID="lblid" Visible="false"></asp:Label>
                                        <h5 class="panel-title txt-dark">Project Id : 
                                            <label id="projectid" runat="server" style="font-size: small; font-weight: 500; color: black"></label>
                                        </h5>

                                        <h5 class="panel-title txt-dark">Project Name : 
                                            <label id="lblpname" runat="server" style="font-size: small; font-weight: 500; color: black"></label>
                                        </h5>
                                    </div>

                                    <div class="clearfix"></div>
                                </div>
                                <div class="form-wrap">
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <asp:Label ID="lblmsg" runat="server" Font-Bold="true"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6 col-xs-12">
                                        </div>
                                        <div class="col-sm-2 col-xs-6">
                                            <div class="form-group">
                                                <label class="container1_ppd">
                                                    <input runat="server" type="checkbox" id="chkTokens" onchange="Tokens(this.id)" />Tokens<span class="checkmark_ppd"></span></label>
                                            </div>
                                        </div>
                                        <%--<div class="col-sm-2 col-xs-6">
                                            <div class="form-group">
                                                <label class="container1_ppd">
                                                    <input style="position: unset; width: 30px;" runat="server" type="checkbox" id="chkApplyToSupplier" onchange="ApplyVariable(this.id)" />Add Variable<span class="checkmark_ppd"></span></label>
                                            </div>
                                        </div>
                                        <div class="col-sm-2 col-xs-6 ParameterContent">
                                            <div class="form-group">
                                                <label class="container1_ppd">
                                                    <input style="position: unset; width: 30px;" runat="server" type="checkbox" id="chkApplyToSupplier1" onchange="" />Apply To Supplier<span class="checkmark_ppd"></span></label>
                                            </div>
                                        </div>--%>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <div class="row">
                                                    <div class="col-sm-6">
                                                        <label class="control-label mb-10">Country</label>
                                                    </div>
                                                    <div class="col-sm-6">
                                                        <input type="hidden" id="hfitem" />
                                                        <input type="button" value="Add" class="btn btn-default btn-sm pull-right" id="btnAdd" />
                                                    </div>
                                                </div>

                                                <asp:DropDownList ID="ddlCountry" runat="server" class="ui search selection dropdown form-control" data-style="form-control btn-default btn-outline">
                                                </asp:DropDownList>

                                            </div>
                                        </div>
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Url</label>
                                                <asp:TextBox ID="txtUrl" runat="server" class="form-control" placeholder="Enter Project Url"></asp:TextBox>
                                                <span class="help-block WOT" style="color: orange">https://www.abcdefgh.aspx?RIS=20&RID=[respondentID]</span>
                                                <span class="help-block WT" style="color: orange">https://www.abcdefgh.aspx?RIS=20&RID=[respondentID]&TID=[TokenID]</span>
                                            </div>
                                        </div>
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Respondent Quota</label>
                                                <asp:TextBox ID="txtRQ" runat="server" onkeypress="return isNumberKey(event)" class="form-control" placeholder="Enter Respondent Quota"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">CPI ($)</label>
                                                <asp:TextBox ID="txtCPI" runat="server" onkeypress="return isNumberKey(event)" class="form-control" placeholder="Enter CPI"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%--<div class="col-sm-6 col-xs-12 ParameterContent">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Parameter Name</label>
                                                <asp:TextBox runat="server" CssClass="form-control" ID="txtParameterName"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-sm-6 col-xs-12 ParameterContent">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Parameter Value</label>
                                                <asp:DropDownList runat="server" ID="txtParameterValue" CssClass="form-control">
                                                    <asp:ListItem>--Select--</asp:ListItem>
                                                    <asp:ListItem Value="SHA3">SHA3</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>--%>
                                    </div>
                                    <div class="row WT">
                                        <div class="col-sm-12 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Paste Tokens Below</label>
                                                <asp:TextBox ID="txtTokensWithURL" runat="server" class="form-control" placeholder="Paste your tokens here" TextMode="MultiLine" Rows="4"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Notes</label>
                                                <asp:TextBox ID="txtNotes" runat="server" class="form-control" placeholder="Enter your notes here" TextMode="MultiLine" Rows="4"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <asp:Button ID="btnSubmit" runat="server" class="btn btn-success btn-anim" OnClientClick="return validation();"
                                                    Text="Submit" OnClick="btnSubmit_Click" />
                                                <input type="button" value="Back" class="btn btn-danger" onclick="Back()" />
                                                <%--<button type="button" class="btn btn-success btn-anim"><i class="icon-rocket"></i><span class="btn-text">submit</span></button>--%>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <div class="panel panel-default card-view">
                                            <div class="panel-heading">
                                                <div class="pull-left">
                                                    <h6 class="panel-title txt-dark">Project Url(s) </h6>
                                                    <input id="hfProjectnumber" type="hidden" />
                                                </div>
                                                <div class="clearfix"></div>
                                            </div>
                                            <div class="panel-wrapper collapse in">
                                                <div class="panel-body">
                                                    <div class="table-wrap">
                                                        <div class="" style="overflow-x: scroll">
                                                            <table id="myTable1" class="table table-hover table-responsive display table-custom pb-30">
                                                                <thead>
                                                                    <tr>
                                                                        <th>Project Number</th>
                                                                        <th>Project Name</th>
                                                                        <th>Country</th>
                                                                        <th>Url</th>
                                                                        <th>Respondent Quota</th>
                                                                        <th>CPI</th>
                                                                        <th>Notes</th>
                                                                        <th>Edit</th>
                                                                        <th>Upload Tokens</th>
                                                                        <th>View Tokens</th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody></tbody>
                                                            </table>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <%--###########################################--Modals--###########################################--%>
                                            <div class="modal fade" id="mdlTokansUpload" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                                                <div class="modal-dialog">
                                                    <div class="modal-content">
                                                        <!-- Modal Header -->
                                                        <div class="modal-header">
                                                            <button type="button" class="close" data-dismiss="modal">
                                                                <span aria-hidden="true">&times;</span>
                                                                <span class="sr-only">Close</span>
                                                            </button>
                                                            <h4 class="modal-title" id="myModalLabel">Paste tokens below</h4>
                                                        </div>
                                                        <!-- Modal Body -->
                                                        <div class="modal-body">
                                                            <div class="form-horizontal" role="form" style="padding-right: 15px; padding-left: 15px">
                                                                <div class="form-group">
                                                                    <input type="hidden" id="hiddenID" />
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col-md-12 col-sm-12 col-xs-12">
                                                                        <div class="form-group">
                                                                            <%--<label class="control-label mb-10">Over Quota</label>--%>
                                                                            <textarea id="txtTokens" class="form-control" cols="12" rows="10"></textarea>
                                                                        </div>

                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <!-- Modal Footer -->
                                                        <div class="modal-footer">
                                                            <button type="button" class="btn btn-default" id="btnCloseTokanModal" data-dismiss="modal">
                                                                Close
                                                            </button>
                                                            <input type="button" id="btnSaveTokens" class="btn btn-primary" value="Save" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="modal fade" id="mdlTokansViews" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                                                <div class="modal-dialog">
                                                    <div class="modal-content">
                                                        <!-- Modal Header -->
                                                        <div class="modal-header">
                                                            <button type="button" class="close" data-dismiss="modal">
                                                                <span aria-hidden="true">&times;</span>
                                                                <span class="sr-only">Close</span>
                                                            </button>
                                                            <h4 class="modal-title" id="myModalTokans">Tokens</h4>
                                                        </div>
                                                        <!-- Modal Body -->
                                                        <div class="modal-body">
                                                            <div class="form-horizontal" role="form" style="padding-right: 15px; padding-left: 15px">
                                                                <div class="form-group">
                                                                    <input type="hidden" id="hiddenIDTokens" />
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col-md-12 col-sm-12 col-xs-12">
                                                                        <div style="overflow-x: scroll">
                                                                            <table id="tblTokans" class="table table-hover table-responsive display table-custom pb-30" style="width: 100%">
                                                                                <thead>
                                                                                    <tr>
                                                                                        <th>Sr</th>
                                                                                        <th>Tokans</th>
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
                                                            <button type="button" class="btn btn-default" id="btnCloseTokanViewModal" data-dismiss="modal">
                                                                Close
                                                            </button>
                                                            <input type="button" id="btnSaveTokensView" class="btn btn-primary" value="Save" style="display: none" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <%--###########################################--Modals--###########################################--%>
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



    </form>

    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../Scripts/Dropdown/semantic.js"></script>
    <script type="text/javascript">


        jQuery(document).ready(function () {
            $('#ddlCountry').dropdown();
            //btn to add new country
            $('#btnAdd').hide();
            $('#ddlCountry').parent().children('input').attr('id', 'txtSearch');
            getidfromurl(1, 0)
            Populatetableonload();
            savetokens();
            //  $('#myTable1').DataTable();

            //country add procedure
            ApplyVariable('chkApplyToSupplier');
            $('#txtSearch').keyup(function () {
                $('#btnAdd').hide();
                $('#hfitem').val($('#txtSearch').val());
                var findelement
                if ($('#ddlCountry').parent().children('div').find(".message")[0]) {
                    findelement = $('#ddlCountry').parent().children('div').find(".message")[0].innerHTML;
                }

                if (findelement == "No results found.") {
                    $('#btnAdd').show();
                    //$('#lblCode').hide();
                    //$('#lblMessage').hide();
                }
                else {
                    $('#btnAdd').hide();
                    //$('#lblCode').show();
                    //$('#lblMessage').show();
                }
            });
            $('#btnAdd').click(function () {
                var item = $('#hfitem').val();
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'ProjectUrls.aspx/AddItem',
                    data: JSON.stringify({ ITEM: item }),
                    cache: false,
                    dataType: "json",
                    success: function (data) {

                        if (data.d.length > 0 && parseInt(data.d) > 0) {
                            $('#btnAdd').hide();
                            //$('#lblCode').show();
                            //$('#lblMessage').show();
                            //$('#lblCode').text(data.d);
                            //$('#lblMessage').text('Added- Code is : ');
                            //populatedrodown();
                            // alert(data.d);
                            var query = window.location.search.substring(1);
                            window.location.href = 'ProjectUrls.aspx?' + query;
                        }
                        else {
                        }
                    },
                    error: function () {
                        alert("n");
                    }
                });
            });

            Tokens($('#chkTokens').attr('id'));
        });
        function EDIT(id) {

            //alert(id);

            //$.ajax({
            //    type: "POST",
            //    contentType: "application/json; charset=utf-8",
            //    url: 'ProjectUrls.aspx/EditDetails',
            //    data: JSON.stringify({ Id: id }),
            //    cache: false,
            //    dataType: "json",
            //    success: function (data) {

            //        var tbody = "";
            //        for (var i = 0; i < data.d.length; i++) {
            //            alert(data.d[i].Url);
            //            $("#lblpname").value(data.d[i].PName);
            //            $("#ddlCountry").value = data.d[i].Country;
            //            $("#txt1").text(data.d[i].Url);
            //            $('#lblid').value(data.d[i].ID);

            //        }
            //        alert($("#txt1").value())
            //        $('#btnSubmit').text = "Update";
            //        //$("#myTable1 tbody").html(tbody);
            //        // $("#myTable1").DataTable();
            //        //  $('#example').DataTable();
            //    },
            //    error: function (result) {
            //        alert(result.d);
            //    }
            //});



            var url = "ProjectUrls.aspx?IID=" + id;
            window.location.href = url;



        }
        function Populatetableonload() {

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'ProjectUrls.aspx/GetProjectUrls',
                cache: false,
                dataType: "json",
                success: function (data) {
                    $('#lblpname').text(data.d[0].PName);
                    $('#projectid').text(data.d[0].projectid);
                    var TokenUpload = '';
                    var TokenView = '';
                    var tbody = "";
                    for (var i = 0; i < data.d.length; i++) {
                        if (data.d[i].Token == 0) {
                            TokenUpload = ' ';
                            TokenView = ' ';
                        }
                        else {
                            TokenUpload = '<input onclick="AssignToHF(' + data.d[i].ID + ')" type="button" class="btn btn-success samewidth" value="Upload Tokens" data-toggle="modal" data-target="#mdlTokansUpload" />';
                            TokenView = '<input onclick="ViewTokans(' + data.d[i].ID + ')" type="button" class="btn btn-success samewidth" value="View Tokens" data-toggle="modal" data-target="#mdlTokansViews" />';

                        }
                        tbody = tbody + '<tr><td><a href="ProjectPageDetails.aspx?Id=' + data.d[i].PID + '" class="hover_blue">' + data.d[i].projectid + '</td><td>' + data.d[i].PName + '</td><td>' + data.d[i].Country + '</td><td>' + data.d[i].Url + '</td><td>' + data.d[i].Quota + '</td> <td>' + data.d[i].CPI + '</td> <td>' + data.d[i].Notes + '</td> <td><input type="button" class="btn btn-success samewidth" value="Edit" style="width: 70px !important;" onclick="EDIT(' + data.d[i].ID + ')" /></td><td>' + TokenUpload + '</td><td>' + TokenView + '</td></tr >';
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
        function getidfromurl(pid, urlid) {
            if (pid = 1) {
                var id = [];
                var query = window.location.search.substring(1);
                var vars = query.split("&");
                // for (var i = 0; i < vars.length; i++) {
                pair = vars[0].split("=");
                //alert(pair[1]);
                id = pair[1];
                // }
                if (pair[0] == 'ID') {
                    $('#hfProjectnumber').val(id);
                }
            }

        }

        function Back() {
            var id = [];
            var lab = [];
            var query = window.location.search.substring(1);
            var vars = query.split("&");
            for (var i = 0; i < vars.length; i++) {
                pair = vars[i].split("=");
                //alert(pair[1]);
                id[i] = pair[1];
                lab[i] = pair[0];
            }
            if (lab[0] == 'ID') {
                var url = "ProjectPageDetails.aspx?ID=" + id[0];
                window.location.href = url;
            }
            if (lab[0] == 'IID') {
                var promise = findprojectid(id[0])
                var pid = promise.responseJSON.d;
                var url = "ProjectPageDetails.aspx?ID=" + pid;
                window.location.href = url;
            }

        }
        function findprojectid(id) {

            return $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'ProjectUrls.aspx/findprojectid',
                data: JSON.stringify({ Id: id }),
                async: false,
                cache: false,
                dataType: "json",
            });

        }
        function AssignToHF(id) {
            $('#hiddenID').val(id);
            $('#txtTokens').val('');
        }
        function savetokens() {
            $('#btnSaveTokens').click(function () {
                var text = $('#txtTokens').val();
                var vars = text.split("\n");
                var y = 0;
                for (var i = 0; i < vars.length; i++) {
                    if (vars[i] != '') {
                        saveeachtokens(vars[i]).success(function (data) {
                            if (data.d.length > 0) {
                                if (data.d[0].Message == 'Success') {
                                    y += 1;
                                }
                            }
                        }).error(function (result) {

                        });
                    }
                }
                success(y + ' Tokens saved successfully', 'success');
                $('#btnCloseTokanModal').click();
            });
        }
        function saveeachtokens(token) {
            return $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'ProjectUrls.aspx/SaveTokens',
                data: JSON.stringify({ OPT: '0', PROJECTURLID: $('#hiddenID').val(), TOKEN: token }),
                cache: false,
                async: false,
                dataType: "json"
            });
        }
        function ViewTokans(id) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'ProjectUrls.aspx/SaveTokens',
                data: JSON.stringify({ OPT: '1', PROJECTURLID: id, TOKEN: '0000' }),
                cache: false,
                async: false,
                dataType: "json",
                success: function (data) {
                    if (data.d.length > 0) {
                        var tbody = "";
                        for (var i = 0; i < data.d.length; i++) {
                            tbody = tbody + '<tr><td>' + (i + 1) + '</td><td>' + data.d[i].Token + '</td></tr >';
                        }
                        $("#tblTokans tbody").html(tbody);
                        $("#tblTokans").DataTable();
                    }
                    else
                        $("#tblTokans tbody").html('No Records Found');
                    $("#tblTokans").DataTable();
                },
                error: function (result) {
                }
            });
        }

        function Tokens(id) {
            if ($('#' + id).is(':checked')) {
                $('.WT').show();
                $('.WOT').hide();
                //  $('#btnSubmit').val('Submit and Upload Tokens');
                //  $('#btnSubmit').text('Submit and Upload Tokens');

            }
            else {
                $('.WT').hide();
                $('.WOT').show();
                //   $('#btnSubmit').val('Submit');
            }
        }
        function ApplyVariable(id) {
            if ($('#' + id).is(':checked')) {
                $('.ParameterContent').show();
            }
            else {
                $('.ParameterContent').hide();
            }
        }
    </script>
</body>
</html>
