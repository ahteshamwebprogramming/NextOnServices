<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BundledQualificationAnswers.aspx.cs" Inherits="VT_BundledQualificationAnswers" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/css/bootstrap.min.css" integrity="sha384-xOolHFLEh07PJGoPkLv1IbcEPTNtaed2xpHsD9ESMhqIYd0nLMwNLD69Npy4HI+N" crossorigin="anonymous">
    <style>
        #tbl thead tr th, #tbl tbody tr td {
            white-space: nowrap
        }

        .actiontd {
            padding: 1px 1px 1px 16px !important;
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
                            <h5 class="txt-dark">Bundled Qualification Answers</h5>
                        </div>
                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <li><a href="ProjectDetails.aspx"><span>Projects </span></a></li>
                                <li class="active"><span>Bundled Qualification Answers</span></li>
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
                                        <input type="button" value="Update From API" class="btn btn-primary" id="btnUpdateFromAPI" />
                                        <%--<input type="button" value="Test API" class="btn btn-primary" id="btnTestAPI" />--%>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="form-wrap">
                                    <div style="overflow-x: scroll">
                                        <table id="tbl" class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th>Name</th>
                                                    <th>QualificationType</th>
                                                    <th>Qualification Category</th>
                                                    <th>Qualification Type Desc</th>
                                                    <th>Qualification Category GDPRID</th>
                                                    <th>Text</th>

                                                    <th>Answer ID</th>
                                                    <th>Answer Text</th>
                                                    <th>Answer Code</th>
                                                                                                        
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

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/js/bootstrap.min.js" integrity="sha384-+sLIOodYLS7CIrQpBjl+C7nPvqq+FbNUBDunl/OZv93DB7Ln/533i8e/mZXLi/P+" crossorigin="anonymous"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/js/bootstrap.bundle.min.js" integrity="sha384-Fy6S3B9q64WdZWQUiU+q4/2Lc9npb8tCaSX9FK7E8HnRr0Jz8D6OP9dO5Vg3Q9ct" crossorigin="anonymous"></script>
    <script type="text/javascript">
        function alertt(message1, message2, message3) {
            swal(
                message1,
                message2,
                message3
            )
        }
        $(document).ready(function () {
            $("#btnUpdateFromAPI").click(function () {
                $.blockUI({ message: '<h5>Please wait</h5>' });
                updateFromAPI();
            });
            $("#btnTestAPI").click(function () {
                TestAPI();
            });
        });

        function updateFromAPI() {
            return new Promise((resolve, reject) => {
                $.when(
                    $.ajax({
                        type: 'GET',
                        url: "https://api.sample-cube.com/api/Survey/GetSupplierAllocatedSurveys/1595/084853e8-1b98-4828-9af8-15332e5fe165",
                        headers: { 'Cache-Control': 'no-cache', }
                    }),
                    $.ajax({
                        type: 'GET',
                        url: "https://api.sample-cube.com//api/Definition/GetLanguages/1595/084853e8-1b98-4828-9af8-15332e5fe165",
                        headers: { 'Cache-Control': 'no-cache', }
                    }),
                    $.ajax({
                        type: 'GET',
                        url: "https://api.sample-cube.com//api/Definition/GetIndustries/1595/084853e8-1b98-4828-9af8-15332e5fe165",
                        headers: { 'Cache-Control': 'no-cache', }
                    }),
                    $.ajax({
                        type: 'GET',
                        url: "https://api.sample-cube.com//api/Definition/GetStudyTypes/1595/084853e8-1b98-4828-9af8-15332e5fe165",
                        headers: { 'Cache-Control': 'no-cache', }
                    }),
                    $.ajax({
                        type: 'GET',
                        url: "https://api.sample-cube.com//api/Definition/GetSurveyStatuses/1595/084853e8-1b98-4828-9af8-15332e5fe165",
                        headers: { 'Cache-Control': 'no-cache', }
                    }),
                    $.ajax({
                        type: 'GET',
                        url: "https://api.sample-cube.com//api/Definition/GetRedirectTypes/1595/084853e8-1b98-4828-9af8-15332e5fe165",
                        headers: { 'Cache-Control': 'no-cache', }
                    })
                ).then(function (surveys, languages, industries, studytypes, surveyStatuses, redirectTypes) {
                    var returnData = {};
                    returnData["Surveys"] = surveys[0].Surveys;
                    returnData["Languages"] = languages[0].Languages;
                    returnData["Industries"] = industries[0].Industries;
                    returnData["StudyTypes"] = studytypes[0].StudyTypes;
                    returnData["SurveyStatuses"] = surveyStatuses[0].SurveyStatuses;
                    returnData["RedirectTypes"] = redirectTypes[0].RedirectTypes;
                    resolve(returnData);
                }).fail(function (err) {
                    alert(err.responseText)
                });
            });
        }

        function pullSurveysFromLucid() {
            pullSurveysFromLucidAPI().then((data) => {
                var Surveys = data.Surveys;
                var Languages = data.Languages;
                var Industries = data.Industries;
                var StudyTypes = data.StudyTypes;
                var SurveyStatuses = data.SurveyStatuses;
                var RedirectTypes = data.RedirectTypes;


                Surveys.forEach((v, i) => {

                    let Language = Languages.filter(x => x.LanguageId == v.LanguageId)[0].LanguageCode;
                    let Industry = Industries.filter(x => x.IndustryId == v.IndustryId)[0].Description;
                    let StudyType = StudyTypes.filter(x => x.StudyTypeId == v.StudyTypeId)[0].Description;
                    let SurveyStatus = SurveyStatuses.filter(x => x.SurveyStatusId == v.SurveyStatusId)[0].Description;
                    let UrlType = RedirectTypes.filter(x => x.RedirectTypeId == v.UrlTypeId).length > 0 ? RedirectTypes.filter(x => x.RedirectTypeId == v.UrlTypeId)[0].Description : "";

                    $("#tbl tbody").append("<tr><td>" + v.ProjectId + "</td><td>" + v.SurveyId + "</td><td>" + Language + "</td><td>" + StudyType + "</td><td>" + Industry + "</td><td>" + SurveyStatus + "</td>=<td>" + v.TotalRemaining + "</td><td>" + v.IR + "</td><td>" + v.CPI + "</td><td>" + v.LOI + "</td><td>" + v.IsMobileAllowed + "</td><td>" + v.IsNonMobileAllowed + "</td><td>" + v.IsTabletAllowed + "</td><td>" + v.IsManualInc + "</td><td>" + v.IsPIDEnabled + "</td><td>" + v.IsQuotaLevelCPI + "</td><td>" + v.IsSurveyGroupExist + "</td><td>" + v.NewSampleSource + "</td><td>" + v.LiveLink + "</td><td>" + v.TestLink + "</td><td>" + UrlType + "</td><td>" + v.Recommended + "</td><td class='actiontd'><input type='button' class='btn btn-primary btn-sm' value='Add Project' onclick=\"addToProject('" + v.SurveyId + "','" + v.CPI + "','" + v.LOI + "','" + v.IR + "','" + v.TotalRemaining + "','" + v.LiveLink + "')\" /><input type='button' style='margin-left:5px;' class='btn btn-primary btn-sm' value='Get Qualifications' onclick=\"getQualifications('" + v.SurveyId + "')\" /></td></tr>");
                });

                $("#tbl").DataTable();


                $.unblockUI();
            }).catch((err) => {
                $.unblockUI();
            });

            return;

            $.ajax({
                type: "GET",
                //url: 'https://api-hub.market-cube.com/supply-api-v2/api/v2/survey/allocated-surveys/1595/084853e8-1b98-4828-9af8-15332e5fe165',
                url: 'https://api.sample-cube.com/api/Survey/GetSupplierAllocatedSurveys/1595/084853e8-1b98-4828-9af8-15332e5fe165',
                headers: {
                    //'X-MC-SUPPLY-KEY': '1002:0466198A-A78B-4F6C-8CE2-4A79FBC8FA41',
                    //'X-MC-SUPPLY-KEY': '1595/084853e8-1b98-4828-9af8-15332e5fe165',
                    'Cache-Control': 'no-cache',
                },
                success: function (data) {
                    $.unblockUI();
                    if (data.Result.Success == true) {
                        let surveys = data.Surveys;

                        surveys.forEach((v, i) => {
                            $("#tbl tbody").append("<tr><td>" + v.AccountId + "</td><td>" + v.CPI + "</td><td>" + v.CollectPII + "</td><td>" + v.ExternalName + "</td><td>" + v.Group_UpdateTimeStamp + "</td><td>" + v.IR + "</td><td>" + v.IndustryId + "</td><td>" + v.IsManualInc + "</td><td>" + v.IsMobileAllowed + "</td><td>" + v.IsNonMobileAllowed + "</td><td>" + v.IsPIDEnabled + "</td><td>" + v.IsQuotaLevelCPI + "</td><td>" + v.IsSurveyGroupExist + "</td><td>" + v.IsTabletAllowed + "</td><td>" + v.LOI + "</td><td>" + v.LanguageId + "</td><td>" + v.LiveLink + "</td><td>" + v.NewSampleSource + "</td><td>" + v.ProjectId + "</td><td>" + v.Qual_UpdateTimeStamp + "</td><td>" + v.Quota_UpdateTimeStamp + "</td><td>" + v.Recommended + "</td><td>" + v.RefreshSample + "</td><td>" + v.StudyTypeId + "</td><td>" + v.SurveyGuId + "</td><td>" + v.SurveyId + "</td><td>" + v.SurveyScore + "</td><td>" + v.SurveyStatusId + "</td><td>" + v.TestLink + "</td><td>" + v.TotalRemaining + "</td><td>" + v.UpdateTimeStamp + "</td><td>" + v.UrlTypeId + "</td><td><input type='button' class='btn btn-primary btn-sm' value='Add Project' onclick=\"addToProject('" + v.SurveyId + "','" + v.CPI + "','" + v.LOI + "','" + v.IR + "','" + v.LiveLink + "')\" /></td><td><input type='button' class='btn btn-primary btn-sm' value='Get Qualifications' onclick=\"addToProject('" + v.SurveyId + "','" + v.CPI + "','" + v.LOI + "','" + v.IR + "','" + v.TotalRemaining + "','" + v.LiveLink + "')\" /></td></tr>");
                        });

                        $("#tbl").DataTable();
                    }
                },
                error: function (result) {
                    $.unblockUI();
                }
            });
        }

        function addToProject(projectId, cpi, loi, ir, totalRemaining, livelink) {
            $.blockUI({ message: '<h5>Please wait</h5>' });
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'AddProject.aspx/AddProjectFromAPI',
                data: JSON.stringify({ ProjectId: projectId, CPI: cpi, LOI: loi, IR: ir, TotalRemaining: totalRemaining, LiveLink: livelink }),
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

        function getQualifications(surveyId) {
            return new Promise((resolve, reject) => {
                $.ajax({
                    type: "GET",
                    url: "https://api.sample-cube.com/api/Survey/GetSupplierSurveyQualifications/1595/084853e8-1b98-4828-9af8-15332e5fe165/" + surveyId,
                    headers: { 'Cache-Control': 'no-cache' },
                    success: function (qualifications) {
                        resolve(qualifications[0].SurveyQualifications);
                    },
                    error: function (result) {
                        reject();
                    }
                });
            });
        }
        function getQualifications(surveyId) {
            return new Promise((resolve, reject) => {
                $.ajax({
                    type: "GET",
                    url: "https://api.sample-cube.com/api/Survey/GetSupplierSurveyQualifications/1595/084853e8-1b98-4828-9af8-15332e5fe165/" + surveyId,
                    headers: { 'Cache-Control': 'no-cache' },
                    success: function (qualifications) {
                        resolve(qualifications[0].SurveyQualifications);
                    },
                    error: function (result) {
                        reject();
                    }
                });
            });
        }

        function TestAPI() {
            $.blockUI({ message: '<h5>Please wait</h5>' });
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'AddProject.aspx/TestAPI',
                //data: JSON.stringify({ ProjectId: projectId, CPI: cpi, LOI: loi, IR: ir, LiveLink: livelink }),
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

