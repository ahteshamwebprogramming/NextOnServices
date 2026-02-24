<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddPixelMgr.aspx.cs" Inherits="AddPixelMgr" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
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
                alertt('', 'Supplier name cannot left blank', 'error');
                document.getElementById('txtName').focus();
                return false;
            }
            if (txtSize.value == '') {
                alertt('', 'Sample Size cannot be left blank', 'error');
                document.getElementById('txtSize').focus();
                return false;
            }


            if (txtCNumber.value == '') {
                alertt('', 'Contact Number cannot left blank', 'error');
                document.getElementById('txtCNumber').focus();
                return false;
            }
            if (txtCE.value == '') {
                alertt('', 'Email cannot be left blank', 'error');
                document.getElementById('txtCE').focus();
                return false;
            }
            result = echeck(txtCE.value);
            if (result == false) {
                alertt('', ' Invalid Email-Id', 'error');
                document.getElementById('txtCE').focus();

                return false;
            }
            if (ddlCountry.value == '0') {
                alertt('', 'Please select country', 'error');
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
            var charCode = (evt.which) ? evt.which : event.keyCode; if (charCode != 46 && charCode != 45 && charCode > 31 && (charCode < 48 || charCode > 57))
            { return false; }
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
                                <%--<li><a href="#"><span>Supplier</span></a></li>--%>
                                <li class="active"><span>Add Supplier</span></li>
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
                                        <h6 class="panel-title txt-dark">Add Supplier:</h6>
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
                                                <label class="control-label mb-10">Tracking Type</label>
                                                <asp:DropDownList ID="DropDownList1" runat="server" class="form-control" data-style="form-control btn-default btn-outline">
                                                    <asp:ListItem Value="0" Text="Select"></asp:ListItem>
                                                    <asp:ListItem Value="1" Text="Pixel Tracking"></asp:ListItem>
                                                    <asp:ListItem Value="2" Text="Redirect"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>



                                <br />
                            </div>
                        </div>

                    </div>
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="form-wrap">
                                    <div class="row">
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
</html>
