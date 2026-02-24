function PageInit() {
    var QueryString = getQuerystringID();
    PopulateCountry(QueryString);
    PopulateVendors();



}

function getQuerystringID() {
    var id = 0;
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        pair = vars[i].split("=");
        //alert(pair[1]);
        id = pair[1];
    }
    return id;
}

function PopulateCountry(id) {
    $("#ddlCountry").empty();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'UpdateProjectMapping.aspx/GetCountries',
        data: JSON.stringify({ PID: id }),
        cache: false,
        dataType: "json",
        success: function (data) {
            $("#ddlCountry").append('<option value="0">--Select Operator--</option>');
            if (data.d.length > 0) {
                for (var i = 0; i < data.d.length; i++) {
                    $("#ddlCountry").append('<option value="' + data.d[i].Id + '">' + data.d[i].Country + '</option>');
                }
                $('#ddlCountry').dropdown();
            }
            else {
            }
        },
        error: function (result) {
            alert(result.d);
        }
    });

}

function PopulateVendors() {
    $("#ddlsupplier").empty();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'UpdateProjectMapping.aspx/GetSuppliers',
        cache: false,
        async: false,
        dataType: "json",

        success: function (data) {
            $("#ddlsupplier").append('<option value="0">--Select Operator--</option>');
            if (data.d.length > 0) {
                for (var i = 0; i < data.d.length; i++) {
                    $("#ddlsupplier").append('<option value="' + data.d[i].ID + '">' + data.d[i].Name + '</option>');
                }
                $('#ddlsupplier').dropdown();
            }
            else {
            }
        },
        error: function (result) {
            alert(result.d);
        }
    });
}