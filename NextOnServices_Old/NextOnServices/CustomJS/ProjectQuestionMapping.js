var Questions = [];

//import { Session } from "inspector";

function Pageinit() {
    $('#Wrap_Tables').hide();

    $('#ddlProjects').dropdown();
    PopulateProjects();
    PopulateCountry();
    PopulateProjectQuestioner();
    $("#ddlProjects").change(function () {
        if ($('#ddlProjects').val() != 0) {
            jQuery.blockUI({ message: '<h1 style="font-size: 25px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' });//).ajaxStop(jQuery.unblockUI);
            PopulateQuestions(this.value);
            $('#Wrap_Tables').show();
        }
        else {
            $('#Wrap_Tables').hide();
        }
    });
    $('#btnSubmit').click(function () {
        ManageSubmit();
    });

}
function SetHiddenFildsForControls() {
    if ($('#ShowButton').is(":checked"))
        $('#ctrl_PreviousButton').val("1");
    else
        $('#ctrl_PreviousButton').val("0");
    if ($('#ShowQuestion').is(":checked"))
        $('#ctrl_QuestionQID').val("1");
    else
        $('#ctrl_QuestionQID').val("0");
    if ($('#ShowLogo').is(":checked"))
        $('#ctrl_Logo').val("1");
    else
        $('#ctrl_Logo').val("0");

    ManageQuestions();
}
function PopulateProjectQuestioner() {
    if (sessionStorage.getItem("ProjectID") != 0) {
        jQuery.blockUI({ message: '<h1 style="font-size: 25px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' });//).ajaxStop(jQuery.unblockUI);
        PopulateQuestions(sessionStorage.getItem("ProjectID"));
        $('#Wrap_Tables').show();
    }
    else {
        $('#Wrap_Tables').hide();
    }
}
function PopulateProjects() {
    jQuery.blockUI({ message: '<h1 style="font-size: 25px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' });//).ajaxStop(jQuery.unblockUI);
    // var formdata = JSON.stringify({});
    //var ProjectID = JSON.stringify({ 1057 });
    PostRequest('ProjectQuestionMapping.aspx/GetProjects', JSON.stringify({ ProjectID: sessionStorage.getItem("ProjectID") }), ManageGetProjectsServerResponse, 'Post');
}
function PopulateCountry() {
    $("#ddlCountry").empty();
    $("#ddlCountry").append('<option value="' + sessionStorage.getItem("CountryID") + '">' + sessionStorage.getItem("CountryName") + '</option>');
    $('#ddlCountry').dropdown();
}
function ManageGetProjectsServerResponse(data) {
    $("#ddlProjects").empty();
    if (data.d.length > 0) {
        //$("#ddlProjects").append('<option value="0">--Search Project--</option>');
        for (var i = 0; i < data.d.length; i++) {
            $("#ddlProjects").append('<option value="' + data.d[i].ID + '">' + data.d[i].PName + '</option>');
        }
        $('#ddlProjects').dropdown();
        jQuery.unblockUI();
    }
    else {
    }
}
function PopulateQuestions(projectid) {
    var formdata = {};
    var formData;
    formdata["ProjectID"] = projectid;
    formdata["CountryID"] = sessionStorage.getItem("CountryID");
    // formData = JSON.stringify({ ProjectID: projectid });
    // var data = JSON.stringify({ formData: formdata });
    PostRequest('ProjectQuestionMapping.aspx/GetQuestions', JSON.stringify({ formData: formdata }), ManageGetQuestionsServerResponse, 'Post');
    PostRequest('ProjectQuestionMapping.aspx/GetControls', JSON.stringify({ formData: formdata }), ManageGetControlsServerResponse, 'Post');
}
function ManageGetQuestionsServerResponse(data) {
    $('#tblQuestionList tbody').empty();
    var data = data.d;
    if (data.length > 0) {
        var table = '';
        var chk = '';
        var optioncolumn = '';
        for (var i = 0; i < data.length; i++) {
            if (data[i].QuestionType == 0) {
                chk = '<label class="container1_ppd"><input onclick="QuestionsCheckBoxclicked(this)" id=ctrlchk_' + data[i].ID + ' type="checkbox" ><span class="checkmark_ppd"></span></label>';
                optioncolumn = '<td></td>';
                Questions.push([0, data[i].ID, data[i].QuestionID, data[i].QuestionLabel]);
            }
            else {
                chk = '<label class="container1_ppd"><input onclick="QuestionsCheckBoxclicked(this)" id=ctrlchk_' + data[i].ID + ' type="checkbox" checked="checked" ><span class="checkmark_ppd"></span></label>';
                optioncolumn = ' <td> <input onclick="OptionSelect(this)" type="button" id="' + data[i].ID + '" class="btn btn-sm btn-info"  value="View Option" /></td>  ';
                Questions.push([1, data[i].ID, data[i].QuestionID, data[i].QuestionLabel]);
            }
            table = table + '<tr><td>' + chk + '</td><td>' + data[i].QuestionID + '</td><td>' + data[i].QuestionLabel + '</td></tr>';
        }
        $('#tblQuestionList tbody').append(table);
    }
    jQuery.unblockUI();
    FetchOptions();
}
function ManageGetControlsServerResponse(data) {
    var data = data.d;
    if (data.length > 0) {
        if (data[0].Logo == 1) {
            $('#ShowLogo').prop('checked', true);
            $('#ctrl_Logo').val("1");
        }
        if (data[0].QuestionQID == 1) {
            $('#ShowQuestion').prop('checked', true);
            $('#ctrl_QuestionQID').val("1");
        }
        if (data[0].PreviousButton == 1) {
            $('#ShowButton').prop('checked', 'checked');
            $('#ctrl_PreviousButton').val("1");
        }
    }
    else {
        $('#ShowLogo').prop('checked', false);
        $('#ctrl_Logo').val("0");
        $('#ShowQuestion').prop('checked', false);
        $('#ctrl_QuestionQID').val("0");
        $('#ShowButton').prop('checked', false);
        $('#ctrl_PreviousButton').val("0");
    }
}
function ManageSubmit() {
    jQuery.blockUI({ message: '<h1 style="font-size: 25px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' });
    var formdata = {};
    formdata = GetFormData();
    var formData;
    var selectedChkbx = [];
    var allCtrl = jQuery("[id^=ctrlchk_]");
    allCtrl.each(function (i) {
        var curCtrl = jQuery(this);
        if (curCtrl.is(":checked")) {
            selectedChkbx.push(curCtrl.attr('id').split("_")[1]);
            Questions[curCtrl.attr('id').split("_")[1]][0] = 1;
        }
    });
    //alert(selectedChkbx.toString());
    formdata["ProjectID"] = $('#ddlProjects').val();
    formdata["CountryID"] = $('#ddlCountry').val();
    formdata["QuestionIDS"] = selectedChkbx.toString(',');
    //formdata["PreviousButton"] = 0;
    //formdata["QuestionQID"] = 0;
    //formdata["Logo"] = 0;
    formdata["opt"] = 0;
    PostRequest('ProjectQuestionMapping.aspx/MapQuestions', JSON.stringify({ formData: formdata }), ManageMapQuestionsServerResponse, 'Post');
}
function ManageMapQuestionsServerResponse(data) {
    var data = data.d;
    if (data.length > 0) {
        if (data[0].RetStat == "Error while Updating" || data[0].RetStat == "Error while Mapping") {
            alertt('', data[0].RetStat, 'error');
        }
        else {
            alertt('', data[0].RetStat, 'success');
            //$("#ddlProjects").dropdown('set selected', '0');//    val('0');
            //$('#tblQuestionList tbody').empty();
        }
    }
    jQuery.unblockUI();
    FetchOptions();
}


//$('#btnNext').click(function () {

//    $('#tableSlideTest1').slideLeft();
//    $('#tableSlideTest2').slideRight();
//});

//$('#btnPrevious').click(function () {
//    $('#tableSlideTest1').slideRight();
//    $('#tableSlideTest2').slideLeft();

//});


$('#btnNext').click(function () {
    //PopulateQuestionDetailed();
    FetchOptions();
    var hidden = $('#tableSlideTest1');
    var hidden1 = $('#tableSlideTest2');
    //if (hidden.hasClass('visible')) {
    //    hidden.animate({ "left": "-120%" }, "slow").removeClass('visible');
    //} else {
    hidden.animate({ "left": "-120%" }, "slow").addClass('visible');
    hidden1.animate({ "left": "1%" }, "slow").addClass('visible');
    //}
});

$('#btnPrevious').click(function () {
    var hidden = $('#tableSlideTest1');
    var hidden1 = $('#tableSlideTest2');
    //if (hidden.hasClass('visible')) {
    //    hidden.animate({ "left": "-120%" }, "slow").removeClass('visible');
    //    hidden1.animate({ "right": "1%" }, "slow").addClass('visible');
    //} else {
    hidden.animate({ "left": "0%" }, "slow").addClass('visible');
    hidden1.animate({ "left": "110%" }, "slow").addClass('visible');
    //}
});

function PopulateQuestionDetailed() {
    var QuestionsSelected = Questions.filter(e => e[0] == 1);
    $('#QuestionsDetailed tbody').empty(table);
    if (Questions.length > 0) {
        var table = '';
        for (var i = 0; i < QuestionsSelected.length; i++) {
            table += '<tr> <td>' + QuestionsSelected[i][2] + '</td> <td>' + QuestionsSelected[i][3] + '</td> <td>  <input id="' + QuestionsSelected[i][0] + '" type="button" class="btn btn-primary btn-sm" value="View Options"  />  </td>  </tr>';
            //var optionss = Options.filter(e => e[1] == QuestionsSelected[i][1]);
            //for (var j = 0; j < optionss.length; j++) {
            //    table += '<tr> <td></td> <td></td> <td></td> <td>' + optionss[j][2] + '</td>  </tr>';
            //}
        }
        $('#QuestionsDetailed tbody').append(table);
    }
}


function FetchOptions() {
    var QuestionsId = FindArray(1, Questions.filter(e => e[0] == 1)).toString(',');
    var formData = {};
    formData["ProjectID"] = $('#ddlProjects').val();
    formData["CountryID"] = $('#ddlCountry').val();
    formData["QuestionsIds"] = QuestionsId;
    formData["opt"] = 0;
    PostRequest('ProjectQuestionMapping.aspx/FetchOptions', JSON.stringify({ formData }), ManageFetchOptionsServerResponse, 'Post');
}

function FindArray(index, Arr) {
    var arrou = [];
    for (var i = 0; i < Arr.length; i++) {
        arrou.push(Arr[i][index]);
    }
    return arrou;
}
var Options = [];
function ManageFetchOptionsServerResponse(data) {
    Options = [];
    if (data.d != null) {
        if (data.d.length > 0) {
            for (var i = 0; i < data.d.length; i++) {
                Options.push([data.d[i].Id, data.d[i].QID, data.d[i].OptionLabel, data.d[i].OptionCode, data.d[i].Logic, data.d[i].Quota]);
            }
        }
    }
    //PopulateQuestionDetailed();
}
//<label class="container1_ppd"><input type="checkbox" id="textcheckbox" /><span class="checkmark_ppd"></span></label>
function OptionSelect(obj) {
    $('#btnModalOptionSelect').click();
    $('#pErrorForOptionQuota').hide();
    var table = '';
    $('#lblQuestionId').text(Questions.filter(e => e[1] == obj.id)[0][2]);
    $('#lblQuestionLabel').text(Questions.filter(e => e[1] == obj.id)[0][3]);
    $('#OptionsList tbody').empty();
    var optionss = Options.filter(e => e[1] == obj.id);
    if (optionss.length > 0) {
        for (var i = 0; i < optionss.length; i++) {
            var LogicDropDown = '';
            var QuotaStyle = 'style="display:none"';
            if (Options.filter(e => e[4] == optionss[i][4])[0][4] == 0)
                LogicDropDown = '<select onchange="LogicDropdownchanged(this)" class="form-control LogicDropDown dropdown search selection"><option selected="selected" value="0">Select</option><option value="1">Terminate</option><option value="2">Quota</option></select>';
            else if (Options.filter(e => e[4] == optionss[i][4])[0][4] == 1)
                LogicDropDown = '<select onchange="LogicDropdownchanged(this)" class="form-control LogicDropDown dropdown search selection"><option value="0">Select</option><option selected="selected" value="1">Terminate</option><option value="2">Quota</option></select>';
            else if (Options.filter(e => e[4] == optionss[i][4])[0][4] == 2) {
                LogicDropDown = '<select onchange="LogicDropdownchanged(this)" class="form-control LogicDropDown dropdown search selection"><option value="0">Select</option><option  value="1">Terminate</option><option selected="selected" value="2">Quota</option></select>';
                QuotaStyle = '';
            }
            table += '<tr id="' + optionss[i][0] + '"> <td>' + optionss[i][3] + '</td>    <td>' + optionss[i][2] + '</td>  <td>' + LogicDropDown + '</td>   <td>  <input type="text" class="form-control" ' + QuotaStyle + ' value=' + optionss[i][5] + ' /> </td>  </tr> ';
        }
    }
    else {
        table += '<td>No Records found</td>';
    }
    $('#OptionsList tbody').append(table);
}

function OptionCheckboxClicked(obj) {
    var id = obj.id.split('_')[1];
    if ($('#' + obj.id).is(":checked"))
        Options.filter(e => e[0] == id)[0][4] = 1;
    else
        Options.filter(e => e[0] == id)[0][4] = 0;


    //Options.filter(e => e[0] == obj.id)[0][4] = 1;
    ManageOptions(Options.filter(e => e[0] == id)[0][1]);
}



$('#btnSaveOptions').click(function () {

});



function QuestionsCheckBoxclicked(obj) {
    var id = obj.id.split('ctrlchk_')[1];
    if ($('#' + obj.id).is(":checked"))
        Questions.filter(e => e[1] == id)[0][0] = 1;
    else
        Questions.filter(e => e[1] == id)[0][0] = 0;
    //ManageQuestions();

}

function QuestionsTableRedraw() {
    data = Questions;
    $('#tblQuestionList tbody').empty();
    if (data.length > 0) {
        var table = '';
        var chk = '';
        var optioncolumn = '';
        for (var i = 0; i < data.length; i++) {
            if (data[i][0] == 0) {
                chk = '<label class="container1_ppd"><input onclick="QuestionsCheckBoxclicked(this)" id=ctrlchk_' + data[i][1] + ' type="checkbox" ><span class="checkmark_ppd"></span></label>';
                optioncolumn = '<td></td>';
                //Questions.push([0, data[i].ID, data[i].QuestionID, data[i].QuestionLabel]);
            }
            else {
                chk = '<label class="container1_ppd"><input onclick="QuestionsCheckBoxclicked(this)" id=ctrlchk_' + data[i][1] + ' type="checkbox" checked="checked" ><span class="checkmark_ppd"></span></label>';
                optioncolumn = ' <td> <input onclick="OptionSelect(this)" type="button" id="' + data[i][1] + '" class="btn btn-sm btn-info"  value="View Option" /></td>  ';
                //Questions.push([1, data[i].ID, data[i].QuestionID, data[i].QuestionLabel]);
            }
            table = table + '<tr><td>' + chk + '</td><td>' + data[i][2] + '</td><td>' + data[i][3] + '</td>' + optioncolumn + '</tr>';
        }
        $('#tblQuestionList tbody').append(table);
    }
}
function QuestionsTableRedrawForOptions() {
    data = Questions;
    $('#tblSelectedQuestionsList tbody').empty();
    if (data.length > 0) {
        var table = '';
        var chk = '';
        var optioncolumn = '';
        for (var i = 0; i < data.length; i++) {
            if (data[i][0] != 0) {
                table = table + '<tr><td>' + data[i][2] + '</td><td>' + data[i][3] + '</td><td><input onclick="OptionSelect(this)" type="button" id="' + data[i][1] + '" class="btn btn-sm btn-info"  value="View Option" /></td></tr>';
            }
        }
        //<input onclick="OptionSelect(this)" type="button" id="' + data[i][1] + '" class="btn btn-sm btn-info"  value="View Option" />
        $('#tblSelectedQuestionsList tbody').append(table);
        $('.LogicDropDown').dropdown();
    }
}
function ManageQuestions() {
    // jQuery.blockUI({ message: '<h1 style="font-size: 25px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' });
    var formdata = {};
    formdata = GetFormData();
    var formData;
    var selectedChkbx = [];
    //var allCtrl = jQuery("[id^=ctrlchk_]");
    //allCtrl.each(function (i) {
    //    var curCtrl = jQuery(this);
    //    if (curCtrl.is(":checked")) {
    //        selectedChkbx.push(curCtrl.attr('id').split("_")[1]);
    //        Questions[curCtrl.attr('id').split("_")[1]][0] = 1;
    //    }
    //});
    formdata["ProjectID"] = $('#ddlProjects').val();
    formdata["CountryID"] = $('#ddlCountry').val();
    formdata["QuestionIDS"] = FindArray(1, Questions.filter(e => e[0] == 1)).toString(',');
    formdata["opt"] = 0;
    PostRequest('ProjectQuestionMapping.aspx/MapQuestions', JSON.stringify({ formData: formdata }), ManageQuestionsServerResponse, 'Post');
}
function ManageQuestionsServerResponse() {
    //QuestionsTableRedraw();
    QuestionsTableRedrawForOptions();
    $('#AllQuestionsList').slideUp();
    $('#SelectedQuestionList').slideDown();
}

$('#btnSaveAndNext').click(function () {
    ManageQuestions();
    //$('#AllQuestionsList').slideUp();
    //$('#SelectedQuestionList').slideDown();
});

$('#btnBackToQuestionList').click(function () {
    $('#AllQuestionsList').slideDown();
    $('#SelectedQuestionList').slideUp();

});
$('.btnBackToProjectMapping').click(function () {
    window.location.href = 'ProjectPageDetails.aspx?ID=' + sessionStorage.getItem('ProjectID');
});

function LogicDropdownchanged(obj) {
    if ($(obj).val() == 2) {
        $(obj).parent().parent().find('input[type="text"]').show();
    }
    else
        $(obj).parent().parent().find('input[type="text"]').hide();


    //   ManageOptions(Options.filter(e => e[0] == id)[0][1]);
}


function ManageOptions(QuestionId) {
    // jQuery.blockUI({ message: '<h1 style="font-size: 25px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' });
    var formdata = {};
    //formdata = GetFormData();
    var formData;
    var selectedChkbx = [];
    //var allCtrl = jQuery("[id^=ctrlchk_]");
    //allCtrl.each(function (i) {
    //    var curCtrl = jQuery(this);
    //    if (curCtrl.is(":checked")) {
    //        selectedChkbx.push(curCtrl.attr('id').split("_")[1]);
    //        Questions[curCtrl.attr('id').split("_")[1]][0] = 1;
    //    }
    //});
    formdata["ProjectID"] = $('#ddlProjects').val();
    formdata["CountryID"] = $('#ddlCountry').val();
    //formdata["QuestionIDS"] = FindArray(0, Options.filter(e => e[1] == QuestionId)).toString(',');
    formdata["QuestionIDS"] = FindArray(0, Options.filter(e => e[4] == 1)).toString(',');
    //formdata["QuestionIDS"] = FindArray(0, Options).toString(',');



    formdata["opt"] = 2;
    PostRequest('ProjectQuestionMapping.aspx/MapOptions', JSON.stringify({ formData: formdata }), ManageOptionsServerResponse, 'Post');
}
function ManageOptionsServerResponse() {
    QuestionsTableRedraw();
}

$('#btnSaveOptions').click(function () {
    formdata = [];
    $('#pErrorForOptionQuota').hide();
    $('#OptionsList tbody').find('tr').each(function () {
        var data = {};
        data["PID"] = $('#ddlProjects').val();
        data["CID"] = $('#ddlCountry').val();
        data["OptionId"] = $(this)[0].id;
        data["Logic"] = 0;
        data["Quota"] = 0;
        data["opt"] = 3;
        Options.find(e => e[0] == $(this)[0].id)[5] = 0;
        Options.find(e => e[0] == $(this)[0].id)[4] = 0;
        if ($(this).find('select').val() == 2) {
            data["Logic"] = 2;
            Options.find(e => e[0] == $(this)[0].id)[4] = 2;
            if ($.trim($(this).find('input[type="text"]').val()) == '') {
                $('#pErrorForOptionQuota').text('Please insert Quota');
                $('#pErrorForOptionQuota').show();
                return;
            }
            else {
                data["Quota"] = $.trim($(this).find('input[type="text"]').val());
                Options.find(e => e[0] == $(this)[0].id)[5] = data["Quota"];
            }
        }
        else if ($(this).find('select').val() == 1) {
            data["Logic"] = 1;
            Options.find(e => e[0] == $(this)[0].id)[4] = 1;
        }
        formdata.push(data);
    });

    PostRequest('ProjectQuestionMapping.aspx/MapOptions1', JSON.stringify({ formData: formdata }), ManageOptionsServerResponse1, 'Post');

});
function ManageOptionsServerResponse1(data) {
    if (data.d == 0) {
        $('#pErrorForOptionQuota').show();
        $('#pErrorForOptionQuota').text('Some error has occured. Please check the internet connection and try again');
    }
    else if (data.d == 1) {
        $('#pErrorForOptionQuota').show();
        $('#pErrorForOptionQuota').text('Saved successfully');
    }
    else {
        $('#pErrorForOptionQuota').show();
        $('#pErrorForOptionQuota').text('Some error has occurred. Please contact the administrator');
    }
}