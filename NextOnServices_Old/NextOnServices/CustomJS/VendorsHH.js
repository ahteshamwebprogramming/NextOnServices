function PageInitialization() {
    GetSuppliers();
}

function GetSuppliers() {
    var formData = {};
    formData["opt"] = 0;
    PostRequest('VendorsHH.aspx/GetSuppliers', JSON.stringify({ formData }), ManageGetSupplierServerResponse, 'Post');
}


function ManageGetSupplierServerResponse(data) {
    if (data.d != null) {
        var tbody = "";
        $('#Suppliers tbody').empty();
        if (data.d.length > 0) {
            for (var i = 0; i < data.d.length; i++) {
                tbody += '<tr><td>' + data.d[i].Name + '</td> <td>s</td><td>s</td></tr> ';
            }
        }
        //$('#Suppliers tbody').append(tbody);
        if ($.fn.dataTable.isDataTable('#Suppliers')) {
            table.destroy();
            $("#Suppliers tbody").html(tbody);
            table = $('#Suppliers').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'copy', 'csv', 'excel', 'pdf', 'print'
                ],
                "aaSorting": [[0, 'desc']],

                "aLengthMenu": [
                    [10, 20, 50, 100, -1],
                    [10, 20, 50, 100, "All"]
                ],
                "iDisplayLength": 10,
            });
        }
        else {
            $("#Suppliers tbody").html(tbody);
            table = $('#Suppliers').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    'copy', 'csv', 'excel', 'pdf', 'print'
                ],
                "aaSorting": [[0, 'desc']],

                "aLengthMenu": [
                    [10, 20, 50, 100, -1],
                    [10, 20, 50, 100, "All"]
                ],
                "iDisplayLength": 10,
            });
        }

    }
}