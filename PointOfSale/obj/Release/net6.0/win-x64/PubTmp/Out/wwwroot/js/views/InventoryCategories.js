let tableData;
let rowSelected;

const BASIC_MODEL = {
    idCategory: 0,
    description:"",
    isActive: 1
}


$(document).ready(function () {


    tableData = $("#tbData").DataTable({
        responsive: true,
        "ajax": {
            "url": "/Admin/Inventory/GetCategories",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "idCategory",
                "visible": false,
                "searchable": false
            },
            { "data": "description" },
            {
                "data": "isActive", render: function (data) {
                    if (data == 1)
                        return '<span class="badge badge-info">Active</span>';
                    else
                        return '<span class="badge badge-danger">Inactive</span>';
                }
            },
            {
                "defaultContent": '<button class="btn btn-primary btn-edit btn-sm mr-2"><i class="mdi mdi-pencil"></i></button>' +
                    '<button class="btn btn-danger btn-delete btn-sm"><i class="mdi mdi-trash-can"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Export Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Report categories',
                exportOptions: {
                    columns: [1,2]
                }
            }, 'pageLength'
        ]
    });
})
$(document).ready(function () {
    // Prevent consecutive spaces during typing
    $('#txtDescription').on('input', function () {
        let value = $(this).val();

        // Remove consecutive spaces
        value = value.replace(/\s{2,}/g, ' ');

        // Remove leading spaces
        value = value.replace(/^\s+/, '');

        $(this).val(value);
    });

    // Prevent pasting consecutive spaces
    $('#txtDescription').on('paste', function (e) {
        setTimeout(() => {
            let value = $(this).val();
            value = value.replace(/\s{2,}/g, ' ').replace(/^\s+/, '');
            $(this).val(value);
        }, 0);
    });

    // Alternative: Prevent space key when inappropriate
    $('#txtDescription').on('keypress', function (e) {
        const currentValue = $(this).val();
        const cursorPosition = this.selectionStart;

        // If space key is pressed
        if (e.which === 32) {
            // Prevent space if:
            // 1. At the beginning of input
            // 2. Previous character is already a space
            if (cursorPosition === 0 || currentValue[cursorPosition - 1] === ' ') {
                e.preventDefault();
                return false;
            }
        }
    });
});


const openModal = (model = BASIC_MODEL) => {
    $("#txtId").val(model.idCategory);
    $("#txtDescription").val(model.description);
    $("#cboState").val(model.isActive);

    $("#modalData").modal("show")

}

$("#btnNewUser").on("click", function () {
    openModal()
})

$("#btnSave").on("click", function () {
    //const inputs = $("input.input-validate").serializeArray();
    //const inputs_without_value = inputs.filter((item) => item.value.trim() == "")

    //if (inputs_without_value.length > 0) {
    //    const msg = `You must complete the field : "${inputs_without_value[0].name}"`;
    //    toastr.warning(msg, "");
    //    $(`input[name="${inputs_without_value[0].name}"]`).focus();
    //    return;
    //}
    const descriptionValue = $("#txtDescription").val().trim();
    $("#txtDescription").val(descriptionValue);

    // Custom validation for consecutive spaces
    const spacePattern = /^[a-zA-Z0-9][a-zA-Z0-9.,\-_]*( [a-zA-Z0-9.,\-_]+)*$/;

    if (!spacePattern.test(descriptionValue)) {
        toastr.warning("Description cannot have consecutive spaces, leading/trailing spaces", "");
        $("#txtDescription").focus();
        return;
    }

    const inputs = $("input.input-validate").serializeArray();
    const inputs_without_value = inputs.filter((item) => item.value.trim() == "")

    if (inputs_without_value.length > 0) {
        const msg = `You must complete the field : "${inputs_without_value[0].name}"`;
        toastr.warning(msg, "");
        $(`input[name="${inputs_without_value[0].name}"]`).focus();
        return;
    }

    const model = structuredClone(BASIC_MODEL);
    model["idCategory"] = parseInt($("#txtId").val());
    model["description"] = $("#txtDescription").val();
    model["isActive"] = $("#cboState").val();


    $("#modalData").find("div.modal-content").LoadingOverlay("show")

    
    if (model.idCategory == 0) {
        fetch("/Admin/Inventory/CreateCategory", {
            method: "POST",
            headers: { 'Content-Type': 'application/json;charset=utf-8' },
            body: JSON.stringify(model)
        }).then(response => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide")
            return response.ok ? response.json() : Promise.reject(response);
        }).then(responseJson => {

            if (responseJson.state) {

                tableData.row.add(responseJson.object).draw(false);
                $("#modalData").modal("hide");
                swal("Successful!", "The category was created", "success");

            } else {
                swal("CategoryName is already taken", "error");
            }
        }).catch((error) => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide")
        })
    } else {

        fetch("/Admin/Inventory/UpdateCategory", {
            method: "PUT",
            headers: { 'Content-Type': 'application/json;charset=utf-8' },
            body: JSON.stringify(model)
        }).then(response => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide")
            return response.ok ? response.json() : Promise.reject(response);
        }).then(responseJson => {
            if (responseJson.state) {

                tableData.row(rowSelected).data(responseJson.object).draw(false);
                rowSelected = null;
                $("#modalData").modal("hide");
                swal("Successful!", "The category was modified", "success");

            } else {
                swal("We're sorry", responseJson.message, "error");
            }
        }).catch((error) => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide")
        })
    }

})

$("#tbData tbody").on("click", ".btn-edit", function () {

    if ($(this).closest('tr').hasClass('child')) {
        rowSelected = $(this).closest('tr').prev();
    } else {
        rowSelected = $(this).closest('tr');
    }

    const data = tableData.row(rowSelected).data();

    openModal(data);
})



$("#tbData tbody").on("click", ".btn-delete", function () {

    let row;

    if ($(this).closest('tr').hasClass('child')) {
        row = $(this).closest('tr').prev();
    } else {
        row = $(this).closest('tr');
    }
    const data = tableData.row(row).data();

    swal({
        title: "¿Are you sure?",
        text: `Delete the category "${data.description}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Yes, delete",
        cancelButtonText: "No, cancel",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {

            if (respuesta) {

                $(".showSweetAlert").LoadingOverlay("show")

                fetch(`/Admin/Inventory/DeleteCategory?idCategory=${data.idCategory}`, {
                    method: "DELETE"
                }).then(response => {
                    $(".showSweetAlert").LoadingOverlay("hide")
                    return response.ok ? response.json() : Promise.reject(response);
                }).then(responseJson => {
                    if (responseJson.state) {

                        tableData.row(row).remove().draw();
                        swal("Successful!", "Category was deleted", "success");

                    } else {
                        swal("We're sorry", responseJson.message, "error");
                    }
                })
                    .catch((error) => {
                        $(".showSweetAlert").LoadingOverlay("hide")
                    })
            }
        });
})