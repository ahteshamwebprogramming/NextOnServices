<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProjectsStatus.aspx.cs" Inherits="ProjectsStatus" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dashboard Details</title>

    <script>
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
        //function openWindow(code) {
        //    window.open('ProjectPageDetails.aspx?Id=' + code, 'open_window', ' width=1000, height=600, left=100, top=30');
        //}
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
                <div class="container-fluid pt-25">
                    <div class="row heading-bg">
                        <div class="col-lg-3 col-md-4 col-sm-4 col-xs-12">
                            <h5 class="txt-dark">Project Status</h5>
                        </div>

                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <li><a href="ProjectDetails.aspx"><span>Projects</span></a></li>
                                <li class="active"><span>Project Status</span></li>
                            </ol>
                        </div>
                        <!-- /Breadcrumb -->

                    </div>

                    <!-- Row -->
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="panel panel-default card-view">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Projects Status</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-wrapper collapse in">
                                    <div class="panel-body">
                                        <div class="form-wrap">
                                            <div class="row">
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <label class="control-label mb-10">Projects</label>
                                                        <asp:DropDownList ID="ddlProject" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlProject_SelectedIndexChanged" data-style="form-control btn-default btn-outline">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <label class="control-label mb-10">Status</label>
                                                        <asp:DropDownList ID="ddlStatus" runat="server" class="form-control" data-style="form-control btn-default btn-outline">
                                                            <asp:ListItem Value="0">-- Select Status --</asp:ListItem>
                                                            <asp:ListItem Value="1">Closed</asp:ListItem>
                                                            <asp:ListItem Value="2">Live</asp:ListItem>
                                                            <asp:ListItem Value="3">On Hold</asp:ListItem>
                                                            <asp:ListItem Value="4">Cancelled</asp:ListItem>
                                                            <asp:ListItem Value="5">Awarded</asp:ListItem>
                                                            <asp:ListItem Value="6">Invoiced</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col-sm-6">
                                                    <asp:Button class="btn btn-success btn-anim" runat="server" ID="btSubmit" Text="Submit" OnClick="btSubmit_Click" />
                                                </div>
                                            </div>


                                        </div>


                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- /Row -->

                </div>
                <!-- Footer -->
                <uc2:Footer ID="Footer1" runat="server" />
                <!-- /Footer -->

            </div>

        </div>

    </form>
</body>
</html>
