<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddUser.aspx.cs" Inherits="AddUser" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript">

        function validation() {
          
            var txtUN = document.getElementById('txtUN');
            var txtUNid = document.getElementById('txtUserID');
            var txtPWD = document.getElementById('txtPWD');
            var txtEmail = document.getElementById('txtEmail');
            var txtCN = document.getElementById('txtCN');
            var txtAdd = document.getElementById('txtAdd');
            var role = document.getElementById('ddlrole');


            // alert('Asif');

            var result;



            if (txtUN.value == '') {
                alertt('', 'Name cannot be left blank', 'error');
                document.getElementById('txtUN').focus();
                return false;
            }
            if (txtUNid.value == '') {
                alertt('', 'Username cannot be left blank', 'error');
                document.getElementById('txtUserID').focus();
                return false;
            }
            if (txtEmail.value == '') {
                alertt('', 'Email cannot be left blank', 'error');
                document.getElementById('txtEmail').focus();
                return false;
            }
            result = echeck(txtEmail.value);
            if (result == false) {
                alertt('', ' Invalid Email-Id', 'error');
                document.getElementById('txtEmail').focus();

                return false;
            }
            if (txtPWD.value == '') {
                alertt('', 'Password cannot be left blank', 'error');
                document.getElementById('txtPWD').focus();
                return false;
            }
            if (txtCN.value == '') {
                alertt('', 'Contact Number cannot be left blank', 'error');
                document.getElementById('txtCN').focus();
                return false;
            }
            if (txtAdd.value == '') {
                alertt('', 'Address cannot be left blank', 'error');
                document.getElementById('txtAdd').focus();
                return false;
            }
            if (role.value == '0') {
                alertt('', 'Please select role of the user', 'error');
                document.getElementById('ddlrole').focus();
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
            var charCode = (evt.which) ? evt.which : event.keyCode; if (charCode != 46 && charCode != 45 && charCode > 31 && (charCode < 48 || charCode > 57))
            { return false; }
            return true;
        };

        function isspacekey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode; if (charCode == 32)
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
                            <h5 class="txt-dark">User</h5>
                        </div>

                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <%--<li><a href="#"><span>User</span></a></li>--%>
                                <li class="active"><span>Add User</span></li>
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
                                        <h6 class="panel-title txt-dark">Add User:</h6>
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
                                                <label class="control-label mb-10">Name</label>
                                                <asp:TextBox ID="txtUN" runat="server" class="form-control" placeholder="Enter Name"></asp:TextBox>

                                            </div>
                                        </div>
                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">User Name</label>
                                                <asp:TextBox ID="txtUserID" runat="server" class="form-control" onkeypress="return isspacekey(event)" placeholder="Enter User Name"></asp:TextBox>

                                            </div>
                                        </div>
                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Email ID</label>
                                                <asp:TextBox ID="txtEmail" runat="server" class="form-control" placeholder="Enter Mail ID"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Password</label>
                                                <asp:TextBox ID="txtPWD" runat="server" TextMode="Password" class="form-control" placeholder="Password"></asp:TextBox>

                                            </div>
                                        </div>

                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Contact Number</label>
                                                <asp:TextBox ID="txtCN" runat="server" class="form-control" MaxLength="12" onkeypress="return isNumberKey(event)" placeholder="Enter your Contact Number"></asp:TextBox>

                                            </div>
                                        </div>

                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Role</label>
                                                <asp:DropDownList runat="server" ID="ddlrole" CssClass="form-control">
                                                    <asp:ListItem Value="0" Text="--Select--"></asp:ListItem>
                                                    <asp:ListItem Value="1" Text="Admin"></asp:ListItem>
                                                    <asp:ListItem Value="2" Text="User"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>


                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Address</label>
                                                <asp:TextBox ID="txtAdd" runat="server" class="form-control" placeholder="Enter your Address" TextMode="MultiLine" Rows="4"></asp:TextBox>

                                            </div>
                                        </div>

                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <asp:Button ID="btnSubmit" runat="server" class="btn btn-success btn-anim" onkeypress="return isNumberKey(event)"
                                                    Text="Submit" OnClientClick="return validation();" OnClick="btnSubmit_Click" />
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
</html>
