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

var projectUrlCountrySearchTerm = '';

function countryOptionMatchesSearch($option, searchTerm) {
    var value = $option.val();
    if (value === '0' || value === '' || value === undefined) {
        return false;
    }
    var text = ($option.text() || '').toLowerCase();
    return text.indexOf(searchTerm) >= 0;
}

function updateProjectUrlAddCountryButton(searchTerm) {
    var $btn = $('#btnAddCountry');
    if ($btn.length === 0) {
        return;
    }

    var rawTerm = $.trim(searchTerm || '');
    var term = rawTerm.toLowerCase();
    projectUrlCountrySearchTerm = rawTerm;

    if (term === '') {
        $btn.hide().removeData('pendingCountryName');
        projectUrlCountrySearchTerm = '';
        return;
    }

    var matchCount = 0;
    $('#CountryId option').each(function () {
        if (countryOptionMatchesSearch($(this), term)) {
            matchCount++;
        }
    });

    if (matchCount === 0) {
        $btn.show().data('pendingCountryName', rawTerm);
    } else {
        $btn.hide().removeData('pendingCountryName');
    }
}

function clearProjectUrlPendingCountry() {
    $('#btnAddCountry').hide().removeData('pendingCountryName');
    projectUrlCountrySearchTerm = '';
}

function resolveProjectUrlCountryNameToAdd() {
    var $btn = $('#btnAddCountry');
    var countryName = $.trim($btn.data('pendingCountryName') || '');
    if (countryName === '') {
        countryName = $.trim(projectUrlCountrySearchTerm || '');
    }
    if (countryName === '') {
        var $search = $('.select2-container--open .select2-search__field');
        countryName = $.trim($search.val());
    }
    return countryName;
}

function detachProjectUrlCountrySelectHandlers($countrySelect) {
    $countrySelect.off('select2:open.projectUrlCountry select2:close.projectUrlCountry select2:select.projectUrlCountry');
    $(document).off('input.projectUrlCountry', '.select2-container--open .select2-search__field');
}

function bindProjectUrlCountrySelectHandlers($countrySelect) {
    detachProjectUrlCountrySelectHandlers($countrySelect);

    $countrySelect.on('select2:open.projectUrlCountry', function () {
        var $search = $('.select2-container--open .select2-search__field');
        updateProjectUrlAddCountryButton($search.val());

        $(document).on('input.projectUrlCountry', '.select2-container--open .select2-search__field', function () {
            updateProjectUrlAddCountryButton($(this).val());
        });
    });

    $countrySelect.on('select2:close.projectUrlCountry', function () {
        $(document).off('input.projectUrlCountry', '.select2-container--open .select2-search__field');
        if (!$('#btnAddCountry').data('pendingCountryName')) {
            clearProjectUrlPendingCountry();
        }
    });

    $countrySelect.on('select2:select.projectUrlCountry', function () {
        clearProjectUrlPendingCountry();
    });
}

function addProjectUrlCountryFromSearch() {
    var countryName = resolveProjectUrlCountryNameToAdd();

    if (countryName === '') {
        $erroralert('Validation', 'Enter a country name in the search box before adding.');
        return;
    }

    BlockUI();
    $.ajax({
        type: 'POST',
        url: '/VT/ProjectURLs/AddCountry',
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify({ countryName: countryName }),
        success: function (data) {
            var countryId = data && (data.countryId || data.CountryId);
            if (!countryId) {
                $erroralert('Error!', 'Country was added but the response was invalid.');
                return;
            }

            var $countrySelect = $('#CountryId');
            var optionValue = String(countryId);
            var optionText = data.country || data.Country || countryName;

            if ($countrySelect.find('option[value="' + optionValue + '"]').length === 0) {
                $countrySelect.append(new Option(optionText, optionValue, true, true));
            } else {
                $countrySelect.val(optionValue);
            }

            $countrySelect.trigger('change');
            clearProjectUrlPendingCountry();
            $countrySelect.select2('close');

            Swal.fire({
                title: 'Success',
                text: 'Country "' + optionText + '" added.',
                icon: 'success',
                confirmButtonText: 'OK'
            });
        },
        error: function (xhr) {
            $erroralert('Error!', getAjaxErrorMessage(xhr, 'Unable to add country.'));
        },
        complete: function () {
            UnblockUI();
        }
    });
}

function initializeProjectUrlSelects() {
    var $selects = $("#PartialView_AddProjectURL .select2");
    if ($selects.length === 0) {
        return;
    }

    var $countrySelect = $('#CountryId');
    if ($countrySelect.length > 0) {
        detachProjectUrlCountrySelectHandlers($countrySelect);
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

    if ($countrySelect.length > 0) {
        bindProjectUrlCountrySelectHandlers($countrySelect);
    }
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

$(document).on('mousedown.projectUrlCountry', '#btnAddCountry', function (e) {
    e.preventDefault();
    e.stopPropagation();
    addProjectUrlCountryFromSearch();
});
