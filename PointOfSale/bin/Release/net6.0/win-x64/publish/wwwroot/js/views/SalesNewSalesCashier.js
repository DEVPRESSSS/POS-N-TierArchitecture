
let TaxValue = 18;
let ProductsForSale = [];
let CurrentUser = null; // Store current user info

$(document).ready(function () {
    // Fetch current user information
    fetch("/Cashier/Sales/GetCurrentUser")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.state) {
                CurrentUser = responseJson.object;
                console.log('Current User:', CurrentUser);
            }
        })
        .catch(error => {
            console.error('Error fetching current user:', error);
        });

    fetch("/Cashier/Sales/ListTypeDocumentSale")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        }).then(responseJson => {

            //borrar los options de cboTipoDocumentoVenta
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboTypeDocumentSale").append(
                        $("<option>").val(item.idTypeDocumentSale).text(item.description)
                    )
                });
            }
        })

    //fetch("/Negocio/Obtener")
    //    .then(response => {
    //        return response.ok ? response.json() : Promise.reject(response);
    //    }).then(responseJson => {
    //        if (responseJson.estado) {

    //            const d = responseJson.objeto;

    //            $("#inputGroupSubTotal").text(`Sub Total - ${d.simboloMoneda}`)
    //            $("#inputGroupIGV").text(`IGV(${d.porcentajeImpuesto}%) - ${d.simboloMoneda}`)
    //            $("#inputGroupTotal").text(`Total - ${d.simboloMoneda}`)

    //            TaxValue = parseFloat(d.porcentajeImpuesto)

    //        } else {
    //            $("#inputGroupIGV").text(`IGV(0}%)`)
    //            TaxValue = 0
    //        }
    //    })

    $("#cboSearchProduct").select2({
        ajax: {
            url: "/Cashier/Sales/GetProducts",
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            delay: 250,
            data: function (params) {
                return {
                    search: params.term
                };
            },
            processResults: function (data) {
                return {
                    results: data.map((item) => (
                        {
                            id: item.idProduct,
                            text: item.description,

                            brand: item.brand,
                            category: item.nameCategory,
                            photoBase64: item.photoBase64,
                            price: parseFloat(item.price),
                            stock: parseInt(item.stock) || parseInt(item.quantity) || parseInt(item.stockQuantity) || 0

                        }
                    ))
                };
            }
        },
        placeholder: 'Search product...',
        minimumInputLength: 1,
        templateResult: formatResults
    });


})

function formatResults(data) {

    if (data.loading)
        return data.text;

    var container = $(
        `<table width="100%">
            <tr>
                <td style="width:60px">
                    <img style="height:60px;width:60px;margin-right:10px" src="data:image/png;base64,${data.photoBase64}"/>
                </td>
                <td>
                    <p style="font-weight: bolder;margin:2px">${data.brand}</p>
                    <p style="margin:2px">${data.text}</p>
                </td>
            </tr>
         </table>`
    );

    return container;
}


$(document).on('select2:open', () => {
    document.querySelector('.select2-search__field').focus();
});

$('#cboSearchProduct').on('select2:select', function (e) {
    var data = e.params.data;

    let product_found = ProductsForSale.filter(prod => prod.idProduct == data.id)
    if (product_found.length > 0) {
        $("#cboSearchProduct").val("").trigger('change');
        toastr.warning("", "The product has already been added");
        return false
    }

    swal({
        title: data.brand,
        text: data.text,
        type: "input",
        showCancelButton: true,
        closeOnConfirm: false,
        inputPlaceholder: "Enter quantity"
    }, function (value) {

        if (value === false) return false;
        if (value === "") {
            toastr.warning("", "You need to enter the amount");
            return false
        }
        // Remove any non-numeric characters and validate
        let cleanValue = value.replace(/[^0-9]/g, '');
        if (cleanValue === "" || cleanValue === "0") {
            toastr.warning("", "You must enter a valid positive integer");
            return false
        }

        let enteredQuantity = parseInt(cleanValue);
        let availableStock = parseInt(data.stock) || 0;
        if (enteredQuantity > availableStock) {
            toastr.warning("", `Insufficient stock. Available quantity: ${availableStock}`);
            return false
        }



        let product = {
            idProduct: data.id,
            brandProduct: data.brand,
            descriptionProduct: data.text,
            categoryProducty: data.category,
            quantity: parseInt(value),
            price: data.price.toString(),
            total: (parseFloat(value) * data.price).toString()
        }

        ProductsForSale.push(product)
        showProducts_Prices();

        $("#cboSearchProduct").val("").trigger('change');
        swal.close();

    });
    // Add event listener to restrict input to numbers only in the SweetAlert input
    setTimeout(() => {
        const swalInput = document.querySelector('.sweet-alert input[type="text"]');
        if (swalInput) {
            swalInput.addEventListener('input', function (e) {
                // Allow only numeric characters
                this.value = this.value.replace(/[^0-9]/g, '');
            });

            swalInput.addEventListener('keypress', function (e) {
                // Prevent non-numeric key presses
                if (!/[0-9]/.test(e.key) && !['Backspace', 'Delete', 'Tab', 'Enter'].includes(e.key)) {
                    e.preventDefault();
                }
            });
        }
    }, 100);

});

function showProducts_Prices() {

    let total = 0;
    let tax = 0;
    let subtotal = 0;
    let percentage = TaxValue / 100;

    $("#tbProduct tbody").html("")

    ProductsForSale.forEach((item) => {

        total = total + parseFloat(item.total);

        $("#tbProduct tbody").append(
            $("<tr>").append(
                $("<td>").append(
                    $("<button>").addClass("btn btn-danger btn-delete btn-sm").append(
                        $("<i>").addClass("mdi mdi-trash-can")
                    ).data("idProduct", item.idProduct)
                ),
                $("<td>").text(item.descriptionProduct),
                $("<td>").text(item.quantity),
                $("<td>").text(item.price),
                $("<td>").text(item.total)
            )
        )

    })

    subtotal = total / (1 + percentage);
    tax = total - subtotal;

    $("#txtSubTotal").val(subtotal.toFixed(2))
    $("#txtTotalTaxes").val(tax.toFixed(2))
    $("#txtTotal").val(total.toFixed(2))

}

$(document).on("click", "button.btn-delete", function () {
    const _idproduct = $(this).data("idProduct")

    ProductsForSale = ProductsForSale.filter(p => p.idProduct != _idproduct)

    showProducts_Prices()
})



$("#btnFinalizeSale").click(function () {

    if (ProductsForSale.length < 1) {
        toastr.warning("", "You must enter products");
        return;
    }

    // Get the amount paid value first
    const amountPaid = parseFloat($("#txtAmountPaid").val()) || 0;
    const totalAmount = parseFloat($("#txtTotal").val()) || 0;

    // Validate if the amount is valid
    if (amountPaid < totalAmount) {
        swal({
            title: "Insufficient Amount",
            text: `Amount paid:  ₱${amountPaid.toFixed(2)}\nTotal required: ₱${totalAmount.toFixed(2)}\nShortage: ₱${(totalAmount - amountPaid).toFixed(2)}`,
            icon: "warning",
            button: "OK"
        });
        return;
    }
    const change = amountPaid - totalAmount;

    const vmDetailSale = ProductsForSale;

    const sale = {
        idTypeDocumentSale: $("#cboTypeDocumentSale").val(),
        customerDocument: $("#txtDocumentClient").val(),
        clientName: $("#txtNameClient").val(),
        subtotal: $("#txtSubTotal").val(),
        totalTaxes: $("#txtTotalTaxes").val(),
        total: $("#txtTotal").val(),
        amountPaid: amountPaid.toString(),
        change: change.toString(),
        paymentType: "Cash",
        detailSales: vmDetailSale
    }


    $("#btnFinalizeSale").closest("div.card-body").LoadingOverlay("show")

    fetch("/Cashier/Sales/RegisterSale", {
        method: "POST",
        headers: { 'Content-Type': 'application/json;charset=utf-8' },
        body: JSON.stringify(sale)
    }).then(response => {
   
        $("#btnFinalizeSale").closest("div.card-body").LoadingOverlay("hide")
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {

        if (responseJson.state) {

            ProductsForSale = [];
            showProducts_Prices();
            $("#txtDocumentClient").val("");
            $("#txtNameClient").val("");
            $("#txtAmountPaid").val("");
            $("#cboTypeDocumentSale").val($("#cboTypeDocumentSale option:first").val());

            //swal("Registered!", `Sale Number : ${responseJson.object.saleNumber}`, "success");

            const saleNumber = responseJson.object.saleNumber;


         

            setTimeout(() => {
                const date = new Date().toLocaleString();

                const receiptWindow = window.open('', '_blank', 'width=400,height=600');

                receiptWindow.document.write(`
            <html>
            <head>
                <title>Receipt</title>
                <style>
                    body { font-family: monospace; padding: 10px; font-size: 14px; }
                    h2 { text-align: center; margin: 5px 0; }
                    .line { border-top: 1px dashed #000; margin: 8px 0; }
                    table { width: 100%; border-collapse: collapse; }
                    td { padding: 4px 0; }
                    .right { text-align: right; }
                    .center { text-align: center; }
                </style>
            </head>
            <body>
                <div class="logo">
                       <img src="/Logo/Logo.png" alt="Logo" style="display:block;margin:auto;width:80px;height:auto;">
                </div>
                <h2>DevpressTech</h2>
                <div class="center">Blk 38-1 lOT 6p Phase 2 Area 1, Kaunlaran Village, Navotas City</div>
                <div class="center">Tel: 0912-345-6789</div>
                <div class="center">Email: devpresstech@business.com </div>
                <div class="line"></div>
                <div>Sale No: ${saleNumber}</div>
                <div>Date: ${date}</div>
                <div>Customer: ${sale.clientName || "Walk-in"}</div>
                <div>Document: ${sale.customerDocument || "-"}</div>
                <div>Cashier: ${CurrentUser ? CurrentUser.name : "N/A"}</div>

                <div class="line"></div>
                <table>
                    ${sale.detailSales.map(i => `
                        <tr>
                            <td>${i.quantity} x ${i.brandProduct}</td>
                            <td class="right">₱${(i.quantity * i.price).toFixed(2)}</td>
                        </tr>
                    `).join('')}
                </table>
                <div class="line"></div>
                <table>
                    <tr><td>Subtotal</td><td class="right">₱${parseFloat(sale.subtotal).toFixed(2)}</td></tr>
                    <tr><td>Tax</td><td class="right">₱${parseFloat(sale.totalTaxes).toFixed(2)}</td></tr>
                    <tr><td><strong>Total</strong></td><td class="right"><strong>₱${parseFloat(sale.total).toFixed(2)}</strong></td></tr>
                    <tr><td>Paid</td><td class="right">₱${parseFloat(sale.amountPaid).toFixed(2)}</td></tr>
                    <tr><td>Change</td><td class="right">₱${parseFloat(sale.change).toFixed(2)}</td></tr>
                    <tr><td>Payment</td><td class="right">${sale.paymentType}</td></tr>
                </table>
                <div class="line"></div>
                <div class="center">Thank you for shopping!</div>
            </body>
            </html>
        `);

                receiptWindow.document.close();
                receiptWindow.print();
            }, 2000);



            swal("Success!", `Sale registered: ${saleNumber}\nReceipt will open shortly`, "success");

          
        } else {
            swal("We're sorry", "The sale could not be registered", "error");
        }
    }).catch((error) => {
        $("#btnFinalizeSale").closest("div.card-body").LoadingOverlay("hide")
    })


})





