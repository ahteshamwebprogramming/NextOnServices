function ApplyVariable(id) {
    if ($('#' + id).is(':checked')) {
        $('.ParameterContent').show();
    }
    else {
        $('.ParameterContent').hide();
    }
}

function AddProjectMappingPartialView() {
    let inputDTO = {};
    inputDTO.encProjectId = $("[name='ProjectMappingWithChild.encProjectId']").val();
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/VT/ProjectMapping/AddProjectMappingPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $("#PartialView_AddProjectMapping").html(data);
            $(".select2").select2();
            ApplyVariable('chkAddHashing');
        },
        error: function (result) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
}
function ListProjectMappingPartialView() {
    let inputDTO = {};
    inputDTO.encProjectId = $("[name='ProjectMappingWithChild.encProjectId']").val();
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/VT/ProjectMapping/ListProjectMappingPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $("#PartialView_ListProjectMapping").html(data);
        },
        error: function (result) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
}

function AddProjectMapping() {
    let userData = $("#ProjectMappingForm");
    var inputDTO = {};
    inputDTO.Id = $(userData).find("[name='ProjectMappingWithChild.Id']").val() == "" ? 0 : $(userData).find("[name='ProjectMappingWithChild.Id']").val();
    inputDTO.CountryId = $(userData).find("[name='ProjectMappingWithChild.CountryId']").val();
    inputDTO.SupplierId = $(userData).find("[name='ProjectMappingWithChild.SupplierId']").val();
    inputDTO.Cpi = $(userData).find("[name='ProjectMappingWithChild.Cpi']").val();
    inputDTO.Respondants = $(userData).find("[name='ProjectMappingWithChild.Respondants']").val();
    inputDTO.encProjectId = $("[name='ProjectMappingWithChild.encProjectId']").val();
    inputDTO.AddHashingBool = $(userData).find("[name='ProjectMappingWithChild.AddHashingBool']").prop('checked');

    inputDTO.Notes = $(userData).find("[name='ProjectMappingWithChild.Notes']").val();

    inputDTO.ParameterName = $(userData).find("[name='ProjectMappingWithChild.ParameterName']").val();
    inputDTO.HashingType = $(userData).find("[name='ProjectMappingWithChild.HashingType']").val();



    BlockUI();
    $.ajax({
        type: "POST",
        url: "/VT/ProjectMapping/SaveProjectMapping",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            Swal.fire({ title: 'Success!', text: "Project Mapped", icon: 'success', confirmButtonText: 'OK' }).then((result) => {
                if (result.isConfirmed) {
                    window.history.back();
                    //window.location.href = "/VT/Projects/ProjectsList";
                }
            });
        },
        error: function (error) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
}

function EditProjectMapping(Id) {
    let inputDTO = {};
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
        success: function (data, textStatus, jqXHR) {

            UnblockUI();
            $("#PartialView_AddProjectMapping").html(data);
            $(".select2").select2();
            ApplyVariable('chkAddHashing');

            //UnblockUI();
            //$("#PartialView_AddProjectURL").html(data);
            //$(".select2").select2();
            //Tokens($('#chkTokens').attr('id'));
        },
        error: function (result) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
}

function CheckMapping(Id, curObj) {
    let inputDTO = {};
    inputDTO.Id = Id;
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/VT/ProjectMapping/CheckProjectMapping",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();            
        },
        error: function (error) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
    
}