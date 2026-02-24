<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UpdateProjectMapping.aspx.cs" EnableEventValidation="false" Inherits="UpdateProjectMapping" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
    <link href="../dist/css/CustmAhte.css" rel="stylesheet" />
    <script type="text/javascript">

        function validation() {


            var txtSLink = document.getElementById('txtSLink');
            var ddlCountry = document.getElementById('ddlCountry');
            var ddlPN = document.getElementById('ddlPN');
            var ddlsupplier = document.getElementById('ddlsupplier');
            var txtrespondent = document.getElementById('txtRCQ');
            //alert('Asif');
            // alert(txtrespondent.value);
            var result;


            if (ddlPN.value == '0') {
                alert('Please select Project');
                document.getElementById('ddlPN').focus();
                return false;
            }

            //if (ddlCountry.value == '0') {
            //    alertt('Please select country');
            //    document.getElementById('ddlCountry').focus();
            //    return false;
            //}

            //if (ddlsupplier.value == '0') {
            //    alertt('Please select Supplier');
            //    document.getElementById('ddlsupplier').focus();
            //    return false;
            //}
            // alert(txtrespondent.value);
            if (txtrespondent.value == '') {
                alert('Field cannot left blank');
                document.getElementById('txtRCQ').focus();
                return false;
            }
            if (txtSLink.value == '') {
                alert('Link cannot left blank');
                document.getElementById('txtSLink').focus();
                return false;
            }
            // alert("hi");

            return true;
        }

        function confirmDelete() {
            return confirm('Are you sure you want to delete?');
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
        function alertt(message1, message2, message3) {
            swal(
                message1,
                message2,
                message3
            )
        }
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
                            <h5 class="txt-dark">Edit Project Sampling </h5>
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
                                        <%--<h6 class="panel-title txt-dark">Project Sampling:</h6>--%>
                                        <label id="lblPnumber" style="font: bold; font-size: large"></label>
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
                                                    <input style="position: unset; width: 30px;" type="checkbox" id="chkAddHashing" onchange="ApplyVariable(this.id)" />Add Hashing<span class="checkmark_ppd"></span></label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Project Name</label>
                                                <input type="text" id="ddlPN" readonly="true" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Country</label>
                                                <%--<input type="text" id="ddlCountry" readonly="true" class="form-control" />--%>
                                                <asp:DropDownList ID="ddlCountry" runat="server" class="form-control ui search selection dropdown" data-style="form-control btn-default btn-outline"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Supplier</label>
                                                <%--<input type="text" id="ddlsupplier" class="form-control" disabled="disabled" />--%>
                                                <asp:DropDownList ID="ddlsupplier" runat="server" class="form-control ui search selection dropdown" data-style="form-control btn-default btn-outline"></asp:DropDownList>

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
                                                <asp:TextBox runat="server" CssClass="form-control" ID="txtParameterName"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-sm-6 col-xs-12 ParameterContent">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Hashing Type</label>
                                                <asp:DropDownList runat="server" ID="txtHashingType" CssClass="form-control">
                                                    <asp:ListItem>--Select--</asp:ListItem>
                                                    <asp:ListItem Value="SHA3">SHA3</asp:ListItem>
                                                    <asp:ListItem Value="SHA1">SHA1</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Notes</label>
                                                <asp:TextBox ID="txtNotes" runat="server" class="form-control" placeholder="Enter your notes here" TextMode="MultiLine" Rows="4"></asp:TextBox>
                                            </div>

                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <div class="form-group">

                                                <input type="hidden" value="0" id="hfProjectid" />
                                                <asp:Button ID="btnSubmit" runat="server" class="btn btn-success btn-anim" OnClientClick="return validation();"
                                                    Text="Submit" />

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
    <script src="../CustomJS/UpdateProjectMapping.js"></script>
    <script type="text/javascript">

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
        function GetQuerystringID() {
            var id = 0;
            var key = [];
            var value = [];
            var query = window.location.search.substring(1);
            var vars = query.split("&");
            for (var i = 0; i < vars.length; i++) {
                var pair = vars[i].split("=");
                key[i] = vars[i].split("=")[0];
                value[i] = vars[i].split("=")[1];
            }
            var obj = [key, value];
            return obj;
        }
        jQuery(document).ready(function () {
            PageInit();
            ApplyVariable('chkAddHashing');
            trackingtype();
            var id;
            var obj = GetQuerystringID();
            var key = obj[0];
            var value = obj[1];
            if (key.indexOf("ID") > -1) {
                var index = key.indexOf("ID");
                id = value[index];
            }

            if (id > -1) {
                //querystring coding starts
                //is querystring
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'UpdateProjectMapping.aspx/GetProjectsDetailsbyid',
                    data: JSON.stringify({ ID: id }),
                    cache: false,
                    dataType: "json",
                    success: function (data) {
                        //alert(data.d[0].RespondantQuota);
                        $('#hfProjectid').val(data.d[0].ProjectID);
                        $('#txtRCQ').val(data.d[0].RespondantQuota);
                        $('#txtCPI').val(data.d[0].CPI);
                        $('#txtNotes').val(data.d[0].Notes);
                        $('#ddlPN').val(data.d[0].ProjectName);
                        //$('#ddlsupplier').val(data.d[0].Supplier);
                        $('#ddlsupplier').dropdown('set selected', data.d[0].SUpplierID);
                        // $('#ddlCountry').val(data.d[0].Country);
                        $('#ddlCountry').dropdown('set selected', data.d[0].CountryID);
                        $('#lblPnumber').text(data.d[0].PNumber);
                        $('#ddlTrackingType').val(data.d[0].TrackingType);
                        if ($('#ddlTrackingType').val() == 0) {
                            $('#Section_Redirects').slideDown();
                            $('#Section_Pixel').slideUp();
                        }
                        else if ($('#ddlTrackingType').val() == 1) {
                            $('#Section_Redirects').slideUp();
                            $('#Section_Pixel').slideDown();
                        }
                        $('#txtcomplete').val(data.d[0].Completes);
                        $('#txtTerminate').val(data.d[0].Terminate);
                        $('#txtOverquota').val(data.d[0].Overquota);
                        $('#txtSecurity').val(data.d[0].Security);
                        $('#txtFraud').val(data.d[0].Fraud);
                        $('#txtSuccess').val(data.d[0].SUCCESS);
                        $('#txtDefault').val(data.d[0].DEFAULT);
                        $('#txtFailure').val(data.d[0].FAILURE);
                        $('#ddlQuality').val(data.d[0].QUALITY_TERMINATION);
                        $('#ddlOverquota').val(data.d[0].OVER_QUOTA);

                        //$('#lblPnumber').val(data.d[0].PNumber);
                        // alert(data.d[0].Supplier);

                        if (data.d[0].AddHashing == 1) {
                            $("#chkAddHashing").attr("checked", "checked");
                        }
                        else {
                            $("#chkAddHashing").removeAttr("checked");
                        }
                        $("#txtParameterName").val(data.d[0].ParameterName);
                        $("#txtHashingType").val(data.d[0].HashingType);
                        ApplyVariable("chkAddHashing");

                    },
                    error: function (result) {
                        // 
                        (result.d);
                    }
                });
                //querystring coding end
            }
            else {
                $('body').html('Invalid Parameters');
            }


            $('#btnSubmit').click(function (e) {
                e.preventDefault();
                if ($('#ddlCountry').val() == 0) {
                    alertt('', 'Please select country first', 'error');
                    return;
                }
                else if ($('#ddlsupplier').val() == 0) {
                    alertt('', 'Please select supplier first', 'error');
                    return;
                }
                else {
                    var addHashing = 0;
                    if ($("#chkAddHashing").is(":checked")) {
                        addHashing = 1;
                    }
                    else {
                        addHashing = 0;
                    }
                    var formData = {};
                    formData["ID"] = id;
                    formData["quota"] = $('#txtRCQ').val();
                    formData["cpi"] = $('#txtCPI').val();
                    formData["notes"] = $('#txtNotes').val();
                    formData["CountryID"] = $('#ddlCountry').val();
                    formData["SupplierID"] = $('#ddlsupplier').val();
                    formData["ProjectID"] = $('#hfProjectid').val();
                    formData["AddHashing"] = addHashing;
                    formData["ParameterName"] = $('#txtParameterName').val();
                    formData["HashingType"] = $('#txtHashingType').val();
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: 'UpdateProjectMapping.aspx/Updatepromapping',
                        //data: JSON.stringify({ ID: id, quota: $('#txtRCQ').val(), cpi: $('#txtCPI').val(), notes: $('#txtNotes').val(), CountryID: $('#ddlCountry').val(), SupplierID: $('#ddlsupplier').val(), ProjectID: $('#hfProjectid').val() }),
                        data: JSON.stringify({ formData }),
                        cache: false,
                        dataType: "json",
                        success: function (data) {
                            if (data.d.length > 0) {
                                if (data.d == '1') {
                                    var obj = GetQuerystringID();
                                    var id = 0;
                                    if (key.indexOf("PID") > -1) {
                                        var index = key.indexOf("PID");
                                        id = value[index];
                                    }
                                    var url = "ProjectMapping.aspx?Id=" + id;
                                    window.location.href = url;
                                }
                                else if (data.d == '0') {
                                    alert("Error: Please try again");
                                }
                                else if (data.d == '-1') {
                                    alertt('This country vendor and project is already mapped. Please try with another country');
                                }
                                else if (data.d == 'error') {
                                    alert("Please contact administrator");
                                }
                            }
                        },
                        error: function (result) {
                            // 
                            (result.d);
                        }
                    });
                }

            });

        });

        function Edit(id) {
            var url = "ProjectMapping.aspx?ID=" + id;
            window.location.href = url;
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
