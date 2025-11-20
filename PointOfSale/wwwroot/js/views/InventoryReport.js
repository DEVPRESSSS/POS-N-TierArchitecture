$(document).ready(function () {
    const table = $("#tbData").DataTable({
        responsive: true,
        ajax: {
            url: '/Admin/Reports/GetInventoryReport', // Update to your actual controller route
            type: 'GET',
            datatype: 'json',
            dataSrc: function (json) {
                return json.data; // your controller returns { data: [...] }
            },
            error: function (xhr, error, code) {
                console.error("DataTable Ajax error:", xhr.responseText);
                $("#tbData tbody").html('<tr><td colspan="5">Error loading data</td></tr>');
            }
        },
        columns: [
            {
                data: null,
                render: function (data, type, row, meta) {
                    return meta.row + 1; // Auto-increment number
                }
            },
                { data: "productName" },
                { data: "initialStock" },
                { data: "quantitySold" },
                { data: "remainingStock" }
            

        ],
        dom: 'Bfrtip',
        buttons: [
            'csv', 'excel'
        ]
    });

    // Handle Today button
    $("#btnDateToday").click(function () {
        const today = new Date().toISOString().split('T')[0];
        table.ajax.url(`/Admin/Reports/GetInventoryReport?reportDate=${today}`).load();
    });

    // Handle Weekly button (last 7 days)
    $("#btnDateWeekly").click(function () {
        const endDate = new Date().toISOString().split('T')[0];
        const startDate = new Date();
        startDate.setDate(startDate.getDate() - 6);
        const start = startDate.toISOString().split('T')[0];

        table.ajax.url(`/Admin/Reports/GetInventoryReport?reportDate=${endDate}`).load();
        // If you want to pass startDate, you need to modify your controller to accept startDate
    });
});
