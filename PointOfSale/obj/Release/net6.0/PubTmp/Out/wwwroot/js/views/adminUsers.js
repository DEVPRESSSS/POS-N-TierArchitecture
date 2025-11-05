let tableData;
let rowSelected;

const BASIC_MODEL = {
    idUsers: "",
    name: "",
    email: "",
    phone: "",
    password: "",
    idRol: "",
    dateCreated : "",
    isActive: 1,
    photo: ""
}

$(document).ready(function () {
    // Load roles for dropdown
    fetch("/Admin/User/GetRoles")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        }).then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboRol").append(
                        $("<option>").val(item.idRol).text(item.description)
                    )
                });
            }
        })

    // Initialize DataTable
    tableData = $("#tbData").DataTable({
        responsive: true,
        "ajax": {
            "url": "/Admin/User/GetUsers",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "idUsers",
                "visible": false,
                "searchable": false
            },
            { "data": "name" },
            { "data": "email" },
            { "data": "phone" },
            { "data": "nameRol" },
            //{ "data": "dateCreated" },
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
                filename: 'Report Users',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5]
                }
            }, 'pageLength'
        ]
    });

    $('#tbData_filter input').attr('maxlength', 10);
    $('#tbData_filter input').on('input', function () {
        if (this.value.length > 15) {
            this.value = this.value.slice(0, 15);
        }
    });
})

const openModal = (model = BASIC_MODEL) => {
    $("#txtName").val(model.name);
    $("#txtEmail").val(model.email);
    $("#txtPhone").val(model.phone);
    $("#cboRol").val(model.idRol || $("#cboRol option:first").val());
    $("#cboState").val(model.isActive);
    $("#txtPhoto").val("");

    const isUpdate = model.idUsers && model.idUsers !== "";

    if (isUpdate) {
        $("#txtPassWord").closest('.form-group').hide();
        $("#txtPassWord").removeClass('input-validate');
        $("#txtPassWord").val("");
    } else {
        $("#txtPassWord").closest('.form-group').show();
        $("#txtPassWord").addClass('input-validate'); 
        $("#txtPassWord").val(model.password);
    }

    $("#modalData").data("currentModel", model);

    $("#modalData").modal("show")
}

$("#btnNewUser").on("click", function () {
    openModal()
})

$("#btnSave").on("click", function () {
    const inputs = $("input.input-validate").serializeArray();
    const inputs_without_value = inputs.filter((item) => item.value.trim() == "")

    if (inputs_without_value.length > 0) {
        const msg = `You must complete the field : "${inputs_without_value[0].name}"`;
        toastr.warning(msg, "");
        $(`input[name="${inputs_without_value[0].name}"]`).focus();
        return;
    }

    const currentModel = $("#modalData").data("currentModel") || BASIC_MODEL;
    const model = structuredClone(BASIC_MODEL);
    const isUpdate = currentModel.idUsers && currentModel.idUsers !== "";

    model["idUsers"] = currentModel.idUsers || "";
    model["name"] = $("#txtName").val();
    model["email"] = $("#txtEmail").val();
    model["phone"] = $("#txtPhone").val();
    model["idRol"] = $("#cboRol").val();
    //model["dateCreated"] = new Date().toISOString();
    model["isActive"] = $("#cboState").val();

    if (!isUpdate) {
        model["password"] = $("#txtPassWord").val();
    } else {
        model["password"] = "";
    }
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(model["email"])) {
        swal("Error", "Please enter a valid email address", "error");
        $("#txtEmail").focus();
        $("#txtEmail").val("");
        return;
    }

    if (model["phone"].length < 11) {

        swal("Error", "Contact number must be 11 numbers", "error");
        $("#txtPhone").focus();
        $("#txtPhone").val("");

        return;
    }

    const inputPhoto = document.getElementById('txtPhoto');


    const formData = new FormData();
    if (inputPhoto.files[0]) {
        formData.append('photo', inputPhoto.files[0]);
    }
    formData.append('model', JSON.stringify(model));

    $("#modalData").find("div.modal-content").LoadingOverlay("show")

    if (!model.idUsers || model.idUsers === "") {
        fetch("/Admin/User/CreateUser", {
            method: "POST",
            body: formData
        }).then(response => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide")
            return response.ok ? response.json() : Promise.reject(response);
        }).then(responseJson => {
            if (responseJson.state) {
                tableData.row.add(responseJson.object).draw(false);
                $("#modalData").modal("hide");
                swal("Successful!", "The user was created", "success");

            } else {
                swal("We're sorry", responseJson.message, "error");
                ClearAllField();

            }
        }).catch((error) => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide")
            swal("Error", "Name, contact, or email are already taken", "error");
            ClearAllField();

        })
    } else {
        // UPDATE USER
        fetch("/Admin/User/UpdateUser", {
            method: "PUT",
            body: formData
        }).then(response => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide")
            return response.ok ? response.json() : Promise.reject(response);
        }).then(responseJson => {
            if (responseJson.state) {
                tableData.row(rowSelected).data(responseJson.object).draw(false);
                rowSelected = null;
                $("#modalData").modal("hide");
                swal("Successful!", "The user was modified", "success");
            } else {
                swal("We're sorry", responseJson.message, "error");
                $("txtEmail").focus();
                $("txtEmail").val("");


            }
        }).catch((error) => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide")
            swal("Error", "An error occurred while updating the user", "error");
           

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
        title: "Are you sure?",
        text: `Delete the user "${data.name}"`,
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

                fetch(`/Admin/User/DeleteUser?IdUser=${data.idUsers}`, {
                    method: "DELETE"
                }).then(response => {
                    $(".showSweetAlert").LoadingOverlay("hide")
                    return response.ok ? response.json() : Promise.reject(response);
                }).then(responseJson => {
                    if (responseJson.state) {
                        tableData.row(row).remove().draw();
                        swal("Successful!", "User was deleted", "success");
                    } else {
                        swal("We're sorry", responseJson.message, "error");
                    }
                })
                    .catch((error) => {
                        $(".showSweetAlert").LoadingOverlay("hide")
                        console.error('Error:', error);
                        swal("Error", "An error occurred while deleting the user", "error");
                    })
            }
        });
})

$(document).ready(function () {
    $("#txtName").on("input", function () {
        let value = $(this).val();

        value = value.replace(/[^a-zA-Z\s,.]/g, '');

        value = value.replace(/\s+/g, ' ');

        value = value.replace(/^[\s,.]+/, '');

        value = value.replace(/[,.]{2,}/g, function (match) {
            return match.charAt(0);
        });

        $(this).val(value);
    });

    $("#txtName").on("keyup", function () {
        let value = $(this).val();

        value = value.replace(/\s+([,.])/g, '$1');

        value = value.replace(/([,.])([a-zA-Z])/g, '$1 $2');

        $(this).val(value);
    });

    $("#txtPassWord").on("input", function () {
        let value = $(this).val();

        value = value.replace(/\s/g, '');

        $(this).val(value);
    });

    $("#txtPassWord").on("keydown", function (e) {
        if (e.keyCode === 32) {
            e.preventDefault();
            return false;
        }
    });

    $("#txtPhone").on("input", function () {
        let value = $(this).val();

        value = value.replace(/[^0-9]/g, '');

        $(this).val(value);
    });

    $("#txtPhone").on("keydown", function (e) {
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
            (e.keyCode === 65 && e.ctrlKey === true) ||
            (e.keyCode === 67 && e.ctrlKey === true) ||
            (e.keyCode === 86 && e.ctrlKey === true) ||
            (e.keyCode === 88 && e.ctrlKey === true) ||
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            return;
        }
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });

    $("#txtEmail").on("input", function () {
        let value = $(this).val();

        value = value.replace(/\s/g, '');

        value = value.toLowerCase();

        $(this).val(value);
    });

    $("#txtPhoto").on("change", function () {
        const $imgUser = $("#imgUser");

        if (this.files && this.files[0]) {
            const file = this.files[0];

            const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
            if (!allowedTypes.includes(file.type)) {
                alert("Please select a valid image file (JPEG, PNG, GIF)");
                this.value = '';
                $imgUser.attr('src', '');
                return;
            }

            if (file.size > 5 * 1024 * 1024) {
                alert("Image size must be less than 5MB");
                this.value = '';
                $imgUser.attr('src', '');
                return;
            }

            const reader = new FileReader();
            reader.onload = function (e) {
                $imgUser.attr('src', e.target.result);
            };
            reader.readAsDataURL(file);
        } else {
            $imgUser.attr('src', '');
        }
    });
});

function ClearAllField() {
    $("#txtName").val("");
    $("#txtEmail").val("");
    $("#txtPhone").val("");
    $("#txtPassWord").val("");
    $("#txtPhoto").val("");

    $("#cboRol").val($("#cboRol option:first").val());
    $("#cboState").val("true"); 

    $(".text-danger").text("");
    $(".is-invalid").removeClass("is-invalid");
}
