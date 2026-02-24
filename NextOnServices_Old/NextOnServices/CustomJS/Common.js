function RunAjax(loadUrl, InputData, callback, method) {

    jQuery.ajax({
        type: method,
        url: loadUrl,
        data: InputData,
        success: function (response) {
            callback(response);
        }
    });

}

function PostRequest(loadUrl, InputData, callback, method) {
    jQuery.ajax({
        type: method,
        contentType: "application/json; charset=utf-8",
        url: loadUrl,
        data: InputData,
        cache: false,
        dataType: "json",
        success: function (response) {
            callback(response);
        },
        error: function (result) {
            // alert("Unable to mark attendance");
            alert('Status ' + result.status + ' : ' + result.statusText + ' error :' + result.responseText);
        }
    });

}


function GetFormData() {
    var formData = {};
    var allCtrl = jQuery("[id^=ctrl_]");
    allCtrl.each(function (i) {
        var curCtrl = jQuery(this);
        formData[curCtrl.attr('dbcol')] = curCtrl.val();
    });
    return formData;
}

//JSONToExcelWithDelete(data, "HRM Employees", true, sColToDelete)
function JSONToExcelWithDelete(JSONData, ReportTitle, ShowLabel, sColToDelete) {
    //If JSONData is not an object then JSON.parse will parse the JSON string in an Object
    var arrData = typeof JSONData != 'object' ? JSON.parse(JSONData) : JSONData;

    var CSV = "<html><head></head><table border='1'>";

    //This condition will generate the Label/Header
    if (ShowLabel) {
        var row = "<thead><tr style='background-color:Green'>";

        //This loop will extract the label from 1st index of on array

        for (var index in arrData[0]) {
            if (sColToDelete.indexOf(',' + index + ',') >= 0)
                delete arrData[0][index]
            else
                row += "<th>" + index + "</th>";
        }

        //        row = row.slice(0, -1);

        //append Label row with line break
        CSV += row + "</tr></thead>";
    }

    //1st loop is to extract each row
    for (var i = 0; i < arrData.length; i++) {
        var row = "<tr>";

        //2nd loop will extract each column and convert it in string comma-seprated
        for (var index in arrData[i]) {

            if (sColToDelete.indexOf(',' + index + ',') >= 0)
                delete arrData[i][index]
            else
                row += "<td>" + arrData[i][index] + "</td>";
        }

        //        row.slice(0, row.length - 1);      
        CSV += row + "</tr>"
    }
    CSV += "</table></html>";
    if (CSV == '') {
        alert("Invalid data");
        return;
    }

    var fileName = ReportTitle;
    var uri = "data:application/vnd.ms-excel," + escape(CSV);
    var link = document.createElement("a");
    link.href = uri;
    link.style = "visibility:hidden";
    link.download = fileName + ".xls";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}