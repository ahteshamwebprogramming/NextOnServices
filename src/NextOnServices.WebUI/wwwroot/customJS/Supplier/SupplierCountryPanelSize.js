function AddSupplierCountryPanelSizePartialView() {
    let inputDTO = {};
    inputDTO.Id = 0;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/VT/Supplier/AddSupplierCountryPanelSizePartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $("#PartialView_AddSupplierCountryPanelSize").html(data);
            $(".select2").select2();
        },
        error: function (result) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
}
function ListSupplierCountryPanelSizePartialView() {
    let inputDTO = {};
    inputDTO.encSupplierId = $("[name='encId']").val();
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/VT/Supplier/ListSupplierCountryPanelSizePartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $("#PartialView_ListSupplierCountryPanelSize").html(data);
        },
        error: function (result) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
}

function AddSupplierCountryPanelSize() {
    let Data = $("#SupplierCountryPanelSizeForm");
    var inputDTO = {};
    inputDTO.Id = $(Data).find("[name='SupplierPanelSize.Id']").val() == "" ? 0 : $(Data).find("[name='SupplierPanelSize.Id']").val();
    inputDTO.CountryId = $(Data).find("[name='SupplierPanelSize.CountryId']").val();
    inputDTO.Psize = $(Data).find("[name='SupplierPanelSize.Psize']").val();
    inputDTO.encSupplierId = $("[name='encId']").val();
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/VT/Supplier/SaveSupplierCountryPanelSize",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            Swal.fire({ title: 'Success!', text: "Panel Size Added", icon: 'success', confirmButtonText: 'OK' }).then((result) => {
                if (result.isConfirmed) {
                    //window.location.href = "/VT/Supplier/ListSupplier";
                    ListSupplierCountryPanelSizePartialView();
                    AddSupplierCountryPanelSizePartialView();
                }
            });
        },
        error: function (error) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
}

function EditSupplierCountryPanelSize(Id) {
    let inputDTO = {};
    inputDTO.Id = Id;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/VT/Supplier/AddSupplierCountryPanelSizePartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $("#PartialView_AddSupplierCountryPanelSize").html(data);
            $(".select2").select2();
        },
        error: function (result) {
            $erroralert("Error!", error.responseText);
            UnblockUI();
        }
    });
}
