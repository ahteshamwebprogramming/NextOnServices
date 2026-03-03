function ApplyVariable(id) {
    var isChecked = $('#' + id).is(':checked');
    $('.ParameterContent').toggle(isChecked);
}

function getProjectMappingAjaxErrorMessage(xhr, fallbackMessage) {
    if (xhr && xhr.responseText && $.trim(xhr.responseText) !== '') {
        return xhr.responseText;
    }
    return fallbackMessage;
}

function initializeProjectMappingSelects() {
    var $selects = $("#PartialView_AddProjectMapping .select2");
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

function AddProjectMappingPartialView() {
    var inputDTO = {};
    inputDTO.encProjectId = $("[name='ProjectMappingWithChild.encProjectId']").val();

    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/VT/ProjectMapping/AddProjectMappingPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data) {
            $("#PartialView_AddProjectMapping").html(data);
            initializeProjectMappingSelects();
            ApplyVariable('chkAddHashing');
            UnblockUI();
        },
        error: function (xhr) {
            UnblockUI();
            $erroralert("Error!", getProjectMappingAjaxErrorMessage(xhr, "Unable to load mapping form."));
        }
    });
}

function ListProjectMappingPartialView() {
    var inputDTO = {};
    inputDTO.encProjectId = $("[name='ProjectMappingWithChild.encProjectId']").val();

    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/VT/ProjectMapping/ListProjectMappingPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data) {
            $("#PartialView_ListProjectMapping").html(data);
            UnblockUI();
        },
        error: function (xhr) {
            UnblockUI();
            $erroralert("Error!", getProjectMappingAjaxErrorMessage(xhr, "Unable to load mapping list."));
        }
    });
}

function AddProjectMapping() {
    var form = $("#ProjectMappingForm");
    var inputDTO = {};
    inputDTO.Id = $(form).find("[name='ProjectMappingWithChild.Id']").val() === "" ? 0 : $(form).find("[name='ProjectMappingWithChild.Id']").val();
    inputDTO.CountryId = $(form).find("[name='ProjectMappingWithChild.CountryId']").val();
    inputDTO.SupplierId = $(form).find("[name='ProjectMappingWithChild.SupplierId']").val();
    inputDTO.Cpi = $(form).find("[name='ProjectMappingWithChild.Cpi']").val();
    inputDTO.Respondants = $(form).find("[name='ProjectMappingWithChild.Respondants']").val();
    inputDTO.encProjectId = $("[name='ProjectMappingWithChild.encProjectId']").val();
    inputDTO.AddHashingBool = $(form).find("[name='ProjectMappingWithChild.AddHashingBool']").prop('checked');
    inputDTO.Notes = $(form).find("[name='ProjectMappingWithChild.Notes']").val();
    inputDTO.ParameterName = $(form).find("[name='ProjectMappingWithChild.ParameterName']").val();
    inputDTO.HashingType = $(form).find("[name='ProjectMappingWithChild.HashingType']").val();

    BlockUI();
    $.ajax({
        type: "POST",
        url: "/VT/ProjectMapping/SaveProjectMapping",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            var successMessage = (typeof data === "string" && $.trim(data) !== "") ? data : "Project mapping saved successfully.";
            UnblockUI();
            Swal.fire({ title: 'Success', text: successMessage, icon: 'success', confirmButtonText: 'OK' }).then(function () {
                ListProjectMappingPartialView();
                AddProjectMappingPartialView();
            });
        },
        error: function (xhr) {
            UnblockUI();
            $erroralert("Error!", getProjectMappingAjaxErrorMessage(xhr, "Unable to save project mapping."));
        }
    });
}

function EditProjectMapping(Id) {
    var inputDTO = {};
    inputDTO.Id = Id;
    inputDTO.encProjectId = $("[name='ProjectMappingWithChild.encProjectId']").val();

    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/VT/ProjectMapping/AddProjectMappingPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data) {
            $("#PartialView_AddProjectMapping").html(data);
            initializeProjectMappingSelects();
            ApplyVariable('chkAddHashing');
            UnblockUI();
        },
        error: function (xhr) {
            UnblockUI();
            $erroralert("Error!", getProjectMappingAjaxErrorMessage(xhr, "Unable to load mapping for edit."));
        }
    });
}

function CheckMapping(Id) {
    var inputDTO = {};
    inputDTO.Id = Id;

    BlockUI();
    $.ajax({
        type: "POST",
        url: "/VT/ProjectMapping/CheckProjectMapping",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function () {
            UnblockUI();
        },
        error: function (xhr) {
            UnblockUI();
            $erroralert("Error!", getProjectMappingAjaxErrorMessage(xhr, "Unable to update mapping check status."));
        }
    });
}
