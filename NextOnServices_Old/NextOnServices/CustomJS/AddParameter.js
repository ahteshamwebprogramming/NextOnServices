function InitPage() {
    GetProjects();
}
function GetProjects() {
    return new Promise((resolve, reject) => {
        //var formData = {};
        //formData[""]
        jQuery.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "AddParameter.aspx/GetProjects",
            //data: "",
            cache: false,
            dataType: "json",
            success: function (data) {
                $('[name="Project"]').empty();
                $('[name="Project"]').append("<option value='0'>--Select Project--</option>");
                if (data.d != null) {
                    if (data.d.length > 0) {
                        for (var i = 0; i < data.d.length; i++) {
                            $('[name="Project"]').append("<option value='" + data.d[i].Id + "'>" + data.d[i].PName + "</option>");
                        }
                    }
                }
            },
            error: function (result) {
                alert('Status ' + result.status + ' : ' + result.statusText + ' error :' + result.responseText);
            }
        });
    });
}

function SaveParameters() {
    return new Promise((resolve, reject) => {
        var formData = {};
        formData["ProjectId"] = $('[name="Project"]').val();
        formData["ParameterName"] = $('[name="Project"]').val();
        jQuery.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "AddParameter.aspx/GetProjects",
            data: formData,
            cache: false,
            dataType: "json",
            success: function (data) {
                $('[name="Project"]').empty();
                $('[name="Project"]').append("<option value='0'>--Select Project--</option>");
                if (data.d != null) {
                    if (data.d.length > 0) {
                        for (var i = 0; i < data.d.length; i++) {
                            $('[name="Project"]').append("<option value='" + data.d[i].Id + "'>" + data.d[i].PName + "</option>");
                        }
                    }
                }
            },
            error: function (result) {
                alert('Status ' + result.status + ' : ' + result.statusText + ' error :' + result.responseText);
            }
        });
    });
}