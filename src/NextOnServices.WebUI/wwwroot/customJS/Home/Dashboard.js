/**
 * Initialize Select2 on dashboard filter dropdowns only (PM + Status in #TableFilters).
 * Uses teal styling classes; no search box; dropdown attached to body to avoid clipping.
 */
function initVtDashboardFilterSelects() {
    if (!$.fn.select2) return;
    var $filters = $('#TableFilters select.dashboard-filter');
    $filters.each(function () {
        var $el = $(this);
        if ($el.hasClass('select2-hidden-accessible')) {
            $el.select2('destroy');
        }
        $el.select2({
            width: '100%',
            dropdownCssClass: 'vt-ref-select2-dropdown',
            selectionCssClass: 'vt-ref-select2-selection',
            dropdownParent: $('body')
        });
    });
}

/**
 * Initialize Select2 on dashboard filter dropdowns only (PM + Status in #TableFilters).
 * Uses teal styling classes; no search box; dropdown attached to body to avoid clipping.
 */
function initVtDashboardFilterSelects() {
    if (!$.fn.select2) return;
    var $filters = $('#TableFilters select.dashboard-filter');
    $filters.each(function () {
        var $el = $(this);
        if ($el.hasClass('select2-hidden-accessible')) {
            $el.select2('destroy');
        }
        $el.select2({
            width: '100%',
            dropdownCssClass: 'vt-ref-select2-dropdown',
            selectionCssClass: 'vt-ref-select2-selection',
            dropdownParent: $('body')
        });
    });
}

/**
 * Initialize DataTable on #vtDashboardProjectTable (destroy first if exists).
 * Top row: Buttons (left) + Search (right). Bottom row: Info (left) + Pagination (right).
 */
function initVtDashboardProjectTable() {
    var $tbl = $('#vtDashboardProjectTable');
    if (!$tbl.length || !$.fn.DataTable) return;
    if ($.fn.DataTable.isDataTable($tbl[0])) {
        $tbl.DataTable().destroy();
    }
    var dt = $tbl.DataTable({
        aaSorting: [],
        ordering: true,
        searching: true,
        paging: true,
        info: true,
        pageLength: 50,
        lengthChange: false,
        autoWidth: false,
        scrollX: true,
        scrollCollapse: true,
        responsive: false,
        pagingType: 'simple_numbers',
        dom: '<"ref-dt-top"Bf>t<"ref-dt-bottom"ip>',
        buttons: [
            {
                extend: 'excelHtml5',
                titleAttr: 'Export to Excel',
                filename: 'Data_Export',
                className: 'btn-excel',
                text: '<span class="dt-btn-icon" aria-hidden="true"><svg viewBox="0 0 24 24" width="12" height="12"><path fill="currentColor" d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8zm0 1.5L18.5 8H14zM8.3 17l1.9-3-1.8-3h1.8l1 1.8 1-1.8H14l-1.8 3 1.9 3h-1.8l-1.1-1.9L10 17z"/></svg></span><span>Excel</span>'
            },
            {
                extend: 'csvHtml5',
                titleAttr: 'Export to CSV',
                filename: 'Data_Export',
                className: 'btn-csv',
                text: '<span class="dt-btn-icon" aria-hidden="true"><svg viewBox="0 0 24 24" width="12" height="12"><path fill="currentColor" d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8zm0 1.5L18.5 8H14zM8 16.2c-.9 0-1.6-.7-1.6-1.6V13c0-.9.7-1.6 1.6-1.6h1.6v1.1H8.2c-.3 0-.5.2-.5.5v1.3c0 .3.2.5.5.5h1.4V16.2zm2.5 0v-4.8h2.7v1.1h-1.4v.8H13v1.1h-1.2v.8h1.5v1.1zm3.3 0v-1.1h1.1c.2 0 .3-.1.3-.3 0-.1 0-.2-.1-.2l-1-.8c-.4-.3-.6-.7-.6-1.2 0-.8.6-1.3 1.4-1.3H16v1.1h-.9c-.2 0-.3.1-.3.2s0 .2.1.2l1 .8c.4.3.7.7.7 1.2 0 .8-.6 1.4-1.5 1.4z"/></svg></span><span>CSV</span>'
            }
        ],
        columnDefs: [
            { targets: 0, visible: false, searchable: false },
            { targets: 1, className: 'col-pno', width: '70px' },
            { targets: 2, className: 'col-pname', width: '220px' },
            { targets: 3, className: 'col-client', width: '80px' },
            { targets: 4, className: 'col-pm', width: '90px' },
            { targets: 5, className: 'col-country', width: '120px' },
            { targets: 6, className: 'col-loi', width: '50px' },
            { targets: 7, className: 'col-cpi', width: '56px' },
            { targets: 8, className: 'col-irate', width: '56px' },
            { targets: 9, className: 'col-status', width: '90px' },
            { targets: 10, className: 'col-total', width: '50px' },
            { targets: 11, className: 'col-co', width: '44px' },
            { targets: 12, className: 'col-tr', width: '44px' },
            { targets: 13, className: 'col-oq', width: '44px' },
            { targets: 14, className: 'col-st', width: '44px' },
            { targets: 15, className: 'col-fe', width: '44px' },
            { targets: 16, className: 'col-ic', width: '44px' },
            { targets: 17, className: 'col-ir', width: '44px' },
            { targets: 18, className: 'col-act-loi', width: '50px' },
            { targets: 19, className: 'col-flag', width: '40px' }
        ],
        drawCallback: function () {
            var api = this.api();
            setTimeout(function () { api.columns.adjust(); }, 0);
        }
    });
    dt.columns.adjust().draw(false);
    setTimeout(function () {
        if ($.fn.DataTable.isDataTable($tbl[0])) {
            $tbl.DataTable().columns.adjust().draw(false);
        }
    }, 50);
    $(window).off('resize.dashboardProjectTable').on('resize.dashboardProjectTable', function () {
        if ($.fn.DataTable.isDataTable($tbl[0])) {
            $tbl.DataTable().columns.adjust();
        }
    });
}

function getProjectTablePartialView() {
    var Pmanager = $("#TableFilters").find("[name='Managers']").val();
    var Status = $("#TableFilters").find("[name='Status']").val();
    var flagstat = chckchckbox();
    var inputData = {
        "Pmanager": Pmanager,
        "Status": Status,
        "Flag": flagstat
    };

    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "/VT/Home/GetProjects",
            contentType: 'application/json',
            data: JSON.stringify(inputData),
            success: function (data) {
                $('#div_ProjectTable').html(data);
                initVtDashboardProjectTable();
                resolve();
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function openChangeStatusBox(status, projectId) {
    // Bootstrap 5 native way
    const modalElement = document.getElementById('mdlChangeStatus');
    const modal = new bootstrap.Modal(modalElement);
    modal.show();

    let statusId = status == "Closed" ? 1 : status == "Live" ? 2 : status == "On Hold" ? 3 : status == "Cancelled" ? 4 : status == "Awarded" ? 5 : status == "Invoiced" ? 6 : 0;

    $("#mdlChangeStatus").find("[name='Status']").val(statusId);
    $("#mdlChangeStatus").find("[name='ProjectId']").val(projectId);
}

function UpdateStatus() {
    //BlockUI();
    let status = $("#mdlChangeStatus").find("[name='Status']").val();
    let projectId = $("#mdlChangeStatus").find("[name='ProjectId']").val();

    jQuery.ajax({
        type: "POST",
        url: "/VT/Home/ChangeProjectStatus",
        data: { Status: status, ProjectId: projectId },
        cache: false,
        dataType: "json",
        success: function (data) {
            // Close modal manually
            $('#mdlChangeStatus').hide();
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open');

            // Refresh table
            setTimeout(function () {
                getProjectTablePartialView()
                    .then(() => {
                        console.log("Table refreshed successfully");
                    })
                    .catch((error) => {
                        console.error("Table refresh failed:", error);
                    });
            }, 300);
        },
        error: function (xhr) {
            alert("Error! " + xhr.responseText);
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
function testyourlink(link) {
    debugger
    var number = Math.floor(Math.random() * 200000000);
    number = 'test_' + number;
    urlval = link.substring(link.lastIndexOf('=') + 1);
    link = link.replace(urlval, number);
    // alert(link);
    var win = window.open(link, '_blank');
    if (win) {
        //Browser has allowed it to be opened
        win.focus();
        //loadSupplierDetails(id);
        //getfractionComplete();
    } else {

        alert('Please allow popups for this website');
    }
}



//function testyourlink(link) {
//    var number = Math.floor(Math.random() * 200000000);
//    number = 'test_' + number;
//    urlval = link.substring(link.lastIndexOf('=') + 1);
//    link = link.replace(urlval, number);
//    // alert(link);
//    var win = window.open(link, '_blank');
//    if (win) {
//        //Browser has allowed it to be opened
//        win.focus();
//        loadSupplierDetails(id);
//        getfractionComplete();
//    } else {
//        //Browser has blocked it
//        alert('Please allow popups for this website');
//    }
//}