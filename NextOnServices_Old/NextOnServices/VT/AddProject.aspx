<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddProject.aspx.cs" Inherits="AddProject" EnableEventValidation="false" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../dist/vendors/bootstrap-datetimepicker/build/css/bootstrap-datetimepicker.css" rel="stylesheet" />
    <style>
       
    </style>
    <script type="text/javascript">

        function validation() {


            var txtPName = document.getElementById('txtPName');
            var ddlclient = document.getElementById('ddlclient');
            var ddlPM = document.getElementById('ddlPM');
            var txtLOI = document.getElementById('txtLOI');
            var txtIRate = document.getElementById('txtIRate');
            var txtCPI = document.getElementById('txtCPI');
            //  var txtSSize = document.getElementById('txtSSize');
            var txtQuota = document.getElementById('txtQuota');
            var txtSDate = document.getElementById('txt1');
            var txtEDate = document.getElementById('txt2');
            var ddlCountry = document.getElementById('ddlCountry');

            // alert('Asif');

            var result;
            if (txtPName.value == '') {
                alertt('', 'Project name cannot be left blank', 'error');
                document.getElementById('txtPName').focus();
                return false;
            }
            if (ddlclient.value == '0') {
                alertt('', 'Please select client', 'error');
                document.getElementById('ddlclient').focus();
                return false;
            }
            if (ddlPM.value == '0') {
                alertt('', 'Please select Project Manager', 'error');
                document.getElementById('ddlPM').focus();
                return false;
            }
            if (txtLOI.value == '') {
                alertt('', 'LOI cannot left blank', 'error');
                document.getElementById('txtLOI').focus();
                return false;
            }
            if (txtLOI.value == 0) {
                alertt('', 'LOI cannot be Zero', 'error');
                document.getElementById('txtLOI').value = '';
                return false;
            }
            if (txtIRate.value == '') {
                alertt('', 'Incidence Rate cannot be left blank', 'error');
                document.getElementById('txtIRate').focus();
                return false;
            }
            if (txtIRate.value > 100) {
                alertt('', 'Incident Rate cannot be more then 100', 'error');
                // document.getElementById('txtIRate').value = '';
                return false;
            }
            if (txtCPI.value == '') {
                alertt('', 'LOI cannot left blank', 'error');
                document.getElementById('txtCPI').focus();
                return false;
            }
            //if (txtSSize.value == '') {
            //    alertt('', 'Incidence Rate be left blank', 'error');
            //    document.getElementById('txtSSize').focus();
            //    return false;
            //}
            if (txtQuota.value == '') {
                alertt('', 'LOI cannot left blank', 'error');
                document.getElementById('txtQuota').focus();
                return false;
            }
            if (txtSDate.value == '') {
                alertt('', 'Start Date be left blank', 'error');
                document.getElementById('txtSDate').focus();
                return false;
            }
            if (txtEDate.value == '') {
                alertt('', 'End Date be left blank', 'error');
                document.getElementById('txtEDate').focus();
                return false;
            }
            if (ddlCountry.value == '0') {
                alertt('', 'Please select country', 'error');
                document.getElementById('ddlCountry').focus();
                return false;
            }
            if (ddlstatus.value == '0') {
                alertt('', 'Please select status', 'error');
                document.getElementById('ddlstatus').focus();
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

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode; if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) { return false; }
            return true;
        };

        function isNumberKeyNonDecimal(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) { return false; }
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
                            <h5 class="txt-dark">Project</h5>
                        </div>

                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <%--<li><a href="#"><span>Project</span></a></li>--%>
                                <li class="active"><span>Add Project</span></li>
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
                                        <h6 class="panel-title txt-dark">Add Project:</h6>
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

                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Project Name</label>
                                                <asp:TextBox ID="txtPName" runat="server" class="form-control" placeholder="Enter Project Name "></asp:TextBox>

                                            </div>
                                        </div>

                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Project Description</label>
                                                <asp:TextBox ID="txtDesc" runat="server" class="form-control" placeholder="Enter Description"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Client Name</label>
                                                <asp:DropDownList ID="ddlclient" runat="server" class="form-control" data-style="form-control btn-default btn-outline">
                                                </asp:DropDownList>


                                            </div>
                                        </div>

                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Project Manager</label>
                                                <asp:DropDownList ID="ddlPM" runat="server" class="form-control" data-style="form-control btn-default btn-outline">
                                                </asp:DropDownList>
                                                <%-- <asp:TextBox ID="txtPM" runat="server" class="form-control" placeholder="Enter Project Manager"></asp:TextBox>--%>
                                            </div>
                                        </div>
                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">LOI(Mins)</label>
                                                <asp:TextBox ID="txtLOI" runat="server" class="form-control" onkeypress="return isNumberKeyNonDecimal(event)" placeholder="Enter LOI"></asp:TextBox>

                                            </div>
                                        </div>

                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Incidence Rate(%)</label>
                                                <asp:TextBox ID="txtIRate" runat="server" class="form-control" onkeypress="return isNumberKey(event)" placeholder="Enter Incidence Rate"></asp:TextBox>

                                            </div>
                                        </div>
                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">CPI ($)</label>
                                                <asp:TextBox ID="txtCPI" runat="server" onkeypress="return isNumberKey(event)" class="form-control" placeholder="Enter CPI"></asp:TextBox>

                                            </div>
                                        </div>

                                        <%-- <div class="col-md-4 col-sm-6 col-xs-12">
											<div class="form-group">
												<label class="control-label mb-10">Sample Size</label>
                                                   <asp:TextBox ID="txtSSize" runat="server" class="form-control" placeholder="Enter Sample Size"></asp:TextBox>
											
											</div>	
										</div>--%>
                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Respondent Quota</label>
                                                <asp:TextBox ID="txtQuota" runat="server" onkeypress="return isNumberKey(event)" class="form-control" placeholder="Enter Respondent Quota"></asp:TextBox>

                                            </div>
                                        </div>
                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Country</label>
                                                <asp:DropDownList ID="ddlCountry" runat="server" class="form-control" data-style="form-control btn-default btn-outline">
                                                </asp:DropDownList>

                                            </div>
                                        </div>
                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Project Start Date</label>
                                                <asp:TextBox ID="txtSDate" runat="server" class="form-control" placeholder="Enter Start Date(MM-DD-YYYY)" AutoComplete="off"></asp:TextBox>
                                                <%--<input type="date" id="txt1" name="txt1" class="form-control" />
                                                <asp:HiddenField runat="server" ID="txtSDate" />--%>
                                            </div>
                                        </div>
                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Project End Date</label>
                                                <asp:TextBox ID="txtEDate" runat="server" class="form-control" placeholder="Enter End Date(MM-DD-YYYY)" AutoComplete="off"></asp:TextBox>
                                                <%--<input type="date" id="txt2" name="txt2" class="form-control" />
                                                <asp:HiddenField runat="server" ID="txtEDate" />--%>
                                            </div>
                                        </div>




                                        <div class="col-md-4 col-sm-6 col-xs-12">

                                            <div class="form-group radio1">
                                                <label class="control-label mb-10">Project Link Type</label>
                                                <asp:RadioButtonList ID="rdotype" runat="server" class="form-group" RepeatDirection="Horizontal">
                                                    <asp:ListItem Value="1" Selected="True">Single Country</asp:ListItem>
                                                    <asp:ListItem Value="2">Multiple Countries</asp:ListItem>
                                                </asp:RadioButtonList>

                                            </div>
                                        </div>




                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Project Status</label>
                                                <asp:DropDownList ID="ddlstatus" runat="server" class="form-control" data-style="form-control btn-default btn-outline">
                                                    <%-- <asp:ListItem Value="0" Selected="True">-- Select Status --</asp:ListItem>--%>
                                                    <asp:ListItem Value="1">Closed</asp:ListItem>
                                                    <asp:ListItem Value="2">Live</asp:ListItem>
                                                    <asp:ListItem Value="3">On hold</asp:ListItem>
                                                    <asp:ListItem Value="4">Cancelled</asp:ListItem>
                                                    <asp:ListItem Value="5" Selected="True">Awarded</asp:ListItem>
                                                    <asp:ListItem Value="6">Invoiced</asp:ListItem>
                                                </asp:DropDownList>

                                            </div>
                                        </div>
                                       
                                        <div class="col-sm-12 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Notes</label>
                                                <asp:TextBox ID="txtNotes" runat="server" class="form-control" placeholder="Enter your notes here" TextMode="MultiLine" Rows="4"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <asp:Button ID="btnSubmit" runat="server" class="btn btn-success btn-anim" OnClientClick="return validation();" Text="Submit" OnClick="btnSubmit_Click" />
                                                <%--	<button type="button" class="btn btn-success btn-anim"><i class="icon-rocket"></i><span class="btn-text">submit</span></button>--%>
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
        </div>
    </form>

    <script src="../Scripts/Dropdown/semantic.js"></script>
    <script src="../Scripts/UI/jquery-1.4.2.js"></script>
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <link href="../Scripts/UI/themes/base/jquery-ui.css" rel="stylesheet" />
    <script src="../Scripts/UI/ui/minified/jquery-ui.min.js"></script>
    <%-- <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.5/jquery-ui.min.js" type="text/javascript"></script>--%>
    <script type="text/javascript">
        jQuery.noConflict();
        //function alertt()
        //{
        //    swal('Any fool can use a computer');
        //}


        jQuery("#txtSDate").datepicker({ dateFormat: 'mm-dd-yy' });
        jQuery("#txtEDate").datepicker({ dateFormat: 'mm-dd-yy' });
        $(document).ready(function () {
            //$('#btnSubmit').click(function () {
            //    $('#txtSDate').val($('#txt1').val());
            //    $('#txtEDate').val($('#txt2').val());
            //});
            $('#btntest').click(function () {


                swal({
                    title: 'Are you sure?',
                    text: "You won't be able to revert this!",
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'Yes, delete it!'
                }).then((result) => {

                    if (result.value) {
                        swal(
                            'Deleted!',
                            'Your file has been deleted.',
                            'success'
                        )
                    }
                })

            })

        });


    </script>

</body>
</html>
