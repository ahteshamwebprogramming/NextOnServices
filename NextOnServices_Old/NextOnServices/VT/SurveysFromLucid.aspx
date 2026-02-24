<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SurveysFromLucid.aspx.cs" EnableEventValidation="false" Inherits="VT_SurveysFromLucid" %>

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

        .actiontd {
            padding: 1px 1px 1px 16px !important;
        }

        .card {
            border: 1px solid;
            box-shadow: 1px 1px 1px gray;
            margin-top: 5px;
        }

        #qualificationmodel table tbody tr td:nth-child(1) {
            width: 25%;
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
                                        <input style="display: none" type="button" value="modal" data-toggle="modal" data-target="#qualificationmodel" id="btnqualificationmodel" />
                                        <%--<input type="button" value="Test API" class="btn btn-primary" id="btnTestAPI" />--%>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="form-wrap">
                                    <div style="overflow-x: scroll">
                                        <table id="tbl" class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th>Project ID</th>
                                                    <th>Survey ID</th>
                                                    <th>Language</th>
                                                    <th>Study Type</th>
                                                    <th>Industry Type</th>
                                                    <th>Survey Status</th>

                                                    <th>Total Remaining</th>
                                                    <th>IR</th>
                                                    <th>CPI</th>
                                                    <th>LOI</th>

                                                    <th>Mobile Allowed</th>
                                                    <th>Non-Mobile Allowed</th>
                                                    <th>Tablet Allowed</th>
                                                    <th>Manual Inc.</th>
                                                    <th>PID Enabled</th>
                                                    <th>Quota Level CPI</th>
                                                    <th>Survey Group</th>
                                                    <th>New Sample Source</th>
                                                    <th>Live Link</th>
                                                    <th>Test Link</th>
                                                    <th>URL Type</th>

                                                    <th>Recommended</th>
                                                    <th>Action</th>
                                                    <%--<th>ExternalName</th>--%>
                                                    <%--<th>CollectPII</th>--%>
                                                    <%--<th>Quota_UpdateTimeStamp</th>--%>
                                                    <%--<th>Qual_UpdateTimeStamp</th>--%>
                                                    <%--<th>RefreshSample</th>--%>
                                                    <%--<th>SurveyGuId</th>--%>
                                                    <%--<th>UpdateTimeStamp</th>--%>
                                                    <%--<th>AccountId</th>--%>
                                                    <%--<th>Group_UpdateTimeStamp</th>--%>
                                                    <%--<th>SurveyScore</th>--%>
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

        <div class="modal" tabindex="-1" role="dialog" id="qualificationmodel">
            <div class="modal-dialog modal-lg" style="width: 70%" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" style="display: inline">Qualifications</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">

                        <div class="row">
                            <div class="col-md-12">
                                <div class="card">
                                    <div class="card-body">
                                        <div id="Qualifications_12">
                                            <table class="table table-striped">
                                                <tbody>
                                                    <tr>
                                                        <td>Question Text</td>
                                                        <td>: What is your age?</td>
                                                    </tr>
                                                    <tr>
                                                        <td>Type</td>
                                                        <td>: Numeric - Open-end</td>
                                                    </tr>
                                                    <tr>
                                                        <td>Allowed Answer Options</td>
                                                        <td>25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65</td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <input type="hidden" name="surveyId" value="" />
                        <button type="button" class="btn btn-primary" onclick="AccceptProject()">Add Project</button>
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>


    </form>
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.blockUI/2.70/jquery.blockUI.min.js"></script>


    <script type="text/javascript">
        let QualificationTypes;
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
            $("#btnTestAPI").click(function () {

                TestAPI();
            });
        });

        function pullSurveysFromLucidAPI() {
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
                    }),
                    $.ajax({
                        type: 'GET',
                        url: "https://api.sample-cube.com///api/Definition/GetQualificationTypes/1595/084853e8-1b98-4828-9af8-15332e5fe165",
                        headers: { 'Cache-Control': 'no-cache', }
                    })
                ).then(function (surveys, languages, industries, studytypes, surveyStatuses, redirectTypes, qualificationTypes) {
                    var returnData = {};
                    returnData["Surveys"] = surveys[0].Surveys;
                    returnData["Languages"] = languages[0].Languages;
                    returnData["Industries"] = industries[0].Industries;
                    returnData["StudyTypes"] = studytypes[0].StudyTypes;
                    returnData["SurveyStatuses"] = surveyStatuses[0].SurveyStatuses;
                    returnData["RedirectTypes"] = redirectTypes[0].RedirectTypes;
                    returnData["QualificationTypes"] = qualificationTypes[0].QualificationTypes;
                    resolve(returnData);
                }).fail(function (err) {
                    alert(err.responseText)
                });
            });
        }

        function pullSurveysFromLucid() {
            $("#tbl tbody").empty();
            pullSurveysFromLucidAPI().then((data) => {
                var Surveys = data.Surveys;
                var Languages = data.Languages;
                var Industries = data.Industries;
                var StudyTypes = data.StudyTypes;
                var SurveyStatuses = data.SurveyStatuses;
                var RedirectTypes = data.RedirectTypes;
                QualificationTypes = data.QualificationTypes;

                for (var i = 81; i < Surveys.length; i++) {
                    let Language = Languages.filter(x => x.LanguageId == Surveys[i].LanguageId)[0].LanguageCode;
                    let Industry = Industries.filter(x => x.IndustryId == Surveys[i].IndustryId).length > 0 ? Industries.filter(x => x.IndustryId == Surveys[i].IndustryId)[0].Description : "";
                    let UrlType = RedirectTypes.filter(x => x.RedirectTypeId == Surveys[i].UrlTypeId).length > 0 ? RedirectTypes.filter(x => x.RedirectTypeId == Surveys[i].UrlTypeId)[0].Description : "";
                    let StudyType = StudyTypes.filter(x => x.StudyTypeId == Surveys[i].StudyTypeId)[0].Description;
                    let SurveyStatus = SurveyStatuses.filter(x => x.SurveyStatusId == Surveys[i].SurveyStatusId)[0].Description;

                    console.log(i + "- Language : " + Language + " ,Industry : " + Industry + " ,StudyType : " + StudyType + " ,SurveyStatus : " + SurveyStatus + " ,UrlType : " + UrlType);
                }

                Surveys.forEach((v, i) => {

                    let Language = Languages.filter(x => x.LanguageId == v.LanguageId)[0].LanguageCode;
                    let Industry = Industries.filter(x => x.IndustryId == v.IndustryId).length > 0 ? Industries.filter(x => x.IndustryId == v.IndustryId)[0].Description : "";
                    let UrlType = RedirectTypes.filter(x => x.RedirectTypeId == v.UrlTypeId).length > 0 ? RedirectTypes.filter(x => x.RedirectTypeId == v.UrlTypeId)[0].Description : "";
                    let StudyType = StudyTypes.filter(x => x.StudyTypeId == v.StudyTypeId)[0].Description;
                    let SurveyStatus = SurveyStatuses.filter(x => x.SurveyStatusId == v.SurveyStatusId)[0].Description;


                    /*  try {*/
                    $("#tbl tbody").append("<tr surveyId='" + v.SurveyId + "'><td>" + v.ProjectId + "</td><td surveyId='" + v.SurveyId + "'>" + v.SurveyId + "</td><td languageId='" + v.LanguageId + "'>" + Language + "</td><td>" + StudyType + "</td><td>" + Industry + "</td><td>" + SurveyStatus + "</td><td totalRemaining='" + v.TotalRemaining + "'>" + v.TotalRemaining + "</td><td ir='" + v.IR + "'>" + v.IR + "</td><td cpi='" + v.CPI + "'>" + v.CPI + "</td><td loi='" + v.LOI + "'>" + v.LOI + "</td><td>" + v.IsMobileAllowed + "</td><td>" + v.IsNonMobileAllowed + "</td><td>" + v.IsTabletAllowed + "</td><td>" + v.IsManualInc + "</td><td>" + v.IsPIDEnabled + "</td><td>" + v.IsQuotaLevelCPI + "</td><td>" + v.IsSurveyGroupExist + "</td><td>" + v.NewSampleSource + "</td><td livelink='" + v.LiveLink + "'>" + v.LiveLink + "</td><td>" + v.TestLink + "</td><td>" + UrlType + "</td><td>" + v.Recommended + "</td><td class='actiontd'><input type='button' class='btn btn-primary btn-sm' value='Add Project' onclick=\"addToProject('" + v.SurveyId + "','" + v.CPI + "','" + v.LOI + "','" + v.IR + "','" + v.TotalRemaining + "','" + v.LiveLink + "')\" /><input type='button' style='margin-left:5px;' class='btn btn-primary btn-sm' value='Get Qualifications' onclick=\"getQualifications('" + v.SurveyId + "','" + v.LanguageId + "')\" /></td></tr>");
                    //}
                    //catch {
                    //console.log(i + "- Language : " + Language + " ,Industry : " + Industry + " ,StudyType : " + StudyType + " ,SurveyStatus : " + SurveyStatus + " ,UrlType : " + UrlType);
                    //    console.log("error");
                    //}
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
        function AccceptProject() {
            let surveyId = $("#qualificationmodel").find("[name='surveyId']").val();

            let cpi = $("#tbl tbody").find("tr[surveyId='" + surveyId + "']").find("td[cpi]").attr("cpi")
            let loi = $("#tbl tbody").find("tr[surveyId='" + surveyId + "']").find("td[loi]").attr("loi")
            let ir = $("#tbl tbody").find("tr[surveyId='" + surveyId + "']").find("td[ir]").attr("ir")
            let totalRemaining = $("#tbl tbody").find("tr[surveyId='" + surveyId + "']").find("td[totalRemaining]").attr("totalRemaining")
            let livelink = $("#tbl tbody").find("tr[surveyId='" + surveyId + "']").find("td[livelink]").attr("livelink")
            addToProject(surveyId, cpi, loi, ir, totalRemaining, livelink);

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



        function getQualifications(surveyId, languageId) {
            $("#qualificationmodel").find(".modal-body").empty();
            $.blockUI({ message: '<h5>Please wait</h5>' });
            getQualificationsFromAPI(surveyId).then((qualifications) => {
                getBundledQualificationsFromAPI(languageId).then((bundledQualifications) => {

                    let Qualshtml = "";
                    qualifications.forEach((v, i) => {
                        let Quals = bundledQualifications.filter(x => x.QualificationId == v.QualificationId)[0];
                        let ansHTML = "";
                        if (v.QualificationTypeId == 1 || v.QualificationTypeId == 2) {
                            let qualAnswers = Quals.QualificationAnswers;
                            //qualAnswers.forEach((ansV, ansI) => {
                            //    ansHTML += ansV.Text + ", ";
                            //});
                            let answerIds = v.AnswerIds;

                            answerIds.forEach((answerValue, answerIndex) => {
                                ansHTML += qualAnswers.filter(x => x.AnswerId == answerValue)[0].Text + " ,";
                            });



                        } else {
                            ansHTML = v.AnswerIds[0];
                        }


                        let lQualificationType = QualificationTypes.filter(x => x.QualificationTypeId == Quals.QualificationTypeId)[0].Description;

                        Qualshtml += '<div class="row"><div class="col-md-12"><div class="card"><div class="card-body"><div id="Qualifications_' + Quals.QualificationId + '"><table class="table table-striped"><tbody><tr><td>Question Text</td><td>: ' + Quals.Text + '</td></tr><tr><td>Type</td><td>: ' + lQualificationType + '</td></tr><tr><td>Allowed Answer Options</td><td>' + ansHTML + '</td></tr></tbody></table></div></div></div></div></div>';

                    });


                    $("#qualificationmodel").find("[name='surveyId']").val(surveyId);
                    $("#qualificationmodel").find(".modal-body").append(Qualshtml);
                    $("#btnqualificationmodel").click();
                    $.unblockUI();
                });
            });
        }
        function getQualificationsFromAPI(surveyId) {
            return new Promise((resolve, reject) => {
                $.ajax({
                    type: "GET",
                    url: "https://api.sample-cube.com/api/Survey/GetSupplierSurveyQualifications/1595/084853e8-1b98-4828-9af8-15332e5fe165/" + surveyId,
                    headers: { 'Cache-Control': 'no-cache' },
                    success: function (qualifications) {
                        resolve(qualifications.SurveyQualifications);
                    },
                    error: function (result) {
                        reject();
                    }
                });
            });
        }
        function getBundledQualificationsFromAPI(languageId) {
            return new Promise((resolve, reject) => {
                $.ajax({
                    type: "GET",
                    url: "https://api.sample-cube.com/api/Definition/GetBundledQualificationAnswers/1595/084853e8-1b98-4828-9af8-15332e5fe165/" + languageId,
                    headers: { 'Cache-Control': 'no-cache' },
                    success: function (bundledQualifications) {
                        resolve(bundledQualifications.Qualifications);
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

        function pullQualificationsDataFromAPI(surveyId, languageId) {
            return new Promise((resolve, reject) => {
                $.when(
                    $.ajax({
                        type: 'GET',
                        url: "https://api.sample-cube.com/api/Survey/GetSupplierSurveyQualifications/1595/084853e8-1b98-4828-9af8-15332e5fe165/" + surveyId,
                        headers: { 'Cache-Control': 'no-cache', }
                    }),
                    $.ajax({
                        type: 'GET',
                        url: "https://api/Definition/GetBundledQualificationAnswers/1595/084853e8-1b98-4828-9af8-15332e5fe165/" + languageId,
                        headers: { 'Cache-Control': 'no-cache', }
                    })
                ).then(function (surveyQualifications, languages, industries, studytypes, surveyStatuses, redirectTypes) {
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


    </script>
</body>
</html>
