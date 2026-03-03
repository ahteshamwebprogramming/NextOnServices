function Tokens(id) {
    var isChecked = $('#' + id).is(':checked');
    $('.WT').toggle(isChecked);
    $('.WOT').toggle(!isChecked);
}

function getAjaxErrorMessage(xhr, fallbackMessage) {
    if (xhr && xhr.responseText && $.trim(xhr.responseText) !== '') {
        return xhr.responseText;
    }
    return fallbackMessage;
}

function initializeProjectUrlSelects() {
    var $selects = $("#PartialView_AddProjectURL .select2");
    if ($selects.length === 0) {
        return;
    }

    $selects.each(function () {
        if ($(this).hasClass('select2-hidden-accessible')) {
            $(this).select2('destroy');
        }
    });

    $selects.select2({
        dropdownCssClass: 'vt-theme-dropdown',
        width: '100%'
    });
}

function AddProjectURLPartialView() {
    var inputDTO = {};
    inputDTO.encProjectId = $("[name='ProjectsURL.encProjectId']").val();

    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/VT/ProjectURLs/AddProjectURLPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data) {
            $("#PartialView_AddProjectURL").html(data);
            initializeProjectUrlSelects();
            Tokens('chkTokens');
            UnblockUI();
        },
        error: function (xhr) {
            UnblockUI();
            $erroralert("Error!", getAjaxErrorMessage(xhr, "Unable to load URL form."));
        }
    });
}

function ListProjectURLPartialView() {
    var inputDTO = {};
    inputDTO.encProjectId = $("[name='ProjectsURL.encProjectId']").val();

    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/VT/ProjectURLs/ListProjectURLPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data) {
            $("#PartialView_ListProjectURL").html(data);
            UnblockUI();
        },
        error: function (xhr) {
            UnblockUI();
            $erroralert("Error!", getAjaxErrorMessage(xhr, "Unable to load URL list."));
        }
    });
}

function AddURL() {
    var form = $("#ProjectURLForm");
    var inputDTO = {};
    inputDTO.Id = $(form).find("[name='ProjectsURL.Id']").val() === "" ? 0 : $(form).find("[name='ProjectsURL.Id']").val();
    inputDTO.Cid = $(form).find("[name='ProjectsURL.Cid']").val();
    inputDTO.TokenBool = $(form).find("[name='ProjectsURL.TokenBool']").prop('checked');
    inputDTO.Url = $.trim($(form).find("[name='ProjectsURL.Url']").val());
    inputDTO.Quota = $(form).find("[name='ProjectsURL.Quota']").val();
    inputDTO.Cpi = $(form).find("[name='ProjectsURL.Cpi']").val();
    inputDTO.Notes = $(form).find("[name='ProjectsURL.Notes']").val();
    inputDTO.TokenRaw = $(form).find("[name='ProjectsURL.TokenRaw']").val();
    inputDTO.encProjectId = $("[name='ProjectsURL.encProjectId']").val();

    BlockUI();
    $.ajax({
        type: "POST",
        url: "/VT/ProjectURLs/SaveProjectURL",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            var successMessage = (typeof data === "string" && $.trim(data) !== "") ? data : "Project URL saved successfully.";
            UnblockUI();
            Swal.fire({ title: 'Success', text: successMessage, icon: 'success', confirmButtonText: 'OK' }).then(function () {
                AddProjectURLPartialView();
                ListProjectURLPartialView();
            });
        },
        error: function (xhr) {
            UnblockUI();
            $erroralert("Error!", getAjaxErrorMessage(xhr, "Unable to save URL mapping."));
        }
    });
}

function EditProjectURL(Id) {
    var inputDTO = {};
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
        success: function (data) {
            $("#PartialView_AddProjectURL").html(data);
            initializeProjectUrlSelects();
            Tokens('chkTokens');
            UnblockUI();
        },
        error: function (xhr) {
            UnblockUI();
            $erroralert("Error!", getAjaxErrorMessage(xhr, "Unable to load mapping for edit."));
        }
    });
}

function LaunchModalUploadTokens(Id) {
    $("#btnUploadTokens").click();
    $("#modalTokenUpload").find("[name='Id']").val(Id);
    $("#modalTokenUpload").find("[name='UploadModalTokensRaw']").val("");
}

function UploadTokens() {
    var formData = $("#modalTokenUpload");
    var inputDTO = {};
    inputDTO.Id = $("#modalTokenUpload").find("[name='Id']").val();
    inputDTO.TokenRaw = $(formData).find("[name='UploadModalTokensRaw']").val();

    BlockUI();
    $.ajax({
        type: "POST",
        url: "/VT/ProjectURLs/UploadTokensFromModal",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function () {
            UnblockUI();
            $("#modalTokenUpload").find(".close").click();
            Swal.fire({ title: 'Success', text: "Tokens uploaded successfully.", icon: 'success', confirmButtonText: 'OK' }).then(function () {
                ListProjectURLPartialView();
            });
        },
        error: function (xhr) {
            UnblockUI();
            $erroralert("Error!", getAjaxErrorMessage(xhr, "Unable to upload tokens."));
        }
    });
}

function ViewTokens(Id) {
    var inputDTO = {};
    inputDTO.Id = Id;

    BlockUI();
    $.ajax({
        type: "POST",
        url: "/VT/ProjectURLs/ViewTokensFromModal",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            var $table = $("#modalTokenView").find("[name='TokensList']");
            $("#btnTokenView").click();
            $table.find('tbody').empty();

            var tbody = "";
            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    tbody = tbody + '<tr><td>' + (i + 1) + '</td><td>' + data[i].token + '</td></tr>';
                }
                $table.find('tbody').html(tbody);
            }

            if ($.fn.DataTable.isDataTable($table)) {
                $table.DataTable().destroy();
            }

            $table.DataTable({
                paging: true,
                pageLength: 10,
                lengthChange: false,
                searching: true,
                ordering: false,
                info: false,
                dom: '<"ref-dt-top"f>t<"ref-dt-bottom"p>'
            });

            UnblockUI();
        },
        error: function (xhr) {
            UnblockUI();
            $erroralert("Error!", getAjaxErrorMessage(xhr, "Unable to load tokens."));
        }
    });
}
