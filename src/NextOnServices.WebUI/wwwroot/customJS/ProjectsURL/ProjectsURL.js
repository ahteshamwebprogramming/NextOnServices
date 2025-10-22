function Tokens(id) {
    if ($('#' + id).is(':checked')) {
        $('.WT').show();
        $('.WOT').hide();
        //  $('#btnSubmit').val('Submit and Upload Tokens');
        //  $('#btnSubmit').text('Submit and Upload Tokens');

    }
    else {
        $('.WT').hide();
        $('.WOT').show();
        //   $('#btnSubmit').val('Submit');
    }
}

function AddProjectURLPartialView() {
    let inputDTO = {};
    inputDTO.encProjectId = $("[name='ProjectsURL.encProjectId']").val();
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/VT/ProjectURLs/AddProjectURLPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $("#PartialView_AddProjectURL").html(data);
            $(".select2").select2();
            Tokens($('#chkTokens').attr('id'));
        },
        error: function (result) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
}
function ListProjectURLPartialView() {
    let inputDTO = {};
    inputDTO.encProjectId = $("[name='ProjectsURL.encProjectId']").val();
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/VT/ProjectURLs/ListProjectURLPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $("#PartialView_ListProjectURL").html(data);
        },
        error: function (result) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
}

function AddURL() {
    let userData = $("#ProjectURLForm");
    var inputDTO = {};
    inputDTO.Id = $(userData).find("[name='ProjectsURL.Id']").val() == "" ? 0 : $(userData).find("[name='ProjectsURL.Id']").val();
    inputDTO.Cid = $(userData).find("[name='ProjectsURL.Cid']").val();
    inputDTO.TokenBool = $(userData).find("[name='ProjectsURL.TokenBool']").prop('checked');
    inputDTO.Url = $(userData).find("[name='ProjectsURL.Url']").val();
    inputDTO.Quota = $(userData).find("[name='ProjectsURL.Quota']").val();
    inputDTO.Cpi = $(userData).find("[name='ProjectsURL.Cpi']").val();
    inputDTO.Notes = $(userData).find("[name='ProjectsURL.Notes']").val();
    inputDTO.TokenRaw = $(userData).find("[name='ProjectsURL.TokenRaw']").val();
    inputDTO.encProjectId = $("[name='ProjectsURL.encProjectId']").val();

    BlockUI();
    $.ajax({
        type: "POST",
        url: "/VT/ProjectURLs/SaveProjectURL",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            Swal.fire({ title: 'Success!', text: "Project Added", icon: 'success', confirmButtonText: 'OK' }).then((result) => {
                if (result.isConfirmed) {
                    AddProjectURLPartialView();
                    ListProjectURLPartialView();
                }
            });
        },
        error: function (error) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
}

function EditProjectURL(Id) {
    let inputDTO = {};
    inputDTO.Id = Id;
    inputDTO.encProjectId = $("[name='ProjectsURL.encProjectId']").val();
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/VT/ProjectURLs/AddProjectURLPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $("#PartialView_AddProjectURL").html(data);
            $(".select2").select2();
            Tokens($('#chkTokens').attr('id'));
        },
        error: function (result) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
}

function LaunchModalUploadTokens(Id) {
    $("#btnUploadTokens").click();
    $("#modalTokenUpload").find("[name='Id']").val(Id);
    $("#modalTokenUpload").find("[name='UploadModalTokensRaw']").val("");
}
function UploadTokens() {
    let formData = $("#modalTokenUpload");
    var inputDTO = {};
    inputDTO.Id = $("#modalTokenUpload").find("[name='Id']").val();
    inputDTO.TokenRaw = $(formData).find("[name='UploadModalTokensRaw']").val();
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/VT/ProjectURLs/UploadTokensFromModal",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            $("#modalTokenUpload").find(".close").click();
            Swal.fire({ title: 'Success!', text: "Tokens Added", icon: 'success', confirmButtonText: 'OK' }).then((result) => {
                if (result.isConfirmed) {

                }
            });
        },
        error: function (error) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
}
function ViewTokens(Id) {
    let formData = $("#modalTokenUpload");
    var inputDTO = {};
    inputDTO.Id = Id;
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/VT/ProjectURLs/ViewTokensFromModal",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            let $table = $("#modalTokenView").find("[name='TokensList']");
            $("#btnTokenView").click();
            $table.find('tbody').empty();
            let tbody = "";
            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    tbody = tbody + '<tr><td>' + (i + 1) + '</td><td>' + data[i].token + '</td></tr >';
                }
                $table.find('tbody').html(tbody);
            }

            if ($.fn.DataTable.isDataTable($table)) {
                $table.DataTable().destroy();
            }
            $table.DataTable();
        },
        error: function (error) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
}