<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProjectsListSago.aspx.cs" Inherits="VT_ProjectsListSago" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <style>
        #tbl thead tr th, #tbl tbody tr td {
            white-space: nowrap
        }
    </style>

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
                            <h5 class="txt-dark">Surveys From Lucid</h5>
                        </div>
                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <li><a href="ProjectDetails.aspx"><span>Projects </span></a></li>
                                <li class="active"><span>Surveys From Lucid</span></li>
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
                                    <div class="pull-right">
                                        <input type="button" value="Pull Surveys" class="btn btn-primary" id="btnPullSurveys" />
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="form-wrap">
                                    <div style="overflow-x: scroll">
                                        <table id="tbl" class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th>SurveyId</th>
                                                    <th>LanguageId</th>
                                                    <th>BillingEntityId</th>
                                                    <th>CPI</th>
                                                    <th>LOI</th>
                                                    <th>IR</th>
                                                    <th>IndustryId</th>
                                                    <th>StudyTypeId</th>
                                                    <th>IsMobileAllowed</th>
                                                    <th>IsNonMobileAllowed</th>
                                                    <th>IsTabletAllowed</th>
                                                    <th>IsSurveyGroupExist</th>
                                                    <th>CollectPII</th>
                                                    <th>UrlTypeId</th>
                                                    <th>IsManualInc</th>
                                                    <th>IsQuotaLevelCPI</th>
                                                    <th>LiveLink</th>
                                                    <th>Action</th>
                                                </tr>
                                            </thead>
                                            <tbody></tbody>
                                        </table>
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
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.blockUI/2.70/jquery.blockUI.min.js"></script>
    <script type="text/javascript">
        function alertt(message1, message2, message3) {
            swal(
                message1,
                message2,
                message3
            )
        }
        $(document).ready(function () {
            $("#btnPullSurveys").click(function () {
                $.blockUI({ message: '<h5>Please wait</h5>' });
                pullSurveysFromLucid();
            });
        });

        function pullSurveysFromLucid() {
            $.ajax({
                type: "GET",
                url: 'https://api.sample-cube.com/api/Survey/GetSupplierAllocatedSurveys/1595/084853e8-1b98-4828-9af8-15332e5fe165',
                headers: {
                    //'X-MC-SUPPLY-KEY': '1002:0466198A-A78B-4F6C-8CE2-4A79FBC8FA41',
                    //'X-MC-SUPPLY-KEY': '1595:084853e8-1b98-4828-9af8-15332e5fe165',
                    'Cache-Control': 'no-cache',
                },
                success: function (data) {
                    $.unblockUI();
                    if (data.Result.Success == true) {
                        let surveys = data.Surveys;

                        surveys.forEach((v, i) => {
                            $("#tbl tbody").append("<tr><td>" + v.AccountId + "</td><td>" + v.CPI + "</td><td>" + v.CollectPII + "</td><td>" + v.ExternalName + "</td><td>" + v.Group_UpdateTimeStamp + "</td><td>" + v.IR + "</td><td>" + v.IndustryId + "</td><td>" + v.IsManualInc + "</td><td>" + v.IsMobileAllowed + "</td><td>" + v.IsNonMobileAllowed + "</td><td>" + v.IsPIDEnabled + "</td><td>" + v.IsQuotaLevelCPI + "</td><td>" + v.IsSurveyGroupExist + "</td><td>" + v.IsTabletAllowed + "</td><td>" + v.LOI + "</td><td>" + v.LanguageId + "</td><td>" + v.LiveLink + "</td><td>" + v.NewSampleSource + "</td><td>" + v.ProjectId + "</td><td>" + v.Qual_UpdateTimeStamp + "</td><td>" + v.Quota_UpdateTimeStamp + "</td><td>" + v.Recommended + "</td><td>" + v.RefreshSample + "</td><td>" + v.StudyTypeId + "</td><td>" + v.SurveyGuId + "</td><td>" + v.SurveyId + "</td><td>" + v.SurveyScore + "</td><td>" + v.SurveyStatusId + "</td><td>" + v.TestLink + "</td><td>" + v.TotalRemaining + "</td><td>" + v.UpdateTimeStamp + "</td><td>" + v.UrlTypeId + "</td><td><input type='button' class='btn btn-primary btn-sm' value='Add Project' onclick=\"addToProject('" + v.SurveyId + "','" + v.CPI + "','" + v.LOI + "','" + v.IR + "','" + v.TotalRemaining + "','" + v.LiveLink + "')\" /></td><td><input type='button' class='btn btn-primary btn-sm' value='Get Qualifications' onclick=\"addToProject('" + v.SurveyId + "','" + v.CPI + "','" + v.LOI + "','" + v.IR + "','" + v.TotalRemaining + "','" + v.LiveLink + "')\" /></td></tr>");
                        });

                        $("#tbl").DataTable();
                    }
                },
                error: function (result) {
                    $.unblockUI();
                }
            });
        }

        function addToProject(projectId, cpi, loi, ir, livelink) {
            $.blockUI({ message: '<h5>Please wait</h5>' });
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'AddProject.aspx/AddProjectFromAPI',
                data: JSON.stringify({ ProjectId: projectId, CPI: cpi, LOI: loi, IR: ir, LiveLink: livelink }),
                cache: false,
                dataType: "json",
                success: function (data) {
                    let res = data.d.Result;
                    if (res.Result == true) {
                        alertt("Successfully added", "", "");
                        // pullSurveysFromLucid();
                    }
                    else {
                        alertt("Error", res.Message, "");
                    }
                    $.unblockUI();
                },
                error: function (result) {
                    alertt("Error", "", "");
                    $.unblockUI();
                }
            });
        }

    </script>
</body>
</html>

