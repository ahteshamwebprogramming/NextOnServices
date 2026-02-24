var tblSurveySpecs;
var tblCountry;
var tblSupplierDetails;
var tblSurveyLinks;
var pair;
var id;
var querystringid;
function redirecttostatus() {
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        pair = vars[i].split("=");
        //alert(pair[1]);
        id = pair[1];
    }
    var url = "ProjectsStatus.aspx?ID=" + id;
    window.location.href = url;
}
function loadSupplierDetails(id) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'ProjectPageDetails.aspx/GetProjectsDetails',
        data: JSON.stringify({ projectid: id }),
        async: false,
        cache: false,
        dataType: "json",
        success: function (response) {
            if (response.d != null) {
                var countrynotesobj = {};
                var datass = response.d[1];
                for (var i = 0; i < datass.length; i++) {
                    if (datass[i].Notes != "" && datass[i].Notes != " " && datass[i].Notes != null) {
                        countrynotesobj["" + datass[i].country + ""] = datass[i].Notes;
                    }
                    else
                        countrynotesobj["" + datass[i].country + ""] = "no notes found";
                }
                ///////////////////////////////////////////////////SUPPLIER DETAILS/////////////////////////////////////////////////////////////////////
                var tbody = "";
                var sdnotes;
                var data = response.d[0];
                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].Country in countrynotesobj && countrynotesobj[data[i].Country] === undefined) {
                            sdnotes = "no notes found";
                        }
                        else {
                            sdnotes = countrynotesobj[data[i].Country];
                        }
                        var trac; tblSurveySpecs
                        if (data[i].TrackingType == 0) {
                            //trac = 'Redirects';
                            trac = '<input type="button" id="btn' + data[i].ID + '" onclick="redirectclick(this.id)" class="btn btn-primary samewidth" data-toggle="modal" data-target="#myModalHorizontal" value="Redirects" />';
                        }
                        else {
                            //trac = 'Pixel Tracking';
                            trac = '<input type="button" id="btn' + data[i].ID + '" onclick="redirectclick(this.id)" class="btn btn-primary samewidth" data-toggle="modal" data-target="#myModalHorizontal" value="Pixel Tracking" />';
                        }
                        var PreScreeningSwitch
                        if (data[i].PreScreening == 1) {
                            PreScreeningSwitch = '<label class="container1_ppd"><input id=PrescreeningChk_' + data[i].ID + ' type="checkbox" checked="checked" onchange="Prescreeningchecked(this.id)" ><span class="checkmark_ppd"></span></label>';
                        }
                        else {
                            PreScreeningSwitch = '<label class="container1_ppd"><input id=PrescreeningChk_' + data[i].ID + ' type="checkbox"  onchange="Prescreeningchecked(this.id)" ><span class="checkmark_ppd"></span></label>';
                        }

                        if (data[i].Block == 0) {
                            tbody += '<tr><td style="display:none">' + data[i].ID + '</td><td class="tooltip1">' + data[i].Country + '<span class="tooltiptext1 toolti_cust">' + sdnotes + '</span></td><td>' + data[i].Name + '</td><td><label class="mw">$ ' + data[i].CPI + '</label></td><td>' + data[i].Quota + '</td><td>' + data[i].Total + '</td><td>' + data[i].Completes + '</td><td>' + data[i].Terminate + '</td><td>' + data[i].Overquota + '</td><td>' + data[i].Security + '</td><td>' + data[i].Fraud + '</td><td>' + data[i].Incomplete + '</td><td>' + data[i].IR + '%</td><td>' + data[i].ActLOI + '</td><td> <label class="container1"><input type="checkbox" id="' + data[i].ID + '" onchange="chkchecked(this.id)" /><span class="checkmark"></span></label></td><td>' + PreScreeningSwitch + '</td><td><input type="button" class="btn btn-success samewidth" data-toggle="modal" data-target="#modal_cpi" onclick="Edit(this.id)" id="' + data[i].ID + '" value="Edit" /></td><td>' + trac + '</td></tr>';
                        }
                        else {
                            tbody += '<tr><td style="display:none">' + data[i].ID + '</td><td class="tooltip1">' + data[i].Country + '<span class="tooltiptext1 toolti_cust">' + sdnotes + '</span></td><td>' + data[i].Name + '</td><td>$ ' + data[i].CPI + '</td><td>' + data[i].Quota + '</td><td>' + data[i].Total + '</td><td>' + data[i].Completes + '</td><td>' + data[i].Terminate + '</td><td>' + data[i].Overquota + '</td><td>' + data[i].Security + '</td><td>' + data[i].Fraud + '</td><td>' + data[i].Incomplete + '</td><td>' + data[i].IR + '%</td><td>' + data[i].ActLOI + '</td><td> <label class="container1"><input type="checkbox" id="' + data[i].ID + '" checked="checked" onchange="chkchecked(this.id)" /><span class="checkmark"></span></label></td><td>' + PreScreeningSwitch + '</td><td><input type="button" class="btn btn-success samewidth" data-toggle="modal" data-target="#modal_cpi" onclick="Edit(this.id)" id="' + data[i].ID + '" value="Edit" /></td><td>' + trac + '</td></tr>';
                        }
                    }
                    if ($.fn.dataTable.isDataTable('#tblSupplierDetails')) {
                        tblSupplierDetails.destroy();
                        $("#tblSupplierDetails tbody").html(tbody);
                        tblSupplierDetails = $("#tblSupplierDetails").DataTable({
                            "iDisplayLength": 50
                        });
                    }
                    else {
                        $("#tblSupplierDetails tbody").html(tbody);
                        tblSupplierDetails = $("#tblSupplierDetails").DataTable({
                            "iDisplayLength": 50
                        });
                    }
                }
                //////////////////////////////////////////////SURVEY SPECIFICATIONS/////////////////////////////////////////////////////////              
                var tbodyss = "";
                var totalresp = 0;
                var flag = 0;
                var data = [];
                if (datass.length > 0) {
                    for (var i = 0; i < datass.length; i++) {
                        if (datass[i].country in countrynotesobj && countrynotesobj[datass[i].country] === undefined) {
                            sdnotes = "no notes found";
                        }
                        else {
                            sdnotes = countrynotesobj[datass[i].country];
                        }
                        // countrynotesobj["" + datass[i].country + ""] = datass[i].Notes;
                        var color = '';
                        data.push(datass[i].Status);
                        var dropdown = '<td><select name="status" id="' + datass[i].id + '"  class="ui dropdown abc" onchange="changestatus(this.id,' + datass[i].Status + ')" ><option value="1" selected="selected">Closed</option><option value="2" >Live</option><option value="3">On Hold</option><option value="4">Canceled</option><option value="5">Awarded</option><option value="6">Invoiced</option></select></td>';
                        if (datass[i].Survey_Quota > datass[i].complete) {
                            color = 'style="color:green"';
                            status = '<td style="color:green">In Process</td>'
                            //dropdown = '<select name="status" id="' + i + '"  class="ui dropdown abc" ><option value="1" selected="selected">Closed</option><option value="2" >Live</option><option value="3">On Hold</option><option value="4">Canceled</option><option value="5">Awarded</option><option value="6">Invoiced</option></select>';
                            flag += 1;
                        }
                        else {
                            color = 'style="color:red"';
                            status = '<td style="color:red">Closed</td>'
                        }
                        totalresp = totalresp + parseInt(datass[i].total);
                        //var irstyle = getcolor(datass[i].IRPercent, datass[i].irate, 'IR')
                        //var irspan = getcolor(datass[i].IRPercent, datass[i].irate, 'ChildIRtooltip');
                        var irstyle = Calculate_Flag(datass[i].irate, datass[i].IRPercent, datass[i].loi, datass[i].ActLOI, 'SuppDetails');
                        var irspan = Calculate_Flag(datass[i].irate, datass[i].IRPercent, datass[i].loi, datass[i].ActLOI, 'ChildIRtooltip');
                        tbodyss += '<tr><td style="display:none">' + datass[i].id + '</td><td ' + color + ' class="tooltip1">' + datass[i].country + '<span class="tooltiptext1 toolti_cust">' + sdnotes + '</span></td><td ' + color + '><label onclick="editQuota(' + datass[i].id + ')" class="handcursor">' + datass[i].Survey_Quota + '</label></td><td ' + color + '><label class="mw">$ ' + datass[i].cpi + '</label></td><td ' + color + '>' + datass[i].complete + '</td><td ' + color + '>' + datass[i].completepercent + '</td>' + dropdown + '<td>' + datass[i].IRPercent + '%</td><td>' + datass[i].ActLOI + '</td><td><span class="fa fa-flag fa-1x tooltip1 totalcountfordisplay" ' + irstyle + '>' + irspan + '</span></td><td><input id="btn' + datass[i].id + '" type="button" class="btn samewidth btn-primary" value="Map IP" onclick="mgrCountryMapping(this.id)"  data-toggle="modal" data-target="#mdlMapCountry" /></td><td><input type="button" class="btn btn-primary samewidth" id="btnQuestionMapping" value="Pre Screening" onclick="ProjectQuestionMapping(' + datass[i].countryid + ',\'' + datass[i].country + '\')"/></td><td></td></tr>';
                    }
                    if ($.fn.dataTable.isDataTable('#tblSurveySpecs')) {
                        tblSurveySpecs.destroy();
                        $("#tblSurveySpecs tbody").html(tbodyss);
                        tblSurveySpecs = $("#tblSurveySpecs").DataTable({
                            "iDisplayLength": 50
                        });
                    }
                    else {
                        $("#tblSurveySpecs tbody").html(tbodyss);
                        tblSurveySpecs = $("#tblSurveySpecs").DataTable({
                            "iDisplayLength": 50
                        });
                    }

                }
                //$('.mapclass').dropdown();
                // $("#tblSurveySpecs tbody").html(tbodyss);
                if (totalresp < 50) {
                    $('.totalcountfordisplay').hide();
                }
                if (data.length > 0) {
                    statusccp(data);
                }
                else {
                    $('#hstatus').text(convertIntToStatus(response.d[3][0].Status, 'intToString'));
                }
                if (datass.length > 0) {
                    for (var i = 0; i < datass.length; i++) {
                        $('#' + datass[i].id).val(datass[i].Status);
                    }
                }
                //if (flag > 0) {
                //    $('#hstatus').text('In Process');
                //}
                //else {
                //    $('#hstatus').text('Closed');
                //}
                ///////////////////////////////////////////////////COUNTRY WISE/////////////////////////////////////////////////////////////////////
                var tbodyCountrywise = "";
                var data = response.d[2];
                var cwnotes = '';
                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].Country in countrynotesobj && countrynotesobj[data[i].Country] === undefined) {
                            cwnotes = "no notes found";
                        }
                        else {
                            cwnotes = countrynotesobj[data[i].Country];
                        }
                        tbodyCountrywise += '<tr><td  class="tooltip1">' + data[i].Country + '<span class="tooltiptext1 toolti_cust">' + cwnotes + '</span></td><td>' + data[i].total + '</td><td>' + data[i].completed + '</td><td>' + data[i].terminate + '</td><td>' + data[i].overquota + '</td><td>' + data[i].securityterm + '</td><td>' + data[i].frauderror + '</td><td>' + data[i].incomplete + '</td><td>' + data[i].Cancelled + '</td></tr>';
                    }
                    if ($.fn.dataTable.isDataTable('#tblCountry')) {
                        tblCountry.destroy();
                        $("#tblCountry tbody").html(tbodyCountrywise);
                        tblCountry = $("#tblCountry").DataTable({
                            "iDisplayLength": 50
                        });
                    }
                    else {
                        $("#tblCountry tbody").html(tbodyCountrywise);
                        tblCountry = $("#tblCountry").DataTable({
                            "iDisplayLength": 50
                        });
                    }
                }
                // $("#tblCountry tbody").html(tbodyCountrywise);
                var securitycheckdata = response.d[3];
                if (securitycheckdata.length > 0) {
                    PopulateSecurityCheck(securitycheckdata);
                }
                //////////////////////////////////////SURVAY LINKS////////////////////////////////////////
                var tbodySurveyLinks = "";
                var data = response.d[4];
                var urlval = "'";
                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].Country in countrynotesobj && countrynotesobj[data[i].Country] === undefined) {
                            cwnotes = "no notes found";
                        }
                        else {
                            cwnotes = countrynotesobj[data[i].Country];
                        }
                        tbodySurveyLinks += '<tr><td  class="tooltip1">' + data[i].Country + '<span class="tooltiptext1 toolti_cust">' + cwnotes + '</span></td><td>' + data[i].SupplierName + '</td><td>' + data[i].ClientSurveyLink + '</td><td>' + data[i].SupplierLink + '</td><td><input type="button" class="btn samewidth btn-primary" value="test" onclick="testyourlink(' + urlval + data[i].SupplierLink + urlval + ');" /></td></tr>';
                    }
                    $("#tblSurveyLinks tbody").html(tbodySurveyLinks);
                    // $("#tblSurveyLinks").DataTable();
                }
            }

        },
        error: function (result) {
            (result.d);
        }
    });
}

function manageblocking(id, opt) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'ProjectPageDetails.aspx/ManageBlocking',
        data: JSON.stringify({ ID: id, OPT: opt }),
        cache: false,
        dataType: "json",
        success: function (data) {
        },
        error: function (result) {
            (result.d);
        }
    });
}
function blockVendor(id) {
    if ($('#' + id).is(':checked')) {
        manageblocking(id, 1);
    }
    else {
        manageblocking(id, 0);
    }

}

function Prescreeningchecked(id) {
    var formdata = {};
    if ($('#' + id).is(':checked')) {
        formdata["Prescreening"] = 1;
    }
    else
        formdata["Prescreening"] = 0;
    id = id.split('_')[1];

    formdata["Id"] = id;
    PostRequest("ProjectPageDetails.aspx/mgrVendorPreScreening", JSON.stringify({ formData: formdata }), ManagemgrVendorPreScreeningServerResponse, "POST");
}
function ManagemgrVendorPreScreeningServerResponse(data) {
    if (data.d.length > 0) {
        if (data.d[0].RetVal == -1) {
            alertt('', 'error', 'error');
        }
        else if (data.d[0].RetVal == 1) {
            alertt('', 'success', 'success');
        }
    }
}

function Edit(id) {
    //var url = "UpdateProjectMapping.aspx?ID=" + id;
    //window.location.href = url;
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'UpdateProjectMapping.aspx/GetProjectsDetailsbyid',
        data: JSON.stringify({ ID: id }),
        cache: false,
        dataType: "json",
        success: function (data) {
            //alert(data.d[0].RespondantQuota);                
            $('#hiddenCPIID').val(id);
            $('#txtQuota').val(data.d[0].RespondantQuota);
            $('#txtCPI').val(data.d[0].CPI);
            $('#txtNOTES').val(data.d[0].Notes);
            $('#lblPname').text(data.d[0].ProjectName);
            $('#lblSupplierName').text(data.d[0].Supplier);
            $('#lblCountryName').text(data.d[0].Country);
            $('#lnlPnumber').text(data.d[0].PNumber);
        },
        error: function (result) {
            (result.statusText);
        }
    });
}
jQuery(document).ready(function () {
    //alert("hi");
    // $('#toggle_nav_btn').click();    
    //var id;
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        pair = vars[i].split("=");
        //alert(pair[1]);
        id = pair[1];
        querystringid = id;
    }
    getIPCount();
    $('#btnEdit').click(function () {
        var url = "UpdateProject.aspx?ID=" + id;
        window.location.href = url;

    });
    var pname = $('#lblSurvayname').text();
    $('#btnEditURL').click(function () {

        var url = "ProjectUrls.aspx?ID=" + id + "&Name=" + pname;
        //  alert(pname);
        window.location.href = url;

    });
    $('#btnProjectMapping').click(function () {
        var url = "ProjectMapping.aspx?Id=" + id;
        window.location.href = url;

    });
    loadSupplierDetails(id);

    trackingtype();

    $('#btnsavetrackingtype').click(function () {
        var attr = new Array();
        var object = {};

        object['ID'] = $('#hiddenID').val();;
        attr.push(object);
        object['Projectid'] = id;
        attr.push(object);
        object['TrackingType'] = $('#ddlTrackingType').val();
        attr.push(object);
        object['Completes'] = $('#txtcomplete').val();
        attr.push(object);
        object['Terminate'] = $('#txtTerminate').val();
        attr.push(object);
        object['Overquota'] = $('#txtOverquota').val();
        attr.push(object);
        object['Security'] = $('#txtSecurity').val();
        attr.push(object);
        object['Fraud'] = $('#txtFraud').val();
        attr.push(object);
        object['SUCCESS'] = $('#txtSuccess').val();
        attr.push(object);
        object['DEFAULT'] = $('#txtDefault').val();
        attr.push(object);
        object['FAILURE'] = $('#txtFailure').val();
        attr.push(object);
        object['QUALITY_TERMINATION'] = $('#ddlQuality').val();
        attr.push(object);
        object['OVER_QUOTA'] = $('#ddlOverquota').val();
        attr.push(object);
        savetrackings(object);
    });

    getfractionComplete();
    LoadCountries();
    MappedCountriesView();
    //if ($('#chkIpAddress').is(':checked')) {
    //    $('#IPCountSpan').show();
    //}
    //else {
    //    $('#IPCountSpan').hide();
    //}
    //$('#btnGenerateURL').click(function () {
    //    var url = "ProjectUrls.aspx?ID=" + id + "&Name=" + pname;
    //    window.location.href = url;

    //});

    //    $.ajax({
    //        type: "POST",
    //        contentType: "application/json; charset=utf-8",
    //        url: 'ProjectPageDetails.aspx/GetProjectDetails',
    //        cache: false,
    //        dataType: "json",
    //        success: function () {
    //            alert(pair[1]);
    //            //var tbody = "";
    //            //for (var i = 0; i < data.d.length; i++) {
    //            //  tbody += '<tr><td>' + data.d[i].PName + '</td><td>' + data.d[i].ClientName + '</td><td>' + data.d[i].PManager + '</td><td>' + data.d[i].LOI + '</td><td>' + data.d[i].SDate + '</td><td>' + data.d[i].EDate + '</td><td><input type="button" value="Edit" onclick=EDIT("' + data.d[i].ID + '") /></td><td><input type="button" value="Url Mapping" onclick=Mapping("' + data.d[i].ID + '","' + data.d[i].PName + '") /></td></tr>';
    //            //$("#myTable1 tbody").append('<tr><td>' + data.d[i].Company + '</td><td>' + data.d[i].Person + '</td><td>' + data.d[i].Number + '</td><td>' + data.d[i].Email + '</td><td>' + data.d[i].Country + '</td></tr>')
    //            // }
    //            $("#myTable1 tbody").html(tbody);
    //            $("#myTable1").DataTable();
    //        },
    //        error: function (result) {
    //            alert(result.d);
    //        }
    //    });

    //    // $('#myTable1').DataTable();     
});

function ProjectQuestionMapping(CID, CName) {
    sessionStorage.setItem("ProjectID", id);
    sessionStorage.setItem("CountryID", CID);
    sessionStorage.setItem("CountryName", CName);
    var url = "ProjectQuestionMapping.aspx";
    window.location.href = url;
}
function savecpichanges() {
    var formdata = getdata('CPI');
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'UpdateProjectMapping.aspx/UpdatepromappingCPI',
        data: JSON.stringify({ ID: formdata.id, quota: formdata.quota, cpi: formdata.cpi, notes: formdata.notes }),
        cache: false,
        dataType: "json",
        success: function (data) {
            if (data.d.length > 0) {
                if (data.d = '1') {
                    //var url = "ProjectMapping.aspx";
                    //window.location.href = url;
                    $('#modal_cpi .close').click();
                    loadSupplierDetails(querystringid);
                }
                else if (data.d = '0') {
                    alert("Error: Please try again");
                }
                else if (data.d = 'error') {
                    alert("Please contact administrator");
                }
            }

        },
        error: function (result) {
            // 
            (result.d);
        }
    });
}
function changestatus(id, valuee) {
    var status = $('#' + id + '').val();
    swal({
        title: 'Are you sure?',
        text: "You want to change the status?",
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#26d06c',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, update it!'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'ProjectPageDetails.aspx/UpdateStatus',
                data: JSON.stringify({ Id: id, Status: status }),
                cache: false,
                dataType: "json",
                success: function (data) {
                    $('#' + id).val(status);
                    $('#hstatus').text(data.d);
                    swal(
                        'Updated!',
                        'Status has been Updated.',
                        'success'
                    )
                },
                error: function (result) {
                    alert(result.d);
                }
            });
        }
        $('#' + id).val(valuee);
    })
}
function statusccp(stat) {
    var len = stat.length;
    if ($.inArray(2, stat) > -1) {
        $('#hstatus').text('Live');
    }
    else if ($.inArray(3, stat) > -1) {
        $('#hstatus').text('On Hold');
    }
    else if ($.inArray(5, stat) > -1) {
        $('#hstatus').text('Awarded');
    }
    else if ($.inArray(1, stat) > -1) {
        $('#hstatus').text('Closed');
    }
    else if ($.inArray(6, stat) > -1) {
        $('#hstatus').text('Invoiced');
    }
    else if ($.inArray(4, stat) > -1) {
        $('#hstatus').text('Cancelled');
    }
    else {
        $('#hstatus').text('NAN');
    }
}
function convertIntToStatus(value, operation) {
    if (operation == 'intToString') {
        if (value == 1)
            return 'Closed';
        else if (value == 2)
            return 'Live';
        else if (value == 3)
            return 'On Hold';
        else if (value == 4)
            return 'Cancelled';
        else if (value == 5)
            return 'Awarded';
        else if (value == 6)
            return 'Invoiced';
    }
}
function trackingtype() {
    $('#Section_Pixel').hide();
    $('#ddlTrackingType').change(function () {
        checkTrackingtype();
    });
}
function checkTrackingtype() {
    if ($('#ddlTrackingType').val() == 0) {
        $('#Section_Redirects').slideDown();
        $('#Section_Pixel').slideUp();
    }
    else if ($('#ddlTrackingType').val() == 1) {
        $('#Section_Redirects').slideUp();
        $('#Section_Pixel').slideDown();
    }
}
function redirectclick(id) {
    $('#ddlTrackingType').val(0);
    checkTrackingtype();
    getredirectdata(id.substring(3))
    //  $('#txtcomplete').val(id.substring(3));
}
async function editcpi() {
    //alert('h');
    const { value: name } = await swal({
        title: 'What is your name?',
        input: 'text',
        inputPlaceholder: 'Enter your name or nickname',
        showCancelButton: true,
        inputValidator: (value) => {
            return !value && 'You need to write something!'
        }
    })
    if (name) {
        swal({ type: 'success', title: 'Hi, ' + name })
    }
}
function getredirectdata(id) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'UpdateProjectMapping.aspx/GetProjectsDetailsbyid',
        data: JSON.stringify({ ID: id }),
        cache: false,
        dataType: "json",
        success: function (data) {
            //alert(data.d[0].RespondantQuota);
            //$('#txtRCQ').val(data.d[0].RespondantQuota);
            //$('#txtCPI').val(data.d[0].CPI);
            //$('#txtNotes').val(data.d[0].Notes);
            //$('#ddlPN').val(data.d[0].ProjectName);
            //$('#ddlsupplier').val(data.d[0].Supplier);
            //$('#ddlCountry').val(data.d[0].Country);
            //$('#lblPnumber').text(data.d[0].PNumber);
            $('#ddlTrackingType').val(data.d[0].TrackingType);
            if ($('#ddlTrackingType').val() == 0) {

                $('#Section_Redirects').slideDown();
                $('#Section_Pixel').slideUp();
            }

            else if ($('#ddlTrackingType').val() == 1) {

                $('#Section_Redirects').slideUp();
                $('#Section_Pixel').slideDown();
            }
            hiddenID
            $('#hiddenID').val(id);
            $('#txtcomplete').val(data.d[0].Completes);
            $('#txtTerminate').val(data.d[0].Terminate);
            $('#txtOverquota').val(data.d[0].Overquota);
            $('#txtSecurity').val(data.d[0].Security);
            $('#txtFraud').val(data.d[0].Fraud);
            $('#txtSuccess').val(data.d[0].SUCCESS);
            $('#txtDefault').val(data.d[0].DEFAULT);
            $('#txtFailure').val(data.d[0].FAILURE);
            $('#ddlQuality').val(data.d[0].QUALITY_TERMINATION);
            $('#ddlOverquota').val(data.d[0].OVER_QUOTA);

            //$('#lblPnumber').val(data.d[0].PNumber);
            // alert(data.d[0].Supplier);

        },
        error: function (result) {
            alert(result.statusText);
        }
    });
}
function savetrackings(formdata) {
    //if (formdata.TrackingType == 0) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'ProjectPageDetails.aspx/UpdateRedirectsInProMap',
        data: JSON.stringify({ formdata }),
        cache: false,
        dataType: "json",
        success: function (data) {
            if (data.d.length > 0) {
                if (data.d = '1') {
                    //var url = "ProjectPageDetails.aspx?ID=" + formdata.Projectid;
                    //window.location.href = url;
                    $('#myModalHorizontal .close').click();
                    loadSupplierDetails(formdata.Projectid);
                }
                else if (data.d = '0') {
                    alert("Error: Please try again");
                }
                else if (data.d = 'error') {
                    alert("Please contact administrator");
                }
            }

        },
        error: function (result) {
            // 
            (result.d);
        }
    });
    //}

}
function getdata(opt) {
    if (opt == 'CPI') {
        var attr = new Array();
        var object = {};
        object['id'] = $('#hiddenCPIID').val();
        attr.push(object);
        object['notes'] = $('#txtNOTES').val();
        attr.push(object);
        object['cpi'] = $('#txtCPI').val();
        attr.push(object);
        object['quota'] = $('#txtQuota').val();
        attr.push(object);
        return object
    }
}
function chkchecked(value) {
    var status
    if ($('#' + value).is(':checked')) {

        status = 'Check';
    }
    else {
        status = 'Uncheck';
    }
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'ProjectMapping.aspx/updatestatus',
        data: JSON.stringify({ id: value, action: status }),
        cache: false,
        dataType: "json",
        success: function (data) {
            //alert(data.d.length);
            if (data.d.length > 0) {

                $('#lblmsg').val(data.d);
                // alert(data.d);
                // alert("Updated succesfuly");

                if (data.d == 'Overquota') {
                    //alert(data.d);
                    success(data.d, 'error')
                    $('#' + value).prop("checked", true);
                }
                else {
                    success(data.d, 'success')
                }
            }
        },
        error: function (result) {
            alert(result.d);
        }
    });
}
function success(message, typee) {
    swal({
        position: 'top-center',
        type: typee,
        title: message,
        showConfirmButton: false,
        timer: 1000
    });
}
function DeviceControl(id) {
    var reqmobile = 0;
    var reqIOS = 0;
    var reqDesktop = 0;
    var reqtablet = 0;
    var reqIP = 0;
    if ($('#chkMobile').is(':checked')) {
        reqmobile = 1;
    }
    else {
        reqmobile = 0;
    }
    if ($('#chkIOSMobile').is(':checked')) {
        reqIOS = 1;
    }
    else {
        reqIOS = 0;
    }
    if ($('#chkDesktop').is(':checked')) {
        reqDesktop = 1
    }
    else {
        reqDesktop = 0;
    }
    if ($('#chkTablet').is(':checked')) {
        reqtablet = 1
    }
    else {
        reqtablet = 0;
    }

    if ($('#chkIpAddress').is(':checked')) {
        reqIP = 1

    }
    else {
        reqIP = 0;
    }
    var req = reqmobile + '' + reqIOS + '' + reqDesktop + '' + reqtablet + '' + reqIP;

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'ProjectPageDetails.aspx/DeviceControl',
        data: JSON.stringify({ ID: querystringid, code: req, OPT: 1 }),
        cache: false,
        dataType: "json",
        success: function (data) {
            if (data.d == 'Succefull') {
                if (id != 'chkIpAddress') {
                    success('Success', 'success')
                }
            }
        },
        error: function (result) {
            (result.d);
        }
    });
    // alert(req);

}
function PopulateSecurityCheck(data1) {
    var code = [];
    data = data1[0].DeviceBlock;
    var code = data.split('');
    if (code[0] == 1) {
        $('#chkMobile').attr('Checked', 'true');
    }
    if (code[1] == 1) {
        $('#chkIOSMobile').attr('Checked', 'true');
    }
    if (code[2] == 1) {
        $('#chkDesktop').attr('Checked', 'true');
    }
    if (code[3] == 1) {
        $('#chkTablet').attr('Checked', 'true');
    }
    if (code[4] == 1) {
        $('#chkIpAddress').attr('Checked', 'true');

        $('#IPCountSpan').show();

    }
    else {
        $('#IPCountSpan').hide();
    }
}
function testyourlink(link) {
    var number = Math.floor(Math.random() * 200000000);
    number = 'test_' + number;
    urlval = link.substring(link.lastIndexOf('=') + 1);
    link = link.replace(urlval, number);
    // alert(link);
    var win = window.open(link, '_blank');
    if (win) {
        //Browser has allowed it to be opened
        win.focus();
        loadSupplierDetails(id);
        getfractionComplete();
    } else {
        //Browser has blocked it
        alert('Please allow popups for this website');
    }
}
function getquerystringid() {
    var pair1;
    var querystringid1;
    var id1;
    var query1 = window.location.search.substring(1);
    var vars1 = query1.split("&");
    for (var i = 0; i < vars1.length; i++) {
        pair1 = vars1[i].split("=");
        //alert(pair[1]);
        id1 = pair1[1];
        querystringid1 = id1;
    }
    return querystringid
}
function getfractionComplete() {
    var id = getquerystringid();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'ProjectPageDetails.aspx/GetFractionComplete',
        data: JSON.stringify({ ProjectId: id }),
        cache: false,
        dataType: "json",
        success: function (response) {
            if (response.d != null) {
                var data = response.d[0];
                var data1 = response.d[1];
                var data2 = response.d[2];
                $('#spncompletes').empty();
                $('#spnTotalcomplete').empty();
                // $('#IRActual').empty();
                $('#spnpercentIR').empty();
                $('#spntooltipir').empty();
                $('#spnLOI').empty();
                $('#spntooltipir').removeClass('color_amber_b color_blue_b');
                $('#spnflag').removeClass('color_red color_blue color_yellow color_white');
                var EXPIR;
                var ACTIR;
                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        $('#spnTotalcomplete').append(data[i].Total);
                        $('#spncompletes').append(data[i].Complete);
                    }
                }
                if (data1.length > 0) {
                    for (var i = 0; i < data1.length; i++) {
                        $('#txtEIR').empty('');
                        EXPIR = data1[i].ExpectedIR;
                        ACTIR = data1[i].IRPercent;
                        $('#txtEIR').append(data1[i].ExpectedIR + '%');
                        // $('#IRActual').append(data1[i].ActualIR);
                        $('#spnpercentIR').append(data1[i].IRPercent);
                        // getcolor(data1[i].IRPercent, data1[i].ExpectedIR, 'IRMain');
                    }
                }
                if (data2.length > 0) {
                    if (data2[0].Total < 50) {
                        $('#spnflag').hide();
                    }
                    for (var i = 0; i < data1.length; i++) {
                        //  $('#txtEIR').empty('');
                        //  $('#txtEIR').append(data2[i].ExpectedIR + '%');
                        // $('#IRActual').append(data2[i].ActualIR);
                        $('#spnLOI').append(data2[i].ActualLOI);
                        Calculate_Flag(EXPIR, ACTIR, data2[i].ExpectedLOI, data2[i].ActualLOI, 'MainFlag');
                    }
                }
            }
        },
        error: function (result) {
            (result.d);
        }
    });
}
function getcolor(value, comvalue, operation) {
    if (operation == 'IR') {
        var style = '';
        if (comvalue > 0) {
            if (value > parseInt(comvalue * 50 / 100) + parseInt(comvalue) || value < parseInt(comvalue) - parseInt(comvalue * 50 / 100)) {
                style = 'style="color:orange"';
            }
            else {
                style = 'style="color:blue"';
            }
        }
        else {
            style = 'style="color:blue"';
        }
        return style;
    }
    if (operation == 'ChildIRtooltip') {
        var IRspan = '<span class="tooltiptext1 toolti_cust"></span>';
        if (comvalue > 0) {
            if (value > parseInt(comvalue * 50 / 100) + parseInt(comvalue)) {
                IRspan = '<span class="tooltiptext1 toolti_cust color_amber_b">IR raised by 25%</span>';
            }
            else if (value < parseInt(comvalue) - parseInt(comvalue * 50 / 100)) {
                IRspan = '<span class="tooltiptext1 toolti_cust color_amber_b">IR dropped by 25%</span>';
            }
            else {
                IRspan = '<span class="tooltiptext1 toolti_cust color_blue_b">IR is under 25% of actual IR</span>';
            }
        }
        else {
            IRspan = '<span class="tooltiptext1 toolti_cust color_blue_b">IR is under 25% of actual IR</span>';
        }
        return IRspan;
    }
}
function Calculate_Flag(ExpectedIR, ActualIR, ExpectedLOI, ActualLOI, operation) {
    var flagIR = 0;
    var flagLOI = 0;
    if ((ExpectedIR - ActualIR) > 0) {
        var drop = ExpectedIR - ActualIR;
        var droppercent = drop * 100 / ExpectedIR;
        if (droppercent > 50) {
            flagIR = 1;
        }
    }
    if ((ActualLOI - ExpectedLOI) > 0) {
        var drop = ActualLOI - ExpectedLOI;
        var droppercent = drop * 100 / ExpectedLOI;
        if (droppercent > 35) {
            flagLOI = 1;
        }
    }
    if (operation == 'SuppDetails') {
        var style;
        if (flagIR > 0 && flagLOI > 0) { style = 'style="color:red"'; }
        else if (flagIR > 0) { style = 'style = "color:blue"'; }
        else if (flagLOI > 0) { style = 'style = "color:yellow"'; }
        else {
            style = 'style="display:none"';
        }
        return style;
    }
    if (operation == 'ChildIRtooltip') {
        var IRspan;
        if (flagIR > 0 && flagLOI > 0) { IRspan = '<span class="tooltiptext1 toolti_cust color_red_b">IR drop by 50%. LOI raised by 35%</span>'; }
        else if (flagIR > 0) { IRspan = '<span class="tooltiptext1 toolti_cust color_blue_b">IR drop by 50%</span>'; }
        else if (flagLOI > 0) { IRspan = '<span class="tooltiptext1 toolti_cust color_yellow_b">LOI raised by 35%</span>'; }
        else { IRspan = ''; }
        return IRspan;
    }
    if (operation == 'MainFlag') {
        var IRspan;
        if (flagIR > 0 && flagLOI > 0) {
            $('#spnflag').addClass('color_red');
            $('#spntooltipir').append('IR drop by 50%.LOI raised by 35%');
        }
        else if (flagIR > 0) {
            $('#spnflag').addClass('color_blue');
            $('#spntooltipir').append('IR drop by 50%');
        }
        else if (flagLOI > 0) {
            $('#spnflag').addClass('color_yellow');
            $('#spntooltipir').append('LOI raised by 35%');
        }
        else {
            $('#spnflag').addClass('color_white1');
            $('#spntooltipir').append('IR and LOI under control');
        }

    }

}
//IP Check Part for Security
$('#chkIpAddress').change(function () {
    if ($('#chkIpAddress').is(':checked')) {
        $('.IP_unchecked').hide();
        $('.IP_checked').show();
    }
    else {
        $('.IP_unchecked').show();
        $('.IP_checked').hide();
    }
});
$('#Ip_control_Cancel').click(function () {
    // alert('h');
    //if ($('#chkIpAddress').is(':checked')) {
    $('#chkIpAddress').prop('checked', true);

    //}
});
$('#Ip_control_Close').click(function () {
    // alert('h');
    //if ($('#chkIpAddress').is(':checked')) {
    $('#chkIpAddress').prop('checked', false);

    //}
});
$('#btnSaveIpCount').click(function () {
    if (validation() == true) {
        DeviceControl('abc');
        var promise = IPCountmgr(querystringid, $('#txtIPCount').val(), '0')
        promise.responseJSON.d[0].Message;
        $('#btnhidemodal').click();
        $('#IPCountSpan').show();
        getIPCount();
    }



});
$('#btnYes').click(function () {
    DeviceControl('abc');
    $('#IPCountSpan').hide();
});
$('#txtIPCount').keydown(function () {
    $('#lblIperror').text('');
    $('#lblIperror').hide();
});
function IPCountmgr(projectid, ipcount, operation) {
    return $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'ProjectPageDetails.aspx/IPSecuritymgr',
        data: JSON.stringify({ ProjectID: projectid, count: ipcount, opt: operation }),
        async: false,
        cache: false,
        dataType: "json",
    });
}
function getIPCount() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'ProjectPageDetails.aspx/DeviceControl',
        data: JSON.stringify({ ID: querystringid, code: '000', OPT: 2 }),
        cache: false,
        dataType: "json",
        success: function (data) {
            if (data.d.length > 0) {
                $('#spnIPCount').empty();
                $('#spnIPCount').append(data.d);
            }
        },
        error: function (result) {
            (result.d);
        }
    });
}
//IP Check Part for Security

//Edit Quota Start

function editQuota(id) {
    $('#txtQuotaCount').empty();
    $('#txtQuotaCount').val('');
    $('#btnShowQuotaModal').click();
    $('#forquotaprojecturl').val(id);
    $('#lblQuotaCWediterror').text('');
    $('#lblQuotaCWediterror').hide();
    $('.UpdateOnlyCWQuota').show();
    $('.UpdateCWandProjectQuota').hide();
}

$('#btnSaveQuotaCW').click(function () {
    var projectidq = getquerystringid();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'ProjectPageDetails.aspx/SaveQuotaCW',
        data: JSON.stringify({ Quota: $('#txtQuotaCount').val(), opt: '1', projecturlid: $('#forquotaprojecturl').val() }),
        cache: false,
        dataType: "json",
        success: function (data) {
            if (data.d.length > 0) {
                var mess = data.d.split(' ')[0];
                if (data.d == 'Successfully Updated') {
                    $('#btnSaveQuotaCWClose').click();
                    $('#txtQuotaCount').empty();
                    $('#txtQuotaCount').val('');
                    success(data.d, 'Success');
                    loadSupplierDetails(projectidq);
                }
                else {
                    if (mess == "Update") {
                        $('#lblQuotaCWediterror').text(data.d);
                        $('#lblQuotaCWediterror').show();
                        $('.UpdateOnlyCWQuota').hide();
                        $('.UpdateCWandProjectQuota').show();
                    }
                    else {
                        $('#lblQuotaCWediterror').text(data.d);
                        $('#lblQuotaCWediterror').show();
                    }
                }
            }
        },
        error: function (result) {
            (result.d);
        }
    });
});
$('#btnSaveQuotaCWandProject').click(function () {
    var projectidq = getquerystringid();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'ProjectPageDetails.aspx/SaveQuotaCWandProject',
        data: JSON.stringify({ Quota: $('#txtQuotaCount').val(), opt: '2', projecturlid: $('#forquotaprojecturl').val() }),
        cache: false,
        dataType: "json",
        success: function (data) {
            if (data.d.length > 0) {
                var mess = data.d.split(' ')[0];
                if (data.d == 'Successfully Updated') {
                    $('#btnSaveQuotaCWClose').click();
                    $('#txtQuotaCount').empty();
                    $('#txtQuotaCount').val('');
                    success(data.d, 'Success');
                    loadSupplierDetails(projectidq);
                }
                else {
                    if (mess == "Update") {
                        $('#UpdateOnlyCWQuota').hide();
                        $('#UpdateCWandProjectQuota').show();
                    }
                    else {
                        $('#lblQuotaCWediterror').text(data.d);
                        $('#lblQuotaCWediterror').show();
                    }
                }
            }
        },
        error: function (result) {
            (result.d);
        }
    });

});
function LoadCountries() {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'ProjectPageDetails.aspx/GetCountries',
        cache: false,
        dataType: "json",
        success: function (data) {

            // alert(data.d.length);
            // $("#OperatorMobile").append('<option value="SE">--Select Operator--</option>');
            if (data.d.length > 0) {
                // $(".mapclass").append('<option value="0">--Search Country--</option>');
                for (var i = 0; i < data.d.length; i++) {
                    $(".mapclass").append('<option value="' + data.d[i].ID + '">' + data.d[i].Country + '</option>');
                }
                $('.mapclass').dropdown();
            }
            else {
            }
        },
        error: function (result) {
            alert(result.d);
        }
    });
}
//Edit Quota End
//Delete Mapped Country
function deletemappedcountry(id) {
    alertmessage(id);
    //var countryid = id.split('_')[0];
    //var projecturlid = id.split('_')[1];
    //alert(countryid + 'and' + projecturlid);
    //if (alertmessage() == 'true') {
    //    // deleteMappedCountry(countryid, projecturlid);
    //    //  MappedCountriesView();
    //    swal('Deleted!', 'Status has been Deleted.', 'success')
    //}
    //else if (alertmessage() == 'cancel') {
    //    swal('Cancel!', 'Cancelled.', 'error')
    //}


    // deleteMappedCountry(countryid, projecturlid);
    //  MappedCountriesView();
}
function mgrCountryMapping(id) {
    id = id.split('btn')[1];
    $('#hdnMapCountry1').val(id);
    $('#mulddlCountries').dropdown('clear');

}
function SaveMappings() {
    var contries = $('#mulddlCountries').val();
    for (var i = 0; i < contries.length; i++) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: 'ProjectPageDetails.aspx/mgrMappingCountries',
            data: JSON.stringify({ ProjectURLID: $('#hdnMapCountry1').val(), CountryID: contries[i], OPT: 0 }),
            cache: false,
            dataType: "json",
            success: function (data) {

                //if (data.d.length > 0) {
                //    $(".mapclass").append('<option value="0">--Search Country--</option>');
                //    for (var i = 0; i < data.d.length; i++) {
                //        $(".mapclass").append('<option value="' + data.d[i].ID + '">' + data.d[i].Country + '</option>');
                //    }
                //    $('.mapclass').dropdown();
                //}
                //else {
                //}
            },
            error: function (result) {
                alert(result.d);
            }
        });
    }
    MappedCountriesView();
    $('#mulddlCountries').val(0);
    $('#btnCloseMappingModal').click();

}

function MappedCountriesView() {
    var i = 0
    $('#tblSurveySpecs').find('tbody').find('tr').each(function () {
        i = i + 1;
        var row = $(this);
        var rowcol = '';
        $(row.find('td:last'))[0].innerHTML = '';
        var id = $(row.find('td:first'))[0].innerHTML;
        GetMappedCountries(id).done(function (data) {
            if (data.d.length > 0) {

                for (var i = 0; i < data.d.length; i++) {
                    rowcol = rowcol + '<label class="B_grnd" style="display: inline-block!important; color: black">' + data.d[i].Country + '<i class="delete icon" id= "' + data.d[i].ID + '_' + id + '" onclick= "deletemappedcountry(this.id)" ></i ></label >';
                }
            }
        })
        $(row.find('td:last')).append(rowcol);


    });

}

function GetMappedCountries(id) {
    return $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'ProjectPageDetails.aspx/mgrMappingCountries',
        data: JSON.stringify({ ProjectURLID: id, CountryID: '0000', OPT: 1 }),
        async: false,
        cache: false,
        dataType: "json",
        //  success: function (data) {            if (data.d.length > 0) {                var rowcol = '';                <label class="B_grnd" style="display: inline-block!important; color: black">                   India                                                        <i class="delete icon" id="" onclick="deletemappedcountry(this.id)"></i>                </label>                $(".mapclass").append('<option value="0">--Search Country--</option>');                for (var i = 0; i < data.d.length; i++) {                    rowcol = rowcol + '<label class="B_grnd" style="display: inline-block!important; color: black">' + data.d[i].Country + '< i class="delete icon" id= "' + data.d[i].ID + '" onclick= "deletemappedcountry(this.id)" ></i ></label >';
        //  $(".mapclass").append('<option value="' + data.d[i].ID + '">' + data.d[i].Country + '</option>');                }                // $('.mapclass').dropdown();            }            else {            }        },      //  error: function (result) {            alert(result.d);        }
    });
}

function deleteMappedCountry(countryid, projecturlid) {
    return $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: 'ProjectPageDetails.aspx/mgrMappingCountries',
        data: JSON.stringify({ ProjectURLID: projecturlid, CountryID: countryid, OPT: 2 }),
        async: false,
        cache: false,
        dataType: "json",
    });
}
//Delete Mapped Country

//alert message
function alertmessage(id) {
    return swal({
        title: 'Are you sure?',
        text: "You want to delete?",
        type: 'warning',
        async: false,
        showCancelButton: true,
        confirmButtonColor: '#26d06c',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.value) {
            var countryid = id.split('_')[0];
            var projecturlid = id.split('_')[1];
            deleteMappedCountry(countryid, projecturlid);
            MappedCountriesView();
            // return result.value;
            swal('Deleted!', 'Status has been Deleted.', 'success');
        }
        else if (result.dismiss) {

            swal('Cancel!', 'Cancelled.', 'error');
        }
    });
}
//alert message


function btnDownloadResponseData() {
    var formData = {};
    formData["Id"] = id;
    PostRequest("ProjectPageDetails.aspx/SurvayResponseData", JSON.stringify({ formData }), ManagesurvayResponseDataServerResponse, "POST");
}
var UIDS = [];
var Questions = [];
var Options = [];
function ManagesurvayResponseDataServerResponse(data) {
    if (data.d != null) {
        JSONToExcelWithDelete(data.d, 'Survey Response', true, ',__type,');
    }
}
