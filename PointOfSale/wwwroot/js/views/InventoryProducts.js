let tableData;
let rowSelected;

const BASIC_MODEL = {
    idProduct: 0,
    barCode: "",
    brand: "",
    description: "",
    idCategory: 0,
    quantity: 0,
    price: 0,
    isActive: 1,
    photo: ""
}

$(document).ready(function () {

    // Load categories with error handling
    fetch("/Admin/Inventory/GetCategories")
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(responseJson => {
            if (responseJson.data && responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#cboCategory").append(
                        $("<option>").val(item.idCategory).text(item.description)
                    );
                });
            }
        })
        .catch(error => {
            console.error('Error loading categories:', error);
            // Add default categories for testing
            $("#cboCategory").append([
                $("<option>").val(1).text("Electronics"),
                $("<option>").val(2).text("Clothing"),
                $("<option>").val(3).text("Food & Beverages")
            ]);
        });

    // Initialize DataTable with error handling
    tableData = $("#tbData").DataTable({
        responsive: true,
        "ajax": {
            "url": "/Admin/Inventory/GetProducts",
            "type": "GET",
            "datatype": "json",
            "error": function (xhr, error, code) {
                console.error("DataTable Ajax error:", xhr.responseText);
                // Show user-friendly message
                $("#tbData tbody").html('<tr><td colspan="10" class="text-center text-danger">Error loading products. Please check if the server is running and the endpoint exists.</td></tr>');
            }
        },
        "columns": [
            {
                "data": "idProduct",
                "visible": false,
                "searchable": false
            },
            {
                "data": "photoBase64",
                "render": function (data) {
                    if (data) {
                        return `<img style="height:60px;" src="data:image/png;base64,${data}" class="rounded mx-auto d-block" />`;
                    } else {
                        return '<div style="height:60px;" class="d-flex align-items-center justify-content-center bg-light rounded">No Image</div>';
                    }
                }
            },
            { "data": "barCode" },
            { "data": "brand" },
            { "data": "description" },
            { "data": "nameCategory" },
            { "data": "quantity" },
            { "data": "price" },
            {
                "data": "isActive",
                "render": function (data) {
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
                filename: 'Report Products',
                exportOptions: {
                    columns: [2, 3, 4, 5, 6]
                }
            }, 'pageLength'
        ]
    });
});

const openModal = (model = BASIC_MODEL) => {
    $("#txtId").val(model.idProduct);
    $("#txtBarCode").val(model.barCode);
    $("#txtBrand").val(model.brand);
    $("#txtDescription").val(model.description);
    $("#cboCategory").val(model.idCategory == 0 ? $("#cboCategory option:first").val() : model.idCategory);
    $("#txtQuantity").val(model.quantity);
    $("#txtPrice").val(model.price);
    $("#cboState").val(model.isActive);
    $("#txtPhoto").val("");

    if (model.photoBase64) {
        $("#imgProduct").attr("src", `data:image/png;base64,${model.photoBase64}`);
    } else {
        $("#imgProduct").attr("src", "data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZGRkIi8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtc2l6ZT0iMTgiIHRleHQtYW5jaG9yPSJtaWRkbGUiIGR5PSIuM2VtIj5ObyBJbWFnZTwvdGV4dD48L3N2Zz4=");
    }

    $("#modalData").modal("show");
}

$("#btnNewProduct").on("click", function () {
    openModal();
});

$("#btnSave").on("click", function () {
    const inputs = $("input.input-validate").serializeArray();
    const inputs_without_value = inputs.filter((item) => item.value.trim() == "");

    if (inputs_without_value.length > 0) {
        const msg = `You must complete the field : "${inputs_without_value[0].name}"`;
        if (typeof toastr !== 'undefined') {
            toastr.warning(msg, "");
        } else {
            alert(msg);
        }
        $(`input[name="${inputs_without_value[0].name}"]`).focus();
        return;
    }
    const quantity = parseInt($("#txtQuantity").val());
    if (quantity === 0) {
        const msg = "Quantity must be greater than zero";
        if (typeof toastr !== 'undefined') {
            toastr.warning(msg, "");
        } else {
            alert(msg);
        }
        $("#txtQuantity").focus();
        return;
    }

    const model = structuredClone(BASIC_MODEL);
    model["idProduct"] = parseInt($("#txtId").val());
    model["barCode"] = $("#txtBarCode").val();
    model["brand"] = $("#txtBrand").val();
    model["description"] = $("#txtDescription").val();
    model["idCategory"] = $("#cboCategory").val();
    model["quantity"] = $("#txtQuantity").val();
    model["price"] = $("#txtPrice").val();
    model["isActive"] = $("#cboState").val();

   

    const inputPhoto = document.getElementById('txtPhoto');
    const formData = new FormData();

    if (inputPhoto.files[0]) {
        formData.append('photo', inputPhoto.files[0]);
    }
    formData.append('model', JSON.stringify(model));

    // Show loading overlay if available
    if (typeof $.fn.LoadingOverlay !== 'undefined') {
        $("#modalData").find("div.modal-content").LoadingOverlay("show");
    }

    const endpoint = model.idProduct == 0 ? "/Admin/Inventory/CreateProduct" : "/Admin/Inventory/EditProduct";
    const method = model.idProduct == 0 ? "POST" : "PUT";

    fetch(endpoint, {
        method: method,
        body: formData
    })
        .then(response => {
            if (typeof $.fn.LoadingOverlay !== 'undefined') {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            }
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(responseJson => {
            if (responseJson.state) {
                if (model.idProduct == 0) {
                    tableData.row.add(responseJson.object).draw(false);
                    showSuccess("The product was created");
                } else {
                    tableData.row(rowSelected).data(responseJson.object).draw(false);
                    rowSelected = null;
                    showSuccess("The product was modified");
                }
                $("#modalData").modal("hide");
            } else {
                showError(responseJson.message || "An error occurred");
            }
        })
        .catch((error) => {
            if (typeof $.fn.LoadingOverlay !== 'undefined') {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            }
            console.error('Error:', error);
            showError("Server error. Please check if the endpoint exists and is working.");
        });
});

function showSuccess(message) {
    if (typeof swal !== 'undefined') {
        swal("Successful!", message, "success");
    } else {
        alert("Success: " + message);
    }
}

function showError(message) {
    if (typeof swal !== 'undefined') {
        swal("We're sorry", message, "error");
    } else {
        alert("Error: " + message);
    }
}

$("#tbData tbody").on("click", ".btn-edit", function () {
    if ($(this).closest('tr').hasClass('child')) {
        rowSelected = $(this).closest('tr').prev();
    } else {
        rowSelected = $(this).closest('tr');
    }

    const data = tableData.row(rowSelected).data();
    openModal(data);
});

$("#tbData tbody").on("click", ".btn-delete", function () {
    let row;

    if ($(this).closest('tr').hasClass('child')) {
        row = $(this).closest('tr').prev();
    } else {
        row = $(this).closest('tr');
    }
    const data = tableData.row(row).data();

    const confirmDelete = () => {
        if (typeof $.fn.LoadingOverlay !== 'undefined') {
            $(".showSweetAlert").LoadingOverlay("show");
        }

        fetch(`/Admin/Inventory/DeleteProduct?IdProduct=${data.idProduct}`, {
            method: "DELETE"
        })
            .then(response => {
                if (typeof $.fn.LoadingOverlay !== 'undefined') {
                    $(".showSweetAlert").LoadingOverlay("hide");
                }
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(responseJson => {
                if (responseJson.state) {
                    tableData.row(row).remove().draw();
                    showSuccess("Product was deleted");
                } else {
                    showError(responseJson.message || "Delete failed");
                }
            })
            .catch((error) => {
                if (typeof $.fn.LoadingOverlay !== 'undefined') {
                    $(".showSweetAlert").LoadingOverlay("hide");
                }
                console.error('Error:', error);
                showError("Server error. Please check if the endpoint exists.");
            });
    };

    if (typeof swal !== 'undefined') {
        swal({
            title: "Are you sure?",
            text: `Delete the product "${data.description}"`,
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete",
            cancelButtonText: "No, cancel",
            closeOnConfirm: false,
            closeOnCancel: true
        }, function (respuesta) {
            if (respuesta) {
                confirmDelete();
            }
        });
    } else {
        if (confirm(`Are you sure you want to delete "${data.description}"?`)) {
            confirmDelete();
        }
    }
});


$(document).ready(function () {
    // Description field validation (existing code)
    $('#txtDescription').on('input', function () {
        let value = $(this).val();
        value = value.replace(/\s{2,}/g, ' ');
        value = value.replace(/^\s+/, '');
        $(this).val(value);
    });

    $('#txtDescription').on('paste', function (e) {
        setTimeout(() => {
            let value = $(this).val();
            value = value.replace(/\s{2,}/g, ' ').replace(/^\s+/, '');
            $(this).val(value);
        }, 0);
    });

    $('#txtDescription').on('keypress', function (e) {
        const currentValue = $(this).val();
        const cursorPosition = this.selectionStart;
        if (e.which === 32) {
            if (cursorPosition === 0 || currentValue[cursorPosition - 1] === ' ') {
                e.preventDefault();
                return false;
            }
        }
    });

    // Brand field validation - similar to description but allow &, ., -
    $('#txtBrand').on('input', function () {
        let value = $(this).val();
        value = value.replace(/\s{2,}/g, ' ');
        value = value.replace(/^\s+/, '');
        $(this).val(value);
    });

    $('#txtBrand').on('keypress', function (e) {
        const currentValue = $(this).val();
        const cursorPosition = this.selectionStart;
        if (e.which === 32) {
            if (cursorPosition === 0 || currentValue[cursorPosition - 1] === ' ') {
                e.preventDefault();
                return false;
            }
        }
    });

    // BarCode field validation - only alphanumeric, no spaces
    $('#txtBarCode').on('input', function () {
        let value = $(this).val();
        // Remove any non-alphanumeric characters
        value = value.replace(/[^A-Za-z0-9]/g, '');
        $(this).val(value);
    });

    // Quantity field validation - only positive integers
    $('#txtQuantity').on('input', function () {
        let value = $(this).val();
        // Remove any non-digit characters
        value = value.replace(/[^0-9]/g, '');
        // Ensure it doesn't start with 0 (unless it's just "0")
        if (value.length > 1 && value[0] === '0') {
            value = value.substring(1);
        }
        $(this).val(value);
    });

    // Price field validation - decimal numbers with up to 2 decimal places
    $('#txtPrice').on('input', function () {
        let value = $(this).val();
        // Allow only digits and one decimal point
        value = value.replace(/[^0-9.]/g, '');

        // Ensure only one decimal point
        const parts = value.split('.');
        if (parts.length > 2) {
            value = parts[0] + '.' + parts.slice(1).join('');
        }

        // Limit to 2 decimal places
        if (parts.length === 2 && parts[1].length > 2) {
            value = parts[0] + '.' + parts[1].substring(0, 2);
        }

        $(this).val(value);
    });

    // Prevent leading zeros in price (except for decimals like 0.50)
    $('#txtPrice').on('blur', function () {
        let value = $(this).val();
        if (value && !value.includes('.') && value.length > 1 && value[0] === '0') {
            value = value.replace(/^0+/, '') || '0';
            $(this).val(value);
        }
    });

    // General validation feedback - only show invalid state, no valid checkmarks
    $('.input-validate').on('invalid', function (e) {
        $(this).addClass('is-invalid');
    });

    $('.input-validate').on('input', function (e) {
        if (this.validity.valid) {
            // Remove both invalid and valid classes when input is valid
            $(this).removeClass('is-invalid is-valid');
        } else {
            // Only add invalid class, don't add valid class
            $(this).removeClass('is-valid');
            $(this).addClass('is-invalid');
        }
    });

    // Photo preview
    $('#txtPhoto').on('change', function (e) {
        const file = e.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                $('#imgProduct').attr('src', e.target.result);
            };
            reader.readAsDataURL(file);
        }
    });
});