<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProjectQuestionMapping.aspx.cs" Inherits="VT_ProjectQuestionMapping" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
    <link href="../dist/css/CustmAhte.css" rel="stylesheet" />

    <style>
        .container1_ppd {
            margin-bottom: 15px;
        }

        .tab1 {
            left: 0%;
            position: relative;
            float: left;
            width: 100%;
        }

        .tab2 {
            float: left;
            position: absolute;
            right: -100%;
            width: 98%
        }

        .form-control {
            height: 32px !important;
        }



        /*#QuestionsDetailed thead tr th {
            padding: 11px;
        }

        #QuestionsDetailed tbody tr td {
            padding: 5px 15px !important;
        }*/
    </style>

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

        function confirmDelete() {
            return confirm('Are you sure you want to delete this?');
        }

        function validation() {

            //var txtQuestion = document.getElementById('txtQuestion');
            //var ddlQType = document.getElementById('ddlQType');
            //var QLabel = document.getElementById('QLabel');



            //alert(txtQuestion.value);
            //alert(ddlQType.value);
            //alert(QLabel.value);

            //var result;



            //if (txtQuestion.value == '') {
            //    alertt('', 'Question ID cannot be left blank', 'error');
            //    document.getElementById('txtQuestion').focus();
            //    return false;
            //}
            //if (ddlQType.value == '0') {
            //    alertt('', 'Please select question type', 'error');
            //    document.getElementById('ddlQType').focus();
            //    return false;
            //}
            //if (QLabel.value == '') {
            //    alertt('', 'Question label cannot be left blank', 'error');
            //    document.getElementById('QLabel').focus();
            //    return false;
            //}

            //return true;
        }
    </script>
</head>
<body>
    <%-- <asp:GridView>
        <Columns>
            <asp:TemplateField>
            </asp:TemplateField>
            <asp:ButtonField />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            </Columns>
    </asp:GridView>--%>
    <form id="form1" runat="server">
        <div class="preloader-it">
            <div class="la-anim-1">&nbsp;</div>
        </div>
        <!--/Preloader-->
        <div class="wrapper theme-1-active pimary-color-red">
            <uc1:Header ID="Header1" runat="server" />
            <style>
                #tblSelectedQuestionsList td {
                    padding: 3px 15px !important;
                }

                #OptionsList td {
                    padding: 5px 15px !important;
                }
            </style>
            <link href="../dist/css/Toggleswitch.css" rel="stylesheet" />
            <div class="right-sidebar-backdrop"></div>
            <div class="page-wrapper">
                <div class="container-fluid">
                    <!-- Title -->
                    <div class="row heading-bg">
                        <div class="col-lg-3 col-md-4 col-sm-4 col-xs-12">
                            <h5 class="txt-dark">Mapping</h5>
                        </div>
                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <li><a href="#"><span>Pre Screening</span></a></li>
                                <li class="active"><span>Mapping</span></li>
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
                                        <h6 class="panel-title txt-dark">Project/Questions Mapping</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="form-wrap">
                                    <div class="row">
                                        <%--<img src="../Imgs/logo_transparent.png" style="height:100px;width:100px" />--%>
                                        <div class="col-md-4 col-sm-6 col-xs-12">
                                            <asp:Label ID="lblmsg" runat="server" Font-Bold="true"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-4 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Project</label>
                                                <select id="ddlProjects" class="form-control dropdown search selection">
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-sm-4 col-xs-12">
                                            <div class="form-group">
                                                <label class="control-label mb-10">Country</label>
                                                <select id="ddlCountry" class="form-control dropdown search selection">
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" id="Wrap_Tables">
                                        <div class="col-xs-8">
                                            <div id="AllQuestionsList">
                                                <table class="table table-bordered table-striped display pb-30 table-custom" id="tblQuestionList">
                                                    <thead>
                                                        <tr>
                                                            <th>Check</th>
                                                            <th>Question ID</th>
                                                            <th>Label</th>
                                                            <%-- <th>Options</th>--%>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                    </tbody>
                                                </table>
                                                <input type="button" id="btnSaveAndNext" class="btn btn-info btn-sm" value="SAVE AND NEXT" />
                                                <input type="button" id="btnBackToProjectMapping" class="btn btn-info btn-sm btnBackToProjectMapping" value="BACK TO PROJECT DASHBOARD" />
                                            </div>
                                            <div id="SelectedQuestionList" style="display: none">
                                                <table id="tblSelectedQuestionsList" class="table table-hover display  pb-30 table-custom">
                                                    <thead>
                                                        <tr>
                                                            <th>Question Id</th>
                                                            <th>Label</th>
                                                           <%-- <th>Logic</th>--%>
                                                            <th>Options</th>
                                                            <%--<th>Quota</th>--%>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td>Q1</td>
                                                            <td>Age</td>
                                                            <td>
                                                                <select class="form-control LogicDropDown dropdown search selection">
                                                                    <option value="0">Select</option>
                                                                    <option value="1">Terminate</option>
                                                                    <option value="2">Quota</option>
                                                                </select></td>
                                                        </tr>
                                                        <tr>
                                                            <td>Q1</td>
                                                            <td>Age</td>
                                                            <td>
                                                                <select class="form-control LogicDropDown dropdown search selection">
                                                                    <option value="0">Select</option>
                                                                    <option value="1">Terminate</option>
                                                                    <option value="2">Quota</option>
                                                                </select></td>
                                                        </tr>
                                                        <tr>
                                                            <td>Q1</td>
                                                            <td>Age</td>
                                                            <td>
                                                                <select class="form-control LogicDropDown dropdown fluid search selection">
                                                                    <option value="0">Select</option>
                                                                    <option value="1">Terminate</option>
                                                                    <option value="2">Quota</option>
                                                                </select></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                                <input type="button" class="btn btn-info btn-sm" value="BACK TO QUESTION LIST" id="btnBackToQuestionList" />
                                                <input type="button" class="btn btn-info btn-sm btnBackToProjectMapping" value="BACK TO PROJECT DASHBOARD" id="btnBackToDashboard" />
                                            </div>
                                        </div>
                                        <div class="col-xs-4">
                                            <table class="table table-bordered table-striped">
                                                <tbody>
                                                    <tr>
                                                        <th><strong>Show previous button</strong></th>
                                                        <td class="Switch_body">
                                                            <div id='center_ShowButton'>
                                                                <input type="checkbox" class="switch" id="ShowButton" onchange="SetHiddenFildsForControls()" />
                                                                <div class="wrap">
                                                                    <h6 class="SwitchTextLeft">ON</h6>
                                                                    <label class="Switch_label" for="ShowButton">
                                                                        <span class="rib"></span>
                                                                        <span class="rib"></span>
                                                                        <span class="rib"></span>
                                                                    </label>
                                                                    <h6 class="SwitchTextRight">OFF</h6>
                                                                </div>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th><strong>Show question ID</strong> </th>
                                                        <td class="Switch_body">
                                                            <div id="center_ShowQuestion">
                                                                <input type="checkbox" class="switch" id="ShowQuestion" onchange="SetHiddenFildsForControls()" />
                                                                <div class="wrap">
                                                                    <h6 class="SwitchTextLeft">ON</h6>
                                                                    <label class="Switch_label" for="ShowQuestion">
                                                                        <span class="rib"></span>
                                                                        <span class="rib"></span>
                                                                        <span class="rib"></span>
                                                                    </label>
                                                                    <h6 class="SwitchTextRight">OFF</h6>
                                                                </div>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th><strong>Show Logo</strong></th>
                                                        <td class="Switch_body">
                                                            <div id="center_ShowLogo">
                                                                <input type="checkbox" class="switch" id="ShowLogo" onchange="SetHiddenFildsForControls()" />
                                                                <div class="wrap">
                                                                    <h6 class="SwitchTextLeft">ON</h6>
                                                                    <label class="Switch_label" for="ShowLogo">
                                                                        <span class="rib"></span>
                                                                        <span class="rib"></span>
                                                                        <span class="rib"></span>
                                                                    </label>
                                                                    <h6 class="SwitchTextRight">OFF</h6>
                                                                </div>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                            <input type="hidden" id="ctrl_PreviousButton" dbcol="PreviousButton" value="0" />
                                            <input type="hidden" id="ctrl_QuestionQID" dbcol="QuestionQID" value="0" />
                                            <input type="hidden" id="ctrl_Logo" dbcol="Logo" value="0" />
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class=" col-xs-12">
                                            <div class="form-group">
                                                <br />
                                                <%--<input type="button" id="btnSubmit" class="btn btn-success btn-anim" value="Submit" />--%>
                                            </div>
                                        </div>
                                    </div>

                                    <%--modals start--%>
                                    <input type="button" style="display: none" class="btn btn-primary btn-lg" id="btnModalOptionSelect" data-toggle="modal" data-target="#OptionSelect" value="" />


                                    <div class="modal fade" id="OptionSelect" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                                        <div class="modal-dialog" style="width: 50%">
                                            <div class="modal-content">
                                                <!-- Modal Header -->
                                                <div class="modal-header">
                                                    <button type="button" class="close" data-dismiss="modal">
                                                        <span aria-hidden="true">&times;</span>
                                                        <span class="sr-only">Close</span>
                                                    </button>
                                                    <h4 class="modal-title" id="mdlMapIP">Logic</h4>
                                                </div>
                                                <!-- Modal Body -->
                                                <div class="modal-body">
                                                    <h5>
                                                        <label style="color: #006767" id="lblQuestionId"></label>
                                                        :
                                                        <label style="color: #006767" id="lblQuestionLabel"></label>
                                                    </h5>
                                                    <div class="clearfix" style="margin-bottom: 10px"></div>
                                                    <table id="OptionsList" class="table table-hover display  pb-30 table-custom">
                                                        <thead>
                                                            <tr>
                                                                <th style="width: 20%">Code</th>
                                                                <th style="width: 20%">Label</th>
                                                                <th style="width: 40%">Logic</th>
                                                                <th style="width: 20%">Quota</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                        </tbody>
                                                    </table>
                                                    
                                                    <p style="color:red;display:none" id="pErrorForOptionQuota" ></p>
                                                </div>
                                                <!-- Modal Footer -->
                                                <div class="modal-footer">
                                                    <button type="button" id="" class="btn btn-default" data-dismiss="modal">
                                                        Close
                                                    </button>
                                                    <input type="button" id="btnSaveOptions" class="btn btn-primary" value="Save" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <%--modals end--%>
                                </div>
                            </div>
                            <br />
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
    <script src="../Scripts/UI/ui/minified/jquery-ui.min.js"></script>
    <script src="../Scripts/jquery.table2excel.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.blockUI/2.70/jquery.blockUI.min.js"></script>
    <script src="../CustomJS/Common.js"></script>
    <script src="../CustomJS/ProjectQuestionMapping.js"></script>
    <script type="text/javascript">
        jQuery.noConflict();
        jQuery(document).ready(function () {
            Pageinit();
            $('.LogicDropDown').dropdown();
            $('#ShowButton').change(function () {

                if ($('#ShowButton').is(":checked")) {

                }
                else {

                }

            });

        });


    </script>
</body>
</html>
