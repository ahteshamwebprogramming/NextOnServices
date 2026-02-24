<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SurveysFromSpectrum.aspx.cs" Inherits="VT_SurveysFromSpectrum" %>

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
                            <h5 class="txt-dark">Surveys From Pure Spectrum</h5>
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
                                        <input type="button" value="Test API" class="btn btn-primary" style="display: none" id="btnTestAPI" />
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="form-wrap">
                                    <div style="overflow-x: scroll">

                                        <table id="tbl" class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th>survey_id</th>
                                                    <th>survey_name</th>
                                                    <%--<th>buyer_id</th>
                                                    <th>category</th>
                                                    <th>category_code</th>--%>
                                                    <%--<th>channel_parent</th>--%>
                                                    <%--<th>channel_type</th>--%>
                                                    <%--<th>click_balancing</th>--%>
                                                    <th>cpi</th>
                                                    <th>crtd_on</th>
                                                    <th>field_end_date</th>
                                                    <%--<th>incl_excl</th>--%>
                                                    <th>last_complete_date</th>
                                                    <%--<th>link_security</th>--%>
                                                    <%--<th>mod_on</th>--%>
                                                    <th>soft_launch</th>
                                                    <th>supplier_completes : achieved</th>
                                                    <th>supplier_completes : guaranteed_allocation</th>
                                                    <th>supplier_completes : guaranteed_allocation_remaining</th>
                                                    <th>supplier_completes : needed</th>
                                                    <th>supplier_completes : remaining</th>
                                                    <th>surveyLocalization</th>
                                                    <th>survey_grouping : exclusion_period</th>
                                                    <%--<th>survey_grouping : survey_ids</th>--%>


                                                    <%--<th>survey_performance : last_block : bdp</th>
                                                    <th>survey_performance : last_block : bqtp</th>
                                                    <th>survey_performance : last_block : bstp</th>
                                                    <th>survey_performance : last_block : btp</th>
                                                    <th>survey_performance : last_block : ir</th>
                                                    <th>survey_performance : last_block : loi</th>
                                                    <th>survey_performance : last_block : oqp</th>--%>
                                                    <th>survey_performance : overall : ir</th>
                                                    <th>survey_performance : overall : loi</th>
                                                    <th>survey_status</th>
                                                    <th>Action</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                            </tbody>
                                        </table>

                                        <table id="tbl1" class="table table-bordered" style="display: none">
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
                pullSurveysFromAPI();
            });
            $("#btnTestAPI").click(function () {

                TestAPI();
            });
        });

        function pullSurveysFromSpectrumAPI() {
            return new Promise((resolve, reject) => {
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'SurveysFromSpectrum.aspx/GetSurveys',
                    //data: JSON.stringify({ ProjectId: projectId, CPI: cpi, LOI: loi, IR: ir, LiveLink: livelink }),
                    cache: false,
                    dataType: "json",
                    success: function (data) {
                        let res = JSON.parse(data.d);
                        resolve(res.surveys);
                    },
                    error: function (result) {
                        reject();
                        $.unblockUI();
                    }
                });
            });
        }


        function pullSurveysFromSpectrumAPI1() {
            return new Promise((resolve, reject) => {
                $.when(
                    $.ajax({
                        type: 'GET',
                        //url: "https://api.spectrumsurveys.com/suppliers/v2/surveys",
                        url: "http://staging.spectrumsurveys.com/suppliers/v2/surveys",
                        headers: {
                            'Cache-Control': 'no-cache',
                            'access-token': 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjY1MDMyZTdjZjM5MDcxNDZkMjBjNmIyOCIsInVzcl9pZCI6IjM0MzciLCJpYXQiOjE2OTQ3MDczMjR9.92Ewak2OcZTbuFS5REq_Nwcw5iB6Psv43ZMEEBCGHGo'
                        }
                    })
                    //    ,
                    //$.ajax({
                    //    type: 'GET',
                    //    url: "https://api.sample-cube.com//api/Definition/GetLanguages/1595/084853e8-1b98-4828-9af8-15332e5fe165",
                    //    headers: { 'Cache-Control': 'no-cache', }
                    //}),
                    //$.ajax({
                    //    type: 'GET',
                    //    url: "https://api.sample-cube.com//api/Definition/GetIndustries/1595/084853e8-1b98-4828-9af8-15332e5fe165",
                    //    headers: { 'Cache-Control': 'no-cache', }
                    //}),
                    //$.ajax({
                    //    type: 'GET',
                    //    url: "https://api.sample-cube.com//api/Definition/GetStudyTypes/1595/084853e8-1b98-4828-9af8-15332e5fe165",
                    //    headers: { 'Cache-Control': 'no-cache', }
                    //}),
                    //$.ajax({
                    //    type: 'GET',
                    //    url: "https://api.sample-cube.com//api/Definition/GetSurveyStatuses/1595/084853e8-1b98-4828-9af8-15332e5fe165",
                    //    headers: { 'Cache-Control': 'no-cache', }
                    //}),
                    //$.ajax({
                    //    type: 'GET',
                    //    url: "https://api.sample-cube.com//api/Definition/GetRedirectTypes/1595/084853e8-1b98-4828-9af8-15332e5fe165",
                    //    headers: { 'Cache-Control': 'no-cache', }
                    //}),
                    //$.ajax({
                    //    type: 'GET',
                    //    url: "https://api.sample-cube.com///api/Definition/GetQualificationTypes/1595/084853e8-1b98-4828-9af8-15332e5fe165",
                    //    headers: { 'Cache-Control': 'no-cache', }
                    //})
                    //).then(function (surveys, languages, industries, studytypes, surveyStatuses, redirectTypes, qualificationTypes) {
                ).then(function (surveys) {


                    alert(surveys.msg);
                    var returnData = {};
                    returnData["Surveys"] = surveys[0].Surveys;
                    //returnData["Languages"] = languages[0].Languages;
                    //returnData["Industries"] = industries[0].Industries;
                    //returnData["StudyTypes"] = studytypes[0].StudyTypes;
                    //returnData["SurveyStatuses"] = surveyStatuses[0].SurveyStatuses;
                    //returnData["RedirectTypes"] = redirectTypes[0].RedirectTypes;
                    //returnData["QualificationTypes"] = qualificationTypes[0].QualificationTypes;
                    resolve(returnData);
                }).fail(function (err) {
                    alert(err.responseText)
                });
            });
        }

        function pullSurveysFromAPI() {
            pullSurveysFromSpectrumAPI().then((data) => {
                var Surveys = data;
                //var Languages = data.Languages;
                //var Industries = data.Industries;
                //var StudyTypes = data.StudyTypes;
                //var SurveyStatuses = data.SurveyStatuses;
                //var RedirectTypes = data.RedirectTypes;
                //QualificationTypes = data.QualificationTypes;

                $("#tbl tbody").empty();
                Surveys.forEach((v, i) => {

                    //let Language = Languages.filter(x => x.LanguageId == v.LanguageId)[0].LanguageCode;
                    //let Industry = Industries.filter(x => x.IndustryId == v.IndustryId)[0].Description;
                    //let StudyType = StudyTypes.filter(x => x.StudyTypeId == v.StudyTypeId)[0].Description;
                    //let SurveyStatus = SurveyStatuses.filter(x => x.SurveyStatusId == v.SurveyStatusId)[0].Description;
                    //let UrlType = RedirectTypes.filter(x => x.RedirectTypeId == v.UrlTypeId).length > 0 ? RedirectTypes.filter(x => x.RedirectTypeId == v.UrlTypeId)[0].Description : "";

                    //$("#tbl tbody").append("<tr><td>" + v.buyer_id + "</td><td>" + v.category + "</td><td>" + v.category_code + "</td><td languageId=''>" + v.channel_parent + "</td><td>" + v.channel_type + "</td><td>" + v.click_balancing + "</td><td>" + v.cpi + "</td><td totalRemaining=''>" + v.crtd_on + "</td><td ir=''>" + v.field_end_date + "</td><td cpi=''>" + v.incl_excl + "</td><td loi=''>" + v.last_complete_date + "</td><td>" + v.link_security + "</td><td>" + v.mod_on + "</td><td>" + v.soft_launch + "</td><td>" + v.supplier_completes.achieved + "</td><td>" + v.supplier_completes.guaranteed_allocation + "</td><td>" + v.supplier_completes.guaranteed_allocation_remaining + "</td><td>" + v.supplier_completes.needed + "</td><td>" + v.supplier_completes.remaining + "</td><td>" + v.surveyLocalization + "</td><td>" + v.survey_grouping.exclusion_period + "</td><td>" + v.survey_grouping.survey_ids + "</td><td>" + v.survey_id + "</td><td>" + v.survey_name + "</td><td>" + v.survey_performance.last_block.bdp + "</td><td>" + v.survey_performance.last_block.bqtp + "</td><td>" + v.survey_performance.last_block.bstp + "</td><td>" + v.survey_performance.last_block.btp + "</td><td>" + v.survey_performance.last_block.ir + "</td><td>" + v.survey_performance.last_block.loi + "</td><td>" + v.survey_performance.last_block.oqp + "</td><td>" + v.survey_performance.overall.ir + "</td><td>" + v.survey_performance.overall.loi + "</td><td>" + v.survey_status + "</td></tr>");
                    $("#tbl tbody").append("<tr><td>" + v.survey_id + "</td><td>" + v.survey_name + "</td><td>" + v.cpi + "</td><td>" + v.crtd_on + "</td><td ir=''>" + v.field_end_date + "</td><td loi=''>" + v.last_complete_date + "</td><td>" + v.soft_launch + "</td><td>" + v.supplier_completes.achieved + "</td><td>" + v.supplier_completes.guaranteed_allocation + "</td><td>" + v.supplier_completes.guaranteed_allocation_remaining + "</td><td>" + v.supplier_completes.needed + "</td><td>" + v.supplier_completes.remaining + "</td><td>" + v.surveyLocalization + "</td><td>" + v.survey_grouping.exclusion_period + "</td><td>" + v.survey_performance.overall.ir + "</td><td>" + v.survey_performance.overall.loi + "</td><td>" + (v.survey_status == 22 ? "Live" : v.survey_status == 33 ? "Paused" : v.survey_status == 44 ? "Closed" : "Other") + "</td><td class='actiontd'><input type='button' class='btn btn-primary btn-sm' value='Add Project' onclick=\"addToProject('" + v.survey_id + "','" + v.survey_name + "','" + v.cpi + "','" + v.survey_performance.overall.loi + "','" + v.survey_performance.overall.ir + "','" + v.supplier_completes.remaining + "','')\" /></td></tr>");
                });

                $("#tbl").DataTable();


                $.unblockUI();
            }).catch((err) => {
                $.unblockUI();
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
        function addToProject(projectId, projectName, cpi, loi, ir, totalRemaining, livelink) {
            $.blockUI({ message: '<h5>Please wait</h5>' });
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'AddProject.aspx/AddProjectFromSpectrumAPI',
                data: JSON.stringify({ ProjectId: projectId, ProjectName: projectName, CPI: cpi, LOI: loi, IR: ir, TotalRemaining: totalRemaining, LiveLink: livelink }),
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
                url: 'SurveysFromSpectrum.aspx/TestAPI',
                //data: JSON.stringify({ ProjectId: projectId, CPI: cpi, LOI: loi, IR: ir, LiveLink: livelink }),
                cache: false,
                dataType: "json",
                success: function (data) {
                    let res = JSON.parse(data.d);
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

