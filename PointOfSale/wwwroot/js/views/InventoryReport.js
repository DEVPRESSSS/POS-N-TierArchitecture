$(document).ready(function () {
    const table = $("#tbData").DataTable({
        responsive: true,
        ajax: {
            url: '/Admin/Reports/GetInventoryReport',
            type: 'GET',
            datatype: 'json',
            dataSrc: function (json) {
                return json.data;
            },
            error: function (xhr, error, code) {
                console.error("DataTable Ajax error:", xhr.responseText);
                $("#tbData tbody").html('<tr><td colspan="7">Error loading data</td></tr>');
            }
        },
        columns: [
            { data: null, render: (data, type, row, meta) => meta.row + 1 },
            { data: 'productName' },
            { data: 'saleNumber' },
            {
                data: 'saleDate',
                render: function (data) {
                    if (!data || data === '0001-01-01T00:00:00') return '-';
                    return new Date(data).toLocaleDateString();
                }
            },
            { data: 'quantitySold' },
            { data: 'initialStock' },
            { data: 'remainingStock' }
        ],
        dom: 'Bfrtip',
        buttons: [
            'csv', 'excel'
        ]
    });

    // Handle Today button
    $("#btnDateAll").click(function () {
        table.ajax.url('/Admin/Reports/GetInventoryReport').load();
        console.log('All records loaded');
    });

    $("#btnDateToday").click(function () {
        const today = new Date();
        today.setDate(today.getDate() + 1);
        const tomorrow = today.toISOString().split('T')[0];


        table.ajax.url('/Admin/Reports/GetInventoryReport?reportDate=' + tomorrow).load();
        console.log('Today filter applied: ' + tomorrow);
    });

    $("#btnDateWeekly").click(function () {
        const endDate = new Date();
        endDate.setDate(endDate.getDate() + 1); 

        const startDate = new Date();
        startDate.setDate(startDate.getDate() + 1 - 7);

        const start = startDate.toISOString().split('T')[0];
        const end = endDate.toISOString().split('T')[0];

        table.ajax.url('/Admin/Reports/GetInventoryReport?startDate=' + start + '&endDate=' + end).load();
        console.log('Weekly filter: ' + start + ' to ' + end);
    });


    $('div.dataTables_filter input').attr('maxlength', '20').on('keyup', function () {
        table.search(this.value).draw();
    });

 
});