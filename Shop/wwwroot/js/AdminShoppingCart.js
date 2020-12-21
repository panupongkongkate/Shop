var dataTable;

$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/SearchShoppingCart/GetAll"
        },
        "columns": [
            { "data": "receiptnumber", "width": "15%" },
            { "data": "customerName", "width": "15%" },
            { "data": "customerPhoneNumber", "width": "15%" },
            { "data": "customerEmail", "width": "15%" },
            {
                "data": "receiptnumber",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Admin/SearchShoppingCart/ShowDetail/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                    <i class="fas fa-edit"></i>
                                </a>
                                <a onclick=Delete("/Admin/SearchShoppingCart/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
                                    <i class="fas fa-trash-alt"></i>
                                </a>
                            </div>
                           `;
                }, "width": "40%"
            }
        ]
    });
}

function Delete(url){
    swal({
        title: "Are you sure you want to Delete?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type:"DELETE",
                url: url,
                success:function (data){
                    if (data.success){
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else{
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}