<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UpdateSupplier.aspx.cs" Inherits="UpdateSupplier" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
    <script type="text/javascript">

        function validation() {


            var txtName = document.getElementById('txtName');
            var txtSize = document.getElementById('txtSize');
            var txtCNumber = document.getElementById('txtCNumber');
            var txtCE = document.getElementById('txtCE');
            var ddlCountry = document.getElementById('ddlCountry');
            // alert('Asif');

            var result;



            if (txtName.value == '') {
                alert('Supplier name cannot left blank');
                document.getElementById('txtName').focus();
                return false;
            }
            if (txtSize.value == '') {
                alert('Sample Size cannot be left blank');
                document.getElementById('txtSize').focus();
                return false;
            }


            if (txtCNumber.value == '') {
                alert('Contact Number cannot left blank');
                document.getElementById('txtCNumber').focus();
                return false;
            }
            if (txtCE.value == '') {
                alert('Email cannot be left blank');
                document.getElementById('txtCE').focus();
                return false;
            }
            result = echeck(txtCE.value);
            if (result == false) {
                alert(' Invalid Email-Id');
                document.getElementById('txtCE').focus();

                return false;
            }
            if (ddlCountry.value == '0') {
                alert('Please select country');
                document.getElementById('ddlCountry').focus();
                return false;
            }
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
                            <h5 class="txt-dark">Supplier</h5>
                        </div>

                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <li><a href="#"><span>Supplier</span></a></li>
                                <li class="active"><span>Update Supplier</span></li>
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
                                        <h6 class="panel-title txt-dark">Update Supplier:</h6>
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
                                                <label class="control-label mb-10">Supplier Name</label>
                                                <asp:Label ID="txtName" runat="server" class="form-control" placeholder="Enter your Name "></asp:Label>

                                            </div>
                                        </div>

                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Supplier Description</label>
                                                <asp:TextBox ID="txtDesc" runat="server" class="form-control" placeholder="Enter Description"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Contact Number</label>
                                                <asp:TextBox ID="txtCNumber" runat="server" class="form-control" MaxLength="12" onkeypress="return isNumberKey(event)" placeholder="99 999-9999"></asp:TextBox>

                                            </div>
                                        </div>

                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Email</label>
                                                <asp:TextBox ID="txtCE" runat="server" class="form-control" placeholder="Enter your Email"></asp:TextBox>

                                            </div>
                                        </div>
                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Panel Size</label>
                                                <asp:TextBox ID="txtSize" runat="server" onkeypress="return isNumberKey(event)" class="form-control" placeholder="Enter Panel Size"></asp:TextBox>

                                            </div>
                                        </div>

                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Country</label>
                                                <asp:DropDownList ID="ddlCountry" runat="server" class="form-control ui search selection dropdown" data-style="form-control btn-default btn-outline">
                                                </asp:DropDownList>

                                            </div>
                                        </div>
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Completes</label>
                                                <asp:TextBox ID="txtcomplete" runat="server" class="form-control" placeholder="Enter your status complete url here"></asp:TextBox>

                                            </div>
                                        </div>
                                        <%-- <div class="col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Incomplete</label>
                                                <asp:TextBox ID="txtIncomplete" runat="server" class="form-control" placeholder="Enter your status Incomplete url here"></asp:TextBox>
                                            </div>
                                        </div>--%>
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
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group col-md-8">
                                                <label class="control-label mb-10">Success</label>
                                                <asp:TextBox ID="txtSuccess" runat="server" class="form-control" data-style="form-control btn-default btn-outline">
                                                    
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group col-md-8">
                                                <label class="control-label mb-10">Default</label>
                                                <asp:TextBox ID="txtDefault" runat="server" class="form-control" data-style="form-control btn-default btn-outline" TextMode="MultiLine" Rows="5">
                                                    
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group col-md-8">
                                                <label class="control-label mb-10">Failure</label>
                                                <asp:TextBox ID="txtFailure" runat="server" class="form-control" data-style="form-control btn-default btn-outline" TextMode="MultiLine" Rows="5">
                                                    
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group col-md-8">
                                                <label class="control-label mb-10">Quality Termination</label>
                                                <asp:TextBox ID="ddlQuality" runat="server" class="form-control" data-style="form-control btn-default btn-outline" TextMode="MultiLine" Rows="5">
                                                    
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group col-md-8">
                                                <label class="control-label mb-10">Over Quota</label>
                                                <asp:TextBox ID="ddlOverquota" runat="server" class="form-control" data-style="form-control btn-default btn-outline" TextMode="MultiLine" Rows="5">
                                                    
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Notes</label>
                                                <asp:TextBox ID="txtNotes" runat="server" class="form-control" placeholder="Enter your notes here" TextMode="MultiLine" Rows="4"></asp:TextBox>
                                            </div>
                                        </div>
                                        <%--<div class="col-xs-12" style="border: 1px solid rgba(33, 33, 33, 0.12); margin-bottom: 10px;"></div>
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Hashing Link</label>
                                                <asp:TextBox ID="txtHashingLink" runat="server" class="form-control" placeholder="Enter URL"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Authorization Key</label>
                                                <asp:TextBox ID="txtAuthorizationKey" runat="server" class="form-control" placeholder="Key"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Complete Status</label>
                                                <asp:TextBox ID="txtCompleteStatus" runat="server" class="form-control" placeholder="eg: 10"></asp:TextBox>

                                            </div>
                                        </div>

                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Other then Complete</label>
                                                <asp:TextBox ID="txtOtherThenComplete" runat="server" class="form-control" placeholder="Eg: 70"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Allow Hashing</label>
                                                <asp:RadioButtonList ID="rdbAllowHashing" runat="server" RepeatDirection="Horizontal" CellSpacing="10" CellPadding="20" Width="150px">
                                                    <asp:ListItem Value="1">Yes</asp:ListItem>
                                                    <asp:ListItem Value="0" Selected="True">No</asp:ListItem>
                                                </asp:RadioButtonList>

                                            </div>
                                        </div>--%>
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <asp:Button ID="btnSubmit" runat="server" class="btn btn-success btn-anim" OnClientClick="return validation();"
                                                    Text="Update" OnClick="btnSubmit_Click" />
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
</body>
<script src="../Scripts/Dropdown/semantic.js"></script>
<script type="text/javascript">
    $(document).ready(function () {
        $('#ddlCountry').dropdown();
    });
</script>
</html>
