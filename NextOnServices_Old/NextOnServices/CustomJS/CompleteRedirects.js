
$(document).ready(function () {
    FetchCompleteRedirects();
    FetchDeletedRedirects();
    $('#a_GenerateLink').click(function () {
        ResetPopupLinkGenerator();
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
        if (parseInt(CharLimit) < 8 || parseInt(CharLimit) > 16) {
            alertt('Error', 'Only integers between 8 to 16 are allowed', 'error');
        }
        else {
            var formdata = {};
            var formData;
            formdata["Charl"] = CharLimit;
            $('#hdnCharactersNumber').val(CharLimit);
            //formdata["CountryID"] = sessionStorage.getItem("CountryID");
            PostRequest('Redirects.aspx/GenerateRandomLink', JSON.stringify({ formData: formdata }), ManageGenerateRandomLinkServerResponse, 'Post');
        }
    });

    $('#btnSaveLink').click(function () {
        if ($('#txtRandomCode').val() == "") {
            alert('Cannot leave the code empty');
            return false;
        }
        var Code = $('#txtRandomCode').val();
        if ($('#hdnCharactersNumber').val() == Code.length) {

            if (/^[a-zA-Z0-9]*$/.test(Code) == false) {
                alert('Your Code contains illegal characters.');
            }
            else {
                var formdata = {};
                var formData;
                formdata["Link"] = "https://nexton.us/VT/ProjectStatus.aspx?ID=[respondentID]&Status=" + Code + "&RC=0";
                formdata["Code"] = Code;
                //formdata["CountryID"] = sessionStorage.getItem("CountryID");
                PostRequest('Redirects.aspx/SaveCompleteLink', JSON.stringify({ formData: formdata }), ManageSaveCompleteLinkServerResponse, 'Post');
            }
            //var regexItem = new Regex("^[a-zA-Z0-9 ]*$");

            //if (regexItem.IsMatch(Code)) {
            //    alert('Alpha Numeric Code');
            //}
            //else {
            //    alert('Not an alphanumeric code');
            //}
        }
        else {
            alert('Code number does not match');
        }
    });
});

function ManageGenerateRandomLinkServerResponse(data) {
    if (data.d != null) {
        if (data.d != "") {

            if (data.d == "InvalidNumber101") {
                alertt('Error', 'Only integers between 8 to 16 are allowed', 'error');
            }
            else {
                $('#div_GenerateLink').slideUp();
                $('#div_CheckLink').slideDown();
                $('#btnGenerateLink').hide();
                $('#btnSaveLink').show();
                $('#txtRandomCode').val(data.d);
            }
        }
        else {
            //alert('Entered Value is not an integer');
            alertt('Error', 'Entered Value is not an integer', 'error');
        }
    }
    else {
        //alert('Some error has occurred');
        alertt('Error', 'Some error has occurred', 'error');
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
                    redirects += '<li><label class="form-control lbl">https://nexton.us/VT/ProjectStatus.aspx?ID=[respondentID]&Status=' + data.d[i].Code + '&RC=0 <a href="javascript:void(0)" style="margin-left:10px;color:red" class="pull-right" id="" onclick="Delete(' + data.d[i].Id + ')">Delete</a> <a href="javascript:void(0)" class="pull-right" style="margin-left:10px;color:red" id="" onclick="Disable(' + data.d[i].Id + ')">Disable</a></label></li>';
                }
                else {
                    redirects += '<li><label style="text-decoration:line-through" class="form-control lbl">https://nexton.us/VT/ProjectStatus.aspx?ID=[respondentID]&Status=' + data.d[i].Code + '&RC=0 <a href="javascript:void(0)" style="margin-left:10px;color:red" class="pull-right" id=""  onclick="Delete(' + data.d[i].Id + ')">Delete</a> <a href="javascript:void(0)" class="pull-right" style="margin-left:10px;color:red" id=""  onclick="Enable(' + data.d[i].Id + ')">Enable</a></label></li>';
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
    swal({
        title: 'Are you sure?',
        text: "You want to Delete the Redirect?",
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#26d06c',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'Redirects.aspx/UpdateStatusCompleteLink',
                data: JSON.stringify({ formData: formdata }),
                cache: false,
                dataType: "json",
                success: function (data) {
                    ManageUpdateStatusCompleteLinkServerResponse(data);
                },
                error: function (result) {
                    alert(result.d);
                }
            });
        }
    });
    //PostRequest('Redirects.aspx/UpdateStatusCompleteLink', JSON.stringify({ formData: formdata }), ManageUpdateStatusCompleteLinkServerResponse, 'Post');
}
function Enable(Id) {
    var formdata = {};
    var formData;
    formdata["opt"] = 4;
    formdata["Id"] = Id;
    //formdata["CountryID"] = sessionStorage.getItem("CountryID");
    swal({
        title: 'Are you sure?',
        text: "You want to Enable the Redirect?",
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#26d06c',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, enable it!'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'Redirects.aspx/UpdateStatusCompleteLink',
                data: JSON.stringify({ formData: formdata }),
                cache: false,
                dataType: "json",
                success: function (data) {
                    ManageUpdateStatusCompleteLinkServerResponse(data);
                },
                error: function (result) {
                    alert(result.d);
                }
            });
        }
    });
    //PostRequest('Redirects.aspx/UpdateStatusCompleteLink', JSON.stringify({ formData: formdata }), ManageUpdateStatusCompleteLinkServerResponse, 'Post');
}
function Disable(Id) {
    var formdata = {};
    var formData;
    formdata["opt"] = 5;
    formdata["Id"] = Id;
    //formdata["CountryID"] = sessionStorage.getItem("CountryID");
    swal({
        title: 'Are you sure?',
        text: "You want to Disable the Redirect?",
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#26d06c',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, disable it!'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'Redirects.aspx/UpdateStatusCompleteLink',
                data: JSON.stringify({ formData: formdata }),
                cache: false,
                dataType: "json",
                success: function (data) {
                    ManageUpdateStatusCompleteLinkServerResponse(data);
                },
                error: function (result) {
                    alert(result.d);
                }
            });
        }
    });
    //PostRequest('Redirects.aspx/UpdateStatusCompleteLink', JSON.stringify({ formData: formdata }), ManageUpdateStatusCompleteLinkServerResponse, 'Post');
}

function ManageUpdateStatusCompleteLinkServerResponse(data) {
    if (data.d != null) {
        if (data.d.length > 0) {
            if (data.d[0].RetVal == 1) {
                FetchCompleteRedirects();
                FetchDeletedRedirects();
            }
        }
    }
}



function FetchDeletedRedirects() {
    var formdata = {};
    var formData;
    formdata["opt"] = 6;
    //formdata["CountryID"] = sessionStorage.getItem("CountryID");
    PostRequest('Redirects.aspx/FetchCompleteLink', JSON.stringify({ formData: formdata }), ManageFetchDeletedRedirectsServerResponse, 'Post');
}
function ManageFetchDeletedRedirectsServerResponse(data) {
    $('#divDeletedCompleteRedirectLinks').empty();
    var redirects = '<ul>';
    if (data.d != null) {
        if (data.d.length > 0) {
            for (var i = 0; i < data.d.length; i++) {
                redirects += '<li><label class="form-control lbl">https://nexton.us/VT/ProjectStatus.aspx?ID=[respondentID]&Status=' + data.d[i].Code + '&RC=0 </label></li>';
            }
        }
    }
    redirects += '</ul>';
    $('#divDeletedCompleteRedirectLinks').append(redirects);
}

function ResetPopupLinkGenerator() {
    $('#div_GenerateLink').show();
    $('#div_CheckLink').hide();
    $('#btnGenerateLink').show();
    $('#btnSaveLink').hide();
    $('#txtChar').val('');
    $('#txtRandomCode').val('');
}