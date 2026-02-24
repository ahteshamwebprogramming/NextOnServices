
$(document).ready(function () {
    FetchCompleteRedirects();
    $('#a_GenerateLink').click(function () {
        $('#OpenCharacterLengthPopUp').click();
    });

    $("#txtChar").bind("paste change keyup keypress", function (evt) {
        //alert('e');
        //if (evt.type = "paste") {
        //    $("#txtChar")
        //}
        //else {

        //}
        evt = (evt) ? evt : window.event;
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode > 31 && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    });

    $('#btnGenerateLink').click(function () {
        var CharLimit = $('#txtChar').val();
        var formdata = {};
        var formData;
        formdata["Charl"] = CharLimit;
        //formdata["CountryID"] = sessionStorage.getItem("CountryID");
        PostRequest('Redirects.aspx/GenerateRandomLink', JSON.stringify({ formData: formdata }), ManageGenerateRandomLinkServerResponse, 'Post');
    });

    $('#btnSaveLink').click(function () {
        if ($('#txtRandomCode').val() == "") {
            alert('Cannot leave the code empty');
            return false;
        }
        var Code = $('#txtRandomCode').val();
        var formdata = {};
        var formData;
        formdata["Link"] = "https://nexton.us/VT/ProjectStatus.aspx?ID=[respondentID]&Status=" + Code + "&RC=0";
        formdata["Code"] = Code;

        //formdata["CountryID"] = sessionStorage.getItem("CountryID");
        PostRequest('Redirects.aspx/SaveCompleteLink', JSON.stringify({ formData: formdata }), ManageSaveCompleteLinkServerResponse, 'Post');
    });


});

function ManageGenerateRandomLinkServerResponse(data) {
    if (data.d != null) {
        if (data.d != "") {
            $('#div_GenerateLink').slideUp();
            $('#div_CheckLink').slideDown();
            $('#btnGenerateLink').hide();
            $('#btnSaveLink').show();
            $('#txtRandomCode').val(data.d);
        }
        else {
            alert('Entered Value is not an integer');
        }
    }
    else {
        alert('Some error has occurred');
    }
}

function ManageSaveCompleteLinkServerResponse(data) {
    if (data.d != null) {
        if (data.d[0].RetVal == 1) {
            alert('Saved Succesfully');
            FetchCompleteRedirects();
            $('#CloseGenerateLink').click();
        }
        else if (data.d[0].RetVal == -1) {
            alert('Already Exists');
        }
        else if (data.d[0].RetVal == -2) {
            alert('Cannot Leave Code Empty');
        }
    }
}

function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

function FetchCompleteRedirects() {
    var formdata = {};
    var formData;
    formdata["opt"] = 1;
    //formdata["CountryID"] = sessionStorage.getItem("CountryID");
    PostRequest('Redirects.aspx/FetchCompleteLink', JSON.stringify({ formData: formdata }), ManageFetchCompleteRedirectsServerResponse, 'Post');
}

function ManageFetchCompleteRedirectsServerResponse(data) {
    $('#divCompleteRedirectLinks').empty();
    var redirects = '<ul>';
    if (data.d != null) {
        if (data.d.length > 0) {
            for (var i = 0; i < data.d.length; i++) {
                if (data.d[i].Enable == 1) {
                    redirects += '<li><label class="form-control lbl">https://nexton.us/VT/ProjectStatus.aspx?ID=[respondentID]&Status=' + data.d[i].Code + '&RC=0</label></li>';
                }
                else {
                    redirects += '<li><label style="text-decoration:line-through" class="form-control lbl">https://nexton.us/VT/ProjectStatus.aspx?ID=[respondentID]&Status=' + data.d[i].Code + '&RC=0</label></li>';
                }

            }
        }
    }
    redirects += '</ul>';
    $('#divCompleteRedirectLinks').append(redirects);
}

function Delete(Id) {
    var formdata = {};
    var formData;
    formdata["opt"] = 3;
    formdata["Id"] = Id;
    //formdata["CountryID"] = sessionStorage.getItem("CountryID");
    PostRequest('Redirects.aspx/UpdateStatusCompleteLink', JSON.stringify({ formData: formdata }), ManageUpdateStatusCompleteLinkServerResponse, 'Post');
}
function Enable(Id) {
    var formdata = {};
    var formData;
    formdata["opt"] = 4;
    formdata["Id"] = Id;
    //formdata["CountryID"] = sessionStorage.getItem("CountryID");
    PostRequest('Redirects.aspx/UpdateStatusCompleteLink', JSON.stringify({ formData: formdata }), ManageUpdateStatusCompleteLinkServerResponse, 'Post');
}
function Disable(Id) {
    var formdata = {};
    var formData;
    formdata["opt"] = 5;
    formdata["Id"] = Id;
    //formdata["CountryID"] = sessionStorage.getItem("CountryID");
    PostRequest('Redirects.aspx/UpdateStatusCompleteLink', JSON.stringify({ formData: formdata }), ManageUpdateStatusCompleteLinkServerResponse, 'Post');
}

function ManageUpdateStatusCompleteLinkServerResponse(data) {
    if (data.d != null) {
        if (data.d.length > 0) {
            if (data.d[0].RetVal == 1) {
                FetchCompleteRedirects();
            }
        }
    }
}