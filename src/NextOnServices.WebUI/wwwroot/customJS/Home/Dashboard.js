function getProjectTablePartialView() {

    let Pmanager = $("#TableFilters").find("[name='Managers']").val();
    let Status = $("#TableFilters").find("[name='Status']").val();
    let flagstat = chckchckbox();
    var inputData = {
        "Pmanager": Pmanager,
        "Status": Status,
        "Flag": flagstat
    }

    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "/VT/Home/GetProjects",
            contentType: 'application/json',
            data: JSON.stringify(inputData),
            //dataType: "json",
            success: function (data) {
                $('#div_ProjectTable').html(data);



                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function openChangeStatusBox(status, projectId) {
    $("#mdlChangeStatus").modal('show');
    let statusId = status == "Closed" ? 1 : status == "Live" ? 2 : status == "On Hold" ? 3 : status == "Cancelled" ? 4 : status == "Awarded" ? 5 : status == "Invoiced" ? 6 : 0;
    $("#mdlChangeStatus").find("[name='Status']").val(statusId);
    $("#mdlChangeStatus").find("[name='ProjectId']").val(projectId);

}

function UpdateStatus() {
    BlockUI();
    let status = $("#mdlChangeStatus").find("[name='Status']").val();
    let projectId = $("#mdlChangeStatus").find("[name='ProjectId']").val();
    jQuery.ajax({
        type: "POST",
        url: "/VT/Home/ChangeProjectStatus/",
        data: { Status: status, ProjectId: projectId },
        cache: false,
        dataType: "json",
        success: function (data) {
            $("#mdlChangeStatus").modal('hide');
            getProjectTablePartialView().then(() => {
                UnblockUI();
            });
        },
        error: function (result) {
            UnblockUI();
            alert("Error!", result.responseText + "!")
        }
    });
}
function chckchckbox() {
    var UF = '0', B = '0', R = '0', Y = '0', flag;

    if ($('#chkBlue').is(':checked')) {
        B = '1';
    }
    if ($('#chkYellow').is(':checked')) {
        Y = '1';
    }
    if ($('#chkRed').is(':checked')) {
        R = '1';
    }
    var bit = B + Y + R;
    if (bit == '000')
        flag = 0;
    else if (bit == '100')
        flag = 1
    else if (bit == '010')
        flag = 2
    else if (bit == '001')
        flag = 3
    else if (bit == '110')
        flag = 4
    else if (bit == '101')
        flag = 5
    else if (bit == '011')
        flag = 6
    else if (bit == '111')
        flag = 7

    return flag;
}