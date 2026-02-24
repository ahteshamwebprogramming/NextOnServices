function PageInitOverAll() {
    var currMonth = new Date();
    var dd = String(currMonth.getDate()).padStart(2, '0');
    //var mm1 = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var mm = String(currMonth.getMonth()).padStart(2, '0'); //January is 0!
    var yyyy = currMonth.getFullYear();

    jQuery("#txtSDate1").datepicker({ dateFormat: 'mm-dd-yy' });
    jQuery("#txtSDate1").datepicker('setDate', new Date(yyyy, mm, 1));
    jQuery("#txtEDate1").datepicker({ dateFormat: 'mm-dd-yy' });
    jQuery("#txtEDate1").datepicker('setDate', 'today');
    if ($('#chkswitch').attr('checked', 'checked')) {
        $('#lblValue').show();
        $('#lblCompletes').hide();
        $('.revenuetable').show();
        $('.completetable').hide();
        $('#hHeading').html("Revenue");
    }
    $('#chkswitch').change(function () {
        if ($('#chkswitch').is(':checked')) {

            $('#lblValue').show();
            $('#lblCompletes').hide();
            $('.revenuetable').show();
            $('.completetable').hide();
            $('#hHeading').html("Revenue");
        }
        else {

            $('#lblValue').hide();
            $('#lblCompletes').show();
            $('.revenuetable').hide();
            $('.completetable').show();
            $('#hHeading').html("Completes");
        }
    });
    loadManagerwisereport('onload');
    loadCharts();

    var formData1 = getallDataNew();
    var formData = JSON.stringify({});
    getoverallreportdata(formData, formData1);
}
function loadCharts() {
    //for charts start
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'OverallReport.aspx/GetPieChartData',
        cache: false,
        dataType: "json",
        success: function (data) {
            if (data == null) {
                return;
            }
            //for pie chart start
            //alert(data.d[0][0].Max);
            var chartData = [{
                "country": "Min",
                "litres": data.d[0][0].Min
            }, {
                "country": "Max",
                "litres": data.d[0][0].Max
            }, {
                "country": "Mean",
                "litres": data.d[0][0].Mean
            }];

            /**
             * Add a 50% slice
             */

            var sum = 0;
            for (var x in chartData) {
                sum += chartData[x].litres;
            }

            chartData.push({
                "litres": sum,
                "alpha": 0
            });

            /**
             * Create the chart
             */

            var chart = AmCharts.makeChart("chartdiv", {
                "type": "pie",
                "startAngle": 0,
                "radius": "90%",
                "innerRadius": "50%",
                "dataProvider": chartData,
                "valueField": "litres",
                "titleField": "country",
                "alphaField": "alpha",
                "labelsEnabled": false,
                "pullOutRadius": 0,
                "pieY": "95%"
            });
            // for pie chart end

            //for bar chart start

            var chart = AmCharts.makeChart("chartdiv2", {
                "type": "serial",
                "theme": "light",
                "dataProvider": [{
                    "name": data.d[1][0].Months,
                    "open": 0,
                    "close": data.d[1][0].Completes,
                    "color": "#54cb6a",
                    "balloonValue": data.d[1][0].Completes
                }, {
                    "name": data.d[1][1].Months,
                    "open": 0,
                    "close": data.d[1][1].Completes,
                    "color": "#54cb6a",
                    "balloonValue": data.d[1][1].Completes
                }, {
                    "name": data.d[1][2].Months,
                    "open": 0,
                    "close": data.d[1][2].Completes,
                    "color": "#169b2f",
                    "balloonValue": data.d[1][2].Completes
                }],
                "valueAxes": [{
                    "axisAlpha": 0,
                    "gridAlpha": 0.1,
                    "position": "left"
                }],
                "startDuration": 1

                ,
                "graphs": [{
                    "balloonText": "<span style='color:[[color]]'>[[category]]</span><br><b>[[balloonValue]]</b>",
                    "colorField": "color",
                    "fillAlphas": 0.8,
                    "labelText": "[[balloonValue]]",
                    "lineColor": "#BBBBBB",
                    "openField": "open",
                    "type": "column",
                    "valueField": "close"
                }],
                "trendLines": [{
                    "dashLength": 3,
                    "finalCategory": "Month 2",
                    "finalValue": 11.13,
                    "initialCategory": "Month 1",
                    "initialValue": 11.13,
                    "lineColor": "#888888"
                }, {
                    "dashLength": 3,
                    "finalCategory": "Month 3",
                    "finalValue": 15.81,
                    "initialCategory": "Month 2",
                    "initialValue": 15.81,
                    "lineColor": "#888888"
                }],
                "columnWidth": 0.6,
                "categoryField": "name",
                "categoryAxis": {
                    "gridPosition": "start",
                    "axisAlpha": 0,
                    "gridAlpha": 0.1
                },
                "export": {
                    "enabled": true
                }
            });

            //for barchart end
        },
        error: function (result) {
            alert(result.d);
        }
    });
    //alert("hi");
    //for charts end
}

function getoverallreportdata(formData, formData1) {
    PostRequest("ClientReport.aspx/GetCountries", formData, ManageGetCountriesServerResponse, "POST");
    PostRequest("OverallReport.aspx/GetClients", formData, ManageGetClientsServerResponse, "POST");
    PostRequest("OverallReport.aspx/GetSuppliers", formData, ManageGetSuppliersServerResponse, "POST");
    PostRequest("OverallReport.aspx/GetReportForCompletionWD", formData1, ManageGetReportForCompletionServerResponse, "POST");
    PostRequest("OverallReport.aspx/GetReportForRateWD", formData1, ManageGetReportForRateServerResponse, "POST");
    PostRequest("OverallReport.aspx/GetReportForRespondantsWD", formData1, ManageGetReportForRespondantsServerResponse, "POST");
    PostRequest("OverallReport.aspx/ReturnValuesWD", formData1, ManageReturnValuesServerResponse, "POST");



    
}
function ManageGetReportForCompletionServerResponse(data) {
    var tbodytotal = "";
    for (var i = 0; i < data.d[3].length; i++) {
        tbodytotal += '<tr><td>' + data.d[3][i].Total + '</td><td>' + data.d[3][i].Closed + '</td><td>' + data.d[3][i].Inprogress + '</td><td>' + data.d[3][i].Onhold + '</td><td>' + data.d[3][i].Cancelled + '</td></tr>';
    }
    $("#tblprojectstotal tbody").html(tbodytotal);
    //tblSupplierWise
    var tbodysupplier = "";
    for (var i = 0; i < data.d[0].length; i++) {
        tbodysupplier += '<tr><td>' + data.d[0][i].SupplierName + '</td><td>' + data.d[0][i].complete + '</td><td>' + data.d[0][i].percentage + '</td></tr>';
    }
    $("#tblSupplierWise tbody").html(tbodysupplier);
    //tblClientWise
    var tbodyClient = "";
    for (var i = 0; i < data.d[1].length; i++) {
        tbodyClient += '<tr><td>' + data.d[1][i].ClientName + '</td><td>' + data.d[1][i].complete + '</td><td>' + data.d[1][i].percentage + '</td></tr>';
    }
    $("#tblClientWise tbody").html(tbodyClient);
    //tblCountry
    var tbodyCountry = "";
    for (var i = 0; i < data.d[2].length; i++) {
        tbodyCountry += '<tr><td>' + data.d[2][i].Country + '</td><td>' + data.d[2][i].Total + '</td><td>' + data.d[2][i].Percentage + '</td></tr>';
    }
    $("#tblCountry tbody").html(tbodyCountry);
}
function ManageGetReportForRateServerResponse(data) {
    if (data.d == null) {
        return;
    }
    var tbodyIrate = "";
    for (var i = 0; i < data.d[0].length; i++) {
        tbodyIrate += '<tr><td>Incident Rate</td><td>' + data.d[0][i].MinIRate + '</td><td>' + data.d[0][i].MaxIRate + '</td><td>' + data.d[0][i].AvgIRate + '</td></tr><tr><td>Expected LOI</td><td>' + data.d[1][i].MinLOI + '</td><td>' + data.d[1][i].MaxLOI + '</td><td>' + data.d[1][i].AvgLOI + '</td></tr><tr><td>Actual LOI</td><td>' + data.d[3][i].MinLOI + '</td><td>' + data.d[3][i].MaxLOI + '</td><td>' + data.d[3][i].AvgLOI + '</td></tr><tr><td>CPI</td><td>' + data.d[2][i].MinCPI + '</td><td>' + data.d[2][i].MaxCPI + '</td><td>' + data.d[2][i].AvgCPI + '</td></tr>';
    }
    $("#tblIrate tbody").html(tbodyIrate);
    $('#lblValue').empty();
    $('#lblValue').text(data.d[4][0].Summ);
}
function ManageGetReportForRespondantsServerResponse(data) {
    if (data.d == null) {
        return;
    }
    var tbodySuccessRate = "";
    for (var i = 0; i < data.d[0].length; i++) {
        tbodySuccessRate += '<tr><td>' + data.d[0][i].ProjectSuccessRate + '</td><td>' + data.d[0][i].IRSuccessRate + '</td></tr>';
    }
    $("#tblSuccesrate tbody").html(tbodySuccessRate);
    var tbodyRespondants = "";
    $('#lblCompletes').empty();
    for (var i = 0; i < data.d[1].length; i++) {

        tbodyRespondants += '<tr><td>' + data.d[1][i].Total + '</td><td>' + data.d[1][i].Complete + '</td><td>' + data.d[1][i].Incomplete + '</td><td>' + data.d[1][i].Screened + '</td><td>' + data.d[1][i].Quotafull + '</td><td>' + data.d[1][i].Terminate + '</td></tr>';
        $('#lblCompletes').text(data.d[1][i].Complete);
    }
    $("#tblRespondants tbody").html(tbodyRespondants);
}
function ManageReturnValuesServerResponse(data) {
    var tbodytblSupplierwiseRev = "";
    for (var i = 0; i < data.d[0].length; i++) {
        tbodytblSupplierwiseRev += '<tr><td>' + data.d[0][i].SupplierName + '</td><td>' + data.d[0][i].suppcost + '</td><td>' + data.d[0][i].SuppPercent + '</td></tr>';
    }
    $("#tblSupplierwiseRev tbody").html(tbodytblSupplierwiseRev);
    var tbodytblClientRev = "";
    for (var i = 0; i < data.d[1].length; i++) {
        tbodytblClientRev += '<tr><td>' + data.d[1][i].ClientName + '</td><td>' + data.d[1][i].ClientValue + '</td><td>' + data.d[1][i].ClientPercent + '</td></tr>';
    }
    $("#tblClientwiseRev tbody").html(tbodytblClientRev);
    var tbodytblcontRev = "";
    for (var i = 0; i < data.d[2].length; i++) {
        tbodytblcontRev += '<tr><td>' + data.d[2][i].CountryName + '</td><td>' + data.d[2][i].ContValue + '</td><td>' + data.d[2][i].ContPercent + '</td></tr>';
    }
    $("#tblCountryRev tbody").html(tbodytblcontRev);

}
function ManageGetCountriesServerResponse(data) {
    // alert(data.d.length);
    // $("#OperatorMobile").append('<option value="SE">--Select Operator--</option>');
    if (data.d.length > 0) {
        $("#ddlCountry").append('<option value="0">--Search Country--</option>');
        for (var i = 0; i < data.d.length; i++) {
            $("#ddlCountry").append('<option value="' + data.d[i].ID + '">' + data.d[i].Country + '</option>');
        }
        $('#ddlCountry').dropdown();
    }
    else {
    }
}
function ManageGetClientsServerResponse(data) {
    // alert(data.d.length);
    // $("#OperatorMobile").append('<option value="SE">--Select Operator--</option>');
    if (data.d.length > 0) {
        $("#ddlClients").append('<option value="0">--Search Clients--</option>');
        for (var i = 0; i < data.d.length; i++) {
            $("#ddlClients").append('<option value="' + data.d[i].Id + '">' + data.d[i].ClientName + '</option>');
        }
        $('#ddlClients').dropdown();
    }
    else {
    }
}
function ManageGetSuppliersServerResponse(data) {
    if (data.d.length > 0) {
        $("#ddlSuppliers").append('<option value="0">--Search Suppliers--</option>');
        for (var i = 0; i < data.d.length; i++) {
            $("#ddlSuppliers").append('<option value="' + data.d[i].Id + '">' + data.d[i].SupplierName + '</option>');
        }
        $('#ddlSuppliers').dropdown();
    }
    else {
    }
}


function ManageButtonClick() {
    $(document).ajaxStart(jQuery.blockUI({ message: '<h1 style="font-size: 25px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' })).ajaxStop(jQuery.unblockUI);
    loadManagerwisereport('onchange');
    var countryid = $('#ddlCountry').val();
    var clientid = $('#ddlClients').val();
    var supplierid = $('#ddlSuppliers').val();
    var sDate = $('#txtSDate1').val();
    var eDate = $('#txtEDate1').val();
    if (sDate = '') {
        sDate = null;
    }
    if (eDate = '') {
        eDate = null;
    }
    var formData = JSON.stringify({ CountryId: countryid, Clientid: clientid, Supplierid: supplierid, sdate: $('#txtSDate1').val(), edate: $('#txtEDate1').val() });
    //PostRequest("ClientReport.aspx/GetCountriesWD", formData, ManageGetCountriesServerResponse, "POST");
    //PostRequest("OverallReport.aspx/GetClientsWD", formData, ManageGetClientsServerResponse, "POST");
    //PostRequest("OverallReport.aspx/GetSuppliersWD", formData, ManageGetSuppliersServerResponse, "POST");
    PostRequest("OverallReport.aspx/GetReportForCompletionWD", formData, ManageGetReportForCompletionServerResponse, "POST");
    PostRequest("OverallReport.aspx/GetReportForRateWD", formData, ManageGetReportForRateServerResponse, "POST");
    PostRequest("OverallReport.aspx/GetReportForRespondantsWD", formData, ManageGetReportForRespondantsServerResponse, "POST");
    PostRequest("OverallReport.aspx/ReturnValuesWD", formData, ManageReturnValuesServerResponse, "POST");
}


function getallDataNew() {
    var countryid = $('#ddlCountry').val();
    var clientid = $('#ddlClients').val();
    var supplierid = $('#ddlSuppliers').val();
    var sDate = $('#txtSDate1').val();
    var eDate = $('#txtEDate1').val();
    if (sDate = '') {
        sDate = null;
    }
    if (eDate = '') {
        eDate = null;
    }
    var formData = JSON.stringify({ CountryId: countryid, Clientid: clientid, Supplierid: supplierid, sdate: $('#txtSDate1').val(), edate: $('#txtEDate1').val() });
    return formData;
}

