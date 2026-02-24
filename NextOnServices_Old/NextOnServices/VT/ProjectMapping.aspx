<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProjectMapping.aspx.cs" EnableEventValidation="false" Inherits="ProjectMapping" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
    <title></title>
    <link href="../dist/css/CustmAhte.css" rel="stylesheet" />
    <style>
        #ProjectRpt thead tr th {
            padding: 11px;
        }

        #ProjectRpt tbody tr td {
            padding: 5px 15px !important;
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

            //alert('Asif');
            // alert(txtrespondent.value);
            var result;


            if (ddlPN.value == '0') {
                alertt('', 'Please select Project', 'error');
                document.getElementById('ddlPN').focus();
                return false;
            }

            if (ddlCountry.value == '0') {
                alertt('', 'Please select country', 'error');
                document.getElementById('ddlCountry').focus();
                return false;
            }

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
            if (txtCPI.value == '') {
                alertt('CPI', 'cannot be left blank', 'error');
                document.getElementById('txtCPI').focus();
                return false;
            }
            if (txtSLink.value == '') {
                alertt('', 'Link cannot left blank', 'error');
                document.getElementById('txtSLink').focus();
                return false;
            }
            // alert("hi");

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
                            <h5 class="txt-dark">Project Sampling </h5>
                        </div>

                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <li><a href="ProjectDetails.aspx"><span>Projects </span></a></li>
                                <li class="active"><span>Project Sampling</span></li>
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
                                        <h6 class="panel-title txt-dark">Project Sampling:</h6>
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
                                        <div class="col-sm-6 col-xs-12">
                                        </div>
                                        <div class="col-sm-3 col-xs-6">
                                            <div class="form-group">
                                                <label class="container1_ppd">
                                                    <input style="position: unset; width: 30px;" runat="server" type="checkbox" id="chkAddHashing" onchange="ApplyVariable(this.id)" />Add Hashing<span class="checkmark_ppd"></span></label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Project Name</label>
                                                <asp:DropDownList ID="ddlPN" runat="server" class="form-control ui search selection dropdown"
                                                    AutoPostBack="True" OnSelectedIndexChanged="ddlPN_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Country</label>
                                                <asp:DropDownList ID="ddlCountry" runat="server" class="form-control ui search selection dropdown"
                                                    data-style="form-control btn-default btn-outline" AutoPostBack="True"
                                                    OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged">
                                                </asp:DropDownList>

                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Supplier</label>
                                                <asp:DropDownList ID="ddlsupplier" runat="server" class="form-control  ui search selection dropdown" data-style="form-control btn-default btn-outline">
                                                </asp:DropDownList>

                                            </div>
                                        </div>
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Respondent Quota</label>
                                                <asp:TextBox ID="txtRCQ" runat="server" class="form-control" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%--<div class="col-sm-6 col-xs-12">
											<div class="form-group">
												<label class="control-label mb-10">Survay Link</label>
                                                <asp:TextBox ID="txtSLink" runat="server" class="form-control" ></asp:TextBox>
											</div>	
										</div>--%>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">CPI ($)</label>
                                                <asp:TextBox ID="txtCPI" runat="server" class="form-control" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-sm-6 col-xs-12 ParameterContent">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Parameter Name</label>
                                                <asp:TextBox runat="server" Text="enc" ReadOnly="true" CssClass="form-control" ID="txtParameterName"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-sm-6 col-xs-12 ParameterContent">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Hashing Type</label>
                                                <asp:DropDownList runat="server" ID="txtHashingType" CssClass="form-control">
                                                    <asp:ListItem Selected="True" Value="SHA3">SHA3</asp:ListItem>
                                                    <asp:ListItem Value="SHA1">SHA1</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <%-- <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Tracking Type</label>
                                                <asp:DropDownList runat="server" ID="ddlTrackingType" class="form-control">
                                                    <asp:ListItem Value="0">Redirects</asp:ListItem>
                                                    <asp:ListItem Value="1">Pixel Tracking</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>--%>
                                    </div>

                                    <%--<div class="row" id="Section_Redirects">
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Completes</label>
                                                <asp:TextBox ID="txtcomplete" runat="server" class="form-control" placeholder="Enter your status complete url here"></asp:TextBox>

                                            </div>
                                        </div>
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Terminate</label>
                                                <asp:TextBox ID="txtTerminate" runat="server" class="form-control" placeholder="Enter your Terminate url"></asp:TextBox>

                                            </div>
                                        </div>
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Overquota</label>
                                                <asp:TextBox ID="txtOverquota" runat="server" class="form-control" placeholder="Enter your Overquota url"></asp:TextBox>

                                            </div>
                                        </div>
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Security Term</label>
                                                <asp:TextBox ID="txtSecurity" runat="server" class="form-control" placeholder="Enter your Security Term url"></asp:TextBox>

                                            </div>
                                        </div>
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Fraud Error</label>
                                                <asp:TextBox ID="txtFraud" runat="server" class="form-control" placeholder="Enter your Fraud Error url"></asp:TextBox>

                                            </div>
                                        </div>
                                    </div>--%>
                                    <%--<div class="row" id="Section_Pixel">
                                        <div class="col-md-8 col-sm-8 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Success</label>
                                                <asp:TextBox ID="txtSuccess" runat="server" class="form-control" data-style="form-control btn-default btn-outline">
                                                    
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-8 col-sm-8 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Default</label>
                                                <asp:TextBox ID="txtDefault" runat="server" class="form-control" data-style="form-control btn-default btn-outline" TextMode="MultiLine" Rows="5">
                                                    
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-8 col-sm-8 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Failure</label>
                                                <asp:TextBox ID="txtFailure" runat="server" class="form-control" data-style="form-control btn-default btn-outline" TextMode="MultiLine" Rows="5">
                                                    
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-8 col-sm-8 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Quality Termination</label>
                                                <asp:TextBox ID="ddlQuality" runat="server" class="form-control" data-style="form-control btn-default btn-outline" TextMode="MultiLine" Rows="5">
                                                    
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-8 col-sm-8 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Over Quota</label>
                                                <asp:TextBox ID="ddlOverquota" runat="server" class="form-control" data-style="form-control btn-default btn-outline" TextMode="MultiLine" Rows="5">
                                                    
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>--%>
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
                                                <input type="button" value="Back" id="btnBack" class="btn btn-danger" />

                                                <%--	<button type="button" class="btn btn-success btn-anim"><i class="icon-rocket"></i><span class="btn-text">submit</span></button>--%>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <asp:GridView ID="grdlink" runat="server" AutoGenerateColumns="false" Visible="false" DataKeyNames="ID" class="table table-hover display  pb-30">
                                                <Columns>
                                                    <asp:BoundField DataField="PName" HeaderText="Project Name" />
                                                    <asp:BoundField DataField="Country" HeaderText="Country" />
                                                    <asp:BoundField DataField="Respondants" HeaderText="Quota" />
                                                    <asp:BoundField DataField="OLink" HeaderText="Client Link" />
                                                    <asp:BoundField DataField="MLink" HeaderText="Supply Link" />
                                                </Columns>
                                            </asp:GridView>

                                            <div class="panel-wrapper collapse in">
                                                <div class="panel-body">
                                                    <div class="table-wrap">
                                                        <div class="" style="overflow-x: scroll">
                                                            <table id="ProjectRpt" class="table table-hover display  pb-30">
                                                                <thead>
                                                                    <tr>
                                                                        <th>
                                                                            <label>Project Number</label></th>
                                                                        <th>Project Name</th>
                                                                        <th>Country</th>
                                                                        <th>Supplier</th>
                                                                        <th>Quota</th>
                                                                        <th>Client Link</th>
                                                                        <th>Supply Link</th>
                                                                        <th>Notes</th>
                                                                        <th>Check</th>
                                                                        <th>Edit
                                                                        </th>
                                                                        <%--  <th>ID</th>--%>
                                                                        <%--<th>Panel Size</th>--%>
                                                                        <%--<th>Country</th>--%>
                                                                        <%--<th>Edit</th>--%>
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
    <script type="text/javascript">


        //function SectionSelect() {
        //    alert('h');
        //}




        function chkchecked(value) {
            var status
            if ($('#' + value).is(':checked')) {

                status = 'Check';
            }
            else {
                status = 'Uncheck';
            }
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'ProjectMapping.aspx/updatestatus',
                data: JSON.stringify({ id: value, action: status }),
                cache: false,
                dataType: "json",
                success: function (data) {
                    //alert(data.d.length);
                    if (data.d.length > 0) {

                        $('#lblmsg').val(data.d);
                        // alert(data.d);
                        // alert("Updated succesfuly");

                        if (data.d == 'Overquota') {
                            alert(data.d);
                            $('#' + value).prop("checked", true);
                        }
                    }
                },
                error: function (result) {
                    alert(result.d);
                }
            });



        }

        function trackingtype() {
            $('#Section_Pixel').hide();
            $('#ddlTrackingType').change(function () {
                if ($('#ddlTrackingType').val() == 0) {

                    $('#Section_Redirects').slideDown();
                    $('#Section_Pixel').slideUp();
                }

                else if ($('#ddlTrackingType').val() == 1) {

                    $('#Section_Redirects').slideUp();
                    $('#Section_Pixel').slideDown();
                }
            });
        }
        function getredirects() {
            $('#ddlsupplier').change(function () {
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'ProjectMapping.aspx/GetRedirects',
                    data: JSON.stringify({ ID: $('#ddlsupplier').val() }),
                    cache: false,
                    dataType: "json",
                    success: function (data) {
                        $('#txtcomplete').val(data.d[0].Completes);
                        $('#txtTerminate').val(data.d[0].Terminate);
                        $('#txtOverquota').val(data.d[0].Overquota);
                        $('#txtSecurity').val(data.d[0].Security);
                        $('#txtFraud').val(data.d[0].Fraud);
                    },
                    error: function (result) {
                        (result.d);
                    }
                });
            });
        }

        jQuery(document).ready(function () {
            $('#ddlPN').dropdown();
            $('#ddlCountry').dropdown();
            $('#ddlsupplier').dropdown();
            trackingtype();
            getredirects();
            ApplyVariable('chkAddHashing');
            var id = 0;
            var query = window.location.search.substring(1);
            var vars = query.split("&");
            for (var i = 0; i < vars.length; i++) {
                pair = vars[i].split("=");
                //alert(pair[1]);
                id = pair[1];
            }
            if (id) {
                populatetablebyid(id);
            }
            else {
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'ProjectMapping.aspx/GetProjectsDetails',
                    cache: false,
                    dataType: "json",
                    success: function (data) {
                        var tbody = "";
                        // $("#ProjectRpt tbody").empty();
                        for (var i = 0; i < data.d.length; i++) {
                            if (data.d[i].Status == 0) {
                                tbody += '<tr><td><a href="ProjectPageDetails.aspx?ID=' + data.d[i].ProjectID + '" class="hover_blue">' + data.d[i].PNumber + '</td><td>' + data.d[i].PName + '</td><td>' + data.d[i].Country + '</td><td>' + data.d[i].Supplier + '</td><td>' + data.d[i].Respoondants + '</td><td>' + data.d[i].OLink + '</td><td>' + data.d[i].MLink + '</td><td>' + data.d[i].Notes + '</td><td> <label class="container1"><input type="checkbox" id="' + data.d[i].ID + '" onchange="chkchecked(this.id)" /><span class="checkmark"></span></label><td><input type="button" class="btn btn-success samewidth" onclick="Edit(this.id)" id="' + data.d[i].ID + '" value="Edit" /></td></td></tr>';
                            }
                            else {
                                tbody += '<tr><td><a href="ProjectPageDetails.aspx?ID=' + data.d[i].ProjectID + '" class="hover_blue">' + data.d[i].PNumber + '</td><td>' + data.d[i].PName + '</td><td>' + data.d[i].Country + '</td><td>' + data.d[i].Supplier + '</td><td>' + data.d[i].Respoondants + '</td><td>' + data.d[i].OLink + '</td><td>' + data.d[i].MLink + '</td><td>' + data.d[i].Notes + '</td><td> <label class="container1"><input type="checkbox" checked="checked" id="' + data.d[i].ID + '" onchange="chkchecked(this.id)" /><span class="checkmark"></span></label><td><input type="button" class="btn btn-success samewidth" onclick="Edit(this.id)" id="' + data.d[i].ID + '" value="Edit" /></td></td></tr>';
                            }

                        }
                        $("#ProjectRpt tbody").html(tbody);
                        $("#ProjectRpt").DataTable();

                    },
                    error: function (result) {
                        // 
                        (result.d);
                    }
                });
            }
            $('#btnBack').click(function () {
                var url = "ProjectPageDetails.aspx?ID=" + id;
                window.location.href = url;
            });
        });

        function Edit(id) {
            var url = "UpdateProjectMapping.aspx?ID=" + id + "&PID=" + getQuerystringID();
            window.location.href = url;
        }

        function populatetablebyid(id) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'ProjectMapping.aspx/GetProjectsDetailsbyid',
                data: JSON.stringify({ PID: id }),
                cache: false,
                dataType: "json",
                success: function (data) {
                    var tbody = "";
                    // $("#ProjectRpt tbody").empty();
                    for (var i = 0; i < data.d.length; i++) {
                        if (data.d[i].Status == 0) {
                            tbody += '<tr><td><a href="ProjectPageDetails.aspx?ID=' + data.d[i].ProjectID + '" class="hover_blue">' + data.d[i].PNumber + '</td><td>' + data.d[i].PName + '</td><td>' + data.d[i].Country + '</td><td>' + data.d[i].Supplier + '</td><td>' + data.d[i].Respoondants + '</td><td>' + data.d[i].OLink + '</td><td>' + data.d[i].MLink + '</td><td>' + data.d[i].Notes + '</td><td> <label class="container1"><input type="checkbox" id="' + data.d[i].ID + '" onchange="chkchecked(this.id)" /><span class="checkmark"></span></label><td><input type="button" class="btn btn-success samewidth" onclick="Edit(this.id)" id="' + data.d[i].ID + '" value="Edit" /></td></td></tr>';
                        }
                        else {
                            tbody += '<tr><td><a href="ProjectPageDetails.aspx?ID=' + data.d[i].ProjectID + '" class="hover_blue">' + data.d[i].PNumber + '</td><td>' + data.d[i].PName + '</td><td>' + data.d[i].Country + '</td><td>' + data.d[i].Supplier + '</td><td>' + data.d[i].Respoondants + '</td><td>' + data.d[i].OLink + '</td><td>' + data.d[i].MLink + '</td><td>' + data.d[i].Notes + '</td><td> <label class="container1"><input type="checkbox" checked="checked" id="' + data.d[i].ID + '" onchange="chkchecked(this.id)" /><span class="checkmark"></span></label><td><input type="button" class="btn btn-success samewidth" onclick="Edit(this.id)" id="' + data.d[i].ID + '" value="Edit" /></td></td></tr>';
                        }

                    }
                    $("#ProjectRpt tbody").html(tbody);
                    $("#ProjectRpt").DataTable();

                },
                error: function (result) {
                    // 
                    (result.d);
                }
            });
        }
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
        //function EDIT(id) {
        //    var url = "/UpdateSupplier.aspx?ID=" + id;
        //    window.location.href = url;
        //}
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
