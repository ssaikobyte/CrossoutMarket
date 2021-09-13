
var tables = [];
tables[] = $('.team-table').DataTable({
    dom: '<tr>',
    order: [[7, 'desc']],
    searching: true,
    lengthChange: false,
    paging: false,
    info: false,
    autoWidth: false,
    columnDefs: [
        { width: '15%', targets: [1, 2, 6, 7] },
        { width: '12%', targets: [3, 4, 5] },
        { orderable: false, width: '4%', targets: [0] }
    ]
});

$('.team-table tbody').on('click', 'td.details-control', function () {
    var table = $(this).closest('.team-table');
    var tr = $(this).closest('tr');
    var row = table.row(tr);

    if (row.child.isShown()) {
        row.child.hide();
        tr.removeClass('shown');
        $(this).children('td.details-control > svg.minus').addClass('d-none');
        $(this).children('td.details-control > svg.plus').removeClass('d-none');
    }
    else {
        row.child(format(table, row.data())).show();
        tr.addClass('shown');
        $(this).children('td.details-control > svg.plus').addClass('d-none');
        $(this).children('td.details-control > svg.minus').removeClass('d-none');
    }
});

function format(table, rowData) {
    var matchId = table.data('matchid');
    var div = $('<div/>')
        .addClass('loading')
        .text('Loading...');

    $.ajax({
        url: '/data/match/' + matchId + '/player/' + rowData[8],
        dataType: 'json',
        success: function (json) {
            var html = '<table class="table table-borderless" style="width:50%">' +
                '<thead>' +
                '<tr>' +
                '<th>Weapon</th>' +
                '<th>Damage</th>' +
                '</tr>' +
                '</thead>' +
                '<tbody>';
            json.damageData.forEach(function (e, i) {
                var image = ''
                if (e.imageExists)
                    image = '<img class="mr-1" width="32"height="32" src="/img/items/' + e.itemId + '.png"></img>';
                var name = e.weaponDisplayName
                if (e.itemId !== 0)
                    name = '<a href="/item/' + e.itemId + '">' + e.weaponDisplayName + '</a>';
                html += '<tr><td>' + image + name + '</td><td>' + e.damage + '</td></tr>';
            });
            html += '</tbody>';
            div.html(html);
        }
    });

    return div;
}