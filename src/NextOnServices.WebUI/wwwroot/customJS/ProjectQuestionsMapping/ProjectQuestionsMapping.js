function InitializeProjectQuestionsMapping() {
    // Handle project dropdown change
    $('#ddlProject').on('change', function () {
        LoadCountriesByProject();
    });

    // Handle country dropdown change
    $('#ddlCountry').on('change', function () {
        LoadExistingMapping();
    });

    // Handle select all checkbox
    $('#chkSelectAll').on('change', function () {
        $('.question-checkbox').prop('checked', $(this).prop('checked'));
    });

    // Handle individual checkbox change
    $(document).on('change', '.question-checkbox', function () {
        var totalCheckboxes = $('.question-checkbox').length;
        var checkedCheckboxes = $('.question-checkbox:checked').length;
        $('#chkSelectAll').prop('checked', totalCheckboxes === checkedCheckboxes);
    });

    // Initialize with existing project if available
    var projectIdEnc = $('#hdnProjectIdEnc').val();
    if (projectIdEnc) {
        $('#ddlProject').val(projectIdEnc).trigger('change');
    }
}

function LoadCountriesByProject() {
    var projectIdEnc = $('#ddlProject').val();
    if (!projectIdEnc) {
        $('#ddlCountry').html('<option value="0">--Select Country--</option>');
        return;
    }

    BlockUI();
    $.ajax({
        type: "POST",
        url: '/VT/ProjectQuestionsMapping/GetCountriesByProject',
        contentType: "application/json",
        data: JSON.stringify({ encProjectId: projectIdEnc }),
        success: function (data) {
            UnblockUI();
            var html = '<option value="0">--Select Country--</option>';
            if (data && data.length > 0) {
                $.each(data, function (index, country) {
                    html += '<option value="' + country.countryId + '">' + country.country + '</option>';
                });
            }
            $('#ddlCountry').html(html);
            $('#ddlCountry').val(0).trigger('change');
        },
        error: function (error) {
            UnblockUI();
            $erroralert("Error!", "Failed to load countries");
        }
    });
}

function LoadExistingMapping() {
    var projectIdEnc = $('#ddlProject').val();
    var countryId = $('#ddlCountry').val();

    if (!projectIdEnc || !countryId || countryId == '0') {
        ClearMapping();
        return;
    }

    BlockUI();
    $.ajax({
        type: "POST",
        url: '/VT/ProjectQuestionsMapping/GetProjectQuestionsMapping',
        contentType: "application/json",
        data: JSON.stringify({
            encProjectId: projectIdEnc,
            Cid: parseInt(countryId)
        }),
        success: function (data) {
            UnblockUI();
            if (data) {
                LoadMappingData(data);
            } else {
                ClearMapping();
            }
        },
        error: function (error) {
            UnblockUI();
            $erroralert("Error!", "Failed to load mapping");
        }
    });
}

function LoadMappingData(mapping) {
    // Set mapping ID
    $('#hdnMappingId').val(mapping.id || 0);

    // Set toggle switches
    $('#chkPreviousButton').prop('checked', mapping.previousButton === 1);
    $('#chkQuestionID').prop('checked', mapping.questionQid === 1);
    $('#chkLogo').prop('checked', mapping.logo === 1);

    // Set selected questions
    $('.question-checkbox').prop('checked', false);
    if (mapping.qids) {
        var selectedIds = mapping.qids.split(',').map(function (id) { return parseInt(id.trim()); });
        $('.question-checkbox').each(function () {
            var questionId = parseInt($(this).data('question-id'));
            if (selectedIds.indexOf(questionId) !== -1) {
                $(this).prop('checked', true);
            }
        });
    }

    // Update select all checkbox
    var totalCheckboxes = $('.question-checkbox').length;
    var checkedCheckboxes = $('.question-checkbox:checked').length;
    $('#chkSelectAll').prop('checked', totalCheckboxes === checkedCheckboxes && totalCheckboxes > 0);
}

function ClearMapping() {
    $('#hdnMappingId').val(0);
    $('#chkPreviousButton').prop('checked', false);
    $('#chkQuestionID').prop('checked', false);
    $('#chkLogo').prop('checked', false);
    $('.question-checkbox').prop('checked', false);
    $('#chkSelectAll').prop('checked', false);
}

function SaveMapping() {
    // Validate inputs
    var validationResult = validateMapping();
    if (!validationResult.isValid) {
        $erroralert("Validation Error!", validationResult.message);
        return;
    }

    var projectIdEnc = $('#ddlProject').val();
    var countryId = $('#ddlCountry').val();
    var mappingId = $('#hdnMappingId').val();

    // Get selected question IDs
    var selectedQuestionIds = getSelectedQuestionIds();

    var mappingDTO = {
        Id: parseInt(mappingId) || 0,
        encProjectId: projectIdEnc,
        Cid: parseInt(countryId),
        PreviousButton: $('#chkPreviousButton').prop('checked') ? 1 : 0,
        QuestionQid: $('#chkQuestionID').prop('checked') ? 1 : 0,
        Logo: $('#chkLogo').prop('checked') ? 1 : 0,
        SelectedQuestionIds: selectedQuestionIds
    };

    BlockUI();
    $.ajax({
        type: "POST",
        url: '/VT/ProjectQuestionsMapping/SaveProjectQuestionsMapping',
        contentType: "application/json",
        data: JSON.stringify(mappingDTO),
        success: function (response) {
            UnblockUI();
            if (response && response.id) {
                $('#hdnMappingId').val(response.id);
                $successalert("Success!", "Mapping saved successfully");
            } else {
                $successalert("Success!", "Mapping saved successfully");
            }
        },
        error: function (error) {
            UnblockUI();
            var errorMessage = "Failed to save mapping";
            if (error.responseJSON && error.responseJSON.message) {
                errorMessage = error.responseJSON.message;
            } else if (error.responseText) {
                errorMessage = error.responseText;
            }
            $erroralert("Error!", errorMessage);
        }
    });
}

function SaveAndNext() {
    // Validate inputs
    var validationResult = validateMapping();
    if (!validationResult.isValid) {
        $erroralert("Validation Error!", validationResult.message);
        return;
    }

    var projectIdEnc = $('#ddlProject').val();
    var countryId = $('#ddlCountry').val();

    // Save the mapping first
    var mappingDTO = {
        Id: parseInt($('#hdnMappingId').val()) || 0,
        encProjectId: projectIdEnc,
        Cid: parseInt(countryId),
        PreviousButton: $('#chkPreviousButton').prop('checked') ? 1 : 0,
        QuestionQid: $('#chkQuestionID').prop('checked') ? 1 : 0,
        Logo: $('#chkLogo').prop('checked') ? 1 : 0,
        SelectedQuestionIds: getSelectedQuestionIds()
    };

    BlockUI();
    $.ajax({
        type: "POST",
        url: '/VT/ProjectQuestionsMapping/SaveProjectQuestionsMapping',
        contentType: "application/json",
        data: JSON.stringify(mappingDTO),
        success: function (response) {
            UnblockUI();
            if (response && response.id) {
                $('#hdnMappingId').val(response.id);
            }
            $successalert("Success!", "Mapping saved successfully");
            // Navigate to next step or reload the page
            // You can customize this based on your workflow
        },
        error: function (error) {
            UnblockUI();
            var errorMessage = "Failed to save mapping";
            if (error.responseJSON && error.responseJSON.message) {
                errorMessage = error.responseJSON.message;
            } else if (error.responseText) {
                errorMessage = error.responseText;
            }
            $erroralert("Error!", errorMessage);
        }
    });
}

function validateMapping() {
    var projectIdEnc = $('#ddlProject').val();
    var countryId = $('#ddlCountry').val();
    
    // Validate Project
    if (!projectIdEnc) {
        return {
            isValid: false,
            message: "Please select a project."
        };
    }

    // Validate Country
    if (!countryId || countryId == '0') {
        return {
            isValid: false,
            message: "Please select a country."
        };
    }

    // Validate that at least one question is selected
    var selectedQuestionIds = getSelectedQuestionIds();
    if (!selectedQuestionIds || selectedQuestionIds.length === 0) {
        return {
            isValid: false,
            message: "Please select at least one question to map."
        };
    }

    return {
        isValid: true,
        message: ""
    };
}

function getSelectedQuestionIds() {
    var selectedQuestionIds = [];
    $('.question-checkbox:checked').each(function () {
        selectedQuestionIds.push(parseInt($(this).data('question-id')));
    });
    return selectedQuestionIds;
}

function BackToProjectDashboard() {
    var projectIdEnc = $('#hdnProjectIdEnc').val();
    if (projectIdEnc) {
        // Navigate back to Project Page with encrypted project ID
        window.location.href = '/VT/Home/ProjectPage/' + projectIdEnc;
    } else {
        window.location.href = '/VT/Home/Dashboard';
    }
}

