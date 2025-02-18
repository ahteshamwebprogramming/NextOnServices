function editField(field) {
    var displayElement = document.getElementById(field + "Display");
    var editElement = document.getElementById(field + "Edit");
    displayElement.style.display = 'none';
    editElement.style.display = 'block';
}
function cancelField(field) {
    var displayElement = document.getElementById(field + "Display");
    var editElement = document.getElementById(field + "Edit");
    displayElement.style.display = 'block';
    editElement.style.display = 'none';
}
function saveField(field) {
    var inputValue = document.getElementById(field + "Input").value;
    var displayElement = document.getElementById(field + "Display");
    var editElement = document.getElementById(field + "Edit");

    // Here you can handle the saving logic (e.g., AJAX to update the data on the server)
    // For now, we will just update the display with the new value

    var inputDTO = {};
    inputDTO.encId = $("#hfSupplierId").val();
    inputDTO.Key = field;
    inputDTO.Value = inputValue;

    if (!validateField(field)) {
        return;
    }

    BlockUI();

    $.ajax({
        type: "POST",
        url: "/VT/Supplier/UpdateSupplierDetailsFromProfilePage",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            Swal.fire({ title: 'Success!', text: "Updated Succesfully", icon: 'success', confirmButtonText: 'OK' }).then((result) => {
                if (result.isConfirmed) {
                    displayElement.querySelector('p').innerText = inputValue;

                    displayElement.style.display = 'block';
                    editElement.style.display = 'none';
                }
            });
        },
        error: function (error) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
}

function validateField(field) {
    var inputValue = document.getElementById(field + "Input").value;
    if ($.trim(inputValue) == "") {
        $("#validationError" + field).show();
        return false;
    }
    else {
        $("#validationError" + field).hide();
        return true;
    }
}