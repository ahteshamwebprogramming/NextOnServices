<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RecontactPageDetails.aspx.cs" Inherits="VT_RecontactPageDetails" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../dist/css/CustmAhte.css" rel="stylesheet" />
    <title>Details</title>
    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
    <link href="../Scripts/POPup/styles.min.css" rel="stylesheet" />
    <link href="../dist/css/Tooltip.css" rel="stylesheet" />
    <style>
        .logo-wrap img {
            min-width: 97px !important;
        }

        .mw {
            min-width: max-content;
        }

        #tblSupplierDetails thead tr th label {
            min-width: max-content;
        }

        .handcursor:hover {
            cursor: pointer;
        }

        .P_B_O {
            padding-bottom: 0px !important;
        }

        #IPCountSpan {
            border-top: 1px solid #f1f1f1;
            box-shadow: 0px 1px 4px 1px #f1f1f1;
            margin-top: 10px;
        }

        .tooltip1 {
            border-bottom: 0px dotted black !important;
        }

        .color_amber_b {
            /*background-color: orange !important;*/
        }

        .color_yellow_b {
            /*background-color: yellow !important;*/
        }

        .color_red_b {
            /*background-color: red !important;*/
        }

        .toolti_cust {
            font-size: 10px;
            font-weight: 700;
            background-color: #01c853 !important;
        }
        /*div.dataTables_wrapper {
            width: 1208px;
            margin: 0 auto;
        }*/
        .color_blue_b {
            /*background-color: blue !important;*/
        }

        .ppdhighlight {
            padding-left: 70px;
            padding-right: 70px;
            margin-top: 10px;
        }

        .bordertophighlight {
            border-top: 1px solid #f1f1f1;
            box-shadow: 0px 1px 4px 1px #f1f1f1;
        }

        .color_amber {
            color: orange;
        }

        .color_red {
            color: red;
        }

        .color_yellow {
            color: yellow;
        }

        .color_blue {
            color: blue;
        }

        .color_white {
            color: wheat;
        }

        .color_white1 {
            display: none !important;
        }

        .color_green {
            color: green;
        }

        #tblSurveySpecs thead tr th, #tblCountry thead tr th, #tblSupplierDetails thead tr th, #tblSurveyLinks thead tr th {
            padding: 11px;
        }

        #tblSurveySpecs tbody tr td, #tblCountry tbody tr td, #tblSupplierDetails tbody tr td, #tblSurveyLinks tbody tr td {
            padding: 10px 15px !important;
        }

            #tblSurveySpecs tbody tr td select {
                padding: 1px 10px 2px 2px;
                height: 22px;
            }

            #tblSupplierDetails tbody tr td .samewidth, #tblSurveyLinks tbody tr td .samewidth {
                padding: 0px 7px !important;
                font-size: 12px;
                width: 90px !important;
            }

        .tblSupplierDetails th {
            font: bold;
        }

        .modal-body .form-horizontal .col-sm-2,
        .modal-body .form-horizontal .col-sm-10 {
            width: 100%;
        }

        .modal-body .form-horizontal .control-label {
            text-align: left;
        }

        .modal-body .form-horizontal .col-sm-offset-2 {
            margin-left: 15px;
        }

        #tblSurveySpecs thead tr th:nth-of-type(3), #tblSurveySpecs thead tr th:nth-of-type(4), #tblSurveySpecs thead tr th:nth-of-type(5), #tblSurveySpecs thead tr th:nth-of-type(6), #tblSurveySpecs thead tr th:nth-of-type(8), #tblSurveySpecs tbody tr td:nth-of-type(8), #tblSurveySpecs tbody tr td:nth-of-type(6), #tblSurveySpecs tbody tr td:nth-of-type(5), #tblSurveySpecs tbody tr td:nth-of-type(4), #tblSurveySpecs tbody tr td:nth-of-type(3) {
            text-align: center;
        }

        #tblCountry thead tr th:nth-of-type(2), #tblCountry thead tr th:nth-of-type(3), #tblCountry thead tr th:nth-of-type(4), #tblCountry thead tr th:nth-of-type(5), #tblCountry thead tr th:nth-of-type(6), #tblCountry thead tr th:nth-of-type(7), #tblCountry thead tr th:nth-of-type(8), #tblCountry thead tr th:nth-of-type(9), #tblCountry thead tr th:nth-of-type(10), #tblCountry tbody tr td:nth-of-type(6), #tblCountry tbody tr td:nth-of-type(5), #tblCountry tbody tr td:nth-of-type(4), #tblCountry tbody tr td:nth-of-type(3), #tblCountry tbody tr td:nth-of-type(2), #tblCountry tbody tr td:nth-of-type(7), #tblCountry tbody tr td:nth-of-type(8), #tblCountry tbody tr td:nth-of-type(9), #tblCountry tbody tr td:nth-of-type(10) {
            text-align: center;
        }

        #tblSupplierDetails thead tr th, #tblSupplierDetails tbody tr td {
            text-align: center;
        }

            #tblSupplierDetails thead tr th:nth-of-type(2), #tblSupplierDetails thead tr th:nth-of-type(3), #tblSupplierDetails tbody tr td:nth-of-type(2), #tblSupplierDetails tbody tr td:nth-of-type(3) {
                text-align: left !important;
            }

        .ui.multiple.dropdown > .label {
            font-size: 0.8em;
        }

        .ui.fluid.dropdown {
            min-width: 251px;
        }

        .ui.dropdown .menu > .item {
            font-size: 0.8rem;
        }

        .B_grnd {
            padding-left: 7px;
            margin: 0.14285714rem 0.28571429rem 0.14285714rem 0em;
            box-shadow: 0px 0px 0px 1px rgba(34, 36, 38, 0.15);
            border-radius: 5px;
            background-color: #f0ecec;
        }

        .delete.icon:hover {
            color: black !important;
        }

        .delete.icon {
            cursor: pointer !important;
            margin-right: 0em;
            margin-left: 0.5em;
            font-size: 0.92857143em;
            opacity: 0.5;
            -webkit-transition: background 0.1s ease;
            transition: background 0.1s ease;
        }

        a.ui.label:hover {
            background-color: #E0E0E0;
            border-color: #E0E0E0;
            background-image: none;
            color: rgba(0, 0, 0, 0.8);
        }
    </style>
    <script>
        function validation() {
            var txtPName = document.getElementById('txtIPCount');
            // var ddlclient = document.getElementById('ddlclient');            
            // alert('Asif');
            var result;
            if (txtPName.value == '') {
                //alertt('', 'Field cannot be left blank', 'error');
                $('#lblIperror').text('Please fill the number');
                $('#lblIperror').show();
                document.getElementById('txtIPCount').focus();
                return false;
            }
            //if (ddlCountry.value == '0') {
            //    alertt('', 'Please select country', 'error');
            //    document.getElementById('ddlCountry').focus();
            //    return false;
            //}           
            return true;
        }
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode; if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) { return false; }
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
        <div class="wrapper theme-1-active pimary-color-red">
            <uc1:Header ID="Header1" runat="server" />
            <div class="right-sidebar-backdrop"></div>
            <div class="page-wrapper">
                <div class="container-fluid pt-25">
                    <!-- Title -->
                    <div class="row heading-bg">
                        <div class="col-lg-3 col-md-4 col-sm-4 col-xs-12">
                            <h5 class="txt-dark">Project</h5>
                        </div>
                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <li class="active"><a href="#"><span>Project Details</span></a></li>
                                <%--<li class="active"><span>Update Project</span></li>--%>
                            </ol>
                        </div>
                        <!-- /Breadcrumb -->
                    </div>
                    <!-- /Title -->
                    <%--Modals Start--%>
                    <input type="button" id="invokemodal_modal_EditRecontactProject" data-toggle="modal" data-target="#modal_EditRecontactProject" style="display: none" />
                    <div class="modal fade" id="modal_EditRecontactProject" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                        <div class="modal-dialog">
                            <div class="modal-content">
                                <!-- Modal Header -->
                                <div class="modal-header">
                                    <button type="button" class="close" data-dismiss="modal">
                                        <span aria-hidden="true">&times;</span>
                                        <span class="sr-only">Close</span>
                                    </button>
                                    <h4 class="modal-title" id="modal_h4__cpi">Edit</h4>
                                </div>
                                <!-- Modal Body -->
                                <div class="modal-body">
                                    <div class="form-horizontal" role="form">
                                        <div class="form-group">
                                            <div class="col-sm-10">
                                                <input type="hidden" id="hfId" />
                                                <input type="hidden" id="hiddenCPIID" />
                                                <label style="color: #006767; font-size: 14px; font-weight: 500" class="" id="lblPname">Recontact Project</label>
                                                <br />
                                                <label style="color: #006767; font-size: 14px; font-weight: 500" class="" id="lnlPnumber">NXT0001</label>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-sm-2 control-label" for="txtRecontactDescription">Recontact Description</label>
                                            <div class="col-sm-10">
                                                <input type="text" class="form-control" id="txtRecontactDescription" placeholder="Recontact Description" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-sm-2 control-label" for="txtLOI">LOI</label>
                                            <div class="col-sm-10">
                                                <input type="text" class="form-control" id="txtLOI" placeholder="LOI" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-sm-2 control-label" for="txtIR">Incident Rate(%)</label>
                                            <div class="col-sm-10">
                                                <input type="text" class="form-control" id="txtIR" placeholder="Incident Rate" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-sm-2 control-label" for="txtCPI">CPI</label>
                                            <div class="col-sm-10">
                                                <input type="text" class="form-control" id="txtCPI" placeholder="CPI" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-sm-2 control-label" for="txtRCQ">Respondant Quota</label>
                                            <div class="col-sm-10">
                                                <input type="text" class="form-control" id="txtRCQ" placeholder="Respondant Quota" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label class="col-sm-2 control-label" for="txtNOTES">NOTES</label>
                                            <div class="col-sm-10">
                                                <textarea class="form-control" id="txtNOTES"></textarea>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <!-- Modal Footer -->
                                <div class="modal-footer">
                                    <input type="button" class="btn btn-default" data-dismiss="modal" value="Close" />
                                    <input type="button" class="btn btn-primary" onclick="SaveRecontactDetails()" value="Save changes" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <%--Modals End--%>
                    <!-- Row -->
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Survey Details</h6>
                                    </div>
                                    <div class="pull-right">
                                        <input type="button" onclick="GetRecontactProjectDetails()" class="btn btn-success samewidth" id="btnEdit" value="Edit" />
                                        <%--<input type="button" class="btn btn-success samewidth clr2" id="btnEditURL" value="Edit URL" />--%>
                                        <%--<input type="button" class="btn btn-success samewidth" id="btnProjectMapping" value="Project Sampling" />--%>
                                        <asp:Button runat="server" ID="btnDownloadData" class="btn btn-success samewidth clr2" OnClick="btnDownloadData_Click" Text="Download Data"></asp:Button>
                                        <%--  <input type="button" class="btn btn-success samewidth"  value="Edit" />
                                     <input type="button" class="btn btn-success samewidth"  value="Edit URL" />
                                     <input type="button" class="btn btn-success samewidth"  value="Geberate URL" />
                                     <input type="button" class="btn btn-primary samewidth"  value="Download Data " />--%>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-wrapper collapse in">
                                    <div class="panel-body">
                                        <div class="table-wrap">
                                            <style>
                                                .header {
                                                    font-weight: 600;
                                                }
                                            </style>
                                            <%--<asp:GridView ID="GrdSurveyDetails" CssClass="table table-hover display  pb-30  table-bordered table-custom" runat="server" Width="100%" OnRowCreated="GrdSurveyDetails_RowCreated">
                                                <Columns>
                                                    <asp:BoundColumn DataField="PName" HeaderText="Survey Name" HeaderStyle-Font-Bold="true" />
                                                    <asp:BoundColumn DataField="Company" HeaderText="Client" HeaderStyle-Font-Bold="true" />
                                                    <asp:BoundColumn DataField="UserName" HeaderText="Project Manager" HeaderStyle-Font-Bold="true" />
                                                    <asp:BoundColumn DataField="Notes" HeaderText="Notes" HeaderStyle-Font-Bold="true" />
                                                </Columns>
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:Button runat="server" CssClass="btn btn-primary" Text="" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>--%>
                                            <div class="row">
                                                <div class="col-sm-7">
                                                    <table class="table table-hover display table-bordered table-custom">
                                                        <tr>
                                                            <td class="header w-30">Survey ID</td>
                                                            <td>
                                                                <%--  <label runat="server" id="lblSurveyID" name="lblSurveyID" />--%>
                                                                <asp:Label runat="server" ID="lblSurveyID" Text=""></asp:Label>
                                                                <asp:HiddenField runat="server" ID="hflblSurveyID" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="header">Survey Name</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblSurvayname" Text=""></asp:Label></td>
                                                            <asp:HiddenField runat="server" ID="hflblSurvayname" />
                                                            <%--<td>
                                                            <input type="button" id="btnEdit" value="Edit" />
                                                        </td>--%>
                                                        </tr>
                                                        <tr>
                                                            <td class="header">Length Of Interview</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblLOI" Text="Yet to Come"></asp:Label>&nbsp<span>min</span> </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="header">CPI</td>
                                                            <td>$&nbsp
                                                                <asp:Label runat="server" ID="lblCPI" Text="Yet to Come"></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="header">Expected IR</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="txtEIR" Text="Yet to Come"></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="header">Notes</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblNotes" Text=""></asp:Label></td>
                                                            <%--<td>
                                                            <asp:Button runat="server" ID="btnDownloadData" Text="Download Data" OnClick="btnDownloadData_Click"></asp:Button>
                                                        </td>--%>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <div class="col-sm-5" style="text-align: center; vertical-align: middle; padding-top: 40px">
                                                    <div class="row">
                                                        <div class="col-md-12">
                                                            <h1 id="hstatus" runat="server" style="color: #006767; font-weight: bold; font-size: 50px">STATUS</h1>
                                                            <br />
                                                            <h1 style="color: #006767; font-weight: bold; font-size: 50px">
                                                                <asp:Label runat="server" ID="lblCompletion">50</asp:Label>%
                                                        <label style="font-size: 16px; color: #01c853; font-weight: normal">Achieved</label></h1>
                                                        </div>
                                                    </div>
                                                    <div class="row ppdhighlight" style="margin-top: 20px">
                                                        <div class="col-md-12 bordertophighlight">
                                                            <span id="spncompletes" runat="server" style="font-size: 40px; color: #006767; font-weight: bold">5</span><span style="font-size: 16px; color: #01c853; font-weight: normal"> out of </span><span runat="server" id="spnTotalcomplete" style="font-size: 40px; color: #006767; font-weight: bold">15</span>
                                                        </div>
                                                    </div>
                                                    <div class="row ppdhighlight">
                                                        <div class="col-md-12 bordertophighlight">
                                                            <div class="row">
                                                                <span style="font-size: 14px; color: #01c853; font-weight: normal">Actual IR</span>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp <span runat="server" id="spnpercentIR" style="font-size: 40px; color: #006767; font-weight: bold"></span><span style="font-size: 16px; color: #01c853; font-weight: normal">&nbsp%</span>
                                                                <%-- <div class="col-sm-3">
                                                                    <span id="spnflag" class="fa fa-flag fa-3x tooltip1"><span id="spntooltipir" class="tooltiptext1 toolti_cust color_amber_b">tool tip</span></span>
                                                                </div>--%>
                                                            </div>
                                                            <div class="row">
                                                                <span style="font-size: 14px; color: #01c853; font-weight: normal">Actual LOI</span>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp <span runat="server" id="spnLOI" style="font-size: 40px; color: #006767; font-weight: bold">1231</span><span style="font-size: 16px; color: #01c853; font-weight: normal">&nbsp min</span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <asp:GridView runat="server" ID="gdvProjectDownloadDetails">
                                            </asp:GridView>
                                            <asp:HiddenField runat="server" ID="hfPID" />
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
                                <div class="panel-heading P_B_O">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Country Details</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-body">
                                    <table id="tblCountry" class="table table-hover display table-bordered table-custom">
                                        <thead>
                                            <tr>
                                                <th>Country</th>
                                                <th>Total</th>
                                                <th>Complete</th>
                                                <%--<th>Screened</th>--%>
                                                <th>Terminate</th>
                                                <th>Overquota</th>
                                                <%--<th>Quotafull</th>--%>
                                                <th>Security Term</th>
                                                <th>Fraud Error</th>
                                                <th>Incomplete</th>
                                                <th>Cancelled</th>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>

                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="panel-heading P_B_O">
                                    <div class="pull-left">
                                        <%-- <a href="#" data-modal-target="#overlay1">Launch internal overlay</a>
                                        <div id="overlay1" style="background-color: darkred" class="hidden">
                                            <p>
                                                Lorem ipsum dolor sit amet...
                                            </p>
                                        </div>--%>
                                        <h6 class="panel-title txt-dark">Supplier Details</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-body " style="overflow-x: scroll">
                                    <table id="tblSupplierDetails" class="table table-hover display table-bordered table-custom">
                                        <thead>
                                            <tr>
                                                <th style="display: none">ID</th>
                                                <th>Supplier Name</th>
                                                <th>Total</th>
                                                <th>Complete</th>
                                                <%--<th>Screened</th>--%>
                                                <th>Terminate</th>
                                                <th>Overquota</th>
                                                <th>Security Term</th>
                                                <th>Fraud Error</th>
                                                <th>Incomplete</th>
                                                <th>Cancelled</th>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                    <%-- <asp:GridView ID="GrdSupplierDetails" CssClass="table table-hover display  pb-30  table-bordered table-custom" runat="server" Width="100%" AutoGenerateColumns="false">
                                        <Columns>
                                            <asp:BoundField DataField="Country" HeaderText="Country" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Suppliers" HeaderText="Supplier Name" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="CPI" HeaderText="CPI" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Total" HeaderText="Total" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Complete" HeaderText="Complete" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Incomplete" HeaderText="Incomplete" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Screened" HeaderText="Screened" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Terminate" HeaderText="Terminate" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Overquota" HeaderText="Overquota" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Sterm" HeaderText="Security Term" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Ferror" HeaderText="Fraud Error" HeaderStyle-Font-Bold="true" />
                                            <asp:TemplateField HeaderText="Block" HeaderStyle-Font-Bold="true">
                                                <ItemTemplate>
                                                    <label class="container1">
                                                        <input type="checkbox" id="chkall" /><span class="checkmark"></span>
                                                    </label>
                                                </ItemTemplate>

                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>--%>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="panel-heading P_B_O">
                                    <div class="pull-left">
                                        <%-- <a href="#" data-modal-target="#overlay1">Launch internal overlay</a>
                                        <div id="overlay1" style="background-color: darkred" class="hidden">
                                            <p>
                                                Lorem ipsum dolor sit amet...
                                            </p>
                                        </div>--%>
                                        <h6 class="panel-title txt-dark">Redirect Links</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-body " style="overflow-x: scroll">
                                    <div class="col-xs-12">
                                        <div class="form-group">
                                            <label class="control-label mb-10">Completes</label>
                                            <label class="form-control lbl" id="lblComplete">http://182.18.138.233/VT/ProjectStatus.aspx?ID=[respondentID]&Status=COMPLETE&RC=0</label>
                                            <%--<asp:TextBox ID="txtcomplete" runat="server" class="form-control" placeholder="Enter your status complete url here"></asp:TextBox>--%>
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
                                            <label class="form-control lbl" id="lblTerminate">http://182.18.138.233/VT/ProjectStatus.aspx?ID=[respondentID]&Status=TERMINATE&RC=0</label>

                                        </div>
                                    </div>

                                    <div class="col-xs-12">
                                        <div class="form-group">
                                            <label class="control-label mb-10">Overquota</label>
                                            <label class="form-control lbl" id="lblOverquota">http://182.18.138.233/VT/ProjectStatus.aspx?ID=[respondentID]&Status=OVERQUOTA&RC=0</label>

                                        </div>
                                    </div>
                                    <div class="col-xs-12">
                                        <div class="form-group">
                                            <label class="control-label mb-10">Security Term</label>
                                            <label class="form-control lbl" style="background-color: lightblue" id="lblS_Term">http://182.18.138.233/VT/ProjectStatus.aspx?ID=[respondentID]&Status=SEC_TERM&RC=0</label>

                                        </div>
                                    </div>
                                    <div class="col-xs-12">
                                        <div class="form-group">
                                            <label class="control-label mb-10">Fraud Error</label>
                                            <br />
                                            <label class="lbl form-control" id="lblF_Error">http://182.18.138.233/VT/ProjectStatus.aspx?ID=[respondentID]&Status=F_ERROR&RC=0</label>
                                        </div>
                                    </div>
                                    <div class="col-xs-12">
                                        <div class="form-group">
                                            <label class="control-label mb-10">Variable 1</label>
                                            <label class="form-control lbl" id="lblVar1">http://182.18.138.233/VT/ProjectStatus.aspx?ID=[respondentID]&Status=Variable1&RC=0</label>

                                        </div>
                                    </div>
                                    <div class="col-xs-12">
                                        <div class="form-group">
                                            <label class="control-label mb-10">Variable 2</label>
                                            <label class="form-control lbl" id="lblVar2">http://182.18.138.233/VT/ProjectStatus.aspx?ID=[respondentID]&Status=Variable2&RC=0</label>

                                        </div>
                                    </div>
                                    <%--<div class="col-xs-12">
                                        <div class="form-group">
                                            <label class="control-label mb-10">Notes</label>
                                            <label class="form-control lbl" style="height: 100px; font-size: 18px; color: #843534; font-weight: 600;">Replace [respondentID] with unique system id in the link to update the status in our database</label>
                                        </div>
                                    </div>--%>
                                </div>

                            </div>
                        </div>
                    </div>



                    <%-- --%>
                </div>
                <!-- Footer -->
                <uc2:Footer ID="Footer1" runat="server" />
                <!-- /Footer -->

            </div>

        </div>




    </form>
</body>


<%--<script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js" type="text/javascript"></script>--%>

<script src="../Scripts/POPup/main.min.js"></script>
<script src="//cdn.datatables.net/1.10.16/js/jquery.dataTables.min.js" type="text/javascript"></script>
<script src="../Scripts/Dropdown/semantic.js"></script>

<script type="text/javascript">
    $(document).ready(function () {
        var id = getquerystringid();
        LoadTable(id, 0);
    });

    function getquerystringid() {
        var pair1;
        var querystringid1;
        var id1;
        var query1 = window.location.search.substring(1);
        var vars1 = query1.split("&");
        for (var i = 0; i < vars1.length; i++) {
            pair1 = vars1[i].split("=");
            //alert(pair[1]);
            id1 = pair1[1];
            querystringid1 = id1;
        }
        return querystringid1
    }
    function LoadTable(id, opt) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: 'RecontactPageDetails.aspx/FetchRecontactProjects',
            data: JSON.stringify({ ID: id, OPT: opt }),
            async: false,
            cache: false,
            dataType: "json",
            success: function (response) {
                var countrydata = response.d[0];
                var tbodycountry = "";
                $("#tblCountry tbody").empty();
                if (countrydata.length > 0) {
                    for (var i = 0; i < countrydata.length; i++) {
                        {
                            tbodycountry += '<tr><td style="display:none">' + countrydata[i].ID + '</td><td>' + countrydata[i].Country + '</td><td>' + countrydata[i].Total + '</td><td>' + countrydata[i].Complete + '</td><td>' + countrydata[i].Terminate + '</td><td>' + countrydata[i].Overquota + '</td><td>' + countrydata[i].S_term + '</td><td>' + countrydata[i].F_error + '</td><td>' + countrydata[i].Incomplete + '</td><td>' + countrydata[i].Cancelled + '</td></tr>';
                        }
                        $("#tblCountry tbody").html(tbodycountry);
                        // $("#tblCountry").DataTable();
                    }
                }
                var supplierdata = response.d[1];
                var tbodysupp = "";
                $("#tblSupplierDetails tbody").empty();
                if (supplierdata.length > 0) {
                    for (var i = 0; i < supplierdata.length; i++) {
                        {
                            tbodysupp += '<tr><td style="display:none">' + supplierdata[i].ID + '</td><td>' + supplierdata[i].Supplier + '</td><td>' + supplierdata[i].Total + '</td><td>' + supplierdata[i].Complete + '</td><td>' + supplierdata[i].Terminate + '</td><td>' + supplierdata[i].Overquota + '</td><td>' + supplierdata[i].S_term + '</td><td>' + supplierdata[i].F_error + '</td><td>' + supplierdata[i].Incomplete + '</td><td>' + supplierdata[i].Cancelled + '</td></tr>';
                        }
                        $("#tblSupplierDetails tbody").html(tbodysupp);
                        // $("#tblCountry").DataTable();
                    }
                }
                var redirects = response.d[2];

                if (redirects.length > 0) {
                    for (var i = 0; i < redirects.length; i++) {
                        {
                            $('#lblComplete').text(redirects[i].Complete);
                            $('#lblTerminate').text(redirects[i].Terminate);
                            $('#lblOverquota').text(redirects[i].Overquota);
                            $('#lblS_Term').text(redirects[i].S_Term);
                            $('#lblF_Error').text(redirects[i].F_Error);
                            $('#lblVar1').text(redirects[i].Var1);
                            $('#lblVar2').text(redirects[i].Var2);
                        }
                        //$("#tblSupplierDetails tbody").html(tbodysupp);
                        // $("#tblCountry").DataTable();
                    }
                }
                var remainingdetails = response.d[3];

                if (remainingdetails.length > 0) {
                    for (var i = 0; i < remainingdetails.length; i++) {
                        {
                            //  $('#lblSurveyID').text(remainingdetails[i].PID);
                            $('#lblSurveyID').text(remainingdetails[i].PID);
                            $('#hflblSurveyID').val(remainingdetails[i].PID);
                            $('#lblSurvayname').text(remainingdetails[i].RecontactName);
                            $('#hflblSurvayname').val(remainingdetails[i].RecontactName);
                            $('#lblLOI').text(remainingdetails[i].LOI);
                            $('#lblCPI').text(remainingdetails[i].CPI);
                            $('#txtEIR').text(remainingdetails[i].IR);
                            $('#hstatus').text(remainingdetails[i].StatusStr);
                            $('#lblNotes').text(remainingdetails[i].Notes);
                        }
                        //$("#tblSupplierDetails tbody").html(tbodysupp);
                        // $("#tblCountry").DataTable();
                    }
                }
            },
            error: function (result) {
                (result.d);
            }
        });
    }
    function GetRecontactProjectDetails() {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: 'RecontactPageDetails.aspx/FetchRecontactProjects',
            data: JSON.stringify({ ID: getquerystringid(), OPT: 0 }),
            async: false,
            cache: false,
            dataType: "json",
            success: function (response) {
                var remainingdetails = response.d[3];
                if (remainingdetails.length > 0) {
                    for (var i = 0; i < remainingdetails.length; i++) {
                        {
                            $('#lnlPnumber').text(remainingdetails[i].PID);
                            $('#lblPname').text(remainingdetails[i].RecontactName);
                            $('#txtRecontactDescription').val(remainingdetails[i].RecontactDescription);
                            $('#txtLOI').val(remainingdetails[i].LOI);
                            $('#txtIR').val(remainingdetails[i].IR);
                            $('#txtCPI').val(remainingdetails[i].CPI);
                            $('#txtRCQ').val(remainingdetails[i].RCQ);
                            $('#txtNOTES').val(remainingdetails[i].Notes);
                            $('#hfId').val(getquerystringid());
                            $('#invokemodal_modal_EditRecontactProject').click();
                        }
                    }
                }
            },
            error: function (result) {
                alert(result.d);
            }
        });

    }

    function GetFormData() {
        var attr = new Array();
        var object = {};
        object['ID'] = $('#hfId').val();
        attr.push(object);
        //object['Projectid'] = id;
        //attr.push(object);
        object['RecontactDescription'] = $('#txtRecontactDescription').val();
        attr.push(object);
        object['LOI'] = $('#txtLOI').val();
        attr.push(object);
        object['IR'] = $('#txtIR').val();
        attr.push(object);
        object['CPI'] = $('#txtCPI').val();
        attr.push(object);
        object['RCQ'] = $('#txtRCQ').val();
        attr.push(object);
        object['Notes'] = $('#txtNOTES').val();
        attr.push(object);

        // return attr;
        return object;
    }
    function SaveRecontactDetails() {
        var formdata = GetFormData();
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: 'RecontactPageDetails.aspx/UpdateRecontactProjects',
            data: JSON.stringify({ formdata }),
            cache: false,
            dataType: "json",
            success: function (data) {
                if (data.d.length > 0) {
                    if (data.d = '1') {
                        //var url = "ProjectPageDetails.aspx?ID=" + formdata.Projectid;
                        //window.location.href = url;
                        $('#modal_EditRecontactProject .close').click();
                        LoadTable(formdata.ID, 0);
                    }
                    else if (data.d = '0') {
                        alert("Error: Please try again");
                    }
                    else if (data.d = 'error') {
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

</script>
</html>
