$(document).ready(function () {

});
var myspinner = '<div class="spinner-border spinner-border-lg text-primary" role="status"><span class="visually-hidden"> Loading...</span></div>';
function BlockUI() {
    $.blockUI({ message: myspinner });
}
function UnblockUI() {
    $.unblockUI();
}

function $successalert(title, message) {
    Swal.fire({
        title: title,
        text: message,
        icon: 'success',
        customClass: {
            confirmButton: 'btn btn-success'
        },
        buttonsStyling: false
    });
}
function $erroralert(title, message) {
    Swal.fire({
        title: title,
        text: message,
        icon: 'error',
        customClass: {
            confirmButton: 'btn btn-primary'
        },
        buttonsStyling: false
    });
}