<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProjectPageDetails.aspx.cs" EnableEventValidation="false" Inherits="ProjectPageDetails" %>

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
        .container1_ppd {
            margin-bottom: 15px;
        }

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
            //  alert(ddlclient.value);
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
            <link href="../dist/css/Toggleswitch.css" rel="stylesheet" />
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

                    <!-- Row -->
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="">
                                <div class="panel-wrapper collapse in">
                                    <div class="panel-body">
                                        <%--<input type="button" class="btn btn-primary btn-lg" data-toggle="modal" data-target="#myModalHorizontal" value=" Launch Horizontal Form" />--%>
                                        <!-- Modal -->
                                        <%--Modal to map country start  --%>
                                        <div class="modal fade" id="mdlMapCountry" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                                            <div class="modal-dialog">
                                                <div class="modal-content">
                                                    <!-- Modal Header -->
                                                    <div class="modal-header">
                                                        <button type="button" class="close" data-dismiss="modal">
                                                            <span aria-hidden="true">&times;</span>
                                                            <span class="sr-only">Close</span>
                                                        </button>
                                                        <h4 class="modal-title" id="mdlMapIP">Map IP</h4>
                                                    </div>
                                                    <!-- Modal Body -->
                                                    <div class="modal-body">
                                                        <div class="form-horizontal" role="form" style="padding-right: 15px; padding-left: 15px">
                                                            <div class="form-group">
                                                                <input type="hidden" id="hdnMapCountry1" />
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="control-label mb-10">Select Country/Countries</label>
                                                                <select class="ui fluid search selection dropdown mapclass" multiple="" id="mulddlCountries"></select>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <!-- Modal Footer -->
                                                    <div class="modal-footer">
                                                        <button type="button" id="btnCloseMappingModal" class="btn btn-default" data-dismiss="modal">
                                                            Close
                                                        </button>
                                                        <input type="button" id="btnSaveMapping" class="btn btn-primary" onclick="SaveMappings();" value="Save" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <%--Modal to map country Stop  --%>

                                        <%--================================Modal to edit Quota CountryWise=================================================--%>
                                        <input type="button" id="btnShowQuotaModal" data-toggle="modal" data-target="#modal_EditQuotaCW" style="display: none" />
                                        <input type="hidden" id="forquotaprojecturl" />
                                        <div class="modal fade" id="modal_EditQuotaCW" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="myModalLabel" aria-hidden="true">
                                            <div class="modal-dialog">
                                                <div class="modal-content">
                                                    <!-- Modal Header -->
                                                    <div class="modal-header">
                                                        <button type="button" class="close" data-dismiss="modal">
                                                            <span aria-hidden="true">&times;</span>
                                                            <span class="sr-only">Close</span>
                                                        </button>
                                                        <h4 class="modal-title" id="MTHQCW">Edit Quota</h4>
                                                    </div>
                                                    <div class="modal-body">
                                                        <div class="form-horizontal" role="form" style="padding-right: 15px; padding-left: 15px">
                                                            <div class="IP_checked">
                                                                <div class="form-group">
                                                                    <label for="txtQuotaCount">Enter Quota</label>
                                                                    <input id="txtQuotaCount" onkeypress="return isNumberKey(event)" type="text" class="form-control" placeholder="05" />
                                                                    <label id="lblQuotaCWediterror" style="color: red"></label>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                    <div class="modal-footer">
                                                        <div class="UpdateOnlyCWQuota">
                                                            <button type="button" id="btnSaveQuotaCWClose" class="btn btn-default" data-dismiss="modal">Close</button>
                                                            <input type="button" id="btnSaveQuotaCW" class="btn btn-primary" value="Save" />
                                                        </div>
                                                        <div class="UpdateCWandProjectQuota">
                                                            <button type="button" id="btnSaveQuotaCWandProjectClose" class="btn btn-default" data-dismiss="modal">Close</button>
                                                            <input type="button" id="btnSaveQuotaCWandProject" class="btn btn-primary" value="Yes! Update" />
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <%--================================Modal to edit tracking=================================================--%>
                                        <div class="modal fade" id="modal_IPControl" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="myModalLabel" aria-hidden="true">
                                            <div class="modal-dialog">
                                                <div class="modal-content">
                                                    <!-- Modal Header -->
                                                    <div class="modal-header">
                                                        <button type="button" id="btnhidemodal" class="close" data-dismiss="modal">
                                                            <span aria-hidden="true"></span>
                                                            <span class="sr-only">Close</span>
                                                        </button>
                                                        <h4 class="modal-title" id="abc">Security</h4>
                                                    </div>
                                                    <div class="modal-body">
                                                        <div class="form-horizontal" role="form" style="padding-right: 15px; padding-left: 15px">
                                                            <div class="IP_checked">
                                                                <div class="form-group">
                                                                    <label for="txtIPCount">Enter the max count of IP you want to allow</label>
                                                                    <input id="txtIPCount" onkeypress="return isNumberKey(event)" type="text" class="form-control" placeholder="05" />
                                                                    <span class="help-block">Max limit - 100</span>
                                                                    <label id="lblIperror" style="color: red"></label>
                                                                </div>
                                                            </div>
                                                            <div class="IP_unchecked">
                                                                Do you want to remove duplicate IP Security
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="modal-footer">
                                                        <div class="IP_checked">
                                                            <button type="button" id="Ip_control_Close" class="btn btn-default" data-dismiss="modal">Close</button>
                                                            <input type="button" id="btnSaveIpCount" class="btn btn-primary" value="Save" />
                                                        </div>
                                                        <div class="IP_unchecked">
                                                            <button type="button" class="btn btn-default" id="Ip_control_Cancel" data-dismiss="modal">Cancel</button>
                                                            <input type="button" id="btnYes" class="btn btn-primary" data-dismiss="modal" value="Yes" />
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="modal fade" id="myModalHorizontal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                                            <div class="modal-dialog">
                                                <div class="modal-content">
                                                    <!-- Modal Header -->
                                                    <div class="modal-header">
                                                        <button type="button" class="close" data-dismiss="modal">
                                                            <span aria-hidden="true">&times;</span>
                                                            <span class="sr-only">Close</span>
                                                        </button>
                                                        <h4 class="modal-title" id="myModalLabel">Tracking Type</h4>
                                                    </div>
                                                    <!-- Modal Body -->
                                                    <div class="modal-body">
                                                        <div class="form-horizontal" role="form" style="padding-right: 15px; padding-left: 15px">
                                                            <%-- <div class="form-group">
                                                                <label class="col-sm-2 control-label"
                                                                    for="inputEmail3">
                                                                    Email</label>
                                                                <div class="col-sm-10">
                                                                    <input type="email" class="form-control"
                                                                        id="inputEmail3" placeholder="Email" />
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-sm-2 control-label"
                                                                    for="inputPassword3">
                                                                    Password</label>
                                                                <div class="col-sm-10">
                                                                    <input type="password" class="form-control"
                                                                        id="inputPassword3" placeholder="Password" />
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <div class="col-sm-offset-2 col-sm-10">
                                                                    <div class="checkbox">
                                                                        <label>
                                                                            <input type="checkbox" />
                                                                            Remember me
                                                                        </label>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <div class="col-sm-offset-2 col-sm-10">
                                                                    <button type="submit" class="btn btn-default">Sign in</button>
                                                                </div>
                                                            </div>--%>
                                                            <div class="form-group">
                                                                <input type="hidden" id="hiddenID" />
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="control-label mb-10">Tracking Type</label>
                                                                <select id="ddlTrackingType" runat="server" class="form-control">
                                                                    <option value="0">Redirects</option>
                                                                    <option value="1">Pixel</option>
                                                                </select>
                                                            </div>
                                                            <div class="row" id="Section_Redirects">
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
                                                            </div>
                                                            <div class="row" id="Section_Pixel">
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
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <!-- Modal Footer -->
                                                    <div class="modal-footer">
                                                        <button type="button" class="btn btn-default"
                                                            data-dismiss="modal">
                                                            Close
                                                        </button>
                                                        <input type="button" id="btnsavetrackingtype" class="btn btn-primary" value="Save changes" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <%--=================================modal to edit cpi and all=============================================--%>
                                        <div class="modal fade" id="modal_cpi" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                                            <div class="modal-dialog">
                                                <div class="modal-content">
                                                    <!-- Modal Header -->
                                                    <div class="modal-header">
                                                        <button type="button" class="close" data-dismiss="modal">
                                                            <span aria-hidden="true">&times;</span>
                                                            <span class="sr-only">Close</span>
                                                        </button>
                                                        <h4 class="modal-title" id="modal_cpi">Edit</h4>
                                                    </div>
                                                    <!-- Modal Body -->
                                                    <div class="modal-body">
                                                        <div class="form-horizontal" role="form">
                                                            <div class="form-group">
                                                                <div class="col-sm-10">
                                                                    <input type="hidden" id="hiddenCPIID" />
                                                                    <label class="" id="lblPname"></label>
                                                                    <br />
                                                                    <label class="" id="lnlPnumber"></label>
                                                                    <br />
                                                                    <label class="" id="lblCountryName"></label>
                                                                    <br />
                                                                    <label class="" id="lblSupplierName"></label>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-sm-2 control-label" for="txtQuota">RESPONDENT QUOTA</label>
                                                                <div class="col-sm-10">
                                                                    <input type="text" class="form-control" id="txtQuota" placeholder="RESPONDENT QUOTA" />
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-sm-2 control-label" for="txtCPI">CPI</label>
                                                                <div class="col-sm-10">
                                                                    <input type="text" class="form-control" id="txtCPI" placeholder="CPI" />
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-sm-2 control-label" for="txtNOTES">NOTES</label>
                                                                <div class="col-sm-10">
                                                                    <input type="text" class="form-control" id="txtNOTES" placeholder="NOTES" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!-- Modal Footer -->
                                                    <div class="modal-footer">
                                                        <input type="button" class="btn btn-default" data-dismiss="modal" value="Close" />
                                                        <input type="button" class="btn btn-primary" onclick="savecpichanges()" value="Save changes" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Survey Details</h6>
                                    </div>
                                    <div class="pull-right">
                                        <input type="button" class="btn btn-success samewidth" id="btnEdit" value="Edit" />
                                        <input type="button" class="btn btn-success samewidth clr2" id="btnEditURL" value="Edit URL" />
                                        <input type="button" class="btn btn-success samewidth" id="btnProjectMapping" value="Project Sampling" />
                                        <%--<input type="button" class="btn btn-success samewidth" id="btnQuestionMapping" value="Question Mapping" />--%>
                                        <asp:Button runat="server" ID="btnDownloadData" class="btn btn-success samewidth clr2" Text="Download Data" OnClick="btnDownloadData_Click"></asp:Button>
                                        <input type="button" id="btnDownloadResponse" class="btn btn-success samewidth clr2" value="Download Response" onclick="btnDownloadResponseData()" />
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
                                            <div class="row">
                                                <div class="col-sm-7">

                                                    <table class="table table-hover display table-bordered table-custom">
                                                        <tr>
                                                            <td class="header w-30">Survey ID</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblSurveyID" Text=""></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="header">Survey Name</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblSurvayname" Text=""></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="header">Client</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblClient" Text="Some Client"></asp:Label></td>

                                                        </tr>
                                                        <tr>
                                                            <td class="header">Project Manager</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblProjectManager" Text="Project Manager"></asp:Label></td>

                                                        </tr>
                                                        <tr>
                                                            <td class="header">Country</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblCountry" Text="Yet to Come"></asp:Label></td>
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
                                                            <td class="header">End Date</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblEDate" Text="Yet to Come"></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="header">Notes</td>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblNotes" Text="Notes"></asp:Label></td>

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
                                                            <span id="spncompletes" style="font-size: 40px; color: #006767; font-weight: bold">5</span><span style="font-size: 16px; color: #01c853; font-weight: normal"> out of </span><span id="spnTotalcomplete" style="font-size: 40px; color: #006767; font-weight: bold">15</span>
                                                        </div>
                                                    </div>
                                                    <div class="row ppdhighlight">
                                                        <div class="col-md-12 bordertophighlight">
                                                            <div class="row">
                                                                <div class="col-sm-9">
                                                                    <span style="font-size: 14px; color: #01c853; font-weight: normal">Actual IR</span>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp <span id="spnpercentIR" style="font-size: 40px; color: #006767; font-weight: bold"></span><span style="font-size: 16px; color: #01c853; font-weight: normal">&nbsp%</span>
                                                                </div>
                                                                <div class="col-sm-3">
                                                                    <span id="spnflag" class="fa fa-flag fa-3x tooltip1"><span id="spntooltipir" class="tooltiptext1 toolti_cust color_amber_b">tool tip</span></span>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <span style="font-size: 14px; color: #01c853; font-weight: normal">Actual LOI</span>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp <span id="spnLOI" style="font-size: 40px; color: #006767; font-weight: bold">1231</span><span style="font-size: 16px; color: #01c853; font-weight: normal">&nbsp min</span>
                                                            </div>


                                                        </div>

                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row" style="padding-left: 15px; padding-right: 15px">
                                                <div class="col-md-12" style="border: 1px solid #f1f1f1; padding: 0px 10px 15px 25px;">
                                                    <div class="row">
                                                        <h1 id="h1" runat="server" style="color: #006767; font-weight: bold; font-size: 16px">Device Control</h1>
                                                    </div>
                                                    <%--<div class="row" style="padding-left: 30px">
                                                        <h4 style="color: #006767; font-weight: bold; text-align: left">Block</h4>
                                                    </div>--%>
                                                    <div class="row">
                                                        <div class="col-md-12" id="DivDeviceControl">
                                                            <div class="col-md-2 M_L_0 P_L_0" style="margin-top: 10px">
                                                                <div class="col-md-1 M_L_0 P_L_0 P_R_0">
                                                                    <label class="container1_ppd">
                                                                        <input type="checkbox" id="chkMobile" onchange="DeviceControl(this.id)" /><span class="checkmark_ppd"></span></label>
                                                                </div>
                                                                <div class="col-md-11 P_R_0 M_R_0" style="text-align: left">
                                                                    <label>Mobile (non-IOS)</label>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-2 M_L_0 P_L_0" style="margin-top: 10px;">
                                                                <div class="col-md-1 M_L_0 P_L_0 P_R_0">
                                                                    <label class="container1_ppd">
                                                                        <input type="checkbox" id="chkIOSMobile" onchange="DeviceControl(this.id)" /><span class="checkmark_ppd"></span></label>
                                                                </div>
                                                                <div class="col-md-11 P_R_0" style="text-align: left">
                                                                    <label>IOS Mobile</label>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-2  M_L_0 P_L_0" style="margin-top: 10px">
                                                                <div class="col-md-1  M_L_0 P_L_0 P_R_0">
                                                                    <label class="container1_ppd">
                                                                        <input type="checkbox" id="chkDesktop" onchange="DeviceControl(this.id)" /><span class="checkmark_ppd"></span></label>
                                                                </div>
                                                                <div class="col-md-11 P_R_0" style="text-align: left">
                                                                    <label>Desktop/Laptop</label>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-2  M_L_0 P_L_0" style="margin-top: 10px">
                                                                <div class="col-md-1  M_L_0 P_L_0 P_R_0">
                                                                    <label class="container1_ppd">
                                                                        <input type="checkbox" id="chkTablet" onchange="DeviceControl(this.id)" /><span class="checkmark_ppd"></span></label>
                                                                </div>
                                                                <div class="col-md-11 P_R_0" style="text-align: left">
                                                                    <label>Tablet</label>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-3  M_L_0 P_L_0" style="margin-top: 10px">

                                                                <div class="row">
                                                                    <div class="col-md-1  M_L_0 P_L_0 P_R_0">
                                                                        <label class="container1_ppd">
                                                                            <input type="checkbox" id="chkIpAddress" data-toggle="modal" data-target="#modal_IPControl" /><span class="checkmark_ppd"></span></label>
                                                                    </div>
                                                                    <div class="col-md-5 P_R_0" style="text-align: left">
                                                                        <label>Duplicate IP</label>
                                                                    </div>
                                                                    <div class="col-md-4" id="IPCountSpan" style="font-size: 10px; color: red; margin-top: 0px">
                                                                        <span>Count : </span><span id="spnIPCount">4</span>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <%--<div class="col-md-6" id="IPCountSpan" style="font-size: 10px; color: red">
                                                                        <span>Count : </span><span id="spnIPCount">4</span>
                                                                    </div>--%>
                                                                </div>

                                                            </div>
                                                        </div>

                                                    </div>
                                                    <div class="row" style="padding-left: 30px; margin-top: 15px; margin-left: -45px">
                                                        <p style="color: #006767; text-align: left">Please check to block</p>
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
                                        <h6 class="panel-title txt-dark">Survey Specs</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-body" style="overflow-x: scroll">

                                    <table id="tblSurveySpecs" class="table table-hover display table-bordered table-custom" style="overflow-x: scroll">
                                        <thead>
                                            <tr>
                                                <th style="display: none">ID</th>
                                                <th>Country</th>
                                                <th>Quota</th>
                                                <%-- <th>LOI</th>--%>
                                                <th>CPI</th>
                                                <%-- <th>IR</th>--%>
                                                <th>Complete</th>
                                                <th>Percentage</th>
                                                <th>Status</th>
                                                <th>IR</th>
                                                <th>LOI</th>
                                                <th>Flag</th>
                                                <th>Map IP</th>
                                                <th>Pre Screening</th>
                                                <th>Countries Mapped</th>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>

                                    <%--<asp:GridView ID="GrdSurveySpecs" CssClass="table table-hover display  pb-30  table-bordered table-custom" runat="server" Width="100%" AutoGenerateColumns="false">
                                        <Columns>
                                            <asp:BoundField DataField="Country" HeaderText="Country" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Quota" HeaderText="Survey Quota" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="LOI" HeaderText="LOI" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="CPI" HeaderText="CPI" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="IRate" HeaderText="IR" HeaderStyle-Font-Bold="true" />
                                            <asp:TemplateField HeaderText="Status" HeaderStyle-Font-Bold="true">
                                                <ItemTemplate>
                                                    <a href="#" onclick="redirecttostatus()">
                                                        <asp:Label runat="server" ID="lblStatus" Text='<%#Bind("Status") %>'>Status</asp:Label></a>
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
                                        <h6 class="panel-title txt-dark">Country Details</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-body" style="overflow-x: scroll">
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
                                                <th>Country</th>
                                                <th>Supplier Name</th>
                                                <th>CPI</th>
                                                <th>Quota</th>
                                                <th>Total</th>
                                                <th>Complete</th>
                                                <%--<th>Screened</th>--%>
                                                <th>Terminate</th>
                                                <th>Overquota</th>
                                                <th>Security Term</th>
                                                <th>Fraud Error</th>
                                                <th>Incomplete</th>
                                                <th>
                                                    <label>IR(in %)</label></th>
                                                <th>LOI</th>
                                                <th>Block</th>
                                                <th>Prescreening</th>
                                                <th>Edit</th>
                                                <th>Tracking Type</th>
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
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Survey Links</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-body" style="overflow-x: scroll">
                                    <%--<asp:GridView ID="GrdSurveyLinks" CssClass="table table-hover display  pb-30  table-bordered table-custom" runat="server" Width="100%" AutoGenerateColumns="false">
                                        <Columns>
                                            <asp:BoundField DataField="Country" HeaderText="Country" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="Suppliers" HeaderText="Supplier Name" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="OLink" HeaderText="Client Survey Link" HeaderStyle-Font-Bold="true" />
                                            <asp:BoundField DataField="MLink" HeaderText="Supplier Link" HeaderStyle-Font-Bold="true" />
                                        </Columns>
                                    </asp:GridView>--%>
                                    <table id="tblSurveyLinks" class="table table-hover display table-bordered table-custom" style="width: 100%">
                                        <thead>
                                            <tr>
                                                <th style="display: none">ID</th>
                                                <th>Country</th>
                                                <th>Supplier Name</th>
                                                <th>Client Survey Link</th>
                                                <th>Supplier Link</th>
                                                <th>Test</th>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                                <br />
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
<script src="../CustomJS/Common.js"></script>
<script src="../CustomJS/ProjectPageDetails.js"></script>
<script type="text/javascript">
    //$('body').on('focus', ".datepicker", function () {
    //    $(this).datepicker();
    //});
    //var jq = $.noConflict();

</script>
</html>

