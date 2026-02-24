<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CompleteRedirects.aspx.cs" Inherits="VT_CompleteRedirects" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <link href="../dist/css/Alert.css" rel="stylesheet" />
    <script>
        function alertt(message1, message2, message3) {
            swal(
                message1,
                message2,
                message3
            )
        }
    </script>

    <style>
        #redirects .lbl {
            background-color: #eee !important;
            border: 0px !important;
            line-height: 1.92857143 !important;
            /*-webkit-box-shadow: 3px 3px 16px -3px rgba(0,0,0,0.75) !important;
            -moz-box-shadow: 3px 3px 16px -3px rgba(0,0,0,0.75) !important;
            box-shadow: 3px 3px 16px -3px rgba(0,0,0,0.75) !important;*/
        }
    </style>
    <title></title>
</head>
<body>
    <%--Modals--%>
    <div class="modal" id="CharacterLength" tabindex="-1" role="dialog">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Generate Link</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <div id="div_GenerateLink">
                        <p>Please enter length (between 8 to 16 character)of the code for Complete</p>
                        <input type="text" id="txtChar" class="form-control" style="width: 50%" />
                    </div>
                    <div id="div_CheckLink" style="display: none">
                        <label>https://nexton.us/VT/ProjectStatus.aspx?ID=[respondentID]&Status=</label><input type="text" id="txtRandomCode" class="form-control" /><label>&RC=0</label>
                    </div>
                    <div class="div_SaveLink">
                        <input type="hidden" value="" id="hdnCharactersNumber" />
                    </div>
                </div>
                <div class="modal-footer">
                    <input type="button" class="btn btn-primary" value="Generate Link" id="btnGenerateLink" />
                    <input type="button" class="btn btn-primary" value="Save Link" id="btnSaveLink" style="display: none" />
                    <button type="button" class="btn btn-secondary" data-dismiss="modal" id="CloseGenerateLink">Close</button>
                </div>
            </div>
        </div>
    </div>

    <%--Modals Stop--%>


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
                            <h5 class="txt-dark">Redirects</h5>
                        </div>
                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <%--<li><a href="#"><span>Supplier</span></a></li>--%>
                                <%--<li class="active"><span>Add Supplier</span></li>--%>
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
                                        <h6 class="panel-title txt-dark"></h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="form-wrap">
                                    <div class="row">
                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <asp:Label ID="lblmsg" runat="server" Font-Bold="true"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row" id="redirects">
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <input style="display: none" type="button" id="OpenCharacterLengthPopUp" class="btn btn-primary" data-toggle="modal" data-target="#CharacterLength" />
                                                <label class="control-label mb-10">Completes</label><br />
                                                <a href="javascript:void(0)" id="a_GenerateLink">Generate Link</a>
                                                <div id="divCompleteRedirectLinks">
                                                    <%--https://nexton.us/VT--%>
                                                    <ul>
                                                        <li>
                                                            <label class="form-control lbl" style="text-decoration: line-through">https://nexton.us/VT/ProjectStatus.aspx?ID=[respondentID]&Status=COMPLETE&RC=0   <a href="javascript:void(0)" style="margin-left: 10px; color: red" class="pull-right" id="">Delete</a> <a href="javascript:void(0)" class="pull-right" style="margin-left: 10px; color: red" id="">Enable</a>  <a href="javascript:void(0)" class="pull-right" style="margin-left: 10px; color: red" id="">Disable</a>  </label>
                                                        </li>
                                                        <li>
                                                            <label class="form-control lbl">https://nexton.us/VT/ProjectStatus.aspx?ID=[respondentID]&Status=1901&RC=0</label><a href="javascript:void(0)" id="">Disable</a></li>
                                                    </ul>
                                                    <%--<asp:TextBox ID="txtcomplete" runat="server" class="form-control" placeholder="Enter your status complete url here"></asp:TextBox>--%>
                                                </div>
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
                                                <label class="control-label mb-10">Deleted Redirects</label>
                                                <br />
                                                <div id="divDeletedCompleteRedirectLinks">
                                                    <ul>
                                                        <li>
                                                            <label class="form-control lbl">https://nexton.us/VT/ProjectStatus.aspx?ID=[respondentID]&Status=COMPLETE&RC=0</label>
                                                        </li>
                                                        <li>
                                                            <label class="form-control lbl">https://nexton.us/VT/ProjectStatus.aspx?ID=[respondentID]&Status=COMPLETE&RC=0</label>
                                                        </li>
                                                    </ul>
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
        </div>
    </form>
    <script src="../vendors/bower_components/jquery/dist/jquery.min.js"></script>
    <script src="../Scripts/Alerts.js"></script>
    <script src="../CustomJS/Common.js"></script>
    <script src="../CustomJS/CompleteRedirects.js"></script>
</body>
</html>
